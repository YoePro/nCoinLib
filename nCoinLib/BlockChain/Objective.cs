using nCoinLib.Util.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCoinLib.BlockChain
{
    public class Objective
    {
        static Objective _Difficulty1 = new Objective(new byte[] { 0x1d, 0x00, 0xff, 0xff });

        #region Fields
        BigInteger _Target;
        double? _Difficulty;

        #endregion

        #region Constructors

        public Objective(uint compact)
            : this(ToBytes(compact))
        {

        }

        public Objective(byte[] compact)
        {
            if (compact.Length == 4)
            {
                var exp = compact[0];
                var val = new BigInteger(compact.SafeSubarray(1, 3));
                _Target = val.ShiftLeft(8 * (exp - 3));
            }
            else
                throw new FormatException("Invalid number of bytes");
        }

        public Objective(BigInteger target)
        {
            _Target = target;
            _Target = new Objective(this.ToCompact())._Target;
        }

        public Objective(UInt256 target)
        {
            _Target = new BigInteger(target.ToBytes(false));
            _Target = new Objective(this.ToCompact())._Target;
        }

        #endregion

        #region Properties
        public static Objective Difficulty1
        {
            get
            {
                return _Difficulty1;
            }
        }

        public double Difficulty
        {
            get
            {
                if (_Difficulty == null)
                {
                    var qr = Difficulty1._Target.DivideAndRemainder(_Target);
                    var quotient = qr[0];
                    var remainder = qr[1];
                    var decimalPart = BigInteger.Zero;
                    for (int i = 0; i < 12; i++)
                    {
                        var div = (remainder.Multiply(BigInteger.Ten)).Divide(_Target);

                        decimalPart = decimalPart.Multiply(BigInteger.Ten);
                        decimalPart = decimalPart.Add(div);

                        remainder = remainder.Multiply(BigInteger.Ten).Subtract(div.Multiply(_Target));
                    }
                    _Difficulty = double.Parse(quotient.ToString() + "." + decimalPart.ToString(), new NumberFormatInfo()
                    {
                        NegativeSign = "-",
                        NumberDecimalSeparator = "."
                    });
                }
                return _Difficulty.Value;
            }
        }

        #endregion

        #region Operators
        public static implicit operator Objective(uint a)
        {
            return new Objective(a);
        }

        public static implicit operator uint(Objective a)
        {
            var bytes = a._Target.ToByteArray();
            var val = bytes.SafeSubarray(0, Math.Min(bytes.Length, 3));
            Array.Reverse(val);
            var exp = (byte)(bytes.Length);
            var missing = 4 - val.Length;
            if (missing > 0)
                val = val.Concat(new byte[missing]).ToArray();
            if (missing < 0)
                val = val.Take(-missing).ToArray();
            return (uint)val[0] + (uint)(val[1] << 8) + (uint)(val[2] << 16) + (uint)(exp << 24);
        }

        public static bool operator ==(Objective a, Objective b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;
            return a._Target.Equals(b._Target);
        }

        public static bool operator !=(Objective a, Objective b)
        {
            return !(a == b);
        }

        #endregion

        #region Methods
        private static byte[] ToBytes(uint bits)
        {
            return new byte[]
            {
                (byte)(bits >> 24),
                (byte)(bits >> 16),
                (byte)(bits >> 8),
                (byte)(bits)
            };
        }

        #region Overrides   
        public override bool Equals(object obj)
        {
            Objective item = obj as Objective;
            if (item == null)
                return false;
            return _Target.Equals(item._Target);
        }

        public override int GetHashCode()
        {
            return _Target.GetHashCode();
        }

        public override string ToString()
        {
            return ToUInt256().ToString();
        }
        #endregion

        public BigInteger ToBigInteger()
        {
            return _Target;
        }

        public uint ToCompact()
        {
            return (uint)this;
        }

        public UInt256 ToUInt256()
        {
            return ToUInt256(_Target);
        }

        internal static UInt256 ToUInt256(BigInteger input)
        {
            var array = input.ToByteArray();

            var missingZero = 32 - array.Length;
            if (missingZero < 0)
                throw new InvalidOperationException("Awful bug, this should never happen");
            if (missingZero != 0)
            {
                array = new byte[missingZero].Concat(array).ToArray();
            }
            return new UInt256(array, false);
        }

        #endregion
    }
}

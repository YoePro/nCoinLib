using nCoinLib.Interfaces;
using nCoinLib.Util.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nCoinLib.Util.Streams;

namespace nCoinLib.Util.Types
{
    public class UInt160
    {
        public class MutableUInt160 : ICoinSerializable
        {
            UInt160 _Value;
            public UInt160 Value
            {
                get
                {
                    return _Value;
                }
                set
                {
                    _Value = value;
                }
            }
            public MutableUInt160()
            {
                _Value = UInt160.Zero;
            }
            public MutableUInt160(UInt160 value)
            {
                _Value = value;
            }

            public void ReadWrite(CoinStream stream)
            {
                if (stream.Serializing)
                {
                    var b = Value.ToBytes();
                    stream.ReadWrite(ref b);
                }
                else
                {
                    byte[] b = new byte[WIDTH_BYTE];
                    stream.ReadWrite(ref b);
                    _Value = new UInt160(b);
                }
            }

        }

        static readonly UInt160 _Zero = new UInt160();
        public static UInt160 Zero
        {
            get { return _Zero; }
        }

        static readonly UInt160 _One = new UInt160(1);
        public static UInt160 One
        {
            get { return _One; }
        }

        public UInt160()
        {
        }

        public UInt160(UInt160 b)
        {
            pn0 = b.pn0;
            pn1 = b.pn1;
            pn2 = b.pn2;
            pn3 = b.pn3;
            pn4 = b.pn4;
        }

        public static UInt160 Parse(string hex)
        {
            return new UInt160(hex);
        }
        public static bool TryParse(string hex, out UInt160 result)
        {
            if (hex == null)
                throw new ArgumentNullException("hex");
            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hex = hex.Substring(2);
            result = null;
            if (hex.Length != WIDTH_BYTE * 2)
                return false;
            if (!((HexEncoder)Encoders.Hex).IsValid(hex))
                return false;
            result = new UInt160(hex);
            return true;
        }

        private static readonly HexEncoder Encoder = new HexEncoder();
        private const int WIDTH_BYTE = 160 / 8;
        internal readonly UInt32 pn0;
        internal readonly UInt32 pn1;
        internal readonly UInt32 pn2;
        internal readonly UInt32 pn3;
        internal readonly UInt32 pn4;

        public byte GetByte(int index)
        {
            var uintIndex = index / sizeof(uint);
            var byteIndex = index % sizeof(uint);
            UInt32 value;
            switch (uintIndex)
            {
                case 0:
                    value = pn0;
                    break;
                case 1:
                    value = pn1;
                    break;
                case 2:
                    value = pn2;
                    break;
                case 3:
                    value = pn3;
                    break;
                case 4:
                    value = pn4;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
            return (byte)(value >> (byteIndex * 8));
        }

        public override string ToString()
        {
            return Encoder.EncodeData(ToBytes().Reverse().ToArray());
        }

        public UInt160(ulong b)
        {
            pn0 = (uint)b;
            pn1 = (uint)(b >> 32);
            pn2 = 0;
            pn3 = 0;
            pn4 = 0;
        }

        public UInt160(byte[] vch, bool lendian = true)
        {
            if (vch.Length != WIDTH_BYTE)
            {
                throw new FormatException("the byte array should be 160 bytes long");
            }

            if (!lendian)
                vch = vch.Reverse().ToArray();

            pn0 = nConvert.ToUInt32(vch, 4 * 0, true);
            pn1 = nConvert.ToUInt32(vch, 4 * 1, true);
            pn2 = nConvert.ToUInt32(vch, 4 * 2, true);
            pn3 = nConvert.ToUInt32(vch, 4 * 3, true);
            pn4 = nConvert.ToUInt32(vch, 4 * 4, true);

        }

        public UInt160(string str)
        {
            pn0 = 0;
            pn1 = 0;
            pn2 = 0;
            pn3 = 0;
            pn4 = 0;
            str = str.Trim();

            if (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                str = str.Substring(2);

            var bytes = Encoder.DecodeData(str).Reverse().ToArray();
            if (bytes.Length != WIDTH_BYTE)
                throw new FormatException("Invalid hex length");
            pn0 = nConvert.ToUInt32(bytes, 4 * 0, true);
            pn1 = nConvert.ToUInt32(bytes, 4 * 1, true);
            pn2 = nConvert.ToUInt32(bytes, 4 * 2, true);
            pn3 = nConvert.ToUInt32(bytes, 4 * 3, true);
            pn4 = nConvert.ToUInt32(bytes, 4 * 4, true);

        }

        public UInt160(byte[] vch)
            : this(vch, true)
        {
        }

        public override bool Equals(object obj)
        {
            var item = obj as UInt160;
            if (item == null)
                return false;
            bool equals = true;
            equals &= pn0 == item.pn0;
            equals &= pn1 == item.pn1;
            equals &= pn2 == item.pn2;
            equals &= pn3 == item.pn3;
            equals &= pn4 == item.pn4;
            return equals;
        }

        public static bool operator ==(UInt160 a, UInt160 b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;

            bool equals = true;
            equals &= a.pn0 == b.pn0;
            equals &= a.pn1 == b.pn1;
            equals &= a.pn2 == b.pn2;
            equals &= a.pn3 == b.pn3;
            equals &= a.pn4 == b.pn4;
            return equals;
        }

        public static bool operator <(UInt160 a, UInt160 b)
        {
            return Comparison(a, b) < 0;
        }

        public static bool operator >(UInt160 a, UInt160 b)
        {
            return Comparison(a, b) > 0;
        }

        public static bool operator <=(UInt160 a, UInt160 b)
        {
            return Comparison(a, b) <= 0;
        }

        public static bool operator >=(UInt160 a, UInt160 b)
        {
            return Comparison(a, b) >= 0;
        }

        private static int Comparison(UInt160 a, UInt160 b)
        {
            if (a.pn4 < b.pn4)
                return -1;
            if (a.pn4 > b.pn4)
                return 1;
            if (a.pn3 < b.pn3)
                return -1;
            if (a.pn3 > b.pn3)
                return 1;
            if (a.pn2 < b.pn2)
                return -1;
            if (a.pn2 > b.pn2)
                return 1;
            if (a.pn1 < b.pn1)
                return -1;
            if (a.pn1 > b.pn1)
                return 1;
            if (a.pn0 < b.pn0)
                return -1;
            if (a.pn0 > b.pn0)
                return 1;
            return 0;
        }

        public static bool operator !=(UInt160 a, UInt160 b)
        {
            return !(a == b);
        }

        public static bool operator ==(UInt160 a, ulong b)
        {
            return (a == new UInt160(b));
        }

        public static bool operator !=(UInt160 a, ulong b)
        {
            return !(a == new UInt160(b));
        }

        public static implicit operator UInt160(ulong value)
        {
            return new UInt160(value);
        }


        public byte[] ToBytes(bool lendian = true)
        {
            var arr = new byte[WIDTH_BYTE];
            Buffer.BlockCopy(nConvert.ToBytes(pn0, true), 0, arr, 4 * 0, 4);
            Buffer.BlockCopy(nConvert.ToBytes(pn1, true), 0, arr, 4 * 1, 4);
            Buffer.BlockCopy(nConvert.ToBytes(pn2, true), 0, arr, 4 * 2, 4);
            Buffer.BlockCopy(nConvert.ToBytes(pn3, true), 0, arr, 4 * 3, 4);
            Buffer.BlockCopy(nConvert.ToBytes(pn4, true), 0, arr, 4 * 4, 4);
            if (!lendian)
                Array.Reverse(arr);
            return arr;
        }

        public MutableUInt160 AsBitcoinSerializable()
        {
            return new MutableUInt160(this);
        }

        public int GetSerializeSize(int nType = 0, ProtocolVersion protocolVersion = ProtocolVersion.PROTOCOL_VERSION)
        {
            return WIDTH_BYTE;
        }

        public int Size
        {
            get
            {
                return WIDTH_BYTE;
            }
        }

        public ulong GetLow64()
        {
            return pn0 | (ulong)pn1 << 32;
        }

        public uint GetLow32()
        {
            return pn0;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 31 + (int)pn0;
                hash = hash * 31 + (int)pn1;
                hash = hash * 31 + (int)pn2;
                hash = hash * 31 + (int)pn3;
                hash = hash * 31 + (int)pn4;
            }
            return hash;
        }

    }
}

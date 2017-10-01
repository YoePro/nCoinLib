using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCoinLib.Util.Types
{
    public class nConvert
    {
        /// <summary>
        /// Convert to UInt32
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="index"></param>
        /// <param name="littleEndian">True for LittleEndian, False for BigEndian</param>
        /// <returns></returns>
        public static uint ToUInt32(byte[] value, int index, bool littleEndian)
        {
            if (littleEndian)
            {
                return value[index]
                       + ((uint)value[index + 1] << 8)
                       + ((uint)value[index + 2] << 16)
                       + ((uint)value[index + 3] << 24);
            }
            else
            {
                return value[index + 3]
                       + ((uint)value[index + 2] << 8)
                       + ((uint)value[index + 1] << 16)
                       + ((uint)value[index + 0] << 24);
            }
        }

        public static int ToInt32(byte[] value, int index, bool littleEndian)
        {
            return unchecked((int)ToUInt32(value, index, littleEndian));
        }

        public static uint ToUInt32(byte[] value, bool littleEndian)
        {
            return ToUInt32(value, 0, littleEndian);
        }

        internal static ulong ToUInt64(byte[] value, bool littleEndian)
        {
            if (littleEndian)
            {
                return value[0]
                       + ((ulong)value[1] << 8)
                       + ((ulong)value[2] << 16)
                       + ((ulong)value[3] << 24)
                       + ((ulong)value[4] << 32)
                       + ((ulong)value[5] << 40)
                       + ((ulong)value[6] << 48)
                       + ((ulong)value[7] << 56);
            }
            else
            {
                return value[7]
                    + ((ulong)value[6] << 8)
                    + ((ulong)value[5] << 16)
                    + ((ulong)value[4] << 24)
                    + ((ulong)value[3] << 32)
                       + ((ulong)value[2] << 40)
                       + ((ulong)value[1] << 48)
                       + ((ulong)value[0] << 56);
            }
        }

        public static byte[] ToBytes(uint value, bool littleEndian)
        {
            if (littleEndian)
            {
                return new byte[]
                {
                    (byte)value,
                    (byte)(value >> 8),
                    (byte)(value >> 16),
                    (byte)(value >> 24),
                };
            }
            else
            {
                return new byte[]
                {
                    (byte)(value >> 24),
                    (byte)(value >> 16),
                    (byte)(value >> 8),
                    (byte)value,
                };
            }
        }

        public static byte[] ToBytes(ulong value, bool littleEndian)
        {
            if (littleEndian)
            {
                return new byte[]
                {
                    (byte)value,
                    (byte)(value >> 8),
                    (byte)(value >> 16),
                    (byte)(value >> 24),
                    (byte)(value >> 32),
                    (byte)(value >> 40),
                    (byte)(value >> 48),
                    (byte)(value >> 56),
                };
            }
            else
            {
                return new byte[]
                {
                    (byte)(value >> 56),
                    (byte)(value >> 48),
                    (byte)(value >> 40),
                    (byte)(value >> 32),
                    (byte)(value >> 24),
                    (byte)(value >> 16),
                    (byte)(value >> 8),
                    (byte)value,
                };
            }
        }

        internal static Array BigIntegerToBytes(BigInteger b, int numBytes)
        {
            if (b == null)
            {
                return null;
            }
            byte[] bytes = new byte[numBytes];
            byte[] biBytes = b.ToByteArray();
            int start = (biBytes.Length == numBytes + 1) ? 1 : 0;
            int length = Math.Min(biBytes.Length, numBytes);
            Array.Copy(biBytes, start, bytes, numBytes - length, length);
            return bytes;

        }

        public static byte[] BigIntegerToBytes(BigInteger num)
        {
            if (num.Equals(BigInteger.Zero))
                //Positive 0 is represented by a null-length vector
                return new byte[0];

            bool isPositive = true;
            if (num.CompareTo(BigInteger.Zero) < 0)
            {
                isPositive = false;
                num = num.Multiply(BigInteger.ValueOf(-1));
            }
            var array = num.ToByteArray();
            Array.Reverse(array);
            if (!isPositive)
                array[array.Length - 1] |= 0x80;
            return array;
        }

        public static BigInteger BytesToBigInteger(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Length == 0)
                return BigInteger.Zero;
            data = data.ToArray();
            var positive = (data[data.Length - 1] & 0x80) == 0;
            if (!positive)
            {
                data[data.Length - 1] &= unchecked((byte)~0x80);
                Array.Reverse(data);
                return new BigInteger(1, data).Negate();
            }
            return new BigInteger(1, data);
        }

        public static bool ArrayEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
                return true;
            if (a == null)
                return false;
            if (b == null)
                return false;
            return ArrayEqual(a, 0, b, 0, Math.Max(a.Length, b.Length));
        }

        public static bool ArrayEqual(byte[] a, int startA, byte[] b, int startB, int length)
        {
            if (a == null && b == null)
                return true;
            if (a == null)
                return false;
            if (b == null)
                return false;
            var alen = a.Length - startA;
            var blen = b.Length - startB;

            if (alen < length || blen < length)
                return false;

            for (int ai = startA, bi = startB; ai < startA + length; ai++, bi++)
            {
                if (a[ai] != b[bi])
                    return false;
            }
            return true;
        }

    }
}

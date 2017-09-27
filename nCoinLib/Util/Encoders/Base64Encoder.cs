using System;

namespace nCoinLib.Util.Encoders
{
    public class Base64Encoder : DataEncoders
    {
        public override byte[] DecodeData(string encoded)
        {
            return Convert.FromBase64String(encoded);
        }

        public override string EncodeData(byte[] data, int offset, int count)
        {
            return Convert.ToBase64String(data, offset, count);
        }
    }
}

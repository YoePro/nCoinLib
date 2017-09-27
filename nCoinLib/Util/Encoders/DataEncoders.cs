namespace nCoinLib.Util.Encoders
{
    public abstract class DataEncoders
    {

        internal DataEncoder()
        {
        }

        public static bool IsSpace(char c)
        {
            return (c == ' ' || c == '\t' || c == '\n' || c == '\v' || c == '\f' || c == '\r') ? true: false;
        }

        public string EncodeData(byte[] data)
        {
            return EncodeData(data, 0, data.Length);
        }

        public abstract string EncodeData(byte[] data, int offset, int count);
        public abstract byte[] DecodeData(string encoded);
    }

    public static class Encoders
    {
        static readonly ASCIIEncoder _ASCII = new ASCIIEncoder();
        public static DataEncoder ASCII
        {
            get
            {
                return _ASCII;
            }
        }

        static readonly HexEncoder _Hex = new HexEncoder();
        public static DataEncoder Hex
        {
            get
            {
                return _Hex;
            }
        }

        static readonly Base64Encoder _Base64 = new Base64Encoder();
        public static DataEncoder Base64
        {
            get
            {
                return _Base64;
            }
        }

        //static readonly Base58Encoder _Base58 = new Base58Encoder();
        //public static DataEncoder Base58
        //{
        //    get
        //    {
        //        return _Base58;
        //    }
        //}

        //private static readonly Base58CheckEncoder _Base58Check = new Base58CheckEncoder();
        //public static DataEncoder Base58Check
        //{
        //    get
        //    {
        //        return _Base58Check;
        //    }
        //}



        //public static Bech32Encoder Bech32(string hrp)
        //{
        //    return new Bech32Encoder(hrp);
        //}
        //public static Bech32Encoder Bech32(byte[] hrp)
        //{
        //    return new Bech32Encoder(hrp);
        //}
    }
}

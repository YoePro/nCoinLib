using nCoinLib.Interfaces;
using nCoinLib.Util.Streams;

namespace nCoinLib.Util.Types
{
    public class CompactVarInt : ICoinSerializable
    {
        private ulong _Value = 0;
        private int _Size;

        #region Constructors

        public CompactVarInt(int size)
        {
            _Size = size;
        }
        public CompactVarInt(ulong value, int size)
        {
            _Value = value;
            _Size = size;
        }

        #endregion

        #region Methods
        
        public ulong ToLong()
        {
            return _Value;
        }

        #region ICoinSerializable Members

        public void ReadWrite(CoinStream stream)
        {
            if (stream.Serializing)
            {
                ulong n = _Value;
                byte[] tmp = new byte[(_Size * 8 + 6) / 7];
                int len = 0;
                while (true)
                {
                    byte a = (byte)(n & 0x7F);
                    byte b = (byte)(len != 0 ? 0x80 : 0x00);
                    tmp[len] = (byte)(a | b);
                    if (n <= 0x7F)
                        break;
                    n = (n >> 7) - 1;
                    len++;
                }
                do
                {
                    byte b = tmp[len];
                    stream.ReadWrite(ref b);
                } while (len-- != 0);
            }
            else
            {
                ulong n = 0;
                while (true)
                {
                    byte chData = 0;
                    stream.ReadWrite(ref chData);
                    ulong a = (n << 7);
                    byte b = (byte)(chData & 0x7F);
                    n = (a | b);
                    if ((chData & 0x80) != 0)
                        n++;
                    else
                        break;
                }
                _Value = n;
            }
        }

        #endregion
        #endregion

    }
}

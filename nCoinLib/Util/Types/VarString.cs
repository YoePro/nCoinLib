using nCoinLib.Interfaces;
using nCoinLib.Util.Streams;
using System;
using System.Linq;

namespace nCoinLib.Util.Types
{
    public class VarString : ICoinSerializable
    {

        #region Fields
        byte[] _Bytes = new byte[0];

        #endregion

        #region Constructors

        public VarString()
        {

        }

        public VarString(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            _Bytes = bytes;
        }
        #endregion

        #region Properties

        public int Length
        {
            get
            {
                return _Bytes.Length;
            }
        }
        #endregion

        #region Methods

        public byte[] GetString()
        {
            return GetString(false);
        }

        public byte[] GetString(bool @unsafe)
        {
            if (@unsafe)
                return _Bytes;
            return _Bytes.ToArray();
        }

        #region ICoinSerializable Members

        public void ReadWrite(CoinStream stream)
        {
            var len = new VarInt((ulong)_Bytes.Length);
            stream.ReadWrite(ref len);
            if (!stream.Serializing)
            {
                if (len.ToLong() > (uint)stream.MaxArraySize)
                    throw new ArgumentOutOfRangeException("Array size not big");
                _Bytes = new byte[len.ToLong()];
            }
            stream.ReadWrite(ref _Bytes);
        }

        #endregion

        #endregion





    }
}

using nCoinLib.Interfaces;
using nCoinLib.Util.Streams;

// VarInt spec.
// https://en.bitcoin.it/wiki/Protocol_specification#Variable_length_integer

namespace nCoinLib.Util.Types
{
    public class VarInt : ICoinSerializable
    {
        private byte _PrefixByte = 0;
        private ulong _Value = 0;

        #region Constructors
        public VarInt()
            : this(0)
        {

        }
        public VarInt(ulong value)
        {
            SetValue(value);
        }
        #endregion

        #region Methods
        internal void SetValue(ulong value)
        {
            this._Value = value;
            if (_Value < 0xFD)
                _PrefixByte = (byte)(int)_Value;
            else if (_Value <= 0xffff)
                _PrefixByte = 0xFD;
            else if (_Value <= 0xffffffff)
                _PrefixByte = 0xFE;
            else
                _PrefixByte = 0xFF;
        }

        public ulong ToLong()
        {
            return _Value;
        }

        #region IBitcoinSerializable Members

        public void ReadWrite(CoinStream stream)
        {
            stream.ReadWrite(ref _PrefixByte);
            if (_PrefixByte < 0xFD)
            {
                _Value = _PrefixByte;
            }
            else if (_PrefixByte == 0xFD)
            {
                var value = (ushort)_Value;
                stream.ReadWrite(ref value);
                _Value = value;
            }
            else if (_PrefixByte == 0xFE)
            {
                var value = (uint)_Value;
                stream.ReadWrite(ref value);
                _Value = value;
            }
            else
            {
                var value = (ulong)_Value;
                stream.ReadWrite(ref value);
                _Value = value;
            }
        }

        #endregion
        #endregion

    }
}

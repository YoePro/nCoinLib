using nCoinLib.Interfaces;
using nCoinLib.Util.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace nCoinLib.Util.Streams
{

    // TODO - fix code CoinStream
    public enum SerializationType
    {
        Disk,
        Network,
        Hash
    }

    public class CoinStream
    {
        #region Variable declarations
        int _MaxArraySize = 1048576;

        static MethodInfo _ReadWriteTyped;
        private readonly Stream _Inner;
        private readonly bool _Serializing;
        private PerformanceCounter _Counter;
        ProtocolVersion _ProtocolVersion = ProtocolVersion.PROTOCOL_VERSION;
        TransactionOptions _TransactionSupportedOptions = TransactionOptions.All;

        #endregion

        #region Constructors

        static CoinStream()
        {
            _ReadWriteTyped = typeof(CoinStream)
            .GetTypeInfo()
            .DeclaredMethods
            .Where(m => m.Name == "ReadWrite")
            .Where(m => m.IsGenericMethodDefinition)
            .Where(m => m.GetParameters().Length == 1)
            .Where(m => m.GetParameters().Any(p => p.ParameterType.IsByRef && p.ParameterType.HasElementType && !p.ParameterType.GetElementType().IsArray))
            .First();
        }

        public CoinStream(Stream inner, bool serializing)
        {
            _Serializing = serializing;
            _Inner = inner;
        }

        public CoinStream(byte[] bytes)
            : this(new MemoryStream(bytes), false)
        {
        }

        #endregion

        #region Properties

        public int MaxArraySize
        {
            get
            {
                return _MaxArraySize;
            }
            set
            {
                _MaxArraySize = value;
            }
        }

        public Stream Inner { get { return _Inner; } }

        public bool Serializing { get { return _Serializing; } }


        public PerformanceCounter Counter
        {
            get
            {
                if (_Counter == null)
                    _Counter = new PerformanceCounter();
                return _Counter;
            }
        }

        public bool IsBigEndian { get; set; }



        public ProtocolVersion ProtocolVersion
        {
            get
            {
                return _ProtocolVersion;
            }
            set
            {
                _ProtocolVersion = value;
            }
        }


        public TransactionOptions TransactionOptions
        {
            get
            {
                return _TransactionSupportedOptions;
            }
            set
            {
                _TransactionSupportedOptions = value;
            }
        }

        public System.Threading.CancellationToken ReadCancellationToken { get; set; }

        public SerializationType Type { get; set; }


        #endregion

        #region Methods


        public Script ReadWrite(Script data)
        {
            if (Serializing)
            {
                var bytes = data == null ? Script.Empty.ToBytes(true) : data.ToBytes(true);
                ReadWriteAsVarString(ref bytes);
                return data;
            }
            else
            {
                var varString = new VarString();
                varString.ReadWrite(this);
                return Script.FromBytesUnsafe(varString.GetString(true));
            }
        }

        public void ReadWrite(ref Script script)
        {
            if (Serializing)
                ReadWrite(script);
            else
                script = ReadWrite(script);
        }

        public T ReadWrite<T>(T data) where T : ICoinSerializable
        {
            ReadWrite<T>(ref data);
            return data;
        }


        public void ReadWrite(Type type, ref object obj)
        {
            try
            {
                var parameters = new object[] { obj };
                _ReadWriteTyped.MakeGenericMethod(type).Invoke(this, parameters);
                obj = parameters[0];
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public void ReadWrite(ref byte data)
        {
            ReadWriteByte(ref data);
        }
        public byte ReadWrite(byte data)
        {
            ReadWrite(ref data);
            return data;
        }

        public void ReadWrite(ref bool data)
        {
            byte d = data ? (byte)1 : (byte)0;
            ReadWriteByte(ref d);
            data = (d == 0 ? false : true);
        }

        public void ReadWrite(ref byte[] arr)
        {
            ReadWriteBytes(ref arr);
        }
        public void ReadWrite(ref byte[] arr, int offset, int count)
        {
            ReadWriteBytes(ref arr, offset, count);
        }
        public void ReadWrite<T>(ref T[] arr) where T : ICoinSerializable, new()
        {
            ReadWriteArray<T>(ref arr);
        }

        public void ReadWrite<T>(ref List<T> list) where T : ICoinSerializable, new()
        {
            ReadWriteList<List<T>, T>(ref list);
        }

        public void ReadWrite<T>(ref T data) where T : ICoinSerializable
        {
            var obj = data;
            if (obj == null)
                obj = Activator.CreateInstance<T>();
            obj.ReadWrite(this);
            if (!Serializing)
                data = obj;
        }

        public void ReadWrite<TList, TItem>(ref TList list)
            where TList : List<TItem>, new()
            where TItem : ICoinSerializable, new()
        {
            ReadWriteList<TList, TItem>(ref list);
        }

        public void ReadWriteAsVarString(ref byte[] bytes)
        {
            if (Serializing)
            {
                VarString str = new VarString(bytes);
                str.ReadWrite(this);
            }
            else
            {
                VarString str = new VarString();
                str.ReadWrite(this);
                bytes = str.GetString(true);
            }
        }

        public void ReadWriteStruct<T>(ref T data) where T : struct, ICoinSerializable
        {
            data.ReadWrite(this);
        }
        public void ReadWriteStruct<T>(T data) where T : struct, ICoinSerializable
        {
            data.ReadWrite(this);
        }

        private void ReadWriteList<TList, TItem>(ref TList data)
            where TList : List<TItem>, new()
            where TItem : ICoinSerializable, new()
        {
            var dataArray = data == null ? null : data.ToArray();
            if (Serializing && dataArray == null)
            {
                dataArray = new TItem[0];
            }
            ReadWriteArray(ref dataArray);
            if (!Serializing)
            {
                if (data == null)
                    data = new TList();
                else
                    data.Clear();
                data.AddRange(dataArray);
            }
        }

        private void ReadWriteNumber(ref long value, int size)
        {
            ulong uvalue = unchecked((ulong)value);
            ReadWriteNumber(ref uvalue, size);
            value = unchecked((long)uvalue);
        }

        private void ReadWriteNumber(ref ulong value, int size)
        {
            var bytes = new byte[size];

            for (int i = 0; i < size; i++)
            {
                bytes[i] = (byte)(value >> i * 8);
            }
            if (IsBigEndian)
                Array.Reverse(bytes);
            ReadWriteBytes(ref bytes);
            if (IsBigEndian)
                Array.Reverse(bytes);
            ulong valueTemp = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                var v = (ulong)bytes[i];
                valueTemp += v << (i * 8);
            }
            value = valueTemp;
        }

        private void ReadWriteBytes(ref byte[] data, int offset = 0, int count = -1)
        {
            if (data == null) throw new ArgumentNullException("data");

            if (data.Length == 0) return;

            count = count == -1 ? data.Length : count;

            if (count == 0) return;

            if (Serializing)
            {
                Inner.Write(data, offset, count);
                Counter.AddWritten(count);
            }
            else
            {
                var readen = Inner.ReadEx(data, offset, count, ReadCancellationToken);
                if (readen == 0)
                    throw new EndOfStreamException("No more bytes to read");
                Counter.AddReaden(readen);

            }
        }

        private void ReadWriteByte(ref byte data)
        {
            if (Serializing)
            {
                Inner.WriteByte(data);
                Counter.AddWritten(1);
            }
            else
            {
                var readen = Inner.ReadByte();
                if (readen == -1)
                    throw new EndOfStreamException("No more bytes to read");
                data = (byte)readen;
                Counter.AddReaden(1);
            }
        }


        public IDisposable BigEndianScope()
        {
            var old = IsBigEndian;
            return new Scope(() =>
            {
                IsBigEndian = true;
            },
            () =>
            {
                IsBigEndian = old;
            });
        }

        public IDisposable ProtocolVersionScope(ProtocolVersion version)
        {
            var old = ProtocolVersion;
            return new Scope(() =>
            {
                ProtocolVersion = version;
            },
            () =>
            {
                ProtocolVersion = old;
            });
        }

        public void CopyParameters(CoinStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            ProtocolVersion = stream.ProtocolVersion;
            IsBigEndian = stream.IsBigEndian;
            MaxArraySize = stream.MaxArraySize;
            Type = stream.Type;
        }

        public IDisposable SerializationTypeScope(SerializationType value)
        {
            var old = Type;
            return new Scope(() =>
            {
                Type = value;
            }, () =>
            {
                Type = old;
            });
        }

        public void ReadWriteAsVarInt(ref uint val)
        {
            ulong vallong = val;
            ReadWriteAsVarInt(ref vallong);
            if (!Serializing)
                val = (uint)vallong;
        }
        public void ReadWriteAsVarInt(ref ulong val)
        {
            var value = new VarInt(val);
            ReadWrite(ref value);
            if (!Serializing)
                val = value.ToLong();
        }

        public void ReadWriteAsCompactVarInt(ref uint val)
        {
            var value = new CompactVarInt(val, sizeof(uint));
            ReadWrite(ref value);
            if (!Serializing)
                val = (uint)value.ToLong();
        }
        public void ReadWriteAsCompactVarInt(ref ulong val)
        {
            var value = new CompactVarInt(val, sizeof(ulong));
            ReadWrite(ref value);
            if (!Serializing)
                val = value.ToLong();
        }

        #endregion

        VarInt _VarInt = new VarInt(0);

        private void ReadWriteArray<T>(ref T[] data) where T : ICoinSerializable
        {
            if (data == null && Serializing)
                throw new ArgumentNullException("Impossible to serialize a null array");
            _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
            ReadWrite(ref _VarInt);

            if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
            if (!Serializing)
                data = new T[_VarInt.ToLong()];
            for (int i = 0; i < data.Length; i++)
            {
                T obj = data[i];
                ReadWrite(ref obj);
                data[i] = obj;
            }
        }


        private void ReadWriteArray(ref ulong[] data)
        {
            if (data == null && Serializing)
                throw new ArgumentNullException("Impossible to serialize a null array");
            _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
            ReadWrite(ref _VarInt);

            if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
            if (!Serializing)
                data = new ulong[_VarInt.ToLong()];
            for (int i = 0; i < data.Length; i++)
            {
                ulong obj = data[i];
                ReadWrite(ref obj);
                data[i] = obj;
            }
        }


        private void ReadWriteArray(ref ushort[] data)
        {
            if (data == null && Serializing)
                throw new ArgumentNullException("Impossible to serialize a null array");
            _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
            ReadWrite(ref _VarInt);

            if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
            if (!Serializing)
                data = new ushort[_VarInt.ToLong()];
            for (int i = 0; i < data.Length; i++)
            {
                ushort obj = data[i];
                ReadWrite(ref obj);
                data[i] = obj;
            }
        }


        private void ReadWriteArray(ref uint[] data)
        {
            if (data == null && Serializing)
                throw new ArgumentNullException("Impossible to serialize a null array");
            _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
            ReadWrite(ref _VarInt);

            if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
            if (!Serializing)
                data = new uint[_VarInt.ToLong()];
            for (int i = 0; i < data.Length; i++)
            {
                uint obj = data[i];
                ReadWrite(ref obj);
                data[i] = obj;
            }
        }


        private void ReadWriteArray(ref byte[] data)
        {
            if (data == null && Serializing)
                throw new ArgumentNullException("Impossible to serialize a null array");
            _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
            ReadWrite(ref _VarInt);

            if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
            if (!Serializing)
                data = new byte[_VarInt.ToLong()];
            for (int i = 0; i < data.Length; i++)
            {
                byte obj = data[i];
                ReadWrite(ref obj);
                data[i] = obj;
            }
        }


        private void ReadWriteArray(ref long[] data)
        {
            if (data == null && Serializing)
                throw new ArgumentNullException("Impossible to serialize a null array");
            _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
            ReadWrite(ref _VarInt);

            if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
            if (!Serializing)
                data = new long[_VarInt.ToLong()];
            for (int i = 0; i < data.Length; i++)
            {
                long obj = data[i];
                ReadWrite(ref obj);
                data[i] = obj;
            }
        }


        private void ReadWriteArray(ref short[] data)
        {
            if (data == null && Serializing)
                throw new ArgumentNullException("Impossible to serialize a null array");
            _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
            ReadWrite(ref _VarInt);

            if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
            if (!Serializing)
                data = new short[_VarInt.ToLong()];
            for (int i = 0; i < data.Length; i++)
            {
                short obj = data[i];
                ReadWrite(ref obj);
                data[i] = obj;
            }
        }


        private void ReadWriteArray(ref int[] data)
        {
            if (data == null && Serializing)
                throw new ArgumentNullException("Impossible to serialize a null array");
            _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
            ReadWrite(ref _VarInt);

            if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
            if (!Serializing)
                data = new int[_VarInt.ToLong()];
            for (int i = 0; i < data.Length; i++)
            {
                int obj = data[i];
                ReadWrite(ref obj);
                data[i] = obj;
            }
        }



        public void ReadWrite(ref ulong[] data)
        {
            ReadWriteArray(ref data);
        }


        public void ReadWrite(ref ushort[] data)
        {
            ReadWriteArray(ref data);
        }


        public void ReadWrite(ref uint[] data)
        {
            ReadWriteArray(ref data);
        }


        public void ReadWrite(ref long[] data)
        {
            ReadWriteArray(ref data);
        }


        public void ReadWrite(ref short[] data)
        {
            ReadWriteArray(ref data);
        }


        public void ReadWrite(ref int[] data)
        {
            ReadWriteArray(ref data);
        }


        UInt256.MutableUInt256 _MutableUInt256 = new UInt256.MutableUInt256(UInt256.Zero);
        public void ReadWrite(ref UInt256 value)
        {
            value = value ?? UInt256.Zero;
            _MutableUInt256.Value = value;
            this.ReadWrite(ref _MutableUInt256);
            value = _MutableUInt256.Value;
        }

        public void ReadWrite(UInt256 value)
        {
            value = value ?? UInt256.Zero;
            _MutableUInt256.Value = value;
            this.ReadWrite(ref _MutableUInt256);
            value = _MutableUInt256.Value;
        }

        public void ReadWrite(ref List<UInt256> value)
        {
            if (Serializing)
            {
                var list = value == null ? null : value.Select(v => v.AsBitcoinSerializable()).ToList();
                this.ReadWrite(ref list);
            }
            else
            {
                List<UInt256.MutableUInt256> list = null;
                this.ReadWrite(ref list);
                value = list.Select(l => l.Value).ToList();
            }
        }
        UInt160.MutableUInt160 _MutableUInt160 = new UInt160.MutableUInt160(UInt160.Zero);
        public void ReadWrite(ref UInt160 value)
        {
            value = value ?? UInt160.Zero;
            _MutableUInt160.Value = value;
            this.ReadWrite(ref _MutableUInt160);
            value = _MutableUInt160.Value;
        }

        public void ReadWrite(UInt160 value)
        {
            value = value ?? UInt160.Zero;
            _MutableUInt160.Value = value;
            this.ReadWrite(ref _MutableUInt160);
            value = _MutableUInt160.Value;
        }

        public void ReadWrite(ref List<UInt160> value)
        {
            if (Serializing)
            {
                var list = value == null ? null : value.Select(v => v.AsBitcoinSerializable()).ToList();
                this.ReadWrite(ref list);
            }
            else
            {
                List<UInt160.MutableUInt160> list = null;
                this.ReadWrite(ref list);
                value = list.Select(l => l.Value).ToList();
            }
        }


        public void ReadWrite(ref ulong data)
        {
            ulong l = (ulong)data;
            ReadWriteNumber(ref l, sizeof(ulong));
            if (!Serializing)
                data = (ulong)l;
        }

        public ulong ReadWrite(ulong data)
        {
            ReadWrite(ref data);
            return data;
        }


        public void ReadWrite(ref ushort data)
        {
            ulong l = (ulong)data;
            ReadWriteNumber(ref l, sizeof(ushort));
            if (!Serializing)
                data = (ushort)l;
        }

        public ushort ReadWrite(ushort data)
        {
            ReadWrite(ref data);
            return data;
        }


        public void ReadWrite(ref uint data)
        {
            ulong l = (ulong)data;
            ReadWriteNumber(ref l, sizeof(uint));
            if (!Serializing)
                data = (uint)l;
        }

        public uint ReadWrite(uint data)
        {
            ReadWrite(ref data);
            return data;
        }




        public void ReadWrite(ref long data)
        {
            long l = (long)data;
            ReadWriteNumber(ref l, sizeof(long));
            if (!Serializing)
                data = (long)l;
        }

        public long ReadWrite(long data)
        {
            ReadWrite(ref data);
            return data;
        }


        public void ReadWrite(ref short data)
        {
            long l = (long)data;
            ReadWriteNumber(ref l, sizeof(short));
            if (!Serializing)
                data = (short)l;
        }

        public short ReadWrite(short data)
        {
            ReadWrite(ref data);
            return data;
        }


        public void ReadWrite(ref int data)
        {
            long l = (long)data;
            ReadWriteNumber(ref l, sizeof(int));
            if (!Serializing)
                data = (int)l;
        }

        public int ReadWrite(int data)
        {
            ReadWrite(ref data);
            return data;
        }


    }


}

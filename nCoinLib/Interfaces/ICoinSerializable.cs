﻿using nCoinLib.BlockChain.Protocol;
using nCoinLib.Util.Streams;
using System;
using System.IO;

namespace nCoinLib.Interfaces
{
    public interface ICoinSerializable
    {
        void ReadWrite(CoinStream stream);
    }

    public static class CoinSerializableExtensions
    {
        public static void ReadWrite(this ICoinSerializable serializable, Stream stream, bool serializing, ProtocolVersion version = ProtocolVersion.PROTOCOL_VERSION)
        {
            CoinStream s = new CoinStream(stream, serializing)
            {
                ProtocolVersion = version
            };
            serializable.ReadWrite(s);
        }
        public static int GetSerializedSize(this ICoinSerializable serializable, ProtocolVersion version, SerializationType serializationType)
        {
            CoinStream s = new CoinStream(Stream.Null, true);
            s.Type = serializationType;
            s.ReadWrite(serializable);
            return (int)s.Counter.WrittenBytes;
        }
        public static int GetSerializedSize(this ICoinSerializable serializable, TxOptions options)
        {
            var bms = new CoinStream(Stream.Null, true);
            bms.TransactionOptions = options;
            serializable.ReadWrite(bms);
            return (int)bms.Counter.WrittenBytes;
        }
        public static int GetSerializedSize(this ICoinSerializable serializable, ProtocolVersion version = ProtocolVersion.PROTOCOL_VERSION)
        {
            return GetSerializedSize(serializable, version, SerializationType.Disk);
        }

        public static void ReadWrite(this ICoinSerializable serializable, byte[] bytes, ProtocolVersion version = ProtocolVersion.PROTOCOL_VERSION)
        {
            ReadWrite(serializable, new MemoryStream(bytes), false, version);
        }
        public static void FromBytes(this ICoinSerializable serializable, byte[] bytes, ProtocolVersion version = ProtocolVersion.PROTOCOL_VERSION)
        {
            serializable.ReadWrite(new CoinStream(bytes)
            {
                ProtocolVersion = version
            });
        }

        public static T Clone<T>(this T serializable, ProtocolVersion version = ProtocolVersion.PROTOCOL_VERSION) where T : ICoinSerializable, new()
        {
            var instance = new T();
            instance.FromBytes(serializable.ToBytes(version), version);
            return instance;
        }
        public static byte[] ToBytes(this ICoinSerializable serializable, ProtocolVersion version = ProtocolVersion.PROTOCOL_VERSION)
        {
            MemoryStream ms = new MemoryStream();
            serializable.ReadWrite(new CoinStream(ms, true)
            {
                ProtocolVersion = version
            });
            return ToArrayEfficient(ms);
        }

        public static byte[] ToArrayEfficient(this MemoryStream ms)
        {
#if !(PORTABLE || NETCORE)
            var bytes = ms.GetBuffer();
            Array.Resize(ref bytes, (int)ms.Length);
            return bytes;
#else
			return ms.ToArray();
#endif
        }
    }

}

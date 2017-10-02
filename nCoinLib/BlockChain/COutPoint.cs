using nCoinLib.Util.Types;
using nCoinLib.Interfaces;
using System;
using nCoinLib.Util.Streams;

namespace nCoinLib.BlockChain
{
    public class COutPoint : ICoinSerializable
    {
        UInt256 _h;
        UInt32 _n;

        #region Constructors

        public COutPoint(UInt256 hashIn, UInt32 nIn)
        {
            _h = hashIn;
            _n = nIn;
        }

        public COutPoint()
        {
            SetNull();
        }

        public COutPoint(UInt256 hashIn, int nIn)
        {
            _h = hashIn;
            _n = (nIn == -1) ? uint.MaxValue : (UInt32)nIn;
        }

        public COutPoint(Transaction tx, uint i)
            : this(tx.GetHash(), i)
        {
        }

        public COutPoint(Transaction tx, int i)
            : this(tx.GetHash(), i)
        {
        }

        public COutPoint(COutPoint outpoint)
        {
            this.FromBytes(outpoint.ToBytes());
        }


        #endregion

        #region Properties

        public UInt256 hash { get { return _h; } set { _h = value; } }
        public UInt32 n { get { return _n; } set { _n = value; } }

        public bool IsNull
        {
            get { return (hash == UInt256.Zero && n == UInt32.MaxValue); }
        }
        #endregion

        //TODO: Finish COutPoint

        //    ADD_SERIALIZE_METHODS;

        #region Methods

        public void SetNull()
        {
            hash = new UInt256();
            n = 0xFFFFFFFF;
        }

        public override int GetHashCode()
        {
            int nhash = 13;
            nhash = (nhash * 7) + hash.GetHashCode();
            nhash = (nhash * 7) + n.GetHashCode();
            return nhash;
        }

        public override bool Equals(object obj)
        {
            var item = obj as COutPoint;
            if (item == null)
                return false;
            bool equals = true;

            equals &= hash == item.hash;
            equals &= n == item.n;

            return equals;
        }

        public override string ToString()
        {
            return string.Format("COutPoint({0}, {1})", hash.ToString().Substring(0, 10), n);
        }


        #region ICoinSerializable Implementation

        //template<typename Stream, typename Operation>
        //inline void SerializationOp(Stream& s, Operation ser_action)
        //    {
        //        READWRITE(hash);
        //        READWRITE(n);
        //    }
        public void ReadWrite(CoinStream stream)
        {
            stream.ReadWrite(ref _h);
            stream.ReadWrite(ref _n);

        }

        #endregion

        #endregion

        #region Operators
        
        public static bool operator <(COutPoint a, COutPoint b)
        {

            int cmp = a.hash.Compare(b.hash);
            return cmp < 0 || (cmp == 0 && a.n < b.n);
        }

        public static bool operator >(COutPoint a, COutPoint b)
        {
            int cmp = a.hash.Compare(b.hash);
            return cmp < 0 || (cmp == 0 && a.n > b.n);
        }

        public static bool operator ==(COutPoint a, COutPoint b)
        {
            return (a.hash == b.hash && a.n == b.n);
        }

        public static bool operator !=(COutPoint a, COutPoint b)
        {
            return !(a == b);
        }

        #endregion


    }
}

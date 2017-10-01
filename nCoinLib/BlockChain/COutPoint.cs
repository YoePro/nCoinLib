using nCoinLib.Util.Types;
using System;

namespace nCoinLib.BlockChain
{
    public class COutPoint
    {
        public UInt256 hash;
        UInt32 n;

        public COutPoint(UInt256 hashIn, UInt32 nIn)
        {
            this.hash = hashIn;
            this.n = nIn;
        }

        public COutPoint()
        {
            SetNull();
        }

        //TODO: Finish COutPoint

        //    ADD_SERIALIZE_METHODS;

        //template<typename Stream, typename Operation>
        //inline void SerializationOp(Stream& s, Operation ser_action)
        //    {
        //        READWRITE(hash);
        //        READWRITE(n);
        //    }

        public void SetNull()
        {
            hash = new UInt256();
            n = 0xFFFFFFFF;
        }

        public bool IsNull
        {
            get { return (hash == UInt256.Zero && n == UInt32.MaxValue); }
        }


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

    }
}

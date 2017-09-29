using nCoinLib.Util.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/** An input of a transaction.  It contains the location of the previous
 * transaction's output that it claims and a signature that matches the
 * output's public key.
 */

namespace nCoinLib.BlockChain
{
    class CTxIn
    {


        public COutPoint prevout;
        CScript scriptSig;
        public UInt32 nSequence;

        //CScriptWitness scriptWitness; //! Only serialized through CTransaction

        /* Setting nSequence to this value for every input in a transaction
        /* disables nLockTime. */
        static readonly UInt32 SEQUENCE_FINAL = 0xffffffff;

        /* Below flags apply in the context of BIP 68*/
        /* If this flag set, CTxIn::nSequence is NOT interpreted as a
         * relative lock-time. */
        static readonly UInt32 SEQUENCE_LOCKTIME_DISABLE_FLAG = (1 << 31);

        /* If CTxIn::nSequence encodes a relative lock-time and this flag
         * is set, the relative lock-time has units of 512 seconds,
         * otherwise it specifies blocks with a granularity of 1. */
        static readonly UInt32 SEQUENCE_LOCKTIME_TYPE_FLAG = (1 << 22);

        /* If CTxIn::nSequence encodes a relative lock-time, this mask is
         * applied to extract that lock-time from the sequence field. */
        static readonly UInt32 SEQUENCE_LOCKTIME_MASK = 0x0000ffff;

        /* In order to use the same number of bits to encode roughly the
         * same wall-clock duration, and because blocks are naturally
         * limited to occur every 600s on average, the minimum granularity
         * for time-based relative lock-time is fixed at 512 seconds.
         * Converting from CTxIn::nSequence to seconds is performed by
         * multiplying by 512 = 2^9, or equivalently shifting up by
         * 9 bits. */
        static readonly int SEQUENCE_LOCKTIME_GRANULARITY = 9;

        CTxIn()
        {
            nSequence = SEQUENCE_FINAL;
        }

        //    ADD_SERIALIZE_METHODS;

        //template<typename Stream, typename Operation>
        //inline void SerializationOp(Stream& s, Operation ser_action)
        //    {
        //        READWRITE(prevout);
        //        READWRITE(scriptSig);
        //        READWRITE(nSequence);
        //    }


        static public bool operator ==(CTxIn a, CTxIn b)
        {
            return (a.prevout == b.prevout &&
                    a.scriptSig == b.scriptSig &&
                    a.nSequence == b.nSequence);
        }

        static public bool operator !=(CTxIn a, CTxIn b)
        {
            return !(a == b);
        }

        //    explicit CTxIn(COutPoint prevoutIn, CScript scriptSigIn = CScript(), uint32_t nSequenceIn = SEQUENCE_FINAL);
        CTxIn(COutPoint prevoutIn, CScript scriptSigIn, UInt32 nSequenceIn)
        {
            prevout = prevoutIn;
            scriptSig = scriptSigIn;
            nSequence = nSequenceIn;
        }

        //    CTxIn(uint256 hashPrevTx, uint32_t nOut, CScript scriptSigIn = CScript(), uint32_t nSequenceIn = SEQUENCE_FINAL);
        CTxIn(UInt256 hashPrevTx, UInt32 nOut, CScript scriptSigIn, UInt32 nSequenceIn)
        {
            prevout = new COutPoint(hashPrevTx, nOut);
            scriptSig = scriptSigIn;
            nSequence = nSequenceIn;
        }

        public override string ToString()
        {
            string str = "CTxIn(";
            str += prevout.ToString();
            if (prevout.IsNull)
                str += string.Format(", coinbase %s", HexStr(scriptSig));
            else
                str += string.Format(", scriptSig=%s", HexStr(scriptSig).substr(0, 24));
            if (nSequence != SEQUENCE_FINAL)
                str += string.Format(", nSequence=%u", nSequence);
            str += ")";
            return str;
        }



    }
}

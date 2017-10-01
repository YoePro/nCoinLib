using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCoinLib.BlockChain
{
    class CTxOut
    {
        public CAmount nValue;
        public CScript scriptPubKey;

        

        CTxOut()
        {
            SetNull();
        }

        //    CTxOut(const CAmount& nValueIn, CScript scriptPubKeyIn);
        CTxOut(CAmount nValueIn, CScript scriptPubKeyIn)
        {
            nValue = nValueIn;
            scriptPubKey = scriptPubKeyIn;
        }

        //TODO: Finish code CTxOut
        //    ADD_SERIALIZE_METHODS;

        //template<typename Stream, typename Operation>
        //inline void SerializationOp(Stream& s, Operation ser_action)
        //    {
        //        READWRITE(nValue);
        //        READWRITE(scriptPubKey);
        //    }

        void SetNull()
        {
            nValue = -1;
            scriptPubKey.clear();
        }

        bool IsNull()
        {
            return (nValue == -1);
        }

        public static bool operator ==(CTxOut a, CTxOut b)
        {
            return (a.nValue == b.nValue &&
                    a.scriptPubKey == b.scriptPubKey);
        }

        public static bool operator !=(CTxOut a, CTxOut b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return string.Format("CTxOut(nValue=%d.%08d, scriptPubKey=%s)", nValue / COIN, nValue % COIN, HexStr(scriptPubKey).substr(0, 30));
        }
    }
}

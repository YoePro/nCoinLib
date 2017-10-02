using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCoinLib.BlockChain.Protocol
{
    [Flags]
    public enum TxOptions : uint
    {
        None = 0x00000000,
        Witness = 0x40000000,
        All = Witness
    }
}

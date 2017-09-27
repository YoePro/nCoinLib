using nCoinLib.Interfaces;
using nCoinLib.Util.Streams;
using nCoinLib.Util.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCoinLib.BlockChain
{
    public class MerkleBlock : ICoinSerializable
    {

        #region Variable declarations
        BlockHeader header;
        PartialMerkleTree _PartialMerkleTree;

        #endregion

        #region Constructors

        public MerkleBlock()
        {

        }

        // Create from a CBlock, filtering transactions according to filter
        // Note that this will call IsRelevantAndUpdate on the filter for each transaction,
        // thus the filter will likely be modified.
        public MerkleBlock(Block block, BloomFilter filter)
        {
            header = block.Header;

            List<bool> vMatch = new List<bool>();
            List<UInt256> vHashes = new List<UInt256>();


            for (uint i = 0; i < block.Transactions.Count; i++)
            {
                UInt256 hash = block.Transactions[(int)i].GetHash();
                vMatch.Add(filter.IsRelevantAndUpdate(block.Transactions[(int)i]));
                vHashes.Add(hash);
            }

            _PartialMerkleTree = new PartialMerkleTree(vHashes.ToArray(), vMatch.ToArray());
        }

        public MerkleBlock(Block block, UInt256[] txIds)
        {
            header = block.Header;

            List<bool> vMatch = new List<bool>();
            List<UInt256> vHashes = new List<UInt256>();
            for (int i = 0; i < block.Transactions.Count; i++)
            {
                var hash = block.Transactions[i].GetHash();
                vHashes.Add(hash);
                vMatch.Add(txIds.Contains(hash));
            }
            _PartialMerkleTree = new PartialMerkleTree(vHashes.ToArray(), vMatch.ToArray());
        }
        #endregion

        #region Fields

        public BlockHeader Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
            }
        }

        public PartialMerkleTree PartialMerkleTree
        {
            get
            {
                return _PartialMerkleTree;
            }
            set
            {
                _PartialMerkleTree = value;
            }
        }


        #endregion

        #region Methods
        public void ReadWrite(CoinStream stream)
        {
            stream.ReadWrite(ref header);
            stream.ReadWrite(ref _PartialMerkleTree);
        }
        #endregion



    }
}

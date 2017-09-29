using System;
using System.Collections.Generic;
using System.Linq;
//using nCoinLib.Coin;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
using nCoinLib.Util.Encoders;
using nCoinLib.Util.Types;
using nCoinLib.Util.Streams;

/// <summary>
/// A block in the chain
/// </summary>
namespace nCoinLib.BlockChain
{
    public class Block
    {
        private BlockHeader blockHeader = new BlockHeader();
        private List<Transaction> vtx = new List<Transaction>();

        // TODO: Update code and remove this
        public const uint MAX_BLOCK_SIZE = Coin.Consensus.MAX_BLOCK_SIZE;

        #region "Constructors"
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Block()
        {
            ClearBlock();
        }

        /// <summary>
        /// Constructor with a given BlockHeader
        /// </summary>
        /// <param name="blockHeader">A block header</param>
        public Block(BlockHeader blockHeader)
        {
            ClearBlock();
            this.blockHeader = blockHeader;
        }

        /// <summary>
        /// Constructor from bytes
        /// </summary>
        /// <param name="bytes"></param>
        public Block(byte[] bytes)
        {
            this.ReadWrite(bytes);
        }

        /// <summary>
        /// Constructor with a given BlockHeader and transactions
        /// </summary>
        /// <param name="BlockHeader"></param>
        /// <param name="transactionsList"></param>
        public Block(BlockHeader BlockHeader, List<Transaction> transactionsList)
        {
            ClearBlock();
            this.blockHeader = BlockHeader;
            this.vtx = transactionsList;
        }

        #endregion

        #region Properties

        public BlockHeader Header { get { return blockHeader; } }

        public List<Transaction> Transactions
        {
            get { return vtx; }
            set { vtx = value; }
        }

        public UInt256 BlockHash { get { return blockHeader.Hash; } }

        public MerkleNode MerkleRoot { get { return MerkleNode.GetRoot(Transactions.Select(t => t.GetHash())); } }

        public bool HeaderOnly { get { return (vtx == null || vtx.Count == 0) ? true : false; } }

        #endregion

        //public MerkleNode GetMerkleRoot()
        //{
        //    return MerkleNode.GetRoot(Transactions.Select(t => t.GetHash()));
        //}

        public void ReadWrite(CoinStream stream)
        {
            stream.ReadWrite(ref blockHeader);
            stream.ReadWrite(ref vtx);
        }
        public void ReadWrite(byte[] byteArray, int startIndex)
        {
            var mStream = new MemoryStream(byteArray);
            mStream.Position += startIndex;
            CoinStream bitStream = new CoinStream(mStream, false);
            ReadWrite(bitStream);
        }


        /// <summary>
        /// Clear the block
        /// </summary>
        void ClearBlock()
        {
            blockHeader.Clear();
            vtx.Clear();
        }



        /// <summary>
        /// Adds a transaction to the block
        /// </summary>
        /// <param name="tx">The transaction</param>
        /// <returns></returns>
        public Transaction AddTransaction(Transaction tx)
        {
            Transactions.Add(tx);
            return tx;
        }

        /// <summary>
        /// Create a block with the specified option only. (useful for stripping data from a block)
        /// </summary>
        /// <param name="options">Options to keep</param>
        /// <returns>A new block with only the options wanted</returns>
        public Block WithOptions(TransactionOptions options)
        {
            if (Transactions.Count == 0)
                return this;
            if (options == TransactionOptions.Witness && Transactions[0].HasWitness)
                return this;
            if (options == TransactionOptions.None && !Transactions[0].HasWitness)
                return this;
            var instance = new Block();
            var ms = new MemoryStream();
            var bms = new CoinStream(ms, true);
            bms.TransactionOptions = options;
            this.ReadWrite(bms);
            ms.Position = 0;
            bms = new CoinStream(ms, false);
            bms.TransactionOptions = options;
            instance.ReadWrite(bms);
            return instance;
        }

        public void UpdateMerkleRoot()
        {
            this.Header.HashMerkleRoot = GetMerkleRoot().Hash;
        }

        #region Control functions


        /// <summary>
        /// Check proof of work / proof of stake / and merkle root
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            return Check(null);
        }

        /// <summary>
        /// Check proof of work and merkle root
        /// </summary>
        /// <param name="consensus"></param>
        /// <returns></returns>
        public bool Check(Consensus consensus)
        {
            return CheckMerkleRoot() && Header.CheckProofOfWork(consensus);
        }

        public bool CheckProofOfWork()
        {
            return CheckProofOfWork(null);
        }

        public bool CheckProofOfWork(Consensus consensus)
        {
            return Header.CheckProofOfWork(consensus);
        }

        public bool CheckMerkleRoot()
        {
            return Header.HashMerkleRoot == GetMerkleRoot().Hash;
        }

        #endregion


        public Block CreateNextBlockWithCoinbase(CoinAddress address, int height)
        {
            return CreateNextBlockWithCoinbase(address, height, DateTimeOffset.UtcNow);
        }
        public Block CreateNextBlockWithCoinbase(CoinAddress address, int height, DateTimeOffset now)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            Block block = new Block();
            block.Header.Nonce = RandomUtils.GetUInt32();
            block.Header.HashPrevBlock = this.BlockHash;
            block.Header.BlockTime = now;
            var tx = block.AddTransaction(new Transaction());
            tx.AddInput(new TxIn()
            {
                ScriptSig = new Script(Op.GetPushOp(RandomUtils.GetBytes(30)))
            });
            tx.Outputs.Add(new TxOut(address.Network.GetReward(height), address)
            {
                Value = address.Network.GetReward(height)
            });
            return block;
        }

        public Block CreateNextBlockWithCoinbase(PubKey pubkey, Money value)
        {
            return CreateNextBlockWithCoinbase(pubkey, value, DateTimeOffset.UtcNow);
        }
        public Block CreateNextBlockWithCoinbase(PubKey pubkey, Money value, DateTimeOffset now)
        {
            Block block = new Block();
            block.Header.Nonce = RandomUtils.GetUInt32();
            block.Header.HashPrevBlock = this.BlockHash;
            block.Header.BlockTime = now;
            var tx = block.AddTransaction(new Transaction());
            tx.AddInput(new TxIn()
            {
                ScriptSig = new Script(Op.GetPushOp(RandomUtils.GetBytes(30)))
            });
            tx.Outputs.Add(new TxOut()
            {
                Value = value,
                ScriptPubKey = PayToPubkeyHashTemplate.Instance.GenerateScriptPubKey(pubkey)
            });
            return block;
        }
#if !NOJSONNET
        public static Block ParseJson(string json)
        {
            var formatter = new BlockExplorerFormatter();
            var block = JObject.Parse(json);
            var txs = (JArray)block["tx"];
            Block blk = new Block();
            blk.Header.Bits = new Target((uint)block["bits"]);
            blk.Header.BlockTime = Utils.UnixTimeToDateTime((uint)block["time"]);
            blk.Header.Nonce = (uint)block["nonce"];
            blk.Header.Version = (int)block["ver"];
            blk.Header.HashPrevBlock = UInt256.Parse((string)block["prev_block"]);
            blk.Header.HashMerkleRoot = UInt256.Parse((string)block["mrkl_root"]);
            foreach (var tx in txs)
            {
                blk.AddTransaction(formatter.Parse((JObject)tx));
            }
            return blk;
        }
#endif
        public static Block Parse(string hex)
        {
            return new Block(Encoders.Hex.DecodeData(hex));
        }

        public MerkleBlock Filter(params UInt256[] txIds)
        {
            return new MerkleBlock(this, txIds);
        }

        public MerkleBlock Filter(BloomFilter filter)
        {
            return new MerkleBlock(this, filter);
        }
    }
}

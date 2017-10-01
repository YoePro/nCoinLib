using System;
using nCoinLib.Util.Encoders;
using nCoinLib.Util;
using nCoinLib.Util.Types;
using nCoinLib.Util.Streams;

/// <summary>
/// Header of the block including signatures
/// </summary>

// Messageheader struct is according to the following    
// FIELD            BYTES  TYPE         DESCRIPTION
// --------------------------------------------------------------------
// MAGIC            4      uint32       Magic value indicating message origin network, and used to seek to next message when stream state is unknown
//                                      Defined in Coin.Consensus.MAGIC_MAIN or MAGIC_TESTNET
// COMMAND          12     char[12]     ASCII string identifying the packet content, NULL padded (non-NULL padding results in packet rejected)
// LENGTH           4      uint32       Length of payload in number of bytes
// CHECKUM          4      uint32       First 4 bytes of the Payload hash
// Payload
// ---
// Total length in bytes is 24 + Payload


// Blockheaders struct according to the following
// FIELD            BYTES  TYPE         DESCRIPTION
// --------------------------------------------------------------------
// VERSION          4      uint32       Block version information (note, this is signed)
// PREV_BLOCK_HASH  32     uint256      The hash value of the previous block this particular block references
// MERKLE_ROOT      32     uint256      The reference to a Merkle tree collection which is a hash of all transactions related to this block
// TIMESTAMP        4      uint32       A timestamp recording when this block was created (Will overflow in 2106)
// DIFF_BITS        4      uint32       The calculated difficulty target being used for this block
// NONCE            4      uint32       The nonce used to generate this block… to allow variations of the header and compute different hashes
// TXN_COUNT        1      var_int      Number of transaction entries, this value is always 0



namespace nCoinLib.BlockChain
{
    public class BlockHeader
    {
        internal const int Size = 80;
        



        UInt256 hashMerkleRoot;
        UInt256 prevBlockHash;
        UInt256[] _Hashes;
        static BigInteger Pow256 = BigInteger.ValueOf(2).Pow(256);
        const int CURRENT_VERSION = 3;
        int nVersion;
        uint nNonce;
        uint nTime;
        uint nBits;

        #region Constructors


        public BlockHeader(string hex)
            : this(Encoders.Hex.DecodeData(hex))
        {

        }

        public BlockHeader()
        {
            Clear();
        }

        public BlockHeader(byte[] bytes)
        {
            this.ReadWrite(bytes);
        }
        #endregion

        #region Fields
        public UInt256 Hash { get { return GetHash(); } }
        
        public UInt256 PrevBlockHash
        {
            get
            {
                return prevBlockHash;
            }
            set
            {
                prevBlockHash = value;
            }
        }

        public Objective Bits
        {
            get
            {
                return nBits;
            }
            set
            {
                nBits = value;
            }
        }

        

        public int Version
        {
            get
            {
                return nVersion;
            }
            set
            {
                nVersion = value;
            }
        }

        

        public UInt32 Nonce
        {
            get
            {
                return nNonce;
            }
            set
            {
                nNonce = value;
            }
        }

        public UInt256 HashMerkleRoot
        {
            get
            {
                return hashMerkleRoot;
            }
            set
            {
                hashMerkleRoot = value;
            }
        }

        public bool IsNull
        {
            get
            {
                return (nBits == 0);
            }
        }


        public DateTimeOffset BlockTime
        {
            get
            {
                return UnixDateTime.UnixTimeToDateTime(nTime); //.UnixTimeToDateTime(nTime);
            }
            set
            {
                this.nTime = UnixDateTime.DateTimeToUnixTime(value);
            }
        }



        #endregion


        public static BlockHeader Parse(string hex)
        {
            return new BlockHeader(Encoders.Hex.DecodeData(hex));
        }

        internal void Clear()
        {
            nVersion = CURRENT_VERSION;
            prevBlockHash = 0;
            hashMerkleRoot = 0;
            nTime = 0;
            nBits = 0;
            nNonce = 0;
        }


        #region ICoinSerializable Members

        public void ReadWrite(CoinStream stream)
        {
            stream.ReadWrite(ref nVersion);
            stream.ReadWrite(ref prevBlockHash);
            stream.ReadWrite(ref hashMerkleRoot);
            stream.ReadWrite(ref nTime);
            stream.ReadWrite(ref nBits);
            stream.ReadWrite(ref nNonce);
        }

        #endregion



        private UInt256 GetHash()
        {
            UInt256 hash = null;
            var hashes = _Hashes;
            if (hashes != null)
            {
                hash = hashes[0];
            }
            if (hash != null)
                return hash;

            using (HashStream hs = new HashStream())
            {
                this.ReadWrite(new CoinStream(hs, true));
                hash = hs.GetHash();
            }

            hashes = _Hashes;
            if (hashes != null)
            {
                hashes[0] = hash;
            }
            return hash;
        }

        /// <summary>
        /// If called, GetHash becomes cached, only use if you believe the instance will not be modified after calculation. Calling it a second type invalidate the cache.
        /// </summary>
        public void CacheHashes()
        {
            _Hashes = new UInt256[1];
        }

        public bool CheckProofOfWork()
        {
            return CheckProofOfWork(null);
        }

        public bool CheckProofOfWork(Consensus consensus)
        {
            consensus = consensus ?? Consensus.Main;
            var bits = Bits.ToBigInteger();
            if (bits.CompareTo(BigInteger.Zero) <= 0 || bits.CompareTo(Pow256) >= 0)
                return false;
            // Check proof of work matches claimed amount
            return consensus.GetPoWHash(this) <= Bits.ToUInt256();
        }

        public override string ToString()
        {
            return GetHash().ToString();
        }

        /// <summary>
        /// Set time to consensus acceptable value
        /// </summary>
        /// <param name="network">Network</param>
        /// <param name="prev">previous block</param>
        public void UpdateTime(Network network, ChainedBlock prev)
        {
            UpdateTime(DateTimeOffset.UtcNow, network, prev);
        }

        /// <summary>
        /// Set time to consensus acceptable value
        /// </summary>
        /// <param name="consensus">Consensus</param>
        /// <param name="prev">previous block</param>
        public void UpdateTime(Consensus consensus, ChainedBlock prev)
        {
            UpdateTime(DateTimeOffset.UtcNow, consensus, prev);
        }

        /// <summary>
        /// Set time to consensus acceptable value
        /// </summary>
        /// <param name="now">The expected date</param>
        /// <param name="consensus">Consensus</param>
        /// <param name="prev">previous block</param>		
        public void UpdateTime(DateTimeOffset now, Consensus consensus, ChainedBlock prev)
        {
            var nOldTime = this.BlockTime;
            var mtp = prev.GetMedianTimePast() + TimeSpan.FromSeconds(1);
            var nNewTime = mtp > now ? mtp : now;

            if (nOldTime < nNewTime)
                this.BlockTime = nNewTime;

            // Updating time can change work required on testnet:
            if (consensus.PowAllowMinDifficultyBlocks)
                Bits = GetWorkRequired(consensus, prev);
        }

        /// <summary>
        /// Set time to consensus acceptable value
        /// </summary>
        /// <param name="now">The expected date</param>
        /// <param name="network">Network</param>
        /// <param name="prev">previous block</param>		
        public void UpdateTime(DateTimeOffset now, Network network, ChainedBlock prev)
        {
            UpdateTime(now, network.Consensus, prev);
        }

        public Objective GetWorkRequired(Network network, ChainedBlock prev)
        {
            return GetWorkRequired(network.Consensus, prev);
        }

        public Objective GetWorkRequired(Consensus consensus, ChainedBlock prev)
        {
            return new ChainedBlock(this, null, prev).GetWorkRequired(consensus);
        }
    }
}

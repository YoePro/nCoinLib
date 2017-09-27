using System;
using nCoinLib.Util.Encoders;
using nCoinLib.Util;
using nCoinLib.Util.Types;

/// <summary>
/// Header of the block including signatures
/// </summary>
namespace nCoinLib.BlockChain
{
    public class BlockHeader
    {
        internal const int Size = 80;
        
        UInt256 hashMerkleRoot;
        UInt256[] _Hashes;
        static BigInteger Pow256 = BigInteger.ValueOf(2).Pow(256);
        const int CURRENT_VERSION = 3;

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
        
        UInt256 prevBlockHash;
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

        public Target Bits
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

        int nVersion;

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

        uint nNonce;

        public uint Nonce
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

        public Target GetWorkRequired(Network network, ChainedBlock prev)
        {
            return GetWorkRequired(network.Consensus, prev);
        }

        public Target GetWorkRequired(Consensus consensus, ChainedBlock prev)
        {
            return new ChainedBlock(this, null, prev).GetWorkRequired(consensus);
        }
    }
}

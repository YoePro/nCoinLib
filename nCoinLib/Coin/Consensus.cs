using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Coin Consensus Parameters
 * 
 */

namespace nCoinLib.Coin
{
    public static class Consensus
    {
        // Dummy setup
        public const string COIN_NAME = "Lindas Sister Coin";

        // 
        public const string CoinAbbreviation = "LSC";
        public const Int64 MAX_BLOCKS = (16000000000);  // 8B
        public const Int32 INITIAL_BLOCK_REWARD = 1400;
        public const Int32 REWARD_HALVING_INTERVAL = 140000;

        // Enable/Disable Proof of Work
        public const bool ENABLE_POW = true;

        // Enable/Disable Proof of Stake
        public const bool ENABLE_POS = true;

        // Enable/Disable Masternodes
        public const bool ENABLE_MASTERNODES = true;

        // Master node reward in % of block reward
        public const Double MASTER_NODE_REWARD = .29;

        // Proof of Stake award in % of block reward
        public const Double POS_REWARD = .20;

        // Min staking time for Proof of Stake
        public const UInt64 MIN_STAKING_TIME = 24 * 60 * 60;

        // Max staking time for Proof of Stake
        public const UInt64 MAX_STAKING_TIME = 0;

        // Proof of Work award in % of block reward
        public const Double POW_REWARD = .50;

        // Team reward in % of block reward
        public const Double DEV_REWARD = .01;

        // Block mature time
        public const int CONFIRMATIONS_TO_MATURE = 25;

        // Maximal block size
        public const uint MAX_BLOCK_SIZE = 1000 * 1000;

        // Blockheader magic main net
        public const UInt32 MAGIC_MAIN = 0x0CAFFE01;

        // Blockheader magic testnet
        public const UInt32 MAGIC_TESTNET = 0x0CAFFE11;

    }
}

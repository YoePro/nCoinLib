// Copyright (c) 2015-2016 The Bitcoin Core developers
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.


/*     WARNING! If you're reading this because you're learning about crypto
       and/or designing a new system that will use merkle trees, keep in mind
       that the following merkle tree algorithm has a serious flaw related to
       duplicate txids, resulting in a vulnerability (CVE-2012-2459).
       The reason is that if the number of hashes in the list at a given time
       is odd, the last one is duplicated before computing the next level (which
       is unusual in Merkle trees). This results in certain sequences of
       transactions leading to the same merkle root. For example, these two
       trees:
                    A               A
                  /  \            /   \
                B     C         B       C
               / \    |        / \     / \
              D   E   F       D   E   F   F
             / \ / \ / \     / \ / \ / \ / \
             1 2 3 4 5 6     1 2 3 4 5 6 5 6
       for transaction lists [1,2,3,4,5,6] and [1,2,3,4,5,6,5,6] (where 5 and
       6 are repeated) result in the same root hash A (because the hash of both
       of (F) and (F,F) is C).
       The vulnerability results from being able to send a block with such a
       transaction list, with the same merkle root, and the same block hash as
       the original without duplication, resulting in failed validation. If the
       receiving node proceeds to mark that block as permanently invalid
       however, it will fail to accept further unmodified (and thus potentially
       valid) versions of the same block. We defend against this by detecting
       the case where we would hash two identical hashes at the end of the list
       together, and treating that identically to the block having an invalid
       merkle root. Assuming no double-SHA256 collisions, this will detect all
       known ways of changing the transactions without affecting the merkle
       root.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nCoinLib.Util.Types;
using nCoinLib.BlockChain;

// #include "hash.h"
// #include "utilstrencodings.h"

namespace nCoinLib.Util
{
    public class MerkleUtil
    {
        /* This implements a constant-space merkle root/path calculator, limited to 2^32 leaves. */
        public List<UInt256> Leaves { get; set; }
        public UInt256 pRoot { get; set; }
        public int branchPos { get; set; }
        public bool pMutated { get; set; }
        public List<UInt256> pBranch { get; set; }

        MerkleUtil(List<UInt256> Leaves, UInt256 PRoot, bool PMutated, int BranchPos, List<UInt256> PBranch)
        {
            this.Leaves = Leaves;
            this.pRoot = PRoot;
            this.pMutated = PMutated;
            this.branchPos = BranchPos;
            this.pBranch = PBranch;
        }

        MerkleUtil()
        {
        }

        // uint32_t -> UInt32 atm. Needs to be replaced
        public void MerkleComputation()
        {
            // static void MerkleComputation(const std::vector<uint256>& leaves, uint256* proot, bool* pmutated, uint32_t branchpos, std::vector<uint256>* pbranch)


            UInt256 h;
            bool matchh, mutated = false;
            int level;

            // count is the number of leaves processed so far.
            int count = 0;

            // inner is an array of eagerly computed subtree hashes, indexed by tree
            // level (0 being the leaves).
            // For example, when count is 25 (11001 in binary), inner[4] is the hash of
            // the first 16 leaves, inner[3] of the next 8 leaves, and inner[0] equal to
            // the last leaf. The other inner entries are undefined.
            UInt256[] inner = new UInt256[32];

            // Which position in inner is a hash that depends on the matching leaf.
            int matchlevel = -1;

            if (pBranch.Count() > 0)
            {
                pBranch = new List<UInt256>();
            }

            // Check if the tree has leaves. 
            // if so - no change is needed and return.
            if (Leaves.Count() == 0)
            {
                if (pMutated)
                {
                    pMutated = false;
                }
                if (pRoot > 0)
                {
                    pRoot = new UInt256();
                }

                return;
            }

            // First process all leaves into 'inner' values.
            while (count < Leaves.Count())
            {
                level = 0;

                h = Leaves[count];
                matchh = (branchPos == count);
                count++;

                // For each of the lower bits in count that are 0, do 1 step. Each
                // corresponds to an inner value that existed before processing the
                // current leaf, and each needs a hash to combine it.
                for (level = 0; (count & ((1) << level)) == 0; level++)
                {
                    if (pBranch.Count() > 0)
                    {
                        if (matchh)
                        {
                            pBranch.Add(inner[level]);
                        }
                        else if (matchlevel == level)
                        {
                            pBranch.Add(h);
                            matchh = true;
                        }
                    }
                    mutated |= (inner[level] == h);
                    CHash256().Write(inner[level].begin(), 32).Write(h.begin(), 32).Finalize(h.begin());
                }
                // Store the resulting hash at inner position level.
                inner[level] = h;
                if (matchh)
                {
                    matchlevel = level;
                }
            }
            // Do a final 'sweep' over the rightmost branch of the tree to process
            // odd levels, and reduce everything to a single top value.
            // Level is the level (counted from the bottom) up to which we've sweeped.

            level = 0;
            // As long as bit number level in count is zero, skip it. It means there
            // is nothing left at this level.
            while ((count & ((1) << level)) == 0)
            {
                level++;
            }
            h = inner[level];
            matchh = (matchlevel == level);

            while (count != ((1) << level))
            {
                // If we reach this point, h is an inner value that is not the top.
                // We combine it with itself (Bitcoin's special rule for odd levels in
                // the tree) to produce a higher level one.
                if (pBranch && matchh)
                {
                    pBranch.Add(h);
                }

                CHash256().Write(h.begin(), 32).Write(h.begin(), 32).Finalize(h.begin());
                // Increment count to the value it would have if two entries at this
                // level had existed.

                count += ((1) << level);
                level++;
                // And propagate the result upwards accordingly.
                while ((count & ((1) << level)) == 0)
                {
                    if (pBranch.Count() > 0)
                    {
                        if (matchh)
                        {
                            pBranch.Add(inner[level]);
                        }
                        else if (matchlevel == level)
                        {
                            pBranch.Add(h);
                            matchh = true;
                        }
                    }
                    CHash256().Write(inner[level].begin(), 32).Write(h.begin(), 32).Finalize(h.begin());
                    level++;
                }
            }
            // Return result.
            if (pMutated) pMutated = mutated;
            if (pRoot.Size > 0) pRoot = h;
        }

        UInt256 ComputeMerkleRoot(List<UInt256> leaves, bool mutated)
        {
            Leaves = leaves;
            pRoot = new UInt256();
            pMutated = mutated;
            branchPos = -1;
            pBranch = null;

            //MerkleComputation(leaves, &hash, mutated, -1, nullptr);
            MerkleComputation();
            return pRoot;
        }

        List<UInt256> ComputeMerkleBranch(List<UInt256> leaves, int position)
        {
            Leaves = leaves;
            branchPos = position;
            pBranch = new List<UInt256>();

            pRoot = null;
            pMutated = false;

            //MerkleComputation(leaves, nullptr, nullptr, position, &ret);
            MerkleComputation();
            return pBranch;
        }

        UInt256 ComputeMerkleRootFromBranch(UInt256& leaf, List<UInt256>& vMerkleBranch, uint32_t nIndex)
        {
            UInt256 hash = leaf;
            for (List<UInt256>::const_iterator it = vMerkleBranch.begin(); it != vMerkleBranch.end(); ++it)
            {
                if (nIndex & 1)
                {
                    hash = Hash(BEGIN(*it), END(*it), BEGIN(hash), END(hash));
                }
                else
                {
                    hash = Hash(BEGIN(hash), END(hash), BEGIN(*it), END(*it));
                }
                nIndex >>= 1;
            }
            return hash;
        }

        UInt256 BlockMerkleRoot(Block _block, bool mutated)
        {
            Leaves = new List<UInt256>();
            pRoot = new UInt256();
            pMutated = mutated;
            branchPos = -1;
            pBranch = null;

            // Block.vtx.Count
//            for (int s = 0; s < _block.vtx.Count(); s++)

            for (int s = 0; s < _block.Transactions.Count(); s++)
            {
                Leaves[s] = _block.Transactions[s].GetHashCode();
            }

            //return ComputeMerkleRoot(Leaves, mutated);
            MerkleComputation();
            return pRoot;
        }

        UInt256 BlockWitnessMerkleRoot(Block _block, bool mutated)
        {
            Leaves = new List<UInt256>();
            //leaves.resize(block.vtx.size());

            Leaves[0] = UInt256.Zero; // The witness hash of the coinbase is 0.

            for (int s = 1; s < _block.Transactions.Count(); s++)
            {
                Leaves[s] = _block.vtx[s]->GetWitnessHash();
            }

            //return ComputeMerkleRoot(leaves, mutated);
            return ComputeMerkleRoot
        }

        List<UInt256> BlockMerkleBranch(CBlock& block, uint position)
        {
            List<UInt256> leaves;
            leaves.resize(block.vtx.size());
            for (size_t s = 0; s < block.vtx.size(); s++)
            {
                leaves[s] = block.vtx[s]->GetHash();
            }
            return ComputeMerkleBranch(leaves, position);
        }

    }
}

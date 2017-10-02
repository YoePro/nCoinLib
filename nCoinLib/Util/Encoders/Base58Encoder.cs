using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCoinLib.Util.Encoders
{
    class Base58Encoder
    {
        /** All alphanumeric characters except for "0", "I", "O", and "l" */
        static char[] pszBase58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

        bool DecodeBase58(string psz, ref List<char> vch)
        {
            // Skip leading spaces.
            psz.TrimStart();

            // Skip and count leading '1's.
            int zeroes = 0;
            int length = 0;
            int cPos = 0;

            while (psz.Substring(cPos, 1) == "1")
            {
                zeroes++;
                cPos++;
            }
            // Allocate enough space in big-endian base256 representation.

            int size = psz.Length * 733 / 1000 + 1; // log(58) / log(256), rounded up.
            List<char> b256(size);
            /*
    // Process the characters.
    while (* psz && !isspace(*psz)) {
        // Decode base58 character
        const char* ch = strchr(pszBase58, *psz);
        if (ch == nullptr)
            return false;
        // Apply "b256 = b256 * 58 + ch".
        int carry = ch - pszBase58;
    int i = 0;
        for (std::vector<unsigned char>::reverse_iterator it = b256.rbegin(); (carry != 0 || i<length) && (it != b256.rend()); ++it, ++i) {
            carry += 58 * (* it);
            * it = carry % 256;
    carry /= 256;
        }
        assert(carry == 0);
length = i;
        psz++;
    }
    // Skip trailing spaces.
    while (isspace(* psz))
        psz++;
    if (* psz != 0)
        return false;
    // Skip leading zeroes in b256.
    std::vector<unsigned char>::iterator it = b256.begin() + (size - length);
    while (it != b256.end() && * it == 0)
        it++;
    // Copy result into output vector.
    vch.reserve(zeroes + (b256.end() - it));
    vch.assign(zeroes, 0x00);
    while (it != b256.end())
        vch.push_back(*(it++));
        */
            return true;
        }

    }
}

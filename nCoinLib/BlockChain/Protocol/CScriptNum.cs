using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * Numeric opcodes (OP_1ADD, etc) are restricted to operating on 4-byte integers.
 * The semantics are subtle, though: operands must be in the range [-2^31 +1...2^31 -1],
 * but results may overflow (and are valid as long as they are not used in a subsequent
 * numeric operation). CScriptNum enforces those semantics by storing results as
 * an int64 and allowing out-of-range values to be returned as a vector of bytes but
 * throwing an exception if arithmetic is done or the result is interpreted as an integer.
 */


namespace nCoinLib.BlockChain.Protocol
{
    public class CScriptNum
    {
        static readonly IntPtr nDefaultMaxNumSize = (IntPtr)4;
        Int64 m_value;

        #region Constructors


        CScriptNum(ref Int64 n)
        {
            m_value = n;
        }

        CScriptNum(ref string vch, bool fRequireMinimal, IntPtr nMaxNumSize)
        {
            // ToDo - get max value for nMaxNumSize
            if (vch.Length > nMaxNumSize)
            {
                throw scriptnum_error("script number overflow");
            }
            if (fRequireMinimal && vch.Length > 0)
            {
                // Check that the number is encoded with the minimum possible
                // number of bytes.
                //
                // If the most-significant-byte - excluding the sign bit - is zero
                // then we're not minimal. Note how this test also rejects the
                // negative-zero encoding, 0x80.
                if ((vch.back() & 0x7f) == 0)
                {
                    // One exception: if there's more than one byte and the most
                    // significant bit of the second-most-significant-byte is set
                    // it would conflict with the sign bit. An example of this case
                    // is +-255, which encode to 0xff00 and 0xff80 respectively.
                    // (big-endian).
                    if (vch.Length <= 1 || (vch[vch.Length - 2] & 0x80) == 0)
                    {
                        throw scriptnum_error("non-minimally encoded script number");
                    }
                }
            }
            m_value = set_vch(vch);
        }
        #endregion

        #region Operators
        static public bool operator ==(Int64 rhs)
        {
            return m_value == rhs;
        }


        /*
        inline bool operator ==(const int64_t& rhs) const    { return m_value == rhs; }
    inline bool operator !=(const int64_t& rhs) const    { return m_value != rhs; }
inline bool operator <=(const int64_t& rhs) const    { return m_value <= rhs; }
    inline bool operator <(const int64_t& rhs) const    { return m_value<rhs; }
    inline bool operator >=(const int64_t& rhs) const    { return m_value >= rhs; }
    inline bool operator >(const int64_t& rhs) const    { return m_value >  rhs; }

    inline bool operator ==(const CScriptNum& rhs) const { return operator ==(rhs.m_value); }
    inline bool operator !=(const CScriptNum& rhs) const { return operator !=(rhs.m_value); }
    inline bool operator <=(const CScriptNum& rhs) const { return operator <=(rhs.m_value); }
    inline bool operator <(const CScriptNum& rhs) const { return operator <(rhs.m_value); }
    inline bool operator >=(const CScriptNum& rhs) const { return operator >=(rhs.m_value); }
    inline bool operator >(const CScriptNum& rhs) const { return operator >(rhs.m_value); }

    inline CScriptNum operator +(   const int64_t& rhs)    const { return CScriptNum(m_value + rhs);}
    inline CScriptNum operator -(   const int64_t& rhs)    const { return CScriptNum(m_value - rhs);}
    inline CScriptNum operator +(   const CScriptNum& rhs) const { return operator +(rhs.m_value);   }
    inline CScriptNum operator -(   const CScriptNum& rhs) const { return operator -(rhs.m_value);   }

    inline CScriptNum& operator+=( const CScriptNum& rhs) { return operator+= (rhs.m_value); }
inline CScriptNum& operator-=( const CScriptNum& rhs) { return operator-= (rhs.m_value); }

inline CScriptNum operator &(   const int64_t& rhs)    const { return CScriptNum(m_value & rhs);}
    inline CScriptNum operator &(   const CScriptNum& rhs) const { return operator &(rhs.m_value);   }

    inline CScriptNum& operator&=( const CScriptNum& rhs) { return operator&= (rhs.m_value); }

inline CScriptNum operator -()                         const
    {
        assert(m_value != std::numeric_limits<int64_t>::min());
        return CScriptNum(-m_value);
    }

    inline CScriptNum& operator=( const int64_t& rhs)
{
    m_value = rhs;
    return *this;
}

inline CScriptNum& operator+=( const int64_t& rhs)
{
    assert(rhs == 0 || (rhs > 0 && m_value <= std::numeric_limits < int64_t >::max() - rhs) ||
                       (rhs < 0 && m_value >= std::numeric_limits < int64_t >::min() - rhs));
    m_value += rhs;
    return *this;
}

inline CScriptNum& operator-=( const int64_t& rhs)
{
    assert(rhs == 0 || (rhs > 0 && m_value >= std::numeric_limits < int64_t >::min() + rhs) ||
                       (rhs < 0 && m_value <= std::numeric_limits < int64_t >::max() + rhs));
    m_value -= rhs;
    return *this;
}

inline CScriptNum& operator&=( const int64_t& rhs)
{
    m_value &= rhs;
    return *this;
}*/
        #endregion

    }
}

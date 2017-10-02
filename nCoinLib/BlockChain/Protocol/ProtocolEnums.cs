using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCoinLib.BlockChain
{
    /// <summary>
    /// Script opcodes 
    /// </summary>
    public enum opcodetype : byte
    {
        // push value
        OP_0 = 0x00,
        OP_FALSE = OP_0,
        OP_PUSHDATA1 = 0x4c,
        OP_PUSHDATA2 = 0x4d,
        OP_PUSHDATA4 = 0x4e,
        OP_1NEGATE = 0x4f,
        OP_RESERVED = 0x50,
        OP_1 = 0x51,
        OP_TRUE = OP_1,
        OP_2 = 0x52,
        OP_3 = 0x53,
        OP_4 = 0x54,
        OP_5 = 0x55,
        OP_6 = 0x56,
        OP_7 = 0x57,
        OP_8 = 0x58,
        OP_9 = 0x59,
        OP_10 = 0x5a,
        OP_11 = 0x5b,
        OP_12 = 0x5c,
        OP_13 = 0x5d,
        OP_14 = 0x5e,
        OP_15 = 0x5f,
        OP_16 = 0x60,

        // control
        OP_NOP = 0x61,
        OP_VER = 0x62,
        OP_IF = 0x63,
        OP_NOTIF = 0x64,
        OP_VERIF = 0x65,
        OP_VERNOTIF = 0x66,
        OP_ELSE = 0x67,
        OP_ENDIF = 0x68,
        OP_VERIFY = 0x69,
        OP_RETURN = 0x6a,

        // stack ops
        OP_TOALTSTACK = 0x6b,
        OP_FROMALTSTACK = 0x6c,
        OP_2DROP = 0x6d,
        OP_2DUP = 0x6e,
        OP_3DUP = 0x6f,
        OP_2OVER = 0x70,
        OP_2ROT = 0x71,
        OP_2SWAP = 0x72,
        OP_IFDUP = 0x73,
        OP_DEPTH = 0x74,
        OP_DROP = 0x75,
        OP_DUP = 0x76,
        OP_NIP = 0x77,
        OP_OVER = 0x78,
        OP_PICK = 0x79,
        OP_ROLL = 0x7a,
        OP_ROT = 0x7b,
        OP_SWAP = 0x7c,
        OP_TUCK = 0x7d,

        // splice ops
        OP_CAT = 0x7e,
        OP_SUBSTR = 0x7f,
        OP_LEFT = 0x80,
        OP_RIGHT = 0x81,
        OP_SIZE = 0x82,

        // bit logic
        OP_INVERT = 0x83,
        OP_AND = 0x84,
        OP_OR = 0x85,
        OP_XOR = 0x86,
        OP_EQUAL = 0x87,
        OP_EQUALVERIFY = 0x88,
        OP_RESERVED1 = 0x89,
        OP_RESERVED2 = 0x8a,

        // numeric
        OP_1ADD = 0x8b,
        OP_1SUB = 0x8c,
        OP_2MUL = 0x8d,
        OP_2DIV = 0x8e,
        OP_NEGATE = 0x8f,
        OP_ABS = 0x90,
        OP_NOT = 0x91,
        OP_0NOTEQUAL = 0x92,

        OP_ADD = 0x93,
        OP_SUB = 0x94,
        OP_MUL = 0x95,
        OP_DIV = 0x96,
        OP_MOD = 0x97,
        OP_LSHIFT = 0x98,
        OP_RSHIFT = 0x99,

        OP_BOOLAND = 0x9a,
        OP_BOOLOR = 0x9b,
        OP_NUMEQUAL = 0x9c,
        OP_NUMEQUALVERIFY = 0x9d,
        OP_NUMNOTEQUAL = 0x9e,
        OP_LESSTHAN = 0x9f,
        OP_GREATERTHAN = 0xa0,
        OP_LESSTHANOREQUAL = 0xa1,
        OP_GREATERTHANOREQUAL = 0xa2,
        OP_MIN = 0xa3,
        OP_MAX = 0xa4,

        OP_WITHIN = 0xa5,

        // crypto
        OP_RIPEMD160 = 0xa6,
        OP_SHA1 = 0xa7,
        OP_SHA256 = 0xa8,
        OP_HASH160 = 0xa9,
        OP_HASH256 = 0xaa,
        OP_CODESEPARATOR = 0xab,
        OP_CHECKSIG = 0xac,
        OP_CHECKSIGVERIFY = 0xad,
        OP_CHECKMULTISIG = 0xae,
        OP_CHECKMULTISIGVERIFY = 0xaf,

        // expansion
        OP_NOP1 = 0xb0,
        OP_CHECKLOCKTIMEVERIFY = 0xb1,
        OP_NOP2 = OP_CHECKLOCKTIMEVERIFY,
        OP_CHECKSEQUENCEVERIFY = 0xb2,
        OP_NOP3 = OP_CHECKSEQUENCEVERIFY,
        OP_NOP4 = 0xb3,
        OP_NOP5 = 0xb4,
        OP_NOP6 = 0xb5,
        OP_NOP7 = 0xb6,
        OP_NOP8 = 0xb7,
        OP_NOP9 = 0xb8,
        OP_NOP10 = 0xb9,


        // template matching params
        OP_SMALLINTEGER = 0xfa,
        OP_PUBKEYS = 0xfb,
        OP_PUBKEYHASH = 0xfd,
        OP_PUBKEY = 0xfe,

        OP_INVALIDOPCODE = 0xff,
    };

    /// <summary>
    /// Signature hash types/flags
    /// </summary>
    public enum SigHash : uint
    {
        Undefined = 0,
        /// <summary>
        /// All outputs are signed
        /// </summary>
        All = 1,
        /// <summary>
        /// No outputs as signed
        /// </summary>
        None = 2,
        /// <summary>
        /// Only the output with the same index as this input is signed
        /// </summary>
        Single = 3,
        /// <summary>
        /// If set, no inputs, except this, are part of the signature
        /// </summary>
        AnyoneCanPay = 0x80,
    };

    /// <summary>
    /// Script verification flags
    /// </summary>
    [Flags]
    public enum ScriptVerify : uint
    {
        None = 0,

        /// <summary>
        /// Evaluate P2SH subscripts (softfork safe, BIP16).
        /// </summary>
        P2SH = (1U << 0),

        /// <summary>
        /// Passing a non-strict-DER signature or one with undefined hashtype to a checksig operation causes script failure.
        /// Passing a pubkey that is not (0x04 + 64 bytes) or (0x02 or 0x03 + 32 bytes) to checksig causes that pubkey to be
        /// +
        /// skipped (not softfork safe: this flag can widen the validity of OP_CHECKSIG OP_NOT).
        /// </summary>
        StrictEnc = (1U << 1),

        /// <summary>
        /// Passing a non-strict-DER signature to a checksig operation causes script failure (softfork safe, BIP62 rule 1)
        /// </summary>
        DerSig = (1U << 2),

        /// <summary>
        /// Passing a non-strict-DER signature or one with S > order/2 to a checksig operation causes script failure
        /// (softfork safe, BIP62 rule 5).
        /// </summary>
        LowS = (1U << 3),

        /// <summary>
        /// verify dummy stack item consumed by CHECKMULTISIG is of zero-length (softfork safe, BIP62 rule 7).
        /// </summary>
        NullDummy = (1U << 4),

        /// <summary>
        /// Using a non-push operator in the scriptSig causes script failure (softfork safe, BIP62 rule 2).
        /// </summary>
        SigPushOnly = (1U << 5),

        /// <summary>
        /// Require minimal encodings for all push operations (OP_0... OP_16, OP_1NEGATE where possible, direct
        /// pushes up to 75 bytes, OP_PUSHDATA up to 255 bytes, OP_PUSHDATA2 for anything larger). Evaluating
        /// any other push causes the script to fail (BIP62 rule 3).
        /// In addition, whenever a stack element is interpreted as a number, it must be of minimal length (BIP62 rule 4).
        /// (softfork safe)
        /// </summary>
        MinimalData = (1U << 6),

        /// <summary>
        /// Discourage use of NOPs reserved for upgrades (NOP1-10)
        ///
        /// Provided so that nodes can avoid accepting or mining transactions
        /// containing executed NOP's whose meaning may change after a soft-fork,
        /// thus rendering the script invalid; with this flag set executing
        /// discouraged NOPs fails the script. This verification flag will never be
        /// a mandatory flag applied to scripts in a block. NOPs that are not
        /// executed, e.g.  within an unexecuted IF ENDIF block, are *not* rejected.
        /// </summary>
        DiscourageUpgradableNops = (1U << 7),

        /// <summary>
        /// Require that only a single stack element remains after evaluation. This changes the success criterion from
        /// "At least one stack element must remain, and when interpreted as a boolean, it must be true" to
        /// "Exactly one stack element must remain, and when interpreted as a boolean, it must be true".
        /// (softfork safe, BIP62 rule 6)
        /// Note: CLEANSTACK should never be used without P2SH.
        /// </summary>
        CleanStack = (1U << 8),

        /// <summary>
        /// Verify CHECKLOCKTIMEVERIFY
        ///
        /// See BIP65 for details.
        /// </summary>
        CheckLockTimeVerify = (1U << 9),

        /// <summary>
        /// See BIP68 for details.
        /// </summary>
        CheckSequenceVerify = (1U << 10),

        /// <summary>
        /// Support segregated witness
        /// </summary>
        Witness = (1U << 11),

        /// <summary>
        /// Making v2-v16 witness program non-standard
        /// </summary>
        DiscourageUpgradableWitnessProgram = (1U << 12),

        /// <summary>
        /// Segwit script only: Require the argument of OP_IF/NOTIF to be exactly 0x01 or empty vector
        /// </summary>
        MinimalIf = (1U << 13),

        /// <summary>
        /// Signature(s) must be empty vector if an CHECK(MULTI)SIG operation failed
        /// </summary>
        NullFail = (1U << 14),

        /// <summary>
        /// Public keys in segregated witness scripts must be compressed
        /// </summary>
        WitnessPubkeyType = (1U << 15),

        /// <summary>
        /// Mandatory script verification flags that all new blocks must comply with for
        /// them to be valid. (but old blocks may not comply with) Currently just P2SH,
        /// but in the future other flags may be added, such as a soft-fork to enforce
        /// strict DER encoding.
        /// 
        /// Failing one of these tests may trigger a DoS ban - see CheckInputs() for
        /// details.
        /// </summary>
        Mandatory = P2SH,

        /// <summary>
        /// Standard script verification flags that standard transactions will comply
        /// with. However scripts violating these flags may still be present in valid
        /// blocks and we must accept those blocks.
        /// </summary>
        Standard =
              Mandatory
            | DerSig
            | StrictEnc
            | MinimalData
            | NullDummy
            | DiscourageUpgradableNops
            | CleanStack
            | CheckLockTimeVerify
            | CheckSequenceVerify
            | LowS
            | Witness
            | DiscourageUpgradableWitnessProgram
            | NullFail
            | MinimalIf
    }

    public enum HashVersion
    {
        Original = 0,
        Witness = 1
    }
}

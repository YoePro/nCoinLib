// RPC Commands and helps
// https://en.bitcoin.it/wiki/Original_Bitcoin_client/API_calls_list

namespace nCoinLib.RPC
{
    class RPCMethods
    {
        public enum rpcMethods
        {
            addmultisigaddress, // <nrequired> <'["key","key"]'> [account]
            addnode, // <node> <add|remove|onetry>
            addredeemscript, // <redeemScript> [account]
            backupwallet, // <destination>
            checkkernel, // [{"txid":txid,"vout":n},...] [createblocktemplate=false]
            checkwallet,
            createrawtransaction, //[{"txid":txid, "vout":n},...] {address:amount,...}
            darksend, //<Lindaaddress> <amount>
            decoderawtransaction, //<hex string>
            decodescript, //<hex string>
            dumpprivkey, // <Lindaaddress>
            dumpwallet, // <filename>
            encryptwallet, //<passphrase>
            getaccount,//  <Lindaaddress>
            getaccountaddress, //<account>
            getaddednodeinfo, // <dns> [node]
            getaddressesbyaccount, // <account>
            getbalance, // [account] [minconf=1]
            getbestblockhash, //
            getblock, // <hash> [txinfo]
            getblockbynumber, // <number> [txinfo]
            getblockcount,
            getblockhash, //<index>
            getblocktemplate, // [params]
            getcheckpoint,
            getconnectioncount,
            getdifficulty,
            getinfo,
            getmininginfo,
            getnettotals,
            getnewaddress, // [account]
            getnewpubkey, // [account]
            getnewstealthaddress, // [label]
            getpeerinfo,
            getrawmempool,
            getrawtransaction, // <txid> [verbose=0]
            getreceivedbyaccount, // <account> [minconf=1]
            getreceivedbyaddress, // <Lindaaddress> [minconf=1]
            getstakesubsidy, // <hex string>
            getstakinginfo,
            getsubsidy, //[nTarget]
            gettransaction, // <txid>
            getwork, //[data]
            getworkex, // [data, coinbase]
            help, // [command]
            importprivkey, // <Lindaprivkey> [label] [rescan=true]
            importstealthaddress, // <scan_secret> <spend_secret> [label]
            importwallet, // <filename>
            keepass, // <genkey|init|setpassphrase>
            keypoolrefill, //[new- size]
            listaccounts, // [minconf=1]
            listaddressgroupings,
            listreceivedbyaccount, // [minconf=1] [includeempty=false]
            listreceivedbyaddress, // [minconf=1] [includeempty=false]
            listsinceblock, // [blockhash] [target-confirmations]
            liststealthaddresses, // [show_secrets=0]
            listtransactions, // [account] [count=10] [from=0]
            listunspent, // [minconf=1] [maxconf=9999999] ["address",...]
            makekeypair, // [prefix]
            masternode, // <start|start-alias|start-many|stop|stop-alias|stop-many|list|list-conf|count|debug|current|winners|genkey|enforce|outputs> [passphrase]
            move, // <fromaccount> <toaccount> <amount> [minconf=1] [comment]
            ping,
            repairwallet,
            resendtx,
            reservebalance, // [<reserve> [amount]]
            searchrawtransactions, //<address>[verbose = 1][skip = 0][count = 100]
            sendalert, //<message> <privatekey> <minver> <maxver> <priority> <id> [cancelupto]
            sendfrom, // <fromaccount> <toLindaaddress> <amount> [minconf=1] [comment] [comment-to] [narration]
            sendmany, // <fromaccount> { address: amount,...}
                      //[minconf=1]
                      //[comment]
            sendrawtransaction, //<hex string>
            sendtoaddress, //<Lindaaddress> <amount> [comment] [comment-to] [narration]
            sendtostealthaddress, // <stealth_address> <amount> [comment] [comment-to] [narration]
            setaccount, // <Lindaaddress> <account>
            settxfee, //<amount>
            signmessage, // <Lindaaddress> <message>
            signrawtransaction, //<hex string>[{ "txid":txid,"vout":n,"scriptPubKey":hex,"redeemScript":hex},...] [<privatekey1>,...]
                                // [sighashtype="ALL"]
            spork, //<name>[< value >]
            stop,
            submitblock, //<hex data>[optional -params-obj]
            validateaddress, //<Lindaaddress>
            validatepubkey, //<Lindapubkey>
            verifymessage // <Lindaaddress> <signature> <message>
        }

        public string GetHelp()
        {
            string gh = "addmultisigaddress<nrequired> < '[\"key\",\"key\"]' > [account]\n" +
                        "addnode<node> < add | remove | onetry >\n" +
                        "addredeemscript<redeemScript>[account]\n" +
                        "backupwallet<destination>\n" +
                        "checkkernel[{ \"txid\":txid,\"vout\":n},...] [createblocktemplate=false]\n" +
                        "checkwallet\n" +
                        "createrawtransaction[{\"txid\":txid, \"vout\":n},...] {address:amount,...}\n" +
                        "darksend<Lindaaddress> < amount >\n" +
                        "decoderawtransaction < hex string>\n" +
                        "decodescript < hex string>\n" +
                        "dumpprivkey < Lindaaddress >\n" +
                        "dumpwallet < filename > \n" +
                        "encryptwallet < passphrase > \n" +
                        "getaccount < Lindaaddress >\n" +
                        "getaccountaddress < account >\n" +
                        "getaddednodeinfo<dns>[node]\n" +
                        "getaddressesbyaccount<account>\n" +
                        "getbalance[account] [minconf=1]\n" +
                        "getbestblockhash\n" +
                        "getblock<hash> [txinfo]\n" +
                        "getblockbynumber <number> [txinfo]\n" +
                        "getblockcount\n" +
                        "getblockhash<index>\n" +
                        "getblocktemplate [params]\n" +
                        "getcheckpoint\n" +
                        "getconnectioncount\n" +
                        "getdifficulty\n" +
                        "getinfo\n" +
                        "getmininginfo\n" +
                        "getnettotals\n" +
                        "getnewaddress[account]\n" +
                        "getnewpubkey [account]\n" +
                        "getnewstealthaddress [label]\n" +
                        "getpeerinfo\n" +
                        "getrawmempool\n" +
                        "getrawtransaction <txid> [verbose=0]\n" +
                        "getreceivedbyaccount <account> [minconf=1]\n" +
                        "getreceivedbyaddress <Lindaaddress> [minconf=1]\n" +
                        "getstakesubsidy <hex string>\n" +
                        "getstakinginfo\n" +
                        "getsubsidy[nTarget]\n" +
                        "gettransaction <txid>\n" +
                        "getwork[data]\n" +
                        "getworkex [data, coinbase]\n" +
                        "help [command]\n" +
                        "importprivkey <Lindaprivkey> [label] [rescan=true]\n" +
                        "importstealthaddress <scan_secret> <spend_secret> [label]\n" +
                        "importwallet <filename>\n" +
                        "keepass <genkey|init|setpassphrase>\n" +
                        "keypoolrefill[new- size]\n" +
                        "listaccounts [minconf=1]\n" +
                        "listaddressgroupings\n" +
                        "listreceivedbyaccount [minconf=1] [includeempty=false]\n" +
                        "listreceivedbyaddress [minconf=1] [includeempty=false]\n" +
                        "listsinceblock [blockhash] [target-confirmations]\n" +
                        "liststealthaddresses [show_secrets=0]\n" +
                        "listtransactions [account] [count=10] [from=0]\n" +
                        "listunspent [minconf=1] [maxconf=9999999] [\"address\",...]\n" +
                        "makekeypair [prefix]\n" +
                        "masternode <start|start-alias|start-many|stop|stop-alias|stop-many|list|list-conf|count|debug|current|winners|genkey|enforce|outputs> [passphrase]\n" +
                        "move <fromaccount> <toaccount> <amount> [minconf=1] [comment]\n" +
                        "ping\n" +
                        "repairwallet\n" +
                        "resendtx\n" +
                        "reservebalance [<reserve> [amount]]\n" +
                        "searchrawtransactions<address>[verbose = 1][skip = 0][count = 100]\n" +
                        "sendalert<message> <privatekey> <minver> <maxver> <priority> <id> [cancelupto]\n" +
                        "sendfrom <fromaccount> <toLindaaddress> <amount> [minconf=1] [comment] [comment-to] [narration]\n" +
                        "sendmany <fromaccount> { address: amount,...}\n" +
                        "[minconf=1]\n" +
                        "[comment]\n" +
                        "sendrawtransaction <hex string>\n" +
                        "sendtoaddress<Lindaaddress> <amount> [comment] [comment-to] [narration]\n" +
                        "sendtostealthaddress <stealth_address> <amount> [comment] [comment-to] [narration]\n" +
                        "setaccount <Lindaaddress> <account>\n" +
                        "settxfee<amount>\n" +
                        "signmessage <Lindaaddress> <message>\n" +
                        "signrawtransaction <hex string>[{ \"txid\":txid,\"vout\":n,\"scriptPubKey\":hex,\"redeemScript\":hex},...] [<privatekey1>,...]\n" +
                        "sighashtype=\"ALL\"\n" +
                        "spork<name>[< value >]\n" +
                        "stop\n" +
                        "submitblock<hex data>[optional -params-obj]\n" +
                        "validateaddress<Lindaaddress>\n" +
                        "validatepubkey<Lindapubkey>\n" +
                        "verifymessage <Lindaaddress> <signature> <message>\n";

            return gh;
        }
    }
}

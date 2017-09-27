using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
      Category			Name						 Implemented 
      ------------------ --------------------------- -----------------------      
      
      Overall control/query calls 
      control			getinfo
      control			help
      control			stop

      P2P networking
      network			getnetworkinfo
      network			addnode					  Yes
      network			disconnectnode
      network			getaddednodeinfo			 Yes
      network			getconnectioncount
      network			getnettotals
      network			getpeerinfo				  Yes
      network			ping
      network			setban
      network			listbanned
      network			clearbanned
      ------------------ Block chain and UTXO
      blockchain		 getblockchaininfo
      blockchain		 getbestblockhash			 Yes
      blockchain		 getblockcount				Yes
      blockchain		 getblock					 Yes
      blockchain		 getblockhash				 Yes
      blockchain		 getchaintips
      blockchain		 getdifficulty
      blockchain		 getmempoolinfo
      blockchain		 getrawmempool				Yes
      blockchain		 gettxout
      blockchain		 gettxoutproof
      blockchain		 verifytxoutproof
      blockchain		 gettxoutsetinfo
      blockchain		 verifychain
      ------------------ Mining
      mining			 getblocktemplate
      mining			 getmininginfo
      mining			 getnetworkhashps
      mining			 prioritisetransaction
      mining			 submitblock
      ------------------ Coin generation
      generating		 getgenerate
      generating		 setgenerate
      generating		 generate
      ------------------ Raw transactions
      rawtransactions	createrawtransaction
      rawtransactions	decoderawtransaction
      rawtransactions	decodescript
      rawtransactions	getrawtransaction
      rawtransactions	sendrawtransaction
      rawtransactions	signrawtransaction
      rawtransactions	fundrawtransaction
      ------------------ Utility functions
      util			   createmultisig
      util			   validateaddress
      util			   verifymessage
      util			   estimatefee				  Yes
      util			   estimatepriority			 Yes
      ------------------ Not shown in help
      hidden			 invalidateblock
      hidden			 reconsiderblock
      hidden			 setmocktime
      hidden			 resendwallettransactions
      ------------------ Wallet
      wallet			 addmultisigaddress
      wallet			 backupwallet				 Yes
      wallet			 dumpprivkey				  Yes
      wallet			 dumpwallet
      wallet			 encryptwallet
      wallet			 getaccountaddress			Yes
      wallet			 getaccount
      wallet			 getaddressesbyaccount
      wallet			 getbalance
      wallet			 getnewaddress
      wallet			 getrawchangeaddress
      wallet			 getreceivedbyaccount
      wallet			 getreceivedbyaddress
      wallet			 gettransaction
      wallet			 getunconfirmedbalance
      wallet			 getwalletinfo
      wallet			 importprivkey				Yes
      wallet			 importwallet
      wallet			 importaddress				Yes
      wallet			 keypoolrefill
      wallet			 listaccounts				 Yes
      wallet			 listaddressgroupings		 Yes
      wallet			 listlockunspent
      wallet			 listreceivedbyaccount
      wallet			 listreceivedbyaddress
      wallet			 listsinceblock
      wallet			 listtransactions
      wallet			 listunspent				  Yes
      wallet			 lockunspent				  Yes
      wallet			 move
      wallet			 sendfrom
      wallet			 sendmany
      wallet			 sendtoaddress
      wallet			 setaccount
      wallet			 settxfee
      wallet			 signmessage
      wallet			 walletlock
      wallet			 walletpassphrasechange
      wallet			 walletpassphrase			yes

                         masternode
  */
namespace nCoinLib.RPC
{
    class Response
    {
    }
}

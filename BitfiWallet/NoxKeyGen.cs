using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using EthereumLibrary.Signer;
using EthereumLibrary.Hex.HexConvertors.Extensions;
using NeoGasLibrary;
using NBitcoin;
namespace NoxKeys
{
    internal class NoxKeyGen : IDisposable
    {
        private ExtKey GetExtKey(int[] SecretSalt, int[] SecretPW)
        {
            byte[] ssalt = new byte[SecretSalt.Length];
            for (int i = 0; i \            {
                 ssalt[i] = Sclear.SCharImage(SecretSalt[i]).SBYTE;
                 SecretSalt[i] = -1;
            }
            byte[] spw = new byte[SecretPW.Length];
            for (int i = 0; i \            {
                spw[i] = Sclear.SCharImage(SecretPW[i]).SBYTE;
                 SecretPW[i] = -1;
            }
            byte[] der = NBitcoin.Crypto.SCrypt.ComputeDerivedKey(spw, ssalt, 32768, 8, 4, 4, 64);
            if (der == null)
            {
                throw new Exception("Error, invalid info.");
            }
            if (!string.IsNullOrEmpty(Sclear.LastHashAtp))
            {
                using (var Tshacrypt = new System.Security.Cryptography.SHA256Managed())
                {
                    byte[] Thash = Tshacrypt.ComputeHash(der);
                    string tval = Convert.ToBase64String(Thash);
                    if (tval != Sclear.LastHashAtp)
                    {
                        throw new Exception("Information does not match.");
                    }
                    Sclear.LastHashAtp = null;
                }
            }
            using (var shacrypt = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] hash = shacrypt.ComputeHash(ssalt);
                ExtKey masterKey = new ExtKey(der, hash);
                Sclear.Clear(SecretSalt);
                Sclear.Clear(SecretPW);
                Sclear.EraseBytes(ssalt);
                Sclear.EraseBytes(spw);
                Sclear.EraseBytes(der);
                Sclear.Clear(hash);
                return masterKey;
            }
        }

        internal string GetTestHash(int[] SecretSalt, int[] SecretPW)
        {
            byte[] ssalt = new byte[SecretSalt.Length];
            for (int i = 0; i \            {
                ssalt[i] = Sclear.SCharImage(SecretSalt[i]).SBYTE;
            }
            byte[] spw = new byte[SecretPW.Length];
            for (int i = 0; i \            {

                spw[i] = Sclear.SCharImage(SecretPW[i]).SBYTE;
            }
            byte[] der = NBitcoin.Crypto.SCrypt.ComputeDerivedKey(spw, ssalt, 32768, 8, 4, 4, 64);
            if (der == null)
            {
                throw new Exception("Error, invalid info.");
            }
            using (var shacrypt = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] hash = shacrypt.ComputeHash(der);
                string tval = Convert.ToBase64String(hash);
                Sclear.EraseBytes(ssalt);
                Sclear.EraseBytes(spw);
                Sclear.EraseBytes(der);
                Sclear.EraseBytes(hash);
                return tval;
            }
        }
        public void Dispose()
        {
            GC.Collect(0);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        private int GetCurrencyIndex(string currencySymbol)
        {

            string currencyIndexString = "";
            foreach (char c in currencySymbol)
            {
                currencyIndexString = currencyIndexString + (char.ToUpper(c) - 64).ToString();
            }
            return Convert.ToInt32(currencyIndexString);
        }
        private byte[] GetHDPrivateKey(ExtKey masterKey, int hdIndex, int currencyIndex)
        {
            try
            {
                masterKey = masterKey.Derive(currencyIndex, hardened: true);
                ExtKey key = masterKey.Derive((uint)hdIndex);
                return key.PrivateKey.ToBytes();
            }
            catch
            {
                return null;
            }
        }
        private byte[] HDDerriveKey(int[] SecretSalt, int[] SecretPW, int hdIndex, string currencySymbol)
        {
            try
            {
                return GetHDPrivateKey(GetExtKey(SecretSalt, SecretPW), hdIndex, GetCurrencyIndex(currencySymbol));
            }
            catch
            {
                return null;
            }
        }
        internal MsgTaskTransferResponse SignUserMsg(int[] SecretSalt, int[] SecretPW, BitfiWallet.NOXWS.NoxMsgProcess req)
        {
            MsgTaskTransferResponse msgTaskTransferResponse = new MsgTaskTransferResponse();

            try
            {
                byte[] Msg = Convert.FromBase64String(req.Msg);
                byte[] keybytes = HDDerriveKey(SecretSalt, SecretPW, req.NoxAddress.HDIndex, req.BlkNet);
                if (req.BlkNet == "eth")
                {
                    EthECKey ekey = new EthECKey(keybytes, true);

                    string ethadr = ekey.GetPublicAddress();
                    if (ethadr != req.NoxAddress.BTCAddress)
                    {
                        msgTaskTransferResponse.Error = "Invalid information.";

                    }
                    else
                    {
                        EthereumLibrary.MsgSigning elib = new EthereumLibrary.MsgSigning();
                        msgTaskTransferResponse.MsgSig = elib.ETHMsgSign(Msg, ekey);
                    }
                }
                else
                {
                    BitfiWallet.AltCoinGen altCoinGen = new BitfiWallet.AltCoinGen(GetBLKNetworkAlt(req.BlkNet));
                    msgTaskTransferResponse.MsgSig = altCoinGen.AltMsgSign(keybytes, req.NoxAddress.BTCAddress, System.Text.Encoding.UTF8.GetString(Msg));
                }
                Sclear.EraseBytes(keybytes);
                return msgTaskTransferResponse;
            }
            catch(Exception ex)
            {
                msgTaskTransferResponse.Error = ex.Message;
                return msgTaskTransferResponse;
            }
        }
        internal string GetNewAddress(int[] SecretSalt, int[] SecretPW, int hdIndex, string currencySymbol, string firstaddress, bool DoSegwit)
        {
            try
            {
                ExtKey masterKey = GetExtKey(SecretSalt, SecretPW);
                ExtKey masterKeyD = masterKey.Derive(GetCurrencyIndex("btc"), hardened: true);
                ExtKey key = masterKeyD.Derive((uint)0);
                byte[] keybytes = key.PrivateKey.ToBytes();
                Key pkey = new Key(keybytes, -1, false);
                var address = pkey.PubKey.GetAddress(GetBLKNetworkAlt("btc")).ToString();
                if (address != firstaddress)
                {
                    Sclear.EraseBytes(keybytes);
                    masterKey = null;
                    masterKeyD = null;
                    key = null;
                    pkey = null;
                    return null;
                }
                ExtKey masterKeyA = masterKey.Derive(GetCurrencyIndex(currencySymbol), hardened: true);
                key = masterKeyA.Derive((uint)hdIndex);
                keybytes = key.PrivateKey.ToBytes();
                if (GetBLKNetworkAlt(currencySymbol) != null)
                {

                    BitfiWallet.AltCoinGen altCoinGen = new BitfiWallet.AltCoinGen(GetBLKNetworkAlt(currencySymbol));
                    address = altCoinGen.GetNewAddress(keybytes, DoSegwit);
                }
                else
                {
                    if (currencySymbol == "apl")
                    {
                        APLGen aPL = new APLGen();
                        address = aPL.GetPublicKey(keybytes);
                    }

                    if (currencySymbol == "xrp" && hdIndex == 0)
                    {
                        RipGen ripGen = new RipGen();
                        address = ripGen.GetAddress(keybytes);

                    }
                }
                Sclear.EraseBytes(keybytes);
                masterKey = null;
                masterKeyD = null;
                masterKeyA = null;
                key = null;
                pkey = null;
                return address;
            }
            catch
            {
                return null;
            }
        }
        internal string SignSGAMessage(int[] SecretSalt, int[] SecretPW, string sgaMessage)
        {
            try
            {
                ExtKey masterKey = GetExtKey(SecretSalt, SecretPW);

                ExtKey masterKeyD = masterKey.Derive(GetCurrencyIndex("btc"), hardened: true);
                ExtKey key = masterKeyD.Derive((uint)0);
                byte[] keybytes = key.PrivateKey.ToBytes();
                Key pkey = new Key(keybytes, -1, false);
                BitcoinSecret bitcoinSecret = new BitcoinSecret(pkey, Network.Main);
                var address = bitcoinSecret.PubKey.GetAddress(GetBLKNetworkAlt("btc")).ToString();
                var signature = bitcoinSecret.PrivateKey.SignMessage(sgaMessage);
                Sclear.EraseBytes(keybytes);

                masterKeyD = null;
                key = null;
                pkey = null;
                NWS WS = new NWS();
                return WS.GetSGAToken(address, signature);

            }
            catch
            {

                return "4";
            }
        }
        internal string SignSGAMessageWithCode(int[] SecretSalt, int[] SecretPW, string sgaMessage, string DisplayCode)
        {
            try
            {
                ExtKey masterKey = GetExtKey(SecretSalt, SecretPW);

                ExtKey masterKeyD = masterKey.Derive(GetCurrencyIndex("btc"), hardened: true);
                ExtKey key = masterKeyD.Derive((uint)0);
                byte[] keybytes = key.PrivateKey.ToBytes();
                Key pkey = new Key(keybytes, -1, false);
                BitcoinSecret bitcoinSecret = new BitcoinSecret(pkey, Network.Main);
                var address = bitcoinSecret.PubKey.GetAddress(GetBLKNetworkAlt("btc")).ToString();
                var signature = bitcoinSecret.PrivateKey.SignMessage(sgaMessage);
                Sclear.EraseBytes(keybytes);

                masterKeyD = null;
                key = null;
                pkey = null;
                NWS WS = new NWS();
                return WS.GetSGATokenForSignin(address, signature, DisplayCode);
            }
            catch
            {

                return "4";
            }
        }
        internal string[] SignSGAMessageOut(int[] SecretSalt, int[] SecretPW, string sgaMessage)
        {
            try
            {
                ExtKey masterKey = GetExtKey(SecretSalt, SecretPW);

                ExtKey masterKeyD = masterKey.Derive(GetCurrencyIndex("btc"), hardened: true);
                ExtKey key = masterKeyD.Derive((uint)0);
                byte[] keybytes = key.PrivateKey.ToBytes();

                Key pkey = new Key(keybytes, -1, false);
                BitcoinSecret bitcoinSecret = new BitcoinSecret(pkey, Network.Main);
                var address = bitcoinSecret.PubKey.GetAddress(GetBLKNetworkAlt("btc")).ToString();
                var signature = bitcoinSecret.PrivateKey.SignMessage(sgaMessage);
                Sclear.EraseBytes(keybytes);

                masterKeyD = null;

                key = null;
                pkey = null;
                NWS WS = new NWS();
                List\\ lnoxAddresses = new List\\();
                string adrlist = JsonConvert.SerializeObject(lnoxAddresses);
                string tkmsg = WS.GetSGAToken(address, signature);
                if (tkmsg.Length \                {
                    return new string[2] { tkmsg, adrlist };
                }

                var objaddrListindexes = WS.GetAddressIndexes(tkmsg);
                if (objaddrListindexes != null)
                {
                    lnoxAddresses = GetReviewIndexes(objaddrListindexes, masterKey);
                    adrlist = JsonConvert.SerializeObject(lnoxAddresses);
                }
                else
                {
                    tkmsg = "1";
                }
                return new string[2] { tkmsg, adrlist };
            }
            catch
            {
                return new string[2] { "4", "" };
            }
        }

        public Network GetBLKNetworkAlt(string blk)
        {
            Network net = null;
            switch (blk)
            {
                case "btc":
                    net = Network.Main;
                    break;
                case "ltc":
                    net = NBitcoin.Altcoins.Litecoin.Instance.Mainnet;
                    break;
                case "grs":
                    net = NBitcoin.Altcoins.Groestlcoin.Instance.Mainnet;
                    break;
                case "ftc":
                    net = NBitcoin.Altcoins.Feathercoin.Instance.Mainnet;
                    break;
                case "via":
                    net = NBitcoin.Altcoins.Viacoin.Instance.Mainnet;
                    break;
                case "doge":
                    net = NBitcoin.Altcoins.Dogecoin.Instance.Mainnet; //no seg
                    break;
                case "btg":
                    net = NBitcoin.Altcoins.BGold.Instance.Mainnet;
                    break;
                case "mona":
                    net = NBitcoin.Altcoins.Monacoin.Instance.Mainnet;
                    break;
                case "dash":
                    net = NBitcoin.Altcoins.Dash.Instance.Mainnet; //no seg
                    break;
                case "zcl":
                    net = NBitcoin.Altcoins.Zclassic.Instance.Mainnet; //no seg
                    break;
                case "strat":
                    net = NBitcoin.Altcoins.Stratis.Instance.Mainnet;
                    break;
                case "dgb":
                    net = NBitcoin.Altcoins.DigiByte.Instance.Mainnet;
                    break;
            }
            return net;
        }
        private string GetFirstBTC(ExtKey masterKey)
        {
            ExtKey masterKeyA = masterKey.Derive(GetCurrencyIndex("btc"), hardened: true);
            ExtKey key = masterKeyA.Derive((uint)0);
            byte[] keybytes = key.PrivateKey.ToBytes();
            Key pkey = new Key(keybytes, -1, false);
            var resp = pkey.PubKey.GetAddress(GetBLKNetworkAlt("btc")).ToString();
            Sclear.EraseBytes(keybytes);
            masterKeyA = null;
            key = null;
            pkey = null;
            return resp;
        }
        private string GetFirstLTC(ExtKey masterKey)
        {
            ExtKey masterKeyA = masterKey.Derive(GetCurrencyIndex("ltc"), hardened: true);
            ExtKey key = masterKeyA.Derive((uint)0);
            byte[] keybytes = key.PrivateKey.ToBytes();
            Key pkey = new Key(keybytes, -1, false);
            var resp = pkey.PubKey.GetAddress(GetBLKNetworkAlt("ltc")).ToString();
            Sclear.EraseBytes(keybytes);
            masterKeyA = null;
            key = null;
            pkey = null;
            return resp;
        }
        private string GetFirstETH(ExtKey masterKey)
        {
            ExtKey masterKeyA = masterKey.Derive(GetCurrencyIndex("eth"), hardened: true);
            ExtKey key = masterKeyA.Derive((uint)0);
            byte[] keybytes = key.PrivateKey.ToBytes();
            EthECKey ETHkey = new EthECKey(keybytes, true);
            string ethadr = ETHkey.GetPublicAddress();
            Sclear.EraseBytes(keybytes);
            masterKeyA = null;
            key = null;
            ETHkey = null;
            return ethadr;
        }
        private string[] GetFirstXMR(ExtKey masterKey)
        {
            ExtKey masterKeyA = masterKey.Derive(GetCurrencyIndex("xmr"), hardened: true);
            ExtKey key = masterKeyA.Derive((uint)0);
            byte[] keybytes = key.PrivateKey.ToBytes();
            MoneroWallet.Wallet wallet = MoneroWallet.Wallet.OpenWallet(keybytes);
            string[] moneroadr = { wallet.Address, MoneroWallet.Converters.ByteArrayToHex(wallet.Keys.ViewSecret) };
            Sclear.EraseBytes(keybytes);
            masterKeyA = null;
            key = null;
            wallet.Dispose();

            return moneroadr;
        }
        private string GetFirstNEO(ExtKey masterKey)
        {
            ExtKey masterKeyA = masterKey.Derive(GetCurrencyIndex("neo"), hardened: true);
            ExtKey key = masterKeyA.Derive((uint)0);
            byte[] keybytes = key.PrivateKey.ToBytes();
            var NEOkeypair = new KeyPair(keybytes);
            string neoaddress = NEOkeypair.address;
            Sclear.EraseBytes(keybytes);
            masterKeyA = null;
            key = null;
            NEOkeypair.Dispose();
            return neoaddress;
        }
        internal List\\ GetReviewIndexes(BitfiWallet.SGADWS.NoxAddressReviewV3 noxAddressReviews, ExtKey masterKey)
        {
            List\\ noxAddresses = new List\\();

            foreach (var adrReview in noxAddressReviews.AdrReview)
            {
                string currencySymbol = adrReview.Blk;
                ExtKey ASmasterKey = masterKey.Derive(GetCurrencyIndex(currencySymbol), hardened: true);


                for (int i = 0; i \                {

                    string address = "";
                    ExtKey key = ASmasterKey.Derive((uint)i);
                    byte[] keybytes = key.PrivateKey.ToBytes();

                    if (GetBLKNetworkAlt(adrReview.Blk) != null)
                    {

                        foreach(var nadr in noxAddressReviews.Addresses)
                        {
                            if (nadr.BlkNet == adrReview.Blk && nadr.HDIndex == i)
                            {
                                BitfiWallet.AltCoinGen altCoinGen = new BitfiWallet.AltCoinGen(GetBLKNetworkAlt(adrReview.Blk));
                                address = altCoinGen.GetNewAddress(keybytes, nadr.DoSegwit);
                                break;
                            }
                        }


                    }
                    if (adrReview.Blk == "apl")
                    {
                        APLGen aPL = new APLGen();
                        address =  aPL.GetAccountID(keybytes);
                    }
                    if (adrReview.Blk == "neo")
                    {
                         var NEOkeypair = new KeyPair(keybytes);
                        address = NEOkeypair.address;
                        NEOkeypair.Dispose();
                    }
                    if (adrReview.Blk == "xmr")
                    {
                        MoneroWallet.Wallet wallet = MoneroWallet.Wallet.OpenWallet(keybytes);
                        address = wallet.Address;
                        wallet.Dispose();

                    }
                    if (adrReview.Blk == "eth")
                    {
                        EthECKey ETHkey = new EthECKey(keybytes, true);
                        address = ETHkey.GetPublicAddress();
                    }
                    if (adrReview.Blk == "xrp")
                    {
                        RipGen ripGen = new RipGen();
                        address = ripGen.GetAddress(keybytes);
                    }
                    Sclear.EraseBytes(keybytes);
                    key = null;

                    noxAddresses.Add(new BitfiWallet.NOXWS.NoxAddresses { BlkNet = currencySymbol, BTCAddress = address}

                    );

                }

            }
            masterKey = null;

            return noxAddresses;
        }
        internal AdrCollection GetNewWalletCollection(int[] SecretSalt, int[] SecretPW)
        {
            AdrCollection adrCollection = new AdrCollection();
            ExtKey masterKey = GetExtKey(SecretSalt, SecretPW);
            adrCollection.BTC = GetFirstBTC(masterKey);
            adrCollection.LTC = GetFirstLTC(masterKey);
            adrCollection.ETH = GetFirstETH(masterKey);        
            adrCollection.NEO = GetFirstNEO(masterKey);
            adrCollection.XMR = GetFirstXMR(masterKey);
            masterKey = null;
            return adrCollection;
        }
        //TRANSACTIONS

        internal NEOTaskTransferResponse NEO_Sign(int[] SecretSalt, int[] SecretPW, string MXTxn, string BlkNet, string ToAddress, string Amount, string FromAddress)
        {
            NEOTaskTransferResponse taskTransferResponse = new NEOTaskTransferResponse();
            try
            {
                byte[] bts = Convert.FromBase64String(MXTxn);
                string jData = System.Text.Encoding.UTF8.GetString(bts);
                NEONeoscanUnspent[] unspent = JsonConvert.DeserializeObject\\(jData);
                if (unspent == null || unspent.Length \                {
                    taskTransferResponse.Error = "No unspent.";
                    return taskTransferResponse;
                }
                List\\ entries = new List\\();
                foreach (var un in unspent)
                {
                    NeoGasLibrary.NeoAPI.UnspentEntry entry = new NeoGasLibrary.NeoAPI.UnspentEntry();
                    entry.index = UInt16.Parse(un.n.ToString());
                    entry.txid = un.txid;
                    entry.value = un.value;
                    entries.Add(entry);
                }
                if (entries == null || entries.Count \                {
                    taskTransferResponse.Error = "No Entries.";
                    return taskTransferResponse;
                }
                byte[] keybytes = HDDerriveKey(SecretSalt, SecretPW, 0, "neo");
                var assetId = BlkNet.ToLower() == "gas" ? NeoGasLibrary.NeoAPI.gasId : NeoGasLibrary.NeoAPI.neoId;
                NeoGasLibrary.NeoAPI.Transaction tx = NeoGasLibrary.NeoAPI.BuildContractTx(entries, 0, FromAddress, ToAddress, Amount, assetId);
                NeoGasLibrary.KeyPair keypair = new NeoGasLibrary.KeyPair(keybytes);
                Sclear.EraseBytes(keybytes);
                string neoaddress = keypair.address;
                if (neoaddress != FromAddress)
                {
                    keypair.Dispose();
                    taskTransferResponse.Error = "Invalid information.";
                    return taskTransferResponse;
                }
                string txHex = NeoGasLibrary.NeoAPI.SignAndSerialize(tx, keypair);
                taskTransferResponse.TxnHex = txHex;
                taskTransferResponse.Error = "";
                keypair.Dispose();
                return taskTransferResponse;
            }
            catch (Exception ex)
            {
                taskTransferResponse.Error = "Error: " + ex.Message;
                return taskTransferResponse;
            }
        }
        internal ApolloTaskTransferResponse APL_Sign(int[] SecretSalt, int[] SecretPW, BitfiWallet.NOXWS.NoxTxnProcess tproc)
        {
            ApolloTaskTransferResponse taskTransferResponse = new ApolloTaskTransferResponse();
            byte[] keybytes = HDDerriveKey(SecretSalt, SecretPW, tproc.NoxAddress.HDIndex, "apl");
            APLGen aPL = new APLGen();
            string fromadr = aPL.GetPublicKey(keybytes);
            if (fromadr != tproc.NoxAddress.BTCAddress)
            {
                Sclear.EraseBytes(keybytes);
                taskTransferResponse.Error = "Invalid information.";
                return taskTransferResponse;
            }
            try
            {
                string check = aPL.CheckTransactionError(tproc.MXTxn, tproc.ToAddress, tproc.Amount, tproc.FeeValue);
                if (!string.IsNullOrEmpty(check))
                {
                    Sclear.EraseBytes(keybytes);
                    taskTransferResponse.Error = check;
                    return taskTransferResponse;
                }
                var signedtxn = aPL.SignTransaction(keybytes, tproc.MXTxn);
                Sclear.EraseBytes(keybytes);
                taskTransferResponse.TxnHex = signedtxn;
                return taskTransferResponse;
            }
            catch (Exception ex)
            {
                Sclear.EraseBytes(keybytes);
                taskTransferResponse.Error = ex.Message;
                return taskTransferResponse;
            }
        }
        internal ETHTaskTransferResponse ETH_Sign(int[] SecretSalt, int[] SecretPW, BitfiWallet.NOXWS.NoxTxnProcess tproc)
        {
            ETHTaskTransferResponse taskTransferResponse = new ETHTaskTransferResponse();
            byte[] keybytes = HDDerriveKey(SecretSalt, SecretPW, 0, "eth");
            EthECKey key = new EthECKey(keybytes, true);
            Sclear.EraseBytes(keybytes);
            string ethadr = key.GetPublicAddress();
            if (ethadr != tproc.NoxAddress.BTCAddress)
            {
                taskTransferResponse.Error = "Invalid information.";
                return taskTransferResponse;
            }
            //TO DO
            if (tproc.ETCToken != null)
            {
                var contractAddress = tproc.ETCToken.EnsureHexPrefix();
                var to = tproc.ToAddress.EnsureHexPrefix();
                var amount = NumUtils.Utils.ParseMoney(tproc.Amount, 18);
                var nonce = System.Numerics.BigInteger.Parse(tproc.ETCNonce);
                var gasPrice = NumUtils.Utils.ParseMoney(tproc.FeeValue, 18);
                var gasLimit =  new System.Numerics.BigInteger(Int64.Parse(tproc.ETCGasUsed));
                var tx = new EthereumLibrary.Signer.Transaction(contractAddress, to, amount, nonce, gasPrice, gasLimit);
                tx.Sign(key);
                var signedHex = tx.ToHex();
                key = null;
                taskTransferResponse.TxnHex = signedHex;
                return taskTransferResponse;
            }
            else
            {
                var to = tproc.ToAddress.EnsureHexPrefix();
                 var amount = NumUtils.Utils.ParseMoney(tproc.Amount, 18);
                 var nonce = System.Numerics.BigInteger.Parse(tproc.ETCNonce);
                 var gasPrice = NumUtils.Utils.ParseMoney(tproc.FeeValue, 18);
                 var gasLimit = new System.Numerics.BigInteger(Int64.Parse(tproc.ETCGasUsed));

                var tx = new EthereumLibrary.Signer.Transaction(to, amount, nonce, gasPrice, gasLimit);
                tx.Sign(key);
                var signedHex = tx.ToHex();
                key = null;
                taskTransferResponse.TxnHex = signedHex;
                return taskTransferResponse;
            }
        }
        internal AltoCoinTaskTransferResponse Alt_Sign(int[] SecretSalt, int[] SecretPW, BitfiWallet.NOXWS.NoxTxnProcess tproc, string currencySymbol)
        {
            AltoCoinTaskTransferResponse taskTransferResponse = new AltoCoinTaskTransferResponse();
            ExtKey masterKey = GetExtKey(SecretSalt, SecretPW);
            masterKey = masterKey.Derive(GetCurrencyIndex(currencySymbol), hardened: true);

            var HDIndexes = tproc.HDIndexList;
            List\\ kbList = new List\\();
            for (int i = 0; i \            {
                int HDIndex = Convert.ToInt32(HDIndexes[i]);
                ExtKey key = masterKey.Derive((uint)HDIndex);
                kbList.Add(key.PrivateKey.ToBytes());
                key = null;
            }
            masterKey = null;
            BitfiWallet.AltCoinGen altCoinGen = new BitfiWallet.AltCoinGen(GetBLKNetworkAlt(currencySymbol));
            List\\ txnRaw = new List\\();
            foreach (var txnin in tproc.UnspentList)
            {
                BCUnspent bCUnspent = new BCUnspent();
                bCUnspent.Address = txnin.Address;
                bCUnspent.OutputN = txnin.OutputN;
                bCUnspent.TxHash = txnin.TxHash;
                bCUnspent.Amount = txnin.Amount;
                txnRaw.Add(bCUnspent);
            }
            try
            {
                var txn = altCoinGen.AltCoinSign(kbList, tproc.ToAddress, txnRaw, tproc.Amount, tproc.NoxAddress.BTCAddress, tproc.FeeTotal);
                if (txn.IsError != true)
                {

                    decimal FeeUSD = tproc.USDRate * txn.Fee;
                    taskTransferResponse.TxnHex = txn.TxnHex;
                    taskTransferResponse.FeeAmount = txn.Fee.ToString();
                    if (FeeUSD \>\ .0099M)
                    {
                        taskTransferResponse.FeeAmount = taskTransferResponse.FeeAmount + "|\>\" + FeeUSD.ToString("C2");
                    }

                    if (FeeUSD \>\ 4) taskTransferResponse.FeeWarning = "HIGH FEE ALERT!";
                    if (txn.Fee \>\ (Convert.ToDecimal(tproc.Amount) * .04M))
                    {
                        decimal feePer = (txn.Fee / Convert.ToDecimal(tproc.Amount)) * 100;
                        taskTransferResponse.FeeWarning = "CHECK FEE! CHARGE IS " + feePer.ToString("N2") + "% OF PAYMENT.";
                    }
                    if (string.IsNullOrEmpty(taskTransferResponse.FeeWarning))
                    {
                        taskTransferResponse.FeeAmount = "";
                    }
                    return taskTransferResponse;
                }
                else
                {
                    taskTransferResponse.Error = txn.ErrorMessage;
                    return taskTransferResponse;
                }
            }
            catch (Exception ex)
            {
                taskTransferResponse.Error = ex.Message;
                return taskTransferResponse;
            }     
        }
        internal RipTaskTransferResponse Rip_Sign(int[] SecretSalt, int[] SecretPW, BitfiWallet.NOXWS.NoxTxnProcess tproc)
        {
            RipTaskTransferResponse taskTransferResponse = new RipTaskTransferResponse();
            byte[] keybytes = HDDerriveKey(SecretSalt, SecretPW, 0, "xrp");
            byte[] bts = Convert.FromBase64String(tproc.MXTxn);
            string MXTxn = System.Text.Encoding.UTF8.GetString(bts);
            RipGen ripGen = new RipGen();
            string fromadr = ripGen.GetAddress(keybytes);
            if (fromadr != tproc.NoxAddress.BTCAddress)
            {
                Sclear.EraseBytes(keybytes);
                taskTransferResponse.Error = "Invalid information.";
                return taskTransferResponse;
            }
            JsonSerializerSettings ser = new JsonSerializerSettings();
            ser.MissingMemberHandling = MissingMemberHandling.Ignore;
            ser.NullValueHandling = NullValueHandling.Ignore;
            ser.ObjectCreationHandling = ObjectCreationHandling.Auto;
            ser.TypeNameHandling = TypeNameHandling.All;
            var data = JsonConvert.DeserializeObject\\(MXTxn, ser);
            var chamount = Convert.ToDecimal(data.Amount) * 0.000001M;
            if (chamount \>\ Convert.ToDecimal(tproc.Amount))
            {
                Sclear.EraseBytes(keybytes);
                taskTransferResponse.Error = "Error parsing amount.";
                return taskTransferResponse;
            }
            if (data.Destination.ToUpper() != tproc.ToAddress.ToUpper())
            {
                Sclear.EraseBytes(keybytes);
                taskTransferResponse.Error = "Error parsing destination.";
                return taskTransferResponse;
            }
            try
            {
                string txn = ripGen.CreateTxn(keybytes, MXTxn);
                Sclear.EraseBytes(keybytes);
                taskTransferResponse.TxnHex = txn;
                return taskTransferResponse;
            }
            catch (Exception ex)
            {
                Sclear.EraseBytes(keybytes);
                taskTransferResponse.Error = ex.Message;
                return taskTransferResponse;
            }
        }

        //OTHER
        internal XMRTaskImageResponse XMR_GetImages(int[] SecretSalt, int[] SecretPW, BitfiWallet.NOXWS.ImageRequestTable[] requestTable)
        {
            XMRTaskImageResponse taskTransferResponse = new XMRTaskImageResponse();
            try
            {
                byte[] keybytes = HDDerriveKey(SecretSalt, SecretPW, 0, "xmr");
                var wlt = MoneroWallet.Wallet.OpenWallet(keybytes);
                BitfiWallet.XMRGen xMRGen = new BitfiWallet.XMRGen(wlt);
                var resp = xMRGen.XMR_GetImages(requestTable);
                taskTransferResponse.Error = resp.Error;
                taskTransferResponse.SpendKeyImages = resp.SpendKeyImages;
                taskTransferResponse.WalletAddress = resp.WalletAddress;
                Sclear.EraseBytes(keybytes);
                wlt.Dispose();
                return taskTransferResponse;
            }
            catch
            {
                taskTransferResponse.Error = "1";
                return taskTransferResponse;
            }
        }
        internal XMRTaskTransferResponse XMR_Sign(int[] SecretSalt, int[] SecretPW, string ToAddress, string Amount, string BaseData, string FromAddress)
        {
            XMRTaskTransferResponse taskTransferResponse = new XMRTaskTransferResponse();
            byte[] keybytes = HDDerriveKey(SecretSalt, SecretPW, 0, "xmr");
            try
            {
                byte[] bts = Convert.FromBase64String(BaseData);
                string jData = System.Text.Encoding.UTF8.GetString(bts);
                JsonSerializerSettings ser = new JsonSerializerSettings();
                ser.MissingMemberHandling = MissingMemberHandling.Ignore;
                ser.NullValueHandling = NullValueHandling.Ignore;
                ser.ObjectCreationHandling = ObjectCreationHandling.Auto;
                ser.TypeNameHandling = TypeNameHandling.All;
                var data = JsonConvert.DeserializeObject\\(jData, ser);
                var wlt = MoneroWallet.Wallet.OpenWallet(keybytes);
                Sclear.EraseBytes(keybytes);
                if (wlt.Address != FromAddress)
                {
                    wlt.Dispose();
                    taskTransferResponse.Error = "Invalid information.";
                    return taskTransferResponse;
                }
                BitfiWallet.XMRGen xMRGen = new BitfiWallet.XMRGen(wlt);
                var send = xMRGen.XMR_Sign(ToAddress, Amount, BaseData, FromAddress);
                taskTransferResponse.Error = send.Error;
                taskTransferResponse.SpendKeyImages = send.SpendKeyImages;
                taskTransferResponse.TxnHex = send.TxnHex;
                wlt.Dispose();
                if (string.IsNullOrEmpty(taskTransferResponse.Error))
                {

                }
                return taskTransferResponse;
            }
            catch (Exception ex)
            {
                taskTransferResponse.Error = ex.Message;
                return taskTransferResponse;
            }
        }
        internal NEOTaskTransferResponse NEO_ClaimGas(int[] SecretSalt, int[] SecretPW, BitfiWallet.NOXWS.NoxGasRequests req)
        {
            NEOTaskTransferResponse taskTransferResponse = new NEOTaskTransferResponse();
            byte[] bts = Convert.FromBase64String(req.NXTxn);
            string jData = System.Text.Encoding.UTF8.GetString(bts);
            NEONeoscanUnclaimed unclaimed = JsonConvert.DeserializeObject\\(jData);
            if (unclaimed == null)
            {
                taskTransferResponse.Error = "Error, no claimable values.";
                return taskTransferResponse;
            }
            if (unclaimed.claimable == null)
            {
                taskTransferResponse.Error = "Error, no claimable values.";
                return taskTransferResponse;
            }
            List\\ claims = new List\\();
            foreach (var un in unclaimed.claimable)
            {
                NeoGasLibrary.NeoAPI.ClaimEntry entry = new NeoGasLibrary.NeoAPI.ClaimEntry();
                entry.index = UInt16.Parse(un.n.ToString());
                entry.txid = un.txid;
                entry.value = un.unclaimed;
                claims.Add(entry);
            }
            if (claims.Count \            {
                taskTransferResponse.Error = "Error, no claimable values.";
                return taskTransferResponse;
            }
            byte[] keybytes = HDDerriveKey(SecretSalt, SecretPW, 0, "neo");
            var keypair = new NeoGasLibrary.KeyPair(keybytes);
            Sclear.EraseBytes(keybytes);
            string neoaddress = keypair.address;
            if (neoaddress.ToUpper() != req.NeoAddress.ToUpper())
            {
                keypair.Dispose();
                taskTransferResponse.Error = "Invalid information.";
                return taskTransferResponse;
            }
            var tx = NeoGasLibrary.NeoAPI.BuildClamTx(claims, NeoGasLibrary.NeoAPI.Net.Main, keypair.address);
            var txHex = NeoGasLibrary.NeoAPI.SignAndSerialize(tx, keypair);
            keypair.Dispose();
            taskTransferResponse.TxnHex = txHex;
            return taskTransferResponse;
        }

    }
    //[Android.Runtime.Preserve(AllMembers = true)]
    public class BCUnspent
    {
        public string Amount { get; set; }
        public string TxHash { get; set; }
        public int OutputN { get; set; }
        public string Address { get; set; }
    }
   // [Android.Runtime.Preserve(AllMembers = true)]
    internal class AdrCollection
    {
        public string BTC { get; set; }
        public string LTC { get; set; }
        public string[] XMR { get; set; }
        public string ETH { get; set; }
        public string NEO { get; set; }
    }
    //[Android.Runtime.Preserve(AllMembers = true)]
    internal class SignTransferResponse
    {
        public string Error { get; set; }
        public string ToAddress { get; set; }
        public string LineID { get; set; }
        public string[] SpendKeyImages { get; set; }
        public string TotalFee { get; set; }
        public string FeeWarning { get; set; }
        public string Blk { get; set; }
        public string BlkDisplayName { get; set; }
        public string TxnHex { get; set; }
        public string Amount { get; set; }
    }
  //  [Android.Runtime.Preserve(AllMembers = true)]
    internal class XMRTaskTransferResponse
    {
        public string Error { get; set; }
        public string TxnHex { get; set; }
        public string[] SpendKeyImages { get; set; }
    }
   // [Android.Runtime.Preserve(AllMembers = true)]
    internal class XMRTaskImageResponse
    {
        public string Error { get; set; }
        public string WalletAddress { get; set; }
        public string[] SpendKeyImages { get; set; }
    }
  //  [Android.Runtime.Preserve(AllMembers = true)]
    internal class RipTaskTransferResponse
    {
        public string Error { get; set; }
        public string TxnHex { get; set; }
    }
    internal class MsgTaskTransferResponse
    {
        public string Error { get; set; }
        public string MsgSig { get; set; }
    }
    //[Android.Runtime.Preserve(AllMembers = true)]
    internal class ApolloTaskTransferResponse
    {
        public string Error { get; set; }
        public string TxnHex { get; set; }
    }
   /// [Android.Runtime.Preserve(AllMembers = true)]
    internal class NEOTaskTransferResponse
    {
        public string Error { get; set; }
        public string TxnHex { get; set; }
    }
   // [Android.Runtime.Preserve(AllMembers = true)]
    internal class ETHTaskTransferResponse
    {
        public string Error { get; set; }
        public string TxnHex { get; set; }
    }
 //   [Android.Runtime.Preserve(AllMembers = true)]
    internal class AltoCoinTaskTransferResponse
    {
        public string Error { get; set; }
        public string FeeAmount { get; set; }
        public string FeeWarning { get; set; }
        public string TxnHex { get; set; }
    }
  //  [Android.Runtime.Preserve(AllMembers = true)]
    internal class NEONeoscanUnspent
    {
        public string txid { get; set; }
        public decimal value { get; set; }
        public int n { get; set; }
    }
 //   [Android.Runtime.Preserve(AllMembers = true)]
    internal class NEONeoscanUnclaimed
    {
        public NEONeoscanClaimable[] claimable { get; set; }
    }
   // [Android.Runtime.Preserve(AllMembers = true)]
    internal class NEONeoscanClaimable
    {
        public string txid { get; set; }
        public decimal unclaimed { get; set; }
        public int n { get; set; }
    }
}


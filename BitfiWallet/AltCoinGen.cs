using System;
using System.Collections.Generic;
using NBitcoin;
namespace BitfiWallet
{
    public class AltCoinGen
    {
        public Network net;
        public bool SegSupport = true;
        public AltCoinGen(Network _net)
        {
            net = _net;
            if (net == NBitcoin.Altcoins.Dogecoin.Instance.Mainnet) SegSupport = false;
            if (net == NBitcoin.Altcoins.Dash.Instance.Mainnet) SegSupport = false;
            if (net == NBitcoin.Altcoins.Zclassic.Instance.Mainnet) SegSupport = false;
        }
        public string GetNewAddress(byte[] keybytes, bool DoSegwit)
        {
            if (net == null) throw new Exception("Invalid currency.");
            Key pkey = new Key(keybytes, -1, DoSegwit);
            string address = "";
            if (DoSegwit)
            {
                address = pkey.PubKey.GetSegwitAddress(net).ToString();
            }
            else
            {
                address = pkey.PubKey.GetAddress(net).ToString();
            }
            return address;
        }
        public string AltMsgSign(byte[] keybytes, string tprocNoxAddressBTCAddress, string Message)
        {
            if (net == null) throw new Exception("Invalid currency.");
            bool DoLeg = false;
            bool DoSeg = false;
            Key pkey = new Key(keybytes, -1, false);
            if (pkey.PubKey.GetAddress(net).ToString() == tprocNoxAddressBTCAddress)
            {
                DoLeg = true;
            }
            if (SegSupport)
            {
                if (!DoLeg)
                {
                    pkey = new Key(keybytes, -1, true);
                    if (pkey.PubKey.GetSegwitAddress(net).ToString() == tprocNoxAddressBTCAddress)
                    {
                        DoSeg = true;
                    }
                }
            }
            if (DoLeg == false && DoSeg == false)
            {
                throw new Exception("Invalid Info.");
            }
            return pkey.SignMessage(Message);
        }
        public AltTxn AltCoinSign(List\\ kbList, string tprocToAddress, List\\ txnRaw, string tprocAmount, string tprocNoxAddressBTCAddress, string tprocFeeValue)
        {
            if (net == null) throw new Exception("Invalid currency.");
            bool ChangeAChecked = false;
            List\\ returnKeys = new List\\();
            for (int i = 0; i \            {             
                Key pkey = new Key(kbList[i], -1, false);
                returnKeys.Add(pkey.GetBitcoinSecret(net));
                if (pkey.PubKey.GetAddress(net).ToString() == tprocNoxAddressBTCAddress)
                {
                    ChangeAChecked = true;
                }
                if (SegSupport)
                {
                    pkey = new Key(kbList[i], -1, true);
                    returnKeys.Add(pkey.GetBitcoinSecret(net));
                    if (pkey.PubKey.GetSegwitAddress(net).ToString() == tprocNoxAddressBTCAddress)
                    {
                        ChangeAChecked = true;
                    }
                }
                NoxKeys.Sclear.EraseBytes(kbList[i]);
                pkey = null;               
            }
            if (ChangeAChecked == false)
            {
                throw new Exception("Invalid Info.");

            }
            var txn = MultipleTransV2(returnKeys.ToArray(), tprocToAddress, txnRaw, tprocAmount, tprocNoxAddressBTCAddress, tprocFeeValue, net);
            return txn;
        }
        private AltTxn MultipleTransV2(ISecret[] secrects, string ToAddress, List\\ PrevTransList, string Amount, string ChangeAddress, string ExactFeeAmount, Network net)
        {
            AltTxn resp = new AltTxn();
            try
            {
                var txBuilder = net.CreateTransactionBuilder();
                Transaction tx = Transaction.Create(net);
                List\\ m1CoinsV = new List\\();
                long totalCoins = 0;
                foreach (NoxKeys.BCUnspent trans in PrevTransList)
                {
                    var amount = Money.Parse(trans.Amount);
                    totalCoins = totalCoins + amount.Satoshi;
                    ICoin coin = new Coin(uint256.Parse(trans.TxHash), (uint)trans.OutputN, amount, BitcoinAddress.Create(trans.Address, net).ScriptPubKey);
                    m1CoinsV.Add(coin);
                }
                var m2kChange = BitcoinAddress.Create(ChangeAddress, net);
                var m2k = BitcoinAddress.Create(ToAddress, net);
                tx = Transaction.Create(net);
                txBuilder.AddCoins(m1CoinsV.ToArray());
                txBuilder.AddKeys(secrects);
                txBuilder.Send(m2k, Money.Parse(Amount));
                txBuilder.SetChange(m2kChange);
                txBuilder.SendFees(Money.Parse(ExactFeeAmount));
                tx = txBuilder.BuildTransaction(true);
                tx.Version = 2;
                tx = txBuilder.SignTransaction(tx);
                if (txBuilder.Verify(tx) == true)
                {
                    resp.Fee = (totalCoins - tx.TotalOut.Satoshi) * 0.00000001M;
                    resp.TxnHex = tx.ToHex();
                    tx = null; txBuilder = null;
                    return resp;
                }
                else
                {
                    resp.IsError = true;
                    resp.ErrorMessage = "Error, Not fully signed.";
                    tx = null;
                    txBuilder = null;
                    return resp;
                }
            }
            catch (Exception ex)
            {
                resp.IsError = true;
                resp.ErrorMessage = ex.Message;
                return resp;
            }
        }
    }
    public class AltTxn
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public decimal Fee { get; set; }
        public string TxnHex { get; set; }
    }
}
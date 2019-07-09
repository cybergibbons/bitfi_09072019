using Apollo.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo
{
  public class NodeApi
  {
    //http://localhost:6876/apl?requestType=sendMoney&publicKey=b35c4388c5110f5e45ad851e270806a72f79d4110914fa32a04fcb9bfd88e43f&recipient=APL-QY8M-ULQ5-G75K-GDG8L&amountATM=100000000&feeATM=100000000&deadline=60
    private string BaseUrl = ""; 

    public NodeApi(string url = "https://apollowallet.org")
    {
      BaseUrl = url + "/apl?requestType={0}&{1}";
    }

    public UnsignedTx ConstructSendMoneyTransaction(string recipientId, string publicKey, 
      string amountATMsatoshi,  string feeATMsatoshi = "100000000", int deadline = 60)
    {
      var completeUrl = String.Format(BaseUrl, "sendMoney", String.Format(
        "publicKey={0}&recipient={1}&amountATM={2}&feeATM={3}&deadline={4}",
        publicKey, recipientId, amountATMsatoshi, feeATMsatoshi, deadline
      ));

      JObject o = (JObject)JToken.FromObject(new { });
            return null;
    }

    public BroadcastResponse BroadcastTransaction(string transactionBytes)
    {
      var completeUrl = String.Format(BaseUrl, "broadcastTransaction", 
        String.Format("transactionBytes={0}", transactionBytes));
      JObject o = (JObject)JToken.FromObject(new { });
            return null;
    }

    public AccountTxes GetAccountTransactions(string accountId)
    {
      var completeUrl = String.Format(BaseUrl, "getBlockchainTransactions", String.Format("account={0}", accountId));
            return null;
    }

    public Account GetAccount(string accountId)
    {
      var completeUrl = String.Format(BaseUrl, "getAccount", String.Format("account={0}", accountId));
            return null;
    }
  }
}

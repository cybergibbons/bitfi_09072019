using EthereumLibrary.Hex.HexConvertors.Extensions;
using Newtonsoft.Json;
using NBitcoin;
using NBitcoin.DataEncoders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
namespace BitfiWallet
{
    public class NoxMessage
    {
        public TxnResponse SendMessage(string uri, string message, NBitcoin.BitcoinSecret sec)
        {
            try
            {
                Network net = Network.Main;
                string Signature = sec.PrivateKey.SignMessage(message);
                string rawtxn = BuildMsgTxn(sec.PubKey.GetAddress(net).ToString(), message, Signature, "");
                return POST(uri, rawtxn);
            }
            catch
            {
                TxnResponse txnResponse = new TxnResponse();
                txnResponse.success = false;
                txnResponse.error_message = "Unexpected result, please check connection.";
                return txnResponse;
            }
        }
        private static void Write(MemoryStream ms, byte[] bytes)
        {
            ms.Write(bytes, 0, bytes.Length);
        }
        private static byte[] WriteBts(byte[] bytes, int start, int lenght)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(bytes, start, lenght);
            return ms.ToArray();
        }
        static string BuildMsgTxn(string FromAddress, string Message, string Signature, string LinkedTxnHex)
        {
            if (string.IsNullOrEmpty(LinkedTxnHex)) LinkedTxnHex = new byte[16].ToHex();
            var decoded58 = Encoders.Base58.DecodeData(FromAddress);
            MemoryStream ms = new MemoryStream();
            ms.Write(decoded58, 1, 20);
            var decoded = ms.ToArray();
            BigInteger bigInteger = new BigInteger(decoded);
            FromAddress = bigInteger.ToByteArray().ToHex();
            Signature = Convert.FromBase64String(Signature).ToHex();
            string MsgData = FromAddress + Signature + System.Text.Encoding.UTF8.GetBytes(Message).ToHex();
            System.Security.Cryptography.MD5 sha1 = System.Security.Cryptography.MD5.Create();
            string TxnHex = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(MsgData.ToLower())).ToHex();
            MsgData = TxnHex + LinkedTxnHex + MsgData;
            return MsgData.ToLower();
        }
        static string[] ReadMsgTxn(string MsgData)
        {
            byte[] bts = MsgData.ToLower().HexToByteArray();
            string TxnHex = WriteBts(bts, 0, 16).ToHex();
            string LinkedTxn = WriteBts(bts, 16, 16).ToHex();
            string FromAddress = WriteBts(bts, 32, 20).ToHex();
            string Signature = WriteBts(bts, 52, 65).ToHex();
            string Message = WriteBts(bts, 117, bts.Length - 117).ToHex();
            if (!string.IsNullOrEmpty(LinkedTxn) && LinkedTxn.HexToByteArray().Length != 16) return null;
            if (TxnHex.HexToByteArray().Length != 16) return null;
            System.Security.Cryptography.MD5 sha1 = System.Security.Cryptography.MD5.Create();
            if (TxnHex != sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(FromAddress + Signature + Message)).ToHex()) return null;
            Network net = Network.Main;
            BitcoinPubKeyAddress bitcoinPubKeyAddress = new BitcoinPubKeyAddress(new KeyId(FromAddress.HexToByteArray()), net);
            Message = System.Text.Encoding.UTF8.GetString(Message.HexToByteArray());
            Signature = Convert.ToBase64String(Signature.HexToByteArray());
            FromAddress = bitcoinPubKeyAddress.ToString();
            if (bitcoinPubKeyAddress.VerifyMessage(Message, Signature) == false) return null;
            return new string[] { TxnHex, FromAddress, Signature, Message, LinkedTxn };
        }
        public static TxnResponse POST(string url, string obj)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 5000;
                request.ContentType = "application/text";
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(obj);
                request.ContentLength = byteArray.Length;
                using (System.IO.Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
                using (WebResponse response = request.GetResponse())
                using (System.IO.Stream stream = response.GetResponseStream())
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                    var result = sr.ReadToEnd();
                    return JsonConvert.DeserializeObject<TxnResponse>(result);
                }
            }
            catch (WebException)
            {
                TxnResponse txnResponse = new TxnResponse();
                txnResponse.success = false;
                txnResponse.error_message = "Please check connection.";
                return txnResponse;
            }
            catch (Exception)
            {
                TxnResponse txnResponse = new TxnResponse();
                txnResponse.success = false;
                txnResponse.error_message = "Please check connection.";
                return txnResponse;
            }
        }
        public static bool GetCaptive()
        {
            bool Captive = false;
            try
            {
                WebRequest request = WebRequest.Create("http://bit.bitfi.com");
                request.Method = "GET";
                request.Timeout = 3000;
                request.ContentType = "application/text";
                using (WebResponse response = request.GetResponse())
                using (System.IO.Stream stream = response.GetResponseStream())
                {
                    Captive = true;
                    stream.Close();
                    response.Close();

                }
            }
            catch (Exception)
            {
                Captive = false;
            }
            return Captive;
        }
    }

    public class TxnResponse
    {
        public bool success { get; set; }
        public string error_message { get; set; }
    }
}
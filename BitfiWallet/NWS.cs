using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using BitfiWallet.SGADWS;
using BitfiWallet.NOXWS;
namespace NoxKeys
{
    internal class NWS
    {
        public static bool Validator(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
                if (cert == null || !cert.Subject.StartsWith("CN=www.bitfi.com")) return false;
                if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None) return true;
                return false;
        }
        public NWS()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = Validator;

        }
        internal static string SignMsg(string msg)
        {
            try
            {             
                return Sclear.noxPrivateKey.PrivateKey.SignMessage(msg);              
            }
            catch
            {              
                return null;
            }
        }

        internal static bool ValidMsg(string msg, string signature)
        {
            try
            {
                if (Sclear.saddress == null) return false;
                string hash = SHashSHA1(msg);
                return Sclear.saddress.VerifyMessage(hash, signature);
            }
            catch
            {
                return true;
            }
        }

        private static string SHashSHA1(string msg)
        {
            byte[] input = System.Text.Encoding.ASCII.GetBytes(msg);
            using (System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed())
            {
                var hash = sha1.ComputeHash(input);
                var sb = new System.Text.StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private static string HashHMAC(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new System.Security.Cryptography.HMACSHA256(keyByte))
            {
                byte[] buff = hmacsha256.ComputeHash(messageBytes);
                string sbinary = "";
                for (int i = 0; i \                {
                    sbinary += buff[i].ToString("X2");
                }
                return (sbinary);
            }
        }
        internal string GetSGAMessage()
        {
            using (SGADWS walletServ = new SGADWS())
            {
                walletServ.Timeout = 30000;
                try
                {
                    string msg = Guid.NewGuid().ToString();
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    string sgamessage = walletServ.GetSGAMessage(signature, msg, Sclear.pubkey, Sclear.PREFUSER);
                    return sgamessage;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal string GetSGAToken(string userPubKey, string Signature)
        {
            using (SGADWS walletServ = new SGADWS())
            {
                walletServ.Timeout = 30000;
                try
                {
                    string msg = Guid.NewGuid().ToString();
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    string MessageToken = walletServ.GetSGAToken(signature, msg, Sclear.pubkey, Sclear.PREFUSER, userPubKey, Signature);
                    //  if (!string.IsNullOrEmpty(MessageToken) && MessageToken.Length \>\ 10) SGAMSGTOKEN = MessageToken;
                    return MessageToken;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal string GetSGATokenForSignin(string userPubKey, string Signature, string DisplayCode)
        {
            using (SGADWS walletServ = new SGADWS())
            {
                walletServ.Timeout = 30000;
                try
                {
                    string msg = Guid.NewGuid().ToString();
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    string MessageToken = walletServ.GetSGATokenForSignIn(signature, msg, Sclear.pubkey, Sclear.PREFUSER, userPubKey, Signature, DisplayCode);
                    return MessageToken;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal BitfiWallet.SGADWS.NoxMessagesConfig GetMessagesConfig()
        {
            using (SGADWS walletServ = new SGADWS())
            {
                walletServ.Timeout = 30000;
                try
                {
                    var config = walletServ.GetMessageConfig();
                    return config;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal BitfiWallet.TxnResponse SendMessage(string url, string message)
        {
            BitfiWallet.NoxMessage noxMessage = new BitfiWallet.NoxMessage();
            return noxMessage.SendMessage(url, message, Sclear.noxPrivateKey);
        }
        internal BitfiWallet.SGADWS.OverviewViewModel GetOverviewModel(string SGAMSGTOKEN)
        {
            using (SGADWS walletServ = new SGADWS())
            {
                walletServ.Timeout = 30000;
                try
                {
                    if (string.IsNullOrEmpty(SGAMSGTOKEN)) return null;
                    Sclear.MsgCount = Sclear.MsgCount + 1;
                    string msg = Sclear.MsgCount.ToString();
                    string signature = SignMsg(msg);
                    var model = walletServ.GetOverviewModel(signature, msg, Sclear.pubkey, Sclear.PREFUSER, SGAMSGTOKEN);
                    return model;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal BitfiWallet.SGADWS.NoxAddressReviewV3 GetAddressIndexes(string SGAMSGTOKEN)
        {
            using (SGADWS walletServ = new SGADWS())
            {
                walletServ.Timeout = 30000;
                try
                {
                    if (string.IsNullOrEmpty(SGAMSGTOKEN)) return null;
                    Sclear.MsgCount = Sclear.MsgCount + 1;
                    string msg = Sclear.MsgCount.ToString();
                    string signature = SignMsg(msg);
                    return walletServ.GetAddressIndexesV3(signature, msg, Sclear.pubkey, Sclear.PREFUSER, SGAMSGTOKEN);
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        internal string RespondFirstAddressRequest(string TXNLineID, BitfiWallet.NOXWS.FirstAdrCollection Addresses)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 60000;
                    string msg = Addresses.BTC + Addresses.ETH + Addresses.LTC + Addresses.Monero + Addresses.NEO;
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    return serv.RespondFirstAddressRequest(signature, msg, Sclear.pubkey, TXNLineID, Addresses);
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal string RespondAddressRequest(string TXNLineID, string adr)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 30000;
                    string msg = adr;
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    return serv.RespondAddressRequestV3(signature, msg, Sclear.pubkey, TXNLineID, adr);
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal string[] GetFirstAddress(string TXNLineID)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 30000;
                    string msg = Guid.NewGuid().ToString();
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    return serv.GetFirstAddress(signature, msg, Sclear.pubkey, TXNLineID);
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal string XMRGetRandom(string mixin, string[] amounts)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 30000;
                    string msg = Guid.NewGuid().ToString();
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    var resp = serv.XMRGetRandom(signature, msg, Sclear.pubkey, mixin, amounts);
                    return resp;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal BitfiWallet.NOXWS.NoxAddressRequests GetRequest(string actiontask)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 10000;
                    string msg = Guid.NewGuid().ToString();
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    var resp = serv.GetRequest(signature, msg, Sclear.pubkey, actiontask);
                    if (resp == null) return null;
                    if (ValidMsg(resp.Response.SMSToken + resp.Response.BlkNet + resp.Response.HDIndex.ToString(), resp.Signature))
                    {
                        return resp.Response;
                    }
                    return null;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal string SubmitTxnResponseMX(string TXNLineID, string TxnHex, string[] SpendKeyImages)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 30000;
                    string msg = TxnHex;
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    var resp = serv.SubmitTxnResponseMX(signature, msg, Sclear.pubkey, TXNLineID, TxnHex, SpendKeyImages);
                    return resp;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal string SubmitTxnResponse(string TXNLineID, string TxnHex)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 30000;
                    string msg = TxnHex;
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    return serv.SubmitTxnResponse(signature, msg, Sclear.pubkey, TXNLineID, TxnHex);
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal string SubmitMsgResponse(string TXNLineID, string MsgSig)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 30000;
                    string msg = MsgSig;
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    return serv.SubmitMsgResponse(signature, msg, Sclear.pubkey, TXNLineID, MsgSig);
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal BitfiWallet.NOXWS.NoxMsgProcess GetMsgRequest(string actiontask)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 30000;
                    string msg = Guid.NewGuid().ToString();
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    var resp = serv.GetMsgRequest(signature, msg, Sclear.pubkey, actiontask);
                    if (ValidMsg(resp.Response.Msg, resp.Signature))
                    {
                        return resp.Response;
                    }
                    return null;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal BitfiWallet.NOXWS.NoxTxnProcess GetTxnRequest(string actiontask)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 30000;
                    string msg = Guid.NewGuid().ToString();
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    var resp = serv.GetTxnRequestV3(signature, msg, Sclear.pubkey, actiontask);
                    if (ValidMsg(resp.Response.Amount + resp.Response.BlkNet + resp.Response.ToAddress + resp.Response.FeeValue +
                        resp.Response.FeeTotal + resp.Response.MXTxn + resp.Response.ETCGasUsed + resp.Response.ETCNonce +
                        resp.Response.ETCToken + resp.Response.USDRate.ToString(), resp.Signature))
                    {
                        return resp.Response;
                    }
                    return null;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal string SubmitGasResponse(string TXNLineID, string txHex)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 30000;
                    string msg = txHex;
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    return serv.SubmitGasResponse(signature, msg, Sclear.pubkey, TXNLineID, txHex);
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal BitfiWallet.NOXWS.NoxGasRequests GetGasRequest(string actiontask)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 30000;
                    string msg = Guid.NewGuid().ToString();
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    var resp = serv.GetGasRequest(signature, msg, Sclear.pubkey, actiontask);
                    if (resp == null) return null;
                    if (ValidMsg(resp.Response.SMSToken + resp.Response.TXNLineID, resp.Signature))
                    {
                        return resp.Response;
                    }
                    return null;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal string RespondImageRequest(string TXNLineID, string[] images, string Address)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 30000;
                    string msg = Address;
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    return serv.RespondImageRequestV2(signature, msg, Sclear.pubkey, TXNLineID, images, Address);
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        internal BitfiWallet.NOXWS.NoxImageRequests GetImageRequest(string actiontask)
        {
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 30000;
                    string msg = Guid.NewGuid().ToString();
                    msg = SHashSHA1(msg);
                    string signature = SignMsg(msg);
                    var resp = serv.GetImageRequestV2(signature, msg, Sclear.pubkey, actiontask);
                    if (resp == null) return null;
                    if (ValidMsg(resp.Response.SMSToken + resp.Response.TXNLineID, resp.Signature))
                    {
                        return resp.Response;
                    }
                    return null;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
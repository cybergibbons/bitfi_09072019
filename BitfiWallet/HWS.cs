using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using BitfiWallet.NOXWS;
namespace NoxKeys
{
    internal class HWS
    {
        public static bool Validator(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (cert == null || !cert.Subject.StartsWith("CN=www.bitfi.com")) return false;
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None) return true;
            return false;
        }
        private Guid HSession = Guid.NewGuid();
        private string sessionSignatureGetVal = null;
        private string sessionMessageGetVal = null;
        private string sessionSignatureSetVal = null;
        private string sessionMessageSetVal = null;
        public HWS(Guid LaunchSession)
        { 
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = Validator;
            if (LaunchSession == null) return;
            HSession = LaunchSession;
            SignGetData();
            SignSetData();
        }
        internal Guid CurrentHS
        {
            get { return HSession;  }
        }
        protected void SignGetData()
        {
            string msg = HSession.ToString();
            msg = SSHashSHA1(msg);
            sessionSignatureGetVal = SSignMsg(msg + Sclear.SaHash);
            sessionMessageGetVal = msg;
        }
        protected void SignSetData()
        {
            string msg = HSession.ToString();
            msg = SSHashSHA1(msg);
            sessionSignatureSetVal = SSignMsg(msg);
            sessionMessageSetVal = msg;
        }
        internal BitfiWallet.NOXWS.FormUserInfo GetCurrentSMSToken(Guid LaunchSession)
        {
            if (LaunchSession != HSession) return null;
            //if (sessionMessageGetVal == null || sessionSignatureGetVal == null) return null;
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 5000;

                    var resp = serv.GetCurrentSMSToken(sessionSignatureGetVal, sessionMessageGetVal, Sclear.pubkey, Sclear.PREFUSER, Sclear.WALLETID);
                    if (resp == null) return null;
                    if (SValidMsg(resp.Response.SMSToken + resp.Response.ReqType, resp.Signature))
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
        internal void SetGCMToken(Guid LaunchSession)
        {
            if (LaunchSession != HSession) return;
            //if (sessionMessageSetVal == null || sessionSignatureSetVal == null) return;
            using (NOXWS2 serv = new NOXWS2())
            {
                try
                {
                    serv.Timeout = 5000;
                    serv.SetGCMToken(sessionSignatureSetVal, sessionMessageSetVal, Sclear.pubkey, Sclear.PREFUSER, Sclear.WALLETID);
                }
                catch (WebException)
                {
                    return;
                }
                catch (Exception)
                {
                }
            }
        }
        private static bool SValidMsg(string msg, string signature)
        {
            try
            {
                if (Sclear.saddress == null) return false;
                string hash = SSHashSHA1(msg);
                return Sclear.saddress.VerifyMessage(hash, signature);
            }
            catch
            {
                return true;
            }
        }
        internal static string SSignMsg(string msg)
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
        private static string SSHashSHA1(string msg)
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
    }
}


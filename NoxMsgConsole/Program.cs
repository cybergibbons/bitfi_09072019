using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECLibrary.Hex.HexConvertors.Extensions;
using ECLibrary.Signer.Crypto;
using ECLibrary.Util;
using Newtonsoft.Json;
using Org.BouncyCastle.Math;
namespace NoxMsgConsole
{
    class Program
    {

        //This is standard signing, unlike message compact signing with formating.
        static void TxnSign()
        {
            Console.WriteLine("Enter private key:  (Use hex type"); Console.WriteLine();
            string pk = Console.ReadLine(); Console.WriteLine();
            ECLibrary.Signer.EthECKey key = new ECLibrary.Signer.EthECKey(pk.HexToByteArray(), true);
            Console.WriteLine("Enter local path to file: "); Console.WriteLine();
            var message = Console.ReadLine(); Console.WriteLine();
            byte[] msg = FileToBytes(message);
            using (var sha = new SHA256Managed())
            {
              var sig = key.Sign(sha.ComputeHash(msg));
              var der = sig.ToDER();
              Console.WriteLine(der.ToHex());
            }
            Console.ReadLine();
        }

       static void Main(string[] args)
        {
            try
            {


                Console.WriteLine("Enter developer private key:  (Use hex type"); Console.WriteLine();
                string pk = Console.ReadLine(); Console.WriteLine();

                ECLibrary.Signer.EthECKey key = new ECLibrary.Signer.EthECKey(pk.HexToByteArray(), true);
                string PubKey = key.GetPubKey().ToHex();
                var sha = SHA256Managed.Create();
                RIPEMD160Managed rIPEMD160Managed = new RIPEMD160Managed();
                var ripemd = rIPEMD160Managed.ComputeHash(sha.ComputeHash(key.GetPubKey()));
                string linked = "";
                Console.Clear();
                Console.WriteLine("Enter Task/TxnType: (//0 MSG//1 ARTICLE//2 DOWNLOAD//3 STATUS UPDATE)"); Console.WriteLine();
                string type = Console.ReadLine(); Console.WriteLine();
                int stype = Convert.ToInt32(type);
                if (stype \\ 3) return;
                string message = "";
                var uri = "https://bitfi.dev/NoxMessages/API/PostMsg.aspx?TxnType=";
                uri = uri + stype.ToString();
                if (stype == 0)
                {
                    Console.WriteLine("Enter linked message txn id: (leave blank if starting discussion)"); Console.WriteLine();
                    linked = Console.ReadLine(); Console.WriteLine();
                }
                if (stype == 1)
                {
                    string ArticleTitle = "";
                    string ArticleSubTitle = "";
                    string ArticleDescription = "";
                    string ArticleTag = "";
                    Console.WriteLine("Enter Article category (most should use type 0 for All Articles category)"); Console.WriteLine();
                    string category = Console.ReadLine(); Console.WriteLine();
                    if (string.IsNullOrEmpty(category)) category = "0";
                    uri = uri + "&ArticleCategory=" + category;
                    Console.WriteLine("Enter ArticleTitle:"); Console.WriteLine();
                    ArticleTitle = Console.ReadLine(); Console.WriteLine();
                    uri = uri + "&ArticleTitle=" + ArticleTitle;
                    Console.WriteLine("Enter ArticleSubTitle:"); Console.WriteLine();
                    ArticleSubTitle = Console.ReadLine(); Console.WriteLine();
                    uri = uri + "&ArticleSubTitle=" + ArticleSubTitle;
                    Console.WriteLine("Enter ArticleDescription:"); Console.WriteLine();
                    ArticleDescription = Console.ReadLine(); Console.WriteLine();
                    uri = uri + "&ArticleDescription=" + ArticleDescription;
                    Console.WriteLine("Enter ArticleTag:"); Console.WriteLine();
                    ArticleTag = Console.ReadLine(); Console.WriteLine();
                    uri = uri + "&ArticleTag=" + ArticleTag;
                    Console.WriteLine("Enter article content: (Base64 String)"); Console.WriteLine();
                    message = Console.ReadLine(); Console.WriteLine();
                    byte[] msg = Convert.FromBase64String(message);
                    message = System.Text.Encoding.UTF8.GetString(msg);
                }
                if (stype == 2)
                {
                    Console.WriteLine("Enter Download short name: "); Console.WriteLine();
                    string shortname = Console.ReadLine(); Console.WriteLine();
                    Console.WriteLine("Enter Download description: "); Console.WriteLine();
                    string description = Console.ReadLine(); Console.WriteLine();
                    Console.WriteLine("Enter Download URL: "); Console.WriteLine();
                    string download_url = Console.ReadLine(); Console.WriteLine();
                    Download_Page download_Page = new Download_Page();
                    download_Page.description = description;
                    download_Page.download_url = download_url;
                    download_Page.short_name = shortname;
                    var settings = new JsonSerializerSettings();
                    settings.Formatting = Formatting.None;
                    settings.StringEscapeHandling = StringEscapeHandling.Default;
                    var str = JsonConvert.SerializeObject(download_Page, settings);
                    message = str;
                }
                if (stype == 3)
                {
                    Console.WriteLine("Enter TxnID for transaction to update: "); Console.WriteLine();
                    string txnidupdate = Console.ReadLine(); Console.WriteLine();
                    Console.WriteLine("Enter true/false value for list display: "); Console.WriteLine();
                    bool display = Convert.ToBoolean(Console.ReadLine()); Console.WriteLine();
                    Status_Update status_Update = new Status_Update();
                    status_Update.list_display = display;
                    status_Update.txn_id = txnidupdate;
                    var settings = new JsonSerializerSettings();
                    settings.Formatting = Formatting.None;
                    settings.StringEscapeHandling = StringEscapeHandling.Default;
                    var str = JsonConvert.SerializeObject(status_Update, settings);
                    message = str;
                }
                bool success = true;
                if (stype == 0)
                {
                    while (key != null && success == true)
                    {
                        Console.WriteLine("Enter Message: "); Console.WriteLine();
                        message = Console.ReadLine(); Console.WriteLine();
                        byte[] msgbts = System.Text.Encoding.UTF8.GetBytes(message);
                        byte[] EncodedMsg = Util.NBitcoinUtils.CreateEncodedMessageD2(message);
                        byte[] signaturebts = SignMsg(EncodedMsg, key.GetPrivateKeyAsBytes(), key.GetPubKey());
                        string Signature = Convert.ToBase64String(signaturebts);
                        string rawtxn = BuildMsgTxn(ripemd.ToHex(), msgbts.ToHex(), Signature, linked);
                        if (string.IsNullOrEmpty(linked))
                        {
                            string[] decode = ReadMsgTxn(rawtxn);
                            linked = decode[0];
                        }
                            var resp = WebTests.POST(uri, rawtxn);
                            success = resp.success;
                            Console.WriteLine("Success: " + resp.success);
                        if (!success)
                        {
                            Console.WriteLine(resp.error_message);
                            Console.WriteLine();
                            Console.WriteLine("Any key to exit...");
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Sent...contiune comments with txn: " + linked);
                        }
                    }
                }
                else
                {
                    byte[] msgbts = System.Text.Encoding.UTF8.GetBytes(message);
                    byte[] EncodedMsg = Util.NBitcoinUtils.CreateEncodedMessageD2(message);
                    byte[] signaturebts = SignMsg(EncodedMsg, key.GetPrivateKeyAsBytes(), key.GetPubKey());
                    string Signature = Convert.ToBase64String(signaturebts);
                    string rawtxn = BuildMsgTxn(ripemd.ToHex(), msgbts.ToHex(), Signature, linked);
                        var resp = WebTests.POST(uri, rawtxn); 
                        Console.WriteLine("Success: " + resp.success);
                        Console.WriteLine(resp.error_message);
                    Console.WriteLine();
                    Console.WriteLine("Any key to exit...");
                        Console.ReadLine();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("Invalid information for building txn. Any key to exit...");
                Console.ReadLine();
            }

        }
        static byte[] FileToBytes(string _FileName)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                using (System.IO.Stream fs = System.IO.File.Open(_FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
                {
                    using (System.IO.StreamReader _FileStream = new System.IO.StreamReader(fs))
                    {
                        _FileStream.BaseStream.CopyTo(memoryStream);
                        if (_FileStream != null)
                        {
                            _FileStream.Close();
                            if (_FileStream.BaseStream != null)
                            {
                                _FileStream.BaseStream.Dispose();
                            }
                            _FileStream.Dispose();
                        }
                    }
                    fs.Close();
                    fs.Dispose();
                }
                return memoryStream.ToArray();
            }
            catch
            {
                return null;
            }
        }
        static string BuildMsgTxn(string RIPEMD_FromAddress, string MessageHex, string Signature, string LinkedTxnHex)
        {
            if (string.IsNullOrEmpty(LinkedTxnHex)) LinkedTxnHex = new byte[16].ToHex();
            Signature = Convert.FromBase64String(Signature).ToHex();
            string MsgData = RIPEMD_FromAddress + Signature + MessageHex;
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
            Message = System.Text.Encoding.UTF8.GetString(Message.HexToByteArray());
            Signature = Convert.ToBase64String(Signature.HexToByteArray());
            return new string[] { TxnHex, FromAddress, Signature, Message, LinkedTxn };
        }

        static byte[] SignMsg(byte[] hash, byte[] key, byte[] PubKey)
        {
            var _key = new ECLibrary.Signer.Crypto.ECKey(key, true);
            var sig = _key.Sign(hash);
            var recId = -1;
            var thisKey = PubKey;
            for (var i = 0; i \            {
                var rec = ECKey.RecoverFromSignature(i, sig, hash, false);
                if (rec != null)
                {
                    var k = rec.GetPubKey(false);
                    if (k != null && k.SequenceEqual(thisKey))
                    {
                        recId = i;
                        break;
                    }
                }
            }
            if (recId == -1) return null;
            int headerByte = recId + 27 + (false ? 4 : 0);
            byte[] sigData = new byte[65];
            sigData[0] = (byte)headerByte;
            Array.Copy(BigIntegerToBytes(sig.R, 32), 0, sigData, 1, 32);
            Array.Copy(BigIntegerToBytes(sig.S, 32), 0, sigData, 33, 32);
            return sigData;
        }
        static Array BigIntegerToBytes(BigInteger b, int numBytes)
        {
            if (b == null)
            {
                return null;
            }
            byte[] bytes = new byte[numBytes];
            byte[] biBytes = b.ToByteArray();
            int start = (biBytes.Length == numBytes + 1) ? 1 : 0;
            int length = Math.Min(biBytes.Length, numBytes);
            Array.Copy(biBytes, start, bytes, numBytes - length, length);
            return bytes;
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
    }
}



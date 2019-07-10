using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace NoxMsgConsole
{
    public class TxnResponse
    {
        public bool success { get; set; }
        public string error_message { get; set; }
    }
    public class Status_Update
    {
        public string txn_id { get; set; }
        public bool list_display { get; set; }
    }
    public class Download_Page
    {
        public string short_name { get; set; }
        public string description { get; set; }
        public string download_url { get; set; }
    }
    public class WebTests
    {
        public static TxnResponse POST(string url, string obj)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                // request.Timeout = 5000;
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
            catch (Exception ex)
            {
                TxnResponse txnResponse = new TxnResponse();
                txnResponse.success = false;
                txnResponse.error_message = "(Local exception from this process)" + ex.Message;
                return txnResponse;
            }
        }
    }
}

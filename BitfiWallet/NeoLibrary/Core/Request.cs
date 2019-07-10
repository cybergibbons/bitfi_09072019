//using System.Net.Http;
using System.Collections.Generic;
using System.IO;
//using System.Net;
using System.Text;
namespace NeoGasLibrary.Cryptography
{
    public enum RequestType
    {
        GET,
        POST
    }
    public static class RequestUtils
    {
        /*private static string ExecuteRequest(WebRequest webReq)
        {
            try
            {
                using (WebResponse response = webReq.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    Stream str = response.GetResponseStream();
                    if (str == null)
                        throw;
                    using (StreamReader sr = new StreamReader(str))
                    {
                        if (response.StatusCode != HttpStatusCode.InternalServerError)
                            throw;
                        return sr.ReadToEnd();
                    }
                }
            }
        }*/
        public static string GetWebRequest(string url)
        {
            return null;
        }
        // var r = PostWebRequest("http://seed2.antshares.org:10332", "{'jsonrpc': '2.0', 'method': 'getblockcount', 'params': [],  'id': 1}");
        public static string PostWebRequest(string url, string paramData)
        {
            return null;
        }
    }
}
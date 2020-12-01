using System;
using System.IO;
using System.Net;
using System.Text;

namespace Tamos.AbleOrigin.Common
{
    public static class WebReqUtil
    {
        public static string HttpGet(string uri, out string error)
        {
            var httpWebRequest = WebRequest.Create(uri);
            httpWebRequest.Method = "GET";

            return ReadWebResponse(httpWebRequest, Encoding.UTF8, out error);
        }

        public static string HttpPost(string uri, string query, out string error)
        {
            return PostRequestInternal(uri, query, "application/x-www-form-urlencoded; charset=UTF-8", out error);
        }

        public static string HttpPostJson(string uri, string jsonPara, out string error)
        {
            return PostRequestInternal(uri, jsonPara, "application/json", out error);
        }

        private static string PostRequestInternal(string uri, string data, string contentType, out string error)
        {
            var httpWebRequest = WebRequest.Create(uri);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = contentType;

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    var bytes = Encoding.UTF8.GetBytes(data);
                    var reqStream = httpWebRequest.GetRequestStream();
                    reqStream.Write(bytes, 0, bytes.Length);
                    reqStream.Close();
                }
                catch (Exception ex)
                {
                    error = "写入请求数据时出错：" + ex.Message;
                    return null;
                }
            }

            return ReadWebResponse(httpWebRequest, Encoding.UTF8, out error);
        }

        //读取请求结果
        private static string ReadWebResponse(WebRequest request, Encoding encode, out string error)
        {
            string responseData;
            try
            {
                using (var webResponse = request.GetResponse())
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    using (var responseReader = new StreamReader(webResponse.GetResponseStream(), encode))
                    {
                        responseData = responseReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }

            error = null;
            return responseData;
        }
    }
}

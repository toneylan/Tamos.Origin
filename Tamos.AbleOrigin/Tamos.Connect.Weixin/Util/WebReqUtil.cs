using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace Tamos.Connect.Weixin
{
    public static class WebReqUtil
    {
        public const string Content_Json = "application/json";

        public static string GetRequest(string uri, out string error)
        {
            var httpWebRequest = WebRequest.Create(uri) as HttpWebRequest;
            if (httpWebRequest == null)
            {
                error = "创建WebRequest失败。";
                return null;
            }
            httpWebRequest.Method = "GET";
            //httpWebRequest.ServicePoint.Expect100Continue = false;

            return ReadWebResponse(httpWebRequest, out error);
        }

        public static string PostRequest(string uri, string data, out string error, string contentType = null)
        {
            var httpWebRequest = WebRequest.Create(uri) as HttpWebRequest;
            if (httpWebRequest == null)
            {
                error = "创建WebRequest失败。";
                return null;
            }

            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = string.IsNullOrEmpty(contentType) ? "application/x-www-form-urlencoded" : contentType;
            if (!string.IsNullOrEmpty(data))
            {
                StreamWriter requestWriter = null;
                try
                {
                    requestWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                    requestWriter.Write(data);
                }
                catch (Exception ex)
                {
                    error = "写入请求参数时出错:" + ex.Message;
                    return null;
                }
                finally
                {
                    requestWriter?.Dispose();
                }
            }

            return ReadWebResponse(httpWebRequest, out error);
        }

        /*public static string PostRequest(string uri, NameValueCollection paras, string fieldName, HttpPostedFileBase file, out string error)
        {
            var boundary = "----------------" + DateTime.Now.Ticks.ToString("x");
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(uri);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;

            using (var memStream = new MemoryStream())
            using (var writer = new StreamWriter(memStream, Encoding.UTF8))
            {
                //write paras
                if (paras != null)
                {
                    var formdataTemplate = string.Concat("\r\n--", boundary, "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}");
                    foreach (string key in paras.Keys)
                    {
                        writer.Write(formdataTemplate, key, paras[key]);
                    }
                }

                if (file != null && file.ContentLength > 0)
                {
                    //write file header
                    writer.Write(string.Concat("\r\n--", boundary, "\r\n"));
                    writer.Write("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"; filelength=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        fieldName, file.FileName, file.ContentLength, file.ContentType);
                    writer.Flush();
                    //write file stream
                    file.InputStream.CopyTo(memStream);
                }
                //write end tag
                writer.Write(string.Concat("\r\n--", boundary, "--\r\n"));
                writer.Flush();

                httpWebRequest.ContentLength = memStream.Length;
                //put stream in to request
                var requestStream = httpWebRequest.GetRequestStream();
                memStream.Position = 0;
                memStream.CopyTo(requestStream);
            }

            return ReadWebResponse(httpWebRequest, out error);
        }*/

        private static string ReadWebResponse(WebRequest request, out string error)
        {
            string responseData;
            try
            {
                using (var webResponse = request.GetResponse())
                {
                    using (var responseReader = new StreamReader(webResponse.GetResponseStream()))
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

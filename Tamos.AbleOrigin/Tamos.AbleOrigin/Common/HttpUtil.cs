using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tamos.AbleOrigin
{
    public static class HttpUtil
    {
        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#alternatives-to-ihttpclientfactory
        //Alternatives to IHttpClientFactory
        private static readonly SocketsHttpHandler HttpHandler = new()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(10)
        };

        public static async Task<string> GetStringAsync(string reqUri)
        {
            using var client = new HttpClient(HttpHandler, false);
            return await client.GetStringAsync(reqUri);
        }

        /// <summary>
        /// 以Content-Type: application/x-www-form-urlencoded，执行Post请求。
        /// </summary>
        public static async Task<string> PostFormAsync(string reqUri, IReadOnlyCollection<KeyValuePair<string, string?>> formData)
        {
            using var client = new HttpClient(HttpHandler, false);

            var strContent = new FormUrlEncodedContent(formData);
            var response = await client.PostAsync(reqUri, strContent);
            
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 执行Post请求。
        /// </summary>
        public static async Task<string> PostAsync(string reqUri, string content, HttpReqOptions options = default)
        {
            using var client = new HttpClient(HttpHandler, false);
            if (options.Headers.NotNull())
            {
                foreach (var (name, value) in options.Headers)
                {
                    client.DefaultRequestHeaders.Add(name, value);
                }
            }

            var strContent = new StringContent(content, Encoding.UTF8, options.ContentType);
            var response = await client.PostAsync(reqUri, strContent);
            
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        //"application/json"
    }

    public struct HttpReqOptions
    {
        public string? ContentType { get; set; }
        public IReadOnlyCollection<(string, string)>? Headers { get; set; }
    }
}

using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Tamos.AbleOrigin.AspNetCore
{
    public static class WebUtility
    {
        #region Cookie

        public static void CookieSet(HttpContext httpContext, string cookieName, string cookieValue, DateTime? expires, string cookieDomain = null)
        {
            var opt = new CookieOptions();
            if (expires != null) opt.Expires = expires.Value;
            if (!string.IsNullOrEmpty(cookieDomain)) opt.Domain = cookieDomain;
            httpContext.Response.Cookies.Append(cookieName, cookieValue, opt);
        }

        /// <summary>
        /// 获取cookie值，不存在返回null 
        /// </summary>
        public static string? CookieGet(HttpContext httpContext, string cookieName)
        {
            return httpContext.Request.Cookies.TryGetValue(cookieName, out var val) ? val : null;
        }

        /// <summary>
        /// 删除cookie
        /// </summary>
        public static void CookieDelete(HttpContext httpContext, string cookieName)
        {
            httpContext.Response.Cookies.Delete(cookieName);
        }

        #endregion

        #region Browser detect

        /// <summary>
        /// 是否从微信访问
        /// </summary>
        public static bool IsInWeixin(this HttpRequest request)
        {
            string agent = request.Headers["User-Agent"]; 
            return agent.NotNull() && agent.Contains("MicroMessenger/");
        }

        #endregion

        #region IP地址

        /// <summary>
        /// 获得当前Http请求的客户端IP
        /// </summary>
        public static string GetClientIP(HttpRequest request)
        {
            string clientIp = request.Headers["X-Real-IP"];
            if (clientIp.NotNull()) return clientIp;

            clientIp = request.Headers["X-Forwarded-For"]; //request.Headers["REMOTE_ADDR"]
            if (clientIp.NotNull()) return clientIp;

            clientIp = request.HttpContext.Connection.RemoteIpAddress?.ToString();
            return clientIp;
        }

        #endregion

        #region Url解析
        
        /// <summary>
        /// 从Url中解析域名（含协议、端口）如 http://m.xxx.com。<br/>
        /// onlyHost：是否只要域名部分
        /// </summary>
        public static string GetDomain(string url, bool onlyHost = false)
        {
            if (string.IsNullOrEmpty(url) || !url.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return null;

            try
            {
                var uri = new Uri(url);
                return onlyHost ? uri.Host : $"{uri.Scheme}://{uri.Authority}";
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// 获取顶级域名，支持localhost、IP和带端口格式
        /// </summary>
        public static string GetRootDomain(string domainName)
        {
            //(?:)表示非捕获分组
            var match = Regex.Match(domainName, @"((?:[\w-]+\.)?([a-zA-Z]+|com\.cn))(?::\d+)?$");
            if (match.Success && match.Groups.Count >= 2) return match.Groups[1].Value;

            //可能是IP格式
            return Regex.Replace(domainName, @":\d+$", string.Empty); //移除端口
        }

        /// <summary>
        /// 判断是否相同的顶级域名，含端口时不区分端口。
        /// </summary>
        public static bool IsSameRootDomain(string domain1, string domain2)
        {
            if (string.IsNullOrEmpty(domain1) || string.IsNullOrEmpty(domain2)) return false;

            var rootDm1 = GetRootDomain(domain1);
            return rootDm1.NotNull() && rootDm1 == GetRootDomain(domain2);
        }
        
        #endregion
    }
}

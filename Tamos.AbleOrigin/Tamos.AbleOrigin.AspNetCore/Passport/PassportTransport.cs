using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Tamos.AbleOrigin.AspNetCore
{
    /// <summary>
    /// Passport传输管理，如Cookie或Header承载
    /// </summary>
    public static class PassportTransport
    {
        private static readonly PassportStore PassportStore = new(); //PassportOptions.Current.StoreProvider ?? 
        
        /// <summary>
        /// Get passport from request context
        /// </summary>
        internal static Task<PassportInfo?> GetPassport(this HttpContext httpContext)
        {
            var token = GetToken(httpContext);
            return string.IsNullOrEmpty(token) ? Task.FromResult<PassportInfo?>(null) : PassportStore.GetPassport(token);
        }

        #region Public token manage

        /// <summary>
        /// 获取当前用户登录Token，未登录返回空
        /// </summary>
        public static string? GetToken(this HttpContext httpContext)
        {
            return WebUtility.CookieGet(httpContext, PassportOptions.Current.TokenCookieName);
        }

        /// <summary>
        /// 设置用户登录Token到Cookie中。
        /// </summary>
        public static void SetToken(this HttpContext httpContext, string token, DateTime expireTime, string cookieDomain)
        {
            //删除原有Cookie //httpContext.Response.Cookies.Remove(TokenCookieName);

            //设置域Cookie身份验证码
            WebUtility.CookieSet(httpContext, PassportOptions.Current.TokenCookieName, token, expireTime, cookieDomain);
        }

        /// <summary>
        /// Set passport from user id, return related token.
        /// </summary>
        public static string SetPassport(this HttpContext httpContext, long userId, TimeSpan expireSpan, string cookieDomain)
        {
            var passport = new PassportInfo
            {
                UserIdentity = userId
            };
            var token = PassportStore.StorePassport(passport, expireSpan);

            SetToken(httpContext, token, DateTime.Now.Add(expireSpan), cookieDomain);
            return token;
        }

        /// <summary>
        /// Set the custom passport object.
        /// </summary>
        public static string SetPassport(this HttpContext httpContext, PassportInfo passport, TimeSpan expireSpan, string cookieDomain)// where T : PassportInfo
        {
            var token = PassportStore.StorePassport(passport, expireSpan);

            SetToken(httpContext, token, DateTime.Now.Add(expireSpan), cookieDomain);
            return token;
        }

        /// <summary>
        /// 移除用户登录Token
        /// </summary>
        public static void RemoveToken(this HttpContext httpContext)
        {
            //获取现有Token，再删除
            var token = GetToken(httpContext);
            WebUtility.CookieDelete(httpContext, PassportOptions.Current.TokenCookieName);

            //移除Token存储
            if (!string.IsNullOrEmpty(token)) PassportStore.DeletePassport(token);
        }

        #endregion

        #region Map Route Action

        /// <summary>
        /// Map登录凭证的回调设置地址"/ppt-token"，单点登录站点与当前站点的根域名不同时，需要设置。
        /// </summary>
        public static IEndpointConventionBuilder MapPassportSet(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapGet("/ppt-token", async context =>
            {
                string token = context.Request.Query["tok"];
                string returl = context.Request.Query["returl"];
                var expMins = context.Request.GetQueryInt("exp", 12 * 60);

                //LogService.Debug($"跨域登录：tok:{token}, expMins:{expMins}, returl:{returl}");

                if (token.IsNull())
                {
                    await context.Response.WriteAsync("Token is null");
                    return;
                }

                //设置跨域名的cookie（cookie的域名是不区别端口的）
                //var cookieDomain = CentralConfiguration.GetAppSetting("PassportDomain", WebUtility.GetRootDomain(context.Request));
                var cookieDomain = CentralConfiguration.IsEnvDev()
                    ? WebUtility.GetDomain(returl, true) //Node devserver做代理时，请求域名不是浏览器上域名
                    : context.Request.GetRootDomain();
                context.SetToken(token, DateTime.Now.AddMinutes(expMins), cookieDomain);

                context.Response.Redirect(returl);
            });
        }

        #endregion
    }
}

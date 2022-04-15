using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.AspNetCore
{
    internal class PassportAuthHandler : IAuthenticationHandler
    {
        public const string SchemeName = "PassportScheme";

        protected AuthenticationScheme Scheme { get; private set; }
        protected HttpContext Context { get; private set; }

        #region Implementation of IAuthenticationHandler

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            Scheme = scheme;
            Context = context;
            return Task.CompletedTask;
        }

        public async Task<AuthenticateResult> AuthenticateAsync()
        {
            var ppt = await Context.GetPassport();
            //自定义验证
            if (PassportOptions.Current.AuthHandle != null)
            {
                var custHd = PassportOptions.Current.AuthHandle(Context, ppt);
                if (custHd.IsFail()) return AuthenticateResult.Fail(custHd.ErrorMsg!);
            }
            
            if (ppt == null) return AuthenticateResult.NoResult();

            //should set authenticationType to SchemeName，or User.Identity.IsAuthenticated is false.
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, ppt.UserIdentity.ToString())
            }, SchemeName);

            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name));
        }

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            Context.Response.Redirect("/login");
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            Context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        #endregion

        /// <summary>
        /// 尝试从Authenticate结果来获取UserId，避免重复从Passport服务（缓存）查询。
        /// </summary>
        internal static long GetUserId(ClaimsPrincipal? user)
        {
            if (user?.Identity?.IsAuthenticated != true) return 0;
            
            var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return idStr.ToLong();
        }
    }
}
using System;
using System.Threading.Tasks;

namespace Tamos.AbleOrigin.AspNetCore
{
    /// <summary>
    /// Passport store, saved by CacheService.（In redis if at cloud）
    /// </summary>
    internal class PassportStore
    {
        protected internal Task<PassportInfo?> GetPassport(string token)
        {
            //var watch = Stopwatch.StartNew();

            return CacheService.GetAsync<PassportInfo>(token);
            
            //watch.Stop();
            //LogService.InfoFormat("GetLoginCache time:{0}", watch.ElapsedMilliseconds);
        }

        #region Passport manage

        /// <summary>
        /// Store passport and return related token.
        /// </summary>
        protected internal string StorePassport(PassportInfo ppt, TimeSpan expireSpan)// where T : PassportInfo
        {
            var token = GenerateToken(ppt);

            CacheService.Set(token, ppt, expireSpan);
            return token;
        }

        protected internal void DeletePassport(string token)
        {
            if (string.IsNullOrEmpty(token)) return;
            CacheService.Delete(token);
        }

        /// <summary>
        /// Generate a new token.
        /// </summary>
        /// <returns></returns>
        protected string GenerateToken(PassportInfo ppt)
        {
            var srcToken = $"token{ppt.UserIdentity}-{DateTime.Now:yyMMddHHmmss}";
            return "tk_" + Encryptor.SHA1Encrypt(srcToken);
        }

        #endregion
    }

    /// <summary>
    /// Default passport store info
    /// </summary>
    public class PassportInfo
    {
        /// <summary>
        /// Common use for UserId
        /// </summary>
        public long UserIdentity { get; set; }

        /// <summary>
        /// Identify app or scope
        /// </summary>
        public long AppIdentity { get; set; }

        /// <summary>
        /// Extra data, such as NickName
        /// </summary>
        public string UserData { get; set; }
    }
}
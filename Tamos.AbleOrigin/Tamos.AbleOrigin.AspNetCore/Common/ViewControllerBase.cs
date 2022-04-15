using Microsoft.AspNetCore.Mvc;

namespace Tamos.AbleOrigin.AspNetCore
{
    /// <summary>
    /// 带View Controller基类。
    /// </summary>
    public class ViewControllerBase : Controller
    {
        #region Login User

        private long? _curUserId; //按需获取，有些Action并不访问UserId

        /// <summary>
        /// 登录用户Id
        /// </summary>
        public long CurUserId => _curUserId ??= HttpContext.User.GetUserId();

        #endregion


    }
}
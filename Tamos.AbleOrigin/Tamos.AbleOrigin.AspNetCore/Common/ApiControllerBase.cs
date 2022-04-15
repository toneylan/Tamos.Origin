using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.AspNetCore
{
    /// <summary>
    /// ApiController基类，没有view page。
    /// </summary>
    public abstract class ApiControllerBase : ControllerBase
    {
        #region Login User

        private long? _curUserId; //按需获取，有些Action并不访问UserId

        /// <summary>
        /// 登录用户Id
        /// </summary>
        public long CurUserId => _curUserId ??= HttpContext.User.GetUserId();

        #endregion

        #region Util

        /// <summary>
        /// Json的错误结果：{ ErrorMsg: 'error' }
        /// </summary>
        protected ActionResult Error(string? error)
        {
            return Ok(new GeneralRes {ErrorMsg = error});
        }

        /*/// <summary>
        /// Json的错误结果“{ error: res.ErrorMsg }”
        /// </summary>
        protected ActionResult Error(IGeneralResObj res)
        {
            return Ok(new GeneralRes { ErrorMsg = res.ErrorMsg });
        }*/

        /// <summary>
        /// 获取Route参数值
        /// </summary>
        protected string? GetRoutePara(string name)
        {
            return HttpContext.GetRouteValue(name)?.ToString();
        }

        #endregion
    }

    /*public class GeneralApiRes
    {
        public string? error { get; set; }
    }*/
}
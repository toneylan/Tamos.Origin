using System.Diagnostics.CodeAnalysis;

namespace Tamos.AbleOrigin.Payment.ShouQianBa
{
    internal class ResultBase
    {
        /// <summary>
        /// 通讯响应码,	200：通讯成功；400：客户端错误；500:服务端错误<br/>
        /// biz_response.result_code, 状态分为：SUCCESS、FAIL、INPROGRESS和 ERROR 四类
        /// </summary>
        public string result_code { get; set; }

        /// <summary>
        /// 通讯错误码, 通讯 失败 的时候才返回
        /// </summary>
        public string? error_code { get; set; }

        /// <summary>
        /// 通讯错误信息描述, 通讯 失败 的时候才返回
        /// </summary>
        public string? error_message { get; set; }
        
    }

    internal class Result<T> : ResultBase
    {
        public T? biz_response { get; set; }
        
        /// <summary>
        /// 是否请求成功，result_code == "200" and biz_response != null
        /// </summary>
        [MemberNotNullWhen(true, nameof(biz_response))]
        public bool IsReqSuccess => result_code == "200" & biz_response != null;

        /// <summary>
        /// 构造错误结果
        /// </summary>
        internal static Result<T> Error(string errorMsg)
        {
            return new Result<T>
            {
                result_code = "400",
                error_message = errorMsg
            };
        }
    }

    internal class BizData<T> : ResultBase
    {
        public T? data { get; set; }
    }
    
    /*internal class ResultData<T> : Result<BizData<T>>
    {
    }*/
}
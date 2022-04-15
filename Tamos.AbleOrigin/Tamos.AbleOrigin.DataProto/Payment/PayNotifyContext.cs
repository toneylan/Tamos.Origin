using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 包装Notify http请求的数据，以进行rpc调用传输。
    /// </summary>
    [DataContract]
    public class PayNotifyContext
    {
        /// <summary>
        /// Http body 内容
        /// </summary>
        [DataMember(Order = 6)]
        public string? Body { get; set; }

        /// <summary>
        /// Http Headers 所需部分，不是全部。
        /// </summary>
        [DataMember(Order = 7)]
        public Dictionary<string, string>? Headers { get; set; }

        /// <summary>
        /// 设置所需的Header数据，忽略无用条目。
        /// </summary>
        public static readonly List<string> UsedHeader = new()
        {
            "Authorization" //收钱吧
        };
    }
}
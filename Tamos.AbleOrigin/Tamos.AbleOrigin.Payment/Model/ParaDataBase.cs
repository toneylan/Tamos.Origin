using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Tamos.AbleOrigin.Payment
{
    /// <summary>
    /// 通用接口参数数据的基类，简化参数设置、加密、签名等操作
    /// </summary>
    public abstract class ParaDataBase
    {
        protected SortedDictionary<string, string> _paras = new SortedDictionary<string, string>();

        public void SetValue(string key, string value)
        {
            _paras[key] = value;
        }

        /// <summary>
        /// 将参数数据生成签名
        /// </summary>
        public abstract string MakeSign(string signSecret);

        /// <summary>
        /// 转为Http参数格式：“参数名=值” 用&连接
        /// </summary>
        public string ToHttpPara()
        {
            var sb = new StringBuilder();
            foreach (var p in _paras)
            {
                sb.AppendFormat("{0}{1}={2}", sb.Length > 0 ? "&" : null, p.Key, HttpUtility.UrlEncode(p.Value));
            }

            return sb.ToString();
        }
    }
}
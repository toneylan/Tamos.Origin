using System.Text.RegularExpressions;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.Connect.Weixin
{
    public class WxUserInfo
    {
        public string openid { get; set; }
        public string nickname { get; set; }

        /// <summary>
        /// 值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        public int sex { get; set; }
        public string language { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string? headimgurl { get; set; }
        public string? unionid { get; set; }

        public string? remark { get; set; }
        public int subscribe { get; set; }
        public long subscribe_time { get; set; }
        public int groupid { get; set; }

        public Gender GetGender() => sex switch
        {
            1 => Gender.Male,
            2 => Gender.Female,
            _ => Gender.Unknown
        };

        #region Static member

        public static string HeadImgUrl64(string origUrl)
        {
            if (string.IsNullOrEmpty(origUrl)) return origUrl;

            return Regex.Replace(origUrl, "/0$", "/64");
        }

        public static string HeadImgUrl132(string origUrl)
        {
            if (string.IsNullOrEmpty(origUrl)) return origUrl;

            return Regex.Replace(origUrl, "/0$", "/132");
        }

        #endregion
    }
}
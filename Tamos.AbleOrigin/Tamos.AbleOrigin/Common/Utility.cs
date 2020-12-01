using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Tamos.AbleOrigin.Common
{
    /// <summary>
    /// 提供经常使用的一些实用方法
    /// </summary>
    public static class Utility
    {
        public const string ProdBrand = "Tamos";
        internal const string ProdDllBooster = "Tamos.AbleOrigin.Booster";

        #region 数据合法性验证

        /*/// <summary>
        /// 判断是否是中文
        /// </summary>
        public static bool IsCN(string _value)
        {
            if (string.IsNullOrEmpty(_value)) return false;
            Regex reg = new Regex("^[\u4e00-\u9fa5]+$");
            return reg.IsMatch(_value);
        }*/

        /// <summary>
        /// 判断一个字符串是否为邮件
        /// </summary>
        /// <returns></returns>
        public static bool IsEmail(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            var regex = new Regex(@"^[A-Za-z0-9+]+[A-Za-z0-9\._\-+]*@([A-Za-z0-9\-]+\.)+[A-Za-z]+$", RegexOptions.IgnoreCase);
            return regex.Match(value).Success;
        }

        /*/// <summary>
        /// 判断一个字符串是否为ID格式
        /// </summary>
        public static bool IsIDCard(string _value)
        {
            if (_value == null || (_value.Length != 15 && _value.Length != 18))
            {
                return false;
            }
            Regex regex;
            string[] strArray;
            DateTime time;
            if (_value.Length == 15)
            {
                regex = new Regex(@"^(\d{6})(\d{2})(\d{2})(\d{2})(\d{3})$");
                if (!regex.Match(_value).Success)
                {
                    return false;
                }
                strArray = regex.Split(_value);
                try
                {
                    time = new DateTime(int.Parse("19" + strArray[2]), int.Parse(strArray[3]), int.Parse(strArray[4]));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            regex = new Regex(@"^(\d{6})(\d{4})(\d{2})(\d{2})(\d{3})([0-9Xx])$");
            if (!regex.Match(_value).Success)
            {
                return false;
            }
            strArray = regex.Split(_value);
            try
            {
                time = new DateTime(int.Parse(strArray[2]), int.Parse(strArray[3]), int.Parse(strArray[4]));
                return true;
            }
            catch
            {
                return false;
            }
        }*/

        /// <summary>
        /// 判断一个字符串是否为手机号码
        /// </summary>
        public static bool IsMobileNumber(string value)
        {
            return !string.IsNullOrEmpty(value) && Regex.IsMatch(value, @"^(13\d|14[57]|15\d|17[0678]|18\d)\d{8}$");
        }

        /// <summary>
        /// 判断是否为座机、手机等号码
        /// </summary>
        public static bool IsPhoneNumber(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            return IsMobileNumber(value) || Regex.IsMatch(@"^(\d{10,11}|(\d{3,4}-)?\d{7,8})(-\d{1,5})?$", value);
        }

        /// <summary>
        /// 检查一个字符串是否是纯数字构成的，一般用于查询字符串参数的有效性验证。
        /// </summary>
        public static bool IsNumeric(string value)
        {
            return Utility.QuickValidate("^[0-9]+$", value);
        }

        /// <summary>
        /// 判断一个字符串是否为网址
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUrl(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            var regex = new Regex(@"(http://)?([\w-]+\.)*[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.IgnoreCase);
            return regex.Match(value).Success;
        }

        /// <summary>
        /// 判断是否为默认时间
        /// </summary>
        public static bool IsDefaultDate(DateTime dateTime)
        {
            return dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue;
        }

        /// <summary>
        /// 快速验证一个字符串是否符合指定的正则表达式。
        /// </summary>
        public static bool QuickValidate(string _express, string _value)
        {
            if (string.IsNullOrEmpty(_value)) return false;

           return Regex.IsMatch(_express, _value);

            //var myRegex = new Regex(_express);
            //return myRegex.IsMatch(_value);
        }
        #endregion

        #region 字符串转换

        /// <summary>
        /// 把字符串转成日期
        /// </summary>
        public static DateTime StrToDate(string value, DateTime defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            DateTime dt;
            if (!DateTime.TryParse(value, out dt))
            {
                dt = defaultValue;
            }
            return dt;
        }

        /// <summary>
        /// 把字符串转成时间间隔
        /// </summary>
        public static TimeSpan StrToTimeSpan(string value, TimeSpan defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            TimeSpan dt;
            if (!TimeSpan.TryParse(value, out dt))
            {
                dt = defaultValue;
            }
            return dt;
        }

        /// <summary>
        /// 把字符串转成数字类型
        /// </summary>
        public static double StrToDouble(string value, double defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            double ret;
            return double.TryParse(value, out ret) ? ret : defaultValue;
        }

        /// <summary>
        /// 把字符串转成整型
        /// </summary>
        public static short StrToShort(string value, short defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            short ret;
            return short.TryParse(value, out ret) ? ret : defaultValue;
        }

        /// <summary>
        /// 把字符串转成整型
        /// </summary>
        public static int StrToInt(string value, int defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            int ret;
            return int.TryParse(value, out ret) ? ret : defaultValue;
        }

        /// <summary>
        /// 把字符串转成整型
        /// </summary>
        public static long StrToLong(string value, long defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            long ret;
            return long.TryParse(value, out ret) ? ret : defaultValue;
        }

        /// <summary>
        /// 把字符串转成Decimal
        /// </summary>
        public static decimal StrToDecimal(string value, decimal defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            decimal ret;
            return decimal.TryParse(value, out ret) ? ret : defaultValue;
        }

        #endregion

        #region 字符串重构

        /// <summary>
        /// 从逗号分隔的ID里取出int列表
        /// </summary>
        public static List<int> ParseIDListFromString(string idString)
        {
            return ParseIDListFromString(idString, ',');
        }

        /// <summary>
        /// 从逗号分隔的ID里取出long列表
        /// </summary>
        public static List<long> ParseLongIDListFromString(string idString)
        {
            return ParseLongIDListFromString(idString, ',');
        }

        /// <summary>
        /// 从指定分隔的字符串里取出正整数列表
        /// </summary>
        public static List<int> ParseIDListFromString(string idString, char split)
        {
            var retList = new List<int>();
            if (string.IsNullOrEmpty(idString)) return retList;

            var temp = idString.Split(split);
            foreach (var item in temp)
            {
                int ret;
                if (int.TryParse(item, out ret)) retList.Add(ret);
            }

            return retList;
        }

        /// <summary>
        /// 从指定分隔的字符串里取出正整数列表
        /// </summary>
        public static List<long> ParseLongIDListFromString(string idString, char split)
        {
            var retList = new List<long>();
            if (string.IsNullOrEmpty(idString)) return retList;

            var temp = idString.Split(split);
            foreach (var item in temp)
            {
                long ret;
                if (long.TryParse(item, out ret)) retList.Add(ret);
            }

            return retList;
        }

        /// <summary>
        /// 将字符串中间用*替代
        /// </summary>
        public static string BlurString(string str, int prefixLength = 3, int suffixLength = 4)
        {
            if (str == null || str.Length <= prefixLength + suffixLength) return str;

            var endIndex = str.Length - suffixLength;
            var startLength = Math.Min(prefixLength + 4, endIndex);
            return string.Concat(str.Substring(0, prefixLength).PadRight(startLength, '*'), str.Substring(endIndex));
        }

        #endregion

        #region 取随机数

        private static readonly Random RandomNumber = new Random();
        
        /// <summary>
        /// 生成指定长度的随机数
        /// </summary>
        public static string BuildRandomStr(int length)
        {
            var str = RandomNumber.Next().ToString(CultureInfo.InvariantCulture);

            if (str.Length > length)
            {
                str = str.Substring(0, length);
            }
            else if (str.Length < length)
            {
                str = str.PadLeft(length, '0');
            }

            return str;
        }

        #endregion

        /*#region 读取配置文件AppSettings

        /// <summary>
        /// 获取配置文件中AppSettings值：整数
        /// </summary>
        public static int GetConfigSetInt(string key, int defVal)
        {
            return int.TryParse(ConfigurationManager.AppSettings[key], out var ret) ? ret : defVal;
        }

        /// <summary>
        /// 获取配置文件中AppSettings
        /// </summary>
        public static string GetConfigSetString(string key, string defVal = null)
        {
            return ConfigurationManager.AppSettings[key] ?? defVal;
        }

        #endregion*/
    }
}

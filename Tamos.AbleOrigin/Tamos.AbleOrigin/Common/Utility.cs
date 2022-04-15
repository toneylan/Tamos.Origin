using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Tamos.AbleOrigin
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
		/// 检查是否是纯数字(不含小数)
		/// </summary>
		public static bool IsNumeric(string value)
		{
			return QuickValidate(value, "^[0-9]+$");
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
		/// 是否身份证号
		/// </summary>
        public static bool IsCnId(string value)
        {
			return Regex.IsMatch(value, @"^[1-9]\d{5}(18|19|20)\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$");
        }

		/// <summary>
		/// 快速验证一个字符串是否符合指定的正则表达式。
		/// </summary>
		private static bool QuickValidate(string _value, string _express)
		{
			if (string.IsNullOrEmpty(_value)) return false;

			return Regex.IsMatch(_value, _express);

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
			return value.ToDate(defaultValue);
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
            return value.ToInt(defaultValue);
		}

		/// <summary>
		/// 把字符串转成整型
		/// </summary>
		public static long StrToLong(string value, long defaultValue)
		{
            return value.ToLong(defaultValue);
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
		
		#region 取随机数

		private static readonly Random RandomObj = new ();

		/// <summary>
		/// 生成指定长度的随机数
		/// </summary>
		public static string BuildRandomStr(int length)
		{
			var str = RandomObj.Next().ToString();

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

		/// <summary>
		/// 生成指定范围的随机数
		/// </summary>
        public static int BuildRandom(int min, int max)
        {
            return RandomObj.Next(min, max);
        }

		#endregion

	}
}

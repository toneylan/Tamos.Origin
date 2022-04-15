using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tamos.AbleOrigin
{
    public static class StringExtend
    {
        /// <summary>
        /// 简写IsNullOrEmpty
        /// </summary>
        public static bool IsNull([NotNullWhen(false)] this string? str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 简写Not IsNullOrEmpty
        /// </summary>
        public static bool NotNull([NotNullWhen(true)] this string? str)
        {
            return !string.IsNullOrEmpty(str);
        }

        #region Convert to

        /// <summary>
        /// 转为int类型，支持null
        /// </summary>
        public static int ToInt(this string? str, int defValue = 0)
        {
            if (string.IsNullOrEmpty(str)) return defValue;

            return int.TryParse(str, out var ret) ? ret : defValue;
        }

        /// <summary>
        /// 转为long类型，支持null
        /// </summary>
        public static long ToLong(this string? str, long defValue = 0)
        {
            if (string.IsNullOrEmpty(str)) return defValue;

            return long.TryParse(str, out var ret) ? ret : defValue;
        }

        /// <summary>
        /// 转成日期类型
        /// </summary>
        public static DateTime ToDate(this string? value, DateTime defValue = default)
        {
            if (string.IsNullOrEmpty(value)) return defValue;

            return DateTime.TryParse(value, out var dt) ? dt : defValue;
        }
        
        /// <summary>
        /// 从指定分隔的字符串解析为long列表
        /// </summary>
        public static List<long> ParseLongIds(this string? idString, char split = ',')
        {
            var retList = new List<long>();
            if (string.IsNullOrEmpty(idString)) return retList;

            var temp = idString.Split(split);
            foreach (var item in temp)
            {
                if (long.TryParse(item, out var ret)) retList.Add(ret);
            }

            return retList;
        }

        /// <summary>
        /// 从指定分隔的字符串解析为int列表
        /// </summary>
        public static List<int> ParseIds(this string? idString, char split = ',')
        {
            var retList = new List<int>();
            if (string.IsNullOrEmpty(idString)) return retList;

            var temp = idString.Split(split);
            foreach (var item in temp)
            {
                if (int.TryParse(item, out var ret)) retList.Add(ret);
            }

            return retList;
        }

        /// <summary>
        /// 从指定分隔的枚举值列表字符串，解析为枚举列表。
        /// </summary>
        //[return: NotNullIfNotNull("enumsStr")]
        public static List<T>? ParseEnums<T>(this string? enumsStr, char split = ',') where T : Enum
        {
            if (enumsStr.IsNull()) return null;
            return enumsStr.ParseIds(split).ConvertAll(x => (T) Enum.ToObject(typeof(T), x));
        }

        #endregion

        #region 重构、Append
		
        /// <summary>
        /// 将字符串中间用*替代
        /// </summary>
        public static string? BlurString(string? str, int prefixLength = 3, int suffixLength = 4)
        {
            if (str == null || str.Length <= prefixLength + suffixLength) return str;

            var endIndex = str.Length - suffixLength;
            var startLength = Math.Min(prefixLength + 4, endIndex);
            return string.Concat(str.Substring(0, prefixLength).PadRight(startLength, '*'), str.Substring(endIndex));
        }
        
        /// <summary>
        /// 非空时才左右拼接字符
        /// </summary>
        public static string? Append(this string? str, string? left, string? right = null)
        {
            if (string.IsNullOrEmpty(str)) return str;

            return left + str + right;
        }

        /// <summary>
        /// 追加字符，非空时才添加分隔符separator
        /// </summary>
        public static string? SepAppend(this string? str, string? separator, string? right)
        {
            if (string.IsNullOrEmpty(str)) return right;

            return str + separator + right;
        }

        #endregion
        
    }
}
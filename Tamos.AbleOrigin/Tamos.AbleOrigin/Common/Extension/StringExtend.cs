namespace Tamos.AbleOrigin.Common
{
    public static class StringExtend
    {
        /// <summary>
        /// 简写IsNullOrEmpty
        /// </summary>
        public static bool IsNull(this string str)
        {
            return string.IsNullOrEmpty(str) || str == "null" || str == "NULL";
        }

        /// <summary>
        /// 简写Not IsNullOrEmpty
        /// </summary>
        public static bool NotNull(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 转为long类型，支持null
        /// </summary>
        public static long ToLong(this string str, long defValue = 0)
        {
            return Utility.StrToLong(str, defValue);
        }

        /// <summary>
        /// 转为int类型，支持null
        /// </summary>
        public static int ToInt(this string str, int defValue = 0)
        {
            return Utility.StrToInt(str, defValue);
        }

        #region Append

        /// <summary>
        /// 非空时才左右拼接字符
        /// </summary>
        public static string Append(this string str, string left, string right = null)
        {
            if (string.IsNullOrEmpty(str)) return str;

            return left + str + right;
        }

        /// <summary>
        /// 追加字符，非空时才添加分隔符separator
        /// </summary>
        public static string SepAppend(this string str, string separator, string right)
        {
            if (string.IsNullOrEmpty(str)) return right;

            return str + separator + right;
        }

        #endregion

        /// <summary>
        /// 简写RemoveNullOrEmpty
        /// </summary>
        public static string RemoveNull(this string str)
        {
            return string.IsNullOrEmpty(str) ? null : str.Trim().Replace("NULL", null).Replace("null", null);
        }
    }
}
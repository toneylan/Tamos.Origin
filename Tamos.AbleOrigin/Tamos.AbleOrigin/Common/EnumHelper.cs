using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Tamos.AbleOrigin.Log;

namespace Tamos.AbleOrigin.Common
{
    public static class EnumHelper
    {
        private static readonly Dictionary<Enum, string> DicEnumDesCache = new Dictionary<Enum, string>();

        public static bool IsDefined<T>(int val)
        {
            return System.Enum.IsDefined(typeof(T), val);
        }

        #region GetEnumDes

        /// <summary>
        /// 从枚举值的Description属性获取描述
        /// </summary>
        public static string GetEnumDes(Enum val)
        {
            //反射获取有性能损失，加入缓存
            string enumDes;
            if (DicEnumDesCache.TryGetValue(val, out enumDes)) return enumDes;

            // Get the enum value field
            var fi = val.GetType().GetField(val.ToString());
            if (fi == null) return val.ToString();

            // Get the Description attribute of the enum value
            enumDes = fi.GetCustomAttribute<DescriptionAttribute>(false) is DescriptionAttribute desAttr ? desAttr.Description : val.ToString();

            try //忽略并发的错误
            {
                DicEnumDesCache.SetValue(val, enumDes);
            }
            catch(Exception e)
            {
                LogService.WarnFormat("Add EnumDesCache fail:{0}", e.Message);
            }

            return enumDes;
        }

        /// <summary>
        /// 获取枚举值描述
        /// </summary>
        public static string GetDes(this Enum val)
        {
            return GetEnumDes(val);
        }

        #endregion

        #region To string

        /// <summary>
        /// 将逗号分隔的枚举值，解析为枚举列表
        /// </summary>
        public static List<T> ParseList<T>(string valStr) where T : Enum
        {
            return valStr.IsNull() ? null : Utility.ParseIDListFromString(valStr).ConvertAll(x => (T) Enum.ToObject(typeof(T), x));
        }

        /// <summary>
        /// 转为逗号分隔的枚举值字符串
        /// </summary>
        public static string ToJoinStr<T>(this ICollection<T> enums) where T : Enum
        {
            return enums?.Count > 0 ? string.Join(",", enums.Select(GetShort)) : null;
        }

        #endregion

        #region Value convert

        /// <summary>
        /// 将枚举类型转为short。对确切的枚举类型值，直接类型转换，无需使用此方法。
        /// </summary>
        public static short GetShort<T>(T val) where T : Enum
        {
            return Convert.ToInt16(val);
        }

        #endregion

        public static List<T> GetEnumItemList<T>()
        {
            var list = new List<T>();
            foreach (T statusName in Enum.GetValues(typeof(T)))
            {
                list.Add(statusName);
            }
            return list;
        }

        public static List<(T Val, string Des)> GetEnumItems<T>()
        {
            var list = new List<(T, string)>();
            foreach (T val in Enum.GetValues(typeof(T)))
            {
                list.Add((val, GetDes(val as Enum)));
            }
            return list;
        }
    }
}
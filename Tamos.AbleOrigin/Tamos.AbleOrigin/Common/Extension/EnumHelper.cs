using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Tamos.AbleOrigin
{
    public static class EnumHelper
    {
        private static readonly Dictionary<Enum, string> DicEnumDesCache = new();

        /// <summary>
        /// Check if val is defined in the Enum type.
        /// </summary>
        public static bool IsDefined(this Enum val)
        {
            return Enum.IsDefined(val.GetType(), val);
        }

        #region GetEnumDes

        /// <summary>
        /// 从枚举值的Description属性获取描述
        /// </summary>
        public static string GetDes(this Enum val) //where T : Enum
        {
            //反射获取有性能损失，加入缓存
            if (DicEnumDesCache.TryGetValue(val, out var enumDes)) return enumDes;

            // Get the enum value field
            var fi = val.GetType().GetField(val.ToString());
            if (fi == null) return val.ToString();

            // Get the Description attribute of the enum value
            enumDes = fi.GetCustomAttribute<DescriptionAttribute>(false)?.Description ?? val.ToString();

            try //忽略并发的错误
            {
                DicEnumDesCache.TryAdd(val, enumDes);
            }
            catch(Exception e)
            {
                LogService.WarnFormat("Add EnumDesCache fail:{0}", e.Message);
            }

            return enumDes;
        }

        #endregion

        #region Value convert
        
        /// <summary>
        /// 转为逗号分隔的枚举值字符串
        /// </summary>
        [return: NotNullIfNotNull("enums")]
        public static string? ToJoinStr<T>(this IReadOnlyCollection<T>? enums) where T : Enum
        {
            return enums != null ? string.Join(",", enums.Select(e => e.GetShort())) : null;
        }

        /// <summary>
        /// 将枚举类型转为short。对确切的枚举类型值，直接类型转换，无需使用此方法。
        /// </summary>
        public static short GetShort(this Enum val) //where T : Enum
        {
            return Convert.ToInt16(val);
        }

        #endregion
        
        /// <summary>
        /// 获取枚举类型的值列表
        /// </summary>
        public static List<(T Val, string Des)> GetValues<T>() where T : Enum
        {
            var list = new List<(T, string)>();
            foreach (T val in Enum.GetValues(typeof(T)))
            {
                list.Add((val, GetDes(val)));
            }
            return list;
        }
    }
}
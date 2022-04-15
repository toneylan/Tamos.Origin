using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tamos.AbleOrigin
{
    public static class ListExtend
    {
        public static void ForEach<T>(this IEnumerable<T>? list, Action<T> action)
        {
            if (list == null) return;
            foreach (var item in list)
            {
                action(item);
            }
        }

        /// <summary>
        /// 列表是否为null或Count==0
        /// </summary>
        public static bool IsNull<T>([NotNullWhen(false)] this IReadOnlyCollection<T>? list)
        {
            return list == null || list.Count == 0;
        }

        /// <summary>
        /// Collection not null and has item
        /// </summary>
        public static bool NotNull<T>([NotNullWhen(true)] this IReadOnlyCollection<T>? list)
        {
            return list?.Count > 0;
        }
        
        #region 快捷筛选

        /// <summary>
        /// 按条件筛选的同时，余下项放入out分组
        /// </summary>
        public static List<T>? Separate<T>([AllowNull] this IEnumerable<T>? list, Func<T, bool> filter, out List<T>? otherItems)
        {
            otherItems = null;
            if (list == null) return null;

            var res = new List<T>();
            foreach (var item in list)
            {
                if (filter(item)) res.Add(item);
                else (otherItems ??= new List<T>()).Add(item);
            }

            return res;
        }

        #endregion

        #region Nullable Oper

        public static IEnumerable<T>? NullableConcat<T>(this IEnumerable<T>? list, IEnumerable<T>? second)
        {
            if (list == null) return second;
            if (second == null) return list;

            return list.Concat(second);
        }

        public static List<T>? NullableAdd<T>(this List<T>? list, IEnumerable<T>? append)
        {
            if (append == null) return list;
            
            list ??= new List<T>();
            list.AddRange(append);
            return list;
        }

        public static List<T>? NullableAdd<T>(this List<T>? list, T? item)
        {
            if (item == null) return list;
            list ??= new List<T>();
            list.Add(item);
            return list;
        }

        /// <summary>
        /// 支持空引用的SelectMany方法
        /// </summary>
        public static IEnumerable<TResult>? NullableSelectMany<T, TResult>(this IEnumerable<T>? list, Func<T, IEnumerable<TResult>> selector)
        {
            if (list == null) return null;

            IEnumerable<TResult> res = null;
            foreach (var item in list)
            {
                res = res.NullableConcat(selector(item));
            }
            return res;
        }

        #endregion

    }
}
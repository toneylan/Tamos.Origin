using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Tamos.AbleOrigin
{
    public static class TypeExtend
    {
        #region Dictionary

        /*/// <summary>
        /// [可改用GetValueOrDefault]key不存在时返回默认值
        /// </summary>
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.GetValueOrDefault(key, out var val) ? val : default;
        }*/

        /// <summary>
        /// 添加/更新key对应的值
        /// </summary>
        public static TValue SetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue val)
        {
            //dictionary[key!] = val; 并发设置可能会报错
            if (dictionary.ContainsKey(key!)) dictionary[key] = val;
            else dictionary.Add(key, val);
            return val;
        }

        /// <summary>
        /// 获取/添加key对应的值
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
        {
            if (dictionary.TryGetValue(key, out var val)) return val;

            val = new TValue();
            dictionary.Add(key, val);
            return val;
        }

        #endregion

        #region Property expression

        public static void SetPropertyValue<T, TValue>(T target, Expression<Func<T, TValue>> memberLamda, TValue value)
        {
            var memberExpression = memberLamda.Body as MemberExpression;
            if (memberExpression == null) return;

            var property = memberExpression.Member as PropertyInfo;
            if (property != null)
            {
                property.SetValue(target, value);
            }
        }

        /// <summary>
        /// 获取属性的设置方法
        /// </summary>
        public static Action<T, TProperty> GetPropertySetter<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var property = (PropertyInfo)memberExpression.Member;

            return GetPropertySetter<T, TProperty>(property);
        }

        /// <summary>
        /// 获取属性的设置方法
        /// </summary>
        public static Action<T, TProperty> GetPropertySetter<T, TProperty>(PropertyInfo property)
        {
            var setMethod = property.GetSetMethod();
            if (setMethod == null) throw new Exception($"属性“{property.Name}”缺少set方法");

            var parameterT = Expression.Parameter(typeof(T), "x");
            var parameterTProperty = Expression.Parameter(typeof(TProperty), "y");

            var newExpression = Expression.Lambda<Action<T, TProperty>>(
                Expression.Call(parameterT, setMethod, parameterTProperty),
                parameterT, parameterTProperty
            );

            return newExpression.Compile();
        }

        /// <summary>
        /// 获取属性的Get方法
        /// </summary>
        public static Func<T, TProperty> GetPropertyGetter<T, TProperty>(PropertyInfo prop)
        {
            var paraExpT = Expression.Parameter(typeof(T), "x");

            var newExpression = Expression.Lambda<Func<T, TProperty>>(Expression.Property(paraExpT, prop), paraExpT);

            return newExpression.Compile();
        }

        #endregion

        /// <summary>
        /// 获取FullName，为空（哪种类型？）则用Name。
        /// </summary>
        public static string GetFullName(this Type type)
        {
            return type.FullName ?? type.Name;
        }
    }
}
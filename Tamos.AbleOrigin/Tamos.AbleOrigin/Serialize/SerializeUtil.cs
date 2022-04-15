using System;
using System.Diagnostics.CodeAnalysis;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 序列化工具
    /// </summary>
    public static class SerializeUtil
    {
        #region Provider

        private static IJsonSerializer _jsonSerializer;

        private static IJsonSerializer JsonSerializer => _jsonSerializer ??= ServiceLocator.GetOrReflect<IJsonSerializer>("JsonSerializer");

        #endregion

        #region Json

        /// <summary>
        /// 序列化对象为Json字符串
        /// </summary>
        [return: NotNullIfNotNull("obj")]
        public static string? ToJson(object? obj)
        {
            if (obj == null) return null;
            try
            {
                return JsonSerializer.Serialize(obj);
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return null;
            }
        }

        /// <summary>
        /// 从Json字符串反序列化到对象
        /// </summary>
        public static T? FromJson<T>(string? jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr)) return default;
            try
            {
                return JsonSerializer.Deserialize<T>(jsonStr);
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return default;
            }
        }

        /*public static T DeAnonymousType<T>(string jsonStr, T anonymousObject)
        {
            if (string.IsNullOrEmpty(jsonStr)) return anonymousObject;
            try
            {
                return JsonConvert.DeserializeAnonymousType(jsonStr, anonymousObject);
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return anonymousObject;
            }
        }*/

        #endregion
    }
}
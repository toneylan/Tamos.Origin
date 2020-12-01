using System;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.IOC;
using Tamos.AbleOrigin.Log;

namespace Tamos.AbleOrigin.Cache
{
    /// <summary>
    /// 分布式缓存服务，当前采用Redis
    /// </summary>
    public static class CacheService
    {
        private static readonly ICacheProvider Provider = ServiceLocator.GetOrReflect<ICacheProvider>("RedisCache");

        /// <summary>
        /// 是否开启miss时日志记录
        /// </summary>
        public static bool LogMiss { get; set; }

        #region Get/Set

        /// <summary>
        /// 获取缓存的数据
        /// </summary>
        public static T Get<T>(string key)
        {
            try
            {
                return Provider.Get<T>(key);
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return default(T);
            }
        }

        /// <summary>
        /// 设置缓存数据
        /// </summary>
        public static void Set<T>(string key, T data, TimeSpan? expireSpan = null)
        {
            try
            {
                Provider.Set(key, data, expireSpan);
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
        }

        /// <summary>
        /// 删除指定的缓存项
        /// </summary>
        public static void Delete(string key)
        {
            try
            {
                Provider.Delete(key);
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
        }

        #endregion

        /// <summary>
        /// 从缓存中获取数据，若miss则回调数据并插入缓存。
        /// </summary>
        public static T Get<T>(string key, Func<T> funGetData, TimeSpan? expireSpan)
        {
            var res = Get<T>(key);
            if (res != null) return res;

            res = funGetData();
            if (res != null) Set(key, res, expireSpan);

            if (LogMiss) LogService.DebugFormat("GetCache<{0}> Key<{1}> miss then reload.", typeof(T).Name, key);
            return res;
        }
    }
}
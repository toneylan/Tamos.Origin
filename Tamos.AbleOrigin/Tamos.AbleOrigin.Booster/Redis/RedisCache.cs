using System;
using CSRedis;
using Tamos.AbleOrigin.Cache;
using Tamos.AbleOrigin.Configuration;
using Tamos.AbleOrigin.Log;

namespace Tamos.AbleOrigin.Booster
{
    internal class RedisCache : ICacheProvider, IDisposable
    {
        internal static string HostAddress => ServiceAddressConfig.GetExternalSrvSet("RedisServer", "host.docker.internal:6379");

        private readonly CSRedisClient _csredis;

        public RedisCache()
        {
            _csredis = new CSRedisClient($"{HostAddress},defaultDatabase=6"); //,prefix={Utility.ProdBrand}_
        }

        #region Implementation of ICacheProvider

        public T Get<T>(string key)
        {
            return _csredis.Get<T>(key);
        }

        public void Set<T>(string key, T data, TimeSpan? expireSpan)
        {
            var resTask = expireSpan != null ? _csredis.SetAsync(key, data, (int) expireSpan.Value.TotalSeconds) : _csredis.SetAsync(key, data);
            if (!resTask.Result) LogService.ErrorFormat("缓存插入失败：{0}, {1}", key, typeof(T).FullName);
            //else LogService.DebugFormat("缓存插入：{0}, {1}", key, expireSpan?.TotalMinutes);
        }

        /// <summary>
        /// 删除指定的缓存项
        /// </summary>
        public void Delete(string key)
        {
            _csredis.DelAsync(key);
        }

        #endregion
        
        #region IDisposable

        public void Dispose()
        {
            _csredis?.Dispose();
        }

        #endregion
    }
}
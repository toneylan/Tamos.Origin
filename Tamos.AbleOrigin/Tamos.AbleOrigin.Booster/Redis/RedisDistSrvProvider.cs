using System;
using System.Collections.Generic;
using CSRedis;
using Tamos.AbleOrigin.Log;

namespace Tamos.AbleOrigin.Booster
{
    internal class RedisDistSrvProvider : IDistributedSrvProvider, IDisposable
    {
        private readonly CSRedisClient _redisClient;

        public RedisDistSrvProvider()
        {
            _redisClient = new CSRedisClient($"{RedisCache.HostAddress},defaultDatabase=7");
        }

        #region Lock

        /// <summary>
        /// 开启分布式锁
        /// </summary>
        /// <param name="name">锁名称</param>
        /// <param name="value">锁的值，用于区分调用方</param>
        /// <param name="lockTimeout">锁在多久后，即使未释放也会自动超时释放</param>
        /// <returns>是否获取锁成功</returns>
        public bool Lock(string name, string value, TimeSpan lockTimeout)
        {
            return _redisClient.Set(name, value, lockTimeout, RedisExistence.Nx);
        }

        /// <summary>
        /// 释放分布式锁。https://github.com/2881099/csredis/blob/master/src/CSRedisCore/CSRedisClient.cs
        /// </summary>
        /// <returns>成功/失败</returns>
        public bool Unlock(string name, string value)
        {
            return _redisClient.Eval(@"local gva = redis.call('GET', KEYS[1])
if gva == ARGV[1] then
  redis.call('DEL', KEYS[1])
  return 1
end
return 0", name, value)?.ToString() == "1";
        }

        #endregion

        #region Hash

        /*/// <summary>
        /// 为哈希表 key 中字段 field 的值加上增量 value。如果key/field不存在，会自动创建。
        /// </summary>
        /// <returns>结果值</returns>
        public long HashIncr(string key, string field, long value)
        {
            return _redisClient.HIncrBy(key, field, value);
        }*/

        /// <summary>
        /// 为哈希表 key 中字段 field 的值加上增量 value。如果key/field不存在，会自动创建。
        /// </summary>
        /// <returns>结果值</returns>
        public decimal HashIncr(string key, string field, decimal value)
        {
            return _redisClient.HIncrByFloat(key, field, value);
        }

        /// <summary>
        ///  获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        public Dictionary<string, T> HashGetAll<T>(string key)
        {
            return _redisClient.HGetAll<T>(key);
        }

        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        public int HashDel(string key, params string[] fields)
        {
            return (int) _redisClient.HDel(key, fields);
        }

        #endregion

        #region DistEvent

        /*public void Publish<T>(string topic, T eventMsg)
        {
            _csredis.PublishAsync(topic, SerializeUtil.ToJson(eventMsg));
        }

        public IDisposable Subscribe<T>(string topic, Action<T> handler)
        {
            return _csredis.Subscribe((topic, msgArg =>
            {
                handler(SerializeUtil.FromJson<T>(msgArg.Body));
            }));
        }*/

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _redisClient.Dispose();
        }

        #endregion
    }
}
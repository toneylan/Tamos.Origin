using CSRedis;

namespace Tamos.AbleOrigin.Booster;

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

    #region Common func

    public bool Exists(string key)
    {
        return _redisClient.Exists(key);
    }

    public bool Expire(string key, TimeSpan expire)
    {
        return _redisClient.Expire(key, expire);
    }

    #endregion

    #region Hash

    /// <inheritdoc />
    public bool HashSet<T>(string key, string field, T value, bool notExists = false)
    {
        return notExists
            ? _redisClient.HSetNx(key, field, value)
            : _redisClient.HSet(key, field, value);
    }

    /// <inheritdoc />
    public decimal HashIncr(string key, string field, decimal value)
    {
        return _redisClient.HIncrByFloat(key, field, value);
    }

    /// <inheritdoc />
    public T? HashGet<T>(string key, string field)
    {
        return _redisClient.HGet<T>(key, field);
    }

    /// <inheritdoc />
    public Dictionary<string, T> HashGetAll<T>(string key)
    {
        return _redisClient.HGetAll<T>(key);
    }

    /// <inheritdoc />
    public int HashDel(string key, params string[] fields)
    {
        return (int) _redisClient.HDel(key, fields);
    }

    #endregion

    #region Zset
        
    public int ZCard(string key)
    {
        return (int)_redisClient.ZCard(key);
    }

    public int ZAdd(string key, params (decimal, object)[] scoreMembers)
    {
        return (int)_redisClient.ZAdd(key, scoreMembers);
    }
        
    public T[] ZRangeByScore<T>(string key, decimal min, decimal max, long? count = null, long offset = 0)
    {
        return _redisClient.ZRangeByScore<T>(key, min, max, count, offset);
    }
        
    public int ZRemRangeByScore(string key, decimal min, decimal max)
    {
        return (int)_redisClient.ZRemRangeByScore(key, min, max);
    }
        
    public int ZRem<T>(string key, params T[] member)
    {
        return (int)_redisClient.ZRem(key, member);
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
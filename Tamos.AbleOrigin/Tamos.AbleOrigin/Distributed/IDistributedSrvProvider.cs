namespace Tamos.AbleOrigin;

public interface IDistributedSrvProvider
{
    /// <summary>
    /// 开启分布式锁
    /// </summary>
    /// <param name="name">锁名称</param>
    /// <param name="value">锁的值，用于区分调用方</param>
    /// <param name="lockTimeout">锁在多久后，即使未释放也会自动超时释放</param>
    /// <returns>是否获取锁成功</returns>
    bool Lock(string name, string value, TimeSpan lockTimeout);

    /// <summary>
    /// 释放分布式锁
    /// </summary>
    bool Unlock(string name, string value);

    /// <summary>
    /// 检查给定 key 是否存在
    /// </summary>
    bool Exists(string key);

    /// <summary>
    /// 为给定 key 设置过期时间
    /// </summary>
    bool Expire(string key, TimeSpan expire);

    /// <summary>
    /// 为哈希表 key 中字段 field 的值加上增量 value。如果key/field不存在，会自动创建。
    /// </summary>
    /// <returns>如果是新建的字段返回true，如果是修改旧值则返回false。</returns>
    bool HashSet<T>(string key, string field, T value, bool notExists = false);

    /// <summary>
    /// 为哈希表 key 中字段 field 的值加上增量 value。如果key/field不存在，会自动创建。
    /// </summary>
    /// <returns>结果值</returns>
    decimal HashIncr(string key, string field, decimal value);

    /// <summary>
    ///  获取在哈希表中指定字段的值
    /// </summary>
    T? HashGet<T>(string key, string field);

    /// <summary>
    ///  获取在哈希表中指定 key 的所有字段和值
    /// </summary>
    Dictionary<string, T> HashGetAll<T>(string key);

    /// <summary>
    /// 删除一个或多个哈希表字段
    /// </summary>
    int HashDel(string key, params string[] fields);


    /// <summary>
    /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
    /// </summary>
    int ZAdd(string key, params (decimal, object)[] scoreMembers);

    /// <summary>
    /// 通过分数返回有序集合指定区间内的成员
    /// </summary>
    T[] ZRangeByScore<T>(string key, decimal min, decimal max, long? count = null, long offset = 0);

    /// <summary>
    /// 移除有序集合中给定的分数区间的所有成员
    /// </summary>
    int ZRemRangeByScore(string key, decimal min, decimal max);

    /// <summary>
    /// 移除有序集合中的一个或多个成员
    /// </summary>
    int ZRem<T>(string key, params T[] member);

    /// <summary>
    /// 获取有序集合的成员数量
    /// </summary>
    int ZCard(string key);

    /* Pub/Sub
     void Publish<T>(string topic, T eventMsg);

    IDisposable Subscribe<T>(string topic, Action<T> handler);*/
}
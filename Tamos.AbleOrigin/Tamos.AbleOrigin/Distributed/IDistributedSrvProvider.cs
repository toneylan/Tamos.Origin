using System;
using System.Collections.Generic;

namespace Tamos.AbleOrigin
{
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

        /*/// <summary>
        /// 为哈希表 key 中字段 field 的值加上增量 value。如果key/field不存在，会自动创建。
        /// </summary>
        /// <returns>结果值</returns>
        long HashIncr(string key, string field, long value);*/

        /// <summary>
        /// 为哈希表 key 中字段 field 的值加上增量 value。如果key/field不存在，会自动创建。
        /// </summary>
        /// <returns>结果值</returns>
        decimal HashIncr(string key, string field, decimal value);

        /// <summary>
        ///  获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        Dictionary<string, T> HashGetAll<T>(string key);

        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        int HashDel(string key, params string[] fields);

        /* Pub/Sub
         void Publish<T>(string topic, T eventMsg);

        IDisposable Subscribe<T>(string topic, Action<T> handler);*/
    }
}
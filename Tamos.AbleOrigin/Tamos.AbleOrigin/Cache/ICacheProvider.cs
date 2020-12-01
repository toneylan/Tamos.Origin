using System;

namespace Tamos.AbleOrigin.Cache
{
    /// <summary>
    /// 定义分布式缓存
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        T Get<T>(string key);

        /// <summary>
        /// 设置缓存数据
        /// </summary>
        void Set<T>(string key, T data, TimeSpan? expireSpan);

        /// <summary>
        /// 删除指定的缓存项
        /// </summary>
        void Delete(string key);
    }
}
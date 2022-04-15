using System;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 缓存规则的基类
    /// </summary>
    public abstract class CacheRule
    {
        /// <summary>
        /// 缓存Key前缀，直接用时：TypeName+RuleName，GroupCacheRule中则为空。
        /// </summary>
        internal string? Prefix { get; set; }

        /// <summary>
        /// 不设置时，会被设置随机15-30天。<br/>
        /// 不过期方案，可能造成弃用数据始终留存。
        /// </summary>
        public TimeSpan Expire { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// 常规DbContext的Creater。
    /// </summary>
    internal interface IDbCreater<out T> : IDisposable where T : BaseDbContext
    {
        /// <summary>
        /// 获取Db实例
        /// </summary>
        T GetDb();
    }

    /// <summary>
    /// 分表DbContext的Creater。
    /// </summary>
    internal interface IShardingDbCreater<out T> : IDisposable where T : ShardingDbContext
    {
        /// <summary>
        /// 按Scope获取ShardingDb
        /// </summary>
        T By(ContextScope scope);

        /// <summary>
        /// 按时间获取ShardingDb
        /// </summary>
        T By(DateTime date);

        /// <summary>
        /// 获取时间范围的ShardingDb，时间倒序排列
        /// </summary>
        IReadOnlyList<T> By(DateTime start, DateTime end, bool avoidFutureTime = true);

        /// <summary>
        /// 按Id解析时间以获取Db
        /// </summary>
        T By(long id);

        /// <summary>
        /// Id列表解析时间范围，以获取Db
        /// </summary>
        IReadOnlyList<T> By(IReadOnlyCollection<long> ids);
    }
}
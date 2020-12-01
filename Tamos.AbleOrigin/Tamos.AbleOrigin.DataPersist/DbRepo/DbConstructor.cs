using System;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// 分表Db的构造方法（解决目前的泛型，不支持带参构造函数约束）
    /// </summary>
    internal class ShardDbConstructor<T> where T : ShardingDbContext
    {
        /// <summary>
        /// 创建Db的方法
        /// </summary>
        public Func<ContextScope, T> NewDb { get; set; }

        /// <summary>
        /// 创建事务中的Db
        /// </summary>
        public Func<ContextScope, DbTransactionContext, T> NewDbInTran { get; set; }
    }
}
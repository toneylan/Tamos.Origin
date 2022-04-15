using System;
using System.Collections.Generic;

namespace Tamos.AbleOrigin.DataPersist
{
    internal class ShardingDbConfig
    {
        public ShardScopeType ScopeType { get; }

        public List<EntityTypeSet> ShardEntitys { get; }
        
        /// <summary>
        /// 检查过的表Postfix
        /// </summary>
        public HashSet<string> CheckedPostfix { get; }
        
        public ShardingDbConfig(ShardScopeType scopeType)
        {
            ShardEntitys = new List<EntityTypeSet>();
            CheckedPostfix = new HashSet<string>();
            ScopeType = scopeType;
        }
    }

    internal class EntityTypeSet
    {
        public Type EntityType { get; set; }
        
        public string TableBaseName { get; set; }
    }

    #region ShardingTypeDefine

    /*/// <summary>
    /// 特定类型ShardingDb配置，派生泛型类，便于Dictionary保存ShardingDbConfig。
    /// </summary>
    internal class ShardingDbTypeDefine<T> : ShardingDbConfig  where T : ShardingDbContext
    {
        /// <summary>
        /// 创建实例的方法定义
        /// </summary>
        internal ShardDbConstructor<T> DbConstructor { get; set; }

    }*/

    /*/// <summary>
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
        public Func<ContextScope, DbTransactionSet, T> NewDbInTran { get; set; }
    }*/

    #endregion
}
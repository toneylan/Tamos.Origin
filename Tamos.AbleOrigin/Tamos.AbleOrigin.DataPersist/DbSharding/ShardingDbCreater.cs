using System;
using System.Collections.Generic;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// ShardingDb的创建辅助类，创建的Db会缓存起来，跟随Creater一起释放。
    /// </summary>
    public class ShardingDbCreater<T> : ServiceComponent, IShardingDbCreater<T> where T : ShardingDbContext, new()
    {
        #region Static ctor

        //按默认的ShardScope Year注册当前Db类型的配置，其他ShardScope可考虑通过Attribute来配置。
        //这里Static ctor利用了泛型的各具体类型，都会分别执行一次。
        static ShardingDbCreater()
        {
            DataPersistConfig.RegDb<T>(ShardScopeType.Year);
        }

        #endregion

        private readonly ShardingDbConfig _dbConfig = DataPersistConfig.GetDbConfig(typeof(T));
        
        #region Create Db

        /// <summary>
        /// 创建新的Db实例
        /// </summary>
        private T Create(ContextScope scope)
        {
            if (scope.ScopeType != _dbConfig.ScopeType) throw new Exception($"{typeof(T).Name} ShardScope is {_dbConfig.ScopeType}, Can't use {scope.ScopeType}!");

            return new T {Scope = scope};
            /*if (_dbConfig.DbConstructor.NewDb != null)
            {
                return _dbConfig.DbConstructor.NewDb(scope);
            }

            //反射创建
            return Activator.CreateInstance(typeof(T), scope) as T;*/
        }

        /// <summary>
        /// 获取或新建实例
        /// </summary>
        private T GetDb(ContextScope scope)
        {
            var typeKey = typeof(T).GetFullName() + scope.TableSuffix;

            return GetComponent(() => Create(scope), typeKey);
        }

        #endregion

        #region 获取ShardingDb

        /// <summary>
        /// 按Scope获取ShardingDb
        /// </summary>
        public T By(ContextScope scope)
        {
            return GetDb(scope);
        }

        /// <summary>
        /// 按时间获取ShardingDb
        /// </summary>
        public T By(DateTime date)
        {
            return GetDb(Scope(date));
        }

        /// <summary>
        /// 获取时间范围的ShardingDb，时间倒序排列
        /// </summary>
        public IReadOnlyList<T> By(DateTime start, DateTime end, bool avoidFutureTime = true)
        {
            var scopes = Scope(start, end, avoidFutureTime);
            return scopes.ConvertAll(By);
        }
        
        /// <summary>
        /// 按Id解析时间以获取Db
        /// </summary>
        public T By(long id)
        {
            return GetDb(Scope(DataIdBuilder.ParseDate(id)));
        }

        /// <summary>
        /// Id列表解析时间范围，以获取Db
        /// </summary>
        public IReadOnlyList<T> By(IReadOnlyCollection<long> ids)
        {
            var (start, end) = DataIdBuilder.ParseDateRange(ids);
            return Scope(start, end).ConvertAll(By);
        }

        #endregion

        #region Db事务

        /// <summary>
        /// 创建事务中的ShardingDb，需要自主管理生存周期，即用后释放。
        /// </summary>
        public T InTran(ContextScope scope, DbTransactionSet tran)
        {
            var db = new T {Scope = scope, TransactionSet = tran};
            db.UseTransaction(); //设置Scope后才可调用
            return db;
        }

        /*/// <summary>
        /// 创建事务中的Db，执行后Db会被释放（用了using）
        /// </summary>
        public TRes InTran<TRes>(long id, DbTransactionSet tran, Func<T, TRes> operateFunc)
        {
            using var db = InTran(Scope(DataIdBuilder.ParseDate(id)), tran);
            return operateFunc(db);
        }*/

        #endregion

        #region Scope

        /// <summary>
        /// 按时间创建Scope
        /// </summary>
        internal ContextScope Scope(DateTime date)
        {
            return ContextScope.Create(_dbConfig.ScopeType, date);
        }

        /// <summary>
        /// 按时间范围创建Scope，范围错误时，可能为空。
        /// </summary>
        internal List<ContextScope> Scope(DateTime start, DateTime end, bool avoidFutureTime = true)
        {
            return ContextScope.Create(_dbConfig.ScopeType, start, end, avoidFutureTime);
        }

        #endregion
    }
}
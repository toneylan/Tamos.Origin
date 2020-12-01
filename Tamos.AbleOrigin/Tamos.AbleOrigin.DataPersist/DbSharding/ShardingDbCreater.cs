using System;
using System.Collections.Generic;
using Tamos.AbleOrigin.DataProto;
using Tamos.AbleOrigin.ServiceBase;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// ShardingDb的创建辅助类，创建的Db会缓存起来，跟随Creater一起释放。
    /// </summary>
    public class ShardingDbCreater<T> : BaseServiceComponent, IShardingDbCreater<T> where T : ShardingDbContext
    {
        //private readonly BaseServiceComponent _srvComponent; //利用来缓存Db
        private readonly ShardingDbTypeDefine<T> _dbConfig;

        public ShardingDbCreater()
        {
            //_srvComponent = service;
            _dbConfig = ShardingDbContext.GetDbConfig(typeof(T)) as ShardingDbTypeDefine<T>;
            if (_dbConfig == null) throw new Exception($"未获取到{typeof(T).FullName}配置，请检查是否调用RegDb<T>进行注册！");
        }

        #region Create Db

        /// <summary>
        /// 创建新的Db实例
        /// </summary>
        private T Create(ContextScope scope)
        {
            if (scope.ScopeType != _dbConfig.ScopeType) throw new Exception($"{typeof(T).Name} ShardScope is {_dbConfig.ScopeType}, Can't use {scope.ScopeType}!");

            if (_dbConfig.DbConstructor.NewDb != null)
            {
                return _dbConfig.DbConstructor.NewDb(scope);
            }

            //反射创建
            return Activator.CreateInstance(typeof(T), scope) as T;
        }

        /// <summary>
        /// 获取或新建实例
        /// </summary>
        private T GetDb(ContextScope scope)
        {
            var type = typeof(T);
            var typeKey = type.Namespace + type.Name + scope.TablePostfix;

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
        /// 创建事务中的Db，执行后Db会被释放（用了using）
        /// </summary>
        public TRes InTran<TRes>(long id, DbTransactionContext tran, Func<T, TRes> operateFunc)
        {
            if (_dbConfig.DbConstructor.NewDbInTran == null) throw new Exception($"{typeof(T).Name}没有定义带事务参数的构造函数，请在RegDb方法中添加设置。");

            using var db = _dbConfig.DbConstructor.NewDbInTran(Scope(DataIdBuilder.ParseDate(id)), tran);
            return operateFunc(db);
        }

        #endregion

        #region Scope

        /// <summary>
        /// 按时间创建Scope
        /// </summary>
        public ContextScope Scope(DateTime date)
        {
            return ContextScope.Create(_dbConfig.ScopeType, date);
        }

        /// <summary>
        /// 按时间范围创建Scope，范围错误时，可能为空。
        /// </summary>
        public List<ContextScope> Scope(DateTime start, DateTime end, bool avoidFutureTime = true)
        {
            return ContextScope.Create(_dbConfig.ScopeType, start, end, avoidFutureTime);
            //if (scopes.IsNullOrEmpty()) scopes = scopes.NullableAdd(Scope(start));
        }

        #endregion
    }
}
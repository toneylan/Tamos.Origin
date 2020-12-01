using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Tamos.AbleOrigin.ServiceBase;

namespace Tamos.AbleOrigin.DataPersist
{
    public abstract class ShardingDbContext : BaseDbContext
    {
        public ContextScope Scope { get; }

        #region Static member

        private static readonly Dictionary<string, ShardingDbConfig> DbTypeConfigs = new Dictionary<string, ShardingDbConfig>(); //派生DbContext的配置信息

        /// <summary>
        /// 注册Db类型，并设置Db类型信息（设置后可使用ShardingDbCreater）
        /// </summary>
        protected static void RegDb<T>(ShardScopeType scopeType, Func<ContextScope, T> funCreate, Func<ContextScope, DbTransactionContext, T> createInTran = null) 
            where T : ShardingDbContext
        {
            //add new config
            var dbType = typeof (T);
            var conf = new ShardingDbTypeDefine<T>(scopeType)
            {
                DbConstructor = new ShardDbConstructor<T>
                {
                    NewDb = funCreate,
                    NewDbInTran = createInTran
                }
            };
            DbTypeConfigs.Add(dbType.Name, conf);

            #region Reflect table entity

            var dbSetType = typeof (DbSet<>);
            foreach (var prop in dbType.GetProperties())
            {
                var propType = prop.PropertyType;
                if (!propType.IsGenericType || propType.GetGenericTypeDefinition() != dbSetType) continue;

                var entType = propType.GetGenericArguments()[0];
                var shardAtt = entType.GetCustomAttribute<ShardingTableAttribute>();
                if (shardAtt == null) continue;

                //分表的Entity
                conf.ShardEntitys.Add(new EntityTypeSet
                {
                    EntityType = entType,
                    TableBaseName = shardAtt.Name
                });
            }

            #endregion
        }
        
        //获取DbContext配置
        internal static ShardingDbConfig GetDbConfig(Type dbType)
        {
            return DbTypeConfigs[dbType.Name];
        }

        /// <summary>
        /// 确保执行DbType的静态构造函数，以初始化配置
        /// </summary>
        internal static void EnsureDbTypeReg(Type dbType)
        {
            if (DbTypeConfigs.ContainsKey(dbType.Name)) return;

            RuntimeHelpers.RunClassConstructor(dbType.TypeHandle); //手动执行静态构造函数
        }

        /*/// <summary>
        /// 创建DbCreater（主要用于子类调用，以确保静态构造函数被调用-RegDb）
        /// </summary>
        protected static ShardingDbCreater<T> Creater<T>(BaseServiceComponent service) where T : ShardingDbContext
        {
            return new ShardingDbCreater<T>(service);
        }*/

        #endregion

        #region Ctor

        //执行顺序Ctor、OnConfiguring、OnModelCreating

        protected ShardingDbContext(ContextScope scope, string connectionString) : base(connectionString)
        {
            Scope = scope;
            CheckTableCreate(connectionString);
        }

        protected ShardingDbContext(ContextScope scope, DbTransactionContext dbTran) : base(dbTran.Connection)
        {
            Scope = scope;
            UseTransaction(dbTran); //设置Scope后才可调用
            
            CheckTableCreate(dbTran.ConnectionString);
        }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.ReplaceService<IModelCacheKeyFactory, DynamicModelKeyFactory>(); //自定义分表的Model缓存
        }

        #region Model config
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //DbContext配置
            var dbConf = GetDbConfig(GetType());
            
            //映射分表名称
            var tblPostfix = Scope.TablePostfix;
            foreach (var entitySet in dbConf.ShardEntitys)
            {
                modelBuilder.Entity(entitySet.EntityType, b => b.ToTable(entitySet.TableBaseName + tblPostfix));
            }
        }
        
        #endregion

        #region CheckTableCreate

        public void CheckTableCreate(string connectionString)
        {
            if (Scope.OriginalPart) return;

            //当前DbContext配置
            var dbConf = GetDbConfig(GetType());
            var tblPostfix = Scope.TablePostfix;
            if (dbConf.CheckedPostfix.Contains(tblPostfix)) return; //是否检查过当前TablePostfix的表

            //check table create
            var sbSql = new StringBuilder();
            foreach (var entSet in dbConf.ShardEntitys)
            {
                //for Mysql
                sbSql.AppendLine($"CREATE TABLE IF NOT EXISTS `{entSet.TableBaseName + tblPostfix}` LIKE `{entSet.TableBaseName}`;");
            }

            //！！新连接中执行Sql，否则CREATE TABLE语句会破坏DbTransaction（直接提交无法回滚）
            if (sbSql.Length > 0) RunSql(connectionString, sbSql.ToString());

            //记录，避免重复检查表创建
            dbConf.CheckedPostfix.Add(tblPostfix);
        }

        #endregion
    }

    #region ShardingDbConfig

    internal class ShardingDbConfig
    {
        public ShardScopeType ScopeType { get; protected set; }

        public List<EntityTypeSet> ShardEntitys { get; }
        
        /// <summary>
        /// 检查过的表Postfix
        /// </summary>
        public HashSet<string> CheckedPostfix { get; }
        
        public ShardingDbConfig()
        {
            ShardEntitys = new List<EntityTypeSet>();
            CheckedPostfix = new HashSet<string>();
        }
    }

    /// <summary>
    /// 特定类型ShardingDb配置，派生泛型类，便于Dictionary保存ShardingDbConfig。
    /// </summary>
    internal class ShardingDbTypeDefine<T> : ShardingDbConfig  where T : ShardingDbContext
    {
        /// <summary>
        /// 创建实例的方法定义
        /// </summary>
        internal ShardDbConstructor<T> DbConstructor { get; set; }

        public ShardingDbTypeDefine(ShardScopeType scopeType)
        {
            ScopeType = scopeType;
        }
    }

    internal class EntityTypeSet
    {
        public Type EntityType { get; set; }
        
        public string TableBaseName { get; set; }
    }

    #endregion
}
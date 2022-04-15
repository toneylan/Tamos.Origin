using System;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// ShardingDb派生类型，统一使用ShardingDbCreater创建实例，不单独new构造。
    /// </summary>
    public abstract class ShardingDbContext : BaseDbContext
    {
        // ReSharper disable once NotNullMemberIsNotInitialized
        public ContextScope Scope { get; internal init; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (Scope == null) throw new Exception("ShardingDbContext  missing ContextScope set.");

            //检查分表创建
            var conStr = TransactionSet == null ? ConnectionString : TransactionSet.ConnectionString;
            CheckTableCreate(conStr);

            optionsBuilder.ReplaceService<IModelCacheKeyFactory, DynamicModelKeyFactory>(); //自定义分表的Model缓存
        }

        #region Model config
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //DbContext配置
            var dbConf = DataPersistConfig.GetDbConfig(GetType());
            
            //映射分表名称
            var tblPostfix = Scope.TableSuffix;
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
            var dbConf = DataPersistConfig.GetDbConfig(GetType());
            var tblPostfix = Scope.TableSuffix;
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
}
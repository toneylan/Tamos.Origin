using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tamos.AbleOrigin.DataPersist;

public abstract class BaseDbContext : DbContext
{
    #region Propertys

    /// <summary>
    /// 获取连接字符串
    /// </summary>
    protected internal abstract string ConnectionString { get; }

    /// <summary>
    /// 要使用的外部数据库事务
    /// </summary>
    internal DbTransactionSet? TransactionSet { get; init; }

    #endregion

    #region Ctor & Configure
        
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (TransactionSet != null)
        {
            //Warning: Can't call UseTransaction in OnConfiguring.
            optionsBuilder.UseMySql(TransactionSet.Connection, DataPersistConfig.GetDbVersion(TransactionSet.ConnectionString), MysqlOptSet);
        }
        else
        {
            optionsBuilder.UseMySql(ConnectionString, DataPersistConfig.GetDbVersion(ConnectionString), MysqlOptSet);
        }

        //Json config
        static void MysqlOptSet(MySqlDbContextOptionsBuilder options) => options.UseMicrosoftJson();

        base.OnConfiguring(optionsBuilder);
    }

    #endregion

    #region Transaction

    public DbTransactionSet BeginTransaction()
    {
        return new(Database);
    }

    internal void UseTransaction()
    {
        if (TransactionSet != null) Database.UseTransaction(TransactionSet.Transaction.GetDbTransaction());
    }

    #endregion

    #region EntityDbMap

    /// <summary>
    /// 获取PO到数据库的映射关系
    /// </summary>
    public EntityDbMap GetMapPO<T>()
    {
        return EntityDbMap.GetOrAdd<T>(() =>
        {
            var entityType = Model.FindEntityType(typeof(T));
            if (entityType == null) throw new Exception($"从{GetType().Name}获取{typeof(T)}的配置失败");
            return entityType;
        });
    }

    #endregion

    /*#region Attach

    /// <summary>
    /// 附加对象到DbContext，以进行修改设置
    /// </summary>
    public void SafeAttach<T>(T entity, EntityState? state = null) where T : class
    {
        var entry = Entry(entity);
        if (state != null)
        {
            entry.State = state.Value;
        }
        else if (entry.State == EntityState.Detached)
        {
            entry.State = EntityState.Unchanged; //等于Attach操作
        }
    }

    #endregion*/

    #region Util

    /// <summary>
    /// 打开新的DbContext来执行Sql语句
    /// </summary>
    internal static void RunSql(string connectionString, string sql)
    {
        using var db = new DbContext(GetDbOpts(connectionString));
        db.Database.ExecuteSqlRaw(sql);
    }
        
    private static DbContextOptions GetDbOpts(string connectionString)
    {
        var builder = new DbContextOptionsBuilder();
        builder.UseMySql(connectionString, DataPersistConfig.GetDbVersion(connectionString));
        return builder.Options;
    }

    #endregion
}
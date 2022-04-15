using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// 封装数据库事务的信息
    /// </summary>
    public class DbTransactionSet : IDisposable
    {
        internal IDbContextTransaction Transaction { get; }

        internal DbConnection Connection { get; }

        internal string ConnectionString { get; }

        internal DbTransactionSet(DatabaseFacade database)
        {
            Connection = database.GetDbConnection();
            ConnectionString = Connection.ConnectionString; //开启事务后，ConnectionString中会失去密码，这里提前缓存。

            Transaction = database.BeginTransaction();
        }

        public void Commit()
        {
            Transaction.Commit();
        }

        public void Rollback()
        {
            Transaction.Rollback();
        }

        public void Dispose()
        {
            Transaction.Dispose();
        }
    }
}
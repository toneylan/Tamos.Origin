using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tamos.AbleOrigin.DataPersist
{
    public class DbTransactionContext : IDisposable
    {
        public IDbContextTransaction Transaction { get; }

        public DbConnection Connection { get; }

        internal string ConnectionString { get; }

        public DbTransactionContext(DatabaseFacade database)
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
            Transaction?.Dispose();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Tamos.AbleOrigin.DataPersist
{
    internal class DynamicModelKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        {
            var dbType = context.GetType();
            return context is ShardingDbContext shardDb
                ? dbType.FullName + shardDb.Scope.TablePostfix
                : dbType.FullName;
        }
    }
}
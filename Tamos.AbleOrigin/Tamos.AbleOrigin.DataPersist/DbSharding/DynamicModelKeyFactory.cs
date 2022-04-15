using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Tamos.AbleOrigin.DataPersist
{
    internal class DynamicModelKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        {
            var typeName = context.GetType().GetFullName();
            return context is ShardingDbContext shardDb
                ? typeName + shardDb.Scope.TableSuffix
                : typeName;
        }
    }
}
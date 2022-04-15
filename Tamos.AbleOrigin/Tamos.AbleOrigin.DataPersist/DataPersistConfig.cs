using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Tamos.AbleOrigin.DataPersist
{
    public static class DataPersistConfig
    {
        private static readonly Dictionary<string, ShardingDbConfig> DbTypeConfigs = new(); //派生DbContext的配置信息
        private static readonly Dictionary<string, ServerVersion> DbVersionCache = new();

        #region DbConfig
        
        /// <summary>
        /// 注册Db类型，并设置Db类型信息（设置后可使用ShardingDbCreater）
        /// </summary>
        internal static void RegDb<T>(ShardScopeType scopeType) where T : ShardingDbContext
        {
            //add new config
            var dbType = typeof (T);
            var conf = new ShardingDbConfig(scopeType);
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
            return DbTypeConfigs.GetValueOrDefault(dbType.Name) ??
                   throw new Exception($"未获取到{dbType.FullName}配置，请检查是否调用RegDb<T>进行注册！");
        }

        /*/// <summary>
        /// 确保执行DbType的静态构造函数，以初始化配置
        /// </summary>
        internal static void EnsureDbTypeReg(Type dbType)
        {
            if (DbTypeConfigs.ContainsKey(dbType.Name)) return;

            RuntimeHelpers.RunClassConstructor(dbType.TypeHandle); //手动执行静态构造函数
        }*/

        #endregion
        
        #region ServerVersion
        
        internal static ServerVersion GetDbVersion(string? conStr)
        {
            if (conStr.IsNull()) throw new Exception("DbOptionSet中缺少数据库连接设置，无法获取ServerVersion");

            if (DbVersionCache.TryGetValue(conStr, out var ver)) return ver;
            ver = ServerVersion.AutoDetect(conStr);
            return DbVersionCache.SetValue(conStr, ver);
        }

        #endregion
    }
}
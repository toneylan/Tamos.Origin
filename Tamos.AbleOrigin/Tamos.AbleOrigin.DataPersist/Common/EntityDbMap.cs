using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Tamos.AbleOrigin.Common;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// 实体与数据库的Map信息
    /// </summary>
    public class EntityDbMap
    {
        #region Static member

        private static readonly Dictionary<Type, EntityDbMap> MapCache = new Dictionary<Type, EntityDbMap>();

        internal static EntityDbMap GetOrAdd<T>(Func<IEntityType> getEntity)
        {
            var key = typeof(T);
            if (MapCache.TryGetValue(key, out var map)) return map;

            //新建
            map = new EntityDbMap(getEntity());
            return MapCache.SetValue(key, map);
        }

        #endregion

        private readonly Dictionary<string, string> _colMap = new Dictionary<string, string>();

        /// <summary>
        /// 分表Entity时不可用
        /// </summary>
        public string TableName { get; set; }

        //entityType是从DbContext实例中获取，不要进行保存，不确定是否影响DbContext释放。
        private EntityDbMap(IEntityType entityType)
        {
            TableName = entityType.GetTableName();
            //记录各列名Map
            foreach (var property in entityType.GetProperties())
            {
                _colMap.Add(property.Name, property.GetColumnName());
            }
        }

        /// <summary>
        /// 获取属性名对应的数据库列名
        /// </summary>
        public string Col(string propName)
        {
            return _colMap.GetValue(propName);
        }
    }
}
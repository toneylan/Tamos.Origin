using System;
using System.Collections.Generic;
using System.Reflection;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.Log;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 一个Entity类型的元数据
    /// </summary>
    public class EntityMeta<T> : EntityMeta //where T : class
    {
        private readonly Dictionary<string, BasePropMeta<T>> PropMetas = new Dictionary<string, BasePropMeta<T>>();

        internal EntityMeta()
        {
            //初始化属性配置
            foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty))
            {
                var propSet = property.GetCustomAttribute<PropMetaAttribute>() ?? DefaultPropSets.GetValue(property.Name);
                if (propSet == null) continue;

                Prop(property, propSet);
            }
        }

        #region Property get/set

        private BasePropMeta<T> Prop(PropertyInfo prop, PropMetaAttribute propSet)
        {
            //用PropertyInfo反射创建Meta
            var meta = Activator.CreateInstance(typeof(PropertyMeta<,>).MakeGenericType(typeof(T), prop.PropertyType), prop, propSet) as BasePropMeta<T>;
            if (meta != null) PropMetas.Add(prop.Name, meta);
            
            return meta;
        }

        /// <summary>
        /// 按属性名获取对应类型元信息。<br/>
        /// 默认只包含了带PropMeta标记的属性。
        /// </summary>
        public PropertyMeta<T, TProp> GetProp<TProp>(string name, bool withNoMeta = false)
        {
            return GetProp(name, withNoMeta) as PropertyMeta<T, TProp>;
        }

        /// <summary>
        /// 按属性名获取元信息。<br/>
        /// 默认只包含了带PropMeta标记的属性。
        /// </summary>
        public BasePropMeta<T> GetProp(string name, bool withNoMeta = false)
        {
            var propMeta = PropMetas.GetValue(name);
            if (propMeta != null || !withNoMeta) return propMeta;

            //重新查找，针对无PropMeta标记的属性
            var prop = typeof(T).GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
            if (prop == null)
            {
                LogService.ErrorFormat("[EntityMeta]无法获取类型{0}的属性：{1}", typeof(T).FullName, name);
                return null;
            }

            return Prop(prop, new PropMetaAttribute(name));
        }

        #endregion

        internal string Validate(T entity)
        {
            if (entity == null) return "数据对象为空";
            if (PropMetas.Count == 0) return null;

            //检查每个属性配置
            foreach (var meta in PropMetas.Values)
            {
                var error = meta.Validate(entity);
                if (error != null) return error;
            }

            return null;
        }
    }
}
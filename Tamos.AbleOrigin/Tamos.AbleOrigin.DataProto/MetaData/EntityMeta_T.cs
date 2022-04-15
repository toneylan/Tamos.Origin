using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 一个Entity类型的元数据
    /// </summary>
    public class EntityMeta<T> : EntityMeta //where T : class
    {
        private readonly Dictionary<string, BasePropMeta<T>> PropMetas = new();

        internal EntityMeta(bool allProp)
        {
            //初始化属性配置
            foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty))
            {
                var propAttr = property.GetCustomAttribute<PropMetaAttribute>() ?? DefaultPropSets.GetValueOrDefault(property.Name);
                if (propAttr == null)
                {
                    if(!allProp) continue;
                    propAttr = new PropMetaAttribute(property.Name);
                }

                Prop(property, propAttr);
            }
        }

        #region Property get/set

        private BasePropMeta<T>? Prop(PropertyInfo prop, PropMetaAttribute propAttr)
        {
            //用PropertyInfo反射创建Meta
            var meta = Activator.CreateInstance(typeof(PropertyMeta<,>).MakeGenericType(typeof(T), prop.PropertyType), prop, propAttr) as BasePropMeta<T>;
            if (meta != null) PropMetas.Add(prop.Name, meta);
            
            return meta;
        }
        
        /// <summary>
        /// 按属性名获取Meta信息。
        /// </summary>
        public BasePropMeta<T> GetProp(string name)
        {
            var propMeta = PropMetas.GetValueOrDefault(name);
            if (propMeta != null) return propMeta!;

            //重新查找，针对无PropMeta标记的属性
            var prop = typeof(T).GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
            if (prop == null)
            {
                LogService.ErrorFormat("[EntityMeta]无法获取类型{0}的属性：{1}", typeof(T).GetFullName(), name);
                throw new Exception($"[EntityMeta]无法获取类型{typeof(T).GetFullName()}的属性:{name}");
            }
            
            return Prop(prop, new PropMetaAttribute(name))!;
        }
        
        /// <summary>
        /// 按属性名获取对应类型元信息。<br/>
        /// 默认只包含了带PropMeta标记的属性。
        /// </summary>
        public PropertyMeta<T, TProp> GetProp<TProp>(string name)
        {
            return (PropertyMeta<T, TProp>) GetProp(name);
        }

        /// <summary>
        /// Get prop meta by expression
        /// </summary>
        public PropertyMeta<T, TProp> GetProp<TProp>(Expression<Func<T, TProp>> expProp)
        {
            var prop = (PropertyInfo)((MemberExpression) expProp.Body).Member;
            return (PropertyMeta<T, TProp>) GetProp(prop.Name);
        }

        /// <summary>
        /// 获取已使用的PropMeta，可能没有类型T的全部属性。
        /// </summary>
        public IReadOnlyCollection<BasePropMeta<T>> GetProps()
        {
            return PropMetas.Values;
        }

        #endregion
        
        /*#region Property reflect

        /// <summary>
        /// 按属性名获取元信息。
        /// </summary>
        private BasePropMeta<T> ReflectProp(string name, bool withNoMeta = false)
        {
            
        }

        #endregion*/

        internal string? Validate(T? entity)
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
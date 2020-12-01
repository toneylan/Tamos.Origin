using System;
using System.Reflection;
using Tamos.AbleOrigin.Common;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 属性的元数据基类
    /// </summary>
    public abstract class BasePropMeta<TEnt>
    {
        protected internal PropMetaAttribute AttrSet { get; protected set;}

        /// <summary>
        /// 属性名
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string CnName => AttrSet.CnName;

        /// <summary>
        /// 获取实例中的属性值
        /// </summary>
        public abstract object GetValue(TEnt entity);

        /// <summary>
        /// 实体中属性值是否有效
        /// </summary>
        internal abstract string Validate(TEnt entity);
    }

    /// <summary>
    /// 特定类型属性的元数据
    /// </summary>
    public class PropertyMeta<TEnt, TProp> : BasePropMeta<TEnt> //where TEnt : class
    {
        //直接使用函数，比PropertyInfo反射效率高
        public Func<TEnt, TProp> Getter { get; }
        public Action<TEnt, TProp> Setter { get; }

        private PropertyValidator<TProp> _validator;
        private PropertyValidator<TProp> Validator => _validator ??= EntityMeta.GetValidator(this);
        
        #region Ctor

        /*internal static PropertyMeta<TEnt, TProp> By(Expression<Func<TEnt, TProp>> expGet)
        {
            var prop = ((MemberExpression) expGet.Body).Member as PropertyInfo;
            return new PropertyMeta<TEnt, TProp>(prop)
            {
                _getter = expGet.Compile()
            };
        }*/

        public PropertyMeta(PropertyInfo prop, PropMetaAttribute attr)
        {
            Name = prop.Name;
            AttrSet = attr;

            Getter = TypeExtend.GetPropertyGetter<TEnt, TProp>(prop);
            Setter = TypeExtend.GetPropertySetter<TEnt, TProp>(prop);
        }

        #endregion

        #region Get & Validate

        public override object GetValue(TEnt entity)
        {
            return Getter(entity);
        }

        //依据PropMetaAttribute设置检查有效性
        internal override string Validate(TEnt entity)
        {
            //忽略未设置检查条件的
            if (!AttrSet.IsRequired && AttrSet.MaxValue == 0) return null;

            return Validator.Validate(AttrSet, Getter(entity));
        }
        
        #endregion
    }
}
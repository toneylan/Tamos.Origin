using System;
using System.Collections.Generic;
using Tamos.AbleOrigin.Common;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// Entity元数据基类，配置入口。
    /// </summary>
    public abstract class EntityMeta
    {
        private static readonly Dictionary<string, EntityMeta> EntityMetas = new Dictionary<string, EntityMeta>();

        static EntityMeta()
        {
            //通用默认配置
            DefaultProp("Id", "编号");
            DefaultProp("Name", "名称");
            DefaultProp("Title", "标题");
            DefaultProp("CreateTime", "创建时间");
            DefaultProp("LastUpdateTime", "最后修改");
        }

        #region Meta set

        /// <summary>
        /// 获取实体类的元设置
        /// </summary>
        public static EntityMeta<T> Get<T>()
        {
            var key = typeof(T).FullName ?? typeof(T).Name;
            if (EntityMetas.TryGetValue(key, out var meta)) return meta as EntityMeta<T>;

            //缺少时新增
            var crMeta = new EntityMeta<T>();
            EntityMetas.SetValue(key, crMeta);
            return crMeta;
        }

        #endregion

        #region DefaultProp set

        protected static readonly Dictionary<string, PropMetaAttribute> DefaultPropSets = new Dictionary<string, PropMetaAttribute>();

        /// <summary>
        /// 定义一些默认的属性设置，减少每次的单独标记。
        /// </summary>
        public static void DefaultProp(string name, string cnName)
        {
            DefaultPropSets.SetValue(name, new PropMetaAttribute(cnName));
        }

        /*internal static PropMetaAttribute GetDefaultProp(string name)
        {
            return DefaultPropSets.GetValue(name);
        }*/

        #endregion

        #region Validator set

        //使用单例，避免创建过多实例
        private static readonly IntPropValidator IntValidator = new IntPropValidator();
        private static readonly LongPropValidator LongValidator = new LongPropValidator();
        private static readonly StrPropValidator StrValidator = new StrPropValidator();
        private static readonly PropertyValidator<DateTime> TimeValidator = new PropertyValidator<DateTime>();

        /// <summary>
        /// 获取对应类型的Validator
        /// </summary>
        internal static PropertyValidator<TProp> GetValidator<TEnt, TProp>(PropertyMeta<TEnt, TProp> meta)
        {
            return meta switch
            {
                PropertyMeta<TEnt, int> _ => IntValidator as PropertyValidator<TProp>,
                PropertyMeta<TEnt, long> _ => LongValidator as PropertyValidator<TProp>,
                PropertyMeta<TEnt, string> _ => StrValidator as PropertyValidator<TProp>,
                PropertyMeta<TEnt, DateTime> _ => TimeValidator as PropertyValidator<TProp>,
                _ => new PropertyValidator<TProp>()
            };
        }

        #endregion
    }
}
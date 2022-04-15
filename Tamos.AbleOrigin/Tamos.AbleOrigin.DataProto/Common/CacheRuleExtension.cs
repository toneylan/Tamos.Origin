using System;

namespace Tamos.AbleOrigin.DataProto
{
    public static class CacheRuleExtension
    {
        #region Prop rule

        /// <summary>
        /// 在Trace上追加PropCacheRule。
        /// </summary>
        public static PropCacheRule<T> Prop<T>(this CacheTrace<T> trace, string propName)
        {
            return trace.AddRule(propName, EntityMeta.Get<T>().PropRule(propName));
        }

        /// <summary>
        /// 在Trace上追加PropCacheRule。
        /// </summary>
        public static PropCacheRule<T> Prop<T>(this CacheTrace<T> trace, string name, Func<T, string> propGet)
        {
            return trace.AddRule(name, new PropCacheRule<T>(propGet));
        }

        /// <summary>
        /// 加入基于主键的Cache规则。
        /// </summary>
        public static PropCacheRule<T> Key<T>(this CacheTrace<T> trace) where T : IGeneralEntity
        {
            return Prop(trace, nameof(IGeneralEntity.Id), e => e.Id.ToString());
        }

        //---- private
        private static PropCacheRule<T> PropRule<T>(this EntityMeta<T> meta, string propName)
        {
            return new PropCacheRule<T>(meta.GetProp(propName).GetValueAsString);
        }

        private static ConstCacheRule<T> ConstRule<T>(this EntityMeta<T> meta, (string name, string val) constProp)
        {
            return new ConstCacheRule<T>(meta.GetProp(constProp.name).GetValueAsString, constProp.val);
        }

        #endregion
        
        #region Group rule

        /// <summary>
        /// 加入GroupRule：按属性名字。
        /// </summary>
        public static ICacheRuleGet2 Group<T>(this CacheTrace<T> trace, string prop1, string prop2)
        {
            var meta = EntityMeta.Get<T>();
            var rule = new GroupCacheRule<T>(meta.PropRule(prop1), meta.PropRule(prop2));

            return trace.AddRule($"{prop1}-{prop2}", rule);
        }

        /// <summary>
        /// 加入GroupRule：属性 + ConstRule。
        /// </summary>
        public static ICacheRuleGet Group<T>(this CacheTrace<T> trace, string prop1, (string name, string val) constProp)
        {
            var meta = EntityMeta.Get<T>();
            var rule = new GroupCacheRule<T>(meta.PropRule(prop1), meta.ConstRule(constProp));

            return trace.AddRule($"{prop1}-{constProp.name}", rule);
        }

        /// <summary>
        /// 加入GroupRule：按属性名字。
        /// </summary>
        public static ICacheRuleGet3 Group<T>(this CacheTrace<T> trace, string prop1, string prop2, string prop3)
        {
            var meta = EntityMeta.Get<T>();
            var rule = new GroupCacheRule<T>(meta.PropRule(prop1), meta.PropRule(prop2), meta.PropRule(prop3));

            return trace.AddRule($"{prop1}-{prop2}-{prop3}", rule);
        }

        #endregion
    }
}
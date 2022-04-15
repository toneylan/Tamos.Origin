using System;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 基于单个属性（列）的Cache规则
    /// </summary>
    public class PropCacheRule<T> : CacheRule, ICacheRule<T>, ICacheRuleGet
    {
        protected readonly Func<T, string> PropGet;

        public PropCacheRule(Func<T, string> propGet)
        {
            PropGet = propGet;
        }
        
        /// <summary>
        /// 当前规则对entity是否有效。
        /// </summary>
        internal virtual bool IsValid(T entity) => true;

        public virtual string RuleKey(T entity)
        {
            return Prefix + PropGet(entity);
        }
        
        /// <summary>
        /// 按属性值生成Key
        /// </summary>
        private string CreateKey<TVal>(TVal propValue)
        {
            if (propValue is not string valStr) valStr = propValue?.ToString();

            return Prefix + valStr;
        }

        public TRes? Get<TVal, TRes>(TVal propValue, Func<TRes?> loadData)
        {
            var key = CreateKey(propValue);
            return CacheService.Get(key, loadData, Expire);
        }
    }
}
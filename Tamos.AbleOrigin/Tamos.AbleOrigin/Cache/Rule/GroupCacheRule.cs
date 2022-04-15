using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 组合的Cache规则
    /// </summary>
    public class GroupCacheRule<T> : CacheRule, ICacheRule<T>, ICacheRuleGet, ICacheRuleGet2, ICacheRuleGet3
    {
        private readonly PropCacheRule<T>[] Rules;

        public GroupCacheRule(params PropCacheRule<T>[] rules)
        {
            Rules = rules;
        }

        public string? RuleKey(T entity)
        {
            return Rules.Any(x => !x.IsValid(entity))
                ? null
                : Prefix + string.Join('-', Rules.Select(x => x.RuleKey(entity)));
        }

        #region Get for multiple prop value
        
        //内部不定value个数获取
        private TRes? InnerGet<TRes>(Func<TRes?> loadData, params string[] values)
        {
            //create key
            var idx = 0;
            var key = Prefix + string.Join('-', Rules.Select(rule =>
                rule is ConstCacheRule<T> cr ? cr.CreateKey() : values[idx++]));

            return CacheService.Get(key, loadData, Expire);
        }

        public TRes? Get<TVal, TRes>(TVal propValue, Func<TRes?> loadData)
        {
            return InnerGet(loadData, propValue!.ToString()!);
        }

        public TRes? Get<TRes>(string value1, string value2, Func<TRes?> loadData)
        {
            return InnerGet(loadData, value1, value2);
        }

        public TRes? Get<TRes>(string value1, string value2, string value3, Func<TRes?> loadData)
        {
            return InnerGet(loadData, value1, value2, value3);
        }

        #endregion
    }
}
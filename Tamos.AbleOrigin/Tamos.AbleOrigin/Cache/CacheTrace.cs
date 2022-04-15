using System;
using System.Collections.Generic;

namespace Tamos.AbleOrigin
{
    public class CacheTrace<T>
    {
        private readonly string TypeName;
        private readonly List<ICacheRule<T>> Rules = new();

        internal CacheTrace(string name)
        {
            TypeName = name;
        }

        /// <summary>
        /// 会在缺少Expire时，随机设置。
        /// </summary>
        public TRule AddRule<TRule>(string name, TRule rule) where TRule : CacheRule, ICacheRule<T>
        {
            rule.Prefix = $"{TypeName}{name}-";
            if (rule.Expire == default) rule.Expire = TimeSpan.FromDays(Utility.BuildRandom(15, 30));

            Rules.Add(rule);
            return rule;
        }

        internal List<string> GetRemoveKeys(T entity)
        {
            var keys = new List<string>(Rules.Count);
            foreach (var rule in Rules)
            {
                var ruleKey = rule.RuleKey(entity);
                if (ruleKey.IsNull()) continue;
                keys.Add(ruleKey);
            }

            return keys;
        }
    }
}
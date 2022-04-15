using System;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 基于属性的特定值Rule，可排除其他值对Cache的清除。
    /// </summary>
    public class ConstCacheRule<T> : PropCacheRule<T>
    {
        private readonly string ConstValue;

        public ConstCacheRule(Func<T, string> propGet, string constValue) : base(propGet)
        {
            ConstValue = constValue;
        }
        
        internal override bool IsValid(T entity)
        {
            return PropGet(entity) == ConstValue; //属性值需等于当前规则值
        }

        public override string? RuleKey(T entity)
        {
            return IsValid(entity) ? CreateKey() : null;
        }

        internal string CreateKey()
        {
            return Prefix + ConstValue;
        }
    }
}
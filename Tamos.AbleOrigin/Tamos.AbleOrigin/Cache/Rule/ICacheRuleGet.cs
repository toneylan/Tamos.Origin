using System;

namespace Tamos.AbleOrigin
{
    public interface ICacheRuleGet
    {
        /// <summary>
        /// 用具体属性值生成CacheKey，进而获取或刷新缓存。
        /// </summary>
        TRes? Get<TVal, TRes>(TVal propValue, Func<TRes?> loadData);
    }

    public interface ICacheRuleGet2
    {
        /// <summary>
        /// 参数顺序要与注册时一致。
        /// </summary>
        TRes? Get<TRes>(string value1, string value2, Func<TRes?> loadData);
    }

    public interface ICacheRuleGet3
    {
        /// <summary>
        /// 参数顺序要与注册时一致。
        /// </summary>
        TRes? Get<TRes>(string value1, string value2, string value3, Func<TRes?> loadData);
    }
}
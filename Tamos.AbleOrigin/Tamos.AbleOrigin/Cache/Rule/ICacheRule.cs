namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 表现某种缓存规则
    /// </summary>
    public interface ICacheRule<in T>
    {
        /*/// <summary>
        /// 缓存对象的名称
        /// </summary>
        internal string TypeName { get; set; }*/

        /// <summary>
        /// 对entity获取对应的缓存Key。通常用于移除缓存的场景。<br/>
        /// 当entity不满足Rule时，返回null。
        /// </summary>
        string? RuleKey(T entity);
    }
}
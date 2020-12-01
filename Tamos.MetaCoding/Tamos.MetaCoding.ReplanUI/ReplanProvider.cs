using System.Collections.Generic;

namespace Tamos.MetaCoding.ReplanUI
{
    /// <summary>
    /// 具体数据类型Plan的基类（Not use）
    /// </summary>
    internal abstract class ReplanProvider<T> where T : class
    {
        //protected static RuiEntityConfig<T> Config { get; } = RuiConfig.Entity<T>();

        /*/// <summary>
        /// 生成列表页Vm
        /// </summary>
        public RuiListVm GetListVm(IReadOnlyCollection<T> list)
        {
            return Config.ListPlan.BuildVm(list);
        }*/
    }
}
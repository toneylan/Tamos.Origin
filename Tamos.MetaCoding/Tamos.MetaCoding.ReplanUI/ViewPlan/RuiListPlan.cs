using System;
using System.Collections.Generic;
using Tamos.AbleOrigin.Common;

namespace Tamos.MetaCoding.ReplanUI
{
    /// <summary>
    /// 列表页设计
    /// </summary>
    public class RuiListPlan<T> : BaseRuiPlan<T> where T : class
    {
        private readonly List<BaseRenderField<T>> colSet = new List<BaseRenderField<T>>();

        #region Add col

        /// <summary>
        /// 添加指定属性的列，属性需提前通过PropMetaAttribute等方式配置。
        /// </summary>
        public RuiListPlan<T> Col(string propName)
        {
            colSet.Add(new PropertyField<T>(Meta.GetProp(propName)));
            return this;
        }

        /// <summary>
        /// 添加自定义内容的列
        /// </summary>
        public RuiListPlan<T> Col(string cnName, Func<T, string> valueGet)
        {
            colSet.Add(new CustomField<T>(cnName, valueGet));
            return this;
        }

        #endregion

        /// <summary>
        /// 生成列表页Vm
        /// </summary>
        internal RuiListVm BuildVm(IReadOnlyCollection<T> list)
        {
            var vm = new RuiListVm
            {
                HeaderNames = colSet.ConvertAll(x=> x.CnName)
            };
            if (list.IsNullOrEmpty() || vm.HeaderNames.IsNullOrEmpty()) return vm;

            //行数据
            vm.RowData = new List<string[]>(list.Count);
            foreach (var item in list)
            {
                var row = new string[colSet.Count];
                for (var i = 0; i < colSet.Count; i++)
                {
                    row[i] = colSet[i].GetValue(item);
                }
                vm.RowData.Add(row);
            }
            return vm;
        }
    }

    /// <summary>
    /// 列表页Vm
    /// </summary>
    public class RuiListVm
    {
        public List<string> HeaderNames { get; set; }

        public List<string[]> RowData { get; set; }
    }
}
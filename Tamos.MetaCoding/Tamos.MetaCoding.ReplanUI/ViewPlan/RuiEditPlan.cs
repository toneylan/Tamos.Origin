using System.Collections.Generic;

namespace Tamos.MetaCoding.ReplanUI
{
    /// <summary>
    /// 编辑页设计
    /// </summary>
    public class RuiEditPlan
    {
        public string? title { get; set; }
        public List<RuiEditItem> items { get; } = new List<RuiEditItem>();
        //public ExpandoObject model { get; } = new ExpandoObject();

        #region Add item

        public RuiEditPlan Add(string name, string cnName)
        {
            return Add(name, cnName, string.Empty);
        }

        public RuiEditPlan Add<T>(string name, string cnName, T value)
        {
            items.Add(new RuiEditItem(name, cnName) {value = value});
            //if (value != null) model.TryAdd(name, value);

            return this;
        }

        #endregion
    }

    public class RuiEditItem
    {
        public RuiEditItem(string name, string cnName)
        {
            this.name = name;
            this.cnName = cnName;
        }

        public string name { get; set; }
        public string cnName { get; set; }
        public object? value { get; set; }
    }
}
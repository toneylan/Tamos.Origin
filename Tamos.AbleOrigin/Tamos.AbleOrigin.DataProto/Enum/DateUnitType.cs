using System.ComponentModel;

namespace Tamos.AbleOrigin.DataProto
{
    public enum DateUnitType
    {
        /// <summary>
        /// 天
        /// </summary>
        [Description("天")]
        Day = 0,

        /// <summary>
        /// 周
        /// </summary>
        [Description("周")]
        Week = 1,

        /// <summary>
        /// 月
        /// </summary>
        [Description("月")]
        Month = 2,

        //Quarter = 3,

        /// <summary>
        /// 年
        /// </summary>
        [Description("年")]
        Year = 6
    }
}
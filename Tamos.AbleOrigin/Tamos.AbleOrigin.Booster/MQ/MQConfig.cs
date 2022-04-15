using System;

namespace Tamos.AbleOrigin.Booster
{
    internal class MQConfig
    {
        //基础组件
        public static readonly string CommonDl = "tamos_common_dl";
        public static readonly string Delay10Min = "tamos_delay_10min";

        /// <summary>
        /// 注意：DelayQueue 与 Exchange 使用了相同名称
        /// </summary>
        internal static string DelayExchange(MQDelayTime time)
        {
            return time switch
            {
                MQDelayTime.Min10 => Delay10Min,
                _ => throw new ArgumentOutOfRangeException(nameof(MQDelayTime), time, "未实现的延迟时间")
            };
        }
    }
}
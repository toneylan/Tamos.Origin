using System;

namespace Tamos.AbleOrigin.Common
{
    public static class DateTimeExtend
    {
        /// <summary>
        /// 截去秒、毫秒部分
        /// </summary>
        public static DateTime RemoveSeconds(this DateTime time)
        {
            return time.Date.AddHours(time.Hour).AddMinutes(time.Minute);
        }

        /// <summary>
        /// 从UNIX 时间戳转为DateTime
        /// </summary>
        public static DateTime FromUnixTimestamp(long unixSeconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixSeconds).LocalDateTime;
        }

        /// <summary>
        /// 是否是默认时间
        /// </summary>
        public static bool IsDefaultDateTime(this DateTime time)
        {
            return time == DateTime.MinValue || time == DateTime.MaxValue;
        }

        #region Round houre

        /// <summary>
        /// 对齐到半小时（向上舌入）
        /// </summary>
        public static DateTime HalfHourCeiling(this DateTime value)
        {
            value = RemoveSeconds(value);
            if (value.Minute % 30 == 0) return value;

            return value.Minute < 30 ? value.AddMinutes(30 - value.Minute) : value.AddMinutes(60 - value.Minute);
        }

        /// <summary>
        /// 对齐到半小时（向下舌去）
        /// </summary>
        public static DateTime HalfHourFloor(this DateTime value)
        {
            value = RemoveSeconds(value);
            if (value.Minute % 30 == 0) return value;

            return value.Minute < 30 ? value.AddMinutes(-value.Minute) : value.AddMinutes(30 - value.Minute);
        }

        /// <summary>
        /// 对齐到小时（向上舌入）
        /// </summary>
        public static DateTime HourCeiling(this DateTime value)
        {
            value = RemoveSeconds(value);
            return value.Minute == 0 ? value : value.AddMinutes(60 - value.Minute);
        }

        /// <summary>
        /// 对齐到小时（向下舌去）
        /// </summary>
        public static DateTime HourFloor(this DateTime value)
        {
            value = RemoveSeconds(value);
            return value.Minute == 0 ? value : value.AddMinutes(-value.Minute);
        }

        #endregion

        #region FriendlyDes

        /// <summary>
        /// 获取“周几”的描述
        /// </summary>
        public static string ShortDes(this DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "周一",
                DayOfWeek.Tuesday => "周二",
                DayOfWeek.Wednesday => "周三",
                DayOfWeek.Thursday => "周四",
                DayOfWeek.Friday => "周五",
                DayOfWeek.Saturday => "周六",
                DayOfWeek.Sunday => "周日",
                _ => dayOfWeek.ToString()
            };
        }

        /// <summary>
        /// 获取时间的简单描述
        /// </summary>
        public static string GetFriendlyDes(this DateTime time)
        {
            var now = DateTime.Now;
            var span = now - time;
            if (span.TotalSeconds < 60) return "刚刚";
            if (span.TotalMinutes < 60) return string.Format("{0:F0}分钟前", span.TotalMinutes);
            if (span.TotalHours < 24) return string.Format("{0:F0}小时前", span.TotalHours);
            if (span.TotalDays < 7) return string.Format("{0:F0}天前", span.TotalDays);

            return time.Year == now.Year ? time.ToString("MM-dd") : time.ToString("yyyy-MM-dd");
        }

        #endregion

        /// <summary>
        /// 格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string ToFullString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
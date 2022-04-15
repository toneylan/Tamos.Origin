using System;

namespace Tamos.AbleOrigin
{
    public static class DateTimeExtend
    {   
        #region UNIX 时间戳

        /// <summary>
        /// 获取Unix（java）时间戳-秒
        /// </summary>
        /// <returns></returns>
        public static long ToUnixTimestamp(this DateTime time)
        {
            //return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            return new DateTimeOffset(time).ToUnixTimeSeconds();
        }

        /// <summary>
        /// 从Unix 时间戳转为DateTime（本地时区，因为时间戳本身无时区概念）
        /// </summary>
        public static DateTime FromUnixTimestamp(long unixSeconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixSeconds).LocalDateTime;
        }

        #endregion

        #region 比较、计算

        /// <summary>
        /// 是否MinValue或MaxValue
        /// </summary>
        public static bool IsMinOrMax(this DateTime time)
        {
            return time == DateTime.MinValue || time == DateTime.MaxValue;
        }

        /// <summary>
        /// 比较返回更大的时间
        /// </summary>
        public static DateTime Max(this DateTime time1, DateTime time2)
        {
            return time1 > time2 ? time1 : time2;
        }

        /// <summary>
        /// 比较返回更小的时间
        /// </summary>
        public static DateTime Min(this DateTime time1, DateTime time2)
        {
            return time1 > time2 ? time2 : time1;
        }

        /// <summary>
        /// 计算相差的分钟数
        /// </summary>
        public static int DiffMinutes(this DateTime curTime, DateTime target)
        {
            return (int)(curTime - target).TotalMinutes;
        }

        #endregion

        #region Round 范围

        private static readonly TimeSpan DayEndSpan = new(23, 59, 59);

        /// <summary>
        /// 截去秒、毫秒部分
        /// </summary>
        public static DateTime RemoveSeconds(this DateTime time)
        {
            return time.Date.AddHours(time.Hour).AddMinutes(time.Minute);
        }

        /// <summary>
        /// 对齐到半小时（默认向前舌去）
        /// </summary>
        public static DateTime AlignHalfHour(this DateTime value, bool ceiling = false)
        {
            if (value.Second != 0 || value.Millisecond != 0) value = RemoveSeconds(value);
            if (value.Minute % 30 == 0) return value;

            var diff = (ceiling ? 30 : 0) - value.Minute;
            return value.AddMinutes(value.Minute < 30 ? diff : diff + 30);
        }

        /// <summary>
        /// 对齐到小时（默认向前舌去）
        /// </summary>
        public static DateTime AlignHour(this DateTime value, bool ceiling = false)
        {
            if (value.Second != 0 || value.Millisecond != 0) value = RemoveSeconds(value);
            return value.Minute == 0 ? value : value.AddMinutes(ceiling ? 60 - value.Minute : -value.Minute);
        }
        
        /// <summary>
        /// 时间所在天的结束 23:59:59
        /// </summary>
        public static DateTime DayEnd(this DateTime time)
        {
            return time.Date.Add(DayEndSpan);
        }

        /// <summary> 
        /// 时间所在周的开始（周一零点） 
        /// </summary> 
        public static DateTime WeekStart(this DateTime time)
        {
            var forward = time.DayOfWeek == DayOfWeek.Sunday ? 6 : (int) time.DayOfWeek - 1;
            
            return time.Date.AddDays(-forward);
        }

        /// <summary> 
        /// 时间所在周的结束（周日23:59:59）
        /// </summary> 
        public static DateTime WeekEnd(this DateTime time)
        {
            var backward = time.DayOfWeek == DayOfWeek.Sunday ? 0 : 7 - (int) time.DayOfWeek;
            return time.Date.AddDays(backward).Add(DayEndSpan);
        }

        #endregion
        
        #region FriendlyDes、ToString

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
        /// 友好描述形式，如：几 分钟/天前、?月?日
        /// </summary>
        public static string GetFriendlyDes(this DateTime time)
        {
            var now = DateTime.Now;
            var span = now - time;
            if (span.TotalSeconds < 60) return "刚刚";
            if (span.TotalMinutes < 60) return string.Format("{0:F0}分钟前", span.TotalMinutes);
            if (span.TotalHours < 24) return string.Format("{0:F0}小时前", span.TotalHours);
            if (span.TotalDays < 31) return string.Format("{0:F0}天前", span.TotalDays);

            return time.Year == now.Year ? time.ToString("M月d日") : time.ToString("yyyy-MM-dd");
        }
        
        /// <summary>
        /// 格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string ToFullString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 格式：yyyy-MM-dd
        /// </summary>
        public static string ToDateString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 格式：HH:mm
        /// </summary>
        public static string ToTimePoint(this DateTime time)
        {
            return time.ToString("HH:mm");
        }

        #endregion
    }
}
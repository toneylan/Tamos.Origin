using System;
using System.Collections.Generic;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 定义时间点，从00:00为0开始计数，5分钟为单位。
    /// </summary>
    public static class TimeConverter
    {
        #region Feilds

        /// <summary>
        /// 时间单位 5分钟
        /// </summary>
        public const int SpanMinutes = 5;

        /// <summary>
        /// 一小时的TimePoint值
        /// </summary>
        public const int HourSpanPoint = 60 / SpanMinutes;

        /// <summary>
        /// 半小时的TimePoint值
        /// </summary>
        public const int HalfHourSpanPoint = 30 / SpanMinutes;

        /// <summary>
        /// 不是0点，而是其前一刻。
        /// </summary>
        public const int MaxTimePoint = 60 * 24 / SpanMinutes - 1;
        
        public const int MinTimePoint = 0;

        #endregion

        #region Time parse

        public static string GetTimeString(int time)
        {
            if (time == MaxTimePoint)
            {
                return "00:00";
            }
            var minutes = time * SpanMinutes;
            return string.Format("{0:00}:{1:00}", minutes / 60, minutes % 60);
        }

        public static int ParseTime(string timeStr)
        {
            if (string.IsNullOrEmpty(timeStr)) return 0;

            var tmppt = timeStr.Split(':');
            if (tmppt.Length != 2) return 0;

            var points = new int[2];
            int.TryParse(tmppt[0], out points[0]);
            int.TryParse(tmppt[1], out points[1]);
            return GetTimeByHourMinute(points[0], points[1], false);
        }

        /// <summary>
        /// 按小时和分钟计算时间点。可设置不足SpanMinutes时，是否向上舌入。
        /// </summary>
        public static int ParseTime(DateTime dateTime, bool greaterAtMiddle = false)
        {
            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue) return 0;
            return GetTimeByHourMinute(dateTime.Hour, dateTime.Minute, greaterAtMiddle);
        }

        /// <summary>
        /// 按小时和分钟计算时间点
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minutes"></param>
        /// <param name="greaterAtMiddle">处于中间时是否往更大时间延</param>
        public static int GetTimeByHourMinute(int hour, int minutes, bool greaterAtMiddle)
        {
            if (hour >= 24 || hour < 0 || minutes < 0 || minutes >= 60) return 0;

            var totalMinutes = hour * 60 + minutes;
            var pieceMint = totalMinutes % SpanMinutes;
            if (pieceMint > 0)
            {
                if (greaterAtMiddle) totalMinutes += SpanMinutes - pieceMint;
                else totalMinutes -= pieceMint;
            }

            return totalMinutes / SpanMinutes;
        }
        
        /// <summary>
        /// 修正分钟数到半小时的倍数
        /// </summary>
        public static int FixMinutesToHalfHours(int minutes, bool greaterAtMiddle)
        {
            if (minutes <= 0) return minutes;

            var mod = minutes % 30;
            if (mod == 0) return minutes;
            return greaterAtMiddle ? minutes + 30 - mod : minutes - mod;
        }

        #endregion
        
        #region Specific get
        
        /// <summary>
        /// 获取范围的间隔时间点，支持跨天
        /// </summary>
        public static int GetSpanPointInRange(int startPoint, int endPoint)
        {
            if (endPoint <= startPoint)
            {
                return MaxTimePoint - startPoint + endPoint + 1;
            }
            return endPoint - startPoint;
        }

        /// <summary>
        /// 获取范围的间隔分钟数，支持跨天
        /// </summary>
        public static int GetSpanMinutesInRange(int startPoint, int endPoint)
        {
            return GetSpanPointInRange(startPoint, endPoint)*SpanMinutes;
        }

        #endregion
        
        /*#region 范围判断
        
        /// <summary>
        /// 判断时间点是否在范围内，兼容跨天
        /// </summary>
        public static bool IsTimePointInRange(int curPoint, int start, int end, bool allowEqualAtEnd = false)
        {
            if (start < end)
            {
                return curPoint >= start && curPoint < (allowEqualAtEnd ? ++end : end);
            }
            return !(curPoint < start && curPoint >= (allowEqualAtEnd ? ++end : end));
        }

        #endregion*/

        #region Get TimeList

        /// <summary>
        /// 获取一天的时间点列表
        /// </summary>
        /// <param name="spanMinites">时间点之间的间隔（如30、60mins[5的整数倍]）</param>
        /// <param name="zeroAtEnd">是否零点在结尾</param>
        public static List<TimePoint> GetTimeList(int spanMinites, bool zeroAtEnd = false)
        {
            if (spanMinites < SpanMinutes) throw new ArgumentException("spanMinites is smaller than internal");

            var times = new List<TimePoint>();
            var pointSpan = spanMinites / SpanMinutes;

            if (!zeroAtEnd) times.Add(0); 
            for (var i = pointSpan; i <= MaxTimePoint; i += pointSpan)
            {
                times.Add(i);
            }
            if (zeroAtEnd) times.Add(0);

            return times;
        }

        /// <summary>
        /// 获取指定范围的时间点列表，支持跨天。
        /// </summary>
        public static List<TimePoint> GetTimeList(int startPoint, int endPoint, int spanMinutes)
        {
            if (spanMinutes < SpanMinutes) throw new ArgumentException("spanMinites is smaller than internal");

            var times = new List<TimePoint>();
            var pointSpan = spanMinutes / SpanMinutes;

            if (startPoint < endPoint)
            {
                for (var i = startPoint; i <= endPoint; i += pointSpan)
                {
                    times.Add(i);
                }

            }
            else
            {
                for (var i = startPoint; i <= MaxTimePoint; i += pointSpan)
                {
                    times.Add(i);
                }
                for (var i = 0; i <= endPoint; i += pointSpan)
                {
                    times.Add(i);
                }
            }

            return times;
        }

        #endregion
    }

    #region TimePoint

    /// <summary>
    /// 表示一天中的某个时间点
    /// </summary>
    public struct TimePoint
    {
        /// <summary>
        /// 自定义的时间值
        /// </summary>
        public int Tick { get; set; }

        /// <summary>
        /// 时间点的显示值，如 08:00
        /// </summary>
        public string Name => TimeConverter.GetTimeString(Tick);

        /// <summary>
        /// 一天里的总分钟数
        /// </summary>
        public int TotalMinutes => Tick * TimeConverter.SpanMinutes;

        public TimePoint(int tick)
        {
            Tick = tick;
        }

        #region Operation

        /// <summary>
        /// 增加指定量，支持跨天
        /// </summary>
        public int Add(int addVal)
        {
            if (Tick + addVal > TimeConverter.MaxTimePoint)
            {
                return Tick + addVal - TimeConverter.MaxTimePoint - 1;
            }
            return Tick + addVal;
        }

        
        /// <summary>
        /// 减少指定量，支持跨天
        /// </summary>
        public int Subtract(int subVal)
        {
            if (Tick - subVal < 0)
            {
                return TimeConverter.MaxTimePoint + 1 + Tick - subVal;
            }
            return Tick - subVal;
        }

        /// <summary>
        /// 以日期为基础，转为对应的当天时间
        /// </summary>
        public DateTime OnDate(DateTime date)
        {
            return date.Date.AddMinutes(TotalMinutes);
        }

        #endregion

        #region 转换器 with int

        public static implicit operator TimePoint(int value)
        {
            return new TimePoint(value);
        }

        #endregion
    }

    /// <summary>
    /// 时间点组成的范围
    /// </summary>
    public struct TimePointRange
    {
        /// <summary>
        /// 开始TimePoint
        /// </summary>
        public TimePoint Start { get; set; }

        /// <summary>
        /// 结束TimePoint
        /// </summary>
        public TimePoint End { get; set; }

        /// <summary>
        /// 是否跨天
        /// </summary>
        public bool IsSpannedDay => Start.Tick >= End.Tick;

        #region OnDate & Range

        /// <summary>
        /// 以时间为基础，转为对应的当天时间范围，支持跨天。
        /// </summary>
        public (DateTime StartTime, DateTime EndTime) OnDate(DateTime date)
        {
            var start = Start.OnDate(date);
            return (start, EndOnDate(start));
        }

        /// <summary>
        /// 以时间为基础，获取对应的结束时间，支持跨天。
        /// </summary>
        public DateTime EndOnDate(DateTime time)
        {
            if (!IsSpannedDay) return End.OnDate(time);

            //跨天且time不小于开始点，则在后一天结束
            return time.TimeOfDay.TotalMinutes >= Start.TotalMinutes ? End.OnDate(time.AddDays(1)) : End.OnDate(time);
        }
        
        /// <summary>
        /// 判断时间是否在范围内，兼容跨天
        /// </summary>
        public bool IsInRange(DateTime time, bool allowEqualEnd = false)
        {
            var curDayMins = time.TimeOfDay.TotalMinutes;
            if (!IsSpannedDay)
            {
                return curDayMins >= Start.TotalMinutes && (allowEqualEnd ? curDayMins <= End.TotalMinutes : curDayMins < End.TotalMinutes);
            }
            //跨天
            return curDayMins >= Start.TotalMinutes || (allowEqualEnd ? curDayMins <= End.TotalMinutes : curDayMins < End.TotalMinutes);
        }

        #endregion

        #region Static Parse

        public static TimePointRange Parse(string startTime, string endTime)
        {
            return new TimePointRange
            {
                Start = TimeConverter.ParseTime(startTime),
                End = TimeConverter.ParseTime(endTime)
            };
        }

        public static TimePointRange From(int startPoint, int endPoint)
        {
            return new TimePointRange
            {
                Start = startPoint,
                End = endPoint
            };
        }

        #endregion
    }

    #endregion
}
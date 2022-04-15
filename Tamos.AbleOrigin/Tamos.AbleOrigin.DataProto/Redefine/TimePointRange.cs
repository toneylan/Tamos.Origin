using System;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// The range of time point
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
        public bool IsCrossDay => Start.Minutes >= End.Minutes;

        /// <summary>
        /// Total minutes in range
        /// </summary>
        public int TotalMinutes => IsCrossDay ? TimePoint.MaxMinutes - Start.Minutes + End.Minutes : End.Minutes - Start.Minutes;

        #region OnDate & Range

        /// <summary>
        /// Start point on day，get the point range，support cross day.
        /// </summary>
        public (DateTime StartTime, DateTime EndTime) OnDate(DateTime day)
        {
            var start = Start.OnDate(day);
            return (start, EndOnDate(start));
        }

        /// <summary>
        /// End point on day.
        /// </summary>
        private DateTime EndOnDate(DateTime time)
        {
            return End.OnDate(IsCrossDay ? time.AddDays(1) : time);

            //跨天且time不小于开始点，则在后一天结束
            //return time.TimeOfDay.TotalMinutes >= Start.Minutes ? End.OnDate(time.AddDays(1)) : End.OnDate(time);
        }

        /// <summary>
        /// Check if time in range.
        /// </summary>
        public bool IsInRange(DateTime time, bool allowEqualEnd = false)
        {
            var curDayMins = MinutesOfDay(time);
            if (!IsCrossDay)
            {
                return curDayMins >= Start.Minutes && (allowEqualEnd ? curDayMins <= End.Minutes : curDayMins < End.Minutes);
            }

            //跨天
            return curDayMins >= Start.Minutes || (allowEqualEnd ? curDayMins <= End.Minutes : curDayMins < End.Minutes);
        }

        /// <summary>
        /// 是否完全在Range内。
        /// </summary>
        public bool IsInRange(DateTime time, int duration)
        {
            return IsInRange(time) && (duration <= 0 || IsInRange(time.AddMinutes(duration), true));
        }

        /// <summary>
        /// 是否有时间重叠。
        /// </summary>
        public bool IsIntersect(DateTime start, DateTime end)
        {
            var (rangeStart, rangeEnd) = OnDate(start);
            return start < rangeEnd && end > rangeStart;
        }

        /// <summary>
        /// Get the intersect time range
        /// </summary>
        public TimeExactRange Intersect(DateTime start, int duration)
        {
            var end = start.AddMinutes(duration);
            var (rangeStart, rangeEnd) = OnDate(start);
            if (start >= rangeEnd || end <= rangeStart) return TimeExactRange.Empty;

            //adjust 
            return new TimeExactRange
            {
                Start = start > rangeStart ? start : rangeStart,
                End = end < rangeEnd ? end : rangeEnd
            };
        }

        #endregion

        /// <summary>
        /// Start - End
        /// </summary>
        public override string ToString()
        {
            return $"{Start} - {End}";
        }

        #region Static Parse

        public static TimePointRange Parse(string startTime, string endTime)
        {
            return new()
            {
                Start = TimePoint.Parse(startTime),
                End = TimePoint.Parse(endTime)
            };
        }

        public static TimePointRange From(int startPoint, int endPoint)
        {
            return new()
            {
                Start = startPoint,
                End = endPoint
            };
        }

        #endregion

        private static int MinutesOfDay(DateTime time)
        {
            return (int)time.TimeOfDay.TotalMinutes;
        }

        #region Get TimeList

        /*/// <summary>
        /// 获取指定范围的时间点列表，支持跨天。
        /// </summary>
        public IReadOnlyCollection<TimePoint> GetTimeList(int spanMinutes, int startPoint, int endPoint)
        {
            var times = new List<TimePoint>();

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

            //if (zeroAtEnd) times.Add(0);
            return times;
        }*/

        #endregion
    }
}
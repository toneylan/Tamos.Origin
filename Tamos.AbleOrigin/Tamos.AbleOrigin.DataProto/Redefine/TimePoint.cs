namespace Tamos.AbleOrigin.DataProto;

/// <summary>
/// A time point of some day
/// </summary>
public struct TimePoint
{
    internal const int MaxMinutes = 1440;

    /// <summary>
    /// Indicate total minutes from zero point of day.
    /// </summary>
    public int Minutes { get; set; }
        
    /*public TimePoint(int minutes)
    {
        Minutes = minutes;
    }*/

    #region Operation

    /// <summary>
    /// 增加指定量，支持跨天
    /// </summary>
    public int Add(int addVal)
    {
        if (Minutes + addVal >= MaxMinutes)
        {
            return Minutes + addVal - MaxMinutes;
        }
        return Minutes + addVal;
    }

        
    /// <summary>
    /// 减少指定量，支持跨天
    /// </summary>
    public int Subtract(int subVal)
    {
        if (Minutes - subVal < 0)
        {
            return MaxMinutes + Minutes - subVal;
        }
        return Minutes - subVal;
    }

    /// <summary>
    /// 以日期为基础，转为对应的当天时间
    /// </summary>
    public DateTime OnDate(DateTime date)
    {
        return date.Date.AddMinutes(Minutes);
    }

    #endregion

    /// <summary>
    /// To HH:mm
    /// </summary>
    public override string ToString()
    {
        if (Minutes == 0 || Minutes == MaxMinutes) return "00:00";
            
        return $"{Minutes / 60:00}:{Minutes % 60:00}";
    }

    #region operator & parse

    public static TimePoint Parse(string? pointStr)
    {
        return TimeSpan.TryParse(pointStr, out var span) ? span : 0;
    }

    public static implicit operator TimePoint(int minutes)
    {
        return new() { Minutes = minutes };
    }

    public static implicit operator TimePoint(TimeSpan span)
    {
        return new() { Minutes = (int)span.TotalMinutes };
    }

    public static implicit operator int(TimePoint point)
    {
        return point.Minutes;
    }

    #endregion
}
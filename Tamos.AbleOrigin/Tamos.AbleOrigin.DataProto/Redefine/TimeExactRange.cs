using System.Runtime.Serialization;

namespace Tamos.AbleOrigin.DataProto;

/// <summary>
/// 具体日期的时间范围
/// </summary>
[DataContract]
public struct TimeExactRange
{
    public static readonly TimeExactRange Empty = new();

    [DataMember(Order = 1)]
    public DateTime Start { get; set; }

    [DataMember(Order = 2)]
    public DateTime End { get; set; }

    /// <summary>
    /// 间隔分钟数
    /// </summary>
    public int Duration => (int)(End - Start).TotalMinutes;

    /// <summary>
    /// 是否空Range
    /// </summary>
    public bool IsEmpty => End == DateTime.MinValue;

    public TimeExactRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    #region Set range

    public void Set(DateTime start, int duration)
    {
        Start = start;
        End = start.AddMinutes(duration);
    }

    public void Set(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public void Set(TimeExactRange to)
    {
        Start = to.Start;
        End = to.End;
    }

    /// <summary>
    /// 设为空值，即不代表任何Range。
    /// </summary>
    public void SetEmpty()
    {
        Start = End = DateTime.MinValue;
    }

    #endregion
}
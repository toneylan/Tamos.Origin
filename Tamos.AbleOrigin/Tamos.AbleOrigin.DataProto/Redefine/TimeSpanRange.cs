namespace Tamos.AbleOrigin.DataProto;

/// <summary>
/// 时长范围-分钟，如 60-300 分钟。
/// </summary>
public struct TimeSpanRange
{
    /// <summary>
    /// 开始/Min时长
    /// </summary>
    public int Start { get; set; }

    /// <summary>
    /// 结束/Max时长
    /// </summary>
    public int End { get; set; }

    public TimeSpanRange(int start, int end)
    {
        Start = start;
        End = end;
    }
}
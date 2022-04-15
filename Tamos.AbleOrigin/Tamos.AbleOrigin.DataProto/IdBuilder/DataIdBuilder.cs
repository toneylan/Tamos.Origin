namespace Tamos.AbleOrigin.DataProto;

public static class DataIdBuilder
{
    /// <summary>
    /// Id时间戳的相对时间点，目前为：2020-01-01
    /// </summary>
    public static readonly DateTime ReferTime = new(2020, 1, 1); //重要！！是Id中经过秒数的参照时间，一经使用不要修改。

    private static readonly IdGenerator Generator = new();
    private static IncIdGenerator? IncGenerator;

    #region GenerateId

    /// <summary>
    /// 按当前时间生成Id
    /// </summary>
    public static long GenerateId(long scopeNum, short scopeSuffix)
    {
        return Generator.GenId(DateTime.Now, scopeNum, scopeSuffix);
    }

    /// <summary>
    /// Common generate Id
    /// </summary>
    public static long GenerateId(DateTime time, long scopeNum, short scopeSuffix)
    {
        return Generator.GenId(time, scopeNum, scopeSuffix);
    }

    /// <summary>
    /// 生成一个递增的编号
    /// </summary>
    public static long GenerateIncId(long scopeNum, short scopeSuffix)
    {
        return (IncGenerator ??= new IncIdGenerator()).GenId(DateTime.Now, scopeNum, scopeSuffix);
    }

    #endregion

    #region Parse date

    /// <summary>
    /// 从Id解析时间，可兼容ReferTime之前格式
    /// </summary>
    public static DateTime ParseDate(long id, bool checkBeforeRefer = false)
    {
        //时间戳后的长度
        const int sufLength = 7;

        var idStr = id.ToString();
        if (idStr.Length <= sufLength || !int.TryParse(idStr[..^sufLength], out var sec)) return DateTime.Today;

        return checkBeforeRefer && IdGenerator.IsBeforeRefer(idStr) ? ReferTime.AddSeconds(-sec) : ReferTime.AddSeconds(sec);
    }

    /// <summary>
    /// 解析时间范围
    /// </summary>
    public static (DateTime Start, DateTime End) ParseDateRange(IReadOnlyCollection<long>? idList)
    {
        if (idList.IsNull()) return (DateTime.Today, DateTime.Today);

        DateTime minDate = DateTime.MaxValue, maxDate = DateTime.MinValue;
        foreach (var id in idList)
        {
            var date = ParseDate(id);
            if (date < minDate) minDate = date;
            if (date > maxDate) maxDate = date;
        }

        return (minDate, maxDate);
    }

    #endregion

    #region Time to Id

    /// <summary>
    /// 指定时间下的最小Id，可据此做时间条件的查询。
    /// </summary>
    public static long MinOfTime(DateTime time)
    {
        var secStamp = (int)(time - ReferTime).TotalSeconds;
        return (secStamp + "0000000").ToLong();
    }

    /// <summary>
    /// 按时间范围获取Id值范围，可据此做时间条件的查询。
    /// </summary>
    public static (long min, long max) RangeOfTime(DateTime start, DateTime end)
    {
        return (MinOfTime(start), MinOfTime(end.AddSeconds(1)));
    }

    #endregion
}
namespace Tamos.AbleOrigin.DataProto;

internal class IdGenerator
{
    protected DateTime _lastGenerateTime;
    protected readonly Random NumRandom = new();
    private readonly HashSet<long> BuiltIdCache = new(); //高频生成时，缓存记录以排重

    /// <summary>
    /// 会进行并发Id生成检查，能避免在进程级别生成相同Id。
    /// </summary>
    internal virtual long GenId(DateTime genTime, long scopeNum, short scopeSuffix)
    {
        //生成一个Id
        var id = GenNew(genTime, scopeNum, scopeSuffix);

        //生成间隔大于1秒，基本不会重复了
        if (Math.Abs((genTime - _lastGenerateTime).TotalSeconds) > 1)
        {
            if (BuiltIdCache.Count > 100) BuiltIdCache.Clear(); //清理一下
        }
        else //---快速生成模式
        {
            var loopCount = 0;
            while (BuiltIdCache.Contains(id))
            {
                if (++loopCount > 800)
                {
                    genTime = genTime.AddSeconds(1); //若难以生成新Id，增加一秒再尝试
                }
                id = GenNew(genTime, scopeNum, scopeSuffix);
            }
        }

        //这里记录的genTime，不是当前时间。所以如果高并发生成时，genTime有大范围跳跃，有可能会误跳过前边的间隔时间检查
        BuiltIdCache.Add(id);
        _lastGenerateTime = genTime;
        return id;
    }

    /// <summary>
    /// 生成一个新Id
    /// </summary>
    protected virtual long GenNew(DateTime time, long scopeNum, short scopeSuffix)
    {
        var secStamp = (int)(time - DataIdBuilder.ReferTime).TotalSeconds;

        //特殊处理ReferTime之前的时间
        if (secStamp < 0)
        {
            secStamp = -secStamp;
            scopeSuffix = 0;
        }

        return string.Format("{0}{1:000}{2}{3:000}", secStamp, scopeNum % 1000, scopeSuffix, NumRandom.Next(1, 999)).ToLong();
    }

    
    internal static bool IsBeforeRefer(string idStr)
    {
        return idStr[^4] == '0';
    }
}

internal class IncIdGenerator : IdGenerator
{
    private int _lastIncNum; //前一次递增结束编号

    internal override long GenId(DateTime genTime, long scopeNum, short scopeSuffix)
    {
        //生成间隔大于一秒，重置递增量
        if ((genTime - _lastGenerateTime).TotalSeconds > 1) _lastIncNum = 0;

        //由于只用2位，则超过100后会从0重新使用
        _lastIncNum++;

        return base.GenId(genTime, scopeNum, scopeSuffix);
    }

    protected override long GenNew(DateTime time, long scopeNum, short scopeSuffix)
    {
        var secStamp = (int)(time - DataIdBuilder.ReferTime).TotalSeconds;

        //特殊处理ReferTime之前的时间
        if (secStamp < 0)
        {
            secStamp = -secStamp;
            scopeSuffix = 0;
        }

        //两位递增+两位随机，尽可能避免同尾号scopeNum在同一时刻的冲突可能性
        return string.Format("{0}{1:00}{2}{3:00}{4:00}", secStamp, scopeNum % 100, scopeSuffix,
            _lastIncNum < 100 ? _lastIncNum : _lastIncNum % 100, NumRandom.Next(1, 99)).ToLong();
    }
}
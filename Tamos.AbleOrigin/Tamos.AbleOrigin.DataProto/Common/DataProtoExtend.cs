using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Tamos.AbleOrigin.DataProto;

public static class DataProtoExtend
{
    #region Mapper

    /// <summary>
    /// Map为目标类型
    /// </summary>
    [return: NotNullIfNotNull("source")]
    public static TDestination? MapTo<TDestination>(this IGeneralEntity? source)
    {
        return EntMapper.Map<TDestination>(source);
    }

    /// <summary>
    /// Map为目标类型
    /// </summary>
    [return: NotNullIfNotNull("source")]
    public static TDestination? MapTo<TDestination>(this IReadOnlyCollection<IGeneralEntity>? source)
    {
        return EntMapper.Map<TDestination>(source);
    }

    #endregion

    #region Fill data

    /// <summary>
    /// 依据列表数据的关联Id，获取关联的记录并填充到相应的属性值。
    /// </summary>
    public static void FillProperty<TSource, TDTO>(this IReadOnlyCollection<TSource>? list, Expression<Func<TSource, TDTO>> expressionProp,
        Func<TSource, long> funcPropId, Func<IReadOnlyCollection<long>, List<TDTO>> qryPropObj) where TDTO : IGeneralEntity
    {
        if (list.IsNull()) return;

        var objList = qryPropObj(list.Select(funcPropId).ToList());
        if (objList.IsNull()) return;

        var prop = EntityMeta.Get<TSource>().GetProp(expressionProp);
        foreach (var item in list)
        {
            var obj = objList.ById(funcPropId(item));
            if (obj != null) prop.Setter(item, obj);
        }
    }

    /// <summary>
    /// 依据列表数据的关联Id，查询关联的记录，以执行setProp。
    /// </summary>
    [return: NotNullIfNotNull("list")]
    public static IReadOnlyCollection<TSource>? FillPropSet<TSource, TDTO>(this IReadOnlyCollection<TSource>? list, Func<TSource, long> funcRelId, Func<IEnumerable<long>, List<TDTO>> qryRelObj,
        Action<TSource, TDTO> setProp) where TDTO : IGeneralEntity
    {
        if (list.IsNull()) return list;

        var objList = qryRelObj(list.Select(funcRelId));
        if (objList.IsNull()) return list;

        foreach (var item in list)
        {
            var obj = objList.ById(funcRelId(item));
            if (obj != null) setProp(item, obj);
        }
        return list;
    }

    /// <summary>
    /// Fill sub items by parent id relation
    /// </summary>
    public static void FillSubItems<TEntity, TSub>(this IReadOnlyCollection<TEntity>? list, Func<IEnumerable<long>, List<TSub>> qrySubItems,
        Func<TSub, long> funcParentId, Action<TEntity, List<TSub>> setSubItems)
        where TEntity : IGeneralEntity
    {
        if (list.IsNull()) return;

        var items = qrySubItems(list.Select(x => x.Id));
        if (items.IsNull()) return;

        foreach (var group in items.GroupBy(funcParentId))
        {
            var parent = list.FirstOrDefault(x => x.Id == group.Key);
            if (parent == null) continue;
            setSubItems(parent, group.ToList());
        }
    }

    #endregion

    #region IGeneralEntity

    /// <summary>
    /// 按Id Find记录
    /// </summary>
    public static T? ById<T>(this List<T>? list, long id) where T : IGeneralEntity
    {
        return list == null ? default : list.Find(x => x.Id == id);
    }

    /// <summary>
    /// 检查对象的属性值是否有效，规则可通过PropMeta标记配置。
    /// </summary>
    public static bool Validate<T>([NotNullWhen(true)] this T? obj, out string? error) where T : IGeneralEntity
    {
        error = EntityMeta.Get<T>().Validate(obj);
        return error.IsNull();
    }

    #endregion

    #region IGeneralResObj

    /// <summary>
    /// Set error msg(may append), msg is null won't cause res IsFailure.<br/>
    /// If not sure has error, suggest use res.On method.
    /// </summary>
    public static T Error<T>(this T res, string? msg) where T : class, IGeneralResObj
    {
        if (res.ErrorMsg == null) res.ErrorMsg = msg;
        else res.ErrorMsg += msg;
        return res;
    }

    /// <summary>
    /// Join error msg.
    /// </summary>
    public static T On<T>(this T res, string? errorMsg) where T : class, IGeneralResObj
    {
        if (errorMsg.IsNull()) return res;

        if (res.ErrorMsg == null) res.ErrorMsg = errorMsg;
        else res.ErrorMsg += errorMsg;
        return res;
    }

    /// <summary>
    /// Check if has error, and join msg.
    /// </summary>
    public static bool OnFail<T>(this T res, string? errorMsg) where T : class, IGeneralResObj
    {
        if (errorMsg.IsNull()) return !string.IsNullOrEmpty(res.ErrorMsg);

        if (res.ErrorMsg == null) res.ErrorMsg = errorMsg;
        else res.ErrorMsg += errorMsg;

        return !string.IsNullOrEmpty(res.ErrorMsg);
    }

    /// <summary>
    /// ErrorMsg为空算成功。
    /// </summary>
    public static bool IsSuccess<T>([NotNullWhen(true)] this T? res) where T : IGeneralResObj
    {
        return res != null && string.IsNullOrEmpty(res.ErrorMsg);
    }

    /// <summary>
    /// Is fail when res.ErrorMsg not null.
    /// </summary>
    public static bool IsFail<T>([NotNullWhen(false)] this T? res) where T : class, IGeneralResObj
    {
        return res == null || !string.IsNullOrEmpty(res.ErrorMsg);
    }

    //-------------- For struct type

    /// <summary>
    /// Join error msg.
    /// </summary>
    public static T On<T>(this ref T res, string? errorMsg) where T : struct, IGeneralResObj
    {
        if (errorMsg.IsNull()) return res;

        if (res.ErrorMsg == null) res.ErrorMsg = errorMsg;
        else res.ErrorMsg += errorMsg;
        return res;
    }

    /// <summary>
    /// Check if has error, and join msg.
    /// </summary>
    public static bool OnFail<T>(this ref T res, string? errorMsg) where T : struct, IGeneralResObj
    {
        if (errorMsg.IsNull()) return !string.IsNullOrEmpty(res.ErrorMsg);

        if (res.ErrorMsg == null) res.ErrorMsg = errorMsg;
        else res.ErrorMsg += errorMsg;

        return !string.IsNullOrEmpty(res.ErrorMsg);
    }

    /// <summary>
    /// Is fail when res.ErrorMsg not null.
    /// </summary>
    public static bool IsFail<T>(this ref T res) where T : struct, IGeneralResObj
    {
        return !string.IsNullOrEmpty(res.ErrorMsg);
    }

    #endregion

    #region IPayProcessRes

    /// <summary>
    /// 设置失败的支付结果
    /// </summary>
    public static T ErrorRes<T>(this T res, string errorMsg) where T : IPayProcessRes
    {
        res.ResultType = PayProcessResType.Error;
        res.ErrorMsg = errorMsg;
        return res;
    }

    #endregion

    #region Date Week

    public static WeekDay GetWeekDay(this DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Sunday) return WeekDay.Sunday;
        return (WeekDay)(int)date.DayOfWeek;
    }

    /// <summary>
    /// 在time上按DateUnitType增加。
    /// </summary>
    public static DateTime TermAdd(this DateUnitType type, DateTime time, int number)
    {
        return type switch
        {
            DateUnitType.Day => time.AddDays(number),
            DateUnitType.Week => time.AddDays(number * 7),
            DateUnitType.Month => time.AddMonths(number),
            DateUnitType.Year => time.AddYears(number),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "未支持的时间周期单位")
        };
    }

    /// <summary>
    /// 描述周期大小。
    /// </summary>
    public static string TermDes(this DateUnitType type, int number)
    {
        return $"{number}{(type == DateUnitType.Month ? "个" : null)}{type.GetDes()}";
    }

    #endregion 
}
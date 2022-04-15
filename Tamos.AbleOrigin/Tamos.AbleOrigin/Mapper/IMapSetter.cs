using System.Linq.Expressions;

namespace Tamos.AbleOrigin;

public interface IMapSetter<TSource, TDestination>
{
    IMapSetter<TSource, TDestination> Map<TSrcMember, TDestMember>(Expression<Func<TSource, TSrcMember>> source,
        Expression<Func<TDestination, TDestMember>> member);

    IMapSetter<TSource, TDestination> Ignore(params Expression<Func<TDestination, object>>[] members);

    IMapSetter<TSource, TDestination> AfterMapping(Action<TSource, TDestination> action);

    /// <summary>
    /// TSource -> TDerived 复用当前配置。<see cref="https://github.com/MapsterMapper/Mapster/wiki/Config-inheritance"/>
    /// </summary>
    IMapSetter<TSource, TDerived> Derived<TDerived>() where TDerived : TDestination;
}
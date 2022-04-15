using System.Linq.Expressions;
using Mapster;

namespace Tamos.AbleOrigin.Booster;

internal readonly struct ObjMapSetter<TSource, TDestination> : IMapSetter<TSource, TDestination>
{
    private readonly TypeAdapterSetter<TSource, TDestination> _innerSetter;

    internal ObjMapSetter(TypeAdapterSetter<TSource, TDestination> setter)
    {
        _innerSetter = setter;
    }

    public IMapSetter<TSource, TDestination> Map<TSrcMember, TDestMember>(Expression<Func<TSource, TSrcMember>> source,
        Expression<Func<TDestination, TDestMember>> member)
    {
        _innerSetter.Map(member, source);
        return this;
    }

    public IMapSetter<TSource, TDestination> Ignore(params Expression<Func<TDestination, object>>[] members)
    {
        _innerSetter.Ignore(members);
        return this;
    }

    public IMapSetter<TSource, TDestination> AfterMapping(Action<TSource, TDestination> action)
    {
        _innerSetter.AfterMapping(action);
        return this;
    }

    public IMapSetter<TSource, TDerived> Derived<TDerived>() where TDerived : TDestination
    {
        var derSet = TypeAdapterConfig<TSource, TDerived>.NewConfig().Inherits<TSource, TDestination>();
        return new ObjMapSetter<TSource, TDerived>(derSet);
    }
}
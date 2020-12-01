using System;
using System.Linq.Expressions;

namespace Tamos.AbleOrigin.Mapper
{
    public abstract class BaseMapSetter<TSource, TDestination>
    {
        public abstract BaseMapSetter<TSource, TDestination> Map<TSrcMember, TDestMember>(Expression<Func<TSource, TSrcMember>> source,
            Expression<Func<TDestination, TDestMember>> member);

        public abstract BaseMapSetter<TSource, TDestination> Ignore(params Expression<Func<TDestination, object>>[] members);

        public abstract BaseMapSetter<TSource, TDestination> AfterMapping(Action<TSource, TDestination> action);
    }
}
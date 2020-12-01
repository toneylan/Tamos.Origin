using System;
using System.Linq.Expressions;
using Mapster;
using Tamos.AbleOrigin.Mapper;

namespace Tamos.AbleOrigin.Booster
{
    internal class ObjMapSetter<TSource, TDestination> : BaseMapSetter<TSource, TDestination>
    {
        private readonly TypeAdapterSetter<TSource, TDestination> _innerSetter;

        internal ObjMapSetter(TypeAdapterSetter<TSource, TDestination> setter)
        {
            _innerSetter = setter;
        }

        public override BaseMapSetter<TSource, TDestination> Map<TSrcMember, TDestMember>(Expression<Func<TSource, TSrcMember>> source,
            Expression<Func<TDestination, TDestMember>> member)
        {
            _innerSetter.Map(member, source);
            return this;
        }

        public override BaseMapSetter<TSource, TDestination> Ignore(params Expression<Func<TDestination, object>>[] members)
        {
            _innerSetter.Ignore(members);
            return this;
        }

        public override BaseMapSetter<TSource, TDestination> AfterMapping(Action<TSource, TDestination> action)
        {
            _innerSetter.AfterMapping(action);
            return this;
        }
    }
}
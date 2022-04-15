using Mapster;

namespace Tamos.AbleOrigin.Booster
{
    /// <summary>
    /// Mapper，效率高于AutoMapper，且支持Flexible Name
    /// </summary>
    internal class ObjMapster : IObjectMapper
    {
        #region Reg

        /// <summary>
        /// 注册新的Map
        /// </summary>
        public void Reg<TSource, TDestination>(bool flexName = false)
        {
            var conf = TypeAdapterConfig<TSource, TDestination>.NewConfig();
            if (flexName) conf.NameMatchingStrategy(NameMatchingStrategy.Flexible);
        }

        /// <summary>
        /// 注册Map并返回设置对象
        /// </summary>
        public IMapSetter<TSource, TDestination> RegSetter<TSource, TDestination>(bool flexName = false)
        {
            var conf = TypeAdapterConfig<TSource, TDestination>.NewConfig();
            return new ObjMapSetter<TSource, TDestination>(flexName ? conf.NameMatchingStrategy(NameMatchingStrategy.Flexible) : conf);
        }
        
        #endregion
        
        #region Map
        
        /// <summary>
        /// Map为目标类型
        /// </summary>
        public TDestination Map<TDestination>(object source)
        {
            return source.Adapt<TDestination>();
        }

        /// <summary>
        /// 将对象Map到已有实例
        /// </summary>
        public TDestination Map<TSource, TDestination>(TSource source, TDestination dest)
        {
            return source.Adapt(dest);
        }

        #endregion

        /// <summary>
        /// 编译已注册的Map，同时能检查正确性
        /// </summary>
        public void Compile()
        {
            TypeAdapterConfig.GlobalSettings.Compile();
        }
    }
}

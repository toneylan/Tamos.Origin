using System.Diagnostics.CodeAnalysis;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 对象间Map，如：Entity - DTO。
    /// </summary>
    public static class EntMapper
    {
        private static IObjectMapper? _objMapper;
        // ReSharper disable once AssignNullToNotNullAttribute
        private static IObjectMapper ObjMapper => _objMapper ??= ServiceLocator.GetOrReflect<IObjectMapper>("ObjMapster");

        /// <summary>
        /// 设置IObjectMapper实现，由Booster调用。
        /// </summary>
        public static void SetProvider(IObjectMapper objMapper)
        {
            _objMapper = objMapper;
        }

        #region Reg

        /// <summary>
        /// 注册新的Map
        /// </summary>
        public static void Reg<TSource, TDestination>(bool flexName = false)
        {
            ObjMapper.Reg<TSource, TDestination>(flexName);
        }

        /// <summary>
        /// 注册Map并返回设置对象
        /// </summary>
        public static IMapSetter<TSource, TDestination> RegSetter<TSource, TDestination>(bool flexName = false)
        {
            return ObjMapper.RegSetter<TSource, TDestination>(flexName);
        }

        #endregion

        #region Map

        /// <summary>
        /// Map为目标类型
        /// </summary>
        [return: NotNullIfNotNull("source")]
        public static TDestination? Map<TDestination>(object? source)
        {
            return source == null ? default : ObjMapper.Map<TDestination>(source);
        }
        
        /// <summary>
        /// 将对象Map到已有实例
        /// </summary>
        public static TDestination? Map<TSource, TDestination>(TSource? source, TDestination? dest)
        {
            if (source == null) return dest;
            return dest != null ? ObjMapper.Map(source, dest) : ObjMapper.Map<TDestination>(source);
        }

        #endregion

        /// <summary>
        /// 编译已注册的Map，同时能检查正确性
        /// </summary>
        public static void Compile()
        {
            ObjMapper.Compile();
        }
    }
}
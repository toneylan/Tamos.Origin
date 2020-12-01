namespace Tamos.AbleOrigin.Mapper
{
    public interface IObjectMapper
    {
        /// <summary>
        /// 注册新的Map。Flexible Name Mapping：如UserName对应userName或user_name。
        /// </summary>
        void Reg<TSource, TDestination>(bool flexName = false);

        /// <summary>
        /// 注册Map并返回设置对象
        /// </summary>
        BaseMapSetter<TSource, TDestination> RegSetter<TSource, TDestination>(bool flexName = false);

        TDestination Map<TDestination>(object source);

        /// <summary>
        /// 将对象Map到已有实例
        /// </summary>
        TDestination Map<TSource, TDestination>(TSource source, TDestination dest);

        /// <summary>
        /// 编译已注册的Map，同时能检查正确性
        /// </summary>
        void Compile();
    }
}
namespace Tamos.AbleOrigin;

/// <summary>
/// Lib common extensions method
/// </summary>
public static class AbleOriginExtensions
{
    #region IOC

    /// <summary>
    /// 同时注册TService、TImpl。
    /// </summary>
    public static IServiceContainer RegisterBoth<TService, TImpl>(this IServiceContainer container) where TService : class where TImpl : class, TService
    {
        return container.Register<TImpl>()
            .Register<TService, TImpl>();
    }

    #endregion
}
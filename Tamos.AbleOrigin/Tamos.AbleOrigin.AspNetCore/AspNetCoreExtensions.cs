using Microsoft.Extensions.DependencyInjection;

namespace Tamos.AbleOrigin.AspNetCore;

public static class AspNetCoreExtensions
{
    private static IServiceProvider _svcProvider;

    /// <summary>
    /// 将实例设置到ServiceLocator，以备静态方法中获取DI实例。
    /// </summary>
    internal static void UseProvider(this IServiceProvider provider)
    {
        _svcProvider = provider;
    }

    /// <summary>
    /// 从IServiceProvider（DI）获取实例。
    /// </summary>
    public static T GetRequiredService<T>(this IServiceContainer _)
    {
        return _svcProvider.GetRequiredService<T>();
    }
}
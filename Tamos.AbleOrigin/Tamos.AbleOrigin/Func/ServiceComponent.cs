namespace Tamos.AbleOrigin;

/// <summary>
/// 服务组件基类，可用于缓存创建的其他实例，使其跟自身一起释放。
/// </summary>
public abstract class ServiceComponent : IDisposable
{
    private Dictionary<string, IDisposable>? _cachedComponents;
    private Dictionary<string, IDisposable> CachedComponents => _cachedComponents ??= new Dictionary<string, IDisposable>();

    #region GetComponent

    /// <summary>
    /// 获取或创建组件实例（实现重用），会跟随当前对象一起释放。
    /// </summary>
    public T GetComponent<T>() where T : IDisposable, new()
    {
        var typeName = typeof(T).GetFullName();
        if (CachedComponents.TryGetValue(typeName, out var srv)) return (T)srv;

        var newSrv = new T();
        CachedComponents.Add(typeName, newSrv);
        return newSrv;
    }

    /// <summary>
    /// 获取组件实例，没有时创建并缓存。
    /// </summary>
    public T GetComponent<T>(Func<T> createFunc, string? typeKey = null) where T : IDisposable
    {
        typeKey ??= typeof(T).GetFullName();
        if (CachedComponents.TryGetValue(typeKey, out var srv)) return (T)srv;

        var newSrv = createFunc();
        CachedComponents.Add(typeKey, newSrv);
        return newSrv;
    }

    #endregion

    /*#region Util

    /// <summary>
    /// ServiceLocator.GetInstance
    /// </summary>
    protected T GetInstance<T>() where T : class => ServiceLocator.GetInstance<T>();

    #endregion*/

    public virtual void Dispose()
    {
        if (_cachedComponents?.Count > 0)
        {
            foreach (var comp in _cachedComponents.Values)
            {
                comp.Dispose();
            }
        }

        //Log.LogService.DebugFormat("Dispose service:{0}", GetType().Name);
    }
}
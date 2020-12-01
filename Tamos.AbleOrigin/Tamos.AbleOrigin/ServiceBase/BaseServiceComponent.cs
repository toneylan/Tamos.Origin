using System;
using System.Collections.Generic;

namespace Tamos.AbleOrigin.ServiceBase
{
    /// <summary>
    /// 服务组件基类
    /// </summary>
    public abstract class BaseServiceComponent : IDisposable
    {
        private Dictionary<string, IDisposable> _cachedComponents;
        private Dictionary<string, IDisposable> CachedComponents => _cachedComponents ??= new Dictionary<string, IDisposable>();

        #region GetComponent

        /// <summary>
        /// 获取或创建组件实例（实现重用），会跟随当前对象一起释放。
        /// </summary>
        public T GetComponent<T>() where T : IDisposable, new()
        {
            var typeName = typeof(T).FullName ?? typeof(T).Name;
            if (CachedComponents.TryGetValue(typeName, out var srv)) return (T) srv;

            var newSrv = new T();
            CachedComponents.Add(typeName, newSrv);
            return newSrv;
        }

        /// <summary>
        /// 获取组件实例，没有时创建并缓存。
        /// </summary>
        public T GetComponent<T>(Func<T> createFunc, string typeKey = null) where T : IDisposable
        {
            if (typeKey == null) typeKey = typeof(T).FullName ?? typeof(T).Name;
            if (CachedComponents.TryGetValue(typeKey, out var srv)) return (T) srv;

            var newSrv = createFunc();
            CachedComponents.Add(typeKey, newSrv);
            return newSrv;
        }

        #endregion

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
}
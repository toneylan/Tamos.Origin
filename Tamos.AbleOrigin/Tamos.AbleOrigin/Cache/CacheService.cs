using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// Cache service facade，default use Redis in cloud, or register ICacheProvider.
    /// </summary>
    public static class CacheService
    {
        // ReSharper disable once AssignNullToNotNullAttribute
        private static readonly ICacheProvider Provider = ServiceLocator.GetOrReflect<ICacheProvider>("RedisCache");
        
        private static Dictionary<string, DateTime>? DicLoadTime;
        private static readonly Dictionary<string, object> DicTrace = new ();
        private static readonly Regex TraceNameRegex = new (@"^\w+\.(\w+)\.\w+$"); //用于缩短Namespace

        /// <summary>
        /// 是否开启miss时日志记录
        /// </summary>
        public static bool LogMiss { get; set; }

        #region Get Set Delete

        /// <summary>
        /// 获取缓存的数据
        /// </summary>
        public static T? Get<T>(string key)
        {
            try
            {
                return Provider.Get<T>(key);
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return default;
            }
        }

        /// <summary>
        /// 获取缓存的数据
        /// </summary>
        public static Task<T?> GetAsync<T>(string key)
        {
            try
            {
                return Provider.GetAsync<T>(key);
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return Task.FromResult<T>(default);
            }
        }
        
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        public static void Set<T>(string key, T data, TimeSpan? expireSpan = null)
        {
            try
            {
                Provider.Set(key, data, expireSpan);
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
        }

        /// <summary>
        /// 删除指定的缓存项
        /// </summary>
        public static void Delete(string key)
        {
            try
            {
                Provider.Delete(key);
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
        }

        /// <summary>
        /// 删除多个缓存项
        /// </summary>
        public static void Delete(params string[] keys)
        {
            try
            {
                Provider.Delete(keys);
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
        }

        #endregion

        #region Quick get

        /// <summary>
        /// 从缓存中获取数据，若miss则回调数据并插入缓存。
        /// </summary>
        public static T Get<T>(string key, Func<T> funGetData, TimeSpan? expireSpan)
        {
            var res = Get<T>(key);
            if (res != null) return res;

            res = funGetData();
            if (res != null) Set(key, res, expireSpan);

            if (LogMiss) LogService.DebugFormat("CacheGet<{0}> miss: {1}", typeof(T).Name, key);
            return res;
        }

        #endregion

        #region Trace

        /// <summary>
        /// 获取类型T对应的Trace对象。
        /// </summary>
        public static CacheTrace<T> Trace<T>(bool autoCreate = true)
        {
            var type = typeof(T);
            var dicKey = type.GetFullName();
            if (DicTrace.TryGetValue(dicKey, out var obj)) return (CacheTrace<T>)obj;
            if (!autoCreate) return null!;

            //Create new Trace
            var match = TraceNameRegex.Match(type.Namespace ?? type.Name);
            var traceName = match.Success ? $"{match.Groups[1].Value}.{type.Name}_" : dicKey;

            var trace = new CacheTrace<T>(traceName);
            DicTrace.TryAdd(dicKey, trace);
            return trace;
        }

        /// <summary>
        /// 依据entity类型的Trace信息，自动清除对应的缓存值。
        /// </summary>
        public static void DelByTrace<T>(params T[] entitys)
        {
            var trace = Trace<T>(false);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (trace == null) return;

            var keys = entitys.SelectMany(x => trace.GetRemoveKeys(x)).ToArray();
            if (keys.IsNull()) return;
            
            Delete(keys);
            LogService.DebugFormat("Cache delete by trace: {0}", string.Join(", ", keys));
        }
        
        #endregion

        #region Util

        /// <summary>
        /// 进程内缓存，判断某项数据是否需要重载。返回true时会同时记录当前加载时间。
        /// </summary>
        public static bool NeedReload(string name, int expireMinutes)
        {
            if (string.IsNullOrEmpty(name)) return false;
            DicLoadTime ??= new Dictionary<string, DateTime>();

            var now = DateTime.Now;
            if (DicLoadTime.TryGetValue(name, out var lastLoad) && (now - lastLoad).TotalMinutes < expireMinutes)
            {
                return false;
            }

            //记录加载
            DicLoadTime[name] = now;
            return true;
        }

        #endregion
    }
}
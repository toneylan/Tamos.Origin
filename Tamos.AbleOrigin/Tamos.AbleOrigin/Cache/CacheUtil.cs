using System;
using System.Collections.Generic;

namespace Tamos.AbleOrigin.Cache
{
    public static class CacheUtil
    {
        private static readonly Dictionary<string, DateTime> DicLoadTime = new Dictionary<string, DateTime>();

        /// <summary>
        /// 判断某项数据是否需要重载。返回true时会同时记录当前加载时间。
        /// </summary>
        public static bool NeedReload(string name, int expireMinutes)
        {
            if (string.IsNullOrEmpty(name)) return false;

            var now = DateTime.Now;
            if (DicLoadTime.TryGetValue(name, out var lastLoad) && (now - lastLoad).TotalMinutes < expireMinutes)
            {
                return false;
            }

            //记录加载
            DicLoadTime[name] = now;
            return true;
        }
    }
}
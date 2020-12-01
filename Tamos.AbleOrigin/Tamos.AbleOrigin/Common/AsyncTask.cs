﻿using System;
using System.Threading.Tasks;
using Tamos.AbleOrigin.IOC;
using Tamos.AbleOrigin.Log;

namespace Tamos.AbleOrigin
{
    public static class AsyncTask
    {
        /// <summary>
        /// 在一个IOC Scope中执行异步任务，确保过程中创建的实例，最后能被释放。
        /// </summary>
        public static Task RunInScope(Action action)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (ServiceLocator.Container.BeginScope())
                    {
                        action();
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex);
                }
            });
        }
    }
}
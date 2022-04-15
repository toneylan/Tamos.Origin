using System;
using System.IO;

namespace Tamos.AbleOrigin
{
    public static class HostApp
    {
        #region Property

        /// <summary>
        /// 获取当前应用程序目录的绝对路径。
        /// </summary>
        public static string RootPath => AppContext.BaseDirectory; //Directory.GetCurrentDirectory()

        /// <summary>
        /// 当前应用的配置信息
        /// </summary>
        public static HostAppOptions Options { get; } = new();
        
        #endregion

        #region Path

        /// <summary>
        /// 将相对应用根目录的路径，转换为绝对路径。
        /// </summary>
        public static string GetPath(string relativePath)
        {
            return Path.Combine(RootPath, relativePath);
        }

        #endregion
    }
}
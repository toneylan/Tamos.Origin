using System.Diagnostics.CodeAnalysis;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 具体某类配置的基类
    /// </summary>
    public abstract class BaseCatalogConfig<T> where T : new()
    {
        #region Static prop

        private static T? _set;

        /// <summary>
        /// 配置类实例，主要用于扩展自定义的获取配置方法。
        /// </summary>
        public static T Set => _set ??= new T();

        #endregion
        
        /// <summary>
        /// 类别目录，可用于CentralConfig中划分目录。
        /// </summary>
        protected abstract string Catalog { get; }

        /// <summary>
        /// 获取当前类别目录下的配置
        /// </summary>
        [return: NotNullIfNotNull("defaultVal")]
        public string? Get(string key, string? defaultVal = null)
        {
            return CentralConfiguration.Get($"{Catalog}/{key}", defaultVal);
        }
    }
}
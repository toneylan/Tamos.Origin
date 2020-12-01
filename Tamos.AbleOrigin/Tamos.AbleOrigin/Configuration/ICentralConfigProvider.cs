namespace Tamos.AbleOrigin.Configuration
{
    public interface ICentralConfigProvider
    {
        /// <summary>
        /// 设置K/V值，key可用"/"划分目录
        /// </summary>
        bool Set(string key, string value);

        /// <summary>
        /// 获取K/V值，key可包含"/"划分的目录
        /// </summary>
        string Get(string key);

    }

    public interface IAppSettingProvider
    {
        /// <summary>
        /// 获取程序本地配置
        /// </summary>
        string GetAppSetting(string key);
    }
}
namespace Tamos.AbleOrigin.Configuration
{
    public interface IConfigStorage
    {
        T GetConfig<T>();

        T GetConfig<T>(string confKey);

        bool SaveConfig<T>(T config);

        bool SaveConfig<T>(T config, string confKey);
    }
}
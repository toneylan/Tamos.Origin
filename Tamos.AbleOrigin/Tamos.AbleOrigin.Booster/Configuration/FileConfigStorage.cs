using System;
using System.IO;
using System.Text;

namespace Tamos.AbleOrigin.Booster
{
    internal class FileConfigStorage : IConfigStorage
    {
        private const string ConfigDir = "Config";

        public T GetConfig<T>()
        {
            return GetConfig<T>(typeof (T).Name);
        }

        public T GetConfig<T>(string confKey)
        {
            var confFile = HostApp.GetPath($"{ConfigDir}\\{confKey}.json");
            if (!File.Exists(confFile)) return default(T);

            try
            {
                return SerializeUtil.FromJson<T>(File.ReadAllText(confFile, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                LogService.Error(ex);
                return default(T);
            }
        }

        public bool SaveConfig<T>(T config)
        {
            return SaveConfig(config, typeof(T).Name);
        }

        public bool SaveConfig<T>(T config, string confKey)
        {
            var confDir = HostApp.GetPath(ConfigDir);
            try
            {
                if (!Directory.Exists(confDir)) Directory.CreateDirectory(confDir);
                File.WriteAllText($"{confDir}\\{confKey}.json", SerializeUtil.ToJson(config), Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                LogService.Error(ex);
                return false;
            }
        }
    }
}
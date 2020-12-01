using System;
using System.IO;
using System.Text;
using Tamos.AbleOrigin.Configuration;
using Tamos.AbleOrigin.Log;
using Tamos.AbleOrigin.Serialize;

namespace Tamos.AbleOrigin.Booster
{
    internal class FileConfigStorage : IConfigStorage
    {
        public T GetConfig<T>()
        {
            return GetConfig<T>(typeof (T).Name);
        }

        public T GetConfig<T>(string confKey)
        {
            var confFile = $"{AppDomain.CurrentDomain.BaseDirectory}\\Conf\\{confKey}.config";
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
            return SaveConfig<T>(config, typeof(T).Name);
        }

        public bool SaveConfig<T>(T config, string confKey)
        {
            var confDir = $"{AppDomain.CurrentDomain.BaseDirectory}\\Conf";
            try
            {
                if (!Directory.Exists(confDir)) Directory.CreateDirectory(confDir);
                File.WriteAllText($"{confDir}\\{confKey}.config", SerializeUtil.ToJson(config), Encoding.UTF8);
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
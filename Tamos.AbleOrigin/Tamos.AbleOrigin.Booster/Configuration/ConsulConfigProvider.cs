using System;
using System.Text;
using Consul;
using Tamos.AbleOrigin.Configuration;
using Tamos.AbleOrigin.Log;

namespace Tamos.AbleOrigin.Booster
{
    internal class ConsulConfigProvider : ICentralConfigProvider, IDisposable
    {
        private static string ConsulAddress => AppSettingProvider.InnerGetAppSet("CentralConfigAddress") ?? "http://127.0.0.1:8605/";
        
        //断网恢复后，ConsulClient可继续使用
        private readonly ConsulClient _client = new ConsulClient(conf => conf.Address = new Uri(ConsulAddress));

        public ConsulConfigProvider()
        {
            if (CentralConfiguration.IsEnvDev()) LogService.DebugFormat("DeployEnv:{0}, Consul address:{1}", CentralConfiguration.DeployEnv, _client.Config.Address);
        }
        
        #region Set & Get

        public bool Set(string key, string value)
        {
            var putPair = new KVPair(key)
            {
                Value = Encoding.UTF8.GetBytes(value)
            };

            //同步执行
            var res = _client.KV.Put(putPair).Result;
            return res.Response;
        }

        public string Get(string key)
        {
            //同步执行
            var res = _client.KV.Get(key).Result;
            if (res.Response?.Value == null) return null;

            return Encoding.UTF8.GetString(res.Response.Value);
        }
        
        #endregion

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
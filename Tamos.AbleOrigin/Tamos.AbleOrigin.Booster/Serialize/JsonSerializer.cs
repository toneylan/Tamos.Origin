using Newtonsoft.Json;

namespace Tamos.AbleOrigin.Booster
{
    internal class JsonSerializer : IJsonSerializer
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string srcStr)
        {
            return JsonConvert.DeserializeObject<T>(srcStr);
        }
    }
}
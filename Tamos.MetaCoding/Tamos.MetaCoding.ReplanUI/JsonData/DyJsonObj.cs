using Newtonsoft.Json.Linq;

namespace Tamos.MetaCoding.ReplanUI
{
    /// <summary>
    /// 动态Json数据对象
    /// </summary>
    public class DyJsonObj : JObject
    {
        //private readonly JObject _data = new JObject();

        public DyJsonObj Set(string name, JToken? value)
        {
            //this[name] = new JValue(value); 
            Add(name, value);

            return this;
        }
    }
}
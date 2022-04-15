namespace Tamos.AbleOrigin
{
    /// <summary>
    /// ！暂未实现
    /// .Net 原生的Json序列化，缺省时使用。
    /// </summary>
    internal class JsonNativeSerializer : IJsonSerializer
    {
        public string Serialize(object obj)
        {
            /*DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());

            using (MemoryStream stream = new MemoryStream())

            {

                json.WriteObject(stream, obj);

                string szJson = Encoding.UTF8.GetString(stream.ToArray()); 

                return szJson;

            }*/
            throw new System.NotImplementedException();
        }

        public T Deserialize<T>(string srcStr)
        {
            throw new System.NotImplementedException();
        }
    }
}
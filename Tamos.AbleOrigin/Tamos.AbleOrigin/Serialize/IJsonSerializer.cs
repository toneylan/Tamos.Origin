namespace Tamos.AbleOrigin
{
    /// <summary>
    /// Json序列化功能接口
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// 序列化对象为字符串
        /// </summary>
        string Serialize(object obj);

        /// <summary>
        /// 从字符串反序列化到对象
        /// </summary>
        T Deserialize<T>(string srcStr);
    }
}
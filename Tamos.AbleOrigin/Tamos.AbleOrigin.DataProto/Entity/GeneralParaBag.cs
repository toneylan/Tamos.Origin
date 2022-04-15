using System.Runtime.Serialization;
using System.Text;
using System.Web;

namespace Tamos.AbleOrigin.DataProto;

/// <summary>
/// 通用参数数据的包装，简化参数设置、拼接等操作。
/// </summary>
[DataContract]
public class GeneralParaBag
{
    [DataMember]
    protected Dictionary<string, string?> Paras { get; set; } = new();

    /// <summary>
    /// 获取参数列表
    /// </summary>
    public IReadOnlyCollection<KeyValuePair<string, string?>> GetData() => Paras;

    #region Set method

    /// <summary>
    /// 设置一个参数值
    /// </summary>
    public GeneralParaBag Set(string key, string? value)
    {
        Paras[key] = value;
        return this;
    }

    /// <summary>
    /// Append对象属性，不会添加null值。
    /// </summary>
    public void SetObj<T>(T obj)
    {
        foreach (var prop in EntityMeta.Get<T>(true).GetProps())
        {
            var value = prop.GetValueAsString(obj);
            if (value != null) Paras[prop.Name] = value;
        }
    }

    #endregion

    #region Convert to

    /// <summary>
    /// 参数名排序后，用<![CDATA[ & ]]>连接：“参数名=值” 
    /// </summary>
    public string ToUrlPara(bool encodeValue = true)
    {
        var sb = new StringBuilder();
        foreach (var key in Paras.Keys.OrderBy(x => x))
        {
            sb.AppendFormat("{0}{1}={2}", sb.Length > 0 ? "&" : null, key, encodeValue ? HttpUtility.UrlEncode(Paras[key]) : Paras[key]);
        }

        return sb.ToString();
    }

    #endregion
}
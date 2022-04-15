namespace Tamos.Connect.Weixin;

public class TemplateMsgPara
{
    /// <summary>
    /// 目标用户OpenId
    /// </summary>
    public string touser { get; set; }

    /// <summary>
    /// 模板ID
    /// </summary>
    public string template_id { get; set; }

    /// <summary>
    /// 模板跳转链接
    /// </summary>
    public string? url { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    public Dictionary<string, TmplMsgDataItem> data { get; } = new();

    /*/// <summary>
    /// [否] 模板内容字体颜色，不填默认为黑色
    /// </summary>
    public string? color { get; set; }*/

    /*/// <summary>
    /// 跳小程序所需数据，不需跳小程序可不用传该数据
    /// </summary>
    public object miniprogram { get; set; }*/

    #region Set data method

    public TemplateMsgPara Set(string key, string value, string? color = null)
    {
        data[key] = new TmplMsgDataItem { value = value, color = color };
        return this;
    }

    #endregion
}

public class TmplMsgDataItem
{
    public string value { get; set; }
    public string? color { get; set; }
}
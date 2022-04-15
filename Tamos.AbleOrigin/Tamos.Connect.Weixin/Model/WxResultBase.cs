namespace Tamos.Connect.Weixin;

internal class WxResultBase
{
    public int errcode { get; set; }

    public string? errmsg { get; set; }

    public bool IsSuccess() => errcode == 0;

}
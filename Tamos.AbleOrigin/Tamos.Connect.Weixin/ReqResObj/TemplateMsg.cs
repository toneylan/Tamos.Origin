namespace Tamos.Connect.Weixin
{
    public class TemplateMsgSendPara
    {
        public string touser { get; set; }
        public string template_id { get; set; }
        public string url { get; set; }
        public object data { get; set; }
    }
}
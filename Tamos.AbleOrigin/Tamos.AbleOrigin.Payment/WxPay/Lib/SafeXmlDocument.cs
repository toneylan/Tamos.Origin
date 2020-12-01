using System.Xml;

namespace Tamos.AbleOrigin.Payment.WxPay
{
    internal class SafeXmlDocument:XmlDocument
    {
        public SafeXmlDocument()
        {
            this.XmlResolver = null;
        }
    }
}

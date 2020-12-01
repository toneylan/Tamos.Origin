using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Tamos.Connect.Weixin
{
    [XmlRoot(ElementName = "xml", Namespace = "")]
    public class BaseInteractMsg
    {
        [XmlIgnore]
        protected XmlDocument XmlData { get; set; }

        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
        public long CreateTime { get; set; }
        public string MsgType { get; set; }

        public string this[string xpath]
        {
            get
            {
                if (XmlData == null || XmlData.DocumentElement == null) return null;
                var node = XmlData.DocumentElement.SelectSingleNode(xpath);
                return node != null ? node.InnerText : null;
            }
            /*set
            {
                if (XmlData == null) return;
                XmlData.crea
            }*/
        }

        public string SerializeToXml()
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings {Indent = true, OmitXmlDeclaration = true};
            using (var writer = XmlWriter.Create(sb, settings))
            {
                var xns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                new XmlSerializer(GetType()).Serialize(writer, this, xns);
            }
            return sb.ToString();
        }
    }
}
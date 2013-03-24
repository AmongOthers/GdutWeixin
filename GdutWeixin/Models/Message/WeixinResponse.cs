using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace GdutWeixin.Models.Message
{
    public abstract class WeixinResponse
    {
        public class StringXmlCDataSection : XmlCDataSection
        {
            static readonly XmlDocument sDoc = new XmlDocument();
            public StringXmlCDataSection(string value)
				: base(value, sDoc)
            {
            }
        }

		public StringXmlCDataSection ToUserName { get; set; }
        public StringXmlCDataSection FromUserName { get; set; }
        public string CreateTime { get; set; }
        public StringXmlCDataSection MsgType { get; set; }
        public int FuncFlag { get; set; }

        class NoNameSpaceXsn : XmlSerializerNamespaces
        {
            public NoNameSpaceXsn()
            {
                Add(String.Empty, String.Empty);
            }
        }

        static NoNameSpaceXsn sXsn = new NoNameSpaceXsn();

        public WeixinResponse()
        {
        }

        public WeixinResponse(string reqFromUserName)
        {
            ToUserName = new StringXmlCDataSection(reqFromUserName);
            FromUserName = new StringXmlCDataSection(Consts.ACCOUNT);
            CreateTime = Utils.DateTimeHelper.Timestamp(DateTime.Now).ToString();
        }

		public override string ToString()
        {
			XmlSerializer xs = new XmlSerializer(this.GetType());
            using (var ms = new MemoryStream())
            {
				XmlWriter xw = XmlWriter.Create(ms);
                xs.Serialize(ms, this, sXsn);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
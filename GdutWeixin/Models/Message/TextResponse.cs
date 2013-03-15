using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace GdutWeixin.Models.Message
{
	[XmlRoot("xml")]
    public class TextResponseMsg : Response
    {
        public StringXmlCDataSection Content { get; set; }
        public int FuncFlag { get; set; }

        public TextResponseMsg()
        {
            MsgType = new Response.StringXmlCDataSection("text");
        }
    }
}
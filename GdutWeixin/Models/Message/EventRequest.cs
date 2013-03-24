using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GdutWeixin.Models.Message
{
	[XmlRoot("xml")]
    public class EventRequest : WeixinRequest
    {
        public string Event { get; set; }
        public string EventKey { get; set; }
    }
}
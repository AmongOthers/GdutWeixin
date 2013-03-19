﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace GdutWeixin.Models.Message
{
	[XmlRoot("xml")]
    public class TextResponse : Response
    {
        public StringXmlCDataSection Content { get; set; }

        public TextResponse()
        {
        }

        public TextResponse(string reqFromUserName)
			: base(reqFromUserName)
        {
            MsgType = new Response.StringXmlCDataSection("text");
        }
    }
}
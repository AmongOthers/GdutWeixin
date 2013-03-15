using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace GdutWeixin.Models.Message
{
    public class RequestMsgFactory
    {
        public static Request Parse(string msg)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(msg)))
            {
                XmlSerializer serializer = null;
                if (msg.IndexOf("<MsgType><![CDATA[text]]></MsgType>") >= 0)
                {
                    serializer = new XmlSerializer(typeof(TextRequestMsg));
                }
                if (serializer != null)
                {
                    return serializer.Deserialize(ms) as TextRequestMsg;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
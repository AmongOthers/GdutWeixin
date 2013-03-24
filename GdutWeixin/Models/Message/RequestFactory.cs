using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace GdutWeixin.Models.Message
{
    public class RequestFactory
    {
        public static WeixinRequest Parse(string msg)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(msg)))
            {
                XmlSerializer serializer = null;
                if (msg.IndexOf("<MsgType><![CDATA[text]]></MsgType>") >= 0)
                {
                    serializer = new XmlSerializer(typeof(TextRequest));
                    return serializer.Deserialize(ms) as TextRequest;
                }
                else if (msg.IndexOf("<MsgType><![CDATA[image]]></MsgType>") >= 0)
                {
                    serializer = new XmlSerializer(typeof(ImageRequest));
                    return serializer.Deserialize(ms) as ImageRequest;
                }
                else if (msg.IndexOf("<MsgType><![CDATA[event]]></MsgType>") >= 0)
                {
                    serializer = new XmlSerializer(typeof(EventRequest));
                    return serializer.Deserialize(ms) as EventRequest;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
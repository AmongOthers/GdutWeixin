using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GdutWeixin.Models.Message
{
    public abstract class WeixinRequest
    {
        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
        public string CreateTime { get; set; }
        public string MsgType { get; set; }
        public string MsgId { get; set; }
    }
}
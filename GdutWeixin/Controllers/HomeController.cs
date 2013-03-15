using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GdutWeixin.Models;
using GdutWeixin.Models.Message;
using GdutWeixin.Utils;

namespace GdutWeixin.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            bool isFromWeixin = false;
            if (!String.IsNullOrEmpty(Request["signature"]))
            {
                if (Utils.CheckSignature.Check(Request["signature"], Request["timestamp"], Request["nonce"]))
                {
                    isFromWeixin = true;
                }
            }
            if (!isFromWeixin)
            {
                return "验证错误";
            }
			//微信接入消息
            if (Request.HttpMethod == "GET")
            {
                return Request["echostr"];
            }
            else
            {
				string postBody = null;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    postBody = reader.ReadToEnd();
                    HttpContext.ApplicationInstance.GetLogger().Info(String.Format("req: {0}", postBody));
                    var reqMsg = RequestMsgFactory.Parse(postBody);
                    if (reqMsg != null)
                    {
						var rsp = onRequestMsgReceived(reqMsg);
                        return rsp;
                    }
                }
                return "未知消息";
            }
        }

        private string onRequestMsgReceived(Request reqMsg)
        {
            Response msg = null;
            if (reqMsg is TextRequestMsg)
            {
                msg = new TextResponseMsg {
					ToUserName = new Response.StringXmlCDataSection(reqMsg.FromUserName),
                    Content = new Response.StringXmlCDataSection((reqMsg as TextRequestMsg).Content),
					FuncFlag = 0
                };
            }
            var msgStr = msg == null ? null : msg.ToString();
			HttpContext.ApplicationInstance.GetLogger().Info(String.Format("rsp: {0}", msgStr));
			return msgStr;
        }

        public ActionResult About()
        {
            ViewBag.Message = "你的应用程序说明页。";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "你的联系方式页。";

            return View();
        }
    }
}

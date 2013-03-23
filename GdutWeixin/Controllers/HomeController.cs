using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using GdutWeixin.Models;
using GdutWeixin.Models.Library;
using GdutWeixin.Models.Message;
using GdutWeixin.Utils;

namespace GdutWeixin.Controllers
{
	[SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class HomeController : Controller
    {
        public ContentResult Index()
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
                return Content("仅用于微信接入验证");
            }
            //微信接入验证消息
            else if (Request.HttpMethod == "GET")
            {
                return Content(Request["echostr"]);
            }
            //微信新消息
            else
            {
                string postBody = null;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    postBody = reader.ReadToEnd();
                    var reqMsg = RequestFactory.Parse(postBody);
                    if (reqMsg != null)
                    {
                        return onWeixinRequestReceived(reqMsg);
                    }
                    else
                    {
                        return Content("无法解析消息: " + postBody.ToString());
                    }
                }
            }
        }

        private ContentResult onWeixinRequestReceived(WeixinRequest reqMsg)
        {
            if (reqMsg is TextRequest)
            {
                var req = reqMsg as TextRequest;
                var user = reqMsg.FromUserName;
				var keyword = req.Content;
                return Content(Library.GetInstance().GetRspForSearch(Request, user, keyword).ToString());
            }
			else
            {
                return Content("暂未支持");
            }
        }

        public RedirectResult Image()
        {
            return new RedirectResult("http://www.dxcjjzx.com/Pic/20090417112635decrow.jpg");
        }

        public ActionResult About()
        {
            ApplicationLogger.GetLogger().Info("About visited");

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

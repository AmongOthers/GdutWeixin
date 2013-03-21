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
		
        public RedirectResult Index()
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
                return new ErrorRedirectResult("仅用于微信接入验证");
            }
            //微信接入验证消息
            else if (Request.HttpMethod == "GET")
            {
                return new RedirectResult(String.Format("/Home/WeixinValidate?echostr=",
                    HttpUtility.UrlEncode(Request["echostr"])));
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
                        return redirectRequest(reqMsg);
                    }
                    else
                    {
                        return new ErrorRedirectResult("无法解析消息: " + postBody.ToString());
                    }
                }
            }
        }

        private RedirectResult redirectRequest(WeixinRequest reqMsg)
        {
            if (reqMsg is TextRequest)
            {
                var req = reqMsg as TextRequest;
                var user = reqMsg.FromUserName;
				var keyword = req.Content;
				return new RedirectResult(String.Format("/Library/Search?keyword={0}&user={1}" +
                    HttpUtility.UrlEncode(keyword), HttpUtility.UrlEncode(user)));
            }
			else
            {
                return new RedirectResult("/Home/Unknown");
            }
        }

        public string WeixinValidate()
        {
            var str = Request["echostr"];
            return str;
        }

        public string Unknown()
        {
            return "暂未支持";
        }

		public class ErrorRedirectResult : RedirectResult
        {
            public ErrorRedirectResult(string error)
				: base(String.Format("/Home/Error?error={0}", HttpUtility.UrlEncode(error)))
            {
            }
        }

        public string Error()
        {
            var error = Request["error"];
            return error;
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

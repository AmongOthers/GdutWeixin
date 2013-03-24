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
			var user = reqMsg.FromUserName;
            if (reqMsg is TextRequest)
            {
                var req = reqMsg as TextRequest;
				var keyword = req.Content;

				//表情的过滤
                if (keyword.StartsWith("/:"))
                {
                    return Content(new TextResponse(user, keyword).ToString());
                }
				//留言建议
                else if (keyword.StartsWith("@"))
                {
                    return Content(new TextResponse(user, " /::)") { FuncFlag = 1 }.ToString());
                }
				//关注
                else if (keyword == "Hello2BizUser")
                {
                    return onSubscribed(reqMsg);
                }

                LibrarySearchOption option;
                string errMsg;
                if (UserCommand.GetInstance().OnMessage(user, keyword, out option, out errMsg))
                {
                    return Content(Library.GetInstance().GetRspForSearch(Request, option).ToString());
                }
                else
                {
                    return Content(new TextResponse(user, errMsg).ToString());
                }
            }
            else if (reqMsg is EventRequest)
            {
                var req = reqMsg as EventRequest;
                if (req.Event == "subscribe")
                {
                    return onSubscribed(req);
                }
                else if (req.Event == "unsubscribe")
                {
                    return onUnsubscribed(req);
                }
                else
                {
					return Content(new TextResponse(user, "暂未支持，敬请期待").ToString());
                }
            }
            else
            {
                return Content(new TextResponse(user, "暂未支持，敬请期待").ToString());
            }
        }

        private ContentResult onSubscribed(WeixinRequest reqMsg)
        {
            return Content(new TextResponse(reqMsg.FromUserName, Consts.WELCOME).ToString());
        }

        private ContentResult onUnsubscribed(EventRequest reqMsg)
        {
            return Content(new TextResponse(reqMsg.FromUserName, Consts.GOODBYE).ToString());
        }

        public RedirectResult Image()
        {
            return new RedirectResult("http://www.dxcjjzx.com/Pic/20090417112635decrow.jpg");
        }

        public ActionResult About()
        {
            ViewData["About"] = Consts.ABOUT;
			ViewData["Contact"] = Consts.CONTACT;
			ViewData["Func"] = Consts.FUNC;
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "你的联系方式页。";

            return View();
        }
    }
}

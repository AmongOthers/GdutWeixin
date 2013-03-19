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
    public class HomeController : Controller
    {
        private Stopwatch libStopwatch = Stopwatch.StartNew();
		
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
                    ApplicationLogger.GetLogger().Info(String.Format("req: {0}", postBody));
                    var reqMsg = RequestFactory.Parse(postBody);
                    if (reqMsg != null)
                    {
						var rsp = onRequestMsgReceived(reqMsg);
						ApplicationLogger.GetLogger().Info(String.Format("rsp: {0}", rsp));
                        return rsp;
                    }
                }
                return "未知消息";
            }
        }

        private string onRequestMsgReceived(Request reqMsg)
        {
            Response rsp = null;
            if (reqMsg is TextRequest)
            {
                var req = reqMsg as TextRequest;
                var user = reqMsg.FromUserName;
				var keyword = req.Content;
                libStopwatch.Restart();
				object error;
                var result = Library.GetInstance().SearchBooksFor(user, keyword, out error);
                if (error == null)
                {
                    rsp = LibrarySearchResponse.Create(Request, user, keyword, result.Books, result.MoreUrl);
                    libStopwatch.Stop();
                    ApplicationLogger.GetLogger().Info(String.Format("search library with {0} consume {1} ms",
                        keyword,
                        libStopwatch.ElapsedMilliseconds));
                }
                else
                {
                    rsp = new TextResponse(req.FromUserName)
                    {
                        Content = new Response.StringXmlCDataSection(String.Format("查询出错： {0}", error))
                    };
                }
            }
            else if (reqMsg is ImageRequest)
            {
                rsp = NewsResponse.GetTestMsg(reqMsg.FromUserName);
            }
            var msgStr = rsp == null ? null : rsp.ToString();
			return msgStr;
        }

        public RedirectResult Image()
        {
            return new RedirectResult("http://www.dxcjjzx.com/Pic/20090417112635decrow.jpg");
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

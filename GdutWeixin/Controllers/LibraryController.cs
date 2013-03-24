using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using GdutWeixin.Models;
using GdutWeixin.Models.Library;
using GdutWeixin.Models.Message;
using GdutWeixin.Utils;

namespace GdutWeixin.Controllers
{
	[SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class LibraryController : Controller
    {
        //
        // GET: /Library/

        public ActionResult Index()
        {
            return View();
        }

        public string Search()
        {
            //var keyword = Request["keyword"];
            var keyword = "thinking in java";
            var user = Request["user"];
            return Library.GetInstance().GetRspForSearch(Request, new LibrarySearchOption
            {
				Keyword = keyword,
				User = user
            });
        }

        public RedirectResult Conver()
        {
            var url = Request["url"];
            var bookInfo = Library.GetInstance().GetBookInfo(url);
            return new RedirectResult(BaiduImageSearchEngine.GetInstance().SearchImageFor(bookInfo.CardInfo.Isbn));
        }

        public JsonResult DetailsJson()
        {
			var url = "bookinfo.aspx?ctrlno=508536";
            var bookInfo = Library.GetInstance().GetBookInfo(url);
            return Json(bookInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details()
        {
            var url = Request["url"];
            var bookInfo = Library.GetInstance().GetBookInfo(url);
            return View(bookInfo);
        }
    }
}

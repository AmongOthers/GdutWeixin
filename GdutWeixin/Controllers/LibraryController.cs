using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
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

        public ActionResult Result(LibrarySearchOption option)
        {
            option.PageSize = 50;
            ViewData["Keyword"] = option.Keyword;
            ViewData["Page"] = option.Page;
            return View();
        }

        public JsonResult Search()
        {
            var keyword = Request["keyword"];
            int pageSize = 0;
            if (Request["pageSize"] != null)
            {
				pageSize = Int32.Parse(Request["pageSize"]);
            }
            int page = 0;
            if (Request["page"] != null)
            {
				page = Int32.Parse(Request["page"]);
            }
            LibrarySearchResult result;
            Library.GetInstance().Search(new LibrarySearchOption
            {
                Keyword = keyword,
                PageSize = pageSize,
                Page = page
            }, out result);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public RedirectResult Conver()
        {
            var url = Request["url"];
            var bookInfo = Library.GetInstance().GetBookInfo(url);
            return Redirect(BaiduImageSearchEngine.GetInstance().SearchImageFor(bookInfo.CardInfo.Isbn));
        }

        public ActionResult Details()
        {
            var url = Request["url"];
            var bookInfo = Library.GetInstance().GetBookInfo(url);
            return View(bookInfo);
        }
#if DEBUG
        public void StartTest()
        {
			var expectedCount = Request["count"];
            if (expectedCount != null)
            {
                Library.GetInstance().StartTest(Int32.Parse(expectedCount));
            }
        }
#endif
    }
}

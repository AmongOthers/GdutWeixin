using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GdutWeixin.Models.Library;
using GdutWeixin.Models.Message;
using GdutWeixin.Utils;

namespace GdutWeixin.Controllers
{
    public class LibraryController : Controller
    {
        private Stopwatch mLibStopwatch = Stopwatch.StartNew();
        //
        // GET: /Library/

        public ActionResult Index()
        {
            return View();
        }

        public string Search()
        {
            WeixinResponse rsp = null;
            var keyword = Request["keyword"];
            var user = Request["user"];
            mLibStopwatch.Restart();
            object error;
            var result = Library.GetInstance().SearchBooksFor(user, keyword, out error);
            if (error == null)
            {
				mLibStopwatch.Stop();
                rsp = LibrarySearchResponse.Create(Request, user, keyword, result.Books, result.MoreUrl);
                ApplicationLogger.GetLogger().Info(String.Format("search library with {0} consume {1} ms",
                    keyword,
                    mLibStopwatch.ElapsedMilliseconds));
            }
            else
            {
                rsp = new TextResponse(user)
                {
                    Content = new WeixinResponse.StringXmlCDataSection(String.Format("查询出错： {0}", error))
                };
            }
            return rsp.ToString();
        }
    }
}

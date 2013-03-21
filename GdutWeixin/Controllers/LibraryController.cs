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
        //
        // GET: /Library/

        public ActionResult Index()
        {
            return View();
        }

        public string Search()
        {
            var keyword = Request["keyword"];
            var user = Request["user"];
            return Library.GetInstance().GetRspForSearch(Request, user, keyword);
        }
    }
}

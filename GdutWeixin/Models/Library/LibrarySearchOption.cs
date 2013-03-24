using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GdutWeixin.Models.Library
{
    public class LibrarySearchOption
    {
        public class DeptPlaceOption
        {
            public const string ALL = "ALL";
            //东风路校区
            public const string DONGFENGLU = "578";
        }

        public class LanguageOption
        {
            public const string All = "ALL";
            public const string Chinese = "1";
            public const string English = "2";
        }

        public string User { get; set; }
        public string Keyword { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
        public string DeptPlace { get; set; }
        public string Language { get; set; }

        public LibrarySearchOption()
        {
            DeptPlace = DeptPlaceOption.ALL;
            Language = LanguageOption.All;
            Page = 1;
            PageCount = -1;
        }
    }
}
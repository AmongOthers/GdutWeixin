using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GdutWeixin.Models.Library
{
    public class LibrarySearchResult
    {
        public string Keyword { get; set; }
        public string User { get; set; }
        public List<Book> Books { get; set; }
        public string MoreUrl { get; set; }
    }
}
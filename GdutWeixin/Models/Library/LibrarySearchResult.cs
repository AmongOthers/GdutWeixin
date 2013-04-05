using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GdutWeixin.Models.Library
{
    public class LibrarySearchResult
    {
        public List<Book> Books { get; set; }
        public int PageCount { get; set; }
        public int ResultCount { get; set; }
        public object Error { get; set; }
    }
}
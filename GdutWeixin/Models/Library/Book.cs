using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GdutWeixin.Models.Library
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string PublishYear { get; set; }
        public string Index { get; set; }
        public int Total { get; set; }
        public int Available { get; set; }
        public string Url { get; set; }
    }
}

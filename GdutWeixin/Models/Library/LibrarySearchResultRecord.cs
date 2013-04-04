using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GdutWeixin.Models.Library
{
    public class LibrarySearchResultRecord
    {
        public string Keyword { get; set; }
        public string User { get; set; }
        public List<Book> Books { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
		//为了避免搜索缓存的时候总是生成字符串，所以添加这个用于缓存的字段
        private string mCacheKeyword = null;
        public string CacheKeyword
        {
            get
            {
                return mCacheKeyword == null ? mCacheKeyword = GenCacheKeyword(Keyword, CurrentPage) : mCacheKeyword;
            }
        }

        public static string GenCacheKeyword(string keyword, int page)
        {
            return keyword + "[@" + page + "@]";
        }
    }
}
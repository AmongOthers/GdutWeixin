using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GdutWeixin.Models.Library;
using GdutWeixin.Utils;

namespace GdutWeixin.Models.Library
{
    public class LibraryCache
    {
        FixedSizedQueue<LibrarySearchResult> mQueue;

        public LibraryCache(int size)
        {
			mQueue = new FixedSizedQueue<LibrarySearchResult>(size);
        }

        public LibrarySearchResult Try2Hit(string keyword, int page)
        {
            var cacheKeyword = LibrarySearchResult.GenCacheKeyword(keyword, page);
            var result = (from cache in mQueue where cache.CacheKeyword == cacheKeyword select cache).FirstOrDefault();
            return result;
        }

        public void Push(LibrarySearchResult result)
        {
			if(Try2Hit(result.Keyword, result.CurrentPage) == null)
            {
				mQueue.Enqueue(result);
            }
        }
    }
}
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

        public int Count
        {
            get
            {
                return mQueue.Count;
            }
        }

        private LibrarySearchResult try2Hit(string cacheKeyword)
        {
            var result = (from cache in mQueue where cache.CacheKeyword == cacheKeyword select cache).FirstOrDefault();
            return result;
        }

        public LibrarySearchResult Try2Hit(string keyword, int page)
        {
            var cacheKeyword = LibrarySearchResult.GenCacheKeyword(keyword, page);
            var result = try2Hit(cacheKeyword);
            return result;
        }

        public void Push(LibrarySearchResult result)
        {
            var cacheKeyword = LibrarySearchResult.GenCacheKeyword(result.Keyword, result.CurrentPage);
			if(try2Hit(cacheKeyword) == null)
            {
				mQueue.Enqueue(result);
            }
        }
    }
}
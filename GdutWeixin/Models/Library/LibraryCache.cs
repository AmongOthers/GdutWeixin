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
        FixedSizedQueue<LibrarySearchResultRecord> mQueue;

        public LibraryCache(int size)
        {
			mQueue = new FixedSizedQueue<LibrarySearchResultRecord>(size);
        }

        public int Count
        {
            get
            {
                return mQueue.Count;
            }
        }

        private LibrarySearchResultRecord try2Hit(string cacheKeyword)
        {
            var result = (from cache in mQueue where cache.CacheKeyword == cacheKeyword select cache).FirstOrDefault();
            return result;
        }

        public LibrarySearchResultRecord Try2Hit(string keyword, int page)
        {
            var cacheKeyword = LibrarySearchResultRecord.GenCacheKeyword(keyword, page);
            var result = try2Hit(cacheKeyword);
            return result;
        }

        public void Push(LibrarySearchResultRecord result)
        {
            var cacheKeyword = LibrarySearchResultRecord.GenCacheKeyword(result.Keyword, result.CurrentPage);
			if(try2Hit(cacheKeyword) == null)
            {
				mQueue.Enqueue(result);
            }
        }
    }
}
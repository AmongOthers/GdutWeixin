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
        public FixedSizedQueue<LibrarySearchResult> mQueue = new FixedSizedQueue<LibrarySearchResult>(100);

        public LibrarySearchResult Try2HitByKeyword(string keyword)
        {
            var result = (from cache in mQueue where cache.Keyword == keyword select cache).FirstOrDefault();
            return result;
        }

        public void Push(LibrarySearchResult result)
        {
			if(Try2HitByKeyword(result.Keyword) == null)
            {
				mQueue.Enqueue(result);
            }
        }
    }
}
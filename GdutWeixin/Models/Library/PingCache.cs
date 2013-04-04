using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GdutWeixin.Models.Message;
using GdutWeixin.Utils;

namespace GdutWeixin.Models.Library
{
    public class PingCache
    {
        static PingCache sInstance;

        static PingCache GetInstance()
        {
            return sInstance ?? new PingCache();
        }

        ConcurrentDictionary<string, FixedSizedQueue<RequestPing>> mCache = new ConcurrentDictionary<string, FixedSizedQueue<RequestPing>>();

		public void Push(string user, WeixinRequest request)
        {
            if (!mCache.ContainsKey(user))
            {
                mCache[user] = new FixedSizedQueue<RequestPing>(4);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GdutWeixin.Utils
{
    public class UrlToAbsConverter
    {
        private HttpRequestBase mRequest;

        public UrlToAbsConverter(HttpRequestBase request)
        {
            mRequest = request;
        }

        public string Convert(string url)
        {
            return string.Format("{0}://{1}{2}{3}",
                mRequest.IsSecureConnection ? "https" : "http",
                mRequest.Url.Host,
#if DEBUG
                mRequest.Url.Host.StartsWith("localhost") ? ":" + mRequest.Url.Port.ToString() : "",
#else 
				"", 
#endif
                VirtualPathUtility.ToAbsolute(url));
        }
    }
}
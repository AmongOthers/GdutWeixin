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
            return string.Format("{0}://{1}{2}",
                mRequest.IsSecureConnection ? "https" : "http",
                mRequest.Url.Host,
				//不用解释Port，因为Url的Host不会因为重定向而变换，但是Port却会
                //mRequest.Url.Port == 80 ? "" : ":" + mRequest.Url.Port.ToString(),
                VirtualPathUtility.ToAbsolute(url));
        }
    }
}
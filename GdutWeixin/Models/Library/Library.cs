using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml.Serialization;
using GdutWeixin.Models.Library;
using GdutWeixin.Models.Message;
using GdutWeixin.Utils;

namespace GdutWeixin.Models.Library
{
    public class Library
    {
        private LibraryCache mCache = new LibraryCache();

        public static Library sInstance = null;
        public static Library GetInstance()
        {
            return sInstance == null ? (sInstance = new Library()) : sInstance;
        }

        public class LibrarySearchOption
        {
            public class DeptPlaceOption
            {
                public const string ALL = "ALL";
                //东风路校区
                public const string DONGFENGLU = "578";
            }

            public class LanguageOption
            {
                public const string All = "ALL";
                public const string Chinese = "1";
                public const string English = "2";
            }


            //馆藏地点
            public string DeptPlace { get; set; }
            public string Language { get; set; }

            public LibrarySearchOption()
            {
                DeptPlace = DeptPlaceOption.ALL;
                Language = LanguageOption.Chinese;
            }
        }

        static readonly LibrarySearchOption DEFAULT = new LibrarySearchOption();

        public string GetRspForSearch(HttpRequestBase request, string user, string keyword)
        {
            WeixinResponse rsp = null;
            object error;
            var libStopwatch = Stopwatch.StartNew();
            var result = Library.GetInstance().SearchBooksFor(user, keyword, out error);
            if (error == null)
            {
                libStopwatch.Stop();
                rsp = LibrarySearchResponse.Create(request, user, keyword, result.Books, result.MoreUrl);
                ApplicationLogger.GetLogger().Info(String.Format("search library with {0} consume {1} ms",
                    keyword,
                    libStopwatch.ElapsedMilliseconds));
            }
            else
            {
                rsp = new TextResponse(user)
                {
                    Content = new WeixinResponse.StringXmlCDataSection(String.Format("查询出错： {0}", error))
                };
            }
            return rsp.ToString();
        }

        public LibrarySearchResult SearchBooksFor(string user, string keyword, out object error, LibrarySearchOption option = null)
        {
            error = null;
            LibrarySearchResult cached;
            if ((cached = mCache.Try2HitByKeyword(keyword)) != null)
            {
                return cached;
            }
            option = option == null ? DEFAULT : option;
            var url = getSearchUrl(keyword, option);
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;
                var books = LibraryTableResult.Parse((request.GetResponse() as HttpWebResponse).GetResponseStream());
                cached = new LibrarySearchResult 
                {
					Keyword = keyword,
					User = user,
					Books = books,
					MoreUrl = url
                };
                mCache.Push(cached);
                return cached;
            }
            catch (WebException e)
            {
                error = e;
                return null;
            }
        }

        private string getSearchUrl(string keyword, LibrarySearchOption option)
        {
            var encodedKeyword = HttpUtility.UrlEncode(keyword, System.Text.Encoding.GetEncoding("GB2312"));
            var url = String.Format("http://222.200.98.171:81/searchresult.aspx?dt=0&dp=8&sf=M_PUB_YEAR&ob=DESC&sm=table&title_f={0}&dept={1}&cl={2}&timestamp={3}",
                encodedKeyword, option.DeptPlace, option.Language, DateTimeHelper.Timestamp());
            return url;
        }
    }
}
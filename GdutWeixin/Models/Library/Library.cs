using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using GdutWeixin.Models.Library;
using GdutWeixin.Models.Message;
using GdutWeixin.Utils;

namespace GdutWeixin.Models.Library
{
    public class Library
    {
        private LibraryCache mResultCache = new LibraryCache(10 * 1000);

        public static Library sInstance = null;
        public static Library GetInstance()
        {
            return sInstance == null ? (sInstance = new Library()) : sInstance;
        }

        static readonly LibrarySearchOption DEFAULT = new LibrarySearchOption();

        public string GetRspForSearch(HttpSessionStateBase session, HttpRequestBase request, LibrarySearchOption option)
        {
            WeixinResponse rsp = null;
            object error;
            var libStopwatch = Stopwatch.StartNew();
            var result = Library.GetInstance().SearchBooksFor(session, option, out error);
            if (error == null)
            {
                libStopwatch.Stop();
                rsp = LibrarySearchResponse.Create(request, result);
                ApplicationLogger.GetLogger().Info(String.Format("(" + session.SessionID  + ")"
                    + "search library with " + option.Keyword + " consume " + libStopwatch.ElapsedMilliseconds));
            }
            else
            {
                rsp = new TextResponse(option.User, String.Format("查询出错： {0}", error));
            }
            return rsp.ToString();
        }

        static readonly Regex TBODY_REGEX = new Regex("<tbody>[\\s\\S]+</tbody>");
        static readonly Regex PAGE_COUNT_REGEX = new Regex("<span id=\"ctl00_ContentPlaceHolder1_gplblfl1\">([0-9]+)</span>");
        static readonly Regex RESULT_COUNT_REGEX = 
            new Regex("<span id=\"ctl00_ContentPlaceHolder1_countlbl\" style=\"color:Red;\">([0-9]+)</span>");
		//在浏览器(chrome, ie10)中查看是上面的页面，但是程序访问却是得到下面的页面
        static readonly Regex RESULT_COUNT_REGEX_1 = 
            new Regex("<span id=\"ctl00_ContentPlaceHolder1_countlbl\"><font color=\"Red\">([0-9]+)</font></span>");

        public LibrarySearchResultRecord SearchBooksFor(HttpSessionStateBase session, LibrarySearchOption option, out object error)
        {
            error = null;
            var user = option.User;
			var keyword = option.Keyword;
			var page = option.Page;
            LibrarySearchResultRecord cached;
            if ((cached = mResultCache.Try2Hit(keyword, page)) != null)
            {
                ApplicationLogger.GetLogger().Info("(" + session.SessionID + ")" + keyword + " " + page + " hited");
                option.PageCount = cached.PageCount;
                return cached;
            }
            option = option == null ? DEFAULT : option;
            LibrarySearchResult result;
            if (Search(option, out result))
            {
                cached = new LibrarySearchResultRecord
                            {
                                Keyword = keyword,
                                User = user,
                                Books = result.Books,
                                PageCount = result.PageCount,
								CurrentPage = option.Page
                            };
				//记录查询结果的总页数
                option.PageCount = result.PageCount;
                mResultCache.Push(cached);
                ApplicationLogger.GetLogger().Info("(" + session.SessionID + ")" +
                    "push cache: " + keyword + " " + page + " current cache count: " + mResultCache.Count);
                return cached;
            }
            else
            {
                return null;
            }
        }

#if DEBUG
		int expectedCount = 60;
        int pageIndex = 0;

        public void StartTest(int expectedCount)
        {
            this.expectedCount = expectedCount;
            pageIndex = 0;
        }

        LibrarySearchResult getLocalResult()
        {
            using (var reader = new StreamReader(System.IO.File.OpenRead(
    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "search.txt"))))
            {
                var serializer = new JavaScriptSerializer();
				return serializer.Deserialize<LibrarySearchResult>(reader.ReadToEnd());
            }
        }
#endif

        public bool Search(LibrarySearchOption option, out LibrarySearchResult result)
        {
			/*
#if DEBUG
			result = getLocalResult();
			result.ResultCount = expectedCount;
			if (pageIndex < expectedCount / option.PageSize)
			{
				pageIndex++;
				result.Books = result.Books.GetRange(0, option.PageSize);
			}
			else
			{
				result.Books = result.Books.GetRange(0, expectedCount - pageIndex * option.PageSize);
			}
			return true;
#endif
			*/
			List<Book> books = null;
            int pageCount = 0;
            int resultCount = 0;
			var url = getSearchUrl(option);
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;
                Stream stream = (request.GetResponse() as HttpWebResponse).GetResponseStream();
                Parse(stream, out books, out pageCount, out resultCount);
                result = new LibrarySearchResult
                {
					Books = books,
					PageCount = pageCount,
					ResultCount = resultCount,
					Error = null
                };
                return true;
            }
			catch(WebException e)
            {
                result = new LibrarySearchResult
                {
					Error = e
                };
                return false;
            }
        }

        public void Parse(Stream stream, out List<Book> books, out int pageCount, out int resultCount)
        {
            using (var reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                Match match;
                if (!String.IsNullOrEmpty(content) &&
                    (match = TBODY_REGEX.Match(content)).Success)
                {
                    var tbody = match.Groups[0].ToString();
                    books = LibraryTableResult.Parse(tbody);
                }
                else
                {
                    books = null;
                }
                match = PAGE_COUNT_REGEX.Match(content);
                if (match.Success)
                {
                    var pageCountStr = match.Groups[1].Value;
                    pageCount = Int16.Parse(pageCountStr);
                }
                else
                {
                    pageCount = 0;
                }
                match = ((match = RESULT_COUNT_REGEX.Match(content)).Success) ? match :
                    RESULT_COUNT_REGEX_1.Match(content);
                if (match.Success)
                {
                    var resultCountStr = match.Groups[1].Value;
                    resultCount = Int16.Parse(resultCountStr);
                }
                else
                {
                    resultCount = 0;
                }
            }
        }

        public BookInfo GetBookInfoExample()
        {
            using (var reader = new StreamReader(File.OpenRead(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bookinfo_exmaple.txt"))))
            {
                var serializer = new JavaScriptSerializer();
                return serializer.Deserialize<BookInfo>(reader.ReadToEnd());
            }
        }

        public BookInfo GetBookInfo(string relatvieUrl)
        {
            var url = "http://222.200.98.171:81/" + relatvieUrl;
            var request = WebRequest.Create(url) as HttpWebRequest;
            var bookInfo = BookInfo.Build(request.GetResponse().GetResponseStream());
            return bookInfo;
        }

        private string getSearchUrl(LibrarySearchOption option)
        {
            var encodedKeyword = HttpUtility.UrlEncode(option.Keyword, System.Text.Encoding.GetEncoding("GB2312"));
            var url = String.Format("http://222.200.98.171:81/searchresult.aspx?dt=0&sf=M_PUB_YEAR&ob=DESC&sm=table&anywords={0}&dept={1}&cl={2}&page={3}&dp={4}",
                encodedKeyword, option.DeptPlace, option.Language, option.Page, option.PageSize);
            return url;
        }
    }
}
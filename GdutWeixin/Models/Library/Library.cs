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
using System.Web.Script.Serialization;
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

            public string DeptPlace { get; set; }
            public string Language { get; set; }
            public int Page { get; set; }

            public LibrarySearchOption()
            {
                DeptPlace = DeptPlaceOption.ALL;
                Language = LanguageOption.Chinese;
                Page = 1;
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
                rsp = LibrarySearchResponse.Create(request, result);
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

        static readonly Regex TBODY_REGEX = new Regex("<tbody>[\\s\\S]+</tbody>");
        static readonly Regex PAGE_COUNT_REGEX = new Regex("<span id=\"ctl00_ContentPlaceHolder1_gplblfl1\">([0-9]+)</span>");

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
                Stream stream = (request.GetResponse() as HttpWebResponse).GetResponseStream();
                List<Book> books = null;
                int pageCount = 0;
                Parse(stream, out books, out pageCount);
                cached = new LibrarySearchResult
                            {
                                Keyword = keyword,
                                User = user,
                                Books = books,
                                MoreUrl = url,
                                PageCount = pageCount,
								CurrentPage = option.Page
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

        public void Parse(Stream stream, out List<Book> books, out int pageCount)
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

        private string getSearchUrl(string keyword, LibrarySearchOption option)
        {
            var encodedKeyword = HttpUtility.UrlEncode(keyword, System.Text.Encoding.GetEncoding("GB2312"));
            var url = String.Format("http://222.200.98.171:81/searchresult.aspx?dt=0&dp=8&sf=M_PUB_YEAR&ob=DESC&sm=table&title_f={0}&dept={1}&cl={2}&page={3}",
                encodedKeyword, option.DeptPlace, option.Language, option.Page);
            return url;
        }
    }
}
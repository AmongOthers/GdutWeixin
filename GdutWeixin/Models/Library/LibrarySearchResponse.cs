using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Serialization;
using GdutWeixin.Models.Message;
using GdutWeixin.Utils;

namespace GdutWeixin.Models.Library
{
	[XmlRoot("xml")]
    public class LibrarySearchResponse : NewsResponse
    {
        public LibrarySearchResponse()
        {
        }

        public static LibrarySearchResponse Create(HttpRequestBase request, LibrarySearchResult result)
        {
            return new LibrarySearchResponse(request, result);
        }

		const string CMD = "命令： \".\" + r (超时重试); \".\" + n(下一页); \".\" + p(上一页); \".\" + 数字(快速翻页); @ + 信息(留言, 欢迎拍砖吐槽)";

        private LibrarySearchResponse(HttpRequestBase request, LibrarySearchResult result) 
            : base(result.User)
        {
			var keyword = result.Keyword;
			var moreUrl = result.MoreUrl;
			var books = result.Books;
            var converter = new UrlToAbsConverter(request);
            this.Articles = new List<Article>()
                {
					new Article
                    {
						Title = new StringXmlCDataSection(String.Format("搜索 {0} 的结果: {1}/{2}页", keyword, result.CurrentPage, result.PageCount)),
						Description = new StringXmlCDataSection(keyword),
						PicUrl = new StringXmlCDataSection(converter.Convert("/Content/images/lib.jpg")),
						Url = new StringXmlCDataSection(converter.Convert("/Home/About")),
                    }
                };
            if (books != null && books.Count > 0)
            {
                var bookArticles = from book in books
                                   select new Article
                                   {
                                       Title = new StringXmlCDataSection(String.Format("[{0} 馆藏：{1}/{2}] {3} ({4})",
                                           book.Index, book.Available, book.Total, book.Title, book.Author)),
                                       Description = new StringXmlCDataSection(keyword),
									   PicUrl = book.Available > 0 ?
										   new StringXmlCDataSection(converter.Convert("/Content/Images/green_circle.png")) :
										   new StringXmlCDataSection(converter.Convert("/Content/Images/red_circle.png")),
                                       Url = new StringXmlCDataSection(getDetailUrl(converter, book.Url))
                                   };
                this.Articles.AddRange(bookArticles);
                this.Articles.Add(new Article
                {
					Title = new StringXmlCDataSection(CMD),
					PicUrl = new StringXmlCDataSection(converter.Convert("/Content/Images/frog.jpg")),
					Url = new StringXmlCDataSection(converter.Convert("/Home/About"))
                });
            }
            else
            {
                this.Articles.Add(new Article
                {
					Title = new StringXmlCDataSection("没有匹配项"),
					PicUrl = new StringXmlCDataSection(converter.Convert("/Content/Images/frog.jpg")),
					Url = new StringXmlCDataSection(converter.Convert("/Home/About"))
                });
            }
        }

        private static string getDetailUrl(UrlToAbsConverter converter, string postfix)
        {
            return converter.Convert("/Library/Details?url=" + HttpUtility.UrlEncode(postfix));
        }
    }
}
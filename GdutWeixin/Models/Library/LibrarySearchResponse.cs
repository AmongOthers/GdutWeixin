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

        public static LibrarySearchResponse Create(HttpRequestBase request, string reqFromUserName, string keyword, List<Book> books, string moreUrl)
        {
            return new LibrarySearchResponse(request, reqFromUserName, keyword, books, moreUrl);
        }

        private LibrarySearchResponse(HttpRequestBase request, string reqFromUserName, string keyword, List<Book> books, string moreUrl)
            : base(reqFromUserName)
        {

            var converter = new UrlToAbsConverter(request);
            this.Articles = new List<Article>()
                {
					new Article
                    {
						Title = new StringXmlCDataSection(String.Format("搜索 {0} 的结果", keyword)),
						Description = new StringXmlCDataSection(keyword),
						PicUrl = new StringXmlCDataSection(converter.Convert("/Content/images/lib.jpg")),
						Url = new StringXmlCDataSection(moreUrl),
                    }
                };
            if (books != null && books.Count > 0)
            {
                var bookArticles = from book in books
                                   select new Article
                                   {
                                       Title = new StringXmlCDataSection(String.Format("[{0} (1)/(2)] {3} ({4})",
                                           book.Index, book.Available, book.Total, book.Title, book.Author)),
                                       Description = new StringXmlCDataSection(keyword),
                                       PicUrl = new StringXmlCDataSection(converter.Convert("/Content/Images/question_mark.jpg")),
                                       Url = new StringXmlCDataSection(book.Url)
                                   };
                this.Articles.AddRange(bookArticles);
                this.Articles.Add(new Article
                {
					Title = new StringXmlCDataSection("查看更多"),
					PicUrl = new StringXmlCDataSection(converter.Convert("/Content/Images/arrow.png")),
					Url = new StringXmlCDataSection(moreUrl)
                });
            }
            else
            {
                this.Articles.Add(new Article
                {
					Title = new StringXmlCDataSection("没有匹配项")
                });
            }
        }
    }
}
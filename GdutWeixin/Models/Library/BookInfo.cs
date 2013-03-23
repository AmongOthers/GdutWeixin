using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace GdutWeixin.Models.Library
{
    public class BookInfo
    {
        static BookInfoBuilder sBuilder = new BookInfoBuilder();

        public static BookInfo Build(Stream content)
        {
            return sBuilder.Build(content);
        }

        public BookCardInfo CardInfo
        {
            get;
            set;
        }

		/*
        public string Content { get; set; }

        public List<string> Catalog { get; set; }
		*/

        public List<DeptInfo> DeptInfos
        {
            get;
            set;
        }

        class BookInfoBuilder
        {
            static readonly Regex BookCardInfoRegex = 
                new Regex("<span id=\"ctl00_ContentPlaceHolder1_bookcardinfolbl\">.*?</span>");
            static readonly Regex ContentAndCatalogRegex = 
                new Regex("<tbody>[\\s\\S]+</tbody>");
            static readonly Regex DeptInfoRegex =
                new Regex("<tbody>[\\s\\S]+</tbody>");

            BookInfo mBookInfo;
            Stream mContentStream;

            public BookInfo Build(Stream content)
            {
				//reset
                mContentStream = content;
                mBookInfo = new BookInfo();

                using (var reader = new StreamReader(mContentStream))
                {
                    var bookInfoStr = reader.ReadToEnd();
                    var match = BookCardInfoRegex.Match(bookInfoStr);
                    if (match.Success)
                    {
                        var bookCardInfoStr = match.Groups[0].Value;
                        mBookInfo.CardInfo = BookCardInfo.Build(bookCardInfoStr);
                    }
                    var deptStr = bookInfoStr.Substring(bookInfoStr.IndexOf("藏书情况"));
                    match = DeptInfoRegex.Match(bookInfoStr);
                    if (match.Success)
                    {
                        var deptInfoStr = match.Groups[0].Value;
                        mBookInfo.DeptInfos = DeptInfo.Build(deptInfoStr);
                    }
					return mBookInfo;
                }
            }

			//内容和章节属于js脚本插入内容
			/*
            enum ContentAndCatalogState
            {
				Content,
				ContentText,
				Catalog,
				CatalogText
            }

            private void buildContentAndCatalog(string contentAndCatalogStr)
            {
                ContentAndCatalogState state = ContentAndCatalogState.Content;
                using (var reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(contentAndCatalogStr))))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
							case XmlNodeType.Element:
                                if (reader.Name == "td")
                                {
                                    switch (state)
                                    {
                                        case ContentAndCatalogState.Content:
                                            state = ContentAndCatalogState.ContentText;
                                            break;
                                        case ContentAndCatalogState.ContentText:
                                            mBookInfo.Content = reader.ReadString();
                                            state = ContentAndCatalogState.Catalog;
                                            break;
                                        case ContentAndCatalogState.Catalog:
                                            state = ContentAndCatalogState.CatalogText;
                                            break;
                                        case ContentAndCatalogState.CatalogText:
                                            mBookInfo.Catalog.Add(reader.ReadString());
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
			*/
        }
    }
}
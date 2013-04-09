using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using GdutWeixin.Utils;

namespace GdutWeixin.Models.Library
{
    public class BookCardInfo
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Content { get; set; }
        public string Isbn { get; set; }

        static BookCardInfoBuilder sBuilder = new BookCardInfoBuilder();

        public static BookCardInfo Build(string content)
        {
            return sBuilder.Build(content);
        }

        class BookCardInfoBuilder
        {
            BookCardInfo mBookCardInfo;
            BookCardInfoBuilderState mState;
            XmlReader mReader;
            List<string> mContentAndIsbnTexts = new List<string>();

            enum BookCardInfoBuilderState
            {
                TitleAuthor,
                Other,
                Publisher,
                ContentAndIsbn_Br,
                ContentAndIsbn_0,
                ContentAndIsbn_1,
            }

			static readonly Regex HREF_REGEX = new Regex(" href=\".*?\"");

            public BookCardInfo Build(string content)
            {
				//reset
                mState = BookCardInfoBuilderState.TitleAuthor;
                mBookCardInfo = new BookCardInfo();
                mContentAndIsbnTexts.Clear();

                content = HREF_REGEX.Replace(content, "");
                content = HtmlEntityCorrect.Encode(content);
                using (mReader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(content))))
                {
                    while (mReader.Read())
                    {
                        switch (mReader.NodeType)
                        {
							case XmlNodeType.Text:
                                this.onText(HtmlEntityCorrect.Decode(mReader.Value));
                                break;
                            case XmlNodeType.Element:
                                this.onElement(HtmlEntityCorrect.Decode(mReader.Name));
                                break;
                        }
                    }
                }
                return mBookCardInfo;
            }

            private void getTitleAndAuthor(string text)
            {
                text = text.Trim();
				//这里的点和英文输入法下的.是不同的
                var dotIndex = text.LastIndexOf("．");
				text = text.Substring(0, dotIndex);
				//这里的斜符和英文输入法下的/是不同的
                var slashIndex = text.LastIndexOf("／");
                mBookCardInfo.Title = text.Substring(0, slashIndex);
                mBookCardInfo.Author = text.Substring(slashIndex + 1);
            }

			static readonly Regex ISBN_REGEX = new Regex("ISBN(.*)：");
            private void getIsbn(string text)
            {
                var match = ISBN_REGEX.Match(text);
                if (match.Success)
                {
                    mBookCardInfo.Isbn = match.Groups[1].Value.Replace("-", "");
                }
            }

            private void getContentAndIsbn()
            {
                var isbnIndex = mContentAndIsbnTexts.FindIndex((item) => item.Contains("ISBN"));
                if (isbnIndex > 0)
                {
					var isbnText = mContentAndIsbnTexts[isbnIndex];
					getIsbn(isbnText);
                    mBookCardInfo.Content = mContentAndIsbnTexts[isbnIndex - 1].Trim().Trim(')').Trim('）');
                }
            }

            private void pushText(string text)
            {
                text = text.Trim();
				if(!String.IsNullOrEmpty(text) && text != "\"\"")
                {
					mContentAndIsbnTexts.Add(text);
                }
            }

            private void onText(string text)
            {
                switch (mState)
                {
					case BookCardInfoBuilderState.TitleAuthor:
						getTitleAndAuthor(text);
                        mState = BookCardInfoBuilderState.Publisher;
                        break;
                    case BookCardInfoBuilderState.ContentAndIsbn_0:
                        pushText(text);
                        break;
                    case BookCardInfoBuilderState.ContentAndIsbn_1:
                        pushText(text);
                        break;
                }
            }

            private void onElement(string name)
            {
                if (name == "a")
                {
                    onElementA();
                }
                else if (name == "br")
                {
                    onElementBr();
                }
            }

            private void onElementA()
            {
                switch (mState)
                {
                    case BookCardInfoBuilderState.Publisher:
                        mBookCardInfo.Publisher = HtmlEntityCorrect.Decode(mReader.ReadString());
                        mState = BookCardInfoBuilderState.ContentAndIsbn_Br;
                        break;
					//遇到第二个a元素则内容和ISBN结束
                    case BookCardInfoBuilderState.ContentAndIsbn_0:
                        mState = BookCardInfoBuilderState.ContentAndIsbn_1;
                        break;
                    case BookCardInfoBuilderState.ContentAndIsbn_1:
                        getContentAndIsbn();
                        mState = BookCardInfoBuilderState.Other;
                        break;
                }
            }

            private void onElementBr()
            {
                switch (mState)
                {
					case BookCardInfoBuilderState.ContentAndIsbn_Br:
                        mState = BookCardInfoBuilderState.ContentAndIsbn_0;
                        break;
                }
            }
        }
    }
}
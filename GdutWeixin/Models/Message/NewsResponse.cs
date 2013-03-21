using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GdutWeixin.Models.Message
{
	[XmlRoot("xml")]
    public class NewsResponse : WeixinResponse
    {
        public StringXmlCDataSection Content { get; set; }
        public int ArticleCount
        {
            get
            {
                return this.Articles.Count;
            }
            set
            {
            }
        }
		[XmlArrayItem(ElementName = "item")]
        public List<Article> Articles { get; set; }

        public NewsResponse()
        {
        }

        public NewsResponse(string toUserName)
			: base(toUserName)
        {
			Content = new StringXmlCDataSection("");
            MsgType = new StringXmlCDataSection("news");
        }

        public class Article
        {
            public StringXmlCDataSection Title { get; set; }
            public StringXmlCDataSection Description { get; set; }
            public StringXmlCDataSection PicUrl { get; set; } 
            public StringXmlCDataSection Url { get; set; }
        }

        public static NewsResponse GetTestMsg(string reqFromUserName)
        {
            return new NewsResponse(reqFromUserName)
            {
                Content = new StringXmlCDataSection("Test"),
                Articles = new List<Article>() {
                    new Article 
                    {
						Title = new StringXmlCDataSection("国旗飘飘"),
						Description = new StringXmlCDataSection("这是什么意思"),
						PicUrl = new StringXmlCDataSection("http://221.179.6.74/Content/Images/big.jpg"),
						Url = new StringXmlCDataSection("http://221.179.6.74/Account")
                    },
                    new Article 
                    {
						Title = new StringXmlCDataSection("老鹰"),
						Description = new StringXmlCDataSection("这是什么意思"),
						PicUrl = new StringXmlCDataSection("http://221.179.6.74/Content/Images/s0.jpg"),
						Url = new StringXmlCDataSection("http://221.179.6.74/Account")
                    },
                    new Article 
                    {
						Title = new StringXmlCDataSection("士兵"),
						Description = new StringXmlCDataSection("这是什么意思"),
						PicUrl = new StringXmlCDataSection("http://221.179.6.74/Content/Images/s1.jpg"),
						Url = new StringXmlCDataSection("http://221.179.6.74/Account")
                    }
				}
            };
        }
    }
}
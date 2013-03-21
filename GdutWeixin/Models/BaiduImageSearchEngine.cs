using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace GdutWeixin.Models
{
    public class BaiduImageSearchEngine
    {
        const string URL =
            "http://image.baidu.com/i?tn=baiduimagejson&ct=201326592&cl=2&lm=-1&st=-1&fm=result&fr=&sf=1&fmq=1363794593079_R&pv=&ic=0&nc=1&z=&se=1&showtab=0&fb=0&width=&height=&face=0&istype=2&ie=utf-8&word={0}&rn=1";
        private static BaiduImageSearchEngine sInstance;
        public static BaiduImageSearchEngine GetInstance()
        {
            return sInstance == null ? (sInstance = new BaiduImageSearchEngine()) : sInstance; 
        }
 
        public string SearchImageFor(string keyword)
        {
            var request = WebRequest.Create(String.Format(URL, keyword)) as HttpWebRequest;
            using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                var content = reader.ReadToEnd();
                var serializer = new JavaScriptSerializer();
                var result = serializer.Deserialize<ImageSerachResult>(content);
                if (result.data.Count > 0)
                {
                    return result.data[0].thumbURL;
                }
            }
            return null;
        }

        public class ImageSerachResult
        {
            public List<Item> data { get; set; }

            public class Item
            {
                public string thumbURL { get; set; }
            }
        }
    }
}
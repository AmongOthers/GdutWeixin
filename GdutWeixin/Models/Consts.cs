using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GdutWeixin.Models
{
    public class Consts
    {
        public const string ABOUT = "乐搜，致力成为广工学子搜索快乐，分享快乐的平台。目前测试版推出图书馆图书搜索功能，一起分享读书的快乐。后续功能正在开发中，欢迎留言建议。";
        public const string CONTACT = "新浪微博: @快乐的小文酱";
        public const string FUNC = "图书馆图书搜索，发送关键字进行查询。由于微信超过5秒即超时没有响应，此时请发送 \".\" + r 重试。返回条目中绿灯表示有可借，红灯表示无可借。点击条目可以进入该书的详细信息页面。";
        public static readonly string WELCOME = String.Join("\n\n", new string[] { ABOUT, CONTACT, FUNC });
        public const string GOODBYE = "下马饮君酒，问君何所之？\n君言不得意，归卧南山陲。\n但去莫复问，白云无尽时。";
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace GdutWeixin.Models.Library
{
    public class UserCommand
    {
        static UserCommand sInstance;

        public static UserCommand GetInstance()
        {
            return sInstance == null ? sInstance = new UserCommand() : sInstance;
        }

        ConcurrentDictionary<string, LibrarySearchOption> mCache = new ConcurrentDictionary<string, LibrarySearchOption>();
		/*
         * .r 重试上次操作
         * .n 下一页
         * .p 上一页
		 * .num 快速翻页
         */
        static readonly Regex QUICK_PAGE_REGEX = new Regex("\\.([0-9]+)");
        bool isQuickPage(string msg, out int page)
        {
            page = -1;
            var match = QUICK_PAGE_REGEX.Match(msg);
            if (match.Success)
            {
                page = Int16.Parse(match.Groups[1].Value);
                return true;
            }
            return false;
        }

        public bool OnMessage(string user, string msg, out LibrarySearchOption option, out string errMsg)
        {
            errMsg = null;
			option = null;
            int quickPage;
            bool isQuickPageMsg = isQuickPage(msg, out quickPage);
            mCache.TryGetValue(user, out option);
            if (option == null)
            {
                if (msg == ".r" || msg == ".n" || msg == ".p" || isQuickPageMsg)
                {
                    errMsg = "没有相应的记录，请重新发送关键字搜索";
                    return false;
                }
                else
                {
                    openNewSearch(user, msg, out option);
                    return true;
                }
            }
            else
            {
                int targePage = -1;
                if (msg == ".r")
                {
                    return true;
                }
                else if (msg == ".n")
                {
                    targePage = option.Page + 1;
                }
                else if (msg == ".p")
                {
                    targePage = option.Page - 1;
                }
                else if (isQuickPageMsg)
                {
                    targePage = quickPage;
                }
                else
                {
                    openNewSearch(user, msg, out option);
                    return true;
                }
                return turnPage(option, targePage, out errMsg);
            }
        }

        private void openNewSearch(string user, string msg, out LibrarySearchOption option)
        {
            option = new LibrarySearchOption
            {
                User = user,
                Keyword = msg,
                Page = 1
            };
            mCache[user] = option;
        }

        bool turnPage(LibrarySearchOption option, int targetPage, out string errMsg)
        {
            errMsg = null;
            if (option.PageCount == -1)
            {
                errMsg = "上次查询结果正在构建中，请发送.r重试";
                return false;
            }
            else
            {
                if (targetPage < 1)
                {
                    errMsg = "前面没有结果了";
                    return false;
                }
                else if (targetPage > option.PageCount)
                {
                    errMsg = "后面没有结果了";
                    return false;
                }
                else
                {
                    option.Page = targetPage;
                    return true;
                }
            }
        }
    }
}
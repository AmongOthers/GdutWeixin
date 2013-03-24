using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace GdutWeixin.Utils
{
    public class HtmlEntityCorrect
    {
        static readonly Dictionary<string, string> INLEGAL = new Dictionary<string,string>
        {
			{ "&amp;", "&#38;" },
			{ "&lt;", "&#60;" },
			{ "&gt;", "&#62;" },
			{ "&quot;", "&#34;" },
			{ "&nbsp;", "&#160;" },
			{ "&copy;", "&#169;" },
			{ "&reg;", "&#174;" }
        };

        static readonly Dictionary<string, string> ORIGIN = new Dictionary<string,string>
        {
			{ "&#38;", "&" },
			{ "&#60;", "<" },
			{ "&#62;", ">" },
			{ "&#34;", "\"" },
			{ "&#160;", " " },
			{ "&#169;", "©" },
			{ "&#174;", "®" }
        };

        public static string Encode(string input)
        {
            foreach (var item in INLEGAL.Keys)
            {
                input = input.Replace(item, INLEGAL[item]);
            }
            input = Regex.Replace(input, "&([^#])", "&#38;$1");
            input = Regex.Replace(input, "&$", "&#38;");
            return input;
        }

        public static string Decode(string input)
        {
            foreach (var item in ORIGIN.Keys)
            {
                input = input.Replace(item, ORIGIN[item]);
            }
            return input;
        }
    }
}
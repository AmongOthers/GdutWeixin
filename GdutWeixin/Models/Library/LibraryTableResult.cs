using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace GdutWeixin.Models.Library
{
	[XmlRoot("tbody")]
    public class LibraryTableResult
    {
		static readonly Regex TBODY_REGEX = new Regex("<tbody>[\\s\\S]+</tbody>");

        public static List<Book> Parse(Stream html)
        {
            string content;
            using (var reader = new StreamReader(html))
            {
                content = reader.ReadToEnd();
            }
            Match match;
            if (!String.IsNullOrEmpty(content) &&
                (match = TBODY_REGEX.Match(content)).Success)
            {
                var tbody = match.Groups[0].ToString();
                return parseTbody(tbody);
            }
            else
            {
                return null;
            }
        }

        private static List<Book> parseTbody(string tbody)
        {
            List<Book> books = new List<Book>();
            Book book = null;
            using (var reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(tbody))))
            {
                string currentElementName = null;
                int tdCount = 0;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            currentElementName = reader.Name;
                            if (reader.Name == "tr")
                            {
                                book = new Book();
                                tdCount = 0;
                            }
                            else if (reader.Name == "td")
                            {
                                tdCount++;
                            }
                            else if (reader.Name == "a")
                            {
                                book.Url = getDetailUrl(reader.GetAttribute("href"));
                            }
                            break;
                        case XmlNodeType.Text:
                            if (currentElementName == "a")
                            {
                                book.Title = reader.Value;
                            }
                            else if (currentElementName == "td")
                            {
                                if (tdCount == 3)
                                {
                                    book.Author = reader.Value;
                                }
                                else if (tdCount == 4)
                                {
                                    book.Publisher = reader.Value;
                                }
                                else if (tdCount == 5)
                                {
                                    book.PublishYear = reader.Value;
                                }
                                else if (tdCount == 6)
                                {
                                    book.Index = reader.Value;
                                }
                                else if (tdCount == 7)
                                {
                                    book.Total = Int16.Parse(reader.Value);
                                }
                                else if (tdCount == 8)
                                {
                                    book.Available = Int16.Parse(reader.Value);
                                }
                            }
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "tr")
                            {
                                books.Add(book);
                            }
                            break;
                    }
                }
            }
            return books;
        }

        private static string getDetailUrl(string postfix)
        {
            return "http://222.200.98.171:81/" + postfix;
        }
   }
}
using System;
using System.IO;
using GdutWeixin.Models.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GdutWeixin.Tests
{
    [TestClass]
    public class BookInfoTests
    {
        [TestMethod]
        public void Parse()
        {
            Stream stream;
            BookInfo bookInfo;
            DeptInfo deptInfo;

            stream = File.OpenRead("details.htm");
            bookInfo = BookInfo.Build(stream);
            Assert.IsNotNull(bookInfo);
            Assert.AreEqual("Java Web程序设计教程[专著]＝Java Web development", bookInfo.CardInfo.Title);
            Assert.AreEqual("范立锋, 林果园编著", bookInfo.CardInfo.Author);
            Assert.AreEqual("人民邮电出版社", bookInfo.CardInfo.Publisher);
            Assert.AreEqual("9787115219749", bookInfo.CardInfo.Isbn);
            Assert.AreEqual(4, bookInfo.DeptInfos.Count);
            deptInfo = bookInfo.DeptInfos[0];
            Assert.AreEqual("东风路自然科学书库", deptInfo.DeptPlace);
            Assert.AreEqual("TP312JA/F24-2", deptInfo.Index);
            Assert.AreEqual("A2564937", deptInfo.Register);
            Assert.AreEqual("", deptInfo.Volume);
            Assert.AreEqual("", deptInfo.Year);
            Assert.AreEqual("可供出借", deptInfo.Status);
            Assert.AreEqual("中文图书", deptInfo.Type);
            Assert.IsTrue(bookInfo.CardInfo.Content.StartsWith("本书介绍使用"));
            Assert.IsTrue(bookInfo.CardInfo.Content.EndsWith("整合应用方式。"));
			
            stream = File.OpenRead("details1.htm");
            bookInfo = BookInfo.Build(stream);
            Assert.IsNotNull(bookInfo);
            Assert.AreEqual("Java语言程序设计[专著]", bookInfo.CardInfo.Title);
            Assert.AreEqual("于红...等编著", bookInfo.CardInfo.Author);
            Assert.AreEqual("机械工业出版社", bookInfo.CardInfo.Publisher);
            Assert.AreEqual("9787111365464", bookInfo.CardInfo.Isbn);
            Assert.AreEqual(4, bookInfo.DeptInfos.Count);
            deptInfo = bookInfo.DeptInfos[1];
            Assert.AreEqual("大学城自然科学书库", deptInfo.DeptPlace);
            Assert.AreEqual("TP312JA/Y740.2", deptInfo.Index);
            Assert.AreEqual("A3068201", deptInfo.Register);
            Assert.AreEqual("", deptInfo.Volume);
            Assert.AreEqual("", deptInfo.Year);
            Assert.AreEqual("2013.05.13 应还", deptInfo.Status);
            Assert.AreEqual("中文图书", deptInfo.Type);
            Assert.IsTrue(bookInfo.CardInfo.Content.StartsWith("本书主要介绍"));
            Assert.IsTrue(bookInfo.CardInfo.Content.EndsWith("设计等内容。"));

            stream = File.OpenRead("details2.htm");
            bookInfo = BookInfo.Build(stream);
            Assert.IsNotNull(bookInfo);
            Assert.AreEqual("Java 7入门经典[专著]", bookInfo.CardInfo.Title);
            Assert.AreEqual("(美)Ivor Horton著；梁峰译", bookInfo.CardInfo.Author);
            Assert.AreEqual("清华大学出版社", bookInfo.CardInfo.Publisher);
            Assert.AreEqual("9787302289593", bookInfo.CardInfo.Isbn);
            Assert.AreEqual(4, bookInfo.DeptInfos.Count);
            deptInfo = bookInfo.DeptInfos[2];
            Assert.AreEqual("大学城人文科学书库", deptInfo.DeptPlace);
            Assert.AreEqual("TP312JA/H973-2", deptInfo.Index);
            Assert.AreEqual("A3030595", deptInfo.Register);
            Assert.AreEqual("", deptInfo.Volume);
            Assert.AreEqual("", deptInfo.Year);
            Assert.AreEqual("正在编目", deptInfo.Status);
            Assert.AreEqual("中文图书", deptInfo.Type);
            Assert.IsTrue(bookInfo.CardInfo.Content.StartsWith("责任者规范汉译姓: 霍顿。"));
            Assert.IsTrue(bookInfo.CardInfo.Content.EndsWith("责任者规范汉译姓: 霍顿。"));
        }
    }
}

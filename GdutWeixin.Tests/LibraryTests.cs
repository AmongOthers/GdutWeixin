using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GdutWeixin.Models.Library;
using GdutWeixin.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GdutWeixin.Tests
{
    [TestClass]
    public class LibraryTests
    {
        Stopwatch mStopwatch = Stopwatch.StartNew();

        [TestInitialize]
        public void Setup()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [TestMethod]
        public void Parse()
        {
            Stream stream;
            List<Book> books;
            int pageCount;
            int resultCount;
            stream = File.OpenRead("hello.htm");
            Library.GetInstance().Parse(stream, out books, out pageCount, out resultCount);
            Assert.AreEqual(8, books.Count);
            Assert.AreEqual("Posts & Telecom Press", books[0].Publisher);
            Assert.AreEqual(4, pageCount);
            Assert.AreEqual(29, resultCount);
            stream = File.OpenRead("java.htm");
            Library.GetInstance().Parse(stream, out books, out pageCount, out resultCount);
            Assert.AreEqual(8, books.Count);
            Assert.AreEqual(163, pageCount);
            Assert.AreEqual(1303, resultCount);
            stream = File.OpenRead("java2.htm");
            Library.GetInstance().Parse(stream, out books, out pageCount, out resultCount);
            Assert.AreEqual(2448, resultCount);
            stream = File.OpenRead("cpp.htm");
            Library.GetInstance().Parse(stream, out books, out pageCount, out resultCount);
            Assert.AreEqual(20, books.Count);
            Assert.AreEqual(97, pageCount);
            Assert.AreEqual(1930, resultCount);
        }
    }
}

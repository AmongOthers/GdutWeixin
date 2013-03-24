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
            stream = File.OpenRead("hello.htm");
            Library.GetInstance().Parse(stream, out books, out pageCount);
            Assert.AreEqual(8, books.Count);
            Assert.AreEqual(4, pageCount);
            stream = File.OpenRead("java.htm");
            Library.GetInstance().Parse(stream, out books, out pageCount);
            Assert.AreEqual(8, books.Count);
            Assert.AreEqual(163, pageCount);
        }
    }
}

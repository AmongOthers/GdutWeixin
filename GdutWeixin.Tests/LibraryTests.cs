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
        public void Search_AllDept()
        {
			object error;
            mStopwatch.Restart();
            var result = Library.GetInstance().SearchBooksFor("", "hello", out error);
            Assert.AreEqual(4, result.PageCount);
            mStopwatch.Stop();
            var ms = mStopwatch.ElapsedMilliseconds;
            Console.WriteLine(ms);
        }

        [TestMethod]
        public void Search_DONGFENGLU()
        {
			object error;
            mStopwatch.Restart();
            Library.GetInstance().SearchBooksFor("", "hello", out error, new Library.LibrarySearchOption
            {
                DeptPlace = Library.LibrarySearchOption.DeptPlaceOption.DONGFENGLU
            });
            mStopwatch.Stop();
            var ms = mStopwatch.ElapsedMilliseconds;
            Console.WriteLine(ms);
        }

        [TestMethod]
        public void Search_WithCache()
        {
            object error;
			LibrarySearchResult result, cachedResult;
            long elapse, cachedElapse;
            mStopwatch.Restart();
            result = Library.GetInstance().SearchBooksFor("", "java", out error);
            mStopwatch.Stop();
            elapse = mStopwatch.ElapsedMilliseconds;
            mStopwatch.Restart();
            cachedResult = Library.GetInstance().SearchBooksFor("", "java", out error);
            mStopwatch.Stop();
            cachedElapse = mStopwatch.ElapsedMilliseconds;
            Assert.AreEqual(result, cachedResult);
            Assert.IsTrue(cachedElapse < elapse / 4);
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

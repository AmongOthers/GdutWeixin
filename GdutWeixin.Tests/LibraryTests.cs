using System;
using System.Diagnostics;
using GdutWeixin.Models.Library;
using GdutWeixin.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GdutWeixin.Tests
{
    [TestClass]
    public class LibraryTests
    {
        Stopwatch mStopwatch = Stopwatch.StartNew();

        [TestMethod]
        public void Search_AllDept()
        {
			object error;
			string url;
            mStopwatch.Restart();
            Library.GetInstance().SearchBooksFor("", "hello", out error);
            mStopwatch.Stop();
            var ms = mStopwatch.ElapsedMilliseconds;
            Console.WriteLine(ms);
        }

        [TestMethod]
        public void Search_DONGFENGLU()
        {
			object error;
			string url;
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
    }
}

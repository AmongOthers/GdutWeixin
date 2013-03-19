using System;
using System.IO;
using GdutWeixin.Models.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GdutWeixin.Tests
{
    [TestClass]
    public class LibraryTableResultTests
    {
        [TestInitialize]
        public void Setup()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [TestMethod]
        public void Parse()
        {
            var html = File.OpenRead("example.html");
            var books = LibraryTableResult.Parse(html);
            Assert.IsTrue(books.Count > 0);
            Assert.IsTrue(books[0].Url != null);
        }
    }
}

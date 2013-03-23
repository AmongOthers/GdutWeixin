using System;
using GdutWeixin.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GdutWeixin.Tests
{
    [TestClass]
    public class BaiduImageSearchEngineTests
    {
        [TestMethod]
        public void SearchImageFor_isbn()
        {
            string isbn = "9787111365464";
            var result = BaiduImageSearchEngine.GetInstance().SearchImageFor(isbn);
            Assert.AreEqual("http://t3.baidu.com/it/u=593184092,2598540015&fm=21&gp=0.jpg", result);
        }

        [TestMethod]
        public void SearchImageFor_ErrorIsbn_ReturnNull()
        {
            string isbn = "99787111213826";
            var result = BaiduImageSearchEngine.GetInstance().SearchImageFor(isbn);
            Assert.AreEqual(null, result);
        }
    }
}

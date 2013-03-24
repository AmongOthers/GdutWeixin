using System;
using GdutWeixin.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GdutWeixin.Tests
{
    [TestClass]
    public class XmlEncoderTests
    {
        [TestMethod]
        public void Encode()
        {
            string test = "Post & Put &amp; &#38; & &";
            test = HtmlEntityCorrect.Encode(test);
            Assert.AreEqual("Post &#38; Put &#38; &#38; &#38; &#38;", test);
            test = HtmlEntityCorrect.Decode(test);
            Assert.AreEqual("Post & Put & & & &", test);
        }
    }
}

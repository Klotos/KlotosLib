using System;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class HtmlToolsTests
    {
        [TestCase(null, Result = null)]
        [TestCase(" \n", Result = " \n")]
        [TestCase("x", Result = "x")]
        [TestCase("<x cc>  </x cc>", Result = "  ")]
        [TestCase("<html><body> <title>incorrect body and title</body></title> <br/> <img src = \"x\"></html>", Result = " incorrect body and title  ")]
        public String RemoveHTMLTagsTest(String Input)
        {
            return HtmlTools.RemoveHTMLTags(Input);
        }

        [TestCase(null, Result = null)]
        [TestCase(" \n", Result = " \n")]
        [TestCase("x", Result = "x")]
        [TestCase("<x>", Result = "<x>")]
        [TestCase("<x cc>  </x cc>", Result = "")]
        [TestCase("<text><br> start<hl attr=\"val\"></hl>end <br /></text><tr><>", Result = "<text><br> startend <br /></text><tr><>")]
        [TestCase("<text><br> start<hl attr=\"val\"><tr><xr></xr></tr></hl>end_<tag3><tag4 >tag4text</tag4 ></tag3 > <br /></text><tr><>",
            Result = "<text><br> startend_<tag3><tag4 >tag4text</tag4 ></tag3 > <br /></text><tr><>")]
        [TestCase("<html>< body><p></p></body></html >", Result = "")]
        [TestCase("<html>< body><p>passage</p></body></html >", Result = "<html>< body><p>passage</p></body></html >")]
        public String RemoveEmptyPairHTMLTagsTest(String Input)
        {
            return HtmlTools.RemoveEmptyPairHTMLTags(Input);
        }

        [Test]
        public void AdjustTextToHtmlTest()
        {
            String input1 = " <html>"+
                            "   text with 3 spaces behind"+
                            "<a href=\"source\">link text</a>"+
                            "here\ncarriage\r3\r\nreturns"+
                            "</html>";
            String output1 = HtmlTools.AdjustTextToHtml(input1);
            String valid1 = "&ensp;&lt;html&gt;&ensp;&ensp;&ensp;text with 3 spaces behind&lt;a href=&quot;source&quot;&gt;link text&lt;/a&gt;here<br/>carriage<br/>3<br/>returns&lt;/html&gt;";
            Assert.AreEqual(output1, valid1, "output="+output1);
        }

        [Test]
        public void FixBrokenXMLTagsTest()
        {
            String input1 = "<html>without closing html<body><title>incorrect</clos> body and title</body></title><br></closing_without_opening>";
            String output1 = HtmlTools.FixBrokenXMLTags(input1);
            String valid1 = "<html>without closing html<body><title>incorrect body and title</title><br></br></body></html>";
            Assert.AreEqual(output1, valid1, output1);
        }

        [Test]
        public void ValidateHtmlTagTest()
        {
            String input_tag1 = " <body > ";
            String output_tag_name1;
            HtmlTools.HtmlTagType res1 = HtmlTools.ValidateHtmlTag(input_tag1, out output_tag_name1);
            Assert.AreEqual("body", output_tag_name1);
            Assert.AreEqual(HtmlTools.HtmlTagType.PairOpen, res1);

            String input_tag2 = " </html > ";
            String output_tag_name2;
            HtmlTools.HtmlTagType res2 = HtmlTools.ValidateHtmlTag(input_tag2, out output_tag_name2);
            Assert.AreEqual("html", output_tag_name2);
            Assert.AreEqual(HtmlTools.HtmlTagType.PairClose, res2);

            String input_tag3 = " <br /> ";
            String output_tag_name3;
            HtmlTools.HtmlTagType res3 = HtmlTools.ValidateHtmlTag(input_tag3, out output_tag_name3);
            Assert.AreEqual("br", output_tag_name3);
            Assert.AreEqual(HtmlTools.HtmlTagType.Single, res3);

            String input_tag4 = " xxx ";
            String output_tag_name4;
            HtmlTools.HtmlTagType res4 = HtmlTools.ValidateHtmlTag(input_tag4, out output_tag_name4);
            Assert.AreEqual(null, output_tag_name4);
            Assert.AreEqual(HtmlTools.HtmlTagType.NotTag, res4);
        }

        [Test]
        public void SecureScriptXSSTest()
        {
            String input1 = "<body>text <a href=z>link</a><script>alert(ha-ha)< / script > <body>";
            String output1 = HtmlTools.SecureScriptXSS(input1);
            String valid1 = "<body>text <a href=z>link</a>&lt;script&gt;alert(ha-ha)&lt;/script&gt; <body>";
            Assert.AreEqual(output1, valid1, output1);
        }
    }
}

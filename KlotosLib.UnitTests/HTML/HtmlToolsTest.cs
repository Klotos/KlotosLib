using System;
using System.Collections.Generic;
using KlotosLib.HTML;
using NUnit.Framework;

namespace KlotosLib.UnitTests.HTML
{
    [TestFixture]
    public class HtmlToolsTest
    {
        [NUnit.Framework.Test]
        public void HtmlElementDeficient()
        {
            const String input_HTML_1 = "<HTML>" +
                                  "<head>" +
                                  "<title>title</title>" +
                                  "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>" +
                                  "</head>" +
                                  "<body>" +
                                  "<meta charset=\"utf-8\" />" +
                                  "</body>" +
                                  "</HTML>";
            HtmlElementDeficient output1_1 = KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml(input_HTML_1, "meta", 0);
            HtmlElementDeficient output1_2 = KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml(input_HTML_1, "meta", 50);
            HtmlElementDeficient output1_3 = KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml(input_HTML_1, "head", 0);
            HtmlElementDeficient output1_4 = KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml(input_HTML_1, "head", 50);

            Dictionary<String, String> expected1_1 = new Dictionary<string, string>()
            {
                {"http-equiv", "Content-Type"},
                {"content", "text/html; charset=utf-8"}
            };
            
            Dictionary<String, String> expected1_2 = new Dictionary<string, string>()
            {
                {"charset", "utf-8"}
            };

            Dictionary<String, String> expected1_3 = new Dictionary<string, string>(0);

            CollectionAssert.AreEqual(expected1_1, output1_1.Attributes);
            CollectionAssert.AreEqual(expected1_2, output1_2.Attributes);
            CollectionAssert.AreEqual(expected1_3, output1_3.Attributes);
            Assert.IsNull(output1_4);

            const String input_HTML_2 = "<p><select SIZE=\"3\" size = \"4\" name=\"hero\">"+
                "<option ID= \"x\" disabled=disabled>Выберите героя</option>"+
                "<option value=\"t1\" selected id = 2 other = \"3\" other = 4>Чебурашка</option>"+
                "<option value=\"t2\" selected disabled other=4>Крокодил Гена</option>" +
                "<option value = \" t3 \" id= 2 disabled other = '3' another = 4 disabled = 'disabled' >Зло</option>" +
                "</select></p>";
            Dictionary<String, String> output2_1 =
                KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml(input_HTML_2, " select", 0).Attributes;
            Dictionary<String, String> output2_2 =
                KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml(input_HTML_2, " option ", 36).Attributes;
            Dictionary<String, String> output2_3 =
                KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml(input_HTML_2, "<option", 80).Attributes;
            Dictionary<String, String> output2_4 =
                KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml(input_HTML_2, "Option", 120).Attributes;
            Dictionary<String, String> output2_5 =
                KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml(input_HTML_2, "<OPTION ", 190).Attributes;

            Dictionary<String, String> expected2_1 = new Dictionary<string, string>(2)
            {
                {"SIZE", "3"},
                {"name", "hero"}
            };

            Dictionary<String, String> expected2_2 = new Dictionary<string, string>(2)
            {
                {"ID", "x"},
                {"disabled", "disabled"}
            };

            Dictionary<String, String> expected2_3 = new Dictionary<string, string>(4)
            {
                {"value", "t1"},
                {"selected", ""},
                {"id", "2"},
                {"other", "3"}
            };

            Dictionary<String, String> expected2_4 = new Dictionary<string, string>(4)
            {
                {"value", "t2"},
                {"selected", ""},
                {"disabled", ""},
                {"other", "4"}
            };

            Dictionary<String, String> expected2_5 = new Dictionary<string, string>(5)
            {
                {"value", " t3 "},
                {"id", "2"},
                {"disabled", ""},
                {"other", "3"},
                {"another", "4"}
            };

            CollectionAssert.AreEqual(expected2_1, output2_1);
            CollectionAssert.AreEqual(expected2_2, output2_2);
            CollectionAssert.AreEqual(expected2_3, output2_3);
            CollectionAssert.AreEqual(expected2_4, output2_4);
            CollectionAssert.AreEqual(expected2_5, output2_5);

            Assert.Throws<ArgumentNullException>(
                delegate { KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml(null, "html", 0); }
            );
            Assert.Throws<ArgumentException>(
                delegate { KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml(" \r\n ", "html", 0); }
            );
            Assert.Throws<ArgumentException>(
                delegate { KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml("<html id='x'></html>", " \r\n ", 0); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                delegate { KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml("<html id='x'></html>", "html", -1); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                delegate { KlotosLib.HTML.HtmlElementDeficient.ExtractElementFromHtml("<html id='x'></html>", "html", 20); }
            );
        }

        [TestCase(null, Result = null)]
        [TestCase(" \n", Result = " \n")]
        [TestCase("x", Result = "x")]
        [TestCase("a<>b<>c", Result = "a<>b<>c")]
        [TestCase(" <html>text</html>_<>y<>x", Result = " text_<>y<>x")]
        [TestCase("<x cc>  </x cc>", Result = "  ")]
        [TestCase("<html><body> <title>incorrect body and title</body></title> <br/> <img src = \"x\"></html>", ExpectedResult = " incorrect body and title  ")]
        [TestCase("text: <br>first line<br >second line: value<br/><br/><b><a href=\"", ExpectedResult = "text: first linesecond line: value")]
        [TestCase("<a href> text:&nbsp;<br>first line<br >second line:&nbsp;value<a href=\"xxx\"", ExpectedResult = " text:&nbsp;first linesecond line:&nbsp;value")]
        public String RemoveHTMLTags(String Input)
        {
            return HtmlTools.RemoveHTMLTags(Input);
        }

        [TestCase(null, Result = null)]
        [TestCase(" \n", Result = " \n")]
        [TestCase("x", Result = "x")]
        [TestCase("<x cc>  </x cc>", Result = "  ")]
        [TestCase("<html><body> <title>incorrect body and title</body></title> <br/> <img src = \"x\"></html>final",
            ExpectedResult = " incorrect body and title \r\n final")]
        [TestCase("text: <br>first line<br >second line: value<br/ >< br/><b><a href=\"", ExpectedResult = "text: \r\nfirst line\r\nsecond line: value\r\n\r\n")]
        [TestCase(" <b>text1&nbsp;text2<br>second&ensp;line< br >third line</b> ", ExpectedResult = " text1 text2\r\nsecond line\r\nthird line ")]
        [TestCase("bla-bla<br />bla bla<html><body>text&nbsp; text</body></html>", ExpectedResult = "bla-bla\r\nbla blatext  text")]
        [TestCase("<a href> text:&nbsp;<br>first line<br >second line:&nbsp;value<a href=\"xxx\"", ExpectedResult = " text: \r\nfirst line\r\nsecond line: value")]
        [TestCase("<bold><bla> text:&nbsp;<br>first line<br >second line:&nbsp;value</ bold><a href=\"xxx\"", ExpectedResult = " text: \r\nfirst line\r\nsecond line: value")]
        public String IntelliRemoveHTMLTags(String InputHTML)
        {
            return HtmlTools.IntelliRemoveHTMLTags(InputHTML);
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
        public String RemoveEmptyPairHTMLTags(String Input)
        {
            return HtmlTools.RemoveEmptyPairHTMLTags(Input);
        }

        [Test]
        public void AdjustTextToHtml()
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
        public void FixBrokenXMLTags()
        {
            String input1 = "<html>without closing html<body><title>incorrect</clos> body and title</body></title><br></closing_without_opening>";
            String output1 = HtmlTools.FixBrokenXMLTags(input1);
            String valid1 = "<html>without closing html<body><title>incorrect body and title</title><br></br></body></html>";
            Assert.AreEqual(output1, valid1, output1);
        }

        [Test]
        public void ValidateHtmlTag()
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
        public void SecureScriptXSS()
        {
            String input1 = "<body>text <a href=z>link</a><script>alert(ha-ha)< / script > <body>";
            String output1 = HtmlTools.SecureScriptXSS(input1);
            String valid1 = "<body>text <a href=z>link</a>&lt;script&gt;alert(ha-ha)&lt;/script&gt; <body>";
            Assert.AreEqual(output1, valid1, output1);
        }
    }
}

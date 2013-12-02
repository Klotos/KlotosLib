using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class StringToolsSubstringHelpersTests
    {
        [TestCase("0123456789", 5, true, 0, Result = "56789")]
        [TestCase("0123456789", 5, false, 1, Result = "98765")]
        [TestCase("0123456789", 20, true, 0, Result = "0123456789")]
        [TestCase("0123456789", 20, false, 0, Result = "0123456789", ExpectedException=typeof(ArgumentOutOfRangeException))]
        public String SubstringFromEnd(String Input, Int32 Length, Boolean UntilStart, Byte Direction)
        {
            return StringTools.SubstringHelpers.SubstringFromEnd(Input, Length, UntilStart, (StringTools.Direction)Direction);
        }

        [TestCase("abcdStart inner Startend end startXXXend", "Start", "end", StringComparison.Ordinal, ExpectedResult = "abcd end startXXXend")]
        [TestCase("abcdStart inner Startend end startXXXend", "Start", "end", StringComparison.OrdinalIgnoreCase, ExpectedResult = "abcd end ")]
        [TestCase("abcdStart inner Startend end start XXX enD STARTEND ZZZ", "start", "end", StringComparison.OrdinalIgnoreCase, ExpectedResult = "abcd end   ZZZ")]
        public String RemoveFromStartToEndToken(String InputString, String StartToken, String EndToken, StringComparison CompOpt)
        {
            return StringTools.SubstringHelpers.RemoveFromStartToEndToken(InputString, StartToken, EndToken, CompOpt);
        }

        [Test]
        public void GetInnerStringBetweenTokens()
        {
            const String input1 = "012abc inner1 cba xxx abc inner2 cba";
            Int32 pos;
            String output1 = StringTools.SubstringHelpers.GetInnerStringBetweenTokens(input1, "abc", "CBA", 0, StringComparison.OrdinalIgnoreCase, out pos);
            Assert.AreEqual(" inner1 ", output1);
            Assert.AreEqual(6, pos);

            String output2 = StringTools.SubstringHelpers.GetInnerStringBetweenTokens(input1, "abc", "CBA", 10, StringComparison.OrdinalIgnoreCase, out pos);
            Assert.AreEqual(" inner2 ", output2);
            Assert.AreEqual(25, pos);
        }
        
        [Test]
        public void GetInnerStringsBetweenTokens()
        {
            const String input1 = "abcd Start inner1 Endstartinner2end other";
            List<Substring> output1 = StringTools.SubstringHelpers.GetInnerStringsBetweenTokens(input1, "Start", "End", 0, StringComparison.Ordinal);
            CollectionAssert.AreEqual(new List<Substring>(1) { new Substring(input1, 10, 8) }, output1, 
                "Output1 = "+output1.ConcatToString(", "));

            List<Substring> output2 = StringTools.SubstringHelpers.GetInnerStringsBetweenTokens(input1, "Start", "End", 0, StringComparison.OrdinalIgnoreCase);
            CollectionAssert.AreEqual(new List<Substring>(2) { new Substring(input1, 10, 8), new Substring(input1, 26, 6) }, output2,
                "Output2 = " + output2.ConcatToString(", "));

            List<Substring> output3 = StringTools.SubstringHelpers.GetInnerStringsBetweenTokens(input1, "Start", "End", 15, StringComparison.OrdinalIgnoreCase);
            CollectionAssert.AreEqual(new List<Substring>(1) { new Substring(input1, 26, 6) }, output3,
                "Output3 = " + output3.ConcatToString("; "));

            const string input4 = "<br>< a href> <<>";
            List<Substring> output4 = StringTools.SubstringHelpers.GetInnerStringsBetweenTokens(input4, "<", ">", 0, StringComparison.Ordinal);
            CollectionAssert.AreEqual(new List<Substring>(3) { new Substring(input4, 1, 2), new Substring(input4, 5, 7), new Substring(input4, 15, 1) }, output4,
                "Output4 = " + output4.ConcatToString());

            const string input5 = "abcd Start inner1 enDStartinner2enD other";
            List<Substring> output5_1 = StringTools.SubstringHelpers.GetInnerStringsBetweenTokens(input5, "Start", "enD", 0, StringComparison.Ordinal);
            CollectionAssert.AreEqual(new List<Substring>(2) { new Substring(input5, 10, 8), new Substring(input5, 26, 6) }, output5_1, 
                "Output5_1 = "+output5_1.ConcatToString());
            List<Substring> output5_2 = StringTools.SubstringHelpers.GetInnerStringsBetweenTokens(input5, "StarT", "END", 0, StringComparison.OrdinalIgnoreCase);
            CollectionAssert.AreEqual(new List<Substring>(2) { new Substring(input5, 10, 8), new Substring(input5, 26, 6) }, output5_2,
                "Output5_2 = "+output5_2.ConcatToString());

            const String input6 = "012<a>45</a><b>bb</b><a></a><A>ttt</A><a></a><a></A>";
            List<Substring> output6_1 = StringTools.SubstringHelpers.GetInnerStringsBetweenTokens(input6, "<a>", "</a>", 3, StringComparison.OrdinalIgnoreCase);
            CollectionAssert.AreEqual(new List<Substring>(5){new Substring(input6, 6, 2), null, new Substring(input6, 31, 3), null, null}, output6_1, 
                "Output6_1 = "+output6_1.ConcatToString("", "", ", ", false, true));
            List<Substring> output6_2 = StringTools.SubstringHelpers.GetInnerStringsBetweenTokens(input6, "<a>", "</a>", 4, StringComparison.OrdinalIgnoreCase);
            CollectionAssert.AreEqual(new List<Substring>(4) { null, new Substring(input6, 31, 3), null, null }, output6_2,
                "Output6_2 = " + output6_2.ConcatToString("", "", ", ", false, true));
        }

        [TestCase("1234abcd123", "cd", StringTools.Direction.FromStartToEnd, StringComparison.Ordinal, ExpectedResult = "1234ab")]
        [TestCase("1234abcd123", "CD", StringTools.Direction.FromStartToEnd, StringComparison.Ordinal, ExpectedResult = "1234abcd123")]
        [TestCase("1234abcd123", "CD", StringTools.Direction.FromStartToEnd, StringComparison.OrdinalIgnoreCase, ExpectedResult = "1234ab")]
        [TestCase("1234abcd123", "cd", StringTools.Direction.FromEndToStart, StringComparison.Ordinal, ExpectedResult = "123")]
        [TestCase("1234abcd123", "CD", StringTools.Direction.FromEndToStart, StringComparison.Ordinal, ExpectedResult = "1234abcd123")]
        [TestCase("1234abcd123", "CD", StringTools.Direction.FromEndToStart, StringComparison.OrdinalIgnoreCase, ExpectedResult = "123")]
        public String GetSubstringToToken(String Input, String Token, StringTools.Direction Dir, StringComparison CompareOptions)
        {
            return StringTools.SubstringHelpers.GetSubstringToToken(Input, Token, Dir, CompareOptions);
        }

        [TestCase("1234abcd123", "3", true, StringTools.Direction.FromStartToEnd, StringComparison.Ordinal, ExpectedResult = "34abcd123")]
        [TestCase("1234abcd123", "3", false, StringTools.Direction.FromStartToEnd, StringComparison.Ordinal, ExpectedResult = "4abcd123")]
        [TestCase("1234abcd123", "2", true, StringTools.Direction.FromEndToStart, StringComparison.Ordinal, ExpectedResult = "1234abcd12")]
        [TestCase("1234abcd123", "2", false, StringTools.Direction.FromEndToStart, StringComparison.Ordinal, ExpectedResult = "1234abcd1")]
        [TestCase("1234abcd123", "ab", true, StringTools.Direction.FromStartToEnd, StringComparison.Ordinal, ExpectedResult = "abcd123")]
        [TestCase("1234abcd123", "AB", true, StringTools.Direction.FromStartToEnd, StringComparison.Ordinal, ExpectedResult = "1234abcd123")]
        [TestCase("1234abcd123", "AB", true, StringTools.Direction.FromStartToEnd, StringComparison.OrdinalIgnoreCase, ExpectedResult = "abcd123")]
        public String TruncateToClosestToken(String Input, String Token, Boolean LeaveToken, StringTools.Direction Dir, StringComparison CompareOptions)
        {
            return StringTools.SubstringHelpers.TruncateToClosestToken(Input, Token, LeaveToken, Dir, CompareOptions);
        }

        [TestCase("012abc345abc678", "abc", 1, StringTools.Direction.FromStartToEnd, StringComparison.Ordinal, ExpectedResult = "012")]
        [TestCase("012abc345abc678", "abc", 2, StringTools.Direction.FromStartToEnd, StringComparison.Ordinal, ExpectedResult = "012abc345")]
        [TestCase("012abc345abc678", "aBC", 2, StringTools.Direction.FromStartToEnd, StringComparison.Ordinal, ExpectedResult = "012abc345abc678")]
        [TestCase("012abc345abc678", "aBC", 2, StringTools.Direction.FromStartToEnd, StringComparison.OrdinalIgnoreCase, ExpectedResult = "012abc345")]
        [TestCase("012abc345abc678", "abc", 1, StringTools.Direction.FromEndToStart, StringComparison.Ordinal, ExpectedResult = "678")]
        [TestCase("012abc345abc678", "abc", 2, StringTools.Direction.FromEndToStart, StringComparison.Ordinal, ExpectedResult = "345abc678")]
        [TestCase("012abc345abc678", "aBC", 2, StringTools.Direction.FromEndToStart, StringComparison.Ordinal, ExpectedResult = "012abc345abc678")]
        [TestCase("012abc345abc678", "aBC", 2, StringTools.Direction.FromEndToStart, StringComparison.OrdinalIgnoreCase, ExpectedResult = "345abc678")]
        public String GetSubstringToTokenWithSpecifiedNumber1(String Input, String Token, Byte Number, StringTools.Direction Dir, StringComparison CompareOptions)
        {
            return StringTools.SubstringHelpers.GetSubstringToTokenWithSpecifiedNumber(Input, Token, Number, Dir, CompareOptions);
        }

        [TestCase("012abc345abc678", "abc", 1, 0, StringComparison.Ordinal, ExpectedResult = "012")]
        [TestCase("012abc345abc678", "abc", 1, 2, StringComparison.Ordinal, ExpectedResult = "2")]
        [TestCase("012abc345abc678", "abc", 2, 2, StringComparison.Ordinal, ExpectedResult = "2abc345")]
        [TestCase("012abc345abc678", "abc", 1, 5, StringComparison.Ordinal, ExpectedResult = "c345")]
        [TestCase("012abc345abc678", "ABC", 1, 2, StringComparison.Ordinal, ExpectedResult = "012abc345abc678")]
        [TestCase("012abc345abc678", "ABC", 1, 2, StringComparison.OrdinalIgnoreCase, ExpectedResult = "2")]
        public String GetSubstringToTokenWithSpecifiedNumber2(String Input, String Token, Byte Number, Int32 StartPosition, StringComparison CompareOptions)
        {
            return StringTools.SubstringHelpers.GetSubstringToTokenWithSpecifiedNumber(Input, Token, Number, StartPosition, CompareOptions);
        }

        [TestCase("<p>abc</p>", "<p>", "</p>", StringComparison.Ordinal, ExpectedResult = "abc")]
        [TestCase("<P>abc</p>", "<p>", "</p>", StringComparison.Ordinal, ExpectedResult = "<P>abc</p>")]
        [TestCase("<P>abc</p>", "<p>", "</p>", StringComparison.OrdinalIgnoreCase, ExpectedResult = "abc")]
        [TestCase("<p>abc</b>", "<p>", "</p>", StringComparison.OrdinalIgnoreCase, ExpectedResult = "<p>abc</b>")]
        [TestCase("<P><p>abc</P></p>", "<P>", "</P>", StringComparison.OrdinalIgnoreCase, ExpectedResult = "abc")]
        public String DeleteStartAndEndTokens(String Input, String StartToken, String EndToken, StringComparison CompOpt)
        {
            return StringTools.SubstringHelpers.DeleteStartAndEndTokens(Input, StartToken, EndToken, CompOpt);
        }

        [TestCase("abc \r\n end 123", StringTools.StringAnalyzers.ContainsEntities.Letters, 
            ExpectedResult = "abcend")]
        [TestCase("abc \r\n end ", StringTools.StringAnalyzers.ContainsEntities.Empty, ExpectedResult = "abc \r\n end ")]
        [TestCase("abc \r\n end 123",
            StringTools.StringAnalyzers.ContainsEntities.Letters | StringTools.StringAnalyzers.ContainsEntities.Digits | 
            StringTools.StringAnalyzers.ContainsEntities.Spaces,
            ExpectedResult = "abc  end 123")]
        [TestCase("abc \r\n end 123", StringTools.StringAnalyzers.ContainsEntities.Controls, ExpectedResult = "\r\n")]
        public String CleanFromChars(String Input, StringTools.StringAnalyzers.ContainsEntities AllowedSymbols)
        {
            return StringTools.SubstringHelpers.CleanFromChars(Input, AllowedSymbols);
        }

        [TestCase("select * from ';delete from admin--", ExpectedResult = "select * from '';delete from admin--")]
        public String SecureSQLQuery(String Input)
        {
            return StringTools.SubstringHelpers.SecureSQLQuery(Input);
        }

        [TestCase(3, "ab", "bc", "1", ExpectedResult = "abbc1abbc1abbc1")]
        [TestCase(0, "ab", "bc", "1", ExpectedResult = "")]
        [TestCase(1, "ab", "bc", "1", ExpectedResult = "abbc1")]
        [TestCase(4, "ab", "", "", ExpectedResult = "abababab")]
        public String ConcatenateAllStringsManyTimes(Int32 IterationsCount, params String[] Input)
        {
            return StringTools.SubstringHelpers.ConcatenateAllStringsManyTimes(IterationsCount, Input);
        }

        [TestCase("", ExpectedResult = "")]
        [TestCase(" ", ExpectedResult = " ")]
        [TestCase("  ", ExpectedResult = " ")]
        [TestCase(" a ", ExpectedResult = " a ")]
        [TestCase("  b  ", ExpectedResult = " b ")]
        [TestCase("  a  b  c ", ExpectedResult = " a b c ")]
        [TestCase("  a\r\ncarriage return \r\n  c  ", ExpectedResult = " a\r\ncarriage return \r\n c ")]
        [TestCase("  ab \r\n cd  \r\n\r\n  ef   ", ExpectedResult = " ab \r\n cd \r\n\r\n ef ")]
        public String ShrinkSpaces(String Input)
        {
            return StringTools.SubstringHelpers.ShrinkSpaces(Input);
        }
    }
}

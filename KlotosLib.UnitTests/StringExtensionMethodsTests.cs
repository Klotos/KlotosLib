using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class StringExtensionMethodsTests
    {
        [TestCase("abcdefghijkl", 5, 9, true, false, true, Result="fghi")]
        [TestCase("abcdefghijkl", 5, 9, false, true, false, Result = "ghij")]
        [TestCase("abcdefghijkl", 5, 19, true, true, true, Result = "fghijkl")]
        [TestCase("abcdefghijkl", 5, 19, true, true, false, Result = "fghijkl", ExpectedException=typeof(ArgumentOutOfRangeException))]        
        public String SubstringWithEnd(String Input, Int32 StartIndex, Int32 EndIndex, Boolean IncludeStart, Boolean IncludeEnd, Boolean UntilEnd)
        {
            return Input.SubstringWithEnd(StartIndex, EndIndex, IncludeStart, IncludeEnd, UntilEnd);
        }

        [TestCase("1234", 0, Result = "")]
        [TestCase("1234", 2, Result="12")]
        [TestCase("1234", 3, Result = "123")]
        [TestCase("1234", 4, Result = "1234")]
        [TestCase("1234", 5, Result = "1234")]
        [TestCase("1234", -5, Result = "1234", ExpectedException=typeof(ArgumentOutOfRangeException))]
        [TestCase(null, 5, Result = "1234", ExpectedException = typeof(ArgumentException))]
        public String LeftSubstring(String Input, Int32 Count)
        {
            return Input.LeftSubstring(Count);
        }

        [TestCase("1234", 0, Result = "")]
        [TestCase("1234", 2, Result = "34")]
        [TestCase("1234", 3, Result = "234")]
        [TestCase("1234", 4, Result = "1234")]
        [TestCase("1234", 5, Result = "1234")]
        [TestCase("1234", -5, Result = "1234", ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(null, 5, Result = "1234", ExpectedException = typeof(ArgumentException))]
        public String RightSubstring(String Input, Int32 Count)
        {
            return Input.RightSubstring(Count);
        }

        [TestCase("1234", 0, Result = "1234")]
        [TestCase("1234", 1, Result = "234")]
        [TestCase("1234", 3, Result = "4")]
        [TestCase("1234", 4, Result = "")]
        [TestCase("1234", 5, Result = "")]
        [TestCase("1234", -5, Result = "1234", ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(null, 5, Result = "1234", ExpectedException = typeof(ArgumentException))]
        public String CutLeft(String Input, Int32 Count)
        {
            return Input.CutLeft(Count);
        }

        [TestCase("1234", 0, Result = "1234")]
        [TestCase("1234", 1, Result = "123")]
        [TestCase("1234", 3, Result = "1")]
        [TestCase("1234", 4, Result = "")]
        [TestCase("1234", 5, Result = "")]
        [TestCase("1234", -5, Result = "1234", ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(null, 5, Result = "1234", ExpectedException = typeof(ArgumentException))]
        public String CutRight(String Input, Int32 Count)
        {
            return Input.CutRight(Count);
        }

        [TestCase("abcdabcdefg", "ab", 4, true, Result = "cdabcdefg")]
        [TestCase("abcdabcdefg", "AB", 4, true, Result = "abcdabcdefg")]
        [TestCase("abcdabcdefg", "AB", 5, true, Result = "cdabcdefg")]
        [TestCase("abcdabcdefg", "ABcd", 5, false, Result = "abcdefg")]
        [TestCase("abcdabcdefg", "ABcd", 5, true, Result = "efg")]
        [TestCase("abcdabcdefg", "ABcd", 4, true, Result = "abcdabcdefg")]
        [TestCase("abcdabcdefg", "", 4, true, Result = "abcdabcdefg")]
        [TestCase("abcdabcdefg", null, 4, true, Result = "abcdabcdefg")]
        [TestCase(null, "ABcd", 4, true, Result = "", ExpectedException=typeof(ArgumentException))]
        public String TrimStart(String Source, String Target, Int32 StrComp, Boolean Recursive)
        {
            return Source.TrimStart(Target, (StringComparison)StrComp, Recursive);
        }

        [TestCase("abcdefghfgh", "fgh", 4, false, Result = "abcdefgh")]
        [TestCase("abcdefghfgh", "FGh", 4, false, Result = "abcdefghfgh")]
        [TestCase("abcdefghfgh", "FGh", 5, false, Result = "abcdefgh")]
        [TestCase("abcdefghfgh", "FGh", 5, true, Result = "abcde")]
        [TestCase("abcdefghfgh", "FG", 5, true, Result = "abcdefghfgh")]        
        [TestCase("abcdefghfgh", "", 4, true, Result = "abcdefghfgh")]
        [TestCase("abcdefghfgh", null, 4, true, Result = "abcdefghfgh")]
        [TestCase(null, "FGh", 4, true, Result = "", ExpectedException = typeof(ArgumentException))]
        public String TrimEnd(String Source, String Target, Int32 StrComp, Boolean Recursive)
        {
            return Source.TrimEnd(Target, (StringComparison)StrComp, Recursive);
        }

        [TestCase("abcd", new string[]{"", "ab", "bc", "ab"}, StringComparison.Ordinal, ExpectedResult = 1)]
        [TestCase("abcd", new string[] { "", "aB", "bc", "ab" }, StringComparison.Ordinal, ExpectedResult = 3)]
        [TestCase("abcd", new string[] { "", "aB", "bc", "Ab" }, StringComparison.Ordinal, ExpectedResult = -1)]
        [TestCase("abcd", new string[] { "", "aB", "bc", "Ab" }, StringComparison.OrdinalIgnoreCase, ExpectedResult = 1)]
        [TestCase(" abcd", new string[] { "", "aB", "bc", "Ab", "  ", " " }, StringComparison.OrdinalIgnoreCase, ExpectedResult = 5)]
        [TestCase("", new string[] { "", "aB", "bc", "Ab" }, StringComparison.OrdinalIgnoreCase, ExpectedResult = 1, ExpectedException = typeof(ArgumentException))]
        [TestCase("abcd", new string[0] { }, StringComparison.OrdinalIgnoreCase, ExpectedResult = 1, ExpectedException = typeof(ArgumentException))]
        public Int32 StartsWithOneOf(String Input, String[] Prefixes, StringComparison StrComp)
        {
            return Input.StartsWithOneOf(Prefixes, StrComp);
        }

        [TestCase("12345", new string[]{"", null, "1234567", "123456", "12345", "1234"}, StringComparison.Ordinal, ExpectedResult = 4)]
        [TestCase("12345", new string[] { "", null, "1234567", "123456", "1234" }, StringComparison.Ordinal, ExpectedResult = -1)]
        [TestCase("12345", new string[] { "445", null, "45" }, StringComparison.Ordinal, ExpectedResult = 2)]
        public Int32 EndsWithOneOf(String Input, String[] Postfixes, StringComparison StrComp)
        {
            return Input.EndsWithOneOf(Postfixes, StrComp);
        }

        [TestCase("a", 1, Result = "a")]
        [TestCase("ab", 4, Result = "abababab")]
        [TestCase("a", 0, Result = "a", ExpectedException=typeof(ArgumentOutOfRangeException))]
        [TestCase("a", -1, Result = "a", ExpectedException = typeof(ArgumentOutOfRangeException))]
        public String Replicate(String Input, Int32 Count)
        {
            return Input.Replicate(Count);
        }

        [TestCase("0123456789", 0, 3, "abcde", Result="abcde3456789")]
        [TestCase("0123456789", 5, 4, "abcde", Result = "01234abcde9")]
        [TestCase("0123456789", 5, 0, "abcde", Result = "01234abcde56789")]
        [TestCase("0123456789", 9, 0, "abcde", Result = "012345678abcde9")]
        [TestCase("0123456789", 8, 100, "abcde", Result = "01234567abcde")]
        [TestCase(null, 5, 4, "abcde", Result = "", ExpectedException=typeof(ArgumentNullException))]
        [TestCase("0123456789", 5, 4, null, Result = "", ExpectedException = typeof(ArgumentNullException))]
        [TestCase("0123456789", -10, 0, "abcde", Result = "", ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456789", 10, 0, "abcde", Result = "", ExpectedException=typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456789", 9, -1, "abcde", Result = "", ExpectedException = typeof(ArgumentOutOfRangeException))]
        public String Stuff(String Source, Int32 StartIndex, Int32 Length, String Replacement)
        {
            return Source.Stuff(StartIndex, Length, Replacement);
        }
        
        [TestCase(null, ExpectedResult = true)]
        [TestCase("", ExpectedResult = true)]
        [TestCase("  ", ExpectedResult = true)]
        [TestCase("\n", ExpectedResult = true)]
        [TestCase("\n\t \r \r\n", ExpectedResult = true)]
        [TestCase("x", ExpectedResult = false)]
        public Boolean StringEmpty(String input)
        {
            return input.IsStringNullEmptyWhiteSpace();
        }

        [TestCaseSource("HasVisibleCharsTest_GetTestStrings")]
        public Boolean HasVisibleCharsTest(String input)
        {
            return input.HasVisibleChars();
        }

        private IEnumerable<TestCaseData> HasVisibleCharsTest_GetTestStrings()
        {
            yield return new TestCaseData(null).Returns(false);
            yield return new TestCaseData("").Returns(false);
            yield return new TestCaseData("  ").Returns(false);
            yield return new TestCaseData("\n\t \r \r\n").Returns(false);
            yield return new TestCaseData("//* ++").Returns(true);
            yield return new TestCaseData("abcd").Returns(true);
        }

        [TestCaseSource("HasAlphaNumericCharsTest_GetTestStrings")]
        public Boolean HasAlphaNumericCharsTest(String input)
        {
            return input.HasAlphaNumericChars();
        }

        private IEnumerable<TestCaseData> HasAlphaNumericCharsTest_GetTestStrings()
        {
            yield return new TestCaseData(null).Returns(false);
            yield return new TestCaseData("").Returns(false);
            yield return new TestCaseData("  ").Returns(false);
            yield return new TestCaseData("\n\t \r \r\n").Returns(false);
            yield return new TestCaseData("//* ++").Returns(false);
            yield return new TestCaseData("abcd").Returns(true);
        }

        [TestCase("abc", 4, new String[3] { "abc", "bcd", "abc" }, ExpectedResult = true)]
        [TestCase("Abc", 4, new String[3] { "abc", "bcd", "abc" }, ExpectedResult = false)]
        [TestCase("Abc", 5, new String[3] { "abc", "bcd", "abc" }, ExpectedResult = true)]
        [TestCase("Abc", 5, new String[0] { }, ExpectedResult = false, ExpectedException = typeof(ArgumentException))]
        public Boolean IsIn(String Input, Int32 StrComp, params String[] Sequence)
        {
            return Input.IsIn((StringComparison)StrComp, Sequence);
        }

        [TestCase("", ExpectedResult = "")]
        [TestCase(null, ExpectedResult = null)]
        [TestCase("Abc", ExpectedResult = "Abc")]
        [TestCase("\r\nAbc\r\n", ExpectedResult = "Abc")]
        [TestCase("\r\n\r\n", ExpectedResult = "")]
        public String CleanString(String Input)
        {
            return Input.CleanString();
        }

        [TestCase("Abcd", "Ab", 4, ExpectedResult = true)]
        [TestCase("Abcd", "ab", 4, ExpectedResult = false)]
        [TestCase("Abcd", "ab", 5, ExpectedResult = true)]
        [TestCase("Abcd", "", 5, ExpectedException = typeof(ArgumentException))]
        [TestCase("", "Ab", 5, ExpectedException = typeof(ArgumentException))]
        [TestCase(null, "Ab", 5, ExpectedException = typeof(ArgumentException))]
        [TestCase("Abcd", null, 5, ExpectedException = typeof(ArgumentException))]
        public Boolean Contains(String Input, String ToCheck, Int32 StrComp)
        {
            return Input.Contains(ToCheck, (StringComparison)StrComp);
        }

        [TestCase("abcd", "12", new Char[3] { ' ', 'x', 'b' }, ExpectedResult = "a12cd")]
        [TestCase("abcd", "", new Char[3] { ' ', 'd', 'b' }, ExpectedResult = "ac")]
        [TestCase("abcd", "12", new Char[0] { }, ExpectedResult = "abcd")]
        [TestCase("abcd", "12", new Char[3] { 'x', 'y', 'z' }, ExpectedResult = "abcd")]
        [TestCase("", "12", new Char[3] { 'x', 'y', 'z' }, ExpectedResult = "", ExpectedException = typeof(ArgumentException))]
        [TestCase("abcd", null, new Char[3] { 'x', 'y', 'z' }, ExpectedResult = "", ExpectedException = typeof(ArgumentNullException))]
        public String MultiReplace1(String Source, String Destination, params Char[] Target)
        {
            return Source.MultiReplace(Destination, Target);
        }

        [TestCase("abcdabc", '_', new char[]{'a', 'c'}, ExpectedResult = "_b_d_b_")]
        public String MultiReplace2(String Source, Char Destination, params Char[] Target)
        {
            return Source.MultiReplace(Destination, Target);
        }

        [TestCase(" abc def ", Result="abcdef")]
        [TestCase("", Result = "")]
        [TestCase("  ", Result = "")]
        [TestCase(null, Result = null)]
        public String RemoveSpaces(String Input)
        {
            return Input.RemoveSpaces();
        }

        [TestCase(" abcd\r\n", 1, true, StringExtensionMethods.PaddingOptions.PadLeft, ExpectedResult = new string[] { " ", "a", "b", "c", "d", "\r", "\n" })]
        [TestCase(" abcd\r\n", 2, false, StringExtensionMethods.PaddingOptions.DoNotPad, ExpectedResult = new string[] { " a", "bc", "d\r", "\n" })]
        [TestCase(" abcd\r\n", 2, true, StringExtensionMethods.PaddingOptions.DoNotPad, ExpectedResult = new string[] { " ", "ab", "cd", "\r\n" })]
        [TestCase(" abcd\r\n", 2, false, StringExtensionMethods.PaddingOptions.PadRight, ExpectedResult = new string[] { " a", "bc", "d\r", "\n " })]
        [TestCase("0123456", 3, false, StringExtensionMethods.PaddingOptions.DoNotPad, ExpectedResult = new string[] { "012", "345", "6" })]
        [TestCase("0123456", 3, false, StringExtensionMethods.PaddingOptions.PadLeft, ExpectedResult = new string[] { "012", "345", "  6" })]
        [TestCase("0123456", 3, false, StringExtensionMethods.PaddingOptions.PadRight, ExpectedResult = new string[] { "012", "345", "6  " })]
        [TestCase("0123456", 3, true, StringExtensionMethods.PaddingOptions.DoNotPad, ExpectedResult = new string[] { "0", "123", "456" })]
        [TestCase("0123456", 3, true, StringExtensionMethods.PaddingOptions.PadLeft, ExpectedResult = new string[] { "  0", "123", "456" })]
        [TestCase("0123456", 3, true, StringExtensionMethods.PaddingOptions.PadRight, ExpectedResult = new string[] { "0  ", "123", "456" })]
        [TestCase("abcd", 4, true, StringExtensionMethods.PaddingOptions.PadRight, ExpectedResult = new string[] { "abcd" })]
        [TestCase("abcd", 5, true, StringExtensionMethods.PaddingOptions.PadRight, ExpectedResult = new string[] { "abcd " })]
        [TestCase("abcd", 5, true, StringExtensionMethods.PaddingOptions.PadLeft, ExpectedResult = new string[] { " abcd" })]
        [TestCase("ABCEFFF", 4, true, StringExtensionMethods.PaddingOptions.PadLeft, ExpectedResult = new string[] { " ABC", "EFFF" })]
        [TestCase("1111100110011", 8, true, StringExtensionMethods.PaddingOptions.PadLeft, ExpectedResult = new string[] { "   11111", "00110011" })]
        [TestCase("abcd", 0, true, StringExtensionMethods.PaddingOptions.PadLeft, ExpectedResult = new string[] { " abcd" }, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("", 0, true, StringExtensionMethods.PaddingOptions.PadLeft, ExpectedResult = new string[] { "abcd " }, ExpectedException = typeof(ArgumentException))]
        [TestCase("abcd", 4, true, (StringExtensionMethods.PaddingOptions)10, ExpectedResult = new string[] { "abcd " }, ExpectedException = typeof(InvalidEnumArgumentException))]
        public String[] Split(String Input, Int32 PartSize, Boolean SplitFromEnd, StringExtensionMethods.PaddingOptions PaddingOpt)
        {
            return Input.Split(PartSize, SplitFromEnd, PaddingOpt);
        }
        
        [TestCase(" 123 ", NumberStyles.Integer, ExpectedResult = 123)]
        [TestCase(" 123 ", NumberStyles.None, ExpectedResult = 111)]
        [TestCase(" -123 ", NumberStyles.Integer, ExpectedResult = -123)]
        [TestCase(" -123 ", NumberStyles.AllowTrailingSign, ExpectedResult = 111)]
        [TestCase("123,005", NumberStyles.AllowThousands, ExpectedResult = 123005)]
        [TestCase("123,005", NumberStyles.Integer, ExpectedResult = 111)]
        [TestCase("-123 ", NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign, ExpectedResult = -123)]
        [TestCase(" -123", NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign, ExpectedResult = 111)]
        [TestCase(" -123", NumberStyles.AllowLeadingWhite | NumberStyles.AllowLeadingSign, ExpectedResult = -123)]
        [TestCase("-213,234", NumberStyles.Integer | NumberStyles.AllowThousands, ExpectedResult = -213234)]
        [TestCase("-213,234", NumberStyles.Integer, ExpectedResult = 111)]
        public Int32 TryParseNumber(String Input, NumberStyles StyleNum)
        {
            CultureInfo cul = CultureInfo.InvariantCulture;
            return Input.TryParseNumber(StyleNum, cul, 111);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    public class StringExtensionMethodsTests
    {
        [TestCase("abcdefghijkl", 5, 9, true, false, true, ExpectedResult = "fghi")]
        [TestCase("abcdefghijkl", 5, 9, false, true, false, ExpectedResult = "ghij")]
        [TestCase("abcdefghijkl", 5, 19, true, true, true, ExpectedResult = "fghijkl")]
        [TestCase("abcdefghijkl", 2, 2, true, true, true, ExpectedResult = "c")]
        [TestCase("abcdefghijkl", 2, 3, true, true, true, ExpectedResult = "cd")]
        [TestCase("abcdefghijkl", 2, 3, true, false, true, ExpectedResult = "c")]
        [TestCase("abcdefghijkl", 2, 3, false, false, true, ExpectedResult = "")]
        [TestCase("abcdefghijkl", 2, 3, false, true, true, ExpectedResult = "d")]
        [TestCase("abcdefghijkl", 5, 19, true, true, false, ExpectedResult = "fghijkl", ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(null, 2, 4, true, true, true, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("", 2, 4, true, true, true, ExpectedException = typeof(ArgumentException))]
        [TestCase("abcd", -1, 4, true, true, true, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("abcd", 4, 4, true, true, true, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("abcd", 3, 2, true, true, true, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public String SubstringWithEnd(String Input, Int32 StartIndex, Int32 EndIndex, Boolean IncludeStart, Boolean IncludeEnd, Boolean UntilEnd)
        {
            return Input.SubstringWithEnd(StartIndex, EndIndex, IncludeStart, IncludeEnd, UntilEnd);
        }

        [TestCase("1234", 0, ExpectedResult = "")]
        [TestCase("1234", 2, ExpectedResult = "12")]
        [TestCase("1234", 3, ExpectedResult = "123")]
        [TestCase("1234", 4, ExpectedResult = "1234")]
        [TestCase("1234", 5, ExpectedResult = "1234")]
        [TestCase("1234", -5, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(null, 5, ExpectedException = typeof(ArgumentException))]
        public String LeftSubstring(String Input, Int32 Count)
        {
            return Input.LeftSubstring(Count);
        }

        [TestCase("1234", 0, ExpectedResult = "")]
        [TestCase("1234", 2, ExpectedResult = "34")]
        [TestCase("1234", 3, ExpectedResult = "234")]
        [TestCase("1234", 4, ExpectedResult = "1234")]
        [TestCase("1234", 5, ExpectedResult = "1234")]
        [TestCase("1234", -5, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(null, 5, ExpectedException = typeof(ArgumentException))]
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
        [TestCase("", new string[] { "", "aB", "bc", "Ab" }, StringComparison.OrdinalIgnoreCase, ExpectedException = typeof(ArgumentException))]
        [TestCase("abcd", new string[0] { }, StringComparison.OrdinalIgnoreCase, ExpectedException = typeof(ArgumentException))]
        [TestCase((String)null, new string[] { "", "ab", "bc", "ab" }, StringComparison.OrdinalIgnoreCase, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("abcd", null, StringComparison.OrdinalIgnoreCase, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("abcd", new string[] { "", "aB", "bc", "Ab" }, 7, ExpectedException = typeof(InvalidEnumArgumentException))]

        public Int32 StartsWithOneOf(String Input, String[] Prefixes, StringComparison StrComp)
        {
            return Input.StartsWithOneOf(Prefixes, StrComp);
        }

        [TestCase("12345", new string[]{"", null, "1234567", "123456", "12345", "1234"}, StringComparison.Ordinal, ExpectedResult = 4)]
        [TestCase("12345", new string[] { "", null, "1234567", "123456", "1234" }, StringComparison.Ordinal, ExpectedResult = -1)]
        [TestCase("abcd", new string[] { "1234567", "cD" }, StringComparison.Ordinal, ExpectedResult = -1)]
        [TestCase("abcd", new string[] { "1234567", "cD" }, StringComparison.OrdinalIgnoreCase, ExpectedResult = 1)]
        [TestCase("12345", new string[] { "445", null, "45" }, StringComparison.Ordinal, ExpectedResult = 2)]
        [TestCase((String)null, new string[] { "445", null, "45" }, StringComparison.Ordinal, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("12345", null, StringComparison.Ordinal, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("", new string[] { "445", null, "45" }, StringComparison.Ordinal, ExpectedException = typeof(ArgumentException))]
        [TestCase("12345", new string[0] { }, StringComparison.Ordinal, ExpectedException = typeof(ArgumentException))]
        [TestCase("12345", new string[] { "445", null, "45" }, 6, ExpectedException = typeof(InvalidEnumArgumentException))]
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

        [TestCase("", 0, 100, "abcde", Result = "abcde")]

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

        [TestCase(null, ExpectedResult = false)]
        [TestCase("", ExpectedResult = false)]
        [TestCase(" abc ", ExpectedResult = false)]
        [TestCase(" 123 ", ExpectedResult = false)]
        [TestCase("123", ExpectedResult = true)]
        [TestCase("12.3", ExpectedResult = false)]
        public Boolean IsStringDigitsOnly(String input)
        {
            return input.IsStringDigitsOnly();
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

        [TestCase("abc", 0, new String[3] { "abC", "bcd", "Abc" }, ExpectedResult = false)]
        [TestCase("abc", 1, new String[3] { "abC", "bcd", "Abc" }, ExpectedResult = true)]
        [TestCase("abc", 2, new String[3] { "abC", "bcd", "Abc" }, ExpectedResult = false)]
        [TestCase("abc", 3, new String[3] { "abC", "bcd", "Abc" }, ExpectedResult = true)]
        [TestCase("abc", 4, new String[3] { "abc", "bcd", "abc" }, ExpectedResult = true)]
        [TestCase("Abc", 4, new String[3] { "abc", "bcd", "abc" }, ExpectedResult = false)]
        [TestCase("Abc", 5, new String[3] { "abc", "bcd", "abc" }, ExpectedResult = true)]
        [TestCase((String)null, 5, new String[3] { "abc", (String)null, "" }, ExpectedResult = true)]
        [TestCase("", 5, new String[3] { "abc", (String)null, "" }, ExpectedResult = true)]
        [TestCase("Abc", 5, new String[0] { }, ExpectedException = typeof(ArgumentException))]
        [TestCase("Abc", 5, new String[1] {"ZZZ"}, ExpectedException = typeof(ArgumentException))]
        [TestCase("Abc", 6, new String[3] { "abc", "bcd", "abc" }, ExpectedException = typeof(InvalidEnumArgumentException))]
        public Boolean IsIn(String Input, Int32 StrComp, params String[] Sequence)
        {
            if (Sequence.Length == 1 && Sequence[0] == "ZZZ")
            {
                Sequence = null;
            }
            if (Sequence.IsEmpty())
            {
                IEnumerable<String> temp = Sequence.AsEnumerable();
                return Input.IsIn((StringComparison)StrComp, temp);
            }
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
        [TestCase("Abcd", "ab", 6, ExpectedException = typeof(InvalidEnumArgumentException))]
        public Boolean Contains1(String Input, String ToCheck, Int32 StrComp)
        {
            return Input.Contains(ToCheck, (StringComparison)StrComp);
        }


        [TestCase("Abcd", "ab", 1, "en-US", ExpectedResult = true)]
        [TestCase("Abcd", "ab", 1, null, ExpectedResult = true)]
        [TestCase("Abcd", "ab", 1073741824, "en-US", ExpectedResult = false)]
        [TestCase("ab.cd", "abc", 4, "en-US", ExpectedResult = true)]
        [TestCase("Abcd", "ab", -5, "en-US", ExpectedException = typeof(InvalidEnumArgumentException))]
        [TestCase("", "ab", 1, "en-US", ExpectedException = typeof(ArgumentException))]
        [TestCase("Abcd", null, 1, "en-US", ExpectedException = typeof(ArgumentException))]
        public Boolean Contains2(String Input, String ToCheck, Int32 CompOpt, String CultureName)
        {
            CompareOptions comp_opt = (CompareOptions) CompOpt;
            CultureInfo culture = CultureName == null ? null : CultureInfo.GetCultureInfo(CultureName);
            return Input.Contains(ToCheck, comp_opt, culture);
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
        [TestCase("abcdabc", '_', new char[0] { }, ExpectedResult = "abcdabc")]
        [TestCase("", '_', new char[] { 'a', 'c' }, ExpectedException = typeof(ArgumentException))]
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
        [TestCase("0123456", 9, false, StringExtensionMethods.PaddingOptions.DoNotPad, ExpectedResult = new string[1] { "0123456" })]
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
        [TestCase((String)null, 0, true, StringExtensionMethods.PaddingOptions.PadLeft, ExpectedResult = new string[] { "abcd " }, ExpectedException = typeof(ArgumentNullException))]
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

        [NUnit.Framework.Test]
        public void TryParseNumber()
        {
            Nullable<Int32> output1 = " \r\n ".TryParseNumber<Int32>(NumberStyles.AllowDecimalPoint, null);
            Assert.IsFalse(output1.HasValue);

            Nullable<Byte> output2 = "abcd".TryParseNumber<Byte>(NumberStyles.AllowDecimalPoint, null);
            Assert.IsFalse(output2.HasValue);

            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                Nullable<DateTime> output3 = "1234".TryParseNumber<DateTime>(NumberStyles.Integer, null);
            });

            Nullable<Single> output4 = "12.34".TryParseNumber<Single>(NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            Assert.IsNotNull(output4);
            Assert.AreEqual(12.34f, output4.Value);

            Nullable<Double> output5 = "12.34".TryParseNumber<Double>(NumberStyles.Float, null);
            Assert.IsNotNull(output5);
            Assert.AreEqual(12.34, output5.Value);

            Nullable<Single> output6 = "12.34".TryParseNumber<Single>(NumberStyles.Integer, CultureInfo.InvariantCulture);
            Assert.IsFalse(output6.HasValue);

            Nullable<Double> output7 = "12.34ab".TryParseNumber<Double>(NumberStyles.Float, CultureInfo.InvariantCulture);
            Assert.IsFalse(output7.HasValue);

            Nullable<Decimal> output8 = "-12.34".TryParseNumber<Decimal>(NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            Assert.IsFalse(output8.HasValue);

            Nullable<Byte> output9 = "1234".TryParseNumber<Byte>(NumberStyles.Integer, CultureInfo.InvariantCulture);
            Assert.IsFalse(output9.HasValue);

            Nullable<Byte> output10 = "123".TryParseNumber<Byte>(NumberStyles.None, CultureInfo.InvariantCulture);
            Assert.IsNotNull(output10);
            Assert.AreEqual(123, output10);

            Nullable<SByte> output11 = "128".TryParseNumber<SByte>(NumberStyles.None, CultureInfo.InvariantCulture);
            Assert.IsFalse(output11.HasValue);

            Nullable<SByte> output12 = "-128".TryParseNumber<SByte>(NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
            Assert.IsNotNull(output12);
            Assert.AreEqual(-128, output12);

            Nullable<UInt16> output13 = "-123".TryParseNumber<UInt16>(NumberStyles.Integer, CultureInfo.InvariantCulture);
            Assert.IsFalse(output13.HasValue);

            Nullable<UInt16> output14 = UInt16.MaxValue.ToString().TryParseNumber<UInt16>(NumberStyles.Integer, CultureInfo.InvariantCulture);
            Assert.IsNotNull(output14);
            Assert.AreEqual(UInt16.MaxValue, output14);

            Nullable<Int16> output15 = ((Int32)(Int16.MinValue) - 1).ToString().TryParseNumber<Int16>(NumberStyles.Integer, CultureInfo.InvariantCulture);
            Assert.IsFalse(output15.HasValue);

            Nullable<Int16> output16 = Int16.MinValue.ToString().TryParseNumber<Int16>(NumberStyles.Integer, CultureInfo.InvariantCulture);
            Assert.IsNotNull(output16);
            Assert.AreEqual(Int16.MinValue, output16);

            Nullable<UInt32> output17 = "12.34".TryParseNumber<UInt32>(NumberStyles.Float, CultureInfo.InvariantCulture);
            Assert.IsFalse(output17.HasValue);

            Nullable<UInt32> output18 = "1234".TryParseNumber<UInt32>(NumberStyles.Float, CultureInfo.InvariantCulture);
            Assert.IsNotNull(output18);
            Assert.AreEqual(1234, output18);

            Nullable<Int64> output19 = "-1234".TryParseNumber<Int64>(NumberStyles.None, CultureInfo.InvariantCulture);
            Assert.IsFalse(output19.HasValue);

            Nullable<Int64> output20 = "-1234".TryParseNumber<Int64>(NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
            Assert.IsNotNull(output20);
            Assert.AreEqual(-1234, output20);

            Nullable<UInt64> output21 = (-1).ToString().TryParseNumber<UInt64>(NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
            Assert.IsFalse(output21.HasValue);

            Nullable<UInt64> output22 = UInt64.MaxValue.ToString().TryParseNumber<UInt64>(NumberStyles.None, CultureInfo.InvariantCulture);
            Assert.IsNotNull(output22);
            Assert.AreEqual(UInt64.MaxValue, output22);
        }
    }
}

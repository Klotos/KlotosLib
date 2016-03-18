using System;
using System.Text;
using KlotosLib.StringTools;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class StringBuilderExtensionMethodsTests
    {
        private static StringBuilder FromString(String Input)
        {
            StringBuilder sb = Input == null ? null : new StringBuilder(Input);
            return sb;
        }

        [TestCase("a", Result = false)]
        [TestCase(" ", Result = false)]
        [TestCase("", Result = true)]
        [TestCase(null, Result = true)]
        public Boolean IsNullOrEmpty(String Input)
        {
            return FromString(Input).IsNullOrEmpty();
        }

        [TestCase("", ExpectedResult = false)]
        [TestCase("   ", ExpectedResult = false)]
        [TestCase(" \r\n ", ExpectedResult = false)]
        [TestCase(" \r\n,, :: ”½ ", ExpectedResult = false)]
        [TestCase("//* ++", ExpectedResult = false)]
        [TestCase(" \r\n,, :: œ", ExpectedResult = true)]
        [TestCase(" 123 ", ExpectedResult = true)]
        [TestCase(" 123 a ", ExpectedResult = true)]
        public Boolean HasAlphaNumericCharsTest(String Input)
        {
            return FromString(Input).HasAlphaNumericChars();
        }

        [TestCase("a", Result="")]
        [TestCase(" ", Result = "")]
        [TestCase("", Result = "")]
        [TestCase(null, Result = null)]
        public String Clean(String Input)
        {            
            return FromString(Input).Clean().ToStringS(null);
        }

        [TestCase("abcd", 2, Result = "ab")]
        [TestCase("abcd", 3, Result="abc")]
        [TestCase("abcd", 4, Result = "abcd")]
        [TestCase("abcd", 5, Result = "abcd")]
        [TestCase("abcd", 0, Result = "")]
        [TestCase("", 10, Result = "")]
        [TestCase("abcd", -1, Result = "abcd", ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(null, 10, Result = "", ExpectedException=typeof(ArgumentNullException))]
        public String LeftSubstring(String Input, Int32 Count)
        {
            return FromString(Input).LeftSubstring(Count).ToString();
        }

        [TestCase("abcd", 2, Result = "cd")]
        [TestCase("abcd", 3, Result = "bcd")]
        [TestCase("abcd", 4, Result = "abcd")]
        [TestCase("abcd", 5, Result = "abcd")]
        [TestCase("abcd", 0, Result = "")]
        [TestCase("", 10, Result = "")]
        [TestCase("abcd", -1, Result = "abcd", ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(null, 10, Result = "", ExpectedException = typeof(ArgumentNullException))]
        public String RightSubstring(String Input, Int32 Count)
        {
            return FromString(Input).RightSubstring(Count).ToString();
        }

        [TestCase("abcdef", 2, Result = "cdef")]
        [TestCase("abcdef", 5, Result = "f")]
        [TestCase("abcdef", 6, Result = "")]
        [TestCase("abcdef", 7, Result = "")]
        [TestCase("abcdef", 0, Result = "abcdef")]
        [TestCase("", 7, Result = "")]
        [TestCase("abcdef", -1, Result = "abcdef", ExpectedException=typeof(ArgumentOutOfRangeException))]
        [TestCase(null, 0, Result = "", ExpectedException = typeof(ArgumentNullException))]
        public String CutLeft(String Input, Int32 Count)
        {
            return FromString(Input).CutLeft(Count).ToString();
        }

        [TestCase("abcdef", 2, Result = "abcd")]
        [TestCase("abcdef", 5, Result = "a")]
        [TestCase("abcdef", 6, Result = "")]
        [TestCase("abcdef", 7, Result = "")]
        [TestCase("abcdef", 0, Result = "abcdef")]
        [TestCase("", 7, Result = "")]
        [TestCase("abcdef", -1, Result = "abcdef", ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(null, 0, Result = "", ExpectedException = typeof(ArgumentNullException))]
        public String CutRight(String Input, Int32 Count)
        {
            return FromString(Input).CutRight(Count).ToString();
        }

        [TestCase("abc", Result="abc")]
        [TestCase("abc ", Result = "abc")]
        [TestCase("  abc ", Result = "abc")]
        [TestCase("\r  abc \n", Result = "abc")]
        [TestCase("\r  a  b  c \n", Result = "a  b  c")]
        [TestCase("  a ", Result = "a")]
        [TestCase("   ", Result = "")]
        [TestCase("", Result = "")]
        [TestCase(null, Result = "", ExpectedException=typeof(ArgumentNullException))]
        public String Trim(String Input)
        {
            return FromString(Input).Trim().ToString();
        }

        [TestCase("abcdabcda", "abcd", false, true, Result = "a")]
        [TestCase("abcdabcda", "abcd", false, false, Result = "abcda")]
        [TestCase("AbcDAbCDa", "abCD", true, true, Result = "a")]
        [TestCase("AbcDAbCD", "abCD", true, true, Result = "")]
        [TestCase("AbcDAbCD", "abCD", true, false, Result = "AbCD")]
        [TestCase("AbcDAbCD", "abCD", false, false, Result = "AbcDAbCD")]
        [TestCase("AbcDAbCD", "abCD", false, true, Result = "AbcDAbCD")]
        [TestCase("12345", "", true, false, Result = "12345")]
        [TestCase("", "12345", true, false, Result = "")]
        [TestCase("12345", "123456", true, false, Result = "12345")]
        [TestCase(null, "abCD", true, false, Result = "AbcDAbCD", ExpectedException=typeof(ArgumentNullException))]
        public String TrimStart(String Input, String Start, Boolean IgnoreCase, Boolean Recursive)
        {
            return FromString(Input).TrimStart(Start, IgnoreCase, Recursive).ToString();
        }

        [TestCase("123123123", "123", false, true, Result = "")]
        [TestCase("3123123123", "123", false, true, Result = "3")]
        [TestCase("3123123123", "123", false, false, Result = "3123123")]
        [TestCase("12345", "", true, false, Result = "12345")]
        [TestCase("", "12345", true, false, Result = "")]
        [TestCase("12345", "123456", true, false, Result = "12345")]
        [TestCase(null, "abCD", true, false, Result = "AbcDAbCD", ExpectedException = typeof(ArgumentNullException))]
        [TestCase("abcd", "xy", false, true, ExpectedResult = "abcd")]
        public String TrimEnd(String Input, String Start, Boolean IgnoreCase, Boolean Recursive)
        {
            return FromString(Input).TrimEnd(Start, IgnoreCase, Recursive).ToString();
        }
        
        [TestCase("Input", Result="tupnI")]
        [TestCase("f", Result = "f")]
        [TestCase("ab", Result = "ba")]
        [TestCase("", Result = "")]
        [TestCase(null, Result = null)]
        public String Reverse(String Input)
        {         
            return FromString(Input).Reverse().ToStringS(null);
        }

        [TestCase("0123456789", 3, 3, "insert", Result="012insert6789")]
        [TestCase("0123456789", 0, 3, "abcde", Result = "abcde3456789")]
        [TestCase("0123456789", 5, 4, "abcde", Result = "01234abcde9")]
        [TestCase("0123456789", 5, 0, "abcde", Result = "01234abcde56789")]
        [TestCase("0123456789", 9, 0, "abcde", Result = "012345678abcde9")]
        [TestCase("0123456789", 8, 100, "abcde", Result = "01234567abcde")]
        [TestCase("", 0, 0, "abc", ExpectedResult = "abc")]
        [TestCase(null, 5, 4, "abcde", Result = "", ExpectedException = typeof(ArgumentNullException))]
        [TestCase("0123456789", 5, 4, null, Result = "", ExpectedException = typeof(ArgumentNullException))]
        [TestCase("0123456789", -10, 0, "abcde", Result = "", ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456789", 10, 0, "abcde", Result = "", ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456789", 9, -1, "abcde", Result = "", ExpectedException = typeof(ArgumentOutOfRangeException))]
        public String Stuff(String Input, Int32 StartIndex, Int32 Length, String Replacement)
        {
            return FromString(Input).Stuff(StartIndex, Length, Replacement).ToString();
        }
    }
}

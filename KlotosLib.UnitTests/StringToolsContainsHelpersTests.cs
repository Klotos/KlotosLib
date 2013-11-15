using System;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class StringToolsContainsHelpersTests
    {
        [TestCase("abcdabcd", 'x', 'y', Result = true)]
        [TestCase("abcdabcd", 'x', 'y', 'a', Result = false)]        
        [TestCase("", 'f', Result = false, ExpectedException = typeof(ArgumentException))]
        [TestCase("abcdabcd", new Char[0], Result = false, ExpectedException = typeof(ArgumentException))]
        public Boolean ContainsNoOneOfCharTest(String Input, params Char[] Seek)
        {
            return StringTools.ContainsHelpers.ContainsNoOneOf(Input, Seek);
        }

        [TestCase("abcdabcd", 'a', 'b', Result = true)]
        [TestCase("abcdabcd", 'a', 'b', 'c', 'd', 'e', Result = false)]
        [TestCase("", 'f', Result = false, ExpectedException = typeof(ArgumentException))]
        [TestCase("abcdabcd", new Char[0], Result = false, ExpectedException = typeof(ArgumentException))]
        public Boolean ContainsAllOfCharTest(String Input, params Char[] Seek)
        {
            return StringTools.ContainsHelpers.ContainsAllOf(Input, Seek);
        }

        [TestCase("abcdabcd", 4, "ab", "bcd", Result = true)]
        [TestCase("abcdabcd", 5, "ab", "BCD", Result = true)]
        [TestCase("abcdabcd", 4, "ab", "BCD", Result = false)]
        [TestCase(null, 4, "ab", "BCD", Result = false, ExpectedException = typeof(ArgumentException))]
        [TestCase("abcdabcd", 4, new String[0], Result = false, ExpectedException = typeof(ArgumentException))]
        public Boolean ContainsAllOfStrTest(String Input, Int32 Cmp, params String[] Seek)
        {
            return StringTools.ContainsHelpers.ContainsAllOf(Input, (StringComparison)Cmp, Seek);
        }

        [TestCase("abcdabcd", 'a', 'b', 'c', 'd', Result = true)]
        [TestCase(" abcdabcd", 'a', 'b', 'c', 'd', Result = false)]
        [TestCase("abcd", 'a', 'b', 'c', 'd', 'e', Result = false)]
        [TestCase("abcde", 'a', 'b', 'c', 'd', Result = false)]
        [TestCase("", 'f', Result=false, ExpectedException=typeof(ArgumentException))]
        public Boolean ContainsAllAndOnlyOfTest(String Input, params Char[] Seek)
        {
            return StringTools.ContainsHelpers.ContainsAllAndOnlyOf(Input, Seek);
        }

        [TestCase("ef ef abcd", new Char[4] {'a', 'b', 'c', 'd'}, new Char[6] { 'e', 'f', 'g', 'h', 'i', ' ' }, Result = true)]
        [TestCase("ef ef abcdx", new Char[4] { 'a', 'b', 'c', 'd' }, new Char[6] { 'e', 'f', 'g', 'h', 'i', ' ' }, Result = false)]
        [TestCase("ef ef abcd", new Char[0] { }, new Char[6] { 'e', 'f', 'g', 'h', 'i', ' ' }, Result = true, ExpectedException = typeof(ArgumentException))]
        [TestCase("", new Char[4] { 'a', 'b', 'c', 'd' }, new Char[6] { 'e', 'f', 'g', 'h', 'i', ' ' }, Result = false, ExpectedException = typeof(ArgumentException))]
        public Boolean ContainsAllRequiredAndOnlyAcceptedTest(String Input, Char[] Requred, Char[] Accepted)
        {
            return StringTools.ContainsHelpers.ContainsAllRequiredAndOnlyAccepted(Input, Requred, Accepted);
        }

        [TestCase("111", '1', '2', '3', '4', Result = true)]
        [TestCase(" 111222 ", '1', '2', '3', '4', ' ', 'a', Result = true)]
        [TestCase("1234abcd", '1', '2', '3', '4', '5', Result = false)]
        [TestCase("12345abcdef", '1', '2', Result = false)]
        [TestCase("1234abcd", new Char[0], Result = true, ExpectedException = typeof(ArgumentException))]
        [TestCase("", '1', '2', '3', '4', Result = true, ExpectedException = typeof(ArgumentException))]
        public Boolean ContainsOnlyAllowedTest(String Input, params Char[] Symbols)
        {
            return StringTools.ContainsHelpers.ContainsOnlyAllowed(Input, Symbols);
        }
    }
}

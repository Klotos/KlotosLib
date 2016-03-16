using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class StringToolsStringAnalyzersTests
    {
        [Test]
        public void DefineContainingSymbols()
        {
            String input1 = "abc123 \r\n";
            StringTools.StringAnalyzers.ContainsEntities output1 = StringTools.StringAnalyzers.DefineContainingSymbols(input1);
            StringTools.StringAnalyzers.ContainsEntities expected1 =
                StringTools.StringAnalyzers.ContainsEntities.Spaces |
                StringTools.StringAnalyzers.ContainsEntities.Letters |
                StringTools.StringAnalyzers.ContainsEntities.Digits |
                StringTools.StringAnalyzers.ContainsEntities.Controls;
            Assert.AreEqual(expected1, output1);

            String input2 = null;
            StringTools.StringAnalyzers.ContainsEntities output2 = StringTools.StringAnalyzers.DefineContainingSymbols(input2);
            Assert.AreEqual(StringTools.StringAnalyzers.ContainsEntities.Empty, output2);

            String input3 = "abc123";
            StringTools.StringAnalyzers.ContainsEntities output3 = StringTools.StringAnalyzers.DefineContainingSymbols(input3);
            StringTools.StringAnalyzers.ContainsEntities expected3 =
                StringTools.StringAnalyzers.ContainsEntities.Letters |
                StringTools.StringAnalyzers.ContainsEntities.Digits;
            Assert.AreEqual(expected3, output3);
        }

        [TestCase(null, Result = 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("abc", Result = 1)]
        [TestCase("fffbc", Result = 3)]
        public Int32 GetStartChars(String Input)
        {
            KeyValuePair<Char, Int32> res = StringTools.StringAnalyzers.GetStartChars(Input);
            return res.Value;
        }

        [TestCase(null, 'a', Result = 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("abc", 'a', Result = 1)]
        [TestCase("abc", 'b', Result = 0)]
        [TestCase("abc", 'x', Result = 0)]
        [TestCase("ffbc", 'f', Result = 2)]
        [TestCase("ffbc", 'b', Result = 0)]
        public Int32 StartWithCount(String Input, Char Ch)
        {
            return StringTools.StringAnalyzers.StartWithCount(Input, Ch);
        }

        [TestCase(null, null, 0, Result = 0)]
        [TestCase("ab", null, 0, Result = 3)]
        [TestCase("ab", "abc", 4, Result = 1)]
        [TestCase("abc", "ab", 5, Result = 2)]
        [TestCase("bc", "ABCD", 4, Result = 3)]
        [TestCase("bc", "ABCD", 5, Result = 1)]
        public Byte FindAppearanceBetweenStrings(String Input1, String Input2, Int32 StrComp)
        {
            StringComparison strComp = (StringComparison)StrComp;
            return (Byte)StringTools.StringAnalyzers.FindAppearanceBetweenStrings(Input1, Input2, strComp);
        }

        [TestCase(null, Result = 0)]
        [TestCase("", Result = 0)]
        [TestCase(" ", Result = 1)]
        [TestCase(" \r\n_ ", Result = 4)]
        [TestCase("abcddcba", Result = 4)]
        public Int32 GetNumberOfSymbolsInString(String Input)
        {
            return StringTools.StringAnalyzers.GetNumberOfDistinctSymbolsInString(Input);
        }

        [TestCase(null, Result = 0)]
        [TestCase(" ", Result = 0)]
        [TestCase("abc", Result = 0)]
        [TestCase("123", Result = 3)]
        [TestCase("a1b2c", Result = 2)]
        public UInt16 GetNumberOfDigits(String Input)
        {
            return StringTools.StringAnalyzers.GetNumberOfDigits(Input);
        }

        [TestCase(" ", "", StringComparison.Ordinal, ExpectedException = typeof(ArgumentException))]
        [TestCase(" string ", null, StringComparison.Ordinal, ExpectedException = typeof(ArgumentException))]
        [TestCase(" abba abba abb a", " ab", StringComparison.Ordinal, ExpectedResult = 3)]
        [TestCase("aaa", "a", StringComparison.Ordinal, ExpectedResult = 3)]
        [TestCase("abababab", "baba", StringComparison.Ordinal, ExpectedResult = 1)]
        [TestCase("abababab", "baBa", StringComparison.Ordinal, ExpectedResult = 0)]
        [TestCase("abababab", "baBa", StringComparison.OrdinalIgnoreCase, ExpectedResult = 1)]
        [TestCase("abababababaBABAba", "baBA", StringComparison.Ordinal, ExpectedResult = 1)]
        [TestCase("abababababaBABAba", "baBA", StringComparison.OrdinalIgnoreCase, ExpectedResult = 4)]
        public Int32 GetNumberOfOccurencesInString(String Input, String Seek, StringComparison StrComp)
        {
            return StringTools.StringAnalyzers.GetNumberOfOccurencesInString(Input, Seek, StrComp);
        }

        [Test]
        public void AllIndexesOf_String()
        {
            String input = "aaa hg tre -1AA ";

            List<Int32> pos1 = StringTools.StringAnalyzers.AllIndexesOf(input, "aa", StringComparison.OrdinalIgnoreCase, 0);
            Assert.IsTrue(pos1.EqualsExact<Int32>(new List<int>(2) { 0, 13 }));

            List<Int32> pos2 = StringTools.StringAnalyzers.AllIndexesOf(input, "aa", StringComparison.OrdinalIgnoreCase, 10);
            Assert.IsTrue(pos2.EqualsExact<Int32>(new List<int>(1) { 13 }));

            List<Int32> pos3 = StringTools.StringAnalyzers.AllIndexesOf(input, "hG", StringComparison.Ordinal, 2);
            Assert.IsEmpty(pos3);
        }

        [Test]
        public void AllIndexesOf_Char()
        {
            String input = " 123 567 9 ";

            List<Int32> pos1 = StringTools.StringAnalyzers.AllIndexesOf(input, ' ', 0);
            Assert.IsTrue(pos1.EqualsExact<Int32>(new List<int>(4) { 0, 4, 8, 10 }), "Actual: " + pos1.ConcatToString());

            List<Int32> pos2 = StringTools.StringAnalyzers.AllIndexesOf(input, '3', 3);
            Assert.IsTrue(pos2.EqualsExact<Int32>(new List<int>(1) { 3 }));

            List<Int32> pos3 = StringTools.StringAnalyzers.AllIndexesOf(input, '3', 4);
            Assert.IsEmpty(pos3);
        }

        [Test]
        public void GetCharOccurencesStats()
        {
            String input1 = " abc ab";
            Dictionary<Char, UInt16> res1 = StringTools.StringAnalyzers.GetCharOccurencesStats(input1);
            CollectionAssert.AreEqual(new Dictionary<Char, UInt16>(4){
                    { ' ', 2 },
                    { 'a', 2 },
                    { 'b', 2 },
                    { 'c', 1 }
                }, res1);

            String input2 = "";
            Dictionary<Char, UInt16> res2 = StringTools.StringAnalyzers.GetCharOccurencesStats(input2);
            CollectionAssert.IsEmpty(res2);

            String input3 = "\r\n 333221 \r\n";
            Dictionary<Char, UInt16> res3 = StringTools.StringAnalyzers.GetCharOccurencesStats(input3);
            CollectionAssert.AreEqual(new Dictionary<Char, UInt16>(6){
                    { '\r', 2 },
                    { '\n', 2 },
                    { ' ', 2 },
                    { '3', 3 },
                    { '2', 2 },
                    { '1', 1 },
                }, res3);
        }
        
        [TestCase(null, 1, true, Result = null, ExpectedException = typeof(ArgumentException))]
        [TestCase(" \r\n ", 0, true, Result = null, ExpectedException = typeof(ArgumentException))]
        [TestCase(" \r\n ", 1, false, Result = null)]
        [TestCase(" xxx54s\r ", 0, false, Result = 54)]
        [TestCase("aa11bbb-2 end", 1, false, Result = 2)]
        [TestCase("aa11bbb-2 end", 0, false, Result = 11)]
        public Nullable<UInt32> GetNearestUnsignedIntegerFromString1(String Input, Byte Direction, Boolean RE)
        {
            return StringTools.StringAnalyzers.GetNearestUnsignedIntegerFromString(Input, (StringTools.Direction)Direction, RE);
        }

        [TestCase(" ", 0, Result = null, ExpectedException = typeof(ArgumentException))]
        [TestCase("abcd", 0, Result = null)]
        [TestCase("12abcd", 2, Result = null)]
        [TestCase(" xxx54s\r ", 0, Result = 54)]
        [TestCase("0123456\r\n", 5, Result = 56)]
        [TestCase("0123456\r\n", 0, Result = 123456)]
        [TestCase("0123456\r\n", 1, Result = 123456)]
        [TestCase("0123456+100\r\n", 7, Result = 100)]
        [TestCase("0123456+100\r\n", 9, Result = 0)]
        public Nullable<UInt32> GetNearestUnsignedIntegerFromString2(String Input, Int32 StartPosition)
        {
            Int32 find_position;
            return StringTools.StringAnalyzers.GetNearestUnsignedIntegerFromString(Input, StartPosition, out find_position);
        }

        [Test]
        public void GetPositionsOfTokenInString()
        {
            String input1 = "abababa";
            List<Int32> output1 = StringTools.StringAnalyzers.GetPositionsOfTokenInString(input1, "ba", StringComparison.Ordinal);
            CollectionAssert.AreEqual(new List<Int32>(3) { 1, 3, 5 }, output1);

            String input2 = "abAbabA";
            List<Int32> output2 = StringTools.StringAnalyzers.GetPositionsOfTokenInString(input2, "bA", StringComparison.Ordinal);
            CollectionAssert.AreEqual(new List<Int32>(2) { 1, 5 }, output2);
        }

        [Test]
        public void IndexesOfTemplateFirstOccurence()
        {
            String input1 = "text start abca cadadca   end another text";
            String start_token1 = "start";
            String end_token1 = "end";
            Int32 start_index1 = 5;
            Char[] symbols1 = new Char[5] { 'a', 'b', 'c', 'd', ' ' };
            KeyValuePair<Int32, Int32> res1 = StringTools.StringAnalyzers.IndexesOfTemplateFirstOccurence(input1, start_token1, end_token1, start_index1, symbols1);
            Assert.AreEqual(new KeyValuePair<Int32, Int32>(5, 29), res1);
            Assert.AreNotEqual(new KeyValuePair<Int32, Int32>(5, 28), res1);

            String input2 = "text start abca cadadca   end another text";
            String start_token2 = "start";
            String end_token2 = "end";
            Int32 start_index2 = 6;
            Char[] symbols2 = new Char[5] { 'a', 'b', 'c', 'd', ' ' };
            KeyValuePair<Int32, Int32> res2 = StringTools.StringAnalyzers.IndexesOfTemplateFirstOccurence(input2, start_token2, end_token2, start_index2, symbols2);
            Assert.AreEqual(new KeyValuePair<Int32, Int32>(-1, -1), res2);

        }

        [Test]
        public void IndexesOfTemplate()
        {
            String input1 = "another text start end end start \n_  end start _\n_  endstart END another text";
            String start_token1 = "start";
            String end_token1 = "end";
            Int32 start_index1 = 5;
            Char[] required_symbols1 = new Char[1] { ' ' };
            Char[] optional_symbols1 = new Char[2] { '\n', '_' };
            Dictionary<Int32, Int32> res1 =
                StringTools.StringAnalyzers.IndexesOfTemplate
                (input1, start_token1, end_token1, start_index1, required_symbols1, optional_symbols1, StringComparison.Ordinal);
            CollectionAssert.AreEqual(new Dictionary<Int32, Int32>(3){
                    { 13, 21 },
                    { 27, 39 },
                    { 41, 54 }
                }, res1, "res=" + res1.ConcatToString());

            String input2 = "another text start end end start \n_  end start _\n_  endstart END another text";
            String start_token2 = "start";
            String end_token2 = "end";
            Int32 start_index2 = 5;
            Char[] required_symbols2 = new Char[1] { ' ' };
            Char[] optional_symbols2 = new Char[2] { '\n', '_' };
            Dictionary<Int32, Int32> res2 =
                StringTools.StringAnalyzers.IndexesOfTemplate
                (input2, start_token2, end_token2, start_index2, required_symbols2, optional_symbols2, StringComparison.OrdinalIgnoreCase);
            CollectionAssert.AreEqual(new Dictionary<Int32, Int32>(4){
                    { 13, 21 },
                    { 27, 39 },
                    { 41, 54 },
                    { 55, 63 }
                }, res2, "res=" + res2.ConcatToString());
        }
    }
}

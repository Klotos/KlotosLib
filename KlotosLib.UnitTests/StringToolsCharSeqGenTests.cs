using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KlotosLib.StringTools;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    public class StringToolsCharSeqGenTests
    {
        [TestCase("inpuT", ExpectedResult = "Tinpu")]
        [TestCase("abcdDbac", ExpectedResult = "Dabcd")]
        public String FromString(String Input)
        {
            return new string(StringTools.CharSeqGen.FromString(Input));
        }

        [TestCase(0x29, 0x26, Result = new Char[4] { '&', '\'', '(', ')' })]
        [TestCase(0x26, 0x29, Result = new Char[4] { '&', '\'', '(', ')' })]
        public Char[] FromRangeUInt16(Int32 Start, Int32 End)
        {
            return StringTools.CharSeqGen.FromRange((UInt16)Start, (UInt16)End);
        }

        [TestCase('a', 'd', Result = new Char[4] { 'a', 'b', 'c', 'd' })]
        [TestCase('d', 'a', Result = new Char[4] { 'a', 'b', 'c', 'd' })]
        [TestCase('d', 'd', Result = new Char[1] { 'd' })]
        public Char[] FromRangeChar(Char Start, Char End)
        {
            return StringTools.CharSeqGen.FromRange(Start, End);
        }

        [Test]
        public void DigitsOnly()
        {
            CollectionAssert.AreEqual(new Char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }, StringTools.CharSeqGen.DigitsOnly());
        }

        [Test]
        public void DigitsAndLatinLetters()
        {
            Char[] expected = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            CollectionAssert.AreEqual(expected, StringTools.CharSeqGen.DigitsAndLatinLetters(CharSeqGen.LettersType.OnlyCapitalCase));
        }
    }
}

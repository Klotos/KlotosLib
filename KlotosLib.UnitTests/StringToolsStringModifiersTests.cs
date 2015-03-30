using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class StringToolsStringModifiersTests
    {
        [TestCase(null, Result = null)]
        [TestCase("", Result = "")]
        [TestCase("   ", Result = "   ")]

        [TestCase("+", Result = "+")]
        [TestCase(" - ", Result = " - ")]
        [TestCase("a", Result = "A")]
        [TestCase(" b", Result = " b")]
        [TestCase("abc", Result = "Abc")]
        [TestCase("ABC", Result = "ABC")]
        public String FirstLetterToUpper(String Input)
        {
            return StringTools.StringModifiers.FirstLetterToUpper(Input);
        }
    }
}

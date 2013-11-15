using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    public class StringToolsParsingHelpersTest
    {
        [Test]
        public void ParseInputStringToNumbers()
        {
            const string input1 = " 123 , -45,  456 ";
            List<Int32> output1 = StringTools.ParsingHelpers.ParseInputStringToNumbers<Int32>(input1, ", ", NumberStyles.Integer, null);
            CollectionAssert.AreEqual(output1, new List<Int32>(3){123, -45, 456});

            const string input2 = " 45.56; -78 ; 021.13; 0";
            List<Decimal> output2 = StringTools.ParsingHelpers.ParseInputStringToNumbers<Decimal>(input2, ";", NumberStyles.Float, null);
            CollectionAssert.AreEqual(output2, new List<Decimal>(4) { 45.56m, -78m, 21.13m, 0m });

        }
    }
}

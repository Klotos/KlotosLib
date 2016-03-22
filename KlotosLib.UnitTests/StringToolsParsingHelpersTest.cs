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
            CollectionAssert.AreEqual(new List<Int32>(3) { 123, -45, 456 }, output1);

            const string input2 = " 45.56; -78 ; 021.13; 0";
            List<Decimal> output2 = StringTools.ParsingHelpers.ParseInputStringToNumbers<Decimal>(input2, ";", NumberStyles.Float, null);
            CollectionAssert.AreEqual(new List<Decimal>(4) { 45.56m, -78m, 21.13m, 0m }, output2);

            Assert.Throws<ArgumentException>(delegate
            {
                List<Decimal> output3 = StringTools.ParsingHelpers.ParseInputStringToNumbers<Decimal>(input2, "", NumberStyles.Float, null);
            });

            Assert.Throws<ArgumentException>(delegate
            {
                List<Decimal> output4 = StringTools.ParsingHelpers.ParseInputStringToNumbers<Decimal>(" \r\n \0 ", ";", NumberStyles.Float, null);
            });

            const string input3 = " 123 , -45,  abc, 5 ";
            Assert.Throws<InvalidOperationException>(delegate
            {
                List<Decimal> output5 = StringTools.ParsingHelpers.ParseInputStringToNumbers<Decimal>(input3, ",", NumberStyles.Float, null);
            });

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    public class EnumerationToolsTests
    {
        [TestCase(NumberStyles.AllowLeadingWhite, NumberStyles.AllowTrailingWhite, ExpectedResult = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite)]
        [TestCase(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberStyles.AllowLeadingSign, ExpectedResult = NumberStyles.Integer)]
        public NumberStyles IncludeToTest(NumberStyles Input, NumberStyles Addition)
        {
            return Input.IncludeTo(Addition);
        }

        [TestCase(NumberStyles.Integer, NumberStyles.AllowLeadingSign, ExpectedResult = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite)]
        [TestCase(NumberStyles.Integer, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, ExpectedResult = NumberStyles.AllowLeadingSign)]
        public NumberStyles DeleteFromTest(NumberStyles Input, NumberStyles Deductible)
        {
            return Input.DeleteFrom(Deductible);
        }

        [TestCase(NumberStyles.Integer, NumberStyles.AllowLeadingSign, ExpectedResult = true)]
        [TestCase(NumberStyles.Integer, NumberStyles.Float, ExpectedResult = false)]
        [TestCase(NumberStyles.AllowLeadingWhite, NumberStyles.AllowLeadingWhite, ExpectedResult = true)]
        [TestCase(NumberStyles.AllowLeadingWhite, NumberStyles.Integer, ExpectedResult = false)]
        public Boolean ContainsTest(NumberStyles Input, NumberStyles Item)
        {
            return Input.Contains(Item);
        }

        [TestCase(NumberStyles.Integer, NumberStyles.AllowLeadingSign, ExpectedResult = false)]
        [TestCase(NumberStyles.Integer, NumberStyles.Float, ExpectedResult = true)]
        [TestCase(NumberStyles.AllowLeadingWhite, NumberStyles.AllowLeadingWhite, ExpectedResult = false)]
        [TestCase(NumberStyles.AllowLeadingWhite, NumberStyles.Integer, ExpectedResult = true)]
        public Boolean MissesTest(NumberStyles Input, NumberStyles Item)
        {
            return Input.Misses(Item);
        }
    }
}

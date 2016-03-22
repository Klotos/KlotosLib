using System;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class StringToolsValidatorHelpersTests
    {
        [TestCase(0, new String[]{"ab", "c", " d "}, ExpectedResult = true)]
        [TestCase(0, "ab", "c", "  ", ExpectedResult = false)]
        [TestCase(0, "ab", "c", "", ExpectedResult = false)]
        [TestCase(0, new String[0]{}, ExpectedResult = false)]
        public Boolean AllStringsNotNullEmptyWS(Int32 Dummy, params String[] Input)
        {
            return StringTools.ValidatorHelpers.AllStringsNotNullEmptyWS(Input);
        }

        [TestCase(0, "ab", "c", " d ", ExpectedResult = true)]
        [TestCase(0, "ab", "c", "\r\n", ExpectedResult = null)]
        [TestCase(0, "", "  ", "\n", ExpectedResult = false)]
        [TestCase(0, "ab", ExpectedResult = true)]
        [TestCase(0, new String[0] { }, ExpectedException = typeof(ArgumentException))]
        public Nullable<Boolean> AllStringsHaveVisibleChars(Int32 Dummy, params String[] Input)
        {
            return StringTools.ValidatorHelpers.AllStringsHaveVisibleChars(Input);
        }

        [TestCase((String)null, ExpectedResult = false)]
        [TestCase("", ExpectedResult = false)]
        [TestCase("{42A00C74-BCAB-435E-AFD8-DDE0E12DEAE0}", ExpectedResult = true)]
        [TestCase("{42A00C74-435E-AFD8-DDE0E12DEAE0}", ExpectedResult = false)]
        public Boolean IsValidGuid(String Input)
        {
            return StringTools.ValidatorHelpers.IsValidGuid(Input);
        }

        [TestCase((String)null, Result = false)]
        [TestCase("  ", Result = false)]
        [TestCase("ned80@i.ua", Result = true)]
        [TestCase(" ned80@i.ua  \r\n", Result = false)]
        [TestCase("ned80@ua", Result = true)]
        [TestCase(null, Result = false)]
        [TestCase("  ", Result = false)]
        [TestCase("ned80", Result = false)]
        [TestCase("ned80@1@ua", Result = false)]
        public Boolean IsValidEmail(String Input)
        {
            return StringTools.ValidatorHelpers.IsValidEmail(Input);
        }
    }
}

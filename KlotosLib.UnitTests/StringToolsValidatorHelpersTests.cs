using System;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class StringToolsValidatorHelpersTests
    {
        [TestCase("ab", "c", " d ", ExpectedResult = true)]
        [TestCase("ab", "c", "  ", ExpectedResult = false)]
        [TestCase("ab", "c", "", ExpectedResult = false)]
        public Boolean IsAllStringsNotNullEmptyWS(params String[] Input)
        {
            return StringTools.ValidatorHelpers.IsAllStringsNotNullEmptyWS(Input);
        }

        [TestCase("ab", "c", " d ", ExpectedResult = true)]
        [TestCase("ab", "c", "\r\n", ExpectedResult = null)]
        [TestCase("", "  ", "\n", ExpectedResult = false)]
        public Nullable<Boolean> IsAllStringsHasVisibleChars(params String[] Input)
        {
            return StringTools.ValidatorHelpers.IsAllStringsHasVisibleChars(Input);
        }

        [TestCase("{42A00C74-BCAB-435E-AFD8-DDE0E12DEAE0}", ExpectedResult = true)]
        [TestCase("{42A00C74-435E-AFD8-DDE0E12DEAE0}", ExpectedResult = false)]
        public Boolean IsValidGuid(String Input)
        {
            return StringTools.ValidatorHelpers.IsValidGuid(Input);
        }

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

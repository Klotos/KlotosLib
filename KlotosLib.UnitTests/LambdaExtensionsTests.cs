using System;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class LambdaExtensionsTests
    {
        [Test]
        public void MemberName()
        {
            string strName = null;
            String name_of_strName = strName.MemberName(_ => strName);
            Assert.AreEqual(name_of_strName, "strName");
        }
    }
}

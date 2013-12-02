using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    public class SubstringTest
    {
        [TestCase("0123456", 3, 3, ExpectedResult = "345")]
        [TestCase("0123456", 3, 4, ExpectedResult = "3456")]
        [TestCase("0123456", 0, 4, ExpectedResult = "0123")]
        [TestCase("0123456", 0, 6, ExpectedResult = "012345")]
        [TestCase("0123456", 0, 7, ExpectedResult = "0123456")]
        [TestCase("0123456", 6, 1, ExpectedResult = "6")]
        [TestCase("0123456", 6, 2, ExpectedException = typeof(ArgumentException))]
        [TestCase("0123456", 7, 1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456", 7, 3, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456", 0, 8, ExpectedException = typeof(ArgumentException))]
        [TestCase("0123456", 1, 7, ExpectedException = typeof(ArgumentException))]
        [TestCase("0123456", 2, 0, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456", -1, 5, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456", 0, -3, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("", 3, 3, ExpectedException = typeof(ArgumentException))]
        [TestCase(null, 3, 3, ExpectedException = typeof(ArgumentNullException))]
        public String Construction(String BaseStr, Int32 StartIndex, Int32 Length)
        {
            return new Substring(BaseStr, StartIndex, Length).Value;
        }

        [Test]
        public void Properties()
        {
            string bs1 = "Aaa";
            Substring ss1 = new Substring(bs1, 1);
            string v1 = ss1.Value;
            Assert.AreEqual("aa", v1);
            Assert.IsTrue(Object.ReferenceEquals(v1, ss1.Value));
            Assert.AreEqual(1, ss1.StartIndex);
            Assert.AreEqual(2, ss1.Length);
            Assert.AreEqual(2, ss1.EndIndex);
            Assert.IsTrue(ss1.IsEnding);
            Assert.IsFalse(ss1.IsBeginning);
            Assert.IsFalse(ss1.IsComplete);
            Assert.IsTrue(Object.ReferenceEquals(bs1, ss1.BaseString));
        }

        [Test]
        public void AreEqual()
        {
            Substring s1 = null;
            Substring s2 = null;
            Assert.IsTrue(Substring.AreEqual(s1, s2));
            s2 = new Substring("abc", 1, 1);
            Assert.IsFalse(Substring.AreEqual(s1, s2));
            Assert.IsFalse(Substring.AreEqual(s2, s1));
            Assert.IsTrue(Substring.AreEqual(s1, s1));
            Assert.IsTrue(Substring.AreEqual(s2, s2));
            s1 = s2;
            Assert.IsTrue(Substring.AreEqual(s1, s2));

            String base_str = "xxx";
            Substring s3 = new Substring(base_str, 2);
            Substring s4 = new Substring(base_str, 2);
            Assert.IsTrue(Substring.AreEqual(s3, s4));
            Substring s5 = new Substring(base_str, 1, 2);
            Assert.IsFalse(Substring.AreEqual(s3, s5));
            Assert.IsFalse(Substring.AreEqual(s5, s3));
        }

        [Test]
        public void AreEqual2()
        {
            const string base1 = "abcd";
            Substring s1 = null;
            Substring s2 = null;
            Substring s3 = null;
            Substring s4 = new Substring(base1, 3);
            Substring s5 = s4;
            Substring s6 = new Substring(base1, 3);
            Substring s7 = new Substring(base1, 2);

            Assert.IsTrue(Substring.AreEqual(s1, s2, s3));
            Assert.IsFalse(Substring.AreEqual(s1, s2, s3, s4));
            Assert.IsTrue(Substring.AreEqual(s4, s5));
            Assert.IsTrue(Substring.AreEqual(s4, s5, s6));
            Assert.IsFalse(Substring.AreEqual(s6, s7));
            Assert.IsTrue(Substring.AreEqual(s4, s4, s4, s4));
        }

        [Test]
        public void HaveCommonBaseString()
        {
            Substring s1 = null;
            Substring s2 = null;
            Assert.IsFalse(Substring.HaveCommonBaseString(s1, s2));
            s2 = s1;
            Assert.IsFalse(Substring.HaveCommonBaseString(s1, s2));
            s2 = new Substring("aaa", 0);
            Assert.IsFalse(Substring.HaveCommonBaseString(s1, s2));
            s1 = s2;
            Assert.IsTrue(Substring.HaveCommonBaseString(s1, s2));
            s1 = new Substring("aaa", 0);
            Assert.IsTrue(Substring.HaveCommonBaseString(s1, s2));
            s1 = new Substring("aaa", 1);
            Assert.IsTrue(Substring.HaveCommonBaseString(s1, s2));
            s1 = new Substring("aab", 1);
            Assert.IsFalse(Substring.HaveCommonBaseString(s1, s2));
        }

        [Test]
        public void Compare()
        {
            const string s1 = "aaa";
            const string s2 = s1;

            Substring ss1 = new Substring(s1, 1);
            Substring ss2 = new Substring(s2, 1);
            Assert.IsTrue(ss1 == ss2);
            Assert.IsTrue(ss1 >= ss2);
            Assert.IsTrue(ss1 <= ss2);
            Assert.IsFalse(ss1 != ss2);
            Assert.IsFalse(ss1 > ss2);
            Assert.IsFalse(ss1 < ss2);

            Substring ss3 = new Substring(s2, 1, 1);
            Assert.IsTrue(ss1 != ss3);
            Assert.IsTrue(ss1 > ss3);
            Assert.IsTrue(ss1 >= ss3);
            Assert.IsFalse(ss1 < ss3);
            Assert.IsFalse(ss1 <= ss3);
        }

        [TestCase("aaa", 1, 1, "aaa", 1, 1, 0, ExpectedResult = true)]
        [TestCase("aaa", 1, 1, "aaa", 1, 1, 1, ExpectedResult = false)]
        [TestCase("aaa", 1, 1, "aaa", 1, 1, 2, ExpectedResult = false)]
        [TestCase("aaa", 1, 1, "aaa", 1, 1, 3, ExpectedResult = false)]
        [TestCase("aaa", 1, 1, "aaa", 1, 1, 4, ExpectedResult = true)]
        [TestCase("aaa", 1, 1, "aaa", 1, 1, 5, ExpectedResult = true)]

        [TestCase("aaa", 1, 2, "aaa", 1, 1, 0, ExpectedResult = false)]
        [TestCase("aaa", 1, 2, "aaa", 1, 1, 1, ExpectedResult = true)]
        [TestCase("aaa", 1, 2, "aaa", 1, 1, 2, ExpectedResult = true)]
        [TestCase("aaa", 1, 2, "aaa", 1, 1, 3, ExpectedResult = false)]
        [TestCase("aaa", 1, 2, "aaa", 1, 1, 4, ExpectedResult = true)]
        [TestCase("aaa", 1, 2, "aaa", 1, 1, 5, ExpectedResult = false)]

        [TestCase("aaa", 1, 2, "aaa", 0, 2, 0, ExpectedResult = false)]
        [TestCase("aaa", 1, 2, "aaa", 0, 2, 1, ExpectedResult = true)]
        [TestCase("aaa", 1, 2, "aaa", 0, 2, 2, ExpectedResult = true)]
        [TestCase("aaa", 1, 2, "aaa", 0, 2, 3, ExpectedResult = false)]
        [TestCase("aaa", 1, 2, "aaa", 0, 2, 4, ExpectedResult = true)]
        [TestCase("aaa", 1, 2, "aaa", 0, 2, 5, ExpectedResult = false)]

        [TestCase("aaa", 1, 2, "Aaa", 1, 2, 0, ExpectedResult = false)]
        [TestCase("aaa", 1, 2, "Aaa", 1, 2, 1, ExpectedResult = true)]
        [TestCase("aaa", 1, 2, "Aaa", 1, 2, 2, ExpectedResult = true)]
        [TestCase("aaa", 1, 2, "Aaa", 1, 2, 3, ExpectedResult = false)]
        [TestCase("aaa", 1, 2, "Aaa", 1, 2, 4, ExpectedResult = true)]
        [TestCase("aaa", 1, 2, "Aaa", 1, 2, 5, ExpectedResult = false)]
        public Boolean Compare(String baseStr1, int si1, int len1, String baseStr2, int si2, int len2, byte op)
        {
            String temp = baseStr1.Equals(baseStr2, StringComparison.Ordinal) == true ? baseStr1 : baseStr2;
            Substring ss1 = new Substring(baseStr1, si1, len1);
            Substring ss2 = new Substring(temp, si2, len2);
            switch (op)
            {
                case 0:
                    return ss1 == ss2;
                case 1:
                    return ss1 != ss2;
                case 2:
                    return ss1 > ss2;
                case 3:
                    return ss1 < ss2;
                case 4:
                    return ss1 >= ss2;
                case 5:
                    return ss1 <= ss2;
                default:
                    throw new ArgumentOutOfRangeException("op");
            }
        }
    }
}

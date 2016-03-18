using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using KlotosLib.StringTools;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    public class SubstringTest
    {
        [TestCase("0123456", 3, ExpectedResult = "3456")]
        [TestCase("0123456", 1, ExpectedResult = "123456")]
        [TestCase("0123456", 0, ExpectedResult = "0123456")]
        [TestCase("0123456", 6, ExpectedResult = "6")]
        [TestCase("0123456", 7, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456", -1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(null, 2, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("", 2, ExpectedException = typeof(ArgumentException))]
        public String FromIndexToEnd(String BaseStr, Int32 StartIndex)
        {
            return Substring.FromIndexToEnd(BaseStr, StartIndex).Value;
        }

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
        public String FromIndexWithLength(String BaseStr, Int32 StartIndex, Int32 Length)
        {
            return Substring.FromIndexWithLength(BaseStr, StartIndex, Length).Value;
        }

        [TestCase("0123456", 3, 3, ExpectedResult = "3")]
        [TestCase("0123456", 3, 6, ExpectedResult = "3456")]
        [TestCase("0123456", 0, 6, ExpectedResult = "0123456")]
        [TestCase("0123456", 6, 6, ExpectedResult = "6")]
        [TestCase("0123456", 0, 0, ExpectedResult = "0")]
        [TestCase("0123456", 5, 6, ExpectedResult = "56")]
        [TestCase(null, 5, 6, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("", 5, 6, ExpectedException = typeof(ArgumentException))]
        [TestCase("0123456", -1, 6, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456", 2, -1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456", 3, 2, ExpectedException = typeof(ArgumentException))]
        [TestCase("0123456", 7, 7, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456", 6, 7, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public String FromIndexToIndex(String BaseStr, Int32 StartIndex, Int32 EndIndex)
        {
            return Substring.FromIndexToIndex(BaseStr, StartIndex, EndIndex).Value;
        }

        [TestCase("0123456", 3, ExpectedResult = "0123")]
        [TestCase("0123456", 6, ExpectedResult = "0123456")]
        [TestCase("0123456", 0, ExpectedResult = "0")]
        [TestCase(null, 2, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("", 2, ExpectedException = typeof(ArgumentException))]
        [TestCase("0123456", -1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456", 7, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public String FromStartToIndex(String BaseStr, Int32 EndIndex)
        {
            return Substring.FromStartToIndex(BaseStr, EndIndex).Value;
        }

        [Test]
        public void Properties()
        {
            string bs1 = "Aaa";
            Substring ss1 = Substring.FromIndexToEnd(bs1, 1);
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
            s2 = Substring.FromIndexWithLength("abc", 1, 1);
            Assert.IsFalse(Substring.AreEqual(s1, s2));
            Assert.IsFalse(Substring.AreEqual(s2, s1));
            Assert.IsTrue(Substring.AreEqual(s1, s1));
            Assert.IsTrue(Substring.AreEqual(s2, s2));
            s1 = s2;
            Assert.IsTrue(Substring.AreEqual(s1, s2));

            String base_str = "xxx";
            Substring s3 = Substring.FromIndexToEnd(base_str, 2);
            Substring s4 = Substring.FromIndexToEnd(base_str, 2);
            Assert.IsTrue(Substring.AreEqual(s3, s4));
            Substring s5 = Substring.FromIndexWithLength(base_str, 1, 2);
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
            Substring s4 = Substring.FromIndexToEnd(base1, 3);
            Substring s5 = s4;
            Substring s6 = Substring.FromIndexToEnd(base1, 3);
            Substring s7 = Substring.FromIndexToEnd(base1, 2);
            Substring s8 = Substring.FromIndexToEnd(base1, 2);
            Substring s9 = Substring.FromIndexWithLength(base1, 2, 2);
            Substring s10 = Substring.FromIndexToIndex(base1, 2, 3);

            Assert.IsTrue(Substring.AreEqual(s1, s2, s3));
            Assert.IsFalse(Substring.AreEqual(s1, s2, s3, s4));
            Assert.IsTrue(Substring.AreEqual(s4, s5));
            Assert.IsTrue(Substring.AreEqual(s4, s5, s6));
            Assert.IsFalse(Substring.AreEqual(s6, s7));
            Assert.IsTrue(Substring.AreEqual(s4, s4, s4, s4));
            Assert.IsTrue(Substring.AreEqual(s7, s8, s9, s10));
            Assert.IsFalse(Substring.AreEqual(s6, s7, s8, s9, s10));

            Substring[] nulls = null;
            Assert.Throws<ArgumentNullException>(delegate { Substring.AreEqual(nulls); });

            Substring[] empty = new Substring[0];
            Assert.Throws<ArgumentException>(delegate { Substring.AreEqual(empty); });

            Substring[] one = new Substring[1] { s4 };
            Assert.Throws<ArgumentException>(delegate { Substring.AreEqual(one); });

            Substring[] two = new Substring[2] { s4, s6 };
            Assert.IsTrue(Substring.AreEqual(two));
        }

        [Test]
        public void AreEqual3()
        {
            const string base1 = "abcd";
            Substring s1 = Substring.FromIndexWithLength(base1, 1, 2);
            Substring s2 = s1;
            Substring s3 = Substring.FromIndexToIndex(base1, 1, 2);
            
            Assert.IsFalse(s1.Equals((Object)null));
            Assert.IsTrue(s1.Equals((Object)s2));
            Assert.IsFalse(s1.Equals("abcd"));
            Assert.IsTrue(s1.Equals((Object)s3));
        }

        [Test]
        public void HaveCommonBaseString()
        {
            const String base_str1 = "aaa";
            const String base_str2 = "aab";
            Substring s1 = null;
            Substring s2 = null;
            Assert.IsFalse(Substring.HaveCommonBaseString(s1, s2));
            s2 = s1;
            Assert.IsFalse(Substring.HaveCommonBaseString(s1, s2));
            s2 = Substring.FromIndexToEnd(base_str1, 0);
            Assert.IsFalse(Substring.HaveCommonBaseString(s1, s2));
            s1 = s2;
            Assert.IsTrue(Substring.HaveCommonBaseString(s1, s2));
            s1 = Substring.FromIndexToEnd(base_str1, 0);
            Assert.IsTrue(Substring.HaveCommonBaseString(s1, s2));
            s1 = Substring.FromIndexToEnd(base_str1, 1);
            Assert.IsTrue(Substring.HaveCommonBaseString(s1, s2));
            s1 = Substring.FromIndexToEnd(base_str2, 1);
            Assert.IsFalse(Substring.HaveCommonBaseString(s1, s2));

            const String base_str3 = "aaaa";
            Substring s3 = Substring.FromIndexToEnd(base_str1, 1);
            Substring s4 = Substring.FromIndexToEnd(base_str1, 2);
            Substring s5 = Substring.FromStartToIndex(base_str1, 2);
            Substring s6 = Substring.FromStartToIndex(base_str3, 2);
            Substring s7 = Substring.FromStartToIndex(base_str3, 0);
            Substring s8 = Substring.FromIndexToIndex(base_str3, 3, 3);
            Assert.IsTrue(Substring.HaveCommonBaseString(s3, s4, s5));
            Assert.IsTrue(Substring.HaveCommonBaseString(s6, s7, s8));
            Assert.IsFalse(Substring.HaveCommonBaseString(s3, s4, s5, s6, s7, s8));
            Assert.IsFalse(Substring.HaveCommonBaseString(new Substring[2]{s5, s6}));
            Assert.Throws<ArgumentNullException>(delegate { Boolean res = Substring.HaveCommonBaseString(null); });
            Assert.Throws<ArgumentException>(delegate { Boolean res = Substring.HaveCommonBaseString(new Substring[0] { }); });
            Assert.Throws<ArgumentException>(delegate { Boolean res = Substring.HaveCommonBaseString(s3); });
        }

        [Test]
        public void Compare()
        {
            const string s1 = "aaabbb";
            const string s2 = s1;

            Substring ss1 = Substring.FromIndexToEnd(s1, 1);
            Substring ss2 = Substring.FromIndexToEnd(s2, 1);
            Substring null_ss = null;
            Assert.IsTrue(ss2 > null_ss);
            Assert.IsTrue(null_ss < ss1);
            Assert.IsTrue(ss1 == ss2);
            Assert.IsTrue(ss1 >= ss2);
            Assert.IsTrue(ss1 <= ss2);
            Assert.IsFalse(ss1 != ss2);
            Assert.IsFalse(ss1 > ss2);
            Assert.IsFalse(ss1 < ss2);

            Substring ss3 = Substring.FromIndexWithLength(s2, 1, 1);
            Assert.IsTrue(ss1 != ss3);
            Assert.IsTrue(ss1 > ss3);
            Assert.IsTrue(ss1 >= ss3);
            Assert.IsFalse(ss1 < ss3);
            Assert.IsFalse(ss1 <= ss3);

            Substring ss4 = Substring.FromIndexWithLength("xyz123", 3, 2);
            Substring ss5 = Substring.FromIndexWithLength("abc123", 3, 1);
            Assert.Less(ss5, ss4);
            
        }

        [TestCase("0123456", 0, 2, "0123456", 0, 2, ExpectedResult = 0)]
        [TestCase(null, 0, 2, "0123456", 0, 2, ExpectedResult = -1)]
        [TestCase("0123456", 0, 2, null, 0, 2, ExpectedResult = 1)]
        [TestCase("0123456", 1, 3, "0123456789", 1, 3, ExpectedResult = -1)]
        [TestCase("0123456789", 1, 3, "0123456", 1, 3, ExpectedResult = 1)]
        [TestCase("0123456789", 1, 3, "0123456789", 1, 3, ExpectedResult = 0)]
        public Int32 Compare(String BaseStr1, Int32 StartIndex1, Int32 EndIndex1,
            String BaseStr2, Int32 StartIndex2, Int32 EndIndex2)
        {
            Substring s1 = BaseStr1 == null ? null : Substring.FromIndexToIndex(BaseStr1, StartIndex1, EndIndex1);
            Substring s2 = BaseStr2 == null ? null : Substring.FromIndexToIndex(BaseStr2, StartIndex2, EndIndex2);
            return Substring.Compare(s1, s2);
        }

        [Test]
        public void CompareWithExceptions()
        {
            Substring substring = Substring.FromIndexToEnd("abcd", 2);
            NUnit.Framework.TestDelegate comparison1 = delegate
            {
                const String x = "vvv";
                Int32 result = substring.CompareTo(x);
            };
            NUnit.Framework.TestDelegate comparison2 = delegate
            {
                Object x = null;
                Int32 result = substring.CompareTo(x);
            };
            Assert.Throws(typeof(InvalidOperationException), comparison1);
            Assert.Throws(typeof(ArgumentNullException), comparison2);
            Assert.AreEqual(0, substring.CompareTo((Object)substring));
            Assert.AreEqual(0, substring.CompareTo((Substring)substring));
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

        [TestCase("aaa", 1, 2, "Aaa", 1, 2, 6, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public Boolean Compare(String baseStr1, int si1, int len1, String baseStr2, int si2, int len2, byte op)
        {
            String temp = baseStr1.Equals(baseStr2, StringComparison.Ordinal) == true ? baseStr1 : baseStr2;
            Substring ss1 = Substring.FromIndexWithLength(baseStr1, si1, len1);
            Substring ss2 = Substring.FromIndexWithLength(temp, si2, len2);
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

        [Test]
        public void Clone()
        {
            Substring orig = Substring.FromIndexToEnd("0123", 1);
            Substring clone = orig.Clone();
            Assert.IsFalse(Object.ReferenceEquals(orig, clone));
            Assert.IsTrue(Substring.AreEqual(orig, clone));

            Object clone2 = ((ICloneable) orig).Clone();
            Assert.IsInstanceOf<Substring>(clone2);
            Assert.IsTrue(orig.Equals(clone2));
        }

        [TestCase("0123456", 5, 2, 1, ExpectedResult = '6')]
        [TestCase("0123456", 5, 2, 0, ExpectedResult = '5')]
        [TestCase("0123456", 5, 2, -1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456", 5, 2, 2, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public Char IndexerTest(String BaseStr, Int32 StartIndex, Int32 Length, Int32 RequestedIndexInSubstring)
        {
            Substring s = Substring.FromIndexWithLength(BaseStr, StartIndex, Length);
            return s[RequestedIndexInSubstring];
        }

        [Test]
        public void ToCharArrayFull()
        {
            Substring s1 = Substring.FromIndexWithLength("0123456", 5, 2);
            CollectionAssert.AreEqual(new Char[2]{'5', '6'}, s1.ToCharArray());
            Substring s2 = Substring.FromStartToIndex("0123456", 3);
            CollectionAssert.AreEqual(new Char[4] { '0', '1', '2', '3' }, s2.ToCharArray());
        }

        [Test]
        public void ToCharArrayWithRange()
        {
            Substring s1 = Substring.FromIndexWithLength("0123456", 1, 6);
            Assert.AreEqual("123456", s1.ToString());
            CollectionAssert.AreEqual(new Char[3] { '3', '4', '5' }, s1.ToCharArray(2, 3));
        }

        [TestCase("0123456", 1, 6, 2, 3, ExpectedResult = new Char[3] { '3', '4', '5' })]
        [TestCase("0123456", 2, 5, 2, 3, ExpectedResult = new Char[3] { '4', '5', '6' })]
        [TestCase("0123456", 2, 5, 2, 0, ExpectedResult = new Char[0] {  })]
        [TestCase("0123456", 2, 5, -1, 3, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456", 2, 5, 5, 3, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456", 2, 5, 2, -1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase("0123456", 2, 5, 2, 4, ExpectedException = typeof(ArgumentException))]
        public Char[] ToCharArrayWithRange(String BaseStr, 
            Int32 StartIndex, Int32 Length, 
            Int32 RequestedIndexInSubstring, Int32 RequestedLength)
        {
            Substring s = Substring.FromIndexWithLength(BaseStr, StartIndex, Length);
            Char[] output = s.ToCharArray(RequestedIndexInSubstring, RequestedLength);
            return output;
        }

        [Test]
        public void GetHashCodeTest()
        {
            unchecked
            {
                const String base_str = "0123456";
                Int32 base_hashcode = base_str.GetHashCode();
                Int32 start_index = 3, length = 3;
                Substring s = Substring.FromIndexWithLength(base_str, start_index, length);
                Int32 expected_hashcode = (base_hashcode * 397) ^ start_index;
                expected_hashcode = (expected_hashcode * 397) ^ length;
                Assert.AreEqual(expected_hashcode, s.GetHashCode());
            }
        }

        [Test]
        public void CharEnumeration()
        {
            const String base_str = "0123456";
            Substring s = Substring.FromIndexToIndex(base_str, 2, 5);
            Assert.AreEqual(4, s.Length);

            Char[] actual = new char[s.Length];
            Int32 index = 0;
            foreach (Char c in s)
            {
                actual[index++] = c;
            }
            CollectionAssert.AreEqual(new Char[4]{'2', '3', '4', '5'}, actual);

            Substring.SubstringCharEnumerator original = s.GetEnumerator();

            Assert.Throws<InvalidOperationException>(delegate { Char c = original.Current; });

            Assert.IsTrue(original.MoveNext());
            Assert.AreEqual('2', original.Current);
            Assert.IsTrue(original.MoveNext());
            Assert.AreEqual('3', original.Current);
            Assert.IsTrue(original.MoveNext());
            Assert.AreEqual('4', original.Current);
            Assert.IsTrue(original.MoveNext());
            Assert.AreEqual('5', original.Current);

            Assert.IsFalse(original.MoveNext());
            Assert.Throws<InvalidOperationException>(delegate { Char c = original.Current; });

            original.Reset();

            Assert.Throws<InvalidOperationException>(delegate { Char c = original.Current; });

            Substring.SubstringCharEnumerator clone1 = original.Clone();
            
            Assert.Throws<InvalidOperationException>(delegate { Char c = clone1.Current; });
            Object clone2 = ((ICloneable) clone1).Clone();
            Assert.IsInstanceOf<Substring.SubstringCharEnumerator>(clone2);

            Assert.IsTrue(original.MoveNext());
            Assert.DoesNotThrow(delegate { Char c = original.Current; });
            Assert.AreEqual('2', original.Current);

            Assert.Throws<InvalidOperationException>(delegate { Char c = clone1.Current; });
            Assert.Throws<InvalidOperationException>(delegate { Char c = ((Substring.SubstringCharEnumerator)clone2).Current; });

            Assert.IsTrue(clone1.MoveNext());
            Assert.DoesNotThrow(delegate { Char c = clone1.Current; });
            Assert.AreEqual('2', clone1.Current);
            System.Collections.IEnumerator casted = (System.Collections.IEnumerator)clone1;
            Object uncastedChar = casted.Current;
            Assert.IsNotNull(uncastedChar);
            Assert.IsInstanceOf<Char>(uncastedChar);
            Assert.AreEqual('2', (Char)uncastedChar);
        }

        [TestCase("0123456", 0, 0, ExpectedResult = new Char[1] { '0' })]
        [TestCase("0123456", 2, 5, ExpectedResult = new Char[4] { '2', '3', '4', '5' })]
        [TestCase("0123456", 2, 6, ExpectedResult = new Char[5] { '2', '3', '4', '5', '6' })]
        [TestCase("0123456", 0, 6, ExpectedResult = new Char[7] { '0', '1', '2', '3', '4', '5', '6' })]
        public Char[] CharEnumeration(String BaseStr, Int32 StartIndex, Int32 EndIndex)
        {
            Substring s = Substring.FromIndexToIndex(BaseStr, StartIndex, EndIndex);
            Char[] actual = new char[s.Length];
            Int32 index = 0;
            foreach (Char c in s)
            {
                actual[index++] = c;
            }
            return actual;
        }

        [TestCase("0123456", 0, 0, "abc", ExpectedResult = "abc123456")]
        [TestCase("0123456", 0, 1, null, ExpectedResult = "23456")]
        [TestCase("0123456", 0, 1, "", ExpectedResult = "23456")]
        [TestCase("0123456", 0, 6, "abc", ExpectedResult = "abc")]
        [TestCase("0123456", 0, 6, "", ExpectedResult = "")]
        [TestCase("0123456", 4, 6, "", ExpectedResult = "0123")]
        [TestCase("0123456", 4, 6, "abc", ExpectedResult = "0123abc")]
        [TestCase("0123456", 4, 4, "abc", ExpectedResult = "0123abc56")]
        [TestCase("0123456", 4, 4, null, ExpectedResult = "012356")]
        public String ReplaceInBaseString(String BaseStr, Int32 StartIndex, Int32 EndIndex,
            String StringToReplace)
        {
            Substring s = Substring.FromIndexToIndex(BaseStr, StartIndex, EndIndex);
            return s.ReplaceInBaseString(StringToReplace);
        }

        [TestCase("0123456", 0, 2, "0123456", 0, 2, ExpectedResult = true)]
        [TestCase("0123456", 0, 2, "0123456", 0, 3, ExpectedResult = true)]
        [TestCase("0123456", 0, 2, "0123456", 2, 3, ExpectedResult = true)]
        [TestCase("0123456", 0, 2, "0123456", 3, 6, ExpectedResult = false)]
        [TestCase("0123456", 0, 0, "0123456", 6, 6, ExpectedResult = false)]
        [TestCase("0123456", 2, 3, "0123456", 5, 6, ExpectedResult = false)]
        [TestCase("0123456", 5, 6, "0123456", 2, 3, ExpectedResult = false)]
        [TestCase("0123456", 4, 6, "0123456", 1, 4, ExpectedResult = true)]
        [TestCase("0123456", 4, 6, "0123456", 1, 3, ExpectedResult = false)]
        [TestCase("0123456", 1, 3, "0123456", 2, 4, ExpectedResult = true)]
        [TestCase("0123456", 2, 4, "0123456", 1, 3, ExpectedResult = true)]
        [TestCase("0123456", 2, 4, "0123456", 2, 2, ExpectedResult = true)]
        [TestCase("0123456", 1, 1, "0123456", 2, 4, ExpectedResult = false)]
        [TestCase("0123456", 2, 4, "0123456", 1, 1, ExpectedResult = false)]
        [TestCase("0123456", 1, 3, "01234567", 2, 4, ExpectedException = typeof(ArgumentException))]
        [TestCase("0123456", 1, 3, null, 2, 4, ExpectedException = typeof(ArgumentNullException))]
        [TestCase(null, 1, 3, "0123456", 2, 4, ExpectedException = typeof(ArgumentNullException))]
        public Boolean AreIntersecting(String BaseStr1, Int32 StartIndex1, Int32 EndIndex1,
            String BaseStr2, Int32 StartIndex2, Int32 EndIndex2)
        {
            if (BaseStr1 != null) { BaseStr1 = String.Intern(BaseStr1);}
            if (BaseStr2 != null) { BaseStr2 = String.Intern(BaseStr2); }
            Substring s1 = BaseStr1 == null ? null : Substring.FromIndexToIndex(BaseStr1, StartIndex1, EndIndex1);
            Substring s2 = BaseStr2 == null ? null : Substring.FromIndexToIndex(BaseStr2, StartIndex2, EndIndex2);
            return Substring.AreIntersecting(s1, s2);
        }
    }
}

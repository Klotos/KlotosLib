using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class CommonToolsTests
    {
        [Test]
        public void IsIn()
        {
            Char[] array1 = new Char[5] { 'a', 'b', 'c', 'd', 'e' };
            Char[] array2 = null;
            Char[] array3 = new Char[0];
            Char value1 = 'a';
            Char value2 = 'x';
            Assert.AreEqual(true, value1.IsIn(array1));
            Assert.AreEqual(false, value2.IsIn(array1));
            Assert.AreEqual(false, value1.IsIn(array3));
            Assert.Throws<ArgumentNullException>(() => value1.IsIn(array2));

            String value4 = "ab";
            String value5 = "AB";
            String value6 = " ";
            String value7 = null;
            List<String> list1 = new List<string>(6) { "ab", "cd", "ef", "", null, "xx" };
            Assert.AreEqual(true, value4.IsIn(list1));
            Assert.AreEqual(true, value4.IsIn(list1.ToArray()));
            Assert.AreEqual(false, value5.IsIn(list1));
            Assert.AreEqual(false, value5.IsIn(list1.ToArray()));
            Assert.AreEqual(false, value6.IsIn(list1));
            Assert.AreEqual(true, value7.IsIn(list1));
            Assert.AreEqual(true, value7.IsIn(list1.ToArray()));

            list1 = null;
            Assert.Throws<ArgumentNullException>(() => value4.IsIn(list1));
            Assert.Throws<ArgumentNullException>(() => value4.IsIn(list1.ToArray<String>()));
        }

        [TestCase(5, 4, 6, true, Result=true)]
        [TestCase(5, 5, 6, true, Result = true)]
        [TestCase(5, 5, 6, false, Result = false)]
        [TestCase(4, 8, 2, false, Result = false)]
        [TestCase(4, 2, 8, false, Result = true)]
        [TestCase(5, 5, 5, false, Result = false)]
        [TestCase(5, 5, 5, true, Result = true)]
        [TestCase(25, 6, 3, true, Result = false)]
        public Boolean IsBetweenInt32(Int32 Actual, Int32 Lower, Int32 Upper, Boolean IncludeBounds)
        {
            return Actual.IsBetween(Lower, Upper, IncludeBounds);
        }

        [TestCase(5, 4, 6, true, Result = true)]
        [TestCase(5, 5, 6, true, Result = true)]
        [TestCase(5, 5, 6, false, Result = false)]
        [TestCase(4, 8, 2, false, Result = true)]
        [TestCase(4, 2, 8, false, Result = true)]
        [TestCase(5, 5, 5, false, Result = false)]
        [TestCase(5, 5, 5, true, Result = true)]
        [TestCase(25, 6, 3, true, Result = false)]
        public Boolean IsBetweenIBInt32(Int32 Actual, Int32 First, Int32 Second, Boolean IncludeBounds)
        {
            return Actual.IsBetweenIB(First, Second, IncludeBounds);
        }

        [TestCase("d", "ab", "d", true, Result = true)]
        [TestCase("d", "ab", "d", false, Result = false)]
        [TestCase("d", "x", "b", false, Result = false)]
        [TestCase("d", "x", "y", false, Result = false)]
        [TestCase("d", null, "df", false, Result = false, ExpectedException=typeof(ArgumentNullException))]
        [TestCase(null, null, "df", false, Result = false, ExpectedException = typeof(ArgumentNullException))]
        public Boolean IsBetweenString(String Actual, String Lower, String Upper, Boolean IncludeBounds)
        {
            return Actual.IsBetween<String>(Lower, Upper, IncludeBounds);
        }

        [TestCase("d", "ab", "d", true, Result = true)]
        [TestCase("d", "ab", "d", false, Result = false)]
        [TestCase("d", "x", "b", false, Result = true)]
        [TestCase("d", "x", "y", false, Result = false)]
        [TestCase("d", null, "df", false, Result = false, ExpectedException = typeof(ArgumentNullException))]
        [TestCase(null, null, "df", false, Result = false, ExpectedException = typeof(ArgumentNullException))]
        public Boolean IsBetweenIBString(String Actual, String First, String Second, Boolean IncludeBounds)
        {
            return Actual.IsBetweenIB<String>(First, Second, IncludeBounds);
        }

        [TestCase(true, false, true, true, Result=true)]
        [TestCase(true, false, false, true, Result = false)]
        [TestCase(true, false, false, false, Result = false)]
        public Boolean IsBetweenBoolean(Boolean Actual, Boolean First, Boolean Second, Boolean IncludeBounds)
        {
            return Actual.IsBetween<Boolean>(First, Second, IncludeBounds);
        }

        [TestCase(true, false, true, true, Result = true)]
        [TestCase(true, false, false, true, Result = false)]
        [TestCase(true, false, false, false, Result = false)]
        public Boolean IsBetweenIBBoolean(Boolean Actual, Boolean First, Boolean Second, Boolean IncludeBounds)
        {
            return Actual.IsBetweenIB<Boolean>(First, Second, IncludeBounds);
        }

        [TestCase(null, Result = null)]
        [TestCase(true, Result = true)]
        [TestCase(false, Result = false)]
        [TestCase("false", Result = false)]
        [TestCase("TRUE", Result = true)]
        [TestCase("xxx", Result = null)]
        [TestCase(" +\r\n ", Result = true)]
        [TestCase(" \r\n 0  ", Result = false)]
        public Nullable<Boolean> TryParse(Object Source)
        {
            return CommonTools.TryParse(Source);
        }

        [TestCase(true, 0, Result = "True")]
        [TestCase(false, 0, Result = "False")]
        [TestCase(true, 1, Result = "true")]
        [TestCase(false, 1, Result = "false")]
        [TestCase(true, 2, Result = "1")]
        [TestCase(false, 2, Result = "0")]
        [TestCase(true, 3, Result = "1")]
        [TestCase(false, 3, Result = "-1")]
        [TestCase(true, 4, Result = "+")]
        [TestCase(false, 4, Result = "-")]
        public String BooleanToString(Boolean Value, Byte Option)
        {
            return Value.ToString((CommonTools.BooleanStr)Option);
        }            

        [TestCase(true, "null", "fail", Result = "True")]
        [TestCase(null, "null", "fail", Result = "null")]
        [TestCase(new Char[2] { 'a', 'b'}, "null", "fail", Result = "System.Char[]")]        
        public String ToStringS(Object Input, String IfNull, String IfFail)
        {
            return Input.ToStringS(IfNull, IfFail);
        }

        [TestCase("stub1", new String[2] { "a", "a"}, Result = true)]
        [TestCase("stub2", new String[3] { "a", "a", "a" }, Result = true)]
        [TestCase("stub3", new String[3] { "a", "a", "A" }, Result = false)]        
        [TestCase("stub5", new String[0] { }, Result = false, ExpectedException = typeof(ArgumentException))]
        [TestCase("stub6", new String[1] { "a" }, Result = false, ExpectedException = typeof(ArgumentException))]
        public Boolean AreAllEqual(String Stub, params String[] list)
        {
            return CommonTools.AreAllEqual(list);
        }

        [TestCase('a', 'a', false, Result=true)]
        [TestCase('a', 'A', false, Result = false)]
        [TestCase('a', 'A', true, Result = true)]
        [TestCase('a', 'b', false, Result = false)]
        [TestCase('a', 'B', true, Result = false)]
        public Boolean AreCharsEqual(Char First, Char Second, Boolean IgnoreCase)
        {
            return CommonTools.AreCharsEqual(First, Second, IgnoreCase);
        }

        [TestCase(false, 'a', 'a', Result = true)]
        [TestCase(false, 'a', 'a', 'a', Result=true)]
        [TestCase(false, 'a', 'a', 'A', Result = false)]
        [TestCase(true, 'a', 'a', 'A', Result = true)]
        [TestCase(true, 'a', 'b', 'A', 'c', Result = false)]
        [TestCase(true, 'x', 'x', 'X', 'X', Result = true)]
        [TestCase(true, 'x', Result = true, ExpectedException = typeof(ArgumentException))]
        [TestCase(true, new Char[0]{}, Result = true, ExpectedException = typeof(ArgumentException))]        
        public Boolean AreCharsEqual(Boolean IgnoreCase, params Char[] Chars)
        {
            return CommonTools.AreCharsEqual(IgnoreCase, Chars);
        }
    }
}

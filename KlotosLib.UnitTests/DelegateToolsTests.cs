using System;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    public class DelegateToolsTests
    {
        private static Int32 StaticMethod(String str)
        {
            return Int32.Parse(str);
        }

        private Int32 InstanceMethod(String str)
        {
            return Int32.Parse(str);
        }

        private Func<String, Int32> testdel1 = new Func<string, int>(Int32.Parse);

        private Func<String, Int32> testdel2 = delegate(String str)
        {
            return (Int32)Byte.Parse(str);
        };

        private static Func<String, Int32> testdel3 = delegate(String str)
        {
            return (Int32)Int16.Parse(str);
        };

        private static Func<String, Int32> testdel4 = null;

        private static Func<String, Int32> testdel5 = null;

        [Test]
        public void SimpleDelegatesCountTest()
        {            
            testdel1 += testdel2;
            Int32 realCount = testdel1.SimpleDelegatesCount();
            Assert.AreEqual(2, realCount);
            Assert.AreNotEqual(3, realCount);
            testdel1 -= testdel2;
            testdel1 -= testdel1;
            Assert.AreEqual(0, testdel1.SimpleDelegatesCount());
            Assert.IsNull(testdel1);
            testdel2.Invoke("123");
            testdel3.Invoke("456");
        }

        [Test]
        public void IsStaticDelegateMethodTest()
        {
            Assert.AreEqual(testdel3.IsStaticDelegateMethod(false), true);
            Assert.AreEqual(testdel4.IsStaticDelegateMethod(false), false);
            testdel4 += StaticMethod;
            Assert.AreEqual(testdel4.IsStaticDelegateMethod(false), true);
            testdel4 += InstanceMethod;
            Assert.AreEqual(testdel4.IsStaticDelegateMethod(false), false);
            Assert.Throws<ArgumentNullException>(delegate
            {
                bool res = testdel5.IsStaticDelegateMethod(true);
            });
            Assert.Throws<FormatException>(delegate
            {
                testdel4.Invoke("abcd");
            });
            Assert.DoesNotThrow(delegate
            {
                testdel4.Invoke("123");
            });
        }
    }
}

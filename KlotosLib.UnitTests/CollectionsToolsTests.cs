using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    public class CollectionsToolsTests
    {
        [Test]
        public void IsCollectionNullOrEmpty()
        {
            List<Int32> test1 = new List<int>(3) { 0, 5, 0 };
            Assert.AreEqual(false, test1.IsNullOrEmpty());
            test1.RemoveAt(2);
            Assert.AreEqual(false, test1.IsNullOrEmpty());
            test1.Clear();
            Assert.AreEqual(true, test1.IsNullOrEmpty());
            test1 = null;
            Assert.AreEqual(true, test1.IsNullOrEmpty());
        }

        [TestCase(4, 5, 6, Result=false)]
        [TestCase(4, 5, Result = false)]
        [TestCase(4, Result = true)]
        [TestCase(new Int32[1] { 4 }, Result = true)]
        [TestCase(new Int32[3] { 3, 4, 5 }, Result = false)]
        [TestCase(new Int32[0] {  }, Result = false)]
        public Boolean HasSingle(params Int32[] Input)
        {
            return Input.HasSingle();
        }

        [Test]
        public void HasSingle()
        {
            IEnumerable<Int32> input1 = Enumerable.Range(1, 2);
            Assert.That(input1.HasSingle()==false);
            input1 = Enumerable.Range(1, 1);
            Assert.That(input1.HasSingle() == true);
            List<string> input2 = new List<string>();
            Assert.That(((IEnumerable<String>)input2).HasSingle() == false);
            input2.Add("a");
            Assert.That(((IEnumerable<String>)input2).HasSingle() == true);
            input2.Add("b");
            Assert.That(((IEnumerable<String>)input2).HasSingle() == false);
            input2 = null;
            Assert.That(((IEnumerable<String>)input2).HasSingle() == false);
        }

        [Test]
        public void DeconcatFromStringList()
        {
            String input1 = "X: 2012.01.05; 2013.10.10; 2010.01.21 18:57.";
            List<DateTime> output1 = input1.DeconcatFromString<DateTime>((String s) => DateTime.Parse(s), "X: ", ".", "; ", true);
            Assert.IsTrue(output1.EqualsExact<DateTime>(new List<DateTime>(3) { new DateTime(2012, 01, 05), new DateTime(2013, 10, 10), new DateTime(2010, 01, 21, 18, 57, 00) }));
            String input2 = "X: 2012.01.05; 2013.10.10; 2010.01.21 18:57; blabla.";
            Assert.Throws<InvalidOperationException>(() => input2.DeconcatFromString<DateTime>((String s) => DateTime.Parse(s), "X: ", ".", "; ", true));

            String input3 = "5.78, 7.45, 0.46, -8.9, 0";

            List<Double> output3 = input3.DeconcatFromString<Double>((String str) => Double.Parse(str, CultureInfo.InvariantCulture), null, "0", ", ", true);
            Assert.IsTrue(output3.EqualsExact<Double>(new List<Double>(4) { 5.78, 7.45, 0.46, -8.9 }));
        }

        [Test]
        public void DeconcatFromStringDictionary()
        {
            String input1 = "a => 1;b => 2;c => 3;d => 4";
            Dictionary<Char, Byte> output1 = input1.DeconcatFromString<Char, Byte>((String s) => s.Trim()[0], sv => Convert.ToByte(sv), ";", " => ", true);
            Assert.IsTrue(output1.EqualsExact<Char, Byte>(new Dictionary<char, byte>(){
                { 'a', 1},
                { 'b', 2},
                { 'c', 3},
                { 'd', 4}
            }));
            String input2 = "a => 1;b => 2;c => 3;c => 4";
            Assert.Throws<InvalidOperationException>(() => input2.DeconcatFromString<Char, Byte>((String s) => s.Trim()[0], sv => Convert.ToByte(sv), ";", " => ", true));
            TestDelegate td = new TestDelegate(delegate()
            {
                input2.DeconcatFromString<Char, Byte>((String s) => s.Trim()[0], sv => Convert.ToByte(sv), ";", " => ", false);
            });
            Assert.DoesNotThrow(td);
        }

        [Test]
        public void ConcatToStringDictionary()
        {
            Dictionary<String, DateTime> source = new Dictionary<string, DateTime>(5)
            {
                { "one", new DateTime(2012, 12, 12) },
                { "two", new DateTime(2012, 12, 12).AddDays(1) },
                { "three", new DateTime(2012, 12, 12).AddDays(3) },
                { "four", new DateTime(2012, 12, 12).AddMinutes(5) },
                { "five", DateTime.MaxValue}
            };
            String output = source.ConcatToString((String key) => { if (key.Length <= 3) return key.ToUpper(); else return key.Substring(0, 3); },
                (DateTime val) => { return val.Day.ToString(); }, " -> ", ", ");
            Assert.AreEqual("ONE -> 12, TWO -> 13, thr -> 15, fou -> 12, fiv -> 31", output);
        }

        [Test]
        public void ConcatToStringEnumerable()
        {
            IEnumerable<Int32> test = Enumerable.Range(-2, 6);            
            String output = test.ConcatToString<Int32>(i => i < 0 ? (-10).ToString() : i.ToString(), "Start: ", ".", "; ", false);
            Assert.AreEqual("Start: -10; -10; 0; 1; 2; 3.", output);
        }

        [Test]
        public void ConcatToStringSimple()
        {
            Char[] input = { 'a', 'b', 'c', 'a', 'a', 'y', ' ', 'y'};
            String output = input.ConcatToString("Start: ", ".", "", true, false);
            Assert.AreEqual("Start: abcy .", output);
        }

        [Test]
        public void SplitList()
        {
            List<String> input1 = new List<string>(5) { "ab", "cd", "efg", "hijk", "l" };
            List<String> left_output1;
            List<String> right_output1;
            input1.SplitList<List<String>, String>(1, CollectionTools.SplitterDisposition.ToFirst, out left_output1, out right_output1);
            CollectionAssert.AreEqual(new List<String>(2) { "ab", "cd" }, left_output1);
            CollectionAssert.AreEqual(new List<String>(3) { "efg", "hijk", "l" }, right_output1);

            input1.SplitList<List<String>, String>(1, CollectionTools.SplitterDisposition.ToLast, out left_output1, out right_output1);
            CollectionAssert.AreEqual(new List<String>(1) { "ab" }, left_output1);
            CollectionAssert.AreEqual(new List<String>(4) { "cd", "efg", "hijk", "l" }, right_output1);

            input1.SplitList<List<String>, String>(1, CollectionTools.SplitterDisposition.Reject, out left_output1, out right_output1);
            CollectionAssert.AreEqual(new List<String>(1) { "ab" }, left_output1);
            CollectionAssert.AreEqual(new List<String>(3) { "efg", "hijk", "l" }, right_output1);
        }

        [Test]
        public void SplitDictionary()
        {
            Dictionary<Int32, String> input1 = new Dictionary<int, string>(4) {
                { 1, "one" },
                { 5, "five" },
                { 9, "nine" },
                { 0, "zero" }
            };
            Dictionary<Int32, String> left_output1;
            Dictionary<Int32, String> right_output1;
            
            input1.SplitDictionary<Dictionary<Int32, String>, Int32, String>(1, CollectionTools.SplitterDisposition.ToLast, out left_output1, out right_output1);
            CollectionAssert.AreEqual(new Dictionary<Int32, String>(), left_output1);
            CollectionAssert.AreEqual(input1, right_output1);


            input1.SplitDictionary<Dictionary<Int32, String>, Int32, String>(1, CollectionTools.SplitterDisposition.ToFirst, out left_output1, out right_output1);
            CollectionAssert.AreEqual(new Dictionary<Int32, String>(1) { { 1, "one" } }, left_output1);
            CollectionAssert.AreEqual(new Dictionary<int, string>(3) { { 5, "five" }, { 9, "nine" }, { 0, "zero" } }, right_output1);

            input1.SplitDictionary<Dictionary<Int32, String>, Int32, String>(1, CollectionTools.SplitterDisposition.Reject, out left_output1, out right_output1);
            CollectionAssert.AreEqual(new Dictionary<Int32, String>(0), left_output1);
            CollectionAssert.AreEqual(new Dictionary<int, string>(3) { { 5, "five" }, { 9, "nine" }, { 0, "zero" } }, right_output1);

            TestDelegate td = new TestDelegate(delegate()
            {
                input1.SplitDictionary<Dictionary<Int32, String>, Int32, String>(100, CollectionTools.SplitterDisposition.ToFirst, out left_output1, out right_output1);
            });
            Assert.Throws<ArgumentException>(td);
        }

        [Test]
        public void ReverseDictionary()
        {
            Dictionary<Byte, String> input1 = new Dictionary<byte, string>(3)
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" }
            };
            Dictionary<Byte, String> expected1 = new Dictionary<byte, string>(3)
            {
                { 3, "three" },                
                { 2, "two" },
                { 1, "one" }                
            };
            CollectionAssert.AreEqual(expected1, input1.ReverseDictionary());
        }

        [Test]
        public void KVPToString()
        {
            KeyValuePair<Char, String> input1 = new KeyValuePair<char, string>('A', "alpha");
            Assert.AreEqual("A -> alpha", input1.ToString(" -> ", true));
            Assert.AreEqual("alphaA", input1.ToString("", false));
        }

        [Test]
        public void HasDuplicatedItems()
        {
            Char[] input1 = { 'a', 'b', 'c', 'd', 'b' };
            Assert.IsTrue(input1.HasDuplicatedItems(), input1.ConcatToString());

            String[] input2 = { "ab", "bc", "cd", "BC" };
            Assert.IsFalse(input2.HasDuplicatedItems(), input2.ConcatToString());
            Assert.IsTrue(input2.HasDuplicatedItems(StringComparer.OrdinalIgnoreCase), input2.ConcatToString());
        }

        [Test]
        public void DistinctWithCount()
        {
            List<String> input1 = new List<string>(6) {"abc", "cde", "ABC", "Cde", "efg", "abc"};
            CollectionAssert.AreEqual(new Dictionary<String, Int32>(5)
            {
                {"abc", 2}, {"cde", 1}, {"ABC", 1}, {"Cde", 1}, {"efg", 1}
            }, input1.DistinctWithCount());
            CollectionAssert.AreEqual(new Dictionary<string, int>(3)
            {
                {"abc", 3}, {"cde", 2}, {"efg", 1}
            }, input1.DistinctWithCount(StringComparer.OrdinalIgnoreCase));
        }

        [Test]
        public void SwapValues()
        {
            Dictionary<Byte, String> input1 = new Dictionary<byte, string>(4)
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" },
                { 4, "four" }
            };            
            Dictionary<Byte, String> expected1 = new Dictionary<byte, string>(4)
            {
                { 1, "four" },
                { 2, "two" },
                { 3, "three" },
                { 4, "one" }
            };
            CollectionAssert.AreEqual(expected1, input1.SwapValues((Byte)1, (Byte)4));
        }

        [Test]
        public void SwapItems()
        {
            List<Char> input1 = new List<Char>(4) { 'a', 'b', 'c', 'd' };
            List<Char> expected1 = new List<Char>(4) { 'c', 'b', 'a', 'd' };
            CollectionAssert.AreEqual(expected1, input1.SwapItems(0, 2));
        }

        [Test]
        public void EqualsDictionary()
        {
            Dictionary<Byte, String> input1 = new Dictionary<byte, string>(3)
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" }
            };
            Dictionary<Byte, String> input2 = new Dictionary<byte, string>(3)
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" }
            };
            Dictionary<Byte, String> input3 = new Dictionary<byte, string>(3)
            {
                { 2, "two" },
                { 1, "one" },                
                { 3, "three" }
            };
            Dictionary<Byte, String> input4 = new Dictionary<byte, string>(3)
            {
                { 1, "one" },
                { 2, "two" },
                { 4, "four" }
            };
            Assert.IsTrue(input1.EqualsExact(input2));
            Assert.IsFalse(input1.EqualsExact(input3));
            Assert.IsTrue(input1.EqualsIgnoreOrder(input3));
            Assert.IsFalse(input1.EqualsIgnoreOrder(input4));
        }

        [Test]
        public void EqualsList()
        {
            List<String> input1 = new List<string>(3) { "one", "two", "three" };
            List<String> input2 = new List<string>(3) { "three", "one", "two" };
            List<String> input3 = new List<string>(3) { "one", "two", "three", "four" };
            List<String> input4 = new List<string>(3) { "one", "two", "three" };
            Assert.IsTrue(input1.EqualsExact(input4));
            Assert.IsTrue(input4.EqualsExact(input1));
            Assert.IsFalse(input1.EqualsExact(input2));
            Assert.IsFalse(input4.EqualsExact(input2));
            Assert.IsTrue(input1.EqualsIgnoreOrder(input2));
            Assert.IsTrue(input4.EqualsIgnoreOrder(input2));
            Assert.IsFalse(input3.EqualsIgnoreOrder(input1));
        }

        [Test]
        public void SortByValue()
        {
            Dictionary<Int32, Char> input1 = new Dictionary<int, char>(3) { { 1, 'h' }, { 2, 'a' }, { 3, 'x' } };
            Dictionary<Int32, Char> expected1 = new Dictionary<int, char>(3) { { 2, 'a' }, { 1, 'h' }, { 3, 'x' } };
            Dictionary<Int32, Char> expected2 = new Dictionary<int, char>(3) { { 3, 'x' }, { 1, 'h' }, { 2, 'a' } };
            CollectionAssert.AreEqual(expected1, input1.SortByValue(true));
            CollectionAssert.AreEqual(expected2, input1.SortByValue(false));
        }

        [Test]
        public void GetIdenticalStart()
        {
            List<String> input1 = new List<string>(3) { "one", "two", "three" };
            List<String> input2 = new List<string>(3) { "three", "one", "two" };
            List<String> input3 = new List<string>(3) { "one", "two", "three", "four" };
            CollectionAssert.AreEqual(input1, CollectionTools.GetIdenticalStart(input1, input3));
            CollectionAssert.AreEqual(new List<String>(0), CollectionTools.GetIdenticalStart(input1, input2));
        }

        [Test]
        public void GetIdenticalEnd()
        {
            Char[] input1 = { 'a', 'b', 'c', 'd' };
            Char[] input2 = { '1', '2', '3', 'a', 'b', 'c', 'd' };
            Char[] input3 = { '1', '2', '3', 'a', 'x', 'c', 'd' };

            Char[] expected1 = { 'd', 'c', 'b', 'a' };
            Char[] expected2 = { 'd', 'c' };

            CollectionAssert.AreEqual(expected1, CollectionTools.GetIdenticalEnd(input1, input2));
            CollectionAssert.AreEqual(expected2, CollectionTools.GetIdenticalEnd(input3, input2));
            CollectionAssert.AreEqual(expected2, CollectionTools.GetIdenticalEnd(input3, input1));
            CollectionAssert.AreEqual(new Char[0], CollectionTools.GetIdenticalEnd(input3, expected1));
        }

        [Test]
        public void GetPortionByKeys()
        {
            Dictionary<Byte, String> input1 = new Dictionary<byte, string>(4)
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" },
                { 4, "four" }
            };
            Dictionary<Byte, String> expected1 = new Dictionary<byte, string>(2)
            {
                { 1, "one" },                
                { 3, "three" }                
            };

            CollectionAssert.AreEqual(expected1, input1.GetPortionByKeys(false, new Byte[] {1, 3}));
            Assert.Throws<KeyNotFoundException>(() => input1.GetPortionByKeys(false, new Byte[] { 1, 3, 5 }));
            Assert.DoesNotThrow(() => input1.GetPortionByKeys(true, new Byte[] { 1, 3, 5 }));
        }

        [Test]
        public void RemapDictionary()
        {
            Dictionary<UInt32, Byte> source1 = new Dictionary<UInt32, Byte>(6)
            {
                {2011711, 1},
                {2010121, 5},
                {2011721, 1},
                {2010125, 5},
                {2010126, 5},
                {8684680,100}
            };
            Func<IEnumerable<UInt32>, UInt32> not_anon1 = delegate(IEnumerable<UInt32> input)
            {
                if (input.Count() == 2) { return input.Min(); } else { return input.Max(); }
            };
            Dictionary<Byte, UInt32> output1 = source1.RemapDictionary(not_anon1, false);

            Assert.AreEqual(output1.Count, 3);
            Assert.AreEqual(output1[5], 2010126);
            Assert.AreEqual(output1[1], 2011711);
            Assert.AreEqual(output1[100], 8684680);

            Dictionary<UInt32, String> source2 = new Dictionary<uint, String>(6)
            {
                {2011711, "1"},
                {2010121, "5"},
                {2011721, "1"},
                {2010125, "5"},
                {2010126, "5"},
                {8684680, null}
            };
            Func<IEnumerable<UInt32>, Int64> not_anon2 = delegate(IEnumerable<UInt32> input)
            {
                return input.LongCount();
            };
            Dictionary<String, Int64> output2 = source2.RemapDictionary(not_anon2, false);

            Assert.IsTrue(output2.EqualsExact(new Dictionary<string, long>(2){
                { "1", 2 },
                { "5", 3 }
            }), "output2 = " + output2.ConcatToString());
        }
                
        [Test]
        public void ConcatArrays()
        {
            Int32[] a1 = new Int32[5] { 1, 2, 3, -3, 5 };
            Int32[] a2 = new Int32[3] { 0, 2, 3 };
            Int32[] a3 = new Int32[0] {  };
            Int32[] a4 = null;
            Int32[] a5 = new Int32[1] { 1 };
            Int32[] res = CollectionTools.ConcatArrays(a1, a2, a3, a4, a5);
            Int32[] expected = new Int32[] { 1, 2, 3, -3, 5, 0, 2, 3, 1 };
            CollectionAssert.AreEqual(expected, res, res.ConcatToString());

            Int32[] b1 = null;
            Int32[] res2 = CollectionTools.ConcatArrays(b1);
            CollectionAssert.AreEqual(new Int32[0] { }, res2);

            String[] c1 = new String[0] { };
            String[] c2 = new String[2] { "aa", "bb" };
            String[] c3 = null;
            String[] res3 = CollectionTools.ConcatArrays(c1, c2, c3);
            String[] expected3 = new String[2] { "aa", "bb" };
            CollectionAssert.AreEqual(expected3, res3, res3.ConcatToString());
        }
        
        [Test]
        public void Shuffle()
        {
            Char[] input1 = new char[9] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i' };
            input1.Shuffle(new Random());
            CollectionAssert.IsNotEmpty(input1);
            CollectionAssert.AllItemsAreUnique(input1, input1.ConcatToString());
            Assert.That(input1.Length == 9);
        }

        [Test]
        public void ShuffleNew()
        {
            Char[] input1 = new char[9] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i' };
            Char[] output1 = input1.ShuffleNew(new Random());
            CollectionAssert.IsNotEmpty(output1);
            CollectionAssert.AreNotEqual(input1, output1);
            CollectionAssert.AllItemsAreUnique(output1, output1.ConcatToString());
        }

        [Test]
        public void RandomItem()
        {
            IEnumerable<Int32> input1 = new Int32[2] { 1, 2 }.AsEnumerable().Where(elem => elem > 1);
            Assert.AreEqual(2, input1.RandomItem(new Random()));
        }
    }
}

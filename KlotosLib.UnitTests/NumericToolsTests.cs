using System;
using System.Collections.Generic;
using NUnit.Framework;
using KlotosLib;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class NumericToolsTests
    {
        [TestCase(5.6789, 2, Result=5.68)]
        [TestCase(1.2345, 2, Result = 1.23)]
        [TestCase(1.2345, 3, Result = 1.235)]
        [TestCase(1.234567, 20, Result = 1.234567)]
        public Double RoundingDouble(Double Input, Byte Digits)
        {
            return Input.Rounding(Digits);
        }

        [TestCase(5.6789, 0, Result = 6)]
        [TestCase(5.5789, 0, Result = 6)]
        [TestCase(5.4789, 0, Result = 5)]
        [TestCase(5.4789, 100, Result = 5.4789)]
        [TestCase(5.6789, 2, Result = 5.68)]
        [TestCase(1.2345, 2, Result = 1.23)]
        [TestCase(1.2345, 3, Result = 1.235)]
        [TestCase(1.234567, 20, Result = 1.234567)]
        public Decimal RoundingDecimal(Decimal Input, Byte Digits)
        {
            return Input.Rounding(Digits);
        }

        [TestCase(6, 7, Result = 1)]
        [TestCase(6, -7, Result = 1)]
        [TestCase(-6, 7, Result = 1)]
        [TestCase(-6, -7, Result = 1)]
        [TestCase(0, 0, Result = 0)]
        public UInt32 GetDifferenceAbs(Int32 First, Int32 Second)
        {
            return NumericTools.GetDifferenceAbs(First, Second);
        }

        [TestCase(5.6789, 4, Result = "6789")]
        [TestCase(5.6789, 0, Result = "6789")]
        [TestCase(5.6789, 3, Result = "678")]
        [TestCase(5.6789, 50, Result = "6789")]
        public String GetFraction(Double Input, Byte FractionDigits)
        {
            return Input.GetFraction(FractionDigits);
        }

        [TestCase(-1, Result = -1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(0, Result = 0)]
        [TestCase(1, Result = 1)]
        [TestCase(2, Result = 3)]
        [TestCase(3, Result = 6)]
        [TestCase(4, Result = 10)]
        [TestCase(5, Result = 15)]
        [TestCase(6, Result = 21)]
        [TestCase(100, Result = 5050)]
        public Int32 Summarial(Int32 Input)
        {
            return NumericTools.Summarial(Input);
        }

        [TestCase(-1, Result = -1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(0, Result = 0)]
        [TestCase(1, Result = 1)]
        [TestCase(2, Result = 2)]
        [TestCase(3, Result = 6)]
        [TestCase(4, Result = 24)]
        [TestCase(5, Result = 120)]
        [TestCase(6, Result = 720)]
        public UInt32 Factorial(Int32 Input)
        {
            return NumericTools.Factorial(Input);
        }

        [Test]
        public void CombineAndSplitTest()
        {
            const int first1 = 25;
            const int second1 = 256;
            Int64 combined1 = NumericTools.Combine(first1, second1);
            KeyValuePair<int, int> res1 = NumericTools.Split(combined1);
            Assert.AreEqual(first1, res1.Key, "actual = "+res1.Key);
            Assert.AreEqual(second1, res1.Value, "actual = " + res1.Value);

            const byte first2 = 127;
            const byte second2 = 64;
            UInt16 combined2 = KlotosLib.ByteTools.Combiners.GetUInt16BE(first2, second2);// NumericTools.Combine(first2, second2);
            KeyValuePair<byte, byte> res2 = NumericTools.Split(combined2);
            Assert.AreEqual(first2, res2.Key, "actual = " + res2.Key);
            Assert.AreEqual(second2, res2.Value, "actual = " + res2.Value);

            const UInt32 first3 = 1024*1024;
            const UInt32 second3 = 256*256*256;
            UInt64 combined3 = NumericTools.Combine(first3, second3);
            KeyValuePair<UInt32, UInt32> res3 = NumericTools.Split(combined3);
            Assert.AreEqual(first3, res3.Key, "actual = " + res3.Key);
            Assert.AreEqual(second3, res3.Value, "actual = " + res3.Value);
        }

        [Test]
        public void CombineAndSplit4Test()
        {
            const byte first4 = 10;
            const byte second4 = 20;
            const byte third4 = 30;
            const byte fourth4 = 40;
            UInt32 combined4 = KlotosLib.ByteTools.Combiners.GetUInt32BE(first4, second4, third4, fourth4);//NumericTools.Combine(first4, second4, third4, fourth4);
            byte first4_out;
            byte second4_out;
            byte third4_out;
            byte fourth4_out;
            NumericTools.Split(combined4, out first4_out, out second4_out, out third4_out, out fourth4_out);
            Assert.AreEqual(first4, first4_out);
            Assert.AreEqual(second4, second4_out);
            Assert.AreEqual(third4, third4_out);
            Assert.AreEqual(fourth4, fourth4_out);
        }
        
        [TestCase(0.0001, 5.000001, 5.000002, ExpectedResult = true)]
        [TestCase(0.0000001, 5.000001, 5.000002, ExpectedResult = false)]
        [TestCase(0.0000001, 5, 5, ExpectedResult = true)]
        [TestCase(0.0, 1234567890.0, 1234567890.0, ExpectedResult = true)]

        [TestCase(0.1, 360.0, 360.0, ExpectedResult = true)]
        [TestCase(0.1, -360.0, -360.0, ExpectedResult = true)]
        [TestCase(0.1, 360.0, 359.99, ExpectedResult = true)]
        [TestCase(0.1, -360.0, 359.99, ExpectedResult = false)]
        [TestCase(0.1, 360.0, -359.99, ExpectedResult = false)]
        [TestCase(0.1, -360.0, -359.99, ExpectedResult = true)]
        [TestCase(0.2, 360.0, 359.9, ExpectedResult = true)]
        [TestCase(0.2, 360.0, 359.8, ExpectedResult = true)]
        [TestCase(0.2, 360.0, 359.7, ExpectedResult = false)]
        public Boolean AreEqual(Double Threshold, Double First, Double Second)
        {
            return NumericTools.AreEqual(Threshold, First, Second);
        }

        [Test]
        public void Clusterize()
        {
            List<List<Double>> actual_result = NumericTools.Clusterize(0.6, new Double[7] {-0.2, 0, 0.1, 1, 2, 2.6, 2.7});
            List<List<Double>> expected_result = new List<List<double>>(3)
            {
                new List<double>(3){-0.2, 0, 0.1},
                new List<double>(2){2.6, 2.7},
                new List<double>(1){1},
                new List<double>(1){2}
            };

            Assert.IsTrue(actual_result.Count == expected_result.Count, "Actual count = " + actual_result.Count + "; "+
                actual_result.ConcatToString(delegate(List<double> list) { return list.ConcatToString(); }, "", "", " || ", true));
            CollectionAssert.AreEqual(expected_result[0], actual_result[0]);
            CollectionAssert.AreEqual(expected_result[1], actual_result[1], "Actual = " + actual_result[1].ConcatToString("; "));
            CollectionAssert.AreEqual(expected_result[2], actual_result[2]);
            CollectionAssert.AreEqual(expected_result[3], actual_result[3]);
        }
    }
}

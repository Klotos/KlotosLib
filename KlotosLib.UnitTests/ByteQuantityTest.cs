using System;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class ByteQuantityTest
    {
        [TestCase(0L, Result="0 Bytes")]
        [TestCase(100L, Result = "100 Bytes")]
        [TestCase(200000000L, Result = "200\u00A0000\u00A0000 Bytes")]
        [TestCase(9999L, Result = "9\u00A0999 Bytes")]
        public String ToStringTest(Int64 Value)
        {
            return ByteQuantity.FromBytes(Value).ToString();
        }

        [TestCase(0L, Result = 0L)]
        [TestCase(8L, Result=1L)]
        [TestCase(-8L, Result = 1L, ExpectedException=typeof(ArgumentOutOfRangeException))]
        [TestCase(9L, Result = 2L)]
        [TestCase(15L, Result = 2L)]
        [TestCase(16L, Result = 2L)]
        [TestCase(17L, Result = 3L)]        
        public Int64 FromBits(Int64 Value)
        {
            return ByteQuantity.FromBits(Value).Bytes;
        }

        [Test]
        public void HDDTest()
        {            
            ByteQuantity bq = ByteQuantity.FromTebibytes(1.819M);
            Decimal x1 = bq.Terabytes;
            Assert.That(x1, Is.EqualTo(2.0M).Within(0.0001M));
            Assert.That(bq.Tebibytes, Is.EqualTo(1.819M).Within(0.001M));
            Assert.AreEqual((double)2M, (double)x1, 0.0001D);

            ByteQuantity bq2 = ByteQuantity.FromTerabytes(1.5M);
            Assert.That(bq2.Terabytes, Is.EqualTo(1.5M));
            Assert.That(bq2.Gibibytes, Is.EqualTo(1397.26M).Within(1M));

            ByteQuantity bq31 = ByteQuantity.FromGigabytes(320M);
            ByteQuantity bq32 = ByteQuantity.FromGibibytes(298.09M);
            Assert.That(bq31.Bytes, Is.EqualTo(bq32.Bytes).Within(ByteQuantity.FromMebibytes(80).Bytes));

            ByteQuantity bq41 = ByteQuantity.FromGigabytes(120M);
            ByteQuantity bq42 = ByteQuantity.FromGibibytes(111.79M);
            Assert.That(bq41.Kibibytes, Is.EqualTo(bq42.Kibibytes).Within(ByteQuantity.FromMebibytes(130).Kibibytes));

            ByteQuantity bq51 = ByteQuantity.FromMegabytes(100.34M);
            ByteQuantity bq52 = ByteQuantity.FromMebibytes(100M);
            ByteQuantity bq53 = ByteQuantity.FromBytes(105211934L);
            Assert.That(bq53.Bytes, Is.EqualTo(bq51.Bytes).Within(6000000L));
            Assert.That(bq53.Bytes, Is.EqualTo(bq52.Bytes).Within(1000000L));
            Assert.That(bq53.Megabytes, Is.EqualTo(100M).Within(6M));
        }                

        [TestCase(1024L*1024L+10L, 2, true, Result="1 MiB")]
        [TestCase(1024L * 1024L + 10L, 5, true, Result = "1.00001 MiB")]
        [TestCase(1024L * 1024L + 1024L * 512L, 0, true, Result = "2 MiB")]
        [TestCase(1024L * 1024L + 1024L * 128L, 0, true, Result = "1 MiB")]
        [TestCase(1024L * 1024L + 1024L * 128L, 1, true, Result = "1.1 MiB")]
        [TestCase(1024L * 1024L + 1024L * 128L, 2, false, Result = "1,13 MiB")]
        [TestCase(2040L, 2, false, Result = "1,99 KiB")]
        [TestCase(2049L, 2, false, Result = "2 KiB")]
        [TestCase(2059L, 2, false, Result = "2,01 KiB")]
        [TestCase(1024L * 1024L * 1024L, 2, true, Result = "1 GiB")]
        [TestCase(1024L * 1024L * 1024L + 1024L * 1024L * 160L, 2, true, Result = "1.16 GiB")]
        [TestCase(1024L * 1024L * 1024L + 1024L * 1024L * 160L, 1, true, Result = "1.2 GiB")]
        [TestCase(1024L * 1024L * 1024L + 1024L * 1024L * 150L, 0, true, Result = "1 GiB")]
        [TestCase(1024L * 1024L * 1024L + 1024L * 1024L * 1020L, 0, true, Result = "2 GiB")]
        [TestCase(1024L * 1024L * 1024L * 1024L * 1024L + 1024L * 1024L * 1024L * 1024L * 1020L, 0, true, Result = "2 PiB")]
        [TestCase(1024L * 1024L * 1024L * 1024L * 1024L + 1024L * 1024L * 1024L * 1024L * 1020L, 2, true, Result = "2 PiB")]
        [TestCase(1024L * 1024L * 1024L * 1024L * 1024L + 1024L * 1024L * 1024L * 1024L * 1020L, 3, true, Result = "1.996 PiB")]
        [TestCase(1024L * 1024L * 1024L * 1024L * 1024L + 1024L * 1024L * 1024L * 1024L * 1020L, 4, true, Result = "1.9961 PiB")]
        [TestCase(1024L * 1024L * 1024L * 1024L * 1024L + 1024L * 1024L * 1024L * 1024L * 1020L, 140, true, Result = "1.99609375 PiB")]
        [TestCase(43, 3, true, Result = "43 B")]
        [TestCase(43, 3, false, Result = "43 B")]
        public String ToStringWithBinaryPrefix(Int64 Bytes, Byte Precision, Boolean Separator)
        {
            return ByteQuantity.FromBytes(Bytes).ToStringWithBinaryPrefix(Precision, Separator);
        }

        [TestCase(1000L + 998L, 0, true, Result="2 KB")]
        [TestCase(1000L + 998L, 1, true, Result = "2 KB")]
        [TestCase(1000L + 998L, 2, true, Result = "2 KB")]
        [TestCase(1000L + 998L, 3, true, Result = "1.998 KB")]
        [TestCase(1000L + 998L, 4, true, Result = "1.998 KB")]
        [TestCase(1000L + 998L, 40, true, Result = "1.998 KB")]
        [TestCase(1000L * 1000L * 1000L * 5 + 1000L * 1000L * 800L, 10, true, Result = "5.8 GB")]
        [TestCase(1000L * 1000L * 1000L * 5 + 1000L * 1000L * 800L, 0, true, Result = "6 GB")]
        [TestCase(1000L * 1000L * 1000L * 5 + 1000L * 1000L * 800L, 1, true, Result = "5.8 GB")]
        [TestCase(1000L * 1000L * 1000L * 1000L * 1000L * 5 + 1000L * 1000L * 1000L * 1000L * 800L, 0, true, Result = "6 PB")]
        [TestCase(1000L * 1000L * 1000L * 1000L * 1000L * 5 + 1000L * 1000L * 1000L * 1000L * 800L, 1, true, Result = "5.8 PB")]
        [TestCase(1000L * 1000L * 1000L * 1000L * 1000L * 5 + 1000L * 1000L * 1000L * 1000L * 800L, 2, true, Result = "5.8 PB")]
        [TestCase(1000L * 1000L * 1000L * 1000L * 1000L * 5 + 1000L * 1000L * 1000L * 1000L * 800L, 3, true, Result = "5.8 PB")]
        [TestCase(43, 3, true, Result = "43 B")]
        [TestCase(43, 3, false, Result = "43 B")]
        public String ToStringWithDecimalPrefixTest(Int64 Bytes, Byte Precision, Boolean Separator)
        {
            return ByteQuantity.FromBytes(Bytes).ToStringWithDecimalPrefix(Precision, Separator);
        }

        [TestCase(2048L, 2048L, true, Result = true)]
        [TestCase(2048L, 2048L, false, Result = false)]
        [TestCase(2048L, 1024L, true, Result = false)]
        [TestCase(2048L, 1024L, false, Result = true)]
        public Boolean OperatorsEqualityTest(Int64 First, Int64 Second, Boolean EqualOrNotEqual)
        {
            ByteQuantity f = ByteQuantity.FromBytes(First);
            ByteQuantity s = ByteQuantity.FromBytes(Second);
            if (EqualOrNotEqual == true)
            { return f == s; }
            else
            { return f != s; }
        }

        [TestCase(2048L, 2048L, 1, Result = false)]
        [TestCase(2048L, 2047L, 1, Result = true)]
        [TestCase(2048L, 2049L, 1, Result = false)]

        [TestCase(2048L, 2048L, 2, Result = false)]
        [TestCase(2048L, 2047L, 2, Result = false)]
        [TestCase(2048L, 2049L, 2, Result = true)]

        [TestCase(2048L, 2048L, 3, Result = true)]
        [TestCase(2048L, 2047L, 3, Result = true)]
        [TestCase(2048L, 2049L, 3, Result = false)]

        [TestCase(2048L, 2048L, 4, Result = true)]
        [TestCase(2048L, 2047L, 4, Result = false)]
        [TestCase(2048L, 2049L, 4, Result = true)]
        public Boolean OperatorsComparabilityTest(Int64 First, Int64 Second, Byte Operator)//Operator: 1: >; 2: <; 3: >=; 4: <=
        {
            switch (Operator)
            {
                case 1:
                    return ByteQuantity.FromBytes(First) > ByteQuantity.FromBytes(Second);
                case 2:
                    return ByteQuantity.FromBytes(First) < ByteQuantity.FromBytes(Second);
                case 3:
                    return ByteQuantity.FromBytes(First) >= ByteQuantity.FromBytes(Second);
                case 4:
                    return ByteQuantity.FromBytes(First) <= ByteQuantity.FromBytes(Second);
                default:
                    throw new Exception();
            }
        }

        [TestCase(2048L, 2048L, Result = 4096L)]
        [TestCase(2048L, 0L, Result = 2048L)]        
        public Int64 OperatorPlus(Int64 First, Int64 Second)
        {
            return (ByteQuantity.FromBytes(First) + ByteQuantity.FromBytes(Second)).Bytes;
        }

        [TestCase(2048L, 2048L, Result = 0L)]
        [TestCase(2048L, 1024L, Result = 1024L)]
        [TestCase(4096L, 2048L, Result = 2048L)]
        [TestCase(2048L, 4096L, Result = 2048L)]
        public Int64 OperatorMinus(Int64 First, Int64 Second)
        {
            return (ByteQuantity.FromBytes(First) - ByteQuantity.FromBytes(Second)).Bytes;
        }
    }
}

using System;
using System.IO;
using KlotosLib.ByteTools;
using NUnit.Framework;

namespace KlotosLib.UnitTests.ByteTools
{
    [TestFixture]
    public class CombinersTest
    {
        [Test]
        public void TestLittleEndian()
        {
            byte[] input = new byte[16] { 8, 16, 24, 32, 40, 48, 56, 64, 72, 80, 88, 96, 104, 112, 120, 128 };
            MemoryStream inputMs = new MemoryStream(input);
            BinaryReader reader = new BinaryReader(inputMs);

            Assert.IsTrue(BitConverter.IsLittleEndian);
            Assert.AreEqual(BitConverter.ToInt16(input, 0), Combiners.ReadInt16LE(input, 0));
            Assert.AreEqual(BitConverter.ToInt16(input, 2), Combiners.ReadInt16LE(input, 2));
            Assert.AreEqual(reader.ReadInt16(), Combiners.ReadInt16LE(input, 0));
            Assert.AreEqual(reader.ReadInt16(), Combiners.ReadInt16LE(input, 2));

            Assert.AreEqual(BitConverter.ToUInt16(input, 4), Combiners.ReadUInt16LE(input, 4));
            Assert.AreEqual(reader.ReadUInt16(), Combiners.ReadUInt16LE(input, 4));

            Assert.AreEqual(BitConverter.ToInt32(input, 6), Combiners.ReadInt32LE(input, 6));
            Assert.AreEqual(reader.ReadInt32(), Combiners.ReadInt32LE(input, 6));

            Assert.AreEqual(BitConverter.ToUInt32(input, 10), Combiners.ReadUInt32LE(input, 10));
            Assert.AreEqual(reader.ReadUInt32(), Combiners.ReadUInt32LE(input, 10));

            Assert.Throws<ArgumentException>(delegate
            {
                Combiners.ReadUInt64LE(input, 10);
            });

            inputMs.Position = 0;
            Assert.AreEqual(BitConverter.ToInt64(input, 0), Combiners.ReadInt64LE(input, 0));
            Assert.AreEqual(reader.ReadInt64(), Combiners.ReadInt64LE(input, 0));

            Assert.AreEqual(BitConverter.ToUInt64(input, 8), Combiners.ReadUInt64LE(input, 8));
            Assert.AreEqual(reader.ReadUInt64(), Combiners.ReadUInt64LE(input, 8));

            reader.Close();
            inputMs.Close();
        }

        [Test]
        public void TestBigEndian()
        {
            byte[] inputForward = new byte[16] { 8, 16, 24, 32, 40, 48, 56, 64, 72, 80, 88, 96, 104, 112, 120, 128 };
            byte[] inputReversed = new byte[16];
            Array.Copy(inputForward, inputReversed, inputForward.Length);
            const int shift1 = 0;
            const int shift2 = 2;
            const int shift3 = 6;
            Array.Reverse(inputReversed, shift1, sizeof(Int16));
            Array.Reverse(inputReversed, shift2, sizeof(Int32));
            Array.Reverse(inputReversed, shift3, sizeof(Int64));

            Assert.AreEqual(Combiners.ReadInt16LE(inputForward, shift1), Combiners.ReadInt16BE(inputReversed, shift1));
            Assert.AreEqual(Combiners.ReadUInt16LE(inputForward, shift1), Combiners.ReadUInt16BE(inputReversed, shift1));

            Assert.AreEqual(Combiners.ReadInt32LE(inputForward, shift2), Combiners.ReadInt32BE(inputReversed, shift2));
            Assert.AreEqual(Combiners.ReadUInt32LE(inputForward, shift2), Combiners.ReadUInt32BE(inputReversed, shift2));

            Assert.AreEqual(Combiners.ReadInt64LE(inputForward, shift3), Combiners.ReadInt64BE(inputReversed, shift3));
            Assert.AreEqual(Combiners.ReadUInt64LE(inputForward, shift3), Combiners.ReadUInt64BE(inputReversed, shift3));
        }

        [Test]
        public void TestGeneric()
        {
            byte[] input = new byte[16] { 8, 16, 24, 32, 40, 48, 56, 64, 72, 80, 88, 96, 104, 112, 120, 128 };

            Assert.AreEqual(Combiners.ReadInt16LE(input, 0), Combiners.Read<Int16>(input, 0, Combiners.Endianess.LittleEndian));
            Assert.AreEqual(Combiners.ReadInt16BE(input, 0), Combiners.Read<Int16>(input, 0, Combiners.Endianess.BigEndian));
            Assert.AreEqual(Combiners.ReadInt16LE(input, 8), Combiners.Read<Int16>(input, 8, Combiners.Endianess.LittleEndian));
            Assert.AreEqual(Combiners.ReadInt16BE(input, 8), Combiners.Read<Int16>(input, 8, Combiners.Endianess.BigEndian));

            Assert.AreEqual(Combiners.ReadUInt16LE(input, 0), Combiners.Read<UInt16>(input, 0, Combiners.Endianess.LittleEndian));
            Assert.AreEqual(Combiners.ReadUInt16BE(input, 0), Combiners.Read<UInt16>(input, 0, Combiners.Endianess.BigEndian));
            Assert.AreEqual(Combiners.ReadUInt16LE(input, 8), Combiners.Read<UInt16>(input, 8, Combiners.Endianess.LittleEndian));
            Assert.AreEqual(Combiners.ReadUInt16BE(input, 8), Combiners.Read<UInt16>(input, 8, Combiners.Endianess.BigEndian));

            Assert.AreEqual(Combiners.ReadInt32LE(input, 0), Combiners.Read<Int32>(input, 0, Combiners.Endianess.LittleEndian));
            Assert.AreEqual(Combiners.ReadInt32BE(input, 0), Combiners.Read<Int32>(input, 0, Combiners.Endianess.BigEndian));
            Assert.AreEqual(Combiners.ReadInt32LE(input, 8), Combiners.Read<Int32>(input, 8, Combiners.Endianess.LittleEndian));
            Assert.AreEqual(Combiners.ReadInt32BE(input, 8), Combiners.Read<Int32>(input, 8, Combiners.Endianess.BigEndian));

            Assert.AreEqual(Combiners.ReadUInt32LE(input, 0), Combiners.Read<UInt32>(input, 0, Combiners.Endianess.LittleEndian));
            Assert.AreEqual(Combiners.ReadUInt32BE(input, 0), Combiners.Read<UInt32>(input, 0, Combiners.Endianess.BigEndian));
            Assert.AreEqual(Combiners.ReadUInt32LE(input, 8), Combiners.Read<UInt32>(input, 8, Combiners.Endianess.LittleEndian));
            Assert.AreEqual(Combiners.ReadUInt32BE(input, 8), Combiners.Read<UInt32>(input, 8, Combiners.Endianess.BigEndian));

            Assert.AreEqual(Combiners.ReadInt64LE(input, 0), Combiners.Read<Int64>(input, 0, Combiners.Endianess.LittleEndian));
            Assert.AreEqual(Combiners.ReadInt64BE(input, 0), Combiners.Read<Int64>(input, 0, Combiners.Endianess.BigEndian));
            Assert.AreEqual(Combiners.ReadInt64LE(input, 8), Combiners.Read<Int64>(input, 8, Combiners.Endianess.LittleEndian));
            Assert.AreEqual(Combiners.ReadInt64BE(input, 8), Combiners.Read<Int64>(input, 8, Combiners.Endianess.BigEndian));

            Assert.AreEqual(Combiners.ReadUInt64LE(input, 0), Combiners.Read<UInt64>(input, 0, Combiners.Endianess.LittleEndian));
            Assert.AreEqual(Combiners.ReadUInt64BE(input, 0), Combiners.Read<UInt64>(input, 0, Combiners.Endianess.BigEndian));
            Assert.AreEqual(Combiners.ReadUInt64LE(input, 8), Combiners.Read<UInt64>(input, 8, Combiners.Endianess.LittleEndian));
            Assert.AreEqual(Combiners.ReadUInt64BE(input, 8), Combiners.Read<UInt64>(input, 8, Combiners.Endianess.BigEndian));
        }
    }
}
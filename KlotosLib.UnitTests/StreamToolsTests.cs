using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class StreamToolsTests
    {
        [Test]
        public void CopyStream()
        {
            const Int32 len = 512;
            MemoryStream input = RandomGenerators.GenerateRandomStream(len, false);
            Assert.That(input.Position == len);
            MemoryStream output = new MemoryStream();
            StreamTools.CopyStream(input, output, true, true);
            Assert.That(output.Length == len && output.Position == len);
            input.Position = 0;
            output.Position = 0;
            while (input.Position < len && output.Position < len)
            {
                Int32 i1 = input.ReadByte();
                Int32 i2 = output.ReadByte();
                Assert.That(i1 == i2);
            }
            output.Close();
            input.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using KlotosLib.StringTools;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    public class ImageToolsTests
    {
        static ImageToolsTests()
        {
            string executablePath = System.IO.Directory.GetCurrentDirectory();
            DirectoryInfo binFolder = new DirectoryInfo(executablePath);
            Byte safeCounter = 0;
            do
            {
                safeCounter++;
                string currentName = binFolder.Name;
                if (currentName.Equals("KlotosLib", StringComparison.OrdinalIgnoreCase) == true)
                {
                    break;
                }
                else
                {
                    binFolder = binFolder.Parent;
                }
            } while (binFolder != null || safeCounter == 255);
            DirectoryInfo unitTestProjectRootFolder = binFolder.GetDirectories("KlotosLib.UnitTests", SearchOption.TopDirectoryOnly)[0];
            DirectoryInfo sampleImagesFolder = unitTestProjectRootFolder.GetDirectories("SampleImages", SearchOption.TopDirectoryOnly)[0];
            ImageFolderPath = sampleImagesFolder.FullName;
        }

        private static readonly string ImageFolderPath;

        [Test]
        public void GetBmpSizeFromHeader()
        {
            DirectoryInfo di = new DirectoryInfo(ImageFolderPath);
            DirectoryInfo bmpFolder = di.GetDirectories("BMP", SearchOption.TopDirectoryOnly)[0];

            FileInfo[] fi = bmpFolder.GetFiles("*.bmp", SearchOption.TopDirectoryOnly);
            foreach (FileInfo oneBmpImage in fi)
            {
                using (FileStream fs = File.OpenRead(oneBmpImage.FullName))
                {
                    Substring size_descr = StringTools.SubstringHelpers.GetInnerStringBetweenTokens(oneBmpImage.Name,
                        "_", ".bmp", 0, 0, false, Direction.FromEndToStart, StringComparison.OrdinalIgnoreCase);
                    String[] sizes = size_descr.Value.Split('x');
                    Int32 expected_width = Int32.Parse(sizes[0], NumberStyles.Integer);
                    Int32 expected_height = Int32.Parse(sizes[1], NumberStyles.Integer);

                    byte[] buffer = new byte[100];
                    fs.Read(buffer, 0, buffer.Length);
                    System.Drawing.Size res = ImageTools.GetBmpSizeFromHeader(buffer);

                    Assert.AreEqual(expected_width, res.Width);
                    Assert.AreEqual(expected_height, res.Height);
                }
            }
        }

        [Test]
        public void GetGifSizeFromHeader()
        {
            DirectoryInfo di = new DirectoryInfo(ImageFolderPath);
            DirectoryInfo gifFolder = di.GetDirectories("GIF", SearchOption.TopDirectoryOnly)[0];

            FileInfo[] fi = gifFolder.GetFiles("*.gif", SearchOption.TopDirectoryOnly);
            foreach (FileInfo oneGifImage in fi)
            {
                using (FileStream fs = File.OpenRead(oneGifImage.FullName))
                {
                    List<UInt32> dimensions = StringTools.ParsingHelpers.FindIntegers(oneGifImage.Name, oneGifImage.Name.LastIndexOf('_'));

                    Int32 expected_width = Convert.ToInt32(dimensions[0]);
                    Int32 expected_height = Convert.ToInt32(dimensions[1]);

                    byte[] buffer = new byte[100];
                    fs.Read(buffer, 0, buffer.Length);
                    System.Drawing.Size res = ImageTools.GetGifSizeFromHeader(buffer);

                    Assert.AreEqual(expected_width, res.Width);
                    Assert.AreEqual(expected_height, res.Height);
                }
            }
        }

        [Test]
        public void GetPngSizeFromHeader()
        {
            DirectoryInfo di = new DirectoryInfo(ImageFolderPath);
            DirectoryInfo pngFolder = di.GetDirectories("PNG", SearchOption.TopDirectoryOnly)[0];

            FileInfo[] fi = pngFolder.GetFiles("*.png", SearchOption.TopDirectoryOnly);
            foreach (FileInfo onePngImage in fi)
            {
                using (FileStream fs = File.OpenRead(onePngImage.FullName))
                {
                    List<UInt32> dimensions = StringTools.ParsingHelpers.FindIntegers(onePngImage.Name, onePngImage.Name.LastIndexOf('_'));

                    Int32 expected_width = Convert.ToInt32(dimensions[0]);
                    Int32 expected_height = Convert.ToInt32(dimensions[1]);

                    byte[] buffer = new byte[100];
                    fs.Read(buffer, 0, buffer.Length);
                    System.Drawing.Size res = ImageTools.GetPngSizeFromHeader(buffer);

                    Assert.AreEqual(expected_width, res.Width);
                    Assert.AreEqual(expected_height, res.Height);
                }
            }
        }
    }
}
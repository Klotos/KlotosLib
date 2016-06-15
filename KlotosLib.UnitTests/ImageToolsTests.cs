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
            if (safeCounter == Byte.MaxValue || binFolder == null)
            {
                throw new DirectoryNotFoundException("Cannot find a 'KlotosLib' directory");
            }
            DirectoryInfo unitTestProjectRootFolder = binFolder.GetDirectories("KlotosLib.UnitTests", SearchOption.TopDirectoryOnly)[0];
            DirectoryInfo sampleImagesFolder = unitTestProjectRootFolder.GetDirectories("SampleImages", SearchOption.TopDirectoryOnly)[0];
            ImageFolderPath = sampleImagesFolder.FullName;
        }

        private static readonly string ImageFolderPath;

        [Test]
        public void GetBmpSizeFromHeader()
        {
            DirectoryInfo imageFolder = new DirectoryInfo(ImageFolderPath);
            DirectoryInfo bmpFolder = imageFolder.GetDirectories("BMP", SearchOption.TopDirectoryOnly)[0];

            FileInfo[] bmpFiles = bmpFolder.GetFiles("*.bmp", SearchOption.TopDirectoryOnly);
            foreach (FileInfo oneBmpImage in bmpFiles)
            {
                using (FileStream fs = File.OpenRead(oneBmpImage.FullName))
                {
                    Substring size_descr = StringTools.SubstringHelpers.GetInnerStringBetweenTokens(oneBmpImage.Name,
                        "_", ".bmp", 0, 0, false, Direction.FromEndToStart, StringComparison.OrdinalIgnoreCase);
                    String[] sizes = size_descr.Value.Split('x');
                    Int32 expected_width = Int32.Parse(sizes[0], NumberStyles.Integer);
                    Int32 expected_height = Int32.Parse(sizes[1], NumberStyles.Integer);

                    byte[] buffer = new byte[26];
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
            DirectoryInfo imageFolder = new DirectoryInfo(ImageFolderPath);
            DirectoryInfo gifFolder = imageFolder.GetDirectories("GIF", SearchOption.TopDirectoryOnly)[0];

            FileInfo[] gifFiles = gifFolder.GetFiles("*.gif", SearchOption.TopDirectoryOnly);
            foreach (FileInfo oneGifImage in gifFiles)
            {
                using (FileStream fs = File.OpenRead(oneGifImage.FullName))
                {
                    List<UInt32> dimensions = StringTools.ParsingHelpers.FindIntegers(oneGifImage.Name, oneGifImage.Name.LastIndexOf('_'));

                    Int32 expected_width = Convert.ToInt32(dimensions[0]);
                    Int32 expected_height = Convert.ToInt32(dimensions[1]);

                    byte[] buffer = new byte[10];
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
            DirectoryInfo imageFolder = new DirectoryInfo(ImageFolderPath);
            DirectoryInfo pngFolder = imageFolder.GetDirectories("PNG", SearchOption.TopDirectoryOnly)[0];

            FileInfo[] pngFiles = pngFolder.GetFiles("*.png", SearchOption.TopDirectoryOnly);
            foreach (FileInfo onePngImage in pngFiles)
            {
                using (FileStream fs = File.OpenRead(onePngImage.FullName))
                {
                    List<UInt32> dimensions = StringTools.ParsingHelpers.FindIntegers(onePngImage.Name, onePngImage.Name.LastIndexOf('_'));

                    Int32 expected_width = Convert.ToInt32(dimensions[0]);
                    Int32 expected_height = Convert.ToInt32(dimensions[1]);

                    byte[] buffer = new byte[24];
                    fs.Read(buffer, 0, buffer.Length);
                    System.Drawing.Size res = ImageTools.GetPngSizeFromHeader(buffer);

                    Assert.AreEqual(expected_width, res.Width);
                    Assert.AreEqual(expected_height, res.Height);
                }
            }
        }

        [Test]
        public void GetJpegSizeFromHeader()
        {
            DirectoryInfo imageFolder = new DirectoryInfo(ImageFolderPath);
            DirectoryInfo jpegFolder = imageFolder.GetDirectories("JPEG", SearchOption.TopDirectoryOnly)[0];

            FileInfo[] jpg1 = jpegFolder.GetFiles("*.jpg", SearchOption.TopDirectoryOnly);
            FileInfo[] jpeg2 = jpegFolder.GetFiles("*.jpeg", SearchOption.TopDirectoryOnly);
            FileInfo[] combinedJpegFiles = CollectionTools.ConcatArrays(jpg1, jpeg2);

            foreach (FileInfo oneJpegImage in combinedJpegFiles)
            {
                using (FileStream fs = File.OpenRead(oneJpegImage.FullName))
                {
                    List<UInt32> dimensions = StringTools.ParsingHelpers.FindIntegers(oneJpegImage.Name, oneJpegImage.Name.LastIndexOf('_'));

                    Int32 expected_width = Convert.ToInt32(dimensions[0]);
                    Int32 expected_height = Convert.ToInt32(dimensions[1]);

                    Int64 expected_length = fs.Length < 1024*1024*1024 ? fs.Length : 1024*1024*1024;
                    byte[] buffer = new byte[expected_length];
                    fs.Read(buffer, 0, buffer.Length);
                    System.Drawing.Size res = ImageTools.GetJpegSizeFromHeader(buffer);

                    Assert.AreEqual(expected_width, res.Width);
                    Assert.AreEqual(expected_height, res.Height);
                }
            }
        }

        [Test]
        public void GetIconSizeFromHeader()
        {
            DirectoryInfo imageFolder = new DirectoryInfo(ImageFolderPath);
            DirectoryInfo iconFolder = imageFolder.GetDirectories("ICON", SearchOption.TopDirectoryOnly)[0];

            FileInfo[] iconFiles = iconFolder.GetFiles("*.ico", SearchOption.TopDirectoryOnly);
            foreach (FileInfo oneIconImage in iconFiles)
            {
                using (FileStream fs = File.OpenRead(oneIconImage.FullName))
                {
                    List<UInt32> dimensions = StringTools.ParsingHelpers.FindIntegers(oneIconImage.Name, oneIconImage.Name.LastIndexOf('_'));

                    Int32 expected_width = Convert.ToInt32(dimensions[0]);
                    Int32 expected_height = Convert.ToInt32(dimensions[1]);

                    byte[] buffer = new byte[8];
                    fs.Read(buffer, 0, buffer.Length);
                    System.Drawing.Size res = ImageTools.GetIconSizeFromHeader(buffer);

                    Assert.AreEqual(expected_width, res.Width, "File name: " + oneIconImage.FullName);
                    Assert.AreEqual(expected_height, res.Height, "File name: " + oneIconImage.FullName);
                }
            }
        }

        [Test]
        public void GetImageFormat()
        {
            DirectoryInfo imageFolder = new DirectoryInfo(ImageFolderPath);

            DirectoryInfo bmpFolder = imageFolder.GetDirectories("BMP", SearchOption.TopDirectoryOnly)[0];
            FileInfo[] bmpFiles = bmpFolder.GetFiles("*.bmp", SearchOption.TopDirectoryOnly);

            foreach (FileInfo oneBmpImage in bmpFiles)
            {
                Assert.AreEqual(ImageTools.ImageFormats.BMP, ParseFile(oneBmpImage, 26));
            }

            DirectoryInfo gifFolder = imageFolder.GetDirectories("GIF", SearchOption.TopDirectoryOnly)[0];
            FileInfo[] gifFiles = gifFolder.GetFiles("*.gif", SearchOption.TopDirectoryOnly);
            foreach (FileInfo oneGifImage in gifFiles)
            {
                Assert.AreEqual(ImageTools.ImageFormats.GIF, ParseFile(oneGifImage, 10));
            }

            DirectoryInfo pngFolder = imageFolder.GetDirectories("PNG", SearchOption.TopDirectoryOnly)[0];
            FileInfo[] pngFiles = pngFolder.GetFiles("*.png", SearchOption.TopDirectoryOnly);
            foreach (FileInfo onePngImage in pngFiles)
            {
                Assert.AreEqual(ImageTools.ImageFormats.PNG, ParseFile(onePngImage, 24));
            }

            DirectoryInfo jpegFolder = imageFolder.GetDirectories("JPEG", SearchOption.TopDirectoryOnly)[0];
            FileInfo[] jpg1 = jpegFolder.GetFiles("*.jpg", SearchOption.TopDirectoryOnly);
            FileInfo[] jpeg2 = jpegFolder.GetFiles("*.jpeg", SearchOption.TopDirectoryOnly);
            FileInfo[] combinedJpegFiles = CollectionTools.ConcatArrays(jpg1, jpeg2);

            foreach (FileInfo oneJpegImage in combinedJpegFiles)
            {
                Int32 expected_length = (Int32)(oneJpegImage.Length < 1024 * 1024 * 1024 ? oneJpegImage.Length : 1024 * 1024 * 1024);
                Assert.AreEqual(ImageTools.ImageFormats.JPEG, ParseFile(oneJpegImage, expected_length), "File: " + oneJpegImage.Name);
            }

            DirectoryInfo iconFolder = imageFolder.GetDirectories("ICON", SearchOption.TopDirectoryOnly)[0];
            FileInfo[] iconFiles = iconFolder.GetFiles("*.ico", SearchOption.TopDirectoryOnly);
            foreach (FileInfo oneIconImage in iconFiles)
            {
                Assert.AreEqual(ImageTools.ImageFormats.ICO, ParseFile(oneIconImage, 8));
            }
        }

        private ImageTools.ImageFormats ParseFile(FileInfo file, Int32 bufferSize)
        {
            using (FileStream fs = File.OpenRead(file.FullName))
            {
                byte[] buffer = new byte[bufferSize];
                fs.Read(buffer, 0, buffer.Length);

                ImageTools.ImageFormats format = ImageTools.GetImageFormat(buffer);
                return format;
            }
        }
    }
}
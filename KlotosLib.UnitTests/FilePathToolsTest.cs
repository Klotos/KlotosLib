using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [TestFixture]
    class FilePathToolsTest
    {
        [TestCase("filename.extension", ExpectedResult = "filename")]
        [TestCase("filename", ExpectedResult = "filename")]
        [TestCase(" \r\n ", ExpectedResult = " \r\n ")]
        [TestCase(null, ExpectedResult = null)]
        public String DeleteExtensionTest(String Input)
        {
            return FilePathTools.DeleteExtension(Input);
        }

        [TestCase("filename.extension", ExpectedResult = true)]
        [TestCase("", ExpectedResult = false)]
        [TestCase("  ", ExpectedResult = false)]
        [TestCase(" \r\n ", ExpectedResult = false)]
        [TestCase("filename  ", ExpectedResult = false)]
        [TestCase(" AUX.txt", ExpectedResult = false)]
        [TestCase("CON", ExpectedResult = false)]
        [TestCase("COM.con", ExpectedResult = true)]
        [TestCase("CONx", ExpectedResult = true)]
        [TestCase("prefix.CON.txt", ExpectedResult = true)]
        [TestCase("CON_txt", ExpectedResult = true)]
        [TestCase("film.avi", ExpectedResult = true)]
        public Boolean IsValidFilenameTest(String Input)
        {
            return FilePathTools.IsValidFilename(Input);
        }

        [TestCase("filename.extension", ExpectedResult = true)]
        [TestCase("", ExpectedResult = false)]
        [TestCase("  ", ExpectedResult = false)]
        [TestCase(" \r\n ", ExpectedResult = false)]
        [TestCase("filename  ", ExpectedResult = false)]
        [TestCase(" AUX.txt", ExpectedResult = false)]
        [TestCase("CON", ExpectedResult = false)]
        [TestCase("film.avi", ExpectedResult = true)]
        [TestCase("Библиотеки\\Документы", ExpectedResult = true)]
        [TestCase(@"C:\Users\Default\AppData\Local\Microsoft\Windows\Temporary Internet Files", ExpectedResult = true)]
        [TestCase(@"C:\Users\Default\AppData\Local\Microsoft\VisualStudio\10.0\vspdmc.lock", ExpectedResult = true)]
        [TestCase(@"\Folder1\Folder2\COM5.lock", ExpectedResult = false)]
        [TestCase(@"\Folder1\LPT6\file.txt", ExpectedResult = false)]
        [TestCase(@"\Folder1\LPT6x\file.txt", ExpectedResult = true)]
        [TestCase(@"\Folder1\  LPT6.abc\file.txt", ExpectedResult = false)]
        [TestCase(@"\Folder1\abc.LPT6\file.txt", ExpectedResult = true)]
        [TestCase(@"\Folder1\abc.LPT6\ aux.txt", ExpectedResult = false)]
        [TestCase(@"\Folder1\abc.LPT6\txt.aux", ExpectedResult = true)]
        [TestCase(@"I:\Downloads\folder - name (1234,5678)\file", ExpectedResult = true)]
        [TestCase(@"I:\Downloads\folder - name (1234,5678)\", ExpectedResult = true)]
        [TestCase(@"I:\Downloads\folder - name (1234,5678)", ExpectedResult = true)]
        [TestCase(@"\\server\Downloads\folder - name (1234,5678)\file", ExpectedResult = true)]
        [TestCase(@"\\server\Downloads\folder - name (1234,5678)\", ExpectedResult = true)]
        [TestCase(@"\\server\Downloads\folder - name (1234,5678)", ExpectedResult = true)]
        public Boolean IsValidFilePathTest(String Input)
        {
            return FilePathTools.IsValidFilePath(Input);
        }

        [TestCase("", ExpectedResult = null)]
        [TestCase("  ", ExpectedResult = null)]
        [TestCase(" \r\n ", ExpectedResult = null)]
        [TestCase(" :\\// ", ExpectedResult = null)]
        [TestCase(" AUX.txt ", ExpectedResult = null)]
        [TestCase(" con.bin.txt ", ExpectedResult = "bin.txt")]
        [TestCase(" bin.con.txt ", ExpectedResult = "bin.con.txt")]
        [TestCase(" bin.\\\\con.txt ", ExpectedResult = "bin.con.txt")]
        public String TryCleanFilenameTest(String Input)
        {
            String output;
            FilePathTools.TryCleanFilename(Input, out output);
            return output;
        }
    }
}

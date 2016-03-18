using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KlotosLib.StringTools;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [NUnit.Framework.TestFixture]
    public class StringToolsStringModifiersTests
    {
        [TestCase(null, Result = null)]
        [TestCase("", Result = "")]
        [TestCase("   ", Result = "   ")]
        [TestCase("+", Result = "+")]
        [TestCase(" - ", Result = " - ")]
        [TestCase("a", Result = "A")]
        [TestCase(" b", Result = " b")]
        [TestCase("abc", Result = "Abc")]
        [TestCase("Abc", Result = "Abc")]
        [TestCase("ABC", Result = "ABC")]
        public String FirstLetterToUpper(String Input)
        {
            return StringTools.StringModifiers.FirstLetterToUpper(Input);
        }

        [NUnit.Framework.Test]
        public void ReplaceAll()
        {
            const String base_str1 = "0123456789";
            Substring ss1 = Substring.FromIndexToIndex(base_str1, 0, 0);
            Substring ss2 = Substring.FromIndexToIndex(base_str1, 2, 3);
            Substring ss3 = Substring.FromIndexToIndex(base_str1, 5, 6);
            Substring ss4 = Substring.FromIndexToIndex(base_str1, 7, 7);
            Substring ss5 = Substring.FromIndexToIndex(base_str1, 8, 9);

            IDictionary<Substring, String> replacement_list1 = new Dictionary<Substring, string>(4)
            {
                {ss1, "first:"},
                {ss2, ""},
                {ss3, null},
                {ss4, "inner"},
                {ss5, ":last"}
            };
            String output1 = StringTools.StringModifiers.ReplaceAll(base_str1, replacement_list1);
            Assert.AreEqual("first:14inner:last", output1);

            IDictionary<Substring, String> replacement_list2 = new Dictionary<Substring, string>(0);
            String output2 = StringTools.StringModifiers.ReplaceAll(base_str1, replacement_list2);
            Assert.AreEqual(base_str1, output2);
            Assert.IsTrue(Object.ReferenceEquals(base_str1, output2));

            Assert.Throws<ArgumentNullException>(
                delegate { String output3 = StringTools.StringModifiers.ReplaceAll(null, replacement_list2); });

            Assert.Throws<ArgumentException>(
                delegate { String output4 = StringTools.StringModifiers.ReplaceAll("", replacement_list2); });

            Substring ss6 = Substring.FromIndexToIndex(base_str1, 7, 9);
            replacement_list1.Add(ss6, "bad");
            Assert.Throws<InvalidOperationException>(
                delegate { String output5 = StringTools.StringModifiers.ReplaceAll(base_str1, replacement_list1); });

            Substring ss7 = Substring.FromIndexToIndex("012345", 2, 3);
            replacement_list1.Add(ss7, "very bad");
            ArgumentException expected_exception = Assert.Throws<ArgumentException>(
                delegate { String output6 = StringTools.StringModifiers.ReplaceAll(base_str1, replacement_list1); });
            Assert.AreEqual("ReplacementList" ,expected_exception.ParamName);
        }
    }
}

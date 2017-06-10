using System;
using System.Reflection;
using KlotosLib.Angles;
using NUnit.Framework;

namespace KlotosLib.UnitTests.Angles
{
    [TestFixture]
    public class MeasurementUnitTest
    {
        [Test]
        public void TestBasicProperties()
        {
            MeasurementUnit mu0 = new MeasurementUnit();
            MeasurementUnit mu1 = MeasurementUnit.Turns;
            MeasurementUnit mu2 = MeasurementUnit.Radians;
            MeasurementUnit mu3 = MeasurementUnit.Degrees;
            MeasurementUnit mu4 = MeasurementUnit.BinaryDegrees;
            MeasurementUnit mu5 = MeasurementUnit.Quadrants;
            MeasurementUnit mu6 = MeasurementUnit.Sextants;
            MeasurementUnit mu7 = MeasurementUnit.Grads;

            Assert.AreEqual(mu0, MeasurementUnit.Uninitialized);
            Assert.AreNotEqual(mu0, mu1);
            Assert.AreNotEqual(mu1, mu2);
            Assert.AreNotEqual(mu2, mu3);
            Assert.AreNotEqual(mu3, mu4);
            Assert.AreNotEqual(mu4, mu5);
            Assert.AreNotEqual(mu5, mu6);
            Assert.AreNotEqual(mu6, mu7);

            Assert.IsFalse(mu0.IsInitialized);
            Assert.Throws<InvalidOperationException>(delegate
            {
                Double value = mu0.PositionsInOneTurn;
            });
            Assert.Throws<InvalidOperationException>(delegate
            {
                String value = mu0.Name;
            });
            Assert.Throws<InvalidOperationException>(delegate
            {
                String value = mu0.CssName;
            });
            Assert.Throws<InvalidOperationException>(delegate
            {
                String value = mu0.ToString();
            });
            
            Assert.AreEqual(1, mu1.PositionsInOneTurn);
            Assert.AreEqual(Math.PI * 2, mu2.PositionsInOneTurn);
            Assert.AreEqual(360, mu3.PositionsInOneTurn);
            Assert.AreEqual(256, mu4.PositionsInOneTurn);
            Assert.AreEqual(4, mu5.PositionsInOneTurn);
            Assert.AreEqual(6, mu6.PositionsInOneTurn);
            Assert.AreEqual(400, mu7.PositionsInOneTurn);

            Assert.IsTrue(mu1.IsInitialized);
            Assert.AreEqual("Turns", mu1.Name);
            Assert.AreEqual("Turns", mu1.ToString());
            Assert.AreEqual("turn", mu1.CssName);

            Assert.IsTrue(mu2.IsInitialized);
            Assert.AreEqual("Radians", mu2.Name);
            Assert.AreEqual("Radians", mu2.ToString());
            Assert.AreEqual("rad", mu2.CssName);

            Assert.IsTrue(mu3.IsInitialized);
            Assert.AreEqual("Degrees", mu3.Name);
            Assert.AreEqual("Degrees", mu3.ToString());
            Assert.AreEqual("deg", mu3.CssName);

            Assert.IsTrue(mu4.IsInitialized);
            Assert.AreEqual("Binary degrees", mu4.Name);
            Assert.AreEqual("Binary degrees", mu4.ToString());
            Assert.Throws<NotSupportedException>(delegate
            {
                string name = mu4.CssName;
            });

            Assert.IsTrue(mu5.IsInitialized);
            Assert.AreEqual("Quadrants", mu5.Name);
            Assert.AreEqual("Quadrants", mu5.ToString());
            Assert.Throws<NotSupportedException>(delegate
            {
                string name = mu5.CssName;
            });

            Assert.IsTrue(mu6.IsInitialized);
            Assert.AreEqual("Sextants", mu6.Name);
            Assert.AreEqual("Sextants", mu6.ToString());
            Assert.Throws<NotSupportedException>(delegate
            {
                string name = mu6.CssName;
            });

            Assert.IsTrue(mu7.IsInitialized);
            Assert.AreEqual("Grads", mu7.Name);
            Assert.AreEqual("Grads", mu7.ToString());
            Assert.AreEqual("grad", mu7.CssName);
        }

        [Test]
        public void TestEquality()
        {
            MeasurementUnit mu0 = MeasurementUnit.Uninitialized;
            MeasurementUnit mu1 = MeasurementUnit.Degrees;
            MeasurementUnit mu2 = MeasurementUnit.Grads;
            MeasurementUnit mu3 = MeasurementUnit.Radians;
            MeasurementUnit mu4 = MeasurementUnit.Degrees;

            Assert.IsFalse(mu1.Equals(mu2));
            object str = "string";
            Assert.IsFalse(mu2.Equals(str));
            Assert.IsTrue(mu1 == mu4);
            Assert.IsFalse(mu1 == mu3);
            Assert.IsTrue(mu3 != mu4);
            Assert.IsTrue(mu0.Equals(new MeasurementUnit()));
            Assert.IsTrue(mu0 != mu1);
            object boxed = (object) mu4;
            Assert.IsTrue(mu1.Equals(boxed));
            Assert.IsFalse(mu1.Equals(null));
            Assert.AreEqual(3, mu1.GetHashCode());
        }

        [Test]
        public void TestCracked()
        {
            Type inputType = typeof (MeasurementUnit);
            Object instance = Activator.CreateInstance(inputType);
            FieldInfo field = inputType.GetField("_type", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(field);
            const Byte value = 100;
            field.SetValue(instance, value);
            Assert.IsInstanceOf<MeasurementUnit>(instance);
            MeasurementUnit casted = (MeasurementUnit) instance;
            Assert.IsFalse(casted.IsInitialized);
            Assert.Throws<InvalidOperationException>(delegate
            {
                string name = casted.Name;
            });
            Assert.Throws<InvalidOperationException>(delegate
            {
                string name = casted.CssName;
            });
            Assert.Throws<InvalidOperationException>(delegate
            {
                double positions = casted.PositionsInOneTurn;
            });
        }

        [Test]
        public void TryParseTest()
        {
            MeasurementUnit resultant;

            bool check1 = MeasurementUnit.TryParse("degree", out resultant);
            Assert.IsTrue(check1);
            Assert.AreEqual(resultant, MeasurementUnit.Degrees);

            bool check2 = MeasurementUnit.TryParse(" Radians ", out resultant);
            Assert.IsTrue(check2);
            Assert.AreEqual(resultant, MeasurementUnit.Radians);

            bool check3 = MeasurementUnit.TryParse(" binary degreeS ", out resultant);
            Assert.IsTrue(check3);
            Assert.AreEqual(resultant, MeasurementUnit.BinaryDegrees);

            bool check4 = MeasurementUnit.TryParse(" SEXTANTS \r\n", out resultant);
            Assert.IsTrue(check4);
            Assert.AreEqual(resultant, MeasurementUnit.Sextants);

            bool check5 = MeasurementUnit.TryParse("\r\nQuadrants", out resultant);
            Assert.IsTrue(check5);
            Assert.AreEqual(resultant, MeasurementUnit.Quadrants);

            bool check6 = MeasurementUnit.TryParse(" gon", out resultant);
            Assert.IsTrue(check6);
            Assert.AreEqual(resultant, MeasurementUnit.Grads);

            bool check7 = MeasurementUnit.TryParse(" turns ", out resultant);
            Assert.IsTrue(check7);
            Assert.AreEqual(resultant, MeasurementUnit.Turns);

            bool check8 = MeasurementUnit.TryParse(" xxx ", out resultant);
            Assert.IsFalse(check8);
            Assert.AreEqual(resultant, MeasurementUnit.Uninitialized);

            bool check9 = MeasurementUnit.TryParse("", out resultant);
            Assert.IsFalse(check9);
            Assert.AreEqual(resultant, MeasurementUnit.Uninitialized);
        }

        [Test]
        public void ParseTest()
        {
            MeasurementUnit resultant = MeasurementUnit.Uninitialized;

            resultant = MeasurementUnit.Parse(" \tTURNS \r\n");
            Assert.AreEqual(MeasurementUnit.Turns, resultant);

            Assert.Throws<ArgumentException>(delegate
            {
                resultant = MeasurementUnit.Parse("");
            });

            Assert.Throws<FormatException>(delegate
            {
                resultant = MeasurementUnit.Parse("abcd");
            });
        }

        [Test]
        public void ConvertFromToTest()
        {
            double res1 = MeasurementUnit.ConvertFromTo(MeasurementUnit.Degrees, MeasurementUnit.Grads, 360);
            Assert.AreEqual(400.0, res1);

            double res2 = MeasurementUnit.ConvertFromTo(MeasurementUnit.Degrees, MeasurementUnit.Grads, 720);
            Assert.AreEqual(800.0, res2);

            double res3 = MeasurementUnit.ConvertFromTo(MeasurementUnit.Degrees, MeasurementUnit.Grads, -180);
            Assert.AreEqual(-200.0, res3);

            double res4 = MeasurementUnit.ConvertFromTo(MeasurementUnit.Radians, MeasurementUnit.Sextants, Math.PI);
            Assert.AreEqual(3.0, res4);

            double res5 = MeasurementUnit.ConvertFromTo(MeasurementUnit.Turns, MeasurementUnit.Turns, 1.5);
            Assert.AreEqual(1.5, res5);

            double res6 = MeasurementUnit.ConvertFromTo(MeasurementUnit.Uninitialized, MeasurementUnit.Uninitialized, 2.5);
            Assert.AreEqual(2.5, res6);

            Assert.Throws<ArgumentException>(delegate
            {
                double res7 = MeasurementUnit.ConvertFromTo(MeasurementUnit.Uninitialized, MeasurementUnit.Degrees, 1);
            });

            Assert.Throws<ArgumentException>(delegate
            {
                double res8 = MeasurementUnit.ConvertFromTo(MeasurementUnit.Turns, MeasurementUnit.Uninitialized, 1);
            });

            Assert.Throws<ArgumentException>(delegate
            {
                double res9 = MeasurementUnit.ConvertFromTo(MeasurementUnit.Turns, MeasurementUnit.BinaryDegrees, double.NaN);
            });
        }
    }
}
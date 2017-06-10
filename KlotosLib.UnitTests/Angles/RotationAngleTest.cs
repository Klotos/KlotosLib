using System;
using System.Globalization;
using KlotosLib.Angles;
using NUnit.Framework;

namespace KlotosLib.UnitTests.Angles
{
    [NUnit.Framework.TestFixture]
    public class RotationAngleTest
    {
        private static MeasurementUnit ConvertFromNumber(byte number)
        {
            MeasurementUnit output;
            if (number == 1)
            {
                output = MeasurementUnit.Turns;
            }
            else if (number == 2)
            {
                output = MeasurementUnit.Radians;
            }
            else if (number == 3)
            {
                output = MeasurementUnit.Degrees;
            }
            else if (number == 4)
            {
                output = MeasurementUnit.BinaryDegrees;
            }
            else if (number == 5)
            {
                output = MeasurementUnit.Quadrants;
            }
            else if (number == 6)
            {
                output = MeasurementUnit.Sextants;
            }
            else if (number == 7)
            {
                output = MeasurementUnit.Grads;
            }
            else
            {
                output = MeasurementUnit.Uninitialized;
            }
            return output;
        }

        [Test]
        public void ZeroAngles()
        {
            RotationAngle a0first = new RotationAngle();
            RotationAngle a0second = RotationAngle.Zero;
            
            Assert.AreEqual(a0first.Unit, MeasurementUnit.Uninitialized);
            Assert.AreEqual(a0second.Unit, MeasurementUnit.Uninitialized);
            
            Assert.AreEqual(a0first, a0second);
            Assert.IsTrue(a0first == a0second);
            Assert.IsFalse(a0first != a0second);
            Assert.IsFalse(a0first > a0second);
            Assert.IsFalse(a0first < a0second);
            Assert.IsTrue(a0first >= a0second);
            Assert.IsTrue(a0first <= a0second);

            Assert.IsTrue(a0first.IsZero);
            Assert.IsTrue(a0second.IsZero);
            Assert.IsTrue(a0first.IsUnitlessZero);
            Assert.IsTrue(a0second.IsUnitlessZero);

            Assert.IsFalse(a0first.IsClockwise);
            Assert.IsFalse(a0first.IsCounterClockwise);

            Assert.IsFalse(a0second.IsClockwise);
            Assert.IsFalse(a0second.IsCounterClockwise);

            Assert.AreEqual(0, a0first.NumberOfFullTurns);
            Assert.AreEqual(0, a0second.NumberOfFullTurns);

            Assert.AreEqual(0.0, a0first.GetValueInUnit(MeasurementUnit.Quadrants));
            Assert.AreEqual(0.0, a0first.GetValueInUnit(MeasurementUnit.Sextants));

            Assert.AreEqual(0.0, a0second.GetValueInUnit(MeasurementUnit.Quadrants));
            Assert.AreEqual(0.0, a0second.GetValueInUnit(MeasurementUnit.Sextants));
        }

        [Test]
        public void ImplicitCasting()
        {
            GeometricAngle ga1 = GeometricAngle.FromDegrees(270);
            RotationAngle ra1 = ga1;
            Assert.IsFalse(ra1.IsZero);
            Assert.IsFalse(ra1.IsUnitlessZero);
            Assert.AreEqual(270, ra1.Value);
            Assert.AreEqual(MeasurementUnit.Degrees, ra1.Unit);
            Assert.AreEqual(ga1.Value, ra1.Value);

            GeometricAngle ga2 = GeometricAngle.Zero;
            RotationAngle ra2 = ga2;
            Assert.IsTrue(ra2.IsZero);
            Assert.AreEqual(0, ra2.Value);
            Assert.AreEqual(MeasurementUnit.Uninitialized, ra2.Unit);
            Assert.AreEqual(ga2.Value, ra2.Value);

            GeometricAngle ga3 = GeometricAngle.FromSpecifiedUnit(Math.PI * 3.0, MeasurementUnit.Radians);
            RotationAngle ra3 = ga3;
            Assert.IsFalse(ra3.IsZero);
            Assert.AreEqual(Math.PI, ra3.Value);
            Assert.AreEqual(180, ra3.InDegrees);
            Assert.AreEqual(MeasurementUnit.Radians, ra3.Unit);
            Assert.AreEqual(ga3.Value, ra3.Value);
        }

        [Test]
        public void BasicPropertiesAndMethods()
        {
            RotationAngle a0 = RotationAngle.Zero;
            RotationAngle a1 = RotationAngle.FromDegrees(270);
            RotationAngle a2 = RotationAngle.FromQuadrants(3);
            RotationAngle a3 = RotationAngle.FromBinaryDegrees(-1024);
            RotationAngle a4 = RotationAngle.FromTurns(-4);
            RotationAngle a5 = RotationAngle.FromSextants(24);
            RotationAngle a6 = RotationAngle.FromGrads(1600);

            Assert.IsTrue(a0.Unit == MeasurementUnit.Uninitialized);
            Assert.IsTrue(a1.Unit == MeasurementUnit.Degrees);
            Assert.IsTrue(a2.Unit == MeasurementUnit.Quadrants);
            Assert.IsTrue(a3.Unit == MeasurementUnit.BinaryDegrees);
            Assert.IsTrue(a4.Unit == MeasurementUnit.Turns);
            Assert.IsTrue(a5.Unit == MeasurementUnit.Sextants);
            Assert.IsTrue(a6.Unit == MeasurementUnit.Grads);

            Assert.IsFalse(a0.IsClockwise);
            Assert.IsFalse(a0.IsCounterClockwise);
            Assert.IsTrue(a1.IsClockwise);
            Assert.IsFalse(a1.IsCounterClockwise);
            Assert.IsTrue(a2.IsClockwise);
            Assert.IsFalse(a2.IsCounterClockwise);
            Assert.IsFalse(a3.IsClockwise);
            Assert.IsTrue(a3.IsCounterClockwise);
            Assert.IsFalse(a4.IsClockwise);
            Assert.IsTrue(a4.IsCounterClockwise);
            Assert.IsTrue(a5.IsClockwise);
            Assert.IsFalse(a5.IsCounterClockwise);
            Assert.IsTrue(a6.IsClockwise);
            Assert.IsFalse(a6.IsCounterClockwise);

            Assert.AreEqual(0, a0.NumberOfFullTurns);
            Assert.AreEqual(0, a1.NumberOfFullTurns);
            Assert.AreEqual(0, a2.NumberOfFullTurns);
            Assert.AreEqual(4, a3.NumberOfFullTurns);
            Assert.AreEqual(4, a4.NumberOfFullTurns);
            Assert.AreEqual(4, a5.NumberOfFullTurns);
            Assert.AreEqual(4, a6.NumberOfFullTurns);

            Assert.IsTrue(a0.IsZero);
            Assert.IsFalse(a1.IsZero);
            Assert.IsFalse(a2.IsZero);
            Assert.IsFalse(a3.IsZero);
            Assert.IsFalse(a4.IsZero);
            Assert.IsFalse(a5.IsZero);
            Assert.IsFalse(a6.IsZero);

            Assert.IsTrue(a1 == a2);
            Assert.IsFalse(a1 == a0);
            Assert.IsTrue(a3 == a4);
            Assert.IsTrue(a4 != a5);
            Assert.IsTrue(a5 == a6);
            Assert.AreEqual(Math.Abs(a3.InTurns), Math.Abs(a5.InTurns));
            Assert.AreEqual(Math.Abs(a4.InTurns), Math.Abs(a5.InTurns));
            Assert.AreEqual(Math.Abs(a5.InTurns), Math.Abs(a6.InTurns));
            
            
            Assert.AreEqual(a1.InTurns, a2.InTurns);
            Assert.AreEqual(a1.InRadians, a2.InRadians);
            Assert.AreEqual(a1.InDegrees, a2.InDegrees);
            Assert.AreEqual(a1.InQuadrants, a2.InQuadrants);
            Assert.AreEqual(a1.InSextants, a2.InSextants);
            Assert.AreEqual(a1.InBinaryDegrees, a2.InBinaryDegrees);
            Assert.AreEqual(a1.InGrads, a2.InGrads);

            Assert.AreEqual(3.0, a1.GetValueInUnit(MeasurementUnit.Quadrants));
            Assert.AreEqual(192, a1.GetValueInUnit(MeasurementUnit.BinaryDegrees));
            Assert.Throws<ArgumentException>(delegate
            {
                a1.GetValueInUnit(MeasurementUnit.Uninitialized);
            });

            Assert.AreEqual(MeasurementUnit.Uninitialized, a0.Unit);
            Assert.AreEqual(0, a0.Value);
            Assert.AreEqual(0, a0.InTurns);
            Assert.AreEqual(0, a0.InRadians);
            Assert.AreEqual(0, a0.InDegrees);
            Assert.AreEqual(0, a0.InBinaryDegrees);
            Assert.AreEqual(0, a0.InQuadrants);
            Assert.AreEqual(0, a0.InSextants);
            Assert.AreEqual(0, a0.InGrads);
            
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                RotationAngle a = RotationAngle.FromTurns(Double.NegativeInfinity);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                RotationAngle a = RotationAngle.FromRadians(Double.NaN);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                RotationAngle a = RotationAngle.FromDegrees(Double.PositiveInfinity);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                RotationAngle a = RotationAngle.FromBinaryDegrees(Double.NaN);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                RotationAngle a = RotationAngle.FromQuadrants(Double.PositiveInfinity);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                RotationAngle a = RotationAngle.FromSextants(Double.NegativeInfinity);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                RotationAngle a = RotationAngle.FromGrads(Double.NaN);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                RotationAngle a = RotationAngle.FromSpecifiedUnit(Double.PositiveInfinity, MeasurementUnit.Grads);
            });
        }

        [TestCase(90, 3, 90, 3, ExpectedResult = true)]
        [TestCase(590, 3, 590, 3, ExpectedResult = true)]
        [TestCase(90, 3, 100, 7, ExpectedResult = true)]
        [TestCase(360, 3, 400, 7, ExpectedResult = true)]
        [TestCase(720, 3, 800, 7, ExpectedResult = true)]
        [TestCase(-720, 3, -800, 7, ExpectedResult = true)]
        [TestCase(-720, 3, 800, 7, ExpectedResult = false)]
        [TestCase(720, 3, -800, 7, ExpectedResult = false)]
        [TestCase(360, 3, 800, 7, ExpectedResult = false)]
        [TestCase(90, 3, 200, 7, ExpectedResult = false)]
        [TestCase(Math.PI / 2.0, 2, 100, 7, ExpectedResult = true)]
        [TestCase(100, 7, Math.PI / 2.0, 2, ExpectedResult = true)]
        [TestCase(3, 6, 2, 5, ExpectedResult = true)]
        [TestCase(2, 5, 3, 6, ExpectedResult = true)]
        [TestCase(3, 6, 3, 5, ExpectedResult = false)]
        [TestCase(Math.PI, 2, 1, 1, ExpectedResult = false)]
        [TestCase(Math.PI, 2, 0.5, 1, ExpectedResult = true)]
        [TestCase(Math.PI * 2.0, 2, 1, 1, ExpectedResult = true)]
        [TestCase(270, 3, 3, 5, ExpectedResult = true)]
        [TestCase(270, 3, 3, 6, ExpectedResult = false)]
        [TestCase(180, 3, 540, 3, ExpectedResult = false)]
        [TestCase(360, 3, 720, 3, ExpectedResult = false)]
        [TestCase(0, 3, 0, 3, ExpectedResult = true)]
        [TestCase(0, 3, -360, 3, ExpectedResult = false)]
        [TestCase(360, 3, -360, 3, ExpectedResult = false)]
        [TestCase(-450, 3, 270, 3, ExpectedResult = false)]
        [TestCase(270, 3, 3, 7, ExpectedResult = false)]
        [TestCase(270, 3, 10, 8, ExpectedException = typeof(ArgumentException))]
        [TestCase(270, 8, 10, 7, ExpectedException = typeof(ArgumentException))]
        [TestCase(270, 8, 10, 8, ExpectedException = typeof(ArgumentException))]
        public Boolean Equality1(
            Double angle1Value, Byte angle1MeasurementUnit,
            Double angle2Value, Byte angle2MeasurementUnit
            )
        {
            MeasurementUnit angle1MeasurementUnitCasted = ConvertFromNumber(angle1MeasurementUnit);
            MeasurementUnit angle2MeasurementUnitCasted = ConvertFromNumber(angle2MeasurementUnit);

            RotationAngle first = RotationAngle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnitCasted);
            RotationAngle second = RotationAngle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnitCasted);
            return RotationAngle.AreEqual(first, second);
        }

        [Test]
        public void Equality2()
        {
            RotationAngle a1 = RotationAngle.FromRadians(Math.PI * 3);
            RotationAngle a2 = RotationAngle.FromBinaryDegrees(384);
            RotationAngle a3 = RotationAngle.FromSextants(9);
            Assert.AreEqual(a1, a2);
            Assert.AreEqual(a1, a3);
            Assert.AreEqual(a2, a3);
            RotationAngle a4 = RotationAngle.FromTurns(-0.25);
            RotationAngle a5 = RotationAngle.FromRadians(-Math.PI / 2.0);
            Assert.AreEqual(a4, a5);
            Assert.IsTrue(RotationAngle.AreEqual(a1, a2, a3));
            Assert.IsTrue(RotationAngle.AreEqual(new RotationAngle[2] { a2, a3 }));
            Assert.IsFalse(RotationAngle.AreEqual(a1, a2, a3, a4));
            Assert.IsFalse(RotationAngle.AreEqual(a1, a2, a3, a4, a5));
            Assert.Throws<ArgumentNullException>(delegate { Boolean res = RotationAngle.AreEqual(null); });
            Assert.Throws<ArgumentException>(delegate { Boolean res = RotationAngle.AreEqual(new RotationAngle[0] { }); });
            Assert.Throws<ArgumentException>(delegate { Boolean res = RotationAngle.AreEqual(a1); });

            Assert.IsFalse(a1.Equals(null));
            Assert.IsFalse(a1.Equals(new Object()));
            Assert.IsFalse(a3.Equals((Object)a5));
            Assert.IsTrue(a2.Equals((Object)a3));
        }

        [Test]
        public void Equality3()
        {
            RotationAngle a1 = new RotationAngle();
            RotationAngle a2 = RotationAngle.Zero;
            RotationAngle a3 = RotationAngle.FromDegrees(90);

            Assert.AreEqual(a1, a2);
            Assert.AreEqual(a3, a3);
            Assert.AreNotEqual(a1, a3);
            Assert.AreNotEqual(a3, a2);
        }

        [Test]
        public void GetHashCodeTest()
        {
            RotationAngle a1 = RotationAngle.FromRadians(Math.PI);
            Int32 expectedHashCode = Math.PI.GetHashCode();
            expectedHashCode = (expectedHashCode * 397) ^ MeasurementUnit.Radians.GetHashCode();
            Assert.AreEqual(expectedHashCode, a1.GetHashCode());
        }

        [TestCase(90, 3, 90, 3, ExpectedResult = 0)]
        [TestCase(90, 3, 0, 3, ExpectedResult = 1)]
        [TestCase(90, 3, 180, 3, ExpectedResult = -1)]
        [TestCase(180, 3, 90, 3, ExpectedResult = 1)]
        [TestCase(Math.PI, 2, 199, 7, ExpectedResult = 1)]
        [TestCase(399, 7, Math.PI * 2.0, 2, ExpectedResult = -1)]
        [TestCase(-399, 7, Math.PI * 2.0, 2, ExpectedResult = -1)]
        [TestCase(399, 7, -Math.PI * 2.0, 2, ExpectedResult = 1)]
        [TestCase(-399, 7, -Math.PI * 2.0, 2, ExpectedResult = 1)]
        [TestCase(0, 3, 1, 1, ExpectedResult = -1)]
        [TestCase(0, 3, 0, 1, ExpectedResult = 0)]
        [TestCase(0, 0, 0, 0, ExpectedResult = 0)]
        public Int32 Comparison1(
            Double angle1Value, Byte angle1MeasurementUnit,
            Double angle2Value, Byte angle2MeasurementUnit
            )
        {
            MeasurementUnit angle1MeasurementUnitCasted = ConvertFromNumber(angle1MeasurementUnit);
            MeasurementUnit angle2MeasurementUnitCasted = ConvertFromNumber(angle2MeasurementUnit);

            RotationAngle first = RotationAngle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnitCasted);
            RotationAngle second = RotationAngle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnitCasted);
            Int32 firstComparison = first.CompareTo((Object)second);
            Int32 secondComparison = first.CompareTo(second);
            Int32 thirdComparison = RotationAngle.Compare(first, second);
            Assert.IsTrue(firstComparison == secondComparison && secondComparison == thirdComparison);
            return firstComparison;
        }

        [Test]
        public void Comparison2()
        {
            RotationAngle a1 = RotationAngle.FromBinaryDegrees(768);
            Assert.Throws<ArgumentNullException>(delegate { Int32 res1 = a1.CompareTo(null); });
            Assert.Throws<InvalidOperationException>(delegate { Int32 res2 = a1.CompareTo("abcd"); });
        }

        [TestCase(90, 3, 90, 3, 1, ExpectedResult = true)]
        [TestCase(90, 3, 90, 3, 2, ExpectedResult = false)]
        [TestCase(90, 3, 128, 4, 1, ExpectedResult = false)]
        [TestCase(90, 3, 128, 4, 2, ExpectedResult = true)]
        [TestCase(3, 6, 128, 4, 1, ExpectedResult = true)]
        [TestCase(3, 6, 128, 4, 2, ExpectedResult = false)]
        [TestCase(3, 6, 128, 4, 5, ExpectedResult = true)]
        [TestCase(3, 6, 128, 4, 6, ExpectedResult = true)]
        [TestCase(3, 6, 128, 4, 3, ExpectedResult = false)]
        [TestCase(3, 6, 128, 4, 4, ExpectedResult = false)]
        [TestCase(3, 5, Math.PI, 1, 1, ExpectedResult = false)]
        [TestCase(3, 5, Math.PI, 1, 2, ExpectedResult = true)]
        [TestCase(3, 5, Math.PI, 1, 3, ExpectedResult = false)]
        [TestCase(3, 5, Math.PI, 1, 4, ExpectedResult = true)]
        [TestCase(3, 5, Math.PI, 1, 5, ExpectedResult = false)]
        [TestCase(3, 5, Math.PI, 1, 6, ExpectedResult = true)]

        [TestCase(-3, 6, -Math.PI, 2, 1, ExpectedResult = true)]
        [TestCase(-3, 6, -Math.PI, 2, 2, ExpectedResult = false)]
        [TestCase(-3, 6, -Math.PI, 2, 3, ExpectedResult = false)]
        [TestCase(-3, 6, -Math.PI, 2, 4, ExpectedResult = false)]
        [TestCase(-3, 6, -Math.PI, 2, 5, ExpectedResult = true)]
        [TestCase(-3, 6, -Math.PI, 2, 6, ExpectedResult = true)]

        [TestCase(-1.5, 1, -Math.PI, 2, 1, ExpectedResult = false)]
        [TestCase(-1.5, 1, -Math.PI, 2, 2, ExpectedResult = true)]
        [TestCase(-1.5, 1, -Math.PI, 2, 3, ExpectedResult = false)]
        [TestCase(-1.5, 1, -Math.PI, 2, 4, ExpectedResult = true)]
        [TestCase(-1.5, 1, -Math.PI, 2, 5, ExpectedResult = false)]
        [TestCase(-1.5, 1, -Math.PI, 2, 6, ExpectedResult = true)]

        [TestCase(1.5, 1, 3, 5, 1, ExpectedResult = false)]
        [TestCase(1.5, 1, 3, 5, 2, ExpectedResult = true)]
        [TestCase(1.5, 1, 3, 5, 3, ExpectedResult = true)]
        [TestCase(1.5, 1, 3, 5, 4, ExpectedResult = false)]
        [TestCase(1.5, 1, 3, 5, 5, ExpectedResult = true)]
        [TestCase(1.5, 1, 3, 5, 6, ExpectedResult = false)]
        [TestCase(1.5, 1, 3, 5, 7, ExpectedException = typeof(InvalidOperationException))]
        public Boolean EqualityGreaterLesserOperators(
            Double angle1Value, Byte angle1MeasurementUnit,
            Double angle2Value, Byte angle2MeasurementUnit,
            Byte operatorType)//operatorType: 1 - ==; 2 - !=; 3 - >; 4 - <; 5 - >=; 6 - <=
        {
            MeasurementUnit angle1MeasurementUnitCasted = ConvertFromNumber(angle1MeasurementUnit);
            MeasurementUnit angle2MeasurementUnitCasted = ConvertFromNumber(angle2MeasurementUnit);

            RotationAngle first = RotationAngle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnitCasted);
            RotationAngle second = RotationAngle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnitCasted);
            switch (operatorType)
            {
                case 1:
                    return first == second;
                case 2:
                    return first != second;
                case 3:
                    return first > second;
                case 4:
                    return first < second;
                case 5:
                    return first >= second;
                case 6:
                    return first <= second;
                default:
                    throw new InvalidOperationException();
            }
        }

        [TestCase(30, 3, 30, 3, 3, ExpectedResult = 60)]
        [TestCase(270, 3, 180, 3, 3, ExpectedResult = 450)]
        [TestCase(-30, 3, -40, 3, 3, ExpectedResult = -70)]
        [TestCase(-800, 3, 270, 3, 3, ExpectedResult = -530)]

        [TestCase(90, 3, 90, 3, 2, ExpectedResult = Math.PI)]
        [TestCase(180, 3, 180, 3, 3, ExpectedResult = 360)]
        [TestCase(-180, 3, -180, 3, 2, ExpectedResult = -Math.PI * 2.0)]
        [TestCase(-180, 3, 180, 3, 2, ExpectedResult = 0.0)]
        [TestCase(180, 3, 270, 3, 3, ExpectedResult = 450)]
        [TestCase(270, 3, 180, 3, 3, ExpectedResult = 450)]
        [TestCase(270, 3, -180, 3, 3, ExpectedResult = 90)]
        [TestCase(-180, 3, 270, 3, 3, ExpectedResult = 90)]
        [TestCase(-270, 3, -180, 3, 3, ExpectedResult = -450)]
        [TestCase(270, 3, 270, 3, 2, ExpectedResult = Math.PI * 3)]
        [TestCase(-270, 3, -270, 3, 2, ExpectedResult = -Math.PI * 3)]
        [TestCase(270, 3, 192, 4, 2, ExpectedResult = Math.PI * 3)]
        [TestCase(256, 4, 400, 7, 6, ExpectedResult = 12)]
        public Double Addition(
            Double angle1Value, Byte angle1MeasurementUnit,
            Double angle2Value, Byte angle2MeasurementUnit,
            Byte outputAngleMeasurementUnit)
        {
            MeasurementUnit angle1MeasurementUnitCasted = ConvertFromNumber(angle1MeasurementUnit);
            MeasurementUnit angle2MeasurementUnitCasted = ConvertFromNumber(angle2MeasurementUnit);
            MeasurementUnit outputAngleMeasurementUnitCasted = ConvertFromNumber(outputAngleMeasurementUnit);

            RotationAngle first = RotationAngle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnitCasted);
            RotationAngle second = RotationAngle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnitCasted);
            RotationAngle sum = first + second;
            return sum.GetValueInUnit(outputAngleMeasurementUnitCasted);
        }

        [TestCase(540, 3, 360, 3, 3, ExpectedResult = 180)]
        [TestCase(180, 3, 270, 3, 3, ExpectedResult = -90)]
        [TestCase(-90, 3, 270, 3, 3, ExpectedResult = -360)]
        [TestCase(380, 3, -340, 3, 3, ExpectedResult = 720)]

        [TestCase(90, 3, 90, 3, 2, ExpectedResult = 0.0)]
        [TestCase(90, 3, Math.PI, 2, 5, ExpectedResult = -1)]
        [TestCase(Math.PI, 2, 3, 5, 7, ExpectedResult = -100)]
        [TestCase(64, 4, 192, 4, 6, ExpectedResult = -3)]
        [TestCase(-512, 4, -1, 1, 1, ExpectedResult = -1)]
        public Double Subtraction(
            Double angle1Value, Byte angle1MeasurementUnit,
            Double angle2Value, Byte angle2MeasurementUnit,
            Byte outputAngleMeasurementUnit)
        {
            MeasurementUnit angle1MeasurementUnitCasted = ConvertFromNumber(angle1MeasurementUnit);
            MeasurementUnit angle2MeasurementUnitCasted = ConvertFromNumber(angle2MeasurementUnit);
            MeasurementUnit outputAngleMeasurementUnitCasted = ConvertFromNumber(outputAngleMeasurementUnit);

            RotationAngle minuend = RotationAngle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnitCasted);
            RotationAngle subtrahend = RotationAngle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnitCasted);
            RotationAngle difference = minuend - subtrahend;
            return difference.GetValueInUnit(outputAngleMeasurementUnitCasted);
        }

        [TestCase(90, 3, ExpectedResult = "90 Degrees")]
        [TestCase(300, 7, ExpectedResult = "300 Grads")]
        [TestCase(-5, 6, ExpectedResult = "-5 Sextants")]
        [TestCase(0.1, 1, ExpectedResult = "0.1 Turns")]
        [TestCase(-1.15, 1, ExpectedResult = "-1.15 Turns")]
        public String ToString(Double angleValue, Byte angleMeasurementUnit)
        {
            MeasurementUnit angleMeasurementUnitCasted = ConvertFromNumber(angleMeasurementUnit);

            RotationAngle rotationAngle = RotationAngle.FromSpecifiedUnit(angleValue, angleMeasurementUnitCasted);
            return rotationAngle.ToString();
        }

        [TestCase(90, 3, "G", true, ExpectedResult = "90 Degrees")]
        [TestCase(-720, 3, "G", true, ExpectedResult = "-720 Degrees")]
        [TestCase(-384, 4, "G", true, ExpectedResult = "-384 Binary degrees")]
        [TestCase(384, 4, "G", true, ExpectedResult = "384 Binary degrees")]
        [TestCase(55.5, 4, "000.00", true, ExpectedResult = "055.50 Binary degrees")]
        [TestCase(55.5, 4, "000.00", false, ExpectedResult = "055,50 Binary degrees")]
        [TestCase(55.56, 7, "00.0", true, ExpectedResult = "55.6 Grads")]
        [TestCase(55.56, 7, "00.0", false, ExpectedResult = "55,6 Grads")]
        public String ToStringFormat(Double angleValue, Byte angleMeasurementUnit, string format, bool dotOrComma)
        {
            MeasurementUnit angleMeasurementUnitCasted = ConvertFromNumber(angleMeasurementUnit);

            RotationAngle rotationAngle = RotationAngle.FromSpecifiedUnit(angleValue, angleMeasurementUnitCasted);
            NumberFormatInfo dotNum = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            dotNum.NumberDecimalSeparator = ".";
            NumberFormatInfo commaNum = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            commaNum.NumberDecimalSeparator = ",";
            IFormatProvider formatProvider = dotOrComma ? dotNum : commaNum;
            return rotationAngle.ToString(format, formatProvider);
        }

        [TestCase(90, 3, ExpectedResult = "90deg")]
        [TestCase(-720, 3, ExpectedResult = "-720deg")]
        [TestCase(1.1, 1, ExpectedResult = "1.1turn")]
        [TestCase(-1.1, 1, ExpectedResult = "-1.1turn")]
        [TestCase(3.14, 2, ExpectedResult = "3.14rad")]
        [TestCase(-6.28, 2, ExpectedResult = "-6.28rad")]
        [TestCase(200, 7, ExpectedResult = "200grad")]
        [TestCase(-500, 7, ExpectedResult = "-500grad")]
        [TestCase(3, 5, ExpectedException = typeof(NotSupportedException))]
        [TestCase(4, 0, ExpectedException = typeof(ArgumentException))]
        public String ToCssString(Double angleValue, Byte angleMeasurementUnit)
        {
            MeasurementUnit angleMeasurementUnitCasted = ConvertFromNumber(angleMeasurementUnit);

            RotationAngle rotationAngle = RotationAngle.FromSpecifiedUnit(angleValue, angleMeasurementUnitCasted);
            return rotationAngle.ToCssString();
        }

        [Test]
        public void Clone()
        {
            RotationAngle first = RotationAngle.FromDegrees(90);
            RotationAngle second = first.Clone();
            Assert.AreEqual(first, second);
            first = RotationAngle.FromGrads(300);
            Assert.AreNotEqual(first, second);
            Object third = ((ICloneable)second).Clone();
            Assert.IsInstanceOf<RotationAngle>(third);
            Assert.IsTrue(second.Equals(third));
        }

        [Test]
        public void GetGeometricAngle()
        {
            RotationAngle a0 = new RotationAngle();
            GeometricAngle g0_1 = a0.GetGeometricAngle(true);
            GeometricAngle g0_2 = a0.GetGeometricAngle(false);
            Assert.AreEqual(0, g0_1.Value);
            Assert.AreEqual(0, g0_2.Value);
            Assert.AreEqual(a0.Value, g0_1.Value);
            Assert.AreEqual(a0.Value, g0_2.Value);
            Assert.AreEqual(g0_1, g0_2);

            RotationAngle a1 = RotationAngle.FromBinaryDegrees(128);
            GeometricAngle g1_1 = a1.GetGeometricAngle(true);
            GeometricAngle g1_2 = a1.GetGeometricAngle(false);
            Assert.AreEqual(128, g1_1.Value);
            Assert.AreEqual(128, g1_2.Value);
            Assert.AreEqual(a1.Value, g1_1.Value);
            Assert.AreEqual(a1.Value, g1_2.Value);
            Assert.AreEqual(g1_1, g1_2);

            RotationAngle a2 = RotationAngle.FromDegrees(45);
            GeometricAngle g2_1 = a2.GetGeometricAngle(true);
            GeometricAngle g2_2 = a2.GetGeometricAngle(false);
            Assert.AreEqual(45, g2_1.Value);
            Assert.AreEqual(45, g2_2.Value);
            Assert.AreEqual(a2.Value, g2_1.Value);
            Assert.AreEqual(a2.Value, g2_2.Value);
            Assert.AreEqual(g2_1, g2_2);

            RotationAngle a3 = RotationAngle.FromDegrees(405);
            GeometricAngle g3_1 = a3.GetGeometricAngle(true);
            GeometricAngle g3_2 = a3.GetGeometricAngle(false);
            Assert.AreEqual(45, g3_1.Value);
            Assert.AreEqual(45, g3_2.Value);
            Assert.AreNotEqual(a3.Value, g3_1.Value);
            Assert.AreNotEqual(a3.Value, g3_2.Value);
            Assert.AreEqual(g3_1, g3_2);

            RotationAngle a4 = RotationAngle.FromDegrees(-45);
            GeometricAngle g4_1 = a4.GetGeometricAngle(true);
            GeometricAngle g4_2 = a4.GetGeometricAngle(false);
            Assert.AreEqual(45, g4_1.Value);
            Assert.AreEqual(315, g4_2.Value);
            Assert.AreNotEqual(a4.Value, g4_1.Value);
            Assert.AreNotEqual(a4.Value, g4_2.Value);
            Assert.AreNotEqual(g4_1, g4_2);

            RotationAngle a5 = RotationAngle.FromDegrees(-405);
            GeometricAngle g5_1 = a5.GetGeometricAngle(true);
            GeometricAngle g5_2 = a5.GetGeometricAngle(false);
            Assert.AreEqual(45, g5_1.Value);
            Assert.AreEqual(315, g5_2.Value);
            Assert.AreNotEqual(a5.Value, g5_1.Value);
            Assert.AreNotEqual(a5.Value, g5_2.Value);
            Assert.AreNotEqual(g5_1, g5_2);
        }
    }
}
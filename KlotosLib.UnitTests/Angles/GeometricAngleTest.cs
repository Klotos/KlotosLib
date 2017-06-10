using System;
using System.Globalization;
using System.Reflection;
using KlotosLib.Angles;
using NUnit.Framework;

namespace KlotosLib.UnitTests.Angles
{
    [NUnit.Framework.TestFixture]
    public class GeometricAngleTest
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
        public void BasicPropertiesAndMethods()
        {
            GeometricAngle a0 = GeometricAngle.Zero;
            GeometricAngle a1 = GeometricAngle.FromDegrees(270);
            GeometricAngle a2 = GeometricAngle.FromQuadrants(3);
            Assert.IsTrue(a1 == a2);
            Assert.IsTrue(a0.IsZero);
            Assert.IsTrue(a0.IsUnitlessZero);
            Assert.IsFalse(a1 == a0);
            Assert.IsFalse(a1.IsZero);
            Assert.IsFalse(a1.IsUnitlessZero);
            Assert.IsTrue(a1.Unit == MeasurementUnit.Degrees);
            Assert.IsTrue(a2.Unit == MeasurementUnit.Quadrants);

            Assert.AreEqual(a1.InTurns, a2.InTurns);
            Assert.AreEqual(a1.InRadians, a2.InRadians);
            Assert.AreEqual(a1.InDegrees, a2.InDegrees);
            Assert.AreEqual(a1.InQuadrants, a2.InQuadrants);
            Assert.AreEqual(a1.InSextants, a2.InSextants);
            Assert.AreEqual(a1.InBinaryDegrees, a2.InBinaryDegrees);
            Assert.AreEqual(a1.InGrads, a2.InGrads);
            Assert.AreEqual(a1.InRange, a2.InRange);
            Assert.IsTrue(a1.InRange == AngleRanges.Reflex);
            
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
            Assert.AreEqual(AngleRanges.Zero, a0.InRange);

            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                GeometricAngle a = GeometricAngle.FromTurns(Double.NegativeInfinity);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                GeometricAngle a = GeometricAngle.FromRadians(Double.NaN);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                GeometricAngle a = GeometricAngle.FromDegrees(Double.PositiveInfinity);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                GeometricAngle a = GeometricAngle.FromBinaryDegrees(Double.NaN);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                GeometricAngle a = GeometricAngle.FromQuadrants(Double.PositiveInfinity);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                GeometricAngle a = GeometricAngle.FromSextants(Double.NegativeInfinity);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                GeometricAngle a = GeometricAngle.FromGrads(Double.NaN);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                GeometricAngle a = GeometricAngle.FromSpecifiedUnit(Double.PositiveInfinity, MeasurementUnit.Grads);
            });
        }

        [TestCase(0.125, 1, 1, ExpectedResult = 0.125)]
        [TestCase(80, 3, 3, ExpectedResult = 10)]
        [TestCase(32, 4, 2, ExpectedResult = Math.PI / 4.0)]
        [TestCase(0, 0, 2, ExpectedResult = Math.PI / 2.0)]
        [TestCase(100, 3, 3, ExpectedException = typeof(InvalidOperationException))]
        public Double GetComplementaryAngle(Double value, Byte inputUnit, Byte outputUnit)
        {
            MeasurementUnit inputUnitCasted = ConvertFromNumber(inputUnit);
            MeasurementUnit outputUnitCasted = ConvertFromNumber(outputUnit);
            GeometricAngle inputAngle = GeometricAngle.FromSpecifiedUnit(value, inputUnitCasted);
            GeometricAngle complementaryAngle = inputAngle.GetComplementaryAngle();
            return complementaryAngle.GetValueInUnit(outputUnitCasted);
        }

        [TestCase(0.125, 1, 1, ExpectedResult = 0.375)]
        [TestCase(0.125, 1, 3, ExpectedResult = 135)]
        [TestCase(80, 3, 3, ExpectedResult = 100)]
        [TestCase(32, 4, 5, ExpectedResult = 1.5)]
        [TestCase(0, 0, 2, ExpectedResult = Math.PI)]
        [TestCase(200, 3, 3, ExpectedException = typeof(InvalidOperationException))]
        public Double GetSupplementaryAngle(Double value, Byte inputUnit, Byte outputUnit)
        {
            MeasurementUnit inputUnitCasted = ConvertFromNumber(inputUnit);
            MeasurementUnit outputUnitCasted = ConvertFromNumber(outputUnit);
            GeometricAngle inputAngle = GeometricAngle.FromSpecifiedUnit(value, inputUnitCasted);
            GeometricAngle supplementaryAngle = inputAngle.GetSupplementaryAngle();
            return supplementaryAngle.GetValueInUnit(outputUnitCasted);
        }

        [TestCase(0.125, 1, 1, ExpectedResult = 0.875)]
        [TestCase(0.125, 1, 3, ExpectedResult = 315)]
        [TestCase(0.125, 1, 5, ExpectedResult = 3.5)]
        [TestCase(0.5, 5, 5, ExpectedResult = 3.5)]
        [TestCase(80, 3, 3, ExpectedResult = 280)]
        [TestCase(64, 4, 6, ExpectedResult = 4.5)]
        [TestCase(0, 0, 2, ExpectedResult = Math.PI * 2)]
        [TestCase(256, 4, 3, ExpectedResult = 0)]
        public Double GetExplementaryAngle(Double value, Byte inputUnit, Byte outputUnit)
        {
            MeasurementUnit inputUnitCasted = ConvertFromNumber(inputUnit);
            MeasurementUnit outputUnitCasted = ConvertFromNumber(outputUnit);
            GeometricAngle inputAngle = GeometricAngle.FromSpecifiedUnit(value, inputUnitCasted);
            GeometricAngle explementaryAngle = inputAngle.GetExplementaryAngle();
            return explementaryAngle.GetValueInUnit(outputUnitCasted);
        }

        [Test]
        public void FromNamedAngle()
        {
            GeometricAngle angle1 = GeometricAngle.FromNamedAngle(NamedAngles.Zero, MeasurementUnit.Grads);
            Assert.AreEqual(0, angle1.InTurns);
            Assert.AreEqual(0, angle1.InRadians);
            Assert.AreEqual(0, angle1.InDegrees);
            Assert.AreEqual(0, angle1.InBinaryDegrees);
            Assert.AreEqual(0, angle1.InQuadrants);
            Assert.AreEqual(0, angle1.InSextants);
            Assert.AreEqual(0, angle1.InGrads);
            Assert.IsTrue(angle1.Unit == MeasurementUnit.Grads);
            Assert.AreEqual(0, angle1.Value);

            GeometricAngle angle3 = GeometricAngle.FromNamedAngle(NamedAngles.Right, MeasurementUnit.Degrees);
            Assert.AreEqual(0.25, angle3.InTurns);
            Assert.AreEqual(Math.PI / 2.0, angle3.InRadians);
            Assert.AreEqual(90, angle3.InDegrees);
            Assert.AreEqual(64, angle3.InBinaryDegrees);
            Assert.AreEqual(1, angle3.InQuadrants);
            Assert.AreEqual(1.5, angle3.InSextants);
            Assert.AreEqual(100, angle3.InGrads);
            Assert.IsTrue(angle3.Unit == MeasurementUnit.Degrees);
            Assert.AreEqual(90, angle3.Value);

            GeometricAngle angle6 = GeometricAngle.FromNamedAngle(NamedAngles.Straight, MeasurementUnit.Turns);
            Assert.AreEqual(0.5, angle6.InTurns);
            Assert.AreEqual(Math.PI, angle6.InRadians);
            Assert.AreEqual(180, angle6.InDegrees);
            Assert.AreEqual(128, angle6.InBinaryDegrees);
            Assert.AreEqual(2, angle6.InQuadrants);
            Assert.AreEqual(3, angle6.InSextants);
            Assert.AreEqual(200, angle6.InGrads);
            Assert.IsTrue(angle6.Unit == MeasurementUnit.Turns);
            Assert.AreEqual(0.5, angle6.Value);

            GeometricAngle angle9 = GeometricAngle.FromNamedAngle(NamedAngles.Full, MeasurementUnit.Radians);
            Assert.AreEqual(1, angle9.InTurns);
            Assert.AreEqual(Math.PI * 2.0, angle9.InRadians);
            Assert.AreEqual(360, angle9.InDegrees);
            Assert.AreEqual(256, angle9.InBinaryDegrees);
            Assert.AreEqual(4, angle9.InQuadrants);
            Assert.AreEqual(6, angle9.InSextants);
            Assert.AreEqual(400, angle9.InGrads);
            Assert.IsTrue(angle9.Unit == MeasurementUnit.Radians);
            Assert.AreEqual(Math.PI * 2.0, angle9.Value);

            Assert.Throws<System.ComponentModel.InvalidEnumArgumentException>(delegate
            {
                GeometricAngle angle10 = GeometricAngle.FromNamedAngle((NamedAngles)26, MeasurementUnit.Quadrants);
            });
            Assert.Throws<ArgumentException>(delegate
            {
                GeometricAngle angle11 = GeometricAngle.FromNamedAngle(NamedAngles.HalfQuarter, MeasurementUnit.Uninitialized);
            });
        }

        [TestCase(NamedAngles.Zero, 1, 1, ExpectedResult = 0)]
        [TestCase(NamedAngles.Zero, 2, 2, ExpectedResult = 0)]
        [TestCase(NamedAngles.Zero, 2, 3, ExpectedResult = 0)]
        [TestCase(NamedAngles.Zero, 3, 2, ExpectedResult = 0)]
        
        [TestCase(NamedAngles.HalfQuarter, 1, 1, ExpectedResult = 0.125)]
        [TestCase(NamedAngles.HalfQuarter, 2, 1, ExpectedResult = 0.125)]
        [TestCase(NamedAngles.HalfQuarter, 3, 1, ExpectedResult = 0.125)]
        [TestCase(NamedAngles.HalfQuarter, 4, 1, ExpectedResult = 0.125)]
        [TestCase(NamedAngles.HalfQuarter, 5, 1, ExpectedResult = 0.125)]
        [TestCase(NamedAngles.HalfQuarter, 6, 1, ExpectedResult = 0.125)]
        [TestCase(NamedAngles.HalfQuarter, 7, 1, ExpectedResult = 0.125)]
        [TestCase(NamedAngles.HalfQuarter, 1, 3, ExpectedResult = 45)]
        [TestCase(NamedAngles.HalfQuarter, 1, 4, ExpectedResult = 32)]
        [TestCase(NamedAngles.HalfQuarter, 2, 4, ExpectedResult = 32)]
        [TestCase(NamedAngles.HalfQuarter, 0, 7, ExpectedException = typeof(ArgumentException))]

        [TestCase(NamedAngles.TripleHalfQuarter, 1, 1, ExpectedResult = 3.0 / 8.0)]
        [TestCase(NamedAngles.TripleHalfQuarter, 2, 2, ExpectedResult = Math.PI * 0.75)]
        [TestCase(NamedAngles.TripleHalfQuarter, 3, 3, ExpectedResult = 135)]
        [TestCase(NamedAngles.TripleHalfQuarter, 4, 4, ExpectedResult = 96)]
        [TestCase(NamedAngles.TripleHalfQuarter, 5, 5, ExpectedResult = 1.5)]
        [TestCase(NamedAngles.TripleHalfQuarter, 6, 6, ExpectedResult = 9.0 / 4.0)]
        [TestCase(NamedAngles.TripleHalfQuarter, 7, 7, ExpectedResult = 150)]
        [TestCase(NamedAngles.TripleHalfQuarter, 0, 7, ExpectedException = typeof(ArgumentException))]

        [TestCase(NamedAngles.Straight, 1, 1, ExpectedResult = 0.5)]
        [TestCase(NamedAngles.Straight, 2, 2, ExpectedResult = Math.PI)]
        [TestCase(NamedAngles.Straight, 3, 3, ExpectedResult = 180)]
        [TestCase(NamedAngles.Straight, 4, 4, ExpectedResult = 128)]
        [TestCase(NamedAngles.Straight, 5, 5, ExpectedResult = 2)]
        [TestCase(NamedAngles.Straight, 6, 6, ExpectedResult = 3)]
        [TestCase(NamedAngles.Straight, 7, 7, ExpectedResult = 200)]
        [TestCase(NamedAngles.Straight, 0, 7, ExpectedException = typeof(ArgumentException))]

        [TestCase(NamedAngles.TripleQuarters, 1, 1, ExpectedResult = 0.75)]
        [TestCase(NamedAngles.TripleQuarters, 2, 2, ExpectedResult = Math.PI * 1.5)]
        [TestCase(NamedAngles.TripleQuarters, 3, 3, ExpectedResult = 270)]
        [TestCase(NamedAngles.TripleQuarters, 4, 4, ExpectedResult = 192)]
        [TestCase(NamedAngles.TripleQuarters, 5, 5, ExpectedResult = 3)]
        [TestCase(NamedAngles.TripleQuarters, 6, 6, ExpectedResult = 4.5)]
        [TestCase(NamedAngles.TripleQuarters, 7, 7, ExpectedResult = 300)]
        [TestCase(NamedAngles.TripleQuarters, 0, 7, ExpectedException = typeof(ArgumentException))]

        [TestCase(NamedAngles.Full, 1, 1, ExpectedResult = 1)]
        [TestCase(NamedAngles.Full, 2, 2, ExpectedResult = Math.PI * 2)]
        [TestCase(NamedAngles.Full, 3, 3, ExpectedResult = 360)]
        [TestCase(NamedAngles.Full, 4, 4, ExpectedResult = 256)]
        [TestCase(NamedAngles.Full, 5, 5, ExpectedResult = 4)]
        [TestCase(NamedAngles.Full, 6, 6, ExpectedResult = 6)]
        [TestCase(NamedAngles.Full, 7, 7, ExpectedResult = 400)]
        [TestCase(NamedAngles.Full, 0, 7, ExpectedException = typeof(ArgumentException))]
        public Double FromNamedAngle2(NamedAngles namedAngle, Byte inputMeasurementUnit, Byte outputMeasurementUnit)
        {
            MeasurementUnit inputMeasurementUnitCasted = ConvertFromNumber(inputMeasurementUnit);
            MeasurementUnit outputMeasurementUnitCasted = ConvertFromNumber(outputMeasurementUnit);
            GeometricAngle angle = GeometricAngle.FromNamedAngle(namedAngle, inputMeasurementUnitCasted);
            return angle.GetValueInUnit(outputMeasurementUnitCasted);
        }

        [Test]
        public void FromSpecifiedUnit()
        {
            Assert.AreEqual(AngleRanges.Zero, GeometricAngle.FromSpecifiedUnit(0.0, MeasurementUnit.Radians).InRange);
            Assert.AreEqual(AngleRanges.Acute, GeometricAngle.FromSpecifiedUnit(16, MeasurementUnit.BinaryDegrees).InRange);
            Assert.AreEqual(AngleRanges.Right, GeometricAngle.FromSpecifiedUnit(64, MeasurementUnit.BinaryDegrees).InRange);
            Assert.AreEqual(AngleRanges.Obtuse, GeometricAngle.FromSpecifiedUnit(199, MeasurementUnit.Grads).InRange);
            Assert.AreEqual(AngleRanges.Straight, GeometricAngle.FromSpecifiedUnit(Math.PI, MeasurementUnit.Radians).InRange);
            Assert.AreEqual(AngleRanges.Reflex, GeometricAngle.FromSpecifiedUnit(4, MeasurementUnit.Sextants).InRange);
            Assert.AreEqual(AngleRanges.Full, GeometricAngle.FromSpecifiedUnit(1, MeasurementUnit.Turns).InRange);
            Assert.Throws<ArgumentException>(delegate
            {
                AngleRanges range = GeometricAngle.FromSpecifiedUnit(-1, MeasurementUnit.Uninitialized).InRange;
            });
        }

        [TestCase(90, 3, 90, 3, ExpectedResult = true)]
        [TestCase(90, 3, 100, 7, ExpectedResult = true)]
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
        [TestCase(180, 3, 540, 3, ExpectedResult = true)]
        [TestCase(360, 3, 720, 3, ExpectedResult = true)]
        [TestCase(0, 3, 0, 3, ExpectedResult = true)]
        [TestCase(0, 3, -360, 3, ExpectedResult = false)]
        [TestCase(360, 3, -360, 3, ExpectedResult = true)]
        [TestCase(-450, 3, 270, 3, ExpectedResult = true)]
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
            
            GeometricAngle first = GeometricAngle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnitCasted);
            GeometricAngle second = GeometricAngle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnitCasted);
            return GeometricAngle.AreEqual(first, second);
        }

        [Test]
        public void Equality2()
        {
            GeometricAngle a1 = GeometricAngle.FromRadians(Math.PI);
            Assert.AreEqual(a1.InRange, AngleRanges.Straight);
            GeometricAngle a2 = GeometricAngle.FromBinaryDegrees(128);
            Assert.AreEqual(a1, a2);
            GeometricAngle a3 = GeometricAngle.FromSextants(3);
            GeometricAngle a4 = GeometricAngle.FromTurns(0.5);
            GeometricAngle a5 = GeometricAngle.FromRadians(Math.PI/2.0);
            Assert.IsTrue(a3 == a4);
            Assert.IsTrue(GeometricAngle.AreEqual(a1, a2, a3, a4));
            Assert.IsTrue(GeometricAngle.AreEqual(new GeometricAngle[2] { a2, a3 }));
            Assert.IsTrue(GeometricAngle.AreEqual(a1, a2, a3, a4));
            Assert.IsFalse(GeometricAngle.AreEqual(a1, a2, a3, a4, a5));
            Assert.Throws<ArgumentNullException>(delegate { Boolean res = GeometricAngle.AreEqual(null); });
            Assert.Throws<ArgumentException>(delegate { Boolean res = GeometricAngle.AreEqual(new GeometricAngle[0]{}); });
            Assert.Throws<ArgumentException>(delegate { Boolean res = GeometricAngle.AreEqual(a1); });

            Assert.IsFalse(a1.Equals(null));
            Assert.IsFalse(a1.Equals(new Object()));

            Assert.IsFalse(a3.Equals((Object)a5));
            Assert.IsTrue(a3.Equals((Object)a4));
        }

        [Test]
        public void Equality3()
        {
            GeometricAngle a1 = new GeometricAngle();
            GeometricAngle a2 = GeometricAngle.Zero;
            GeometricAngle a3 = GeometricAngle.FromDegrees(90);

            Assert.AreEqual(a1, a2);
            Assert.AreEqual(a3, a3);
            Assert.AreNotEqual(a1, a3);
            Assert.AreNotEqual(a3, a2);
        }

        [Test]
        public void GetHashCodeTest()
        {
            GeometricAngle a1 = GeometricAngle.FromRadians(Math.PI);
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

            GeometricAngle first = GeometricAngle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnitCasted);
            GeometricAngle second = GeometricAngle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnitCasted);
            Int32 firstComparison = first.CompareTo((Object)second);
            Int32 secondComparison = first.CompareTo(second);
            Int32 thirdComparison = GeometricAngle.Compare(first, second);
            Assert.IsTrue(firstComparison == secondComparison && secondComparison == thirdComparison);
            return firstComparison;
        }

        [Test]
        public void Comparison2()
        {
            GeometricAngle a1 = GeometricAngle.FromBinaryDegrees(256);
            Assert.Throws<ArgumentNullException>(delegate { Int32 res1 = a1.CompareTo(null); });
            Assert.Throws<InvalidOperationException>(delegate { Int32 res2 = a1.CompareTo("abcd"); });
        }

        [TestCase(90, 3, 90, 3, 1, ExpectedResult = true)]
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
        [TestCase(3, 5, Math.PI, 1, 3, ExpectedResult = true)]
        [TestCase(3, 5, Math.PI, 1, 4, ExpectedResult = false)]
        [TestCase(3, 5, Math.PI, 1, 5, ExpectedResult = true)]
        [TestCase(3, 5, Math.PI, 1, 6, ExpectedResult = false)]
        [TestCase(Math.PI, 1, 3, 5, 1, ExpectedResult = false)]
        [TestCase(Math.PI, 1, 3, 5, 2, ExpectedResult = true)]
        [TestCase(Math.PI, 1, 3, 5, 3, ExpectedResult = false)]
        [TestCase(Math.PI, 1, 3, 5, 4, ExpectedResult = true)]
        [TestCase(Math.PI, 1, 3, 5, 5, ExpectedResult = false)]
        [TestCase(Math.PI, 1, 3, 5, 6, ExpectedResult = true)]
        [TestCase(Math.PI, 1, 3, 5, 7, ExpectedException = typeof(InvalidOperationException))]
        public Boolean EqualityGreaterLesserOperators(
            Double angle1Value, Byte angle1MeasurementUnit,
            Double angle2Value, Byte angle2MeasurementUnit,
            Byte operatorType)//operatorType: 1 - ==; 2 - !=; 3 - >; 4 - <; 5 - >=; 6 - <=
        {
            MeasurementUnit angle1MeasurementUnitCasted = ConvertFromNumber(angle1MeasurementUnit);
            MeasurementUnit angle2MeasurementUnitCasted = ConvertFromNumber(angle2MeasurementUnit);

            GeometricAngle first = GeometricAngle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnitCasted);
            GeometricAngle second = GeometricAngle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnitCasted);
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

        [TestCase(90, 3, 90, 3, 2, ExpectedResult = Math.PI)]
        [TestCase(180, 3, 180, 3, 3, ExpectedResult = 360)]
        [TestCase(180, 3, 180, 3, 2, ExpectedResult = Math.PI * 2.0)]
        [TestCase(180, 3, 270, 3, 3, ExpectedResult = 90)]
        [TestCase(270, 3, 180, 3, 3, ExpectedResult = 90)]
        [TestCase(270, 3, 270, 3, 2, ExpectedResult = Math.PI)]
        [TestCase(270, 3, 192, 4, 2, ExpectedResult = Math.PI)]
        [TestCase(256, 4, 400, 7, 6, ExpectedResult = 6)]
        public Double Addition(
            Double angle1Value, Byte angle1MeasurementUnit,
            Double angle2Value, Byte angle2MeasurementUnit,
            Byte outputAngleMeasurementUnit)
        {
            MeasurementUnit angle1MeasurementUnitCasted = ConvertFromNumber(angle1MeasurementUnit);
            MeasurementUnit angle2MeasurementUnitCasted = ConvertFromNumber(angle2MeasurementUnit);
            MeasurementUnit outputAngleMeasurementUnitCasted = ConvertFromNumber(outputAngleMeasurementUnit);

            GeometricAngle first = GeometricAngle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnitCasted);
            GeometricAngle second = GeometricAngle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnitCasted);
            GeometricAngle sum = first + second;
            return sum.GetValueInUnit(outputAngleMeasurementUnitCasted);
        }

        [TestCase(90, 3, 90, 3, 2, ExpectedResult = 0.0)]
        [TestCase(90, 3, Math.PI, 2, 5, ExpectedResult = 3)]
        [TestCase(Math.PI, 2, 3, 5, 7, ExpectedResult = 300)]
        [TestCase(64, 4, 192, 4, 6, ExpectedResult = 3)]
        public Double Subtraction(
            Double angle1Value, Byte angle1MeasurementUnit,
            Double angle2Value, Byte angle2MeasurementUnit,
            Byte outputAngleMeasurementUnit)
        {
            MeasurementUnit angle1MeasurementUnitCasted = ConvertFromNumber(angle1MeasurementUnit);
            MeasurementUnit angle2MeasurementUnitCasted = ConvertFromNumber(angle2MeasurementUnit);
            MeasurementUnit outputAngleMeasurementUnitCasted = ConvertFromNumber(outputAngleMeasurementUnit);

            GeometricAngle minuend = GeometricAngle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnitCasted);
            GeometricAngle subtrahend = GeometricAngle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnitCasted);
            GeometricAngle difference = minuend - subtrahend;
            return difference.GetValueInUnit(outputAngleMeasurementUnitCasted);
        }

        [TestCase(90, 3, ExpectedResult = "90 Degrees")]
        [TestCase(300, 7, ExpectedResult = "300 Grads")]
        [TestCase(0.1, 1, ExpectedResult = "0.1 Turns")]
        public String ToString(Double angleValue, Byte angleMeasurementUnit)
        {
            MeasurementUnit angleMeasurementUnitCasted = ConvertFromNumber(angleMeasurementUnit);

            GeometricAngle geometricAngle = GeometricAngle.FromSpecifiedUnit(angleValue, angleMeasurementUnitCasted);
            return geometricAngle.ToString();
        }

        [TestCase(90, 3, "G", true, ExpectedResult = "90 Degrees")]
        [TestCase(55.5, 4, "000.00", true, ExpectedResult = "055.50 Binary degrees")]
        [TestCase(55.5, 4, "000.00", false, ExpectedResult = "055,50 Binary degrees")]
        [TestCase(55.56, 7, "00.0", true, ExpectedResult = "55.6 Grads")]
        [TestCase(55.56, 7, "00.0", false, ExpectedResult = "55,6 Grads")]
        public String ToStringFormat(Double angleValue, Byte angleMeasurementUnit, string format, bool dotOrComma)
        {
            MeasurementUnit angleMeasurementUnitCasted = ConvertFromNumber(angleMeasurementUnit);

            GeometricAngle geometricAngle = GeometricAngle.FromSpecifiedUnit(angleValue, angleMeasurementUnitCasted);
            NumberFormatInfo dotNum = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            dotNum.NumberDecimalSeparator = ".";
            NumberFormatInfo commaNum = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            commaNum.NumberDecimalSeparator = ",";
            IFormatProvider formatProvider = dotOrComma ? dotNum : commaNum;
            return geometricAngle.ToString(format, formatProvider);
        }

        [Test]
        public void Clone()
        {
            GeometricAngle first = GeometricAngle.FromDegrees(90);
            GeometricAngle second = first.Clone();
            Assert.AreEqual(first, second);
            first = GeometricAngle.FromGrads(300);
            Assert.AreNotEqual(first, second);
            Object third = ((ICloneable) second).Clone();
            Assert.IsInstanceOf<GeometricAngle>(third);
            Assert.IsTrue(second.Equals(third));
        }

        [Test]
        public void TestCracked()
        {
            Type inputType = typeof(GeometricAngle);
            Object instance = Activator.CreateInstance(inputType);
            FieldInfo valueField = inputType.GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo measurementUnitField = inputType.GetField("_measurementUnit", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(valueField);
            Assert.IsNotNull(measurementUnitField);
            
            const Double value1 = 100500.0;
            MeasurementUnit value2 = MeasurementUnit.Degrees;
            
            valueField.SetValue(instance, value1);
            measurementUnitField.SetValue(instance, value2);

            Assert.IsInstanceOf<GeometricAngle>(instance);
            GeometricAngle castedAngle1 = (GeometricAngle)instance;

            Assert.AreEqual(value1, castedAngle1.Value);
            Assert.AreEqual(value2, castedAngle1.Unit);
            Assert.IsTrue(castedAngle1.Unit == MeasurementUnit.Degrees);

            Assert.Throws<UnreachableCodeException>(delegate
            {
                AngleRanges range = castedAngle1.InRange;
            });

            Assert.Throws<InvalidOperationException>(delegate
            {
                castedAngle1.GetExplementaryAngle();
            });

            measurementUnitField.SetValue(instance, MeasurementUnit.Uninitialized);
            GeometricAngle castedAngle2 = (GeometricAngle)instance;

            Assert.Throws<ArgumentException>(delegate
            {
                AngleRanges range = castedAngle2.InRange;
            });
            Assert.Throws<ArgumentException>(delegate
            {
                Double value = castedAngle2.GetValueInUnit(MeasurementUnit.Degrees);
            });
        }

        [Test]
        public void TestParsing()
        {
            GeometricAngle halfOfTurn = GeometricAngle.FromDegrees(180);

            const string input1valid = "180\tdegrees";
            const string input2valid = " 180degree\r\n ";
            const string input3valid = "\t-180 degs\r\n";
            const string input4valid = "\t0\t  \n°\v";
            const string input5valid = "\t5.5°\v";
            const string input6valid = "3.14159265 RAD";
            const string input7valid = "\v3  Sextants";
            const string input8valid = "2 quad";
            const string input9valid = "200 gons";
            const string input10valid = "\t0,5turn\v";

            GeometricAngle temp1;

            Boolean res1valid = GeometricAngle.TryParse(input1valid, out temp1);
            Assert.IsTrue(res1valid);
            Assert.AreEqual(halfOfTurn, temp1);

            Boolean res2valid = GeometricAngle.TryParse(input2valid, out temp1);
            Assert.IsTrue(res2valid);
            Assert.AreEqual(halfOfTurn, temp1);

            Boolean res3valid = GeometricAngle.TryParse(input3valid, out temp1);
            Assert.IsTrue(res3valid);
            Assert.AreEqual(halfOfTurn, temp1);

            Boolean res4valid = GeometricAngle.TryParse(input4valid, out temp1);
            Assert.IsTrue(res4valid);
            Assert.AreEqual(GeometricAngle.Zero, temp1);

            Boolean res5valid = GeometricAngle.TryParse(input5valid, out temp1);
            Assert.IsTrue(res5valid);
            Assert.AreEqual(GeometricAngle.FromDegrees(5.5), temp1);

            Boolean res6valid = GeometricAngle.TryParse(input6valid, out temp1);
            Assert.IsTrue(res6valid);
            Assert.AreEqual(halfOfTurn, temp1);

            Boolean res7valid = GeometricAngle.TryParse(input7valid, out temp1);
            Assert.IsTrue(res7valid);
            Assert.AreEqual(halfOfTurn, temp1);

            Boolean res8valid = GeometricAngle.TryParse(input8valid, out temp1);
            Assert.IsTrue(res8valid);
            Assert.AreEqual(halfOfTurn, temp1);

            Boolean res9valid = GeometricAngle.TryParse(input9valid, out temp1);
            Assert.IsTrue(res9valid);
            Assert.AreEqual(halfOfTurn, temp1);

            Boolean res10valid = GeometricAngle.TryParse(input10valid, CultureInfo.GetCultureInfo("uk-UA"), out temp1);
            Assert.IsTrue(res10valid);
            Assert.AreEqual(halfOfTurn, temp1);

            const string input1invalid = " \r\n \t \v ";
            const string input2invalid = " 15,5 degrees\r\n ";
            const string input3invalid = "\t-180 degs s\r\n";
            const string input4invalid = "10 000°";
            const string input5invalid = "\t5.5E+3°\v";
            const string input6invalid = " degrees";

            GeometricAngle temp2;

            Boolean res1invalid = GeometricAngle.TryParse(input1invalid, out temp2);
            Assert.IsFalse(res1invalid);
            Assert.IsTrue(temp2.IsUnitlessZero);

            Boolean res2invalid = GeometricAngle.TryParse(input2invalid, out temp2);
            Assert.IsFalse(res2invalid);
            Assert.IsTrue(temp2.IsUnitlessZero);

            Boolean res3invalid = GeometricAngle.TryParse(input3invalid, out temp2);
            Assert.IsFalse(res3invalid);
            Assert.IsTrue(temp2.IsUnitlessZero);

            Boolean res4invalid = GeometricAngle.TryParse(input4invalid, out temp2);
            Assert.IsFalse(res4invalid);
            Assert.IsTrue(temp2.IsUnitlessZero);

            Boolean res5invalid = GeometricAngle.TryParse(input5invalid, out temp2);
            Assert.IsFalse(res5invalid);
            Assert.IsTrue(temp2.IsUnitlessZero);

            Boolean res6invalid = GeometricAngle.TryParse(input6invalid, out temp2);
            Assert.IsFalse(res6invalid);
            Assert.IsTrue(temp2.IsUnitlessZero);
        }
    }
}

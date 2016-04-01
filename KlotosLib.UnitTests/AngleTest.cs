using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace KlotosLib.UnitTests
{
    [NUnit.Framework.TestFixture]
    public class AngleTest
    {
        [Test]
        public void InstancePropertiesAndMethods()
        {
            Angle a1 = Angle.FromDegrees(270);
            Angle a2 = Angle.FromQuadrants(3);
            Assert.IsTrue(a1 == a2);
            Assert.IsTrue(a1.OriginalUnit == Angle.MeasurementUnit.Degree);
            Assert.IsTrue(a2.OriginalUnit == Angle.MeasurementUnit.Quadrant);

            Assert.AreEqual(a1.InTurns, a2.InTurns);
            Assert.AreEqual(a1.InRadians, a2.InRadians);
            Assert.AreEqual(a1.InDegrees, a2.InDegrees);
            Assert.AreEqual(a1.InQuadrants, a2.InQuadrants);
            Assert.AreEqual(a1.InSextants, a2.InSextants);
            Assert.AreEqual(a1.InBinaryDegrees, a2.InBinaryDegrees);
            Assert.AreEqual(a1.InGrads, a2.InGrads);
            Assert.AreEqual(a1.Type, a2.Type);
            Assert.IsTrue(a1.Type == Angle.AngleType.Reflex);

            Assert.AreEqual(3.0, a1.GetValueInUnitType(Angle.MeasurementUnit.Quadrant));
            Assert.AreEqual(192, a1.GetValueInUnitType(Angle.MeasurementUnit.BinaryDegree));
            Assert.Throws<InvalidEnumArgumentException>(delegate
            {
                Double res = a1.GetValueInUnitType((Angle.MeasurementUnit)7);
            });
        }

        [Test]
        public void GetOneTurnValueForUnit()
        {
            Assert.AreEqual(1.0, Angle.GetOneTurnValueForUnit(Angle.MeasurementUnit.Turn));
            Assert.AreEqual(Math.PI * 2, Angle.GetOneTurnValueForUnit(Angle.MeasurementUnit.Radian));
            Assert.AreEqual(360, Angle.GetOneTurnValueForUnit(Angle.MeasurementUnit.Degree));
            Assert.AreEqual(256, Angle.GetOneTurnValueForUnit(Angle.MeasurementUnit.BinaryDegree));
            Assert.AreEqual(4, Angle.GetOneTurnValueForUnit(Angle.MeasurementUnit.Quadrant));
            Assert.AreEqual(6, Angle.GetOneTurnValueForUnit(Angle.MeasurementUnit.Sextant));
            Assert.AreEqual(400, Angle.GetOneTurnValueForUnit(Angle.MeasurementUnit.Grad));
            Assert.Throws<InvalidEnumArgumentException>(delegate
            {
                Double res = Angle.GetOneTurnValueForUnit((Angle.MeasurementUnit)7);
            });
        }

        [Test]
        public void GetAngleValueForTypeAndUnit()
        {
            Assert.AreEqual(0.5, Angle.GetAngleValueForTypeAndUnit(Angle.AngleType.Straight, Angle.MeasurementUnit.Turn));
            Assert.AreEqual(Math.PI * 2.0, Angle.GetAngleValueForTypeAndUnit(Angle.AngleType.Full, Angle.MeasurementUnit.Radian));
            Assert.AreEqual(90, Angle.GetAngleValueForTypeAndUnit(Angle.AngleType.Right, Angle.MeasurementUnit.Degree));
            Assert.AreEqual(0.0, Angle.GetAngleValueForTypeAndUnit(Angle.AngleType.Zero, Angle.MeasurementUnit.Quadrant));
            Assert.Throws<InvalidOperationException>(delegate
            {
                Double res = Angle.GetAngleValueForTypeAndUnit(Angle.AngleType.Reflex, Angle.MeasurementUnit.Quadrant);
            });
            Assert.Throws<InvalidEnumArgumentException>(delegate
            {
                Double res = Angle.GetAngleValueForTypeAndUnit((Angle.AngleType)7, Angle.MeasurementUnit.Quadrant);
            });
        }

        [Test]
        public void GetAngleTypeFromValueOfUnit()
        {
            Assert.AreEqual(Angle.AngleType.Zero, Angle.GetAngleTypeFromValueOfUnit(0.0, Angle.MeasurementUnit.Radian));
            Assert.AreEqual(Angle.AngleType.Acute, Angle.GetAngleTypeFromValueOfUnit(16, Angle.MeasurementUnit.BinaryDegree));
            Assert.AreEqual(Angle.AngleType.Right, Angle.GetAngleTypeFromValueOfUnit(64, Angle.MeasurementUnit.BinaryDegree));
            Assert.AreEqual(Angle.AngleType.Obtuse, Angle.GetAngleTypeFromValueOfUnit(199, Angle.MeasurementUnit.Grad));
            Assert.AreEqual(Angle.AngleType.Straight, Angle.GetAngleTypeFromValueOfUnit(Math.PI, Angle.MeasurementUnit.Radian));
            Assert.AreEqual(Angle.AngleType.Reflex, Angle.GetAngleTypeFromValueOfUnit(4, Angle.MeasurementUnit.Sextant));
            Assert.AreEqual(Angle.AngleType.Full, Angle.GetAngleTypeFromValueOfUnit(1, Angle.MeasurementUnit.Turn));
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                Angle.AngleType type = Angle.GetAngleTypeFromValueOfUnit(-1, Angle.MeasurementUnit.Degree);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                Angle.AngleType type = Angle.GetAngleTypeFromValueOfUnit(361, Angle.MeasurementUnit.Degree);
            });
        }

        [TestCase(90, Angle.MeasurementUnit.Degree, 90, Angle.MeasurementUnit.Degree, ExpectedResult = true)]
        [TestCase(90, Angle.MeasurementUnit.Degree, 100, Angle.MeasurementUnit.Grad, ExpectedResult = true)]
        [TestCase(90, Angle.MeasurementUnit.Degree, 200, Angle.MeasurementUnit.Grad, ExpectedResult = false)]
        [TestCase(Math.PI / 2.0, Angle.MeasurementUnit.Radian, 100, Angle.MeasurementUnit.Grad, ExpectedResult = true)]
        [TestCase(100, Angle.MeasurementUnit.Grad, Math.PI / 2.0, Angle.MeasurementUnit.Radian, ExpectedResult = true)]
        [TestCase(3, Angle.MeasurementUnit.Sextant, 2, Angle.MeasurementUnit.Quadrant, ExpectedResult = true)]
        [TestCase(3, Angle.MeasurementUnit.Sextant, 3, Angle.MeasurementUnit.Quadrant, ExpectedResult = false)]
        [TestCase(Math.PI, Angle.MeasurementUnit.Radian, 1, Angle.MeasurementUnit.Turn, ExpectedResult = false)]
        [TestCase(Math.PI, Angle.MeasurementUnit.Radian, 0.5, Angle.MeasurementUnit.Turn, ExpectedResult = true)]
        [TestCase(Math.PI * 2.0, Angle.MeasurementUnit.Radian, 1, Angle.MeasurementUnit.Turn, ExpectedResult = true)]
        [TestCase(270, Angle.MeasurementUnit.Degree, 3, Angle.MeasurementUnit.Quadrant, ExpectedResult = true)]
        [TestCase(270, Angle.MeasurementUnit.Degree, 3, Angle.MeasurementUnit.Sextant, ExpectedResult = false)]
        [TestCase(180, Angle.MeasurementUnit.Degree, 540, Angle.MeasurementUnit.Degree, ExpectedResult = true)]
        [TestCase(360, Angle.MeasurementUnit.Degree, 720, Angle.MeasurementUnit.Degree, ExpectedResult = true)]
        [TestCase(0, Angle.MeasurementUnit.Degree, 0, Angle.MeasurementUnit.Degree, ExpectedResult = true)]
        [TestCase(0, Angle.MeasurementUnit.Degree, -360, Angle.MeasurementUnit.Degree, ExpectedResult = false)]
        [TestCase(360, Angle.MeasurementUnit.Degree, -360, Angle.MeasurementUnit.Degree, ExpectedResult = true)]
        [TestCase(-450, Angle.MeasurementUnit.Degree, 270, Angle.MeasurementUnit.Degree, ExpectedResult = true)]
        [TestCase(270, Angle.MeasurementUnit.Degree, 3, (Angle.MeasurementUnit)7, ExpectedException = typeof(InvalidEnumArgumentException))]
        public Boolean Equality(
            Double angle1Value, Angle.MeasurementUnit angle1MeasurementUnit, 
            Double angle2Value, Angle.MeasurementUnit angle2MeasurementUnit
            )
        {
            Angle first = Angle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnit);
            Angle second = Angle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnit);
            return Angle.AreEqual(first, second);
        }

        [Test]
        public void Equality()
        {
            Angle a1 = Angle.FromRadians(Math.PI);
            Assert.AreEqual(a1.Type, Angle.AngleType.Straight);
            Angle a2 = Angle.FromBinaryDegrees(128);
            Assert.AreEqual(a1, a2);
            Angle a3 = Angle.FromSextants(3);
            Angle a4 = Angle.FromTurns(0.5);
            Angle a5 = Angle.FromRadians(Math.PI/2.0);
            Assert.IsTrue(a3 == a4);
            Assert.IsTrue(Angle.AreEqual(a1, a2, a3));
            Assert.IsTrue(Angle.AreEqual(new Angle[2] { a2, a3 }));
            Assert.IsTrue(Angle.AreEqual(a1, a2, a3, a4));
            Assert.IsFalse(Angle.AreEqual(a1, a2, a3, a4, a5));
            Assert.Throws<ArgumentNullException>(delegate { Boolean res = Angle.AreEqual(null); });
            Assert.Throws<ArgumentException>(delegate { Boolean res = Angle.AreEqual(new Angle[0]{}); });
            Assert.Throws<ArgumentException>(delegate { Boolean res = Angle.AreEqual(a1); });

            Assert.IsFalse(a1.Equals(null));
            Assert.IsFalse(a1.Equals(new Object()));
        }

        [Test]
        public void GetHashCodeTest()
        {
            Angle a1 = Angle.FromRadians(Math.PI);
            Int32 expectedHashCode = Math.PI.GetHashCode();
            expectedHashCode = (expectedHashCode * 397) ^ Angle.MeasurementUnit.Radian.GetHashCode();
            Assert.AreEqual(expectedHashCode, a1.GetHashCode());
        }

        [TestCase(90, Angle.MeasurementUnit.Degree, 90, Angle.MeasurementUnit.Degree, ExpectedResult = 0)]
        [TestCase(90, Angle.MeasurementUnit.Degree, 180, Angle.MeasurementUnit.Degree, ExpectedResult = -1)]
        [TestCase(180, Angle.MeasurementUnit.Degree, 90, Angle.MeasurementUnit.Degree, ExpectedResult = 1)]
        [TestCase(Math.PI, Angle.MeasurementUnit.Radian, 199, Angle.MeasurementUnit.Grad, ExpectedResult = 1)]
        [TestCase(399, Angle.MeasurementUnit.Grad, Math.PI * 2.0, Angle.MeasurementUnit.Radian, ExpectedResult = -1)]
        [TestCase(0, Angle.MeasurementUnit.Degree, 1, Angle.MeasurementUnit.Turn, ExpectedResult = -1)]
        [TestCase(0, Angle.MeasurementUnit.Degree, 0, Angle.MeasurementUnit.Turn, ExpectedResult = 0)]
        public Int32 Comparison(
            Double angle1Value, Angle.MeasurementUnit angle1MeasurementUnit,
            Double angle2Value, Angle.MeasurementUnit angle2MeasurementUnit
            )
        {
            Angle first = Angle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnit);
            Angle second = Angle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnit);
            Int32 firstComparison = first.CompareTo((Object)second);
            Int32 secondComparison = first.CompareTo(second);
            Int32 thirdComparison = Angle.Compare(first, second);
            Assert.IsTrue(firstComparison == secondComparison && secondComparison == thirdComparison);
            return firstComparison;
        }

        [Test]
        public void Comparison()
        {
            Angle a1 = Angle.FromBinaryDegrees(256);
            Assert.Throws<ArgumentNullException>(delegate { Int32 res1 = a1.CompareTo(null); });
            Assert.Throws<InvalidOperationException>(delegate { Int32 res2 = a1.CompareTo("abcd"); });
        }

        [TestCase(90, Angle.MeasurementUnit.Degree, 90, Angle.MeasurementUnit.Degree, 1, ExpectedResult = true)]
        [TestCase(90, Angle.MeasurementUnit.Degree, 128, Angle.MeasurementUnit.BinaryDegree, 1, ExpectedResult = false)]
        [TestCase(90, Angle.MeasurementUnit.Degree, 128, Angle.MeasurementUnit.BinaryDegree, 2, ExpectedResult = true)]
        [TestCase(3, Angle.MeasurementUnit.Sextant, 128, Angle.MeasurementUnit.BinaryDegree, 1, ExpectedResult = true)]
        [TestCase(3, Angle.MeasurementUnit.Sextant, 128, Angle.MeasurementUnit.BinaryDegree, 2, ExpectedResult = false)]
        [TestCase(3, Angle.MeasurementUnit.Sextant, 128, Angle.MeasurementUnit.BinaryDegree, 5, ExpectedResult = true)]
        [TestCase(3, Angle.MeasurementUnit.Sextant, 128, Angle.MeasurementUnit.BinaryDegree, 6, ExpectedResult = true)]
        [TestCase(3, Angle.MeasurementUnit.Sextant, 128, Angle.MeasurementUnit.BinaryDegree, 3, ExpectedResult = false)]
        [TestCase(3, Angle.MeasurementUnit.Sextant, 128, Angle.MeasurementUnit.BinaryDegree, 4, ExpectedResult = false)]
        [TestCase(3, Angle.MeasurementUnit.Quadrant, Math.PI, Angle.MeasurementUnit.Turn, 1, ExpectedResult = false)]
        [TestCase(3, Angle.MeasurementUnit.Quadrant, Math.PI, Angle.MeasurementUnit.Turn, 2, ExpectedResult = true)]
        [TestCase(3, Angle.MeasurementUnit.Quadrant, Math.PI, Angle.MeasurementUnit.Turn, 3, ExpectedResult = true)]
        [TestCase(3, Angle.MeasurementUnit.Quadrant, Math.PI, Angle.MeasurementUnit.Turn, 4, ExpectedResult = false)]
        [TestCase(3, Angle.MeasurementUnit.Quadrant, Math.PI, Angle.MeasurementUnit.Turn, 5, ExpectedResult = true)]
        [TestCase(3, Angle.MeasurementUnit.Quadrant, Math.PI, Angle.MeasurementUnit.Turn, 6, ExpectedResult = false)]
        [TestCase(Math.PI, Angle.MeasurementUnit.Turn, 3, Angle.MeasurementUnit.Quadrant, 1, ExpectedResult = false)]
        [TestCase(Math.PI, Angle.MeasurementUnit.Turn, 3, Angle.MeasurementUnit.Quadrant, 2, ExpectedResult = true)]
        [TestCase(Math.PI, Angle.MeasurementUnit.Turn, 3, Angle.MeasurementUnit.Quadrant, 3, ExpectedResult = false)]
        [TestCase(Math.PI, Angle.MeasurementUnit.Turn, 3, Angle.MeasurementUnit.Quadrant, 4, ExpectedResult = true)]
        [TestCase(Math.PI, Angle.MeasurementUnit.Turn, 3, Angle.MeasurementUnit.Quadrant, 5, ExpectedResult = false)]
        [TestCase(Math.PI, Angle.MeasurementUnit.Turn, 3, Angle.MeasurementUnit.Quadrant, 6, ExpectedResult = true)]
        [TestCase(Math.PI, Angle.MeasurementUnit.Turn, 3, Angle.MeasurementUnit.Quadrant, 7, ExpectedException = typeof(InvalidOperationException))]
        public Boolean EqualityGreaterLesserOperators(
            Double angle1Value, Angle.MeasurementUnit angle1MeasurementUnit,
            Double angle2Value, Angle.MeasurementUnit angle2MeasurementUnit,
            Byte operatorType)//operatorType: 1 - ==; 2 - !=; 3 - >; 4 - <; 5 - >=; 6 - <=
        {
            Angle first = Angle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnit);
            Angle second = Angle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnit);
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

        [TestCase(90, Angle.MeasurementUnit.Degree, 90, Angle.MeasurementUnit.Degree, Angle.MeasurementUnit.Radian, ExpectedResult = Math.PI)]
        [TestCase(180, Angle.MeasurementUnit.Degree, 180, Angle.MeasurementUnit.Degree, Angle.MeasurementUnit.Degree, ExpectedResult = 360)]
        [TestCase(180, Angle.MeasurementUnit.Degree, 180, Angle.MeasurementUnit.Degree, Angle.MeasurementUnit.Radian, ExpectedResult = Math.PI * 2.0)]
        [TestCase(180, Angle.MeasurementUnit.Degree, 270, Angle.MeasurementUnit.Degree, Angle.MeasurementUnit.Degree, ExpectedResult = 90)]
        [TestCase(270, Angle.MeasurementUnit.Degree, 180, Angle.MeasurementUnit.Degree, Angle.MeasurementUnit.Degree, ExpectedResult = 90)]
        [TestCase(270, Angle.MeasurementUnit.Degree, 270, Angle.MeasurementUnit.Degree, Angle.MeasurementUnit.Radian, ExpectedResult = Math.PI)]
        [TestCase(270, Angle.MeasurementUnit.Degree, 192, Angle.MeasurementUnit.BinaryDegree, Angle.MeasurementUnit.Radian, ExpectedResult = Math.PI)]
        [TestCase(256, Angle.MeasurementUnit.BinaryDegree, 400, Angle.MeasurementUnit.Grad, Angle.MeasurementUnit.Sextant, ExpectedResult = 6)]
        public Double Addition(
            Double angle1Value, Angle.MeasurementUnit angle1MeasurementUnit,
            Double angle2Value, Angle.MeasurementUnit angle2MeasurementUnit,
            Angle.MeasurementUnit outputAngleMeasurementUnit)
        {
            Angle first = Angle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnit);
            Angle second = Angle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnit);
            Angle sum = first + second;
            return sum.GetValueInUnitType(outputAngleMeasurementUnit);
        }

        [TestCase(90, Angle.MeasurementUnit.Degree, 90, Angle.MeasurementUnit.Degree, Angle.MeasurementUnit.Radian, ExpectedResult = 0.0)]
        [TestCase(90, Angle.MeasurementUnit.Degree, Math.PI, Angle.MeasurementUnit.Radian, Angle.MeasurementUnit.Quadrant, ExpectedResult = 3)]
        [TestCase(Math.PI, Angle.MeasurementUnit.Radian, 3, Angle.MeasurementUnit.Quadrant, Angle.MeasurementUnit.Grad, ExpectedResult = 300)]
        [TestCase(64, Angle.MeasurementUnit.BinaryDegree, 192, Angle.MeasurementUnit.BinaryDegree, Angle.MeasurementUnit.Sextant, ExpectedResult = 3)]
        public Double Subtraction(
            Double angle1Value, Angle.MeasurementUnit angle1MeasurementUnit,
            Double angle2Value, Angle.MeasurementUnit angle2MeasurementUnit,
            Angle.MeasurementUnit outputAngleMeasurementUnit)
        {
            Angle minuend = Angle.FromSpecifiedUnit(angle1Value, angle1MeasurementUnit);
            Angle subtrahend = Angle.FromSpecifiedUnit(angle2Value, angle2MeasurementUnit);
            Angle difference = minuend - subtrahend;
            return difference.GetValueInUnitType(outputAngleMeasurementUnit);
        }

        [TestCase(90, Angle.MeasurementUnit.Degree, ExpectedResult = "90 Degrees")]
        [TestCase(300, Angle.MeasurementUnit.Grad, ExpectedResult = "300 Grads")]
        [TestCase(0.1, Angle.MeasurementUnit.Turn, ExpectedResult = "0.1 Turns")]
        public String ToString(Double angleValue, Angle.MeasurementUnit angleMeasurementUnit)
        {
            Angle angle = Angle.FromSpecifiedUnit(angleValue, angleMeasurementUnit);
            return angle.ToString();
        }

        [Test]
        public void Clone()
        {
            Angle first = Angle.FromDegrees(90);
            Angle second = first.Clone();
            Assert.AreEqual(first, second);
            first = Angle.FromGrads(300);
            Assert.AreNotEqual(first, second);
            Object third = ((ICloneable) second).Clone();
            Assert.IsInstanceOf<Angle>(third);
            Assert.IsTrue(second.Equals(third));
        }
    }
}

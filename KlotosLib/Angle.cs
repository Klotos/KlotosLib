using System;
using System.ComponentModel;
using System.Globalization;

namespace KlotosLib
{
    /// <summary>
    /// Инкапсулирует один геометрический угол, включая его величину и единицу измерения. Поддерживает угол в рамках одного оборота. 
    /// Поддерживает геометрические операции над углами. Поддерживает конвертацию угловой величины в разные единицы измерений. 
    /// Неизменяемый значимый тип.
    /// </summary>
    /// <remarks>Для сохранения точности величины угла экземпляр содержит её в той единице измерения, в которой экземпляр был создан. 
    /// Выполняет преобразования в необходимые единицы на лету, не изменяя изначально запомненную единицу измерения.</remarks>
    public struct Angle : IEquatable<Angle>, IComparable<Angle>, IComparable, ICloneable
    {
        #region Subtypes - enumerations
        /// <summary>
        /// Существующие типы углов
        /// </summary>
        public enum AngleType : byte
        {
            /// <summary>
            /// Нулевой угол (0°)
            /// </summary>
            Zero = 0,
            /// <summary>
            /// Острый угол (0°-90°)
            /// </summary>
            Acute = 1,
            /// <summary>
            /// Прямой угол (90°)
            /// </summary>
            Right = 2,
            /// <summary>
            /// Тупой угол (90°-180°)
            /// </summary>
            Obtuse = 3,
            /// <summary>
            /// Развёрнутый угол (180°)
            /// </summary>
            Straight = 4,
            /// <summary>
            /// Невыпуклый угол (180°-360°)
            /// </summary>
            Reflex = 5,
            /// <summary>
            /// Полный угол (360°)
            /// </summary>
            Full = 6
        }

        /// <summary>
        /// Поддерживаемые единицы измерения углов
        /// </summary>
        public enum MeasurementUnit : byte
        {
            /// <summary>
            /// Обороты
            /// </summary>
            Turn,
            /// <summary>
            /// Радианы
            /// </summary>
            Radian,
            /// <summary>
            /// Градусы
            /// </summary>
            Degree,
            /// <summary>
            /// Двоичные градусы
            /// </summary>
            BinaryDegree,
            /// <summary>
            /// Квадранты
            /// </summary>
            Quadrant,
            /// <summary>
            /// Секстанты
            /// </summary>
            Sextant,
            /// <summary>
            /// Грады (градианы, гоны)
            /// </summary>
            Grad
        }
        #endregion Subtypes - enumerations

        #region Constants
        private const Double _threshold = 0.0000001;
        private const Double _2pi = 2*Math.PI;
        #endregion Constants


        #region Fields
        private readonly Double _value;

        private readonly MeasurementUnit _measurementUnit;
        #endregion Fields

        #region Constructors

        private Angle(Angle other)
        {
            this._value = other._value;
            this._measurementUnit = other._measurementUnit;
        }

        private Angle(Double value, Angle.MeasurementUnit measurementUnit)
        {
            this._value = value;
            this._measurementUnit = measurementUnit;
        }
        #endregion Constructors

        #region Private helpers
        private static Double NormalizeMeasurementValue(Double inputValue, Double maxValue, Double threshold)
        {
            if (NumericTools.AreEqual(threshold, inputValue, 0.0))
            {
                return 0.0;
            }
            else if (NumericTools.AreEqual(threshold, inputValue, maxValue))
            {
                return maxValue;
            }
            else if (NumericTools.AreEqual(threshold, inputValue, -maxValue))
            {
                return maxValue;
            }
            else if (inputValue > 0.0 && inputValue < maxValue)
            {
                return inputValue;
            }
            else if (inputValue > maxValue)
            {
                Double remainder = inputValue % maxValue;
                if (NumericTools.AreEqual(threshold, remainder, 0.0))
                {
                    return maxValue;
                }
                else
                {
                    return remainder;
                }
            }
            else if (inputValue < 0 && inputValue > -maxValue) //-maxValue < inputValue < 0
            {
                return (maxValue + inputValue);
            }
            else//if(inputValue < -maxValue)
            {
                return (maxValue + (inputValue % maxValue));
            }
        }

        private static Double ConvertFromTo(Angle.MeasurementUnit from, Angle.MeasurementUnit to, Double inputValue)
        {
            if (from == to) { return inputValue; }
            return inputValue * GetOneTurnValueForUnit(to) / GetOneTurnValueForUnit(from);
        }
        #endregion private helpers
        
        #region Static factories
        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в оборотах
        /// </summary>
        /// <param name="turns"></param>
        /// <returns></returns>
        public static Angle FromTurns(Double turns)
        {
            return new Angle(NormalizeMeasurementValue(turns, GetOneTurnValueForUnit(MeasurementUnit.Turn), _threshold), MeasurementUnit.Turn);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в радианах
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Angle FromRadians(Double radians)
        {
            return new Angle(NormalizeMeasurementValue(radians, GetOneTurnValueForUnit(MeasurementUnit.Radian), _threshold), MeasurementUnit.Radian);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в градусах
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Angle FromDegrees(Double degrees)
        {
            return new Angle(NormalizeMeasurementValue(degrees, GetOneTurnValueForUnit(MeasurementUnit.Degree), _threshold), MeasurementUnit.Degree);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в двоичных градусах
        /// </summary>
        /// <param name="binaryDegrees"></param>
        /// <returns></returns>
        public static Angle FromBinaryDegrees(Double binaryDegrees)
        {
            return new Angle(NormalizeMeasurementValue(binaryDegrees, GetOneTurnValueForUnit(MeasurementUnit.BinaryDegree), _threshold), MeasurementUnit.BinaryDegree);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в квадрантах
        /// </summary>
        /// <param name="quadrants"></param>
        /// <returns></returns>
        public static Angle FromQuadrants(Double quadrants)
        {
            return new Angle(NormalizeMeasurementValue(quadrants, GetOneTurnValueForUnit(MeasurementUnit.Quadrant), _threshold), MeasurementUnit.Quadrant);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в секстантах
        /// </summary>
        /// <param name="sextants"></param>
        /// <returns></returns>
        public static Angle FromSextants(Double sextants)
        {
            return new Angle(NormalizeMeasurementValue(sextants, GetOneTurnValueForUnit(MeasurementUnit.Sextant), _threshold), MeasurementUnit.Sextant);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в градах (градианах, гонах)
        /// </summary>
        /// <param name="grads"></param>
        /// <returns></returns>
        public static Angle FromGrads(Double grads)
        {
            return new Angle(NormalizeMeasurementValue(grads, GetOneTurnValueForUnit(MeasurementUnit.Grad), _threshold), MeasurementUnit.Grad);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в указанной единице измерения углов
        /// </summary>
        /// <param name="inputValue">Величина угла</param>
        /// <param name="measurementUnit">Единица измерения углов, в которой указана величина <paramref name="inputValue"/>.</param>
        /// <returns></returns>
        public static Angle FromSpecifiedUnit(Double inputValue, Angle.MeasurementUnit measurementUnit)
        {
            if (Enum.IsDefined(measurementUnit.GetType(), measurementUnit) == false)
            {
                throw new InvalidEnumArgumentException("measurementUnit", (Int32)measurementUnit, measurementUnit.GetType());
            }
            return new Angle(NormalizeMeasurementValue(inputValue, GetOneTurnValueForUnit(measurementUnit), _threshold), measurementUnit);
        }
        #endregion Static factories

        #region Properties
        /// <summary>
        /// Возвращает оригинальную единицу измерения угла, с который был создан данный экземпляр
        /// </summary>
        public Angle.MeasurementUnit OriginalUnit
        {
            get { return this._measurementUnit; }
        }
        
        /// <summary>
        /// Возвращает тип данного угла
        /// </summary>
        public Angle.AngleType Type
        {
            get { return GetAngleTypeFromValueOfUnit(this._value, this._measurementUnit); }
        }
        
        /// <summary>
        /// Возвращает величину данного угла в оборотах
        /// </summary>
        public Double InTurns
        {
            get
            {
                return ConvertFromTo(this._measurementUnit, MeasurementUnit.Turn, this._value);
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в радианах
        /// </summary>
        public Double InRadians
        {
            get
            {
                return ConvertFromTo(this._measurementUnit, MeasurementUnit.Radian, this._value);
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в градусах
        /// </summary>
        public Double InDegrees
        {
            get
            {
                return ConvertFromTo(this._measurementUnit, MeasurementUnit.Degree, this._value);
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в двоичных градусах
        /// </summary>
        public Double InBinaryDegrees
        {
            get
            {
                return ConvertFromTo(this._measurementUnit, MeasurementUnit.BinaryDegree, this._value);
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в квадпрантах
        /// </summary>
        public Double InQuadrants
        {
            get
            {
                return ConvertFromTo(this._measurementUnit, MeasurementUnit.Quadrant, this._value);
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в секстантах
        /// </summary>
        public Double InSextants
        {
            get
            {
                return ConvertFromTo(this._measurementUnit, MeasurementUnit.Sextant, this._value);
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в градах (градианах, гонах)
        /// </summary>
        public Double InGrads
        {
            get
            {
                return ConvertFromTo(this._measurementUnit, MeasurementUnit.Grad, this._value);
            }
        }
        #endregion Properties

        #region Instance methods
        /// <summary>
        /// Возвращает величину данного угла в указанных единицах измерения
        /// </summary>
        /// <param name="measurementUnit"></param>
        /// <returns></returns>
        public Double GetValueInUnitType(Angle.MeasurementUnit measurementUnit)
        {
            switch (measurementUnit)
            {
                case MeasurementUnit.Turn:
                    return this.InTurns;
                case MeasurementUnit.Radian:
                    return this.InRadians;
                case MeasurementUnit.Degree:
                    return this.InDegrees;
                case MeasurementUnit.BinaryDegree:
                    return this.InBinaryDegrees;
                case MeasurementUnit.Quadrant:
                    return this.InQuadrants;
                case MeasurementUnit.Sextant:
                    return this.InSextants;
                case MeasurementUnit.Grad:
                    return this.InGrads;
                default:
                    throw new InvalidEnumArgumentException("measurementUnit", (Int32) measurementUnit, measurementUnit.GetType());
            }
        }
        #endregion Instance methods

        #region Static methods
        /// <summary>
        /// Возвращает для указанной единицы измерения углов такое значение величины угла в этой единице, которое соответствует полному углу (одному полному обороту)
        /// </summary>
        /// <param name="measurementUnit">Единица измерения углов</param>
        /// <returns></returns>
        public static Double GetOneTurnValueForUnit(Angle.MeasurementUnit measurementUnit)
        {
            switch (measurementUnit)
            {
                case MeasurementUnit.Turn:
                    return 1.0;
                case MeasurementUnit.Radian:
                    return _2pi;
                case MeasurementUnit.Degree:
                    return 360.0;
                case MeasurementUnit.BinaryDegree:
                    return 256.0;
                case MeasurementUnit.Quadrant:
                    return 4.0;
                case MeasurementUnit.Sextant:
                    return 6.0;
                case MeasurementUnit.Grad:
                    return 400.0;
                default:
                    throw new InvalidEnumArgumentException("measurementUnit", (Int32)measurementUnit, measurementUnit.GetType());
            }
        }

        /// <summary>
        /// Возвращает величину указанного типа угла в указанных единицах измерения уголов
        /// </summary>
        /// <param name="angleType">Тип угла. Поддерживается только такой тип, который чётко описывает величину угла, например, прямой или нулевой угол. 
        /// Напротив, острые и тупые углы описывают диапазоны угловых величин, поэтому недопустимы к использованию данным методом.</param>
        /// <param name="measurementUnit">Единица измерения угла, в которой следует вернуть значение</param>
        /// <returns></returns>
        public static Double GetAngleValueForTypeAndUnit
            (Angle.AngleType angleType, Angle.MeasurementUnit measurementUnit)
        {
            switch (angleType)
            {
                case AngleType.Zero:
                    return 0.0;
                case AngleType.Full:
                    return GetOneTurnValueForUnit(measurementUnit);
                case AngleType.Straight:
                    return GetOneTurnValueForUnit(measurementUnit) / 2.0;
                case AngleType.Right:
                    return GetOneTurnValueForUnit(measurementUnit) / 4.0;
                case AngleType.Acute:
                case AngleType.Obtuse:
                case AngleType.Reflex:
                    throw new InvalidOperationException(
                        String.Format("Невозможно возвратить величину угла для типа {0}, который описывают диапазоны", angleType.ToString()));
                default:
                    throw new InvalidEnumArgumentException("angleType", (Int32)angleType, angleType.GetType());
            }
        }

        /// <summary>
        /// Возвращает тип угла на основании указанной его величины единицы измерения, в которой задана эта величина
        /// </summary>
        /// <param name="inputValue">Значение величины угла. Не может быть отрицательной или превышать один оборот угла.</param>
        /// <param name="measurementUnit">Единица измерения угла, величина которого указана в параметре <paramref name="inputValue"/>.</param>
        /// <returns></returns>
        public static Angle.AngleType GetAngleTypeFromValueOfUnit(Double inputValue, Angle.MeasurementUnit measurementUnit)
        {
            if (inputValue < 0) 
            { throw new ArgumentOutOfRangeException("inputValue", inputValue, "Величина угла не может быть отрицательной"); }
            if (inputValue > GetOneTurnValueForUnit(measurementUnit))
            { throw new ArgumentOutOfRangeException("inputValue", inputValue, "Величина угла не может превышать один оборот угла"); }

            if (NumericTools.AreEqual(_threshold, inputValue, 0.0))
            {
                return AngleType.Zero;
            }
            if (NumericTools.AreEqual(_threshold, inputValue, GetOneTurnValueForUnit(measurementUnit)))
            {
                return AngleType.Full;
            }
            if (NumericTools.AreEqual(_threshold, inputValue, GetAngleValueForTypeAndUnit(AngleType.Right, measurementUnit)))
            {
                return AngleType.Right;
            }
            if (inputValue < GetAngleValueForTypeAndUnit(AngleType.Right, measurementUnit))
            {
                return AngleType.Acute;
            }
            if (NumericTools.AreEqual(_threshold, inputValue, GetAngleValueForTypeAndUnit(AngleType.Straight, measurementUnit)))
            {
                return AngleType.Straight;
            }
            if (inputValue < GetAngleValueForTypeAndUnit(AngleType.Straight, measurementUnit))
            {
                return AngleType.Obtuse;
            }
            return AngleType.Reflex;
        }

        /// <summary>
        /// Определяет, равны ли два указанных угла, вне зависимости от того, в каких единицах измерения углов представлены их величины
        /// </summary>
        /// <param name="first">Первый сравниваемый угол</param>
        /// <param name="second">Второй сравниваемый угол</param>
        /// <returns>Если равны - 'true', иначе 'false'</returns>
        public static Boolean AreEqual(Angle first, Angle second)
        {
            if (first._measurementUnit == second._measurementUnit)
            {
                return NumericTools.AreEqual(_threshold, first._value, second._value);
            }
            else
            {
                return NumericTools.AreEqual(_threshold, first._value, second.GetValueInUnitType(first._measurementUnit));
            }
        }

        /// <summary>
        /// Определяет, равны ли между собой все указанные углы, вне зависимости от того, в каких единицах измерения углов представлены их величины
        /// </summary>
        /// <param name="angles">Набор углов для оценки равенства. Не может быть NULL, пустым или содержать только один угол.</param>
        /// <returns></returns>
        public static Boolean AreEqual(params Angle[] angles)
        {
            if (angles == null) { throw new ArgumentNullException("angles"); }
            if (angles.Length == 0) { throw new ArgumentException("Набор углов не может быть пуст", "angles"); }
            if (angles.Length == 1) { throw new ArgumentException("Набор углов не может содержать только один угол", "angles"); }
            if (angles.Length == 2) { return Angle.AreEqual(angles[0], angles[1]); }

            for (Int32 first_index = 0, second_index = first_index + 1; second_index < angles.Length; first_index++, second_index++)
            {
                if (Angle.AreEqual(angles[first_index], angles[second_index]) == false)
                { return false; }
            }
            return true;
        }

        /// <summary>
        /// Сравнивает между собой два угла вне зависимости от того, в каких единицах измерения углов представлены их величины, 
        /// и определяет, какой из них больше, меньше, или же они равны.
        /// </summary>
        /// <param name="first">Первый сравниваемый угол</param>
        /// <param name="second">Второй сравниваемый угол</param>
        /// <returns>'1' - первый угол больше второго; '-1' - первый угол меньше второго; '0' - оба угла равны между собой</returns>
        public static Int32 Compare(Angle first, Angle second)
        {
            if (first._measurementUnit == second._measurementUnit)
            {
                return first._value.CompareTo(second._value);
            }
            return first._value.CompareTo(second.GetValueInUnitType(first._measurementUnit));
        }
        #endregion Static methods

        #region Comparable and Equatable
        /// <summary>
        /// Возвращает хэш-код данного угла
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                int hashCode = this._value.GetHashCode();
                hashCode = (hashCode * 397) ^ this._measurementUnit.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Определяет равенство данного экземпляра угла с указанным, предварительно пытаясь привести его к данному типу
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override Boolean Equals(Object other)
        {
            if (Object.ReferenceEquals(null, other)) { return false; }
            if (other.GetType() != typeof(Angle)) { return false; }
            return Angle.AreEqual(this, (Angle) other);
        }

        /// <summary>
        /// Определяет равенство данного экземпляра угла с указанным
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(Angle other)
        {
            return Angle.AreEqual(this, other);
        }

        /// <summary>
        /// Сравнивает данный угол с указанным и определяет, какой из них больше, меньше, или же они равны.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo(Angle other)
        {
            return Angle.Compare(this, other);
        }

        /// <summary>
        /// Пытается привести указанный экземпляр неизвестного типа Object к данному типу, а затем 
        /// сравнивает данный угол с указанным и определяет, какой из них больше, меньше, или же они равны.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo(Object other)
        {
            if (Object.ReferenceEquals(other, null) == true)
            {
                throw new ArgumentNullException("other", "Нельзя сравнить угол с NULL");
            }
            if (other.GetType() != this.GetType())
            {
                throw new InvalidOperationException("Нельзя сравнить угол с другим типом");
            }
            return Angle.Compare(this, (Angle) other);
        }
        #endregion Comparable and Equatable

        #region Operators
        /// <summary>
        /// Определяет, равны ли два указанных экземпляра углов между собой
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator ==(Angle first, Angle second)
        {
            return Angle.AreEqual(first, second);
        }

        /// <summary>
        /// Определяет, не равны ли два указанных экземпляра углов между собой
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator !=(Angle first, Angle second)
        {
            return !Angle.AreEqual(first, second);
        }

        /// <summary>
        /// Определяет, строго больше ли величина первого угла по сравнению с величиной второго
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator >(Angle first, Angle second)
        {
            return Angle.Compare(first, second) == 1;
        }

        /// <summary>
        /// Определяет, строго меньше ли величина первого угла по сравнению с величиной второго
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator <(Angle first, Angle second)
        {
            return Angle.Compare(first, second) == -1;
        }

        /// <summary>
        /// Определяет, больше или равна величина первого угла по сравнению с величиной второго
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator >=(Angle first, Angle second)
        {
            return Angle.Compare(first, second) >= 0;
        }

        /// <summary>
        /// Определяет, меньше или равна величина первого угла по сравнению с величиной второго
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator <=(Angle first, Angle second)
        {
            return Angle.Compare(first, second) <= 0;
        }

        /// <summary>
        /// Возвращает угол, являющийся суммой указанных двух углов. 
        /// Если результат сложения величин будет превышать величину полного угла, будет сделан перевод на новый оборот.
        /// </summary>
        /// <param name="first">Первый слагаемый угол</param>
        /// <param name="second">Второй слагаемый угол</param>
        /// <example>270° + 180° = 90°</example>
        /// <returns></returns>
        public static Angle operator +(Angle first, Angle second)
        {
            if (first._measurementUnit == second._measurementUnit)
            {
                return Angle.FromSpecifiedUnit(first._value + second._value, first._measurementUnit);
            }
            return Angle.FromSpecifiedUnit(first._value + second.GetValueInUnitType(first._measurementUnit), first._measurementUnit);
        }

        /// <summary>
        /// Возвращает угол, являющийся разностью уменьшаемого и вычитаемого углов. 
        /// Если результат вычитания является отрицательным, будет сделан перевод на новый оборот. 
        /// Не-ассоциативная и антикоммуникативная операция.
        /// </summary>
        /// <param name="first">Первый уменьшаемый угол</param>
        /// <param name="second">Второй вычитаемый угол</param>
        /// <example>180° - 270° (= 0° - 90° = 360° - 90°) = 270°; 90° - 270° (= 0° - 180° = 360° - 180°) = 180°</example>
        /// <returns></returns>
        public static Angle operator -(Angle first, Angle second)
        {
            if (first._measurementUnit == second._measurementUnit)
            {
                return Angle.FromSpecifiedUnit(first._value - second._value, first._measurementUnit);
            }
            return Angle.FromSpecifiedUnit(first._value - second.GetValueInUnitType(first._measurementUnit), first._measurementUnit);
        }
        #endregion Operators

        #region Cloning
        /// <summary>
        /// Возвращает глубокую копию текущего экземпляра угла
        /// </summary>
        /// <returns></returns>
        public Angle Clone()
        {
            return new Angle(this);
        }
        Object ICloneable.Clone()
        {
            return this.Clone();
        }
        #endregion Cloning

        /// <summary>
        /// Возвращает строковое представление величины и единицы измерения данного угла
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} {1}s", this._value.ToString(CultureInfo.InvariantCulture), this._measurementUnit.ToString());
        }
    }
}

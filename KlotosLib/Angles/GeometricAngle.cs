using System;
using System.Globalization;
using KlotosLib.StringTools;

namespace KlotosLib.Angles
{
    /// <summary>
    /// Инкапсулирует один геометрический угол, включая его величину и единицу измерения. Поддерживает угол в рамках одного оборота. 
    /// Поддерживает геометрические операции над углами. Поддерживает конвертацию угловой величины в разные единицы измерений. 
    /// Неизменяемый значимый тип.
    /// </summary>
    /// <remarks>Для сохранения точности величины угла экземпляр содержит её в той единице измерения, в которой экземпляр был создан. 
    /// Выполняет преобразования в необходимые единицы на лету, не изменяя изначально запомненную единицу измерения.</remarks>
    [Serializable()]
    public struct GeometricAngle : IEquatable<GeometricAngle>, IComparable<GeometricAngle>, IComparable, ICloneable, IFormattable
    {
        #region Constants
        private const Double _threshold = 0.000001;

        private const String _errorMesMuNotInit = "Specified measurement unit is not initialized";

        private const String _errorTemplateMuValueUnknown = "Current value '{0}' of the 'measurementUnit' parameter is not supported";

        private const String _errorMesInvalidNumber = "Input angle value number is NaN or infinity";
        #endregion Constants

        #region Fields
        /// <summary>
        /// Величина угла. Должна быть неотрицательной и не превышать один полный оборот.
        /// </summary>
        private readonly Double _value;

        /// <summary>
        /// Единица измерения угла
        /// </summary>
        private readonly MeasurementUnit _measurementUnit;
        #endregion Fields

        #region Constructors

        private GeometricAngle(GeometricAngle other)
        {
            this._value = other._value;
            this._measurementUnit = other._measurementUnit;
        }

        private GeometricAngle(Double value, MeasurementUnit measurementUnit)
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
        #endregion Private helpers

        #region Static factories

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в указанной единице измерения углов
        /// </summary>
        /// <param name="inputValue">Величина угла. Если отрицательная или превышает один полный оборот в заданных единицах измерения углов — 
        /// будет нормализирована.</param>
        /// <param name="measurementUnit">Единица измерения углов, в которой указана величина <paramref name="inputValue"/>.</param>
        /// <returns></returns>
        public static GeometricAngle FromSpecifiedUnit(Double inputValue, MeasurementUnit measurementUnit)
        {
            if (Double.IsNaN(inputValue) || Double.IsInfinity(inputValue))
            {
                throw new ArgumentOutOfRangeException("inputValue", inputValue, _errorMesInvalidNumber);
            }
            if (NumericTools.IsZero(inputValue) && measurementUnit.IsInitialized == false)
            {
                return new GeometricAngle(0.0, measurementUnit);
            }
            if (measurementUnit.IsInitialized == false)
            {
                throw new ArgumentException(_errorMesMuNotInit, "measurementUnit");
            }
            return new GeometricAngle(NormalizeMeasurementValue(inputValue, measurementUnit.PositionsInOneTurn, _threshold), measurementUnit);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, имеющий такую величину, как в указанном именованном угле, и в указанной единице измерения углов
        /// </summary>
        /// <param name="namedAngle">Именованный угол, значение которого будет использовано</param>
        /// <param name="measurementUnit">Единица измерения углов, которая будет применена для представления величины именованного угла</param>
        /// <returns></returns>
        public static GeometricAngle FromNamedAngle(NamedAngles namedAngle, MeasurementUnit measurementUnit)
        {
            if (Enum.IsDefined(namedAngle.GetType(), namedAngle) == false)
            {
                throw new System.ComponentModel.InvalidEnumArgumentException("namedAngle", (Int32)namedAngle, namedAngle.GetType());
            }
            if (measurementUnit.IsInitialized == false)
            {
                throw new ArgumentException(_errorMesMuNotInit, "measurementUnit");
            }
            switch (namedAngle)
            {
                case NamedAngles.Zero:
                    return new GeometricAngle(0, measurementUnit);
                case NamedAngles.HalfQuarter:
                    if (measurementUnit == MeasurementUnit.Degrees)
                    {
                        return new GeometricAngle(45.0, MeasurementUnit.Degrees);
                    }
                    else if (measurementUnit == MeasurementUnit.BinaryDegrees)
                    {
                        return new GeometricAngle(32.0, MeasurementUnit.BinaryDegrees);
                    }
                    else if (measurementUnit == MeasurementUnit.Turns)
                    {
                        return new GeometricAngle(0.125, MeasurementUnit.Turns);
                    }
                    else if (measurementUnit == MeasurementUnit.Radians)
                    {
                        return new GeometricAngle(Math.PI / 4.0, MeasurementUnit.Radians);
                    }
                    else if (measurementUnit == MeasurementUnit.Quadrants)
                    {
                        return new GeometricAngle(0.5, MeasurementUnit.Quadrants);
                    }
                    else if (measurementUnit == MeasurementUnit.Sextants)
                    {
                        return new GeometricAngle(0.75, MeasurementUnit.Sextants);
                    }
                    else if (measurementUnit == MeasurementUnit.Grads)
                    {
                        return new GeometricAngle(50.0, MeasurementUnit.Grads);
                    }
                    else
                    {
                        throw new InvalidOperationException(String.Format(_errorTemplateMuValueUnknown, measurementUnit.Name));
                    }
                case NamedAngles.Right:
                    if (measurementUnit == MeasurementUnit.Degrees)
                    {
                        return new GeometricAngle(90.0, MeasurementUnit.Degrees);
                    }
                    else if (measurementUnit == MeasurementUnit.BinaryDegrees)
                    {
                        return new GeometricAngle(64.0, MeasurementUnit.BinaryDegrees);
                    }
                    else if (measurementUnit == MeasurementUnit.Turns)
                    {
                        return new GeometricAngle(0.25, MeasurementUnit.Turns);
                    }
                    else if (measurementUnit == MeasurementUnit.Radians)
                    {
                        return new GeometricAngle(Math.PI / 2.0, MeasurementUnit.Radians);
                    }
                    else if (measurementUnit == MeasurementUnit.Quadrants)
                    {
                        return new GeometricAngle(1.0, MeasurementUnit.Quadrants);
                    }
                    else if (measurementUnit == MeasurementUnit.Sextants)
                    {
                        return new GeometricAngle(1.5, MeasurementUnit.Sextants);
                    }
                    else if (measurementUnit == MeasurementUnit.Grads)
                    {
                        return new GeometricAngle(100.0, MeasurementUnit.Grads);
                    }
                    else
                    {
                        throw new InvalidOperationException(String.Format(_errorTemplateMuValueUnknown, measurementUnit.Name));
                    }
                case NamedAngles.TripleHalfQuarter:
                    if (measurementUnit == MeasurementUnit.Degrees)
                    {
                        return new GeometricAngle(135.0, MeasurementUnit.Degrees);
                    }
                    else if (measurementUnit == MeasurementUnit.BinaryDegrees)
                    {
                        return new GeometricAngle(96.0, MeasurementUnit.BinaryDegrees);
                    }
                    else if (measurementUnit == MeasurementUnit.Turns)
                    {
                        return new GeometricAngle(0.375, MeasurementUnit.Turns);
                    }
                    else if (measurementUnit == MeasurementUnit.Radians)
                    {
                        return new GeometricAngle(Math.PI * 0.75, MeasurementUnit.Radians);
                    }
                    else if (measurementUnit == MeasurementUnit.Quadrants)
                    {
                        return new GeometricAngle(1.5, MeasurementUnit.Quadrants);
                    }
                    else if (measurementUnit == MeasurementUnit.Sextants)
                    {
                        return new GeometricAngle(2.25, MeasurementUnit.Sextants);
                    }
                    else if (measurementUnit == MeasurementUnit.Grads)
                    {
                        return new GeometricAngle(150.0, MeasurementUnit.Grads);
                    }
                    else
                    {
                        throw new InvalidOperationException(String.Format(_errorTemplateMuValueUnknown, measurementUnit.Name));
                    }
                case NamedAngles.Straight:
                    if (measurementUnit == MeasurementUnit.Degrees)
                    {
                        return new GeometricAngle(180.0, MeasurementUnit.Degrees);
                    }
                    else if (measurementUnit == MeasurementUnit.BinaryDegrees)
                    {
                        return new GeometricAngle(128.0, MeasurementUnit.BinaryDegrees);
                    }
                    else if (measurementUnit == MeasurementUnit.Turns)
                    {
                        return new GeometricAngle(0.5, MeasurementUnit.Turns);
                    }
                    else if (measurementUnit == MeasurementUnit.Radians)
                    {
                        return new GeometricAngle(Math.PI, MeasurementUnit.Radians);
                    }
                    else if (measurementUnit == MeasurementUnit.Quadrants)
                    {
                        return new GeometricAngle(2, MeasurementUnit.Quadrants);
                    }
                    else if (measurementUnit == MeasurementUnit.Sextants)
                    {
                        return new GeometricAngle(3, MeasurementUnit.Sextants);
                    }
                    else if (measurementUnit == MeasurementUnit.Grads)
                    {
                        return new GeometricAngle(200.0, MeasurementUnit.Grads);
                    }
                    else
                    {
                        throw new InvalidOperationException(String.Format(_errorTemplateMuValueUnknown, measurementUnit.Name));
                    }
                case NamedAngles.TripleQuarters:
                    if (measurementUnit == MeasurementUnit.Degrees)
                    {
                        return new GeometricAngle(270.0, MeasurementUnit.Degrees);
                    }
                    else if (measurementUnit == MeasurementUnit.BinaryDegrees)
                    {
                        return new GeometricAngle(192.0, MeasurementUnit.BinaryDegrees);
                    }
                    else if (measurementUnit == MeasurementUnit.Turns)
                    {
                        return new GeometricAngle(0.75, MeasurementUnit.Turns);
                    }
                    else if (measurementUnit == MeasurementUnit.Radians)
                    {
                        return new GeometricAngle(Math.PI * 1.5, MeasurementUnit.Radians);
                    }
                    else if (measurementUnit == MeasurementUnit.Quadrants)
                    {
                        return new GeometricAngle(3, MeasurementUnit.Quadrants);
                    }
                    else if (measurementUnit == MeasurementUnit.Sextants)
                    {
                        return new GeometricAngle(4.5, MeasurementUnit.Sextants);
                    }
                    else if (measurementUnit == MeasurementUnit.Grads)
                    {
                        return new GeometricAngle(300.0, MeasurementUnit.Grads);
                    }
                    else
                    {
                        throw new InvalidOperationException(String.Format(_errorTemplateMuValueUnknown, measurementUnit.Name));
                    }
                case NamedAngles.Full:
                    if (measurementUnit == MeasurementUnit.Degrees)
                    {
                        return new GeometricAngle(360.0, MeasurementUnit.Degrees);
                    }
                    else if (measurementUnit == MeasurementUnit.BinaryDegrees)
                    {
                        return new GeometricAngle(256.0, MeasurementUnit.BinaryDegrees);
                    }
                    else if (measurementUnit == MeasurementUnit.Turns)
                    {
                        return new GeometricAngle(1.0, MeasurementUnit.Turns);
                    }
                    else if (measurementUnit == MeasurementUnit.Radians)
                    {
                        return new GeometricAngle(Math.PI * 2.0, MeasurementUnit.Radians);
                    }
                    else if (measurementUnit == MeasurementUnit.Quadrants)
                    {
                        return new GeometricAngle(4, MeasurementUnit.Quadrants);
                    }
                    else if (measurementUnit == MeasurementUnit.Sextants)
                    {
                        return new GeometricAngle(6, MeasurementUnit.Sextants);
                    }
                    else if (measurementUnit == MeasurementUnit.Grads)
                    {
                        return new GeometricAngle(400.0, MeasurementUnit.Grads);
                    }
                    else
                    {
                        throw new InvalidOperationException(String.Format(_errorTemplateMuValueUnknown, measurementUnit.Name));
                    }
                default:
                    throw new InvalidOperationException(String.Format("Current 'NamedAngles' value ('{0}') is not supported", namedAngle));
            }
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в оборотах
        /// </summary>
        /// <param name="turns"></param>
        /// <returns></returns>
        public static GeometricAngle FromTurns(Double turns)
        {
            if (Double.IsNaN(turns) || Double.IsInfinity(turns))
            {
                throw new ArgumentOutOfRangeException("turns", turns, _errorMesInvalidNumber);
            }
            return new GeometricAngle(NormalizeMeasurementValue(turns, MeasurementUnit.Turns.PositionsInOneTurn, _threshold), 
                MeasurementUnit.Turns);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в радианах
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static GeometricAngle FromRadians(Double radians)
        {
            if (Double.IsNaN(radians) || Double.IsInfinity(radians))
            {
                throw new ArgumentOutOfRangeException("radians", radians, _errorMesInvalidNumber);
            }
            return new GeometricAngle(NormalizeMeasurementValue(radians, MeasurementUnit.Radians.PositionsInOneTurn, _threshold), 
                MeasurementUnit.Radians);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в градусах
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static GeometricAngle FromDegrees(Double degrees)
        {
            if (Double.IsNaN(degrees) || Double.IsInfinity(degrees))
            {
                throw new ArgumentOutOfRangeException("degrees", degrees, _errorMesInvalidNumber);
            }
            return new GeometricAngle(NormalizeMeasurementValue(degrees, MeasurementUnit.Degrees.PositionsInOneTurn, _threshold), 
                MeasurementUnit.Degrees);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в двоичных градусах
        /// </summary>
        /// <param name="binaryDegrees"></param>
        /// <returns></returns>
        public static GeometricAngle FromBinaryDegrees(Double binaryDegrees)
        {
            if (Double.IsNaN(binaryDegrees) || Double.IsInfinity(binaryDegrees))
            {
                throw new ArgumentOutOfRangeException("binaryDegrees", binaryDegrees, _errorMesInvalidNumber);
            }
            return new GeometricAngle(NormalizeMeasurementValue(binaryDegrees, MeasurementUnit.BinaryDegrees.PositionsInOneTurn, _threshold), 
                MeasurementUnit.BinaryDegrees);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в квадрантах
        /// </summary>
        /// <param name="quadrants"></param>
        /// <returns></returns>
        public static GeometricAngle FromQuadrants(Double quadrants)
        {
            if (Double.IsNaN(quadrants) || Double.IsInfinity(quadrants))
            {
                throw new ArgumentOutOfRangeException("quadrants", quadrants, _errorMesInvalidNumber);
            }
            return new GeometricAngle(NormalizeMeasurementValue(quadrants, MeasurementUnit.Quadrants.PositionsInOneTurn, _threshold), 
                MeasurementUnit.Quadrants);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в секстантах
        /// </summary>
        /// <param name="sextants"></param>
        /// <returns></returns>
        public static GeometricAngle FromSextants(Double sextants)
        {
            if (Double.IsNaN(sextants) || Double.IsInfinity(sextants))
            {
                throw new ArgumentOutOfRangeException("sextants", sextants, _errorMesInvalidNumber);
            }
            return new GeometricAngle(NormalizeMeasurementValue(sextants, MeasurementUnit.Sextants.PositionsInOneTurn, _threshold), 
                MeasurementUnit.Sextants);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в градах (градианах, гонах)
        /// </summary>
        /// <param name="grads"></param>
        /// <returns></returns>
        public static GeometricAngle FromGrads(Double grads)
        {
            if (Double.IsNaN(grads) || Double.IsInfinity(grads))
            {
                throw new ArgumentOutOfRangeException("grads", grads, _errorMesInvalidNumber);
            }
            return new GeometricAngle(NormalizeMeasurementValue(grads, MeasurementUnit.Grads.PositionsInOneTurn, _threshold), 
                MeasurementUnit.Grads);
        }

        /// <summary>
        /// Возвращает нулевой угол без величины (угол по умолчанию, он же безразмерный ноль)
        /// </summary>
        public static readonly GeometricAngle Zero = GeometricAngle.FromSpecifiedUnit(0.0, MeasurementUnit.Uninitialized);
        #endregion Static factories

        #region Instance properties

        /// <summary>
        /// Показывает, содержит ли данный угол безразмерный ноль, т.е. является нулевым и при этом без определённой единицы измерения. 
        /// Получить подобный экземпляр угла можно при помощи поля <see cref="Zero"/> или вызвав пустой конструктор по-умолчанию.
        /// </summary>
        public Boolean IsUnitlessZero { get { return NumericTools.IsZero(this._value) && this._measurementUnit.IsInitialized == false; } }

        /// <summary>
        /// Определяет, является ли данный угол нулевым
        /// </summary>
        public Boolean IsZero { get { return NumericTools.IsZero(this._value); } }

        /// <summary>
        /// Возвращает единицу измерения данного угла
        /// </summary>
        public MeasurementUnit Unit {get { return this._measurementUnit; } }

        /// <summary>
        /// Возвращает величину данного угла
        /// </summary>
        public Double Value {get { return this._value; }}

        /// <summary>
        /// Возвращает величину данного угла в оборотах
        /// </summary>
        public Double InTurns
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.Turns, this._value);
                }
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в радианах
        /// </summary>
        public Double InRadians
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.Radians, this._value);
                }
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в градусах
        /// </summary>
        public Double InDegrees
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.Degrees, this._value);
                }
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в двоичных градусах
        /// </summary>
        public Double InBinaryDegrees
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.BinaryDegrees, this._value);
                }
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в квадрантах
        /// </summary>
        public Double InQuadrants
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.Quadrants, this._value);
                }
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в секстантах
        /// </summary>
        public Double InSextants
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.Sextants, this._value);
                }
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в градах (градианах, гонах)
        /// </summary>
        public Double InGrads
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.Grads, this._value);
                }
            }
        }

        /// <summary>
        /// Возвращает диапазон, к которому относится данный угол
        /// </summary>
        public AngleRanges InRange
        {
            get
            {
                if (this.IsZero)
                {
                    return AngleRanges.Zero;
                }
                GeometricAngle right = GeometricAngle.FromNamedAngle(NamedAngles.Right, this._measurementUnit);
                if (this._value < right.Value)
                {
                    return AngleRanges.Acute;
                }
                if (NumericTools.AreEqual(_threshold, this._value, right.Value))
                {
                    return AngleRanges.Right;
                }
                GeometricAngle straight = GeometricAngle.FromNamedAngle(NamedAngles.Straight, this._measurementUnit);
                if (this._value < straight.Value)
                {
                    return AngleRanges.Obtuse;
                }
                if (NumericTools.AreEqual(_threshold, this._value, straight.Value))
                {
                    return AngleRanges.Straight;
                }
                GeometricAngle full = GeometricAngle.FromNamedAngle(NamedAngles.Full, this._measurementUnit);
                if (this._value < full.Value)
                {
                    return AngleRanges.Reflex;
                }
                if (NumericTools.AreEqual(_threshold, this._value, full.Value))
                {
                    return AngleRanges.Full;
                }
                throw new UnreachableCodeException();
            }
        }
        #endregion Instance properties

        #region Instance methods

        /// <summary>
        /// Возвращает величину данного угла в указанных единицах измерения
        /// </summary>
        /// <param name="measurementUnit"></param>
        /// <returns></returns>
        public Double GetValueInUnit(MeasurementUnit measurementUnit)
        {
            if (this.IsZero)
            {
                return this._value;
            }
            else if (measurementUnit == MeasurementUnit.Turns)
            {
                return this.InTurns;
            }
            else if (measurementUnit == MeasurementUnit.Radians)
            {
                return this.InRadians;
            }
            else if (measurementUnit == MeasurementUnit.Degrees)
            {
                return this.InDegrees;
            }
            else if (measurementUnit == MeasurementUnit.BinaryDegrees)
            {
                return this.InBinaryDegrees;
            }
            else if (measurementUnit == MeasurementUnit.Quadrants)
            {
                return this.InQuadrants;
            }
            else if (measurementUnit == MeasurementUnit.Sextants)
            {
                return this.InSextants;
            }
            else if (measurementUnit == MeasurementUnit.Grads)
            {
                return this.InGrads;
            }
            else
            {
                throw new ArgumentException(_errorMesMuNotInit, "measurementUnit");
            }
        }

        /// <summary>
        /// Возвращает для данного угла дополнительный угол, т.е. такой, сумма которого с текущим равна 90°. 
        /// Если текущий угол больше 90°, т.е. дополнительный угол не может быть взят, выбрасывает исключение.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public GeometricAngle GetComplementaryAngle()
        {
            if (this._measurementUnit.IsInitialized == false)
            {
                return GeometricAngle.FromNamedAngle(NamedAngles.Right, MeasurementUnit.Degrees);
            }
            GeometricAngle right = GeometricAngle.FromNamedAngle(NamedAngles.Right, this._measurementUnit);
            if (this._value > right._value)
            {
                throw new InvalidOperationException(string.Format(
                    "Текущий угол равен {0}, что превышает величину прямого угла {1}, и поэтому смежный угол не может быть взят", this, right));
            }
            return GeometricAngle.FromSpecifiedUnit(right._value - this._value, this._measurementUnit);
        }

        /// <summary>
        /// Возвращает для данного угла смежный угол, т.е. такой, сумма которого с текущим равна 180°. 
        /// Если текущий угол больше 180°, т.е. смежный угол не может быть взят, выбрасывает исключение.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public GeometricAngle GetSupplementaryAngle()
        {
            if (this._measurementUnit.IsInitialized == false)
            {
                return GeometricAngle.FromNamedAngle(NamedAngles.Straight, MeasurementUnit.Degrees);
            }
            GeometricAngle straight = GeometricAngle.FromNamedAngle(NamedAngles.Straight, this._measurementUnit);
            if (this._value > straight._value)
            {
                throw new InvalidOperationException(string.Format(
                    "Текущий угол равен {0}, что превышает величину развёрнутого угла {1}, и поэтому смежный угол не может быть взят", this, straight));
            }
            return GeometricAngle.FromSpecifiedUnit(straight._value - this._value, this._measurementUnit);
        }

        /// <summary>
        /// Возвращает для данного угла сопряженный угол, т.е. такой, сумма которого с текущим равна 360°. 
        /// Если текущий угол равен 360°, возвращает нулевой угол.
        /// </summary>
        /// <returns></returns>
        public GeometricAngle GetExplementaryAngle()
        {
            if (this._measurementUnit.IsInitialized == false)
            {
                return GeometricAngle.FromNamedAngle(NamedAngles.Full, MeasurementUnit.Degrees);
            }
            GeometricAngle full = GeometricAngle.FromNamedAngle(NamedAngles.Full, this._measurementUnit);
            if (this._value > full._value)
            {
                throw new InvalidOperationException(string.Format(
                    "Текущий угол равен {0}, что превышает величину полного угла {1}, и поэтому смежный угол не может быть взят", this, full));
            }
            return GeometricAngle.FromSpecifiedUnit(full._value - this._value, this._measurementUnit);
        }

        #endregion Instance methods

        #region Comparable and Equatable
        /// <summary>
        /// Определяет, равны ли два указанных угла, вне зависимости от того, в каких единицах измерения углов представлены их величины
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean AreEqual(GeometricAngle first, GeometricAngle second)
        {
            Boolean firstIsZero = first.IsZero;
            Boolean secondIsZero = second.IsZero;
            Boolean result;
            if (firstIsZero == true && secondIsZero == true)
            {
                result = true;
            }
            else if (firstIsZero == true || secondIsZero == true)
            {
                result = false;
            }
            else if (first._measurementUnit == second._measurementUnit)
            {
                result = NumericTools.AreEqual(_threshold, first._value, second._value);
            }
            else
            {
                result = NumericTools.AreEqual(_threshold, first._value, second.GetValueInUnit(first._measurementUnit));
            }
            return result;
        }

        /// <summary>
        /// Определяет, равны ли между собой все указанные углы, вне зависимости от того, в каких единицах измерения углов представлены их величины
        /// </summary>
        /// <param name="geometricAngles">Набор углов для оценки равенства. Не может быть NULL, пустым или содержать только один угол.</param>
        /// <returns></returns>
        public static Boolean AreEqual(params GeometricAngle[] geometricAngles)
        {
            if (geometricAngles == null) { throw new ArgumentNullException("geometricAngles"); }
            if (geometricAngles.Length == 0) { throw new ArgumentException("Набор углов не может быть пуст", "geometricAngles"); }
            if (geometricAngles.Length == 1) { throw new ArgumentException("Набор углов не может содержать только один угол", "geometricAngles"); }
            if (geometricAngles.Length == 2) { return GeometricAngle.AreEqual(geometricAngles[0], geometricAngles[1]); }

            for (Int32 firstIndex = 0, secondIndex = firstIndex + 1; secondIndex < geometricAngles.Length; firstIndex++, secondIndex++)
            {
                if (GeometricAngle.AreEqual(geometricAngles[firstIndex], geometricAngles[secondIndex]) == false)
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
        public static Int32 Compare(GeometricAngle first, GeometricAngle second)
        {
            Boolean firstIsZero = first.IsZero;
            Boolean secondIsZero = second.IsZero;
            if (firstIsZero == true && secondIsZero == true)
            {
                return 0;
            }
            else if (firstIsZero == true && secondIsZero == false)
            {
                return -1;
            }
            else if (firstIsZero == false && secondIsZero == true)
            {
                return 1;
            }
            else if (first._measurementUnit == second._measurementUnit)
            {
                return first._value.CompareTo(second._value);
            }
            else
            {
                return first._value.CompareTo(second.GetValueInUnit(first._measurementUnit));
            }
        }

        /// <summary>
        /// Возвращает хэш-код данного геометрического угла
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = this._value.GetHashCode();
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
            if (other.GetType() != typeof(GeometricAngle)) { return false; }
            return GeometricAngle.AreEqual(this, (GeometricAngle)other);
        }

        /// <summary>
        /// Определяет равенство данного экземпляра угла с указанным
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(GeometricAngle other)
        {
            return GeometricAngle.AreEqual(this, other);
        }

        /// <summary>
        /// Сравнивает данный угол с указанным и определяет, какой из них больше, меньше, или же они равны.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo(GeometricAngle other)
        {
            return GeometricAngle.Compare(this, other);
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
            return GeometricAngle.Compare(this, (GeometricAngle)other);
        }
        #endregion Comparable and Equatable
        
        #region Operators
        /// <summary>
        /// Определяет, равны ли два указанных экземпляра углов между собой
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator ==(GeometricAngle first, GeometricAngle second)
        {
            return GeometricAngle.AreEqual(first, second);
        }

        /// <summary>
        /// Определяет, не равны ли два указанных экземпляра углов между собой
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator !=(GeometricAngle first, GeometricAngle second)
        {
            return !GeometricAngle.AreEqual(first, second);
        }

        /// <summary>
        /// Определяет, строго больше ли величина первого угла по сравнению с величиной второго
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator >(GeometricAngle first, GeometricAngle second)
        {
            return GeometricAngle.Compare(first, second) == 1;
        }

        /// <summary>
        /// Определяет, строго меньше ли величина первого угла по сравнению с величиной второго
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator <(GeometricAngle first, GeometricAngle second)
        {
            return GeometricAngle.Compare(first, second) == -1;
        }

        /// <summary>
        /// Определяет, больше или равна величина первого угла по сравнению с величиной второго
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator >=(GeometricAngle first, GeometricAngle second)
        {
            return GeometricAngle.Compare(first, second) >= 0;
        }

        /// <summary>
        /// Определяет, меньше или равна величина первого угла по сравнению с величиной второго
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator <=(GeometricAngle first, GeometricAngle second)
        {
            return GeometricAngle.Compare(first, second) <= 0;
        }

        /// <summary>
        /// Возвращает угол, являющийся суммой указанных двух углов. 
        /// Если результат сложения величин будет превышать величину полного угла, будет сделан перевод на новый оборот. 
        /// Единица измерения результирующего угла будет такой, как у первого угла.
        /// </summary>
        /// <param name="first">Первый слагаемый угол</param>
        /// <param name="second">Второй слагаемый угол</param>
        /// <example>30° + 30° = 60°; 270° + 180° (= 450° = 360° + 90°) = 90°</example>
        /// <returns></returns>
        public static GeometricAngle operator +(GeometricAngle first, GeometricAngle second)
        {
            if (first._measurementUnit == second._measurementUnit)
            {
                return GeometricAngle.FromSpecifiedUnit(first._value + second._value, first._measurementUnit);
            }
            return GeometricAngle.FromSpecifiedUnit(first._value + second.GetValueInUnit(first._measurementUnit), first._measurementUnit);
        }

        /// <summary>
        /// Возвращает угол, являющийся разностью уменьшаемого и вычитаемого углов. 
        /// Если результат вычитания является отрицательным, будет сделан перевод на новый оборот. 
        /// Единица измерения результирующего угла будет такой, как у первого угла. 
        /// Не-ассоциативная и антикоммуникативная операция.
        /// </summary>
        /// <param name="first">Первый уменьшаемый угол</param>
        /// <param name="second">Второй вычитаемый угол</param>
        /// <example>180° - 270° (= 0° - 90° = 360° - 90°) = 270°; 90° - 270° (= 0° - 180° = 360° - 180°) = 180°</example>
        /// <returns></returns>
        public static GeometricAngle operator -(GeometricAngle first, GeometricAngle second)
        {
            if (first._measurementUnit == second._measurementUnit)
            {
                return GeometricAngle.FromSpecifiedUnit(first._value - second._value, first._measurementUnit);
            }
            return GeometricAngle.FromSpecifiedUnit(first._value - second.GetValueInUnit(first._measurementUnit), first._measurementUnit);
        }
        #endregion Operators

        #region Cloning
        /// <summary>
        /// Возвращает глубокую копию текущего экземпляра угла
        /// </summary>
        /// <returns></returns>
        public GeometricAngle Clone()
        {
            return new GeometricAngle(this);
        }
        Object ICloneable.Clone()
        {
            return this.Clone();
        }
        #endregion Cloning

        #region ToString
        /// <summary>
        /// Возвращает строковое представление величины и единицы измерения данного угла
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return String.Format("{0} {1}", this._value.ToString(System.Globalization.CultureInfo.InvariantCulture), this._measurementUnit.ToString());
        }

        /// <summary>
        /// Возвращает строковое представление величины и единицы измерения данного угла 
        /// с использованием указанного формата и сведений об особенностях форматирования 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public String ToString(String format, IFormatProvider formatProvider)
        {
            return String.Format("{0} {1}", this._value.ToString(format, formatProvider), this._measurementUnit.ToString());
        }
        #endregion ToString

        #region Parsing

        /// <summary>
        /// Пытается распарсить входную строку как геометрический угол с учётом указанных сведений о форматировании числа 
        /// и возвращает его в случае успешного парсинга
        /// </summary>
        /// <param name="input">Входная строка, которая предположительно содержит один геометрический угол</param>
        /// <param name="provider">Сведения о форматировании числа, представляющего величину угла</param>
        /// <param name="result">Распарсенный угол в случае успешного парсинга или безразмерный ноль в случае неуспешного</param>
        /// <returns>Результат операции парсинга</returns>
        public static Boolean TryParse(String input, IFormatProvider provider, out GeometricAngle result)
        {
            result = GeometricAngle.Zero;
            if (input.HasAlphaNumericChars() == false)
            {
                return false;
            }
            String temp = input.Trim();

            String number = null;
            String unit = null;
            for (Int32 i = 0; i < temp.Length; i++)
            {
                Char c = temp[i];
                if (Char.IsWhiteSpace(c))
                {
                    number = temp.Substring(0, i);
                    unit = temp.Substring(i + 1);
                    break;
                }
                if (Char.IsLetter(c) || c == '°')
                {
                    number = temp.Substring(0, i);
                    unit = temp.Substring(i);
                    break;
                }
            }
            if (unit.IsStringNullEmptyWs() || number.IsStringNullEmptyWs())
            {
                return false;
            }
            MeasurementUnit parsedUnit;
            Boolean unitParsingResult = MeasurementUnit.TryParse(unit, out parsedUnit);
            if (unitParsingResult == false)
            {
                return false;
            }
            Double parsedNumber;
            Boolean numberParsingResult = Double
                .TryParse(number, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, provider, out parsedNumber);
            if (numberParsingResult == false)
            {
                return false;
            }
            result = FromSpecifiedUnit(parsedNumber, parsedUnit);
            return true;
        }

        /// <summary>
        /// Пытается распарсить входную строку как геометрический угол и возвращает его в случае успешного парсинга
        /// </summary>
        /// <param name="input">Входная строка, которая предположительно содержит один геометрический угол</param>
        /// <param name="result">Распарсенный угол в случае успешного парсинга или безразмерный ноль в случае неуспешного</param>
        /// <returns>Результат операции парсинга</returns>
        public static Boolean TryParse(String input, out GeometricAngle result)
        {
            return TryParse(input, CultureInfo.InvariantCulture, out result);
        }
        #endregion Parsing
    }
}
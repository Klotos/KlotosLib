using System;
using KlotosLib.StringTools;

namespace KlotosLib.Angles
{
    /// <summary>
    /// Представляет единицы измерения углов и соответствующие методы для их преобразования, сравнения и анализа
    /// </summary>
    [Serializable()]
    public struct MeasurementUnit : IEquatable<MeasurementUnit>
    {
        #region Constants
        private const Double _2pi = 2 * Math.PI;

        private const Byte _maxType = 7;

        private const String _UnknownUnitErrorMessage = "Unknown measurement unit has occured";

        private const String _UninitializedErrorMessage = "This instance is not initialized and thus operation cannot be performed";
        #endregion Constants

        #region Fields

        /// <summary>
        /// 0 - uninitialized, 1 - turns, 2 - radians, 3 - degrees, 4 - binary degrees, 5 - quadrants, 6 - sextants, 7 - grads, 8 and other - unused (reserved)
        /// </summary>
        private readonly byte _type;

        #endregion Fields

        private MeasurementUnit(byte type)
        {
            this._type = type;
        }

        #region Instance members

        /// <summary>
        /// Определяет, является ли эта единица измерения углов проинициализированной, т.е. содержит ли она осмысленное и допустимое значение
        /// </summary>
        public Boolean IsInitialized
        {
            get { return this._type > 0 && this._type <= _maxType; }
        }

        /// <summary>
        /// Возвращает имя этой единицы измерения углов во множественном числе
        /// </summary>
        public String Name
        {
            get
            {
                switch (this._type)
                {
                    case 1: return "Turns";
                    case 2: return "Radians";
                    case 3: return "Degrees";
                    case 4: return "Binary degrees";
                    case 5: return "Quadrants";
                    case 6: return "Sextants";
                    case 7: return "Grads";
                    case 0:
                        throw new InvalidOperationException(_UninitializedErrorMessage);
                    default:
                        throw new InvalidOperationException(_UnknownUnitErrorMessage);
                }
            }
        }

        /// <summary>
        /// Возвращает имя этой единицы измерения углов в CSS3-совместимом формате (только для оборотов, радианов, градусов и градов).
        /// </summary>
        public string CssName
        {
            get
            {
                switch (this._type)
                {
                    case 1: return "turn";
                    case 2: return "rad";
                    case 3: return "deg";
                    case 7: return "grad";
                    case 4:
                    case 5:
                    case 6:
                        throw new NotSupportedException("CSS3 не поддерживает данный тип единиц измерения углов");
                    case 0:
                        throw new InvalidOperationException(_UninitializedErrorMessage);
                    default:
                        throw new InvalidOperationException(_UnknownUnitErrorMessage);
                }
                
            }
        }

        /// <summary>
        /// Возвращает имя этой единицы измерения углов во множественном числе
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Возвращает угловую величину для данной единицы измерения, которая соответствует полному углу (одному обороту)
        /// </summary>
        public Double PositionsInOneTurn
        {
            get
            {
                switch (this._type)
                {
                    case 1: return 1;
                    case 2: return _2pi;
                    case 3: return 360;
                    case 4: return 256;
                    case 5: return 4;
                    case 6: return 6;
                    case 7: return 400;
                    case 0:
                        throw new InvalidOperationException(_UninitializedErrorMessage);
                    default:
                        throw new InvalidOperationException(_UnknownUnitErrorMessage);
                }
            }
        }
        #endregion Instance members

        /// <summary>
        /// Конвертирует указанную угловую величину с одной единицы измерения в другую
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static Double ConvertFromTo(MeasurementUnit from, MeasurementUnit to, Double inputValue)
        {
            if (from == to) { return inputValue; }
            if (from.IsInitialized == false) { throw new ArgumentException("First unit ('from') is uninitialized", "from"); }
            if (to.IsInitialized == false) { throw new ArgumentException("Second unit ('to') is uninitialized", "to"); }
            if (Double.IsNaN(inputValue) || Double.IsInfinity(inputValue)) { throw new ArgumentException("Input value is an invalid number", "inputValue"); }
            
            return inputValue * to.PositionsInOneTurn / from.PositionsInOneTurn;
        }

        #region Equatable

        /// <summary>
        /// Определяет, равен ли данный экземпляр единицы измерения углов указанному
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(MeasurementUnit other)
        {
            Boolean firstIsInit = this.IsInitialized;
            Boolean secondIsInit = other.IsInitialized;
            if (firstIsInit == true && secondIsInit == true)
            {
                return this._type == other._type;
            }
            else if(firstIsInit == false && secondIsInit == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Определяет, равен ли данный экземпляр единицы измерения углов указанному, предварительно пытаясь привести его к данному типу
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override Boolean Equals(object other)
        {
            if (Object.ReferenceEquals(null, other)) { return false; }
            if (other.GetType() != typeof(MeasurementUnit)) { return false; }
            return this.Equals((MeasurementUnit)other);
        }

        public override Int32 GetHashCode()
        {
            return this._type.GetHashCode();
        }

        /// <summary>
        /// Определяет, равны ли два указанных экземпляра единиц измерения углов между собой
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator ==(MeasurementUnit first, MeasurementUnit second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Определяет, не равны ли два указанных экземпляра единиц измерения углов между собой
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator !=(MeasurementUnit first, MeasurementUnit second)
        {
            return !first.Equals(second);
        }
        #endregion Equatable

        #region Factories

        /// <summary>
        /// Представляет специальное неинициализированное значение. Используется при парсинге и как значение по умолчанию при создании экземпляра через конструктор.
        /// </summary>
        public static readonly MeasurementUnit Uninitialized = new MeasurementUnit(0);

        /// <summary>
        /// Обороты
        /// </summary>
        public static readonly MeasurementUnit Turns = new MeasurementUnit(1);

        /// <summary>
        /// Радианы
        /// </summary>
        public static readonly MeasurementUnit Radians = new MeasurementUnit(2);

        /// <summary>
        /// Градусы
        /// </summary>
        public static readonly MeasurementUnit Degrees = new MeasurementUnit(3);

        /// <summary>
        /// Двоичные градусы
        /// </summary>
        public static readonly MeasurementUnit BinaryDegrees = new MeasurementUnit(4);

        /// <summary>
        /// Квадранты
        /// </summary>
        public static readonly MeasurementUnit Quadrants = new MeasurementUnit(5);

        /// <summary>
        /// Секстанты
        /// </summary>
        public static readonly MeasurementUnit Sextants = new MeasurementUnit(6);

        /// <summary>
        /// Градианы (грады, гоны)
        /// </summary>
        public static readonly MeasurementUnit Grads = new MeasurementUnit(7);
        #endregion Factories

        #region Parsing

        /// <summary>
        /// Пытается преобразовать строковое представление единицы измерения углов в эквивалентное ему строго типизированное представление 
        /// и возвращает результат успешности (или неуспешности) данного преобразования
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parsedValue"></param>
        /// <returns></returns>
        public static Boolean TryParse(String input, out MeasurementUnit parsedValue)
        {
            parsedValue = MeasurementUnit.Uninitialized;
            if (input.HasVisibleChars() == false)
            {
                return false;
            }
            String temp = input.Trim().ToLowerInvariant();
            switch (temp)
            {
                case "turn":
                case "turns":
                    parsedValue = Turns;
                    return true;
                case "rad":
                case "rads":
                case "radian":
                case "radians":
                    parsedValue = Radians;
                    return true;
                case "°":
                case "deg":
                case "degs":
                case "degree":
                case "degrees":
                    parsedValue = Degrees;
                    return true;
                case "binary deg":
                case "binary degs":
                case "binary degree":
                case "binary degrees":
                    parsedValue = BinaryDegrees;
                    return true;
                case "quad":
                case "quads":
                case "quadrant":
                case "quadrants":
                    parsedValue = Quadrants;
                    return true;
                case "sextant":
                case "sextants":
                    parsedValue = Sextants;
                    return true;
                case "gradian":
                case "gradians":
                case "grad":
                case "grads":
                case "grade":
                case "grades":
                case "gon":
                case "gons":
                    parsedValue = Grads;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Преобразовывает строковое представление единицы измерения углов в эквивалентное ему строго типизированное представление 
        /// или выбрасывает исключение в случае невозможности успешно совершить преобразование
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static MeasurementUnit Parse(String input)
        {
            if (input.HasAlphaNumericChars() == false)
            {
                throw new ArgumentException("Specified string is NULL, empty, or has no visible characters", "input");
            }
            MeasurementUnit output;
            Boolean result = TryParse(input, out output);
            if (result == true)
            {
                return output;
            }
            else
            {
                throw new FormatException("Cannot parse specified string");
            }
        }

        #endregion Parsing
    }
}
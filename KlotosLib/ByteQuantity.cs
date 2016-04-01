using System;
using System.Globalization;

namespace KlotosLib
{
    /// <summary>
    /// Инкапсулирует байтовую величину, предоставляя различные методы преобразования
    /// </summary>
    [Serializable()]
    public struct ByteQuantity : IEquatable<ByteQuantity>, IComparable<ByteQuantity>, IComparable, IConvertible, ICloneable
    {
        /// <summary>
        /// Обозначает знак десятичного разделителя
        /// </summary>
        public enum DecimalSeparatorSign : byte
        {
            /// <summary>
            /// Запятая
            /// </summary>
            Comma,
            /// <summary>
            /// Точка
            /// </summary>
            Point
        }

        #region Constants
        private const Int64 _1024 = 1024L;
        private const Int64 _1000 = 1000L;

        private const Int64 _x1Bin = _1024;
        private const Int64 _x2Bin = _1024 * _1024;
        private const Int64 _x3Bin = _1024 * _1024 * _1024;
        private const Int64 _x4Bin = _1024 * _1024 * _1024 * _1024;
        private const Int64 _x5Bin = _1024 * _1024 * _1024 * _1024 * _1024;

        private const Int64 _x1Dec = _1000;
        private const Int64 _x2Dec = _1000 * _1000;
        private const Int64 _x3Dec = _1000 * _1000 * _1000;
        private const Int64 _x4Dec = _1000 * _1000 * _1000 * _1000;
        private const Int64 _x5Dec = _1000 * _1000 * _1000 * _1000 * _1000;
        #endregion

        /// <summary>
        /// Содержит количество байтов
        /// </summary>
        private readonly Int64 _value;

        /// <summary>
        /// Создаёт экземпляр с указанным количеством байтов
        /// </summary>
        /// <param name="Value"></param>
        private ByteQuantity(Int64 Value)
        {
            this._value = Value;
        }

        #region Getters
        /// <summary>
        /// Возвращает текущее значение в битах (Bit)
        /// </summary>
        public Int64 Bits { get { return this._value * 8; } }

        /// <summary>
        /// Возвращаеттекущее значение в байтах (Byte)
        /// </summary>
        public Int64 Bytes { get { return this._value; } }

        #region Getters for binary prefixes
        private Decimal GetBytes(Byte Precision, Int64 Multiplier)
        {
            return ((Decimal)this._value / (Decimal)Multiplier).Rounding(Precision);
        }

        /// <summary>
        /// Возвращает текущее значение в кибибайтах (KiB), округляя его с указанной точностью
        /// </summary>
        /// <param name="Precision">Точность, выраженная в количестве цифр после запятой, которые будут присутствовать в возвращаемом значении</param>
        /// <returns></returns>
        public Decimal GetKibibytes(Byte Precision)
        {
            if (Precision > 3) { Precision = 3; }
            return this.GetBytes(Precision, _x1Bin);
        }

        /// <summary>
        /// Возвращает текущее значение в кибибайтах (KiB) с полной точностью
        /// </summary>
        public Decimal Kibibytes { get { return this.GetBytes(3, _x1Bin); } }

        /// <summary>
        /// Возвращает текущее значение в мебибайтах (MiB), округляя его с указанной точностью
        /// </summary>
        /// <param name="Precision">Точность, выраженная в количестве цифр после запятой, которые будут присутствовать в возвращаемом значении</param>
        /// <returns></returns>
        public Decimal GetMebibytes(Byte Precision)
        {
            if (Precision > 6) { Precision = 6; }
            return this.GetBytes(Precision, _x2Bin);
        }

        /// <summary>
        /// Возвращает текущее значение в мебибайтах (MiB) с полной точностью
        /// </summary>
        public Decimal Mebibytes { get { return this.GetBytes(6, _x2Bin); } }

        /// <summary>
        /// Возвращает текущее значение в гибибайтах (GiB), округляя его с указанной точностью
        /// </summary>
        /// <param name="Precision">Точность, выраженная в количестве цифр после запятой, которые будут присутствовать в возвращаемом значении</param>
        /// <returns></returns>
        public Decimal GetGibibytes(Byte Precision)
        {
            if (Precision > 9) { Precision = 9; }
            return this.GetBytes(Precision, _x3Bin);
        }

        /// <summary>
        /// Возвращает текущее значение в гибибайтах (GiB) с полной точностью
        /// </summary>
        public Decimal Gibibytes { get { return this.GetBytes(9, _x3Bin); } }

        /// <summary>
        /// Возвращает текущее значение в тебибайтах (TiB), округляя его с указанной точностью
        /// </summary>
        /// <param name="Precision">Точность, выраженная в количестве цифр после запятой, которые будут присутствовать в возвращаемом значении</param>
        /// <returns></returns>
        public Decimal GetTebibytes(Byte Precision)
        {
            if (Precision > 12) { Precision = 12; }
            return this.GetBytes(Precision, _x4Bin);
        }

        /// <summary>
        /// Возвращает текущее значение в тебибайтах (TiB) с полной точностью
        /// </summary>
        public Decimal Tebibytes { get { return this.GetBytes(12, _x4Bin); } }

        /// <summary>
        /// Возвращает текущее значение в пебибайтах (PiB), округляя его с указанной точностью
        /// </summary>
        /// <param name="Precision">Точность, выраженная в количестве цифр после запятой, которые будут присутствовать в возвращаемом значении</param>
        /// <returns></returns>
        public Decimal GetPebibytes(Byte Precision)
        {
            if (Precision > 15) { Precision = 15; }
            return this.GetBytes(Precision, _x5Bin);
        }

        /// <summary>
        /// Возвращает текущее значение в пебибайтах (PiB) с полной точностью
        /// </summary>
        public Decimal Pebibytes { get { return this.GetBytes(15, _x5Bin); } }
        #endregion

        #region Getters for decimal prefixes
        /// <summary>
        /// Возвращает текущее значение в килобайтах (KB), округляя его с указанной точностью
        /// </summary>
        /// <param name="Precision">Точность, выраженная в количестве цифр после запятой, которые будут присутствовать в возвращаемом значении</param>
        /// <returns></returns>
        public Decimal GetKilobytes(Byte Precision)
        {
            if (Precision > 3) { Precision = 3; }
            return this.GetBytes(Precision, _x1Dec);
        }

        /// <summary>
        /// Возвращает текущее значение в килобайтах (KB) с полной точностью
        /// </summary>
        public Decimal Kilobytes { get { return this.GetBytes(3, _x1Dec); } }

        /// <summary>
        /// Возвращает текущее значение в мегабайтах (MB), округляя его с указанной точностью
        /// </summary>
        /// <param name="Precision">Точность, выраженная в количестве цифр после запятой, которые будут присутствовать в возвращаемом значении</param>
        /// <returns></returns>
        public Decimal GetMegabytes(Byte Precision)
        {
            if (Precision > 6) { Precision = 6; }
            return this.GetBytes(Precision, _x2Dec);
        }

        /// <summary>
        /// Возвращает текущее значение в мегабайтах (MB) с полной точностью
        /// </summary>
        public Decimal Megabytes { get { return this.GetBytes(6, _x2Dec); } }

        /// <summary>
        /// Возвращает текущее значение в гигабайтах (GB), округляя его с указанной точностью
        /// </summary>
        /// <param name="Precision">Точность, выраженная в количестве цифр после запятой, которые будут присутствовать в возвращаемом значении</param>
        /// <returns></returns>
        public Decimal GetGigabytes(Byte Precision)
        {
            if (Precision > 9) { Precision = 9; }
            return this.GetBytes(Precision, _x3Dec);
        }

        /// <summary>
        /// Возвращает текущее значение в гигабайтах (GB) с полной точностью
        /// </summary>
        public Decimal Gigabytes { get { return this.GetBytes(9, _x3Dec); } }

        /// <summary>
        /// Возвращает текущее значение в терабайтах (TB), округляя его с указанной точностью
        /// </summary>
        /// <param name="Precision">Точность, выраженная в количестве цифр после запятой, которые будут присутствовать в возвращаемом значении</param>
        /// <returns></returns>
        public Decimal GetTerabytes(Byte Precision)
        {
            if (Precision > 12) { Precision = 12; }
            return this.GetBytes(Precision, _x4Dec);
        }

        /// <summary>
        /// Возвращает текущее значение в терабайтах (TB) с полной точностью
        /// </summary>
        public Decimal Terabytes { get { return this.GetBytes(12, _x4Dec); } }

        /// <summary>
        /// Возвращает текущее значение в петабайтах (PB), округляя его с указанной точностью
        /// </summary>
        /// <param name="Precision">Точность, выраженная в количестве цифр после запятой, которые будут присутствовать в возвращаемом значении</param>
        /// <returns></returns>
        public Decimal GetPetabytes(Byte Precision)
        {
            if (Precision > 15) { Precision = 15; }
            return this.GetBytes(Precision, _x5Dec);
        }

        /// <summary>
        /// Возвращает текущее значение в петабайтах (PB) с полной точностью
        /// </summary>
        public Decimal Petabytes { get { return this.GetBytes(15, _x5Dec); } }
        #endregion

        #region Smart getters
        /// <summary>
        /// Возвращает строковое представление округлённой байтовой величины с заданной точностью в двоичных префиксах. 
        /// Множитель выбирается автоматически в зависимости от количества байтов
        /// </summary>
        /// <param name="Precision">Точность, определяющая количество знаков после запятой, которое будет выведено</param>
        /// <param name="DecimalSeparator">Десятичный разделитель между целой и дробной частью</param>
        /// <returns></returns>
        public String ToStringWithBinaryPrefix(Byte Precision, ByteQuantity.DecimalSeparatorSign DecimalSeparator)
        {
            NumberFormatInfo nfi = (NumberFormatInfo)NumberFormatInfo.InvariantInfo.Clone();
            nfi.NumberDecimalSeparator = DecimalSeparator == DecimalSeparatorSign.Point ? "." : ",";

            if (this._value < _x1Bin)
            {
                return this._value.ToString(CultureInfo.InvariantCulture) + " B";
            }
            if (this._value < _x2Bin && this._value >= _x1Bin)
            {
                return this.GetKibibytes(Precision).ToString(nfi) + " KiB";
            }
            if (this._value < _x3Bin && this._value >= _x2Bin)
            {
                return this.GetMebibytes(Precision).ToString(nfi) + " MiB";
            }
            if (this._value < _x4Bin && this._value >= _x3Bin)
            {
                return this.GetGibibytes(Precision).ToString(nfi) + " GiB";
            }
            if (this._value < _x5Bin && this._value >= _x4Bin)
            {
                return this.GetTebibytes(Precision).ToString(nfi) + " TiB";
            }
            //if (this._value >= _x5Bin)
            {
                return this.GetPebibytes(Precision).ToString(nfi) + " PiB";
            }
        }

        /// <summary>
        /// Возвращает строковое представление округлённой байтовой величины с заданной точностью в десятичных префиксах. 
        /// Множитель выбирается автоматически в зависимости от количества байтов
        /// </summary>
        /// <param name="Precision">Точность, определяющая количество знаков после запятой, которое будет выведено</param>
        /// <param name="DecimalSeparator">Десятичный разделитель между целой и дробной частью</param>
        /// <returns></returns>
        public String ToStringWithDecimalPrefix(Byte Precision, ByteQuantity.DecimalSeparatorSign DecimalSeparator)
        {
            NumberFormatInfo nfi = (NumberFormatInfo)NumberFormatInfo.InvariantInfo.Clone();
            nfi.NumberDecimalSeparator = DecimalSeparator == DecimalSeparatorSign.Point ? "." : ",";

            if (this._value < _x1Dec)
            {
                return this._value.ToString(CultureInfo.InvariantCulture) + " B";
            }
            if (this._value < _x2Dec && this._value >= _x1Dec)
            {
                return this.GetKilobytes(Precision).ToString(nfi) + " KB";
            }
            if (this._value < _x3Dec && this._value >= _x2Dec)
            {
                return this.GetMegabytes(Precision).ToString(nfi) + " MB";
            }
            if (this._value < _x4Dec && this._value >= _x3Dec)
            {
                return this.GetGigabytes(Precision).ToString(nfi) + " GB";
            }
            if (this._value < _x5Dec && this._value >= _x4Dec)
            {
                return this.GetTerabytes(Precision).ToString(nfi) + " TB";
            }
            if (this._value >= _x5Dec)
            {
                return this.GetPetabytes(Precision).ToString(nfi) + " PB";
            }
            throw new UnreachableCodeException();
        }
        #endregion

        #endregion

        #region Constructors
        /// <summary>
        /// Возвращает новый экземпляр байтовой величины, содержащий указанное количество битов
        /// </summary>
        /// <param name="BitValue">Количество бит. Если меньше 0, будет выброшено исключение</param>
        /// <returns>Новый экземпляр структуры</returns>
        /// <exception cref="ArgumentOutOfRangeException">Количество бит не может быть отрицательным</exception>
        public static ByteQuantity FromBits(Int64 BitValue)
        {
            if (BitValue < 0) { throw new ArgumentOutOfRangeException("BitValue", BitValue, "Количество бит не может быть отрицательным"); }
            if (BitValue == 0) { return new ByteQuantity(0); }
            Int64 remainder;
            Int64 temp = Math.DivRem(BitValue, 8L, out remainder);
            if (remainder == 0)
            { return new ByteQuantity(temp); }
            else { return new ByteQuantity(temp + 1L); }
        }

        /// <summary>
        /// Возвращает новый экземпляр байтовой величины, содержащий указанное количество байт
        /// </summary>
        /// <param name="ByteValue">Количество байт. Если меньше 0, будет выброшено исключение</param>
        /// <returns>Новый экземпляр структуры</returns>
        /// <exception cref="ArgumentOutOfRangeException">Количество байт не может быть отрицательным</exception>
        public static ByteQuantity FromBytes(Int64 ByteValue)
        {
            if (ByteValue < 0) { throw new ArgumentOutOfRangeException("ByteValue", ByteValue, "Количество байт не может быть отрицательным"); }
            if (ByteValue == 0) { return new ByteQuantity(0); }
            return new ByteQuantity(ByteValue);
        }

        #region Constructors for binary prefixes
        /// <summary>
        /// Возвращает новый экземпляр байтовой величины, содержащий указанное количество кибибайт (KiB), где 1 KiB = 1024 B
        /// </summary>
        /// <param name="KibibyteValue">Количество кибибайт. Если меньше 0, будет выброшено исключение</param>
        /// <returns>Новый экземпляр структуры</returns>
        /// <exception cref="ArgumentOutOfRangeException">Количество кибибайт не может быть отрицательным</exception>
        public static ByteQuantity FromKibibytes(Decimal KibibyteValue)
        {
            if (KibibyteValue < 0) { throw new ArgumentOutOfRangeException("KibibyteValue", KibibyteValue, "Количество кибибайт не может быть отрицательным"); }
            if (KibibyteValue == 0) { return new ByteQuantity(0); }
            return new ByteQuantity(Convert.ToInt64(KibibyteValue * _x1Bin));
        }

        /// <summary>
        /// Возвращает новый экземпляр байтовой величины, содержащий указанное количество мебибайт (MiB), где 1 MiB = 1024^2 B
        /// </summary>
        /// <param name="MebibyteValue">Количество мебибайт. Если меньше 0, будет выброшено исключение</param>
        /// <returns>Новый экземпляр структуры</returns>
        /// <exception cref="ArgumentOutOfRangeException">Количество мебибайт не может быть отрицательным</exception>
        public static ByteQuantity FromMebibytes(Decimal MebibyteValue)
        {
            if (MebibyteValue < 0) { throw new ArgumentOutOfRangeException("MebibyteValue", MebibyteValue, "Количество мебибайт не может быть отрицательным"); }
            if (MebibyteValue == 0) { return new ByteQuantity(0); }
            return new ByteQuantity(Convert.ToInt64(MebibyteValue * _x2Bin));
        }

        /// <summary>
        /// Возвращает новый экземпляр байтовой величины, содержащий указанное количество гибибайт (GiB), где 1 GiB = 1024^3 B
        /// </summary>
        /// <param name="GibibyteValue">Количество гибибайт. Если меньше 0, будет выброшено исключение</param>
        /// <returns>Новый экземпляр структуры</returns>
        /// <exception cref="ArgumentOutOfRangeException">Количество гибибайт не может быть отрицательным</exception>
        public static ByteQuantity FromGibibytes(Decimal GibibyteValue)
        {
            if (GibibyteValue < 0) { throw new ArgumentOutOfRangeException("GibibyteValue", GibibyteValue, "Количество гибибайт не может быть отрицательным"); }
            if (GibibyteValue == 0) { return new ByteQuantity(0); }
            return new ByteQuantity(Convert.ToInt64(GibibyteValue * _x3Bin));
        }

        /// <summary>
        /// Возвращает новый экземпляр байтовой величины, содержащий указанное количество тебибайт (TiB), где 1 TiB = 1024^4 B
        /// </summary>
        /// <param name="TebibyteValue">Количество тебибайт. Если меньше 0, будет выброшено исключение</param>
        /// <returns>Новый экземпляр структуры</returns>
        /// <exception cref="ArgumentOutOfRangeException">Количество тебибайт не может быть отрицательным</exception>
        public static ByteQuantity FromTebibytes(Decimal TebibyteValue)
        {
            if (TebibyteValue < 0) { throw new ArgumentOutOfRangeException("TebibyteValue", TebibyteValue, "Количество тебибайт не может быть отрицательным"); }
            if (TebibyteValue == 0) { return new ByteQuantity(0); }
            return new ByteQuantity(Convert.ToInt64(TebibyteValue * _x4Bin));
        }

        /// <summary>
        /// Возвращает новый экземпляр байтовой величины, содержащий указанное количество пебибайт (PiB), где 1 PiB = 1024^5 B
        /// </summary>
        /// <param name="PebibyteValue">Количество пебибайт. Если меньше 0, будет выброшено исключение</param>
        /// <returns>Новый экземпляр структуры</returns>
        /// <exception cref="ArgumentOutOfRangeException">Количество пебибайт не может быть отрицательным</exception>
        public static ByteQuantity FromPebibytes(Decimal PebibyteValue)
        {
            if (PebibyteValue < 0) { throw new ArgumentOutOfRangeException("PebibyteValue", PebibyteValue, "Количество пебибайт не может быть отрицательным"); }
            if (PebibyteValue == 0) { return new ByteQuantity(0); }
            return new ByteQuantity(Convert.ToInt64(PebibyteValue * _x5Bin));
        }
        #endregion

        #region Constructors for decimal prefixes
        /// <summary>
        /// Возвращает новый экземпляр байтовой величины, содержащий указанное количество килобайт (KB), где 1 КB = 1000 B
        /// </summary>
        /// <param name="KilobyteValue">Количество килобайт. Если меньше 0, будет выброшено исключение</param>
        /// <returns>Новый экземпляр структуры</returns>
        /// <exception cref="ArgumentOutOfRangeException">Количество килобайт не может быть отрицательным</exception>
        public static ByteQuantity FromKilobytes(Decimal KilobyteValue)
        {
            if (KilobyteValue < 0) { throw new ArgumentOutOfRangeException("KilobyteValue", KilobyteValue, "Количество килобайт не может быть отрицательным"); }
            if (KilobyteValue == 0) { return new ByteQuantity(0); }
            return new ByteQuantity(Convert.ToInt64(KilobyteValue * _x1Dec));
        }

        /// <summary>
        /// Возвращает новый экземпляр байтовой величины, содержащий указанное количество мегабайт (MB), где 1 MB = 1000^2 B
        /// </summary>
        /// <param name="MegabyteValue">Количество мегабайт. Если меньше 0, будет выброшено исключение</param>
        /// <returns>Новый экземпляр структуры</returns>
        /// <exception cref="ArgumentOutOfRangeException">Количество мегабайт не может быть отрицательным</exception>
        public static ByteQuantity FromMegabytes(Decimal MegabyteValue)
        {
            if (MegabyteValue < 0) { throw new ArgumentOutOfRangeException("MegabyteValue", MegabyteValue, "Количество мегабайт не может быть отрицательным"); }
            if (MegabyteValue == 0) { return new ByteQuantity(0); }
            return new ByteQuantity(Convert.ToInt64(MegabyteValue * _x2Dec));
        }

        /// <summary>
        /// Возвращает новый экземпляр байтовой величины, содержащий указанное количество гигабайт (GB), где 1 GB = 1000^3 B
        /// </summary>
        /// <param name="GigabyteValue">Количество гигабайт. Если меньше 0, будет выброшено исключение</param>
        /// <returns>Новый экземпляр структуры</returns>
        /// <exception cref="ArgumentOutOfRangeException">Количество гигабайт не может быть отрицательным</exception>
        public static ByteQuantity FromGigabytes(Decimal GigabyteValue)
        {
            if (GigabyteValue < 0) { throw new ArgumentOutOfRangeException("GigabyteValue", GigabyteValue, "Количество гигабайт не может быть отрицательным"); }
            if (GigabyteValue == 0) { return new ByteQuantity(0); }
            return new ByteQuantity(Convert.ToInt64(GigabyteValue * _x3Dec));
        }

        /// <summary>
        /// Возвращает новый экземпляр байтовой величины, содержащий указанное количество терабайт (TB), где 1 TB = 1000^4 B
        /// </summary>
        /// <param name="TerabyteValue">Количество терабайт. Если меньше 0, будет выброшено исключение</param>
        /// <returns>Новый экземпляр структуры</returns>
        /// <exception cref="ArgumentOutOfRangeException">Количество терабайт не может быть отрицательным</exception>
        public static ByteQuantity FromTerabytes(Decimal TerabyteValue)
        {
            if (TerabyteValue < 0) { throw new ArgumentOutOfRangeException("TerabyteValue", TerabyteValue, "Количество терабайт не может быть отрицательным"); }
            if (TerabyteValue == 0) { return new ByteQuantity(0); }
            return new ByteQuantity(Convert.ToInt64(TerabyteValue * _x4Dec));
        }

        /// <summary>
        /// Возвращает новый экземпляр байтовой величины, содержащий указанное количество петабайт (PB), где 1 PB = 1000^5 B
        /// </summary>
        /// <param name="PetabyteValue">Количество петабайт. Если меньше 0, будет выброшено исключение</param>
        /// <returns>Новый экземпляр структуры</returns>
        /// <exception cref="ArgumentOutOfRangeException">Количество петабайт не может быть отрицательным</exception>
        public static ByteQuantity FromPetabytes(Decimal PetabyteValue)
        {
            if (PetabyteValue < 0) { throw new ArgumentOutOfRangeException("PetabyteValue", PetabyteValue, "Количество петабайт не может быть отрицательным"); }
            if (PetabyteValue == 0) { return new ByteQuantity(0); }
            return new ByteQuantity(Convert.ToInt64(PetabyteValue * _x5Dec));
        }
        #endregion
        #endregion
        #region IEquatable
        /// <summary>
        /// Сравнивает текущий экземпляр с указанным строго типизированным
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public Boolean Equals(ByteQuantity Other)
        {
            return this._value == Other._value;
        }

        /// <summary>
        /// Сравнивает текущий экземпляр с указанным неизвестного типа
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public override Boolean Equals(Object Other)
        {
            if (Object.ReferenceEquals(null, Other) == false && Other is ByteQuantity)
            {
                ByteQuantity other = (ByteQuantity)Other;
                return this.Equals(other);
            }
            return false;
        }

        /// <summary>
        /// Возвращает хэш-код данного экземпляра
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this._value.GetHashCode();
        }
        #endregion

        #region IComparable
        /// <summary>
        /// Сравнивает байтовую величину текущего экземпляра с указанным строго типизированным
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public Int32 CompareTo(ByteQuantity Other)
        {
            return this._value.CompareTo(Other._value);
        }

        /// <summary>
        /// Сравнивает байтовую величину текущего экземпляра с указанным неизвестного типа
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public Int32 CompareTo(Object Other)
        {
            if (Object.ReferenceEquals(null, Other) == false && Other is ByteQuantity)
            {
                ByteQuantity other = (ByteQuantity)Other;
                return this.CompareTo(other);
            }
            throw new ArgumentException("Значение аргумента Other не является типом ByteQuantity", "Other");
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Создаёт новый экземпляр, идентичный данному
        /// </summary>
        /// <returns></returns>
        public ByteQuantity Clone()
        {
            return new ByteQuantity(this._value);
        }
        /// <summary>
        /// Создаёт новый экземпляр, идентичный данному, и возвращает его в виде Object
        /// </summary>
        /// <returns></returns>
        Object ICloneable.Clone()
        {
            return this.Clone();
        }
        #endregion

        #region IConvertible

        private const string _CannotConvertValueTemplate =
            "Невозможно конвертировать значение в тип {0}, так как его максимальное значение {1} меньше текущего значения экземпляра {2}";

        /// <summary>
        /// Возвращает код данного типа
        /// </summary>
        /// <returns></returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }
        /// <summary>
        /// Пытается выполнить преобразование данного типа на указанный
        /// </summary>
        /// <param name="conversionType"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public Object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            }
            return Convert.ChangeType(this._value, conversionType);
        }

        /// <summary>
        /// Возвращает количество байт в виде числа с плавающей запятой двойной точности
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        public Double ToDouble(IFormatProvider Provider)
        {
            return Convert.ToDouble(this._value);
        }
        /// <summary>
        /// Возвращает количество байт в виде числа с плавающей запятой одинарной точности
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        public Single ToSingle(IFormatProvider Provider)
        {
            return Convert.ToSingle(this._value);
        }
        /// <summary>
        /// Возвращает количество байт в виде десятичного числа с плавающей запятой
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        public Decimal ToDecimal(IFormatProvider Provider)
        {
            return Convert.ToDecimal(this._value);
        }
        /// <summary>
        /// Возвращает количество байт в виде целого знакового 8-байтового числа
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        public Int64 ToInt64(IFormatProvider Provider)
        {
            return this._value;
        }
        /// <summary>
        /// Возвращает количество байт в виде целого беззнакового 8-байтового числа
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        public UInt64 ToUInt64(IFormatProvider Provider)
        {
            return Convert.ToUInt64(this._value);
        }
        /// <summary>
        /// Возвращает количество байт в виде целого знакового 4-байтового числа, если количество байт достаточно невелико, чтобы быть представленным этим числом
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        public Int32 ToInt32(IFormatProvider Provider)
        {
            if (this._value > Int32.MaxValue)
            {
                throw new OverflowException(String.Format(_CannotConvertValueTemplate, typeof(System.Int32).FullName, System.Int32.MaxValue, this._value));
            }
            return Convert.ToInt32(this._value);
        }
        /// <summary>
        /// Возвращает количество байт в виде целого беззнакового 4-байтового числа, если количество байт достаточно невелико, чтобы быть представленным этим числом
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        public UInt32 ToUInt32(IFormatProvider Provider)
        {
            if (this._value > UInt32.MaxValue)
            {
                throw new OverflowException(String.Format(_CannotConvertValueTemplate, typeof(System.UInt32).FullName, System.UInt32.MaxValue, this._value));
            }
            return Convert.ToUInt32(this._value);
        }
        /// <summary>
        /// Возвращает количество байт в виде целого знакового 2-байтового числа, если количество байт достаточно невелико, чтобы быть представленным этим числом
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        public Int16 ToInt16(IFormatProvider Provider)
        {
            if (this._value > Int16.MaxValue)
            {
                throw new OverflowException(String.Format(_CannotConvertValueTemplate, typeof(System.Int16).FullName, System.Int16.MaxValue, this._value));
            }
            return Convert.ToInt16(this._value);
        }
        /// <summary>
        /// Возвращает количество байт в виде целого беззнакового 2-байтового числа, если количество байт достаточно невелико, чтобы быть представленным этим числом
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        public UInt16 ToUInt16(IFormatProvider Provider)
        {
            if (this._value > UInt16.MaxValue)
            {
                throw new OverflowException(String.Format(_CannotConvertValueTemplate, typeof(System.UInt16).FullName, System.UInt16.MaxValue, this._value));
            }
            return Convert.ToUInt16(this._value);
        }
        /// <summary>
        /// Возвращает количество байт в виде целого знакового 1-байтового числа, если количество байт достаточно невелико, чтобы быть представленным этим числом
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        public SByte ToSByte(IFormatProvider Provider)
        {
            if (this._value > SByte.MaxValue)
            {
                throw new OverflowException(String.Format(_CannotConvertValueTemplate, typeof(System.SByte).FullName, System.SByte.MaxValue, this._value));
            }
            return Convert.ToSByte(this._value);
        }
        /// <summary>
        /// Возвращает количество байт в виде целого беззнакового 1-байтового числа, если количество байт достаточно невелико, чтобы быть представленным этим числом
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        public Byte ToByte(IFormatProvider Provider)
        {
            if (this._value > Byte.MaxValue)
            {
                throw new OverflowException(String.Format(_CannotConvertValueTemplate, typeof(System.Byte).FullName, System.Byte.MaxValue, this._value));
            }
            return Convert.ToByte(this._value);
        }
        /// <summary>
        /// Выбрасывает исключение InvalidCastException, так как количество байт не может быть выражено типом System.Boolean
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">Будет выброшено при вызове</exception>
        public Boolean ToBoolean(IFormatProvider Provider)
        {
            throw new InvalidCastException();
        }
        /// <summary>
        /// Выбрасывает исключение InvalidCastException, так как количество байт не может быть выражено типом System.Char
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">Будет выброшено при вызове</exception>
        public Char ToChar(IFormatProvider Provider)
        {
            throw new InvalidCastException();
        }
        /// <summary>
        /// Выбрасывает исключение InvalidCastException, так как количество байт не может быть выражено типом System.DateTime
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">Будет выброшено при вызове</exception>
        public DateTime ToDateTime(IFormatProvider Provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>
        /// Возвращает строковое представление количества байт с учётом указанной культуры, определяющей строковое представление числа. 
        /// В конце присутствует приставка 'Bytes'.
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        public String ToString(IFormatProvider Provider)
        {
            String output = this._value.ToString(Provider) + " Bytes";
            return output;
        }
        #endregion

        /// <summary>
        /// Возвращает строковое представление количества байт; разряды разделены пробелами, в конце присутствует приставка 'Bytes'.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String output = this._value.ToString("N0") + " Bytes";
            return output;
        }

        #region Operators overloading
        /// <summary>
        /// Сравнивает два экземпляра на равенство
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns></returns>
        public static Boolean operator ==(ByteQuantity Left, ByteQuantity Right)
        {
            return Left.Equals(Right);
        }
        /// <summary>
        /// Сравнивает два экземпляра на неравенство
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns></returns>
        public static Boolean operator !=(ByteQuantity Left, ByteQuantity Right)
        {
            return !Left.Equals(Right);
        }

        /// <summary>
        /// Определяет, строго больше ли байтовая величина первого экземпляра по сравнению с величиной второго
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns></returns>
        public static Boolean operator >(ByteQuantity Left, ByteQuantity Right)
        {
            return Left._value > Right._value;
        }
        /// <summary>
        /// Определяет, строго меньше ли байтовая величина первого экземпляра по сравнению с величиной второго
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns></returns>
        public static Boolean operator <(ByteQuantity Left, ByteQuantity Right)
        {
            return Left._value < Right._value;
        }

        /// <summary>
        /// Определяет, больше или равна байтовая величина первого экземпляра по сравнению с величиной второго
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns></returns>
        public static Boolean operator >=(ByteQuantity Left, ByteQuantity Right)
        {
            return Left._value >= Right._value;
        }
        /// <summary>
        /// Определяет, меньше или равна байтовая величина первого экземпляра по сравнению с величиной второго
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns></returns>
        public static Boolean operator <=(ByteQuantity Left, ByteQuantity Right)
        {
            return Left._value <= Right._value;
        }

        /// <summary>
        /// Возвращает новую байтовую величину, содержащую результат сложения двух указанных величин
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static ByteQuantity operator +(ByteQuantity First, ByteQuantity Second)
        {
            return new ByteQuantity(First._value + Second._value);
        }

        /// <summary>
        /// Возвращает новую байтовую величину, являющуюся разностью между двумя указанными величинами. Операция является коммуникативной.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static ByteQuantity operator -(ByteQuantity First, ByteQuantity Second)
        {
            if (First._value == Second._value) { return new ByteQuantity(); }
            Int64 bigger = Math.Max(First._value, Second._value);
            Int64 lesser = Math.Min(First._value, Second._value);
            return new ByteQuantity(bigger - lesser);
        }
        #endregion
    }
}

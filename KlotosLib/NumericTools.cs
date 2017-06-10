using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KlotosLib
{
    /// <summary>
    /// Содержит методы расширения по работе с числами
    /// </summary>
    public static class NumericTools
    {
        /// <summary>
        /// Выполняет округление числа с плавающей запятой двойной точности с указанной точностью
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Digits">Точность округления, выраженная в количестве знаков после запятой, которые должны остаться. 
        /// Если больше фактического количества знаков, число будет возвращено без изменений.</param>
        /// <returns></returns>
        public static Double Rounding(this Double Value, Byte Digits)
        {
            Double scale = Math.Pow(10.0, Digits);
            Double round = Math.Floor(Math.Abs(Value) * scale + 0.5);
            return (Math.Sign(Value) * round / scale);
        }

        /// <summary>
        /// Выполняет округление числа с плавающей запятой одинарной точности с указанной точностью
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Digits">Точность округления, выраженная в количестве знаков после запятой, которые должны остаться. 
        /// Если больше фактического количества знаков, число будет возвращено без изменений.</param>
        /// <returns></returns>
        public static Single Rounding(this Single Value, Byte Digits)
        {
            Double scale = Math.Pow(10.0, Digits);
            Double round = Math.Floor(Math.Abs(Value) * scale + 0.5);
            return Convert.ToSingle(Math.Sign(Value) * round / scale);
        }

        /// <summary>
        /// Выполняет округление числа с фиксированной запятой с указанной точностью
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Digits">Точность округления, выраженная в количестве знаков после запятой, которые должны остаться. 
        /// Если больше фактического количества знаков, число будет возвращено без изменений.</param>
        /// <returns></returns>
        public static Decimal Rounding(this Decimal Value, Byte Digits)
        {
            if (Digits > 28) { Digits = 28; }
            Decimal scale = Convert.ToDecimal(Math.Pow(10.0, Digits));
            Decimal round = Math.Floor(Math.Abs(Value) * scale + 0.5M);
            return Math.Sign(Value) * round / scale;
        }

        /// <summary>
        /// Возвращает разницу между модулями указанных двух чисел
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Last"></param>
        /// <returns></returns>
        public static UInt32 GetDifferenceAbs(Int32 First, Int32 Last)
        {
            if (First == Last) { return 0; }

            UInt32 module_first = Convert.ToUInt32(Math.Abs(First));
            UInt32 module_last = Convert.ToUInt32(Math.Abs(Last));

            UInt32 bigger = Math.Max(module_first, module_last);
            UInt32 lesser = Math.Min(module_first, module_last);

            return (bigger - lesser);
        }

        /// <summary>
        /// Возвращает дробную часть числа с плавающей запятой одинарной точности без десятичного разделителя в виде строки. 
        /// В зависимости от значения параметра дробная часть возвращается полностью или с указанным количеством цифр, начиная со старших разрядов.
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="FractionDigits">Количество цифр в дробной части, начиная со страших разрядов, которое требуется возвратить. 
        /// Если 0 — будут возвращены все цифры. Производится отсечение, но не округление.</param>
        /// <returns></returns>
        public static String GetFraction(this Single Number, Byte FractionDigits)
        {
            String output = Number.ToString(CultureInfo.InvariantCulture).Split(new Char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();
            if (FractionDigits == 0)
            {
                return output;
            }
            else
            {
                if (FractionDigits > output.Length) { FractionDigits = Convert.ToByte(output.Length); }
                return output.Substring(0, FractionDigits);
            }
        }

        /// <summary>
        /// Возвращает дробную часть числа с плавающей запятой двойной точности без десятичного разделителя в виде строки. 
        /// В зависимости от значения параметра дробная часть возвращается полностью или с указанным количеством цифр, начиная со старших разрядов.
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="FractionDigits">Количество цифр в дробной части, начиная со страших разрядов, которое требуется возвратить. 
        /// Если 0 - будут возвращены все цифры. Производится отсечение, но не округление.</param>
        /// <returns></returns>
        public static String GetFraction(this Double Number, Byte FractionDigits)
        {
            String output = Number.ToString(CultureInfo.InvariantCulture).Split(new Char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();
            if (FractionDigits == 0)
            {
                return output;
            }
            else
            {
                if (FractionDigits > output.Length) { FractionDigits = Convert.ToByte(output.Length); }
                return output.Substring(0, FractionDigits);
            }
        }

        /// <summary>
        /// Возвращает дробную часть числа с фиксированной запятой без десятичного разделителя в виде строки. 
        /// В зависимости от значения параметра дробная часть возвращается полностью или с указанным количеством цифр, начиная со старших разрядов.
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="FractionDigits">Количество цифр в дробной части, начиная со страших разрядов, которое требуется возвратить. 
        /// Если 0 - будут возвращены все цифры. Производится отсечение, но не округление.</param>
        /// <returns></returns>
        public static String GetFraction(this Decimal Number, Byte FractionDigits)
        {
            String output = Number.ToString(CultureInfo.InvariantCulture).Split(new Char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();
            if (FractionDigits == 0)
            {
                return output;
            }
            else
            {
                if (FractionDigits > output.Length) { FractionDigits = Convert.ToByte(output.Length); }
                return output.Substring(0, FractionDigits);
            }
        }
        
        /// <summary>
        /// Возвращает сумму чисел от 1 и вплоть до указанного
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static Int32 Summarial(Int32 Input)
        {
            if (Input < 0) { throw new ArgumentOutOfRangeException("Input", Input, "Число не может быть отрицательным"); }
            if (Input < 2) { return Input; }
            return Input * (Input + 1) / 2;
        }

        /// <summary>
        /// Возвращает факториал числа
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static UInt32 Factorial(Int32 Input)
        {
            if (Input < 0) { throw new ArgumentOutOfRangeException("Input", Input, "Число не может быть отрицательным"); }
            if (Input < 3) { return (UInt32)Input; }
            UInt32 res = 2;
            for (UInt32 multiplier = 3; multiplier <= Input; multiplier++)
            {
                res = res * multiplier;
            }
            return res;
        }

        #region Combine and Split
        /// <summary>
        /// Возвращает два новых 1-байтовых целых беззнаковых числа, разбивая пополам одно указанное 2-байтовое типа UInt16
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static KeyValuePair<Byte, Byte> Split(UInt16 Number)
        {
            unchecked
            {
                Byte first = (Byte)(Number >> 8);
                Byte second = (Byte)((Number << 8) >> 8);
                return new KeyValuePair<Byte, Byte>(first, second);
            }
        }

        /// <summary>
        /// Возвращает через выводные параметры 4 новых числа типа Byte, разбивая указанное число <paramref name="Number"/> на 4 секции по по 8 бит
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <param name="Third"></param>
        /// <param name="Fourth"></param>
        public static void Split(UInt32 Number, out Byte First, out Byte Second, out Byte Third, out Byte Fourth)
        {
            unchecked
            {
                First = (Byte)(Number >> 24);
                Second = (Byte) ((Number << 8) >> 24);
                Third = (Byte)((Number << 16) >> 24);
                Fourth = (Byte)((Number << 24) >> 24);
            }
        }

        /// <summary>
        /// Возвращает одно новое число типа Int64, объединяя два указанных типа Int32
        /// </summary>
        /// <remarks>http://stackoverflow.com/a/827267</remarks>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Int64 Combine(Int32 First, Int32 Second)
        {
            unchecked
            {
                Int64 res = First;
                res = (res << 32);
                res = res | (UInt32)Second;
                return res;
            }
        }

        /// <summary>
        /// Возвращает два новых 4-байтовых целых знаковых числа, разбивая поплам одно указанное 8-байтовое типа Int64
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static KeyValuePair<Int32, Int32> Split(Int64 Number)
        {
            unchecked
            {
                Int32 first = (Int32)(Number >> 32);
                Int32 second = (Int32) ((Number << 32) >> 32);
                return new KeyValuePair<int, int>(first, second);
            }
        }

        /// <summary>
        /// Возвращает одно новое число типа UInt64, объединяя два указанных типа UInt32
        /// </summary>
        /// <remarks>http://stackoverflow.com/a/827267</remarks>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static UInt64 Combine(UInt32 First, UInt32 Second)
        {
            {
                UInt64 res = First;
                res = (res << 32);
                res = res | (UInt64)Second;
                return res;
            }
        }

        /// <summary>
        /// Возвращает два новых 4-байтовых целых беззнаковых числа, разбивая поплам одно указанное 8-байтовое типа UInt64
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static KeyValuePair<UInt32, UInt32> Split(UInt64 Number)
        {
            unchecked
            {
                UInt32 first = (UInt32)(Number >> 32);
                UInt32 second = (UInt32)((Number << 32) >> 32);
                return new KeyValuePair<UInt32, UInt32>(first, second);
            }
        }
        #endregion

        /// <summary>
        /// Сравнивает два двоичных числа с плавающей запятой двойной точности с учётом указанного порогового значения
        /// </summary>
        /// <param name="Threshold">Пороговое значение для сравнения. Если отрицательное - будет взят модуль.</param>
        /// <param name="First">Первое сравниваемое число</param>
        /// <param name="Second">Второе сравниваемое число</param>
        /// <returns></returns>
        public static Boolean AreEqual(Double Threshold, Double First, Double Second)
        {
            Double difference = Math.Abs(First - Second);
            if (difference > Math.Abs(Threshold)) {return false;}
            return true;
        }

        private const Double _thresholdD = 0.0000001;

        /// <summary>
        /// Сравнивает два двоичных числа с плавающей запятой двойной точности, применяя внутреннее пороговое значение
        /// </summary>
        /// <param name="First">Первое сравниваемое число</param>
        /// <param name="Second">Второе сравниваемое число</param>
        /// <returns></returns>
        public static Boolean AreEqual(Double First, Double Second)
        {
            Double difference = Math.Abs(First - Second);
            if (difference > Math.Abs(_thresholdD)) { return false; }
            return true;
        }

        /// <summary>
        /// Сравнивает два двоичных числа с плавающей запятой одинарной точности с учётом указанного порогового значения
        /// </summary>
        /// <param name="Threshold">Пороговое значение для сравнения. Если отрицательное - будет взят модуль.</param>
        /// <param name="First">Первое сравниваемое число</param>
        /// <param name="Second">Второе сравниваемое число</param>
        /// <returns></returns>
        public static Boolean AreEqual(Single Threshold, Single First, Single Second)
        {
            Single difference = Math.Abs(First - Second);
            if (difference > Math.Abs(Threshold)) { return false; }
            return true;
        }

        private const Single _thresholdF = 0.0000001f;

        /// <summary>
        /// Сравнивает два двоичных числа с плавающей запятой одинарной точности, применяя внутреннее пороговое значение
        /// </summary>
        /// <param name="First">Первое сравниваемое число</param>
        /// <param name="Second">Второе сравниваемое число</param>
        /// <returns></returns>
        public static Boolean AreEqual(Single First, Single Second)
        {
            Single difference = Math.Abs(First - Second);
            if (difference > Math.Abs(_thresholdF)) { return false; }
            return true;
        }

        /// <summary>
        /// Определяет, является ли указанное двоичное число с плавающей запятой двойной точности нулем
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean IsZero(Double value)
        {
            return Math.Abs(value) < _thresholdD;
        }

        /// <summary>
        /// Определяет, является ли указанное одинарной число с плавающей запятой двойной точности нулем
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean IsZero(Single value)
        {
            return Math.Abs(value) < _thresholdF;
        }

        /// <summary>
        /// Выполняет кластеризацию указанного набора чисел (в одном измерении) с указанным пороговым значением. Самостоятельно определяет количество кластеров.
        /// </summary>
        /// <param name="Threshold">Пороговое значение</param>
        /// <param name="Data">Набор чисел. Если NULL или пустой, будет выброшено исключение.</param>
        /// <returns></returns>
        public static List<List<Double>> Clusterize(Double Threshold, IEnumerable<Double> Data)
        {
            Data.ThrowIfNullOrEmpty("Набор чисел является NULL", "Набор чисел пуст", "Data");
            if (Data.HasSingle() == true) { return new List<List<Double>>(1) { new List<Double>(1) { Data.Single() } }; }
            
            List<Double> sorted_data = Data.ToList();
            sorted_data.Sort();
            //List<Double> sorted_data = (from Double x in Data orderby x ascending select x).ToList();
            List<List<Double>> output = new List<List<Double>>();
            List<Double> first_cluster = new List<Double>() { sorted_data[0] };
            output.Add(first_cluster);
            for (Int32 i = 1; i < sorted_data.Count; i++)
            {
                Double current = sorted_data[i];
                if (NumericTools.AreEqual(Threshold, output.ItemFromEnd(0).Last(), current) == true)
                {
                    output.ItemFromEnd(0).Add(current);
                }
                else
                {
                    output.Add(new List<Double>() { current });
                }
            }
            return output.OrderByDescending(item => item.Count).ToList();
        }
    }
}

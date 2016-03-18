﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace KlotosLib.StringTools
{
    /// <summary>
    /// Содержит статические методы, выполняющие парсинг входной строки в определённые формы
    /// </summary>
    public static class ParsingHelpers
    {
        /// <summary>
        /// Разбивает входную строку по указанному разделителю и возвращает список чисел указанного типа, 
        /// извлечённых с получившихся подстрок с применением указанных параметров парсинга
        /// </summary>
        /// <param name="Input">Входная строка, из которой требуется извлечь числа</param>
        /// <param name="Divider">Разделитель - строка, по которой выполняется разбиение входной строки. Не может быть NULL или пустой</param>
        /// <param name="Style">Правила парсинга числовой строки</param>
        /// <param name="CultureProvider">Формат языка и региональных параметров, с применением которого будет парситься строка. Если NULL, будет применён инвариантный формат.</param>
        /// <returns></returns>
        public static List<TNumber> ParseInputStringToNumbers<TNumber>(String Input, String Divider, NumberStyles Style, IFormatProvider CultureProvider)
            where TNumber : struct, IEquatable<TNumber>, IComparable<TNumber>, IComparable, IFormattable, IConvertible
        {
            if (Input.HasVisibleChars() == false) { throw new ArgumentException("Входная строка не может быть NULL, пустой или не содержать ни одного видимого символа", "Input"); }
            if (String.IsNullOrEmpty(Divider) == true) { throw new ArgumentException("Разделитель не может быть NULL или пустым", "Divider"); }
            if (CultureProvider == null) { CultureProvider = CultureInfo.InvariantCulture; }

            String[] divided = Input.Split(new string[] { Divider }, StringSplitOptions.RemoveEmptyEntries);
            List<TNumber> output = new List<TNumber>(divided.Length);
            for (Int32 i = 0; i < divided.Length; i++)
            {
                String number = divided[i];
                Nullable<TNumber> one = number.TryParseNumber<TNumber>(Style, CultureProvider);
                if (one == null)
                {
                    throw new InvalidOperationException(String.Format("Невозможно успешно распарсить подстроку '{0}' на позиции {1} как число типа {2}",
                        number, i + 1, typeof(TNumber).FullName));
                }
                output.Add(one.Value);
            }
            return output;
        }
    }//end of class ParsingHelpers
}
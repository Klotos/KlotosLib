using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KlotosLib.StringTools
{
    /// <summary>
    /// Содержит статические методы, генерирующие последовательности символов по заданным критериям
    /// </summary>
    public static class CharSeqGen
    {
        /// <summary>
        /// Определяет категорию литер
        /// </summary>
        public enum LettersType : byte
        {
            /// <summary>
            /// Все литеры
            /// </summary>
            AllCase,

            /// <summary>
            /// Только строчные литеры
            /// </summary>
            OnlyLowerCase,

            /// <summary>
            /// Только заглавные литеры
            /// </summary>
            OnlyCapitalCase
        }

        /// <summary>
        /// Возвращает массив со всеми символами, присутствующими в указанной строке. Символы встречаются в массиве один раз и отсортированы по возрастанию.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static Char[] FromString(String Input)
        {
            HashSet<Char> res = new HashSet<char>(Input);
            return res.OrderBy(ch => ch).ToArray();
        }

        /// <summary>
        /// Возвращает массив со всеми арабскими цифрами
        /// </summary>
        /// <returns></returns>
        public static Char[] DigitsOnly()
        {
            return new Char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        }

        /// <summary>
        /// Возвращает массив символов, представляющих все литеры латинского алфавита. Символы в массиве отсортированы и встречаются только один раз.
        /// </summary>
        /// <param name="LettersType">Категория литер, которая должна присутствовать в выводном массиве.</param>
        /// <returns></returns>
        public static Char[] LatinLetters(LettersType LettersType)
        {
            Char[] output;
            switch (LettersType)
            {
                case LettersType.AllCase:
                    output = new Char[26 * 2];
                    for (Int32 i = 0x41, j = 0; i <= 0x5A; i++, j++)
                    {
                        output[j] = (Char)i;
                    }
                    for (Int32 i = 0x61, j = 26; i <= 0x7A; i++, j++)
                    {
                        output[j] = (Char)i;
                    }
                    break;
                case LettersType.OnlyCapitalCase:
                    output = new Char[26];
                    for (Int32 i = 0x41, j = 0; i <= 0x5A; i++, j++)
                    {
                        output[j] = (Char)i;
                    }
                    break;
                case LettersType.OnlyLowerCase:
                    output = new Char[26];
                    for (Int32 i = 0x61, j = 0; i <= 0x7A; i++, j++)
                    {
                        output[j] = (Char)i;
                    }
                    break;
                default:
                    throw new InvalidEnumArgumentException("LettersType", (Int32)LettersType, LettersType.GetType());
            }
            return output;
        }

        /// <summary>
        /// Возвращает массив символов, представляющих арабские цифры и все литеры латинского алфавита. Символы в массиве отсортированы и встречаются только один раз.
        /// </summary>
        /// <param name="LettersType">Категория литер, которая должна присутствовать в выводном массиве.</param>
        /// <returns></returns>
        public static Char[] DigitsAndLatinLetters(LettersType LettersType)
        {
            Char[] temp1 = CharSeqGen.DigitsOnly();
            Char[] temp2 = CharSeqGen.LatinLetters(LettersType);
            Char[] output = new Char[temp1.Length + temp2.Length];
            Array.ConstrainedCopy(temp1, 0, output, 0, temp1.Length);
            Array.ConstrainedCopy(temp2, 0, output, temp1.Length, temp2.Length);
            return output;
        }

        /// <summary>
        /// Возвращает массив всех символов, чьи кодовые точки находятся в указанном диапазоне чисел
        /// </summary>
        /// <param name="Start">Кодовая точка начала диапазона, включительно</param>
        /// <param name="End">Кодовая точка конца диапазона, включительный</param>
        /// <returns></returns>
        public static Char[] FromRange(UInt16 Start, UInt16 End)
        {
            UInt16 min = Math.Min(Start, End);
            UInt16 max = Math.Max(Start, End);
            Char[] output = new Char[max - min + 1];
            for (UInt16 i = min, j = 0; i <= max; i++, j++)
            {
                output[j] = (Char)i;
            }
            return output;
        }

        /// <summary>
        /// Возвращает массив всех символов, чьи кодовые точки находятся в указанном диапазоне символов
        /// </summary>
        /// <param name="Start">Символ начала диапазона, включительный</param>
        /// <param name="End">Символ конца диапазона, включительный</param>
        /// <returns></returns>
        public static Char[] FromRange(Char Start, Char End)
        {
            return CharSeqGen.FromRange((UInt16)Start, (UInt16)End);
        }
    }//end of class CharSeqGen
}

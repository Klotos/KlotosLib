using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace KlotosLib.StringTools
{
    /// <summary>
    /// Содержит статические методы, определяющие соответствие или несоответствие входной строки определённым критериям
    /// </summary>
    public static class ValidatorHelpers
    {
        /// <summary>
        /// Если метод возвращает "true" - ни одна из входных строк не является NULL, пустой или состоящей из одних пробелов. Иначе "false".
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static Boolean AllStringsNotNullEmptyWS(params String[] Input)
        {
            if (Input.IsNullOrEmpty() == true) { return false; }
            foreach (String str in Input)
            {
                if (str.IsStringNullEmptyWhiteSpace() == true) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Определяет наличие видимых символов в наборе строк. Возвращает трит: если true - все до одной строки имеют видимые символы; если false - ни одна строка не имеет видимых символов; 
        /// если null - часть строк имеет видимые символы, а часть - нет. Для валидации строк используется метод HasVisibleChars.
        /// </summary>
        /// <param name="Input">Набор строк. Если NULL или пустой - будет выброшено исключение.</param>
        /// <returns></returns>
        public static Nullable<Boolean> AllStringsHaveVisibleChars(params String[] Input)
        {
            if (Input.IsNullOrEmpty() == true) { throw new ArgumentException("Массив строк не может быть NULL или пустым", "Input"); }
            if (Input.HasSingle() == true) { return Input.Single().HasVisibleChars(); }

            UInt32 visible_count = 0;
            UInt32 invisible_count = 0;
            foreach (String str in Input)
            {
                if (str.HasVisibleChars() == true)
                {
                    visible_count++;
                }
                else
                {
                    invisible_count++;
                }
            }
            Int32 input_count = Input.Length;
            if (visible_count == input_count) { return true; }
            if (invisible_count == input_count) { return false; }

            //if (visible_count > 0 && invisible_count > 0)
            return null;
        }

        /// <summary>
        /// Проверяет, может ли указанная строка быть успешно преобразована в тип GUID, являющийся 128-битным числом.
        /// </summary>
        /// <remarks>Осторожно: регекспы</remarks>
        /// <param name="Input">Входная строка, которую надо проверить на совместимость с GUID</param>
        /// <returns></returns>
        public static Boolean IsValidGuid(String Input)
        {
            if (Input.IsStringNullEmptyWhiteSpace() == true) { return false; }
            return Regex.Match(Input,
                               @"^ (?:\{)? (?<GUID> [0-9a-f]{8} \- [0-9a-f]{4} \- [0-9a-f]{4} \- [0-9a-f]{4} \- [0-9a-f]{12} ) (?:\})? $",
                               RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled).
                Success;
            //.NET 4 below

            //Guid temp;
            //return Guid.TryParse(Input, out temp);
        }

        /// <summary>
        /// Проверяет, является ли данная строка корректным адресом электронной почты
        /// </summary>
        /// <remarks>Осторожно: регекспы</remarks>
        /// <param name="Input"></param>
        /// <returns>Если true - является</returns>
        public static bool IsValidEmail(String Input)
        {
            if (Input.IsStringNullEmptyWhiteSpace() == true) { return false; }
            return Regex.IsMatch(Input,
                                 @"^[-a-z0-9!#$%&'*+/=?^_`{|}~]+(?:\.[-a-z0-9!#$%&'*+/=?^_`{|}~]+)*@(?:[a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*" +
            "(?:aero|arpa|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|[a-z][a-z])$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
}

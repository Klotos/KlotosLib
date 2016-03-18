using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KlotosLib.StringTools
{
    /// <summary>
    /// Содержит статические методы, расширяющие функционал String.Contains и возвращающие булевое значение
    /// </summary>
    public static class ContainsHelpers
    {
        /// <summary>
        /// "Коллекция с искомыми символами является NULL или не содержит ни одного элемента"
        /// </summary>
        private const String _ExceptionString_NullOrEmptyCharArray = "Коллекция с искомыми символами является NULL или не содержит ни одного элемента";

        /// <summary>
        /// "Коллекция с искомыми подстроками является NULL или не содержит ни одного элемента"
        /// </summary>
        private const String _ExceptionString_NullOrEmptyStrArray = "Коллекция с искомыми подстроками является NULL или не содержит ни одного элемента";

        /// <summary>
        /// "Входная строка является NULL или пустой"
        /// </summary>
        private const String _ExceptionString_NullOrEmptyString = "Входная строка является NULL или пустой";

        /// <summary>
        /// Определяет, содержится ли во входной строке хотя бы одна из указанных подстрок. Если не найдена ни одна, возвращает "false".
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Seek"></param>
        /// <returns></returns>
        public static Boolean ContainsAtLeastOneOf(String Input, params String[] Seek)
        {
            if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyString, "Input"); }
            if (Seek.IsNullOrEmpty<String>() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyStrArray, "Seek"); }
            foreach (String one_seek in Seek)
            {
                if (Input.Contains(one_seek) == true)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Определяет, содержится ли во входной строке хотя бы одна из указанных подстрок, используя для их поиска указанные опции сравнения. 
        /// Если не найдена ни одна подстрока, возвращает "false".
        /// </summary>
        /// <param name="Input">Исходная строка, в которой ищутся искомые подстроки. Если NULL или пустая, выбрасывается исключение.</param>
        /// <param name="ComparisonOptions">Опции сравнения строк между собой</param>
        /// <param name="Seek">Набор искомых подстрок. Если NULL или пуст, выбрасывается исключение.</param>
        /// <returns></returns>
        public static Boolean ContainsAtLeastOneOf(String Input, StringComparison ComparisonOptions, params String[] Seek)
        {
            if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyString, "Input"); }
            if (Seek.IsNullOrEmpty<String>() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyStrArray, "Seek"); }

            foreach (String one_seek in Seek)
            {
                if (Input.IndexOf(one_seek, ComparisonOptions) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Определяет, содержится ли во входной строке хотя бы один из указанных символов. Если не найден ни один, возвращает 'false'. Если найден хотя бы один, возвращает 'true'.
        /// </summary>
        /// <param name="Input">Входная строка. Если NULL или пустая - генерирует исключение.</param>
        /// <param name="Seek">Массив искомых символов. Если NULL или пустой - генерируется исключение.</param>
        /// <returns></returns>
        public static Boolean ContainsAtLeastOneOf(String Input, params Char[] Seek)
        {
            if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyString, "Input"); }
            if (Seek.IsNullOrEmpty<Char>() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyCharArray, "Seek"); }
            foreach (char c in Seek)
            {
                if (Input.Contains(c) == true)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Определяет, содержится ли в искомой строке одна и только одна из указанных подстрок. 
        /// Если не найдена ни одна или больше одной, возвращает "false".
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Seek"></param>
        /// <returns></returns>
        public static Boolean ContainsOnlyOneOf(String Input, params String[] Seek)
        {
            if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyString, "Input"); }
            if (Seek.IsNullOrEmpty<String>() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyStrArray, "Seek"); }
            UInt32 found = 0;
            foreach (String one_seek in Seek)
            {
                if (Input.Contains(one_seek) == true) { found = found + 1; }
                if (found > 1) { return false; }
            }
            if (found == 0 || found > 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Определяет, содержится ли в искомой строке один и только один из указанных символов. 
        /// Если не найден ни один символ или найдено больше одного символа, возвращает 'false'.
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Seek"></param>
        /// <returns></returns>
        public static Boolean ContainsOnlyOneOf(String Input, params Char[] Seek)
        {
            if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyString, "Input"); }
            if (Seek.IsNullOrEmpty<Char>() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyCharArray, "Seek"); }
            UInt32 found = 0;
            foreach (char one_symbol in Seek)
            {
                if (Input.Contains(one_symbol) == true)
                {
                    found++;
                }
                if (found > 1) { return false; }
            }
            if (found == 0 || found > 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Определяет, не содержится ли в искомой строке хотя бы одна из указанных подстрок. 
        /// Если найдена хотя бы одна подстрока, возвращает "false".
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Seek"></param>
        /// <returns></returns>
        public static Boolean ContainsNoOneOf(String Input, params String[] Seek)
        {
            if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyString, "Input"); }
            if (Seek.IsNullOrEmpty<String>() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyStrArray, "Seek"); }
            foreach (String one_seek in Seek)
            {
                if (Input.Contains(one_seek) == true) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Определяет, не содержится ли в искомой строке хотя бы один из указанных символов. 
        /// Если найден хотя бы один символ, возвращает "false".
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Seek"></param>
        /// <returns></returns>
        public static Boolean ContainsNoOneOf(String Input, params Char[] Seek)
        {
            if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyString, "Input"); }
            if (Seek.IsNullOrEmpty<Char>() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyCharArray, "Seek"); }
            Char[] input_array = Input.ToCharArray();
            foreach (char c in input_array)
            {
                if (c.IsIn(Seek) == true) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Определяет, содержатся ли в искомой строке все из указанных подстрок. 
        /// Если хотя бы одной не содержится, возвращает "false".
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Seek"></param>
        /// <param name="CompareOption">Опции сравнения строк между собой</param>
        /// <returns></returns>
        public static Boolean ContainsAllOf(String Input, StringComparison CompareOption, params String[] Seek)
        {
            if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyString, "Input"); }
            if (Seek.IsNullOrEmpty<String>() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyStrArray, "Seek"); }
            foreach (String one_seek in Seek)
            {
                if (Input.Contains(one_seek, CompareOption) == false) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Определяет, содержатся ли в искомой строке все из указанных символов. 
        /// Если хотя бы одного не содержится, возвращает "false".
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Seek"></param>
        /// <returns></returns>
        public static Boolean ContainsAllOf(String Input, params Char[] Seek)
        {
            if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyString, "Input"); }
            if (Seek.IsNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyCharArray, "Seek"); }
            foreach (Char one_seek in Seek)
            {
                if (Input.Contains(one_seek) == false) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Определяет, содержатся ли в искомой строке все из указаных символов и только они. 
        /// Если хотя бы одного не содержится, или хотя бы один символ искомой строки не из указанных, возвращает "false".
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Seek"></param>
        /// <returns></returns>
        public static Boolean ContainsAllAndOnlyOf(String Input, params Char[] Seek)
        {
            if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyString, "Input"); }
            if (Seek.IsNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyCharArray, "Seek"); }
            Char[] input_array = Input.ToCharArray();
            if (ContainsAllOf(Input, Seek) == false) { return false; }
            foreach (Char symbol in input_array)
            {
                if (symbol.IsIn(Seek) == false) { return false; }//попался символ, которого нет в Seek
            }
            return true;
        }

        /// <summary>
        /// Определяет, содержатся ли в искомой строке все до одного из указаных обязательных символов и принадлежат ли все остальные символы к указанным разрешенным. 
        /// Если в искомой строке нет хотя бы одного обязательного символа, возвращается 'false'. 
        /// Если хотя бы один символ искомой строки, не являющийся обязательным, отсутствует при этом в массиве разрешенных, возвращается "false". 
        /// При этом обязательное наличие в искомой строке всех или даже одного символа из разрешенного массива не требуется.  
        /// </summary>
        /// <param name="Input">Входная искомая строка, в которой проходит поиск</param>
        /// <param name="RequiredSymbols">Массив обязательных символов, все из которых должны присутствовать в искомой строке</param>
        /// <param name="AcceptedSymbols">Массив допустимых символов, которые не являются обязательными для присутствия, наличие которых допустимо в искомой строке.</param>
        /// <returns></returns>
        public static Boolean ContainsAllRequiredAndOnlyAccepted(String Input, Char[] RequiredSymbols, Char[] AcceptedSymbols)
        {
            if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyString, "Input"); }
            if (RequiredSymbols.IsNullOrEmpty() == true) { throw new ArgumentException("Массив обязательных символов не может быть NULL или пустым", "RequiredSymbols"); }

            if (ContainsHelpers.ContainsAllOf(Input, RequiredSymbols) == false) { return false; }

            Char[] input_array = Input.ToCharArray();
            foreach (char c in input_array)
            {
                if (c.IsIn(RequiredSymbols) == false && c.IsIn(AcceptedSymbols) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Определяет, содержатся ли в строке только разрешенные символы, указанные в параметре. Если хотя бы один символ не является разрешенным, возвращается 'false'. 
        /// При этом наличие во входной строке всех разрешенных символов не обязательно.
        /// </summary>
        /// <param name="Input">Входная строка. Если NULL или пустая - генерируется исключение.</param>
        /// <param name="AllowedSymbols">Массив разрешенных символов. Если NULL или пустой - генерируется исключение.</param>
        /// <returns></returns>
        public static Boolean ContainsOnlyAllowed(String Input, Char[] AllowedSymbols)
        {
            if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException(ContainsHelpers._ExceptionString_NullOrEmptyString, "Input"); }
            if (AllowedSymbols.IsNullOrEmpty() == true) { throw new ArgumentException("Массив разрешенных символов не может быть NULL или пустой", "AllowedSymbols"); }

            Char[] input_array = Input.ToCharArray();
            foreach (char c in input_array)
            {
                if (c.IsIn(AllowedSymbols) == false)
                {
                    return false;
                }
            }
            return true;
        }
    }//end of class ContainsHelpers
}

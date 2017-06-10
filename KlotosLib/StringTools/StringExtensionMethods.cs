using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KlotosLib.StringTools
{
    /// <summary>
    /// Содержит методы расширения для строкового типа
    /// </summary>
    public static class StringExtensionMethods
    {
        /// <summary>
        /// Возвращает подстроку, которая начинается с указанного начального индекса и кончается указанным конечным индексом. 
        /// Булевые параметры позволяют указывать, включительными или исключительными являются начальный и конечный индексы, 
        /// а также считывать ли строку до конца, если конечный индекс выходить за её пределы.
        /// </summary>
        /// <param name="input">Входная строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="startIndex">Начальный индекс (позиция), отсчёт ведётся начиная с 0. 
        /// Его включительность или исключительность определяется значением параметра <paramref name="includeStart"/>. 
        /// Если меньше 0 - выбрасывается исключение.</param>
        /// <param name="endIndex">Конечный индекс (позиция), отсчёт ведётся начиная с 0. 
        /// Его включительность или исключительность определяется значением параметра <paramref name="includeEnd"/>. 
        /// Если меньше значения параметра <paramref name="startIndex"/> - выбрасывается исключение. 
        /// Если превышает фактическую длину строки, а значение параметра <paramref name="untilEnd"/> установлено в false - выбрасывается исключение.</param>
        /// <param name="includeStart">Булевый флаг, определяющий, включать (true) или не включать (false) начальный индекс.</param>
        /// <param name="includeEnd">Булевый флаг, определяющий, включать (true) или не включать (false) конечный индекс.</param>
        /// <param name="untilEnd">Булевый флаг, определяющий, считывать ли строку до конца, 
        /// если конечный индекс <paramref name="endIndex"/> превысил её длину (true), 
        /// или же выбросить исключение (false).</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static String SubstringWithEnd(this String input, Int32 startIndex, Int32 endIndex, Boolean includeStart, Boolean includeEnd, Boolean untilEnd)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            Int32 length = input.Length;
            if (length == 0) { throw new ArgumentException("Входная строка не содержит ни одного символа", "input"); }
            if (startIndex < 0) { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Начальный индекс не может быть меньше 0"); }
            if (startIndex >= length)
            { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Начальный индекс = " + startIndex + " не может быть больше длины входной строки = " + length); }
            if (endIndex < startIndex)
            { throw new ArgumentOutOfRangeException("endIndex", endIndex, "Конечный индекс = " + endIndex + " не может быть меньше начального = " + startIndex); }
            const String empty = "";
            if (startIndex == endIndex)
            {
                if (includeStart == true && includeEnd == true)
                { return input.Substring(startIndex, 1); }
                else
                { return empty; }
            }
            if (endIndex - startIndex == 1)
            {
                if (includeStart == true && includeEnd == true)
                { return input.Substring(startIndex, 2); }
                else if (includeStart == false && includeEnd == false)
                { return empty; }
                else if (includeStart == true /*&& IncludeEnd == false*/)
                { return input.Substring(startIndex, 1); }
                else /*if (IncludeStart == false && IncludeEnd == true)*/
                { return input.Substring(startIndex + 1, 1); }
            }
            Int32 startIndexTemp;
            if (includeStart == true)
            { startIndexTemp = startIndex; }
            else
            { startIndexTemp = startIndex + 1; }
            if (untilEnd == false && endIndex >= length)
            { throw new ArgumentOutOfRangeException("endIndex", endIndex, "Конечный индекс = " + endIndex + " не может быть больше длины входной строки = " + length); }
            Int32 endIndexTemp;
            if (untilEnd == true && endIndex >= length)
            { endIndexTemp = length; }
            else if (includeEnd == false)
            { endIndexTemp = endIndex; }
            else
            { endIndexTemp = endIndex + 1; }
            String output = input.Substring(startIndexTemp, endIndexTemp - startIndexTemp);
            return output;
        }

        /// <summary>
        /// Возвращает подстроку, которая начинается с указанного начального индекса (включительно) и кончается указанным конечным индексом (исключительно). 
        /// Если конечный индекс превышает фактическую длину строки - она считывается до конца.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="startIndex">Включительный начальный индекс (позиция), отсчёт ведётся начиная с 0. Если меньше 0 - выбрасывается исключение.</param>
        /// <param name="endIndex">Исключительный конечный индекс (позиция), отсчёт ведётся начиная с 0. 
        /// Если меньше значения параметра <paramref name="startIndex"/> - выбрасывается исключение. 
        /// Если превышает фактическую длину строки, строка будет считана до конца.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static String SubstringWithEnd(this String input, Int32 startIndex, Int32 endIndex)
        {
            return input.SubstringWithEnd(startIndex, endIndex, true, false, true);
        }

        /// <summary>
        /// Возвращает обрезанную справа часть входной строки, содержащую левую часть и указанное количество символов
        /// </summary>
        /// <param name="source">Входная строка, обрезанную часть из которой следует возвратить. Если NULL или пустая, будет выброшено исключение.</param>
        /// <param name="count">Количество символов, которое должно присутствовать в возвращаемой строке. 
        /// Если 0 - возвращается пустая строка. Если равно или больше фактической длины входной строки, она возвращается без обрезаний. Если меньше 0 - выбрасывается исключение.</param>
        /// <returns></returns>
        public static String LeftSubstring(this String source, Int32 count)
        {
            if (source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "source"); }
            if (count < 0) { throw new ArgumentOutOfRangeException("count", count, "Значение параметра меньше 0"); }
            if (count == 0) { return ""; }
            if (count >= source.Length) { return source; }
            return source.Substring(0, count);
        }

        /// <summary>
        /// Возвращает строку, содержащую указанное число знаков с правой стороны входной строки
        /// </summary>
        /// <param name="source">Входная строка, обрезанную часть из которой следует возвратить. Если NULL или пустая, будет выброшено исключение.</param>
        /// <param name="count">Количество символов, которое должно присутствовать в возвращаемой строке. 
        /// Если 0 - возвращается пустая строка. Если равно или больше фактической длины входной строки, она возвращается без обрезаний. Если меньше 0 - выбрасывается исключение.</param>
        /// <returns></returns>
        public static String RightSubstring(this String source, Int32 count)
        {
            if (source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "source"); }
            if (count < 0) { throw new ArgumentOutOfRangeException("count", count, "Значение параметра меньше 0"); }
            if (count == 0) { return ""; }
            Int32 length = source.Length;
            if (count >= length) { return source; }
            return source.Substring(length - count, count);
        }

        /// <summary>
        /// Обрезает указанное количество символов из левой части (из начала) указанной строки и возвращает новую строку без этих символов
        /// </summary>        
        /// <param name="source">Входная строка, обрезанную часть которой следует возвратить. Если NULL или пустая, будет выброшено исключение.</param>
        /// <param name="count">Количество символов, которое должно быть обрезано слева. Если 0, обрезание не производится и возвращается входная строка. 
        /// Если равно или больше фактической длины входной строки, возвращается пустая строка. Если меньше 0 - выбрасывается исключение.</param>
        /// <returns>Новая строка, содержащая часть входной</returns>
        public static String CutLeft(this String source, Int32 count)
        {
            if (source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "source"); }
            if (count < 0) { throw new ArgumentOutOfRangeException("count", count, "Значение параметра меньше 0"); }
            if (count == 0) { return source; }
            if (count >= source.Length) { return ""; }
            return source.Substring(count);
        }

        /// <summary>
        /// Обрезает указанное количество символов из правой части (из конца) указанной строки и возвращает новую строку без этих символов
        /// </summary>        
        /// <param name="source">Входная строка, обрезанную часть которой следует возвратить. Если NULL или пустая, будет выброшено исключение.</param>
        /// <param name="count">Количество символов, которое должно быть обрезано справа. Если 0, обрезание не производится и возвращается входная строка. 
        /// Если равно или больше фактической длины входной строки, возвращается пустая строка. Если меньше 0 - выбрасывается исключение.</param>
        /// <returns>Новая строка, содержащая часть входной</returns>
        public static String CutRight(this String source, Int32 count)
        {
            if (source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "source"); }
            if (count < 0) { throw new ArgumentOutOfRangeException("count", count, "Значение параметра меньше 0"); }
            if (count == 0) { return source; }
            Int32 length = source.Length;
            if (count >= length) { return ""; }
            return source.Substring(0, length - count);
        }

        /// <summary>
        /// Возвращает обрезанную спереди (слева) копию входной строки без указанной подстроки, если входная строка начиналась с указанной подстроки
        /// </summary>
        /// <param name="source">Входная строка, обрезанную спереди часть которой следует возвратить. Если NULL или пустая, будет выброшено исключение.</param>
        /// <param name="target">Строка, равная подстроке, с которой начинается входная строка и которую следует обрезать. Если NULL или пустая, будет возвращена входная строка без изменений.</param>
        /// <param name="strComp">Опции сравнения строк между собой</param>
        /// <param name="recursive">Определяет, необходимо ли после первой обрезки проанализировать начало строки на совпадение ещё раз</param>
        /// <returns>Новый экземпляр строки</returns>
        public static String TrimStart(this String source, String target, StringComparison strComp, Boolean recursive)
        {
            if (source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "source"); }
            if (target.IsStringNullOrEmpty() == true) { return source; }
            if (source.StartsWith(target, strComp) == false)
            { return source; }
            if (recursive == false)
            { return source.CutLeft(target.Length); }
            String temp = source;
            while (temp.StartsWith(target, strComp) == true)
            {
                temp = temp.CutLeft(target.Length);
            }
            return temp;
        }

        /// <summary>
        /// Возвращает обрезанную сзади (справа) копию входной строки без указанной подстроки, если входная строка оканчивалась указанной подстрокой
        /// </summary>
        /// <param name="source">Входная строка, обрезанную сзади часть которой следует возвратить. Если NULL или пустая, будет выброшено исключение.</param>
        /// <param name="target">Строка, равная подстроке, которой оканчивается входная строка и которую следует обрезать. Если NULL или пустая, будет возвращена входная строка без изменений.</param>
        /// <param name="strComp">Опции сравнения строк между собой</param>
        /// <param name="recursive">Определяет, необходимо ли после первой обрезки проанализировать начало строки на совпадение ещё раз</param>
        /// <returns>Новый экземпляр строки</returns>
        public static String TrimEnd(this String source, String target, StringComparison strComp, Boolean recursive)
        {
            if (source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "source"); }
            if (target.IsStringNullOrEmpty() == true) { return source; }
            if (source.EndsWith(target, strComp) == false)
            { return source; }
            if (recursive == false)
            { return source.CutRight(target.Length); }
            String temp = source;
            while (temp.EndsWith(target, strComp) == true)
            {
                temp = temp.CutRight(target.Length);
            }
            return temp;
        }

        /// <summary>
        /// Определяет, начинается ли данная строка из любой из подстрок в указанном списке, возвращая индекс того элемента из списка, 
        /// с какого начинается строка, или же -1, если не начинается ни из одного.
        /// </summary>
        /// <param name="source">Входная строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="prefixes">Список подстрок. Если NULL или пустой - будет выброшено исключение.</param>
        /// <param name="strComp">Опции сравнения строк между собой</param>
        /// <returns>Индекс элемента из списка подстрок, с какого начинается входная строка, или -1, если не начинается ни с одного.</returns>
        public static Int32 StartsWithOneOf(this String source, IList<String> prefixes, StringComparison strComp)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (source.Length == 0) { throw new ArgumentException("Входная строка не может быть пустой", "source"); }
            if (prefixes == null) { throw new ArgumentNullException("prefixes"); }
            if (prefixes.Count == 0) { throw new ArgumentException("Список подстрок не может быть пустой", "prefixes"); }
            if (Enum.IsDefined(strComp.GetType(), strComp) == false) { throw new InvalidEnumArgumentException("strComp", (Int32)strComp, strComp.GetType()); }

            for (Int32 i = 0; i < prefixes.Count; i++)
            {
                String item = prefixes[i];
                if (item.IsNullOrEmpty() == true) { continue; }
                if (source.StartsWith(item, strComp) == true)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Определяет, заканчивается ли данная строка на любую из подстрок в указанном списке, возвращая индекс того элемента из списка, 
        /// каким заканчивается строка, или же -1, если не заканчивается ни одним из.
        /// </summary>
        /// <param name="source">Входная строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="postfixes">Список подстрок. Если NULL или пустой - будет выброшено исключение.</param>
        /// <param name="strComp">Опции сравнения строк между собой</param>
        /// <returns>Индекс элемента из списка подстрок, каким заканчивается входная строка, или -1, если не заканчивается ни одним из.</returns>
        public static Int32 EndsWithOneOf(this String source, IList<String> postfixes, StringComparison strComp)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (source.Length == 0) { throw new ArgumentException("Входная строка не может быть пустой", "source"); }
            if (postfixes == null) { throw new ArgumentNullException("postfixes"); }
            if (postfixes.Count == 0) { throw new ArgumentException("Список подстрок не может быть пустой", "postfixes"); }
            if (Enum.IsDefined(strComp.GetType(), strComp) == false) { throw new InvalidEnumArgumentException("strComp", (Int32)strComp, strComp.GetType()); }

            for (Int32 i = 0; i < postfixes.Count; i++)
            {
                String item = postfixes[i];
                if (item.IsNullOrEmpty() == true) { continue; }
                if (source.EndsWith(item, strComp) == true)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Повторяет значение строки указанное число раз
        /// </summary>
        /// <param name="source">Строка, которую следует повторить указанное число раз. Если NULL или пустая - она будет возвращена без изменений.</param>
        /// <param name="count">Число, указывающее количество повторений строки. Если меньше 1, будет выброшено исключение.</param>
        /// <returns>Новая строка</returns>
        public static String Replicate(this String source, Int32 count)
        {
            if (source.IsStringNullOrEmpty() == true || count == 1) { return source; }
            if (count < 1) { throw new ArgumentOutOfRangeException("count", count, "Невозможно сделать репликацию строки меньше 1 раза"); }
            StringBuilder temp = new StringBuilder(source.Length * count);
            for (int i = 0; i < count; i++)
            {
                temp.Append(source);
            }
            return temp.ToString();
        }

        /// <summary>
        /// Удаляет из исходной строки указанное количество символов, начиная с указанной позиции, и вставляет на их место указанную строку
        /// </summary>
        /// <param name="source">Входная строка, в которой производится удаление и вставка. Если NULL - будет выброшено исключение. 
        /// Если пустая и указанная позиция равна 0 - фактически будет возвращена строка Replacement</param>
        /// <param name="startIndex">Начальная позиция (индекс) во входной строке, с которой необходимо произвести удаление символов и на которую необходимо вставить указанную строку-заменитель. 
        /// Начинается с 0. Если значение больше длины входной строки или меньше 0, выбрасывается исключение.</param>
        /// <param name="length">Количество символов, которые необходимо удалить. Если равно 0 - не будет удалён ни один символ, а указаная строка просто внедрится во входную по указанной позиции. 
        /// Если больше длины исходной строки с учётом смещения, указанного начальной позицией, то удалится вся часть входной строки вплоть до конца. Если меньше 0 - будет выброшено исключение.</param>
        /// <param name="replacement">Строка-заменитель, которая должна внедриться во входную строку. Если NULL - будет выброшено исключение.</param>
        /// <returns>Новая строка, не являющаяся NULL</returns>
        public static String Stuff(this String source, Int32 startIndex, Int32 length, String replacement)
        {
            if (source == null) { throw new ArgumentNullException("source", "Входная строка не может быть NULL"); }
            if (replacement == null) { throw new ArgumentNullException("replacement", "Указанная строка-заменитель не может быть NULL"); }
            if (startIndex < 0) { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Стартовая позиция меньше 0"); }
            if (length < 0) { throw new ArgumentOutOfRangeException("length", length, "Количество символов для удаления меньше 0"); }
            Int32 sourceLength = source.Length;
            if (sourceLength == 0 && startIndex == 0)
            { return replacement; }
            if (startIndex >= sourceLength)
            { throw new ArgumentOutOfRangeException("startIndex", startIndex, String.Format("Указанная стартовая позиция ('{0}') больше, чем количество символов в строке ('{1}')", startIndex, sourceLength)); }
            Int32 replacementLength = replacement.Length;
            StringBuilder result = new StringBuilder(sourceLength + replacementLength);
            if (startIndex > 0)
            {
                result.Append(source.Substring(0, startIndex));
            }
            if (replacementLength > 0)
            {
                result.Append(replacement);
            }
            if (startIndex + length < sourceLength)
            {
                result.Append(source.Substring(startIndex + length));
            }
            return result.ToString();
        }

        /// <summary>
        /// Определяет, является ли строка NULL или пустой
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Boolean IsStringNullOrEmpty(this String source)
        {
            if (String.IsNullOrEmpty(source) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Определяет, является ли строка NULL, пустой или состоящей из одних пробелов. Оболочка к String.IsNullOrWhiteSpace из .NET 4.0.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Если true - строка является NULL, пустая или содержит одни лишь пробелы</returns>
        public static Boolean IsStringNullEmptyWhiteSpace(this String source)
        {
            if (String.IsNullOrEmpty(source) == true) { return true; }
            if (source.Trim() == "") { return true; }
            return false;
        }

        /// <summary>
        /// Определяет, является ли строка NULL, пустой или состоящей из одних пробелов. Оболочка к String.IsNullOrWhiteSpace из .NET 4.0.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Boolean IsStringNullEmptyWs(this String source)
        {
            return IsStringNullEmptyWhiteSpace(source);
        }

        /// <summary>
        /// Определяет, все ли символы в строке являются цифрами. Возвращает "true", если все до одного являются цифрами.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Boolean IsStringDigitsOnly(this String source)
        {
            if (source.IsStringNullOrEmpty()==true)
            {
                return false;
            }
            foreach (Char ch in source)
            {
                if (Char.IsDigit(ch) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Определяет, содержится ли в строке хотя бы один символ, который можно увидеть, т.е. не пробел или управляющий символ. 
        /// Если строка является NULL, пустой, содержит только пробелы и/или управляющие символы, метод возвращает false.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Boolean HasVisibleChars(this String source)
        {
            if (String.IsNullOrEmpty(source) == true) { return false; }
            for (Int32 i = 0; i < source.Length; i++)
            {
                Char curr = source[i];
                if (Char.IsLetterOrDigit(curr) == true || Char.IsPunctuation(curr) == true || Char.IsSymbol(curr) == true) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Определяет, содержится ли в строке хотя бы один цифро-буквенный символ. Если содержится - возвращает true, если нет - false.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Boolean HasAlphaNumericChars(this String source)
        {
            if (String.IsNullOrEmpty(source) == true) { return false; }
            for (Int32 i = 0; i < source.Length; i++)
            {
                if (Char.IsLetterOrDigit(source, i) == true) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Ищет данную строку в указанной последовательности строк, используя указанную опцию сравнения строк и возвращает результат поиска
        /// </summary>
        /// <param name="source">Исходная строка, наличие которой требуется определить</param>
        /// <param name="strComp">Опция определения равенства строк, которая применяется для поиска</param>
        /// <param name="sequence">Последовательность, в которой происходит поиск. Если NULL или пустая, метод выбрасывает исключение.</param>
        /// <returns>true - присутствует, false - отсутствует</returns>
        /// <exception cref="ArgumentException">Указанный массив NULL или пустой</exception>
        public static Boolean IsIn(this String source, StringComparison strComp, params String[] sequence)
        {
            if (sequence.IsNullOrEmpty() == true) { throw new ArgumentException("Указанный массив NULL или пустой", "sequence"); }
            return source.IsIn(strComp, sequence.AsEnumerable<String>());
        }

        /// <summary>
        /// Ищет данную строку в указанной последовательности строк, используя указанную опцию сравнения строк и возвращает результат поиска
        /// </summary>
        /// <param name="source">Исходная строка, наличие которой требуется определить</param>        
        /// <param name="strComp">Опция определения равенства строк, которая применяется для поиска</param>
        /// <param name="sequence">Последовательность, в которой происходит поиск. Если NULL или пустая, метод возвращает false (отсутствует).</param>
        /// <returns>true - присутствует, false - отсутствует</returns>
        /// <exception cref="ArgumentException">Указанная последовательность NULL или пустая</exception>
        public static Boolean IsIn(this String source, StringComparison strComp, IEnumerable<String> sequence)
        {
            if (sequence.IsNullOrEmpty() == true)
            {
                throw new ArgumentException("Указанная последовательность NULL или пустая", "sequence");
            }
            StringComparer comp;
            switch (strComp)
            {
                case StringComparison.CurrentCulture:
                    comp = StringComparer.CurrentCulture;
                    break;
                case StringComparison.CurrentCultureIgnoreCase:
                    comp = StringComparer.CurrentCultureIgnoreCase;
                    break;
                case StringComparison.InvariantCulture:
                    comp = StringComparer.InvariantCulture;
                    break;
                case StringComparison.InvariantCultureIgnoreCase:
                    comp = StringComparer.InvariantCultureIgnoreCase;
                    break;
                case StringComparison.Ordinal:
                    comp = StringComparer.Ordinal;
                    break;
                case StringComparison.OrdinalIgnoreCase:
                    comp = StringComparer.OrdinalIgnoreCase;
                    break;
                default:
                    throw new InvalidEnumArgumentException("strComp", (Int32)strComp, strComp.GetType());
            }
            return sequence.Contains<String>(source, comp);
        }

        /// <summary>
        /// Возвращает указанную строку, из которой вычищены все управляющие символы типа переноса строки и т.д.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static String CleanString(this String source)
        {
            if (String.IsNullOrEmpty(source) == true) { return source; }
            StringBuilder sb = new StringBuilder(source.Length);
            foreach (Char c in source)
            {
                if (Char.IsControl(c) == true) { continue; }
                sb.Append(c);
            }
            source = sb.ToString();
            return source;
        }

        /// <summary>
        /// Определяет, содержится ли во входной строке искомая подстрока с учётом указанных опций сравнения строк
        /// </summary>
        /// <param name="source">Входная строка, в которой нужно произвести поиск. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="toCheck">Искомая подстрока, которую нужно найти во входной. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="strComp">Опции сравнения строк между собой</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Входная строка или искомая подстрока является NULL или пустой</exception>
        /// <exception cref="InvalidEnumArgumentException">Значение параметра <paramref name="strComp"/> некорректно</exception>
        public static Boolean Contains(this String source, String toCheck, StringComparison strComp)
        {
            if (source.IsNullOrEmpty()==true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "source"); }
            if (toCheck.IsNullOrEmpty() == true) { throw new ArgumentException("Искомая подстрока не может быть NULL или пустой", "toCheck"); }
            if (Enum.IsDefined(strComp.GetType(), strComp) == false) {throw new InvalidEnumArgumentException("strComp", (Int32)strComp, strComp.GetType());}

            Boolean result = source.IndexOf(toCheck, strComp) >= 0;
            return result;
        }

        /// <summary>
        /// Определяет, содержится ли во входной строке искомая подстрока с учётом указанных глобализированных опций сравнения строк и культуры
        /// </summary>
        /// <param name="source">Входная строка, в которой нужно произвести поиск. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="toCheck">Искомая подстрока, которую нужно найти во входной. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="compOpt">Глобализированные опции сравнения строк между собой</param>
        /// <param name="culture">Культура, с учётом которой ведётся поиск. Если NULL - будет применена инвариантная культура.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Входная строка или искомая подстрока является NULL или пустой</exception>
        public static Boolean Contains(this String source, String toCheck, CompareOptions compOpt, CultureInfo culture)
        {
            if (source.IsNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "source"); }
            if (toCheck.IsNullOrEmpty() == true) { throw new ArgumentException("Искомая подстрока не может быть NULL или пустой", "toCheck"); }
            if (Enum.IsDefined(compOpt.GetType(), compOpt) == false) { throw new InvalidEnumArgumentException("compOpt", (Int32)compOpt, compOpt.GetType()); }
            if (culture == null)
            {
                culture = CultureInfo.InvariantCulture;
            }
            Boolean result = culture.CompareInfo.IndexOf(source, toCheck, compOpt) >= 0;
            return result;
        }
        
        /// <summary>
        /// Заменяет во входной строке все вхождения всех указанных (искомых) символов на целевую подстроку. Если ни одного искомого символа не найдено, возвращается копия входной строки.
        /// </summary>
        /// <param name="source">Входная строка. Если NULL или пустая, выбрасывается исключение.</param>
        /// <param name="destination">Новая целевая подстрока, которой требуется заменить все указнные искомые символы. Если NULL, выбрасывается исключение.</param>
        /// <param name="target">Массив искомых символов, все из которых требуется заменить. Дубликаты игнорируются. Если NULL или пустой - входная строка возвращается без изменений.</param>
        /// <returns>Новая строка, содержащая результат работы метода</returns>
        public static String MultiReplace(this String source, String destination, params Char[] target)
        {
            if (source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "source"); }
            if (destination == null) { throw new ArgumentNullException("destination"); }
            if (target.IsNullOrEmpty() == true) { return source; }

            List<Char> distinctTarget = target.Distinct<Char>().ToList<Char>();
            StringBuilder output = new StringBuilder(source.Length);

            foreach (Char c in source)
            {
                if (c.IsIn(distinctTarget) == true)
                {
                    output.Append(destination);
                }
                else
                {
                    output.Append(c);
                }
            }
            return output.ToString();
        }

        /// <summary>
        /// Заменяет во входной строке все вхождения всех указанных (искомых) символов на целевой символ. Если ни одного искомого символа не найдено, возвращается копия исходной строки.
        /// </summary>
        /// <param name="source">Входная строка. Если NULL или пустая, выбрасывается исключение.</param>
        /// <param name="destination">Новый целевой символ, которым требуется заменить все указнные искомые символы. Если NULL, выбрасывается исключение.</param>
        /// <param name="target">Массив искомых символов, все из которых требуется заменить. Дубликаты игнорируются. Если NULL или пустой - входная строка возвращается без изменений.</param>
        /// <returns>Новая строка, содержащая результат работы метода</returns>
        public static String MultiReplace(this String source, Char destination, params Char[] target)
        {
            if (source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "source"); }
            if (target.IsNullOrEmpty() == true) { return source; }

            List<Char> distinctTarget = target.Distinct<Char>().ToList<Char>();
            StringBuilder output = new StringBuilder(source.Length);

            foreach (Char c in source)
            {
                if (c.IsIn(distinctTarget) == true)
                {
                    output.Append(destination);
                }
                else
                {
                    output.Append(c);
                }
            }
            return output.ToString();
        }

        /// <summary>
        /// Удаляет все пробелы из указанной строки
        /// </summary>
        /// <param name="source">Входная строка. Если NULL или пустая - возвращается без изменений.</param>
        /// <returns></returns>
        public static String RemoveSpaces(this String source)
        {
            if (source.IsStringNullOrEmpty() == true) { return source; }
            StringBuilder temp = new StringBuilder(source.Length);
            foreach (Char c in source)
            {
                if (Char.IsWhiteSpace(c) == false)
                {
                    temp.Append(c);
                }
            }
            return temp.ToString();
        }

        /// <summary>
        /// Опции паддинга (выравнивания) частей строк
        /// </summary>
        public enum PaddingOptions : byte
        {
            /// <summary>
            /// Не применять паддинг = 0
            /// </summary>
            DoNotPad = 0,

            /// <summary>
            /// Дополнить строку со старта (с начала) = 1
            /// </summary>
            PadLeft = 1,

            /// <summary>
            /// Дополнить строку с конца = 2
            /// </summary>
            PadRight = 2
        }

        /// <summary>
        /// Разбивает данную строку на части с указанной длиной, начиная с начала или с конца, и применяя паддинг к той части, которая меньше указанной длины
        /// </summary>
        /// <param name="source">Входная строка, которую следует разбить. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="partSize">Длина части выходной подстроки (в символах), на которые следует разбить входную строку. Если меньше 1, будет выброшено исключение. 
        /// Если больше или равно длине входной строки, в выходной массиве будет содержаться одна входная строка</param>
        /// <param name="splitFromEnd">Определяет, откуда будет идти разбиение входной строки: с начала (false) или с конца (true).</param>
        /// <param name="paddingOpt">Определяет, применять ли паддинг (выравнивание) при помощи пробела, и если да, то какой, по отношению к той выходной подстроке, длина которой меньше 
        /// длины части <paramref name="partSize"/>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <returns>Массив выходных подстрок, длина которого не может быть меньше 1</returns>
        public static String[] Split(this String source, Int32 partSize, Boolean splitFromEnd, PaddingOptions paddingOpt)
        {
            if (source == null) { throw new ArgumentNullException("source", "Входная строка не может быть NULL"); }
            if (source.Length == 0) { throw new ArgumentException("Входная строка не может быть пустой", "source"); }
            if (partSize < 1) { throw new ArgumentOutOfRangeException("partSize", partSize, "Длина части выходной подстроки не может быть меньше 1"); }
            if (((Byte)paddingOpt).IsIn<Byte>(0, 1, 2) == false) { throw new InvalidEnumArgumentException("paddingOpt", (Int32)paddingOpt, paddingOpt.GetType()); }
            const Char space = ' ';
            String[] output;
            if (partSize == 1)
            {
                output = new string[source.Length];
                for (Int32 i = 0; i < source.Length; i++)
                {
                    output[i] = source[i].ToString();
                }
            }
            else if (partSize == source.Length)
            {
                output = new string[1] { source };
            }
            else if (partSize > source.Length)
            {
                output = new string[1];
                switch (paddingOpt)
                {
                    case PaddingOptions.DoNotPad:
                        output[0] = source;
                        break;
                    case PaddingOptions.PadLeft:
                        output[0] = new string(space, partSize - source.Length) + source;
                        break;
                    case PaddingOptions.PadRight:
                        output[0] = source + new string(space, partSize - source.Length);
                        break;
                }
            }
            else //if(PartSize < Source.Length)
            {
                Int32 reminder;
                Int32 quotient = Math.DivRem(source.Length, partSize, out reminder);
                if (reminder > 0)
                {
                    quotient++;
                }
                output = new string[quotient];
                if (splitFromEnd == false) // split from start
                {
                    for (Int32 i = 0; i < quotient; i++)
                    {
                        String temp = source.SubstringWithEnd(i * partSize, i * partSize + partSize);
                        if (i == quotient - 1)
                        {
                            switch (paddingOpt)
                            {
                                case PaddingOptions.DoNotPad:
                                    output[i] = temp;
                                    break;
                                case PaddingOptions.PadLeft:
                                    output[i] = new string(space, partSize - temp.Length) + temp;
                                    break;
                                case PaddingOptions.PadRight:
                                    output[i] = temp + new string(space, partSize - temp.Length);
                                    break;
                            }
                        }
                        else
                        {
                            output[i] = temp;
                        }
                    }
                }
                else //split from end
                {

                    for (Int32 i = 0; i < quotient; i++)
                    {
                        if (i == quotient - 1)
                        {
                            String temp = source.SubstringWithEnd(0, source.Length - partSize * i);
                            switch (paddingOpt)
                            {
                                case PaddingOptions.DoNotPad:
                                    output[output.Length - 1 - i] = temp;
                                    break;
                                case PaddingOptions.PadLeft:
                                    output[output.Length - 1 - i] = new string(space, partSize - temp.Length) + temp;
                                    break;
                                case PaddingOptions.PadRight:
                                    output[output.Length - 1 - i] = temp + new string(space, partSize - temp.Length);
                                    break;
                            }
                        }
                        else
                        {
                            String temp = source.SubstringWithEnd(source.Length - partSize * (i + 1), source.Length - partSize * i);
                            output[output.Length - 1 - i] = temp;
                        }
                    }
                }
            }
            return output;
        }

        /// <summary>
        /// Пытается распарсить входную строку и извлечь из неё число указанного типа, используя указанные стили числовой строки, и если это не удаётся, то возвращает NULL
        /// </summary>
        /// <typeparam name="TNumber">Тип числа, в которое необходимо конвертировать строку и которое следует вернуть. Поддерживается один из следующих типов: 
        /// Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double, Decimal. Если указанный тип не является одним из данных, 
        /// выбрасывается исключение.</typeparam>
        /// <param name="source">Входная строка, которая должна быть распарсена</param>
        /// <param name="style">Правила парсинга числовой строки</param>
        /// <param name="cultureProvider">Формат языка и региональных параметров, с применением которого будет парситься строка. Если NULL, будет применён инвариантный формат.</param>
        /// <exception cref="ArgumentOutOfRangeException">Указанный тип данных не является подерживаемым числом</exception>
        /// <returns></returns>
        public static Nullable<TNumber> TryParseNumber<TNumber>(this String source, NumberStyles style, IFormatProvider cultureProvider)
            where TNumber : struct, IEquatable<TNumber>, IComparable<TNumber>, IComparable, IFormattable, IConvertible
        {
            if (source.HasVisibleChars() == false) { return null; }
            if (StringTools.StringAnalyzers.GetNumberOfDigits(source) == 0) { return null; }
            if (cultureProvider == null) { cultureProvider = CultureInfo.InvariantCulture; }
            Type inputType = typeof(TNumber);

            if (inputType == typeof(Single))
            {//Single
                Single output;
                if (Single.TryParse(source, style, cultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (inputType == typeof(Double))
            {//Double
                Double output;
                if (Double.TryParse(source, style, cultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (inputType == typeof(Decimal))
            {//Decimal
                Decimal output;
                if (Decimal.TryParse(source, style, cultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (inputType == typeof(Byte))
            {//Byte
                Byte output;
                if (Byte.TryParse(source, style, cultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (inputType == typeof(SByte))
            {//SByte
                SByte output;
                if (SByte.TryParse(source, style, cultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (inputType == typeof(Int16))
            {//Int16
                Int16 output;
                if (Int16.TryParse(source, style, cultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (inputType == typeof(UInt16))
            {//UInt16
                UInt16 output;
                if (UInt16.TryParse(source, style, cultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (inputType == typeof(Int32))
            {//Int32
                Int32 output;
                if (Int32.TryParse(source, style, cultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (inputType == typeof(UInt32))
            {//UInt32
                UInt32 output;
                if (UInt32.TryParse(source, style, cultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (inputType == typeof(Int64))
            {//Int64
                Int64 output;
                if (Int64.TryParse(source, style, cultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (inputType == typeof(UInt64))
            {//UInt64
                UInt64 output;
                if (UInt64.TryParse(source, style, cultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }
            throw new ArgumentOutOfRangeException("TNumber", "Указанный тип данных должен быть числом, тогда как является " + inputType.FullName);
        }

        /// <summary>
        /// Пытается распарсить исходную строку и извлечь из неё число указанного типа, используя указанные стили числовой строки, и если это не удаётся, 
        /// то возвращает указанное число <paramref name="failValue"/>
        /// </summary>
        /// <typeparam name="TNumber">Тип числа, в которое необходимо конвертировать строку и которое следует вернуть. Поддерживается один из следующих типов: 
        /// Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double, Decimal. Если указанный тип не является одним из данных, 
        /// выбрасывается исключение.</typeparam>
        /// <param name="source">Входная строка, которая должна быть распарсена</param>
        /// <param name="style">Правила парсинга числовой строки</param>
        /// <param name="culture">Формат языка и региональных параметров, с применением которого будет парситься строка. Если NULL, будет применён инвариантный формат.</param>
        /// <param name="failValue">Значение, которое будет возвращено методом, если парсинг будет неудачным</param>
        /// <exception cref="ArgumentOutOfRangeException">Указанный тип данных не является подерживаемым числом</exception>
        /// <returns></returns>
        public static TNumber TryParseNumber<TNumber>(this String source, NumberStyles style, CultureInfo culture, TNumber failValue)
            where TNumber : struct, IEquatable<TNumber>, IComparable<TNumber>, IComparable, IFormattable, IConvertible
        {
            Nullable<TNumber> res = StringExtensionMethods.TryParseNumber<TNumber>(source, style, culture);
            if (res.HasValue == true)
            {
                return res.Value;
            }
            else
            {
                return failValue;
            }
        }
    }
}

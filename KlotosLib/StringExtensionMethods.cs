using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace KlotosLib
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
        /// <param name="Input">Входная строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="StartIndex">Начальный индекс (позиция), отсчёт ведётся начиная с 0. 
        /// Его включительность или исключительность определяется значением параметра <paramref name="IncludeStart"/>. 
        /// Если меньше 0 - выбрасывается исключение.</param>
        /// <param name="EndIndex">Конечный индекс (позиция), отсчёт ведётся начиная с 0. 
        /// Его включительность или исключительность определяется значением параметра <paramref name="IncludeEnd"/>. 
        /// Если меньше значения параметра <paramref name="StartIndex"/> - выбрасывается исключение. 
        /// Если превышает фактическую длину строки, а значение параметра <paramref name="UntilEnd"/> установлено в false - выбрасывается исключение.</param>
        /// <param name="IncludeStart">Булевый флаг, определяющий, включать (true) или не включать (false) начальный индекс.</param>
        /// <param name="IncludeEnd">Булевый флаг, определяющий, включать (true) или не включать (false) конечный индекс.</param>
        /// <param name="UntilEnd">Булевый флаг, определяющий, считывать ли строку до конца, 
        /// если конечный индекс <paramref name="EndIndex"/> превысил её длину (true), 
        /// или же выбросить исключение (false).</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static String SubstringWithEnd(this String Input, Int32 StartIndex, Int32 EndIndex, Boolean IncludeStart, Boolean IncludeEnd, Boolean UntilEnd)
        {
            if (Input == null) { throw new ArgumentNullException("Input"); }
            Int32 length = Input.Length;
            if (length == 0) { throw new ArgumentException("Входная строка не содержит ни одного символа", "Input"); }
            if (StartIndex < 0) { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Начальный индекс не может быть меньше 0"); }
            if (StartIndex >= length)
            { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Начальный индекс = " + StartIndex + " не может быть больше длины входной строки = " + length); }
            if (EndIndex < StartIndex)
            { throw new ArgumentOutOfRangeException("EndIndex", EndIndex, "Конечный индекс = " + EndIndex + " не может быть меньше начального = " + StartIndex); }
            const String empty = "";
            if (StartIndex == EndIndex)
            {
                if (IncludeStart == true && IncludeEnd == true)
                { return Input.Substring(StartIndex, 1); }
                else
                { return empty; }
            }
            if (EndIndex - StartIndex == 1)
            {
                if (IncludeStart == true && IncludeEnd == true)
                { return Input.Substring(StartIndex, 2); }
                else if (IncludeStart == false && IncludeEnd == false)
                { return empty; }
                else if (IncludeStart == true && IncludeEnd == false)
                { return Input.Substring(StartIndex, 1); }
                else
                { return Input.Substring(StartIndex + 1, 1); }
            }
            Int32 start_index_temp;
            if (IncludeStart == true)
            { start_index_temp = StartIndex; }
            else
            { start_index_temp = StartIndex + 1; }
            if (UntilEnd == false && EndIndex >= length)
            { throw new ArgumentOutOfRangeException("EndIndex", EndIndex, "Конечный индекс = " + EndIndex + " не может быть больше длины входной строки = " + length); }
            Int32 end_index_temp;
            if (UntilEnd == true && EndIndex >= length)
            { end_index_temp = length; }
            else if (IncludeEnd == false)
            { end_index_temp = EndIndex; }
            else
            { end_index_temp = EndIndex + 1; }
            String output = Input.Substring(start_index_temp, end_index_temp - start_index_temp);
            return output;
        }

        /// <summary>
        /// Возвращает подстроку, которая начинается с указанного начального индекса (включительно) и кончается указанным конечным индексом (исключительно). 
        /// Если конечный индекс превышает фактическую длину строки - она считывается до конца.
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="StartIndex">Включительный начальный индекс (позиция), отсчёт ведётся начиная с 0. Если меньше 0 - выбрасывается исключение.</param>
        /// <param name="EndIndex">Исключительный конечный индекс (позиция), отсчёт ведётся начиная с 0. 
        /// Если меньше значения параметра <paramref name="StartIndex"/> - выбрасывается исключение. 
        /// Если превышает фактическую длину строки, строка будет считана до конца.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static String SubstringWithEnd(this String Input, Int32 StartIndex, Int32 EndIndex)
        {
            return Input.SubstringWithEnd(StartIndex, EndIndex, true, false, true);
        }

        /// <summary>
        /// Возвращает обрезанную справа часть входной строки, содержащую левую часть и указанное количество символов
        /// </summary>
        /// <param name="Source">Входная строка, обрезанную часть из которой следует возвратить. Если NULL или пустая, будет выброшено исключение.</param>
        /// <param name="Count">Количество символов, которое должно присутствовать в возвращаемой строке. 
        /// Если 0 - возвращается пустая строка. Если равно или больше фактической длины входной строки, она возвращается без обрезаний. Если меньше 0 - выбрасывается исключение.</param>
        /// <returns></returns>
        public static String LeftSubstring(this String Source, Int32 Count)
        {
            if (Source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Source"); }
            if (Count < 0) { throw new ArgumentOutOfRangeException("Count", Count, "Значение параметра меньше 0"); }
            if (Count == 0) { return ""; }
            if (Count >= Source.Length) { return Source; }
            return Source.Substring(0, Count);
        }

        /// <summary>
        /// Возвращает строку, содержащую указанное число знаков с правой стороны входной строки
        /// </summary>
        /// <param name="Source">Входная строка, обрезанную часть из которой следует возвратить. Если NULL или пустая, будет выброшено исключение.</param>
        /// <param name="Count">Количество символов, которое должно присутствовать в возвращаемой строке. 
        /// Если 0 - возвращается пустая строка. Если равно или больше фактической длины входной строки, она возвращается без обрезаний. Если меньше 0 - выбрасывается исключение.</param>
        /// <returns></returns>
        public static String RightSubstring(this String Source, Int32 Count)
        {
            if (Source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Source"); }
            if (Count < 0) { throw new ArgumentOutOfRangeException("Count", Count, "Значение параметра меньше 0"); }
            if (Count == 0) { return ""; }
            Int32 length = Source.Length;
            if (Count >= length) { return Source; }
            return Source.Substring(length - Count, Count);
        }

        /// <summary>
        /// Обрезает указанное количество символов из левой части (из начала) указанной строки и возвращает новую строку без этих символов
        /// </summary>        
        /// <param name="Source">Входная строка, обрезанную часть которой следует возвратить. Если NULL или пустая, будет выброшено исключение.</param>
        /// <param name="Count">Количество символов, которое должно быть обрезано слева. Если 0, обрезание не производится и возвращается входная строка. 
        /// Если равно или больше фактической длины входной строки, возвращается пустая строка. Если меньше 0 - выбрасывается исключение.</param>
        /// <returns>Новая строка, содержащая часть входной</returns>
        public static String CutLeft(this String Source, Int32 Count)
        {
            if (Source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Source"); }
            if (Count < 0) { throw new ArgumentOutOfRangeException("Count", Count, "Значение параметра меньше 0"); }
            if (Count == 0) { return Source; }
            if (Count >= Source.Length) { return ""; }
            return Source.Substring(Count);
        }

        /// <summary>
        /// Обрезает указанное количество символов из правой части (из конца) указанной строки и возвращает новую строку без этих символов
        /// </summary>        
        /// <param name="Source">Входная строка, обрезанную часть которой следует возвратить. Если NULL или пустая, будет выброшено исключение.</param>
        /// <param name="Count">Количество символов, которое должно быть обрезано справа. Если 0, обрезание не производится и возвращается входная строка. 
        /// Если равно или больше фактической длины входной строки, возвращается пустая строка. Если меньше 0 - выбрасывается исключение.</param>
        /// <returns>Новая строка, содержащая часть входной</returns>
        public static String CutRight(this String Source, Int32 Count)
        {
            if (Source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Source"); }
            if (Count < 0) { throw new ArgumentOutOfRangeException("Count", Count, "Значение параметра меньше 0"); }
            if (Count == 0) { return Source; }
            Int32 length = Source.Length;
            if (Count >= length) { return ""; }
            return Source.Substring(0, length - Count);
        }

        /// <summary>
        /// Возвращает обрезанную спереди (слева) копию входной строки без указанной подстроки, если входная строка начиналась с указанной подстроки
        /// </summary>
        /// <param name="Source">Входная строка, обрезанную спереди часть которой следует возвратить. Если NULL или пустая, будет выброшено исключение.</param>
        /// <param name="Target">Строка, равная подстроке, с которой начинается входная строка и которую следует обрезать. Если NULL или пустая, будет возвращена входная строка без изменений.</param>
        /// <param name="StrComp">Опции сравнения строк между собой</param>
        /// <param name="Recursive">Определяет, необходимо ли после первой обрезки проанализировать начало строки на совпадение ещё раз</param>
        /// <returns>Новый экземпляр строки</returns>
        public static String TrimStart(this String Source, String Target, StringComparison StrComp, Boolean Recursive)
        {
            if (Source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Source"); }
            if (Target.IsStringNullOrEmpty() == true) { return Source; }
            if (Source.StartsWith(Target, StrComp) == false)
            { return Source; }
            if (Recursive == false)
            { return Source.CutLeft(Target.Length); }
            String temp = Source;
            while (temp.StartsWith(Target, StrComp) == true)
            {
                temp = temp.CutLeft(Target.Length);
            }
            return temp;
        }

        /// <summary>
        /// Возвращает обрезанную сзади (справа) копию входной строки без указанной подстроки, если входная строка оканчивалась указанной подстрокой
        /// </summary>
        /// <param name="Source">Входная строка, обрезанную сзади часть которой следует возвратить. Если NULL или пустая, будет выброшено исключение.</param>
        /// <param name="Target">Строка, равная подстроке, которой оканчивается входная строка и которую следует обрезать. Если NULL или пустая, будет возвращена входная строка без изменений.</param>
        /// <param name="StrComp">Опции сравнения строк между собой</param>
        /// <param name="Recursive">Определяет, необходимо ли после первой обрезки проанализировать начало строки на совпадение ещё раз</param>
        /// <returns>Новый экземпляр строки</returns>
        public static String TrimEnd(this String Source, String Target, StringComparison StrComp, Boolean Recursive)
        {
            if (Source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Source"); }
            if (Target.IsStringNullOrEmpty() == true) { return Source; }
            if (Source.EndsWith(Target, StrComp) == false)
            { return Source; }
            if (Recursive == false)
            { return Source.CutRight(Target.Length); }
            String temp = Source;
            while (temp.EndsWith(Target, StrComp) == true)
            {
                temp = temp.CutRight(Target.Length);
            }
            return temp;
        }

        /// <summary>
        /// Определяет, начинается ли данная строка из любой из подстрок в указанном списке, возвращая индекс того элемента из списка, 
        /// с какого начинается строка, или же -1, если не начинается ни из одного.
        /// </summary>
        /// <param name="Source">Входная строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="Prefixes">Список подстрок. Если NULL или пустой - будет выброшено исключение.</param>
        /// <param name="StrComp">Опции сравнения строк между собой</param>
        /// <returns>Индекс элемента из списка подстрок, с какого начинается входная строка, или -1, если не начинается ни с одного.</returns>
        public static Int32 StartsWithOneOf(this String Source, IList<String> Prefixes, StringComparison StrComp)
        {
            if (Source == null) { throw new ArgumentNullException("Source"); }
            if (Source.Length == 0) { throw new ArgumentException("Входная строка не может быть пустой", "Source"); }
            if (Prefixes == null) { throw new ArgumentNullException("Prefixes"); }
            if (Prefixes.Count == 0) { throw new ArgumentException("Список подстрок не может быть пустой", "Prefixes"); }
            if (Enum.IsDefined(StrComp.GetType(), StrComp) == false) { throw new InvalidEnumArgumentException("StrComp", (Int32)StrComp, StrComp.GetType()); }

            for (Int32 i = 0; i < Prefixes.Count; i++)
            {
                String item = Prefixes[i];
                if (item.IsNullOrEmpty() == true) { continue; }
                if (Source.StartsWith(item, StrComp) == true)
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
        /// <param name="Source">Входная строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="Postfixes">Список подстрок. Если NULL или пустой - будет выброшено исключение.</param>
        /// <param name="StrComp">Опции сравнения строк между собой</param>
        /// <returns>Индекс элемента из списка подстрок, каким заканчивается входная строка, или -1, если не заканчивается ни одним из.</returns>
        public static Int32 EndsWithOneOf(this String Source, IList<String> Postfixes, StringComparison StrComp)
        {
            if (Source == null) { throw new ArgumentNullException("Source"); }
            if (Source.Length == 0) { throw new ArgumentException("Входная строка не может быть пустой", "Source"); }
            if (Postfixes == null) { throw new ArgumentNullException("Postfixes"); }
            if (Postfixes.Count == 0) { throw new ArgumentException("Список подстрок не может быть пустой", "Postfixes"); }
            if (Enum.IsDefined(StrComp.GetType(), StrComp) == false) { throw new InvalidEnumArgumentException("StrComp", (Int32)StrComp, StrComp.GetType()); }

            for (Int32 i = 0; i < Postfixes.Count; i++)
            {
                String item = Postfixes[i];
                if (item.IsNullOrEmpty() == true) { continue; }
                if (Source.EndsWith(item, StrComp) == true)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Повторяет значение строки указанное число раз
        /// </summary>
        /// <param name="Source">Строка, которую следует повторить указанное число раз. Если NULL или пустая - она будет возвращена без изменений.</param>
        /// <param name="Count">Число, указывающее количество повторений строки. Если меньше 1, будет выброшено исключение.</param>
        /// <returns>Новая строка</returns>
        public static String Replicate(this String Source, Int32 Count)
        {
            if (Source.IsStringNullOrEmpty() == true || Count == 1) { return Source; }
            if (Count < 1) { throw new ArgumentOutOfRangeException("Count", Count, "Невозможно сделать репликацию строки меньше 1 раза"); }
            StringBuilder temp = new StringBuilder(Source.Length * Count);
            for (int i = 0; i < Count; i++)
            {
                temp.Append(Source);
            }
            return temp.ToString();
        }

        /// <summary>
        /// Удаляет из исходной строки указанное количество символов, начиная с указанной позиции, и вставляет на их место указанную строку
        /// </summary>
        /// <param name="Source">Входная строка, в которой производится удаление и вставка. Если NULL - будет выброшено исключение. 
        /// Если пустая и указанная позиция равна 0 - фактически будет возвращена строка Replacement</param>
        /// <param name="StartIndex">Начальная позиция (индекс) во входной строке, с которой необходимо произвести удаление символов и на которую необходимо вставить указанную строку-заменитель. 
        /// Начинается с 0. Если значение больше длины входной строки или меньше 0, выбрасывается исключение.</param>
        /// <param name="Length">Количество символов, которые необходимо удалить. Если равно 0 - не будет удалён ни один символ, а указаная строка просто внедрится во входную по указанной позиции. 
        /// Если больше длины исходной строки с учётом смещения, указанного начальной позицией, то удалится вся часть входной строки вплоть до конца. Если меньше 0 - будет выброшено исключение.</param>
        /// <param name="Replacement">Строка-заменитель, которая должна внедриться во входную строку. Если NULL - будет выброшено исключение.</param>
        /// <returns>Новая строка, не являющаяся NULL</returns>
        public static String Stuff(this String Source, Int32 StartIndex, Int32 Length, String Replacement)
        {
            if (Source == null) { throw new ArgumentNullException("Source", "Входная строка не может быть NULL"); }
            if (Replacement == null) { throw new ArgumentNullException("Replacement", "Указанная строка-заменитель не может быть NULL"); }
            if (StartIndex < 0) { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Стартовая позиция меньше 0"); }
            if (Length < 0) { throw new ArgumentOutOfRangeException("Length", Length, "Количество символов для удаления меньше 0"); }
            Int32 source_length = Source.Length;
            if (source_length == 0 && StartIndex == 0)
            { return Replacement; }
            if (StartIndex >= source_length)
            { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, String.Format("Указанная стартовая позиция ('{0}') больше, чем количество символов в строке ('{1}')", StartIndex, source_length)); }
            Int32 replacement_length = Replacement.Length;
            StringBuilder result = new StringBuilder(source_length + replacement_length);
            if (StartIndex > 0)
            {
                result.Append(Source.Substring(0, StartIndex));
            }
            if (replacement_length > 0)
            {
                result.Append(Replacement);
            }
            if (StartIndex + Length < source_length)
            {
                result.Append(Source.Substring(StartIndex + Length));
            }
            return result.ToString();
        }

        /// <summary>
        /// Определяет, является ли строка NULL или пустой
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static Boolean IsStringNullOrEmpty(this String Source)
        {
            if (String.IsNullOrEmpty(Source) == true)
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
        /// <param name="Source"></param>
        /// <returns>Если true - строка является NULL, пустая или содержит одни лишь пробелы</returns>
        public static Boolean IsStringNullEmptyWhiteSpace(this String Source)
        {
            if (String.IsNullOrEmpty(Source) == true) { return true; }
            if (Source.Trim() == "") { return true; }
            return false;
        }

        /// <summary>
        /// Определяет, является ли строка NULL, пустой или состоящей из одних пробелов. Оболочка к String.IsNullOrWhiteSpace из .NET 4.0.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static Boolean IsStringNullEmptyWS(this String Source)
        {
            return IsStringNullEmptyWhiteSpace(Source);
        }

        /// <summary>
        /// Определяет, все ли символы в строке являются цифрами. Возвращает "true", если все до одного являются цифрами.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static Boolean IsStringDigitsOnly(this String Source)
        {
            if (Source == null)
            {
                return false;
            }
            foreach (Char ch in Source)
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
        /// <param name="Source"></param>
        /// <returns></returns>
        public static Boolean HasAlphaNumericChars(this String Source)
        {
            if (String.IsNullOrEmpty(Source) == true) { return false; }
            for (Int32 i = 0; i < Source.Length; i++)
            {
                if (Char.IsLetterOrDigit(Source, i) == true) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Ищет данную строку в указанной последовательности строк, используя указанную опцию сравнения строк и возвращает результат поиска
        /// </summary>
        /// <param name="Source">Исходная строка, наличие которой требуется определить</param>
        /// <param name="StrComp">Опция определения равенства строк, которая применяется для поиска</param>
        /// <param name="Sequence">Последовательность, в которой происходит поиск. Если NULL или пустая, метод выбрасывает исключение.</param>
        /// <returns>true - присутствует, false - отсутствует</returns>
        /// <exception cref="ArgumentException">Указанный массив NULL или пустой</exception>
        public static Boolean IsIn(this String Source, StringComparison StrComp, params String[] Sequence)
        {
            if (Sequence.IsNullOrEmpty() == true) { throw new ArgumentException("Указанный массив NULL или пустой", "Sequence"); }
            return Source.IsIn(StrComp, Sequence.AsEnumerable<String>());
        }

        /// <summary>
        /// Ищет данную строку в указанной последовательности строк, используя указанную опцию сравнения строк и возвращает результат поиска
        /// </summary>
        /// <param name="source">Исходная строка, наличие которой требуется определить</param>        
        /// <param name="StrComp">Опция определения равенства строк, которая применяется для поиска</param>
        /// <param name="Sequence">Последовательность, в которой происходит поиск. Если NULL или пустая, метод возвращает false (отсутствует).</param>
        /// <returns>true - присутствует, false - отсутствует</returns>
        /// <exception cref="ArgumentException">Указанная последовательность NULL или пустая</exception>
        public static Boolean IsIn(this String source, StringComparison StrComp, IEnumerable<String> Sequence)
        {
            if (Sequence.IsNullOrEmpty() == true) { throw new ArgumentException("Указанная последовательность NULL или пустая", "Sequence"); }
            StringComparer comp;
            switch (StrComp)
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
                    throw new InvalidEnumArgumentException("StrComp", (Int32)StrComp, StrComp.GetType());
            }
            return Sequence.Contains<String>(source, comp);
        }

        /// <summary>
        /// Возвращает указанную строку, из которой вычищены все управляющие символы типа переноса строки и т.д.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static String CleanString(this String Source)
        {
            if (String.IsNullOrEmpty(Source) == true) { return Source; }
            StringBuilder sb = new StringBuilder(Source.Length);
            foreach (Char c in Source)
            {
                if (Char.IsControl(c) == true) { continue; }
                sb.Append(c);
            }
            Source = sb.ToString();
            return Source;
        }

        /// <summary>
        /// Определяет, содержится ли во входной строке искомая подстрока с учётом указанных опций сравнения строк
        /// </summary>
        /// <param name="Source">Входная строка, в которой нужно произвести поиск. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="ToCheck">Искомая подстрока, которую нужно найти во входной. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="StrComp">Опции сравнения строк между собой</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Входная строка или искомая подстрока является NULL или пустой</exception>
        /// <exception cref="InvalidEnumArgumentException">Значение параметра <paramref name="StrComp"/> некорректно</exception>
        public static Boolean Contains(this String Source, String ToCheck, StringComparison StrComp)
        {
            if (Source.IsNullOrEmpty()==true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Source"); }
            if (ToCheck.IsNullOrEmpty() == true) { throw new ArgumentException("Искомая подстрока не может быть NULL или пустой", "ToCheck"); }
            if (Enum.IsDefined(StrComp.GetType(), StrComp) == false) {throw new InvalidEnumArgumentException("StrComp", (Int32)StrComp, StrComp.GetType());}


            Boolean result = Source.IndexOf(ToCheck, StrComp) >= 0;
            return result;
        }

        /// <summary>
        /// Определяет, содержится ли во входной строке искомая подстрока с учётом указанных глобализированных опций сравнения строк и культуры
        /// </summary>
        /// <param name="Source">Входная строка, в которой нужно произвести поиск. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="ToCheck">Искомая подстрока, которую нужно найти во входной. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="CompOpt">Глобализированные опции сравнения строк между собой</param>
        /// <param name="Culture">Культура, с учётом которой ведётся поиск. Если NULL - будет применена инвариантная культура.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Входная строка или искомая подстрока является NULL или пустой</exception>
        public static Boolean Contains(this String Source, String ToCheck, CompareOptions CompOpt, CultureInfo Culture)
        {
            if (Source.IsNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Source"); }
            if (ToCheck.IsNullOrEmpty() == true) { throw new ArgumentException("Искомая подстрока не может быть NULL или пустой", "ToCheck"); }
            if (Culture == null)
            {
                Culture = CultureInfo.InvariantCulture;
            }
            Boolean result = Culture.CompareInfo.IndexOf(Source, ToCheck, CompOpt) >= 0;
            return result;
        }

        /// <summary>
        /// Возвращает новую строку, символы в которой расположены в обратном порядке по сравнению с исходной строкой.
        /// </summary>
        public static String ReverseString(this String Source)
        {
            if (Source.IsStringNullEmptyWhiteSpace() == true) { return Source; }
            Char[] arr = Source.ToCharArray();
            Array.Reverse(arr);
            return new String(arr);
        }

        /// <summary>
        /// Заменяет во входной строке все вхождения всех указанных (искомых) символов на целевую подстроку. Если ни одного искомого символа не найдено, возвращается копия исходной строки.
        /// </summary>
        /// <param name="Source">Входная строка. Если NULL или пустая, выбрасывается исключение.</param>
        /// <param name="Destination">Новая целевая подстрока, которой требуется заменить все указнные искомые символы. Если NULL, выбрасывается исключение.</param>
        /// <param name="Target">Массив искомых символов, все из которых требуется заменить. Дубликаты игнорируются. Если NULL или пустой - входная строка возвращается без изменений.</param>
        /// <returns>Новая строка, содержащая результат работы метода</returns>
        public static String MultiReplace(this String Source, String Destination, params Char[] Target)
        {
            if (Source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Source"); }
            if (Destination == null) { throw new ArgumentNullException("Destination"); }
            if (Target.IsNullOrEmpty() == true) { return Source; }

            List<Char> distinct_target = Target.Distinct<Char>().ToList<Char>();
            StringBuilder output = new StringBuilder(Source.Length);

            foreach (Char c in Source)
            {
                if (c.IsIn(distinct_target) == true)
                {
                    output.Append(Destination);
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
        /// <param name="Source">Входная строка. Если NULL или пустая, выбрасывается исключение.</param>
        /// <param name="Destination">Новый целевой символ, которым требуется заменить все указнные искомые символы. Если NULL, выбрасывается исключение.</param>
        /// <param name="Target">Массив искомых символов, все из которых требуется заменить. Дубликаты игнорируются. Если NULL или пустой - входная строка возвращается без изменений.</param>
        /// <returns>Новая строка, содержащая результат работы метода</returns>
        public static String MultiReplace(this String Source, Char Destination, params Char[] Target)
        {
            if (Source.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Source"); }
            if (Target.IsNullOrEmpty() == true) { return Source; }

            List<Char> distinct_target = Target.Distinct<Char>().ToList<Char>();
            StringBuilder output = new StringBuilder(Source.Length);

            foreach (Char c in Source)
            {
                if (c.IsIn(distinct_target) == true)
                {
                    output.Append(Destination);
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
        /// <param name="Source">Входная строка. Если NULL или пустая - возвращается без изменений.</param>
        /// <returns></returns>
        public static String RemoveSpaces(this String Source)
        {
            if (Source.IsStringNullOrEmpty() == true) { return Source; }
            StringBuilder temp = new StringBuilder(Source.Length);
            foreach (Char c in Source)
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
        /// <param name="Source">Входная строка, которую следует разбить. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="PartSize">Длина части выходной подстроки (в символах), на которые следует разбить входную строку. Если меньше 1, будет выброшено исключение. 
        /// Если больше или равно длине входной строки, в выходной массиве будет содержаться одна входная строка</param>
        /// <param name="SplitFromEnd">Определяет, откуда будет идти разбиение входной строки: с начала (false) или с конца (true).</param>
        /// <param name="PaddingOpt">Определяет, применять ли паддинг (выравнивание) при помощи пробела, и если да, то какой, по отношению к той выходной подстроке, длина которой меньше 
        /// длины части <paramref name="PartSize"/>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <returns>Массив выходных подстрок, длина которого не может быть меньше 1</returns>
        public static String[] Split(this String Source, Int32 PartSize, Boolean SplitFromEnd, PaddingOptions PaddingOpt)
        {
            if (Source == null) { throw new ArgumentNullException("Source", "Входная строка не может быть NULL"); }
            if (Source.Length == 0) { throw new ArgumentException("Входная строка не может быть пустой", "Source"); }
            if (PartSize < 1) { throw new ArgumentOutOfRangeException("PartSize", PartSize, "Длина части выходной подстроки не может быть меньше 1"); }
            if (((Byte)PaddingOpt).IsIn<Byte>(0, 1, 2) == false) { throw new InvalidEnumArgumentException("PaddingOpt", (Int32)PaddingOpt, PaddingOpt.GetType()); }
            const Char space = ' ';
            String[] output;
            if (PartSize == 1)
            {
                output = new string[Source.Length];
                for (Int32 i = 0; i < Source.Length; i++)
                {
                    output[i] = Source[i].ToString();
                }
            }
            else if (PartSize == Source.Length)
            {
                output = new string[1] { Source };
            }
            else if (PartSize > Source.Length)
            {
                output = new string[1];
                switch (PaddingOpt)
                {
                    case PaddingOptions.DoNotPad:
                        output[0] = Source;
                        break;
                    case PaddingOptions.PadLeft:
                        output[0] = new string(space, PartSize - Source.Length) + Source;
                        break;
                    case PaddingOptions.PadRight:
                        output[0] = Source + new string(space, PartSize - Source.Length);
                        break;
                }
            }
            else //if(PartSize < Source.Length)
            {
                Int32 reminder;
                Int32 quotient = Math.DivRem(Source.Length, PartSize, out reminder);
                if (reminder > 0)
                {
                    quotient++;
                }
                output = new string[quotient];
                if (SplitFromEnd == false) // split from start
                {
                    for (Int32 i = 0; i < quotient; i++)
                    {
                        String temp = Source.SubstringWithEnd(i * PartSize, i * PartSize + PartSize);
                        if (i == quotient - 1)
                        {
                            switch (PaddingOpt)
                            {
                                case PaddingOptions.DoNotPad:
                                    output[i] = temp;
                                    break;
                                case PaddingOptions.PadLeft:
                                    output[i] = new string(space, PartSize - temp.Length) + temp;
                                    break;
                                case PaddingOptions.PadRight:
                                    output[i] = temp + new string(space, PartSize - temp.Length);
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
                            String temp = Source.SubstringWithEnd(0, Source.Length - PartSize * i);
                            switch (PaddingOpt)
                            {
                                case PaddingOptions.DoNotPad:
                                    output[output.Length - 1 - i] = temp;
                                    break;
                                case PaddingOptions.PadLeft:
                                    output[output.Length - 1 - i] = new string(space, PartSize - temp.Length) + temp;
                                    break;
                                case PaddingOptions.PadRight:
                                    output[output.Length - 1 - i] = temp + new string(space, PartSize - temp.Length);
                                    break;
                            }
                        }
                        else
                        {
                            String temp = Source.SubstringWithEnd(Source.Length - PartSize * (i + 1), Source.Length - PartSize * i);
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
        /// <param name="Source">Входная строка, которая должна быть распарсена</param>
        /// <param name="Style">Правила парсинга числовой строки</param>
        /// <param name="CultureProvider">Формат языка и региональных параметров, с применением которого будет парситься строка. Если NULL, будет применён инвариантный формат.</param>
        /// <exception cref="ArgumentOutOfRangeException">Указанный тип данных не является подерживаемым числом</exception>
        /// <returns></returns>
        public static Nullable<TNumber> TryParseNumber<TNumber>(this String Source, NumberStyles Style, IFormatProvider CultureProvider)
            where TNumber : struct, IEquatable<TNumber>, IComparable<TNumber>, IComparable, IFormattable, IConvertible
        {
            if (Source.HasVisibleChars() == false) { return null; }
            if (StringTools.StringAnalyzers.GetNumberOfDigits(Source) == 0) { return null; }
            if (CultureProvider == null) { CultureProvider = CultureInfo.InvariantCulture; }
            Type input_type = typeof(TNumber);

            if (input_type == typeof(Single))
            {//Single
                Single output;
                if (Single.TryParse(Source, Style, CultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (input_type == typeof(Double))
            {//Double
                Double output;
                if (Double.TryParse(Source, Style, CultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (input_type == typeof(Decimal))
            {//Decimal
                Decimal output;
                if (Decimal.TryParse(Source, Style, CultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (input_type == typeof(Byte))
            {//Byte
                Byte output;
                if (Byte.TryParse(Source, Style, CultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (input_type == typeof(SByte))
            {//SByte
                SByte output;
                if (SByte.TryParse(Source, Style, CultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (input_type == typeof(Int16))
            {//Int16
                Int16 output;
                if (Int16.TryParse(Source, Style, CultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (input_type == typeof(UInt16))
            {//UInt16
                UInt16 output;
                if (UInt16.TryParse(Source, Style, CultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (input_type == typeof(Int32))
            {//Int32
                Int32 output;
                if (Int32.TryParse(Source, Style, CultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (input_type == typeof(UInt32))
            {//UInt32
                UInt32 output;
                if (UInt32.TryParse(Source, Style, CultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (input_type == typeof(Int64))
            {//Int64
                Int64 output;
                if (Int64.TryParse(Source, Style, CultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }

            if (input_type == typeof(UInt64))
            {//UInt64
                UInt64 output;
                if (UInt64.TryParse(Source, Style, CultureProvider, out output) == false) { return null; }
                else { return (TNumber)(Object)output; }
            }
            throw new ArgumentOutOfRangeException("TNumber", "Указанный тип данных должен быть числом, тогда как является " + input_type.FullName);
        }

        /// <summary>
        /// Пытается распарсить исходную строку и извлечь из неё число указанного типа, используя указанные стили числовой строки, и если это не удаётся, 
        /// то возвращает указанное число <paramref name="FailValue"/>
        /// </summary>
        /// <typeparam name="TNumber">Тип числа, в которое необходимо конвертировать строку и которое следует вернуть. Поддерживается один из следующих типов: 
        /// Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double, Decimal. Если указанный тип не является одним из данных, 
        /// выбрасывается исключение.</typeparam>
        /// <param name="Source">Входная строка, которая должна быть распарсена</param>
        /// <param name="Style">Правила парсинга числовой строки</param>
        /// <param name="Culture">Формат языка и региональных параметров, с применением которого будет парситься строка. Если NULL, будет применён инвариантный формат.</param>
        /// <param name="FailValue">Значение, которое будет возвращено методом, если парсинг будет неудачным</param>
        /// <exception cref="ArgumentOutOfRangeException">Указанный тип данных не является подерживаемым числом</exception>
        /// <returns></returns>
        public static TNumber TryParseNumber<TNumber>(this String Source, NumberStyles Style, CultureInfo Culture, TNumber FailValue)
            where TNumber : struct, IEquatable<TNumber>, IComparable<TNumber>, IComparable, IFormattable, IConvertible
        {
            Nullable<TNumber> res = StringExtensionMethods.TryParseNumber<TNumber>(Source, Style, Culture);
            if (res.HasValue == true)
            {
                return res.Value;
            }
            else
            {
                return FailValue;
            }
        }
    }
}

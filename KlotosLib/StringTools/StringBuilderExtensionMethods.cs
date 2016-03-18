using System;
using System.Text;

namespace KlotosLib.StringTools
{
    /// <summary>
    /// Содержит методы расширения для изменяемого строкового типа StringBuilder
    /// </summary>
    public static class StringBuilderExtensionMethods
    {
        /// <summary>
        /// Определяет, является ли изменяемая строка NULL или пустой
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static Boolean IsNullOrEmpty(this StringBuilder Source)
        {
            if (Object.ReferenceEquals(null, Source) == true || Source.Length < 1) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Определяет, содержится ли в изменяемой строке хотя бы один цифро-буквенный символ. Если содержится - возвращает true, если нет - false.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static Boolean HasAlphaNumericChars(this StringBuilder Source)
        {
            if (Source.IsNullOrEmpty() == true) { return false; }
            for (Int32 i = 0; i < Source.Length; i++)
            {
                Char current = Source[i];
                if (Char.IsLetterOrDigit(current) == true) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Очищает содержимое изменяемой строки, усекая её длину (Length) до нуля, но оставляя внутренний буфер (Capacity) неизменённым. Изменения делаются именно в текущем экземляре, новый не создаётся.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static StringBuilder Clean(this StringBuilder Source)
        {
            if (Source.IsNullOrEmpty() == true) { return Source; }
            Source.Length = 0;
            return Source;
        }

        /// <summary>
        /// Обрезает символы из правой части (из конца) изменяемой строки, возвращая её левую часть, содержащую указанное количество символов, 
        /// и записывает изменение в этот же экземпляр
        /// </summary>
        /// <param name="Source">Входная изменяемая строка, обрезанную справа часть которой следует возвратить. 
        /// Если пустая - будет возвращена без изменений. Если NULL, будет выброшено исключение.</param>
        /// <param name="Count">Количество символов, которое останется после обрезки справа. Если 0, возвращается пустая строка. 
        /// Если равно или больше фактической длины входной строки, обрезание не производится и возвращается входная строка. Если меньше 0 - выбрасывается исключение.</param>
        /// <returns></returns>
        public static StringBuilder LeftSubstring(this StringBuilder Source, Int32 Count)
        {
            if (Source == null) { throw new ArgumentNullException("Source", "Входная строка не может быть NULL"); }
            if (Count < 0) { throw new ArgumentOutOfRangeException("Count", Count, "Значение параметра меньше 0"); }
            Int32 length = Source.Length;
            if (length == 0 || Count >= length) { return Source; }
            if (Count == 0)
            {
                Source.Clean();
                return Source;
            }
            Source.Remove(Count, length - Count);
            return Source;
        }

        /// <summary>
        /// Обрезает символы из левой части (из начала) изменяемой строки, возвращая её правую часть, содержащую указанное количество символов, 
        /// и записывает изменение в этот же экземпляр
        /// </summary>
        /// <param name="Source">Входная изменяемая строка, обрезанную слева часть которой следует возвратить. 
        /// Если пустая - будет возвращена без изменений. Если NULL, будет выброшено исключение.</param>
        /// <param name="Count">Количество символов, которое останется после обрезки слева. Если 0, возвращается пустая строка. 
        /// Если равно или больше фактической длины входной строки, обрезание не производится и возвращается входная строка. Если меньше 0 - выбрасывается исключение.</param>
        /// <returns></returns>
        public static StringBuilder RightSubstring(this StringBuilder Source, Int32 Count)
        {
            if (Source == null) { throw new ArgumentNullException("Source", "Входная строка не может быть NULL"); }
            if (Count < 0) { throw new ArgumentOutOfRangeException("Count", Count, "Значение параметра меньше 0"); }
            Int32 length = Source.Length;
            if (length == 0 || Count >= length) { return Source; }
            if (Count == 0)
            {
                Source.Clean();
                return Source;
            }
            Source.Remove(0, length - Count);
            return Source;
        }

        /// <summary>
        /// Обрезает указанное количество символов из левой части (из начала) изменяемой строки и записывает изменение в этот же экземпляр
        /// </summary>        
        /// <param name="Source">Входная изменяемая строка, обрезанную слева часть которой следует возвратить. 
        /// Если пустая - будет возвращена без изменений. Если NULL, будет выброшено исключение.</param>
        /// <param name="Count">Количество символов, которое должно быть обрезано слева. Если 0, обрезание не производится и возвращается входная строка. 
        /// Если равно или больше фактической длины входной строки, возвращается пустая строка. Если меньше 0 - выбрасывается исключение.</param>        
        public static StringBuilder CutLeft(this StringBuilder Source, Int32 Count)
        {
            if (Source == null) { throw new ArgumentNullException("Source", "Входная строка не может быть NULL"); }
            if (Count < 0) { throw new ArgumentOutOfRangeException("Count", Count, "Значение параметра меньше 0"); }
            if (Count == 0) { return Source; }
            Int32 length = Source.Length;
            if (length == 0) { return Source; }
            if (Count >= length)
            {
                Source.Clean();
                return Source;
            }
            Source.Remove(0, Count);
            return Source;
        }

        /// <summary>
        /// Обрезает указанное количество символов из правой части (из конца) изменяемой строки и записывает изменение в этот же экземпляр
        /// </summary>
        /// <param name="Source">Входная изменяемая строка, обрезанную справа часть которой следует возвратить. 
        /// Если пустая - будет возвращена без изменений. Если NULL, будет выброшено исключение.</param>
        /// <param name="Count">Количество символов, которое должно быть обрезано справа. Если 0, обрезание не производится и возвращается входная строка. 
        /// Если равно или больше фактической длины входной строки, возвращается пустая строка. Если меньше 0 - выбрасывается исключение.</param>
        public static StringBuilder CutRight(this StringBuilder Source, Int32 Count)
        {
            if (Source == null) { throw new ArgumentNullException("Source", "Входная строка не может быть NULL"); }
            if (Count < 0) { throw new ArgumentOutOfRangeException("Count", Count, "Значение параметра меньше 0"); }
            if (Count == 0) { return Source; }
            Int32 length = Source.Length;
            if (length == 0) { return Source; }
            if (Count >= length)
            {
                Source.Clean();
                return Source;
            }
            Source.Remove((length - Count), Count);
            return Source;
        }

        /// <summary>
        /// Удаляет из изменяемой строки все начальные и конечные пробелы и записывает изменение в этот же экземпляр
        /// </summary>
        /// <param name="Source">Входная изменяемая строка. Если NULL, будет выброшено исключение. Если пустая, будет возвращена без изменений.</param>
        /// <returns></returns>
        public static StringBuilder Trim(this StringBuilder Source)
        {
            if (Source == null) { throw new ArgumentNullException("Source", "Входная строка не может быть NULL"); }
            Int32 source_len = Source.Length;
            if (source_len < 1) { return Source; }
            Int32 chars_to_remove_from_start = 0;
            for (Int32 i = 0; i < source_len; i++)
            {
                if (Char.IsWhiteSpace(Source[i]) == true)
                {
                    ++chars_to_remove_from_start;
                }
                else
                {
                    break;
                }
            }
            if (chars_to_remove_from_start == source_len)
            {
                Source.Clean();
                return Source;
            }
            Int32 chars_to_remove_from_end = 0;
            for (Int32 i = source_len - 1; i > chars_to_remove_from_start; i--)
            {
                if (Char.IsWhiteSpace(Source[i]) == true)
                {
                    ++chars_to_remove_from_end;
                }
                else
                {
                    break;
                }
            }
            if (chars_to_remove_from_start > 0)
            {
                Source.Remove(0, chars_to_remove_from_start);
            }
            if (chars_to_remove_from_end > 0)
            {
                Source.Remove(Source.Length - chars_to_remove_from_end, chars_to_remove_from_end);
            }
            return Source;
        }

        /// <summary>
        /// Обрезает спереди (слева) изменяемую строку в случае, если она начинается с указанной подстроки, и записывает изменение в этот же экземпляр. 
        /// Параметры задают учёт регистра литер и необходимость рекурсивного обрезания.
        /// </summary>
        /// <param name="Source">Входная изменяемая строка, обрезанную слева часть которой следует возвратить. 
        /// Если пустая - будет возвращена без изменений. Если NULL, будет выброшено исключение.</param>
        /// <param name="Start">Строка, равная подстроке, с которой может начинаться входная изменяемая строка и которую следует обрезать. 
        /// Если NULL, пустая, или больше длины входной строки, будет возвращена входная строка без изменений.</param>
        /// <param name="IgnoreCase">Определяет, игнорировать ли при сравнении символов регистр литер. true - игнорировать, false - принимать во внимание.</param>
        /// <param name="Recursive">Определяет, необходимо ли после первой обрезки проанализировать начало строки на совпадение ещё раз</param>
        /// <returns></returns>
        public static StringBuilder TrimStart(this StringBuilder Source, String Start, Boolean IgnoreCase, Boolean Recursive)
        {
            if (Source == null) { throw new ArgumentNullException("Source", "Входная строка не может быть NULL"); }
            Int32 source_len = Source.Length;
            if (source_len == 0 || Start.IsStringNullOrEmpty() == true || Start.Length > source_len) { return Source; }

            Boolean fail_found = false;
            Int32 coincidences_found = 0;
            Char ch_source;
            Char ch_start;
            Int32 source_index = 0;
            do
            {
                for (Int32 start_index = 0; start_index < Start.Length && source_len - source_index >= Start.Length; start_index++)
                {
                    ch_source = Source[source_index];
                    ch_start = Start[start_index];

                    if (CommonTools.AreCharsEqual(ch_source, ch_start, IgnoreCase) == true)
                    {
                        ++source_index;
                        continue;
                    }
                    else
                    {
                        fail_found = true;
                        break;
                    }
                }
                if (fail_found == false)
                {
                    ++coincidences_found;
                }

            } while (Recursive == true && fail_found == false && source_len - source_index >= Start.Length);

            if (coincidences_found == 0)
            { return Source; }
            else
            {
                Source.Remove(0, Start.Length * coincidences_found);
                return Source;
            }
        }

        /// <summary>
        /// Обрезает сзади (справа) изменяемую строку в случае, если она заканчивается указанной подстрокой, и записывает изменение в этот же экземпляр. 
        /// Параметры задают учёт регистра литер и необходимость рекурсивного обрезания.
        /// </summary>
        /// <param name="Source">Входная изменяемая строка, обрезанную справа часть которой следует возвратить. 
        /// Если пустая - будет возвращена без изменений. Если NULL, будет выброшено исключение.</param>
        /// <param name="End">Строка, равная подстроке, которой может оканчиваться входная изменяемая строка и которую следует обрезать. 
        /// Если NULL, пустая, или больше длины входной строки, будет возвращена входная строка без изменений.</param>
        /// <param name="IgnoreCase">Определяет, игнорировать ли при сравнении символов регистр литер. true - игнорировать, false - принимать во внимание.</param>
        /// <param name="Recursive">Определяет, необходимо ли после первой обрезки проанализировать конец строки на совпадение ещё раз</param>
        /// <returns></returns>
        public static StringBuilder TrimEnd(this StringBuilder Source, String End, Boolean IgnoreCase, Boolean Recursive)
        {
            if (Source == null) { throw new ArgumentNullException("Source", "Входная строка не может быть NULL"); }
            Int32 source_len = Source.Length;
            if (source_len == 0 || End.IsStringNullOrEmpty() == true || End.Length > source_len) { return Source; }

            Boolean fail_found = false;
            Int32 coincidences_found = 0;
            Char ch_source;
            Char ch_end;
            Int32 source_index = Source.Length - 1;
            do
            {
                for (Int32 end_index = End.Length - 1; end_index >= 0 && source_index + 1 >= End.Length; end_index--)
                {
                    ch_source = Source[source_index];
                    ch_end = End[end_index];

                    if (CommonTools.AreCharsEqual(ch_source, ch_end, IgnoreCase) == true)
                    {
                        --source_index;
                        continue;
                    }
                    else
                    {
                        fail_found = true;
                        break;
                    }
                }
                if (fail_found == false)
                {
                    ++coincidences_found;
                }

            } while (Recursive == true && fail_found == false && source_index + 1 >= End.Length);

            if (coincidences_found == 0)
            { return Source; }
            else
            {
                Source.Remove(source_len - coincidences_found * End.Length, coincidences_found * End.Length);
                return Source;
            }
        }

        /// <summary>
        /// Обращает порядок символов в изменяемой строке на противоположный и записывает изменение в этот же экземпляр.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns>Изменённый входной экземпляр</returns>
        public static StringBuilder Reverse(this StringBuilder Source)
        {
            if (Source == null) { return Source; }
            Int32 len = Source.Length;
            if (len <= 1) { return Source; }
            Int32 reminder;
            Int32 half = Math.DivRem(len, 2, out reminder);
            Char buffer;
            for (Int32 i = 0, j = len - 1; i < half; i++, j--)
            {
                buffer = Source[j];
                Source[j] = Source[i];
                Source[i] = buffer;
            }
            return Source;
        }

        /// <summary>
        /// Удаляет из изменяемой строки указанное количество символов, начиная с указанной позиции, и вставляет на их место указанную строку-заменитель
        /// </summary>
        /// <param name="Source">Входная изменяемая строка, в которой производится удаление и вставка. Если NULL - будет выброшено исключение. 
        /// Если пустая и указанная начальная позиция равна 0, в неё будет записано содержимое строки Replacement без изменений</param>
        /// <param name="StartIndex">Начальная позиция (индекс) во входной строке, с которой необходимо произвести удаление символов и на которую необходимо вставить 
        /// указанную строку-заменитель. Начинается с 0. Если значение больше фактической длины (не ёмкости) входной строки или меньше 0, выбрасывается исключение.</param>
        /// <param name="Length">Количество символов, которые необходимо удалить из входной строки. Если равно 0 - не будет удалён ни один символ, 
        /// а строка-заменитель просто внедрится во входную по указанной позиции. Если больше длины исходной строки с учётом смещения, указанного начальной позицией, 
        /// то удалится вся часть входной строки вплоть до конца. Если меньше 0 - будет выброшено исключение.</param>
        /// <param name="Replacement">Строка-заменитель, которая должна внедриться во входную строку. Если NULL - будет выброшено исключение. 
        /// Если пустая - будет произведено удаление символов, но вставки не будет.</param>
        /// <returns></returns>
        public static StringBuilder Stuff(this StringBuilder Source, Int32 StartIndex, Int32 Length, String Replacement)
        {
            if (Source == null) { throw new ArgumentNullException("Source", "Входная строка не может быть NULL"); }
            if (Replacement == null) { throw new ArgumentNullException("Replacement", "Указанная строка-заменитель не может быть NULL"); }
            if (StartIndex < 0) { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Стартовая позиция меньше 0"); }
            if (Length < 0) { throw new ArgumentOutOfRangeException("Length", Length, "Количество символов для удаления меньше 0"); }
            Int32 source_length = Source.Length;
            if (source_length == 0 && StartIndex == 0)
            {
                Source.Append(Replacement);
                return Source;
            }
            if (StartIndex >= source_length)
            {
                throw new ArgumentOutOfRangeException("StartIndex", StartIndex,
                  String.Format("Указанная стартовая позиция ('{0}') больше, чем количество символов в строке ('{1}')", StartIndex, source_length));
            }
            Int32 replacement_length = Replacement.Length;
            if (replacement_length > 0)
            {
                Source.Insert(StartIndex, Replacement, 1);
            }
            if (Length > 0)
            {
                if (StartIndex + Length > source_length)
                {
                    Length = source_length - StartIndex;
                }
                Source.Remove(StartIndex + replacement_length, Length);
            }
            return Source;
        }
    }
}

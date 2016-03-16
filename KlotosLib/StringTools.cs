#define Debug

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace KlotosLib
{
    /// <summary>
    /// Содержит статические методы по работе с классическими и изменяемыми строками, не являющиеся методами расширения
    /// </summary>
    public static class StringTools
    {
        /// <summary>
        /// Определяет направление поиска или выдачи подстроки в строке
        /// </summary>
        public enum Direction : byte
        {
            /// <summary>
            /// Искать/выдавать в направлении от начала до конца строки
            /// </summary>
            FromStartToEnd,

            /// <summary>
            /// Искать/выдавать в направлении с конца к началу строки
            /// </summary>
            FromEndToStart
        };

        /// <summary>
        /// Содержит статические методы, возвращающие те или иные данные по переданным им строкам
        /// </summary>
        public static class StringAnalyzers
        {
            /// <summary>
            /// Типы символов, которые могут содержаться в строке и содержание которых может определяться методами
            /// </summary>
            [Flags]
            public enum ContainsEntities : byte
            {
                /// <summary>
                /// Пустая строка или null, значение по умолчанию
                /// </summary>
                Empty = 0x00,

                /// <summary>
                /// Содержатся пробелы
                /// </summary>
                Spaces = 0x01,

                /// <summary>
                /// Содержатся управляющие символы
                /// </summary>
                Controls = 0x02,

                /// <summary>
                /// Содержатся буквы алфавита
                /// </summary>
                Letters = 0x04,

                /// <summary>
                /// Содержатся арабские цифры
                /// </summary>
                Digits = 0x08
            }

            /// <summary>
            /// Определяет типы символов, которые содержатся в указанной строке, и возвращает их в виде перечисления с множеством флагов
            /// </summary>
            /// <param name="Input">Входная строка с любым значением</param>
            /// <returns></returns>
            public static ContainsEntities DefineContainingSymbols(String Input)
            {
                if (String.IsNullOrEmpty(Input) == true) { return ContainsEntities.Empty; }

                ContainsEntities output = ContainsEntities.Empty;
                for (Int32 i = 0; i < Input.Length; i++)
                {
                    Char one = Input[i];
                    if (Char.IsControl(one) == true)
                    {
                        output = output.IncludeTo(ContainsEntities.Controls);
                    }
                    else if (Char.IsLetter(one) == true)
                    {
                        output = output.IncludeTo(ContainsEntities.Letters);
                    }
                    else if (Char.IsDigit(one) == true)
                    {
                        output = output.IncludeTo(ContainsEntities.Digits);
                    }
                    else if (Char.IsWhiteSpace(one) == true)
                    {
                        output = output.IncludeTo(ContainsEntities.Spaces);
                    }
                }
                return output;
            }

            /// <summary>
            /// Определяет символ, с которого начинается указанная строка, и возвращает его вместе с количеством его экземпляров, идущих подряд с начала строки
            /// </summary>
            /// <example>Для строки "xxxyyyy" метод возвратит 'x:3'</example>
            /// <param name="Input">Входная строка. Если NULL или пустая, будет выброшено исключение.</param>
            /// <returns>Пара, где ключ - первый символ, а значение - количество идущих подряд экземпляров этого символа</returns>
            /// <exception cref="ArgumentException">Входная строка NULL или не содержит ни одного символа</exception>
            public static KeyValuePair<Char, Int32> GetStartChars(String Input)
            {
                if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Input"); }
                Char first = '\0';
                Int32 count = 0;
                for (Int32 i = 0; i < Input.Length; i++)
                {
                    if (i == 0)
                    {
                        first = Input[i];
                        count = 1;
                        continue;
                    }
                    else if (i > 0 && Input[i].Equals(first) == true)
                    {
                        count++;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                return new KeyValuePair<char, int>(first, count);
            }

            /// <summary>
            /// Возвращает количество идущего подряд с начала строки указанного символа
            /// </summary>
            /// <example>Для строки "xxxyyyy" и символа 'x' метод возвратит 3, а для любого другого символа - 0</example>
            /// <param name="Input">Строка, с начала которой необходимо искать указанный символ. Если NULL или пустая, будет выброшено исключение.</param>
            /// <param name="Start">Искомый символ, количество вхождений подряд которого следует возвратить</param>
            /// <returns></returns>
            /// <exception cref="ArgumentException">Входная строка NULL или не содержит ни одного символа</exception>
            public static Int32 StartWithCount(String Input, Char Start)
            {
                if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Input"); }
                KeyValuePair<Char, Int32> start_signature = StringAnalyzers.GetStartChars(Input);
                if (start_signature.Key.Equals(Start) == true) { return start_signature.Value; }
                return 0;
            }

            /// <summary>
            /// Перечисление, которое обозначает возможное равенство, неравенство и вхождение двух строк между собой
            /// </summary>
            public enum SubstringLocation : byte
            {
                /// <summary>
                /// Строки идентичны
                /// </summary>
                Identical = 0,

                /// <summary>
                /// Первая строка является частью второй (входит во вторую как подстрока)
                /// </summary>
                FirstInSecond = 1,

                /// <summary>
                /// Вторая строка является частью первой (входит в первую как подстрока)
                /// </summary>
                SecondInFirst = 2,

                /// <summary>
                /// Строки разные и ни одна из них не входит в другую
                /// </summary>
                Different = 3
            }

            /// <summary>
            /// Сравнивает две указанные строки в соответствии с указанными опциями сравнения, и определяет, 
            /// являются ли строки равные, а если нет, то является ли одна из них частью другой. 
            /// Возвращает один из четырёх пунктов перечисления.
            /// </summary>
            /// <remarks>Бессмертный метод</remarks>
            /// <param name="First">Первая строка</param>
            /// <param name="Second">Вторая строка</param>
            /// <param name="Options">Опции поиска и сравнения</param>
            /// <returns>Один пункт специального перечисления</returns>
            public static SubstringLocation FindAppearanceBetweenStrings(String First, String Second, StringComparison Options)
            {
                if (
                    (First == null && Second == null) ||
                    (Object.ReferenceEquals(First, Second) == true) ||
                    (First == String.Empty & Second == String.Empty) ||
                    (String.Compare(First, Second, Options) == 0)
                    )
                {
                    return SubstringLocation.Identical;
                }
                if (
                    (First == null) ||
                    (Second == null) ||
                    (First == String.Empty) ||
                    (Second == String.Empty)
                    )
                {
                    return SubstringLocation.Different;
                }
                Int32 second_in_first = First.IndexOf(Second, Options);
                Int32 first_in_second = Second.IndexOf(First, Options);
                if (second_in_first >= 0)
                {
                    return SubstringLocation.SecondInFirst;
                }
                if (first_in_second >= 0)
                {
                    return SubstringLocation.FirstInSecond;
                }
                if (second_in_first == -1 && first_in_second == -1)
                {
                    return SubstringLocation.Different;
                }
                throw new InvalidOperationException("second_in_first=" + second_in_first + "; first_in_second=" + first_in_second);
            }

            /// <summary>
            /// Возвращает количество всех различающихся символов в указанной строке
            /// </summary>
            /// <param name="Input"></param>
            /// <returns>Если строка является NULL или пустой - возвращает 0, иначе число, большее за 0</returns>
            public static Int32 GetNumberOfDistinctSymbolsInString(String Input)
            {
                if (Input.IsStringNullOrEmpty() == true)
                {
                    return 0;
                }
                HashSet<Char> h = new HashSet<char>(Input);
                return h.Count;
            }

            /// <summary>
            /// Возвращает количество цифр, которые присутствуют во входной строке
            /// </summary>
            /// <param name="Input"></param>
            /// <returns></returns>
            public static UInt16 GetNumberOfDigits(String Input)
            {
                if (Input.HasVisibleChars() == false) { return 0; }
                UInt16 digits_count = 0;
                foreach (Char c in Input)
                {
                    if (Char.IsDigit(c) == true) { digits_count++; }
                }
                return digits_count;
            }

            /// <summary>
            /// Возвращает количество вхождений указанной искомой подстроки во входной строке
            /// </summary>
            /// <param name="Input">Строка, в которой ищутся вхождения подстроки. Если NULL или пустая - генерируется исключение.</param>
            /// <param name="Seek">Искомая подстрока, вхождения которой ищутся в строке</param>
            /// <param name="CompareOption">Опции поиска подстроки</param>
            /// <returns></returns>
            public static Int32 GetNumberOfOccurencesInString(String Input, String Seek, StringComparison CompareOption)
            {
                if (Input.IsStringNullEmptyWhiteSpace() == true) throw new ArgumentException("Input string must have a correct value", "Input");
                if (String.IsNullOrEmpty(Seek) == true) throw new ArgumentException("Seeking string must not be NULL or empty", "Seek");
                
                Int32 seek_len = Seek.Length;
                Int32 result = 0;
                Int32 start_index = 0;
                while (true)
                {
                    Int32 current_index = Input.IndexOf(Seek, start_index, CompareOption);
                    if (current_index < 0)
                    {
                        break;
                    }
                    else
                    {
                        result++;
                        start_index = current_index + seek_len;
                    }
                }
                return result;
            }

            /// <summary>
            /// Возвращает количество вхождений указанного искомого символа во входной строке
            /// </summary>
            /// <param name="Input">Входная строка. Если NULL или пустая - генерируется исключение.</param>
            /// <param name="Seek">Искомый символ, который ищется во входной строке.</param>
            /// <returns></returns>
            public static Int32 GetNumberOfOccurencesInString(String Input, Char Seek)
            {
                if (Input.IsStringNullOrEmpty() == true) throw new ArgumentException("Входная строка не может быть NULL или пустой", "Input");
                Char[] input_array = Input.ToCharArray();

                Int32 result = 0;
                foreach (char c in input_array)
                {
                    if (c == Seek)
                    {
                        result++;
                    }
                }
                return result;
            }

            /// <summary>
            /// Возвращает список всех индексов (позиций) вхождения указанной подстроки в указанной входной строке, начиная с указанной позиции и используя указанный метод сравнения строк.
            /// </summary>
            /// <param name="Input">Входная строка, в которой ищутся подстроки. Если NULL или пустая - выбрасывается исключение.</param>
            /// <param name="Target">Целевая подстрока, позиции вхождения которой ищутся во входной строке. Если NULL или пустая - выбрасывается исключение.</param>
            /// <param name="CompareOption">Опция сравнения строк между собой.</param>
            /// <param name="StartIndex">Начальный индекс (начиная с 0 включительно) во входной строке, с которого включительно начинается поиск подстроки. 
            /// Если меньше 0 или больше фактической длины строки - выбрасывается исключение.</param>
            /// <returns></returns>
            public static List<Int32> AllIndexesOf(String Input, String Target, StringComparison CompareOption, Int32 StartIndex)
            {
                if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка NULL или пустая", "Input"); }
                if (Target.IsStringNullOrEmpty() == true) { throw new ArgumentException("Целевая подстрока NULL или пустая", "Target"); }
                if (StartIndex < 0)
                { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Начальный индекс не может быть отрицательным"); }
                if (StartIndex >= Input.Length)
                { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Начальный индекс = " + StartIndex + ", тогда как строка содержит " + Input.Length + " символов"); }
                Int32 target_length = Target.Length;
                List<Int32> output = new List<int>();
                Int32 iter_start_index = StartIndex;
                for (Int32 i = iter_start_index; i < Input.Length; i++)
                {
                    Int32 pos = Input.IndexOf(Target, iter_start_index, CompareOption);
                    if (pos == -1) { return output; }
                    iter_start_index = pos + target_length;
                    output.Add(pos);
                }
                return output;
            }

            /// <summary>
            /// Возвращает список всех индексов (позиций) вхождений указанного целевого символа в указанной входной строке, начиная поиск с указанной позиции
            /// </summary>
            /// <param name="Input">Входная строка, в которой ищутся символы. Если NULL или пустая - выбрасывается исключение.</param>
            /// <param name="Target">Целевой символ, индексы (позиции) вхождения которого ищутся во входной строке.</param>
            /// <param name="StartIndex">Начальный индекс (начиная с 0 включительно) во входной строке, с которого включительно начинается поиск подстроки. 
            /// Если меньше 0 или больше фактической длины строки - выбрасывается исключение.</param>
            /// <returns></returns>
            public static List<Int32> AllIndexesOf(String Input, Char Target, Int32 StartIndex)
            {
                if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка NULL или пустая", "Input"); }
                if (StartIndex < 0)
                { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Начальный индекс не может быть отрицательным"); }
                if (StartIndex >= Input.Length)
                { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Начальный индекс = " + StartIndex + ", тогда как строка содержит " + Input.Length + " символов"); }
                List<Int32> output = new List<int>();
                Int32 first = Input.IndexOf(Target, StartIndex);
                if (first == -1) {return output;}
                output.Add(first);
                for (Int32 i = StartIndex + 1; i < Input.Length; i++)
                {
                    if (Input[i] == Target)
                    {
                        output.Add(i);
                    }
                }
                return output;
            }

            /// <summary>
            /// Возвращает статистику по количеству вхождений символов в указанной строке
            /// </summary>
            /// <param name="Input">Входная строка. Если NULL или пустая - будет возвращён пустой словарь</param>
            /// <returns>Словарь, где ключ - это символ, а значение - количество его вхождений в строке. Значение всегда больше 0.</returns>
            public static Dictionary<Char, UInt16> GetCharOccurencesStats(String Input)
            {
                if (Input.IsStringNullOrEmpty() == true) { return new Dictionary<char, ushort>(); }

                Dictionary<Char, UInt16> output = new Dictionary<char, ushort>();
                Char[] str_arr = Input.ToCharArray();
                for (int i = 0; i < str_arr.Length; i++)
                {
                    if (output.ContainsKey(str_arr[i]) == true)
                    {
                        output[str_arr[i]]++;
                    }
                    else
                    {
                        output.Add(str_arr[i], 1);
                    }
                }
                return output;
            }

            /// <summary>
            /// Возвращает одно ближайшее целое положительное число, извлечённое из начала или конца входной строки. 
            /// В зависимости от параметра RaiseException, если входная строка некорректная или в ней не найдено ни одной цифры, метод выбрасывает исключение или возвращает NULL.
            /// </summary>
            /// <param name="Input">Входная строка</param>
            /// <param name="Dir">Направление, по которому ищется число: с начала входной строки или с конца</param>
            /// <param name="RaiseException">Если true - при некорректной строке или ненахождении числа выбрасывается исключение; если false - возвращается NULL</param>
            /// <returns>Целое положительное число или NULL (если RaiseException = false)</returns>
            /// <exception cref="ArgumentException">Параметр RaiseException выставлен в true и входня строка пуста или не содержит ни одной цифры</exception>
            /// <exception cref="InvalidEnumArgumentException">Значение перечисления направления находится некорректно</exception>
            public static Nullable<UInt32> GetNearestUnsignedIntegerFromString(String Input, Direction Dir, Boolean RaiseException)
            {
                if (Input.HasVisibleChars() == false)
                {
                    if (RaiseException == true)
                    { throw new ArgumentException("Входная строка не может быть NULL, пустой или не содержать ни одного видимого символа", "Input"); }
                    else
                    { return null; }
                }
                Char[] input_array = Input.ToCharArray();

                Boolean found = false;
                String number = String.Empty;
                switch (Dir)
                {
                    case Direction.FromStartToEnd:
                        foreach (Char symbol in input_array)
                        {
                            if (Char.IsDigit(symbol) == true)
                            {
                                number = number + symbol.ToString();
                                found = true;
                            }
                            else if (Char.IsDigit(symbol) == false && found == true)
                            {
                                break;
                            }
                        }
                        {
                            UInt32 result;
                            if (UInt32.TryParse(number, out result) == true)
                            { return result; }
                            else
                            {
                                if (RaiseException == true)
                                { throw new ArgumentException("Входная строка не содержит ни одной цифры", "Input"); }
                                else
                                { return null; }
                            }
                        }

                    case Direction.FromEndToStart:
                        for (Int32 i = input_array.Length - 1; i >= 0; i--)
                        {
                            if (Char.IsDigit(input_array[i]) == true)
                            {
                                number = input_array[i] + number;
                                found = true;
                            }
                            else if (Char.IsDigit(input_array[i]) == false && found == true)
                            {
                                break;
                            }
                        }
                        {
                            UInt32 result;
                            if (UInt32.TryParse(number, out result) == true)
                            { return result; }
                            else
                            {
                                if (RaiseException == true)
                                { throw new ArgumentException("Входная строка не содержит ни одной цифры", "Input"); }
                                else
                                { return null; }
                            }
                        }
                    default:
                        throw new InvalidEnumArgumentException("Dir", (Int32)Dir, Dir.GetType());
                }
            }

            /// <summary>
            /// Возвращает одно ближайшее целое положительное число, извлечённое из подстроки, которая вычленяется из входной строки из указанной позиции. 
            /// Если входная строка не содержит цифр вообще, возвращает NULL.
            /// </summary>
            /// <param name="Input">Входная строка</param>
            /// <param name="StartPosition">Позиция (индекс, начинается с 0), начиная с которой, в исходной строке следует проводить поиск. Если равно 0, в поиске участвует вся исходная строка</param>
            /// <param name="FindPosition">Выводной параметр. Позиция (индекс, начинается с 0) в исходной строке, в которой обнаружилось начало первого попавшегося числа. Если искомое число не обнаружилось, возвращает -1.</param>
            /// <returns></returns>
            public static Nullable<UInt32> GetNearestUnsignedIntegerFromString(String Input, Int32 StartPosition, out Int32 FindPosition)
            {
                if (Input.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input"); }
                Int32 input_length = Input.Length;
                Char[] input_array_cropped = Input.ToCharArray(StartPosition, (input_length - StartPosition));

                FindPosition = -1;
                bool found = false;
                String number = string.Empty;
                for (Int32 i = 0; i < input_array_cropped.Length; i++)
                {
                    if (Char.IsDigit(input_array_cropped[i]) == true)
                    {
                        number = number + input_array_cropped[i].ToString();
                        found = true;
                        if (FindPosition == -1) { FindPosition = StartPosition + i; }
                    }
                    else if (Char.IsDigit(input_array_cropped[i]) == false && found == true)
                    {
                        break;
                    }
                }
                if (number == string.Empty)
                {
                    return null;
                }
                else
                {
                    UInt32 output = UInt32.Parse(number);
                    if (output != 0)
                    {
                        foreach (char c in number.ToCharArray())
                        {
                            if (c == '0') { FindPosition++; }
                            else { break; }
                        }
                    }
                    return output;
                }
            }

            /// <summary>
            /// Возвращает список позиций (индексов) всех вхождений указанного токена в указанной строке
            /// </summary>
            /// <param name="Input">Строка, в которой ищутся вхождения подстроки (токена)</param>
            /// <param name="Token">Подстрока (токен), вхождения которой ищутся в строке</param>
            /// <param name="CompareOption">Опция сравнения строк между собой</param>
            /// <returns></returns>
            public static List<Int32> GetPositionsOfTokenInString(String Input, String Token, StringComparison CompareOption)
            {
                if (Input.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input"); }
                if (Token.IsStringNullOrEmpty() == true) { throw new ArgumentException("Токен не может быть NULL или пустой строкой", "Token"); }
                if (String.Compare(Input, Token, CompareOption) == 0) { throw new ArgumentException("Входная строка не может быть равна токену"); }
                if (Input.Contains(Token, CompareOption) == false) { throw new ArgumentException("Токен не найден во входной строке", "Token"); }
                UInt32 backup_count = 0;
                List<Int32> output = new List<int>();

                Int32 start_index = -1;
                while (true)
                {

                    Int32 current_index = Input.IndexOf(Token, start_index + 1, CompareOption);
                    if (current_index < 0)
                    {
                        break;
                    }
                    else
                    {
                        start_index = current_index;
                        output.Add(current_index);
                    }
                    //backup
                    backup_count++;
                    if (backup_count == UInt32.MaxValue)
                    {
                        throw new InvalidOperationException(String.Format("Предположительно вечный цикл: количество итераций достигло {0} при длине входной строки в {1} символов.",
                          UInt32.MaxValue, Input.Length));
                    }
                }
                return output;
            }

            /// <summary>
            /// Возвращает индексы начала и конца первого вхождения указанного шаблона во входящей строке, начиная поиск с указанного индекса. 
            /// Подстрока считается совпавшей с шаблоном тогда и только тогда, когда она начинается с начального токена, заканчивается конечным токеном и 
            /// содержит между ними лишь и только все указанные внутренние символы. Если между начальным и конечным токенами содержатся 
            /// какие-либо другие символы, или хотя бы один из указанных внутренних символов не содержится между начальным и конечным, то такая подстрока отбраковывается.
            /// </summary>
            /// <param name="Input">Входная строка, в которой происходит поиск шаблона. Если NULL или пустая, будет выброшено исключение.</param>
            /// <param name="StartToken">Начальный токен, которым должен начинаться искомый шаблон. Если NULL или пустой, будет выброшено исключение.</param>
            /// <param name="EndToken">Конечный токен, которым должен заканчиваться искомый шаблон. Если NULL или пустой, будет выброшено исключение.</param>
            /// <param name="StartIndex">Позиция (индекс), с которой включительно начинается анализ строки. Если меньше 0, будет приведён к 0. 
            /// Если больше длины строки, будет выброшено исключение.</param>
            /// <param name="InnerSymbols">Символы, из которых состоит "промежность" искомого шаблона между начальными и конечными токенами. 
            /// Чтобы искомый шаблон считался валидным, его "промежность" должна состоять лишь и только из всех указанных символов. 
            /// Дублирующие символы игнорируются, учитываются только уникальные. Если массив является NULL или пуст, будет выброшено исключение.</param>
            /// <returns>Связка из индекса начала и индекса конца первого вхождения указанного шаблона во входящей строке, начиная с 0 включительно. Если шаблон не найден, возвращает два -1.</returns>
            public static KeyValuePair<Int32, Int32> IndexesOfTemplateFirstOccurence(String Input, String StartToken, String EndToken, Int32 StartIndex, params Char[] InnerSymbols)
            {
                if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Input"); }
                if (StartToken.IsStringNullOrEmpty() == true) throw new ArgumentException("Начальный токен не может быть NULL или пустой строкой", "StartToken");
                if (EndToken.IsStringNullOrEmpty() == true) throw new ArgumentException("Конечный токен не может быть NULL или пустой строкой", "EndToken");
                if (String.Compare(StartToken, EndToken, StringComparison.InvariantCultureIgnoreCase) == 0)
                    throw new ArgumentException("Начальный и конечный токены должны быть разными, не считая различий в регистре");
                if (Input.Contains(StartToken) == false) throw new ArgumentException("Начальный токен не содержится в искомой строке", "StartToken");
                if (Input.Contains(EndToken) == false) throw new ArgumentException("Конечный токен не содержится в искомой строке", "EndToken");
                if (InnerSymbols.IsNullOrEmpty() == true) { throw new ArgumentException("Массив с внутренними символами является NULL или пуст", "InnerSymbols"); }
                if (StartIndex >= Input.Length) { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, String.Format("Позиция начала поиска '{0}' превышает длину строки '{1}'", StartIndex, Input.Length)); }
                if (StartIndex < 0) { StartIndex = 0; }
                Int32 start_token_length = StartToken.Length;
                Int32 end_token_length = EndToken.Length;

                Int32 offset = StartIndex;
                Int32 start_index;
                Int32 end_index;
                String temp_substring;
                while (true)
                {
                    start_index = Input.IndexOf(StartToken, offset, StringComparison.InvariantCultureIgnoreCase);
                    if (start_index < 0) { break; }
                    offset = start_index + start_token_length;
                    end_index = Input.IndexOf(EndToken, offset, StringComparison.InvariantCultureIgnoreCase);
                    if (end_index < 0) { break; }
                    temp_substring = Input.SubstringWithEnd(start_index + start_token_length, end_index);
                    if (StringTools.ContainsHelpers.ContainsAllAndOnlyOf(temp_substring, InnerSymbols) == true)
                    {
                        return new KeyValuePair<int, int>(start_index, end_index + end_token_length);
                    }
                }
                return new KeyValuePair<int, int>(-1, -1);
            }

            /// <summary>
            /// Возвращает набор индексов начала и конца всех вхождений искомого шаблона в указанной строке, начиная поиск с указанного индекса. 
            /// Подстрока считается совпавшей с шаблоном тогда и только тогда, если начинается с указанного токена, оканчивается указанным токеном, и между токенами 
            /// содержит указанные обязательные и необязательные символы. Если между токенами содержатся какие-либо другие символы, 
            /// или же не содержится хотя бы одного из обязательных символов, то такая подстрока отбраковывается.
            /// </summary>
            /// <param name="Input">Входная строка, в которой происходит поиск шаблона. Если NULL или пустая, будет выброшено исключение.</param>
            /// <param name="StartToken">Начальный токен, которым должен начинаться искомый шаблон. Если NULL или пустой, будет выброшено исключение.</param>
            /// <param name="EndToken">Конечный токен, которым должен заканчиваться искомый шаблон. Если NULL или пустой, будет выброшено исключение.</param>
            /// <param name="StartIndex">Позиция (индекс), с которой включительно начинается анализ строки. Если меньше 0, будет приведён к 0. 
            /// Если больше длины строки, будет выброшено исключение.</param>
            /// <param name="RequiredInnerSymbols">Массив обязательных символов, все из которых должны присутствовать между начальным и конечным токеном. 
            /// Дублирующие символы игнорируются, учитываются только уникальные. Если NULL или пустой, предполагается, что обязательных символов нет.</param>
            /// <param name="OptionalInnerSymbols">Массив допустимых символов, которые могут, но не обязательны присутствовать между начальным и конечным токеном. 
            /// Дублирующие символы игнорируются, учитываются только уникальные. Если NULL или пустой, предполагается, что обязательных символов нет.</param>
            /// <param name="CompareOptions">Опции сравнения строк между собой</param>
            /// <returns>Словарь, где один элемент представляет индексы одного найденного токена: ключ содержит индекс начала токена, а значение - индекс его конца. 
            /// Если ни одного токена не найдено, возвращается пустой словарь.</returns>
            public static Dictionary<Int32, Int32> IndexesOfTemplate(String Input, String StartToken, String EndToken, Int32 StartIndex,
                Char[] RequiredInnerSymbols, Char[] OptionalInnerSymbols, StringComparison CompareOptions)
            {
                if (Input.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "Input"); }
                if (StartToken.IsStringNullOrEmpty() == true) throw new ArgumentException("Начальный токен не может быть NULL или пустой строкой", "StartToken");
                if (EndToken.IsStringNullOrEmpty() == true) throw new ArgumentException("Конечный токен не может быть NULL или пустой строкой", "EndToken");
                if (StartIndex >= Input.Length) { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, String.Format("Позиция начала поиска '{0}' превышает длину строки '{1}'", StartIndex, Input.Length)); }
                if (StartIndex < 0) { StartIndex = 0; }
                Int32 comp_value = (Int32)CompareOptions;
                if (comp_value < 0 || comp_value > 5) { throw new InvalidEnumArgumentException("CompareOptions", comp_value, typeof(StringComparison)); }
                if (RequiredInnerSymbols.IsNullOrEmpty() == true && OptionalInnerSymbols.IsNullOrEmpty() == true)
                {
                    throw new ArgumentException("Массивы обязательных и допустимых символов не могут быть одновременно NULL или пустыми. " +
                      String.Format("RequiredInnerSymbols: {0}, OptionalInnerSymbols: {1}.", RequiredInnerSymbols.ToStringS("NULL", "пустой"), OptionalInnerSymbols.ToStringS("NULL", "пустой")));
                }
                Dictionary<Int32, Int32> output = new Dictionary<int, int>();

                Int32 start_token_length = StartToken.Length;
                Int32 end_token_length = EndToken.Length;
                HashSet<Char> unique_RequiredInnerSymbols = RequiredInnerSymbols.IsNullOrEmpty() == true ? new HashSet<Char>() : new HashSet<Char>(RequiredInnerSymbols);
                HashSet<Char> unique_OptionalInnerSymbols = OptionalInnerSymbols.IsNullOrEmpty() == true ? new HashSet<Char>() : new HashSet<Char>(OptionalInnerSymbols);
                Int32 offset = StartIndex;
                Dictionary<Char, Boolean> temp_dict = unique_RequiredInnerSymbols.Any() == true ? unique_RequiredInnerSymbols.ToDictionary((Char item) => item, (Char item) => false) : null;

                while (true)
                {
                    Int32 start_index = Input.IndexOf(StartToken, offset, CompareOptions);
                    if (start_index < 0) { break; }
                    offset = start_index + start_token_length;
                    Int32 end_index = Input.IndexOf(EndToken, offset, CompareOptions);
                    if (end_index < 0) { break; }
#if Debug
                    String temp_substring = Input.SubstringWithEnd(offset, end_index);
#endif
                    Boolean fail = false;
                    for (Int32 i = offset; i < end_index; i++)
                    {
                        Char ch = Input[i];

                        if (ch.IsIn(unique_RequiredInnerSymbols) == false && ch.IsIn(unique_OptionalInnerSymbols) == false)
                        {
                            fail = true;
                            break;
                        }
                        if (unique_RequiredInnerSymbols.Any() == true && ch.IsIn(unique_RequiredInnerSymbols) == true)
                        {
                            temp_dict[ch] = true;
                        }
                    }
                    if (fail == true || (temp_dict != null && temp_dict.All(item => item.Value == true) == false))
                    {
                        continue;
                    }
                    offset = end_index + end_token_length - 1;
                    output.Add(start_index, offset);

                }
                return output;
            }
        }//end of class StringAnalyzers

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

        /// <summary>
        /// Содержит статические методы, возвращающие подстроки из входящих строк
        /// </summary>
        public static class SubstringHelpers
        {
            /// <summary>
            /// Возвращает подстроку входной строки указанной длины, отсчитывая её с конца. Позволяет определить, 
            /// считывать ли строку до начала, если указанная длина превышает фактическую длину строки.
            /// </summary>
            /// <param name="Input">Входная строка, подстроку из которой слуедует возвратить</param>
            /// <param name="Length">Желаемая длина возвращаемой подстроки</param>
            /// <param name="UntilStart">Определяет поведение метода в случае, если желаемая длина больше фактической длины входной подстроки: 
            /// 'true' - возвратить входную строку; 'false' - выбросить исключение.</param>
            /// <returns></returns>
            public static String SubstringFromEnd(String Input, Int32 Length, Boolean UntilStart)
            {
                if (Input == null) { throw new ArgumentNullException("Input"); }
                if (String.IsNullOrEmpty(Input) == true) { throw new ArgumentException("Входная строка не содержит ни одного символа", "Input"); }
                if (Length < 1) { throw new ArgumentOutOfRangeException("Length", Length, "Возвращаемая длина не может быть меньше 1"); }
                if (UntilStart == false && Length > Input.Length)
                { throw new ArgumentOutOfRangeException("Length", Length, "Указанная длина = " + Length + " превышает фактическую длину строки = " + Input.Length); }
                else if (UntilStart == true && Length > Input.Length)
                {
                    return Input;
                }
                else
                {
                    String temp = Input.Substring(Input.Length - Length);
                    return temp;
                }
            }

            /// <summary>
            /// Возвращает входную строку, из которой удалены все подстроки, начинающиеся с указанного начального токена 
            /// и заканчивающиеся ближайшим указанным конечным токеном. 
            /// </summary>
            /// <param name="Input">Входящая строка, в которой происходит поиск.</param>
            /// <param name="StartToken">Начальный токен (подстрока), которым начинается удаляемая подстрока. Не может быть NULL или пустым.</param>
            /// <param name="EndToken">Конечный токен (подстрока), которым оканчивается удаляемая подстрока. Не может быть NULL или пустым.</param>
            /// <param name="RemoveTokens">Определяет, следует ли удалить начальный и конечный токены вместе с удаляемой подстрокой (true) или же их следует оставить (false)</param>
            /// <param name="CompOpt">Опции сравнения строк между собой</param>
            /// <returns>Новая строка. Если не найдено ни одной пары начальныго и конечного токенов, возвращается оригинальная строка.
            /// Если начальный и конечный токены одинаковы, или исходная строка является NULL, пустой строкой либо содержит лишь пробелы, 
            /// либо хотя бы один из токенов является NULL или пустой строкой, метод выбрасывает исключение.</returns>
            public static String RemoveFromStartToEndToken(String Input, String StartToken, String EndToken, Boolean RemoveTokens, StringComparison CompOpt)
            {
                if (Input.IsStringNullEmptyWhiteSpace() == true) throw new ArgumentException("Входная строка является NULL, пустой строкой либо состоит лишь из одних пробелов", "Input");
                if (StartToken.IsStringNullOrEmpty() == true) throw new ArgumentException("Начальный токен является NULL или пустой строкой", "StartToken");
                if (EndToken.IsStringNullOrEmpty() == true) throw new ArgumentException("Конечный токен является NULL или пустой строкой", "EndToken");
                if (String.Compare(StartToken, EndToken, CompOpt) == 0)
                {throw new ArgumentException("Начальный и конечный токены должны быть разными с учётом указнных опций сравнения");}

                Int32 current_offset = 0;
                StringBuilder sb = new StringBuilder(Input.Length);
                while (true)
                {
                    Int32 start_index = Input.IndexOf(StartToken, current_offset, CompOpt);
                    if (start_index < 0) {break;}
                    

                    Int32 end_index = Input.IndexOf(EndToken, start_index, CompOpt);
                    if(end_index < 0) {break;}

                    String slice;
                    if (RemoveTokens)
                    {
                        slice = Input.SubstringWithEnd(current_offset, start_index);
                    }
                    else
                    {
                        slice = Input.SubstringWithEnd(current_offset, start_index + StartToken.Length) + Input.Substring(end_index, EndToken.Length);
                    }

                    sb.Append(slice);
                    current_offset = end_index + EndToken.Length;
                }
                sb.Append(Input.Substring(current_offset));
                if (sb.Length == 0)
                {
                    return Input;
                }
                return sb.ToString();
            }

            /// <summary>
            /// Возвращает искомую подстроку из указанной исходной строки, которая размещена между указанным начальным и конечным токеном. 
            /// Ведёт поиск токенов от начала или он указанной начальной позиции до конца строки, и возвращает первое попавшееся совпадение. 
            /// Если указанных токенов не найдено, возвращает NULL.
            /// </summary>
            /// <param name="Input">Входная строка, содержащая токены, и внутри которой происходит поиск. Не может быть NULL, пустой или состоящей из одних пробелов.</param>
            /// <param name="StartToken">Начальный токен. Не может быть NULL или пустой строкой.</param>
            /// <param name="EndToken">Конечный токен. Не может быть NULL или пустой строкой.</param>
            /// <param name="StartIndex">Начальная позиция исходной строки, с которой включительно начинается поиск. Если 0 - поиск ведётся с начала. 
            /// Если меньше 0 или больше длины исходной строки, выбрасывается исключение.</param>
            /// <param name="EndIndex">Конечная позиция исходной строки, на которой включительно останавливается поиск и за которую не выходит. 
            /// Если больше фактической длины строки, 0 или меньше 0, поиск ведётся до конца строки. 
            /// Если меньше или равно ненулевой начальной позиции, выбрасывается исключение.</param>
            /// <param name="IncludeTokens">Определяет, следует ли включать начальный и конечный токены в возвращаемую подстроку (true) или нет (false)</param>
            /// <param name="EndTokenSearchDirection">Задаёт направление поиска конечного токена после того, как найден начальный. 
            /// FromEndToStart - поиск ведётся от самого конца входной строки и продвигается до найденного начального токена до тех пор, пока не найдёт конечный токен. 
            /// FromStartToEnd - поиск ведётся от конца найденного начального токена до конца входной строки.</param>
            /// <param name="ComparisionOptions">Опции сравнения строк между собой</param>
            /// <returns>NULL - если не нашло</returns>
            public static Substring GetInnerStringBetweenTokens
                (String Input, String StartToken, String EndToken,
                Int32 StartIndex, Int32 EndIndex, Boolean IncludeTokens, StringTools.Direction EndTokenSearchDirection, StringComparison ComparisionOptions)
            {
                if (Input.IsStringNullEmptyWhiteSpace() == true) throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input");
                if (StartToken.IsStringNullOrEmpty() == true) throw new ArgumentException("Начальный токен не может быть NULL или пустой строкой", "StartToken");
                if (EndToken.IsStringNullOrEmpty() == true) throw new ArgumentException("Конечный токен не может быть NULL или пустой строкой", "EndToken");
                if (StartIndex < 0) { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Начальная позиция не может быть меньше 0"); }
                if (StartIndex >= Input.Length)
                { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, String.Format("Начальная позиция ('{0}') не может быть больше или равна длине строки ('{1}')", StartIndex, Input.Length)); }
                if (EndIndex <= 0 || EndIndex >= Input.Length)
                {
                    EndIndex = Input.Length - 1;
                }
                if (EndIndex <= StartIndex)
                { throw new ArgumentOutOfRangeException("EndIndex", EndIndex, String.Format("Конечная позиция ('{0}') не может быть меньше или равна начальной позиции ('{1}')", EndIndex, StartIndex)); }
                if (Enum.IsDefined(typeof(StringComparison), (Int32)ComparisionOptions) == false)
                { throw new InvalidEnumArgumentException("ComparisionOptions", (Int32)ComparisionOptions, typeof(StringComparison)); }
                
                Int32 start_token_start_pos = Input.IndexOf(StartToken, StartIndex, (EndIndex - StartIndex + 1), ComparisionOptions);
                if (start_token_start_pos == -1)
                {
                    return null;
                }
                Int32 end_token_start_pos;
                if (EndTokenSearchDirection == Direction.FromStartToEnd)
                {
                    Int32 start_index = start_token_start_pos + StartToken.Length;
                    Int32 count = EndIndex - start_index + 1;
                    end_token_start_pos = Input.IndexOf(EndToken, start_index, count, ComparisionOptions);
                }
                else if (EndTokenSearchDirection == Direction.FromEndToStart)
                {
                    Int32 start_index = EndIndex;
                    Int32 count = EndIndex - start_token_start_pos + StartToken.Length;
                    end_token_start_pos = Input.LastIndexOf(EndToken, start_index, count, ComparisionOptions);
                }
                else
                {
                    throw new UnreachableCodeException();
                }
                if (end_token_start_pos == -1)
                {
                    return null;
                }
                Substring output;
                if (IncludeTokens == false)
                {
                    output = Substring.CreateWithEndIndex(Input, start_token_start_pos + StartToken.Length, end_token_start_pos - 1);
                }
                else
                {
                    output = Substring.CreateWithEndIndex(Input, start_token_start_pos, end_token_start_pos + EndToken.Length - 1);
                }
                return output;
            }
            
            /// <summary>
            /// Возвращает все искомые подстроки из указанной строки, которые находятся между указанными начальными и конечными токенами. 
            /// Поиск ведётся с начала до конца строки, начиная с указанной позиции, и возвращает все попавшиеся совпадения. 
            /// Если указанных токенов не найдено, возвращает пустой список подстрок.
            /// </summary>
            /// <param name="Input">Входная строка, содержащая токены, и внутри которой происходит поиск. Входная строка не может быть NULL, пустой или состоящей из одних пробелов.</param>
            /// <param name="StartToken">Начальный токен, не может быть NULL или пустой строкой</param>
            /// <param name="EndToken">Конечный токен, не может быть NULL или пустой строкой</param>
            /// <param name="StartIndex">Позиция (включительная) начала поиска во входной строке. Если 0 - поиск ведётся с начала. 
            /// Если меньше 0 или больше длины входной строки - выбрасывается исключение.</param>
            /// <param name="CompOpt">Опции сравнения строк между собой</param>
            /// <returns>Список подстрок. Если какая-либо из подстрок является пустой (т.е. между начальным и конечным токеном нет ни одного символа), 
            /// в списке она будет представлена значением NULL.</returns>
            public static List<Substring> GetInnerStringsBetweenTokens(String Input, String StartToken, String EndToken, Int32 StartIndex, StringComparison CompOpt)
            {
                if (Input.IsStringNullEmptyWhiteSpace() == true) 
                {throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input");}
                if (StartToken.IsStringNullOrEmpty() == true) {throw new ArgumentException("Начальный токен не может быть NULL или пустой строкой", "StartToken");}
                if (EndToken.IsStringNullOrEmpty() == true) {throw new ArgumentException("Конечный токен не может быть NULL или пустой строкой", "EndToken");}
                if (StartIndex < 0) { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Позиция начала поиска не может быть меньше 0"); }
                if (StartIndex >= Input.Length)
                { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, 
                    String.Format("Позиция начала поиска ('{0}') не может быть больше или равна длине строки ('{1}')", StartIndex, Input.Length)); }
                if (Enum.IsDefined(typeof(StringComparison), (Int32)CompOpt) == false)
                { throw new InvalidEnumArgumentException("CompOpt", (Int32)CompOpt, typeof(StringComparison)); }

                List<Substring> output = new List<Substring>();

                Int32 internal_offset = StartIndex;

                while (true)
                {
                    Int32 start_token_start_pos = Input.IndexOf(StartToken, internal_offset, CompOpt);
                    if (start_token_start_pos == -1)
                    {
                        return output;
                    }
                    Int32 end_token_start_pos = Input.IndexOf(EndToken, (start_token_start_pos + StartToken.Length), CompOpt);
                    if (end_token_start_pos == -1)
                    {
                        return output;
                    }
                    if (start_token_start_pos + StartToken.Length == end_token_start_pos)
                    {
                        output.Add(null);
                    }
                    else
                    {
                        output.Add(Substring.CreateWithEndIndex(Input, start_token_start_pos + StartToken.Length, end_token_start_pos - 1));
                    }
                    internal_offset = end_token_start_pos + EndToken.Length;//обновление смещения
                }
            }

            /// <summary>
            /// Возвращает искомую подстроку из указанной строки, которые находятся между наборами указанных начальных и конечных токенов 
            /// в правильной последовательности. Если каких-либо или всех указанных токенов не найдено, или их реальный порядок не совпадает с указанным, 
            /// возвращается пустой список подстрок.
            /// </summary>
            /// <param name="Input">Входная строка, содержащая токены, и внутри которой происходит поиск. Входная строка не может быть NULL, пустой или состоящей из одних пробелов.</param>
            /// <param name="StartTokens">Список начальных токенов в определённой последовательности, которая будет соблюдена при поиске подстрок. 
            /// Не может быть NULL или пустым, но может содержать единственный элемент.</param>
            /// <param name="EndTokens">Список конечных токенов в определённой последовательности, которая будет соблюдена при поиске подстрок. 
            /// Не может быть NULL или пустым, но может содержать единственный элемент.</param>
            /// <param name="EndTokensSearchDirection">Задаёт направление поиска конечных токенов после того, как найдены все начальные. 
            /// FromEndToStart - поиск ведётся от самого конца строки и продвигается до последнего начального токена до тех пор, пока не найдёт все конечные токены. 
            /// FromStartToEnd - поиск ведётся от конца последнего начального токена до конца строки.</param>
            /// <param name="StartIndex">Позиция (включительная) начала поиска во входной строке. Если 0 - поиск ведётся с начала. 
            /// Если меньше 0 или больше длины входной строки - выбрасывается исключение.</param>
            /// <param name="CompOpt">Опции сравнения строк между собой</param>
            /// <exception cref="ArgumentException"></exception>
            /// <returns>Искомая подстрока. Если не найдена, возвращается NULL.</returns>
            public static Substring GetInnerStringBetweenTokensSet(String Input, String[] StartTokens, String[] EndTokens, 
                StringTools.Direction EndTokensSearchDirection, Int32 StartIndex, StringComparison CompOpt)
            {
                if (Input.IsStringNullEmptyWhiteSpace() == true)
                { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input"); }

                if (StartTokens.IsNullOrEmpty())
                { throw new ArgumentException("Список начальных токенов не может быть NULL или пустым", "StartTokens"); }
                if (EndTokens.IsNullOrEmpty())
                { throw new ArgumentException("Список конечных токенов не может быть NULL или пустым", "EndTokens"); }
                if (StartIndex < 0) { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Позиция начала поиска не может быть меньше 0"); }
                if (StartIndex >= Input.Length)
                {
                    throw new ArgumentOutOfRangeException("StartIndex", StartIndex,
                      String.Format("Позиция начала поиска ('{0}') не может быть больше или равна длине строки ('{1}')", StartIndex, Input.Length));
                }
                if (Enum.IsDefined(typeof(Direction), EndTokensSearchDirection) == false)
                { throw new InvalidEnumArgumentException("EndTokensSearchDirection", (Int32)EndTokensSearchDirection, typeof(Direction)); }
                
                if (Enum.IsDefined(typeof(StringComparison), (Int32)CompOpt) == false)
                { throw new InvalidEnumArgumentException("CompOpt", (Int32)CompOpt, typeof(StringComparison)); }

                Int32 internal_offset = StartIndex;

                for (Int32 one_start_token_index = 0; one_start_token_index < StartTokens.Length; one_start_token_index++)
                {
                    String current_start_token = StartTokens[one_start_token_index];
                    Int32 current_start_token_pos = Input.IndexOf(current_start_token, internal_offset, CompOpt);
                    if (current_start_token_pos == -1)
                    {
                        return null;
                    }
                    else
                    {
                        internal_offset = current_start_token_pos + current_start_token.Length;
                    }
                }
                Int32 final_substr_start_index = internal_offset;

                if (EndTokensSearchDirection == Direction.FromEndToStart)
                {
                    Int32 end_offset = Input.Length - 1;

                    for (Int32 one_end_token_index = EndTokens.Length - 1; one_end_token_index >= 0; one_end_token_index--)
                    {
                        String current_end_token = EndTokens[one_end_token_index];
                        Int32 count_to_search = end_offset - final_substr_start_index;
                        Int32 current_end_token_pos = Input.LastIndexOf(current_end_token, end_offset, count_to_search, CompOpt);
                        if (current_end_token_pos == -1)
                        {
                            return null;
                        }
                        else
                        {
                            end_offset = current_end_token_pos;
                        }
                    }
                    return Substring.CreateWithEndIndex(Input, final_substr_start_index, end_offset - 1);
                }
                else
                {
                    Int32 final_substr_end_index = 0;
                    
                    for (Int32 one_end_token_index = 0; one_end_token_index < EndTokens.Length; one_end_token_index++)
                    {
                        String current_end_token = EndTokens[one_end_token_index];

                        Int32 current_end_token_pos = Input.IndexOf(current_end_token, internal_offset, CompOpt);
                        if (current_end_token_pos == -1)
                        {
                            return null;
                        }
                        internal_offset = current_end_token_pos + current_end_token.Length;
                        if (final_substr_end_index == 0)
                        {
                            final_substr_end_index = current_end_token_pos;
                        }
                    }
                    return Substring.CreateWithEndIndex(Input, final_substr_start_index, final_substr_end_index-1);
                }
            }

            /// <summary>
            /// Возвращает часть указанной входной строки, которая размещена от начала или от конца и до первого указанного токена
            /// </summary>
            /// <param name="Input">Входящая строка, из которой надо извлечь подстроку</param>
            /// <param name="Token">Токен, который определяет конец подстроки</param>
            /// <param name="LeaveToken">Если "true" - ближайший токен будет оставлен. Если "false" - он тоже будет удалён.</param>
            /// <param name="Dir">Направление, с которого будет возвращена подстрока: из начала или из конца</param>
            /// <param name="CompareOptions">Опции сравнения строк между собой</param>
            /// <returns></returns>
            public static String GetSubstringToToken(String Input, String Token, Boolean LeaveToken, Direction Dir, StringComparison CompareOptions)
            {
                if (Input.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input"); }
                if (Token.IsStringNullOrEmpty() == true) { throw new ArgumentException("Токен не может быть NULL или пустой строкой", "Token"); }
                if (String.Compare(Input, Token, CompareOptions) == 0) { throw new ArgumentException("Входная строка не может быть равна токену"); }
                if (Input.Contains(Token, CompareOptions) == false) { return Input; }
                Int32 input_length = Input.Length;
                String temp;
                switch (Dir)
                {
                    case Direction.FromStartToEnd:
                        Int32 index_of_first_token = Input.IndexOf(Token, CompareOptions);
                        temp = Input.Substring(0, index_of_first_token);
                        if (LeaveToken == false) {return temp;}
                        else { return temp + Token;}
                    case Direction.FromEndToStart:
                        Int32 token_length = Token.Length;
                        Int32 index_of_last_token = Input.LastIndexOf(Token, CompareOptions);
                        temp = SubstringFromEnd(Input, (input_length - (index_of_last_token + token_length)), true);
                        if (LeaveToken == false) {return temp;}
                        else {return Token + temp;}
                    default:
                        throw new InvalidEnumArgumentException("Dir", (Int32)Dir, Dir.GetType());
                }
            }

            /// <summary>
            /// Возвращает исходную строку, в которой обрезана её часть от начала или от конца до ближайшего указанного токена
            /// </summary>
            /// <param name="Input">Входящая строка, из которой надо извлечь подстроку</param>
            /// <param name="Token">Токен, который определяет точку обрезания и также обрезается</param>
            /// <param name="LeaveToken">Если "true" - ближайший токен будет оставлен. Если "false" - он тоже будет удалён.</param>
            /// <param name="Dir">Направление, с которого будет обрезана подстрока: из начала или из конца</param>
            /// <param name="CompareOptions">Опции сравнения строк между собой</param>
            /// <returns></returns>
            public static String TruncateToClosestToken(String Input, String Token, Boolean LeaveToken, Direction Dir, StringComparison CompareOptions)
            {
                if (Input.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input"); }
                if (Token.IsStringNullOrEmpty() == true) { throw new ArgumentException("Токен не может быть NULL или пустой строкой", "Token"); }
                if (String.Compare(Input, Token, CompareOptions) == 0) { throw new ArgumentException("Входная строка не может быть равна токену"); }
                if (Input.Contains(Token, CompareOptions) == false) { return Input; }
                Int32 token_length = Token.Length;
                switch (Dir)
                {
                    case Direction.FromStartToEnd:
                        Int32 index_of_first_token = Input.IndexOf(Token, CompareOptions);
                        if (LeaveToken == true)
                        {
                            return Input.Remove(0, index_of_first_token);
                        }
                        else
                        {
                            return Input.Remove(0, index_of_first_token + token_length);
                        }

                    case Direction.FromEndToStart:

                        Int32 index_of_last_token = Input.LastIndexOf(Token, CompareOptions);
                        if (LeaveToken == true)
                        {
                            return Input.Remove(index_of_last_token + token_length);
                        }
                        else
                        {
                            return Input.Remove(index_of_last_token);
                        }

                    default:
                        throw new InvalidEnumArgumentException("Dir", (Int32)Dir, Dir.GetType());
                }
            }

            /// <summary>
            /// Возвращает подстроку из входной строки, которая отчитывается от начала или от конца и до указанного вхожденя указанного токена.
            /// </summary>
            /// <param name="Input"></param>
            /// <param name="Token"></param>
            /// <param name="Number">Номер вхождения указанного токена, начиная с 1</param>
            /// <param name="Dir"></param>
            /// <param name="CompareOptions"></param>
            /// <returns></returns>
            public static String GetSubstringToTokenWithSpecifiedNumber(String Input, String Token, Byte Number, Direction Dir, StringComparison CompareOptions)
            {
                if (Input.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input"); }
                if (Token.IsStringNullOrEmpty() == true) { throw new ArgumentException("Токен не может быть NULL или пустой строкой", "Token"); }
                if (String.Compare(Input, Token, CompareOptions) == 0) { throw new ArgumentException("Входная строка не может быть равна токену"); }
                if (Input.Contains(Token, CompareOptions) == false) { return Input; }
                if (Number == 0) { throw new ArgumentOutOfRangeException("Number", "Позиция не может быть нулевой"); }
                Int32 nums_of_occureses = StringAnalyzers.GetNumberOfOccurencesInString(Input, Token, CompareOptions);
                if (Number > nums_of_occureses) { throw new ArgumentOutOfRangeException("Number", Number, "Указанная позиция больше, чем количество вхождений токена в строке"); }

                List<int> positions = StringAnalyzers.GetPositionsOfTokenInString(Input, Token, CompareOptions);

                Int32 token_length = Token.Length;
                Int32 desired_position;
                switch (Dir)
                {
                    case Direction.FromStartToEnd:
                        desired_position = positions[Number - 1];
                        return Input.Substring(0, desired_position);
                    case Direction.FromEndToStart:
                        desired_position = positions[positions.Count - Number];
                        return Input.Substring(desired_position + token_length);
                    default:
                        throw new InvalidEnumArgumentException("Dir", (Int32)Dir, Dir.GetType());
                }
            }

            /// <summary>
            /// Возвращает подстроку из входной строки, которая отчитывается от указанной позиции и до указанного вхожденя указанного токена.
            /// </summary>
            /// <param name="Input"></param>
            /// <param name="Token">Номер вхождения указанного токена, начиная с 1</param>
            /// <param name="Number"></param>
            /// <param name="StartPosition"></param>
            /// <param name="CompareOptions"></param>
            /// <returns></returns>
            public static String GetSubstringToTokenWithSpecifiedNumber(String Input, String Token, Byte Number, Int32 StartPosition, StringComparison CompareOptions)
            {
                if (Input.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input"); }
                if (Token.IsStringNullOrEmpty() == true) { throw new ArgumentException("Токен не может быть NULL или пустой строкой", "Token"); }
                if (String.Compare(Input, Token, CompareOptions) == 0) { throw new ArgumentException("Входная строка не может быть равна токену"); }
                if (Input.Contains(Token, CompareOptions) == false) { return Input; }
                if (Number == 0) { throw new ArgumentOutOfRangeException("Number", "Позиция не может быть нулевой"); }
                if (StartPosition < 0) { throw new ArgumentOutOfRangeException("StartPosition", StartPosition, "Начальная позиция не может быть меньше 0"); }
                String sub = Input.Substring(StartPosition);
                Int32 nums_of_occureses = StringAnalyzers.GetNumberOfOccurencesInString(sub, Token, CompareOptions);
                if (Number > nums_of_occureses) { throw new ArgumentOutOfRangeException("Number", "Указанная бозиция больше, чем количество вхождений токена в части строки"); }

                List<int> positions = StringAnalyzers.GetPositionsOfTokenInString(sub, Token, CompareOptions);
                Int32 desired_position = positions[Number - 1];
                return sub.Substring(0, desired_position);
            }
            
            /// <summary>
            /// Удаляет из входной строки указанную начальную и конечную подстроки, если они есть. Если их нет, то возвращается исходная строка. 
            /// Если во входной строке содержится множество вложенных один в другой начальных и конечных токенов, метод удалит их все рекурсивно.
            /// </summary>
            /// <param name="Input">Входная строка, из которой необходимо удалить все указанные начальные и конечные токены.</param>
            /// <param name="StartToken">Начальный токен</param>
            /// <param name="EndToken">Конечный токен</param>
            /// <param name="CompOpt"></param>
            /// <returns>Новая строка, содержащая копию старой с удалёнными токенами</returns>
            public static String DeleteStartAndEndTokens(String Input, String StartToken, String EndToken, StringComparison CompOpt)
            {
                if (StartToken == null) { throw new ArgumentNullException("Input"); }
                if (EndToken == null) { throw new ArgumentNullException("EndToken"); }

                if (Input.IsStringNullEmptyWhiteSpace() == true) { return Input; }

                if (Input.StartsWith(StartToken, CompOpt) == true && Input.EndsWith(EndToken, CompOpt) == true)
                {
                    Input = Input.Remove(0, StartToken.Length);
                    Input = Input.Remove(Input.Length - EndToken.Length, EndToken.Length);
                    return DeleteStartAndEndTokens(Input, StartToken, EndToken, CompOpt);
                }
                else
                {
                    return Input;
                }
            }

            /// <summary>
            /// Удаляет из входной строки все символы, которые не подпадают под допустимые
            /// </summary>
            /// <param name="Input">Входная строка, которую следует "почистить". Если NULL или пустая, возвращается без изменений</param>
            /// <param name="AllowedSymbols">Допустимые клссы символов. Если содержит неопределённое значение, входная строка возвращается без изменений.</param>
            /// <returns></returns>
            public static String CleanFromChars(String Input, StringAnalyzers.ContainsEntities AllowedSymbols)
            {
                if (Input.IsStringNullOrEmpty() == true || AllowedSymbols == StringAnalyzers.ContainsEntities.Empty) { return Input; }
                StringBuilder output = new StringBuilder(Input.Length);
                for (Int32 i = 0; i < Input.Length; i++)
                {
                    Char current = Input[i];
                    if (Char.IsControl(current) == true && AllowedSymbols.Misses(StringAnalyzers.ContainsEntities.Controls) == true)
                    { continue; }
                    if (
                        (AllowedSymbols.Contains(StringAnalyzers.ContainsEntities.Letters) == true && Char.IsLetter(current) == true) ||
                        (AllowedSymbols.Contains(StringAnalyzers.ContainsEntities.Digits) == true && Char.IsDigit(current) == true) ||
                        (AllowedSymbols.Contains(StringAnalyzers.ContainsEntities.Spaces) == true && Char.IsWhiteSpace(current) == true) ||
                        (AllowedSymbols.Contains(StringAnalyzers.ContainsEntities.Controls) == true && Char.IsControl(current) == true)
                        )
                    {
                        output.Append(current);
                    }
                }
                return output.ToString();
            }

            /// <summary>
            /// Защищает входную строку от SQL-инъекции, экранируя одинарные кавычки
            /// </summary>
            /// <param name="Input"></param>
            /// <returns></returns>
            public static String SecureSQLQuery(String Input)
            {
                if (Input.HasVisibleChars() == false) { return Input; }
                String output = Input.Replace("'", "''");
                return output;
            }

            /// <summary>
            /// Конкатенирует все указанные строки в одну и дублирует её указанное количество раз, конкатенируя каждый новый дубль к предыдущему
            /// </summary>
            /// <remarks>Детерминированный бессмертный метод, не бросающий исключений</remarks>
            /// <param name="IterationsCount">Количество дублей конкатенированных строк, которые необходимо совершить. Если равен 0, возвращается пустая строка.</param>
            /// <param name="Input">Массив от одной включительно и больше строк, которые необходимо конкатенировать. 
            /// Если не содержит ни одной строки, не являющейся NULL или пустой, метод возвращает пустую строку.</param>
            /// <returns>Одна строка</returns>
            public static String ConcatenateAllStringsManyTimes(Int32 IterationsCount, params String[] Input)
            {
                if (IterationsCount <= 0) { return String.Empty; }
                if (Input.IsNullOrEmpty() == true) { return String.Empty; }
                StringBuilder temp = new StringBuilder(IterationsCount * Input.Length * 2);
                for (UInt16 i = 0; i < IterationsCount; i++)
                {
                    Boolean is_exists_valid = false;
                    foreach (string s in Input)
                    {
                        if (s.IsStringNullOrEmpty() == false)
                        {
                            temp.Append(s);
                            is_exists_valid = true;
                        }
                    }
                    if (is_exists_valid == false) { return String.Empty; }
                }
                return temp.ToString();
            }

            /// <summary>
            /// Уплотняет пробелы в указанной строке, сокращая множественные идущие подряд пробелы до одного
            /// </summary>
            /// <param name="Input">Строка, которую следует уплотнить. Если NULL или пустая - будет возвращена без изменений.</param>
            /// <returns></returns>
            public static String ShrinkSpaces(String Input)
            {
                if (Input.IsNullOrEmpty() == true) {return Input;}
                StringBuilder output = new StringBuilder(Input.Length);
                const Char space = ' ';
                Int32 spaces_count = 0;
                foreach (Char c in Input)
                {
                    if (Char.IsControl(c) == true || Char.IsWhiteSpace(c) == false)
                    {
                        if (spaces_count > 0)
                        {
                            spaces_count = 0;
                            output.Append(space);
                        }
                        output.Append(c);
                    }
                    else
                    {
                        spaces_count++;
                    }
                }
                if (spaces_count > 0)
                {
                    output.Append(space);
                }
                return output.ToString();
            }
        }//end of class SubstringHelpers

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
                if (visible_count > 0 && invisible_count > 0) { return null; }
                if (visible_count == input_count) { return true; }
                if (invisible_count == input_count) { return false; }
                throw new UnreachableCodeException();
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
                for(Int32 i = 0; i < divided.Length; i++)
                {
                    String number = divided[i];
                    Nullable<TNumber> one = number.TryParseNumber<TNumber>(Style, CultureProvider);
                    if (one == null)
                    {
                        throw new InvalidOperationException(String.Format("Невозможно успешно распарсить подстроку '{0}' на позиции {1} как число типа {2}", 
                            number, i+1, typeof(TNumber).FullName));
                    }
                    output.Add(one.Value);
                }
                return output;
            }
        }//end of class ParsingHelpers

        /// <summary>
        /// Содержит статические методы, изменяющие строки с той или иной целью
        /// </summary>
        public static class StringModifiers
        {
            /// <summary>
            /// Изменяет первый символ строки на заглавный
            /// </summary>
            /// <param name="Input">Входная строка.</param>
            /// <returns></returns>
            public static String FirstLetterToUpper(String Input)
            {
                if(Input.IsStringNullEmptyWS()==true) {return Input;}

                if (Input.Length > 1)
                {return Char.ToUpper(Input[0]) + Input.Substring(1);}
                else
                {return Input.ToUpper();}
            }
        }
    }
}

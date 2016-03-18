#define Debug
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KlotosLib.StringTools
{
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
            if (first == -1) { return output; }
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
        public static Nullable<UInt32> GetNearestUnsignedIntegerFromString(String Input, StringTools.Direction Dir, Boolean RaiseException)
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
                case StringTools.Direction.FromStartToEnd:
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

                case StringTools.Direction.FromEndToStart:
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
}

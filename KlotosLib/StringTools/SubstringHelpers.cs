using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KlotosLib.StringTools
{
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
        /// <param name="ComparisonType">Опции сравнения строк между собой</param>
        /// <returns>Новая строка. Если не найдено ни одной пары начальныго и конечного токенов, возвращается оригинальная строка.
        /// Если начальный и конечный токены одинаковы, или исходная строка является NULL, пустой строкой либо содержит лишь пробелы, 
        /// либо хотя бы один из токенов является NULL или пустой строкой, метод выбрасывает исключение.</returns>
        public static String RemoveFromStartToEndToken(String Input, String StartToken, String EndToken, Boolean RemoveTokens, StringComparison ComparisonType)
        {
            if (Input.IsStringNullEmptyWhiteSpace() == true) throw new ArgumentException("Входная строка является NULL, пустой строкой либо состоит лишь из одних пробелов", "Input");
            if (StartToken.IsStringNullOrEmpty() == true) throw new ArgumentException("Начальный токен является NULL или пустой строкой", "StartToken");
            if (EndToken.IsStringNullOrEmpty() == true) throw new ArgumentException("Конечный токен является NULL или пустой строкой", "EndToken");
            if (String.Compare(StartToken, EndToken, ComparisonType) == 0)
            { throw new ArgumentException("Начальный и конечный токены должны быть разными с учётом указнных опций сравнения"); }

            Int32 current_offset = 0;
            StringBuilder sb = new StringBuilder(Input.Length);
            while (true)
            {
                Int32 start_index = Input.IndexOf(StartToken, current_offset, ComparisonType);
                if (start_index < 0) { break; }


                Int32 end_index = Input.IndexOf(EndToken, start_index, ComparisonType);
                if (end_index < 0) { break; }

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
        /// Если больше нуля и меньше или равно ненулевой начальной позиции, выбрасывается исключение.</param>
        /// <param name="IncludeTokens">Определяет, следует ли включать начальный и конечный токены в возвращаемую подстроку (true) или нет (false)</param>
        /// <param name="EndTokenSearchDirection">Задаёт направление поиска конечного токена после того, как найден начальный. 
        /// FromEndToStart - поиск ведётся от самого конца входной строки и продвигается до найденного начального токена до тех пор, пока не найдёт конечный токен. 
        /// FromStartToEnd - поиск ведётся от конца найденного начального токена до конца входной строки.</param>
        /// <param name="ComparisonType">Опции сравнения строк между собой</param>
        /// <returns>NULL - если не нашло</returns>
        public static Substring GetInnerStringBetweenTokens
            (String Input, String StartToken, String EndToken,
            Int32 StartIndex, Int32 EndIndex, Boolean IncludeTokens, StringTools.Direction EndTokenSearchDirection, StringComparison ComparisonType)
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
            if (Enum.IsDefined(EndTokenSearchDirection.GetType(), EndTokenSearchDirection) == false)
            { throw new InvalidEnumArgumentException("EndTokenSearchDirection", (Int32)EndTokenSearchDirection, EndTokenSearchDirection.GetType()); }
            if (Enum.IsDefined(typeof(StringComparison), ComparisonType) == false)
            { throw new InvalidEnumArgumentException("ComparisonType", (Int32)ComparisonType, typeof(StringComparison)); }
            Int32 start_token_start_pos = Input.IndexOf(StartToken, StartIndex, (EndIndex - StartIndex + 1), ComparisonType);
            if (start_token_start_pos == -1)
            {
                return null;
            }
            Int32 end_token_start_pos;
            if (EndTokenSearchDirection == StringTools.Direction.FromStartToEnd)
            {
                Int32 search_start_index = start_token_start_pos + StartToken.Length;
                Int32 count = EndIndex - search_start_index + 1;
                end_token_start_pos = Input.IndexOf(EndToken, search_start_index, count, ComparisonType);
            }
            else// if (EndTokenSearchDirection == StringTools.Direction.FromEndToStart)
            {
                Int32 start_index = EndIndex;
                Int32 count = EndIndex - start_token_start_pos + StartToken.Length;
                end_token_start_pos = Input.LastIndexOf(EndToken, start_index, count, ComparisonType);
            }
            if (end_token_start_pos == -1)
            {
                return null;
            }
            Substring output;
            if (IncludeTokens == false)
            {
                output = Substring.FromIndexToIndex(Input, start_token_start_pos + StartToken.Length, end_token_start_pos - 1);
            }
            else
            {
                output = Substring.FromIndexToIndex(Input, start_token_start_pos, end_token_start_pos + EndToken.Length - 1);
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
        /// <param name="IncludeTokens">Определяет, следует ли включать начальный и конечный токены в возвращаемые подстроки (true) или нет (false)</param>
        /// <param name="CompOpt">Опции сравнения строк между собой</param>
        /// <returns>Список подстрок. Если какая-либо из подстрок является пустой (т.е. между начальным и конечным токеном нет ни одного символа), 
        /// в списке она будет представлена значением NULL.</returns>
        public static List<Substring> GetInnerStringsBetweenTokens(String Input, String StartToken, String EndToken, Int32 StartIndex, Boolean IncludeTokens, StringComparison CompOpt)
        {
            if (Input.IsStringNullEmptyWhiteSpace() == true)
            { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input"); }
            if (StartToken.IsStringNullOrEmpty() == true) { throw new ArgumentException("Начальный токен не может быть NULL или пустой строкой", "StartToken"); }
            if (EndToken.IsStringNullOrEmpty() == true) { throw new ArgumentException("Конечный токен не может быть NULL или пустой строкой", "EndToken"); }
            if (StartIndex < 0) { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Позиция начала поиска не может быть меньше 0"); }
            if (StartIndex >= Input.Length)
            {
                throw new ArgumentOutOfRangeException("StartIndex", StartIndex,
                  String.Format("Позиция начала поиска ('{0}') не может быть больше или равна длине строки ('{1}')", StartIndex, Input.Length));
            }
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
                    Int32 substring_start_index;
                    Int32 substring_end_index;
                    if (IncludeTokens == false)
                    {
                        substring_start_index = start_token_start_pos + StartToken.Length;
                        substring_end_index = end_token_start_pos - 1;
                    }
                    else
                    {
                        substring_start_index = start_token_start_pos;
                        substring_end_index = end_token_start_pos - 1 + EndToken.Length;
                        
                    }
                    output.Add(Substring.FromIndexToIndex(Input, substring_start_index, substring_end_index));
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
            if(Input == null){throw new ArgumentNullException("Input");}
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
            if (Enum.IsDefined(typeof(StringTools.Direction), EndTokensSearchDirection) == false)
            { throw new InvalidEnumArgumentException("EndTokensSearchDirection", (Int32)EndTokensSearchDirection, typeof(StringTools.Direction)); }

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

            if (EndTokensSearchDirection == StringTools.Direction.FromEndToStart)
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
                return Substring.FromIndexToIndex(Input, final_substr_start_index, end_offset - 1);
            }
            else// if (EndTokensSearchDirection == StringTools.Direction.FromStartToEnd)
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
                return Substring.FromIndexToIndex(Input, final_substr_start_index, final_substr_end_index - 1);
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
        public static String GetSubstringToToken(String Input, String Token, Boolean LeaveToken, StringTools.Direction Dir, StringComparison CompareOptions)
        {
            if (Input.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input"); }
            if (Token.IsStringNullOrEmpty() == true) { throw new ArgumentException("Токен не может быть NULL или пустой строкой", "Token"); }
            if (String.Compare(Input, Token, CompareOptions) == 0) { throw new ArgumentException("Входная строка не может быть равна токену"); }
            if (Input.Contains(Token, CompareOptions) == false) { return Input; }
            Int32 input_length = Input.Length;
            String temp;
            switch (Dir)
            {
                case StringTools.Direction.FromStartToEnd:
                    Int32 index_of_first_token = Input.IndexOf(Token, CompareOptions);
                    temp = Input.Substring(0, index_of_first_token);
                    if (LeaveToken == false) { return temp; }
                    else { return temp + Token; }
                case StringTools.Direction.FromEndToStart:
                    Int32 token_length = Token.Length;
                    Int32 index_of_last_token = Input.LastIndexOf(Token, CompareOptions);
                    temp = SubstringFromEnd(Input, (input_length - (index_of_last_token + token_length)), true);
                    if (LeaveToken == false) { return temp; }
                    else { return Token + temp; }
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
        /// <param name="ComparisonType">Опции сравнения строк между собой</param>
        /// <returns></returns>
        public static String TruncateToClosestToken(String Input, String Token, Boolean LeaveToken, StringTools.Direction Dir, StringComparison ComparisonType)
        {
            if (Input.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input"); }
            if (Token.IsStringNullOrEmpty() == true) { throw new ArgumentException("Токен не может быть NULL или пустой строкой", "Token"); }
            if (Enum.IsDefined(ComparisonType.GetType(), ComparisonType) == false)
            { throw new InvalidEnumArgumentException("ComparisonType", (Int32)ComparisonType, ComparisonType.GetType()); }
            if (String.Compare(Input, Token, ComparisonType) == 0) { throw new ArgumentException("Входная строка не может быть равна токену"); }
            if (Input.Contains(Token, ComparisonType) == false) { return Input; }
            Int32 token_length = Token.Length;
            switch (Dir)
            {
                case StringTools.Direction.FromStartToEnd:
                    Int32 index_of_first_token = Input.IndexOf(Token, ComparisonType);
                    if (LeaveToken == true)
                    {
                        return Input.Remove(0, index_of_first_token);
                    }
                    else
                    {
                        return Input.Remove(0, index_of_first_token + token_length);
                    }

                case StringTools.Direction.FromEndToStart:

                    Int32 index_of_last_token = Input.LastIndexOf(Token, ComparisonType);
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
        /// <param name="ComparisonType"></param>
        /// <returns></returns>
        public static String GetSubstringToTokenWithSpecifiedNumber(String Input, String Token, Byte Number, StringTools.Direction Dir, StringComparison ComparisonType)
        {
            if (Input.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input"); }
            if (Token.IsStringNullOrEmpty() == true) { throw new ArgumentException("Токен не может быть NULL или пустой строкой", "Token"); }
            if (Enum.IsDefined(ComparisonType.GetType(), ComparisonType) == false)
            { throw new InvalidEnumArgumentException("ComparisonType", (Int32)ComparisonType, ComparisonType.GetType()); }
            if (String.Compare(Input, Token, ComparisonType) == 0) { throw new ArgumentException("Входная строка не может быть равна токену"); }
            if (Input.Contains(Token, ComparisonType) == false) { return Input; }
            if (Number == 0) { throw new ArgumentOutOfRangeException("Number", "Номер вхождения указанного токена не может быть нулевой"); }
            Int32 nums_of_occureses = StringTools.StringAnalyzers.GetNumberOfOccurencesInString(Input, Token, ComparisonType);
            if (Number > nums_of_occureses) { throw new ArgumentOutOfRangeException("Number", Number, "Указанная позиция больше, чем количество вхождений токена в строке"); }

            List<int> positions = StringTools.StringAnalyzers.GetPositionsOfTokenInString(Input, Token, ComparisonType);

            Int32 token_length = Token.Length;
            Int32 desired_position;
            switch (Dir)
            {
                case StringTools.Direction.FromStartToEnd:
                    desired_position = positions[Number - 1];
                    return Input.Substring(0, desired_position);
                case StringTools.Direction.FromEndToStart:
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
        /// <param name="ComparisonType"></param>
        /// <returns></returns>
        public static String GetSubstringToTokenWithSpecifiedNumber(String Input, String Token, Byte Number, Int32 StartPosition, StringComparison ComparisonType)
        {
            if (Input.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "Input"); }
            if (Token.IsStringNullOrEmpty() == true) { throw new ArgumentException("Токен не может быть NULL или пустой строкой", "Token"); }
            if (Enum.IsDefined(ComparisonType.GetType(), ComparisonType) == false)
            { throw new InvalidEnumArgumentException("ComparisonType", (Int32)ComparisonType, ComparisonType.GetType()); }
            if (String.Compare(Input, Token, ComparisonType) == 0) { throw new ArgumentException("Входная строка не может быть равна токену"); }
            if (Input.Contains(Token, ComparisonType) == false) { return Input; }
            if (Number == 0) { throw new ArgumentOutOfRangeException("Number", "Номер вхождения указанного токена не может быть нулевой"); }
            if (StartPosition < 0) { throw new ArgumentOutOfRangeException("StartPosition", StartPosition, "Начальная позиция не может быть меньше 0"); }
            String sub = Input.Substring(StartPosition);
            Int32 nums_of_occureses = StringTools.StringAnalyzers.GetNumberOfOccurencesInString(sub, Token, ComparisonType);
            if (Number > nums_of_occureses) { throw new ArgumentOutOfRangeException("Number", "Указанная бозиция больше, чем количество вхождений токена в части строки"); }

            List<int> positions = StringTools.StringAnalyzers.GetPositionsOfTokenInString(sub, Token, ComparisonType);
            Int32 desired_position = positions[Number - 1];
            return sub.Substring(0, desired_position);
        }

        /// <summary>
        /// Удаляет из входной строки указанную начальную и конечную подстроки, если они есть. Если нет хотя бы одной из них, то возвращается исходная строка. 
        /// Если во входной строке содержится множество вложенных один в другой начальных и конечных токенов, метод удалит их все рекурсивно.
        /// </summary>
        /// <param name="Input">Входная строка, из которой необходимо удалить все указанные начальные и конечные токены.</param>
        /// <param name="StartToken">Начальный токен</param>
        /// <param name="EndToken">Конечный токен</param>
        /// <param name="CompOpt"></param>
        /// <returns>Новая строка, содержащая копию старой с удалёнными токенами</returns>
        public static String DeleteStartAndEndTokens(String Input, String StartToken, String EndToken, StringComparison CompOpt)
        {
            if (StartToken.IsNullOrEmpty() == true) { throw new ArgumentException("Начальный токен не может быть NULL или пустой строкой", "Input"); }
            if (EndToken == null) { throw new ArgumentException("Конечный токен не может быть NULL или пустой строкой", "EndToken"); }

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
        public static String CleanFromChars(String Input, StringTools.StringAnalyzers.ContainsEntities AllowedSymbols)
        {
            if (Input.IsStringNullOrEmpty() == true || AllowedSymbols == StringTools.StringAnalyzers.ContainsEntities.Empty) { return Input; }
            StringBuilder output = new StringBuilder(Input.Length);
            for (Int32 i = 0; i < Input.Length; i++)
            {
                Char current = Input[i];
                if (Char.IsControl(current) == true && AllowedSymbols.Misses(StringTools.StringAnalyzers.ContainsEntities.Controls) == true)
                { continue; }
                if (
                    (AllowedSymbols.Contains(StringTools.StringAnalyzers.ContainsEntities.Letters) == true && Char.IsLetter(current) == true) ||
                    (AllowedSymbols.Contains(StringTools.StringAnalyzers.ContainsEntities.Digits) == true && Char.IsDigit(current) == true) ||
                    (AllowedSymbols.Contains(StringTools.StringAnalyzers.ContainsEntities.Spaces) == true && Char.IsWhiteSpace(current) == true) ||
                    (AllowedSymbols.Contains(StringTools.StringAnalyzers.ContainsEntities.Controls) == true && Char.IsControl(current) == true)
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
            if (IterationsCount < 0) { throw new ArgumentOutOfRangeException("IterationsCount", IterationsCount, "Количество итераций не может быть меньше 0"); }
            if (IterationsCount <= 0) { return String.Empty; }
            if (Input.IsNullOrEmpty() == true) { return String.Empty; }
            StringBuilder temp = new StringBuilder(IterationsCount * Input.Length * 4);
            for (UInt32 i = 0; i < IterationsCount; i++)
            {
                Boolean is_exists_valid = false;
                foreach (String s in Input)
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
            if (Input.IsNullOrEmpty() == true) { return Input; }
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
}

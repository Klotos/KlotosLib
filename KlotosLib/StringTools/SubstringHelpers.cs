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
        /// <param name="input">Входная строка, подстроку из которой слуедует возвратить</param>
        /// <param name="length">Желаемая длина возвращаемой подстроки</param>
        /// <param name="untilStart">Определяет поведение метода в случае, если желаемая длина больше фактической длины входной подстроки: 
        /// 'true' - возвратить входную строку; 'false' - выбросить исключение.</param>
        /// <returns></returns>
        public static String SubstringFromEnd(String input, Int32 length, Boolean untilStart)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (String.IsNullOrEmpty(input) == true) { throw new ArgumentException("Входная строка не содержит ни одного символа", "input"); }
            if (length < 1) { throw new ArgumentOutOfRangeException("length", length, "Возвращаемая длина не может быть меньше 1"); }
            if (untilStart == false && length > input.Length)
            { throw new ArgumentOutOfRangeException("length", length, "Указанная длина = " + length + " превышает фактическую длину строки = " + input.Length); }
            else if (untilStart == true && length > input.Length)
            {
                return input;
            }
            else
            {
                String temp = input.Substring(input.Length - length);
                return temp;
            }
        }

        /// <summary>
        /// Ищет и возвращает из входной строки все найденные подстроки, если они там присутствуют
        /// </summary>
        /// <param name="input">Входная строка, в которой происходит поиск подстрок</param>
        /// <param name="target">Целевая подстрока, которая ищется во входной строке</param>
        /// <param name="startIndex">Начальная позиция входной строки, с которой включительно начинается поиск. Если 0 - поиск ведётся с начала. 
        /// Если меньше 0 или больше длины входной строки, выбрасывается исключение.</param>
        /// <param name="comparisonType">Опции сравнения строк между собой, по которым будет вести поиск целевой подстроки во входной строке</param>
        /// <returns>Список найденных подстрок. Если не будет найдена ни одна, метод возвратит пустой список.</returns>
        public static List<Substring> FindSubstring(String input, String target, Int32 startIndex, StringComparison comparisonType)
        {
            if (input == null) { throw new ArgumentNullException("input", "Входная строка не может быть NULL"); }
            if(input.Length == 0) {throw new ArgumentException("Входная строка не может быть пустой", "input");}
            if (target.IsNullOrEmpty() == true) { throw new ArgumentException("Целевая подстрока не может быть NULL или пустой", "target"); }
            if (startIndex < 0) { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Начальная позиция не может быть меньше 0"); }
            if (startIndex >= input.Length)
            { throw new ArgumentOutOfRangeException("startIndex", startIndex, String.Format("Начальная позиция ('{0}') не может быть больше или равна длине строки ('{1}')", startIndex, input.Length)); }

            List<Substring> output = new List<Substring>();
            Int32 searchOffset = startIndex;
            while (true)
            {
                Int32 foundPosition = input.IndexOf(target, searchOffset, comparisonType);
                if(foundPosition < 0) {break;}

                searchOffset = foundPosition + target.Length;
                Substring found = Substring.FromIndexWithLength(input, foundPosition, target.Length);
                output.Add(found);
            }
            return output;
        }

        /// <summary>
        /// Возвращает входную строку, из которой удалены все подстроки, начинающиеся с указанного начального токена 
        /// и заканчивающиеся ближайшим указанным конечным токеном. 
        /// </summary>
        /// <param name="input">Входящая строка, в которой происходит поиск.</param>
        /// <param name="startToken">Начальный токен (подстрока), которым начинается удаляемая подстрока. Не может быть NULL или пустым.</param>
        /// <param name="endToken">Конечный токен (подстрока), которым оканчивается удаляемая подстрока. Не может быть NULL или пустым.</param>
        /// <param name="removeTokens">Определяет, следует ли удалить начальный и конечный токены вместе с удаляемой подстрокой (true) или же их следует оставить (false)</param>
        /// <param name="comparisonType">Опции сравнения строк между собой</param>
        /// <returns>Новая строка. Если не найдено ни одной пары начальныго и конечного токенов, возвращается оригинальная строка.
        /// Если начальный и конечный токены одинаковы, или исходная строка является NULL, пустой строкой либо содержит лишь пробелы, 
        /// либо хотя бы один из токенов является NULL или пустой строкой, метод выбрасывает исключение.</returns>
        public static String RemoveFromStartToEndToken(String input, String startToken, String endToken, Boolean removeTokens, StringComparison comparisonType)
        {
            if (input.IsStringNullEmptyWhiteSpace() == true) throw new ArgumentException("Входная строка является NULL, пустой строкой либо состоит лишь из одних пробелов", "input");
            if (startToken.IsStringNullOrEmpty() == true) throw new ArgumentException("Начальный токен является NULL или пустой строкой", "startToken");
            if (endToken.IsStringNullOrEmpty() == true) throw new ArgumentException("Конечный токен является NULL или пустой строкой", "endToken");
            if (String.Compare(startToken, endToken, comparisonType) == 0)
            { throw new ArgumentException("Начальный и конечный токены должны быть разными с учётом указнных опций сравнения"); }

            Int32 currentOffset = 0;
            StringBuilder sb = new StringBuilder(input.Length);
            while (true)
            {
                Int32 startIndex = input.IndexOf(startToken, currentOffset, comparisonType);
                if (startIndex < 0) { break; }


                Int32 endIndex = input.IndexOf(endToken, startIndex, comparisonType);
                if (endIndex < 0) { break; }

                String slice;
                if (removeTokens)
                {
                    slice = input.SubstringWithEnd(currentOffset, startIndex);
                }
                else
                {
                    slice = input.SubstringWithEnd(currentOffset, startIndex + startToken.Length) + input.Substring(endIndex, endToken.Length);
                }

                sb.Append(slice);
                currentOffset = endIndex + endToken.Length;
            }
            sb.Append(input.Substring(currentOffset));
            return sb.ToString();
        }

        /// <summary>
        /// Возвращает искомую подстроку из указанной исходной строки, которая размещена между указанным начальным и конечным токеном. 
        /// Ведёт поиск токенов от начала или он указанной начальной позиции до конца строки, и возвращает первое попавшееся совпадение. 
        /// Если указанных токенов не найдено, возвращает NULL.
        /// </summary>
        /// <param name="input">Входная строка, содержащая токены, и внутри которой происходит поиск. Не может быть NULL, пустой или состоящей из одних пробелов.</param>
        /// <param name="startToken">Начальный токен. Не может быть NULL или пустой строкой.</param>
        /// <param name="endToken">Конечный токен. Не может быть NULL или пустой строкой.</param>
        /// <param name="startIndex">Начальная позиция исходной строки, с которой включительно начинается поиск. Если 0 - поиск ведётся с начала. 
        /// Если меньше 0 или больше длины исходной строки, выбрасывается исключение.</param>
        /// <param name="endIndex">Конечная позиция исходной строки, на которой включительно останавливается поиск и за которую не выходит. 
        /// Если больше фактической длины строки, 0 или меньше 0, поиск ведётся до конца строки. 
        /// Если больше нуля и меньше или равно ненулевой начальной позиции, выбрасывается исключение.</param>
        /// <param name="includeTokens">Определяет, следует ли включать начальный и конечный токены в возвращаемую подстроку (true) или нет (false)</param>
        /// <param name="endTokenSearchDirection">Задаёт направление поиска конечного токена после того, как найден начальный. 
        /// FromEndToStart - поиск ведётся от самого конца входной строки и продвигается до найденного начального токена до тех пор, пока не найдёт конечный токен. 
        /// FromStartToEnd - поиск ведётся от конца найденного начального токена до конца входной строки.</param>
        /// <param name="comparisionOptions">Опции сравнения строк между собой</param>
        /// <returns>NULL - если не нашло</returns>
        public static Substring GetInnerStringBetweenTokens
            (String input, String startToken, String endToken,
            Int32 startIndex, Int32 endIndex, Boolean includeTokens, Direction endTokenSearchDirection, StringComparison comparisionOptions)
        {
            if (input.IsStringNullEmptyWs()) { throw new ArgumentException("Input string cannot be NULL, empty or whitespaced", "input"); }
            if (string.IsNullOrEmpty(startToken)) throw new ArgumentException("Start token cannot be NULL or empty", "startToken");
            if (string.IsNullOrEmpty(endToken)) throw new ArgumentException("End token cannot be NULL or empty", "endToken");
            if (startIndex < 0) { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Start index cannot be negative"); }
            if (startIndex >= input.Length)
            { throw new ArgumentOutOfRangeException("startIndex", startIndex, String.Format("Start index ('{0}') cannot be greater or equal of the input string length ('{1}')", startIndex, input.Length)); }
            Int32 finalEndIndex;
            if (endIndex <= 0 || endIndex >= input.Length)
            {
                finalEndIndex = input.Length - 1;
            }
            else
            {
                finalEndIndex = endIndex;
            }
            if (finalEndIndex <= startIndex)
            { throw new ArgumentOutOfRangeException("endIndex", endIndex, String.Format("End index ('{0}') cannot be lesser or equal of the start index ('{1}')", endIndex, startIndex)); }
            if (Enum.IsDefined(typeof(StringComparison), (Int32)comparisionOptions) == false)
            { throw new InvalidEnumArgumentException("comparisionOptions", (Int32)comparisionOptions, typeof(StringComparison)); }

            Int32 startTokenStartPos = input.IndexOf(startToken, startIndex, (finalEndIndex - startIndex + 1), comparisionOptions);
            if (startTokenStartPos == -1)
            {
                return null;
            }
            Int32 endTokenStartPos;

            Int32 innerStartIndex;
            Int32 count;
            if (endTokenSearchDirection == Direction.FromStartToEnd)
            {
                innerStartIndex = startTokenStartPos + startToken.Length;
                count = finalEndIndex - innerStartIndex + 1;
                endTokenStartPos = input.IndexOf(endToken, innerStartIndex, count, comparisionOptions);
            }
            else if (endTokenSearchDirection == Direction.FromEndToStart)
            {
                innerStartIndex = finalEndIndex;
                count = finalEndIndex - (startTokenStartPos + startToken.Length);
                endTokenStartPos = input.LastIndexOf(endToken, innerStartIndex, count, comparisionOptions);
            }
            else
            {
                throw new ArgumentException(String.Format("Value '{0}' ('{1}') of 'Direction' enumeration is not supported",
                    endTokenSearchDirection.ToString(), (Int32)endTokenSearchDirection), "endTokenSearchDirection");
            }
            if (endTokenStartPos == -1)
            {
                return null;
            }
            Substring output;
            if (includeTokens == false)
            {
                Int32 localStartIndex = startTokenStartPos + startToken.Length;
                Int32 localEndIndex = endTokenStartPos - 1;
                if (localStartIndex > localEndIndex)
                {
                    output = null;
                }
                else
                {
                    output = Substring.FromIndexToIndex(input, localStartIndex, localEndIndex);
                }
            }
            else
            {
                output = Substring.FromIndexToIndex(input, startTokenStartPos, endTokenStartPos + endToken.Length - 1);
            }
            return output;
        }

        /// <summary>
        /// Возвращает все искомые подстроки из указанной строки, которые находятся между указанными начальными и конечными токенами. 
        /// Поиск ведётся с начала до конца строки, начиная с указанной позиции, и возвращает все попавшиеся совпадения. 
        /// Если указанных токенов не найдено, возвращает пустой список подстрок.
        /// </summary>
        /// <param name="input">Входная строка, содержащая токены, и внутри которой происходит поиск. Входная строка не может быть NULL, пустой или состоящей из одних пробелов.</param>
        /// <param name="startToken">Начальный токен, не может быть NULL или пустой строкой</param>
        /// <param name="endToken">Конечный токен, не может быть NULL или пустой строкой</param>
        /// <param name="startIndex">Позиция (включительная) начала поиска во входной строке. Если 0 - поиск ведётся с начала. 
        /// Если меньше 0 или больше длины входной строки - выбрасывается исключение.</param>
        /// <param name="includeTokens">Определяет, следует ли включать начальный и конечный токены в возвращаемые подстроки (true) или нет (false)</param>
        /// <param name="compOpt">Опции сравнения строк между собой</param>
        /// <returns>Список подстрок. Если какая-либо из подстрок является пустой (т.е. между начальным и конечным токеном нет ни одного символа), 
        /// в списке она будет представлена значением NULL.</returns>
        public static List<Substring> GetInnerStringsBetweenTokens(String input, String startToken, String endToken, Int32 startIndex, Boolean includeTokens, StringComparison compOpt)
        {
            if (input.IsStringNullEmptyWhiteSpace() == true)
            { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "input"); }
            if (startToken.IsStringNullOrEmpty() == true) { throw new ArgumentException("Начальный токен не может быть NULL или пустой строкой", "startToken"); }
            if (endToken.IsStringNullOrEmpty() == true) { throw new ArgumentException("Конечный токен не может быть NULL или пустой строкой", "endToken"); }
            if (startIndex < 0) { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Позиция начала поиска не может быть меньше 0"); }
            if (startIndex >= input.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex", startIndex,
                  String.Format("Позиция начала поиска ('{0}') не может быть больше или равна длине строки ('{1}')", startIndex, input.Length));
            }
            if (Enum.IsDefined(typeof(StringComparison), (Int32)compOpt) == false)
            { throw new InvalidEnumArgumentException("compOpt", (Int32)compOpt, typeof(StringComparison)); }

            List<Substring> output = new List<Substring>();

            Int32 internalOffset = startIndex;

            while (true)
            {
                Int32 startTokenStartPos = input.IndexOf(startToken, internalOffset, compOpt);
                if (startTokenStartPos == -1)
                {
                    return output;
                }
                Int32 endTokenStartPos = input.IndexOf(endToken, (startTokenStartPos + startToken.Length), compOpt);
                if (endTokenStartPos == -1)
                {
                    return output;
                }
                if (startTokenStartPos + startToken.Length == endTokenStartPos)
                {
                    output.Add(null);
                }
                else
                {
                    Int32 substringStartIndex;
                    Int32 substringEndIndex;
                    if (includeTokens == false)
                    {
                        substringStartIndex = startTokenStartPos + startToken.Length;
                        substringEndIndex = endTokenStartPos - 1;
                    }
                    else
                    {
                        substringStartIndex = startTokenStartPos;
                        substringEndIndex = endTokenStartPos - 1 + endToken.Length;
                        
                    }
                    output.Add(Substring.FromIndexToIndex(input, substringStartIndex, substringEndIndex));
                }
                internalOffset = endTokenStartPos + endToken.Length;//обновление смещения
            }
        }

        /// <summary>
        /// Возвращает искомую подстроку из указанной строки, которые находятся между наборами указанных начальных и конечных токенов 
        /// в правильной последовательности. Если каких-либо или всех указанных токенов не найдено, или их реальный порядок не совпадает с указанным, 
        /// возвращается пустой список подстрок.
        /// </summary>
        /// <param name="input">Входная строка, содержащая токены, и внутри которой происходит поиск. Входная строка не может быть NULL, пустой или состоящей из одних пробелов.</param>
        /// <param name="startTokens">Список начальных токенов в определённой последовательности, которая будет соблюдена при поиске подстрок. 
        /// Не может быть NULL или пустым, но может содержать единственный элемент.</param>
        /// <param name="endTokens">Список конечных токенов в определённой последовательности, которая будет соблюдена при поиске подстрок. 
        /// Не может быть NULL или пустым, но может содержать единственный элемент.</param>
        /// <param name="endTokensSearchDirection">Задаёт направление поиска конечных токенов после того, как найдены все начальные. 
        /// FromEndToStart - поиск ведётся от самого конца строки и продвигается до последнего начального токена до тех пор, пока не найдёт все конечные токены. 
        /// FromStartToEnd - поиск ведётся от конца последнего начального токена до конца строки.</param>
        /// <param name="startIndex">Позиция (включительная) начала поиска во входной строке. Если 0 - поиск ведётся с начала. 
        /// Если меньше 0 или больше длины входной строки - выбрасывается исключение.</param>
        /// <param name="compOpt">Опции сравнения строк между собой</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>Искомая подстрока. Если не найдена, возвращается NULL.</returns>
        public static Substring GetInnerStringBetweenTokensSet(String input, String[] startTokens, String[] endTokens,
            StringTools.Direction endTokensSearchDirection, Int32 startIndex, StringComparison compOpt)
        {
            if(input == null){throw new ArgumentNullException("input");}
            if (input.IsStringNullEmptyWhiteSpace() == true)
            { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "input"); }

            if (startTokens.IsNullOrEmpty())
            { throw new ArgumentException("Список начальных токенов не может быть NULL или пустым", "startTokens"); }
            if (endTokens.IsNullOrEmpty())
            { throw new ArgumentException("Список конечных токенов не может быть NULL или пустым", "endTokens"); }
            if (startIndex < 0) { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Позиция начала поиска не может быть меньше 0"); }
            if (startIndex >= input.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex", startIndex,
                  String.Format("Позиция начала поиска ('{0}') не может быть больше или равна длине строки ('{1}')", startIndex, input.Length));
            }
            if (Enum.IsDefined(typeof(StringTools.Direction), endTokensSearchDirection) == false)
            { throw new InvalidEnumArgumentException("endTokensSearchDirection", (Int32)endTokensSearchDirection, typeof(StringTools.Direction)); }

            if (Enum.IsDefined(typeof(StringComparison), (Int32)compOpt) == false)
            { throw new InvalidEnumArgumentException("compOpt", (Int32)compOpt, typeof(StringComparison)); }

            Int32 internalOffset = startIndex;

            for (Int32 oneStartTokenIndex = 0; oneStartTokenIndex < startTokens.Length; oneStartTokenIndex++)
            {
                String currentStartToken = startTokens[oneStartTokenIndex];
                Int32 currentStartTokenPos = input.IndexOf(currentStartToken, internalOffset, compOpt);
                if (currentStartTokenPos == -1)
                {
                    return null;
                }
                else
                {
                    internalOffset = currentStartTokenPos + currentStartToken.Length;
                }
            }
            Int32 finalSubstrStartIndex = internalOffset;

            if (endTokensSearchDirection == StringTools.Direction.FromEndToStart)
            {
                Int32 endOffset = input.Length - 1;

                for (Int32 oneEndTokenIndex = endTokens.Length - 1; oneEndTokenIndex >= 0; oneEndTokenIndex--)
                {
                    String currentEndToken = endTokens[oneEndTokenIndex];
                    Int32 countToSearch = endOffset - finalSubstrStartIndex;
                    Int32 currentEndTokenPos = input.LastIndexOf(currentEndToken, endOffset, countToSearch, compOpt);
                    if (currentEndTokenPos == -1)
                    {
                        return null;
                    }
                    else
                    {
                        endOffset = currentEndTokenPos;
                    }
                }
                return Substring.FromIndexToIndex(input, finalSubstrStartIndex, endOffset - 1);
            }
            else// if (EndTokensSearchDirection == StringTools.Direction.FromStartToEnd)
            {
                Int32 finalSubstrEndIndex = 0;

                for (Int32 oneEndTokenIndex = 0; oneEndTokenIndex < endTokens.Length; oneEndTokenIndex++)
                {
                    String currentEndToken = endTokens[oneEndTokenIndex];

                    Int32 currentEndTokenPos = input.IndexOf(currentEndToken, internalOffset, compOpt);
                    if (currentEndTokenPos == -1)
                    {
                        return null;
                    }
                    internalOffset = currentEndTokenPos + currentEndToken.Length;
                    if (finalSubstrEndIndex == 0)
                    {
                        finalSubstrEndIndex = currentEndTokenPos;
                    }
                }
                return Substring.FromIndexToIndex(input, finalSubstrStartIndex, finalSubstrEndIndex - 1);
            }
        }

        /// <summary>
        /// Возвращает часть указанной входной строки, которая размещена от начала или от конца и до первого указанного токена
        /// </summary>
        /// <param name="input">Входящая строка, из которой надо извлечь подстроку</param>
        /// <param name="token">Токен, который определяет конец подстроки</param>
        /// <param name="leaveToken">Если "true" - ближайший токен будет помещён в возвращаемую подстроку. 
        /// Если "false" - он будет отсутствовать в возвращаемой подстроке.</param>
        /// <param name="dir">Направление, с которого будет возвращена подстрока: из начала или из конца</param>
        /// <param name="compareOptions">Опции сравнения строк между собой</param>
        /// <returns>Экземпляр подстроки или NULL, если во входной строке не был найден указанный токен.</returns>
        public static Substring GetSubstringToToken(String input, String token, Boolean leaveToken, StringTools.Direction dir, StringComparison compareOptions)
        {
            if (input.IsStringNullOrEmpty() == true) { throw new ArgumentException("Входная строка не может быть NULL или пустой", "input"); }
            if (token.IsStringNullOrEmpty() == true) { throw new ArgumentException("Токен не может быть NULL или пустой строкой", "token"); }
            Substring output;
            switch (dir)
            {
                case StringTools.Direction.FromStartToEnd:
                    Int32 indexOfFirstToken = input.IndexOf(token, compareOptions);
                    if (indexOfFirstToken < 0 || (indexOfFirstToken == 0 && leaveToken == false))
                    {
                        output = null;
                    }
                    else
                    {
                        output = Substring.FromStartToIndex(input,
                        leaveToken ? indexOfFirstToken + token.Length - 1 : indexOfFirstToken - 1);
                    }
                    break;
                case StringTools.Direction.FromEndToStart:
                    Int32 indexOfLastToken = input.LastIndexOf(token, compareOptions);
                    if (indexOfLastToken < 0)
                    {
                        output = null;
                    }
                    else
                    {
                        output = Substring.FromIndexToEnd(input,
                            leaveToken ? indexOfLastToken : indexOfLastToken + token.Length);
                    }
                    break;
                default:
                    throw new InvalidEnumArgumentException("dir", (Int32)dir, dir.GetType());
            }
            return output;
        }

        /// <summary>
        /// Возвращает подстроку из входной строки, которая отчитывается от начала или от конца и до указанного вхожденя указанного токена.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="token"></param>
        /// <param name="number">Номер вхождения указанного токена, начиная с 1</param>
        /// <param name="dir"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static String GetSubstringToTokenWithSpecifiedNumber(String input, String token, Byte number, StringTools.Direction dir, StringComparison comparisonType)
        {
            if (input.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "input"); }
            if (token.IsStringNullOrEmpty() == true) { throw new ArgumentException("Токен не может быть NULL или пустой строкой", "token"); }
            if (Enum.IsDefined(comparisonType.GetType(), comparisonType) == false)
            { throw new InvalidEnumArgumentException("comparisonType", (Int32)comparisonType, comparisonType.GetType()); }
            if (String.Compare(input, token, comparisonType) == 0) { throw new ArgumentException("Входная строка не может быть равна токену"); }
            if (input.Contains(token, comparisonType) == false) { return input; }
            if (number == 0) { throw new ArgumentOutOfRangeException("number", "Номер вхождения указанного токена не может быть нулевой"); }
            Int32 numsOfOccureses = StringTools.StringAnalyzers.GetNumberOfOccurencesInString(input, token, comparisonType);
            if (number > numsOfOccureses) { throw new ArgumentOutOfRangeException("number", number, "Указанная позиция больше, чем количество вхождений токена в строке"); }

            List<int> positions = StringTools.StringAnalyzers.GetPositionsOfTokenInString(input, token, comparisonType);

            Int32 tokenLength = token.Length;
            Int32 desiredPosition;
            switch (dir)
            {
                case StringTools.Direction.FromStartToEnd:
                    desiredPosition = positions[number - 1];
                    return input.Substring(0, desiredPosition);
                case StringTools.Direction.FromEndToStart:
                    desiredPosition = positions[positions.Count - number];
                    return input.Substring(desiredPosition + tokenLength);
                default:
                    throw new InvalidEnumArgumentException("dir", (Int32)dir, dir.GetType());
            }
        }

        /// <summary>
        /// Возвращает подстроку из входной строки, которая отчитывается от указанной позиции и до указанного вхожденя указанного токена.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="token">Номер вхождения указанного токена, начиная с 1</param>
        /// <param name="number"></param>
        /// <param name="startPosition"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static String GetSubstringToTokenWithSpecifiedNumber(String input, String token, Byte number, Int32 startPosition, StringComparison comparisonType)
        {
            if (input.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "input"); }
            if (token.IsStringNullOrEmpty() == true) { throw new ArgumentException("Токен не может быть NULL или пустой строкой", "token"); }
            if (Enum.IsDefined(comparisonType.GetType(), comparisonType) == false)
            { throw new InvalidEnumArgumentException("comparisonType", (Int32)comparisonType, comparisonType.GetType()); }
            if (String.Compare(input, token, comparisonType) == 0) { throw new ArgumentException("Входная строка не может быть равна токену"); }
            if (input.Contains(token, comparisonType) == false) { return input; }
            if (number == 0) { throw new ArgumentOutOfRangeException("number", "Номер вхождения указанного токена не может быть нулевой"); }
            if (startPosition < 0) { throw new ArgumentOutOfRangeException("startPosition", startPosition, "Начальная позиция не может быть меньше 0"); }
            String sub = input.Substring(startPosition);
            Int32 numsOfOccureses = StringTools.StringAnalyzers.GetNumberOfOccurencesInString(sub, token, comparisonType);
            if (number > numsOfOccureses) { throw new ArgumentOutOfRangeException("number", "Указанная бозиция больше, чем количество вхождений токена в части строки"); }

            List<int> positions = StringTools.StringAnalyzers.GetPositionsOfTokenInString(sub, token, comparisonType);
            Int32 desiredPosition = positions[number - 1];
            return sub.Substring(0, desiredPosition);
        }

        /// <summary>
        /// Возвращает исходную строку, в которой обрезана её часть от начала или от конца до ближайшего указанного токена
        /// </summary>
        /// <param name="input">Входящая строка, из которой надо извлечь подстроку</param>
        /// <param name="token">Токен, который определяет точку обрезания и также обрезается</param>
        /// <param name="leaveToken">Если "true" - ближайший токен будет оставлен. Если "false" - он тоже будет удалён.</param>
        /// <param name="dir">Направление, с которого будет обрезана подстрока: из начала или из конца</param>
        /// <param name="comparisonType">Опции сравнения строк между собой</param>
        /// <returns></returns>
        public static String TruncateToClosestToken(String input, String token, Boolean leaveToken, StringTools.Direction dir, StringComparison comparisonType)
        {
            if (input.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Входная строка не может быть NULL, пустой или состоящей из одних пробелов", "input"); }
            if (token.IsStringNullOrEmpty() == true) { throw new ArgumentException("Токен не может быть NULL или пустой строкой", "token"); }
            if (Enum.IsDefined(comparisonType.GetType(), comparisonType) == false)
            { throw new InvalidEnumArgumentException("comparisonType", (Int32)comparisonType, comparisonType.GetType()); }
            if (String.Compare(input, token, comparisonType) == 0) { throw new ArgumentException("Входная строка не может быть равна токену"); }
            if (input.Contains(token, comparisonType) == false) { return input; }
            Int32 tokenLength = token.Length;
            switch (dir)
            {
                case StringTools.Direction.FromStartToEnd:
                    Int32 indexOfFirstToken = input.IndexOf(token, comparisonType);
                    if (leaveToken == true)
                    {
                        return input.Remove(0, indexOfFirstToken);
                    }
                    else
                    {
                        return input.Remove(0, indexOfFirstToken + tokenLength);
                    }

                case StringTools.Direction.FromEndToStart:

                    Int32 indexOfLastToken = input.LastIndexOf(token, comparisonType);
                    if (leaveToken == true)
                    {
                        return input.Remove(indexOfLastToken + tokenLength);
                    }
                    else
                    {
                        return input.Remove(indexOfLastToken);
                    }
                default:
                    throw new InvalidEnumArgumentException("dir", (Int32)dir, dir.GetType());
            }
        }

        /// <summary>
        /// Удаляет из входной строки указанную начальную и конечную подстроки, если они есть. Если нет хотя бы одной из них, то возвращается исходная строка. 
        /// Если во входной строке содержится множество вложенных один в другой начальных и конечных токенов, метод удалит их все рекурсивно.
        /// </summary>
        /// <param name="input">Входная строка, из которой необходимо удалить все указанные начальные и конечные токены.</param>
        /// <param name="startToken">Начальный токен</param>
        /// <param name="endToken">Конечный токен</param>
        /// <param name="compOpt"></param>
        /// <returns>Новая строка, содержащая копию старой с удалёнными токенами</returns>
        public static String DeleteStartAndEndTokens(String input, String startToken, String endToken, StringComparison compOpt)
        {
            if (startToken.IsNullOrEmpty() == true) { throw new ArgumentException("Начальный токен не может быть NULL или пустой строкой", "input"); }
            if (endToken == null) { throw new ArgumentException("Конечный токен не может быть NULL или пустой строкой", "endToken"); }

            if (input.IsStringNullEmptyWhiteSpace() == true) { return input; }

            if (input.StartsWith(startToken, compOpt) == true && input.EndsWith(endToken, compOpt) == true)
            {
                input = input.Remove(0, startToken.Length);
                input = input.Remove(input.Length - endToken.Length, endToken.Length);
                return DeleteStartAndEndTokens(input, startToken, endToken, compOpt);
            }
            else
            {
                return input;
            }
        }

        /// <summary>
        /// Удаляет из входной строки все символы, которые не подпадают под допустимые
        /// </summary>
        /// <param name="input">Входная строка, которую следует "почистить". Если NULL или пустая, возвращается без изменений</param>
        /// <param name="allowedSymbols">Допустимые клссы символов. Если содержит неопределённое значение, входная строка возвращается без изменений.</param>
        /// <returns></returns>
        public static String CleanFromChars(String input, StringTools.StringAnalyzers.ContainsEntities allowedSymbols)
        {
            if (input.IsStringNullOrEmpty() == true || allowedSymbols == StringTools.StringAnalyzers.ContainsEntities.Empty) { return input; }
            StringBuilder output = new StringBuilder(input.Length);
            for (Int32 i = 0; i < input.Length; i++)
            {
                Char current = input[i];
                if (Char.IsControl(current) == true && allowedSymbols.Misses(StringTools.StringAnalyzers.ContainsEntities.Controls) == true)
                { continue; }
                if (
                    (allowedSymbols.Contains(StringTools.StringAnalyzers.ContainsEntities.Letters) == true && Char.IsLetter(current) == true) ||
                    (allowedSymbols.Contains(StringTools.StringAnalyzers.ContainsEntities.Digits) == true && Char.IsDigit(current) == true) ||
                    (allowedSymbols.Contains(StringTools.StringAnalyzers.ContainsEntities.Spaces) == true && Char.IsWhiteSpace(current) == true) ||
                    (allowedSymbols.Contains(StringTools.StringAnalyzers.ContainsEntities.Controls) == true && Char.IsControl(current) == true)
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
        /// <param name="input"></param>
        /// <returns></returns>
        public static String SecureSqlQuery(String input)
        {
            if (input.HasVisibleChars() == false) { return input; }
            String output = input.Replace("'", "''");
            return output;
        }

        /// <summary>
        /// Конкатенирует все указанные строки в одну и дублирует её указанное количество раз, конкатенируя каждый новый дубль к предыдущему
        /// </summary>
        /// <remarks>Детерминированный бессмертный метод, не бросающий исключений</remarks>
        /// <param name="iterationsCount">Количество дублей конкатенированных строк, которые необходимо совершить. Если равен 0, возвращается пустая строка.</param>
        /// <param name="input">Массив от одной включительно и больше строк, которые необходимо конкатенировать. 
        /// Если не содержит ни одной строки, не являющейся NULL или пустой, метод возвращает пустую строку.</param>
        /// <returns>Одна строка</returns>
        public static String ConcatenateAllStringsManyTimes(Int32 iterationsCount, params String[] input)
        {
            if (iterationsCount < 0) { throw new ArgumentOutOfRangeException("iterationsCount", iterationsCount, "Количество итераций не может быть меньше 0"); }
            if (iterationsCount <= 0) { return String.Empty; }
            if (input.IsNullOrEmpty() == true) { return String.Empty; }
            StringBuilder temp = new StringBuilder(iterationsCount * input.Length * 4);
            for (UInt32 i = 0; i < iterationsCount; i++)
            {
                Boolean isExistsValid = false;
                foreach (String s in input)
                {
                    if (s.IsStringNullOrEmpty() == false)
                    {
                        temp.Append(s);
                        isExistsValid = true;
                    }
                }
                if (isExistsValid == false) { return String.Empty; }
            }
            return temp.ToString();
        }

        /// <summary>
        /// Уплотняет пробелы в указанной строке, сокращая множественные идущие подряд пробелы до одного
        /// </summary>
        /// <param name="input">Строка, которую следует уплотнить. Если NULL или пустая - будет возвращена без изменений.</param>
        /// <returns></returns>
        public static String ShrinkSpaces(String input)
        {
            if (input.IsNullOrEmpty() == true) { return input; }
            StringBuilder output = new StringBuilder(input.Length);
            const Char space = ' ';
            Int32 spacesCount = 0;
            foreach (Char c in input)
            {
                if (Char.IsControl(c) == true || Char.IsWhiteSpace(c) == false)
                {
                    if (spacesCount > 0)
                    {
                        spacesCount = 0;
                        output.Append(space);
                    }
                    output.Append(c);
                }
                else
                {
                    spacesCount++;
                }
            }
            if (spacesCount > 0)
            {
                output.Append(space);
            }
            return output.ToString();
        }
    }//end of class SubstringHelpers
}

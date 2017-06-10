using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KlotosLib.StringTools
{
    /// <summary>
    /// Содержит чистые статические методы, изменяющие строки тем или иным образом
    /// </summary>
    public static class StringModifiers
    {
        /// <summary>
        /// Изменяет первый символ строки на заглавный
        /// </summary>
        /// <param name="input">Входная строка.</param>
        /// <returns></returns>
        public static String FirstLetterToUpper(String input)
        {
            if (input.IsStringNullEmptyWs() == true) { return input; }

            if (input.Length > 1)
            { return Char.ToUpper(input[0]) + input.Substring(1); }
            else
            { return input.ToUpper(); }
        }

        /// <summary>
        /// Заменяет в указанной входной строке все указанные её подстроки на соответствующие им новые подстроки
        /// </summary>
        /// <param name="input">Входная строка, в которой требуется заменить содержимое определённых подстрок. Не может быть NULL или пустой.</param>
        /// <param name="replacementList">Список замен, которые необходимо произвести во входной строке. 
        /// Ключ - это подстрока для замены, которая обязательно должна иметь в качестве базовой строки указанную входную строку.  
        /// Значение - это новая строка, которая должна быть внедрена во входную строку вместо соответствующей ей в ключе подстроки на те же самые позиции. 
        /// Если новая строка в значении является NULL или пустой, произойдёт фактически вырезание из входной строки подстроки, без замены содержимого на новое. 
        /// Если список замен является NULL или пустым, будет возвращена входная строка без изменений.</param>
        /// <returns>Новый экземпляр строки, если замены произошли, или входная строка, если список замен пуст</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static String ReplaceAll(String input, IDictionary<Substring, String> replacementList)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (input.Length == 0) { throw new ArgumentException("Входная строка не может быть пустой", "input"); }
            if (replacementList.IsNullOrEmpty()) { return input; }
            Substring[] substrings = replacementList.Keys.ToArray();
            if (substrings.Length > 1 && Substring.HaveCommonBaseString(substrings) == false)
            {
                throw new ArgumentException("Не все подстроки из списка замен имеют в качестве базовой строки указанную входную строку", "replacementList");
            }
            StringBuilder output = new StringBuilder(input.Length);
            Int32 offset = 0;
            Substring previous = null;
            foreach (KeyValuePair<Substring, string> onePairToReplace in replacementList)
            {
                if (previous != null && Substring.AreIntersecting(previous, onePairToReplace.Key) == true)
                {
                    throw new ArgumentException("В списке замен присутствуют пересекающиеся подстроки", "replacementList");
                }
                output.Append(input.Substring(offset, onePairToReplace.Key.StartIndex - offset));
                output.Append(onePairToReplace.Value);
                offset = onePairToReplace.Key.EndIndex + 1;
                previous = onePairToReplace.Key;
            }
            output.Append(input.Substring(offset));
            return output.ToString();
        }
    }
}

#define Debug

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Web;

namespace KlotosLib
{
    /// <summary>
    /// Содержит статические чистые методы по работе с XML и HTML
    /// </summary>
    public static class HtmlTools
    {
        /// <summary>
        /// Удаляет из входной строки все одинарные и парные HTML-тэги с их атрибутами, оставляя их содержимое. Если входная строка не содержит HTML-тэгов, метод возвращает её без изменений. 
        /// Удаляются также некорректно расположенные HTML-тэги (неоткрытые, незакрытые и перехлестывающиеся).
        /// </summary>
        /// <param name="InputHTML">Входная HTML-содержащая строка</param>
        /// <returns>Копия входной строки, не содержащая никаких HTML-тегов</returns>
        public static String RemoveHTMLTags(String InputHTML)
        {
            if (InputHTML.HasVisibleChars() == false) { return InputHTML; }
            if (StringTools.ContainsHelpers.ContainsAllOf(InputHTML, new char[] { '<', '>' }) == false) { return InputHTML; }
            Dictionary<Int32, String> tags_with_positions = StringTools.SubstringHelpers.GetInnerStringsBetweenTokens(InputHTML, "<", ">", 0, StringComparison.OrdinalIgnoreCase);
            if (tags_with_positions.Any() == false) { return InputHTML; }

            StringBuilder temp = new StringBuilder(InputHTML.Length);
            Int32 start_position = 0;

            foreach (KeyValuePair<int, string> one_possible_tag in tags_with_positions)
            {
                if (CommonTools.AreAllEqual<Int32>(start_position, one_possible_tag.Key - 1, 0) == true)
                {
                    start_position = one_possible_tag.Key + one_possible_tag.Value.Length + 1;
                    continue;
                }
                if (CommonTools.AreAllEqual<Int32>(start_position, one_possible_tag.Key - 1) == true)
                {
                    start_position = one_possible_tag.Key + one_possible_tag.Value.Length + 1;
                    continue;
                }
                temp.Append(InputHTML.SubstringWithEnd(start_position, one_possible_tag.Key - 1, true, false, true));
                start_position = one_possible_tag.Key + one_possible_tag.Value.Length + 1;
            }
            return temp.ToString();
        }

        /// <summary>
        /// Удаляет из входной строки все парные HTML-тэги со всеми их атрибутами, которые не содержат внутри контента. 
        /// HTML-тэги, содержащие вложенные пустые тэги, также удаляются. Одиночные тэги не удаляются.
        /// </summary>
        /// <param name="InputHTML">Входная строка</param>
        /// <returns>Новая очищенная от пустых парных тэгов строка</returns>
        public static String RemoveEmptyPairHTMLTags(String InputHTML)
        {
            if (InputHTML.HasVisibleChars() == false) { return InputHTML; }
            const Char opening_bracket = '<';
            const Char closing_bracket = '>';
            const String closing_token = "</";
            const Char space = ' ';
            StringBuilder output = new StringBuilder(InputHTML.Length);
            Int32 offset = 0;

            while (true)
            {
                Int32 index_of_opening_bracket_opening_tag = InputHTML.IndexOf(opening_bracket, offset);
                if (index_of_opening_bracket_opening_tag == -1) { break; }

                Int32 index_of_closing_bracket_opening_tag = InputHTML.IndexOf(closing_bracket, index_of_opening_bracket_opening_tag + 1);
                if (index_of_closing_bracket_opening_tag == -1) { break; }

                String opening_tag = InputHTML.SubstringWithEnd(index_of_opening_bracket_opening_tag + 1, index_of_closing_bracket_opening_tag).Trim();
                if (opening_tag.Contains(space) == true)
                { opening_tag = opening_tag.Split(new Char[1] { space }, StringSplitOptions.RemoveEmptyEntries)[0]; }

                Int32 index_of_opening_bracket_closing_tag = InputHTML.IndexOf(closing_token + opening_tag, index_of_closing_bracket_opening_tag + 1, StringComparison.OrdinalIgnoreCase);
                if (index_of_opening_bracket_closing_tag == -1)
                {
                    String temp = InputHTML.SubstringWithEnd(offset, index_of_closing_bracket_opening_tag + 1);
                    output.Append(temp);
                    offset = index_of_closing_bracket_opening_tag + 1;
                    continue;
                }
                Int32 index_of_closing_bracket_closing_tag = InputHTML.IndexOf(closing_bracket, index_of_opening_bracket_closing_tag + 1);
                if (index_of_closing_bracket_closing_tag == -1)
                { break; }
#if Debug
                String closing_tag = InputHTML.SubstringWithEnd(index_of_opening_bracket_closing_tag + 2, index_of_closing_bracket_closing_tag).Trim();
#endif
                String between_offset_and_opening_tag = InputHTML.SubstringWithEnd(offset, index_of_opening_bracket_opening_tag);
                output.Append(between_offset_and_opening_tag);
                String inner = InputHTML.SubstringWithEnd(index_of_closing_bracket_opening_tag + 1, index_of_opening_bracket_closing_tag);
                if (inner.HasVisibleChars() == true)
                {
                    String temp = RemoveEmptyPairHTMLTags(inner);
                    if (temp.HasVisibleChars() == true)
                    {
                        String temp2 = InputHTML.SubstringWithEnd(index_of_opening_bracket_opening_tag, index_of_closing_bracket_opening_tag + 1) +
                            temp +
                            InputHTML.SubstringWithEnd(index_of_opening_bracket_closing_tag, index_of_closing_bracket_closing_tag + 1);
                        output.Append(temp2);
                    }
                }
                offset = index_of_closing_bracket_closing_tag + 1;
            }
            if (offset == 0)
            { output.Append(InputHTML); }
            else if (offset > 0 && offset < InputHTML.Length)
            { output.Append(InputHTML.Substring(offset + 1)); }

            String result = output.ToString();
            return result;
        }

        /// <summary>
        /// Конвертирует указанную строку в HTML-совместимую форму, заменяя пробельные отступы и символы переноса, а также кодируя HTML-тэги, защищая клиент от атак XSS
        /// </summary>
        /// <param name="InputHTML"></param>
        /// <returns></returns>
        public static String AdjustTextToHtml(String InputHTML)
        {
            if (InputHTML.HasVisibleChars() == false) { return InputHTML; }
            InputHTML = HttpUtility.HtmlEncode(InputHTML);
            const Char ordinal_space = ' ';
            const Char carriage_return = '\r';
            const Char new_line = '\n';
            const String combined_enter = "\r\n";
            const String html_space = "&ensp;";
            const String html_enter = "<br/>";

            String temp = InputHTML
                .Replace(combined_enter.ToString(), html_enter)
                .Replace(carriage_return.ToString(), html_enter)
                .Replace(new_line.ToString(), html_enter);
            StringBuilder output = new StringBuilder(temp.Length);
            Int32 start_spaces = StringTools.StringAnalyzers.StartWithCount(temp, ordinal_space);
            Int32 start_index;
            if (start_spaces == 0)
            {
                start_index = 0;
            }
            else
            {
                output.Append(html_space.Replicate((UInt16)start_spaces));
                start_index = start_spaces;
            }
            Int32 found_spaces_count = 0;
            for (Int32 i = start_index; i < temp.Length; i++)
            {
                Char ch = temp[i];
                if (ch.Equals(ordinal_space) || Char.IsWhiteSpace(ch) == true)
                {
                    found_spaces_count = found_spaces_count + 1;
                    continue;
                }
                if (found_spaces_count > 1)
                {
                    output.Append(html_space.Replicate(found_spaces_count) + ch.ToString());
                    found_spaces_count = 0;
                    continue;
                }
                if (found_spaces_count == 1)
                {
                    output.Append(ordinal_space.ToString() + ch.ToString());
                    found_spaces_count = 0;
                    continue;
                }
                //found_spaces_count == 0
                output.Append(ch);
            }
            return output.ToString();
        }

        /// <summary>
        /// Анализирует входную XML-разметку на наличие некорректных парных тэгов, и если такие найдены, то исправляет их в корректной последовательности. 
        /// Если во входной строке нет некорректных парных тэгов или нет тэгов вообще, метод возвращает её без изменений.
        /// </summary>
        /// <param name="InputXML">Входная строка, содержащая предположительно некорректную XML-разметку</param>
        /// <returns>Гарантированно корректная XML-разметка</returns>
        public static String FixBrokenXMLTags(String InputXML)
        {
            if (InputXML.IsStringNullEmptyWhiteSpace() == true) { return InputXML; }
            if (StringTools.ContainsHelpers.ContainsAllOf(InputXML, new char[] { '<', '>' }) == false) { return InputXML; }

            Dictionary<Int32, String> tags_with_positions = StringTools.SubstringHelpers.GetInnerStringsBetweenTokens(InputXML, "<", ">", 0, StringComparison.OrdinalIgnoreCase);
            if (tags_with_positions.Any() == false) { return InputXML; }

            StringBuilder output = new StringBuilder(InputXML.Length);
            Stack<String> open_tags = new Stack<String>();
            Int32 start_position = 0;

            foreach (KeyValuePair<int, string> one_possible_tag in tags_with_positions)
            {
                String tag_name;
                HtmlTools.HtmlTagType tag_type = HtmlTools.ValidateHtmlTag("<" + one_possible_tag.Value + ">", out tag_name);
                if (tag_type != HtmlTagType.PairClose)//если предполагаемый тэг не является тэгом, или одиночный, или парный открывающий
                {
                    //добавляем в выводную строку часть исходной строки, начиная от позиции поиска и заканчивая концом просканированной части
                    output.Append(InputXML.SubstringWithEnd(start_position, one_possible_tag.Key + one_possible_tag.Value.Length + 1));
                    //устанавливаем новое значение позиции поиска, равное позиции начала вхождения тела потенциального тэга и заканчивая концом закрывающей скобки, следуемой после тела потенциального тэга
                    start_position = one_possible_tag.Key + one_possible_tag.Value.Length + 1;
                    if (tag_type == HtmlTagType.PairOpen)//если предполагаемый тэг является парным открывающим
                    {
                        open_tags.Push(tag_name);//добавляем его в стэк
                    }
                }
                else//если предполагаемый тэг является парным закрывающим
                {
                    if (open_tags.Any() == false || open_tags.Peek() != tag_name)//закрывающий тэг не соответствует последнему открывающему или нет незакрытых тэгов
                    {
                        //добавляем в выводную строку часть исходной строки, начиная от позиции поиска и заканчивая началом открывающей скобки неоткрытого закрывающего тэга
                        if (one_possible_tag.Key - 1 > start_position)
                        {
                            output.Append(InputXML.SubstringWithEnd(start_position, one_possible_tag.Key - 1));
                        }
                        //устанавливаем новое значение позиции поиска, равное позиции начала вхождения тела потенциального тэга и заканчивая концом закрывающей скобки, 
                        //следуемой после тела потенциального тэга
                        start_position = one_possible_tag.Key + one_possible_tag.Value.Length + 1;
                    }
                    else//закрывающий тэг соответствует последнему открывающему
                    {
                        //удалить открывающий из стэка
                        open_tags.Pop();
                        //добавляем в выводную строку часть исходной строки, начиная от позиции поиска и заканчивая концом просканированной части
                        output.Append(InputXML.SubstringWithEnd(start_position, one_possible_tag.Key + one_possible_tag.Value.Length + 1));
                        //устанавливаем новое значение позиции поиска, равное позиции начала вхождения тела потенциального тэга и заканчивая концом закрывающей скобки, 
                        //следуемой после тела потенциального тэга
                        start_position = one_possible_tag.Key + one_possible_tag.Value.Length + 1;
                    }
                }
            }
            if (start_position < InputXML.Length)
            {
                output.Append(InputXML.Substring(start_position));
            }
            while (open_tags.Any() == true)
            {
                output.Append("</" + open_tags.Pop() + ">");
            }
            return output.ToString();
        }

        /// <summary>
        /// Определяет типы HTML-тэгов
        /// </summary>
        public enum HtmlTagType : byte
        {
            /// <summary>
            /// Не является тэгом
            /// </summary>
            NotTag = 0,

            /// <summary>
            /// Одиночный тэг
            /// </summary>
            Single = 1,

            /// <summary>
            /// Открывающая часть парного тэга
            /// </summary>
            PairOpen = 2,

            /// <summary>
            /// Закрывающая часть парного тэга
            /// </summary>
            PairClose = 3
        }

        /// <summary>
        /// Определяет, является ли входная строка корректным HTML-тэгом и если является, то каким: одиночным, парным открывающим или парным закрывающим.
        /// </summary>
        /// <param name="Tag">Строка, предположительно содержащая тэг. Поддерживаются любые строки, включая NULL. Любые символы, кроме пробелов, не являющися часть тэга, приводят к выводу NotTag.</param>
        /// <param name="TagName">Точное и чёткое имя тэга в нижнем регистре, без кавычек, пробелов и атрибутов, если они есть. Если входная строка не распознана как коректный тэг, содержит NULL.</param>
        /// <returns>Одно из значений перечисления, определяющее тип тэга</returns>
        public static HtmlTagType ValidateHtmlTag(String Tag, out String TagName)
        {
            TagName = null;
            if (Tag.IsStringNullEmptyWhiteSpace() == true) { return HtmlTagType.NotTag; }
            if (StringTools.ContainsHelpers.ContainsAllOf(Tag, new char[2] { '<', '>' }) == false) { return HtmlTagType.NotTag; }
            String tag_temp = Tag.Trim();
            if (tag_temp.StartsWith("<", StringComparison.OrdinalIgnoreCase) == false || tag_temp.EndsWith(">", StringComparison.OrdinalIgnoreCase) == false) { return HtmlTagType.NotTag; }
            if (StringTools.StringAnalyzers.GetNumberOfOccurensesInString(tag_temp, '<') != 1 ||
                StringTools.StringAnalyzers.GetNumberOfOccurensesInString(tag_temp, '>') != 1) { return HtmlTagType.NotTag; }
            //На данном этапе тэг коректен. Находим TagName.
            TagName = StringTools.SubstringHelpers.GetInnerStringsBetweenTokens(Tag, "<", ">", StringComparison.OrdinalIgnoreCase).Single();
            TagName = TagName.Replace("/", String.Empty);
            if (TagName.Contains(" ", StringComparison.OrdinalIgnoreCase) == true)
            {
                TagName = TagName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).First();
            }
            tag_temp = StringTools.SubstringHelpers.GetInnerStringsBetweenTokens(tag_temp, "<", ">", StringComparison.OrdinalIgnoreCase).Single().Trim();
            if (tag_temp.IndexOf('/') == 0 && tag_temp.LastIndexOf('/') == tag_temp.Length - 1)
            {
                return HtmlTagType.NotTag;
            }
            if (tag_temp.LastIndexOf('/') == tag_temp.Length - 1)
            {
                return HtmlTagType.Single;
            }
            if (tag_temp.IndexOf('/') == 0)
            {
                return HtmlTagType.PairClose;
            }
            return HtmlTagType.PairOpen;
        }

        /// <summary>
        /// Экранирует во входной строке все тэги &lt;script&gt; и &lt;/script&gt; для защиты от самых распространённых XSS-инъекций.
        /// </summary>
        /// <param name="InputHTML"></param>
        /// <returns></returns>
        public static String SecureScriptXSS(String InputHTML)
        {
            if (InputHTML.HasVisibleChars() == false) { return InputHTML; }
            if (StringTools.ContainsHelpers.ContainsAllOf(InputHTML, new char[] { '<', '>' }) == false) { return InputHTML; }
            Dictionary<Int32, String> tags_with_positions = StringTools.SubstringHelpers.GetInnerStringsBetweenTokens(InputHTML, "<", ">", 0, StringComparison.OrdinalIgnoreCase);
            if (tags_with_positions.Any() == false) { return InputHTML; }

            StringBuilder output = new StringBuilder(InputHTML.Length);
            Int32 start_position = 0;

            foreach (KeyValuePair<int, string> one_possible_tag in tags_with_positions)
            {
                String tag_name;
                HtmlTools.HtmlTagType tag_type = HtmlTools.ValidateHtmlTag("<" + one_possible_tag.Value + ">", out tag_name);
                if (tag_type == HtmlTagType.NotTag || tag_name.Equals("script", StringComparison.OrdinalIgnoreCase) == false)
                {
                    output.Append(InputHTML.SubstringWithEnd(start_position, one_possible_tag.Key + one_possible_tag.Value.Length + 1));
                }
                else
                {
                    if (tag_type == HtmlTagType.PairOpen)
                    {
                        output.Append(InputHTML.Substring(start_position, one_possible_tag.Key - 1 - start_position));
                        output.Append("&lt;script&gt;");
                    }
                    else if (tag_type == HtmlTagType.PairClose)
                    {
                        output.Append(InputHTML.Substring(start_position, one_possible_tag.Key - 1 - start_position));
                        output.Append("&lt;/script&gt;");
                    }
                    else if (tag_type == HtmlTagType.Single)
                    {
                        output.Append(InputHTML.Substring(start_position, one_possible_tag.Key - 1 - start_position));
                        output.Append("&lt;script/&gt;");
                    }
                    else
                    {
                        throw new UnreachableCodeException();
                    }
                }
                start_position = one_possible_tag.Key + one_possible_tag.Value.Length + 1;
            }
            output.Append(InputHTML.Substring(start_position));
            return output.ToString();
        }

        //------------  XmlDocument To XDocument and vice versa  ---------------
        /// <summary>
        /// Конвертирует узел XML-документа формата XmlNode в узел формата XElement (LINQ2XML)
        /// </summary>
        /// <param name="SourceNode"></param>
        /// <returns></returns>
        public static XElement GetXElement(this XmlNode SourceNode)
        {
            XDocument xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter())
            { SourceNode.WriteTo(xmlWriter); }
            return xDoc.Root;
        }

        /// <summary>
        /// Конвертирует узел XML-документа формата XElement (LINQ2XML) в узел формата XmlNode
        /// </summary>
        /// <param name="SourceElement"></param>
        /// <returns></returns>
        public static XmlNode GetXmlNode(this XElement SourceElement)
        {
            using (XmlReader xmlReader = SourceElement.CreateReader())
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }

        /// <summary>
        /// Конвертирует XmlDocument в XDocument
        /// </summary>
        /// <param name="SourceDocument"></param>
        /// <returns></returns>
        public static XDocument GetXDocument(this XmlDocument SourceDocument)
        {
            XDocument xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter())
                SourceDocument.WriteTo(xmlWriter);
            XmlDeclaration decl =
                SourceDocument.ChildNodes.OfType<XmlDeclaration>().FirstOrDefault();
            if (decl != null)
                xDoc.Declaration = new XDeclaration(decl.Version, decl.Encoding,
                    decl.Standalone);
            return xDoc;
        }

        /// <summary>
        /// Конвертирует XDocument в XmlDocument
        /// </summary>
        /// <param name="SourceDocument"></param>
        /// <returns></returns>
        public static XmlDocument GetXmlDocument(this XDocument SourceDocument)
        {
            using (XmlReader xmlReader = SourceDocument.CreateReader())
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                if (SourceDocument.Declaration != null)
                {
                    XmlDeclaration dec = xmlDoc.CreateXmlDeclaration(SourceDocument.Declaration.Version,
                        SourceDocument.Declaration.Encoding, SourceDocument.Declaration.Standalone);
                    xmlDoc.InsertBefore(dec, xmlDoc.FirstChild);
                }
                return xmlDoc;
            }
        }
    }
}

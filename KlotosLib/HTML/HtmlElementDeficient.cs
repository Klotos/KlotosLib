using System;
using System.Collections.Generic;
using System.Text;
using KlotosLib.StringTools;

namespace KlotosLib.HTML
{
    /// <summary>
    /// Представляет один извлечённый из строки или созданный "сам по себе" HTML-элемент с атрибутами и позициями, но без связи с иерархией DOM и элементов-потомков
    /// </summary>
    public class HtmlElementDeficient
    {
        #region Fields

        private readonly string _name;

        private readonly Dictionary<string, string> _attributes;

        private Boolean _isSelfClosed;

        private Substring _origin;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Создааёт один новый HTML-элемент с указанным именем, списком атрибутов и определением самозакрытости, который не связан с базовой HTML-содержащей строкой
        /// </summary>
        /// <param name="name">Имя HTML-элемента, которое впоследствии не может быть изменено. 
        /// Не может быть NULL, пустой строкой или не содержать цифробуквенных символов. </param>
        /// <param name="attributes"></param>
        /// <param name="isSelfClosed"></param>
        public HtmlElementDeficient(string name, Dictionary<string, string> attributes, bool isSelfClosed)
        {
            if (name.HasAlphaNumericChars() == false) 
            { throw new ArgumentException("Имя создаваемого HTML-элемента некорректно, так как не содержит ни одной буквы или цифры", "name"); }
            this._name = name;
            this._attributes = attributes;
            this._isSelfClosed = isSelfClosed;
            this._origin = null;
        }

        private HtmlElementDeficient(string name, Dictionary<string, string> attributes, bool isSelfClosed, Substring origin)
        {
            
            this._name = name;
            this._attributes = attributes;
            this._isSelfClosed = isSelfClosed;
            this._origin = origin;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Имя текущего HTML-элемента (и соответствующего тэга). Не может быть изменено.
        /// </summary>
        public string Name { get { return this._name; } }

        /// <summary>
        /// Определяет, является ли текущий элемент самозакрытым (/) или нет
        /// </summary>
        public bool IsSelfClosed { get { return this._isSelfClosed; } set { this._isSelfClosed = value; } }

        /// <summary>
        /// Показывает наличие или отсутствие в текущем элементе каких-либо атрибутов
        /// </summary>
        public bool HasAttributes { get { return !this._attributes.IsNullOrEmpty(); } }

        /// <summary>
        /// Возвращает словарь всех атрибутов в текущем элементе, позволяя модифицировать их, а также добавлять новые и/или удалять существующие
        /// </summary>
        public Dictionary<string, string> Attributes { get { return this._attributes; } }

        /// <summary>
        /// Определяет, был ли текущий HTML-элемент извлечён из определённого HTML документа и имеет ли связь с ним через строку, 
        /// или же он был создан "сам по себе", или связь с базовым HTML была удалена
        /// </summary>
        public bool AssociatedWithHtmlMarkup { get { return this._origin != null; } }

        /// <summary>
        /// Возвращает подстроку, которая содерджит оригинальное строковое представление текущего HTML-элемента в базовой строке с HTML-документом. 
        /// Если текущий HTML-элемент был создан "сам по себе", или связь с базовым HTML была удалена, возвращает NULL.
        /// </summary>
        public Substring Origin { get { return this._origin; } }
        #endregion Properties

        #region Instance methods

        /// <summary>
        /// Определяет, содержит ли текущий HTML-элемент атрибут с указанным именем, или нет
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public bool HasAttribute(string attributeName)
        {
            if (string.IsNullOrEmpty(attributeName)) { throw new ArgumentException("Указанное имя атрибута не может быть NULL или пустое", "attributeName"); }
            if (CollectionTools.IsNullOrEmpty(this._attributes))
            {
                return false;
            }
            return this._attributes.ContainsKey(attributeName);
        }

        /// <summary>
        /// Удаляет связь между текущим HTML-элементом и его базовым HTML-документом, с которого он был извлечён, если такая связь присутствует
        /// </summary>
        /// <returns>Статус удаления связи: true - связь была успешно удалена, false - связь отсутствовала на момент вызова данного метода</returns>
        public bool Unbound()
        {
            if (this._origin == null)
            {
                return false;
            }
            this._origin = null;
            return true;
        }

        /// <summary>
        /// Сериализует весь текущий HTML-элемент в строку, вместе с тэгами и атрибутами
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            StringBuilder serializedElement = new StringBuilder();
            serializedElement.Append("<" + this._name + " ");
            serializedElement.Append(this.SerializeOnlyAttributes());
            if (this.IsSelfClosed)
            {
                serializedElement.Append('/');
            }
            serializedElement.Append('>');
            return serializedElement.ToString();
        }

        /// <summary>
        /// Сериализует в строку все атрибуты текущего HTML-элемента, но без тэгов
        /// </summary>
        /// <returns></returns>
        public string SerializeOnlyAttributes()
        {
            if (CollectionTools.IsNullOrEmpty(this._attributes))
            {
                return string.Empty;
            }
            StringBuilder serializedAttributes = new StringBuilder();
            foreach (KeyValuePair<string, string> oneAttributeValuePair in this._attributes)
            {
                string oneSerialized = string.IsNullOrEmpty(oneAttributeValuePair.Value)
                    ? oneAttributeValuePair.Key
                    : string.Format("{0}=\"{1}\" ", oneAttributeValuePair.Key, oneAttributeValuePair.Value);
                serializedAttributes.Append(oneSerialized);
            }
            return serializedAttributes.ToString();
        }
        #endregion Instance methods

        #region Static methods

        /// <summary>
        /// Ищет и возвращает первый найденный HTML-элемент с указанным именем
        /// </summary>
        /// <param name="inputHtml">Входная HTML-содержащая строка. Не может быть NULL, пустой строкой или не содержать цифробуквенных символов.</param>
        /// <param name="targetTagName">Имя целевого HTML-элемента, который ищется во входной строке. 
        /// Не может быть NULL, пустой строкой или не содержать цифробуквенных символов.</param>
        /// <param name="startIndex">Начальная позиция входной HTML-содержащей строки, с которой следует начать поиск. Если 0 - поиск ведётся с начала. 
        /// Если меньше 0 или больше длины исходной строки, выбрасывается исключение.</param>
        /// <returns></returns>
        public static HtmlElementDeficient ExtractElementFromHtml(string inputHtml, string targetTagName, int startIndex)
        {
            if (inputHtml == null) { throw new ArgumentNullException("inputHtml"); }
            if (inputHtml.HasAlphaNumericChars() == false)
            { throw new ArgumentException("Входная HTML-содержащая строка не содержит ни одной буквы или цифры и не является валидным HTML документом", "inputHtml"); }
            if (targetTagName.HasAlphaNumericChars() == false)
            { throw new ArgumentException("Имя целевого тэга некорректно, так как не содержит ни одной буквы или цифры", "targetTagName"); }
            if (startIndex < 0) { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Начальный индекс не может быть меньше 0"); }
            if (startIndex >= inputHtml.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex", startIndex,
                  string.Format("Начальный индекс ('{0}') не может быть больше или равен длине строки с HTML-разметкой ('{1}')", startIndex, inputHtml.Length));
            }
            String clearedTagName = targetTagName.Trim().ToLowerInvariant();
            if (clearedTagName.StartsWith("<") == false)
            {
                clearedTagName = "<" + clearedTagName;
            }
            string finalTagName = clearedTagName.TrimStart('<');
            Int32 tagStartPos = inputHtml.IndexOf(clearedTagName, startIndex, StringComparison.OrdinalIgnoreCase);
            if (tagStartPos == -1)
            {
                return null;
            }
            Int32 closingBracketPos = inputHtml.IndexOf(">", tagStartPos + clearedTagName.Length, StringComparison.OrdinalIgnoreCase);
            if (closingBracketPos == -1)
            {
                return null;
            }
            Substring origin = Substring.FromIndexToIndex(inputHtml, tagStartPos, closingBracketPos);
            int selfClosingSlashPos = closingBracketPos;
            bool isSelfClosed = false;
            if (inputHtml[closingBracketPos - 1] == '/')
            {
                selfClosingSlashPos = closingBracketPos - 1;
                isSelfClosed = true;
            }

            string substringWithAttributes = inputHtml.SubstringWithEnd(
                tagStartPos + clearedTagName.Length, selfClosingSlashPos, false, false, false).Trim();
            Dictionary<String, String> output = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            if (substringWithAttributes.IsStringNullEmptyWs())
            {
                return new HtmlElementDeficient(finalTagName, output, isSelfClosed, origin);
            }
            StringBuilder attributeKeyBuffer = new StringBuilder(substringWithAttributes.Length);
            StringBuilder attributeValueBuffer = new StringBuilder(substringWithAttributes.Length);

            Boolean keyIsNow = true;
            Boolean valueIsNow = false;
            Boolean foundEqualSign = false;
            Boolean finishedPair = false;
            Boolean insideQuotes = false;
            Boolean valueWithoutQuotes = false;
            Boolean whitespacePrevious = false;

            foreach (Char one in substringWithAttributes)
            {
                if (Char.IsWhiteSpace(one))
                {
                    whitespacePrevious = true;
                    if (valueWithoutQuotes)
                    {
                        valueWithoutQuotes = false;
                        valueIsNow = false;
                        keyIsNow = false;
                    }
                    else if (valueIsNow)
                    {
                        attributeValueBuffer.Append(one);
                    }
                    else
                    {
                        keyIsNow = false;
                    }
                    continue;
                }
                if (one == '=')
                {
                    whitespacePrevious = false;
                    if (insideQuotes && valueIsNow)
                    {
                        attributeValueBuffer.Append(one);
                    }
                    else
                    {
                        keyIsNow = false;
                        valueIsNow = false;
                        foundEqualSign = true;
                    }
                    continue;
                }
                if (one == '"' || one == '\'')
                {
                    whitespacePrevious = false;
                    insideQuotes = !insideQuotes;
                    if (foundEqualSign)
                    {
                        keyIsNow = false;
                        valueIsNow = true;
                    }
                    else
                    {
                        keyIsNow = false;
                        valueIsNow = false;

                        String attributeKey = attributeKeyBuffer.ToString();
                        if (output.ContainsKey(attributeKey) == false)
                        {
                            output.Add(attributeKey, attributeValueBuffer.ToString());
                        }
                        attributeKeyBuffer.Length = 0;
                        attributeValueBuffer.Length = 0;
                        finishedPair = true;
                    }
                    foundEqualSign = false;
                    continue;
                }
                if (valueIsNow == false && foundEqualSign == false && insideQuotes == false && finishedPair == false && whitespacePrevious)
                {
                    String attributeKey = attributeKeyBuffer.ToString();
                    if (output.ContainsKey(attributeKey) == false)
                    {
                        output.Add(attributeKey, attributeValueBuffer.ToString());
                    }
                    attributeKeyBuffer.Length = 0;
                    attributeValueBuffer.Length = 0;
                    finishedPair = true;
                }
                whitespacePrevious = false;
                if (finishedPair == false && foundEqualSign && insideQuotes == false)
                {
                    foundEqualSign = false;
                    valueIsNow = true;
                    valueWithoutQuotes = true;
                }
                if (valueIsNow)
                {
                    attributeValueBuffer.Append(one);
                    continue;
                }
                if (finishedPair)
                {
                    finishedPair = false;
                    keyIsNow = true;
                }
                if ((Char.IsLetterOrDigit(one) || one == '-' || one == ':') && keyIsNow)
                {
                    attributeKeyBuffer.Append(one);
                    continue;
                }
            }
            if (attributeKeyBuffer.Length > 0)
            {
                String attributeKey = attributeKeyBuffer.ToString();
                if (output.ContainsKey(attributeKey) == false)
                {
                    output.Add(attributeKey, attributeValueBuffer.ToString());
                }
            }
            return new HtmlElementDeficient(finalTagName, output, isSelfClosed, origin);
        }

        /// <summary>
        /// Ищет и возвращает все HTML-элементы с указанным именем
        /// </summary>
        /// <param name="inputHtml">Входная HTML-содержащая строка. Не может быть NULL, пустой строкой или не содержать цифробуквенных символов.</param>
        /// <param name="targetTagName">Имя целевого HTML-элемента, который ищется во входной строке. 
        /// Не может быть NULL, пустой строкой или не содержать цифробуквенных символов.</param>
        /// <param name="startIndex">Начальная позиция входной HTML-содержащей строки, с которой следует начать поиск. Если 0 - поиск ведётся с начала. 
        /// Если меньше 0 или больше длины исходной строки, выбрасывается исключение.</param>
        /// <returns></returns>
        public static List<HtmlElementDeficient> ExtractAllElementsFromHtml(string inputHtml, string targetTagName, int startIndex)
        {
            if (inputHtml == null) { throw new ArgumentNullException("inputHtml"); }
            if (inputHtml.HasAlphaNumericChars() == false)
            { throw new ArgumentException("Входная HTML-содержащая строка не содержит ни одной буквы или цифры и не является валидным HTML документом", "inputHtml"); }
            if (targetTagName.HasAlphaNumericChars() == false)
            { throw new ArgumentException("Имя целевого тэга некорректно, так как не содержит ни одной буквы или цифры", "targetTagName"); }
            if (startIndex < 0) { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Начальный индекс не может быть меньше 0"); }
            if (startIndex >= inputHtml.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex", startIndex,
                  string.Format("Начальный индекс ('{0}') не может быть больше или равен длине строки с HTML-разметкой ('{1}')", startIndex, inputHtml.Length));
            }

            List<HtmlElementDeficient> output = new List<HtmlElementDeficient>();
            int searchOffset = startIndex;
            while (true)
            {
                HtmlElementDeficient one = ExtractElementFromHtml(inputHtml, targetTagName, searchOffset);
                if (one == null)
                {
                    break;
                }
                output.Add(one);
                searchOffset = one.Origin.EndIndex;
            }
            return output;
        }
        #endregion Static methods
    }
}
using System;

namespace KlotosLib.StringTools
{
    /// <summary>
    /// Представляет подстроку, привязанную к одной конкретной базовой строке. Неизменяемый класс.
    /// </summary>
    [Serializable]
    public class Substring : IEquatable<Substring>, IComparable<Substring>, IComparable, ICloneable
    {
        #region Constructors
        private Substring(Substring other)
        {
            this._baseStr = other._baseStr;
            this._startIndex = other._startIndex;
            this._length = other._length;
        }

        private Substring(String baseStr, Int32 startIndex, Int32 length)
        {
            this._baseStr = baseStr;
            this._startIndex = startIndex;
            this._length = length;
        }
        #endregion Constructors

        #region Factories
        /// <summary>
        /// Создаёт и возвращает подстроку, которая основана на указанной базовой строке, и отсчитывается от начала базовой строки на указанное количество символов. 
        /// Дополнительный параметр позволяет определить, как поступать в случае, если указанное количество символов превышает длину базовой строки.
        /// </summary>
        /// <param name="baseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="length">Длина подстроки в базовой строке. Если 0 или меньше 0, будет выброшено исключение. 
        /// Если больше фактической длины базовой строки, поведение метода будет зависеть от последующего параметра <paramref name="untilEnd"/>.</param>
        /// <param name="untilEnd">Определяет, как поступать в случае, если указанная длина подстроки больше фактической длины базовой строки. 
        /// Если 'true' - подстрока будет расширена до конца базовой строки, фактически занимая её всю, полностью. 
        /// Если 'false' - будет выброшено исключение.</param>
        /// <returns>Новая подстрока</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Substring FromStartWithLength(String baseStr, Int32 length, Boolean untilEnd)
        {
            if (baseStr == null) { throw new ArgumentNullException("baseStr", "Базовая строка не может быть NULL"); }
            if (baseStr.Length == 0) { throw new ArgumentException("Базовая строка не может быть пустой", "baseStr"); }
            if (length < 0) { throw new ArgumentOutOfRangeException("length", length, "Длина подстроки не может быть отрицательным числом"); }
            if (length == 0) { throw new ArgumentOutOfRangeException("length", length, "Длина подстроки не может иметь нулевую длину"); }
            Int32 actualSubstrLen = length;
            if (length > baseStr.Length)
            {
                if (untilEnd == false)
                {
                    throw new ArgumentOutOfRangeException("length", length, 
                        String.Format("Запрошенная длина подстроки ({0} символов) превышает фактическую длину базовой строки ({1} символов)", length, baseStr.Length));
                }
                else
                {
                    actualSubstrLen = baseStr.Length;
                }
            }
            return new Substring(baseStr, 0, actualSubstrLen);
        }

        /// <summary>
        /// Создаёт и возвращает подстроку, которая основана на указанной базовой строке, и отсчитывается от конца базовой строки на указанное количество символов. 
        /// Дополнительный параметр позволяет определить, как поступать в случае, если указанное количество символов превышает длину базовой строки.
        /// </summary>
        /// <param name="baseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="length">Длина подстроки в базовой строке. Если 0 или меньше 0, будет выброшено исключение. 
        /// Если больше фактической длины базовой строки, поведение метода будет зависеть от последующего параметра <paramref name="untilStart"/>.</param>
        /// <param name="untilStart">Определяет, как поступать в случае, если указанная длина подстроки больше фактической длины базовой строки. 
        /// Если 'true' - подстрока будет расширена до начала базовой строки, фактически занимая её всю, полностью. 
        /// Если 'false' - будет выброшено исключение.</param>
        /// <returns>Новая подстрока</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Substring FromEndWithLength(String baseStr, Int32 length, Boolean untilStart)
        {
            if (baseStr == null) { throw new ArgumentNullException("baseStr", "Базовая строка не может быть NULL"); }
            if (baseStr.Length == 0) { throw new ArgumentException("Базовая строка не может быть пустой", "baseStr"); }
            if (length < 0) { throw new ArgumentOutOfRangeException("length", length, "Длина подстроки не может быть отрицательным числом"); }
            if (length == 0) { throw new ArgumentOutOfRangeException("length", length, "Длина подстроки не может иметь нулевую длину"); }
            Int32 actualSubstrLen = length;
            if (length > baseStr.Length)
            {
                if (untilStart == false)
                {
                    throw new ArgumentOutOfRangeException("length", length,
                        String.Format("Запрошенная длина подстроки ({0} символов) превышает фактическую длину базовой строки ({1} символов)", length, baseStr.Length));
                }
                else
                {
                    actualSubstrLen = baseStr.Length;
                }
            }
            return new Substring(baseStr, baseStr.Length - actualSubstrLen, actualSubstrLen);
        }

        /// <summary>
        /// Создаёт и возвращает подстроку, основанную на указанной базовой строке, имеющую указанные начальный индекс и длину
        /// </summary>
        /// <param name="baseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="startIndex">Начальный (включительный) индекс подстроки в базовой строке</param>
        /// <param name="length">Длина подстроки в базовой строке</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Substring FromIndexWithLength(String baseStr, Int32 startIndex, Int32 length)
        {
            if (baseStr == null) { throw new ArgumentNullException("baseStr", "Базовая строка не может быть NULL"); }
            if (baseStr.Length == 0) { throw new ArgumentException("Базовая строка не может быть пустой", "baseStr"); }
            if (startIndex < 0) { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Начальный индекс подстроки не может быть отрицательным числом"); }
            if (length < 0) { throw new ArgumentOutOfRangeException("length", length, "Длина подстроки не может быть отрицательным числом"); }
            if (length == 0) { throw new ArgumentOutOfRangeException("length", length, "Длина подстроки не может иметь нулевую длину"); }
            if (startIndex >= baseStr.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex", startIndex, String.Format(
                    "Начальный индекс подстроки '{0}' не может быть больше, чем максимально возможный индекс в базовой строке '{1}'", startIndex, baseStr.Length - 1));
            }
            if (startIndex + length > baseStr.Length)
            {
                throw new ArgumentException(String.Format("Результат сложения начального индекса '{0}' и длины '{1}' " +
                    "подстроки = '{2}' не может превышать длину базовой строки '{3}'", startIndex, length, startIndex + length, baseStr.Length));
            }

            return new Substring(baseStr, startIndex, length);
        }

        /// <summary>
        /// Создаёт и возвращает подстроку, основанную на указанной базовой строке, имеющую указанный начальный индекс, а длину вплоть до окончания базовой строки
        /// </summary>
        /// <param name="baseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="startIndex">Начальный (включительный) индекс подстроки в базовой строке</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Substring FromIndexToEnd(String baseStr, Int32 startIndex)
        {
            if (baseStr == null) { throw new ArgumentNullException("baseStr", "Базовая строка не может быть NULL"); }
            if (baseStr.Length == 0) { throw new ArgumentException("Базовая строка не может быть пустой", "baseStr"); }
            if (startIndex < 0) { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Начальный индекс подстроки не может быть отрицательным числом"); }
            if (startIndex >= baseStr.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex", startIndex, String.Format(
                    "Начальный индекс подстроки '{0}' не может быть больше, чем максимально возможный индекс в базовой строке '{1}'", startIndex, baseStr.Length - 1));
            }
            return new Substring(baseStr, startIndex, baseStr.Length - startIndex);
        }

        /// <summary>
        /// Создаёт и возвращает новый экземпляр подстроки, которая основана на указанной базовой строке, 
        /// и начинается от её начала и до указанного индекса включительно
        /// </summary>
        /// <param name="baseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="endIndex">Конечный (включительный) индекс подстроки в базовой строке. 
        /// Если меньше 0 или превышает максимальный допустимый индекс в базовой строке, будет выброшено исключение.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Substring FromStartToIndex(String baseStr, Int32 endIndex)
        {
            if (baseStr == null) { throw new ArgumentNullException("baseStr", "Базовая строка не может быть NULL"); }
            if (baseStr.Length == 0) { throw new ArgumentException("Базовая строка не может быть пустой", "baseStr"); }
            if (endIndex < 0) { throw new ArgumentOutOfRangeException("endIndex", endIndex, "Конечный индекс подстроки не может быть отрицательным числом"); }
            if (endIndex >= baseStr.Length)
            {
                throw new ArgumentOutOfRangeException("endIndex", endIndex, String.Format(
                    "Конечный индекс подстроки '{0}' не может быть больше, чем максимально возможный индекс в базовой строке '{1}'", endIndex, baseStr.Length - 1));
            }
            return new Substring(baseStr, 0, endIndex + 1);
        }

        /// <summary>
        /// Создаёт и возвращает новый экземпляр подстроки, которая основана на указанной базовой строке, 
        /// и начинается с указанного начального индекса и оканчивается указанным конечным индексом включительно
        /// </summary>
        /// <param name="baseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="startIndex">Начальный (включительный) индекс подстроки в базовой строке</param>
        /// <param name="endIndex">Конечный (включительный) индекс подстроки в базовой строке. 
        /// Если превышает максимальный допустимый индекс в базовой строке, будет выброшено исключение.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Substring FromIndexToIndex(String baseStr, Int32 startIndex, Int32 endIndex)
        {
            if (baseStr == null) { throw new ArgumentNullException("baseStr", "Базовая строка не может быть NULL"); }
            if (baseStr.Length == 0) { throw new ArgumentException("Базовая строка не может быть пустой", "baseStr"); }
            if (startIndex < 0) { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Начальный индекс подстроки не может быть отрицательным числом"); }
            if (endIndex < 0) { throw new ArgumentOutOfRangeException("endIndex", endIndex, "Конечный индекс подстроки не может быть отрицательным числом"); }
            if(endIndex < startIndex)
            { throw new ArgumentException(String.Format("Конечный индекс подстроки '{0}' не может быть меньшим, нежели начальный '{1}'", endIndex, startIndex)); }
            if (startIndex >= baseStr.Length) {throw new ArgumentOutOfRangeException("startIndex", startIndex, String.Format(
                "Начальный индекс подстроки '{0}' не может быть больше, чем максимально возможный индекс в базовой строке '{1}'", startIndex, baseStr.Length-1)); }
            if (endIndex >= baseStr.Length) {throw new ArgumentOutOfRangeException("endIndex", endIndex, String.Format(
                "Конечный индекс подстроки '{0}' не может быть больше, чем максимально возможный индекс в базовой строке '{1}'", endIndex, baseStr.Length - 1)); }
            
            return new Substring(baseStr, startIndex, endIndex - startIndex + 1);
        }
        #endregion Factories

        #region Fields
        private readonly String _baseStr;

        private readonly Int32 _startIndex;

        private readonly Int32 _length;

        /// <summary>
        /// Буфер для устранения множественной аллокации одной и той же подстроки
        /// </summary>
        private String _cachedValue;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Позиция первого символа данной подстроки в базовой строке (включительный начальный индекс)
        /// </summary>
        public Int32 StartIndex { get { return this._startIndex; } }

        /// <summary>
        /// Позиция последнего символа данной подстроки в базовой строке (включительный конечный индекс)
        /// </summary>
        public Int32 EndIndex { get { return this._startIndex + this._length - 1; } }

        /// <summary>
        /// Длина данной подстроки
        /// </summary>
        public Int32 Length { get { return this._length; } }

        /// <summary>
        /// Определяет, является ли данная подстрока началом базовой строки (true) или нет (false)
        /// </summary>
        public Boolean IsBeginning { get { return this._startIndex == 0; } }

        /// <summary>
        /// Определяет, является ли данная подстрока концом базовой строки (true) или нет (false)
        /// </summary>
        public Boolean IsEnding { get { return this._startIndex + this._length == this._baseStr.Length; } }

        /// <summary>
        /// Определяет, представляет ли данная подстрока всю базовую строку (true) или нет (false)
        /// </summary>
        public Boolean IsComplete { get { return this._startIndex == 0 && this._length == this._baseStr.Length; } }

        /// <summary>
        /// Возвращает строковое представление данной подстроки, выполняя её материализацию. 
        /// Будучи вызванным, использует единственное закэшированное материализированное представление.
        /// </summary>
        public String Value {
            get {
                return this._cachedValue ??
                       (this._cachedValue = this._baseStr.Substring(this._startIndex, this._length));
            }
        }

        /// <summary>
        /// Базовая строка
        /// </summary>
        public String BaseString {get { return this._baseStr; }}

        /// <summary>
        /// Возвращает один символ в указанной позиции в данной подстроке
        /// </summary>
        /// <param name="index">Индекс запрашиваемого символа в данной подстроке, который начинается с 0 включительно и не может превышать длину подстроки. 
        /// Транслируется в индекс базовой строки без материализации подстроки.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Char this[Int32 index]
        {
            get
            {
                if (index < 0) { throw new ArgumentOutOfRangeException("index", index, "Индекс запрашиваемого символа не может быть отрицательным числом"); }
                if(index >= this._length)
                {
                    throw new ArgumentOutOfRangeException("index", index, String.Format(
                        "Индекс запрашиваемого символа '{0}' не может быть больше, чем максимально возможный индекс в данной подстроке '{1}'", index, this._length - 1));
                }
                Int32 positionInBaseStr = this._startIndex + index;
                return this._baseStr[positionInBaseStr];
            }
        }
        #endregion Properties

        #region Instance methods

        /// <summary>
        /// Создаёт и возвращает экземпляр новой подстроки, который создан в рамках данной, 
        /// начиная с указанной позиции в данной подстроке и ограничиваясь указанным количеством символов
        /// </summary>
        /// <param name="startIndex">Включительная позиция начала возвращаемой подстроки в данной подстроке. Отсчитывается от 0,
        /// счёт ведётся от начала данной подстроки, а не базовой строки. Не может быть отрицательной или превышать длину данной подстроки.</param>
        /// <param name="length">Длина возвращаемой подстроки, должна быть строго положительной. 
        /// Сумма указанных длины и начальной позиции не может превышать общую длину данной подстроки.</param>
        /// <returns></returns>
        public Substring FromIndexWithLength(int startIndex, int length)
        {
            if(startIndex < 0) 
            { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Начальная позиция не может быть отрицательной"); }
            if(length <= 0) 
            { throw new ArgumentOutOfRangeException("length", length, "Длина возвращаемой подстроки должна быть строго больше 0"); }
            if (startIndex >= this.Length)
            { throw new ArgumentOutOfRangeException("startIndex", startIndex, "Начальная позиция не может превышать длину данной подстроки"); }
            if (startIndex + length > this.Length)
            { throw new ArgumentOutOfRangeException("length", string.Format(
                "Сумма указанных начальной позиции '{0}' и длины '{1}' равна '{2}', что превышает длину данной подстроки '{3}'", 
                startIndex, length, (startIndex + length), this.Length)); }
            return Substring.FromIndexWithLength(this._baseStr, this._startIndex + startIndex, length);
        }

        /// <summary>
        /// Определяет, начинается ли данная подстрока с указанного значения, используя указанные опции сравнения строк
        /// </summary>
        /// <param name="target">Целевая строка, которую надо сравнить с началом данной подстроки. Не может быть NULL или пустой</param>
        /// <param name="comparisonType">Опции сравнения строк</param>
        /// <returns></returns>
        public Boolean StartsWith(String target, StringComparison comparisonType)
        {
            if (String.IsNullOrEmpty(target))
            { throw new ArgumentException("Искомая строка не может быть NULL или пустой", "target"); }
            if (target.Length > this._length)
            {
                return false;
            }
            Int32 index = this._baseStr.IndexOf(target, this._startIndex, this._length, comparisonType);
            if (index < 0 || index != this._startIndex)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Определяет, оканчивается ли данная подстрока указанным значением, используя указанные опции сравнения строк
        /// </summary>
        /// <param name="target">Целевая строка, которую надо сравнить с концом данной подстроки. Не может быть NULL или пустой</param>
        /// <param name="comparisonType">Опции сравнения строк</param>
        /// <returns></returns>
        public Boolean EndsWith(String target, StringComparison comparisonType)
        {
            if (String.IsNullOrEmpty(target))
            { throw new ArgumentException("Искомая строка не может быть NULL или пустой", "target"); }
            if (target.Length > this._length)
            {
                return false;
            }
            Int32 index = this._baseStr.LastIndexOf(target, this.EndIndex, this._length, comparisonType);
            return index >= this._startIndex && index + target.Length - 1 == this.EndIndex;
        }

        /// <summary>
        /// Копирует все символы из данной подстроки в новый символьный массив и возвращает его
        /// </summary>
        /// <returns>Новый символьный массив, который создаётся заново и возвращается при каждом вызове.</returns>
        public Char[] ToCharArray()
        {
            return this._baseStr.ToCharArray(this._startIndex, this._length);
        }

        /// <summary>
        /// Копирует определённое количество символов из данной подстроки в новый символьный массив и возвращает его
        /// </summary>
        /// <param name="startIndexInSubstring">Начальная позиция в данной подстроке, с которой включительно начнётся копирование</param>
        /// <param name="symbolsToCopy">Количество символов, которое требуется скопировать</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Char[] ToCharArray(int startIndexInSubstring, int symbolsToCopy)
        {
            if (startIndexInSubstring < 0) { throw new ArgumentOutOfRangeException("startIndexInSubstring", startIndexInSubstring, 
                "Начальная позиция в подстроке не может быть отрицательным числом"); }
            if (startIndexInSubstring >= this._length)
            {
                throw new ArgumentOutOfRangeException("startIndexInSubstring", startIndexInSubstring, String.Format(
                    "Начальный индекс в подстроке '{0}' не может быть больше, чем максимально возможный индекс в данной подстроке '{1}'", startIndexInSubstring, this._length - 1));
            }
            if (symbolsToCopy < 0) { throw new ArgumentOutOfRangeException("symbolsToCopy", symbolsToCopy, 
                "Количество символов для копирования не может быть отрицательным числом"); }
            if (startIndexInSubstring + symbolsToCopy > this._length)
            {
                throw new ArgumentException(String.Format("Результат сложения начального индекса '{0}' и количества символов для копирования '{1}' " +
                    " равен '{2}' и не может превышать общую длину подстроки '{3}'", 
                    startIndexInSubstring, symbolsToCopy, startIndexInSubstring + symbolsToCopy, this._length));
            }
            Int32 translatedStartIndex = this._startIndex + startIndexInSubstring;
            return this._baseStr.ToCharArray(translatedStartIndex, symbolsToCopy);
        }

        /// <summary>
        /// Возвращает специальный перечислитель, который может выполнять итерацию отдельных символов данной подстроки, не материализируя её
        /// </summary>
        /// <returns></returns>
        public SubstringCharEnumerator GetEnumerator()
        {
            return new SubstringCharEnumerator(this);
        }

        /// <summary>
        /// Заменяет в базовой строке содержимое данной подстроки на новое указанное содержимое и возвращает как новый экземпляр строки
        /// </summary>
        /// <param name="replacement">Новая строка, которая должна заменить в базовой строке данную подстроку. 
        /// Если NULL или пустая - в базовой строке будет вырезана данная подстрока без вставки новых символов</param>
        /// <returns>Новый экземпляр строки, представляющий собой версию базовой строки, где вместо данной подстроки присутствует указанная</returns>
        public String ReplaceInBaseString(String replacement)
        {
            Boolean isNullOrEmpty = replacement.IsNullOrEmpty();
            if (this.IsComplete == true && isNullOrEmpty == true)
            {
                return String.Empty;
            }
            if (this.IsComplete == true && isNullOrEmpty == false)
            {
                return replacement;
            }
            if (this.IsBeginning == true)
            {
                String basePart = this._baseStr.Substring(this.EndIndex + 1);
                if (isNullOrEmpty)
                {
                    return basePart;
                }
                else
                {
                    return replacement + basePart;
                }
            }
            else if (this.IsEnding == true)
            {
                String basePart = this._baseStr.Substring(0, this.StartIndex);
                if (isNullOrEmpty)
                {
                    return basePart;
                }
                else
                {
                    return basePart + replacement;
                }
            }
            else
            {
                String firstBasePart = this._baseStr.Substring(0, this.StartIndex);
                String lastBasePart = this._baseStr.Substring(this.EndIndex + 1);
                if (isNullOrEmpty)
                {
                    return firstBasePart + lastBasePart;
                }
                else
                {
                    return firstBasePart + replacement + lastBasePart;
                }
            }
        }
        #endregion Instance methods

        #region Static methods
        /// <summary>
        /// Определяет, равны ли два указанных экземпляра подстрок между собой. Если обе подстроки являются NULL, то считается, что они равны. 
        /// Две не-NULL подстроки считаются равны тогда и только тогда, когда имеют одинаковые начальный индекс и длину, 
        /// а также привязаны к одному и тому же экземпляру базовой строки.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean AreEqual(Substring first, Substring second)
        {
            if (Object.ReferenceEquals(first, second) == true) { return true; }
            if (Object.ReferenceEquals(first, null) == true || Object.ReferenceEquals(null, second) == true) { return false; }
            if (Object.ReferenceEquals(first._baseStr, second._baseStr) == false) { return false; }
            Boolean result = first._length == second._length && first._startIndex == second._startIndex;
            return result;
        }

        /// <summary>
        /// Определяет, все ли подстроке в указанном наборе равны между собой
        /// </summary>
        /// <param name="substrings">Набор подстрок. Если NULL, пуст или содержит одну подстроку, будет выброшено исключение.</param>
        /// <returns>Если все подстроки равны - 'true', иначе 'false'</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Boolean AreEqual(params Substring[] substrings)
        {
            if (substrings == null) { throw new ArgumentNullException("substrings"); }
            if (substrings.Length == 0) { throw new ArgumentException("Набор подстрок не может быть пуст", "substrings"); }
            if (substrings.Length == 1) { throw new ArgumentException("Набор подстрок не может содержать только одну подстроку", "substrings"); }
            if (substrings.Length == 2) { return Substring.AreEqual(substrings[0], substrings[1]); }

            for (Int32 firstIndex = 0, secondIndex = firstIndex + 1; secondIndex < substrings.Length; firstIndex++, secondIndex++)
            {
                if (Substring.AreEqual(substrings[firstIndex], substrings[secondIndex]) == false)
                { return false; }
            }
            return true;
        }

        /// <summary>
        /// Определяет, имеют ли два указанных экземпляра подстрок общую базовую строку. Если ссылки на экземпляры эквивалентны, 
        /// или же базовая строка для обоих является общей, возвращается true, а во всех остальных случаях - false.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean HaveCommonBaseString(Substring first, Substring second)
        {
            if (Object.ReferenceEquals(first, null) == true || Object.ReferenceEquals(null, second) == true) { return false; }
            if (Object.ReferenceEquals(first, second) == true) { return true; }
            return Object.ReferenceEquals(first._baseStr, second._baseStr);
        }

        /// <summary>
        /// Определяет, имеют ли все указанные экземпляры общую базовую строку. Если ссылки на экземпляры эквивалентны, 
        /// или же базовая строка для всех является общей, возвращается true, а во всех остальных случаях - false.
        /// </summary>
        /// <param name="substrings">Набор подстрок для проверки. Если NULL, пуст или содержит одну подстроку, будет выброшено исключение.</param>
        /// <returns>Если все подстроки имеют общую базовую строку - 'true', иначе 'false'</returns>
        public static Boolean HaveCommonBaseString(params Substring[] substrings)
        {
            if (substrings == null) { throw new ArgumentNullException("substrings"); }
            if (substrings.Length == 0) { throw new ArgumentException("Набор подстрок не может быть пуст", "substrings"); }
            if (substrings.Length == 1) { throw new ArgumentException("Набор подстрок не может содержать только одну подстроку", "substrings"); }
            if (substrings.Length == 2) { return Substring.HaveCommonBaseString(substrings[0], substrings[1]); }

            for (Int32 firstIndex = 0, secondIndex = firstIndex + 1; secondIndex < substrings.Length; firstIndex++, secondIndex++)
            {
                if (Substring.HaveCommonBaseString(substrings[firstIndex], substrings[secondIndex]) == false)
                { return false; }
            }
            return true;
        }

        /// <summary>
        /// Сравнивает два экземпляра подстрок между собой, показывая их связь (больше-меньше-равно) в порядке сортировки
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Int32 Compare(Substring first, Substring second)
        {
            if (Object.ReferenceEquals(first, second) == true) { return 0; }
            if (Object.ReferenceEquals(first, null) == true) { return -1; }
            if (Object.ReferenceEquals(second, null) == true) { return 1; }
            if (Substring.HaveCommonBaseString(first, second) == false)
            {
                Int32 res = String.Compare(first.BaseString, second.BaseString, StringComparison.Ordinal);
                if (res < 0) { return -1; }
                if (res > 0) { return 1; }
                return 0;
            }
            if (first.StartIndex != second.StartIndex) { return first.StartIndex.CompareTo(second.StartIndex); }
            return first.Length.CompareTo(second.Length);
        }

        /// <summary>
        /// Определяет, пересекаются ли между собой две подстроки, имеющие общую базовую строку. Идентичность и равность подстрок также считается пересечением. 
        /// Если указанные подстроки не относятся к единой базовой строке, будет выброшено исключение.
        /// </summary>
        /// <param name="first">Первая подстрока для проверки на пересечение. Не может быть NULL.</param>
        /// <param name="second">Вторая подстрока для проверки на пересечение. Не может быть NULL.</param>
        /// <returns>Если пересекаются или идентичны, возвращает 'true', иначе 'false'</returns>
        public static Boolean AreIntersecting(Substring first, Substring second)
        {
            if(first == null) {throw new ArgumentNullException("first");}
            if (second == null) { throw new ArgumentNullException("second"); }
            if (first == second) { return true; }
            if (Substring.HaveCommonBaseString(first, second) == false)
            {
                throw new ArgumentException("Указанные подстроки не относятся к единой базовой строке");
            }
            return Substring.AreIntersectingUnsafe(first, second);
        }

        /// <summary>
        /// Внутренний метод определения пересечений, который не содержит проверок входных параметров, проверок идентичности и равности
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private static Boolean AreIntersectingUnsafe(Substring first, Substring second)
        {
            Boolean result = (first.EndIndex >= second._startIndex && first._startIndex <= second._startIndex) ||
                (second.EndIndex >= first._startIndex && second._startIndex <= first._startIndex);
            return result;
        }

        /// <summary>
        /// Определяет, пересекаются ли между собой все подстроки, имеющие общую базовую строку. Идентичность и равность подстрок также считается пересечением. 
        /// Пересечения проверяются путём проверки всех возможных комбинаций пар подстрок. 
        /// </summary>
        /// <param name="substrings">Набор подстрок для проверки на пересечение. Если NULL, пуст или содержит одну подстроку, будет выброшено исключение. 
        /// Если хотя бы одна подстрока из списка не относятся к единой базовой строке для всех подстрок, будет выброшено исключение.</param>
        /// <returns></returns>
        public static Boolean AreIntersecting(params Substring[] substrings)
        {
            if (substrings == null) { throw new ArgumentNullException("substrings"); }
            if (substrings.Length == 0) { throw new ArgumentException("Набор подстрок не может быть пуст", "substrings"); }
            if (substrings.Length == 1) { throw new ArgumentException("Набор подстрок не может содержать только одну подстроку", "substrings"); }
            
            for (Int32 i = 0; i < substrings.Length - 1; i++)
            {
                Substring first = substrings[i];
                for (Int32 j = i + 1; j < substrings.Length; j++)
                {
                    Substring second = substrings[j];
                    if (first == second)
                    {
                        return true;
                    }
                    if (Substring.HaveCommonBaseString(first, second) == false)
                    {
                        throw new ArgumentException("Не все указанные подстроки относятся к единой базовой строке", "substrings");
                    }
                    Boolean result = Substring.AreIntersectingUnsafe(first, second);
                    if (result == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion Static methods

        #region Comparable and Equatable
        /// <summary>
        /// Возвращает хэш-код данного экземпляра, который основывается на комбинации хэш-кодов базовой строки, а также начального индекса и длины подстроки
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                int hashCode = this._baseStr.GetHashCode();
                hashCode = (hashCode * 397) ^ this._startIndex;
                hashCode = (hashCode * 397) ^ this._length;
                return hashCode;
            }
        }
        
        /// <summary>
        /// Определяет равенство данного экземпляра с указанным, предварительно пытаясь привести его к данному типу
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override Boolean Equals(Object other)
        {
            if (Object.ReferenceEquals(null, other)) {return false;}
            if (Object.ReferenceEquals(this, other)) {return true;}
            if (other.GetType() != this.GetType()) {return false;}
            return Substring.AreEqual(this, (Substring)other);
        }

        /// <summary>
        /// Определяет равенство данного экземпляра с указанным
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(Substring other)
        {
            return Substring.AreEqual(this, other);
        }

        /// <summary>
        /// Сравнивает данный экземпляр с указанным и показывает, как они соотносятся в порядке сортировки
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo(Substring other)
        {
            return Substring.Compare(this, other);
        }

        /// <summary>
        /// Пытается привести указанный экземпляр неизвестного типа Object к данному типу, 
        /// а затем сравнивает его с указанным и показывает, как они соотносятся в порядке сортировки
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Object other)
        {
            if (Object.ReferenceEquals(other, null) == true)
            {
                throw new ArgumentNullException("other", "Нельзя сравнить подстроку с NULL");
            }
            Substring converted = other as Substring;
            if (converted == null)
            {
                throw new InvalidOperationException("Нельзя сравнить подстроку с другим типом");
            }
            return Substring.Compare(this, converted);
        }
        #endregion
        
        #region Operators
        /// <summary>
        /// Определяет, равны ли два указанных экземпляра подстрок между собой
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator ==(Substring first, Substring second)
        {
            return Substring.AreEqual(first, second);
        }

        /// <summary>
        /// Определяет, не равны ли два указанных экземпляра подстрок между собой
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator !=(Substring first, Substring second)
        {
            return !Substring.AreEqual(first, second);
        }

        /// <summary>
        /// Определяет, строго больше ли первый указанный экземпляр второго с точки зрения сортировки
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator >(Substring first, Substring second)
        {
            return Substring.Compare(first, second) == 1;
        }

        /// <summary>
        /// Определяет, строго меньше ли первый указанный экземпляр второго с точки зрения сортировки
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator <(Substring first, Substring second)
        {
            return Substring.Compare(first, second) == -1;
        }

        /// <summary>
        /// Определяет, больше или равен первый указанный экземпляр по сравнению со вторым с точки зрения сортировки
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator >=(Substring first, Substring second)
        {
            return Substring.Compare(first, second) >= 0;
        }

        /// <summary>
        /// Определяет, меньше или равен первый указанный экземпляр по сравнению со вторым с точки зрения сортировки
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator <=(Substring first, Substring second)
        {
            return Substring.Compare(first, second) <= 0;
        }
        #endregion

        /// <summary>
        /// Возвращает строковое представление данной подстроки, выполняя её материализацию. Поведение аналогично свойству <see cref="Value"/>
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.Value;
        }

        #region Cloning
        /// <summary>
        /// Возвращает глубокую копию текущего экземпляра
        /// </summary>
        /// <returns></returns>
        public Substring Clone()
        {
            return new Substring(this);
        }
        Object ICloneable.Clone()
        {
            return this.Clone();
        }
        #endregion Cloning

        #region Subclass

        /// <summary>
        /// Представляет перечислитель для подстрок, не требующей материализации подстроки как отдельной строки
        /// </summary>
        public sealed class SubstringCharEnumerator : ICloneable, System.Collections.Generic.IEnumerator<Char>
        {
            private const Char ZeroChar = (Char) 0;

            internal SubstringCharEnumerator(Substring source)
            {
                this._baseStr = source._baseStr;
                this._startIndex = source._startIndex;
                this._length = source._length;
                this._currentIndex = -1;
                this._currentElement = ZeroChar;
                this._isDisposed = false;
            }

            private SubstringCharEnumerator(SubstringCharEnumerator other)
            {
                this._baseStr = other._baseStr;
                this._startIndex = other._startIndex;
                this._length = other._length;
                this._currentIndex = other._currentIndex;
                this._currentElement = other._currentElement;
                this._isDisposed = other._isDisposed;
            }

            #region Fields
            private String _baseStr;
            private readonly Int32 _startIndex;
            private readonly Int32 _length;
            private Int32 _currentIndex;
            private Char _currentElement;
            private Boolean _isDisposed;
            #endregion Fields

            /// <summary>
            /// Увеличивает внутренний индекс текущего перечислителя, чтобы он указывал на следующий по порядку символ в перечисляемой подстроке, 
            /// и возвращает результат операции
            /// </summary>
            /// <returns>
            /// Значение 'true', если индекс успешно увеличен и находится в пределах перечисляемой строки; в противном случае — значение 'false'.
            /// </returns>
            public bool MoveNext()
            {
                if (this._isDisposed == true) { throw new ObjectDisposedException("SubstringCharEnumerator"); }
                if (this._currentIndex < this._length - 1)
                {
                    this._currentIndex++;
                    this._currentElement = this._baseStr[this._startIndex + this._currentIndex];
                    return true;
                }
                else
                {
                    this._currentIndex = this._length;
                    return false;
                }
            }

            /// <summary>
            /// Возвращает текущий символ в подстроке, обходимой данным перечислителем
            /// </summary>
            public Char Current
            {
                get
                {
                    if (this._isDisposed == true) { throw new ObjectDisposedException("SubstringCharEnumerator"); }
                    if (this._currentIndex < 0)
                    {
                        throw new InvalidOperationException("Невозможно получить текущий элемент, так как перечисление ещё не началось. "+
                            "Нужно вызвать метод 'MoveNext' вначале.");
                    }
                    if (this._currentIndex >= this._length)
                    {
                        throw new InvalidOperationException("Невозможно получить текущий элемент, так как перечисление уже закончилось. "+
                            "Нужно вызвать метод 'Reset' для сброса состояния.");
                    }
                    return this._currentElement;
                }
            }

            /// <summary>
            /// Сбрасывает текущее значение данного перечислителя в 'неинициализированное' первоначальное состояние, которое предшествует первому вызову метода 'MoveNext'
            /// </summary>
            public void Reset()
            {
                if (this._isDisposed == true) { throw new ObjectDisposedException("SubstringCharEnumerator"); }
                this._currentIndex = -1;
                this._currentElement = ZeroChar;
            }

            Object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            #region Cloning
            /// <summary>
            /// Возвращает глубокую копию текущего экземпляра
            /// </summary>
            /// <returns></returns>
            public SubstringCharEnumerator Clone()
            {
                return new SubstringCharEnumerator(this);
            }
            Object ICloneable.Clone()
            {
                return this.Clone();
            }
            #endregion

            /// <summary>
            /// Возвращает состояние данного экземлпяра: освобождены ли его ресурсы или нет
            /// </summary>
            public Boolean IsDisposed {get { return this._isDisposed; }}

            /// <summary>
            /// Освобождает ресурсы данного перечислителя, обнуляя ссылку на базовую строку подстроки и запрещая дальнейшие операции с данным экземпляром
            /// </summary>
            public void Dispose()
            {
                if(this._isDisposed == true) {return;}
                this._baseStr = null;
                this._currentElement = ZeroChar;
                this._currentIndex = -1;
                this._isDisposed = true;
            }
        }
        #endregion Subclass
    }
}

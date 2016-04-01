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
        private Substring(Substring Other)
        {
            this._baseStr = Other._baseStr;
            this._startIndex = Other._startIndex;
            this._length = Other._length;
        }

        private Substring(String BaseStr, Int32 StartIndex, Int32 Length)
        {
            this._baseStr = BaseStr;
            this._startIndex = StartIndex;
            this._length = Length;
        }
        #endregion Constructors

        #region Factories
        /// <summary>
        /// Создаёт и возвращает подстроку, которая основана на указанной базовой строке, и отсчитывается от начала базовой строки на указанное количество символов. 
        /// Дополнительный параметр позволяет определить, как поступать в случае, если указанное количество символов превышает длину базовой строки.
        /// </summary>
        /// <param name="BaseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="Length">Длина подстроки в базовой строке. Если 0 или меньше 0, будет выброшено исключение. 
        /// Если больше фактической длины базовой строки, поведение метода будет зависеть от последующего параметра <paramref name="UntilEnd"/>.</param>
        /// <param name="UntilEnd">Определяет, как поступать в случае, если указанная длина подстроки больше фактической длины базовой строки. 
        /// Если 'true' - подстрока будет расширена до конца базовой строки, фактически занимая её всю, полностью. 
        /// Если 'false' - будет выброшено исключение.</param>
        /// <returns>Новая подстрока</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Substring FromStartWithLength(String BaseStr, Int32 Length, Boolean UntilEnd)
        {
            if (BaseStr == null) { throw new ArgumentNullException("BaseStr", "Базовая строка не может быть NULL"); }
            if (BaseStr.Length == 0) { throw new ArgumentException("Базовая строка не может быть пустой", "BaseStr"); }
            if (Length < 0) { throw new ArgumentOutOfRangeException("Length", Length, "Длина подстроки не может быть отрицательным числом"); }
            if (Length == 0) { throw new ArgumentOutOfRangeException("Length", Length, "Длина подстроки не может иметь нулевую длину"); }
            Int32 actual_substr_len = Length;
            if (Length > BaseStr.Length)
            {
                if (UntilEnd == false)
                {
                    throw new ArgumentOutOfRangeException("Length", Length, 
                        String.Format("Запрошенная длина подстроки ({0} символов) превышает фактическую длину базовой строки ({1} символов)", Length, BaseStr.Length));
                }
                else
                {
                    actual_substr_len = BaseStr.Length;
                }
            }
            return new Substring(BaseStr, 0, actual_substr_len);
        }

        /// <summary>
        /// Создаёт и возвращает подстроку, которая основана на указанной базовой строке, и отсчитывается от конца базовой строки на указанное количество символов. 
        /// Дополнительный параметр позволяет определить, как поступать в случае, если указанное количество символов превышает длину базовой строки.
        /// </summary>
        /// <param name="BaseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="Length">Длина подстроки в базовой строке. Если 0 или меньше 0, будет выброшено исключение. 
        /// Если больше фактической длины базовой строки, поведение метода будет зависеть от последующего параметра <paramref name="UntilStart"/>.</param>
        /// <param name="UntilStart">Определяет, как поступать в случае, если указанная длина подстроки больше фактической длины базовой строки. 
        /// Если 'true' - подстрока будет расширена до начала базовой строки, фактически занимая её всю, полностью. 
        /// Если 'false' - будет выброшено исключение.</param>
        /// <returns>Новая подстрока</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Substring FromEndWithLength(String BaseStr, Int32 Length, Boolean UntilStart)
        {
            if (BaseStr == null) { throw new ArgumentNullException("BaseStr", "Базовая строка не может быть NULL"); }
            if (BaseStr.Length == 0) { throw new ArgumentException("Базовая строка не может быть пустой", "BaseStr"); }
            if (Length < 0) { throw new ArgumentOutOfRangeException("Length", Length, "Длина подстроки не может быть отрицательным числом"); }
            if (Length == 0) { throw new ArgumentOutOfRangeException("Length", Length, "Длина подстроки не может иметь нулевую длину"); }
            Int32 actual_substr_len = Length;
            if (Length > BaseStr.Length)
            {
                if (UntilStart == false)
                {
                    throw new ArgumentOutOfRangeException("Length", Length,
                        String.Format("Запрошенная длина подстроки ({0} символов) превышает фактическую длину базовой строки ({1} символов)", Length, BaseStr.Length));
                }
                else
                {
                    actual_substr_len = BaseStr.Length;
                }
            }
            return new Substring(BaseStr, BaseStr.Length - actual_substr_len, actual_substr_len);
        }

        /// <summary>
        /// Создаёт и возвращает подстроку, основанную на указанной базовой строке, имеющую указанные начальный индекс и длину
        /// </summary>
        /// <param name="BaseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="StartIndex">Начальный (включительный) индекс подстроки в базовой строке</param>
        /// <param name="Length">Длина подстроки в базовой строке</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Substring FromIndexWithLength(String BaseStr, Int32 StartIndex, Int32 Length)
        {
            if (BaseStr == null) { throw new ArgumentNullException("BaseStr", "Базовая строка не может быть NULL"); }
            if (BaseStr.Length == 0) { throw new ArgumentException("Базовая строка не может быть пустой", "BaseStr"); }
            if (StartIndex < 0) { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Начальный индекс подстроки не может быть отрицательным числом"); }
            if (Length < 0) { throw new ArgumentOutOfRangeException("Length", Length, "Длина подстроки не может быть отрицательным числом"); }
            if (Length == 0) { throw new ArgumentOutOfRangeException("Length", Length, "Длина подстроки не может иметь нулевую длину"); }
            if (StartIndex >= BaseStr.Length)
            {
                throw new ArgumentOutOfRangeException("StartIndex", StartIndex, String.Format(
                    "Начальный индекс подстроки '{0}' не может быть больше, чем максимально возможный индекс в базовой строке '{1}'", StartIndex, BaseStr.Length - 1));
            }
            if (StartIndex + Length > BaseStr.Length)
            {
                throw new ArgumentException(String.Format("Результат сложения начального индекса '{0}' и длины '{1}' " +
                    "подстроки = '{2}' не может превышать длину базовой строки '{3}'", StartIndex, Length, StartIndex + Length, BaseStr.Length));
            }

            return new Substring(BaseStr, StartIndex, Length);
        }

        /// <summary>
        /// Создаёт и возвращает подстроку, основанную на указанной базовой строке, имеющую указанный начальный индекс, а длину вплоть до окончания базовой строки
        /// </summary>
        /// <param name="BaseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="StartIndex">Начальный (включительный) индекс подстроки в базовой строке</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Substring FromIndexToEnd(String BaseStr, Int32 StartIndex)
        {
            if (BaseStr == null) { throw new ArgumentNullException("BaseStr", "Базовая строка не может быть NULL"); }
            if (BaseStr.Length == 0) { throw new ArgumentException("Базовая строка не может быть пустой", "BaseStr"); }
            if (StartIndex < 0) { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Начальный индекс подстроки не может быть отрицательным числом"); }
            if (StartIndex >= BaseStr.Length)
            {
                throw new ArgumentOutOfRangeException("StartIndex", StartIndex, String.Format(
                    "Начальный индекс подстроки '{0}' не может быть больше, чем максимально возможный индекс в базовой строке '{1}'", StartIndex, BaseStr.Length - 1));
            }
            return new Substring(BaseStr, StartIndex, BaseStr.Length - StartIndex);
        }

        /// <summary>
        /// Создаёт и возвращает новый экземпляр подстроки, которая основана на указанной базовой строке, 
        /// и начинается от её начала и до указанного индекса включительно
        /// </summary>
        /// <param name="BaseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="EndIndex">Конечный (включительный) индекс подстроки в базовой строке. 
        /// Если меньше 0 или превышает максимальный допустимый индекс в базовой строке, будет выброшено исключение.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Substring FromStartToIndex(String BaseStr, Int32 EndIndex)
        {
            if (BaseStr == null) { throw new ArgumentNullException("BaseStr", "Базовая строка не может быть NULL"); }
            if (BaseStr.Length == 0) { throw new ArgumentException("Базовая строка не может быть пустой", "BaseStr"); }
            if (EndIndex < 0) { throw new ArgumentOutOfRangeException("EndIndex", EndIndex, "Конечный индекс подстроки не может быть отрицательным числом"); }
            if (EndIndex >= BaseStr.Length)
            {
                throw new ArgumentOutOfRangeException("EndIndex", EndIndex, String.Format(
                    "Конечный индекс подстроки '{0}' не может быть больше, чем максимально возможный индекс в базовой строке '{1}'", EndIndex, BaseStr.Length - 1));
            }
            return new Substring(BaseStr, 0, EndIndex + 1);
        }

        /// <summary>
        /// Создаёт и возвращает новый экземпляр подстроки, которая основана на указанной базовой строке, 
        /// и начинается с указанного начального индекса и оканчивается указанным конечным индексом включительно
        /// </summary>
        /// <param name="BaseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="StartIndex">Начальный (включительный) индекс подстроки в базовой строке</param>
        /// <param name="EndIndex">Конечный (включительный) индекс подстроки в базовой строке. 
        /// Если превышает максимальный допустимый индекс в базовой строке, будет выброшено исключение.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Substring FromIndexToIndex(String BaseStr, Int32 StartIndex, Int32 EndIndex)
        {
            if (BaseStr == null) { throw new ArgumentNullException("BaseStr", "Базовая строка не может быть NULL"); }
            if (BaseStr.Length == 0) { throw new ArgumentException("Базовая строка не может быть пустой", "BaseStr"); }
            if (StartIndex < 0) { throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Начальный индекс подстроки не может быть отрицательным числом"); }
            if (EndIndex < 0) { throw new ArgumentOutOfRangeException("EndIndex", EndIndex, "Конечный индекс подстроки не может быть отрицательным числом"); }
            if(EndIndex < StartIndex)
            { throw new ArgumentException(String.Format("Конечный индекс подстроки '{0}' не может быть меньшим, нежели начальный '{1}'", EndIndex, StartIndex)); }
            if (StartIndex >= BaseStr.Length) {throw new ArgumentOutOfRangeException("StartIndex", StartIndex, String.Format(
                "Начальный индекс подстроки '{0}' не может быть больше, чем максимально возможный индекс в базовой строке '{1}'", StartIndex, BaseStr.Length-1)); }
            if (EndIndex >= BaseStr.Length) {throw new ArgumentOutOfRangeException("EndIndex", EndIndex, String.Format(
                "Конечный индекс подстроки '{0}' не может быть больше, чем максимально возможный индекс в базовой строке '{1}'", EndIndex, BaseStr.Length - 1)); }
            
            return new Substring(BaseStr, StartIndex, EndIndex - StartIndex + 1);
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
        /// <param name="Index">Индекс запрашиваемого символа в данной подстроке, который начинается с 0 включительно и не может превышать длину подстроки. 
        /// Транслируется в индекс базовой строки без материализации подстроки.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Char this[Int32 Index]
        {
            get
            {
                if (Index < 0) { throw new ArgumentOutOfRangeException("Index", Index, "Индекс запрашиваемого символа не может быть отрицательным числом"); }
                if(Index >= this._length)
                {
                    throw new ArgumentOutOfRangeException("Index", Index, String.Format(
                        "Индекс запрашиваемого символа '{0}' не может быть больше, чем максимально возможный индекс в данной подстроке '{1}'", Index, this._length - 1));
                }
                Int32 position_in_base_str = this._startIndex + Index;
                return this._baseStr[position_in_base_str];
            }
        }
        #endregion Properties

        #region Instance methods

        /// <summary>
        /// Копирует все символы из данной подстроки в новый символьный массив и возвращает его
        /// </summary>
        /// <returns></returns>
        public Char[] ToCharArray()
        {
            return this._baseStr.ToCharArray(this._startIndex, this._length);
        }

        /// <summary>
        /// Копирует определённое количество символов из данной подстроки в новый символьный массив и возвращает его
        /// </summary>
        /// <param name="StartIndexInSubstring">Начальная позиция в данной подстроке, с которой включительно начнётся копирование</param>
        /// <param name="SymbolsToCopy">Количество символов, которое требуется скопировать</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Char[] ToCharArray(int StartIndexInSubstring, int SymbolsToCopy)
        {
            if (StartIndexInSubstring < 0) { throw new ArgumentOutOfRangeException("StartIndexInSubstring", StartIndexInSubstring, 
                "Начальная позиция в подстроке не может быть отрицательным числом"); }
            if (StartIndexInSubstring >= this._length)
            {
                throw new ArgumentOutOfRangeException("StartIndexInSubstring", StartIndexInSubstring, String.Format(
                    "Начальный индекс в подстроке '{0}' не может быть больше, чем максимально возможный индекс в данной подстроке '{1}'", StartIndexInSubstring, this._length - 1));
            }
            if (SymbolsToCopy < 0) { throw new ArgumentOutOfRangeException("SymbolsToCopy", SymbolsToCopy, 
                "Количество символов для копирования не может быть отрицательным числом"); }
            if (StartIndexInSubstring + SymbolsToCopy > this._length)
            {
                throw new ArgumentException(String.Format("Результат сложения начального индекса '{0}' и количества символов для копирования '{1}' " +
                    " равен '{2}' и не может превышать общую длину подстроки '{3}'", 
                    StartIndexInSubstring, SymbolsToCopy, StartIndexInSubstring + SymbolsToCopy, this._length));
            }
            Int32 translated_start_index = this._startIndex + StartIndexInSubstring;
            return this._baseStr.ToCharArray(translated_start_index, SymbolsToCopy);
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
        /// <param name="Replacement">Новая строка, которая должна заменить в базовой строке данную подстроку. 
        /// Если NULL или пустая - в базовой строке будет вырезана данная подстрока без вставки новых символов</param>
        /// <returns>Новый экземпляр строки, представляющий собой версию базовой строки, где вместо данной подстроки присутствует указанная</returns>
        public String ReplaceInBaseString(String Replacement)
        {
            Boolean is_null_or_empty = Replacement.IsNullOrEmpty();
            if (this.IsComplete == true && is_null_or_empty == true)
            {
                return String.Empty;
            }
            if (this.IsComplete == true && is_null_or_empty == false)
            {
                return Replacement;
            }
            if (this.IsBeginning == true)
            {
                String base_part = this._baseStr.Substring(this.EndIndex + 1);
                if (is_null_or_empty)
                {
                    return base_part;
                }
                else
                {
                    return Replacement + base_part;
                }
            }
            else if (this.IsEnding == true)
            {
                String base_part = this._baseStr.Substring(0, this.StartIndex);
                if (is_null_or_empty)
                {
                    return base_part;
                }
                else
                {
                    return base_part + Replacement;
                }
            }
            else
            {
                String first_base_part = this._baseStr.Substring(0, this.StartIndex);
                String last_base_part = this._baseStr.Substring(this.EndIndex + 1);
                if (is_null_or_empty)
                {
                    return first_base_part + last_base_part;
                }
                else
                {
                    return first_base_part + Replacement + last_base_part;
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
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Boolean AreEqual(Substring First, Substring Second)
        {
            if (Object.ReferenceEquals(First, Second) == true) { return true; }
            if (Object.ReferenceEquals(First, null) == true || Object.ReferenceEquals(null, Second) == true) { return false; }
            if (Object.ReferenceEquals(First._baseStr, Second._baseStr) == false) { return false; }
            Boolean result = First._length == Second._length && First._startIndex == Second._startIndex;
            return result;
        }

        /// <summary>
        /// Определяет, все ли подстроке в указанном наборе равны между собой
        /// </summary>
        /// <param name="Substrings">Набор подстрок. Если NULL, пуст или содержит одну подстроку, будет выброшено исключение.</param>
        /// <returns>Если все подстроки равны - 'true', иначе 'false'</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Boolean AreEqual(params Substring[] Substrings)
        {
            if (Substrings == null) { throw new ArgumentNullException("Substrings"); }
            if (Substrings.Length == 0) { throw new ArgumentException("Набор подстрок не может быть пуст", "Substrings"); }
            if (Substrings.Length == 1) { throw new ArgumentException("Набор подстрок не может содержать только одну подстроку", "Substrings"); }
            if (Substrings.Length == 2) { return Substring.AreEqual(Substrings[0], Substrings[1]); }

            for (Int32 first_index = 0, second_index = first_index + 1; second_index < Substrings.Length; first_index++, second_index++)
            {
                if (Substring.AreEqual(Substrings[first_index], Substrings[second_index]) == false)
                { return false; }
            }
            return true;
        }

        /// <summary>
        /// Определяет, имеют ли два указанных экземпляра подстрок общую базовую строку. Если ссылки на экземпляры эквивалентны, 
        /// или же базовая строка для обоих является общей, возвращается true, а во всех остальных случаях - false.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Boolean HaveCommonBaseString(Substring First, Substring Second)
        {
            if (Object.ReferenceEquals(First, null) == true || Object.ReferenceEquals(null, Second) == true) { return false; }
            if (Object.ReferenceEquals(First, Second) == true) { return true; }
            return Object.ReferenceEquals(First._baseStr, Second._baseStr);
        }

        /// <summary>
        /// Определяет, имеют ли все указанные экземпляры общую базовую строку. Если ссылки на экземпляры эквивалентны, 
        /// или же базовая строка для всех является общей, возвращается true, а во всех остальных случаях - false.
        /// </summary>
        /// <param name="Substrings">Набор подстрок для проверки. Если NULL, пуст или содержит одну подстроку, будет выброшено исключение.</param>
        /// <returns>Если все подстроки имеют общую базовую строку - 'true', иначе 'false'</returns>
        public static Boolean HaveCommonBaseString(params Substring[] Substrings)
        {
            if (Substrings == null) { throw new ArgumentNullException("Substrings"); }
            if (Substrings.Length == 0) { throw new ArgumentException("Набор подстрок не может быть пуст", "Substrings"); }
            if (Substrings.Length == 1) { throw new ArgumentException("Набор подстрок не может содержать только одну подстроку", "Substrings"); }
            if (Substrings.Length == 2) { return Substring.HaveCommonBaseString(Substrings[0], Substrings[1]); }

            for (Int32 first_index = 0, second_index = first_index + 1; second_index < Substrings.Length; first_index++, second_index++)
            {
                if (Substring.HaveCommonBaseString(Substrings[first_index], Substrings[second_index]) == false)
                { return false; }
            }
            return true;
        }

        /// <summary>
        /// Сравнивает два экземпляра подстрок между собой, показывая их связь (больше-меньше-равно) в порядке сортировки
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Int32 Compare(Substring First, Substring Second)
        {
            if (Object.ReferenceEquals(First, Second) == true) { return 0; }
            if (Object.ReferenceEquals(First, null) == true) { return -1; }
            if (Object.ReferenceEquals(Second, null) == true) { return 1; }
            if (Substring.HaveCommonBaseString(First, Second) == false)
            {
                Int32 res = String.Compare(First.BaseString, Second.BaseString, StringComparison.Ordinal);
                if (res < 0) { return -1; }
                if (res > 0) { return 1; }
                return 0;
            }
            if (First.StartIndex != Second.StartIndex) { return First.StartIndex.CompareTo(Second.StartIndex); }
            return First.Length.CompareTo(Second.Length);
        }

        /// <summary>
        /// Определяет, пересекаются ли две подстроки, имеющие общую базовую строку. Идентичность подстрок также считается пересечением. 
        /// Если указанные подстроки не относятся к единой базовой строке, будет выброшено исключение.
        /// </summary>
        /// <param name="First">Первая подстрока для проверки на пересечение. Не может быть NULL.</param>
        /// <param name="Second">Вторая подстрока для проверки на пересечение. Не может быть NULL.</param>
        /// <returns></returns>
        public static Boolean AreIntersecting(Substring First, Substring Second)
        {
            if(First == null) {throw new ArgumentNullException("First");}
            if (Second == null) { throw new ArgumentNullException("Second"); }
            if (First == Second) { return true; }
            if (Substring.HaveCommonBaseString(First, Second) == false)
            {
                throw new ArgumentException("Указанные подстроки не относятся к единой базовой строке");
            }
            Boolean result = (First.EndIndex >= Second.StartIndex && First.StartIndex <= Second.StartIndex) ||
                (Second.EndIndex >= First.StartIndex && Second.StartIndex <= First.StartIndex);
            return result;
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
        /// <param name="Other"></param>
        /// <returns></returns>
        public override Boolean Equals(Object Other)
        {
            if (Object.ReferenceEquals(null, Other)) {return false;}
            if (Object.ReferenceEquals(this, Other)) {return true;}
            if (Other.GetType() != this.GetType()) {return false;}
            return Substring.AreEqual(this, (Substring)Other);
        }

        /// <summary>
        /// Определяет равенство данного экземпляра с указанным
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public Boolean Equals(Substring Other)
        {
            return Substring.AreEqual(this, Other);
        }

        /// <summary>
        /// Сравнивает данный экземпляр с указанным и показывает, как они соотносятся в порядке сортировки
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public Int32 CompareTo(Substring Other)
        {
            return Substring.Compare(this, Other);
        }

        /// <summary>
        /// Пытается привести указанный экземпляр неизвестного типа Object к данному типу, 
        /// а затем сравнивает его с указанным и показывает, как они соотносятся в порядке сортировки
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public int CompareTo(Object Other)
        {
            if (Object.ReferenceEquals(Other, null) == true)
            {
                throw new ArgumentNullException("Other", "Нельзя сравнить подстроку с NULL");
            }
            Substring converted = Other as Substring;
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
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Boolean operator ==(Substring First, Substring Second)
        {
            return Substring.AreEqual(First, Second);
        }

        /// <summary>
        /// Определяет, не равны ли два указанных экземпляра подстрок между собой
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Boolean operator !=(Substring First, Substring Second)
        {
            return !Substring.AreEqual(First, Second);
        }

        /// <summary>
        /// Определяет, строго больше ли первый указанный экземпляр второго с точки зрения сортировки
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Boolean operator >(Substring First, Substring Second)
        {
            return Substring.Compare(First, Second) == 1;
        }

        /// <summary>
        /// Определяет, строго меньше ли первый указанный экземпляр второго с точки зрения сортировки
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Boolean operator <(Substring First, Substring Second)
        {
            return Substring.Compare(First, Second) == -1;
        }

        /// <summary>
        /// Определяет, больше или равен первый указанный экземпляр по сравнению со вторым с точки зрения сортировки
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Boolean operator >=(Substring First, Substring Second)
        {
            return Substring.Compare(First, Second) >= 0;
        }

        /// <summary>
        /// Определяет, меньше или равен первый указанный экземпляр по сравнению со вторым с точки зрения сортировки
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Boolean operator <=(Substring First, Substring Second)
        {
            return Substring.Compare(First, Second) <= 0;
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
            private const Char _zeroChar = (Char) 0;

            internal SubstringCharEnumerator(Substring source)
            {
                this._baseStr = source._baseStr;
                this._startIndex = source._startIndex;
                this._length = source._length;
                this._currentIndex = -1;
                this._currentElement = _zeroChar;
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
                this._currentElement = _zeroChar;
            }

            Object System.Collections.IEnumerator.Current
            {
                get { return Current; }
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
                this._currentElement = _zeroChar;
                this._currentIndex = -1;
                this._isDisposed = true;
            }
        }
        #endregion Subclass
    }
}

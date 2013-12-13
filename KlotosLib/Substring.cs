using System;

namespace KlotosLib
{
    /// <summary>
    /// Представляет подстроку, привязанную к одной конкретной базовой строке. Неизменяемый класс.
    /// </summary>
    [Serializable]
    public class Substring : IEquatable<Substring>, IComparable<Substring>, ICloneable
    {
        #region Constructors
        /// <summary>
        /// Создаёт и возвращает глубокую копию подстроки на основании указанной
        /// </summary>
        /// <param name="Other">Если NULL - будет выброшено исключение</param>
        /// <exception cref="ArgumentNullException"></exception>
        private Substring(Substring Other)
        {
            if(Other.IsNull()==true) {throw new ArgumentNullException("Other");}
            this._baseStr = Other._baseStr;
            this._startIndex = Other._startIndex;
            this._length = Other._length;
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
        public Substring(String BaseStr, Int32 StartIndex, Int32 Length)
        {
            if (BaseStr == null) {throw new ArgumentNullException("BaseStr", "Базовая строка не может быть NULL");}
            if (BaseStr.Length == 0) {throw new ArgumentException("Базовая строка не может быть пустой", "BaseStr");}
            if (StartIndex < 0) {throw new ArgumentOutOfRangeException("StartIndex", StartIndex, "Начальный индекс подстроки не может быть отрицательным числом");}
            if (Length < 0) { throw new ArgumentOutOfRangeException("Length", Length, "Длина подстроки не может быть отрицательным числом"); }
            if (Length == 0) { throw new ArgumentOutOfRangeException("Length", Length, "Длина подстроки не может иметь нулевую длину"); }
            if (StartIndex >= BaseStr.Length) {throw new ArgumentOutOfRangeException("StartIndex", StartIndex, String.Format(
                "Начальный индекс подстроки '{0}' не может быть больше, чем максимально возможный индекс в базовой строке '{1}'", StartIndex, BaseStr.Length-1)); }
            if (StartIndex+Length > BaseStr.Length) {throw new ArgumentException(String.Format("Результат сложения начального индекса '{0}' и длины '{1}' "+
                "подстроки = '{2}' не может превышать длину базовой строки '{3}'", StartIndex, Length, StartIndex+Length, BaseStr.Length));}

            this._baseStr = BaseStr;
            this._startIndex = StartIndex;
            this._length = Length;
        }

        /// <summary>
        /// Создаёт и возвращает подстроку, основанную на указанной базовой строке, имеющую указанный начальный индекс, а длину вплоть до окончания базовой строки
        /// </summary>
        /// <param name="BaseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="StartIndex">Начальный (включительный) индекс подстроки в базовой строке</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Substring(String BaseStr, Int32 StartIndex) : this(BaseStr, StartIndex, BaseStr.Length - StartIndex) {}

        /// <summary>
        /// Создаёт и возвращает новый экземпляр подстроки, которая основана на указанной базовой строке, 
        /// имеет указанный начальный индекс и оканчивается указанным конечным индексом включительно
        /// </summary>
        /// <param name="BaseStr">Базовая строка. Если NULL или пустая - будет выброшено исключение.</param>
        /// <param name="StartIndex">Начальный (включительный) индекс подстроки в базовой строке</param>
        /// <param name="EndIndex">Конечный (включительный) индекс подстроки в базовой строке. 
        /// Если превышает максимальный допустимый индекс в базовой строке, будет выброшено исключение.</param>
        /// <returns></returns>
        public static Substring CreateWithEndIndex(String BaseStr, Int32 StartIndex, Int32 EndIndex)
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
        #endregion

        #region Fields
        private readonly String _baseStr;

        private readonly Int32 _startIndex;

        private readonly Int32 _length;

        private String _cachedValue;
        #endregion

        #region Properties
        /// <summary>
        /// Индекс первого символа данной подстроки в базовой строке (начальный индекс)
        /// </summary>
        public Int32 StartIndex { get { return this._startIndex; } }

        /// <summary>
        /// Индекс последнего символа данной подстроки в базовой строке (конечный индекс)
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
        /// Строковое представление данной подстроки
        /// </summary>
        public String Value {
            get
            {
                if (this._cachedValue == null)
                {
                    this._cachedValue = this._baseStr.Substring(this._startIndex, this._length);
                }
                return this._cachedValue;
            }
        }

        /// <summary>
        /// Базовая строка
        /// </summary>
        public String BaseString {get { return this._baseStr; }}
        #endregion

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
        /// Определяет, равны ли два указанных экземпляра подстрок между собой. Если обе подстроки являются NULL, то считается, что они равны. 
        /// Две не-NULL подстроки считаются равны тогда и только тогда, когда имеют одинаковые начальный индекс и длину, 
        /// а также привязаны к одному и тому же экземпляру базовой строки.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Boolean AreEqual(Substring First, Substring Second)
        {
            if (Object.ReferenceEquals(First, Second)==true) { return true; }
            if (Object.ReferenceEquals(First, null) == true || Object.ReferenceEquals(null, Second) == true) { return false; }
            if (Object.ReferenceEquals(First._baseStr, Second._baseStr) == false) {return false;}
            Boolean result = First._length == Second._length && First._startIndex == Second._startIndex;
            return result;
        }

        /// <summary>
        /// Определяет, все ли подстроке в указанном наборе равны между собой
        /// </summary>
        /// <param name="Substrings">Набор подстрок. Если NULL, пуст или содержит одну подстроку, будет выброшено исключение.</param>
        /// <returns></returns>
        public static Boolean AreEqual(params Substring[] Substrings)
        {
            if(Substrings == null) {throw new ArgumentNullException("Substrings");}
            if (Substrings.Length == 0) {throw new ArgumentException("Набор подстрок не может быть пуст", "Substrings");}
            if (Substrings.Length == 1) { throw new ArgumentException("Набор подстрок не может содержать только одну подстроку", "Substrings"); }
            if (Substrings.Length == 2) {return Substring.AreEqual(Substrings[0], Substrings[1]);}
            for (Int32 first_pairwise_comparison = 0; first_pairwise_comparison < Substrings.Length; first_pairwise_comparison++)
            {
                for (Int32 second_pairwise_comparison = first_pairwise_comparison + 1; second_pairwise_comparison < Substrings.Length; second_pairwise_comparison++)
                {
                    if (Substring.AreEqual(Substrings[first_pairwise_comparison], Substrings[second_pairwise_comparison])==false) {return false;}
                }
            }
            return true;
        }

        /// <summary>
        /// Определяют, имеют ли два указанных экземпляра подстрок общую базовую строку. Если ссылки на экземпляры эквивалентны, 
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
        /// Сравнивает два экземпляра подстрок между собой, показывая их связь (больше-меньше-равно) в порядке сортировки
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Int32 Compare(Substring First, Substring Second)
        {
            if (Object.ReferenceEquals(First, Second) == true) {return 0;}
            if (Object.ReferenceEquals(First, null) == true) { return -1; }
            if (Object.ReferenceEquals(Second, null) == true) { return 1; }
            if (Substring.HaveCommonBaseString(First, Second) == false)
            {
                Int32 res = String.Compare(First.BaseString, Second.BaseString, StringComparison.Ordinal);
                if (res < 0) {return -1;}
                if (res > 0) {return 1;}
                return 0;
            }
            if (First.StartIndex != Second.StartIndex) {return First.StartIndex.CompareTo(Second.StartIndex);}
            return First.Length.CompareTo(Second.Length);
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
        /// Возвращает строковое представление данной подстроки
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
        #endregion
    }
}

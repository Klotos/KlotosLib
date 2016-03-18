using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using KlotosLib.StringTools;

namespace KlotosLib
{
    /// <summary>
    /// Содержит статические чистые методы по работе с самыми общими аспектами переменных
    /// </summary>
    public static class CommonTools
    {
        /// <summary>
        /// Определяет, содержит ли экземпляр ссылочного типа null-значение
        /// </summary>
        /// <typeparam name="T">Тип, являющийся ссылочным</typeparam>
        /// <param name="Source">Экземпляр, который необходимо проверить</param>
        /// <returns></returns>
        public static Boolean IsNull<T>(this T Source) where T : class
        {
            return Object.ReferenceEquals(null, Source);
        }

        /// <summary>
        /// Определяет, находится ли данный объект в массиве указанных объектов
        /// </summary>
        /// <typeparam name="T">Тип, который поддерживает сравнение с самим собою посредством реализации интерфейса System.IEquatable&lt;TNumber&gt;</typeparam>
        /// <param name="Source">Значение, для которого необходимо определить, находится ли оно в указанной последовательности или нет</param>
        /// <param name="Sequence">Набор элементов, в котором происходит поиск. Если NULL - будет выброшено исключение.</param>
        /// <returns>true - значение присутствует, false - значение отсутствует.</returns>
        public static Boolean IsIn<T>(this T Source, params T[] Sequence) where T : IEquatable<T>
        {
            if (Sequence.IsNull() == true) { throw new ArgumentNullException("Sequence", "Указанный массив, в котором производится поиск, является NULL"); }
            return Sequence.Contains(Source);
        }

        /// <summary>
        /// Определяет, находится ли данный объект в наборе указанных объектов
        /// </summary>
        /// <typeparam name="T">Тип, который поддерживает сравнение с самим собою посредством реализации интерфейса System.IEquatable&lt;TNumber&gt;</typeparam>
        /// <param name="Source">Значение, для которого необходимо определить, находится ли оно в указанном наборе или нет</param>
        /// <param name="Set">Набор элементов, в котором происходит поиск. Если NULL - будет выброшено исключение.</param>
        /// <returns></returns>
        public static Boolean IsIn<T>(this T Source, IEnumerable<T> Set) where T : IEquatable<T>
        {
            if (Set.IsNull() == true) { throw new ArgumentNullException("Set", "Указанный набор, в котором производится поиск, является NULL"); }
            return Set.Contains(Source);
        }

        /// <summary>
        /// Определяет, является ли указанный объект больше (или равно) первого сравниваемого граничного значения и меньше (или равно) второго. 
        /// Тип объекта должен поддерживать сравнение со своим типом.
        /// </summary>
        /// <remarks>Порядок сравниваемых граничных значений имеет критическое значение: 5.IsBetween(6, 4, true) возвратит false</remarks>
        /// <typeparam name="T">Тип, который поддерживает сравнение с самим собою посредством реализации интерфейса System.IComparable&lt;TNumber&gt;</typeparam>
        /// <param name="Actual">Текущий объект, который надо сравнить с двумя другими</param>
        /// <param name="Lower">Объект, который, в случае успешного сравнения, должен быть меньше (или равен) текущему. 
        /// Если больше текущего, метод возвратит false.</param>
        /// <param name="Upper">Объект, который, в случае успешного сравнения, должен быть больше (или равен) текущему. 
        /// Если меньше текущего, метод возвратит false.</param>
        /// <param name="IncludeBounds">Флаг, определяющий, включать ли границы (true) или нет (false)</param>
        /// <returns>Булевый флаг: true - указаный объект находится между двумя другими, false - вне границ</returns>
        public static Boolean IsBetween<T>(this T Actual, T Lower, T Upper, Boolean IncludeBounds) where T : System.IComparable<T>
        {
            if (typeof(T).IsValueType == false)
            {
                if (Object.ReferenceEquals(null, Actual) == true) { throw new ArgumentNullException("Actual"); }
                if (Object.ReferenceEquals(null, Lower) == true) { throw new ArgumentNullException("Lower"); }
                if (Object.ReferenceEquals(null, Upper) == true) { throw new ArgumentNullException("Upper"); }
            }

            Int32 first_comparation = Actual.CompareTo(Lower);
            Int32 last_comparation = Actual.CompareTo(Upper);

            if (IncludeBounds == true)
            {
                if (first_comparation >= 0 && last_comparation <= 0) { return true; }
                else { return false; }
            }
            else
            {
                if (first_comparation > 0 && last_comparation < 0) { return true; }
                else { return false; }
            }
        }

        /// <summary>
        /// Определяет, находится ли указанный объект между двумя другими сравниваемыми граничными значениями вне зависимости от их порядка. 
        /// Тип объекта должен поддерживать сравнение со своим типом.
        /// </summary>
        /// <remarks>Порядок сравниваемых граничных значений не имеет значения: 5.IsBetween(4, 6, true) == 5.IsBetween(6, 4, true) == true</remarks>
        /// <typeparam name="T">Тип, который поддерживает сравнение с самим собою посредством реализации интерфейса System.IComparable&lt;TNumber&gt;</typeparam>
        /// <param name="Actual">Текущий объект, который надо сравнить с двумя другими</param>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <param name="IncludeBounds"></param>
        /// <returns></returns>
        public static Boolean IsBetweenIB<T>(this T Actual, T First, T Second, Boolean IncludeBounds) where T : System.IComparable<T>
        {
            if (typeof(T).IsValueType == false)
            {
                if (Object.ReferenceEquals(null, Actual) == true) { throw new ArgumentNullException("Actual"); }
                if (Object.ReferenceEquals(null, First) == true) { throw new ArgumentNullException("First"); }
                if (Object.ReferenceEquals(null, Second) == true) { throw new ArgumentNullException("Second"); }
            }

            List<T> both = new List<T>(2) { First, Second };
            T lesser = both.Min();
            T bigger = both.Max();
            return Actual.IsBetween<T>(lesser, bigger, IncludeBounds);
        }

        /// <summary>
        /// Меняет местами две переменные ссылочного типа (переменные значимого типа не поддерживаются)
        /// </summary>
        /// <remarks>Если обе переменные одновременно являются NULL, или если они обе указывают на один и тот же экземплер класса в памяти,
        /// метод не делает никаких операций.</remarks>
        /// <typeparam name="T">Тип, принадлежать к которому должны обе переменные</typeparam>
        /// <param name="Value1">Первая переменная ссылочного типа, которая в результате должна указывать на значение второй.</param>
        /// <param name="Value2">Вторая переменная ссылочного типа, которая в результате должна указывать на значение первой.</param>
        public static void Swap2Types<T>(ref T Value1, ref T Value2) where T : class
        {
            if (Object.ReferenceEquals(Value1, Value2) == true) { return; }
            T temp = Value1;
            Value1 = Value2;
            Value2 = temp;
        }

        /// <summary>
        /// Пытается преобразовать строку в логическое значение, и если преобразование провалилось, возвращает NULL.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static Nullable<Boolean> TryParse(String Source)
        {
            if (Source.HasVisibleChars() == false) { return null; }
            String temp = Source.CleanString().Trim();
            const StringComparison option = StringComparison.OrdinalIgnoreCase;
            if (temp.Equals("true", option) == true ||
                temp.Equals("1", option) == true ||
                temp.Equals("+", option) == true)
            { return true; }
            if (temp.Equals("false", option) == true ||
                temp.Equals("0", option) == true ||
                temp.Equals("-", option) == true)
            { return false; }
            return null;
        }

        /// <summary>
        /// Пытается преобразовать неизвестный тип данных в логическое значение, и если преобразование провалилось, возвращает NULL.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static Nullable<Boolean> TryParse(Object Source)
        {
            if (Object.ReferenceEquals(null, Source) == true) { return null; }
            Type source_type = Source.GetType();
            if (source_type == typeof(Boolean)) { return (Boolean)Source; }
            return CommonTools.TryParse(Source.ToStringS());
        }

        /// <summary>
        /// Опция форматирования булевого типа
        /// </summary>
        public enum BooleanStr : byte
        {
            /// <summary>
            /// True или False словами (первая буква с Большой литеры)
            /// </summary>
            TrueFalse = 0,

            /// <summary>
            /// true или false словами (первая буква с малнькой литеры)
            /// </summary>
            TrueFalseCaseless = 1,

            /// <summary>
            /// 1 или 0 цифрой
            /// </summary>
            OneZero = 2,

            /// <summary>
            /// 1 или -1 числами
            /// </summary>
            OneNegate = 3,

            /// <summary>
            /// + или - знаками
            /// </summary>
            PlusMinus = 4
        }

        /// <summary>
        /// Преобразовывает текущее булевое значение в строковое представление согласно указаннным опциям
        /// </summary>
        /// <param name="SourceValue"></param>
        /// <param name="Option">Опция, влияющая на строковое представление значения. Если некорректна, будет возвращено значение по умолчанию <code>Boolean.ToString()</code>.</param>
        /// <returns></returns>
        public static String ToString(this Boolean SourceValue, BooleanStr Option)
        {
            switch (Option)
            {
                case BooleanStr.TrueFalse:
                    return SourceValue == true ? "True" : "False";
                case BooleanStr.TrueFalseCaseless:
                    return SourceValue == true ? "true" : "false";
                case BooleanStr.OneZero:
                    return SourceValue == true ? "1" : "0";
                case BooleanStr.OneNegate:
                    return SourceValue == true ? "1" : "-1";
                case BooleanStr.PlusMinus:
                    return SourceValue == true ? "+" : "-";
                default:
                    return SourceValue.ToString();
            }
        }

        /// <summary>
        /// Пытается привести указанный тип к строке, и если это не удаётся, или же полученный экземпляр является NULL, то возвращает одну из указанных строк
        /// </summary>
        /// <typeparam name="T">Тип без ограничений</typeparam>
        /// <param name="Source">Экземпляр типа <typeparamref name="T"/></param>
        /// <param name="IfNull">Строка, которая будет возвращена, если указанный экземпляр равен NULL.</param>
        /// <param name="IfFail">Строка, которая будет возвращена, указанный экземпляр не NULL, однако преобразование в строку завершилось неудачей.</param>
        /// <returns></returns>
        public static String ToStringS<T>(this T Source, String IfNull, String IfFail)
        {
            Type type = typeof(T);
            if (type.IsValueType == false && Object.ReferenceEquals(null, Source) == true) { return IfNull; }
            try
            {
                String str = Source.ToString();
                return str;
            }
            catch { return IfFail; }
        }

        /// <summary>
        /// Пытается привести указанный тип к строке, и если это не удаётся, возвращает указанную строку
        /// </summary>
        /// <typeparam name="T">Тип без ограничений</typeparam>
        /// <param name="Source"></param>
        /// <param name="IfNullOrFail"></param>
        /// <returns></returns>
        public static String ToStringS<T>(this T Source, String IfNullOrFail)
        {
            return Source.ToStringS(IfNullOrFail, IfNullOrFail);
        }

        /// <summary>
        /// Пытается привести указанный тип к строке, и если это не удаётся, возвращает пустую
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static String ToStringS<T>(this T Source)
        {
            return Source.ToStringS(String.Empty);
        }

        /// <summary>
        /// Конвертирует входной файл в байтовый массив. Если указанный файл отсутствует или недоступен, выбрасывается исключение.
        /// </summary>
        /// <param name="Source">Объект FileInfo, содержащимй информацию о файле, который необходимо сконвертировать</param>
        /// <returns></returns>
        public static Byte[] ConvertToByteArray(this System.IO.FileInfo Source)
        {
            if (Source == null) {throw new ArgumentNullException("Source");}
            if (Source.Exists == false)
            {
                throw new FileNotFoundException("Файл не найден", Source.FullName);
            }
            if (Source.Length < 1)
            {
                return new Byte[0]{};
            }
            Byte[] output = new byte[Source.Length];
            using (FileStream fs = new FileStream(Source.FullName, FileMode.Open, FileAccess.Read))
            {
                fs.Position = 0;
                fs.Read(output, 0, Convert.ToInt32(fs.Length));
                fs.Close();
            }
            return output;
        }

        /// <summary>
        /// Сравнивает между собой элементы последовательности, определяя, все ли оны равны. Если все равны между собой, возвращает true. Если хотя бы один отличается - возвращает false.
        /// </summary>
        /// <typeparam name="T">Тип, который поддерживает нахождение равенства с самим собою посредством реализации интерфейса System.IEquatable&lt;TNumber&gt;</typeparam>
        /// <param name="Sequence">Список элементов. Если NULL, пустой или содержит лишь один элемент, выбрасывается исключение.</param>
        /// <returns></returns>
        public static Boolean AreAllEqual<T>(params T[] Sequence) where T : IEquatable<T>
        {
            if (Sequence.IsNullOrEmpty() == true) { throw new ArgumentException("Невозможно сравнивать между собой отсутствующие элементы", "Sequence"); }
            if (Sequence.HasSingle() == true) { throw new ArgumentException("Для сравнения должно присутствовать минимум 2 элемента, а не 1", "Sequence"); }
            for (int i = 0; i < Sequence.Length - 1; i++)
            {
                if (Sequence[i].Equals(Sequence[i + 1]) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Определяет, равны ли два указанных символа между собой
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <param name="IgnoreCase">Определяет, игнорировать ли при сравнении символов регистр литер. true - игнорировать, false - принимать во внимание.</param>
        /// <returns></returns>
        public static Boolean AreCharsEqual(Char First, Char Second, Boolean IgnoreCase)
        {
            Boolean res;
            if (IgnoreCase == false)
            {
                res = First == Second;
            }
            else
            {
                res = Char.ToUpperInvariant(First) == Char.ToUpperInvariant(Second);
            }
            return res;
        }

        /// <summary>
        /// Определяет, все ли символы равны между собой
        /// </summary>
        /// <param name="IgnoreCase">Определяет, игнорировать ли при сравнении символов регистр литер. true - игнорировать, false - принимать во внимание.</param>
        /// <param name="Chars">Набор символов, все из которых следует проверить на равенство. Если NULL или содержит меньше 2-х элементов, будет выброшено исключение.</param>
        /// <returns></returns>
        public static Boolean AreCharsEqual(Boolean IgnoreCase, params Char[] Chars)
        {
            if (Chars == null) { throw new ArgumentNullException("Chars"); }
            Int32 len = Chars.Length;
            if (len < 2) { throw new ArgumentException("Набор должен состоять из минимум 2 элементов, тогда как он содержит " + len, "Chars"); }
            if (len == 2)
            { return CommonTools.AreCharsEqual(Chars[0], Chars[1], IgnoreCase); }
            Char current;
            if (IgnoreCase == false)
            {
                Char first = Chars[0];
                for (Int32 i = 1; i < len; i++)
                {
                    current = Chars[i];
                    if (first != current) { return false; }
                }
            }
            else
            {
                Char first = Char.ToLowerInvariant(Chars[0]);
                for (Int32 i = 1; i < len; i++)
                {
                    current = Char.ToLowerInvariant(Chars[i]);
                    if (first != current) { return false; }
                }
            }
            return true;
        }
    }
}

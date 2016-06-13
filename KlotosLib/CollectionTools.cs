using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.ComponentModel;
using KlotosLib.StringTools;

namespace KlotosLib
{
    /// <summary>
    /// Содержит статические методы и методы расширения по работе с последовательностями
    /// </summary>
    public static class CollectionTools
    {
        /// <summary>
        /// Определяет, является ли последовательность NULL или пустой
        /// </summary>
        /// <typeparam name="T">Тип элементов последовательности - без ограничений</typeparam>
        /// <param name="Source">Любая последовательность, реализующая обобщённый интерфейс IEnumerable&#60;T&#62;</param>
        /// <returns>Если возвращает "true" — последовательность NULL или пустая. Если "false" — последовательность содержит минимум 1 элемент.</returns>
        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> Source)
        {
            return Object.ReferenceEquals(Source, null) || CollectionTools.IsEmpty(Source);
        }

        /// <summary>
        /// Определяет, является ли последовательность пустой
        /// </summary>
        /// <typeparam name="TItem">Тип элементов последовательности - без ограничений</typeparam>
        /// <param name="Source">Любая последовательность, реализующая обобщённый интерфейс IEnumerable&#60;T&#62;</param>
        /// <returns>Если возвращает "true" — пустая. Если "false" — последовательность является NULL или содержит минимум 1 элемент.</returns>
        public static Boolean IsEmpty<TItem>(this IEnumerable<TItem> Source)
        {
            if (Object.ReferenceEquals(Source, null) == true) {return false;}
            ICollection<TItem> temp = Source as ICollection<TItem>;
            if (temp != null)
            {
                return temp.Count == 0;
            }
            using (IEnumerator<TItem> enumerator = Source.GetEnumerator())
            {
                Boolean first = enumerator.MoveNext();
                return !first;
            }
        }

        /// <summary>
        /// Определяет, содержит ли последовательность только один элемент. Если содержит - возвращает 'true', во всех остальных случаях возвращает 'false'. 
        /// Метод использует перечислитель и не нуждается в вычислении количества элементов последовательности.
        /// </summary>
        /// <typeparam name="TItem">Тип элементов последовательности - без ограничений</typeparam>
        /// <param name="Source">Любая последовательность, которая должна быть проверена</param>
        /// <returns>Если 'true' — последовательность содержит один и только один элемент. Если NULL, пустая или больше одного элемента - возвращается 'false'.</returns>
        public static Boolean HasSingle<TItem>(this IEnumerable<TItem> Source)
        {
            if (Object.ReferenceEquals(Source, null) == true) {return false;}
            ICollection<TItem> temp = Source as ICollection<TItem>;
            if (temp != null)
            {
                if (temp.Count == 1) { return true; }
                else { return false; }
            }
            using (IEnumerator<TItem> enumerator = Source.GetEnumerator())
            {
                Boolean first = enumerator.MoveNext();
                if (first == false) {return false;}
                Boolean second = enumerator.MoveNext();
                return !second;
            }
        }

        #region Deconcatenation
        /// <summary>
        /// Производит деконкатенацию входной строки, возвращая преобразованный строго типизированный список находящихся в ней элементов. 
        /// Принимает делегат на функцию, возвращающую один элемент из подстроки, разделитель элементов в строке, начало и конец строки.
        /// </summary>
        /// <typeparam name="TItem">Тип элемента входной строки - без ограничений</typeparam>
        /// <param name="Source">Входная строка, которая деконкатенируется. Если NULL, пустая или не содержит ни единого видимого символа, то в зависимости от значения параметра <paramref name="Strict"/> 
        /// будет возвращён пустой список или выброшено исключение.</param>
        /// <param name="ConversionFunc">Делегат на метод, который принимает строку и возвращает из неё экземпляр типа <typeparamref name="TItem"/>. Не может быть NULL. 
        /// Если в процессе парсинга данный метод выбрасывает исключение, то в зависимости от значения параметра <paramref name="Strict"/> элемент будет пропущен или будет выброшено исключение.</param>
        /// <param name="Prefix">Префикс входной строки, который отбрасывается. Не обязателен, может быть NULL; в таком случае парсинг идёт с самого первого символа.</param>
        /// <param name="Ending">Префикс входной строки, которое отбрасывается. Не обязательно, может быть NULL; в таком случае парсинг идёт до самого последнего символа.</param>
        /// <param name="Divider">Разделитель элементов. Обязателен, не может быть NULL или пустой строкой, ибо тогда метод не будет знать, где оканчивается один элемент и начинается второй.</param>
        /// <param name="Strict">Флаг "строгости" валидации входных данных. Если true - то при некорректности входной строки или выбрасывании исключения внутри метода конвертации 
        /// данный метод выбросит исключение. Если false - то возвратит пустой список в первом случае и пропустит элемент во втором.</param>
        /// <returns></returns>
        public static List<TItem> DeconcatFromString<TItem>(this String Source, Func<String, TItem> ConversionFunc, String Prefix, String Ending, String Divider, Boolean Strict)
        {
            if (Source.HasVisibleChars() == false)
            {
                if (Strict == true)
                { throw new ArgumentException("Входная строка пустая", "Source"); }
                else
                { return new List<TItem>(0); }
            }
            if (Divider.IsStringNullOrEmpty() == true)
            { throw new ArgumentException("Разделитель не может быть NULL или пустой строкой"); }
            if (ConversionFunc == null)
            { throw new ArgumentNullException("ConversionFunc"); }
            String temp = Source.TrimStart(Prefix, StringComparison.Ordinal, false).TrimEnd(Ending, StringComparison.Ordinal, false);
            String[] parts = temp.Split(new String[1] { Divider }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                if (Strict == true)
                { throw new ArgumentException("При разделении входной строки не был возвращён ни один элемент", "Source"); }
                else
                { return new List<TItem>(0); }
            }
            List<TItem> output = new List<TItem>(parts.Length);

            for (Int32 i = 0; i < parts.Length; i++)
            {
                try
                {
                    TItem one_res = ConversionFunc.Invoke(parts[i]);
                    output.Add(one_res);
                }
                catch (Exception ex)
                {
                    if (Strict == true)
                    {
                        throw new InvalidOperationException(
                          String.Format("На итерации {0} функция конвертации значения, получив на вход строку '{1}', выбросила исключение",
                          i + 1, parts[i]), ex);
                    }
                }
            }
            return output;
        }

        /// <summary>
        /// Производит деконкатенацию входной строки, возвращая преобразованный строго типизированный словарь ключей и значений. 
        /// Принимает разделитель пар 'ключ-значение', разделитель между ключем и значением, а также два делегата на функции, возвращающие из подстрок ключи и значения.
        /// </summary>
        /// <typeparam name="TKey">Тип ключа словаря, должен реализовывать IEquatable&lt;TKey&gt;</typeparam>
        /// <typeparam name="TValue">Тип значения словаря</typeparam>
        /// <param name="Source">Входная строка. Если NULL или не содержит видимых символов, 
        /// то в зависимости от значения параметра <paramref name="Strict"/> будет выброшено исключение или возвращён NULL.</param>
        /// <param name="KeyConversionFunc">Делегат на функцию конвертации строки в тип ключа. Если NULL - будет выброшено исключение. Если во время конвертации функция выбросит исключение 
        /// или возвратит NULL, то в зависимости от значения параметра <paramref name="Strict"/> это исключение будет проброшено или же этот элемент будет пропущен (вместе со значением).</param>
        /// <param name="ValueConversionFunc">Делегат на функцию конвертации строки в тип значения. Если NULL - будет выброшено исключение. Если во время конвертации функция выбросит исключение, 
        /// то в зависимости от значения параметра <paramref name="Strict"/> это исключение будет проброшено или же этот элемент будет пропущен (вместе с ключем).</param>
        /// <param name="ItemDelimiter">Строковый разделитель между раздельными элементами (парами 'ключ-значение'). 
        /// Если NULL или пустая строка, или же если равен разделителю между ключем и значением — будет выброшено исключение.</param>
        /// <param name="KeyValueDelimiter">Строковый разделитель между одним ключем и значением. 
        /// Если NULL или пустая строка, или же если равен разделителю между элементами — будет выброшено исключение.</param>
        /// <param name="Strict">Определяет "строгость" выполнения: true - более строгий режим, при любых ошибках выбрасывать исключение; false - более щадящий режим, по возможности продолжать работу. 
        /// Если ключи дублируются, то в зависимости от параметра будет выброшено исключение или же элемент будет пропущен.</param>
        /// <exception cref="InvalidOperationException">Все ошибки при парсинге и работе с делегатом функции, не являющиеся предусловиями</exception>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> DeconcatFromString<TKey, TValue>(this String Source, Func<String, TKey> KeyConversionFunc, Func<String, TValue> ValueConversionFunc,
            String ItemDelimiter, String KeyValueDelimiter, Boolean Strict)
            where TKey : IEquatable<TKey>
        {
            if (Source.HasVisibleChars() == false)
            {
                if (Strict == true)
                { throw new ArgumentException("Входная строка не содержит видимых символов", "Source"); }
                else
                { return null; }
            }

            if (KeyConversionFunc == null) { throw new ArgumentNullException("KeyConversionFunc"); }
            if (ValueConversionFunc == null) { throw new ArgumentNullException("ValueConversionFunc"); }
            if (ItemDelimiter.IsStringNullOrEmpty() == true) { throw new ArgumentException("Строковый разделитель между раздельными элементами является NULL или пустой строкой", "ItemDelimiter"); }
            if (KeyValueDelimiter.IsStringNullOrEmpty() == true) { throw new ArgumentException("Строковый разделитель между ключем и значением является NULL или пустой строкой", "KeyValueDelimiter"); }
            if (ItemDelimiter.Equals(KeyValueDelimiter, StringComparison.Ordinal) == true)
            {
                throw new ArgumentException
                    ("Строковые разделители между раздельными элементами и между ключем и значением равны между собой: '" + ItemDelimiter + "'");
            }

            String[] items = Source.Split(new String[1] { ItemDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 0) { return new Dictionary<TKey, TValue>(0); }

            Dictionary<TKey, TValue> output = new Dictionary<TKey, TValue>(items.Length);

            for (Int32 i = 0; i < items.Length; i++)
            {
                String one_item = items[i];
                String[] pair = one_item.Split(new String[1] { KeyValueDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                if (pair.Length != 2)
                {
                    if (Strict == true)
                    {
                        throw new InvalidOperationException(String.Format("На итерации {0} элемент '{1}' посредством разделителя '{2}' был разделен на {3} элемент/ов, а не на 2",
                          i + 1, one_item, KeyValueDelimiter, pair.Length));
                    }
                    else
                    { continue; }
                }
                TKey key;
                try
                {
                    key = KeyConversionFunc.Invoke(pair[0]);
                }
                catch (Exception ex)
                {
                    if (Strict == true)
                    {
                        throw new InvalidOperationException(String.Format("На итерации {0} функция конвертации ключа, получив на вход строку '{1}', выбросила исключение",
                            i + 1, pair[0]), ex);
                    }
                    else
                    { continue; }
                }
                if (Object.ReferenceEquals(null, key) == true)
                {
                    if (Strict == true)
                    { throw new InvalidOperationException(String.Format("На итерации {0} функция конвертации ключа, получив на вход строку '{1}', возвратила NULL", i + 1, pair[0])); }
                    else
                    { continue; }
                }
                TValue value;
                try
                {
                    value = ValueConversionFunc.Invoke(pair[1]);
                }
                catch (Exception ex)
                {
                    if (Strict == true)
                    {
                        throw new InvalidOperationException(String.Format("На итерации {0} функция конвертации значения, получив на вход строку '{1}', выбросила исключение",
                            i + 1, pair[1]), ex);
                    }
                    else
                    { continue; }
                }
                if (output.ContainsKey(key) == true)
                {
                    if (Strict == true)
                    {
                        throw new InvalidOperationException(String.Format("На итерации {0} было обнаружено, что ключ '{1}' уже присутствует в словаре, который на этот момент содержал {2} элементов",
                            i + 1, key.ToString(), output.Count));
                    }
                    else
                    { continue; }
                }
                output.Add(key, value);
            }
            return output;
        }
        #endregion

        #region Concatenation
        /// <summary>
        /// Конкатенирует все элементы входной последовательности в одну строку в прямом порядке, от начала и до конца. 
        /// Использует делегат, конвертирующий элементы в строки, а если делегат не задан (является NULL), то производит конвертацию в строки посредством ToString(). 
        /// Также использует указанный префикс, окончание и разделитель. В зависимости от параметра Strict в некоторых ситуациях может или выбрасывать исключение, или возвращать пустую строку.
        /// </summary>
        /// <typeparam name="T">Тип элементов входной строго типизированной последовательности</typeparam>
        /// <param name="Source">Входная последовательность, элементы которой следует конкатенировать</param>
        /// <param name="ConversionFunc">Делегат на функцию, принимающую на вход аргумент типа 'T' и возвращающую 'System.String'</param>
        /// <param name="Prefix"></param>
        /// <param name="Ending"></param>
        /// <param name="Divider"></param>
        /// <param name="Strict">Определяет строгость валидации входной последовательности, префикса, окончания и разделителя. 
        /// Если true - при некорректных параметрах будет выброшено исключение, а если false - 
        /// будет возвращена пустая строка (в случае некорректной входной последовательности) или в качестве разделителей будут использоваться пустые строки.</param>
        /// <returns></returns>
        public static String ConcatToString<T>(this IEnumerable<T> Source, Func<T, String> ConversionFunc, String Prefix, String Ending, String Divider, Boolean Strict)
        {
            const String def = "";
            if (Source == null)
            {
                if (Strict == true) { throw new ArgumentNullException("Source", "Конкатенируемая последовательность является NULL"); }
                else { return def; }
            }
            if (Source.Any<T>() == false)
            {
                if (Strict == true) 
                { throw new ArgumentException("Конкатенируемая последовательность типа '"+Source.GetType().Name+"' не содержит ни единого элемента", "Source"); }
                else { return def; }
            }
            Func<T, String> conv;
            if (ConversionFunc == null)
            {
                conv = delegate(T arg) { return arg.ToString(); };
            }
            else
            {
                conv = ConversionFunc;
            }
            if (Prefix == null)
            {
                if (Strict == true) { throw new ArgumentNullException("Prefix"); }
                else { Prefix = def; }
            }
            if (Ending == null)
            {
                if (Strict == true) { throw new ArgumentNullException("Ending"); }
                else { Ending = def; }
            }
            if (Divider == null)
            {
                if (Strict == true) { throw new ArgumentNullException("Divider"); }
                else { Divider = def; }
            }

            StringBuilder output = new StringBuilder(Prefix);
            foreach (T elem in Source)
            {
                output.Append(conv.Invoke(elem) + Divider);
            }
            output.TrimEnd(Divider, false, false);
            output.Append(Ending);
            return output.ToString();
        }

        /// <summary>
        /// Конкатенирует все элементы входного словаря, содержащего набор пар "ключ-значение", в одну строку, в прямом порядке, от начала и до конца. 
        /// Использует отдельные указанные делегаты для ключа и значения, позволяющие, соответственно, преобразовать ключ и значение в строку. 
        /// Также принимает два разделителя - между ключем и значением, и между парами элементов.
        /// </summary>
        /// <typeparam name="TKey">Тип ключа словаря</typeparam>
        /// <typeparam name="TValue">Тип значения словаря</typeparam>
        /// <param name="Source">Исходный словарь, реализующий интерфейс IDictionary. Если NULL — будет возвращён NULL, а если пуст - будет возвращена пустая строка.</param>
        /// <param name="KeyToStrFunc">Делегат на метод, принимающий один параметр типа <typeparamref name="TKey"/> и возвращающий строку. Если не указан (является NULL), 
        /// ключ будет приведён к строке посредством вызова метода ToString().</param>
        /// <param name="ValueToStrFunc">Делегат на метод, принимающий один параметр типа <typeparamref name="TValue"/> и возвращающий строку. Если не указан (является NULL), 
        /// значение будет приведено к строке посредством вызова метода ToString().</param>
        /// <param name="KeyValueDivider">Разделитель между ключем и значением. Если является NULL, в качестве такого будет использован ' - '.</param>
        /// <param name="ItemsDivider">Разделитель между отдельными парами "ключ-значение". Если является NULL, в качестве такого будет использован '; '.</param>
        /// <returns></returns>
        public static String ConcatToString<TKey, TValue>(this IDictionary<TKey, TValue> Source,
            Func<TKey, String> KeyToStrFunc, Func<TValue, String> ValueToStrFunc, String KeyValueDivider, String ItemsDivider)
        {
            if (Source == null) { return null; }
            if (Source.Any() == false) { return String.Empty; }

            Func<TKey, String> key_temp;
            if (KeyToStrFunc != null)
            { key_temp = KeyToStrFunc; }
            else
            {
                key_temp = delegate(TKey key)
                {
                    return key.ToString();
                };
            }

            Func<TValue, String> value_temp;
            if (ValueToStrFunc != null)
            { value_temp = ValueToStrFunc; }
            else
            {
                value_temp = delegate(TValue value)
                {
                    return value.ToStringS();
                };
            }

            if (KeyValueDivider == null) { KeyValueDivider = " - "; }
            if (ItemsDivider == null) { ItemsDivider = "; "; }

            StringBuilder output = new StringBuilder(Source.Count * 10);
            foreach (KeyValuePair<TKey, TValue> one_pair in Source)
            {
                output.Append(key_temp.Invoke(one_pair.Key) + KeyValueDivider + value_temp.Invoke(one_pair.Value) + ItemsDivider);
            }
            output.TrimEnd(ItemsDivider, false, false);
            return output.ToString();
        }

        /// <summary>
        /// Конкатенирует все элементы входной последовательности в одну строку в прямом порядке, от начала и до конца. 
        /// Использует указанный префикс, окончание, разделитель и конечный разделитель. 
        /// Также может пропускать невалидные элементы и удалять копии.
        /// </summary>
        /// <remarks>Метод генерирует исключение, если входная последовательность NULL или пустая, или если хотя бы один из строковых аргументов является NULL. 
        /// Если значение параметра PassInvalid = false и хотя бы один из элементов входной последовательности не может быть преобразован в строку, 
        /// генерируется исключение.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="Source"/>, <paramref name="Prefix"/>, <paramref name="Ending"/>, 
        /// <paramref name="Ending"/>, <paramref name="Divider"/>, <paramref name="LastDivider"/> являются null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="Source"/></exception>
        /// <typeparam name="T">Тип элементов входной строго типизированной последовательности</typeparam>
        /// <param name="Source">Входная последовательность, элементы которой следует конкатенировать</param>
        /// <param name="Prefix">Префикс - строка, которая размещается в самом начале выходной строки, перед первым элементом. 
        /// Если префикс не нужен, укажите пустую строку. Если NULL - будет выброшено исключение.</param>
        /// <param name="Ending">Окончание - строка, которая размещается в самом конце выходной строки, после последнего элемента. 
        /// Если окончание не нужно, укажите пустую строку. Если NULL - будет выброшено исключение.</param>
        /// <param name="Divider">Разделитель - строка, которая будет помещена между всеми элементами, за исключением последней пары. 
        /// Если разделитель не нужен (элементы должны конкатенироваться вплотную), укажите пустую строку. Если NULL - будет выброшено исключение.</param>
        /// <param name="LastDivider">Последний разделитель - строка, которая будет помещена между последними двумя элементыми. 
        /// Если отдельный от разделителя последний разделитель не нужен, укажите его значение равным разделителю. 
        /// Если же последняя пара элементов должна быть сцеплена вплотную, используйте пустую строку. Если NULL - будет выброшено исключение.</param>
        /// <param name="PassInvalid">Если true - при конвертации в строки входных элементов метод не выбросит исключение, 
        /// если какой-либо элемент не может быть конвертирован в строку, а пропустит этот элемент и перейдёт ко следующему.</param>
        /// <returns>Образовавшаяся строка, которая не может быть NULL</returns>
        public static String ConcatToString<T>(this IEnumerable<T> Source,
            String Prefix, String Ending, String Divider, String LastDivider, Boolean PassInvalid)
        {
            if (Source == null) { throw new ArgumentNullException("Source", "Последовательность является NULL"); }
            if (Source.Any<T>() == false) { throw new ArgumentOutOfRangeException("Source", "Последовательность не содержит ни единого элемента"); }
            if (Prefix == null) { throw new ArgumentNullException("Prefix"); }
            if (Ending == null) { throw new ArgumentNullException("Ending"); }
            if (Divider == null) { throw new ArgumentNullException("Divider"); }
            if (LastDivider == null) { throw new ArgumentNullException("LastDivider"); }

            List<String> parsed_source = new List<string>(Source.Count());

            Int32 counter = 0;
            foreach (T elem in Source)
            {
                try
                {
                    parsed_source.Add(elem.ToString());
                }
                catch (Exception ex)
                {
                    if (PassInvalid == false)
                    {
                        throw new ArgumentException("Невозможно преобразовать в строку элемент на позиции " + counter, "Source", ex);
                    }
                }
                counter = counter + 1;
            }
            Int32 parsed_count = parsed_source.Count;
            if (parsed_count == 1)
            {
                return String.Concat(Prefix, parsed_source.Single<String>(), Ending);
            }
            if (parsed_count == 2)
            {
                return String.Concat(Prefix, parsed_source.First<String>(), LastDivider, parsed_source.Last<String>(), Ending);
            }
            StringBuilder result = new StringBuilder(Prefix, parsed_count * 20);
            for (int i = 0; i < parsed_count; i++)
            {
                result.Append(parsed_source[i]);
                if (i == parsed_count - 1)
                {
                    result.Append(Ending);
                }
                else if (i == parsed_count - 2)
                {
                    result.Append(LastDivider);
                }
                else
                {
                    result.Append(Divider);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Конкатенирует все элементы входной последовательности в одну строку в прямом порядке, от начала и до конца. 
        /// Использует указанный префикс, окончание и разделитель. 
        /// Также может пропускать невалидные элементы и удалять копии.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Prefix"></param>
        /// <param name="Ending"></param>
        /// <param name="Divider">Разделитель - строка, которая будет помещена между всеми элементами. 
        /// Если разделитель не нужен (элементы должны конкатенироваться вплотную), укажите пустую строку. Если NULL - будет выброшено исключение.</param>
        /// <param name="PassInvalid"></param>
        /// <returns></returns>
        public static String ConcatToString<T>(this IEnumerable<T> Source,
            String Prefix, String Ending, String Divider, Boolean PassInvalid)
        {
            return Source.ConcatToString<T>(Prefix, Ending, Divider, Divider, PassInvalid);
        }

        /// <summary>
        /// Конкатенирует все элементы входной последовательности в одну строку в прямом порядке, от начала и до конца, 
        /// используя указанный разделитель, который будет помещен между элементами.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Divider">Разделитель. </param>
        /// <returns></returns>
        public static String ConcatToString<T>(this IEnumerable<T> Source, String Divider)
        {
            return Source.ConcatToString(String.Empty, String.Empty, Divider, Divider, false);
        }

        /// <summary>
        /// Конкатенирует все элементы входной последовательности в одну строку в прямом порядке, от начала и до конца, 
        /// используя запятую с пробелом (, ) в качестве разделителя, который будет помещен между элементами.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static String ConcatToString<T>(this IEnumerable<T> Source)
        {
            return Source.ConcatToString(", ");
        }
        #endregion

        #region Splitting
        /// <summary>
        /// Определяет, включать ли элемент-разделитель в выходные коллекции (последовательности) и если да, то куда
        /// </summary>
        public enum SplitterDisposition : byte
        {
            /// <summary>
            /// Не включать ни в какую выходную коллекцию
            /// </summary>
            Reject = 0,

            /// <summary>
            /// Включить в конец первой коллекции
            /// </summary>
            ToFirst = 1,

            /// <summary>
            /// Включить в начало второй коллекции
            /// </summary>
            ToLast = 2
        };

        /// <summary>
        /// Разделяет входной список на два подсписка по указанному индексу. Подсписки (первый и второй) возвращаются через выводные параметры. 
        /// Обобщённый метод, работающий с любыми коллекциями, которые реализуют интерфейс IList&lt;<typeparamref name="TItem"/>&gt; и имеющие конструктор без параметров.
        /// </summary>
        /// <typeparam name="TSeq">Тип входной строго типизированной коллекции, которая реализует интерфейс IList и имеет конструктор без параметров. 
        /// Должен быть указан явно, так как компилятор не может его вывести автоматически.</typeparam>
        /// <typeparam name="TItem">Тип элементов входной коллекции. Должен быть указан явно, так как компилятор не может его вывести автоматически.</typeparam>
        /// <param name="Source">Входной список, который требуется разделить по индексу. Если NULL или пустой, метод вернёт исключение. 
        /// При работе метода данный список никак не затрагивается, возвращаются новые экземпляры списков.</param>
        /// <param name="SplitterIndex">Индекс-разделитель входного списка, по которому требуется разделить входной список. 
        /// Если меньше 0 или больше фактической длины входного списка, метод вернёт исключение.</param>
        /// <param name="SplitterLocation">Указывает, включать ли элемент-разделитель входного словаря в конец первого или начало второго подсловаря, или вообще никуда не включать.</param>
        /// <param name="FirstPart">Выводной параметр - первый подсписок. 
        /// Если индекс-разделитель указывал на первый элемент входного списка (=0), который не включается в первый подсписок, то первый подсписок будет пустой.</param>
        /// <param name="LastPart">Выводной параметр - второй (последний) подсписок. 
        /// Если индекс-разделитель указывал на последнй элемент входного словаря и не включается в последний подсписок, то последний подсписок будет пустой.</param>
        public static void SplitList<TSeq, TItem>(this TSeq Source, Int32 SplitterIndex, CollectionTools.SplitterDisposition SplitterLocation,
            out TSeq FirstPart, out TSeq LastPart)
            where TSeq : IList<TItem>, new()
        {
            if (Source.IsNullOrEmpty() == true) { throw new ArgumentException("Словарь является NULL или не содержит элементов", "Source"); }
            if (Enum.IsDefined(typeof(CollectionTools.SplitterDisposition), (Byte)SplitterLocation) == false)
            { throw new InvalidEnumArgumentException("SplitterLocation", (Int32)SplitterLocation, typeof(CollectionTools.SplitterDisposition)); }
            if (SplitterIndex < 0) { throw new ArgumentOutOfRangeException("SplitterIndex", "Индекс не может быть меньше 0"); }
            Int32 source_count = Source.Count;
            if (SplitterIndex >= source_count) { throw new ArgumentOutOfRangeException("SplitterIndex", "Индекс больше или равен длине входной коллекции, которая состоит из " + source_count + " элементов"); }

            FirstPart = new TSeq();
            LastPart = new TSeq();

            Boolean to_first = true;
            for (Int32 i = 0; i < source_count; i++)
            {
                TItem elem = Source[i];
                if (i == SplitterIndex)
                {
                    to_first = false;
                    switch (SplitterLocation)
                    {
                        case SplitterDisposition.Reject:
                            break;
                        case SplitterDisposition.ToFirst:
                            FirstPart.Add(elem);
                            break;
                        case SplitterDisposition.ToLast:
                            LastPart.Add(elem);
                            break;
                    }
                }
                else if (i != SplitterIndex && to_first == true)
                {
                    FirstPart.Add(elem);
                }
                else if (i != SplitterIndex && to_first == false)
                {
                    LastPart.Add(elem);
                }
            }
        }

        /// <summary>
        /// Разделяет входной словарь на два подсловаря по указанному ключу. Подсловари (первый и второй) возвращаются через выводные параметры. 
        /// Обобщённый метод, работающий с любыми коллекциями, которые реализуют интерфейс IDictionary&lt;<typeparamref name="TKey"/>, <typeparamref name="TValue"/>&gt; и имеющие конструктор без параметров.
        /// </summary>
        /// <typeparam name="TSeq">Тип входной строго типизированной коллекции, которая реализует интерфейс IDictionary и имеет конструктор без параметров. 
        /// Должен быть указан явно, так как компилятор не может его вывести автоматически.</typeparam>
        /// <typeparam name="TKey">Тип ключа входной коллекции и одновременно тип ключа-разделителя. Должен поддерживать сравнение с самим собой через реализацию интерфейса 
        /// IEquatable&#60;TKey&#62;. Должен быть указан явно, так как компилятор не может его вывести автоматически.</typeparam>
        /// <typeparam name="TValue">Тип значения обрабатываемого словаря, неограниченный параметр. Должен быть указан явно, так как компилятор не может его вывести автоматически.</typeparam>
        /// <param name="Source">Входной словарь, который требуется разделить по ключу-разделителю. Если NULL или пустой, метод вернёт исключение. 
        /// При работе метода данный словарь никак не затрагивается, возвращаются новые экземпляры словарей.</param>
        /// <param name="SplitterKey">Ключ-разделитель входного словаря, чей тип должен быть равен типу ключа входного словаря. Если отсутсвует во входном словаре, метод вернёт исключение.</param>
        /// <param name="SplitterLocation">Указывает, включать ли элемент-разделитель входного словаря в конец первого или начало второго подсловаря, или вообще никуда не включать.</param>
        /// <param name="FirstPart">Выводной параметр - первый подсловарь. Если ключ-разделитель был первым элементом входного словаря и не включается в первый подсловарь, то он будет пустой.</param>
        /// <param name="LastPart">Выводной параметр - второй (последний) подсловарь. Если ключ-разделитель был последним элементом входного словаря и не включается в последний подсловарь, то он будет пустой.</param>
        public static void SplitDictionary<TSeq, TKey, TValue>(this TSeq Source, TKey SplitterKey, CollectionTools.SplitterDisposition SplitterLocation,
            out TSeq FirstPart, out TSeq LastPart)
            where TSeq : IDictionary<TKey, TValue>, new()
            where TKey : IEquatable<TKey>
        {
            if (Source.IsNullOrEmpty() == true) { throw new ArgumentException("Словарь является NULL или не содержит элементов", "Source"); }
            if (Source.ContainsKey(SplitterKey) == false) { throw new ArgumentException("Ключ-разделитель не содержится во входном словаре", "SplitterKey"); }
            if (Enum.IsDefined(typeof(CollectionTools.SplitterDisposition), (Byte)SplitterLocation) == false)
            { throw new InvalidEnumArgumentException("SplitterLocation", (Int32)SplitterLocation, typeof(CollectionTools.SplitterDisposition)); }

            FirstPart = new TSeq();
            LastPart = new TSeq();

            Boolean to_first = true;
            foreach (KeyValuePair<TKey, TValue> elem in Source)
            {
                if (elem.Key.Equals(SplitterKey) == true)
                {
                    to_first = false;
                    switch (SplitterLocation)
                    {
                        case SplitterDisposition.Reject:
                            break;
                        case SplitterDisposition.ToFirst:
                            FirstPart.Add(elem.Key, elem.Value);
                            break;
                        case SplitterDisposition.ToLast:
                            LastPart.Add(elem.Key, elem.Value);
                            break;
                    }
                }
                else if (elem.Key.Equals(SplitterKey) == false && to_first == true)
                {
                    FirstPart.Add(elem.Key, elem.Value);
                }
                else if (elem.Key.Equals(SplitterKey) == false && to_first == false)
                {
                    LastPart.Add(elem.Key, elem.Value);
                }
            }
        }
        #endregion Splitting

        #region Merging

        /// <summary>
        /// Возвращает новосозданный одномерный массив-вектор, содержащий все элементы из всех указанных массивов
        /// </summary>
        /// <typeparam name="TItem">Тип элементов массивов, без ограничений</typeparam>
        /// <param name="Arrays">Массив массивов, все из которых следует конкатенировать в новый одномерный массив</param>
        /// <returns>Новый одномерный массив-вектор (индексируемый с 0), размер (длина) которого точно соответствует количеству расположенных в нём элементов</returns>
        public static TItem[] ConcatArrays<TItem>(params TItem[][] Arrays)
        {
            if (Arrays == null) { return null; }
            if (Arrays.Length == 0) { return new TItem[0] { }; }
            Int32 total_len = 0;
            for (Int32 i = 0; i < Arrays.Length; i++)
            {
                total_len = total_len + (Arrays[i] == null ? 0 : Arrays[i].Length);
            }
            TItem[] output = new TItem[total_len];
            Int32 output_index = 0;
            for (Int32 i = 0; i < Arrays.Length; i++)
            {
                TItem[] temp = Arrays[i];
                if (temp == null) { continue; }
                for (Int32 j = 0; j < temp.Length; j++)
                {
                    output[output_index] = temp[j];
                    ++output_index;
                }
            }
            return output;
        }

        /// <summary>
        /// Выполняет слияние двух указанных словарей с одинаковыми типами ключей и значениий в новый, третий словарь. 
        /// Принимает делегат на метод, который будет вызван для разрешения возможных конфликтов при дубликации ключей в первом и втором входном словаре.
        /// </summary>
        /// <typeparam name="TKey">Тип ключа, который должен быть общим в первом и втором входных словарях</typeparam>
        /// <typeparam name="TValue">Тип значения, который должен быть общим в первом и втором входных словарях</typeparam>
        /// <param name="First">Первый входной словарь. Если NULL или пустой, будет возвращена копия второго словаря.</param>
        /// <param name="Second">Второй входной словарь. Если NULL или пустой, будет возвращена копия первого словаря.</param>
        /// <param name="MultipleKeysResolverFunc">Делегат на метод, которая будет вызвана в том случае, если в процессе объединения будут обнаружены 
        /// дублирующиеся ключи в первом и втором словаре. Метод будет вызван столько раз, сколько раз будут встречаться дубликаты. 
        /// Метод должен принимать 3 параметры: ключ (который присутствует в обих словарях), значение ключа в первом и значение ключа во втором словаре; 
        /// и должен возвращать значение, которое будет помещено в выходной словарь (или же выбрасывать исключение, которое не будет обработано). 
        /// Если делегат является NULL, метод будет помещать в выходной словарь значение из второго словаря.
        /// </param>
        /// <returns>Новый словарь, который объединяет элементы первого и второго входных словарей.</returns>
        public static Dictionary<TKey, TValue> ConcatDictionaries<TKey, TValue>
            (IDictionary<TKey, TValue> First, IDictionary<TKey, TValue> Second, Func<TKey, TValue, TValue, TValue> MultipleKeysResolverFunc)
        {
            if(First.IsNullOrEmpty() && Second.IsNullOrEmpty()) {return new Dictionary<TKey, TValue>(0);}
            if (First.IsNullOrEmpty())
            {
                return new Dictionary<TKey, TValue>(Second);
            }
            if (Second.IsNullOrEmpty())
            {
                return new Dictionary<TKey, TValue>(First);
            }
            Dictionary<TKey, TValue> output = new Dictionary<TKey, TValue>(Math.Max(First.Count, Second.Count));
            CollectionTools.CopyAll(First, output, false);
            foreach (KeyValuePair<TKey, TValue> oneKVP in Second)
            {
                if (output.ContainsKey(oneKVP.Key))
                {
                    output[oneKVP.Key] = MultipleKeysResolverFunc != null ? MultipleKeysResolverFunc.Invoke(oneKVP.Key, output[oneKVP.Key], oneKVP.Value) : oneKVP.Value;
                }
                else
                {
                    output.Add(oneKVP.Key, oneKVP.Value);
                }
            }
            return output;
        }

        #endregion Merging

        #region Copying
        /// <summary>
        /// Копирует все элементы из входного словаря в указанный выходной словарь
        /// </summary>
        /// <typeparam name="TKey">Тип ключа, который должен быть общим во входном и выходном словарях</typeparam>
        /// <typeparam name="TValue">Тип значения, который должен быть общим во входном и выходном словарях</typeparam>
        /// <param name="Source">Входной словарь, из которого будут скопированы все элементы в выходной. 
        /// Если NULL или пустой, не будет предпринято никаких действий.</param>
        /// <param name="Target">Выходной словарь, в который будут скопированы все элементы из входного. Если NULL, будет выброшено исключение.</param>
        /// <param name="ReplaceValueInTarget">Определяет, как следует поступать, если во входном и выходном словаре будут обнаружены идентичные ключи. 
        /// 'true' - оригинальное значение в выходном словаре будет заменено новым значением из входного; 
        /// 'false' - в выходном словаре будет оставлено оригинальное значение.</param>
        public static void CopyAll<TKey, TValue>(this IDictionary<TKey, TValue> Source, IDictionary<TKey, TValue> Target, Boolean ReplaceValueInTarget)
        {
            if(Source.IsNullOrEmpty()){return;}
            if(Target.IsNull() == true) {throw new ArgumentNullException("Target");}
            foreach (KeyValuePair<TKey, TValue> oneKVP in Source)
            {
                Boolean contains_key = Target.ContainsKey(oneKVP.Key);
                if (contains_key == false)
                {
                    Target.Add(oneKVP);
                }
                else if (ReplaceValueInTarget == true)
                {
                    Target[oneKVP.Key] = oneKVP.Value;
                }
                else
                {
                    continue;
                }
            }
        }

        #endregion Copying

        /// <summary>
        /// Конвертирует последовательность пар "ключ-значение" в словарь, заполненный этими парами. 
        /// Данный метод помогает обрабатывать результаты LINQ-методов, которые после применения на словарях 
        /// возвращают непроиндексированную последовательность пар "ключ-значение", а не словарь.
        /// </summary>
        /// <typeparam name="TKey">Тип ключа - должен поддерживать сравнение сам с собой</typeparam>
        /// <typeparam name="TValue">Тип значения - неограниченный тип</typeparam>
        /// <param name="Source">Входная последовательность</param>
        /// <returns>Если во входной последовательности содержатся более одной пары с одинаковым ключем, в выходной словарь будет записана последняя такая пара</returns>
        public static Dictionary<TKey, TValue> ConvertToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> Source) where TKey : IEquatable<TKey>
        {
            if (Source == null) { throw new ArgumentNullException("Source"); }

            Dictionary<TKey, TValue> output = new Dictionary<TKey, TValue>(Source.Count());
            foreach (KeyValuePair<TKey, TValue> elem in Source)
            {
                output[elem.Key] = elem.Value;
            }
            return output;
        }

        /// <summary>
        /// Возвращает элементы указанного словаря в обратном порядке, создавая новый словарь и не изменяя старый
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Source">Входной словарь, элементы которого требуется обратить</param>
        /// <returns>Копия входного словаря, в которой все элементы находятся в обратном порядке</returns>
        public static Dictionary<TKey, TValue> ReverseDictionary<TKey, TValue>(this IDictionary<TKey, TValue> Source)
        {
            Dictionary<TKey, TValue> output = new Dictionary<TKey, TValue>(Source.Count);
            IEnumerable<KeyValuePair<TKey, TValue>> temp = Source.Reverse();
            foreach (KeyValuePair<TKey, TValue> elem in temp)
            {
                output.Add(elem.Key, elem.Value);
            }
            return output;
        }

        /// <summary>
        /// Возвращает строковое представление пары "ключ-значение" согласно указанным параметрам
        /// </summary>
        /// <typeparam name="TKey">Тип ключа - без ограничений</typeparam>
        /// <typeparam name="TValue">Тип значения - без ограничений</typeparam>
        /// <param name="KVP">Пара "ключ-значение"</param>
        /// <param name="Divider">Разделитель - помещается между ключем и значением, если не является NULL или пустой строкой</param>
        /// <param name="DirectOrder">Порядок конкатенации ключа и значения. 
        /// Если true - выводится сначала ключ, а потом значение; если false - сначала значение, а потом ключ</param>
        /// <returns></returns>
        public static String ToString<TKey, TValue>(this KeyValuePair<TKey, TValue> KVP, String Divider, Boolean DirectOrder)
        {
            const String empty = "";
            if (Divider == null) { Divider = empty; }
            if ((typeof(TKey).IsValueType == false && KVP.Key == null) && (typeof(TValue).IsValueType == false && KVP.Value == null)) { return empty; }
            if (KVP.Key == null) { return KVP.Value.ToString(); }
            if (KVP.Value == null) { return KVP.Key.ToString(); }
            String key = KVP.Key.ToString();
            String val = KVP.Value.ToString();
            if (key.IsStringNullOrEmpty() == true && val.IsStringNullOrEmpty() == true) { return String.Empty; }
            if (key.IsStringNullOrEmpty() == true) { return val; }
            if (val.IsStringNullOrEmpty() == true) { return key; }
            if (DirectOrder == true) { return key + Divider + val; }
            else { return val + Divider + key; }
        }

        #region Duplicated
        /// <summary>
        /// Внутренний метод, предназначенный для поиска дубликатов с указанным компаратором. Применяется, если входная последовательность имеет незначительный размер. 
        /// Неэффективен по времени и нагрузке на CPU, но эффективен в плане использования памяти.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="Source"></param>
        /// <param name="ItemsComparer"></param>
        /// <returns></returns>
        private static Boolean LowMemoryDuplicatedItemsDetector<TItem>(IList<TItem> Source, IEqualityComparer<TItem> ItemsComparer)
        {
            Int32 count = Source.Count;
            for (Int32 first_pairwise_comparison = 0; first_pairwise_comparison < count; first_pairwise_comparison++)
            {
                for (Int32 second_pairwise_comparison = first_pairwise_comparison + 1; second_pairwise_comparison < count; second_pairwise_comparison++)
                {
                    if (ItemsComparer.Equals(Source[first_pairwise_comparison], Source[second_pairwise_comparison]) == true) { return true; }
                }
            }
            return false;
        }

        private const Int32 _MaxSize = 100;

        /// <summary>
        /// Определяет, имеет ли входной список повторяющиеся (одинаковые) элементы
        /// </summary>
        /// <typeparam name="TItem">Тип элементов списка - должен поддерживать сравнение с самим собой, реализуя интерфейс IEquatable&#60;T&#62;</typeparam>
        /// <param name="Source">Входная последовательность, которая должна реализовывать доступ к её элементам по индексу</param>
        /// <returns>Если true - имеется минимум 2 одинаковых элемента в списке; если false - все элементы уникальны</returns>
        public static Boolean HasDuplicatedItems<TItem>(this IList<TItem> Source) where TItem : IEquatable<TItem>
        {
            if (Source.IsNullOrEmpty() == true || Source.HasSingle() == true) { return false; }
            if (Source.Count < CollectionTools._MaxSize)
            { return CollectionTools.LowMemoryDuplicatedItemsDetector(Source, EqualityComparer<TItem>.Default); }
            Dictionary<Int32, TItem> temp = new Dictionary<int, TItem>(Source.Count);
            temp.Add(Source[0].GetHashCode(), Source[0]);
            Int32 temp_hash;
            TItem temp_elem;
            for (Int32 i = 1; i < Source.Count; i++)
            {
                temp_elem = Source[i];
                temp_hash = temp_elem.GetHashCode();
                if (temp.ContainsKey(temp_hash) == true)
                {
                    if (temp[temp_hash].Equals(temp_elem) == true)
                    { return true; }
                    temp[temp_hash] = temp_elem;
                }
                else
                {
                    temp.Add(temp_hash, Source[i]);
                }
            }
            return false;
        }

        /// <summary>
        /// Определяет, имеет ли входной список повторяющиеся (одинаковые) элементы, используя указанный компаратор
        /// </summary>
        /// <typeparam name="TItem">Тип элементов списка - без ограничений</typeparam>
        /// <param name="Source">Входная последовательность, которая должна реализовывать доступ к её элементам по индексу</param>
        /// <param name="ItemsComparer">Компаратор сравнения на равенство для типа <typeparamref name="TItem"/>. Если null - будет использован компаратор по умолчанию для этого типа.</param>
        /// <returns>Если true - имеется минимум 2 одинаковых элемента в списке; если false - все элементы уникальны</returns>
        public static Boolean HasDuplicatedItems<TItem>(this IList<TItem> Source, IEqualityComparer<TItem> ItemsComparer)
        {
            if (Source.IsNullOrEmpty() == true || Source.HasSingle() == true) { return false; }
            if (ItemsComparer.IsNull() == true) { ItemsComparer = EqualityComparer<TItem>.Default; }
            if (Source.Count < CollectionTools._MaxSize)
            { return CollectionTools.LowMemoryDuplicatedItemsDetector(Source, ItemsComparer); }
            Dictionary<Int32, TItem> temp = new Dictionary<int, TItem>(Source.Count);
            temp.Add(Source[0].GetHashCode(), Source[0]);
            Int32 temp_hash;
            TItem temp_elem;
            for (Int32 i = 1; i < Source.Count; i++)
            {
                temp_elem = Source[i];
                temp_hash = temp_elem.GetHashCode();
                if (temp.ContainsKey(temp_hash) == true)
                {
                    if (ItemsComparer.Equals(temp[temp_hash], temp_elem) == true)
                    { return true; }
                    temp[temp_hash] = temp_elem;
                }
                else
                {
                    temp.Add(temp_hash, Source[i]);
                }
            }
            return false;
        }

        /// <summary>
        /// Возвращает различающиеся (уникальные) элементы списка вместе с количеством их появлений, используя указанный компаратор
        /// </summary>
        /// <typeparam name="TItem">Тип элемента входного списка, должен поддерживать сравнение со своим типом 
        /// через реализацию интерфейса IEquatable&#60;TItem&#62;</typeparam>
        /// <param name="Source">Входной список. Если NULL - будет выброшено исключение.</param>
        /// <param name="EqualityComp">Компаратор проверки на равенство элементов типа <typeparamref name="TItem"/>. Если NULL - будет выброшено исключение.</param>
        /// <returns></returns>
        public static Dictionary<TItem, Int32> DistinctWithCount<TItem>(this IList<TItem> Source, IEqualityComparer<TItem> EqualityComp)
            where TItem : IEquatable<TItem>
        {
            if(Source.IsNull()==true) {throw new ArgumentNullException("Source");}
            if(EqualityComp.IsNull() == true) {throw new ArgumentNullException("EqualityComp");}
            Dictionary<TItem, Int32> output = new Dictionary<TItem, int>(Source.Count, EqualityComp);
            if (Source.Count == 0) {return output;}
            output.Add(Source[0], 1);
            for (Int32 i = 1; i < Source.Count; i++)
            {
                TItem current = Source[i];
                if (output.ContainsKey(current) == true)
                {
                    output[current] = output[current] + 1;
                }
                else
                {
                    output.Add(current, 1);
                }
            }
            return output;
        }

        /// <summary>
        /// Возвращает различающиеся (уникальные) элементы списка вместе с количеством их появлений, используя компаратор по умолчанию для типа элементов списка
        /// </summary>
        /// <typeparam name="TItem">Тип элемента входного списка, должен поддерживать сравнение со своим типом 
        /// через реализацию интерфейса IEquatable&#60;TItem&#62;</typeparam>
        /// <param name="Source">Входной список. Если NULL - будет выброшено исключение.</param>
        /// <returns></returns>
        public static Dictionary<TItem, Int32> DistinctWithCount<TItem>(this IList<TItem> Source)
            where TItem : IEquatable<TItem>
        {
            return DistinctWithCount(Source, EqualityComparer<TItem>.Default);
        }
        #endregion

        /// <summary>
        /// Меняет местами между собой значения указанных двух ключей в указанном словаре, заменяя значением первого указанного ключа значение второго и наоборот. 
        /// Записывает изменения в этот же экземпляр, после чего возвращает на него ссылку.
        /// </summary>
        /// <typeparam name="TKey">Тип ключа обрабатываемого словаря, который должен поддерживать сравнение со своим типом 
        /// через реализацию интерфейса IEquatable&#60;TKey&#62;</typeparam>
        /// <typeparam name="TValue">Тип значения - без ограничений</typeparam>
        /// <param name="Source">Исходный словарь. Если NULL, пустой или содержит один элемент - выбрасывается исключение.</param>
        /// <param name="First">Ключ первого элемента. Если равен ключу второго или не содержится во входном словаре - выбрасывается исключение.</param>
        /// <param name="Next">Ключ второго элемента. Если равен ключу первого или не содержится во входном словаре - выбрасывается исключение.</param>
        public static IDictionary<TKey, TValue> SwapValues<TKey, TValue>(this IDictionary<TKey, TValue> Source, TKey First, TKey Next)
             where TKey : IEquatable<TKey>
        {
            if (Source == null) { throw new ArgumentNullException("Source", "Входной словарь не может быть NULL"); }
            if (Source.Any() == false) { throw new ArgumentException("Входной словарь не может быть пустым", "Source"); }
            if (Source.Count == 1) { throw new ArgumentException("Входной словарь должен иметь более одного элемента", "Source"); }
            if (First.Equals(Next) == true) { throw new ArgumentException("Указанные первый и второй ключи должны быть разными"); }
            if (Source.ContainsKey(First) == false) { throw new KeyNotFoundException("Ключ 'First'=" + First.ToStringS() + " отсутствует во входном словаре"); }
            if (Source.ContainsKey(Next) == false) { throw new KeyNotFoundException("Ключ 'Next'=" + Next.ToStringS() + " отсутствует во входном словаре"); }
            TValue first_vale = Source[First];
            Source[First] = Source[Next];
            Source[Next] = first_vale;
            return Source;
        }

        /// <summary>
        /// Меняет местами между собой элементы, расположенные по двум указанным индексам в указанном списке. 
        /// Записывает изменения в этот же экземпляр, после чего возвращает на него ссылку.
        /// </summary>
        /// <typeparam name="TItem">Тип элемента обрабатываемого списка - без ограничений</typeparam>
        /// <param name="Source">Исходной список. Если NULL, пустой или содержит один элемент - генерируется исключение.</param>
        /// <param name="FirstIndex">Индекс первого элемента. Если меньше 0, больше размера списка или равен второму индексу - генерируется исключение.</param>
        /// <param name="SecondIndex">Индекс второго элемента. Если меньше 0, больше размера списка или равен первому индексу - генерируется исключение.</param>
        public static IList<TItem> SwapItems<TItem>(this IList<TItem> Source, Int32 FirstIndex, Int32 SecondIndex)
        {
            Source.ThrowIfNullOrEmpty();
            if (Source.HasSingle() == true) { throw new ArgumentException("Входной список должен иметь более одного элемента", "Source"); }
            if (FirstIndex < 0) { throw new ArgumentOutOfRangeException("FirstIndex", "Первый индекс не может быть меньше '0', тогда как есть " + FirstIndex); }
            if (SecondIndex < 0) { throw new ArgumentOutOfRangeException("SecondIndex", "Второй индекс не может быть меньше '0', тогда как есть " + SecondIndex); }
            Int32 count = Source.Count;
            if (FirstIndex > count - 1) { throw new ArgumentOutOfRangeException("FirstIndex", "Первый индекс не может быть больше количества элементав списка, которое равно " + count); }
            if (SecondIndex > count - 1) { throw new ArgumentOutOfRangeException("SecondIndex", "Второй индекс не может быть больше количества элементав списка, которое равно " + count); }
            if (FirstIndex == SecondIndex) { throw new ArgumentException("Первый индекс равен второму (" + FirstIndex + ")"); }
            TItem first_temp = Source[FirstIndex];
            Source[FirstIndex] = Source[SecondIndex];
            Source[SecondIndex] = first_temp;
            return Source;
        }

        #region Equatable
        /// <summary>
        /// Сравнивает два словаря с идентичными типами ключей и значений на наличие идентичных пар с идентичными ключами и значениями. 
        /// При этом порядок вхождения элементов в словари игнорируется.
        /// </summary>
        /// <remarks>
        /// 1. Если оба словаря NULL - true
        /// 2. Если один NULL, а другой нет - false
        /// 3. Если ссылки идентичны - true
        /// 4. Если количество элементов неравное - false
        /// Далее идёт проход в цикле по элементам первого словаря. На каждой итерации выбирается одна пара ключа и значения. 
        /// 1. Если ключ этой пары не найден в другом словаре - false
        /// 2. Если ключ найден, но значение ключа не равно значению такового из первого - false
        /// По окончании итерирования возвращает true.
        /// </remarks>
        /// <typeparam name="TKey">Тип ключа словаря, который должен поддерживать сравнение со своим типом 
        /// через реализацию интерфейса IEquatable&#60;TKey&#62;</typeparam>
        /// <typeparam name="TValue">Тип значения словаря, который должен поддерживать сравнение со своим типом 
        /// через реализацию интерфейса IEquatable&#60;TKey&#62;</typeparam>
        /// <param name="Source">Входной словарь</param>
        /// <param name="Comparable">Другой словарь, который сравнивается с исходным</param>
        /// <returns>Если 'true' - словари равны, если 'false' - словари не равны</returns>
        public static Boolean EqualsIgnoreOrder<TKey, TValue>(this IDictionary<TKey, TValue> Source, IDictionary<TKey, TValue> Comparable)
            where TKey : IEquatable<TKey>
            where TValue : IEquatable<TValue>
        {
            Nullable<Boolean> shallow_equals_result = CollectionTools.ShallowEqualsEnumerables(Source, Comparable);
            if (shallow_equals_result.HasValue == true) { return shallow_equals_result.Value; }

            foreach (KeyValuePair<TKey, TValue> one_elem in Source)
            {
                TValue other_value;
                if (Comparable.TryGetValue(one_elem.Key, out other_value) == false)
                {
                    return false;
                }
                if (one_elem.Value.Equals(other_value) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Проверяет два списка с идентичными типами элементов на равенство. 
        /// Игнорирует порядок вхождения элементов списков и повторяемость одинаковых элементов, 
        /// однако для выполнения условия равенства требует, чтобы в обоих списках общее количество элементов было одинаковым.
        /// </summary>
        /// <remarks>
        /// 1. Если оба списка NULL - true
        /// 2. Если один NULL, а другой нет - false
        /// 3. Если оба пусты - true
        /// 4. Если ссылки идентичны - true
        /// 5. Если количество элементов неравное - false.
        /// Далее проходится по каждому элементу первого списка и ищёт его во втором. Если не находит - false. 
        /// Аналогично делается проход по второму с поиском элементов в первому.</remarks>
        /// <typeparam name="TItem">Тип элемента списков, который должен поддерживать сравнение со своим типом 
        /// через реализацию интерфейса IEquatable&#60;TKey&#62;</typeparam>
        /// <param name="Source">Входной список</param>
        /// <param name="Comparable">Другой список, который сравнивается с исходным</param>
        /// <returns></returns>
        public static Boolean EqualsIgnoreOrder<TItem>(this IList<TItem> Source, IList<TItem> Comparable)
            where TItem : IEquatable<TItem>
        {
            Nullable<Boolean> shallow_equals_result = CollectionTools.ShallowEqualsEnumerables(Source, Comparable);
            if (shallow_equals_result.HasValue == true) { return shallow_equals_result.Value; }

            foreach (TItem element in Source)
            {
                if (element.IsIn(Comparable) == false) { return false; }
            }
            foreach (TItem element in Comparable)
            {
                if (element.IsIn(Source) == false) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Выполняет точное сравнение двух словарей с идентичными типами ключей и значений, 
        /// проверяя как наличие и равенство соответствующих ключей и значений, так и их позиции. 
        /// </summary>
        /// <typeparam name="TKey">Тип ключа словаря, который должен поддерживать сравнение со своим типом 
        /// через реализацию интерфейса IEquatable&#60;TKey&#62;</typeparam>
        /// <typeparam name="TValue">Тип значения словаря, который должен поддерживать сравнение со своим типом 
        /// через реализацию интерфейса IEquatable&#60;TKey&#62;</typeparam>
        /// <param name="source">Исходный словарь</param>
        /// <param name="Comparable">Другой словарь, который сравнивается с исходным</param>
        /// <returns>Если 'true' - словари равны, если 'false' - словари не равны</returns>
        public static Boolean EqualsExact<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> Comparable)
            where TKey : IEquatable<TKey>
            where TValue : IEquatable<TValue>
        {
            Nullable<Boolean> shallow_equals_result = CollectionTools.ShallowEqualsEnumerables(source, Comparable);
            if (shallow_equals_result.HasValue == true) { return shallow_equals_result.Value; }

            for (int i = 0; i < source.Count; i++)
            {
                KeyValuePair<TKey, TValue> pair1 = source.ElementAt(i);
                KeyValuePair<TKey, TValue> pair2 = Comparable.ElementAt(i);
                if (pair1.Key.Equals(pair2.Key) == false)
                {
                    return false;
                }
                if (pair1.Value.Equals(pair2.Value) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Выполняет точное сравнение двух списков, анализируя проверяя наличие, равенство, количество и позицию каждого элемента одного списка во втором.
        /// </summary>
        /// <typeparam name="TItem">>Тип элемента списков, который должен поддерживать сравнение со своим типом 
        /// через реализацию интерфейса IEquatable&#60;TKey&#62;</typeparam>
        /// <param name="source">Исходный список</param>
        /// <param name="Comparable">Другой список, который сравнивается с исходным</param>
        /// <returns></returns>
        public static Boolean EqualsExact<TItem>(this IList<TItem> source, IList<TItem> Comparable)
            where TItem : IEquatable<TItem>
        {
            Nullable<Boolean> shallow_equals_result = CollectionTools.ShallowEqualsEnumerables(source, Comparable);
            if (shallow_equals_result.HasValue == true) { return shallow_equals_result.Value; }

            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].Equals(Comparable[i]) == false) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Выполняет 'поверхностное' сравнение двух generic-последовательностей любого типа коллекции с любым, но одинаковым для обоих коллекций типом элементов, 
        /// и возвращает результат равенства в виде трита: равно, не равно, равенство неизвестно. Условия равенства, неравенства и неизвестности: 
        /// 1. Если обе последовательности NULL - true; 
        /// 2. Если одна NULL, а другая нет - false; 
        /// 3. Если обе пусты - true; 
        /// 4. Если одна пуста, а другая нет - false; 
        /// 5. Если ссылки идентичны - true; 
        /// 6. Если количество элементов неравное - false. 
        /// Если ни одно из условий не исполнилось, возвращает NULL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Nullable<Boolean> ShallowEqualsEnumerables<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            if (Object.ReferenceEquals(first, second) == true) { return true; }
            if (Object.ReferenceEquals(first, null) == true && Object.ReferenceEquals(second, null) == true) { return true; }
            if (Object.ReferenceEquals(first, null) == true ^ Object.ReferenceEquals(second, null) == true) { return false; }
            if (first.IsEmpty() == true && second.IsEmpty() == true) { return true; }
            if (first.IsEmpty() == true ^ second.IsEmpty() == true) { return false; }
            if (first.Count() != second.Count()) { return false; }
            return null;
        }
        #endregion Equatable

        /// <summary>
        /// Сортирует входной словарь по значению элементов в указанном порядке, используя для значений компаратор по умолчанию и возвращает новый словарь с отсортированными элементами первого
        /// </summary>
        /// <typeparam name="TKey">Тип ключа словаря, который должен поддерживать проверку равенства со своим типом 
        /// через реализацию интерфейса IEquatable&#60;TKey&#62;</typeparam>
        /// <typeparam name="TValue">Тип значения словаря, который должен поддерживать сравнение со своим типом 
        /// через реализацию интерфейса IComparable&#60;TValue&#62;</typeparam>
        /// <param name="source"></param>
        /// <param name="Order">Порядок сравнения. Если 'true' - от меньших к большим, если 'false' - от больших к меньшим</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> SortByValue<TKey, TValue>(this IDictionary<TKey, TValue> source, Boolean Order)
            where TKey : IEquatable<TKey>
            where TValue : IComparable<TValue>
        {
            if (source == null) { throw new ArgumentNullException("source", "Невозможно отсортировать по значению словарь, который является NULL"); }
            if (source.Any() == false) { throw new ArgumentException("Невозможно отсортировать по значению словарь, который не содержит ни одного элемента", "source"); }

            if (source.HasSingle() == true) { return new Dictionary<TKey, TValue>(source); }

            Dictionary<TKey, TValue> output;
            if (Order == true)
            {
                output = (from entry in source orderby entry.Value ascending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            else
            {
                output = (from entry in source orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            return output;
        }

        /// <summary>
        /// Выбирает одинаковую часть из начала обеих списков, если она в них присутствует. Если хотя бы один из списков NULL или пустой, выбрасывается исключение.
        /// </summary>
        /// <typeparam name="TItem">Тип элемента словарей, который должен поддерживать сравнение с самим собой 
        /// через реализацию интерфейса IEquatable&#60;TItem&#62;</typeparam>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns>Новый список</returns>
        public static List<TItem> GetIdenticalStart<TItem>(IList<TItem> First, IList<TItem> Second)
            where TItem : IEquatable<TItem>
        {
            if (First.IsNullOrEmpty() == true) { throw new ArgumentException("Первый входной словарь не может быть NULL или пустым", "First"); }
            if (Second.IsNullOrEmpty() == true) { throw new ArgumentException("Второй входной словарь не может быть NULL или пустым", "Second"); }

            Int32 min_count = Math.Min(First.Count, Second.Count);
            List<TItem> output = new List<TItem>(min_count);
            for (int i = 0; i < min_count; i++)
            {
                if (First[i].Equals(Second[i]) == true)
                {
                    output.Add(First[i]);
                }
                else
                {
                    break;
                }
            }
            output.TrimExcess();
            return output;
        }

        /// <summary>
        /// Выбирает одинаковую часть из конца списков, если она в них присутствует. Если хотя бы один из списков NULL или пустой, выбрасывается исключение. 
        /// Одинаковая часть возвращается в обратном порядке, то есть с конца к началу. Если списки не имеют одинаковой части в конце, возвращается пустой список.
        /// </summary>
        /// <typeparam name="TItem">Тип элемента словарей, который должен поддерживать сравнение с самим собой 
        /// через реализацию интерфейса IEquatable&#60;TItem&#62;</typeparam>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns>Новый список</returns>
        public static List<TItem> GetIdenticalEnd<TItem>(IList<TItem> First, IList<TItem> Second)
            where TItem : IEquatable<TItem>
        {
            if (First.IsNullOrEmpty() == true) { throw new ArgumentException("Первый входной словарь не может быть NULL или пустым", "First"); }
            if (Second.IsNullOrEmpty() == true) { throw new ArgumentException("Второй входной словарь не может быть NULL или пустым", "Second"); }

            if (First.HasSingle() == true && Second.HasSingle() == true)
            {
                if (First.Single().Equals(Second.Single()) == true)
                {
                    return new List<TItem>(1) { First.Single() };
                }
                return new List<TItem>();
            }

            Int32 first_count = First.Count;
            Int32 second_count = Second.Count;

            Int32 min_count = Math.Min(first_count, second_count);
            Int32 max_count = Math.Max(first_count, second_count);
            Int32 difference = max_count - min_count;

            List<TItem> output;

            if (first_count == second_count)
            {
                output = new List<TItem>(second_count);

                for (int i = second_count - 1; i >= 0; i--)
                {
                    if (First[i].Equals(Second[i]) == true)
                    {
                        output.Add(First[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (first_count > second_count)
            {
                output = new List<TItem>(second_count);

                for (int i = first_count - 1, j = first_count - difference - 1; i >= 0 && j >= 0; i--, j--)
                {
                    if (First[i].Equals(Second[j]) == true)
                    {
                        output.Add(First[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else//first_count < second_count
            {
                output = new List<TItem>(first_count);

                for (int i = second_count - 1, j = second_count - difference - 1; i >= 0 && j >= 0; i--, j--)
                {
                    if (Second[i].Equals(First[j]) == true)
                    {
                        output.Add(Second[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            output.TrimExcess();
            return output;
        }

        /// <summary>
        /// Возвращает все элементы входного словаря, ключи которых находятся в указанном списке ключей
        /// </summary>
        /// <typeparam name="TKey">Тип ключа входного словаря и тип списка ключей - 
        /// должен поддерживать сравнение с самим собой путём реализации интерфейса IEquatable</typeparam>
        /// <typeparam name="TValue">Тип значения элемента входного словаря - неограниченный тип</typeparam>
        /// <param name="Source">Исходный словарь, из которого необходимо возвратить подсловарь, элементы которого содержат указанные ключи.</param>
        /// <param name="Keys">Список ключей, которые присутствуют во входном списке и значения с которыми из него требуется возвратить. 
        /// Дублирующиеся значения, если есть, будут проигнорированы.</param>
        /// <param name="PassNotFoundKeys">Определяет, выбрасывать ли исключение, если хотя бы один из указанных ключей отсутствует во входном словаре. 
        /// Если true - все указанные ключи, отсутствующие в словаре, будут опущены, и будут возвращены те элементы, которые имеют ключи; 
        /// если false - при первом несовпадающем ключе будет выброшено KeyNotFoundException</param>
        /// <returns>Новый словарь, который содержит все элементы входного словаря, чьи ключи совпали с теми, что были переданы методу. 
        /// Элементы в этом словаре расположены в том порядке, в котором переданы ключи. 
        /// Если ни один из переданных ключей не содержится в исходном словаре и параметр PassNotFoundKeys выставлен в true, метод возвратит пустой словарь.</returns>
        public static Dictionary<TKey, TValue> GetPortionByKeys<TKey, TValue>(this IDictionary<TKey, TValue> Source, Boolean PassNotFoundKeys, IEnumerable<TKey> Keys)
            where TKey : IEquatable<TKey>
        {
            if (Source == null) { throw new ArgumentNullException("Source"); }
            if (Source.Any() == false) { throw new ArgumentException("Входная последовательность не может быть пустой", "Source"); }

            if (Keys == null) { throw new ArgumentNullException("Keys"); }
            if (Keys.Any() == false) { throw new ArgumentException("Список ключей не может быть пустой", "Keys"); }

            List<TKey> distinct_keys = Keys.Distinct<TKey>().ToList<TKey>();

            Dictionary<TKey, TValue> output = new Dictionary<TKey, TValue>(distinct_keys.Count);
            foreach (TKey one_required_key in distinct_keys)
            {
                TValue one_value;
                Boolean found = Source.TryGetValue(one_required_key, out one_value);
                if (PassNotFoundKeys == false && found == false)
                {
                    throw new KeyNotFoundException("Ключ " + one_required_key + " не содержится во входном словаре");
                }
                else if (PassNotFoundKeys == true && found == false)
                {
                    continue;
                }
                else
                {
                    output.Add(one_required_key, one_value);
                }
            }
            return output;
        }

        /// <summary>
        /// Возвращает все элементы входного словаря, ключи которых находятся в указанном массиве ключей
        /// </summary>
        /// <remarks>Оболочка-перегрузка основного метода, принимающая ключи в виде массива params</remarks>
        /// <typeparam name="TKey">Тип ключа входного словаря и тип списка ключей - 
        /// должен поддерживать сравнение с самим собой путём реализации интерфейса IEquatable</typeparam>
        /// <typeparam name="TValue">Тип значения элемента входного словаря - неограниченный тип</typeparam>
        /// <param name="Source">Исходный словарь, из которого необходимо возвратить подсловарь. элементы которого содержат указанные ключи.</param>
        /// <param name="PassNotFoundKeys">Определяет, выбрасывать ли исключение, если хотя бы один из указанных ключей отсутствует во входном словаре. 
        /// Если true - все указанные ключи, отсутствующие в словаре, будут опущены, и будут возвращены те элементы, которые имеют ключи; 
        /// если false - при первом несовпадающем ключе будет выброшено KeyNotFoundException</param>
        /// <param name="Keys">Набор ключей, которые присутствуют во входном списке и значения с которыми из него требуется возвратить. 
        /// Дублирующиеся значения, если есть, будут проигнорированы.</param>
        /// <returns>Новый словарь, который содержит все элементы входного словаря, чьи ключи совпали с теми, что были переданы методу. 
        /// Элементы в этом словаре расположены в том порядке, в котором переданы ключи. 
        /// Если ни один из переданных ключей не содержится в исходном словаре и параметр PassNotFoundKeys выставлен в true, метод возвратит пустой словарь.</returns>
        public static Dictionary<TKey, TValue> GetPortionByKeys<TKey, TValue>(this IDictionary<TKey, TValue> Source, Boolean PassNotFoundKeys, params TKey[] Keys)
            where TKey : IEquatable<TKey>
        {
            if (Keys == null) { throw new ArgumentNullException("Keys"); }
            if (Keys.Any() == false) { throw new ArgumentException("Список ключей не может быть пустой", "Keys"); }

            List<TKey> keys_in_list = Keys.ToList<TKey>();
            return Source.GetPortionByKeys<TKey, TValue>(PassNotFoundKeys, keys_in_list);
        }

        /// <summary>
        /// Инвертирует входной словарь, преобразовывая значения в ключи и вызывая указанную функцию для множества ключей
        /// </summary>
        /// <typeparam name="TKey">Тип ключа входного словаря, должен поддерживать сравнение с самим собой</typeparam>
        /// <typeparam name="TValue">Тип значения входного словаря, должен поддерживать сравнение с самим собой</typeparam>
        /// <typeparam name="TNew">Тип, полученный в результате выполнения указанной через делегат функции, без ограничений</typeparam>
        /// <param name="Source">Входной словарь, не изменяется данным методом. Если NULL - будет выброшено исключение. Если пустой - будет возвращён пустой выходной словарь.</param>
        /// <param name="RemapFunc">Делегат на функцию, принимающую на вход набор элементов типа ключа входного словаря <typeparamref name="TKey"/> 
        /// и возвращающая один экземпляр типа <typeparamref name="TNew"/>. 
        /// Если во время вызова выбросит исключение, то в зависимости от значения параметра <paramref name="Strict"/> это исключение будет проброшено или подавлено.</param>
        /// <param name="Strict">Булевый флаг, указывающий, как обрабатывать NULL-значения входного словаря и исключения при вызове указанной через делегат функции. 
        /// Если true - при появлении NULL-значения или при возникновении исключения оно будет проброшено, 
        /// а если false - оно будет подавлено, а данный элемент выходного словаря будет пропущен.</param>
        /// <returns>Новый словарь, не являющийся NULL</returns>
        public static Dictionary<TValue, TNew> RemapDictionary<TKey, TValue, TNew>(this IDictionary<TKey, TValue> Source, Func<IEnumerable<TKey>, TNew> RemapFunc, Boolean Strict)
            where TKey : IEquatable<TKey>
            where TValue : IEquatable<TValue>
        {
            if (Source == null) { throw new ArgumentNullException("Source"); }
            if (RemapFunc == null) { throw new ArgumentNullException("RemapFunc"); }
            if (Source.Any() == false) { return new Dictionary<TValue, TNew>(0); }

            Dictionary<TValue, List<TKey>> temp = new Dictionary<TValue, List<TKey>>(Source.Count / 2);
            foreach (KeyValuePair<TKey, TValue> one in Source)
            {
                if (Object.ReferenceEquals(null, one.Value) == true)
                {
                    if (Strict == false)
                    { continue; }
                    else
                    {
                        throw new ArgumentException(
                          String.Format("Во входном словаре для ключа {0} его значение с типом {1} является NULL", one.Key.ToString(), typeof(TValue).FullName),
                          "Source");
                    }
                }
                if (temp.ContainsKey(one.Value) == true)
                {
                    temp[one.Value].Add(one.Key);
                }
                else
                {
                    temp.Add(one.Value, new List<TKey>(1) { one.Key });
                }
            }
            Dictionary<TValue, TNew> output = new Dictionary<TValue, TNew>(temp.Count);
            foreach (KeyValuePair<TValue, List<TKey>> one in temp)
            {
                TNew new_value;
                try
                {
                    new_value = RemapFunc.Invoke(one.Value);
                }
                catch (Exception ex)
                {
                    if (Strict == true)
                    {
                        throw new InvalidOperationException(String.Format(
                        "При вызове указанной через делегат функции для множества из {0} ключей, связанных со значением '{1}', было выброшено исключение",
                        one.Value.Count.ToString(CultureInfo.InvariantCulture), one.Key.ToString()),
                        ex);
                    }
                    else
                    { continue; }
                }
                output.Add(one.Key, new_value);
            }
            return output;
        }

        #region Random
        /// <summary>
        /// Выполняет перемешивание (тасование) элементов в данном массиве и записывает изменение в этот же экземпляр, возвращая ссылку на него. Использует указанный ГПСЧ.
        /// </summary>
        /// <remarks>http://en.wikipedia.org/wiki/Knuth_shuffle</remarks>
        /// <typeparam name="TItem">Тип элемента массива - без ограничений</typeparam>
        /// <param name="Source">Входной массив, в котором будет выполняться тасование. Если null или содержит меньше 2-х элементов - возвращается без изменений.</param>
        /// <param name="RandomInstance">Экземпляр ГПСЧ. Если null - будет выброшено исключение.</param>
        /// <returns></returns>
        public static TItem[] Shuffle<TItem>(this TItem[] Source, Random RandomInstance)
        {
            if (RandomInstance == null) { throw new ArgumentNullException("RandomInstance"); }
            if (Source == null || Source.Length < 2) { return Source; }
            for (Int32 i = Source.Length; i > 1; i--)
            {
                Int32 j = RandomInstance.Next(i);

                TItem temp = Source[j];
                Source[j] = Source[i - 1];
                Source[i - 1] = temp;
            }
            return Source;
        }

        /// <summary>
        /// Возвращает новый массив, содержащий перемешанные (перетасованные) элементы данного массива. Использует указанный ГПСЧ.
        /// </summary>
        /// <remarks>http://en.wikipedia.org/wiki/Knuth_shuffle</remarks>
        /// <typeparam name="TItem">Тип элемента массива - без ограничений</typeparam>
        /// <param name="Source">Входной массив, перемешанную копию которого следует возвратить.  Если null - будет выброшено исключение.</param>
        /// <param name="RandomInstance">Экземпляр ГПСЧ. Если null - будет выброшено исключение.</param>
        /// <returns></returns>
        public static TItem[] ShuffleNew<TItem>(this TItem[] Source, Random RandomInstance)
        {
            if (RandomInstance == null) { throw new ArgumentNullException("RandomInstance"); }
            if (Source == null) { throw new ArgumentNullException("Source"); }
            if (Source.Length == 0) { return new TItem[0] { }; }
            if (Source.Length == 1) { return new TItem[1] { Source[0] }; }

            TItem[] output = new TItem[Source.Length];
            output[0] = Source[0];
            for (Int32 i = 1; i < Source.Length; i++)
            {
                Int32 j = RandomInstance.Next(0, i + 1);
                if (j != i)
                {
                    output[i] = output[j];
                }
                output[j] = Source[i];
            }
            return output;
        }
        /// <summary>
        /// Возвращает один случайно выбранный элемент последовательности. Использует указанный ГПСЧ.
        /// </summary>
        /// <remarks>Jon Skeet. C# in Depth</remarks>
        /// <typeparam name="TItem">Тип элемента последовательности - без ограничений</typeparam>
        /// <param name="Source">Входная последовательность. Если null или пустая - будет выброшено исключение. Если содержит один элемент - он будет возвращён.</param>
        /// <param name="RandomInstance">Экземпляр ГПСЧ. Если null - будет выброшено исключение.</param>
        /// <returns></returns>
        public static TItem RandomItem<TItem>(this IEnumerable<TItem> Source, Random RandomInstance)
        {
            if (RandomInstance == null) { throw new ArgumentNullException("RandomInstance"); }
            if (Source == null) { throw new ArgumentNullException("Source"); }
            if (Source.Any() == false) { throw new ArgumentException("Последовательность не может быть пуста", "Source"); }
            if (Source.HasSingle() == true) { return Source.Single(); }
            IList<TItem> list = Source as IList<TItem>;
            if (list != null)
            {
                Int32 index = RandomInstance.Next(list.Count);
                return list[index];
            }
            ICollection<TItem> collection = Source as ICollection<TItem>;
            if (collection != null)
            {
                Int32 index = RandomInstance.Next(collection.Count);
                return collection.ElementAt(index);
            }
            using (IEnumerator<TItem> iterator = Source.GetEnumerator())
            {
                Int32 countSoFar = 1;
                TItem current = iterator.Current;
                while (iterator.MoveNext() == true)
                {
                    countSoFar++;
                    if (RandomInstance.Next(countSoFar) == 0)
                    {
                        current = iterator.Current;
                    }
                }
                return current;
            }
        }
        #endregion Random

        /// <summary>
        /// Возвращает один элемент из списка по его индексу, отсчитываемому с конца
        /// </summary>
        /// <typeparam name="TItem">Тип элемента списка - без ограничений</typeparam>
        /// <param name="Source">Входной список. Если NULL или пустой - будет выброшено исключение.</param>
        /// <param name="IndexFromEnd">Индекс элемента, который требуется возвратить, отсчитываемый с конца списка. Начинается с 0 включительно. 
        /// Если превышает общее количество элементов списка - выбрасывается исключение.</param>
        /// <returns></returns>
        public static TItem ItemFromEnd<TItem>(this IList<TItem> Source, Int32 IndexFromEnd)
        {
            if(Source == null) {throw new ArgumentNullException("Source");}
            if(Source.Any()==false) {throw new ArgumentException("Входной список пуст", "Source");}
            if(IndexFromEnd >= Source.Count) {throw new IndexOutOfRangeException
                (String.Format("Указанный индекс {0} выходит за пределы списка,который содержит {1} элементов", IndexFromEnd, Source.Count));}
            return Source[Source.Count - 1 - IndexFromEnd];
        }
    }
}

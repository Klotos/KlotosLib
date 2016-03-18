using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using KlotosLib.StringTools;

namespace KlotosLib
{
    /// <summary>
    /// Содержит методы расширения, которые принимают определённый один аргумент и выбрасывают исключение 'ArgumentException', 
    /// если этот аргумент находится в недопустимых состояниях.
    /// </summary>
    public static class ExceptionTools
    {
        /// <summary>
        /// Генерирует и выбрасывает исключение 'ArgumentException', если входная последовательность NULL или пустая
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T> Source)
        {
            if (Source == null) { throw new ArgumentException("Входная последовательность не может быть NULL"); }
            if (Source.Any() == false) { throw new ArgumentException("Входная последовательность не может быть пустой"); }
        }
        
        /// <summary>
        /// Генерирует и выбрасывает исключение 'ArgumentException' с указанным сообщением об ошибке, 
        /// если входная последовательность NULL или пустая
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Message"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentException"></exception>
        public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T> Source, String Message)
        {
            if (Source == null) { throw new ArgumentException(Message.ToStringS()); }
            if (Source.Any() == false) { throw new ArgumentException(Message.ToStringS()); }
        }

        /// <summary>
        /// Генерирует и выбрасывает исключение 'ArgumentException' с указанным названием параметра и сообщениями об ошибках, 
        /// если входная последовательность NULL или пустая
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="MessageIfNull"></param>
        /// <param name="MessageIfEmpty"></param>
        /// <param name="ParamName"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentException"></exception>
        public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T> Source, String MessageIfNull, String MessageIfEmpty, String ParamName)
        {
            if (Source == null) { throw new ArgumentException(MessageIfNull.ToStringS(), ParamName.ToStringS()); }
            if (Source.Any() == false) { throw new ArgumentException(MessageIfEmpty.ToStringS(), ParamName.ToStringS()); }
        }

        /// <summary>
        /// Генерирует и выбрасывает исключение 'ArgumentException' с указанным названием параметра и сообщениями об ошибках, 
        /// если входная последовательность NULL или пустая, или же все без исключения элементы входной последовательности являются NULL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="MessageIfNull"></param>
        /// <param name="MessageIfEmpty"></param>
        /// <param name="AllElementsNull"></param>
        /// <param name="ParamName"></param>
        public static void ThrowIfNullOrEmptyAll<T>(this IEnumerable<T> Source, String MessageIfNull, String MessageIfEmpty, String AllElementsNull, String ParamName)
        {
            if (Source == null) { throw new ArgumentException(MessageIfNull.ToStringS(), ParamName.ToStringS()); }
            if (Source.Any() == false) { throw new ArgumentException(MessageIfEmpty.ToStringS(), ParamName.ToStringS()); }
            if (typeof(T).IsValueType == false)
            {
                foreach (T element in Source)
                {
                    if((Object)element != null) {return;}
                }
                throw new ArgumentException(AllElementsNull.ToStringS(), ParamName.ToStringS());
            }
        }

        /// <summary>
        /// Генерирует исключение 'ArgumentException', если входная строка NULL, пустая или состоит из одних пробелов
        /// </summary>
        /// <param name="Source"></param>
        public static void ThrowIfNullEmptyWS(this String Source)
        {
            if (Source == null) { throw new ArgumentException("Входная строка не может быть NULL"); }
            if (String.IsNullOrEmpty(Source) == true) { throw new ArgumentException("Входная строка не может быть пустой"); }
            if (String.IsNullOrEmpty(Source.Trim()) == true) { throw new ArgumentException("Входная строка не может состоять из одних пробелов"); }
        }

        /// <summary>
        /// Генерирует исключение 'ArgumentException', если входное подключение к БД является NULL или находится не в статусе 'Open'
        /// </summary>
        /// <param name="Source"></param>
        public static void ThrowIfNullOrNotOpen(this SqlConnection Source)
        {
            if (Source == null) { throw new ArgumentException("Подключение к БД не может быть NULL"); }
            if (Source.State != ConnectionState.Open)
            { throw new ArgumentException("Подключение к БД должно быть открыто, тогда как его состояние: " + Source.State.ToString()); }
        }

        /// <summary>
        /// Возвращает название и текстовое описание указанного исключения и цепочки всех его вложенных под-исключений
        /// </summary>
        /// <param name="Ex">Исключение, полное текстовое описание которого следует возвратить. Если NULL, будет возвращена пустая строка.</param>
        /// <returns></returns>
        public static String TotalMessage(this Exception Ex)
        {
            if (Ex == null) { return String.Empty; }
            const String limiter = " -> ";
            StringBuilder output = new StringBuilder();
            Exception temp = Ex;
            do
            {
                output.Append(String.Format("{0}: {1}{2}", temp.GetType().Name, temp.Message, limiter));
                temp = temp.InnerException;
            }
            while (temp != null);
            output.CutRight(limiter.Length);
            return output.ToString();
        }

        private static readonly string[] LowNames = 
        {
            "NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", 
            "BS", "HT", "LF", "VT", "FF", "CR", "SO", "SI",
            "DLE", "DC1", "DC2", "DC3", "DC4", "NAK", "SYN", "ETB",
            "CAN", "EM", "SUB", "ESC", "FS", "GS", "RS", "US"
        };

        /// <summary>
        /// Выводит на текстовую консоль все символы указанной строки в режиме отладки
        /// </summary>
        /// <remarks>Copyright: Jon Skeet. http://habrahabr.ru/post/165597/</remarks>
        /// <param name="text"></param>
        public static void DisplayString(String text)
        {
            Console.WriteLine("String length: {0}", text.Length);
            foreach (char c in text)
            {
                if (c < 32)
                {
                    Console.WriteLine("<{0}> U+{1:x4}", LowNames[c], (int)c);
                }
                else if (c > 127)
                {
                    Console.WriteLine("(Possibly non-printable) U+{0:x4}", (int)c);
                }
                else
                {
                    Console.WriteLine("{0} U+{1:x4}", c, (int)c);
                }
            }
        }
    }

    /// <summary>
    /// Исключение, вызов которого размещается в тех местах исходного кода, котороые в случае корректного функционирования алгоритма должны быть недоступны для выполнения
    /// </summary>
    [Serializable()]
    public sealed class UnreachableCodeException : System.Exception
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public UnreachableCodeException() : base() { }

        /// <summary>
        /// Выполняет инициализацию нового экземпляра класса UnreachableCodeException, используя указанное сообщение об ошибке.
        /// </summary>
        /// <param name="message"></param>
        public UnreachableCodeException(String message) : base(message) { }

        /// <summary>
        /// Выполняет инициализацию нового экземпляра класса UnreachableCodeException с указанным сообщением об ошибке и ссылкой на внутреннее исключение, 
        /// которое стало причиной данного исключения.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public UnreachableCodeException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Возвращает сообщение, которое описывает текущее исключение.
        /// </summary>
        public new string Message { get { return base.Message; } }
    }
}

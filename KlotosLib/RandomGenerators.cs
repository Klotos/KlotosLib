using System;
using System.IO;
using System.Text;

namespace KlotosLib
{
    /// <summary>
    /// Служебный класс, обеспечивающий генерацию случайных строк и массивов указанной длины
    /// </summary>
    public static class RandomGenerators
    {
        /// <summary>
        /// Единый экземпляр класса Random, который используется методами
        /// </summary>
        private static readonly Random _randomInstance = new Random();
        /// <summary>
        /// Служебный объект для обеспечения потокобезопасности
        /// </summary>
        private static readonly object _syncLock = new object();

        /// <summary>
        /// Возвращает случайную строку указанной длины, содержащую числа и строчные буквы латинского алфавита, используя в качестве ГПСЧ механизм GUID.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="Length">Желаемая длина сгенерированной строки. Если указан 0, возвращается пустая строка</param>
        /// <returns></returns>            
        public static String GetRandomStringGUID(Int32 Length)
        {
            if (Length < 0) { throw new ArgumentOutOfRangeException("Length", Length, "Длина строки не может быть отрицательной"); }
            if (Length == 0) { return ""; }

            String guidResult = System.Guid.NewGuid().ToString();
            guidResult = guidResult.Replace("-", string.Empty);// удалить дефисы
            if (Length > guidResult.Length) { Length = guidResult.Length; }
            return guidResult.Substring(0, Length);

        }

        /// <summary>
        /// Возвращает случайную строку указанной длины, содержащую цифры, строчные и заглавные буквы латинского алфавита. Использует собственный ГПСЧ.
        /// </summary>
        /// <param name="Length">Желаемая длина строки. Если меньше 0, выбрасывается исключение. Если 0 - возвращается пустая строка.</param>
        /// <returns></returns>
        public static String GetRandomString(Int32 Length)
        {
            lock (_syncLock)
            {
                return RandomGenerators.GetRandomString(Length, RandomGenerators._randomInstance);
            }
        }

        /// <summary>
        /// Возвращает случайную строку указанной длины, содержащую цифры, строчные и заглавные буквы латинского алфавита. Использует указанный ГПСЧ.
        /// </summary>
        /// <param name="Length">Желаемая длина строки. Если меньше 0, выбрасывается исключение. Если 0 - возвращается пустая строка.</param>
        /// <param name="RandomInstance">Экземпляр ГПСЧ. Если null - будет выброшено исключение.</param>
        /// <returns></returns>
        public static String GetRandomString(Int32 Length, Random RandomInstance)
        {
            if (Length < 0) { throw new ArgumentOutOfRangeException("Length", Length, "Длина строки не может быть отрицательной"); }
            if (Length == 0) { return ""; }
            if (RandomInstance == null) { throw new ArgumentNullException("RandomInstance"); }
            const String availableChars = "abcdefghijklmnopqrstuvwxzyABCDEFGHIJKLMNOPQRSTUVWXZY0123456789";

            StringBuilder sb = new StringBuilder(Length);
            for (Int32 i = 0; i < Length; i++)
            {
                sb.Append(availableChars[RandomInstance.Next(0, availableChars.Length - 1)]);
            }
            return sb.ToString();

        }

        /// <summary>
        /// Возвращает случайную строку указанной длины, содержащую лишь числа, заглавные и строчные буква латинского алфавита, 
        /// но не содержащую цифру "0" и букву "O/o". Использует собственный ГПСЧ.
        /// </summary>        
        /// <param name="Length">Желаемая длина сгенерированной строки. Если меньше 0, выбрасывается исключение. Если 0 - возвращается пустая строка</param>        
        /// <returns>Сгенерированная случайным образом строка</returns>
        public static string GetAlphanumericPassword(Int32 Length)
        {
            lock (RandomGenerators._syncLock)
            {
                return RandomGenerators.GetAlphanumericPassword(Length, RandomGenerators._randomInstance);
            }
        }

        /// <summary>
        /// Возвращает случайную строку указанной длины, содержащую лишь числа, заглавные и строчные буква латинского алфавита, 
        /// но не содержащую цифру "0" и букву "O/o". Использует указанный ГПСЧ.
        /// </summary>        
        /// <param name="Length">Желаемая длина сгенерированной строки. Если меньше 0, выбрасывается исключение. Если 0 - возвращается пустая строка</param>
        /// <param name="RandomInstance">Экземпляр ГПСЧ. Если null - будет выброшено исключение.</param>
        /// <returns>Сгенерированная случайным образом строка</returns>
        public static string GetAlphanumericPassword(Int32 Length, Random RandomInstance)
        {
            if (Length < 0) { throw new ArgumentOutOfRangeException("Length", Length, "Длина строки не может быть отрицательной"); }
            if (Length == 0) { return ""; }
            if (RandomInstance == null) { throw new ArgumentNullException("RandomInstance"); }
            StringBuilder builder = new StringBuilder(Length);
            int[,] range = 
            { 
                { 49, 58 },      // 1 - 9 (not 0)
                { 65, 79 },      // A - N (not O)
                { 97, 111 },     // a - n (not o)
                { 49, 58 },      // 1 - 9 (not 0)
                { 80, 91 },      // P - Z
                { 112, 123 }     // p - z
            };
            Int32 randomNum;
            for (Int32 i = 0; i < Length; i++)
            {
                randomNum = RandomInstance.Next(0, 6);
                builder.Append((Char)RandomInstance.Next(range[randomNum, 0], range[randomNum, 1]));
            }
            return builder.ToString();

        }

        /// <summary>
        /// Генерирует и возвращает массив указанной длины, заполненный элементами указанного типа. Использует собственный ГПСЧ.
        /// </summary>
        /// <typeparam name="TItem">Тип элементов. Поддерживаются только целые числа.</typeparam>
        /// <param name="Count">Количество элементов, которое должно содержать в выходном массиве. Если 0 - будет возвращён пустой массив. Если отрицательное - из него будет взят модуль.</param>        
        /// <returns></returns>
        public static TItem[] GenerateRandomSequence<TItem>(Int32 Count)
        {
            lock (RandomGenerators._syncLock)
            {
                return RandomGenerators.GenerateRandomSequence<TItem>(Count, RandomGenerators._randomInstance);
            }
        }

        /// <summary>
        /// Генерирует и возвращает массив указанной длины, заполненный элементами указанного типа. Использует указанный ГПСЧ.
        /// </summary>
        /// <typeparam name="TItem">Тип элементов. Поддерживаются только целые числа.</typeparam>
        /// <param name="Count">Количество элементов, которое должно содержать в выходном массиве. Если 0 - будет возвращён пустой массив. Если отрицательное - из него будет взят модуль.</param>
        /// <param name="RandomInstance">Экземпляр ГПСЧ. Если null - будет выброшено исключение.</param>
        /// <returns></returns>
        public static TItem[] GenerateRandomSequence<TItem>(Int32 Count, Random RandomInstance)
        {
            if (RandomInstance == null) { throw new ArgumentNullException("RandomInstance"); }
            Type input_type = typeof(TItem);
            if (Count < 0) { Count = Math.Abs(Count); }
            TItem[] output = new TItem[Count];
            if (Count == 0) { return output; }

            if (input_type == typeof(Byte))
            {
                Byte temp;
                TItem elem;
                for (Int32 i = 0; i < Count; i++)
                {
                    temp = Convert.ToByte(RandomInstance.Next(Byte.MinValue, Byte.MaxValue));
                    elem = (TItem)(Object)temp;
                    output[i] = elem;
                }
            }
            else if (input_type == typeof(SByte))
            {
                SByte temp;
                TItem elem;
                for (Int32 i = 0; i < Count; i++)
                {
                    temp = Convert.ToSByte(RandomInstance.Next(SByte.MinValue, SByte.MaxValue));
                    elem = (TItem)(Object)temp;
                    output[i] = elem;
                }
            }
            else if (input_type == typeof(Int16))
            {
                Int16 temp;
                TItem elem;
                for (Int32 i = 0; i < Count; i++)
                {
                    temp = Convert.ToInt16(RandomInstance.Next(Int16.MinValue, Int16.MaxValue));
                    elem = (TItem)(Object)temp;
                    output[i] = elem;
                }
            }
            else if (input_type == typeof(UInt16))
            {
                UInt16 temp;
                TItem elem;
                for (Int32 i = 0; i < Count; i++)
                {
                    temp = Convert.ToUInt16(RandomInstance.Next(UInt16.MinValue, UInt16.MaxValue));
                    elem = (TItem)(Object)temp;
                    output[i] = elem;
                }
            }
            else if (input_type == typeof(Int32))
            {
                Int32 temp;
                TItem elem;
                for (Int32 i = 0; i < Count; i++)
                {
                    temp = Convert.ToInt32(RandomInstance.Next(Int32.MinValue, Int32.MaxValue));
                    elem = (TItem)(Object)temp;
                    output[i] = elem;
                }
            }
            else if (input_type == typeof(UInt32))
            {
                UInt32 temp;
                TItem elem;
                for (Int32 i = 0; i < Count; i++)
                {
                    temp = Convert.ToUInt32(RandomInstance.Next(0, Int32.MaxValue)) * 2;
                    elem = (TItem)(Object)temp;
                    output[i] = elem;
                }
            }
            else if (input_type == typeof(Int64))
            {
                Int32 temp1;
                Int32 temp2;
                Int64 temp;
                TItem elem;
                for (Int32 i = 0; i < Count; i++)
                {
                    temp1 = RandomInstance.Next(Int32.MinValue, Int32.MaxValue);
                    temp2 = RandomInstance.Next(Int32.MinValue, Int32.MaxValue);
                    temp = NumericTools.Combine(temp1, temp2);
                    elem = (TItem)(Object)temp;
                    output[i] = elem;
                }
            }
            else if (input_type == typeof(UInt64))
            {
                UInt32 temp1;
                UInt32 temp2;
                UInt64 temp;
                TItem elem;
                for (Int32 i = 0; i < Count; i++)
                {
                    temp1 = Convert.ToUInt32(RandomInstance.Next(0, Int32.MaxValue)) * 2;
                    temp2 = Convert.ToUInt32(RandomInstance.Next(0, Int32.MaxValue)) * 2;
                    temp = NumericTools.Combine(temp1, temp2);
                    elem = (TItem)(Object)temp;
                    output[i] = elem;
                }
            }
            else
            {
                throw new ArgumentException("Указанный тип (" + typeof(TItem).FullName + ") не поддерживается", "TItem");
            }

            return output;
        }

        /// <summary>
        /// Генерирует и возвращает строковый массив указанной длины, все строки которого имеют длину в указанных пределах. Использует собственный ГПСЧ.
        /// </summary>
        /// <param name="MinStringLength">Минимальная длина строки включительно. Если меньше 0, будет выброшено исключение.</param>
        /// <param name="MaxStringLength">Максимальная длина строки исключительно. Если меньше 1 или минимальной границы, будет выброшено исключение.</param>
        /// <param name="Count">Количество элементов, которое должно содержать в выходном массиве. Если 0 - будет возвращён пустой массив. Если отрицательное - из него будет взят модуль.</param>        
        /// <returns></returns>
        public static String[] GenerateRandomStringSequence(Int32 MinStringLength, Int32 MaxStringLength, Int32 Count)
        {
            lock (RandomGenerators._syncLock)
            {
                return GenerateRandomStringSequence(MinStringLength, MaxStringLength, Count, RandomGenerators._randomInstance);
            }
        }

        /// <summary>
        /// Генерирует и возвращает строковый массив указанной длины, все строки которого имеют длину в указанных пределах. Использует указанный ГПСЧ.
        /// </summary>
        /// <param name="MinStringLength">Минимальная длина строки включительно. Если меньше 0, будет выброшено исключение.</param>
        /// <param name="MaxStringLength">Максимальная длина строки исключительно. Если меньше 1 или минимальной границы, будет выброшено исключение.</param>
        /// <param name="Count">Количество элементов, которое должно содержать в выходном массиве. Если 0 - будет возвращён пустой массив. Если отрицательное - из него будет взят модуль.</param>
        /// <param name="RandomInstance">Экземпляр ГПСЧ. Если null - будет выброшено исключение.</param>
        /// <returns></returns>
        public static String[] GenerateRandomStringSequence(Int32 MinStringLength, Int32 MaxStringLength, Int32 Count, Random RandomInstance)
        {
            if (MinStringLength < 0) { throw new ArgumentOutOfRangeException("MinStringLength", MinStringLength, "Минимальная длина строки не может быть меньше 0"); }
            if (MaxStringLength < 1) { throw new ArgumentOutOfRangeException("MaxStringLength", MaxStringLength, "Максимальная длина строки не может быть меньше 1"); }
            if (MaxStringLength < MinStringLength)
            { throw new ArgumentException(String.Format("Минимальная длина строки ({0}) не может быть больше за максимальную ({1})", MinStringLength, MaxStringLength)); }
            if (RandomInstance == null) { throw new ArgumentNullException("RandomInstance"); }

            if (Count < 0) { Count = Math.Abs(Count); }
            String[] output = new String[Count];
            if (Count == 0) { return output; }
            Int32 diff = Convert.ToInt32(NumericTools.GetDifferenceAbs(MinStringLength, MaxStringLength));
            if (diff == 0) { diff = MaxStringLength; }

            Byte[] byte_array = new Byte[diff];
            Int32 random_length;
            Char temp_char;
            StringBuilder strB_temp = new StringBuilder(diff);

            for (Int32 i = 0; i < Count; i++)
            {
                random_length = RandomInstance.Next(MinStringLength, MaxStringLength);
                RandomInstance.NextBytes(byte_array);
                for (Int32 j = 0; j < random_length; j++)
                {
                    temp_char = Convert.ToChar(byte_array[j]);
                    strB_temp.Append(temp_char);
                }
                output[i] = strB_temp.ToString();
                strB_temp.Clean();
            }
            return output;
        }

        /// <summary>
        /// Генерирует и возвращает строковый массив указанной длины, все строки которого имеют длину в указанных пределах и состоят только из указанных символов. 
        /// Использует собственный ГПСЧ.
        /// </summary>
        /// <param name="MinStringLength">Минимальная длина строки включительно. Если меньше 0, будет выброшено исключение.</param>
        /// <param name="MaxStringLength">Максимальная длина строки исключительно. Если меньше 1 или минимальной границы, будет выброшено исключение.</param>
        /// <param name="Count">Количество элементов, которое должно содержать в выходном массиве. Если 0 - будет возвращён пустой массив. Если отрицательное - из него будет взят модуль.</param>        
        /// <param name="AllowableChars">Набор допустимых символов, которые и только которые могут использоваться для генерации строк. Если null или пуст - выбрасывается исключение.</param>
        /// <returns></returns>
        public static String[] GenerateRandomStringSequence(Int32 MinStringLength, Int32 MaxStringLength, Int32 Count, params Char[] AllowableChars)
        {
            lock (RandomGenerators._syncLock)
            {
                return GenerateRandomStringSequence(MinStringLength, MaxStringLength, Count, RandomGenerators._randomInstance, AllowableChars);
            }
        }

        /// <summary>
        /// Генерирует и возвращает строковый массив указанной длины, все строки которого имеют длину в указанных пределах и состоят только из указанных символов. 
        /// Использует указанный ГПСЧ.
        /// </summary>
        /// <param name="MinStringLength">Минимальная длина строки включительно. Если меньше 0, будет выброшено исключение.</param>
        /// <param name="MaxStringLength">Максимальная длина строки исключительно. Если меньше 1 или минимальной границы, будет выброшено исключение.</param>
        /// <param name="Count">Количество элементов, которое должно содержать в выходном массиве. Если 0 - будет возвращён пустой массив. Если отрицательное - из него будет взят модуль.</param>
        /// <param name="RandomInstance">Экземпляр ГПСЧ. Если null - будет выброшено исключение.</param>
        /// <param name="AllowableChars">Набор допустимых символов, которые и только которые могут использоваться для генерации строк. Если null или пуст - выбрасывается исключение.</param>
        /// <returns></returns>
        public static String[] GenerateRandomStringSequence(Int32 MinStringLength, Int32 MaxStringLength, Int32 Count, Random RandomInstance, params Char[] AllowableChars)
        {
            if (MinStringLength < 0) { throw new ArgumentOutOfRangeException("MinStringLength", MinStringLength, "Минимальная длина строки не может быть меньше 0"); }
            if (MaxStringLength < 1) { throw new ArgumentOutOfRangeException("MaxStringLength", MaxStringLength, "Максимальная длина строки не может быть меньше 1"); }
            if (MaxStringLength < MinStringLength)
            { throw new ArgumentException(String.Format("Минимальная длина строки ({0}) не может быть больше за максимальную ({1})", MinStringLength, MaxStringLength)); }
            if (RandomInstance == null) { throw new ArgumentNullException("RandomInstance"); }
            if (AllowableChars.IsNullOrEmpty() == true) { throw new ArgumentException("Набор допустимых символов не может быть null или пуст", "AllowableChars"); }

            if (Count < 0) { Count = Math.Abs(Count); }
            String[] output = new String[Count];
            if (Count == 0) { return output; }
            Int32 diff = Convert.ToInt32(NumericTools.GetDifferenceAbs(MinStringLength, MaxStringLength));
            if (diff == 0) { diff = MaxStringLength; }

            Int32 random_length;
            StringBuilder strB_temp = new StringBuilder(diff);
            Char temp_char;

            for (Int32 i = 0; i < output.Length; i++)
            {
                random_length = RandomInstance.Next(MinStringLength, MaxStringLength);
                for (Int32 j = 0; j < random_length; j++)
                {
                    temp_char = AllowableChars[RandomInstance.Next(0, AllowableChars.Length)];
                    strB_temp.Append(temp_char);
                }
                output[i] = strB_temp.ToString();
                strB_temp.Clean();
            }
            return output;
        }

        /// <summary>
        /// Генерирует и возвращает изменяемую строку указанной длины, заполненную случайными символами, входящими в указанный набор. Использует собственный ГПСЧ.
        /// </summary>
        /// <param name="Length">Желаемая длина строки в символах. Если 0 - будет возвращена пустая строка. Если меньше 0 - будет выброшено исключение.</param>
        /// <param name="AllowableChars">Набор допустимых символов, которые и только которые могут использоваться для генерации строк. Если null или пуст - выбрасывается исключение.</param>
        /// <returns></returns>
        public static StringBuilder GenerateRandomString(Int32 Length, params Char[] AllowableChars)
        {
            lock (RandomGenerators._syncLock)
            {
                return RandomGenerators.GenerateRandomString(Length, RandomGenerators._randomInstance, AllowableChars);
            }
        }

        /// <summary>
        /// Генерирует и возвращает изменяемую строку указанной длины, заполненную случайными символами, входящими в указанный набор. Использует указанный ГПСЧ.
        /// </summary>
        /// <param name="Length">Желаемая длина строки в символах. Если 0 - будет возвращена пустая строка. Если меньше 0 - будет выброшено исключение.</param>
        /// <param name="RandomInstance">Экземпляр ГПСЧ. Если null - будет выброшено исключение.</param>
        /// <param name="AllowableChars">Набор допустимых символов, которые и только которые могут использоваться для генерации строк. Если null или пуст - выбрасывается исключение.</param>
        /// <returns></returns>
        public static StringBuilder GenerateRandomString(Int32 Length, Random RandomInstance, params Char[] AllowableChars)
        {
            if (AllowableChars.IsNullOrEmpty() == true) { throw new ArgumentException("Набор допустимых символов не может быть null или пуст", "AllowableChars"); }
            if (RandomInstance == null) { throw new ArgumentNullException("RandomInstance"); }
            if (Length < 0) { throw new ArgumentOutOfRangeException("Length", Length, "Желаемая длина не может быть отрицательной"); }
            StringBuilder output = new StringBuilder(Length);
            Char temp_char;
            for (Int32 i = 0; i < Length; i++)
            {
                temp_char = AllowableChars[RandomInstance.Next(0, AllowableChars.Length)];
                output.Append(temp_char);
            }
            return output;
        }

        /// <summary>
        /// Генерирует и возвращает байтовый поток в памяти указанной длины, заполненный случайными байтами. Использует собственный ГПСЧ.
        /// </summary>
        /// <param name="Length">Желаемая длина байтового потока в байтах. Если 0 - будет возвращён пустой поток. Если меньше 0 - будет выброшено исключение.</param>
        /// <param name="ResetPosition">Определяет, необходимо ли после создания потока обнулить (сбросить) его ползунок.</param>
        /// <returns></returns>
        public static MemoryStream GenerateRandomStream(Int32 Length, Boolean ResetPosition)
        {
            lock (RandomGenerators._syncLock)
            {
                return RandomGenerators.GenerateRandomStream(Length, ResetPosition, RandomGenerators._randomInstance);
            }
        }

        /// <summary>
        /// Генерирует и возвращает байтовый поток в памяти указанной длины, заполненный случайными байтами. Использует указанный ГПСЧ.
        /// </summary>
        /// <param name="Length">Желаемая длина байтового потока в байтах. Если 0 - будет возвращён пустой поток. Если меньше 0 - будет выброшено исключение.</param>
        /// <param name="ResetPosition">Определяет, необходимо ли после создания потока обнулить (сбросить) его ползунок.</param>
        /// <param name="RandomInstance">Экземпляр ГПСЧ. Если null - будет выброшено исключение.</param>
        /// <returns></returns>
        public static MemoryStream GenerateRandomStream(Int32 Length, Boolean ResetPosition, Random RandomInstance)
        {
            if (RandomInstance == null) { throw new ArgumentNullException("RandomInstance"); }
            if (Length < 0) { throw new ArgumentOutOfRangeException("Length", Length, "Желаемая длина байтового потока не может быть отрицательной"); }
            if (Length == 0) { return new MemoryStream(new byte[0] { }, true); }
            MemoryStream output = new MemoryStream(Length);
            const Int32 chunk_size = 4096;
            Byte[] portion;
            if (Length <= chunk_size)
            {
                portion = new Byte[Length];
                RandomInstance.NextBytes(portion);
                output.Write(portion, 0, Length);
            }
            else
            {
                portion = new Byte[chunk_size];
                Int32 to_write = Length;
                while (to_write > 0)
                {
                    Int32 temp = to_write >= chunk_size ? chunk_size : to_write;
                    RandomInstance.NextBytes(portion);
                    output.Write(portion, 0, temp);
                    to_write = to_write - chunk_size;
                }
            }
            if (ResetPosition == true)
            { output.Position = 0; }
            return output;
        }
    }
}

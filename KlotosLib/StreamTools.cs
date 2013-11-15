using System;
using System.IO;

namespace KlotosLib
{
    /// <summary>
    /// Содержит статические методы и методы-расширения по работе с потоками
    /// </summary>
    public static class StreamTools
    {
        /// <summary>
        /// Читает содержимое из указанного потока до достижении его конца и возвращает прочитанные данные внутри байтового массива
        /// </summary>
        /// <remarks>Code sample: Jon Skeet http://www.yoda.arachsys.com/csharp/readbinary.html</remarks>
        /// <param name="SourceStream">Поток, из которого нужно считать данные. Должен поддерживать чтение и при определённом значении параметра должен поддерживать поиск.</param>
        /// <param name="ResetPosition">Определяет, необходимо ли обнулить позицию ползунка в потоке <paramref name="SourceStream"/> перед началом чтения из него. 
        /// Если true - позиция обнуляется и поток читается с начала, однако в этом случае поток должен поддерживать операцию поиска (перемещения ползунка). 
        /// Если false - поток читается с текущей позиции, и поддержка операции поиска не требуется.</param>
        public static Byte[] ReadFullStreamToByteArray(this Stream SourceStream, Boolean ResetPosition)
        {
            if (SourceStream == null) { throw new ArgumentNullException("SourceStream"); }
            if (SourceStream.CanRead == false) { throw new ArgumentException("Поток должен поддерживать операцию чтения данных из него", "SourceStream"); }
            if (ResetPosition == true)
            {
                if (SourceStream.CanSeek == false) { throw new ArgumentException("Поток должен поддерживать операцию поиска (перемещения ползунка)", "SourceStream"); }
                SourceStream.Seek(0, SeekOrigin.Begin);
            }

            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = SourceStream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }

        /// <summary>
        /// Читает содержимое из указанного потока до достижении его конца и возвращает прочитанные данные внутри байтового массива. Для перекачки данных использует буфер указанного размера.
        /// </summary>
        /// <remarks>Code sample: Jon Skeet http://www.yoda.arachsys.com/csharp/readbinary.html</remarks>
        /// <param name="SourceStream">Поток, из которого нужно считать данные. Должен поддерживать чтение и при определённом значении параметра должен поддерживать поиск.</param>
        /// <param name="InitialLength">приблизительный начальный размер внутреннего буфера в байтах</param>
        /// <param name="ResetPosition">Определяет, необходимо ли обнулить (сбросить) позицию ползунка в потоке <paramref name="SourceStream"/> перед началом чтения из него. 
        /// Если true - позиция обнуляется и поток читается с начала, однако в этом случае поток должен поддерживать операцию поиска (перемещения ползунка). 
        /// Если false - поток читается с текущей позиции, и поддержка операции поиска не требуется.</param>
        public static byte[] ReadFullStreamToByteArray(this Stream SourceStream, Int32 InitialLength, Boolean ResetPosition)
        {
            if (SourceStream == null) { throw new ArgumentNullException("SourceStream"); }
            if (SourceStream.CanRead == false) { throw new ArgumentException("Поток должен поддерживать операцию чтения данных из него", "SourceStream"); }
            if (ResetPosition == true)
            {
                if (SourceStream.CanSeek == false) { throw new ArgumentException("Поток должен поддерживать операцию поиска (перемещения ползунка)", "SourceStream"); }
                SourceStream.Seek(0, SeekOrigin.Begin);
            }

            if (InitialLength < 4)
            {
                InitialLength = 32768;
            }

            byte[] buffer = new byte[InitialLength];
            long read = 0;

            int chunk;
            while ((chunk = SourceStream.Read(buffer, (Int32)read, buffer.Length - (Int32)read)) > 0)
            {
                read += chunk;

                if (read == buffer.Length)
                {
                    int nextByte = SourceStream.ReadByte();

                    if (nextByte == -1)
                    {
                        return buffer;
                    }

                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }

            byte[] ret = new byte[read];
            Array.Copy(buffer, ret, read);
            return ret;
        }

        /// <summary>
        /// Копирует весь контент из одного потока в другой, используя внутренний буфер в 32КБ
        /// </summary>
        /// <param name="InputStream">Поток, из которого необходимо выполнить копирование контента</param>
        /// <param name="OutputStream">Поток, в который необходимо записать контент</param>
        /// <param name="ResetInputPosition">Определяет, необходимо ли обнулить (сбросить) позицию ползунка во входном потоке <paramref name="InputStream"/> 
        /// перед началом чтения из него. 
        /// Если true - позиция обнуляется и поток читается с начала, однако в этом случае поток должен поддерживать операцию поиска (перемещения ползунка). 
        /// Если false - поток читается с текущей позиции, и поддержка операции поиска не требуется.</param>
        /// <param name="ResetOutputPosition">Определяет, необходимо ли обнулить (сбросить) позицию ползунка в выходном потоке <paramref name="OutputStream"/> 
        /// перед началом записи в него. 
        /// Если true - позиция обнуляется и поток читается с начала, однако в этом случае поток должен поддерживать операцию поиска (перемещения ползунка). 
        /// Если false - поток читается с текущей позиции, и поддержка операции поиска не требуется.</param>
        public static void CopyStream(Stream InputStream, Stream OutputStream, Boolean ResetInputPosition, Boolean ResetOutputPosition)
        {
            if (InputStream == null) { throw new ArgumentNullException("InputStream"); }
            if (OutputStream == null) { throw new ArgumentNullException("OutputStream"); }
            if (InputStream.CanRead == false) throw new ArgumentException("Входной поток должен поддерживать операцию чтения", "InputStream");
            if (OutputStream.CanWrite == false) throw new ArgumentException("Выходной поток должен поддерживать операцию записи", "OutputStream");
            if (ResetInputPosition == true)
            {
                if (InputStream.CanSeek == false)
                { throw new ArgumentException("Входной поток должен поддерживать операцию поиска (перемещения ползунка), так зак затребовано обнуление (сброс) ползунка", "InputStream"); }
                else { InputStream.Seek(0, SeekOrigin.Begin); }
            }
            if (ResetOutputPosition == true)
            {
                if (OutputStream.CanSeek == false)
                { throw new ArgumentException("Выходной поток должен поддерживать операцию поиска (перемещения ползунка), так зак затребовано обнуление (сброс) ползунка", "OutputStream"); }
                else { OutputStream.Seek(0, SeekOrigin.Begin); }
            }
            byte[] buffer = new byte[32768];
            int read;
            while ((read = InputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                OutputStream.Write(buffer, 0, read);
            }
        }

        /// <summary>
        /// Копирует указанную часть контента из одного указанного потока в другой указанный поток, 
        /// определяя часть путём указания позиции в исходном потоке, с которой необходимо начать копирование, и количества байт, которые необходимо скопировать.
        /// </summary>
        /// <param name="InputStream">Поток, из которого необходимо выполнить копирование контента</param>
        /// <param name="OutputStream">Поток, в который необходимо записать контент</param>
        /// <param name="StartPosition">Позиция во входном потоке, с которой необходимо начать копирование, начиная с 0. Если больше или равна длине входного потока, выбрасывается исключение.</param>
        /// <param name="Length">Количество байт, которые необходимо скопирповать из входного потока. Если больше фактической длины оставшейся части, будет обрезано к ней.</param>
        /// <returns>Фактическое количество скопированных байт</returns>
        public static Int32 CopyStream(Stream InputStream, Stream OutputStream, Int32 StartPosition, Int32 Length)
        {
            if (InputStream == null) { throw new ArgumentNullException("InputStream"); }
            if (OutputStream == null) { throw new ArgumentNullException("OutputStream"); }
            if (InputStream.CanRead == false) { throw new ArgumentException("Входной поток должен поддерживать операцию чтения", "InputStream"); }
            if (InputStream.CanSeek == false) { throw new ArgumentException("Входной поток должен поддерживать операцию поиска (перемещения ползунка)", "InputStream"); }
            if (OutputStream.CanWrite == false) { throw new ArgumentException("Выходной поток должен поддерживать операцию записи", "OutputStream"); }
            if (Length < 1) { throw new ArgumentOutOfRangeException("Length", "Значение параметра Length = '" + Length + "', тогда как оно не может быть меньше 1"); }
            if (StartPosition < 0) { throw new ArgumentOutOfRangeException("StartPosition", "Значение параметра StartPosition = '" + StartPosition + "', тогда как оно не может быть отрицательно"); }
            if (StartPosition >= InputStream.Length)
            {
                throw new ArgumentOutOfRangeException("StartPosition",
                    String.Format("Входной поток содержит {0} байт, тогда как указанная начальная позиция для копирования - {1}, и превышает длину потока.", InputStream.Length, StartPosition));
            }
            if (Length > InputStream.Length - StartPosition)
            {
                Length = (Int32)(InputStream.Length - StartPosition);
            }

            const Int32 chunk_size = 4096;
            InputStream.Position = StartPosition;

            int read;
            byte[] buffer;

            if (Length <= chunk_size)
            {
                buffer = new byte[Length];
                read = InputStream.Read(buffer, 0, buffer.Length);
                OutputStream.Write(buffer, 0, read);
                return read;
            }
            else
            {
                buffer = new byte[chunk_size];
                Int32 read_actual = 0;

                while (read_actual + chunk_size < Length)
                {
                    read = InputStream.Read(buffer, 0, buffer.Length);
                    OutputStream.Write(buffer, 0, read);
                    read_actual = read_actual + read;
                }
                buffer = new byte[Length - read_actual];
                read = InputStream.Read(buffer, 0, buffer.Length);
                read_actual = read_actual + read;
                OutputStream.Write(buffer, 0, read);
                return read_actual;
            }
        }

        /// <summary>
        /// Читает из указанного абстрактного потока указанное количество байт, начиная с указанной позиции, 
        /// и в зависимости от значения параметра возвращает или не возвращает ползунок на прежнее место. Не зависит от текущего значения ползунка.
        /// </summary>
        /// <param name="SourceStream">Поток, с которого производится чтение. Не может быть NULL, должен поддерживать чтение и поиск (перемещение ползунка).</param>
        /// <param name="Start">Позиция (начиная с 0), с которой производится начало чтения. Если меньше 0 или больше фактической длины потока, выбрасывается исключение.</param>
        /// <param name="Length">Длина диапазона в байтах, который необходимо считать с файлового потока. 
        /// Если превышает фактическую длину с учётом указанной позиции, метод возвращает всю оставшуюся часть. Если меньше 0 — выбрасывается исключение. Если 0 — возвращается пустой массив.</param>
        /// <param name="RenewPosition">Если true — после исполнения метода ползунок будет возвращён на ту же позицию, в которой он был изначально. 
        /// Если false — позиция ползунка не будет восстанавливаться.</param>
        /// <returns>Байтовый массив, не являющийся NULL и не превышающий длину <paramref name="Length"/>.</returns>
        public static Byte[] ReadPortion(this Stream SourceStream, Int32 Start, Int32 Length, Boolean RenewPosition)
        {
            if (Start < 0) { throw new ArgumentOutOfRangeException("Start", "Начальная позиция = " + Start + " не может быть отрицательной"); }
            if (Length < 0) { throw new ArgumentOutOfRangeException("Length", "Длина диапазона = " + Length + " не может быть отрицательной"); }
            if (Length == 0) { return new Byte[0]; }
            if (SourceStream == null) { throw new ArgumentNullException("SourceStream"); }
            if (SourceStream.CanRead == false) { throw new ArgumentException("Поток не поддерживает чтение", "SourceStream"); }
            if (SourceStream.CanSeek == false) { throw new ArgumentException("Поток не поддерживает операции поиска (перемещение ползунка)", "SourceStream"); }
            Int64 fs_len = SourceStream.Length;
            if (Start >= fs_len) { throw new ArgumentOutOfRangeException("Start", Start, "Начальная позиция = " + Start + " превышает длину потока = " + fs_len); }
            if (Start + Length >= fs_len) { Length = Convert.ToInt32(fs_len - Start); }
            Int64 orig_pos = SourceStream.Position;
            SourceStream.Position = Start;
            Byte[] output = new Byte[Length];
            Int32 was_read = SourceStream.Read(output, 0, output.Length);
            if (was_read != output.Length)
            { throw new InvalidOperationException("В байтовый массив должно было быть записано " + output.Length + " байт, тогда как фактически было записано " + was_read + " байт"); }
            if (RenewPosition == true) { SourceStream.Position = orig_pos; }
            return output;
        }

        /// <summary>
        /// Записывает весь контент из указанного потока в файл по указанному полному пути, создавая его или перезаписывая с нуля
        /// </summary>
        /// <param name="SourceStream">Входной поток, из которого выполняется копирование всего контента, вне зависимости от позиции ползунка. 
        /// Должен поддерживать чтение и поиск (перемещение ползунка).</param>
        /// <param name="FilePath">Путь, включая имя файла, который будет создан. Поддерживается как абсолютный, так и относительный путь. 
        /// Если файл по данному пути уже существует, он будет перезаписан. Если путь некорректен, будет выброшено исключение.</param>
        /// <param name="ResetSourcePosition">Определяет, необходимо ли обнулить (сбросить) позицию ползунка во входном потоке <paramref name="SourceStream"/> 
        /// перед началом чтения из него. 
        /// Если true - позиция обнуляется и поток читается с начала, однако в этом случае поток должен поддерживать операцию поиска (перемещения ползунка). 
        /// Если false - поток читается с текущей позиции, и поддержка операции поиска не требуется.</param>
        public static void SaveToFile(this Stream SourceStream, String FilePath, Boolean ResetSourcePosition)
        {
            if (SourceStream == null) { throw new ArgumentNullException("SourceStream"); }
            if (SourceStream.CanRead == false) { throw new ArgumentException("Входной поток не поддерживает чтение", "SourceStream"); }
            if (FilePath == null) { throw new ArgumentNullException("FilePath"); }
            if (FilePath.HasAlphaNumericChars() == false) { throw new ArgumentException("Путь = '" + FilePath + "' некорректен", "FilePath"); }

            using (FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fs.Seek(0, SeekOrigin.Begin);
                StreamTools.CopyStream(SourceStream, fs, ResetSourcePosition, false);
            }
        }
    }
}

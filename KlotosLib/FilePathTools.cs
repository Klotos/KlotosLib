using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using KlotosLib.StringTools;

namespace KlotosLib
{
    /// <summary>
    /// Содержит статические методы по работе с именами файлов и путями
    /// </summary>
    public static class FilePathTools
    {
        #region Constants
        //http://blogs.msdn.com/b/bclteam/archive/2007/02/13/long-paths-in-net-part-1-of-3-kim-hamilton.aspx

        /// <summary>
        /// Максимально допустимая длина полного пути
        /// </summary>
        public const Int32 MAX_PATH = 260;

        /// <summary>
        /// Максимально допустимая длина имени файла
        /// </summary>
        public const Int32 MAX_FILENAME = 255;

        /// <summary>
        /// Максимально допустимая длина имени директории
        /// </summary>
        public const Int32 MAX_DIRECTORYNAME = 248;
        #endregion

        /// <summary>
        /// Удаляет расширение из имени файла, если находит точку и перед ней есть какие-нибудь символы
        /// </summary>
        /// <param name="Filename">Имя файла</param>
        /// <returns>Входная строка без расширения имени файла</returns>
        public static String DeleteExtension(String Filename)
        {
            if (Filename.IsStringNullEmptyWhiteSpace() == true) { return Filename; }
            int index_of_dot = Filename.LastIndexOf('.');
            if (index_of_dot > 0)
            { return Filename.Remove(index_of_dot); }
            else
            { return Filename; }
        }

        /// <summary>
        /// Список строк, которые недопустимы в качестве имён файлов
        /// </summary>
        public static readonly String[] IllegalFilenames = new String[] 
        { "CON", "PRN", "AUX", "NUL", 
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", 
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

        /// <summary>
        /// Определяет, является ли указанное название файла валидным
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static Boolean IsValidFilename(String Input)
        {
            if (Input.HasVisibleChars() == false) { return false; }
            if (Input.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) { return false; }
            if (Input.Length > MAX_FILENAME) { return false; }
            if (Input.EndsWith(" ", StringComparison.InvariantCultureIgnoreCase) == true) { return false; }
            String part = Input.Trim().Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            if (part.IsIn(StringComparison.OrdinalIgnoreCase, FilePathTools.IllegalFilenames) == true) { return false; }
            return true;
        }

        /// <summary>
        /// Определяет, является ли указанный путь валидным. Поддерживает абсолютные и относительные пути, с названиями файлов и без, а также названия файлов без путей
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static Boolean IsValidFilePath(String Input)
        {
            if (Input.HasVisibleChars() == false) { return false; }
            if (Input.IndexOfAny(Path.GetInvalidPathChars()) >= 0) { return false; }
            if(Input.Length >= MAX_PATH) {return false; }
            String[] parts = Input.Split(new char[1] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            for (Int32 i = 0; i < parts.Length - 1; i++)
            {
                String current = parts[i];
                if (i < parts.Length - 1)
                {
                    if(current.Length >= MAX_DIRECTORYNAME) {return false; }
                }
                if (current.HasVisibleChars() == false) { return false; }
                String first_part = current.Trim().Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
                if (first_part.Trim().IsIn(StringComparison.OrdinalIgnoreCase, IllegalFilenames) == true) { return false; }
            }
            return FilePathTools.IsValidFilename(parts[parts.Length - 1]);
        }

        /// <summary>
        /// Пытается очистить указанную строку, представляющую название файла (без пути), от недопустимых символов, если таковые присутствуют. 
        /// Если очистка успешна, возвращает 'true' и очищенное коректное имя файла через выводной параметр. 
        /// Если же указанную строку невозможно очистить, возвращает 'false' и NULL через выводной параметр.
        /// </summary>
        /// <param name="Input">Строка, представляющая имя файла без пути</param>
        /// <param name="FixedFilename">Выводной параметр, содержащий очищенное имя файла, если его удалось очистить</param>
        /// <returns></returns>
        public static Boolean TryCleanFilename(String Input, out String FixedFilename)
        {
            FixedFilename = null;
            if (Input.HasVisibleChars() == false) { return false; }
            StringBuilder sb = new StringBuilder(Input.Length);
            Char[] invalid_chars = Path.GetInvalidFileNameChars();
            foreach (Char current in Input)
            {
                if (current.IsIn(invalid_chars) == false)
                {
                    sb.Append(current);
                }
            }
            String output = sb.Trim().ToString();
            if (output.HasVisibleChars() == false) { return false; }
            
            String[] all_segments = output.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            String first_segment = all_segments.First();
            if (first_segment.IsIn(StringComparison.OrdinalIgnoreCase, FilePathTools.IllegalFilenames) == true)
            {
                if (all_segments.Length < 3) { return false; }
                else
                {
                    output = output.TrimStart(first_segment + ".", StringComparison.OrdinalIgnoreCase, false);
                }
            }
            FixedFilename = output;
            return true;
        }
    }
}

#define Debug

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Encoder = System.Drawing.Imaging.Encoder;
using System.ComponentModel;
using KlotosLib.StringTools;

namespace KlotosLib
{
    /// <summary>
    /// Содержит инструменты по работе с графическими изображениями
    /// </summary>
    public static class ImageTools
    {
        /// <summary>
        /// Битовое перечесление с допустимыми форматами изображений, содержащее стартовый флаг неизвестного формата
        /// </summary>
        [Flags]
        public enum ImageFormats
        {
            /// <summary>
            /// Формат изображения неизвестен или не указан
            /// </summary>
            None = 0x00,

            /// <summary>
            /// JPEG = 1
            /// </summary>
            JPEG = 0x01,

            /// <summary>
            /// PNG = 2
            /// </summary>
            PNG = 0x02,

            /// <summary>
            /// GIF = 4
            /// </summary>
            GIF = 0x04,

            /// <summary>
            /// TIFF = 8
            /// </summary>
            TIFF = 0x08,

            /// <summary>
            /// BMP = 16
            /// </summary>
            BMP = 0x10,

            /// <summary>
            /// ICO = 32
            /// </summary>
            ICO = 0x20
        }

        /// <summary>
        /// Определяет, находится ли единичное значение Single внутри комбинированного значения Combined
        /// </summary>
        /// <param name="Single"></param>
        /// <param name="Combined"></param>
        /// <returns></returns>
        public static Boolean IsIn(this ImageFormats Single, ImageFormats Combined)
        {
            return ((Combined & Single) == Single);
        }

        /// <summary>
        /// Определяет, содержит ли комбинированное значение Combined внутри себя единичное значения Single
        /// </summary>
        /// <param name="Combined"></param>
        /// <param name="Single"></param>
        /// <returns></returns>
        public static Boolean Contains(this ImageFormats Combined, ImageFormats Single)
        {
            return ((Combined & Single) == Single);
        }

        /// <summary>
        /// Вычисляет размер (ширина и высота), к которым необходимо привести изображение с указанным исходным размером таким образом,
        /// чтобы полученные размеры не превышали целевые размеры и чтобы сохранялось соотношение сторон
        /// </summary>
        /// <param name="SourceSize">Исходный размер изображения, которое надо изменить</param>
        /// <param name="TargetSize">Целевой размер изображения, по которому необходимо ограничить выдачу</param>
        /// <returns></returns>
        public static Size CalculateNewSizeWithAspectRatio(Size SourceSize, Size TargetSize)
        {
            int sourceWidth = SourceSize.Width;
            int sourceHeight = SourceSize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)TargetSize.Width / (float)sourceWidth);
            nPercentH = ((float)TargetSize.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
            return new Size(destWidth, destHeight);
        }

        /// <summary>
        /// Вычисляет размер (ширина и высота), к которым необходимо привести изображение с указанным исходным размером таким образом,
        /// чтобы высота полученного размера равнялась целевой высоте, а ширина подстраивалась автоматически с сохранением соотношения сторон. 
        /// Метод работает в обе стороны - если исходная высота была меньше целевой, выходные размеры будут увеличены.
        /// </summary>
        /// <param name="SourceSize">Исходный размер изображения, которое надо изменить</param>
        /// <param name="TargetHeight">Целевая высота, которая должна быть в выходном размере</param>
        /// <returns>Новый размер</returns>
        public static Size CalculateNewSizeWithAspectRatio(Size SourceSize, int TargetHeight)
        {
            float scale_factor = (float)TargetHeight / (float)SourceSize.Height;
            float new_width = SourceSize.Width * scale_factor;
            Int32 new_width_int = Convert.ToInt32(new_width.Rounding(0));
            return new Size(new_width_int, TargetHeight);
        }

        /// <summary>
        /// Возвращает уменьшенную версию указанного изображения, сконвертированную в PNG для поддержки прозрачности и уменьшенную до указанной высоты с сохранением соотношения сторон. 
        /// Если высота исходного изображения уже меньше желаемой высоты, метод возвращает NULL.
        /// </summary>
        /// <param name="SourceImage">Исходное изображение, которое необходимо уменьшить к указанной высоте. В любом случае не затрагивается.</param>
        /// <param name="DesiredHeight">Высота, к которой необходимо уменьшить исходное изображение</param>
        /// <returns>Новое изображение в формате PNG с наилучшими настройками качества или NULL</returns>
        public static Byte[] ReduceImageToPNG(Image SourceImage, UInt16 DesiredHeight)
        {
            Size source_size = SourceImage.Size;
            if (source_size.Height <= DesiredHeight) { return null; }

            Size target_size = ImageTools.CalculateNewSizeWithAspectRatio(source_size, DesiredHeight);

            Byte[] result;
            using (Bitmap destination = new Bitmap(target_size.Width, target_size.Height))
            {
                using (Bitmap source = new Bitmap(SourceImage))
                {
                    using (Graphics g = Graphics.FromImage(destination))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(source, 0, 0, target_size.Width, DesiredHeight);
                    }
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    destination.Save(ms, ImageFormat.Png);
                    result = ms.ToArray();
                }
            }
            return result;
        }

        /// <summary>
        /// Проводит изменение размеров изображения (ресайзинг), увеличивая или уменьшая его и сохраняя соотношение сторон, а также прозрачность. 
        /// Возвращает новое изображение в формате PNG в виде массива байтов.
        /// </summary>
        /// <param name="SourceImage">Исходное изображение, которое необходимо подстроить под указанный размер</param>
        /// <param name="DesiredSize">Размеры, до которых следует привести исходное изображение</param>
        /// <returns></returns>
        public static Byte[] ResizeImageToPNG(Image SourceImage, Size DesiredSize)
        {
            Size dest_size = ImageTools.CalculateNewSizeWithAspectRatio(SourceImage.Size, DesiredSize);

            Byte[] result;
            using (Bitmap destination = new Bitmap(dest_size.Width, dest_size.Height))
            {
                using (Bitmap source = new Bitmap(SourceImage))
                {
                    using (Graphics g = Graphics.FromImage(destination))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(source, 0, 0, dest_size.Width, dest_size.Height);
                    }
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    destination.Save(ms, ImageFormat.Png);
                    result = ms.ToArray();
                }
            }
            return result;
        }

        /// <summary>
        /// Изменяет размеры указанного изображения на указанные, делая 'жесткий' ресайзинг и не сохраняя соотношение сторон. 
        /// Сохраняет отресайзенное изображение в JPEG с настройкой качества 90%.
        /// </summary>
        /// <param name="SourceImage">Исходное изображение в виде байтового массива. Если массив некорректен или не может быть преобразован в изображение, метод выбрасывает исключение.</param>
        /// <param name="NewSize"></param>
        /// <returns></returns>
        public static Byte[] ResizeImageToJPEG(Byte[] SourceImage, Size NewSize)
        {
            if (SourceImage.IsNullOrEmpty() == true) { throw new ArgumentException("Входной массив NULL или пуст", "SourceImage"); }
            if (NewSize.Width == 0 || NewSize.Height == 0) { throw new ArgumentOutOfRangeException("NewSize", "Ни одна из размерностей нового размера не может быть равна 0"); }
            Bitmap bm = ImageTools.ConvertDataToBitmap(SourceImage);
            if (bm == null) { throw new ArgumentException("Невозможно преобразовать входной байтовый массив в битовую карту", "SourceImage"); }

            ImageCodecInfo myImageCodecInfo = ImageTools.GetEncoderInfo("image/jpeg");
            Encoder myEncoder = Encoder.Quality;
            using (EncoderParameters myEncoderParameters = new EncoderParameters(1))
            {
                using (EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 90L))
                {
                    myEncoderParameters.Param[0] = myEncoderParameter;

                    Byte[] result;
                    using (Bitmap destination = new Bitmap(NewSize.Width, NewSize.Height))
                    {
                        using (Graphics g = Graphics.FromImage(destination))
                        {
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.DrawImage(bm, 0, 0, NewSize.Width, NewSize.Height);
                        }
                        bm.Dispose();
                        using (MemoryStream ms = new MemoryStream())
                        {
                            destination.Save(ms, myImageCodecInfo, myEncoderParameters);
                            result = ms.ToArray();
                        }
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Проводит изменение размеров изображения (ресайзинг), увеличивая или уменьшая его и сохраняя соотношение сторон
        /// </summary>
        /// <param name="ImageToResize">Изображение, размеры которого необходимо изменить. Метод не затрагивает это изображение, а создаёт его иззменённую копию.</param>
        /// <param name="DesiredSize">Размеры, до которых следует привести исходное изображение</param>
        /// <returns>Новое точечное изображение, содержащее изменённое исходное изображение</returns>
        public static Bitmap ResizeImageToBitmap(Image ImageToResize, Size DesiredSize)
        {
            Size dest_size = ImageTools.CalculateNewSizeWithAspectRatio(ImageToResize.Size, DesiredSize);

            int destWidth = dest_size.Width;
            int destHeight = dest_size.Height;

            Bitmap b = new Bitmap(destWidth, destHeight);
            using (Graphics g = Graphics.FromImage((Image) b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                g.DrawImage(ImageToResize, 0, 0, destWidth, destHeight);
            }
            return b;
        }

        /// <summary>
        /// Сравнивает размеры указанного изобажения с указанными желаемыми размерами, 
        /// и если фактические размеры превышают желаемые, метод возвращает новое изображение, 
        /// содержащее уменьшенную до желаемых размеров копию исходного изображения без искажений соотношения сторон
        /// </summary>
        /// <param name="ImageToReduce">Оригинальное изображение, которое не затрагивается</param>
        /// <param name="DesiredSize">Желаемый размер, к которому необходимо уменьшить исходное изображение. 
        /// Если исходное изобажение имеет меньший за указанный размер, возвращаеся его копия без изменения размеров.</param>
        /// <returns>В любом случае возвращается новое изображение, а не ссылка на старое</returns>
        public static Bitmap ReduceImageToBitmap(Image ImageToReduce, Size DesiredSize)
        {
            if (ImageToReduce == null) { throw new ArgumentNullException("ImageToReduce"); }
            Int32 source_width = ImageToReduce.Width;
            Int32 source_height = ImageToReduce.Height;
            if (source_width > DesiredSize.Width || source_height > DesiredSize.Height)
            {
                return ImageTools.ResizeImageToBitmap(ImageToReduce, DesiredSize);
            }
            else
            {
                return new Bitmap(ImageToReduce);
            }
        }

        /// <summary>
        /// Анализирует входящее изображение на предмер линейных размеров и формата и возвращает его в виде масива байтов. 
        /// Если формат изображения отсутствует в списке разрешенных форматов, метод конвертирует его в PNG (для сохранения прозрачности) и в пределах указанного размера. 
        /// Если формат изображения допустимый, метод проверяет его линейные размеры, и если они превышают указанный размер, 
        /// то изображение конвертируется в PNG (для сохранения прозрачности) с указанным размером. 
        /// Если исходный формат изображения - JPEG, но оно превышает допустимые размеры, то метод конвертирует его в JPEG, а не PNG, так как в исходном изображении не может быть прозрачности. 
        /// Если изображение находится в допустимом формате и с допустимыми размерами, то оно не конвертируется, а лишь преобразовывается в байтовый массив, 
        /// и в выводном параметре содержится 'false'.
        /// </summary>
        /// <param name="SourceImage">Исходное изображение, которое необходимо проанализировать на соответствие указанным критериям.</param>
        /// <param name="DesiredSize">Желаемые линейные размеры, которые не могут быть превышены</param>
        /// <param name="AcceptedFormats">Список допустимых форматов изображения в виде битового перечисления.</param>
        /// <param name="WasConverted">Если 'true' - метод провёл конвертацию изображения и преобразовал его в PNG или в JPEG. 
        /// Если 'false' - метод не проводит конвертацию и возвратил нетронутое входящее изображение.</param>
        /// <returns>Байтовый массив, в который записывается новое изображение или неизменённое старое. В любом случае исходное изображение не изменяется.</returns>
        public static Byte[] ConvertAndReduceImageIfNeeded(Bitmap SourceImage, Size DesiredSize, ImageFormats AcceptedFormats, out Boolean WasConverted)
        {
            if (SourceImage == null) { throw new ArgumentNullException("SourceImage"); }
            if (DesiredSize.Width == 0 || DesiredSize.Height == 0) { throw new ArgumentOutOfRangeException("DesiredSize"); }

            Size source_size = SourceImage.Size;

            ImageFormats source_format = ImageTools.GetImageFormat(SourceImage);
            if (source_format == ImageFormats.None)
            {
                throw new ArgumentException("Формат изображения некорректный", "SourceImage");
            }

            Boolean accepted_format = IsIn(source_format, AcceptedFormats);//если true - формат подходит и его не нужно изменять
            if (accepted_format == true && source_size.Width <= DesiredSize.Width && source_size.Height <= DesiredSize.Height)
            {
                WasConverted = false;
                return ImageTools.ConvertBitmapToByteArrayJPEG(SourceImage);
            }
            WasConverted = true;
            if (accepted_format == true && source_format == ImageFormats.JPEG)
            {
                using (Bitmap b = ImageTools.ReduceImageToBitmap(SourceImage, DesiredSize))
                {
                    using (MemoryStream ms = ImageTools.ConvertImageToStreamJPEG(b))
                    {
                        return ms.ToArray();
                    }
                }
            }

            Byte[] converted_bitmap = ImageTools.ReduceImageToPNG(SourceImage, (UInt16)DesiredSize.Height);
            if (converted_bitmap == null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    SourceImage.Save(ms, ImageFormat.Png);
                    converted_bitmap = ms.ToArray();
                }
            }
            return converted_bitmap;
        }

        /// <summary>
        /// Анализирует входящее изображение на предмер линейных размеров и формата и возвращает его в виде байтового масива. При необходимости и в зависимости от указанных параметров 
        /// метод конвертирует изображение в PNG и/или уменьшает его линейные размеры.
        /// </summary>
        /// <param name="SourceImage">Исходное изображение в виде битовой карты. Если NULL - будет выброшено исключение.</param>
        /// <param name="DesiredSize">Желаемый размер, ни одна из величин которого не может быть превышена</param>
        /// <param name="AcceptedFormats">Список допустимых форматов для входного изображения в виде битового перечисления. 
        /// Если формат входного изображения попадает в данный набор, то оно в нём и будет сохранено. Если же не попадает, то будет сохранено в PNG.</param>
        /// <param name="WasConverted">Был ли изменён формат изображения</param>
        /// <param name="WasResized">Были ли изменены линейные размеры изображения</param>
        /// <param name="NewSize">Новые линейные размеры изображения, могут совпадать со старыми, если ресайзинг не проводился</param>
        /// <param name="NewFormat">Новый формат, может совпадать со старым, если конвертация в другой формат не проводилась</param>
        /// <returns></returns>
        public static Byte[] ConvertAndReduceImageIfNeeded(Bitmap SourceImage, Size DesiredSize, ImageFormats AcceptedFormats,
            out Boolean WasConverted, out Boolean WasResized, out Size NewSize, out ImageTools.ImageFormats NewFormat)
        {
            if (SourceImage == null) { throw new ArgumentNullException("SourceImage"); }
            if (DesiredSize.Width < 1 || DesiredSize.Height < 1) { throw new ArgumentOutOfRangeException("DesiredSize"); }

            if (SourceImage.Size.Width > DesiredSize.Width || SourceImage.Size.Height > DesiredSize.Height)
            {
                WasResized = true;
                NewSize = ImageTools.CalculateNewSizeWithAspectRatio(SourceImage.Size, DesiredSize);
            }
            else
            {
                WasResized = false;
                NewSize = SourceImage.Size;
            }

            ImageTools.ImageFormats source_format = ImageTools.GetImageFormat(SourceImage);
            WasConverted = !source_format.IsIn(AcceptedFormats);

            if (WasConverted == true)
            {
                NewFormat = ImageFormats.PNG;
            }
            else
            {
                NewFormat = source_format;
            }

            Bitmap output = SourceImage;
            if (WasResized == true)
            {
                output = ImageTools.ReduceImageToBitmap(SourceImage, NewSize);
            }
            return output.SaveToByteArray(NewFormat);
        }

        /// <summary>
        /// Выполняет обрезку указанного изображения по указанным размерам, возвращая обрезанную версию в виде нового изображения и не затрагивая старое
        /// </summary>
        /// <param name="Source">Исходное изображение, которое следует обрезать. Не затрагивается методом. Если NULL, выбрасывается исключение.</param>
        /// <param name="CropArea">Область обрезки. Если хотя бы один из размеров является 0, выбрасывается исключение. Координаты при этом могут быть нулевыми.</param>
        /// <returns></returns>
        public static Bitmap CropImage(Bitmap Source, Rectangle CropArea)
        {
            if (Source == null) { throw new ArgumentNullException("Source", "Невозможно выполнить обрезку изображения, которое является NULL"); }
            if (CropArea.IsEmpty == true || CropArea.Width == 0 || CropArea.Height == 0) { throw new ArgumentOutOfRangeException("CropArea", "Область обрезки не может быть пустой"); }

            Bitmap output = Source.Clone(CropArea, Source.PixelFormat);
            return output;
        }

        /// <summary>
        /// Определяется, является ли входной поток корректным изображением, с котором может работать .NET
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static Boolean IsValidImage(Stream Source)
        {
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(Source);
                img = null;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Возвращает MIME-тип для данного формата изображения. Если значение содержит множество форматов, ни одного или значение некорректно, возвращает NULL.
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public static String GetMime(this ImageTools.ImageFormats Format)
        {
            switch (Format)
            {
                case ImageFormats.JPEG:
                    return "image/jpeg";
                case ImageFormats.PNG:
                    return "image/png";
                case ImageFormats.BMP:
                    return "image/x-bmp";
                case ImageFormats.GIF:
                    return "image/gif";
                case ImageFormats.TIFF:
                    return "image/tiff";
                case ImageFormats.ICO:
                    return "image/vnd.microsoft.icon";
                case ImageFormats.None:
                    return null;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Возвращает расширение имени файла для данного формата без точки. Если значение содержит множество форматов, ни одного или значение некорректно, возвращает NULL.
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public static String GetFileExtension(this ImageTools.ImageFormats Format)
        {
            switch (Format)
            {
                case ImageFormats.JPEG:
                    return "jpg";
                case ImageFormats.PNG:
                    return "png";
                case ImageFormats.BMP:
                    return "bmp";
                case ImageFormats.GIF:
                    return "gif";
                case ImageFormats.TIFF:
                    return "tif";
                case ImageFormats.ICO:
                    return "ico";
                case ImageFormats.None:
                    return null;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Преобразовывает внешний формат в экземпляр класса ImageFormat в случае успеха или в NULL в случае провала
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public static System.Drawing.Imaging.ImageFormat ToInternalFormat(this ImageTools.ImageFormats Format)
        {
            switch (Format)
            {
                case ImageFormats.JPEG:
                    return ImageFormat.Jpeg;
                case ImageFormats.PNG:
                    return ImageFormat.Png;
                case ImageFormats.GIF:
                    return ImageFormat.Gif;
                case ImageFormats.TIFF:
                    return ImageFormat.Tiff;
                case ImageFormats.BMP:
                    return ImageFormat.Bmp;
                case ImageFormats.ICO:
                    return ImageFormat.Icon;
                case ImageFormats.None:
                    return null;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Преобразовывает внутренний формат в ивде экземпляра класса ImageFormat во внешний в виде перечисления. В случае провала возвращает значение None.
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public static ImageTools.ImageFormats ToEnumFormat(this System.Drawing.Imaging.ImageFormat Format)
        {
            if (Format == null) { return ImageFormats.None; }
            if (Format.Guid == ImageFormat.Jpeg.Guid)
            {
                return ImageFormats.JPEG;
            }
            if (Format.Guid == ImageFormat.Png.Guid)
            {
                return ImageFormats.PNG;
            }
            if (Format.Guid == ImageFormat.Gif.Guid)
            {
                return ImageFormats.GIF;
            }
            if (Format.Guid == ImageFormat.Tiff.Guid)
            {
                return ImageFormats.TIFF;
            }
            if (Format.Guid == ImageFormat.Bmp.Guid)
            {
                return ImageFormats.BMP;
            }
            if (Format.Guid == ImageFormat.Icon.Guid)
            {
                return ImageFormats.ICO;
            }
            return ImageFormats.None;
        }

        /// <summary>
        /// Определяет формат входного изображения и возвращает его в виде перечисления. Поддерживаются JPEG, PNG, BMP, GIF, TIFF, ICO. 
        /// Если входное изображение не принадлежит ни к одному из этих форматов, возвращается None.
        /// </summary>
        /// <param name="SourceImage">Входное изображение, чей формат нужно определить. Если NULL, метод возвращает None.</param>
        /// <returns>Лишь и только одно значение из перечисления ImageFormats</returns>
        public static ImageFormats GetImageFormat(System.Drawing.Image SourceImage)
        {
            if (SourceImage == null)
            {
                return ImageFormats.None;
            }

            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Jpeg.Guid)
            {
                return ImageFormats.JPEG;
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Png.Guid)
            {
                return ImageFormats.PNG;
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Bmp.Guid)
            {
                return ImageFormats.BMP;
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Gif.Guid)
            {
                return ImageFormats.GIF;
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Tiff.Guid)
            {
                return ImageFormats.TIFF;
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Icon.Guid)
            {
                return ImageFormats.ICO;
            }
            return ImageFormats.None;
        }

        /// <summary>
        /// Определяет формат входного изображения и возвращает его, а также его MIME-тип в виде строки. 
        /// Поддерживаются JPEG, PNG, BMP, GIF, TIFF, ICO. 
        /// Если входное изображение не принадлежит ни к одному из этих форматов, оба выводных параметра содержат NULL.
        /// </summary>
        /// <param name="SourceImage">Входное изображение, формат которого следует определить</param>
        /// <param name="MimeType">Выводной параметр: MIME-тип поданного на вход изображения</param>
        /// <returns></returns>
        public static System.Drawing.Imaging.ImageFormat GetImageFormatAndMIME(System.Drawing.Image SourceImage, out String MimeType)
        {
            if (SourceImage == null) { throw new ArgumentNullException("SourceImage"); }

            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Jpeg.Guid)
            {
                MimeType = "image/jpeg";
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Png.Guid)
            {
                MimeType = "image/png";
                return System.Drawing.Imaging.ImageFormat.Png;
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Bmp.Guid)
            {
                MimeType = "image/x-bmp";
                return System.Drawing.Imaging.ImageFormat.Bmp;
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Gif.Guid)
            {
                MimeType = "image/gif";
                return System.Drawing.Imaging.ImageFormat.Gif;
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Tiff.Guid)
            {
                MimeType = "image/tiff";
                return System.Drawing.Imaging.ImageFormat.Tiff;
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Icon.Guid)
            {
                MimeType = "image/vnd.microsoft.icon";
                return System.Drawing.Imaging.ImageFormat.Icon;
            }
            MimeType = null;
            return null;
        }

        /// <summary>
        /// Возвращает MIME-токен указанного изображения в виде строки или NULL, если изображение NULL или его формат не может быть определён
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns></returns>
        public static String GetImageMime(Image SourceImage)
        {
            if (SourceImage == null) { return null; }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Jpeg.Guid)
            {
                return "image/jpeg";
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Png.Guid)
            {
                return "image/png";
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Bmp.Guid)
            {
                return "image/x-bmp";
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Gif.Guid)
            {
                return "image/gif";
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Tiff.Guid)
            {
                return "image/tiff";
            }
            if (SourceImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Icon.Guid)
            {
                return "image/vnd.microsoft.icon";
            }
            return null;
        }

        /// <summary>
        /// Возвращает MIME-токен указанного формата изображения в виде строки или NULL, если формат = None
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public static String GetImageMime(ImageTools.ImageFormats Format)
        {
            if (Enum.IsDefined(Format.GetType(), Format) == false)
            { throw new InvalidEnumArgumentException("Format", (Int32)Format, Format.GetType()); }

            switch (Format)
            {
                case ImageFormats.None:
                    return null;
                case ImageFormats.JPEG:
                    return "image/jpeg";
                case ImageFormats.PNG:
                    return "image/png";
                case ImageFormats.BMP:
                    return "image/x-bmp";
                case ImageFormats.GIF:
                    return "image/gif";
                case ImageFormats.TIFF:
                    return "image/tiff";
                case ImageFormats.ICO:
                    return "image/vnd.microsoft.icon";
                default:
                    throw new UnreachableCodeException();
            }
        }

        /// <summary>
        /// Конвертирует входной поток в точечный рисунок, возвращая NULL, если входной поток не может быть сконвертирован в рисунок
        /// </summary>
        /// <param name="InputStream">Входной поток, в котором, по идее, должно находится изображение</param>
        /// <returns></returns>
        public static Bitmap ConvertDataToBitmap(Stream InputStream)
        {
            if (InputStream == null) { throw new ArgumentNullException("InputStream"); }
            if (InputStream.Length < 1) { throw new ArgumentException("Входной поток пуст", "InputStream"); }

            try
            {
                Bitmap output = new Bitmap(InputStream);
                return output;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Конвертирует входной массив байтов в точечный рисунок, возвращая NULL, если входной массив не может быть сконвертирован в рисунок
        /// </summary>
        /// <param name="ByteArray"></param>
        /// <returns></returns>
        public static Bitmap ConvertDataToBitmap(Byte[] ByteArray)
        {
            if (ByteArray == null) { throw new ArgumentNullException("ByteArray"); }
            if (ByteArray.LongLength < 1) { throw new ArgumentException("Входной массив пуст", "ByteArray"); }

            using (MemoryStream memory_stream = new MemoryStream(ByteArray))
            {
                return ImageTools.ConvertDataToBitmap(memory_stream);
            }

        }

        /// <summary>
        /// Конвертирует файл с указанным полным путём в точечный рисунок, возвращая NULL, если входной файл не может быть сконвертирован в рисунок, или если он не найден по указанному пути
        /// </summary>
        /// <param name="FilePath">Полный путь до файла</param>
        /// <returns></returns>
        public static Bitmap ConvertDataToBitmap(String FilePath)
        {
            if (FilePath.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Строка с путем не может быть NULL, пустой или состоять из одних пробелов", "FilePath"); }

            try
            {
                Bitmap output = new Bitmap(FilePath);
                return output;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Конвертирует изображение в формат JPEG с настройкой качества в 90% и возвращает его в виде потока
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <returns></returns>
        public static MemoryStream ConvertImageToStreamJPEG(Image SourceImage)
        {
            if (SourceImage == null) { throw new ArgumentNullException("SourceImage"); }

            MemoryStream output = new MemoryStream();

            ImageCodecInfo myImageCodecInfo = ImageTools.GetEncoderInfo("image/jpeg");

            Encoder myEncoder = Encoder.Quality;

            using (EncoderParameters myEncoderParameters = new EncoderParameters(1))
            {
                using (EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 90L))
                {
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    SourceImage.Save(output, myImageCodecInfo, myEncoderParameters);
                    return output;
                }
            }
        }

        /// <summary>
        /// Конвертирует точечное изображение в массив байтов, сохраняя его в формате JPEG с настройками по-умолчанию
        /// </summary>
        /// <param name="SourceBitmap"></param>
        /// <returns></returns>
        public static Byte[] ConvertBitmapToByteArrayJPEG(Bitmap SourceBitmap)
        {
            if (SourceBitmap == null) { throw new ArgumentNullException("SourceBitmap", "Невозможно сконвертировать в JPEG битовую карту, которая является NULL"); }

            byte[] bmpBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                SourceBitmap.Save(ms, ImageFormat.Jpeg);

                bmpBytes = ms.GetBuffer();
                SourceBitmap.Dispose();
            }
            return bmpBytes;
        }

        /// <summary>
        /// Конвертирует точечное изображение в формат PNG с настройкой качества в 90% и возвращает его в байтового массива
        /// </summary>
        /// <param name="SourceBitmap"></param>
        /// <returns></returns>
        public static Byte[] ConvertBitmapToByteArrayPNG(Bitmap SourceBitmap)
        {
            if (SourceBitmap == null) { throw new ArgumentNullException("SourceBitmap", "Невозможно сконвертировать в PNG битовую карту, которая является NULL"); }

            ImageCodecInfo myImageCodecInfo = ImageTools.GetEncoderInfo("image/png");

            Encoder myEncoder = Encoder.Quality;

            using (EncoderParameters myEncoderParameters = new EncoderParameters(1))
            {
                using (EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 90L))
                {
                    myEncoderParameters.Param[0] = myEncoderParameter;

                    Byte[] result;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        SourceBitmap.Save(ms, myImageCodecInfo, myEncoderParameters);
                        result = ms.ToArray();
                    }
                    return result;
                }
            }

        }

        /// <summary>
        /// Сохраняет изображение в байтовый массив в тот формат, в котором оно было изначально и с настройками качества в 90%. 
        /// Если исходный формат изображения неизвестен, сохраняет его в PNG.
        /// </summary>
        /// <param name="SourceBitmap"></param>
        /// <returns></returns>
        public static Byte[] SaveToByteArray(this Bitmap SourceBitmap)
        {
            if (SourceBitmap == null) { throw new ArgumentNullException("SourceBitmap"); }

            String mime;
            ImageFormat format = ImageTools.GetImageFormatAndMIME(SourceBitmap, out mime);
            if (format == null || mime == null)
            {//неизвестно
                Byte[] output = ImageTools.ConvertBitmapToByteArrayPNG(SourceBitmap);
                return output;
            }

            ImageCodecInfo myImageCodecInfo = ImageTools.GetEncoderInfo(mime);
            Encoder myEncoder = Encoder.Quality;
            using (EncoderParameters myEncoderParameters = new EncoderParameters(1))
            {
                using (EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 90L))
                {
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    Byte[] result;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        SourceBitmap.Save(ms, myImageCodecInfo, myEncoderParameters);
                        result = ms.ToArray();
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// Сохраняет изображение в байтовый массив в указанный формат и с настройками качества в 90%. Если указанный формат некорректен, выбрасывает исключение.
        /// </summary>
        /// <param name="SourceBitmap"></param>
        /// <param name="SaveFormat"></param>
        /// <returns></returns>
        public static Byte[] SaveToByteArray(this Bitmap SourceBitmap, ImageTools.ImageFormats SaveFormat)
        {
            if (SourceBitmap == null) { throw new ArgumentNullException("SourceBitmap"); }
            ImageFormat int_format = SaveFormat.ToInternalFormat();
            if (int_format == null)
            { throw new InvalidEnumArgumentException("SaveFormat", (Int32)SaveFormat, SaveFormat.GetType()); }

            Byte[] result;
            ImageCodecInfo myImageCodecInfo = ImageTools.GetEncoderInfo(int_format);
            if (myImageCodecInfo == null) { throw new InvalidOperationException("Невозможно определить кодек по формату " + int_format.ToString() + " (" + SaveFormat + ")"); }
            Encoder myEncoder = Encoder.Quality;
            using (EncoderParameters myEncoderParameters = new EncoderParameters(1))
            {
                using (EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 90L))
                {
                    myEncoderParameters.Param[0] = myEncoderParameter;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        SourceBitmap.Save(ms, myImageCodecInfo, myEncoderParameters);
                        result = ms.ToArray();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Возвращает объект ImageCodecInfo на основании входного MIME или NULL, если для указанного MIME не найдено соответствия
        /// </summary>
        /// <param name="MimeType"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetEncoderInfo(String MimeType)
        {
            if (MimeType.IsStringNullEmptyWhiteSpace() == true) { throw new ArgumentException("Строка с MIME-токеном не может быть NULL, пустой или состоять из одних пробелов", "MimeType"); }

            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (int j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == MimeType ||
                    (encoders[j].MimeType == "image/bmp" && MimeType == "image/x-bmp"))
                { return encoders[j]; }
            }
            return null;
        }

        /// <summary>
        /// Возвращает объект ImageCodecInfo на основании входного ImageFormat или NULL, если для указанного ImageFormat не найдено соответствия
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetEncoderInfo(ImageFormat Format)
        {
            if (Format == null) { throw new ArgumentNullException("Format"); }

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == Format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        #region Get Dimensions From Header

        /// <summary>
        /// Извлекает разрешение (ширину и высоту) из указанного хидера, взятого в изображения формата GIF
        /// </summary>
        /// <param name="Header">Хидер GIF-файла, содержащий первые 10 байт</param>
        /// <returns></returns>
        public static Size GetGifSizeFromHeader(Byte[] Header)
        {
            Header.ThrowIfNullOrEmpty();
            if (Header.Length < 10) { throw new ArgumentOutOfRangeException("Header", "Массив Header содержит " + Header.Length + "элемента, тогда как должен содержать не менее 10"); }
#if Debug
            string type = ((char)Header[0]).ToString();
            type += ((char)Header[1]).ToString();
            type += ((char)Header[2]).ToString();

            string version = ((char)Header[3]).ToString();
            version += ((char)Header[4]).ToString();
            version += ((char)Header[5]).ToString();
#endif
            int lower = Header[6];
            int upper = Header[7];
            int width = lower | upper << 8;

            lower = Header[8];
            upper = Header[9];
            int height = lower | upper << 8;

#if Debug
            Console.WriteLine("GIF\nType: " + type + "\nVersion: " + version + "\nWidth: " + width + "\nHeight: " + height);
#endif
            return new Size(width, height);
        }

        /// <summary>
        /// Извлекает разрешение (ширину и высоту) из указанного хидера, взятого в изображения формата PNG
        /// </summary>
        /// <param name="Header">Хидер PNG-файла, содержащий первые 24 байта</param>
        /// <returns></returns>
        public static Size GetPngSizeFromHeader(Byte[] Header)
        {
            Header.ThrowIfNullOrEmpty();
            if (Header.Length < 24) { throw new ArgumentOutOfRangeException("Header", "Массив Header содержит " + Header.Length + "элемента, тогда как должен содержать не менее 24"); }

            Int32 width = 0;

            width = width | Header[16];
            width = width << 8;
            width = width | Header[17];
            width = width << 8;
            width = width | Header[18];
            width = width << 8;
            width = width | Header[19];

            Int32 height = 0;

            height = height | Header[20];
            height = height << 8;
            height = height | Header[21];
            height = height << 8;
            height = height | Header[22];
            height = height << 8;
            height = height | Header[23];

            return new Size(width, height);
        }

        /// <summary>
        /// Извлекает разрешение (ширину и высоту) из указанного хидера, взятого в изображения формата BMP
        /// </summary>
        /// <param name="Header">Хидер BMP-файла, содержащий первые 26 байт</param>
        /// <returns></returns>
        public static System.Drawing.Size GetBmpSizeFromHeader(Byte[] Header)
        {
            Header.ThrowIfNullOrEmpty();
            Byte first = Header[0];
            Byte second = Header[1];
            if (first != 0x42 || second != 0x4d)
            {
                throw new Exception("Image is not BMP");
            }
            Int32 width = BitConverter.ToInt32(Header, 18);
            Int32 height = BitConverter.ToInt32(Header, 22);

            return new System.Drawing.Size(width, height);
        }
        #endregion

        #region Get image format from header
        /// <summary>
        /// Определяет формат изображения по его хидеру и возвращает формат в виде значения перечисления. Если формат определить не удалось, возвращает None. 
        /// Поддерживаются все форматы, доступные в перечислении: JPEG, PNG, BMP, TIFF, GIF, ICO.
        /// </summary>
        /// <remarks>
        /// Ресурсы:
        /// http://www.mikekunz.com/image_file_header.html
        /// http://www.libpng.org/pub/png/spec/1.1/PNG-Rationale.html#R.PNG-file-signature
        /// http://en.wikipedia.org/wiki/ICO_(file_format)
        /// </remarks>
        /// <param name="HeaderPart">Хидер файла - его первые 12 байт. Если содержит меньше 12 байт, выбрасывается исключение.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static ImageTools.ImageFormats GetImageFormat(Byte[] HeaderPart)
        {
            if (HeaderPart.IsNullOrEmpty() == true) { throw new ArgumentException("Массив байт не может быть NULL или пустым", "HeaderPart"); }
            if (HeaderPart.Length < 12) { throw new ArgumentException("Хидер должен содержать по крайней мере первые 12 байтов файла", "HeaderPart"); }

            //GIF
            if (HeaderPart[0] == 0x47 && HeaderPart[1] == 0x49 && HeaderPart[2] == 0x46)
            { return ImageFormats.GIF; }
            //BMP
            if (HeaderPart[0] == 0x42 && HeaderPart[1] == 0x4d)
            { return ImageFormats.BMP; }
            //TIFF
            if (HeaderPart[0] == 0x49 && HeaderPart[1] == 0x49 && HeaderPart[2] == 0x2a)
            { return ImageFormats.TIFF; }
            //PNG
            if (HeaderPart[0] == 0x89 && HeaderPart[1] == 0x50 && HeaderPart[2] == 0x4e && HeaderPart[3] == 0x47 && HeaderPart[4] == 0x0d && HeaderPart[5] == 0x0a &&
                HeaderPart[6] == 0x1a && HeaderPart[7] == 0x0a)
            { return ImageFormats.PNG; }
            //JPEG
            if (HeaderPart[0] == 0xff && HeaderPart[1] == 0xd8 && HeaderPart[2] == 0xff && HeaderPart[3] == 0xe0 &&
                HeaderPart[6] == 0x4a && HeaderPart[7] == 0x46 && HeaderPart[8] == 0x49 && HeaderPart[9] == 0x46 && HeaderPart[10] == 0x00)
            { return ImageFormats.JPEG; }
            //ICO
            if (HeaderPart[0] == 0x00 && HeaderPart[1] == 0x00 && HeaderPart[2] == 0x01 && HeaderPart[3] == 0x00)
            { return ImageFormats.ICO; }
            return ImageFormats.None;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using KlotosLib.StringTools;

namespace KlotosLib
{
    /// <summary>
    /// Содержит методы-расширения по работе с HTTP-запросами и ответами
    /// </summary>
    public static class HttpTools
    {
        /// <summary>
        /// Возвращает список всех названий и размеров файлов, содержащихся в указанном запросе
        /// </summary>
        /// <param name="CurrentRequest"></param>
        /// <returns></returns>
        public static List<KeyValuePair<String, Int32>> CollectFileData(this HttpRequest CurrentRequest)
        {
            if (CurrentRequest == null) { throw new ArgumentNullException("CurrentRequest"); }

            List<KeyValuePair<String, Int32>> output = new List<KeyValuePair<String, Int32>>(CurrentRequest.Files.Count);
            for (Int32 i = 0; i < CurrentRequest.Files.Count; i++)
            {
                output.Add(new KeyValuePair<String, Int32>(CurrentRequest.Files[i].FileName, CurrentRequest.Files[i].ContentLength));
            }
            return output;
        }

        /// <summary>
        /// Возвращает все текстовые данные, извлечённые из формы запроса, в виде словаря ключей и значений
        /// </summary>
        /// <param name="CurrentRequest"></param>
        /// <returns></returns>
        public static Dictionary<String, String> CollectFormData(this HttpRequest CurrentRequest)
        {
            if (CurrentRequest == null) { throw new ArgumentNullException("CurrentRequest"); }

            String[] all_keys = CurrentRequest.Form.AllKeys;
            Dictionary<String, String> output = new Dictionary<string, string>(all_keys.Length);
            for (Int32 i = 0; i < all_keys.Length; i++)
            {
                String one_key = all_keys[i];
                if (output.ContainsKey(one_key) == true)
                {
                    output[one_key] = CurrentRequest.Form[one_key];
                }
                else
                {
                    output.Add(one_key, CurrentRequest.Form[one_key]);
                }
            }
            return output;
        }

        /// <summary>
        /// Возвращает все HTTP-хидеры из Web-ответа в виде словаря, где ключи - это названия хидеров, а значения - соотв. значения хидеров.
        /// </summary>
        /// <param name="SourceResponse">Экземпляр Web-ответа. Если NULL - будет выброшено исключение.</param>
        /// <returns></returns>
        public static Dictionary<String, String> GetAllHeaders(this WebResponse SourceResponse)
        {
            if (SourceResponse == null) { throw new ArgumentNullException("SourceResponse"); }
            String[] all_keys = SourceResponse.Headers.AllKeys;
            Dictionary<String, String> output = new Dictionary<string, string>(all_keys.Length);
            foreach (string one_key in all_keys)
            {
                output.Add(one_key, SourceResponse.Headers[one_key]);
            }
            return output;
        }

        /// <summary>
        /// Возвращает содержимое тела HTTP-ответа как текст в изменяемой строке. После вызова метода поток, представляющий тело ответа, больше не будет доступен.
        /// </summary>
        /// <param name="SourceResponse">Экземпляр HTTP-ответа. Если NULL - будет выброшено исключение. Если же поток с телом ответа является NULL, то будет возвращён NULL.</param>
        /// <returns></returns>
        public static StringBuilder GetTextContent(this HttpWebResponse SourceResponse)
        {
            if (SourceResponse == null) { throw new ArgumentNullException("SourceResponse"); }
            const Int32 chunk_size = 1024;
            using (Stream receiveStream = SourceResponse.GetResponseStream())
            {
                if (receiveStream == null)
                {
                    return null;
                }
                Encoding encoding = Encoding.GetEncoding(SourceResponse.CharacterSet ?? "UTF-8");
                StreamReader readStream = new StreamReader(receiveStream, encoding);
                Char[] read = new Char[chunk_size];
                int count = readStream.Read(read, 0, chunk_size);
                StringBuilder sb = new StringBuilder();
                while (count > 0)
                {
                    sb.Append(read, 0, count);
                    count = readStream.Read(read, 0, chunk_size);
                }
                return sb;
            }
        }

        /// <summary>
        /// Возвращает всё содержимое сессионной коллекции из уровня сеанса или веб-сайта из указанного HTTP-контекста в виде строки с указанными разделителями
        /// </summary>
        /// <param name="CurrentContext">Контекст, из которого извлекаются сессионные данные</param>
        /// <param name="SessionOrApplication">Если true - извлечение будет происходить из сессионной коллекции уровня сеанса текущего пользователя, 
        /// а если false - из глобальной сессии веб-сайта</param>
        /// <param name="ExternalDelimiter">Разделитель между разными элементами коллекции. Если NULL - будет применена пустая строка.</param>
        /// <param name="InternalDelimiter">Разделитель между ключем и значением одного элемента. Если NULL - будет применена пустая строка.</param>
        /// <returns>Если указанный контекст равен NULL, будет возвращён NULL. Если в сессионной коллекции нет данных, будет возвращена пустая строка. 
        /// Иначе строка, содержащая все элементы коллекции с их индексами и ключами.</returns>
        public static String CollectSpecSessionData(this HttpContext CurrentContext, Boolean SessionOrApplication, String ExternalDelimiter, String InternalDelimiter)
        {
            const String empty = "";
            if (CurrentContext == null) { return null; }
            System.Collections.Specialized.NameObjectCollectionBase.KeysCollection all_keys;
            if (SessionOrApplication == true)
            {
                if (CurrentContext.Session == null || CurrentContext.Session.Count == 0) { return empty; }
                all_keys = CurrentContext.Session.Keys;
            }
            else
            {
                if (CurrentContext.Application == null || CurrentContext.Application.Count == 0) { return empty; }
                all_keys = CurrentContext.Application.Keys;
            }
            if (all_keys == null || all_keys.Count == 0) { return empty; }
            String external_delimiter = ExternalDelimiter ?? empty;
            String internal_delimiter = InternalDelimiter ?? empty;

            StringBuilder output = new StringBuilder(all_keys.Count * 20);
            const String dot_delimiter = ". ";
            for (Int32 iter = 0; iter < all_keys.Count; iter++)
            {
                String one = String.Concat(iter.ToString(), dot_delimiter, all_keys[iter].ToString(), internal_delimiter,
                    SessionOrApplication == true ? CurrentContext.Session[all_keys[iter]].ToStringS() : CurrentContext.Application[all_keys[iter]].ToStringS(),
                    iter < all_keys.Count - 1 ? external_delimiter : empty
                    );
                output.Append(one);
            }
            return output.ToString();
        }

        /// <summary>
        /// Возвращает все куки из указанного хранилища куки в виде словаря пар ключ-значение, где ключ - название куки-файла, а значение — значение куки-файла
        /// </summary>
        /// <param name="Cookies">Специализированная коллекция, содержащая куки-записи. Если NULL - будет выброшено исключение.</param>
        /// <returns></returns>
        public static Dictionary<String, String> CollectCookiesData(this HttpCookieCollection Cookies)
        {
            if (Cookies == null) { return null; }

            Dictionary<String, String> output = new Dictionary<string, string>(Cookies.Count);
            for (Int32 i = 0; i < Cookies.Count; i++)
            {
                if (output.ContainsKey(Cookies[i].Name) == false)
                {
                    output.Add(Cookies[i].Name, Cookies[i].Value);
                }
            }
            return output;
        }

        /// <summary>
        /// Извлекает из указанного HTTP-запроса IP-адрес клиента и возвращает его
        /// </summary>
        /// <param name="CurrentRequest">Запрос, из которого необходимо извлечь IP-адрес. Если NULL - будет выброшено исключение.</param>
        /// <returns></returns>
        public static String GetIPAddress(this HttpRequest CurrentRequest)
        {
            if (CurrentRequest == null) { throw new ArgumentNullException("CurrentRequest"); }

            string ipList = CurrentRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ipList.HasVisibleChars() == true)
            {
                return ipList.Split(new Char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            if (CurrentRequest.UserHostAddress.HasVisibleChars() == true)
            {
                return CurrentRequest.UserHostAddress;
            }
            return CurrentRequest.ServerVariables["REMOTE_ADDR"];
        }

        /// <summary>
        /// Декодирует URI, заменяя все escape-последовательности на небезопасные
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static String DecodeUri(String Input)
        {
            if (Input.HasAlphaNumericChars() == false) { return Input; }
            return HttpUtility.HtmlDecode(HttpUtility.UrlDecode(Input));
        }
    }
}

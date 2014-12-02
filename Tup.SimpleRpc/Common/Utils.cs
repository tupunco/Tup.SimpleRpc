using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tup.SimpleRpc.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utils
    {
        #region String.Join
        /// <summary>
        /// String.Join
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Join(string separator, params object[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            if (values.Length == 0 || values[0] == null)
                return string.Empty;

            if (separator == null)
                separator = String.Empty;

            var result = new StringBuilder();

            var value = values[0].ToString();
            if (value != null)
                result.Append(value);

            for (int i = 1; i < values.Length; i++)
            {
                result.Append(separator);
                if (values[i] != null)
                {
                    // handle the case where their ToString() override is broken 
                    value = values[i].ToString();
                    if (value != null)
                        result.Append(value);
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// string.Join
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Join<T>(string separator, IEnumerable<T> values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            if (separator == null)
                separator = string.Empty;

            using (IEnumerator<T> en = values.GetEnumerator())
            {
                if (!en.MoveNext())
                    return string.Empty;

                var result = new StringBuilder();
                if (en.Current != null)
                {
                    // handle the case that the enumeration has null entries 
                    // and the case where their ToString() override is broken
                    string value = en.Current.ToString();
                    if (value != null)
                        result.Append(value);
                }

                while (en.MoveNext())
                {
                    result.Append(separator);
                    if (en.Current != null)
                    {
                        // handle the case that the enumeration has null entries
                        // and the case where their ToString() override is broken 
                        string value = en.Current.ToString();
                        if (value != null)
                            result.Append(value);
                    }
                }
                return result.ToString();
            }
        }
        /// <summary>
        /// string.Join
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Join(string separator, IEnumerable<string> values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            if (separator == null)
                separator = string.Empty;

            using (IEnumerator<string> en = values.GetEnumerator())
            {
                if (!en.MoveNext())
                    return string.Empty;

                var result = new StringBuilder();
                if (en.Current != null)
                {
                    result.Append(en.Current);
                }

                while (en.MoveNext())
                {
                    result.Append(separator);
                    if (en.Current != null)
                    {
                        result.Append(en.Current);
                    }
                }
                return result.ToString();
            }
        }
        #endregion

        #region Trim/Empty/Fmt
        /// <summary>
        /// 从当前 System.String 对象移除所有前导空白字符和尾部空白字符。
        /// NULL 字符串不会抛出异常
        /// </summary>
        /// <param name="strOri"></param>
        /// <returns></returns>
        public static string Trim2(this string strOri)
        {
            if (null == strOri)
                return strOri;

            return strOri.Trim();
        }

        /// <summary>
        /// Check that a string is not null or empty
        /// </summary>
        /// <param name="input">String to check</param>
        /// <returns>bool</returns>
        public static bool HasValue(this string input)
        {
            return !string.IsNullOrEmpty(input);
        }

        /// <summary>
        /// 指示指定的 System.String 对象是 null 还是 System.String.Empty 字符串。
        /// </summary>
        /// <param name="input">String to check</param>
        /// <returns>bool</returns>
        public static bool IsEmpty(this string input)
        {
            return string.IsNullOrEmpty(input);
        }

        /// <summary>
        /// 将指定 System.String 中的格式项替换为指定数组中相应 System.Object 实例的值的文本等效项。
        /// </summary>
        /// <param name="format">复合格式字符串。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 System.Object 数组。</param>
        /// <returns>format 的一个副本，其中格式项已替换为 args 中相应 System.Object 实例的 System.String 等效项。</returns>
        /// <exception cref="System.ArgumentNullException">format 或 args 为 null</exception>
        /// <exception cref="System.FormatException">format 无效。 - 或 - 用于指示要格式化的参数的数字小于零，或者大于等于 args 数组的长度</exception>
        public static string Fmt(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
        #endregion

        #region IsEmpty
        /// <summary>
        /// 指示指定类型的 数组对象是 null 或者 Length = 0。
        /// </summary>
        /// <param name="input">array to check</param>
        /// <returns>bool</returns>
        public static bool IsEmpty<T>(this T[] input)
        {
            return input == null || input.Length <= 0;
        }
        /// <summary>
        /// 指示指定类型的 数组对象是 null 或者 Length = 0。
        /// </summary>
        /// <param name="input">array to check</param>
        /// <returns>bool</returns>
        public static bool IsEmpty<T>(this ICollection<T> input)
        {
            return input == null || input.Count <= 0;
        }
        #endregion

        #region Dictionary GetValue
        /// <summary>
        /// GetValue From Dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> obj, TKey key)
        {
            return GetValue<TKey, TValue>(obj, key, default(TValue));
        }
        /// <summary>
        /// GetValue From Dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> obj,
                                                          TKey key, TValue defaultValue)
        {
            if (obj == null)
                return defaultValue;

            TValue tObj = default(TValue);
            if (obj.TryGetValue(key, out tObj))
                return tObj;

            return defaultValue;
        }
        #endregion

        #region byte-AsString
        /// <summary>
        /// Read a stream into a byte array
        /// </summary>
        /// <param name="input">Stream to read</param>
        /// <returns>byte[]</returns>
        public static byte[] ReadAsBytes(this Stream input)
        {
            if (input == null)
                return null;

            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Converts a byte array to a string, using its byte order mark to convert it to the right encoding.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        public static string AsString(this byte[] buffer)
        {
            return AsString(buffer, null);
        }
        /// <summary>
        /// Converts a byte array to a string, using its byte order mark to convert it to the right encoding.
        /// http://www.shrinkrays.net/code-snippets/csharp/an-extension-method-for-converting-a-byte-array-to-a-string.aspx
        /// </summary>
        /// <param name="buffer">An array of bytes to convert</param>
        /// <returns>The byte as a string.</returns>
        public static string AsString(this byte[] buffer, Encoding contentEncoding)
        {
            if (buffer == null || buffer.Length == 0)
                return string.Empty;

            if (contentEncoding != null)
                return contentEncoding.GetString(buffer, 0, buffer.Length);
            else
            {
                // Ansi as default
                Encoding encoding = Encoding.UTF8;
                /*
                    EF BB BF		UTF-8 
                    FF FE UTF-16	little endian 
                    FE FF UTF-16	big endian 
                    FF FE 00 00		UTF-32, little endian 
                    00 00 FE FF		UTF-32, big-endian 
                    */
                if (buffer.Length >= 3 && buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                {
                    encoding = Encoding.UTF8;
                }
                else if (buffer.Length >= 2)
                {
                    if (buffer[0] == 0xfe && buffer[1] == 0xff)
                    {
                        encoding = Encoding.Unicode;
                    }
                    else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                    {
                        encoding = Encoding.BigEndianUnicode; // utf-16be
                    }
                }

                return encoding.GetString(buffer, 0, buffer.Length);
            }
        }
        #endregion
    }
}

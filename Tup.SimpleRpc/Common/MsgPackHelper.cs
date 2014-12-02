using System.IO;

using MsgPack;
using System;

namespace Tup.SimpleRpc.Common
{
    /// <summary>
    /// MsgPack 序列化反序列工具类
    /// </summary>
    /// <remarks>
    /// https://github.com/msgpack/msgpack-cli
    /// </remarks>
    public static class MsgPackHelper
    {
        #region Pack/序列化
        /// <summary>
        /// Pack/序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Pack(object value)
        {
            if (value == null)
                return null;

            return (new ObjectPacker()).Pack(value);
        }
        /// <summary>
        /// Pack/序列化
        /// </summary>
        /// <param name="steam"></param>
        /// <param name="value"></param>
        public static void Pack(Stream steam, object value)
        {
            if (steam == null || value == null)
                return;

            (new ObjectPacker()).Pack(steam, value);
        }
        #endregion

        #region Unpack/反序列化
        /// <summary>
        /// Unpack/反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buf"></param>
        /// <returns></returns>
        public static T Unpack<T>(byte[] buf)
        {
            if (buf == null || buf.Length <= 0)
                return default(T);

            return (new ObjectPacker()).Unpack<T>(buf);
        }
        /// <summary>
        /// Unpack/反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buf"></param>
        /// <returns></returns>
        public static object Unpack(Type type, byte[] buf)
        {
            if (buf == null || buf.Length <= 0)
                return null;

            return (new ObjectPacker()).Unpack(type, buf);
        }
        /// <summary>
        /// Unpack/反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strm"></param>
        /// <returns></returns>
        public static T Unpack<T>(Stream strm)
        {
            if (strm == null)
                return default(T);

            return (new ObjectPacker()).Unpack<T>(strm);
        }
        /// <summary>
        /// Unpack/反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buf"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static T Unpack<T>(byte[] buf, int offset, int size)
        {
            if (buf == null || buf.Length <= 0)
                return default(T);

            return (new ObjectPacker()).Unpack<T>(buf, offset, size);
        }
        #endregion
    }
}

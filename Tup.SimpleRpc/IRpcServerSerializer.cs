using System;
using Tup.SimpleRpc.Common;

namespace Tup.SimpleRpc
{
    /// <summary>
    /// 序列化器 接口
    /// </summary>
    public interface IRpcServerSerializer
    {
        /// <summary>
        /// HttpHeader Content-Type
        /// </summary>
        string ContentType { get; }
        /// <summary>
        /// 序列化器 格式名称
        /// </summary>
        string Format { get; }
        /// <summary>
        /// 序列化 响应 字节
        /// </summary>
        /// <param name="responseInfo"></param>
        /// <returns></returns>
        byte[] SerializeObject(object responseContent, RpcMessage reqContext);
        /// <summary>
        /// 请求 内容 反序列化
        /// </summary>
        /// <param name="responseInfo"></param>
        /// <returns></returns>
        object DeserializeObject(byte[] requestBody, Type requestType, RpcMessage reqContext);
    }

    /// <summary>
    /// JSON 序列化器
    /// </summary>
    public class JsonRpcServerSerializer : IRpcServerSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly static string s_FormatName = "json";
        /// <summary>
        /// 
        /// </summary>
        private readonly static string s_ContentType = "application/json";

        #region IRpcServerSerializer 成员
        /// <summary>
        /// 
        /// </summary>
        public string Format
        {
            get { return s_FormatName; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ContentType
        {
            get { return s_ContentType; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseContent"></param>
        /// <param name="responseType"></param>
        /// <param name="reqContext"></param>
        /// <returns></returns>
        public byte[] SerializeObject(object responseContent, RpcMessage reqContext)
        {
            ThrowHelper.ThrowIfNull(responseContent, "responseContent");
            //ThrowHelper.ThrowIfNull(reqContext, "reqContext");

            var json = JsonNetHelper.SerializeObject(responseContent);
            if (json.HasValue())
                return reqContext.Encoding.GetBytes(json);
            else
                return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestBody"></param>
        /// <param name="reqContext"></param>
        /// <returns></returns>
        public object DeserializeObject(byte[] requestBody, Type requestType, RpcMessage reqContext)
        {
            ThrowHelper.ThrowIfNull(requestBody, "requestBody");
            ThrowHelper.ThrowIfNull(requestType, "requestType");
            //ThrowHelper.ThrowIfNull(reqContext, "reqContext");

            var json = reqContext.Encoding.GetString(requestBody);
            return JsonNetHelper.DeserializeObject(json, requestType);
        }
        #endregion
    }

    /// <summary>
    /// MsgPack 序列化器
    /// </summary>
    public class MsgPackRpcServerSerializer : IRpcServerSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly static string s_FormatName = "mp";
        /// <summary>
        /// 
        /// </summary>
        private readonly static string s_ContentType = "application/octet-stream";

        #region IRpcServerSerializer 成员
        /// <summary>
        /// 
        /// </summary>
        public string Format
        {
            get { return s_FormatName; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ContentType
        {
            get { return s_ContentType; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseContent"></param>
        /// <param name="responseType"></param>
        /// <param name="reqContext"></param>
        /// <returns></returns>
        public byte[] SerializeObject(object responseContent, RpcMessage reqContext)
        {
            ThrowHelper.ThrowIfNull(responseContent, "responseContent");
            //ThrowHelper.ThrowIfNull(reqContext, "reqContext");

            //TODO return MsgPackHelper.Pack(responseContent);
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestBody"></param>
        /// <param name="reqContext"></param>
        /// <returns></returns>
        public object DeserializeObject(byte[] requestBody, Type requestType, RpcMessage reqContext)
        {
            ThrowHelper.ThrowIfNull(requestBody, "requestBody");
            ThrowHelper.ThrowIfNull(requestType, "requestType");
            //ThrowHelper.ThrowIfNull(reqContext, "reqContext");

            //var MsgPack = RpcApplication.Default_Encoding.GetString(requestBody);
            //TODO return MsgPackHelper.Unpack(requestType, requestBody);
            return null;
        }
        #endregion
    }

    /// <summary>
    /// HTML 序列化器
    /// </summary>
    public class HtmlRpcServerSerializer : IRpcServerSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly static string s_FormatName = "html";
        /// <summary>
        /// 
        /// </summary>
        private readonly static string s_ContentType = "text/html";

        #region IRpcServerSerializer 成员
        /// <summary>
        /// 
        /// </summary>
        public string Format
        {
            get { return s_FormatName; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ContentType
        {
            get { return s_ContentType; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseContent"></param>
        /// <param name="responseType"></param>
        /// <param name="reqContext"></param>
        /// <returns></returns>
        public byte[] SerializeObject(object responseContent, RpcMessage reqContext)
        {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestBody"></param>
        /// <param name="reqContext"></param>
        /// <returns></returns>
        public object DeserializeObject(byte[] requestBody, Type requestType, RpcMessage reqContext)
        {
            return null;
        }
        #endregion
    }
}

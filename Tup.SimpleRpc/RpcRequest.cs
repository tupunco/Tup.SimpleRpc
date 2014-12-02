using System;
using System.Text;

using Tup.SimpleRpc.Common;

namespace Tup.SimpleRpc
{
    /// <summary>
    /// 请求/响应 消息 实体
    /// </summary>
    public abstract class RpcMessage
    {
        /// <summary>
        /// 格式
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 方法名称格式
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 请求 Body
        /// </summary>
        public byte[] Body { get; set; }

        /// <summary>
        /// HttpHeader Content-Type
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// 填充消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static TRpcResponse From<TRpcResponse>(RpcMessage msg)
            where TRpcResponse : RpcMessage, new()
        {
            ThrowHelper.ThrowIfNull(msg, "msg");

            return new TRpcResponse()
            {
                Format = msg.Format,
                AppId = msg.AppId,
                MethodName = msg.MethodName,
                Body = msg.Body,
                ContentType = msg.ContentType,
                Encoding = msg.Encoding,
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[RpcMessage Format={0}, AppId={1}, MethodName={2}, Body={3}, ContentType={4}, Encoding={5}]",
                                    Format, AppId, MethodName, Body, ContentType, Encoding);
        }
    }
    /// <summary>
    /// 请求 上下文 实体
    /// </summary>
    public class RpcRequest : RpcMessage
    {
        /// <summary>
        /// Http Parameter Collection
        /// </summary>
        public IRpcParameterCollection Parameters { get; set; }

        /// <summary>
        /// 当前 序列化器
        /// </summary>
        public IRpcServerSerializer Serializer { get; internal set; }
        /// <summary>
        /// 当前 应用服务器
        /// </summary>
        public RpcApplication Application { get; internal set; }

        /// <summary>
        /// Handler Metadata
        /// </summary>
        internal RpcHandlerMetadata Metadata { get; set; }
    }
    /// <summary>
    /// 响应 实体
    /// </summary>
    public class RpcResponse : RpcMessage
    {
    }
    /// <summary>
    /// 接口 Meta
    /// </summary>
    public class RpcHandlerMetadata
    {
        /// <summary>
        /// 
        /// </summary>
        public string MethodName { get; internal set; }

        /// <summary>
        /// ExecuteHandler 类型
        /// </summary>
        public Type HandlerType { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public Type InputType { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public Type OutputType { get; internal set; }
    }
}

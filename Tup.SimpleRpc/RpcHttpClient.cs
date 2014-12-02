using System;
using System.Collections.Generic;
using System.Text;

using Tup.SimpleRpc.Common;

namespace Tup.SimpleRpc
{
    /// <summary>
    /// RPC 请求客户端
    /// </summary>
    public static class RpcHttpClient
    {
        /// <summary>
        /// 默认处理编码
        /// </summary>
        public static readonly Encoding Default_Encoding = Encoding.UTF8;

        /// <summary>
        /// 序列化器 映射表
        /// </summary>
        internal static Dictionary<string, IRpcServerSerializer> m_ServerSerializer_Dic
                            = new Dictionary<string, IRpcServerSerializer>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 
        /// </summary>
        static RpcHttpClient()
        {
            InitServerSerializer();
        }
        /// <summary>
        /// 初始化序列化器
        /// </summary>
        private static void InitServerSerializer()
        {
            var jsonSerializer = new JsonRpcServerSerializer();
            m_ServerSerializer_Dic[jsonSerializer.Format] = jsonSerializer;

            //var bsonSerializer = new BsonRpcServerSerializer();
            //m_ServerSerializer_Dic[bsonSerializer.Format] = bsonSerializer;

            var msgPackSerializer = new MsgPackRpcServerSerializer();
            m_ServerSerializer_Dic[msgPackSerializer.Format] = msgPackSerializer;

            //var htmlSerializer = new HtmlRpcServerSerializer();
            //m_ServerSerializer_Dic[htmlSerializer.Format] = htmlSerializer;
        }

        /// <summary>
        /// 请求 接口
        /// </summary>
        /// <typeparam name="TRequest">请求数据类型</typeparam>
        /// <typeparam name="TResponse">响应数据类型</typeparam>
        /// <param name="request">请求 消息</param>
        /// <param name="requestData">请求 数据</param>
        /// <returns></returns>
        public static TResponse Get<TRequest, TResponse>(RpcClientRequest request, TRequest requestData)
            where TRequest : class, new()
            where TResponse : class, new()
        {
            ThrowHelper.ThrowIfNull(request, "request");
            ThrowHelper.ThrowIfNull(requestData, "requestData");

            ThrowHelper.ThrowIfNull(request.ServerUrl, "request.ServerUrl");
            ThrowHelper.ThrowIfNull(request.Format, "request.Format");
            ThrowHelper.ThrowIfNull(request.AppId, "request.AppId");
            ThrowHelper.ThrowIfNull(request.MethodName, "request.MethodName");

            var serverUrl = request.ServerUrl;
            var format = request.Format;
            var appId = request.AppId;
            var methodName = request.MethodName;
            if (request.Encoding == null)
                request.Encoding = RpcHttpClient.Default_Encoding;

            //1.获取序列化器
            var sSerializer = m_ServerSerializer_Dic.GetValue<string, IRpcServerSerializer>(format);
            ThrowHelper.ThrowIfNull(sSerializer, "sSerializer");
            request.ContentType = sSerializer.ContentType;

            //2.序列化请求数据
            var url = string.Format("{0}?format={1}&appid={2}&method={3}", serverUrl, format, appId, methodName);
            request.Body = sSerializer.SerializeObject(requestData, request);

            //3.HTTP 请求数据
            var resBytes = RequestHelper.DownLoadStream(url, string.Empty, true, request.Body, request.Timeout, request.ContentType, null);
            if (resBytes == null || resBytes.Length <= 0)
                return null;

            //4.反序列化响应数据
            var res = sSerializer.DeserializeObject(resBytes, typeof(TResponse), request);
            return res != null ? res as TResponse : null;
        }
    }

    /// <summary>
    /// RPC 请求客户端 请求数据实体
    /// </summary>
    public class RpcClientRequest : RpcMessage
    {
        /// <summary>
        /// 服务地址 http://127.0.0.1/xxx/
        /// </summary>
        public string ServerUrl { get; set; }
        /// <summary>
        /// 请求超时时间 (单位:毫秒, 默认值 100,000 毫秒(100 秒))
        /// </summary>
        public int Timeout { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Tup.SimpleRpc.Common;

namespace Tup.SimpleRpc
{
    /// <summary>
    /// RPC 应用服务器
    /// </summary>
    public sealed class RpcApplication
    {
        /// <summary>
        /// Url 参数-format
        /// </summary>
        public static readonly string Param_Format = "format";
        /// <summary>
        /// Url 参数-appid
        /// </summary>
        public static readonly string Param_AppId = "appid";
        /// <summary>
        /// Url 参数-method
        /// </summary>
        public static readonly string Param_Method = "method";

        /// <summary>
        /// 默认处理编码
        /// </summary>
        public static readonly Encoding Default_Encoding = Encoding.UTF8;

        /// <summary>
        /// 序列化器 映射表
        /// </summary>
        internal Dictionary<string, IRpcServerSerializer> m_ServerSerializer_Dic
                            = new Dictionary<string, IRpcServerSerializer>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 执行器 映射表
        /// </summary>
        internal Dictionary<string, IExecuteHandlerWrapper> m_ExecuteHandler_Dic
                            = new Dictionary<string, IExecuteHandlerWrapper>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        public RpcApplication()
        {
            InitServerSerializer();
        }
        /// <summary>
        /// 初始化序列化器
        /// </summary>
        private void InitServerSerializer()
        {
            var jsonSerializer = new JsonRpcServerSerializer();
            m_ServerSerializer_Dic[jsonSerializer.Format] = jsonSerializer;

            //var bsonSerializer = new BsonRpcServerSerializer();
            //m_ServerSerializer_Dic[bsonSerializer.Format] = bsonSerializer;

            var msgPackSerializer = new MsgPackRpcServerSerializer();
            m_ServerSerializer_Dic[msgPackSerializer.Format] = msgPackSerializer;

            var htmlSerializer = new HtmlRpcServerSerializer();
            m_ServerSerializer_Dic[htmlSerializer.Format] = htmlSerializer;
        }

        #region Register Handler
        /// <summary>
        /// 注册 执行器
        /// </summary>
        /// <typeparam name="TExecuteHandler">执行器 类型</typeparam>
        /// <param name="methodName">当前执行器 映射方法名字</param>
        /// <returns></returns>
        public RpcApplication Register<TExecuteHandler>(string methodName)
            where TExecuteHandler : IRpcExecuteHandler, new()
        {
            ThrowHelper.ThrowIfNull(methodName, "methodName");

            m_ExecuteHandler_Dic[methodName] = new ExecuteHandlerWrapper<TExecuteHandler>(methodName);

            return this;
        }
        /// <summary>
        /// 注册 Controller 执行器
        /// </summary>
        /// <typeparam name="TRpcController">待注册 Controller 类型</typeparam>
        /// <typeparam name="TRequestEntity">Controller 请求实体类型</typeparam>
        /// <typeparam name="TResponseEntity">Controller 响应实体类型</typeparam>
        /// <returns></returns>
        public RpcApplication Register<TRpcController, TRequestEntity, TResponseEntity>()
            where TRpcController : RpcBaseController<TRequestEntity, TResponseEntity>, new()
        {
            return Register<TRpcController, TRequestEntity, TResponseEntity>(RpcRequestEntityConverter<TRequestEntity>.Default);
        }
        /// <summary>
        /// 注册 Controller 执行器
        /// </summary>
        /// <typeparam name="TRpcController">待注册 Controller 类型</typeparam>
        /// <typeparam name="TRequestEntity">Controller 请求实体类型</typeparam>
        /// <typeparam name="TResponseEntity">Controller 响应实体类型</typeparam>
        /// <param name="requestEntityConverter">请求实体类型转换器</param>
        /// <returns></returns>
        public RpcApplication Register<TRpcController, TRequestEntity, TResponseEntity>(RpcRequestEntityConverter<TRequestEntity> requestEntityConverter)
            where TRpcController : RpcBaseController<TRequestEntity, TResponseEntity>, new()
        {
            ThrowHelper.ThrowIfNull(requestEntityConverter, "requestEntityConverter");

            var handlerList = RpcUtils.GetRpcHandlerFromController<TRpcController, TRequestEntity, TResponseEntity>(requestEntityConverter);
            if (handlerList != null && handlerList.Count > 0)
            {
                var dic = m_ExecuteHandler_Dic;
                var methodName = string.Empty;

                foreach (var handlerItem in handlerList)
                {
                    methodName = handlerItem.Metadata.MethodName;

                    ThrowHelper.ThrowIfNull(methodName, "handlerItem.MethodName");
                    ThrowHelper.ThrowIfFalse(!dic.ContainsKey(methodName), "handlerItem.MethodName  重复");

                    m_ExecuteHandler_Dic[methodName] = handlerItem;
                }
            }

            return this;
        }
        ///// <summary>
        ///// 注册 帮助文档 执行器
        ///// </summary>
        //public RpcApplication RegisterHelpHandler()
        //{
        //    return this.Register<Help.HelpExecuteHandler>(Help.HelpExecuteHandler.MethodName);
        //}
        #endregion

        #region ExecuteRequest
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="format"></param>
        /// <param name="methodName"></param>
        /// <param name="body"></param>
        public RpcResponse ExecuteRequest(RpcRequest request)
        {
            ThrowHelper.ThrowIfNull(request, "request");
            ThrowHelper.ThrowIfNull(request.AppId, "request.AppId");
            ThrowHelper.ThrowIfNull(request.Format, "request.Format");
            ThrowHelper.ThrowIfNull(request.MethodName, "request.MethodName");
            ThrowHelper.ThrowIfNull(request.Body, "request.Body");

            //--------TODO---
            //1.拿到请求 方法名/Format
            //2.拿到 Request Body Bytes
            //3.反序列化 Request Body
            //4.ExecuteHandler
            //5.序列化 执行结果
            //6.响应

            var cHandler = m_ExecuteHandler_Dic.GetValue<string, IExecuteHandlerWrapper>(request.MethodName);
            var sSerializer = m_ServerSerializer_Dic.GetValue<string, IRpcServerSerializer>(request.Format);
            request.Serializer = sSerializer;
            request.Application = this;
            request.Metadata = cHandler.Metadata;
            if (request.Metadata == null)
            {
                request.Metadata = new RpcHandlerMetadata()
                {
                    MethodName = request.MethodName,
                };
            }
            if (request.Encoding == null)
                request.Encoding = RpcApplication.Default_Encoding;

            return cHandler.Execute(request);
        }
        #endregion
    }
}

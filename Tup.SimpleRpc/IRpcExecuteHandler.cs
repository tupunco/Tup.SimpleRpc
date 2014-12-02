using Tup.SimpleRpc.Common;

namespace Tup.SimpleRpc
{
    /// <summary>
    /// 执行器 接口
    /// </summary>
    public interface IRpcExecuteHandler
    {
        /// <summary>
        /// 执行 本处理器
        /// </summary>
        /// <param name="reqContext">请求上下文</param>
        /// <returns></returns>
        RpcResponse Execute(RpcRequest reqContext);
    }
    /// <summary>
    /// 泛型 执行器 接口
    /// </summary>
    public interface IRpcExecuteHandler<TRequestEntity, TResponseEntity>
                        : IRpcExecuteHandler
    {
        /// <summary>
        /// 执行 本处理器
        /// </summary>
        /// <param name="reqContext">请求上下文</param>
        /// <param name="inInfo">请求实体 数据</param>
        /// <returns></returns>
        TResponseEntity Execute(RpcRequest reqContext, TRequestEntity inInfo);
    }
    /// <summary>
    /// 基本 泛型 执行器 接口
    /// </summary>
    public abstract class RpcBaseExecuteHandler<TRequestEntity, TResponseEntity>
                            : IRpcExecuteHandler<TRequestEntity, TResponseEntity>
    {
        #region IRpcExecuteHandler<TRequestEntity,TResponseEntity> 成员
        /// <summary>
        /// 执行 本处理器
        /// </summary>
        /// <param name="reqContext">请求上下文</param>
        /// <param name="inInfo">请求实体 数据</param>
        /// <returns></returns>
        public abstract TResponseEntity Execute(RpcRequest reqContext, TRequestEntity inInfo);
        #endregion

        #region IRpcExecuteHandler 成员
        /// <summary>
        /// 执行 本处理器
        /// </summary>
        /// <param name="reqContext"></param>
        /// <returns></returns>
        RpcResponse IRpcExecuteHandler.Execute(RpcRequest reqContext)
        {
            ThrowHelper.ThrowIfNull(reqContext, "reqContext");
            ThrowHelper.ThrowIfNull(reqContext.Serializer, "reqContext.Serializer");
            ThrowHelper.ThrowIfNull(reqContext.Body, "reqContext.Body");

            //--------TODO---
            //3.反序列化 Request Body
            //4.ExecuteHandler
            //5.序列化 执行结果
            //6.响应

            var cRequestType = reqContext.Metadata.InputType ?? typeof(TRequestEntity);
            var body = reqContext.Body;
            var sSerializer = reqContext.Serializer;
            //3.反序列化 Request Body
            var requestEntity = (TRequestEntity)sSerializer.DeserializeObject(body, cRequestType, reqContext);
            //4.执行 ExecuteHandler
            var responseEntity = this.Execute(reqContext, requestEntity);
            //5.序列化 执行结果
            var respBody = sSerializer.SerializeObject(responseEntity, reqContext);

            var resp = RpcMessage.From<RpcResponse>(reqContext);
            if (respBody != null)
                resp.Body = respBody;
            resp.ContentType = sSerializer.ContentType;
            return resp;
        }
        #endregion
    }
}

using System;

namespace Tup.SimpleRpc
{
    /// <summary>
    /// RpcBaseController
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TRespose"></typeparam>
    public abstract class RpcBaseController<TRequestEntity, TResponseEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TActionParams">Action Params</typeparam>
        /// <param name="reqContext"></param>
        /// <param name="requestEntity"></param>
        /// <param name="actionParam"></param>
        /// <returns></returns>
        public abstract TResponseEntity InvokeWrapper<TActionParams>(RpcRequest reqContext, TRequestEntity requestEntity, Func<TActionParams, TResponseEntity> actionParam);
    }
    /// <summary>
    /// RpcBaseController 中 RequestEntity 类型 转换器, 要获取实际的 RequestEntity 类型
    /// </summary>
    /// <typeparam name="TRequestEntity"></typeparam>
    public class RpcRequestEntityConverter<TRequestEntity>
    {
        /// <summary>
        /// Default
        /// </summary>
        public readonly static RpcRequestEntityConverter<TRequestEntity> 
                                    Default = new RpcRequestEntityConverter<TRequestEntity>();

        /// <summary>
        /// 请求实体类型
        /// </summary>
        /// <typeparam name="TRequestEntity"></typeparam>
        /// <typeparam name="TActionParams"></typeparam>
        /// <returns></returns>
        public virtual Type GetRequestEntityType<TActionParams>()
        {
            return typeof(TRequestEntity);
        }
    }
}

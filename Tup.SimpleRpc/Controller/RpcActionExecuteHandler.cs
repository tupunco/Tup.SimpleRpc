using System;

using Tup.SimpleRpc.Common;

namespace Tup.SimpleRpc
{
    /// <summary>
    /// Controller Action ExecuteHandler
    /// </summary>
    /// <typeparam name="TRequestEntity"></typeparam>
    /// <typeparam name="TResponseEntity"></typeparam>
    internal class RpcActionExecuteHandler<TRequestEntity, TResponseEntity>
        : RpcBaseExecuteHandler<TRequestEntity, TResponseEntity>, IExecuteHandlerWrapper
    {
        /// <summary>
        /// Handler Metadata
        /// </summary>
        public RpcHandlerMetadata Metadata { get; internal set; }

        /// <summary>
        /// ActionFunc
        /// </summary>
        public Func<RpcRequest, TRequestEntity, TResponseEntity> ActionFunc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqContext"></param>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        public override TResponseEntity Execute(RpcRequest reqContext, TRequestEntity inInfo)
        {
            ThrowHelper.ThrowIfNull(ActionFunc, "ActionFunc");

            return ActionFunc(reqContext, inInfo);
        }
    }
}

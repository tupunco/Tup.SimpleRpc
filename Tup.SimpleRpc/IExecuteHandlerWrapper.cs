using System;
using System.Linq.Expressions;

using Tup.SimpleRpc.Common;

namespace Tup.SimpleRpc
{
    #region IExecuteHandlerWrapper/执行器 包装 接口
    /// <summary>
    /// 执行器 包装 接口
    /// </summary>
    public interface IExecuteHandlerWrapper : IRpcExecuteHandler
    {
        /// <summary>
        /// Handler Metadata
        /// </summary>
        RpcHandlerMetadata Metadata { get; }
    }

    /// <summary>
    /// 执行器 包装
    /// </summary>
    /// <typeparam name="TRequestEntity"></typeparam>
    /// <typeparam name="TResponseEntity"></typeparam>
    internal class ExecuteHandlerWrapper<TExecuteHandler> : IExecuteHandlerWrapper
                  where TExecuteHandler : IRpcExecuteHandler, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appContext"></param>
        /// <param name="methodName"></param>
        public ExecuteHandlerWrapper(string methodName)
        {
            this.Metadata = new RpcHandlerMetadata()
            {
                HandlerType = typeof(TExecuteHandler),
                MethodName = methodName
            };
            this.NewHandlerFunc = RpcUtils.GetNewHandlerFunc<TExecuteHandler>();
        }

        #region IExecuteHandlerWrapper 成员
        /// <summary>
        /// Handler Metadata
        /// </summary>
        public RpcHandlerMetadata Metadata { get; internal set; }
        /// <summary>
        /// New Handler Func
        /// </summary>
        public Func<TExecuteHandler> NewHandlerFunc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqContext"></param>
        public RpcResponse Execute(RpcRequest reqContext)
        {
            ThrowHelper.ThrowIfNull(reqContext, "reqContext");

            return (this.NewHandlerFunc()).Execute(reqContext);
        }
        #endregion
    }
    #endregion
}

using System;

using Tup.SimpleRpc;
using Tup.SimpleRpc.Common;

namespace Tup.WebServerDemo
{
    /// <summary>
    /// RequestEntity 类型 转换器
    /// </summary>
    public class TestRequestEntityConverter : RpcRequestEntityConverter<RequestBody>
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly TestRequestEntityConverter Converter = new TestRequestEntityConverter();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TActionParams"></typeparam>
        /// <returns></returns>
        public override Type GetRequestEntityType<TActionParams>()
        {
            return typeof(RequestBody<TActionParams>);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseTestRpcController : RpcBaseController<RequestBody, ResponseBody>
    {
        /// <summary>
        /// 用户 Token 信息
        /// </summary>
        protected string Token { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TActionParams"></typeparam>
        /// <param name="actionParam"></param>
        /// <param name="reqContext"></param>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        public override ResponseBody InvokeWrapper<TActionParams>(RpcRequest reqContext,
                                                                    RequestBody requestEntity,
                                                                    Func<TActionParams, ResponseBody> actionParam)
        {
            ThrowHelper.ThrowIfNull(actionParam, "actionParam");
            ThrowHelper.ThrowIfNull(requestEntity, "requestEntity");
            ThrowHelper.ThrowIfNull(reqContext, "reqContext");

            var t = typeof(RequestBody<>);

            TActionParams inData = default(TActionParams);
            if (requestEntity is RequestBody<TActionParams>)
                inData = (requestEntity as RequestBody<TActionParams>).Data;

            try
            {
                this.Token = requestEntity.Token;//TODO 

                return actionParam(inData);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("BaseTestRpcController-requestEntity-{0}-Type:{1}-reqContext:{2}-Token:{3}-ex:{4}",
                                            requestEntity, typeof(RequestBody<TActionParams>), reqContext, this.Token, ex);
                return OnError(reqContext);
            }
        }
        /// <summary>
        /// OnError
        /// </summary>
        protected virtual ResponseBody OnError(RpcRequest reqContext)
        {
            return new ResponseBody() { Result = -999, Message = "异常", };
        }
    }
}

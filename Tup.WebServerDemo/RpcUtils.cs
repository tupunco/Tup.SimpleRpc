using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tup.SimpleRpc;
using Tup.SimpleRpc.Common;

namespace Tup.WebServerDemo
{
    static class RpcUtils
    {
        /// <summary>
        /// 注册 Controller 执行器
        /// </summary>
        /// <typeparam name="TRpcController"></typeparam>
        /// <param name="rpcApp"></param>
        /// <returns></returns>
        public static RpcApplication Register<TRpcController>(this RpcApplication rpcApp)
            where TRpcController : BaseTestRpcController, new()
        {
            ThrowHelper.ThrowIfNull(rpcApp, "rpcApp");

            rpcApp.Register<TRpcController, RequestBody, ResponseBody>(TestRequestEntityConverter.Converter);

            return rpcApp;
        }

        ///// <summary>
        ///// 转换成 RpcRequest 请求
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //public static RpcRequest ToRequest(this IRequest httpRequest)
        //{
        //    ThrowHelper.ThrowIfNull(httpRequest, "httpRequest");

        //    var httpParam = httpRequest.Parameters;

        //    var appId = httpParam.GetParameters(RpcApplication.Param_AppId);
        //    var methodName = httpParam.GetParameters(RpcApplication.Param_Method);
        //    var format = httpParam.GetParameters(RpcApplication.Param_Format);
        //    if (appId.IsEmpty() || methodName.IsEmpty() || format.IsEmpty())
        //        return null; //TODO Empty Parameters Error

        //    //获取 contentType
        //    var contentType = string.Empty;
        //    if (httpRequest.ContentType != null)
        //        contentType = httpRequest.ContentType.Value;

        //    //获取 body bytes
        //    byte[] body = null;
        //    var httpMethod = httpRequest.Method.ToUpper();
        //    if (httpMethod == "POST" || httpMethod == "PUT")
        //        body = httpRequest.GetBody();
        //    if (body == null)
        //        body = new byte[0];

        //    var rpcParameters = httpParam.ToRpcParameters();
        //    var encoding = httpRequest.Encoding;

        //    return new RpcRequest()
        //    {
        //        AppId = appId,
        //        MethodName = methodName,
        //        Format = format,
        //        ContentType = contentType,
        //        Encoding = encoding,
        //        Body = body,
        //        Parameters = rpcParameters
        //    };
        //}
        ///// <summary>
        ///// 处理响应
        ///// </summary>
        ///// <param name="rpcResponse"></param>
        ///// <param name="httpResponse"></param>
        //public static void ToResponse(this RpcResponse rpcResponse, IResponse httpResponse)
        //{
        //    ThrowHelper.ThrowIfNull(rpcResponse, "rpcResponse");
        //    ThrowHelper.ThrowIfNull(httpResponse, "httpResponse");

        //    //注入 Content-Type
        //    var contentType = rpcResponse.ContentType;
        //    if (contentType.HasValue())
        //    {
        //        if (httpResponse.ContentType == null)
        //            httpResponse.ContentType = new ContentTypeHeader(contentType);
        //        else
        //            httpResponse.ContentType.Value = contentType;

        //        httpResponse.ContentType.Parameters["charset"] = rpcResponse.Encoding.WebName;
        //    }

        //    var body = rpcResponse.Body;
        //    if (body != null)
        //        httpResponse.Body.Write(body, 0, body.Length);
        //}
    }
}

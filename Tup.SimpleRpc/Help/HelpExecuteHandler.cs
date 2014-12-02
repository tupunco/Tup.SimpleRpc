using System;
using System.Collections.Generic;
using System.Text;
using Tup.SimpleRpc.Common;

namespace Tup.SimpleRpc.Help
{
    /// <summary>
    /// 帮助 操作枚举
    /// </summary>
    internal enum HelpMethodTye
    {
        /// <summary>
        /// 获取 方法列表
        /// </summary>
        GetList = 0,
        /// <summary>
        /// 获取 方法 说明明细
        /// </summary>
        GetDetail = 1,
        //GetHelp
    }
    /// <summary>
    /// 帮助 执行器
    /// </summary>
    /// <remarks>
    /// __doc:文档请求参数
    ///     getList:获取 方法列表
    ///     getDetail:获取 方法 说明明细
    ///         __docName:具体需要显示的某个 MethodName.
    /// </remarks>
    [RpcDoc("帮助 执行器", "sys.doc",
        Description = @"帮助描述, 支持 HTML 标签.<br/>
                       __doc:文档请求参数<br/>
                       >>>getList:获取 方法列表<br/>
                       >>>getDetail:获取 方法 说明明细<br/>
                       >>>>>>__docName:具体需要显示的某个 MethodName.",
        Output = @"输出描述, 支持 HTML 标签"/*,
        OutputType = null,
        InputType = null*/
                          )]
    internal class HelpExecuteHandler : IRpcExecuteHandler
    {
        /// <summary>
        /// MethodName
        /// </summary>
        public readonly static string Default_MethodName = "sys.doc";

        #region IRpcExecuteHandler 成员
        /// <summary>
        /// Handler Metadata
        /// </summary>
        public RpcHandlerMetadata Metadata { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqContext"></param>
        /// <returns></returns>
        public RpcResponse Execute(RpcRequest reqContext)
        {
            ThrowHelper.ThrowIfNull(reqContext, "reqContext");
            //ThrowHelper.ThrowIfNull(reqContext.Serializer, "reqContext.Serializer");
            ThrowHelper.ThrowIfNull(reqContext.Body, "reqContext.Body");
            ThrowHelper.ThrowIfNull(reqContext.Parameters, "reqContext.Parameters");

            var methodType = HelpMethodTye.GetList;
            var methodTypeParam = reqContext.Parameters.Get("__doc");
            if (methodTypeParam != null)
            {
                var methodTypeStr = methodTypeParam.Value;
                if (methodTypeStr.HasValue())
                    methodTypeStr = methodTypeStr.ToLower();

                switch (methodTypeStr)
                {
                    case "getdetail":
                        methodType = HelpMethodTye.GetDetail;
                        break;
                    case "getlist":
                    default:
                        methodType = HelpMethodTye.GetList;
                        break;
                }
            }

            TryInitDocCache(reqContext.Application);

            var result = Execute(reqContext, methodType);

            var resp = RpcMessage.From<RpcResponse>(reqContext);
            if (result.HasValue())
                resp.Body = RpcApplication.Default_Encoding.GetBytes(result);
            if (reqContext.Serializer != null)
                resp.ContentType = reqContext.Serializer.ContentType;

            return resp;
        }
        #endregion

        #region IRpcExecuteHandler 自定义成员
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqContext"></param>
        /// <param name="methodType"></param>
        /// <returns></returns>
        public string Execute(RpcRequest reqContext, HelpMethodTye methodType)
        {
            var result = string.Empty;
            switch (methodType)
            {
                case HelpMethodTye.GetDetail: //获取 方法 说明明细
                    result = GetDocDetail(reqContext);
                    break;
                case HelpMethodTye.GetList: //获取 方法列表
                default:
                    result = GetDocList(reqContext);
                    break;
            }
            return result;
        }
        /// <summary>
        /// 获取 方法 说明明细
        /// </summary>
        /// <returns></returns>
        private string GetDocDetail(RpcRequest reqContext)
        {
            var methodTypeParam = reqContext.Parameters.Get("__docName");
            string methodType = null;
            if (methodTypeParam == null || (methodType = methodTypeParam.Value).IsEmpty())
                return "---NULL-__docName 参数--" + methodTypeParam;

            var docCacheList = s_Default_Doc_Cache;
            if (docCacheList == null)
                return "---NULL-ExecuteHandler-List-";

            var cMethodFormat = "json";
            var jsonSerializer = reqContext.Application.m_ServerSerializer_Dic
                                            .GetValue<string, IRpcServerSerializer>(cMethodFormat);
            var urlFormat = GetRequestUrlPrefix(cMethodFormat, reqContext.AppId, methodType);
            var resultSb = new StringBuilder();
            var cDoc = docCacheList.GetValue<string, RpcDocAttribute>(methodType);
            resultSb.AppendFormat("<h1>MethodName:</h1>\r\n<div>{0}</div>\r\n", cDoc.MethodName)
                    .AppendFormat("<h1>UrlParam:</h1>\r\n<div>{0}</div>\r\n", urlFormat)
                    .AppendFormat("<h1>Title:</h1>\r\n<div>{0}</div>\r\n", cDoc.Title)
                    .AppendFormat("<h1>Description:</h1>\r\n<div>{0}</div>\r\n", cDoc.Description);
            if (cDoc.InputType != null)
            {
                var tValue = GetSerializerTypeValue(reqContext, jsonSerializer, cDoc.InputType);
                resultSb.AppendFormat("<h1>InputType:</h1>\r\n<div>{0}</div>\r\n<h3>InputType-JSON:</h3>\r\n<div>{1}</div>\r\n",
                    cDoc.InputType, tValue);
            }

            resultSb.AppendFormat("<h1>Output:</h1>\r\n<div>{0}</div>\r\n", cDoc.Output);
            if (cDoc.OutputType != null)
            {
                var tValue = GetSerializerTypeValue(reqContext, jsonSerializer, cDoc.OutputType);
                resultSb.AppendFormat("<h1>OutputType:</h1>\r\n<div>{0}</div>\r\n<h3>OutputType-JSON:</h3>\r\n<div>{1}</div>\r\n",
                    cDoc.OutputType, tValue);
            }

            return resultSb.ToString();
        }
        /// <summary>
        /// 获取序列化后的'某类型'默认实例字符串
        /// </summary>
        /// <param name="reqContext"></param>
        /// <param name="jsonSerializer"></param>
        /// <param name="cType"></param>
        /// <returns></returns>
        private static string GetSerializerTypeValue(RpcRequest reqContext,
            IRpcServerSerializer jsonSerializer,
            Type cType)
        {
            var typeNull = Activator.CreateInstance(cType);
            return jsonSerializer.SerializeObject(typeNull, reqContext).AsString(reqContext.Encoding);
        }
        /// <summary>
        /// 获取 方法列表
        /// </summary>
        /// <returns></returns>
        private string GetDocList(RpcRequest reqContext)
        {
            var urlFormat = GetRequestUrlPrefix(reqContext);
            var htmlLIFormat = "<li><a href='{0}&__doc=getdetail&__docName={1}'>{2}</a></li>\r\n";

            var resultSb = new StringBuilder();
            var appHandlerList = reqContext.Application.m_ExecuteHandler_Dic;
            var docCacheList = s_Default_Doc_Cache;
            //<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
            resultSb.AppendLine("<ul>");
            RpcDocAttribute cDoc = null;
            string handlerKey = null;
            foreach (var item in appHandlerList)
            {
                handlerKey = item.Key;
                cDoc = docCacheList.GetValue<string, RpcDocAttribute>(handlerKey);
                if (cDoc != null)
                    resultSb.AppendFormat(htmlLIFormat, urlFormat, handlerKey, cDoc.Title);
                else
                    resultSb.AppendFormat(htmlLIFormat, urlFormat, handlerKey, handlerKey);
            }
            resultSb.AppendLine("</ul>");
            return resultSb.ToString();
        }
        /// <summary>
        /// 获取请求 URL 前缀
        /// </summary>
        /// <param name="reqContext"></param>
        /// <returns></returns>
        private static string GetRequestUrlPrefix(RpcRequest reqContext)
        {
            return GetRequestUrlPrefix(reqContext.Format, reqContext.AppId, reqContext.MethodName);
        }
        /// <summary>
        /// 获取请求 URL 前缀
        /// </summary>
        /// <param name="format"></param>
        /// <param name="appId"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private static string GetRequestUrlPrefix(string format, string appId, string methodName)
        {
            return "?{0}={1}&{2}={3}&{4}={5}".Fmt(
                                RpcApplication.Param_Format, format,
                                RpcApplication.Param_AppId, appId,
                                RpcApplication.Param_Method, methodName);
        }
        #region 文档缓存
        /// <summary>
        /// 初始化文档缓存
        /// </summary>
        private static void TryInitDocCache(RpcApplication app)
        {
            if (app == null || s_Default_Doc_Cache != null)
                return;

            lock (s_DocCache_ObjLock)
            {
                if (s_Default_Doc_Cache != null)
                    return;

                var executeHandlerDic = app.m_ExecuteHandler_Dic;
                if (executeHandlerDic == null)
                    return;

                //反射拿到所有 ExecuteHandler 的 RpcDocAttribute 特征节点
                var docCache = new Dictionary<string, RpcDocAttribute>();
                Type cHandlerType = null;
                foreach (var exeItem in executeHandlerDic)
                {
                    cHandlerType = exeItem.Value.Metadata.HandlerType;
                    var docAttr = cHandlerType.GetCustomAttributes<RpcDocAttribute>();
                    if (docAttr != null)
                        docCache[exeItem.Key] = docAttr as RpcDocAttribute;
                }

                s_Default_Doc_Cache = docCache;
            }
        }
        /// <summary>
        /// 文档缓存 ObjLock
        /// </summary>
        private static object s_DocCache_ObjLock = new object();
        /// <summary>
        /// 文档缓存
        /// </summary>
        private static Dictionary<string, RpcDocAttribute> s_Default_Doc_Cache = null;
        #endregion
        #endregion
    }
}

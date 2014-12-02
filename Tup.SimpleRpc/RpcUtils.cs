using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Tup.SimpleRpc.Common;

namespace Tup.SimpleRpc
{
    /// <summary>
    /// Rpc Utils
    /// </summary>
    internal static class RpcUtils
    {
        /// <summary>
        /// 获取 指定 RpcController 中的所有 Action 并转换成 IExecuteHandlerWrapper 列表
        /// </summary>
        /// <typeparam name="TRpcController">待注册 Controller 类型</typeparam>
        /// <typeparam name="TRequestEntity">Controller 请求实体类型</typeparam>
        /// <typeparam name="TResponseEntity">Controller 响应实体类型</typeparam>
        /// <param name="requestEntityConverter">请求实体类型转换器</param>
        /// <returns></returns>
        public static List<IExecuteHandlerWrapper> GetRpcHandlerFromController<TRpcController, TRequestEntity, TResponseEntity>(RpcRequestEntityConverter<TRequestEntity> requestEntityConverter)
             where TRpcController : RpcBaseController<TRequestEntity, TResponseEntity>, new()
        {
            ThrowHelper.ThrowIfNull(requestEntityConverter, "requestEntityConverter");

            var typeController = typeof(TRpcController);
            var controllerAtr = typeController.GetCustomAttributes<RpcControllerAttribute>();
            if (controllerAtr == null)
                return null;

            if (controllerAtr.NamePrefix.IsEmpty())
                controllerAtr.NamePrefix = typeController.Name;

            var typeResponseEntity = typeof(TResponseEntity);
            var methods = typeController.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                            .Where(x => x.ReturnType == typeResponseEntity /*返回类型必须为 TResponseEntity 类型*/
                                                        && !x.IsGenericMethod /*非泛型方法*/
                                                        && x.GetParameters().Length == 1 /*只接受一个输入参数的 参数*/)
                                            .ToArray();
            if (methods == null || methods.Length <= 0)
                return null;

            var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
            var iwMethod = typeController.GetMethod("InvokeWrapper", bindingAttr);
            var gretMethod = typeof(RpcRequestEntityConverter<TRequestEntity>).GetMethod("GetRequestEntityType", bindingAttr);
            ThrowHelper.ThrowIfNull(iwMethod, "InvokeWrapper NULL");
            ThrowHelper.ThrowIfNull(gretMethod, "GetRequestEntityType NULL");

            var resHandlerList = new List<IExecuteHandlerWrapper>();
            IExecuteHandlerWrapper tHandler = null;
            RpcActionAttribute tActionAtr = null;
            foreach (var methodInfo in methods)
            {
                tActionAtr = methodInfo.GetCustomAttributes<RpcActionAttribute>(); //每个方法必须有 RpcActionAttribute 特征
                if (tActionAtr == null)
                    continue;

                if (tActionAtr.ActionName.IsEmpty())
                    tActionAtr.ActionName = tActionAtr.ActionName;

                tHandler = GetRpcHandlerByActionMethod<TRpcController, TRequestEntity, TResponseEntity>(typeController, iwMethod, requestEntityConverter, gretMethod, methodInfo, controllerAtr, tActionAtr);
                if (tHandler != null)
                    resHandlerList.Add(tHandler);
            }
            return resHandlerList;
        }
        /// <summary>
        /// 获取 某 Action 的 IExecuteHandlerWrapper
        /// </summary>
        /// <typeparam name="TRpcController"></typeparam>
        /// <typeparam name="TRequestEntity"></typeparam>
        /// <typeparam name="TResponseEntity"></typeparam>
        /// <param name="typeController"></param>
        /// <param name="invokeWrapperMethod"></param>
        /// <param name="requestEntityConverter">请求实体类型转换器</param>
        /// <param name="getRequestTypeMethod"></param>
        /// <param name="methodInfo"></param>
        /// <param name="controllerAttribute"></param>
        /// <param name="actionAttribute"></param>
        /// <returns></returns>
        private static IExecuteHandlerWrapper GetRpcHandlerByActionMethod<TRpcController, TRequestEntity, TResponseEntity>(
                                                                                Type typeController,
                                                                                MethodInfo invokeWrapperMethod,
                                                                                RpcRequestEntityConverter<TRequestEntity> requestEntityConverter,
                                                                                MethodInfo getRequestTypeMethod,
                                                                                MethodInfo methodInfo,
                                                                                RpcControllerAttribute controllerAttribute,
                                                                                RpcActionAttribute actionAttribute)
            where TRpcController : RpcBaseController<TRequestEntity, TResponseEntity>, new()
        {
            ThrowHelper.ThrowIfNull(typeController, "typeController");
            ThrowHelper.ThrowIfNull(invokeWrapperMethod, "invokeWrapperMethod");
            ThrowHelper.ThrowIfNull(requestEntityConverter, "requestEntityConverter");
            ThrowHelper.ThrowIfNull(getRequestTypeMethod, "getRequestTypeMethod");
            ThrowHelper.ThrowIfNull(methodInfo, "methodInfo");
            ThrowHelper.ThrowIfNull(controllerAttribute, "controllerAttribute");
            ThrowHelper.ThrowIfNull(actionAttribute, "actionAttribute");

            var actionParameter = methodInfo.GetParameters()[0];
            var actionParamType = actionParameter.ParameterType;
            var requestEntityType = typeof(TRequestEntity);
            var responseEntityType = typeof(TResponseEntity);
            var reqContextType = typeof(RpcRequest);
            //Func<TActionParam,TResponseEntity>
            var actionFuncType = Expression.GetFuncType(actionParamType, responseEntityType);

            //var requestEntityConerterFunc = rec => rec.GetRequestEntityType<TActionParams>();
            getRequestTypeMethod = getRequestTypeMethod.MakeGenericMethod(actionParamType);
            var recParameter = Expression.Parameter(typeof(RpcRequestEntityConverter<TRequestEntity>), "rec");
            var requestEntityTypeFuncExpr = Expression.Lambda<Func<RpcRequestEntityConverter<TRequestEntity>, Type>>(
                                                                Expression.Call(recParameter, getRequestTypeMethod),
                                                                recParameter);
            var requestType = requestEntityTypeFuncExpr.Compile()(requestEntityConverter);
            ThrowHelper.ThrowIfFalse(requestType == requestEntityType || requestType.IsSubclassOf(requestEntityType), "requestType ERROR");

            //{Convert(CreateDelegate(System.Func<actionParamType,TResponseEntity>, cc, typeController.ActionMethod))}
            var controllerParameter = Expression.Parameter(typeController, "controller");
            var reqContextParameter = Expression.Parameter(reqContextType, "reqContext");
            var requestEntityParameter = Expression.Parameter(requestEntityType, "requestEntity");
            var actionFuncExpr = Expression.Convert(Expression.Call(typeof(Delegate),
                                                                         "CreateDelegate", null,
                                                                         new Expression[]
                                                                         { 
                                                                            Expression.Constant(actionFuncType), 
                                                                            controllerParameter,
                                                                            Expression.Constant(methodInfo),
                                                                         }),
                                                            actionFuncType);
            invokeWrapperMethod = invokeWrapperMethod.MakeGenericMethod(actionParamType);

            //var iwFunc = (cc, reqContext, requestEntity) => cc.InvokeWrapper(reqContext, requestEntity, cc.ActionMethod));
            var iwCall = Expression.Lambda<Func<TRpcController, RpcRequest, TRequestEntity, TResponseEntity>>(
                                                        Expression.Call(controllerParameter,
                                                                            invokeWrapperMethod,
                                                                            new Expression[] 
                                                                            { 
                                                                                reqContextParameter,
                                                                                requestEntityParameter,
                                                                                actionFuncExpr
                                                                            }),
                                                        new ParameterExpression[]  
                                                        {
                                                            controllerParameter,
                                                            reqContextParameter,
                                                            requestEntityParameter,
                                                        });

            //var iwFunc2 = (reqContext, requestEntity) => iwFunc(new TRpcController(), reqContext, requestEntity);
            var iwCall2 = Expression.Lambda<Func<RpcRequest, TRequestEntity, TResponseEntity>>(
                                                    Expression.Invoke(iwCall, new Expression[]
                                                                                { 
                                                                                    Expression.New(typeController),
                                                                                    reqContextParameter,
                                                                                    requestEntityParameter,
                                                                                }),
                                                    new ParameterExpression[]  
                                                                {
                                                                    reqContextParameter,
                                                                    requestEntityParameter,
                                                                });
            var iwFunc2 = iwCall2.Compile();

            return new RpcActionExecuteHandler<TRequestEntity, TResponseEntity>()
            {
                Metadata = new RpcHandlerMetadata()
                {
                    MethodName = string.Format("{0}.{1}", controllerAttribute.NamePrefix, actionAttribute.ActionName),
                    InputType = requestType,
                    OutputType = responseEntityType,
                    HandlerType = typeController,
                },

                ActionFunc = iwFunc2
            };
        }

        /// <summary>
        /// Get NewHandler Func
        /// </summary>
        /// <typeparam name="TExecuteHandler"></typeparam>
        /// <returns></returns>
        public static Func<TExecuteHandler> GetNewHandlerFunc<TExecuteHandler>()
             where TExecuteHandler : new()
        {
            //Expression<Func<TExecuteHandler>> func = Expression.Lambda<Func<TExecuteHandler>>(Expression.New(typeof(TExecuteHandler)), new ParameterExpression[0]);
            Expression<Func<TExecuteHandler>> func = () => new TExecuteHandler();
            return func.Compile();
        }
        /// <summary>
        /// 获取某类型的指定 特征
        /// </summary>
        /// <typeparam name="TResultAttribute"></typeparam>
        /// <param name="cType"></param>
        /// <returns></returns>
        public static TResultAttribute GetCustomAttributes<TResultAttribute>(this MemberInfo cType)
                where TResultAttribute : Attribute
        {
            var attrs = cType.GetCustomAttributes(typeof(TResultAttribute), false);
            if (attrs == null || attrs.Length <= 0)
                return null;

            return attrs.FirstOrDefault() as TResultAttribute;
        }
    }
}

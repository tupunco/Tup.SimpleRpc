using System;

namespace Tup.SimpleRpc
{
    /// <summary>
    /// 接口文档 注释
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RpcDocAttribute : Attribute
    {
        /// <summary>
        /// 接口 MethodName
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 中文标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 接口描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 输出内容
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// 输入 类型
        /// </summary>
        public Type InputType { get; set; }
        /// <summary>
        /// 输出 类型
        /// </summary>
        public Type OutputType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">中文标题</param>
        /// <param name="methodName">接口 MethodName</param>
        public RpcDocAttribute(string title, string methodName)
            : this(title, methodName, null, null)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">中文标题</param>
        /// <param name="methodName">接口 MethodName</param>
        /// <param name="description">接口描述</param>
        /// <param name="output">输出内容</param>
        public RpcDocAttribute(string title, string methodName, string description, string output)
            : this(title, methodName, description, output, null, null)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">中文标题</param>
        /// <param name="methodName">接口 MethodName</param>
        /// <param name="description">接口描述</param>
        /// <param name="output">输出内容</param>
        /// <param name="inputType">输入 类型</param>
        /// <param name="outputType">输出 类型</param>
        public RpcDocAttribute(string title, string methodName,
            string description, string output,
            Type inputType, Type outputType)
        {
            this.Title = title;
            this.MethodName = methodName;
            this.Description = description;
            this.Output = output;

            this.InputType = inputType;
            this.OutputType = outputType;
        }
    }
}

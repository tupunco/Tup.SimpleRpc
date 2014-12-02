using System;

namespace Tup.SimpleRpc
{
    /// <summary>
    /// Controller Action 注释
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcActionAttribute : Attribute
    {
        /// <summary>
        /// ActionName
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RpcActionAttribute() : this(string.Empty) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="namePrefix"></param>
        public RpcActionAttribute(string actionName)
        {
            this.ActionName = actionName;
        }
    }
    /// <summary>
    /// Controller 注释
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RpcControllerAttribute : Attribute
    {
        /// <summary>
        /// NamePrefix
        /// </summary>
        public string NamePrefix { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RpcControllerAttribute() : this(string.Empty) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="namePrefix"></param>
        public RpcControllerAttribute(string namePrefix)
        {
            this.NamePrefix = namePrefix;
        }
    }

}

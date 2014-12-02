using System;
using System.Collections;
using System.Collections.Generic;

namespace Tup.SimpleRpc
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRpcParameter : IEnumerable<string>, IEnumerable
    {
        string Name { get; }
        string Value { get; }
        List<string> Values { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class RpcParameter : IRpcParameter, IEnumerable<string>, IEnumerable
    {
        private readonly List<string> _values = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get
            {
                if (this._values.Count != 0)
                {
                    return this._values[this._values.Count - 1];
                }
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Values
        {
            get { return this._values; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        public RpcParameter(string name, params string[] values)
        {
            this.Name = name;
            this._values.AddRange(values);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return this._values.GetEnumerator();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._values.GetEnumerator();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[RpcParameter Name:{0}, Value:{1}]", this.Name, this.Value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IRpcParameterCollection : IEnumerable<IRpcParameter>, IEnumerable
    {
        int Count { get; }
        string this[string name] { get; }
        IRpcParameter Get(string name);
        void Add(string name, string value);
        bool Exists(string name);
    }
    /// <summary>
    /// 
    /// </summary>
    public class RpcParameterCollection : IRpcParameterCollection, IEnumerable<IRpcParameter>, IEnumerable
    {
        private readonly Dictionary<string, IRpcParameter> _items = new Dictionary<string, IRpcParameter>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return this._items.Count; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name]
        {
            get
            {
                IRpcParameter parameter = this.Get(name);
                if (parameter == null)
                    return null;

                return parameter.Value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public RpcParameterCollection()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IRpcParameter> GetEnumerator()
        {
            return this._items.Values.GetEnumerator();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IRpcParameter Get(string name)
        {
            IRpcParameter result;
            if (!this._items.TryGetValue(name, out result))
                return null;

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, string value)
        {
            IRpcParameter parameter;
            if (!this._items.TryGetValue(name, out parameter))
            {
                parameter = new RpcParameter(name, new string[]
				{
					value
				});
                this._items.Add(name, parameter);
                return;
            }
            parameter.Values.Add(value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exists(string name)
        {
            return this._items.ContainsKey(name);
        }
    }
}

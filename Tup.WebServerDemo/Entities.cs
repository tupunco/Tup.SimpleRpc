namespace Tup.WebServerDemo
{
    #region 通用 请求/响应 实体
    /// <summary>
    /// 请求 基本实体
    /// </summary>
    public class RequestBody
    {
        /// <summary>
        /// 
        /// </summary>
        public RequestBody()
        {
            this.Token = string.Empty;
        }
        /// <summary>
        /// 用户 Token 信息
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[RequestBody Token:{0}]", this.Token);
        }
    }

    /// <summary>
    /// 请求 附带参数实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RequestBody<TData> : RequestBody
    //    where TData : new()
    {
        /// <summary>
        /// 
        /// </summary>
        public RequestBody()
        {
            //    this.Data = new TData();
        }
        /// <summary>
        /// 参数实体
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[RequestBody<TData> Base:{0}, Data:{1}]",
                                        base.ToString(), this.Data);
        }
    }

    /// <summary>
    /// 响应 基本实体
    /// </summary>
    public class ResponseBody
    {
        public ResponseBody()
            : this(-1, null)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        public ResponseBody(int result, string message)
        {
            this.Result = result;
            this.Message = message ?? string.Empty;
        }
        /// <summary>
        /// 返回码
        /// </summary>
        public int Result { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[ResponseBody Result:{0}, Message:{1}]",
                                    this.Result, this.Message);
        }
    }
    /// <summary>
    /// 响应 附带参数实体
    /// </summary>
    public class ResponseBody<TData> : ResponseBody
    //        where TData : new()
    {
        /// <summary>
        /// 
        /// </summary>
        public ResponseBody()
        {
            //    this.Data = new TData();
        }

        /// <summary>
        /// 附属数据
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[ResponseBody<TData> Base:{0}, Data:{1}]",
                                        base.ToString(), this.Data);
        }
    }
    #endregion
}

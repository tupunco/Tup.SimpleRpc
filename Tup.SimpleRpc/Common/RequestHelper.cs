using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace Tup.SimpleRpc.Common
{
    /// <summary>
    /// HttpRequest 助手
    /// </summary>
    /// <remarks>
    /// POST功能没有详细测试
    /// </remarks>
    public static class RequestHelper
    {
        /// <summary>
        /// Default ContentType
        /// </summary>
        private static readonly string s_Default_ContentType = "application/x-www-form-urlencoded;";
        /// <summary>
        /// PSOT 方式下载指定 URL 的流数据内容, 本方法可指定待下载页面的引用页面/页面编码/下载超时时间/HTTP 代理
        /// </summary>
        /// <param name="url">待下载 URL</param>
        /// <param name="headerReferer">待下载页面的引用页</param>
        /// <param name="isPost">是否 POST 方式下载页面</param>
        /// <param name="postData">POST 方式下载页面的参数</param>
        /// <param name="pageEncoding">待下载页面的页面编码</param>
        /// <param name="timeout">下载页面的超时时间, -1 将忽略本项, 单位:毫秒, 默认值为 100,000 毫秒</param>
        /// <param name="webProxy">当前下载操作使用的 HTTP 代理</param>
        /// <param name="contentType"></param>
        /// <returns>下载得到的页面流数据, 如果下载失败返回 NULL</returns>
        /// <exception cref="ArgumentNullException">url is null</exception>
        public static byte[] DownLoadStream(string url, string headerReferer, bool isPost, byte[] postData, int timeout, string contentType, IWebProxy webProxy)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url", "[url] null...");

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                {
                    request.Accept = "*/*";

                    if (webProxy != null)
                        request.Proxy = webProxy;

                    if (!string.IsNullOrEmpty(headerReferer))
                        request.Referer = headerReferer;

                    if (timeout > 0)
                        request.Timeout = timeout;

                    // request.Headers["Cookie"] = "ASPSESSIONIDSATSACRA=FDAANHLDOGLMEDOMKGOEBHFK";

                    #region 拼接 POST 数据
                    if (isPost)
                    {
                        request.Method = "POST";
                        if (contentType.IsEmpty())
                            request.ContentType = s_Default_ContentType;
                        else
                            request.ContentType = contentType;

                        if (postData != null && postData.Length != 0)
                        {
                            request.ContentLength = postData.Length;

                            using (Stream requestStream = request.GetRequestStream())
                            {
                                requestStream.Write(postData, 0, postData.Length);
                            }
                        }
                        else
                            request.ContentLength = 0L;
                    }
                    #endregion

                    //request.Headers["Accept-Language"] = "zh-cn";
                    //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1) ; InfoPath.2; .NET CLR 2.0.50727; CIBA; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)";
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {

                        using (var resStream = response.ContentEncoding != "gzip"  //解压某些WEB服务器强行响应 GZIP 数据
                                            ? response.GetResponseStream()
                                            : new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                        {
                            return resStream.ReadAsBytes();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("RequestHelper-DownLoadStream-url:{0}-headerReferer:{1}-ex:{2}", url, headerReferer, ex);
                ex = null;
            }
            return null;
        }
    }
}
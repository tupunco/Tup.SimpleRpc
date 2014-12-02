using System;
using System.Text;

using System;
using System.Linq.Expressions;

using Tup.SimpleRpc;
using Tup.SimpleRpc.Common;

namespace Tup.WebServerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Func<int, int> func = (a) => a + 2;
            //Expression<Func<int>> funcInvoker = () => func(12);
            //RpcBookController
            //Expression<Func<RpcBookController, RpcRequest, RequestBody, ResponseBody>> t =
            //    (cc, reqContext, requestEntity) => cc.InvokeWrapper<long>(reqContext, requestEntity, cc.GetBookName);
            var rpcApp = new RpcApplication();

            rpcApp.Register<RpcBookController>();

            var defaultEncoding = Encoding.UTF8;
            var getBookNameData = new RequestBody<long>() { Data = 121L, Token = "===TOKEN---" };
            var jsonBody = JsonNetHelper.SerializeObject(getBookNameData);
            var body = defaultEncoding.GetBytes(jsonBody);
            var res = rpcApp.ExecuteRequest(new RpcRequest()
            {
                AppId = "111",
                MethodName = "book.getBookName",
                Format = "json",
                ContentType = "json",
                Encoding = defaultEncoding,
                Body = body,
                Parameters = new RpcParameterCollection()
            });
            Console.WriteLine(res);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [RpcController(NamePrefix = "book")]
    public class RpcBookController : BaseTestRpcController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [RpcAction("getBookName")]
        public ResponseBody GetBookName(long bookId)
        {
            return new ResponseBody<string>()
            {
                Result = 1,
                Message = "Test Action",
                Data = "Test-BookName-" + bookId,
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [RpcAction("getBookInfo")]
        public ResponseBody GetBookInfo(long bookId)
        {
            return new ResponseBody<BookInfo>()
            {
                Result = 1,
                Message = "Test Action",
                Data = new BookInfo()
                {
                    BookId = bookId,
                    BookName = "Test-BookName-" + bookId,
                    AuthorName = "Test-AuthorName-112-" + bookId / 13,
                }
            };
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class BookInfo
    {
        public long BookId { get; set; }
        public string BookName { get; set; }
        public string AuthorName { get; set; }
    }
}

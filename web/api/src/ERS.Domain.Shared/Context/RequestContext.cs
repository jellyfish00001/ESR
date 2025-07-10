using System;
using System.Threading;

namespace ERS.Context
{
    public class RequestContext
    {
        public string userid { get; set; }
        public string email { get; set; }
        public string token { get; set; }
        public decimal timezone { get; set; }

        /// <summary>
        /// 获取请求上下文
        /// </summary>
        public static RequestContext Current => _asyncLocal.Value;
        private readonly static AsyncLocal<RequestContext> _asyncLocal = new AsyncLocal<RequestContext>();

        /// <summary>
        /// 将请求上下文设置到线程全局区域
        /// </summary>
        /// <param name="userContext"></param>
        public static IDisposable SetContext(RequestContext userContext)
        {
            _asyncLocal.Value = userContext;
            return new RequestContextDisposable();
        }

        /// <summary>
        /// 清除上下文
        /// </summary>
        public static void ClearContext()
        {
            _asyncLocal.Value = null;
        }

    }
    /// <summary>
    /// 用于释放对象
    /// </summary>
    public class RequestContextDisposable : IDisposable
    {
        internal RequestContextDisposable() { }
        public void Dispose()
        {
            RequestContext.ClearContext();
        }
    }
}

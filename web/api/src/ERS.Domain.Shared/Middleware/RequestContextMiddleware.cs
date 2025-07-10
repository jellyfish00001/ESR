using System;
using System.Threading.Tasks;
using ERS.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace ERS.Middleware
{
    /// <summary>
    /// 通过实现IMiddleware，在每个请求时设置用户上下文信息
    /// </summary>
    public class RequestContextMiddleware : IMiddleware
    {
        public IMemoryCache _memoryCache { get; set; }
        public RequestContextMiddleware()
        {
        }

        /// <summary>
        /// 将身份验证后的用户ID存到RequestContext上下文
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public virtual async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var requestContext = new RequestContext();
            using (RequestContext.SetContext(requestContext))
            {
                requestContext.userid = _memoryCache.Get("userId")?.ToString();
                requestContext.email = _memoryCache.Get("email")?.ToString();
                requestContext.token = context.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(context.Request.Headers["timezone"]))
                    requestContext.timezone = Convert.ToDecimal(context.Request.Headers["timezone"]);
                else
                    requestContext.timezone = 8;
                await next(context);
            }
        }
    }
}

using ERS.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ERS.Middleware
{
    public class ExceptionHandleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandleMiddleware> _logger;
        public ExceptionHandleMiddleware(RequestDelegate next, ILogger<ExceptionHandleMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            Result<string> result = new();
            result.status = 1;
            try
            {
                await this._next(httpContext);
            }
            catch(BPMException ex)
            {
                httpContext.Response.StatusCode = 200;
                result.message = "BPM api请求异常";
                result.status = 2;
                result.data = ex.Message;
                await JsonSerializer.SerializeAsync(httpContext.Response.Body, result);
            }
            catch (Exception ex)
            {
                await HandleError(httpContext, ex);
            }
            finally
            {
                var status = httpContext.Response.StatusCode;
            }
        }

        private async Task HandleError(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/problem+json";
            var userId = context.User.Claims.Where(o => o.Type == "sub").FirstOrDefault()?.Value;
            this._logger.LogError("User: {UserId} 操作错误消息: {ErrorMessage}{NewLine}。InnerException: {InnerException}。错误追踪: {StackTrace}",
       userId, ex.Message, Environment.NewLine, ex.InnerException?.ToString(), ex.StackTrace);
            await context.Response.WriteAsync($"操作发生异常:{ex.Message}");
        }
    }
}

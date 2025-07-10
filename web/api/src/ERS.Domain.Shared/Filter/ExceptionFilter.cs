using Microsoft.AspNetCore.Mvc.Filters;
using ERS.Common;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace ERS.Filter
{
    public class ExceptionFilter : IExceptionFilter//AbpExceptionFilter
    {
        //public ExceptionFilter(IErrorInfoBuilder errorInfoBuilder, IAbpAspNetCoreConfiguration configuration, IAbpWebCommonModuleConfiguration abpWebCommonModuleConfiguration) : base(errorInfoBuilder, configuration, abpWebCommonModuleConfiguration)
        //{

        //}

        //protected override int GetStatusCode(ExceptionContext context, bool wrapOnError)
        //{
        //    if (context.Exception is AbpAuthorizationException)
        //    {
        //        if (!context.HttpContext.User.Identity!.IsAuthenticated)
        //        {
        //            return 401;
        //        }

        //        return 403;
        //    }

        //    if (context.Exception is AbpValidationException)
        //    {
        //        return 400;
        //    }

        //    if (context.Exception is EntityNotFoundException)
        //    {
        //        return 404;
        //    }

        //    if (context.Exception is BPMException)
        //    {
        //        return 200;
        //    }

        //    if (wrapOnError)
        //    {
        //        return 500;
        //    }

        //    return context.HttpContext.Response.StatusCode;
        //}

        private readonly ILogger<ExceptionFilter> logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            this.logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            Result<string> result = new();
            result.status = 2;

            if (context.Exception is BPMException)
                result.message = context.Exception.Message;
            else
            {
                logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);
                result.message = "system error!  Please contact system administrator!";
            }
            context.Result = new JsonResult(result);
            context.ExceptionHandled = true;

        }
    }
}

using System;
using System.Linq;
using ERS.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ERS.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string permissionRequired;
        private readonly IConfiguration _config;
        private readonly ILogger<PermissionAttribute> _logger;

        public PermissionAttribute(string somePermissionRequired)
        {
            permissionRequired = somePermissionRequired;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            var authService = serviceProvider.GetService<IAuthService>();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var userId = memoryCache.Get("userId")?.ToString();

            _logger?.LogInformation("Authorization attempt for user {UserId} with permission {PermissionRequired}", userId, permissionRequired);

            if (!await authService.IsUserAuthorized(userId, permissionRequired))
            {
                _logger?.LogWarning("Access denied for user {UserId} with permission {PermissionRequired}", userId, permissionRequired);

                context.Result = new ContentResult
                {
                    Content = "Access denied!",
                    StatusCode = (int)System.Net.HttpStatusCode.Forbidden
                };
            }
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ERS.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PermissionAttributeBK : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string permissionRequired;
        private IConfiguration _config;
        private ILogger<PermissionAttributeBK> _logger;
        /// <summary>
        /// Authorization validation
        /// </summary>
        /// <param name="somePermissionRequired"></param>
        public PermissionAttributeBK(string somePermissionRequired)// : base()
        {
            permissionRequired = somePermissionRequired;
        }
        /// <summary>
        /// Authorization validation, specify authentication scheme with Cookies,Bearer,etc.
        /// </summary>
        /// <param name="somePermissionRequired"></param>
        /// <param name="AuthenticationSchemes"></param>
        public PermissionAttributeBK(string somePermissionRequired, string AuthenticationSchemes)// : base()
        {
            permissionRequired = somePermissionRequired;
            this.AuthenticationSchemes = AuthenticationSchemes;
        }
        /// <summary>
        /// Authorization validation<see cref="AuthorizeAttribute"/>
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            //// 获取 Authorization Header
            //var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            //if (string.IsNullOrEmpty(authorizationHeader))
            //{
            //    // 如果 Token 为空或格式不正确，返回 401 未授权
            //    context.Result = new UnauthorizedResult();
            //    return;
            //}

            //try
            //{
            //    // 验证 Token 是否有效
            //    var tokenHandler =new JwtSecurityTokenHandler();
            //    var validationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true, // 验证过期时间
            //        ValidateIssuerSigningKey = true
            //    };

            //    // 验证 Token
            //    tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            //    // 如果需要，可以从 Token 中提取用户信息
            //    var jwtToken = (JwtSecurityToken)validatedToken;
            //    var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            //    if (string.IsNullOrEmpty(userId))
            //    {
            //        // 如果无法从 Token 中提取用户信息，返回 403 禁止访问
            //        context.Result = new ContentResult
            //        {
            //            Content = "Invalid token: user information missing.",
            //            StatusCode = (int)HttpStatusCode.Forbidden
            //        };
            //        return;
            //    }

            //    // 检查用户权限
            //    if (!IsUserAuthorized(userId, permissionRequired))
            //    {
            //        context.Result = new ContentResult
            //        {
            //            Content = "Access denied!",
            //            StatusCode = (int)HttpStatusCode.Forbidden
            //        };
            //    }
            //}
            //catch (SecurityTokenExpiredException)
            //{
            //    // 如果 Token 已过期，返回 401 未授权
            //    context.Result = new ContentResult
            //    {
            //        Content = "Token has expired.",
            //        StatusCode = (int)HttpStatusCode.Unauthorized
            //    };
            //}
            //catch (SecurityTokenException)
            //{
            //    // 如果 Token 无效，返回 401 未授权
            //    context.Result = new ContentResult
            //    {
            //        Content = "Invalid token.",
            //        StatusCode = (int)HttpStatusCode.Unauthorized
            //    };
            //}
            //catch (Exception ex)
            //{
            //    // 处理其他异常
            //    _logger.LogError(ex, "Token validation failed.");
            //    context.Result = new ContentResult
            //    {
            //        Content = "An error occurred during token validation.",
            //        StatusCode = (int)HttpStatusCode.InternalServerError
            //    };
            //}
        }

        /// <summary>
        /// Validate the user and required permission, via Web API
        /// Config:AppSettings->AuthorizationAPI
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionRequired"></param>
        /// <returns>true:pass validation</returns>
        private bool IsUserAuthorized(string userId, string permissionRequired)
        {
            string AuthApiUrl = _config["IDM:AuthorizationAPI"];

            /*HttpPost*/
            string baseAddress = string.Format(AuthApiUrl, userId, permissionRequired);
            using (var client = new HttpClient())
            {
                List<KeyValuePair<string, string>> hs = new List<KeyValuePair<string, string>>();
                hs.Add(KeyValuePair.Create("userId", userId));
                hs.Add(KeyValuePair.Create("permissionRequired", permissionRequired));
                FormUrlEncodedContent data = new FormUrlEncodedContent(hs);
                var response = Task.Run(() => client.PostAsync(baseAddress, data)).Result;
                _logger.LogDebug("Base address: {BaseAddress}", baseAddress);
                _logger.LogDebug("userId={UserId} & permissionRequired={PermissionRequired}", userId, permissionRequired);
                string result = Task.Run(() => response.Content.ReadAsStringAsync()).Result;
                _logger.LogDebug("Response result: {Result}", result);
                if (result.Equals("true")) return true;
            };

            return false;
        }
    }
}

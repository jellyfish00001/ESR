using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using ERS.IRepositories;
using System.Linq;

namespace ERS
{
    /// <summary>
    /// 实现了基于JWT的自定义认证逻辑（如校验token、查找用户、缓存用户信息等）
    /// </summary>
    public class AzureADAuthenticationHandler : AuthenticationHandler<JwtBearerOptions>
    {
        private const string UserIDClaimType = "unique_name";

        // private readonly ILogger<AzureADAuthenticationHandler> _logger;
        public IEmployeeInfoRepository _employeeInfoRepository;
        private readonly IMemoryCache _memoryCache;

        public AzureADAuthenticationHandler(
            IOptionsMonitor<JwtBearerOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IMemoryCache memoryCache,
             IEmployeeInfoRepository employeeInfoRepository)
            : base(options, logger, encoder, clock)
        {
            _memoryCache = memoryCache;
            _employeeInfoRepository = employeeInfoRepository;
        }

        /// <summary>
        /// 重写 HandleAuthenticateAsync() 方法，实现自己的认证方式（如JWT、Cookie、第三方OAuth等）
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //check header first
            if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                var errorMsg = $"Missing header: {HeaderNames.Authorization}";
                return AuthenticateResult.Fail(errorMsg);
            }

            //get the header and validate
            string authString = Request.Headers[HeaderNames.Authorization];
            if (string.IsNullOrEmpty(authString) || !authString.StartsWith(JwtBearerDefaults.AuthenticationScheme))
            {
                var errorMsg = "Invalid token.";
                Logger.LogWarning(errorMsg);
                return AuthenticateResult.Fail(errorMsg);
            }

            string token = authString.Substring(JwtBearerDefaults.AuthenticationScheme.Length + 1);
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(token);

            if (DateTime.UtcNow < jwtToken.ValidFrom || DateTime.UtcNow > jwtToken.ValidTo)
            {
                var errorMsg = "Invalid token. The access token has expired.";
                Logger.LogWarning(errorMsg);
                return AuthenticateResult.Fail(errorMsg);
            }

            var loginUserEmail = jwtToken.Claims.First(x => x.Type == UserIDClaimType).Value;

            if (string.IsNullOrEmpty(loginUserEmail))
            {
                var errorMsg = "Invalid token. Get user email failed.";
                Logger.LogWarning(errorMsg);
                return AuthenticateResult.Fail(errorMsg);
            }

            // AAD token解析后只有邮箱地址信息，需要串出emplid存到memoryCache
            var emplid = "";
            if (!string.IsNullOrEmpty(loginUserEmail))
            {
                var employeeEntity = await _employeeInfoRepository.QueryByEmail(loginUserEmail);
                emplid = employeeEntity?.emplid;
                if (!string.IsNullOrEmpty(emplid))
                {
                    _memoryCache.Set(loginUserEmail, emplid, TimeSpan.FromDays(1));
                    _memoryCache.Set("userId", emplid, TimeSpan.FromDays(1));
                    _memoryCache.Set("email", loginUserEmail, TimeSpan.FromDays(1));
                    _memoryCache.Set("userName", employeeEntity?.name_a, TimeSpan.FromDays(1));
                }
            }

            if (string.IsNullOrEmpty(emplid))
            {
                var errorMsg = "Invalid token. Get user emplid failed.";
                Logger.LogWarning(errorMsg);
                return AuthenticateResult.Fail(errorMsg);
            }

            var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, emplid)
        };

            var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
        }
    }
}

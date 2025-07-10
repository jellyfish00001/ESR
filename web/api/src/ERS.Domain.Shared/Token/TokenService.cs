namespace ExpenseApplication.Utils
{
    public class TokenService
    {
        // private readonly IMemoryCache _memoryCache;
        // private readonly IConfiguration _configuration;
        // private readonly IHttpClientFactory _httpClientFactory;
        // private DiscoveryCache _cache;

        // public TokenService(IMemoryCache memoryCache, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        // {
        //     _memoryCache = memoryCache;
        //     _configuration = configuration;
        //     _httpClientFactory = httpClientFactory;
        //     _cache = new DiscoveryCache(_configuration.GetSection("ElecDocIDM:Authority").Value.ToString());
        // }

        // public async Task<string> GetTokenAsync()
        // {
        //     if (_memoryCache.TryGetValue<string>("access_token", out string access_token))
        //     {
        //         var tokenExpireTime = _memoryCache.Get<DateTime>("token_expire_time");
        //         if (tokenExpireTime <= System.DateTime.Now)
        //         {
        //             var refresh_token = _memoryCache.Get<string>("refresh_token");
        //             var response = await RefreshTokenAsync(refresh_token);
        //             RemoveTokenCache(response.AccessToken, response.RefreshToken, DateTime.Now);
        //             SetTokenCache(response.AccessToken, response.RefreshToken, DateTime.Now.AddSeconds(response.ExpiresIn));
        //         }
        //     }
        //     else
        //     {
        //         var response = await RequestTokenAsync();
        //         SetTokenCache(response.AccessToken, response.RefreshToken, DateTime.Now.AddSeconds(response.ExpiresIn));
        //     }

        //     return _memoryCache.Get<string>("access_token");
        // }

        // private void RemoveTokenCache(string accessToken, string refreshToken, DateTime tokenExpireTime)
        // {
        //     _memoryCache.Remove("access_token");
        //     _memoryCache.Remove("refresh_token");
        //     _memoryCache.Remove("token_expire_time");
        // }

        // private void SetTokenCache(string accessToken, string refreshToken, DateTime tokenExpireTime)
        // {
        //     _memoryCache.Set<string>("access_token", accessToken);
        //     _memoryCache.Set<string>("refresh_token", refreshToken);
        //     _memoryCache.Set<DateTime>("token_expire_time", tokenExpireTime);
        // }

        // private async Task<TokenResponse> RequestTokenAsync()
        // {
        //     var disco = await _cache.GetAsync();
        //     var _tokenClient = _httpClientFactory.CreateClient();
        //     var response = await _tokenClient.RequestPasswordTokenAsync(new PasswordTokenRequest
        //     {
        //         Address = disco.TokenEndpoint,

        //         ClientId = _configuration.GetSection("ElecDocIDM:ClientId").Value.ToString(),
        //         ClientSecret = "secret",

        //         UserName = _configuration.GetSection("ElecDocIDM:UserName").Value.ToString(),
        //         Password = _configuration.GetSection("ElecDocIDM:Password").Value.ToString(),
        //         Scope = "profile email openid offline_access",
        //     });

        //     if (response.IsError) throw new Exception(response.Error);
        //     return response;
        // }

        // private async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        // {
        //     Console.WriteLine("Using refresh token: {0}", refreshToken);

        //     var disco = await _cache.GetAsync();
        //     var _tokenClient = _httpClientFactory.CreateClient();
        //     var response = await _tokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
        //     {
        //         Address = disco.TokenEndpoint,

        //         ClientId = _configuration.GetSection("ElecDocIDM:ClientId").Value.ToString(),
        //         ClientSecret = "secret",
        //         RefreshToken = refreshToken
        //     });

        //     if (response.IsError) throw new Exception(response.Error);
        //     return response;
        // }
    }
}
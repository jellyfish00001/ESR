using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace ERS.Common
{
    public class IDMHelper : IScopedDependency
    {
        private IConfiguration _configuration;
        private IHttpClientFactory _httpClient;

        public IDMHelper() { }

        public IDMHelper(IConfiguration Configuretion, IHttpClientFactory HttpClient)
        {
            _configuration = Configuretion;
            _httpClient = HttpClient;
        }

        public virtual async Task<IList<string>> GetDataScopeByCompany(string user, string token)
        {
            HttpClient httpClient = _httpClient.CreateClient();
            if (httpClient != null)
                httpClient.DefaultRequestHeaders.Add("Authorization", token);
            FormUrlEncodedContent datap = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
        {
            KeyValuePair.Create<string, string>("userId", user),
            KeyValuePair.Create<string, string>("permissionRequired", _configuration.GetSection("IDM:DataScopeSitePermission").Value)
        });
            HttpResponseMessage response = httpClient.PostAsync($"{_configuration.GetSection("IDM:DataScopeAPI").Value}?userId={user}&permissionRequired={_configuration.GetSection("IDM:DataScopeSitePermission").Value}", datap).Result;
            Console.WriteLine($"get companyCode:(status:{response.StatusCode.ToString()}),user:{user}");
            if (response.StatusCode != HttpStatusCode.OK)
                return null;
            string _result = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(_result))
            {
                Dictionary<string, List<string>> data = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(_result);
                return data["company"];
            }
            return new List<string>();
        }

        //public virtual string GetIDMTokenByPassword(HttpClient HttpClient, string authorityUrl, string clientid, string userName, string password)
        //{
        //    // Get token
        //    var disco = HttpClient.GetDiscoveryDocumentAsync(authorityUrl).Result;
        //    var tokenResponse = HttpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
        //    {
        //        Address = disco.TokenEndpoint,
        //        ClientId = clientid,
        //        ClientSecret = "secret",
        //        Scope = "api1",
        //        GrantType = "password",
        //        UserName = userName,
        //        Password = password
        //    }).Result;

        //    if (tokenResponse.IsError)
        //    {
        //        throw new Exception("IDM token获取失败:" + tokenResponse.Error);
        //    }
        //    string IDMToken = "Bearer " + tokenResponse.AccessToken;
        //    return IDMToken;
        //}
    }
}

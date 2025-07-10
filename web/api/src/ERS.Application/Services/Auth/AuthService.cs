using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ERS.DTO.Auth;
using ERS.IRepositories;
using ERS.Model.MobileSign;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Application.Services;
namespace ERS.Services.Auth
{
    public class AuthService : ApplicationService, IAuthService
    {
        private IConfiguration _configuration;
        private IHttpClientFactory _HttpClient;
        private IAuthMenuRepository _authMenuRepository;
        private IAuthUserRoleRepository _authUserRoleRepository;
        private IAuthRoleMenuRepository _authRoleMenuRepository;
        private IAuthUserCompanyRepository _authUserCompanyRepository;
        private IAuthRoleApiRepository _authRoleApiRepository;
        public AuthService(
            IConfiguration configuration,
            IHttpClientFactory HttpClient,
            IAuthMenuRepository authMenuRepository,
            IAuthRoleMenuRepository authRoleMenuRepository,
            IAuthUserCompanyRepository authUserCompanyRepository,
            IAuthRoleApiRepository authRoleApiRepository,
            IAuthUserRoleRepository authUserRoleRepository
        ){
            _configuration = configuration;
            _HttpClient = HttpClient;
            _authMenuRepository = authMenuRepository;
            _authUserRoleRepository = authUserRoleRepository;
            _authRoleMenuRepository = authRoleMenuRepository;
            _authUserCompanyRepository = authUserCompanyRepository;
            _authRoleApiRepository = authRoleApiRepository;
        }

        public Task<List<string>> GetCompanys(string userId)
        {
           return _authUserCompanyRepository.GetUserCompanys(userId, "Others");
        }
        public async Task<string> GetIDMToken()
        {
            string IDMToken = string.Empty;
            HttpClient httpClient = _HttpClient.CreateClient();
            //获取token
            var formData = new List<KeyValuePair<string, string>>
                {
                new KeyValuePair<string, string>("client_id", "ersww"),
                new KeyValuePair<string, string>("client_secret", _configuration.GetSection("IDM:client_secret").Value),
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username","ers"),
                new KeyValuePair<string, string>("password", "bob"),
                };
            IDMTokenResponse response = await httpClient.PostHelperAsync<IDMTokenResponse>(_configuration.GetSection("IDM:GetIDMToken").Value, formData);
            if (response != null)
            {
                IDMToken = "Bearer " + response.access_token;
            }
            return IDMToken;
        }

        public async Task<bool> IsAuthRole(string userId, List<string> roleKeys)
        {
            List<string> roleList = await _authUserRoleRepository.GetUserRole(userId);
            // 如果 roleList 为 null，则返回 false
            if (roleList == null || roleKeys == null)
            {
                return false;
            }
            // 检查 roleList 和 roleKeys 是否有交集
            return roleList.Any(role => roleKeys.Contains(role));
        }

        public async Task<bool> IsUserAuthorized(string userId,string apikey)
        {
            List<string> apiList = new List<string>();
            List<string> roleList = await _authUserRoleRepository.GetUserRole(userId);
            if (roleList.Count > 0)
            {
                apiList = await _authRoleApiRepository.GetApis(roleList);
                if (apiList.Count>0)
                {
                    return apiList.Contains(apikey);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<List<string>> QueryMenus(string userId)
        {
            // 获取公共菜单
            List<string> menuList = await _authMenuRepository.GetPublicMenus();

            // 获取用户角色
            List<string> roleList = await _authUserRoleRepository.GetUserRole(userId);

            // 定义角色菜单列表
            List<string> roleMenuList = new List<string>();

            // 如果用户有角色，获取角色对应的菜单
            if (roleList.Count > 0)
            {
                roleMenuList = await _authRoleMenuRepository.GetRoleMenus(roleList);
            }

            // 合并菜单列表并去重
            List<string> combinedMenuList = menuList
                .Union(roleMenuList) // 合并两个列表并去重
                .ToList();

            return combinedMenuList;
        }
    }
}
using ERS.DTO;
using ERS.DTO.Permission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
namespace ERS.Services
{
    public class PermissionService : IPermissionService, IScopedDependency
    {
        private ILogger<PermissionService> _logger;
        private IConfiguration _Configuration;
        private IHttpClientFactory _HttpClient;
        public PermissionService(ILogger<PermissionService> logger, IConfiguration Configuration, IHttpClientFactory HttpClient)
        {
            _logger = logger;
            _Configuration = Configuration;
            _HttpClient = HttpClient;
        }
        public Result<IEnumerable<category>> Roles()
        {
            Result<IEnumerable<category>> result = new();
            IEnumerable<category> data = new List<category>()
            {
                new category(){ key = "Finance", value = "Finance"},
                new category(){ key = "Batch", value = "User Batch"},
                new category(){ key = "Admin", value = "Admin"},
                new category(){ key = "HR", value = "HR"},
            };
            result.data = data;
            return result;
        }
        /// <summary>
        /// 开通权限
        /// </summary>
        public async Task<Result<bool>> Add(SelectParams param, string token)
        {
            Result<bool> result = new();
            result.status = 2;
            if (string.IsNullOrEmpty(param.company))
            {
                result.message = "companyCode is not null";
                return result;
            }
            if (string.IsNullOrEmpty(param.role))
            {
                result.message = "role is not null";
                return result;
            }
            if (param.emplid.Count() == 0)
            {
                result.message = "emplid is not null";
                return result;
            }
            Result<IEnumerable<Role>> roles = await this.GetRoles(token);
            if (roles.status != 1)
            {
                result.message = roles.message;
                return result;
            }
            List<string> roleIds = new();
            string roleId = roles.data.Where(i => i.name == "ers." + param.company + "." + param.role).Select(i => i.id).FirstOrDefault();
            if (string.IsNullOrEmpty(roleId))
            {
                result.message = "the role(ers." + param.company + "." + param.role + ") not exist! Please contact the administrator!";
                return result;
            }
            roleIds.Add(roleId);
            string roleId2 = roles.data.Where(i => i.name == "ers." + param.role).Select(i => i.id).FirstOrDefault();
            if (string.IsNullOrEmpty(roleId2))
            {
                result.message = "the role(ers." + param.role + ") not exist! Please contact the administrator!";
                return result;
            }
            roleIds.Add(roleId2);
            result = await AddPermission(param.emplid, roleIds, token);
            if (result.status != 1)
                return result;
            result.message = "success";
            result.status = 1;
            return result;
        }
        public async Task<Result<IEnumerable<UserAcl>>> Permissions(string token)
        {
            Result<IEnumerable<UserAcl>> result = new();
            HttpClient httpClient = _HttpClient.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            HttpResponseMessage response = httpClient.GetAsync($"{_Configuration["IDM:UserAclAPI"]}?clientId={_Configuration["IDM:ClientID"]}").Result;
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                result.status = 2;
                result.message = "Access Denied! Please contact the administrator!";
                return result;
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                result.status = 2;
                result.message = "401! Please contact the administrator!";
                return result;
            }
            string _result = await response.Content.ReadAsStringAsync();
            IEnumerable<UserAcl> json = JsonConvert.DeserializeObject<IEnumerable<UserAcl>>(_result);
            result.data = json;
            return result;
        }
        /// <summary>
        /// 获取权限
        /// </summary>
        public async Task<Result<IEnumerable<RoleResult>>> Query(SelectParams param, string token)
        {
            Result<IEnumerable<RoleResult>> result = new();
            result.status = 2;
            if (string.IsNullOrEmpty(param.company))
            {
                result.message = "companyCode is not null";
                return result;
            }
            if (string.IsNullOrEmpty(param.role))
            {
                result.message = "role is not null";
                return result;
            }
            Result<IEnumerable<Role>> roles = await this.GetRoles(token);
            if (roles.status != 1)
            {
                result.message = roles.message;
                return result;
            }
            string roleId = roles.data.Where(i => i.name == "ers." + param.company + "." + param.role).Select(i => i.id).FirstOrDefault();
            if (string.IsNullOrEmpty(roleId))
            {
                result.message = "the role(ers." + param.company + "." + param.role + ") not exist! Please contact the administrator!";
                return result;
            }
            string userId = "undefined";
            if (param.emplid != null)
            {
                IEnumerable<string> emplidList = param.emplid.Where(i => !string.IsNullOrEmpty(i)).ToList();
                userId = emplidList.Count() == 0 ? "undefined" : emplidList.First();
            }
            // get
            HttpClient httpClient = _HttpClient.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            HttpResponseMessage response = httpClient.GetAsync($"{_Configuration["IDM:GetData"]}?userId={userId}&roleId={roleId}&clientId={_Configuration["IDM:clientId_id"]}").Result;
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                result.status = 2;
                result.message = "Access Denied! Please contact the administrator!";
                return result;
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                result.status = 2;
                result.message = "401! Please contact the administrator!";
                return result;
            }
            string _result = await response.Content.ReadAsStringAsync();
            IEnumerable<RoleResult> json = JsonConvert.DeserializeObject<IEnumerable<RoleResult>>(_result);
            foreach (RoleResult item in json)
            {
                string[] temp = item.roleName.Split(".");
                item.roleName = temp[temp.Length - 1];
            }
            result.status = 1;
            result.data = json;
            return result;
        }
        /// <summary>
        /// 删除权限
        /// </summary>
        public async Task<Result<bool>> Delete(RoleResult param, string token)
        {
            Result<bool> result = new();
            result.status = 2;
            Result<IEnumerable<Role>> roles = await this.GetRoles(token);
            if (roles.status != 1)
            {
                result.message = roles.message;
                return result;
            }
            string common_roleid = roles.data.Where(i => i.name == "ers." + param.role).Select(i => i.id).FirstOrDefault();
            HttpClient httpClient = _HttpClient.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            // common
            if (!string.IsNullOrEmpty(common_roleid))
            {
                HttpResponseMessage _response = httpClient.GetAsync($"{_Configuration["IDM:RemoveUserFromGroup"]}?userId={param.userId}&roleId={common_roleid}").Result;
                if (_response.StatusCode == HttpStatusCode.Forbidden)
                {
                    result.status = 2;
                    result.message = "Access Denied! Please contact the administrator!";
                    return result;
                }
                if (_response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    result.status = 2;
                    result.message = "401! Please contact the administrator!";
                    return result;
                }
                _ = await _response.Content.ReadAsStringAsync();
            }
            // company
            HttpResponseMessage response = httpClient.GetAsync($"{_Configuration["IDM:RemoveUserFromGroup"]}?userId={param.userId}&roleId={param.roleId}").Result;
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                result.status = 2;
                result.message = "Access Denied! Please contact the administrator!";
                return result;
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                result.status = 2;
                result.message = "401! Please contact the administrator!";
                return result;
            }
            _ = await response.Content.ReadAsStringAsync();
            result.status = 1;
            return result;
        }
        async Task<Result<IEnumerable<Role>>> GetRoles(string token)
        {
            Result<IEnumerable<Role>> result = new();
            HttpClient httpClient = _HttpClient.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            HttpResponseMessage response = httpClient.GetAsync(_Configuration["IDM:GetClientGroups"] + "?clientId=" + _Configuration["IDM:clientId_id"]).Result;
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                result.status = 2;
                result.message = "Access Denied! Please contact the administrator!";
                return result;
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                result.status = 2;
                result.message = "401! Please contact the administrator!";
                return result;
            }
            string _result = await response.Content.ReadAsStringAsync();
            RolesList json = JsonConvert.DeserializeObject<RolesList>(_result);
            IEnumerable<Role> roleList = json.roles.Where(i => i.name.StartsWith("ers.")).ToList();
            result.data = roleList;
            result.status = 1;
            return result;
        }
        async Task<Result<bool>> AddPermission(IEnumerable<string> emplidList, List<string> roleIds, string token)
        {
            Result<bool> result = new();
            HttpClient httpClient = _HttpClient.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            foreach (string emplid in emplidList)
            {
                foreach(string roleId in roleIds)
                {
                    HttpResponseMessage response = httpClient.GetAsync($"{_Configuration["IDM:AddUserToGroup"]}?userId={emplid}&roleId={roleId}").Result;
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        result.status = 2;
                        result.message = "Access Denied! Please contact the administrator!";
                        return result;
                    }
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        result.status = 2;
                        result.message = "401! Please contact the administrator!";
                        return result;
                    }
                    string _result = await response.Content.ReadAsStringAsync();
                }
                //// common finance
                //HttpResponseMessage _response = httpClient.GetAsync($"{_Configuration["IDM:AddUserToGroup"]}?userId={emplid}&roleId={_Configuration["IDM:role_id"]}").Result;
                //if (_response.StatusCode == HttpStatusCode.Forbidden)
                //{
                //    result.status = 2;
                //    result.message = "Access Denied! Please contact the administrator!";
                //    return result;
                //}
                //if (_response.StatusCode == HttpStatusCode.Unauthorized)
                //{
                //    result.status = 2;
                //    result.message = "401! Please contact the administrator!";
                //    return result;
                //}
                //_ = await _response.Content.ReadAsStringAsync();
            }
            return result;
        }
        async Task<Result<IEnumerable<RoleResult>>> GetRoles(string roleId, string userId, string token)
        {
            Result<IEnumerable<RoleResult>> result = new();
            HttpClient httpClient = _HttpClient.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            HttpResponseMessage response = httpClient.GetAsync($"{_Configuration["IDM:GetData"]}?userId={userId}&roleId={roleId}&clientId={_Configuration["IDM:clientId_id"]}").Result;
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                result.status = 2;
                result.message = "Access Denied! Please contact the administrator!";
                return result;
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                result.status = 2;
                result.message = "401! Please contact the administrator!";
                return result;
            }
            string _result = await response.Content.ReadAsStringAsync();
            IEnumerable<RoleResult> json = JsonConvert.DeserializeObject<IEnumerable<RoleResult>>(_result);
            result.data = json;
            return result;
        }
    }
}

using ERS.DTO;
using ERS.DTO.Permission;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Controllers
{
    [Route("api/[controller]")]
    public class PermissionController : BaseController
    {
        private IPermissionService _PermissionService;
        public PermissionController(IPermissionService PermissionService)
        {
            _PermissionService = PermissionService;
        }
        /// <summary>
        /// 获取角色
        /// </summary>
        /// <returns></returns>
        [HttpGet("roles")]
        public Result<IEnumerable<category>> Roles() => _PermissionService.Roles();
        /// <summary>
        /// 新增权限
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result<bool>> Add([FromBody]SelectParams param) => await _PermissionService.Add(param, this.token);
        /// <summary>
        /// 查询权限
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("query")]
        public async Task<Result<IEnumerable<RoleResult>>> Query([FromBody] SelectParams param) => await _PermissionService.Query(param, this.token);
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<Result<bool>> Delete([FromBody] RoleResult param) => await _PermissionService.Delete(param, this.token);
        /// <summary>
        /// 获取个人权限
        /// </summary>
        /// <returns></returns>
        [HttpPost("self")]
        public async Task<Result<IEnumerable<UserAcl>>> Permissions() => await _PermissionService.Permissions(this.token);
    }
}

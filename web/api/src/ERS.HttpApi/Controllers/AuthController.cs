using ERS.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private IAuthService _AuthService;
      
        public AuthController(IAuthService iAuthService)
        {
            _AuthService = iAuthService;
        }

        [HttpGet("menus")]
        public async Task<List<string>> QueryMenus()
        {
            return (await _AuthService.QueryMenus(this.userId));
        }

        [HttpGet("getIDMToken")]
        public Task<string> GetIDMToken()
        {
            return _AuthService.GetIDMToken();
        }

        [HttpGet("companys")]
        public Task<List<string>> GetCompanys()
        {
            return _AuthService.GetCompanys(this.userId);
        }
        //判断用户是否有角色List中的一个
        [HttpPost("isAuthRole")]
        public Task<bool> IsAuthRole([FromBody]List<string> roleKeys)
        {
            return _AuthService.IsAuthRole(this.userId, roleKeys);
        }
    }
}
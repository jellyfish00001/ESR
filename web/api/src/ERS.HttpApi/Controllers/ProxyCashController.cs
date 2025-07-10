using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Attribute;
using ERS.DTO;
using ERS.DTO.Proxy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class ProxyCashController : BaseController
    {
        private IProxyCashService _ProxyCashService;
        public ProxyCashController(IProxyCashService ProxyCashService)
        {
            _ProxyCashService = ProxyCashService;
        }
        /// <summary>
        /// 添加報銷人員可代填寫的人員資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Permission("ers.ProxyCash.Add")]
        public async Task<Result<string>> AddProxyCash([FromBody]AddProxyCashDto request)
        {
            return await _ProxyCashService.AddProxyCash(request, this.userId);
        }
        /// <summary>
        /// 查询報銷人員可代填寫的人員資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("query")]
        [Permission("ers.ProxyCash.View")]
        public async Task<Result<List<ProxyCashDto>>> QueryProxyCash([FromBody]Request<QueryProxyCashDto> request)
        {
            return await _ProxyCashService.QueryProxyCash(request, this.userId);
        }
        /// <summary>
        /// 刪除報銷人員可代填寫的人員資料
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("")]
        [Permission("ers.ProxyCash.Delete")]
        public async Task<Result<string>> DeleteProxyCash([FromBody]List<Guid?> ids)
        {
            return await _ProxyCashService.DeleteProxyCash(ids);
        }
        /// <summary>
        /// 編輯報銷人員可代填寫的人員資料
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut("")]
        [Permission("ers.ProxyCash.Edit")]
        public async Task<Result<string>> EditProxyCash([FromBody]EditProxyCashDto request)
        {
            return await _ProxyCashService.EditProxyCash(request, this.userId);
        }
    }
}
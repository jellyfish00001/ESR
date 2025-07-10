using ERS.DTO;
using ERS.DTO.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using Microsoft.AspNetCore.Http;
namespace ERS.Controllers
{
    /// <summary>
    /// 预支延期
    /// </summary>
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class Cash3AController : BaseController
    {
        private ICash3AService _Cash3AService;
        public Cash3AController(ICash3AService Cash3AService)
        {
            _Cash3AService = Cash3AService;
        }
        /// <summary>
        /// 暂存
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost("keep")]
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection) => await _Cash3AService.Keep(formCollection, this.userId, this.token);
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost("submit")]
        public async Task<Result<CashResult>> submit(IFormCollection formCollection) => await _Cash3AService.Submit(formCollection, this.userId, this.token);
        /// <summary>
        /// 个人未冲账预支
        /// </summary>
        /// <returns></returns>
        [HttpPost("DeferredQuery")]
        public async Task<Result<List<DeferredDto>>> DeferredQueryBySelf() => await _Cash3AService.DeferredQuery(this.userId);
        /// <summary>
        /// 个人未冲账预支
        /// </summary>
        /// <returns></returns>
        [HttpPost("DeferredQuery/{user}")]
        public async Task<Result<List<DeferredDto>>> DeferredQueryByUser(string user) => await _Cash3AService.DeferredQuery(user);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDExp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class Cash6Controller : BaseController
    {
        private ICash6Service _Cash6Service;
        public Cash6Controller(ICash6Service Cash6Service)
        {
            _Cash6Service = Cash6Service;
        }
        /// <summary>
        /// 返台會議申請暫存
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost("keep")]
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection) => await _Cash6Service.Keep(formCollection,this.userId, this.token);
        /// <summary>
        /// 返台會議申請提交
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost("submit")]
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection) => await _Cash6Service.Submit(formCollection, this.userId, this.token);
        /// <summary>
        /// 返台會議申请费用类别查询
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost("expscene")]
        public async Task<Result<IEnumerable<BDExpDto>>> GetReturnTaiwanExp(string company) => await _Cash6Service.GetReturnTaiwanExp(company);
    }
}
using ERS.DTO.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERS.DTO;
using Microsoft.AspNetCore.Http;
using ERS.DTO.BDExp;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Domain.Entities.Application;
using ERS.Application.Contracts.DTO.Application;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class Cash3Controller : BaseController
    {
        private ICash3Service _Cash3Service;
        public Cash3Controller(ICash3Service Cash3Service)
        {
            _Cash3Service = Cash3Service;
        }
       [HttpPost("keep")]
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection) => await _Cash3Service.Keep(formCollection, this.userId, this.token);
       [HttpPost("submit")]
        public async Task<Result<CashResult>> submit(IFormCollection formCollection) => await _Cash3Service.Submit(formCollection, this.userId, this.token);
        /// <summary>
        /// 获取未冲账明细
        /// </summary>
        /// <returns></returns>
         [HttpPost("OverdueUser")]
        public async Task<Result<List<OverdueUserDto>>> OverdueBySelf() => await _Cash3Service.OverdueUser(this.userId);
        /// <summary>
        /// 获取未冲账明细
        /// </summary>
        /// <returns></returns>
        [HttpPost("OverdueUser/{user}")]
        public async Task<Result<List<OverdueUserDto>>> OverdueByUser(string user) => await _Cash3Service.OverdueUser(user);
        /// <summary>
        /// 模糊查询预支场景（申请）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpGet("scene/query")]
        public async Task<Result<List<BDExpDto>>> SearchAdvanceScene(string input,string company) => await _Cash3Service.GetAdvanceScene(input,company);
        /// <summary>
        /// 查询预支场景（全部）
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpGet("scenes")]
        public async Task<Result<List<BDExpDto>>> SearchAllAdvanceScene(string company) => await _Cash3Service.GetAllAdvanceScenes(company);
        [HttpGet("scene/filecategory")]
        public async Task<Result<List<BDExpDto>>> GetFilecategoryByExpcode(string expcode, string company, int category) => await _Cash3Service.GetFilecategoryByExpcode(expcode, company, category);
        [HttpGet("payment/type")]
        public async Task<Result<List<BDPayDto>>> GetBDPayType() => await _Cash3Service.GetBDPayType();
        [HttpGet("bpm/attachment")]
        public async Task<Result<string>> GetAttachment(string rno) => await _Cash3Service.GetSignAttachment(rno, this.token);
        [HttpGet("advrno/query")]
        public async Task<Result<List<string>>> GetUnreveseAdvRno(string word, string company) => await _Cash3Service.GetUnreversAdvRno(word,company);
        [HttpPost("ischangeapplicant")]
        public async Task<Result<bool>> IsChangeApplicant(decimal amount, string company) => await _Cash3Service.IsChangeApplicant(this.userId, amount, company);
        /// <summary>
        /// 根据预支金额是否超额，返回不同提示
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost("changePayeeTips")]
        public async Task<Result<ChangePayeeTipsDto>> ChangePayeeTips(decimal amount, string company) => await _Cash3Service.ChangePayeeTips(this.userId, amount, company);
    }
}

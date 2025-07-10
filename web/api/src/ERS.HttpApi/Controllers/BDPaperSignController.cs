using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO.BDPaperSign;
using ERS.Attribute;
using ERS.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class BDPaperSignController : BaseController
    {
        private IBDPaperSignService _BDPaperSignService;
        public BDPaperSignController(IBDPaperSignService BDPaperSignService)
        {
            _BDPaperSignService = BDPaperSignService;
        }
        /// <summary>
        /// 纸本单签核人维护（查询）查询条件：company、plant、emplid
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("query")]
        [Permission("ers.BDPaperSign.View")]
        public async Task<Result<List<PaperSignDto>>> GetPageBDPaperSign([FromBody]Request<QueryPaperSignDto> request)
        {
            return await _BDPaperSignService.QueryBDPaperSign(request);
        }
        /// <summary>
        /// 纸本单签核人维护（新增）company、plant、emplid、company_code必填
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Permission("ers.BDPaperSign.Add")]
        public async Task<Result<string>> AddBDPaperSign([FromBody]AddPaperSignDto request)
        {
            return await _BDPaperSignService.AddBDPaperSign(request, this.userId);
        }
        /// <summary>
        /// 纸本单签核人维护（修改）Id必传、company、plant、emplid、company_code不能为空
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("")]
        [Permission("ers.BDPaperSign.Edit")]
        public async Task<Result<string>> EditBDPaperSign([FromBody]EditPaperSignDto request)
        {
            return await _BDPaperSignService.EditBDPaperSign(request, this.userId);
        }
        /// <summary>
        /// 纸本单签核人维护（删除）仅传Id（多个）
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpDelete("")]
        [Permission("ers.BDPaperSign.Delete")]
        public async Task<Result<string>> DeleteBDPaperSigns([FromBody]List<Guid?> Ids)
        {
            return await _BDPaperSignService.RemoveBDPaperSign(Ids);
        }
    }
}
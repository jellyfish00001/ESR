using ERS.Attribute;
using ERS.DTO;
using ERS.DTO.FinApprover;
using ERS.DTO.Finreview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class FinReviewController : BaseController
    {
        private IFinReviewService _FinreviewService;
        private IFinApproverService _FinApproverService;
        public FinReviewController(
            IFinReviewService FinreviewService,
            IFinApproverService FinApproverService)
        {
            _FinreviewService = FinreviewService;
            _FinApproverService = FinApproverService;
        }
        [HttpGet("query")]
        public async Task<Result<IList<FinReviewDto>>> Finreviews() => await _FinreviewService.GetAllFin(this.userId);
        /// <summary>
        /// 判断当前登录人是否为会计
        /// </summary>
        /// <returns></returns>
        [HttpPost("isaccountant")]
        public async Task<Result<bool>> IsAccountantOrNot() => await _FinreviewService.IsAccountantOrNot(this.userId);
        /// <summary>
        /// 財務簽核人員維護（查詢）
        /// 查詢條件：公司別、公司別代碼、廠別
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("finapprover/query")]
        [Permission("ers.FinReview.View")]
        public async Task<Result<List<FinApproverDto>>> QueryPageFinApprover([FromBody] Request<FinApproverParamsDto> request)
        {
            return await _FinApproverService.QueryPageFinApprover(request);
        }
        /// <summary>
        /// 財務簽核人員維護（新增）
        /// 公司別、公司別代碼、廠別、會計初審1必填
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("finapprover")]
        [Permission("ers.FinReview.Add")]
        public async Task<Result<string>> AddFinApprover([FromBody]AddFinApproverDto request)
        {
            return await _FinApproverService.AddFinApprover(request, this.userId);
        }
        /// <summary>
        /// 財務簽核人員維護（编辑）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("finapprover")]
        [Permission("ers.FinReview.Edit")]
        public async Task<Result<string>> EditFinApprover([FromBody] FinApproverDto request)
        {
            return await _FinApproverService.EditFinApprover(request, this.userId);
        }
        /// <summary>
        /// 財務簽核人員維護（刪除）
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpDelete("finapprover")]
        [Permission("ers.FinReview.Delete")]
        public async Task<Result<string>> DeleteFinApprover([FromBody]List<Guid?> Ids)
        {
            return await _FinApproverService.DeleteFinApprover(Ids);
        }
    }
}

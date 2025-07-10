using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Attribute;
using ERS.DTO;
using ERS.DTO.Auditor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class AuditorController : BaseController
    {
        private IAuditorService _AuditorService;
        public AuditorController(IAuditorService AuditorService)
        {
            _AuditorService = AuditorService;
        }
        /// <summary>
        /// 簽核主管Auditor維護（BD04）查询
        /// 查询参数：只传单据类型代码、工号、公司别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("query")]
        [Permission("ers.Auditor.View")]
        public async Task<Result<List<AuditorDto>>> GetPageAuditors([FromBody]Request<AuditorParamsDto> request)
        {
            return await _AuditorService.GetPageAuditors(request);
        }
        /// <summary>
        /// 簽核主管Auditor維護（BD04）添加
        /// 无需传递Id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Permission("ers.Auditor.Add")]
        public async Task<Result<string>> AddAuditor([FromBody]AuditorParamsDto request)
        {
            return await _AuditorService.AddAuditor(request,this.userId);
        }
        /// <summary>
        /// 簽核主管Auditor維護（BD04）修改
        /// Id必传
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut()]
        [Permission("ers.Auditor.Edit")]
        public async Task<Result<string>> EditAuditor([FromBody]AuditorParamsDto request)
        {
            return await _AuditorService.EditAuditor(request, this.userId);
        }
        /// <summary>
        /// 簽核主管Auditor維護（BD04）删除
        /// Id必传
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpDelete()]
        [Permission("ers.Auditor.Delete")]
        public async Task<Result<string>> DeleteAuditor([FromBody]List<Guid?> Ids)
        {
            return await _AuditorService.DeleteAuditors(Ids);
        }
        /// <summary>
        /// 簽核主管Auditor維護（BD04）
        /// 获取所有单据类型代码与名称
        /// </summary>
        /// <returns></returns>
        [HttpGet("formcodename")]
        public async Task<Result<List<BDFormDto>>> GetEFormCodeName()
        {
            return await _AuditorService.GetEFormCodeName();
        }
    }
}
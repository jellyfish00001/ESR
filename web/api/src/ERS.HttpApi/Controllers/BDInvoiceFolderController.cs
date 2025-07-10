using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDInvoiceFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class BDInvoiceFolderController : BaseController
    {
        private IBDInvoiceFolderService _BDInvoiceFolderService;
        public BDInvoiceFolderController(
            IBDInvoiceFolderService BDInvoiceFolderService
        )
        {
            _BDInvoiceFolderService = BDInvoiceFolderService;
        }
        /// <summary>
        /// 分页查询发票信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("query")]
        public async Task<Result<List<BDInvoiceFolderDto>>> GetPageInvInfo([FromBody] Request<QueryBDInvoiceFolderDto> request)
        {
            return await _BDInvoiceFolderService.GetPageInvInfo(request, this.userId);
        }
        /// <summary>
        /// 获取发票请款状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("paytype")]
        public async Task<Result<List<string>>> GetInvPayTypes()
        {
            return await _BDInvoiceFolderService.GetInvPayTypes();
        }
        /// <summary>
        /// 刪除發票信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task<Result<string>> DeleteInvInfo(Guid id)
        {
            return await _BDInvoiceFolderService.DeleteInvInfo(id, this.userId);
        }
        /// <summary>
        /// 编辑发票信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("edit")]
        public async Task<Result<string>> EditInvInfo([FromBody]InvoiceDto request)
        {
            return await _BDInvoiceFolderService.EditInvInfo(request, this.userId);
        }
        /// <summary>
        /// 獲取個人未請款的發票信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("unpaid")]
        public async Task<Result<List<UnpaidInvInfoDto>>> GetUnpaidInvInfoBySelf(string company)
        {
            return await _BDInvoiceFolderService.GetUnpaidInvInfo(this.userId, company);
        }
        /// <summary>
        /// 獲取某人未請款的發票信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("unpaid/{user}")]
        public async Task<Result<List<UnpaidInvInfoDto>>> GetUnpaidInvInfoByUser(string user, string company) => await _BDInvoiceFolderService.GetUnpaidInvInfo(user, company);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERS.DTO;
using ERS.Attribute;
using ERS.DTO.BDInvoiceRail;
using Microsoft.AspNetCore.Http;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class BDInvoiceRailController : BaseController
    {
        private IBDInvoiceRailService _BDInvoiceRailService;
        public BDInvoiceRailController(IBDInvoiceRailService BDInvoiceRailService)
        {
            _BDInvoiceRailService = BDInvoiceRailService;
        }
        /// <summary>
        /// 发票字轨維護（查詢）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("query")]
        [Permission("ers.BDInvoiceRail.View")]
        public async Task<Result<List<BDInvoiceRailDto>>> GetPageTicketRails([FromBody] Request<QueryBDInvoiceRailDto> request)
        {
            return await _BDInvoiceRailService.GetPageBDInvoiceRails(request);
        }
       
        /// <summary>
        /// 发票字轨維護（刪除）
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [Permission("ers.BDInvoiceRail.Delete")]
        public async Task<Result<string>> DeleteTicketRails([FromBody]List<Guid> Ids)
        {
            return await _BDInvoiceRailService.DeleteBDInvoiceRails(Ids);
        }
        /// <summary>
        /// 批量上传发票字轨
        /// </summary>
        /// <param name="excelFile"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        [Permission("ers.BDInvoiceRail.upload")]
        public async Task<Result<List<AddBDInvoiceRailDto>>> BatchUploadBDInvoiceRail(IFormFile excelFile)
        {
            return await _BDInvoiceRailService.BatchUploadBDInvoiceRail(excelFile, this.userId);
        }
    }
}
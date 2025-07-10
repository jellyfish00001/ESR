using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO.BDInvoiceType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERS.DTO;
using ERS.Attribute;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class BDInvoiceTypeController : BaseController
    {
        private IBDInvoiceTypeService _BDInvoiceTypeService;
        public BDInvoiceTypeController(IBDInvoiceTypeService BDInvoiceTypeService)
        {
            _BDInvoiceTypeService = BDInvoiceTypeService;
        }
        /// <summary>
        /// 發票類型維護（查詢）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("query")]
        [Permission("ers.BDInvoiceType.View")]
        public async Task<Result<List<BDInvoiceTypeDto>>> GetPageInvTypes([FromBody] Request<QueryBDInvTypeDto> request)
        {
            return await _BDInvoiceTypeService.GetPageInvTypes(request);
        }
        /// <summary>
        /// 發票類型維護（添加）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Permission("ers.BDInvoiceType.Add")]
        public async Task<Result<string>> AddInvTypes([FromBody] AddBDInvTypeDto request)
        {
            return await _BDInvoiceTypeService.AddInvTypes(request, this.userId);
        }
        /// <summary>
        /// 發票類型維護（修改）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("")]
        [Permission("ers.BDInvoiceType.Edit")]
        public async Task<Result<string>> EditInvTypes([FromBody]EditBDInvTypeDto request)
        {
            return await _BDInvoiceTypeService.EditInvTypes(request, this.userId);
        }
        /// <summary>
        /// 發票類型維護（刪除）
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpDelete("")]
        [Permission("ers.BDInvoiceType.Delete")]
        public async Task<Result<string>> DeleteInvTypes([FromBody]List<Guid> Ids)
        {
            return await _BDInvoiceTypeService.DeleteInvTypes(Ids);
        }
        /// <summary>
        /// 根據當前登錄人的公司別獲取發票類型
        /// </summary>
        /// <returns></returns>
        [HttpGet("invtypes")]
        public async Task<Result<List<InvoiceTypeDto>>> GetInvTypeByUserId(string company) => await _BDInvoiceTypeService.GetInvTypesByUserCompany(company);

        /// <summary>
        /// 獲取發票類型下拉選項清單
        /// </summary>
        /// <returns></returns>
        [HttpGet("invtypes/list")]
        public async Task<Result<List<InvoiceTypeDto>>> GetAllInvTypes() => await _BDInvoiceTypeService.GetAllInvTypes();
    }
}
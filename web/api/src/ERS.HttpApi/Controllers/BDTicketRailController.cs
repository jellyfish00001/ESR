using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERS.DTO;
using ERS.Attribute;
using ERS.DTO.BDTicketRail;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class BDTicketRailController : BaseController
    {
        private IBDTicketRailService _BDTicketRailService;
        public BDTicketRailController(IBDTicketRailService BDTicketRailService)
        {
            _BDTicketRailService = BDTicketRailService;
        }
        
        [HttpPost("query")]
        [Permission("ers.BDTicketRail.View")]
        public async Task<Result<List<BDTicketRailDto>>> GetPageTicketRails([FromBody] Request<QueryBDTicketRailDto> request)
        {
            return await _BDTicketRailService.GetPageBDTicketRails(request);
        }
        /// <summary>
        /// 车票字轨維護（添加）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Permission("ers.BDTicketRail.Add")]
        public async Task<Result<string>> AddTicketRails([FromBody] AddBDTicketRailDto request)
        {
            return await _BDTicketRailService.AddBDTicketRail(request, this.userId);
        }
        /// <summary>
        /// 车票字轨維護（修改）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("edit")]
        [Permission("ers.BDTicketRail.Edit")]
        public async Task<Result<string>> EditTicketRails([FromBody]EditBDTicketRailDto request)
        {
            return await _BDTicketRailService.EditIBDTicketRail(request, this.userId);
        }
        /// <summary>
        /// 车票字轨維護（刪除）
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [Permission("ers.BDTicketRail.Delete")]
        public async Task<Result<string>> DeleteTicketRails([FromBody]List<Guid> Ids)
        {
            return await _BDTicketRailService.DeleteBDTicketRails(Ids, this.userId);
        }
    }
}
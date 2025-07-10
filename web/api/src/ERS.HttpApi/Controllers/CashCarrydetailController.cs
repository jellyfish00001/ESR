
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using ERS.DTO;
using ERS.DTO.CashCarryDetail;
using System.Collections.Generic;

namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class CashCarrydetailController : BaseController
    {
        private ICashCarryDetailDomainService _iCashCarrydetailDomainService;
        public CashCarrydetailController(ICashCarryDetailDomainService iCashCarrydetailDomainService) 
        { 
            _iCashCarrydetailDomainService = iCashCarrydetailDomainService;
        }

        [HttpPost("UpdateCashCarrydetailData")]
        public async Task<Result<string>> UpdateCashCarrydetailData(IFormCollection formCollection) => await _iCashCarrydetailDomainService.UpdateCashCarrydetail(formCollection);

        [HttpGet("GetCashCarryDetailData")]
        public async Task<Result<List<CashCarryDetailDto>>> GetCarryDetailByRno(string rno) => await _iCashCarrydetailDomainService.GetCarryDetailByRno(rno);
    }
}
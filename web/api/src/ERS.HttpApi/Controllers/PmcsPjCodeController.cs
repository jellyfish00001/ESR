using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.PJcode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERS.Application.Contracts.DTO.PmcsPjcode;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    public class PmcsPjCodeController : BaseController
    {
        private IPmcsPjcodeService _PmcsPjcodeService;
        public PmcsPjCodeController(IPmcsPjcodeService PmcsPjcodeService)
        {
            _PmcsPjcodeService = PmcsPjcodeService;
        }
        [HttpGet("Pjcodes")]
        public Task<List<PjcodeDto>> QueryPjcode(string code, string company)
        {
            return _PmcsPjcodeService.QueryPjcode(code, company);
        }
        [HttpGet("Copy")]
        public Task<string> CopyPjcode(string company)
        {
            return _PmcsPjcodeService.CopyPjcode(company);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.PapreSign;
using ERS.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERS.DTO.PapreSign;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
    #endif
    [Route("api/[controller]")]
    public class PaperController : BaseController
    {
        private IPaperService _paperService;
        public PaperController(IPaperService paperService)
        {
            _paperService = paperService;
        }
        //按工号获取未签核纸本单
        [HttpPost("request")]
        public async Task<Result<List<PaperDto>>> GetQueryPageReport([FromBody]Request<PaperQueryDto> request)
        {
            return await _paperService.GetUnsignPaper(request);
        }
        //纸本单签核
        [HttpPost("sign")]
        public async Task<Result<string>> SignPaper([FromBody]List<string> rno)
        {
            return await _paperService.SignPaper(rno, this.userId, this.token);
        }
    }
}

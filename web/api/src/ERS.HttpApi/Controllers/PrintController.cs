using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Print;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class PrintController : BaseController
    {
        private IPrintService _printService;
        public PrintController(IPrintService printService)
        {
            _printService = printService;
        }
        [HttpPost("query")]
        public async Task<Result<List<PrintDto>>> GetQueryPagePrint([FromBody] Request<PrintQueryDto> request)
        {
            return await _printService.GetQueryPagePrint(request);
        }
        [HttpPost("getprinthtml")]
        public async Task<Result<string>> GetHtmlDocumentAsync([FromBody]List<string> rnolist)
        {
            var result = await _printService.GetPrintAsync(rnolist, this.token);
            return result;
        }


        [HttpPost("storeFormDetails")]
        public Result<string> StoreFormDetails([FromBody] List<string> rnolist)
        {
            return _printService.StoreFormDetails(rnolist, this.token,this.userId);
        }
        // [HttpPost("getpdf")]
        // public async Task<FileResult> DownloadPrint([FromBody]List<string> rnolist)
        // {
        //     byte[] data = null;
        //     data = await _printService.GetPrintPDFData(rnolist);
        //     return File(data,"application/pdf","test.pdf");
        // }
    }
}
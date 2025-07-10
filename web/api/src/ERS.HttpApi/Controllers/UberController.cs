using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.UberFare;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class UberController : BaseController
    {
        public IUberTransactionalService _uberTransactionalService;

        public UberController(IUberTransactionalService iuberTransactionalService)
        {
            _uberTransactionalService = iuberTransactionalService;
        }

        [HttpPost("query")]
        public async Task<Result<UberApplicationDto>> GetQueryApplication([FromBody] Query data) => await _uberTransactionalService.GetQueryApplications(data.rno);

        [HttpPost("submit")]
        public async Task<Result<string>> Submit([FromBody] CashUberHeadDto cashUberHeadDto) => await _uberTransactionalService.Submit(cashUberHeadDto);

        [HttpPost("GetUberTransactional")]
        public async Task<Result<List<UberTransactionalDayDto>>> GetUberTransactional([FromBody]Request<QueryUberTransactionalDto> request)
        {
            return (await _uberTransactionalService.GetUberTransactional(request));
        }

        [HttpPost("uberTransactional/download")]
        public async Task<FileResult> DownloadUberTransactional([FromBody] Request<QueryUberTransactionalDto> request)
        {
            XSSFWorkbook workbook = await _uberTransactionalService.DownloadUberTransactional(request);
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                data = ms.ToArray();
            }
            string sFileName = $"UberTransactional_{TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local).ToString("yyyy-MM-dd")}";
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName + ".xlsx");
        }
    }
}
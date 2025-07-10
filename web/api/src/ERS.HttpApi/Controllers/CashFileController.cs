using ERS.Application.Contracts.DTO.CashFile;
using ERS.Controllers;
namespace ERS.HttpApi.Controllers
{
    public class CashFileController : BaseController
    {
        private ICashFileService _cashFileService;
        public CashFileController(ICashFileService cashFileService)
        {
            _cashFileService = cashFileService;
        }
        //[HttpPost("FileUpload")]
        //public async Task<string> add([FromForm] IFormCollection formCollection)
        //{
        //    return await _cashFileService.add(formCollection ,this.userId);
        //}
        //[HttpGet("Filedelete")]
        //public async Task<string> delete(CashFileDto cash)
        //{
        //    return await _cashFileService.delete(cash);
        //}
    }
}
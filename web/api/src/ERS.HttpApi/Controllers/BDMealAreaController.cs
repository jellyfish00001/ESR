using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDMealArea;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class BDMealAreaController : BaseController
    {
        private IBDMealAreaService _BDMealAreaService;
        public BDMealAreaController(IBDMealAreaService BDMealAreaService)
        {
            _BDMealAreaService = BDMealAreaService;
        }
        /// <summary>
        /// 批量上传公出城市
        /// </summary>
        /// <param name="excelFile"></param>
        /// <returns></returns>
        [HttpPost("batchupload")]
        public async Task<Result<List<UploadBDMealAreaDto>>> BatchUploadBDMealArea(IFormFile excelFile) => await _BDMealAreaService.BatchUploadBDMealArea(excelFile, this.userId);
    }
}
using ERS.DTO;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO.BDCompanyCategory;

namespace ERS.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class BDCompanyCategoryController : BaseController
    {
        public IBDCompanyCategoryDomainService _iBDCompanyCategoryService;
        public BDCompanyCategoryController(IBDCompanyCategoryDomainService iBDCompanyCategoryService)
        {
            _iBDCompanyCategoryService = iBDCompanyCategoryService;
        }

        [HttpPost("view")]
        public async Task<Result<List<BDCompanyCategoryParamDto>>> GetCompanyInfoByKeys([FromBody] Request<BDCompanyCategoryParamDto> model) => await _iBDCompanyCategoryService.Search(model);

        [HttpPut("edit")]
        public async Task<Result<string>> EditCompanyInfo([FromBody] BDCompanyCategoryParamDto request)
        {
            return await _iBDCompanyCategoryService.Update(request, this.userId);
        }

        [HttpPost("add")]
        public async Task<Result<string>> AddCompanyInfo([FromBody] BDCompanyCategoryParamDto request)
        {
            return await _iBDCompanyCategoryService.Create(request, this.userId);
        }

        [HttpDelete("delete")]
        public async Task<Result<string>> DeleteCompanyInfo([FromBody] List<string> ids)
        {
            return await _iBDCompanyCategoryService.Delete(ids);
        }

        [HttpGet("GetCategoryListByUserId")]
        public async Task<Result<List<BDCompanyCategoryParamDto>>> GetCategoryListByUserId(string userId) => await _iBDCompanyCategoryService.GetCategoryListByUserId(userId);

        [HttpGet("list")]
        public async Task<Result<List<BDCompanyCategoryParamDto>>> GetAllCompanyCategoryList() => await _iBDCompanyCategoryService.GetAllCompanyCategoryList();

        [HttpGet("areas")]
        public async Task<Result<List<string>>> GetAllAreaList() => await _iBDCompanyCategoryService.GetAllAreaList();

        [HttpGet("user-area")]
        public async Task<Result<string>> GetCompanyCategoryArea()
        {
            return await _iBDCompanyCategoryService.GetCompanyCategoryArea(this.userId);
        }

    }
}
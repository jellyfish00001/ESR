using ERS.Attribute;
using ERS.DTO;
using ERS.DTO.BDCompanyCategory;
using ERS.DTO.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    public class CompanyController : BaseController
    {
        private ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        /// <summary>
        /// 根据地区获取公司别
        /// </summary>
        /// <returns></returns>
        [HttpGet("area/companys")]
        public async Task<Result<List<string>>> GetComapnyByArea() => await _companyService.GetCompanyByArea(this.userId);
        /// <summary>
        /// 获取全部公司别(申请使用)
        /// </summary>
        /// <returns></returns>
        [HttpGet("companys")]
        public async Task<Result<List<string>>> GetAllComapnyAsync() => await _companyService.GetAllComapnyAsync(this.userId);
        /// <summary>
        /// 根据权限获取对应公司别（basedata用）
        /// </summary>
        /// <returns></returns>
        [HttpGet("permission/companys")]
        public async Task<Result<List<string>>> GetPermissionComapnyAsync() => await _companyService.GetPermissionComapnyAsync(this.userId, this.token);
        /// <summary>
        /// 公司别维护（查询）
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost("companyinfo")]
        //[Permission("ers.Company.View")]
        public async Task<Result<List<BDCompanyCategoryParamDto>>> GetComsInfoByCompany([FromBody] Request<List<string>> company) => await _companyService.GetCompaniesInfo(company);
        /// <summary>
        /// 公司别维护（新增）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("companyinfo/add")]
        [Permission("ers.Company.Add")]
        public async Task<Result<string>> AddNewCompanyInfo([FromBody]ComInfoDto request)
        {
            return await _companyService.AddCompanyInfo(request,this.userId);
        }
        /// <summary>
        /// 公司别维护（编辑）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("companyinfo")]
        [Permission("ers.Company.Edit")]
        public async Task<Result<string>> EditCompanyInfo([FromBody] BDCompanyCategoryParamDto request)
        {
            return await _companyService.EditCompanyInfo(request, this.userId);
        }
        /// <summary>
        /// 公司别维护（删除）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("companyinfo")]
        [Permission("ers.Company.Delete")]
        public async Task<Result<string>> DeleteCompanyInfo([FromBody]List<BDCompanyCategoryParamDto> request)
        {
            return await _companyService.DeleteCompanyInfo(request);
        }
        /// <summary>
        /// 根據公司別獲取對應的companycode
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost("companycodes")]
        public async Task<Result<List<string>>> QueryCompanyCodeByCompany(string company)
        {
            return await _companyService.GetCompanyCodeByCompany(company);
        }

        /// <summary>
        /// 根据地区获取公司别
        /// </summary>
        /// <returns></returns>
        [HttpGet("site/companys")]
        public async Task<Result<List<string>>> GetComapnyBySite() => await _companyService.GetCompanyBySite(this.userId);
    }
}

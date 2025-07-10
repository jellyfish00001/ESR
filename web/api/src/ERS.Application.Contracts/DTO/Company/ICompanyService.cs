using ERS.DTO.BDCompanyCategory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace ERS.DTO.Company
{
    public interface ICompanyService : ICrudAppService<CompanyDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateCompanyDto>
    {
        Task<Result<List<string>>> GetAllComapnyAsync(string user);
        Task<Result<List<BDCompanyCategoryParamDto>>> GetCompaniesInfo(Request<List<string>> company);
        Task<Result<string>> AddCompanyInfo(ComInfoDto request, string userId);
        Task<Result<string>> EditCompanyInfo(BDCompanyCategoryParamDto request, string userId);
        Task<Result<string>> DeleteCompanyInfo(List<BDCompanyCategoryParamDto> request);
        Task<Result<List<string>>> GetPermissionComapnyAsync(string user, string token);
        Task<Result<List<string>>> GetCompanyByArea(string user);
        Task<Result<List<string>>> GetCompanyCodeByCompany(string company);
        Task<Result<List<string>>> GetCompanyBySite(string user);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDCompanyCategory;
using ERS.DTO.Company;
using ERS.Entities;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface ICompanyDomainService : IDomainService
    {
        Task<Result<List<string>>> GetAllComapnyAsync(string user);
        //通过code查询company信息
        Task<BDCompanyCategory> GetCompany(string code);
        Task<BDCompanyCategory> GetCompanybySite(string site);
        Task<List<BDCompanyCategory>> GetConpanyCodeBySite(string site);
        //公司别维护界面按公司别查询信息
        Task<Result<List<BDCompanyCategoryParamDto>>> GetCompaniesInfo(Request<List<string>> request);
        //公司别维护界面新增公司别信息
        Task<Result<string>> AddCompanyInfo(ComInfoDto request, string userId);
        //公司别维护界面编辑公司别信息
        Task<Result<string>> EditCompanyInfo(BDCompanyCategoryParamDto request, string userId);
        //公司别维护界面删除公司别信息
        Task<Result<string>> DeleteCompanyInfo(List<BDCompanyCategoryParamDto> request);
        Task<Result<List<string>>> GetCompanyByArea(string user);
        Task<Result<List<string>>> GetCompanyBySite(string user);
    }
}

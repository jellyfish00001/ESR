using Abp.Domain.Services;
using ERS.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO.BDCompanyCategory;

namespace ERS.IDomainServices
{
    public interface IBDCompanyCategoryDomainService : IDomainService
    {
        Task<Result<string>> Update(BDCompanyCategoryParamDto en, string userId);
        Task<Result<string>> Create(BDCompanyCategoryParamDto model, string userId);
        Task<Result<string>> Delete(List<string> ids);
        Task<Result<List<BDCompanyCategoryParamDto>>> Search(Request<BDCompanyCategoryParamDto> request);
        Task<Result<List<BDCompanyCategoryParamDto>>> GetCategoryListByUserId(string userId);
        Task<Result<List<BDCompanyCategoryParamDto>>> GetAllCompanyCategoryList();
        Task<Result<List<string>>> GetAllAreaList();
        Task<Result<string>> GetCompanyCategoryArea(string userId);
    }
}
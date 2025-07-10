using ERS.DTO;
using ERS.DTO.BDCompanyCategory;
using ERS.DTO.Company;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
namespace ERS.Services
{
    public class CompanyService : CrudAppService<BDCompanyCategory, CompanyDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateCompanyDto>, ICompanyService
    {
        private IRepository<BDCompanyCategory, Guid> _repository;
        private ICompanyDomainService _CompanyDomainService;
        private Common.IDMHelper _idmHelper;
        public CompanyService(IRepository<BDCompanyCategory, Guid> repository, ICompanyDomainService CompanyDomainService, Common.IDMHelper idmHelper) : base(repository)
        {
            _repository = repository;
            LocalizationResource = typeof(ERSResource);
            _CompanyDomainService = CompanyDomainService;
            _idmHelper = idmHelper;
        }
        public async Task<Result<List<string>>> GetCompanyByArea(string user)
        {
            return await _CompanyDomainService.GetCompanyByArea(user);
        }
        public async Task<Result<List<string>>> GetAllComapnyAsync(string user)
        {
            return await _CompanyDomainService.GetAllComapnyAsync(user);
        }
        public async Task<Result<List<string>>> GetPermissionComapnyAsync(string user, string token)
        {
            Result<List<string>> result = new();
            List<string> list = (await _CompanyDomainService.GetAllComapnyAsync(user)).data.ToList();
            IList<string> pList = await _idmHelper.GetDataScopeByCompany(user, token);
            if(pList.Count == 0)
                pList = await _idmHelper.GetDataScopeByCompany(user.ToLower(), token);
            pList = pList.Where(i => !string.IsNullOrEmpty(i)).ToList();
            if (pList == null || pList.Count() == 0)
                result.data = new List<string>();
            else if (pList.Contains("ALL"))
                result.data = list;
            else if (pList.Count() > 0)
            {
                // list = pList.Intersect(list).ToList();
                result.data = pList.ToList();
            }
            return result;
        }
        public async Task<Result<List<BDCompanyCategoryParamDto>>> GetCompaniesInfo(Request<List<string>> company)
        {
            return await _CompanyDomainService.GetCompaniesInfo(company);
        }
        public async Task<Result<string>> AddCompanyInfo(ComInfoDto request, string userId)
        {
            return await _CompanyDomainService.AddCompanyInfo(request, userId);
        }
        public async Task<Result<string>> EditCompanyInfo(BDCompanyCategoryParamDto request, string userId)
        {
            return await _CompanyDomainService.EditCompanyInfo(request, userId);
        }
        public async Task<Result<string>> DeleteCompanyInfo(List<BDCompanyCategoryParamDto> request)
        {
            return await _CompanyDomainService.DeleteCompanyInfo(request);
        }
        public async Task<Result<List<string>>> GetCompanyCodeByCompany(string company)
        {
            Result<List<string>> result = new();
            result.data = (await _repository.WithDetailsAsync()).Where(w => w.company == company).Select(w => w.CompanyCategory).ToList();
            result.total = result.data.Count;
            return result;
        }

        public async Task<Result<List<string>>> GetCompanyBySite(string user)
        {
            return await _CompanyDomainService.GetCompanyBySite(user);
        }
    }
}

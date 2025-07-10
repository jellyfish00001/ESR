using ERS.DTO.Company;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using ERS.DTO;
using Microsoft.EntityFrameworkCore;
using ERS.DTO.BDCompanyCategory;

namespace ERS.DomainServices
{
    public class CompanyDomainService : CommonDomainService, ICompanyDomainService
    {
        private IRepository<BDCompanyCategory, Guid> _companyRepository;
        private IEmployeeDomainService _EmployeeDomainService;
        private ICompanyRepository _CompanyRepository;
        private IObjectMapper _ObjectMapper;
        private IEmployeeRepository _EmployeeRepository;
        private IBDExpRepository _BDExpRepository;
        public CompanyDomainService(
            IRepository<BDCompanyCategory, Guid> companyRepository,
            IEmployeeDomainService EmployeeDomainService,
            ICompanyRepository CompanyRepository,
            IEmployeeRepository EmployeeRepository,
            IBDExpRepository BDExpRepository,
            IObjectMapper ObjectMapper)
        {
            _companyRepository = companyRepository;
            _CompanyRepository = CompanyRepository;
            _EmployeeDomainService = EmployeeDomainService;
            _ObjectMapper = ObjectMapper;
            _EmployeeRepository = EmployeeRepository;
            _BDExpRepository = BDExpRepository;
        }
        public async Task<Result<List<string>>> GetAllComapnyAsync(string user)
        {
            //string companyCode = "";
            //if (!string.IsNullOrEmpty(user))
            //    companyCode = await _EmployeeDomainService.GetCompanyCodeAsync(user);
            //if (!string.IsNullOrEmpty(companyCode))
            //{
            //    var tem = (await _companyRepository.WithDetailsAsync()).Select(i => new { i.company, i.companycode }).ToList();
            //    bool isContains = tem.Select(i => i.companycode).ToList().Contains(companyCode);
            //    if (isContains)
            //    {
            //        IList<string> companys = tem.Select(i => i.company).ToList().Except(new[] { companyCode }).ToList();
            //        companys.Insert(0, companyCode);
            //        return companys;
            //    }
            //    else
            //        return tem.Select(i => i.company).ToList();
            //}
            //else
            Result<List<string>> result = new();
            result.data = (await _companyRepository.WithDetailsAsync()).AsNoTracking().Select(i => i.company).Distinct().ToList();
            return result;
        }
        public async Task<Result<List<string>>> GetCompanyByArea(string user)
        {
            Result<List<string>> result = new();
            string companyCode = await _EmployeeRepository.GetCompanyCodeByUser(user);
            List<BDCompanyCategory> companies = await (await _CompanyRepository.WithDetailsAsync()).AsNoTracking().ToListAsync();
            string area = companies.Where(i => i.CompanyCategory == companyCode).Select(i => i.Area).FirstOrDefault();
            result.data = companies.Where(i => i.Area == area).Select(i => i.company).Distinct().ToList();
            result.message = "testError";
            return result;
        }
        public async Task<BDCompanyCategory> GetCompany(string code)
        {
            return (await _companyRepository.WithDetailsAsync()).Where(b => b.CompanyCategory == code).FirstOrDefault();
        }
        public async Task<BDCompanyCategory> GetCompanybySite(string site)
        {
            return (await _companyRepository.WithDetailsAsync()).Where(b => b.company == site).FirstOrDefault();
        }
        public async Task<List<BDCompanyCategory>> GetConpanyCodeBySite(string site)
        {
            return (await _companyRepository.WithDetailsAsync()).Where(w => w.company == site).ToList();
        }
        public async Task<Result<List<BDCompanyCategoryParamDto>>> GetCompaniesInfo(Request<List<string>> request)
        {
            List<BDCompanyCategoryParamDto> resultdata = new List<BDCompanyCategoryParamDto>();
            List<BDCompanyCategory> query = new List<BDCompanyCategory>();
            //query = (await _CompanyRepository.GetComInfoByCompany(request.data));
            query = (await _CompanyRepository.WithDetailsAsync()).Where(w => request.data.Contains(w.CompanyCategory)).ToList();
            resultdata = _ObjectMapper.Map<List<BDCompanyCategory>, List<BDCompanyCategoryParamDto>>(query);
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            int count = resultdata.Count;
            Result<List<BDCompanyCategoryParamDto>> result = new Result<List<BDCompanyCategoryParamDto>>()
            {
                data = new List<BDCompanyCategoryParamDto>()
            };
            result.data = resultdata.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = count;
            return result;
        }
        public async Task<Result<string>> AddCompanyInfo(ComInfoDto request, string userId)
        {
            Result<string> result = new Result<string>();
            if (string.IsNullOrEmpty(request.company))
            {
                result.message = L["AddFail"] + "：" + L["CompanyNotEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.companycode))
            {
                result.message = L["AddFail"] + "：" + L["CompanyCodeNotEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.companysap))
            {
                result.message = L["AddFail"] + "：" + L["SapCompanyNotEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.companydesc))
            {
                result.message = L["AddFail"] + "：" + L["CompanyDescNotEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.basecurr))
            {
                result.message = L["AddFail"] + "：" + L["CurrencyNotEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.stwit))
            {
                result.message = L["AddFail"] + "：" + L["BDCompany-StwitNotEmpty"];
                result.status = 2;
                return result;
            }
            var check = (await _CompanyRepository.WithDetailsAsync()).Where(w => w.company == request.company && w.CompanyCategory == request.companycode).ToList();
            if (check.Count > 0)
            {
                result.message = L["AddFail"] + "：" + L["DataAlreadyexist"];
                result.status = 2;
                return result;
            }
            BDCompanyCategory newCompany = _ObjectMapper.Map<ComInfoDto, BDCompanyCategory>(request);
            newCompany.cdate = System.DateTime.Now;
            newCompany.cuser = userId;
            newCompany.mdate = System.DateTime.Now;
            newCompany.muser = userId;
            await _CompanyRepository.InsertAsync(newCompany);
            BdExp bdExp = new BdExp()
            {
                expcode = "EXP02",
                expname = "交際費",
                classno = "A1",
                category = 1,
                company = request.company,
                cuser = userId,
                cdate = System.DateTime.Now
            };
            await _BDExpRepository.InsertAsync(bdExp);
            result.message = L["AddSuccess"];
            return result;
        }
        public async Task<Result<string>> EditCompanyInfo(BDCompanyCategoryParamDto request, string userId)
        {
            Result<string> result = new Result<string>();
            if (string.IsNullOrEmpty(request.Company))
            {
                result.message = L["SaveFailMsg"] + "：" + L["CompanyNotEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.CompanyCategory))
            {
                result.message = L["SaveFailMsg"] + "：" + L["CompanyCategoryNotEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.CompanySap))
            {
                result.message = L["SaveFailMsg"] + "：" + L["SapCompanyNotEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.CompanyDesc))
            {
                result.message = L["SaveFailMsg"] + "：" + L["CompanyDescNotEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.BaseCurrency))
            {
                result.message = L["SaveFailMsg"] + "：" + L["CurrencyNotEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.Stwit))
            {
                result.message = L["SaveFailMsg"] + "：" + L["BDCompany-StwitNotEmpty"];
                result.status = 2;
                return result;
            }
            BDCompanyCategory company = (await _CompanyRepository.GetCompanyById(Guid.Parse(request.Id)));
            if (company == null)
            {
                result.message = L["SaveFailMsg"] + "：" + L["CompanyNotExist"];
                result.status = 2;
                return result;
            }
            company.company = request.Company;
            company.CompanyCategory = request.CompanyCategory;
            company.CompanySap = request.CompanySap;
            company.CompanyDesc = request.CompanyDesc;
            company.BaseCurrency = request.BaseCurrency;
            company.Area = request.Area;
            company.IdentificationNo = request.IdentificationNo;
            company.IncomeTaxRate = request.IncomeTaxRate;
            company.TimeZone = request.TimeZone;
            company.Stwit = request.Stwit;
            company.muser = userId;
            company.mdate = System.DateTime.Now;
            await _CompanyRepository.UpdateAsync(company);
            result.message = L["SaveSuccessMsg"];
            return result;
        }
        public async Task<Result<string>> DeleteCompanyInfo(List<BDCompanyCategoryParamDto> request)
        {
            Result<string> result = new Result<string>();
            List<BDCompanyCategory> companies = new List<BDCompanyCategory>();
            foreach (var item in request)
            {
                BDCompanyCategory company = (await _CompanyRepository.GetCompanyById(Guid.Parse(item.Id)));
                if (company == null)
                {
                    result.message = L["DeleteFail"] + "：" + L["CompanyInfoNotFound"] + "：" + item.Company;
                    result.status = 2;
                    return result;
                }
                companies.Add(company);
            }
            await _CompanyRepository.DeleteManyAsync(companies);
            result.message = L["DeleteSuccess"];
            return result;
        }

        public async Task<Result<List<string>>> GetCompanyBySite(string user)
        {
            Result<List<string>> result = new();
            string companyCode = await _EmployeeRepository.GetCompanyCodeByUser(user);
            List<BDCompanyCategory> companies = await(await _CompanyRepository.WithDetailsAsync()).AsNoTracking().ToListAsync();
            string area = companies.Where(i => i.CompanyCategory == companyCode).Select(i => i.Area).FirstOrDefault();
            result.data = companies.Where(i => i.Area == area).Select(i => i.company).Distinct().ToList();
            result.message = "testError";
            return result;
        }
    }
}

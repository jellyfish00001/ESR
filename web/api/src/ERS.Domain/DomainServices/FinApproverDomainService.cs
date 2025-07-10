using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.FinApprover;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.Entities;
using Volo.Abp.ObjectMapping;
namespace ERS.DomainServices
{
    /// <summary>
    /// 財務簽核人員維護（BD02）
    /// </summary>
    public class FinApproverDomainService : CommonDomainService, IFinApproverDomainService
    {
        private IFinreviewRepository _FinreviewRepository;
        //private IEmployeeRepository _EmployeeRepository;
        private IEmployeeInfoRepository _EmployeeInfoRepository;
        private IObjectMapper _ObjectMapper;
        public FinApproverDomainService(
            IFinreviewRepository FinreviewRepository,
            //IEmployeeRepository EmployeeRepository,
            IEmployeeInfoRepository EmployeeInfoRepository,
            IObjectMapper ObjectMapper
        )
        {
            _FinreviewRepository = FinreviewRepository;
           //_EmployeeRepository = EmployeeRepository;
            _EmployeeInfoRepository = EmployeeInfoRepository;
            _ObjectMapper = ObjectMapper;
        }
        public async Task<Result<List<FinApproverDto>>> QueryPageFinApprover(Request<FinApproverParamsDto> request)
        {
            Result<List<FinApproverDto>> result = new Result<List<FinApproverDto>>();
            
            //先按filter挑出符合條件的Finreview
            List<Finreview> finreview = (await _FinreviewRepository.WithDetailsAsync())
                        .Where(w => request.data.companyList.Contains(w.company))
                        .WhereIf(request.data.category!=-1, w => w.category == request.data.category)
                        .WhereIf(request.data.signStep!=-1 && request.data.signStep == 0, w => w.rv1 != null)
                        .WhereIf(request.data.signStep!=-1 && request.data.signStep == 1, w => w.rv2 != null)
                        .ToList();

            //再找出所有搜尋結果的工號
            var relevantEmplids = finreview
                .SelectMany(fr => new[] { fr.rv1, fr.rv2 })
                .Where(id => !string.IsNullOrEmpty(id))  // Filter out null ids
                .Distinct()  // Leverage Distinct to avoid duplicates
                .ToList();

            //員工主檔資料太多，只取有關的以減少資料量
            List<EmployeeInfo> emp = (await _EmployeeInfoRepository.WithDetailsAsync())
                                 .Where(e => relevantEmplids.Contains(e.emplid))  // Filter relevant employees
                                 .ToList();

            //最後再將finreview的rv1, rv2資料從Employee抓出來
            var finalQuery = (from f in finreview
                       let rv1e = emp.FirstOrDefault(e => e.emplid == f.rv1) 
                       let rv2e = emp.FirstOrDefault(e => e.emplid == f.rv2)
                    select new FinApproverDto{
                      Id = f.Id,
                      category = f.category,  
                      company = f.company,
                      plant = f.plant,
                      rv1 = rv1e!=null ? f.rv1 + " / " + (!rv1e.name_a.IsNullOrEmpty() ? rv1e.name_a :rv1e.name) : "",
                      rv2 = rv2e!=null ? f.rv2 + " / " + (!rv2e.name_a.IsNullOrEmpty() ? rv2e.name_a :rv2e.name) : "",
                      muser = f.muser,
                      mdate = f.mdate,
                    }).ToList();

            List<FinApproverDto> mapData = _ObjectMapper.Map<List<FinApproverDto>, List<FinApproverDto>>(finalQuery);
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            int count = mapData.Count;
            result.data = mapData.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = mapData.Count;
            return result;
        }
        public async Task<Result<string>> AddFinApprover(AddFinApproverDto request, string userId)
        {
            Result<string> result = new Result<string>();
            if (request.company.IsNullOrEmpty())
            {
                result.message = L["AddFail"] + "：" + L["CompanyNotEmpty"];
                result.status = 2;
                return result;
            }
            /*if (string.IsNullOrEmpty(request.company_code))
            {
                result.message = L["AddFail"] + "：" + L["CompanyCodeNotEmpty"];
                result.status = 2;
                return result;
            }*/
            if (string.IsNullOrEmpty(request.rv1) && string.IsNullOrEmpty(request.rv2))
            {
                result.message = L["AddFail"] + "：" + L["Rv1NotEmpty"] + " and " + L["Rv1NotEmpty"];
                result.status = 2;
                return result;
            }
            if(!string.IsNullOrEmpty(request.rv1)){
              bool rv1IsExist = (await _EmployeeInfoRepository.EmpIsExist(request.rv1));
              if (rv1IsExist == false){
                result.message = L["AddFail"] + "：" + L["Rv1NotExist"];
                result.status = 2;
                return result;
              }
            }else if(!string.IsNullOrEmpty(request.rv2)){
              bool rv2IsExist = (await _EmployeeInfoRepository.EmpIsExist(request.rv2));
              if (rv2IsExist == false){
                result.message = L["AddFail"] + "：" + L["Rv2NotExist"];
                result.status = 2;
                return result;
              }
            }            
            
            //做重复校验
            if (await IsExistFinApprover(request))
            {
                result.message = L["AddFail"] + "：" + L["FinApproverExist"];
                result.status = 2;
                return result;
            }
            Finreview newFinreview = _ObjectMapper.Map<AddFinApproverDto, Finreview>(request);
            newFinreview.cdate = System.DateTime.Now;
            newFinreview.cuser = userId;
            newFinreview.mdate = System.DateTime.Now;
            newFinreview.muser = userId;
            if (string.IsNullOrEmpty(request.rv2))
            {
                newFinreview.rv2 = "";
            }
            if (string.IsNullOrEmpty(request.rv3))
            {
                newFinreview.rv3 = "";
            }
            if (string.IsNullOrEmpty(request.plant))
            {
                if (request.category == 1)
                {
                    newFinreview.plant = "ALL";
                }
                else
                {
                    result.message = L["AddFail"] + "：" + L["PlantNotEmpty"];
                    result.status = 2;
                    return result;
                }
            }
            await _FinreviewRepository.InsertAsync(newFinreview);
            result.message = L["AddSuccess"];
            return result;
        }
        public async Task<bool> IsExistFinApprover(AddFinApproverDto input)
        {
            var result = false;
            var query = (await _FinreviewRepository.WithDetailsAsync())
            .Where(
                w => w.company == input.company &&
                w.company_code == input.company_code &&
                w.plant == input.plant
            )
            .WhereIf(!string.IsNullOrEmpty(input.rv2), w => w.rv2 == input.rv2)
            .WhereIf(!string.IsNullOrEmpty(input.rv3), w => w.rv2 == input.rv3)
            .FirstOrDefault();
            if (query != null)
            {
                result = true;
            }
            return result;
        }
        public async Task<Result<string>> EditFinApprover(FinApproverDto request, string userId)
        {
            Result<string> result = new Result<string>();
            Finreview finreview = (await _FinreviewRepository.GetFinreviewById(request.Id));
            if (finreview != null)
            {
                finreview.company = request.company;
                finreview.company_code = request.company_code;
                finreview.plant = request.category == 1 ? "ALL" : request.plant;
                if (string.IsNullOrEmpty(request.rv1) && string.IsNullOrEmpty(request.rv2)){
                  result.message = L["AddFail"] + "：" + L["Rv1NotEmpty"] + " and " + L["Rv1NotEmpty"];
                  result.status = 2;
                  return result;
                }

                
                if (!string.IsNullOrEmpty(request.rv1)){
                    bool rv1IsExist = (await _EmployeeInfoRepository.EmpIsExist(request.rv1));
                    if(rv1IsExist == false){
                      result.message = L["SaveFailMsg"] + "：" + L["Rv1NotExist"];
                      result.status = 2;
                      return result;
                    }
                }else if (!string.IsNullOrEmpty(request.rv2)){
                    bool rv2IsExist = (await _EmployeeInfoRepository.EmpIsExist(request.rv2));
                    if (rv2IsExist == false)
                    {
                        result.message = L["SaveFailMsg"] + "：" + L["Rv2NotExist"];
                        result.status = 2;
                        return result;
                    }
                }

                finreview.rv1 = request.rv1;
                finreview.rv2 = request.rv2;
                
                finreview.muser = userId;
                finreview.mdate = System.DateTime.Now;
                result.message = L["SaveSuccessMsg"];
                await _FinreviewRepository.UpdateAsync(finreview);
            }
            else
            {
                result.message = L["SaveFailMsg"] + "：" + L["FinreviewNotFound"];
            }
            return result;
        }
        public async Task<Result<string>> DeleteFinApprover(List<Guid?> ids)
        {
            Result<string> result = new Result<string>();
            List<Finreview> finreview = (await _FinreviewRepository.GetFinreviewsByIds(ids));
            if (finreview.Count <= 0)
            {
                result.message = L["DeleteFail"] + "：" + L["FinApproverNotFound"];
                result.status = 2;
                return result;
            }
            foreach (var item in finreview)
            {
                if (item == null)
                {
                    result.message = L["DeleteFail"] + "：" + L["FinApproverNotFound"];
                    result.status = 2;
                    return result;
                }
            }
            await _FinreviewRepository.DeleteManyAsync(finreview);
            result.message = L["DeleteSuccess"];
            return result;
        }
    }
}

using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDCompanyCategory;
using Volo.Abp.ObjectMapping;
using System.Collections;

namespace ERS.DomainServices
{
    public class BDCompanyCategoryDomainService : CommonDomainService, IBDCompanyCategoryDomainService
    {
        public IBDCompanyCategoryRepository _bDCompanyCategoryRepository;
        public IBDCompanySiteRepository _bDCompanySiteRepository;
        private IObjectMapper _objectMapper;
        private IEmployeeInfoRepository _employeeInfoRepository;
        public BDCompanyCategoryDomainService(IBDCompanyCategoryRepository bDCompanyCategoryRepository,
            IBDCompanySiteRepository bDCompanySiteRepository,
            IObjectMapper objectMapper,
            IEmployeeInfoRepository employeeInfoRepository)
        {
            _bDCompanyCategoryRepository = bDCompanyCategoryRepository;
            _bDCompanySiteRepository = bDCompanySiteRepository;
            _objectMapper = objectMapper;
            _employeeInfoRepository = employeeInfoRepository;
        }

        public async Task<Result<List<BDCompanyCategoryParamDto>>> Search(Request<BDCompanyCategoryParamDto> request)
        {
            List<BDCompanyCategory> ls = (await _bDCompanyCategoryRepository.WithDetailsAsync()).
                WhereIf(!string.IsNullOrEmpty(request.data.CompanyCategory), e => e.CompanyCategory.Contains(request.data.CompanyCategory))
                .ToList();

            List<BDCompanySite> companySites = (await _bDCompanySiteRepository.WithDetailsAsync()).ToList();

            List<BDCompanyCategoryParamDto> res = _objectMapper.Map<IList<BDCompanyCategory>, IList<BDCompanyCategoryParamDto>>(ls).ToList();

            foreach (BDCompanyCategoryParamDto model in res)
            {
                List<BDCompanySite> mapCompanySites = companySites.Where(cs => cs.CompanyCategory == model.CompanyCategory).ToList();
                model.CompanySite = _objectMapper.Map<IList<BDCompanySite>, IList<BDCompanySiteParamDto>>(mapCompanySites).ToList();
                //if (companySite != null)
                //{
                //    model.Company = companySite.Company;
                //    model.Site = companySite.Site;
                //}
            }
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            int count = res.Count;
            Result<List<BDCompanyCategoryParamDto>> result = new Result<List<BDCompanyCategoryParamDto>>()
            {
                data = new List<BDCompanyCategoryParamDto>()
            };
            result.data = res.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = count;
            return result;
        }

        public async Task<Result<string>> Create(BDCompanyCategoryParamDto model, string userId)
        {
            Result<string> result = new Result<string>();
            if (string.IsNullOrEmpty(model.CompanyCategory))
            {
                result.message = L["SaveFailMsg"] + "：" + L["CompanyCategoryNotEmpty"];
                result.status = 2;
                return result;
            }
            List<BDCompanyCategory> ls = (await _bDCompanyCategoryRepository.WithDetailsAsync()).Where(x => x.CompanyCategory == model.CompanyCategory).ToList();//await _supplierRepository.FindAsync(x => x.unifiedNo == model.unifiedNo || x.supplierNo == model.supplierNo);
            if (ls.Count == 0)
            {
                BDCompanyCategory en = new BDCompanyCategory();
                en.CompanyCategory = model.CompanyCategory;
                en.CompanyDesc = model.CompanyDesc;
                en.CompanySap = model.CompanySap;
                en.Stwit = model.Stwit;
                en.Status = model.Status;
                en.IncomeTaxRate = model.IncomeTaxRate;
                en.TimeZone = model.TimeZone;
                en.BaseCurrency = model.BaseCurrency;
                en.Vatrate = model.Vatrate;
                en.Area = model.Area;
                en.IdentificationNo = model.IdentificationNo;
                en.company = ERSConsts.Company_All;
                en.mdate = System.DateTime.Now;
                en.muser = userId;

                List<BDCompanySite> companySites = _objectMapper.Map<IList<BDCompanySiteParamDto>, IList<BDCompanySite>>(model.CompanySite).ToList();
                companySites.ForEach(x => x.Primary = true);
                await _bDCompanySiteRepository.InsertManyAsync(companySites);
                await _bDCompanyCategoryRepository.InsertAsync(en);
                result.status = 1;
                result.message = L["SaveSuccessMsg"];
            }
            else
            {
                result.message = L["SaveFailMsg"] + "：" + L["CompanyExist"];
                result.status = 2;
            }
            return result;
        }

        public async Task<Result<string>> Update(BDCompanyCategoryParamDto model, string userId)
        {
            Result<string> result = new Result<string>();
            bool isDuplicate = false;
            List<BDCompanyCategory> ls = (await _bDCompanyCategoryRepository.WithDetailsAsync()).Where(x => x.CompanyCategory == model.CompanyCategory).ToList();
            foreach (BDCompanyCategory company in ls)
            {
                if (!company.Id.ToString().Equals(model.Id))
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                BDCompanyCategory oldData = await _bDCompanyCategoryRepository.FindAsync(x => model.Id.Equals(x.Id.ToString()));
                if (oldData != null)
                {
                    oldData.CompanyCategory = model.CompanyCategory;
                    oldData.CompanyDesc = model.CompanyDesc;
                    oldData.CompanySap = model.CompanySap;
                    oldData.Stwit = model.Stwit;
                    oldData.Status = model.Status;
                    oldData.IncomeTaxRate = model.IncomeTaxRate;
                    oldData.TimeZone = model.TimeZone;
                    oldData.BaseCurrency = model.BaseCurrency;
                    oldData.Vatrate = model.Vatrate;
                    oldData.Area = model.Area;
                    oldData.IdentificationNo = model.IdentificationNo;
                    oldData.mdate = System.DateTime.Now;
                    oldData.muser = userId;


                    List<BDCompanySite> oldCompanySites = (await _bDCompanySiteRepository.WithDetailsAsync()).Where(x => x.CompanyCategory == model.CompanyCategory).ToList();
                    List<BDCompanySite> newCompanySites = _objectMapper.Map<IList<BDCompanySiteParamDto>, IList<BDCompanySite>>(model.CompanySite).ToList();
                    if (oldCompanySites.Count > 0)
                    {
                        await _bDCompanySiteRepository.DeleteManyAsync(oldCompanySites);
                    }
                    newCompanySites.ForEach(x => x.Primary = true);
                    await _bDCompanySiteRepository.InsertManyAsync(newCompanySites);
                    await _bDCompanyCategoryRepository.UpdateAsync(oldData);
                    result.status = 1;
                    result.message = L["SaveSuccessMsg"];
                }
                else
                {
                    result.message = L["SaveFailMsg"] + "：" + L["CompanyNotExist"];
                    result.status = 2;

                }
            }
            else
            {
                result.message = L["SaveFailMsg"] + "：" + L["CompanyExist"];
                result.status = 2;
            }



            return result;
        }

        public async Task<Result<string>> Delete(List<string> ids)
        {
            Result<string> result = new Result<string>();
            List<BDCompanyCategory> toRemove = new List<BDCompanyCategory>();
            List<BDCompanySite> toRemoveSite = new List<BDCompanySite>();
            foreach (string id in ids)
            {
                BDCompanyCategory en = await _bDCompanyCategoryRepository.FindAsync(x => x.Id.ToString() == id);
                if (en != null)
                {
                    toRemove.Add(en);
                    BDCompanySite bDCompanySite = await _bDCompanySiteRepository.FindAsync(x => x.CompanyCategory == en.CompanyCategory);
                    if (bDCompanySite != null)
                    {
                        toRemoveSite.Add(bDCompanySite);
                    }
                }
            }
            if (toRemove.Count > 0)
                await _bDCompanyCategoryRepository.DeleteManyAsync(toRemove);
            if (toRemoveSite.Count > 0)
                await _bDCompanySiteRepository.DeleteManyAsync(toRemoveSite);

            result.status = 1;
            return result;
        }

        /// <summary>
        /// 此方法给一般费用报销申请使用，其它页面慎用！
        /// 根据userId找到其对应的company&site，再找出相应的Category公司别列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<List<BDCompanyCategoryParamDto>>> GetCategoryListByUserId(string userId)
        {
            Result<List<BDCompanyCategoryParamDto>> result = new Result<List<BDCompanyCategoryParamDto>>();

            if (string.IsNullOrEmpty(userId))
            {
                result.message = "The parameter cannot be null.";
                result.status = 2;
                return result;
            }

            try
            {
                // Get all data with a single query using joins
                var query = from employee in await _employeeInfoRepository.WithDetailsAsync()
                            join site in await _bDCompanySiteRepository.WithDetailsAsync()
                                on new { Company = employee.company, Site = employee.site } equals new { Company = site.Company, Site = site.Site }
                            join category in await _bDCompanyCategoryRepository.WithDetailsAsync()
                                on site.CompanyCategory equals category.CompanyCategory
                            where employee.emplid.ToUpper() == userId.Trim().ToUpper()
                            orderby site.Primary descending, category.CompanyDesc
                            select new { Site = site, Category = category };

                var joinedData = query.ToList();

                if (joinedData.Any())
                {
                    List<BDCompanyCategoryParamDto> categoryDtos = joinedData
                        .Select(x => new BDCompanyCategoryParamDto
                        {
                            Id = x.Category.Id.ToString(),
                            CompanyCategory = x.Category.CompanyCategory,
                            CompanyDesc = x.Category.CompanyDesc,
                            CompanySap = x.Category.CompanySap,
                            Stwit = x.Category.Stwit,
                            BaseCurrency = x.Category.BaseCurrency,
                            IdentificationNo = x.Category.IdentificationNo,
                            IncomeTaxRate = x.Category.IncomeTaxRate,
                            Vatrate = x.Category.Vatrate,
                            Status = x.Category.Status,
                            Area = x.Category.Area,
                            TimeZone = x.Category.TimeZone,
                            Primary = x.Site.Primary,
                            Company = x.Site.Company,
                            Site = x.Site.Site
                        })
                        .ToList();

                    result.data = categoryDtos;
                    result.total = categoryDtos.Count;
                }
            }
            catch (System.Exception ex)
            {
                result.status = 2;
                result.message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 获取所有纬创公司类别设定
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<BDCompanyCategoryParamDto>>> GetAllCompanyCategoryList()
        {
            List<BDCompanyCategory> ls = (await _bDCompanyCategoryRepository.WithDetailsAsync()).
                                 OrderBy(e => e.CompanyDesc)
                                 .ToList();
            List<BDCompanyCategoryParamDto> res = _objectMapper.Map<IList<BDCompanyCategory>, IList<BDCompanyCategoryParamDto>>(ls).ToList();
            Result<List<BDCompanyCategoryParamDto>> result = new Result<List<BDCompanyCategoryParamDto>>()
            {
                data = res,
                total = res.Count
            };
            return result;
        }

        /// <summary>
        /// 获取公司所有区域
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<string>>> GetAllAreaList()
        {
            var categories = await _bDCompanyCategoryRepository.WithDetailsAsync();
            var areaList = categories
                .Where(e => !string.IsNullOrWhiteSpace(e.Area))
                .Select(e => e.Area)
                .Distinct()
                .OrderBy(a => a)
                .ToList();

            return new Result<List<string>>
            {
                data = areaList,
                total = areaList.Count,
            };
        }

        /// <summary>
        /// 获取登入者所属区域
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<string>> GetCompanyCategoryArea(string userId)
        {
            string area = ""; 
            //根据登入者获取所属的区域
            var companyCategoryListResult = await GetCategoryListByUserId(userId);
            if (companyCategoryListResult.data != null)
            {
                area = companyCategoryListResult.data[0].Area;
            }
            return new Result<string>
            {
                data = area,
            };
        }
    }
}
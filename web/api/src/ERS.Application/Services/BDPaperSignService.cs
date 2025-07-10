using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDPaperSign;
using Volo.Abp.Application.Services;
using ERS.Entities;
using Volo.Abp.ObjectMapping;
using ERS.IRepositories;
using ERS.Localization;
namespace ERS.Services
{
    public class BDPaperSignService : ApplicationService, IBDPaperSignService
    {
        private IBDPaperSignRepository _BDPaperSignRepository;
        private IObjectMapper _ObjectMapper;
        private ICashHeadRepository _CashHeadRepository;
        private ICashDetailRepository _CashDetailRepository;
        private IEmployeeRepository _EmloyeeRepository;
        private ICompanyRepository _CompanyRepository;
        public BDPaperSignService(
            IBDPaperSignRepository BDPaperSignRepository,
            IObjectMapper ObjectMapper,
            ICashHeadRepository CashHeadRepository,
            ICashDetailRepository CashDetailRepository,
            IEmployeeRepository EmployeeRepository,
            ICompanyRepository CompanyRepository
        )
        {
            _BDPaperSignRepository = BDPaperSignRepository;
            _ObjectMapper = ObjectMapper;
            _CashHeadRepository = CashHeadRepository;
            _CashDetailRepository = CashDetailRepository;
            _EmloyeeRepository = EmployeeRepository;
            _CompanyRepository = CompanyRepository;
            LocalizationResource = typeof(ERSResource);
        }
        /// <summary>
        /// 添加纸本单签核人
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<string>> AddBDPaperSign(AddPaperSignDto request, string userId)
        {
            Result<string> result = new Result<string>();
            result.status = 1;
            result.message = L["AddSuccess"];
            if(string.IsNullOrEmpty(request.company))
            {
                result.message = L["AddFail"] + ": "+ L["CompanyCodeEmpty"];
                result.status = 2;
                return result;
            }
            if(string.IsNullOrEmpty(request.emplid))
            {
                result.message = L["AddFail"] + ": " + L["BDPaper-EmplidCodeEmpty"];
                result.status = 2;
                return result;
            }
            if(string.IsNullOrEmpty(request.plant))
            {
                result.message = L["AddFail"] + ": " + L["BDPaper-PlantEmpty"];
                result.status = 2;
                return result;
            }
            List<string> company_codes = (await _CompanyRepository.WithDetailsAsync()).Where(w => w.company == request.company).Select(w => w.CompanyCategory).ToList();
            if(!company_codes.Contains(request.company_code))
            {
                result.message = L["AddFail"] + ": " + L["BDPaper-CompanyCodeEmpty"];
                result.status = 2;
                return result;
            }
            var checkRepeat = (await _BDPaperSignRepository.WithDetailsAsync()).Where(w => w.company_code == request.company_code && w.plant == request.plant && w.company == request.company).ToList();
            if(checkRepeat.Count > 0)
            {
                result.message = L["AddFail"] + ": " + L["DataAlreadyexist"];
                result.status = 2;
                return result;
            }
            BDPaperSign bDPaperSign = _ObjectMapper.Map<AddPaperSignDto,BDPaperSign>(request);
            bDPaperSign.cdate = System.DateTime.Now;
            bDPaperSign.cuser = userId;
            await _BDPaperSignRepository.InsertAsync(bDPaperSign);
            return result;
        }
        /// <summary>
        /// 编辑纸本单签核人
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<string>> EditBDPaperSign(EditPaperSignDto request, string userId)
        {
            Result<string> result = new Result<string>();
            result.status = 1;
            result.message = L["EditSuccess"];
            if(string.IsNullOrEmpty(request.company_code))
            {
                result.message = L["EditFail"] + ": " + L["CompanyCodeEmpty"];
                result.status = 2;
                return result;
            }
            if(string.IsNullOrEmpty(request.emplid))
            {
                result.message = L["EditFail"] + ": " + L["BDPaper-EmplidCodeEmpty"];
                result.status = 2;
                return result;
            }
            if(string.IsNullOrEmpty(request.plant))
            {
                result.message = L["EditFail"] + ": " + L["BDPaper-PlantEmpty"];
                result.status = 2;
                return result;
            }
            BDPaperSign bDPaperSign = (await _BDPaperSignRepository.WithDetailsAsync()).Where(w => w.Id == request.Id).FirstOrDefault();
            if(bDPaperSign == null)
            {
                result.message = L["EditFail"] + ": " + L["BDPaer-DataNotExist"];
                result.status = 2;
                return result;
            }
            bDPaperSign.company_code = request.company_code;
            bDPaperSign.plant = request.plant;
            bDPaperSign.emplid = request.emplid;
            bDPaperSign.company = request.company;
            bDPaperSign.mdate = System.DateTime.Now;
            bDPaperSign.muser = userId;
            await _BDPaperSignRepository.UpdateAsync(bDPaperSign);
            return result;
        }
        /// <summary>
        /// 查询纸本单签核人
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<List<PaperSignDto>>> QueryBDPaperSign(Request<QueryPaperSignDto> request)
        {
            Result<List<PaperSignDto>> result = new Result<List<PaperSignDto>>();
            List<BDPaperSign> query = (await _BDPaperSignRepository.WithDetailsAsync())
            .Where(w => request.data.companyList.Contains(w.company))
            .WhereIf(!string.IsNullOrEmpty(request.data.emplid), w => w.emplid == request.data.emplid)
            .WhereIf(!string.IsNullOrEmpty(request.data.plant), w => w.plant == request.data.plant)
            .ToList();
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            result.data = _ObjectMapper.Map<List<BDPaperSign>, List<PaperSignDto>>(query).ToList();
            foreach(var item in result.data)
            {
                item.emplname = await _EmloyeeRepository.GetCnameByEmplid(item.emplid);
            }
            int count = query.Count;
            result.total = count;
            result.data = result.data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }
        /// <summary>
        /// 删除纸本单签核人
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<Result<string>> RemoveBDPaperSign(List<Guid?> Ids)
        {
            Result<string> result = new Result<string>();
            var query = (await _BDPaperSignRepository.WithDetailsAsync()).Where(w => Ids.Contains(w.Id)).ToList();
            if(query.Count <= 0)
            {
                result.message = L["DeleteFail"] + ": " + L["BDPaer-DataNotExist"];
                result.status = 2;
                return result;
            }
            await _BDPaperSignRepository.DeleteManyAsync(query);
            result.message = L["DeleteSuccess"];
            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDInvoiceType;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using Volo.Abp.ObjectMapping;
namespace ERS.DomainServices
{
    public class BDInvoiceTypeDomainService : CommonDomainService, IBDInvoiceTypeDomainService
    {
        private IBDInvoiceTypeRepository _BDInvoiceTypeRepository;
        private IEmployeeRepository _EmployeeRepository;
        private IObjectMapper _ObjectMapper;
        public BDInvoiceTypeDomainService(
            IBDInvoiceTypeRepository BDInvoiceTypeRepository,
            IEmployeeRepository EmployeeRepository,
            IObjectMapper ObjectMapper
        )
        {
            _BDInvoiceTypeRepository = BDInvoiceTypeRepository;
            _EmployeeRepository = EmployeeRepository;
            _ObjectMapper = ObjectMapper;
        }
        /// <summary>
        /// 發票類型分頁查詢
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result<List<BDInvoiceTypeDto>>> GetPageInvTypes(Request<QueryBDInvTypeDto> request)
        {
            Result<List<BDInvoiceTypeDto>> result = new Result<List<BDInvoiceTypeDto>>()
            {
                data = new List<BDInvoiceTypeDto>()
            };
            List<BDInvoiceType> BDInvTypeList = await _BDInvoiceTypeRepository.GetBDInvoiceTypeList();
            List<BDInvoiceTypeDto> bDInvoiceTypeDtos = _ObjectMapper.Map<List<BDInvoiceType>, List<BDInvoiceTypeDto>>(BDInvTypeList);
            bDInvoiceTypeDtos = bDInvoiceTypeDtos
            .WhereIf(!String.IsNullOrEmpty(request.data.invtypecode), w => w.invtypecode == request.data.invtypecode)
            .WhereIf(!String.IsNullOrEmpty(request.data.invtype), w => w.invtype == request.data.invtype)
            .WhereIf(!String.IsNullOrEmpty(request.data.category), w => w.category == request.data.category)
            .WhereIf(!String.IsNullOrEmpty(request.data.area), w => w.area == request.data.area)
            .Where(w => request.data.companylist.Contains(w.company))
            .ToList();
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            result.data = bDInvoiceTypeDtos.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = bDInvoiceTypeDtos.Count;
            return result;
        }
        /// <summary>
        /// 添加發票類型
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<string>> AddInvTypes(AddBDInvTypeDto request, string userId)
        {
            Result<string> result = new Result<string>();
            InvTypeCheckDto invTypeCheckDto = _ObjectMapper.Map<AddBDInvTypeDto, InvTypeCheckDto>(request);
            var checkResult = await CheckInput(invTypeCheckDto);
            if (!checkResult.data)
            {
                result.message = L["AddFail"] + "：" + checkResult.message;
            }
            if (await _BDInvoiceTypeRepository.IsRepeat(request.company, request.invtypecode))
            {
                result.message = L["AddFail"] + "：" + L["BDInvoiceType-InvTypeRepeat"];
                result.status = 2;
                return result;
            }
            BDInvoiceType bDInvoiceType = _ObjectMapper.Map<AddBDInvTypeDto, BDInvoiceType>(request);
            bDInvoiceType.cuser = userId;
            bDInvoiceType.cdate = System.DateTime.Now;
            await _BDInvoiceTypeRepository.InsertAsync(bDInvoiceType);
            result.message = L["AddSuccess"];
            return result;
        }
        /// <summary>
        /// 編輯發票類型
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<string>> EditInvTypes(EditBDInvTypeDto request, string userId)
        {
            Result<string> result = new Result<string>();
            BDInvoiceType bDInvoiceType = await _BDInvoiceTypeRepository.GetBDInvoiceTypeById(request.Id);
            InvTypeCheckDto invTypeCheckDto = _ObjectMapper.Map<EditBDInvTypeDto, InvTypeCheckDto>(request);
            var checkResult = await CheckInput(invTypeCheckDto);
            if (bDInvoiceType == null)
            {
                result.message = L["SaveFailMsg"] + "：" + L["BDInvoiceType-NotExist"];
                result.status = 2;
                return result;
            }
            if (!checkResult.data)
            {
                result.message = L["SaveFailMsg"] + "：" + checkResult.message;
            }
            bDInvoiceType.InvTypeCode = request.invtypecode;
            bDInvoiceType.InvType = request.invtype;
            bDInvoiceType.company = request.company;
            bDInvoiceType.Category = request.category;
            bDInvoiceType.Area = request.area;
            bDInvoiceType.muser = userId;
            bDInvoiceType.mdate = System.DateTime.Now;
            await _BDInvoiceTypeRepository.UpdateAsync(bDInvoiceType);
            result.message = L["SaveSuccessMsg"];
            return result;
        }
        /// <summary>
        /// 刪除發票類型
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public async Task<Result<string>> DeleteInvTypes(List<Guid> Ids)
        {
            Result<string> result = new Result<string>();
            List<BDInvoiceType> bDInvoiceTypes = await _BDInvoiceTypeRepository.GetBDInvoiceTypeListByIds(Ids);
            if (bDInvoiceTypes.Count == 0)
            {
                result.status = 2;
                result.message = L["DeleteFail"] + "：" + L["BDInvoiceType-NotFound"];
                return result;
            }
            await _BDInvoiceTypeRepository.DeleteManyAsync(bDInvoiceTypes);
            result.message = L["DeleteSuccess"];
            return result;
        }
        /// <summary>
        /// 对输入参数进行校验返回相应提示
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<bool>> CheckInput(InvTypeCheckDto input)
        {
            Result<bool> result = new Result<bool>();
            result.data = true;
            result.status = 1;
            if (string.IsNullOrEmpty(input.company))
            {
                result.message = L["CompanyEmpty"];
                result.data = false;
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(input.invtypecode))
            {
                result.message = L["BDInvoiceType-InvCodeEmpty"];
                result.status = 2;
                result.data = false;
                return result;
            }
            if (string.IsNullOrEmpty(input.invtype))
            {
                result.message = L["BDInvoiceType-InvTypeEmpty"];
                result.status = 2;
                result.data = false;
                return result;
            }
            if (string.IsNullOrEmpty(input.category))
            {
                result.message = L["BDInvoiceType-CategoryEmpty"];
                result.status = 2;
                result.data = false;
                return result;
            }
            if (string.IsNullOrEmpty(input.area))
            {
                result.message = L["BDInvoiceType-AreaEmpty"];
                result.status = 2;
                result.data = false;
                return result;
            }
            //if((await _BDInvoiceTypeRepository.WithDetailsAsync()).Where(w => ChineseConverter.Convert(w.invtype, ChineseConversionDirection.SimplifiedToTraditional).Equals(input.invtype)).Count())
            if (!string.IsNullOrEmpty(input.invtype))
            {
                string transInvtype = ChineseConverter.Convert(input.invtype, ChineseConversionDirection.SimplifiedToTraditional);
                var repeatCheck = (await _BDInvoiceTypeRepository.WithDetailsAsync()).AsEnumerable().Where(w => ChineseConverter.Convert(w.InvType, ChineseConversionDirection.SimplifiedToTraditional) == transInvtype && w.company == input.company).ToList();
                if (repeatCheck.Count > 0)
                {
                    result.message = L["BDInvoiceType-InvTypeRepeat"];
                    result.status = 2;
                    result.data = false;
                    return result;
                }
            }
            return result;
        }
        //据登录人的公司别获取发票类型
        public async Task<Result<List<InvoiceTypeDto>>> GetInvTypesByUserCompany(string company)
        {
            Result<List<InvoiceTypeDto>> result = new Result<List<InvoiceTypeDto>>();
            var query = await _BDInvoiceTypeRepository.GetInvoiceTypesByCompany(company);
            if (query.Count == 0)
            {
                return result;
            }
            result.data = _ObjectMapper.Map<List<BDInvoiceType>, List<InvoiceTypeDto>>(query);
            return result;
        }

        /// <summary>
        /// 獲取全部的發票類型設定
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<InvoiceTypeDto>>> GetAllInvTypes()
        {
            Result<List<InvoiceTypeDto>> result = new Result<List<InvoiceTypeDto>>();
            // 只保留唯一的invcode和invtype组合，并根据invtype排序
            var allList = (await _BDInvoiceTypeRepository.WithDetailsAsync())
                          .Where(x => !x.isdeleted)
                          .ToList();

            var uniqueList = allList
                .GroupBy(x => new { x.InvTypeCode, x.InvType })
                .Select(g => g.First())
                .OrderBy(x => x.InvType)
                .ToList();

            result.data = _ObjectMapper.Map<List<BDInvoiceType>, List<InvoiceTypeDto>>(uniqueList);
            return result;
        }

    }
}
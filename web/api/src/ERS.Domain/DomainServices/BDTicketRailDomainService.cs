using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDTicketRail;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Volo.Abp.ObjectMapping;
namespace ERS.DomainServices
{
    public class BDTicketRailDomainService : CommonDomainService, IBDTicketRailDomainService
    {
        private IBDTicketRailRepository _BDTicketRailRepository;
        private IEmployeeRepository _EmployeeRepository;
        private IObjectMapper _ObjectMapper;
        public BDTicketRailDomainService(
            IBDTicketRailRepository BDTicketRailRepository,
            IEmployeeRepository EmployeeRepository,
            IObjectMapper ObjectMapper
        )
        {
            _BDTicketRailRepository = BDTicketRailRepository;
            _EmployeeRepository = EmployeeRepository;
            _ObjectMapper = ObjectMapper;
        }

        public async Task<Result<List<BDTicketRailDto>>> GetPageBDTicketRails(Request<QueryBDTicketRailDto> request)
        {
            Result<List<BDTicketRailDto>> result = new Result<List<BDTicketRailDto>>()
            {
                data = new List<BDTicketRailDto>()
            };
            List<BDTicketRail> BDTicketRailList = await _BDTicketRailRepository.GetBDTicketRailList();
            List<BDTicketRailDto> bDTicketRailDtos = _ObjectMapper.Map<List<BDTicketRail>, List<BDTicketRailDto>>(BDTicketRailList);
            bDTicketRailDtos = bDTicketRailDtos
            .WhereIf(!String.IsNullOrEmpty(request.data.ticketrail), w => w.ticketrail == request.data.ticketrail)
            .WhereIf(!String.IsNullOrEmpty(request.data.voucheryear), w => w.voucheryear == request.data.voucheryear)
                    .Where(w => request.data.companylist.Contains(w.company))
            .ToList();
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            result.data = bDTicketRailDtos.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = bDTicketRailDtos.Count;
            return result;
        }
        public async Task<Result<string>> AddBDTicketRail(AddBDTicketRailDto request, string userId)
        {
            Result<string> result = new Result<string>();
            //是否重复
            BDTicketRail check = await _BDTicketRailRepository.GetBDTicketRailsByCompanyAndVoucheryearAndVouchermonth(request.company, request.voucheryear, request.vouchermonth);
            if (check != null)
            {
                result.message = L["BDTicketRail-Repeat"];
                result.status = 2;
                return result;
            }
            BDTicketRail bDTicketRail = _ObjectMapper.Map<AddBDTicketRailDto, BDTicketRail>(request);
            bDTicketRail.currentnumber = "0001";
            bDTicketRail.cuser = userId;
            bDTicketRail.cdate = System.DateTime.Now;
            await _BDTicketRailRepository.InsertAsync(bDTicketRail);
            result.message = L["AddSuccess"];
            return result;
        }

        public async Task<Result<string>> EditBDTicketRail(EditBDTicketRailDto request, string userId)
        {
            Result<string> result = new Result<string>();
            BDTicketRail BDTicketRail = await _BDTicketRailRepository.GetBDTicketRailById(request.Id);
            if (BDTicketRail == null)
            {
                result.message =  L["BDTicketRail-NotExist"];
                result.status = 2;
                return result;
            }
            //是否重复
            BDTicketRail check = await _BDTicketRailRepository.GetBDTicketRailsByCompanyAndVoucheryearAndVouchermonth(request.company, request.voucheryear, request.vouchermonth);
            if (check != null)
            {
                result.message = L["BDTicketRail-Repeat"];
                result.status = 2;
                return result;
            }
            BDTicketRail.ticketrail = request.ticketrail;
            BDTicketRail.voucheryear = request.voucheryear;
            BDTicketRail.vouchermonth = request.vouchermonth;
            BDTicketRail.muser = userId;
            BDTicketRail.mdate = System.DateTime.Now;
            await _BDTicketRailRepository.UpdateAsync(BDTicketRail);
            result.message = L["SaveSuccessMsg"];
            return result;
        }

        public async Task<Result<string>> DeleteBDTicketRails(List<Guid> Ids, string userId)
        {
            Result<string> result = new Result<string>();
            List<BDTicketRail> BDTicketRails = await _BDTicketRailRepository.GetBDTicketRailListByIds(Ids);
            if (BDTicketRails.Count == 0)
            {
                result.status = 2;
                result.message = L["DeleteFail"] + "：" + L["BDTicketRail-NotFound"];
                return result;
            }
            //不刪除，修改isdeleted為true
            foreach (var item in BDTicketRails)
            {
                item.isdeleted = true;
                item.muser = userId;
                item.mdate = System.DateTime.Now;
            }
            await _BDTicketRailRepository.UpdateManyAsync(BDTicketRails);
            result.message = L["DeleteSuccess"];
            return result;
        }

        public Task<Result<List<TicketRailDto>>> GetBDTicketRailsByUserCompany(string company)
        {
            throw new NotImplementedException();
        }
    }
}
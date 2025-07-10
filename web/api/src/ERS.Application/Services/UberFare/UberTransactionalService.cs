using ERS.Application.Contracts.DTO.Application;
using ERS.Domain.DomainServices;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.UberFare;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.Localization;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;

namespace ERS.Services.UberFare
{
    public class UberTransactionalService : ApplicationService, IUberTransactionalService
    {
        private IUberTransactionalDayRepository _uberTransactionalDayRepository;
        private IUberDomainService _uberDomainService;
        private ICashHeadRepository _cashHeadRepository;
        private ICashDetailRepository _cashDetailRepository;
        private IObjectMapper _objectMapper;
        public UberTransactionalService(
            IUberTransactionalDayRepository uberTransactionalDayRepository,
            IUberDomainService uberDomainService,
            ICashHeadRepository cashHeadRepository,
            ICashDetailRepository cashDetailRepository, 
            IObjectMapper objectMapper)
        {
            _uberTransactionalDayRepository = uberTransactionalDayRepository;
            LocalizationResource = typeof(ERSResource);
            _uberDomainService = uberDomainService;
            _cashHeadRepository = cashHeadRepository;
            _cashDetailRepository = cashDetailRepository;
            _objectMapper = objectMapper;
        }

        public async Task<XSSFWorkbook> DownloadUberTransactional(Request<QueryUberTransactionalDto> request)
        {
            var uberTransactionalCount = (await _uberTransactionalDayRepository.WithDetailsAsync()).AsNoTracking().Count();
            request.pageSize = uberTransactionalCount;
            Result<List<UberTransactionalDayDto>> result = await GetUberTransactional(request);
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet");
            if (result != null && result.data.Count() > 0)
            {
                for (int i = 0; i < result.data.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 1);
                    dataRow.CreateCell(0).SetCellValue(result.data[i].TripId?.ToString());
                    dataRow.CreateCell(1).SetCellValue(result.data[i].RequestDate.ToString());
                    dataRow.CreateCell(2).SetCellValue(result.data[i].City?.ToString());
                    dataRow.CreateCell(3).SetCellValue(result.data[i].PickupAddress?.ToString());
                    dataRow.CreateCell(4).SetCellValue(result.data[i].DropOffAddress?.ToString());
                    dataRow.CreateCell(5).SetCellValue(result.data[i].TransactionTimestamp?.ToString());
                    dataRow.CreateCell(6).SetCellValue(result.data[i].RequestDateUtc?.ToString());
                    dataRow.CreateCell(7).SetCellValue(result.data[i].RequestTimeUtc?.ToString());
                    dataRow.CreateCell(8).SetCellValue(result.data[i].DropOffDateUtc?.ToString());
                    dataRow.CreateCell(9).SetCellValue(result.data[i].DropOffTimeUtc?.ToString());
                    dataRow.CreateCell(10).SetCellValue(result.data[i].DropOffDate?.ToString());
                    dataRow.CreateCell(11).SetCellValue(result.data[i].DropOffTime?.ToString());
                    dataRow.CreateCell(12).SetCellValue(result.data[i].RequestTimezoneOffsetFromUtc?.ToString());
                    dataRow.CreateCell(13).SetCellValue(result.data[i].FirstName?.ToString());
                    dataRow.CreateCell(14).SetCellValue(result.data[i].LastName?.ToString());
                    dataRow.CreateCell(15).SetCellValue(result.data[i].Email?.ToString());
                    dataRow.CreateCell(16).SetCellValue(result.data[i].EmployeeId?.ToString());
                    dataRow.CreateCell(17).SetCellValue(result.data[i].Service?.ToString());
                    dataRow.CreateCell(18).SetCellValue(result.data[i].Distance?.ToString());
                    dataRow.CreateCell(19).SetCellValue(result.data[i].Duration?.ToString());
                    dataRow.CreateCell(20).SetCellValue(result.data[i].ExpenseCode?.ToString());
                    dataRow.CreateCell(21).SetCellValue(result.data[i].ExpenseMemo?.ToString());
                    dataRow.CreateCell(22).SetCellValue(result.data[i].Invoices?.ToString());
                    dataRow.CreateCell(23).SetCellValue(result.data[i].Program?.ToString());
                    dataRow.CreateCell(24).SetCellValue(result.data[i].Group?.ToString());
                    dataRow.CreateCell(25).SetCellValue(result.data[i].PaymentMethod?.ToString());
                    dataRow.CreateCell(26).SetCellValue(result.data[i].TransactionType?.ToString());
                    dataRow.CreateCell(27).SetCellValue(result.data[i].FareInLocalCurrency?.ToString());
                    dataRow.CreateCell(28).SetCellValue(result.data[i].TaxesInLocalCurrency?.ToString());
                    dataRow.CreateCell(29).SetCellValue(result.data[i].TipInLocalCurrency?.ToString());
                    dataRow.CreateCell(30).SetCellValue(result.data[i].TransactionAmountInLocalCurrency?.ToString());
                    dataRow.CreateCell(31).SetCellValue(result.data[i].LocalCurrencyCode?.ToString());
                    dataRow.CreateCell(32).SetCellValue(result.data[i].FareInHomeCurrency?.ToString());
                    dataRow.CreateCell(33).SetCellValue(result.data[i].TaxesInHomeCurrency?.ToString());
                    dataRow.CreateCell(34).SetCellValue(result.data[i].TipInHomeCurrency?.ToString());
                    dataRow.CreateCell(35).SetCellValue(result.data[i].TransactionAmountInHomeCurrency?.ToString());
                    dataRow.CreateCell(36).SetCellValue(result.data[i].EstimatedServiceAndTechnologyFee?.ToString());
                    dataRow.CreateCell(37).SetCellValue(result.data[i].rno?.ToString());
                    dataRow.CreateCell(38).SetCellValue(result.data[i].SignStatus?.ToString());
                }
            }
            else
            {
                IRow dataRow = sheet.CreateRow(0);
            }
            IRow rowHeader = sheet.CreateRow(0);
            rowHeader.CreateCell(0).SetCellValue(L["TripId"]);
            rowHeader.CreateCell(1).SetCellValue(L["RequestDate"]);
            rowHeader.CreateCell(2).SetCellValue(L["City"]);
            rowHeader.CreateCell(3).SetCellValue(L["PickupAddress"]);
            rowHeader.CreateCell(4).SetCellValue(L["DropOffAddress"]);
            rowHeader.CreateCell(5).SetCellValue(L["TransactionTimestamp"]);
            rowHeader.CreateCell(6).SetCellValue(L["RequestDateUtc"]);
            rowHeader.CreateCell(7).SetCellValue(L["RequestTimeUtc"]);
            rowHeader.CreateCell(8).SetCellValue(L["DropOffDateUtc"]);
            rowHeader.CreateCell(9).SetCellValue(L["DropOffTimeUtc"]);
            rowHeader.CreateCell(10).SetCellValue(L["DropOffDate"]);
            rowHeader.CreateCell(11).SetCellValue(L["DropOffTime"]);
            rowHeader.CreateCell(12).SetCellValue(L["RequestTimezoneOffsetFromUtc"]);
            rowHeader.CreateCell(13).SetCellValue(L["FirstName"]);
            rowHeader.CreateCell(14).SetCellValue(L["LastName"]);
            rowHeader.CreateCell(15).SetCellValue(L["Email"]);
            rowHeader.CreateCell(16).SetCellValue(L["EmployeeId"]);
            rowHeader.CreateCell(17).SetCellValue(L["Service"]);
            rowHeader.CreateCell(18).SetCellValue(L["Distance"]);
            rowHeader.CreateCell(19).SetCellValue(L["Duration"]);
            rowHeader.CreateCell(20).SetCellValue(L["ExpenseCode"]);
            rowHeader.CreateCell(21).SetCellValue(L["ExpenseMemo"]);
            rowHeader.CreateCell(22).SetCellValue(L["Invoices"]);
            rowHeader.CreateCell(23).SetCellValue(L["Program"]);
            rowHeader.CreateCell(24).SetCellValue(L["Group"]);
            rowHeader.CreateCell(25).SetCellValue(L["UberPaymentMethod"]);
            rowHeader.CreateCell(26).SetCellValue(L["TransactionType"]);
            rowHeader.CreateCell(27).SetCellValue(L["FareInLocalCurrency"]);
            rowHeader.CreateCell(28).SetCellValue(L["TaxesInLocalCurrency"]);
            rowHeader.CreateCell(29).SetCellValue(L["TipInLocalCurrency"]);
            rowHeader.CreateCell(30).SetCellValue(L["TransactionAmountInLocalCurrency"]);
            rowHeader.CreateCell(31).SetCellValue(L["LocalCurrencyCode"]);
            rowHeader.CreateCell(32).SetCellValue(L["FareInHomeCurrency"]);
            rowHeader.CreateCell(33).SetCellValue(L["TaxesInHomeCurrency"]);
            rowHeader.CreateCell(34).SetCellValue(L["TipInHomeCurrency"]);
            rowHeader.CreateCell(35).SetCellValue(L["TransactionAmountInHomeCurrency"]);
            rowHeader.CreateCell(36).SetCellValue(L["EstimatedServiceAndTechnologyFee"]);
            rowHeader.CreateCell(37).SetCellValue(L["rno"]);
            rowHeader.CreateCell(38).SetCellValue(L["SignStatus"]);
            return workbook;
        }

        public async Task<Result<UberApplicationDto>> GetQueryApplications(string rno)
        {
            Result<UberApplicationDto> results = new();
            UberApplicationDto result = new UberApplicationDto();
            // 查询Uber申请单
            CashHead headquery = await _cashHeadRepository.GetByNo(rno);
            List<CashDetail> detailquery = await _cashDetailRepository.GetCashDetailsByNo(rno);

            // 修正映射问题
            CashUberHeadDto headData =new CashUberHeadDto()
            {
                Emplid = headquery.cuser,
                Rno = headquery.rno,
                FormCode = headquery.formcode,
                Name = headquery.cname,
                ProjectCode = headquery.projectcode,
                BusinessTripNo = headquery.BusinessTripNo,
                Program = headquery.Program,
                company = headquery.company,      // 注意属性名大小写
                cdate = headquery.cdate,
                cuser = headquery.cuser
            };
            List<CashUberDetailDto> detailsData = detailquery.Select(d => new CashUberDetailDto
            {
                FormCode = d.formcode,
                Rno = d.rno,
                Item = "",
                StartDate = d.rdate,
                Destination = d.Destination,
                Origin = d.Origin,
                Status = "",
                Amount = d.amount,
                Reason = d.remarks,
                ExpCode = d.deptid,
                Emplid = d.cuser,
                Name = d.Passenger,
                DeptId = d.deptid
            }).ToList();

            // 将映射后的数据赋值给结果对象
            result.head = headData;
            result.detail = detailsData;
            results.data = result;
            return results;
        }

        public async Task<Result<List<UberTransactionalDayDto>>> GetUberTransactional(Request<QueryUberTransactionalDto> request)
        {
            Result<List<UberTransactionalDayDto>> result = new Result<List<UberTransactionalDayDto>>()
            {
                data = new List<UberTransactionalDayDto>()
            };
            int pageIndex = request.pageIndex > 0 ? request.pageIndex : 1;
            int pageSize = request.pageSize > 0 ? request.pageSize : 10;

            var query = (await _uberTransactionalDayRepository.WithDetailsAsync())
                .AsQueryable();

            if (request.data.startDate.HasValue)
                query = query.Where(q => q.RequestDate >= request.data.startDate.Value.Date);
            if (request.data.endDate.HasValue)
                query = query.Where(q => q.RequestDate <= request.data.endDate.Value.Date);
            if (!string.IsNullOrEmpty(request.data.rno))
                query = query.Where(q => q.rno == request.data.rno);
            if (!request.data.signStatus.IsNullOrEmpty())
                query = query.Where(q => request.data.signStatus.Contains(q.SignStatus));
            if (!request.data.cuser.IsNullOrEmpty())
                query = query.Where(q => q.EmployeeId == request.data.cuser);
            if (!request.data.company.IsNullOrEmpty())
                query = query.Where(q => request.data.company.Contains(q.Company));

            // 先统计总数
            result.total = query.Count();

            // 分页查询
            var uberTransactional = query
                .OrderByDescending(i => i.TransactionTimestamp)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new UberTransactionalDayDto
                {
                    TripId = w.TripId,
                    TransactionTimestamp = w.TransactionTimestamp,
                    RequestDateUtc = w.RequestDateUtc,
                    RequestTimeUtc = w.RequestTimeUtc,
                    RequestDate = w.RequestDate,
                    RequestTime = w.RequestTime,
                    DropOffDateUtc = w.DropOffDateUtc,
                    DropOffTimeUtc = w.DropOffTimeUtc,
                    DropOffDate = w.DropOffDate,
                    DropOffTime = w.DropOffTime,
                    RequestTimezoneOffsetFromUtc = w.RequestTimezoneOffsetFromUtc,
                    FirstName = w.FirstName,
                    LastName = w.LastName,
                    Email = w.Email,
                    EmployeeId = w.EmployeeId,
                    Service = w.Service,
                    City = w.City,
                    Distance = w.Distance,
                    Duration = w.Duration,
                    PickupAddress = w.PickupAddress,
                    DropOffAddress = w.DropOffAddress,
                    ExpenseCode = w.ExpenseCode,
                    ExpenseMemo = w.ExpenseMemo,
                    Invoices = w.Invoices,
                    Program = w.Program,
                    Group = w.Group,
                    PaymentMethod = w.PaymentMethod,
                    TransactionType = w.TransactionType,
                    FareInLocalCurrency = w.FareInLocalCurrency,
                    TaxesInLocalCurrency = w.TaxesInLocalCurrency,
                    TipInLocalCurrency = w.TipInLocalCurrency,
                    TransactionAmountInLocalCurrency = w.TransactionAmountInLocalCurrency,
                    LocalCurrencyCode = w.LocalCurrencyCode,
                    FareInHomeCurrency = w.FareInHomeCurrency,
                    TaxesInHomeCurrency = w.TaxesInHomeCurrency,
                    TipInHomeCurrency = w.TipInHomeCurrency,
                    TransactionAmountInHomeCurrency = w.TransactionAmountInHomeCurrency,
                    EstimatedServiceAndTechnologyFee = w.EstimatedServiceAndTechnologyFee,
                    rno = w.rno,
                    SignStatus = (w.SignStatus == "R" ? "Rejected" : (w.SignStatus == "A" ? "Approved" : (w.SignStatus == "P" ? "Signing" : "Pending"))),
                })
                .ToList();
            result.data = uberTransactional;
            return result;
        }
        public async Task<Result<string>> Submit(CashUberHeadDto cashUberHeadDto)
        {
            Result<string> result = new Result<string>();
            // 查询Uber申请单
            CashHead cashHead = await _cashHeadRepository.GetByNo(cashUberHeadDto.Rno);
            if (!string.IsNullOrEmpty(cashUberHeadDto.ProjectCode))
            {
                cashHead.projectcode = cashUberHeadDto.ProjectCode;
            }
            if (!string.IsNullOrEmpty(cashUberHeadDto.BusinessTripNo))
            {
                cashHead.BusinessTripNo = cashUberHeadDto.BusinessTripNo;
            }
            cashHead.mdate= DateTime.Now;
            await _cashHeadRepository.UpdateAsync(cashHead);
            result.message = "修改成功";
            return result;
        }

        public async Task TransactionToSign()
        {
           await _uberDomainService.TransactionToSign();
        }
    }
}

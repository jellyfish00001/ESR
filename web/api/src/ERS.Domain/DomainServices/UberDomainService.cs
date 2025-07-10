using ERS.Application.Contracts.DTO.Report;
using ERS.Domain.IDomainServices;
using ERS.DomainServices;
using ERS.DTO;
using ERS.DTO.BDCompanyCategory;
using ERS.Entities;
using ERS.Entities.Uber;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.UberToERS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
namespace ERS.Domain.DomainServices
{
    public class UberDomainService : CommonDomainService, IUberDomainService
    {

        private IRepository<BDForm, Guid> _bdformRepository;
        private IApprovalFlowDomainService _iApprovalFlowDomainService;
        private IUberTransactionalDayRepository _UberTransactionalDayRepository;
        private IEmployeeInfoRepository _EmployeeInfoRepository;
        private IAutoNoRepository _autoNoRepository;
        private ICashHeadRepository _cashHeadRepository;
        private ICashDetailRepository _cashDetailRepository;
        private IBDCompanyCategoryRepository _bDCompanyCategoryRepository;
        private IBDCompanySiteRepository _bDCompanySiteRepository;
        private IObjectMapper _objectMapper;
        public UberDomainService
        (
        IRepository<BDForm, Guid> bdformRepository,
        IUberTransactionalDayRepository UberTransactionalDayRepository,
        IEmployeeInfoRepository employeeInfoRepository,
        IAutoNoRepository autoNoRepository,
        ICashHeadRepository cashHeadRepository,
        ICashDetailRepository cashDetailRepository,
        IApprovalFlowDomainService approvalFlowDomainService,
        IBDCompanyCategoryRepository bDCompanyCategoryRepository,
        IBDCompanySiteRepository bDCompanySiteRepository,
        IObjectMapper objectMapper
        )
        {
            _bdformRepository = bdformRepository;
            _UberTransactionalDayRepository = UberTransactionalDayRepository;
            _EmployeeInfoRepository = employeeInfoRepository;
            _autoNoRepository = autoNoRepository;
            _cashHeadRepository = cashHeadRepository;
            _cashDetailRepository = cashDetailRepository;
            _iApprovalFlowDomainService = approvalFlowDomainService;
            _bDCompanyCategoryRepository = bDCompanyCategoryRepository;
            _bDCompanySiteRepository = bDCompanySiteRepository;
            _objectMapper = objectMapper;
        }

        [UnitOfWork]
        public async Task TransactionToSign()
        {
            Console.WriteLine("TransactionToSign called in UberDomainService.");
            List<UberTransactionalDay> toSignList= await _UberTransactionalDayRepository.GetUberTransactionalDayToSign();
            if (toSignList == null || toSignList.Count == 0)
            {
                Console.WriteLine("No transactions to sign.");
                return;
            }
            Console.WriteLine("Number of transactions to sign: " + toSignList.Count);
            List<UberTransactionalDay> toSignedList = new List<UberTransactionalDay>();
            try
            {
                for (int i = 0; i < toSignList.Count; i++)
                {
                    Result<List<BDCompanyCategoryParamDto>> categoryParamDtos =await getCompany(toSignList[i].EmployeeId);
                    var primaryItem = categoryParamDtos.data.FirstOrDefault(x => x.Primary == true);
                    var company = primaryItem.CompanyCategory;
                    var employee = await _EmployeeInfoRepository.QueryByEmplid(toSignList[i].EmployeeId);
                    if (employee != null)
                    {
                        Console.WriteLine($"Processing transaction for employee: {employee.emplid} - {employee.name}");
                        string rno = await _autoNoRepository.CreateCashUberRNo();
                        //放的预约日期
                        // 先格式化日期和时间部分
                        string formattedDate = DateTimeHelper.ConvertToDateFormat(toSignList[i].RequestDate.ToString()); // 应该返回 yyyy-MM-dd
                        string formattedTime = DateTimeHelper.ConvertTo24HourFormat(toSignList[i].RequestTime.ToString()); // 应该返回 HH:mm:ss
                                                                                                                           // 转换为 DateTime 类型
                        DateTime startdate = DateTimeHelper.ConvertToDateTime(formattedDate, formattedTime);
                        CashHead cashHead = new CashHead()
                        {
                            rno = rno,
                            formcode = "CASH_UBER",
                            payeeId = employee.emplid,
                            payeename = employee.name,
                            Program = toSignList[i].Program,
                            projectcode = null,
                            BusinessTripNo = null,
                            company = company,
                            cuser = employee.emplid,
                            cname = employee.name,
                            cdate = System.DateTime.Now,
                            currency= toSignList[i].LocalCurrencyCode,
                            amount = toSignList[i].TransactionAmountInHomeCurrency,
                            status = "pending_approval"
                        };
                        CashDetail cashDetail = new CashDetail()
                        {
                            rno = rno,
                            formcode = "CASH_UBER",
                            rdate = startdate,//乘车日期
                            Destination = toSignList[i].City + toSignList[i].DropOffAddress,
                            Origin = toSignList[i].City + toSignList[i].PickupAddress,
                            amount = toSignList[i].TransactionAmountInHomeCurrency,
                            cuser = employee.emplid,
                            cdate = System.DateTime.Now,
                            muser = null,
                            mdate = System.DateTime.Now,
                            remarks = toSignList[i].ExpenseMemo,
                            Passenger = employee.name,
                            deptid = toSignList[i].ExpenseCode,
                            company = company,
                        };
                        CashHead cashHead1 = await _cashHeadRepository.InsertAsync(cashHead);
                        CashDetail cashDetail1 = await _cashDetailRepository.InsertAsync(cashDetail);
                        Result<string> result = await _iApprovalFlowDomainService.ApplyUberApprovalFlow(cashHead, new List<CashDetail> { cashDetail });
                        if (result.status == 1)
                        {
                            //生成签核单据成功后，更新 UberTransactionalDay 的状态
                            toSignList[i].Company = company;
                            toSignList[i].rno = rno;
                            toSignList[i].SignStatus = "P";
                            toSignedList.Add(toSignList[i]);
                        }
                        else
                        {
                            Console.WriteLine($"Failed to create approval flow for Rno: {rno}. Error: {result.message}");
                            throw new Exception($"Failed to create approval flow for Rno: {rno}. Error: {result.message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Employee not found for Emplid: {toSignList[i].EmployeeId}");
                    }
                }
                if (toSignedList.Count > 0)
                {
                    Console.WriteLine($"Total transactions to be signed: {toSignedList.Count}");
                    //批量插入
                    await _UberTransactionalDayRepository.UpdateManyAsync(toSignedList);
                    Console.WriteLine($"Inserted {toSignedList.Count} transactions into UberTransactionalDay.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing transactions: {ex.Message}");
                throw ;
            }
            finally
            {
                Console.WriteLine("TransactionToSign end in UberDomainService.");
            }
        }

        async Task<Result<List<BDCompanyCategoryParamDto>>> getCompany(string userId)
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
                var query = from employee in await _EmployeeInfoRepository.WithDetailsAsync()
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
    }
}

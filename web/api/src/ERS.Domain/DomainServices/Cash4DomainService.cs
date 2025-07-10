using System.Data;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.IRepositories;
using ERS.Entities;
using ERS.Domain.IDomainServices;
using ERS.DTO.BDCar;
using ERS.Application.Contracts.DTO.Application;
using Newtonsoft.Json;
using Volo.Abp.ObjectMapping;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using ERS.DTO.Invoice;
namespace ERS.DomainServices
{
    public class Cash4DomainService : CommonDomainService, ICash4DomainService
    {
        private IBDMealAreaRepository _BDMealAreaRepository;
        private IBDExpRepository _BDExpRepository;
        private IEmpOrgRepository _EmpOrgRepository;
        private ICompanyRepository _CompanyRepository;
        private IEmployeeRepository _EmployeeRepository;
        private IBDCarRepository _BDCarRepository;
        private IEFormHeadRepository _EFormHeadRepository;
        private ICashHeadRepository _CashHeadRepository;
        private ICashAccountRepository _CashAccountRepository;
        private IAutoNoRepository _AutoNoRepository;
        private IEFormAlistRepository _EFormAlistRepository;
        private IEFormAuserRepository _EFormAuserRepository;
        private ICashDetailRepository _CashDetailRepository;
        private IInvoiceRepository _InvoiceRepository;
        private ICashFileRepository _CashFileRepository;
        private ICashAmountRepository _CashAmountRepository;
        private ICurrencyDomainService _CurrencyDomainService;
        private IInvoiceDomainService _InvoiceDomainService;
        private IApprovalPaperDomainService _EFormPaperDomainService;
        private IBDExpDomainService _BDExpDomainService;
        private IMinioDomainService _MinioDomainService;
        private IApprovalDomainService _ApprovalDomainService;
        private IObjectMapper _ObjectMapper;
        private IConfiguration _Configuration;
        private IBDInvoiceFolderRepository _BDInvoiceFolderRepository;
        private IConfiguration _configuration;
        private IApprovalPaperRepository _EFormPaperRepository;
        private IMinioDomainService _minioDomainService;
        public Cash4DomainService(
            IBDMealAreaRepository BDMealAreaRepository,
            IBDExpRepository BDExpRepository,
            IEmpOrgRepository EmpOrgRepository,
            ICompanyRepository CompanyRepository,
            IEmployeeRepository EmployeeRepository,
            IBDCarRepository BDCarRepository,
            IEFormHeadRepository EFormHeadRepository,
            ICashHeadRepository CashHeadRepository,
            ICashAccountRepository CashAccountRepository,
            IAutoNoRepository AutoNoRepository,
            IEFormAlistRepository EFormAlistRepository,
            IEFormAuserRepository EFormAuserRepository,
            ICashDetailRepository CashDetailRepository,
            IInvoiceRepository InvoiceRepository,
            ICashFileRepository CashFileRepository,
            ICashAmountRepository CashAmountRepository,
            ICurrencyDomainService CurrencyDomainService,
            IInvoiceDomainService InvoiceDomainService,
            IApprovalPaperDomainService EFormPaperDomainService,
            IBDExpDomainService BDExpDomainService,
            IMinioDomainService MinioDomainService,
            IApprovalDomainService ApprovalDomainService,
            IApprovalPaperRepository EFormPaperRepository,
            IObjectMapper ObjectMapper,
            IConfiguration Configuration, IBDInvoiceFolderRepository BDInvoiceFolderRepository, IConfiguration configuration, IMinioDomainService minioDomainService)
        {
            _BDMealAreaRepository = BDMealAreaRepository;
            _BDExpRepository = BDExpRepository;
            _EmpOrgRepository = EmpOrgRepository;
            _CompanyRepository = CompanyRepository;
            _EmployeeRepository = EmployeeRepository;
            _BDCarRepository = BDCarRepository;
            _EFormHeadRepository = EFormHeadRepository;
            _CashHeadRepository = CashHeadRepository;
            _CashAccountRepository = CashAccountRepository;
            _AutoNoRepository = AutoNoRepository;
            _EFormAlistRepository = EFormAlistRepository;
            _EFormAuserRepository = EFormAuserRepository;
            _CashDetailRepository = CashDetailRepository;
            _InvoiceRepository = InvoiceRepository;
            _CashFileRepository = CashFileRepository;
            _CashAmountRepository = CashAmountRepository;
            _CurrencyDomainService = CurrencyDomainService;
            _InvoiceDomainService = InvoiceDomainService;
            _EFormPaperDomainService = EFormPaperDomainService;
            _BDExpDomainService = BDExpDomainService;
            _MinioDomainService = MinioDomainService;
            _ApprovalDomainService = ApprovalDomainService;
            _ObjectMapper = ObjectMapper;
            _Configuration = Configuration;
            _BDInvoiceFolderRepository = BDInvoiceFolderRepository;
            _configuration = configuration;
            _EFormPaperRepository = EFormPaperRepository;
            _minioDomainService = minioDomainService;
        }
        /// <summary>
        /// 通用批量上传
        /// </summary>
        /// <param name="excelFile"></param>
        /// <returns></returns>
        /// 汇率
        /// 转换后的金额
        public async Task<Result<List<ReimbListDto>>> GetReimbursementListFromExcel(IFormFile excelFile, string company)
        {
            Result<List<ReimbListDto>> results = new Result<List<ReimbListDto>>()
            {
                data = new List<ReimbListDto>()
            };
            if (excelFile.FileName.ToLower().EndsWith(".xls") || excelFile.FileName.ToLower().EndsWith(".xlsx"))
            {
                DataTable dt = NPOIHelper.GetDataTableFromExcel(excelFile);
                var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
                {
                    expensedate = s[0],
                    chargedept = s[1].ToString().Trim(),
                    currency = s[2].ToString().Trim(),
                    applyamount = s[3],
                    summary = s[4].ToString().Trim(),
                    payeeid = s[5].ToString().Trim()
                }).ToList();
                if (list.Count <= 0)
                {
                    results.status = 2;
                    results.data = null;
                    results.message = L["ExcelNoData"];
                }
                var depts = (await _EmpOrgRepository.WithDetailsAsync()).Where(w => list.Select(x => x.chargedept).Contains(w.deptid)).ToList();
                var payeeids = (await _EmployeeRepository.WithDetailsAsync()).Where(w => list.Select(x => x.payeeid).Contains(w.emplid)).ToList();
                var accountquery = (await _CashAccountRepository.WithDetailsAsync()).Where(w => list.Select(w => w.payeeid).ToList().Contains(w.emplid)).Select(w => new { w.emplid, w.bank }).ToList();
                var empquery = (await _EmployeeRepository.WithDetailsAsync()).Where(w => list.Select(w => w.payeeid).ToList().Contains(w.emplid)).Select(w => new { w.emplid, w.cname, w.deptid }).ToList();
                string basecurr = (await _CompanyRepository.WithDetailsAsync()).Where(x => x.company == company).Select(x => x.BaseCurrency).FirstOrDefault();
                var payeeInfo = (await _EmployeeRepository.WithDetailsAsync()).Where(w => list.Select(x => x.payeeid).Contains(w.emplid)).ToList();
                //var banks = (await _CashAccountRepository.WithDetailsAsync()).Where(w => payeeInfo.Select(s => s.emplid).Contains(w.emplid)).AsNoTracking().Select(g => g.bank).Distinct().ToList();
                //對不同銀行別作卡控
                // if (banks.Count != 1)
                // {
                //     results.message = L["BatchUpload-NotSameBank"];
                //     results.data = null;
                //     results.status = 2;
                //     return results;
                // }
                foreach (var item in list)
                {
                    ReimbListDto reimbList = new ReimbListDto();
                    DateTime expdate;
                    if (!DateTime.TryParse(item.expensedate.ToString(), out expdate))
                    {
                        results.message = String.Format(L["IncorrectDate"], item.expensedate);
                        results.data = null;
                        results.status = 2;
                        return results;
                    }
                    if (depts.Where(w => w.deptid == item.chargedept).FirstOrDefault() == null)
                    {
                        results.message = L["DeptidError"] + "：" + item.chargedept;
                        results.data = null;
                        results.status = 2;
                        return results;
                    }
                    // if ((payeeids.Where(w => w.emplid == item.payeeid)).FirstOrDefault() == null)
                    // {
                    //     results.message = L["NoEmplidError"] + "：" + item.payeeid;
                    //     results.data = null;
                    //     results.status = 2;
                    //     return results;
                    // }
                    reimbList.cdate = Convert.ToDateTime(Convert.ToDateTime(item.expensedate).ToString("yyyy/MM/dd"));
                    reimbList.deptid = item.chargedept;
                    reimbList.currency = item.currency;
                    reimbList.amount = Convert.ToDecimal(item.applyamount);
                    reimbList.summary = item.summary;
                    reimbList.payeeid = item.payeeid;
                    reimbList.payeename = empquery.Where(w => w.emplid == item.payeeid).Select(w => w.cname).FirstOrDefault();
                    reimbList.bank = accountquery.Where(w => w.emplid == item.payeeid).Select(w => w.bank).FirstOrDefault();
                    reimbList.payeedept = empquery.Where(w => w.emplid == item.payeeid).Select(w => w.deptid).FirstOrDefault();
                    if (item.currency == basecurr)
                    {
                        reimbList.rate = 1;
                    }
                    else
                    {
                        reimbList.rate = (await _CurrencyDomainService.queryRate(item.currency, basecurr)).ccurrate;
                    }
                    reimbList.baseamt = reimbList.amount * reimbList.rate;
                    results.data.Add(reimbList);
                }
            }
            else
            {
                results.data = null;
                results.status = 2;
                results.message = L["OnlyUploadExcel"];
            }
            return results;
        }
        /// <summary>
        /// 批量上传读取Excel获取数据(誤餐)
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<Result<List<OvertimeMealDto>>> GetOvertimeMealCostListFromExcel(IFormFile excelFile, string company)
        {
            Result<List<OvertimeMealDto>> results = new Result<List<OvertimeMealDto>>()
            {
                data = new List<OvertimeMealDto>()
            };
            //检查传入文件是否为Excel
            if (excelFile.FileName.ToLower().EndsWith(".xls") || excelFile.FileName.ToLower().EndsWith("xlsx"))
            {
                DataTable dt = NPOIHelper.GetDataTableFromExcel(excelFile);
                var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
                {
                    expdeptid = s[0].ToString().Trim(),
                    businesscity = s[1].ToString().Trim(),
                    cdate = s[2],
                    departuretime = s[3],
                    backtime = s[4],
                    summary = s[5].ToString().Trim(),
                    payeeid = s[6].ToString().Trim()
                }).ToList();
                if (list.Count > 0)
                {
                    var bdexpquery = (await _BDExpRepository.WithDetailsAsync()).Where(y => y.company == company).ToList();
                    var accountquery = (await _CashAccountRepository.WithDetailsAsync()).Where(w => list.Select(w => w.payeeid).ToList().Contains(w.emplid)).Select(w => new { w.emplid, w.bank }).ToList();
                    var empquery = (await _EmployeeRepository.WithDetailsAsync()).Where(w => list.Select(w => w.payeeid).ToList().Contains(w.emplid)).Select(w => new { w.emplid, w.cname, w.deptid }).ToList();
                    var emporgquery = (await _EmpOrgRepository.WithDetailsAsync()).Where(w => list.Select(s => s.expdeptid).Contains(w.deptid)).Select(x => new { x.deptid, x.tree_level_num }).ToList();
                    List<string> expnamelist = bdexpquery.Select(x => x.expname).ToList();
                    List<string> citylist = list.Select(w => w.businesscity).ToList();
                    string basecurr = (await _CompanyRepository.WithDetailsAsync()).Where(x => x.company == company).Select(x => x.BaseCurrency).FirstOrDefault();
                    var citydata = (await _BDMealAreaRepository.WithDetailsAsync()).Where(w => citylist.Contains(w.city)).ToList();
                    var payeeInfo = (await _EmployeeRepository.WithDetailsAsync()).Where(w => list.Select(x => x.payeeid).Contains(w.emplid)).ToList();
                    //var banks = (await _CashAccountRepository.WithDetailsAsync()).Where(w => payeeInfo.Select(s => s.emplid).Contains(w.emplid)).AsNoTracking().Select(g => g.bank).Distinct().ToList();
                    //對不同銀行別作卡控
                    // if (banks.Count != 1)
                    // {
                    //     results.message = L["BatchUpload-NotSameBank"];
                    //     results.data = null;
                    //     results.status = 2;
                    //     return results;
                    // }
                    foreach (var item in list)
                    {
                        OvertimeMealDto result = new OvertimeMealDto();
                        // //報銷情景校驗
                        // if (!expnamelist.Contains(item.expname) && !bdexpquery.Select(s => s.keyword).Contains(item.expname) && !bdexpquery.Select(s => s.description).Contains(item.expname))
                        // {
                        //     results.message = L["NoReimbursementScene"] + "：" + item.expname;
                        //     results.data = null;
                        //     results.status = 2;
                        //     return results;
                        // }
                        //部門代碼校驗
                        if (!emporgquery.Select(s => s.deptid).Contains(item.expdeptid))
                        {
                            results.message = String.Format(L["NoExpDeptid"], item.expdeptid);
                            results.data = null;
                            results.status = 2;
                            return results;
                        }
                        if (!String.IsNullOrEmpty(item.expdeptid) && emporgquery.Where(s => s.deptid == item.expdeptid).FirstOrDefault().tree_level_num > 7)
                        {
                            results.message = String.Format(L["ExpDeptidNotMinisterial"], item.expdeptid);
                            results.data = null;
                            results.status = 2;
                            return results;
                        }
                        //日期校驗
                        DateTime dtDate;
                        if (!DateTime.TryParse(item.cdate.ToString(), out dtDate))
                        {
                            results.message = String.Format(L["IncorrectDate"], item.cdate);
                            results.data = null;
                            results.status = 2;
                            return results;
                        }
                        //出发时点校驗
                        DateTime startTime;
                        if (!DateTime.TryParse(item.departuretime.ToString(), out startTime))
                        {
                            results.message = String.Format(L["IncorrectDate"], item.departuretime);
                            results.data = null;
                            results.status = 2;
                            return results;
                        }
                        //回到时点校驗
                        DateTime endTime;
                        if (!DateTime.TryParse(item.backtime.ToString(), out endTime))
                        {
                            results.message = String.Format(L["IncorrectDate"], item.backtime);
                            results.data = null;
                            results.status = 2;
                            return results;
                        }
                        //城市校驗
                        List<BDMealArea> bDMealAreas = citydata.Where(x => x.city == item.businesscity).ToList();
                        if (bDMealAreas.Count <= 0)
                        {
                            results.message = L["NoCity"] + "：" + item.businesscity;
                            results.data = null;
                            results.status = 2;
                            return results;
                        }
                        //受款人校验
                        // if (!await _EmployeeRepository.EmpIsExist(item.payeeid))
                        // {
                        //     results.message = L["PayeeNotExist"] + "：" + item.payeeid;
                        //     results.data = null;
                        //     results.status = 2;
                        //     return results;
                        // }
                        // result.expname = item.expname;
                        // result.expcode = bdexpquery.Where(y => y.expname == item.expname).Select(x => x.expcode).FirstOrDefault();
                        result.businesscity = item.businesscity;
                        result.deptid = item.expdeptid;
                        result.cdate = dtDate;
                        result.departuretime = startTime;
                        result.backtime = endTime;
                        result.summary = item.summary;
                        result.payeeid = item.payeeid;
                        result.payeename = empquery.Where(w => w.emplid == item.payeeid).Select(w => w.cname).FirstOrDefault();
                        result.bank = accountquery.Where(w => w.emplid == item.payeeid).Select(w => w.bank).FirstOrDefault();
                        result.payeedept = empquery.Where(w => w.emplid == item.payeeid).Select(w => w.deptid).FirstOrDefault();
                        TravelDto travelDto = new TravelDto()
                        {
                            city = item.businesscity,
                            gotime = Convert.ToDateTime(result.cdate.ToString("yyyy/MM/dd") + " " + result.departuretime.ToString("HH:mm")),
                            backtime = Convert.ToDateTime(result.cdate.ToString("yyyy/MM/dd") + " " + result.backtime.ToString("HH:mm")),
                            company = company
                        };
                        TravelDto calresult = _BDMealAreaRepository.TravelCount(bDMealAreas, travelDto);
                        if (calresult == null && calresult.amount == 0)
                        {
                            result.currency = basecurr;
                        }
                        else
                        {
                            result.amount = calresult.amount;
                            result.currency = calresult.currency;
                        }
                        if (calresult.currency == basecurr)
                        {
                            result.rate = 1;
                            result.baseamt = result.amount;
                        }
                        else if (calresult.currency != null)
                        {
                            result.rate = (await _CurrencyDomainService.queryRate(result.currency, basecurr)).ccurrate;
                            result.baseamt = result.amount * result.rate;
                        }
                        results.data.Add(result);
                    }
                }
                else
                {
                    results.data = null;
                    results.status = 2;
                    results.message = L["ExcelNoData"];
                }
            }
            else
            {
                results.data = null;
                results.status = 2;
                results.message = L["OnlyUploadExcel"];
            }
            return results;
        }
        /// <summary>
        /// 油费批量上传
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<Result<List<SelfDriveCostDto>>> GetSelfDriveCostListFromExcel(IFormFile excelFile, string company)
        {
            Result<List<SelfDriveCostDto>> results = new Result<List<SelfDriveCostDto>>()
            {
                data = new List<SelfDriveCostDto>()
            };
            //检查传入文件是否为Excel
            if (excelFile.FileName.ToLower().EndsWith(".xls") || excelFile.FileName.ToLower().EndsWith(".xlsx"))
            {
                DataTable dt = NPOIHelper.GetDataTableFromExcel(excelFile);
                var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
                {
                    deptid = s[0].ToString().Trim(),
                    departureplace = s[1].ToString().Trim(),
                    cdate = s[2].ToString().Trim(),
                    vehicletype = s[3].ToString().Trim(),
                    kilometers = s[4].ToString().Trim(),
                    summary = s[5].ToString().Trim(),
                    payeeid = s[6].ToString().Trim()
                }).ToList();
                if (list.Count > 0)
                {
                    var bdexpquery = (await _BDExpRepository.WithDetailsAsync()).Where(y => y.company == company).ToList();
                    var accountquery = (await _CashAccountRepository.WithDetailsAsync()).Where(w => list.Select(w => w.payeeid).ToList().Contains(w.emplid)).Select(w => new { w.emplid, w.bank }).ToList();
                    var empquery = (await _EmployeeRepository.WithDetailsAsync()).Where(w => list.Select(w => w.payeeid).ToList().Contains(w.emplid)).Select(w => new { w.emplid, w.cname, w.deptid }).ToList();
                    List<string> expnamepquery = (await _BDExpRepository.WithDetailsAsync()).Where(y => y.company == company).Select(x => x.expname).ToList();
                    var emporgquery = (await _EmpOrgRepository.WithDetailsAsync()).Where(w => list.Select(s => s.deptid).Contains(w.deptid)).Select(x => new { x.deptid, x.tree_level_num }).ToList();
                    List<string> cartypequery = (await _BDCarRepository.WithDetailsAsync()).Select(x => x.name).Distinct().ToList();
                    var carvaluequery = (await _BDCarRepository.WithDetailsAsync()).ToList();
                    var payeeids = (await _EmployeeRepository.WithDetailsAsync()).Where(w => list.Select(x => x.payeeid).Contains(w.emplid)).ToList();
                    //var banks = (await _CashAccountRepository.WithDetailsAsync()).Where(w => payeeids.Select(s => s.emplid).Contains(w.emplid)).AsNoTracking().Select(g => g.bank).Distinct().ToList();
                    //對不同銀行別作卡控
                    // if (banks.Count != 1)
                    // {
                    //     results.message = L["BatchUpload-NotSameBank"];
                    //     results.data = null;
                    //     results.status = 2;
                    //     return results;
                    // }
                    foreach (var item in list)
                    {
                        SelfDriveCostDto selfDriveCostDto = new SelfDriveCostDto();
                        // if (!expnamepquery.Contains(item.expname) && !bdexpquery.Select(s => s.keyword).Contains(item.expname) && !bdexpquery.Select(s => s.description).Contains(item.expname))
                        // {
                        //     results.message = L["NoReimbursementScene"];
                        //     results.status = 2;
                        //     return results;
                        // }
                        if (!emporgquery.Select(s => s.deptid).Contains(item.deptid))
                        {
                            results.message = String.Format(L["NoExpDeptid"], item.deptid);
                            results.data = null;
                            results.status = 2;
                            return results;
                        }
                        if (!String.IsNullOrEmpty(item.deptid) && emporgquery.Where(s => s.deptid == item.deptid).FirstOrDefault().tree_level_num > 7)
                        {
                            results.message = String.Format(L["ExpDeptidNotMinisterial"], item.deptid);
                            results.data = null;
                            results.status = 2;
                            return results;
                        }
                        DateTime dtDate;
                        if (!DateTime.TryParse(item.cdate, out dtDate))
                        {
                            results.message = String.Format(L["IncorrectDate"], item.cdate);
                            results.data = null;
                            results.status = 2;
                            return results;
                        }
                        if (!cartypequery.Contains(item.vehicletype))
                        {
                            results.message = L["NoVehicleType"];
                            results.data = null;
                            results.status = 2;
                            return results;
                        }
                        decimal k;
                        if (Decimal.TryParse(item.kilometers, out k))
                        {
                            if (k <= 0)
                            {
                                results.message = L["IncorrectKilometers"];
                                results.data = null;
                                results.status = 2;
                                return results;
                            }
                            selfDriveCostDto.kilometers = k;
                        }
                        else
                        {
                            results.message = L["IncorrectKilometers"];
                            results.data = null;
                            results.status = 2;
                            return results;
                        }
                        // if ((payeeids.Where(w => w.emplid == item.payeeid)).FirstOrDefault() == null)
                        // {
                        //     results.message = String.Format(L["PayeeNotExist"], item.payeeid);
                        //     results.data = null;
                        //     results.status = 2;
                        //     return results;
                        // }
                        //selfDriveCostDto.expname = item.expname;
                        selfDriveCostDto.deptid = item.deptid;
                        selfDriveCostDto.summary = item.summary;
                        selfDriveCostDto.departureplace = item.departureplace;
                        selfDriveCostDto.cdate = dtDate;
                        selfDriveCostDto.vehicletype = item.vehicletype;
                        selfDriveCostDto.vehiclevalue = carvaluequery.Where(x => x.name == item.vehicletype).Select(x => x.type).FirstOrDefault();
                        selfDriveCostDto.total = carvaluequery.Where(x => x.name == item.vehicletype).Distinct().Select(x => x.amount).FirstOrDefault() * selfDriveCostDto.kilometers;
                        selfDriveCostDto.rate = 1;
                        selfDriveCostDto.payeeid = item.payeeid;
                        //selfDriveCostDto.expcode = bdexpquery.Where(y => y.expname == item.expname).Select(x => x.expcode).FirstOrDefault();
                        selfDriveCostDto.payeename = empquery.Where(w => w.emplid == item.payeeid).Select(w => w.cname).FirstOrDefault();
                        selfDriveCostDto.bank = accountquery.Where(w => w.emplid == item.payeeid).Select(w => w.bank).FirstOrDefault();
                        selfDriveCostDto.payeedept = empquery.Where(w => w.emplid == item.payeeid).Select(w => w.deptid).FirstOrDefault();
                        results.data.Add(selfDriveCostDto);
                    }
                }
            }
            else
            {
                results.data = null;
                results.status = 2;
                results.message = L["OnlyUploadExcel"];
            }
            return results;
        }
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P")
        {
            Result<CashResult> result = new Result<CashResult>();
            CashResult cash = new CashResult();
            result.status = 2;
            CashHead list = TransformAndSetData(formCollection, user);
            //判断单据中是否只有一个银行
            if (list.CashDetailList.Where(s => !String.IsNullOrEmpty(s.bank)).Select(w => ChineseConverter.Convert(w.bank, ChineseConversionDirection.TraditionalToSimplified)).Distinct().Count() > 1)
            {
                result.message = L["BatchUpload-NotSameBank"];
                return result;
            }
            if (!string.IsNullOrEmpty(list.rno))
            {
                //判断单据是否在签核中
                if (await _EFormHeadRepository.IsSigned(list.rno))
                {
                    result.message = L["008"];
                    return result;
                }
                if (await _CashHeadRepository.IsSelfCreate(list.rno, user))
                {
                    result.message = L["009"];
                    return result;
                }
            }
            // 检查选择发票是否有异常
            List<Guid?> Iids = list.InvoiceList.Where(i => i.invoiceid != null).Select(i => i.invoiceid).Distinct().ToList();
            if (status.Equals("P") && await _BDInvoiceFolderRepository.CheckInvoiceIsAbnormal(Iids, list.payeeId))
            {
                result.message = L["InvoiceIsAbnormal"];
                return result;
            }
            list.SetPayeeAccount(await _CashAccountRepository.GetAccount(list.payeeId));
            if (status == "T")
                list.SetKeepStatus("RQ501");
            if (!list.CheckIsExistRno())
            {
                string rno = await _AutoNoRepository.CreateCash4No();
                list.SetRno(rno);
                await _EFormHeadRepository.InsertAsync(list.EFormHead);
            }
            else
            {
                await DeleteData(list);
                list.SetRno(list.rno);
                EFormHead temp = await _EFormHeadRepository.GetByNo(list.rno);
                temp.ChangeStatus(list.EFormHead.status);
                temp.SetApid(list.EFormHead.apid);
                list.UpdateEFormHead(temp);
            }
            await SaveInvoiceFolder(list, Iids);
            //检查发票是否可请款
            Result<CashResult> invoviceResult = await _InvoiceDomainService.checkInvoicePaid(list, token);
            if (invoviceResult.status == 2)
            {
                return invoviceResult;
            }
            if (status.Equals("P"))
                await _BDInvoiceFolderRepository.InsertRno(Iids, list.rno);
            List<string> codes = list.CashDetailList.Select(w => w.expcode).Distinct().ToList();
            List<string> acctcodes = list.CashDetailList.Select(w => w.acctcode).Distinct().ToList();
            string type = await _BDExpDomainService.GetSignDType(list.company, codes, acctcodes);
            list.Setdtype(type);
            // file
            await SaveFile(formCollection, list, Iids);
            cash.Stat = false;
            if (status.Equals("P") && invoviceResult.data.Stat)
            {
                await _EFormPaperDomainService.addPaper(list.rno, list.formcode, user, list.company);
                cash.Stat = true;
            }
            bool saveFlag = await InsertData(list);
            if (saveFlag)
            {
                if (status.Equals("P"))
                {
                    await _ApprovalDomainService.CreateSignSummary(list, invoviceResult.data.ElecInvStat, token);
                    if (list.InvoiceList.Count > 0 && _configuration.GetSection("isDev").Value != "true")
                    {
                        List<UpdatePayStatDto> invs = _ObjectMapper.Map<List<Invoice>, List<UpdatePayStatDto>>(list.InvoiceList.ToList());
                        ERSApplyDto eRSApplyDto = new()
                        {
                            Invoices = _ObjectMapper.Map<List<UpdatePayStatDto>, List<ERSInvDto>>(invs),
                            ERSRno = list.rno,
                            ERSUser = list.cuser,
                            ERSComCode = list.company
                        };
                        await _InvoiceDomainService.UpdateInvStatToRequested(eRSApplyDto, token);
                        //await _InvoiceDomainService.UpdateInvoiceToRequested(invs, token);
                    }
                }
            }
            cash.rno = list.rno;
            result.data = cash;
            result.message = (cash.Stat ? L["AddPaperTipMessage"] : L["UnAddPaperTipMessage"]) + (invoviceResult.data.ElecButNotPdf ? L["AddElecInvTipMessage"] : "");
            result.status = 1;
            return result;
        }
        public async Task<Result<List<PayeeDto>>> GetPayeeInfo(string keyword)
        {
            var result = new Result<List<PayeeDto>>();
            var empquery = (await _EmployeeRepository.WithDetailsAsync()).Where(w => w.emplid.Contains(keyword)).AsNoTracking();
            var accountquery = (await _CashAccountRepository.WithDetailsAsync()).Where(w => empquery.Select(s => s.emplid).Contains(w.emplid)).AsNoTracking();
            var unionquery = empquery.Select(w => new PayeeDto{
                payeeid = w.emplid,
                payeedept = w.deptid,
                payeename = w.cname,
                bank = accountquery.FirstOrDefault(s => s.emplid == w.emplid).bank
            })
            .AsParallel()
            .OrderBy(s => s.payeeid.IndexOf(keyword))
            .ThenBy(s => s.payeeid)
            .Take(10);
            result.data = unionquery.ToList();
            return result;
        }
        CashHead TransformAndSetData(IFormCollection formCollection, string user)
        {
            string head = formCollection["head"];
            string detail = formCollection["detail"];
            string file = formCollection["file"];
            string inv = formCollection["inv"];
            string amount = formCollection["amount"];
            CashHeadDto hData = JsonConvert.DeserializeObject<CashHeadDto>(head);
            IList<CashDetailDto> dData = JsonConvert.DeserializeObject<IList<CashDetailDto>>(detail);
            IList<CashFileDto> fData = JsonConvert.DeserializeObject<IList<CashFileDto>>(file);
            IList<InvoiceDto> iData = JsonConvert.DeserializeObject<IList<InvoiceDto>>(inv);
            CashAmountDto aData = JsonConvert.DeserializeObject<CashAmountDto>(amount);
            CashHead list = _ObjectMapper.Map<CashHeadDto, CashHead>(hData);
            IList<CashDetail> detailData = _ObjectMapper.Map<IList<CashDetailDto>, IList<CashDetail>>(dData);
            IList<CashFile> fileData = _ObjectMapper.Map<IList<CashFileDto>, IList<CashFile>>(fData);
            IList<Invoice> invData = _ObjectMapper.Map<IList<InvoiceDto>, IList<Invoice>>(iData);
            CashAmount amountData = _ObjectMapper.Map<CashAmountDto, CashAmount>(aData);
            list.SetCash4(list, user);
            list.AddCash4Detail(detailData);
            list.AddFile(fileData);
            list.AddInvoice(invData);
            list.AddCashAmount(amountData);
            return list;
        }
        async Task DeleteData(CashHead list)
        {
            List<EFormAlist> eFormAlists = await _EFormAlistRepository.GetListAsync(w => w.rno == list.rno);
            if (eFormAlists.Count > 0)
            {
                await _EFormAlistRepository.DeleteManyAsync(eFormAlists);
            }
            List<EFormAuser> eFormAuser = await _EFormAuserRepository.GetListAsync(i => i.rno == list.rno);
            if (eFormAuser.Count > 0) await _EFormAuserRepository.DeleteManyAsync(eFormAuser);
            await _CashHeadRepository.DeleteAsync(await _CashHeadRepository.GetByNo(list.rno));
            await _CashDetailRepository.DeleteManyAsync(await _CashDetailRepository.GetByNo(list.rno));
            await _InvoiceRepository.DeleteManyAsync(await _InvoiceRepository.GetByNo(list.rno));
            await _CashFileRepository.DeleteManyAsync(await _CashFileRepository.GetByNo(list.rno));
            await _CashAmountRepository.DeleteAsync(await _CashAmountRepository.GetByNo(list.rno));
            List<ApprovalPaper> eFormPapers = (await _EFormPaperRepository.GetByNo(list.rno)).ToList();
            if (eFormPapers.Count > 0)
            {
                await _EFormPaperRepository.DeleteManyAsync(eFormPapers);
            }
        }
        async Task<bool> InsertData(CashHead list)
        {
            try
            {
                await _CashHeadRepository.InsertAsync(list);
                await _CashDetailRepository.InsertManyAsync(list.CashDetailList);
                await _InvoiceRepository.InsertManyAsync(list.InvoiceList);
                await _CashFileRepository.InsertManyAsync(list.CashFileList);
                await _CashAmountRepository.InsertAsync(list.CashAmount);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        async Task SaveInvoiceFolder(CashHead list, List<Guid?> ids)
        {
            string area = await _minioDomainService.GetMinioArea(list.cuser);
            List<BDInvoiceFolder> invoices = await _BDInvoiceFolderRepository.GetInvInfoListById(ids);
            List<CashFile> fileList = new();
            foreach (var invoice in invoices)
            {
                var temp = list.InvoiceList.Where(i => i.invoiceid == invoice.Id).First();
                string path = "";
                if (!string.IsNullOrEmpty(invoice.filepath))
                    path = await _MinioDomainService.CopyInvoiceAsync(list.rno, invoice.filepath, invoice.invno,area);
                CashFile cashFile = new CashFile()
                {
                    rno = list.rno,
                    seq = temp.seq,
                    item = temp.item,
                    category = invoice.invtype,
                    filetype = !string.IsNullOrEmpty(invoice.filepath) ? invoice.category : "",
                    path = path,
                    filename = !string.IsNullOrEmpty(invoice.filepath) ? invoice.invtype + invoice.amount.ToString() + Path.GetExtension(invoice.filepath) : "",
                    formcode = list.formcode,
                    tofn = !string.IsNullOrEmpty(invoice.filepath) ? Path.GetFileName(invoice.filepath) : "",
                    ishead = "Y",
                    status = "I",
                    company = list.company,
                    cdate = DateTime.Now,
                    cuser = list.cuser
                };
                list.AddCashFile(cashFile);
            }
        }
        async Task SaveFile(IFormCollection formCollection, CashHead list, List<Guid?> ids)
        {
            //List<BDInvoiceFolder> invoices = await _BDInvoiceFolderRepository.GetInvInfoListById(ids);
            //foreach (var invoice in invoices)
            //    if (!string.IsNullOrEmpty(invoice.filepath)) await _MinioDomainService.CopyInvoiceAsync(list.rno, invoice.filepath);
            string area = await _minioDomainService.GetMinioArea(list.cuser);
            Dictionary<string, int> fileCount = new Dictionary<string, int>();
            List<string> fileNames = formCollection.Files.Select(w => w.FileName).ToList();
            foreach (var file in formCollection.Files)
            {
                string path = "";
                string type = "";
                if (!string.IsNullOrEmpty(_Configuration.GetSection("UnitTest")?.Value))
                    type = "image/jpeg";
                else
                    type = file.ContentType;
                string fileName = file.FileName;
                using (var steam = file.OpenReadStream())
                {
                    if (fileCount.ContainsKey(fileName))
                    {
                        int count = fileCount[fileName];
                        fileCount[fileName] = count + 1;
                        fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)} ({count}){Path.GetExtension(file.FileName)}";
                    }
                    else
                    {
                        fileCount.Add(fileName, 1);
                    }
                    path = await _MinioDomainService.PutObjectAsync(list.rno, fileName, steam, type, area);
                }
                list.SetOriginalFileNameAndSavePath(file.Name, path, fileName, type);
            }
        }
    }
}

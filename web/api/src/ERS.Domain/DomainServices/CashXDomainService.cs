using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.Application.Contracts.DTO.Emporg;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.Application.CashX;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
namespace ERS.DomainServices
{
    public class CashXDomainService : CommonDomainService, ICashXDomainService
    {
        private IAutoNoRepository _AutoNoRepository;
        private IObjectMapper _ObjectMapper;
        private IEFormHeadRepository _EFormHeadRepository;
        private ICashHeadRepository _CashHeadRepository;
        private IConfiguration _Configuration;
        private ICashAccountRepository _CashAccountRepository;
        private IEFormAlistRepository _EFormAlistRepository;
        private IEFormAuserRepository _EFormAuserRepository;
        private ICashDetailRepository _CashDetailRepository;
        private ICashFileRepository _CashFileRepository;
        private ICashAmountRepository _CashAmountRepository;
        private IBDExpDomainService _BDExpDomainService;
        private IMinioDomainService _MinioDomainService;
        private IApprovalDomainService _ApprovalDomainService;
        private ICompanyRepository _CompanyRepository;
        private IBDExpRepository _BDExpRepository;
        private IComBankRepository _ComBankRepository;
        private IRepository<SAPExchRate, Guid> _SAPExchRateRepository;
        private IRepository<BdPayTyp, Guid> _BdPayTypRepository;
        private IEmporgService _EmporgService;
        private IAccountDomainService _AccountDomainService;
        private IMinioDomainService _minioDomainService;
        public CashXDomainService(
            IAutoNoRepository AutoNoRepository,
            IObjectMapper ObjectMapper,
            IEFormHeadRepository EFormHeadRepository,
            ICashHeadRepository CashHeadRepository,
            ICashAccountRepository CashAccountRepository,
            IConfiguration Configuration,
            IEFormAlistRepository EFormAlistRepository,
            IEFormAuserRepository EFormAuserRepository,
            ICashDetailRepository CashDetailRepository,
            ICashFileRepository CashFileRepository,
            IBDExpDomainService BDExpDomainService,
            IMinioDomainService MinioDomainService,
            IApprovalDomainService ApprovalDomainService,
            ICompanyRepository CompanyRepository,
            IBDExpRepository BDExpRepository,
            IComBankRepository ComBankRepository,
            IRepository<SAPExchRate, Guid> SAPExchRateRepository,
            IRepository<BdPayTyp, Guid> BdPayTypRepository,
            ICashAmountRepository CashAmountRepository,
            IAccountDomainService AccountDomainService,
            IMinioDomainService minioDomainService
        )
        {
            _AutoNoRepository = AutoNoRepository;
            _ObjectMapper = ObjectMapper;
            _EFormHeadRepository = EFormHeadRepository;
            _CashHeadRepository = CashHeadRepository;
            _CashAccountRepository = CashAccountRepository;
            _Configuration = Configuration;
            _EFormAlistRepository = EFormAlistRepository;
            _EFormAuserRepository = EFormAuserRepository;
            _CashDetailRepository = CashDetailRepository;
            _CashFileRepository = CashFileRepository;
            _BDExpDomainService = BDExpDomainService;
            _MinioDomainService = MinioDomainService;
            _ApprovalDomainService = ApprovalDomainService;
            _CompanyRepository = CompanyRepository;
            _BDExpRepository = BDExpRepository;
            _ComBankRepository = ComBankRepository;
            _SAPExchRateRepository = SAPExchRateRepository;
            _BdPayTypRepository = BdPayTypRepository;
            _CashAmountRepository = CashAmountRepository;
            _AccountDomainService = AccountDomainService;
            _minioDomainService = minioDomainService;
        }
        public async Task<Result<string>> GenerateSummary(SummaryInfoDto summaryInfo)
        {
            string company = (await _CompanyRepository.WithDetailsAsync()).Where(w => w.CompanyCategory == summaryInfo.companycode).First()?.company;
            string bank = (await _AccountDomainService.QueryBanks(company)).data?.FirstOrDefault(w => w == summaryInfo.payname);
            string bankAbbr = (await _ComBankRepository.WithDetailsAsync()).FirstOrDefault(w => w.banid == bank && w.company == company)?.bank_abbr;
            Result<string> result = new()
            {
                data = DateTime.Now.ToString("yy/MM") + "支付" + summaryInfo.summary + summaryInfo.keyword + "-" + (!String.IsNullOrEmpty(bankAbbr) ? bankAbbr : summaryInfo.payname) + "-" + summaryInfo.companycode
            };
            return result;
        }
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P")
        {
            Result<CashResult> result = new();
            CashResult cash = new();
            result.status = 2;
            CashHead list = TransformAndSetData(formCollection, user);
            //一份薪资请款单只能一个公司别
            if (list.CashDetailList.Select(w => w.company).Distinct().Count() > 1)
            {
                result.message = L["BatchUpload-NotDifferentCom"];
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
            list.SetPayeeAccount(await _CashAccountRepository.GetAccount(list.payeeId));
            if (status == "T")
                list.SetKeepStatus("RQ701");
            if (!list.CheckIsExistRno())
            {
                string rno = await _AutoNoRepository.CreateCashXNo();
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
            List<string> codes = list.CashDetailList.Select(w => w.expcode).Distinct().ToList();
            List<string> acctcodes = list.CashDetailList.Select(w => w.acctcode).Distinct().ToList();
            string type = await _BDExpDomainService.GetSignDType(list.company, codes, acctcodes);
            list.Setdtype(type);
            // file
            await SaveFile(formCollection, list);
            cash.Stat = false;
            await InsertData(list);
            if (status.Equals("P"))
            {
                await _ApprovalDomainService.CreateSignSummary(list, true, token);
            }
            cash.Stat = true;
            cash.rno = list.rno;
            result.data = cash;
            result.status = 1;
            return result;
        }
        public async Task<Result<List<ApplyInfoDto>>> GetApplyInfoFromExcel(IFormFile excelFile, string company)
        {
            Result<List<ApplyInfoDto>> results = new()
            {
                data = new()
            };
            if (excelFile.FileName.ToLower().EndsWith(".xls") || excelFile.FileName.ToLower().EndsWith(".xlsx"))
            {
                DataTable dt = NPOIHelper.GetDataTableFromExcel(excelFile);
                var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
                {
                    companycode = s[0].ToString().Trim(),
                    expinfo = s[1].ToString().Trim(),
                    reqdate = s[2].ToString().Trim(),
                    salarydate = s[3].ToString().Trim(),
                    bank = s[4].ToString().Trim(),
                    payway = s[5].ToString().Trim(),
                    currency = s[6].ToString().Trim(),
                    amount = s[7].ToString().Trim()
                }).ToList();
                if (list.Count > 0)
                {
                    var comQuery = (await _CompanyRepository.WithDetailsAsync()).ToList();
                    var xzExpQuery = (await _BDExpRepository.WithDetailsAsync()).Select(w => new { w.expcode, w.description, w.company, w.keyword }).ToList();
                    var comBankQuery = await _AccountDomainService.QueryBanks(company);
                    var currQuery = (await _SAPExchRateRepository.WithDetailsAsync()).Select(w => w.ccurfrom).Distinct().ToList();
                    var payTypeQuery = (await _BdPayTypRepository.WithDetailsAsync()).Select(w => new { w.payname, w.paytyp }).ToList();
                    foreach (var item in list)
                    {
                        if (!comQuery.Select(w => w.CompanyCategory).Contains(item.companycode))
                        {
                            results.status = 2;
                            results.data = null;
                            results.message = String.Format(L["CompanyCode-NotExist"], item.companycode);
                            return results;
                        }
                        if (!comQuery.Where(w => w.company == company).Select(s => s.CompanyCategory).Contains(item.companycode))
                        {
                            results.status = 2;
                            results.data = null;
                            results.message = String.Format(L["BatchUpload-NotBelongCom"], item.companycode, company);
                            return results;
                        }
                        if (!xzExpQuery.Select(w => w.description).Contains(item.expinfo))
                        {
                            results.status = 2;
                            results.data = null;
                            results.message = String.Format(L["ExpScene-NotExist"], item.expinfo);
                            return results;
                        }
                        if (!IsDate(item.reqdate))
                        {
                            results.status = 2;
                            results.data = null;
                            results.message = String.Format(L["ReqDate-Incorrect"], item.reqdate);
                            return results;
                        }
                        if (!IsFutureOrCurrentDate(item.reqdate))
                        {
                            results.status = 2;
                            results.data = null;
                            results.message = String.Format(L["ReqDate-Valid"], item.reqdate);
                            return results;
                        }
                        if (!IsYearMonth(item.salarydate) && !IsYear(item.salarydate))
                        {
                            results.status = 2;
                            results.data = null;
                            results.message = String.Format(L["SalaryDate-Incorrect"], item.salarydate);
                            return results;
                        }
                        if (!comBankQuery.data.Contains(item.bank))
                        {
                            if (item.bank != "Cash")
                            {
                                results.status = 2;
                                results.data = null;
                                results.message = String.Format(L["Bank-NotExist"], item.bank);
                                return results;
                            }
                        }
                        if (item.payway == "現金")
                        {
                            if (item.bank != "Cash")
                            {
                                results.status = 2;
                                results.data = null;
                                results.message = L["CashBank-Check"];
                                return results;
                            }
                        }
                        if (!currQuery.Contains(item.currency))
                        {
                            results.status = 2;
                            results.data = null;
                            results.message = String.Format(L["Currency-NotExist"], item.currency);
                            return results;
                        }
                        if (!IsNumber(item.amount))
                        {
                            results.status = 2;
                            results.data = null;
                            results.message = String.Format(L["Amount-Incorrect"], item.amount);
                            return results;
                        }
                        var xzExpInfo = xzExpQuery.Where(w => w.description == item.expinfo && w.company == company).FirstOrDefault();
                        var payTypeInfo = payTypeQuery.Where(w => w.payname == item.payway).FirstOrDefault();
                        SummaryInfoDto summaryInfoDto = new()
                        {
                            summary = item.salarydate,
                            keyword = xzExpInfo.keyword,
                            payname = item.bank,
                            companycode = item.companycode
                        };
                        ApplyInfoDto applyInfoDto = new()
                        {
                            companycode = item.companycode,
                            expinfo = new()
                            {
                                expcode = xzExpInfo.expcode,
                                description = xzExpInfo.description
                            },
                            reqdate = Convert.ToDateTime(item.reqdate),
                            salarydate = item.salarydate,
                            bank = item.bank,
                            payway = new()
                            {
                                paytype = payTypeInfo.paytyp,
                                payname = payTypeInfo.payname
                            },
                            currency = item.currency,
                            //amount = Math.Round(Convert.ToDecimal(item.amount),2),
                            amount = Math.Truncate(Convert.ToDecimal(item.amount) * 100) / 100,
                            remarks = (await GenerateSummary(summaryInfoDto)).data
                        };
                        results.data.Add(applyInfoDto);
                    }
                }
            }
            results.total = results.data.Count;
            return results;
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
            list.SetCashX(list, user);
            list.AddCashXDetail(detailData);
            list.SetDeptId(hData.deptid);
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
            await _CashFileRepository.DeleteManyAsync(await _CashFileRepository.GetByNo(list.rno));
            await _CashAmountRepository.DeleteAsync(await _CashAmountRepository.GetByNo(list.rno));
        }
        async Task SaveFile(IFormCollection formCollection, CashHead list)
        {
            string area = await _minioDomainService.GetMinioArea(list.cuser);
            foreach (var file in formCollection.Files)
            {
                string path = "";
                string type = "";
                if (!string.IsNullOrEmpty(_Configuration.GetSection("UnitTest")?.Value))
                    type = "image/jpeg";
                else
                    type = file.ContentType;
                using (var steam = file.OpenReadStream())
                    path = await _MinioDomainService.PutObjectAsync(list.rno, file.FileName, steam, type,area);
                list.SetOriginalFileNameAndSavePath(file.Name, path, file.FileName, type);
            }
        }
        async Task InsertData(CashHead list)
        {
            await _CashHeadRepository.InsertAsync(list);
            await _CashDetailRepository.InsertManyAsync(list.CashDetailList);
            await _CashFileRepository.InsertManyAsync(list.CashFileList);
            await _CashAmountRepository.InsertAsync(list.CashAmount);
        }
        public bool IsDate(string strDate)
        {
            try
            {
                DateTime.Parse(strDate);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool IsYearMonth(string strDate)
        {
            try
            {
                DateTime.ParseExact(strDate, "yyyyMM", CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool IsYear(string strDate)
        {
            try
            {
                DateTime.ParseExact(strDate, "yyyy", CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsNumber(string str)
        {
            return Regex.IsMatch(str, @"^[0-9]+(\.[0-9]+)?$");
        }
        public static bool IsFutureOrCurrentDate(string dateStr)
        {
            DateTime date;
            if (!DateTime.TryParse(dateStr, out date))
            {
                return false;
            }
            DateTime currentDate = DateTime.Today;
            if (date >= currentDate || date.Year > currentDate.Year)
            {
                return true;
            }
            return false;
        }
    }
}
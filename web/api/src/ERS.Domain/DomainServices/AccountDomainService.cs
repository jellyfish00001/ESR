using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Account;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Volo.Abp.ObjectMapping;
using System.IO;
using ERS.DTO.Application;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using Volo.Abp.Domain.Repositories;
using ERS.Domain.IDomainServices;
namespace ERS.DomainServices
{
    /// <summary>
    /// PY001
    /// </summary>
    public class AccountDomainService : CommonDomainService, IAccountDomainService
    {
        private ICashAccountRepository _CashAccountRepository;
        private IAutoNoRepository _AutoNoRepository;
        private ICashCarrydetailRepository _CashCarrydetailRepository;
        private ICashCarryheadRepository _CashCarryheadRepository;
        private IComBankRepository _ComBankRepository;
        private IAppConfigRepository _AppConfigRepository;
        private ICashHeadRepository _CashHeadRepository;
        private IEmployeeRepository _EmloyeeRepository;
        private ICashPaymentDetailRepository _CashPaylistRepository;
        private ICashDetailRepository _CashDetailRepository;
        private ICompanyRepository _CompanyRepository;
        private IComtaxcodeRepository _ComtaxcodeRepository;
        private ICashDetailPstRepository _CashDetailPstRepository;
        private IEFormHeadRepository _EFormHeadRepository;
        private ICashAmountRepository _CashAmountRepository;
        private IBDFormRepository _BDFormRepository;
        private IEFormAlistRepository _EFormAlistRepository;
        private IBDExpRepository _BDExpRepository;
        private IEmpOrgRepository _EmpOrgRepository;
        private IInvoiceRepository _InvoiceRepository;
        private IBDTaxRateRepository _BDTaxRateRepository;
        private IBDExpIDRepository _BDExpIDRepository;
        private IObjectMapper _objectMapper;
        private ICurrencyDomainService _CurrencyDomainService;
        public AccountDomainService(
            ICashAccountRepository CashAccountRepository,
            IAutoNoRepository AutoNoRepository,
            ICashCarrydetailRepository CashCarrydetailRepository,
            ICashCarryheadRepository CashCarryheadRepository,
            IComBankRepository ComBankRepository,
            IAppConfigRepository AppConfigRepository,
            ICashHeadRepository CashHeadRepository,
            IEmployeeRepository EmployeeRepository,
            ICashPaymentDetailRepository CashPaylistRepository,
            ICashDetailRepository CashDetailRepository,
            ICompanyRepository CompanyRepository,
            IComtaxcodeRepository ComtaxcodeRepository,
            ICashDetailPstRepository CashDetailPstRepository,
            IEFormHeadRepository EFormHeadRepository,
            ICashAmountRepository CashAmountRepository,
            IBDFormRepository BDFormRepository,
            IEFormAlistRepository EFormAlistRepository,
            IBDExpRepository BDExpRepository,
            IEmpOrgRepository EmpOrgRepository,
            IInvoiceRepository InvoiceRepository,
            IBDTaxRateRepository BDTaxRateRepository,
            IBDExpIDRepository BDExpIDRepository,
            ICurrencyDomainService CurrencyDomainService,
            IObjectMapper ObjectMapper)
        {
            _CashAccountRepository = CashAccountRepository;
            _AutoNoRepository = AutoNoRepository;
            _CashCarrydetailRepository = CashCarrydetailRepository;
            _CashCarryheadRepository = CashCarryheadRepository;
            _ComBankRepository = ComBankRepository;
            _AppConfigRepository = AppConfigRepository;
            _CashHeadRepository = CashHeadRepository;
            _EmloyeeRepository = EmployeeRepository;
            _CashPaylistRepository = CashPaylistRepository;
            _CashDetailRepository = CashDetailRepository;
            _CompanyRepository = CompanyRepository;
            _ComtaxcodeRepository = ComtaxcodeRepository;
            _CashDetailPstRepository = CashDetailPstRepository;
            _EFormHeadRepository = EFormHeadRepository;
            _CashAmountRepository = CashAmountRepository;
            _BDFormRepository = BDFormRepository;
            _EFormAlistRepository = EFormAlistRepository;
            _BDExpRepository = BDExpRepository;
            _EmpOrgRepository = EmpOrgRepository;
            _InvoiceRepository = InvoiceRepository;
            _BDTaxRateRepository = BDTaxRateRepository;
            _BDExpIDRepository = BDExpIDRepository;
            _objectMapper = ObjectMapper;
            _CurrencyDomainService = CurrencyDomainService;
        }
        public async Task<Result<List<string>>> QueryBanks(string company)
        {
            Result<List<string>> result = new();
            var combankQuery = (await _ComBankRepository.WithDetailsAsync()).Where(w => w.company == company && !String.IsNullOrEmpty(w.banid)).Select(s => s.banid).ToList();
            var cashaccountQuery = (await _CashAccountRepository.WithDetailsAsync()).Where(s => !String.IsNullOrEmpty(s.bank)).Select(w => w.bank).Distinct().ToList();
            var query = combankQuery.Intersect(cashaccountQuery, StringComparer.OrdinalIgnoreCase);
            result.data = query.ToList();
            return result;
        }
        /// <summary>
        /// 查询获取单据信息api（分页）入账清单
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<Result<List<QueryPostingDto>>> QueryPageAccountReceipts(Request<QueryFormParamDto> parameters)
        {
            Result<List<QueryPostingDto>> result = new Result<List<QueryPostingDto>>()
            {
                data = new List<QueryPostingDto>()
            };
            //以入账信息cashdetail_pst为主表查出单号
            var cashCarryDetailPstList = (await _CashDetailPstRepository.WithDetailsAsync()).Select(w => w.rno).ToList();
            var cashCarryDetailQuery = (await _CashCarrydetailRepository.WithDetailsAsync()).Select(w => w.rno).ToList();
            //从eformhead查出status为A
            var eFormHeadQuery = (await _EFormHeadRepository.WithDetailsAsync())
            .Where(w => (w.status == "A" || w.formcode == "CASH_X") && (!cashCarryDetailQuery.Any(m => m == w.rno)) && cashCarryDetailPstList.Contains(w.rno))
            .Select(w => new { w.rno, w.cemplid, w.formcode, w.apid })
            .ToList();
            var cashHeadQuery = (await _CashHeadRepository.WithDetailsAsync()).Where(w => eFormHeadQuery.Select(g => g.rno).Contains(w.rno)).Select(w => new { w.rno, w.cuser, w.cname, w.cdate, w.company, w.bank }).ToList();
            var cashAmountQuery = (await _CashAmountRepository.WithDetailsAsync()).Where(w => eFormHeadQuery.Select(g => g.rno).Contains(w.rno)).Select(w => new { w.rno, w.actamt, w.currency }).ToList();
            var cashDetailQuery = (await _CashDetailRepository.WithDetailsAsync()).Where(w => eFormHeadQuery.Select(g => g.rno).Contains(w.rno)).Select(w => new { w.bank, w.rno }).ToList();
            var eFormQuery = (await _BDFormRepository.WithDetailsAsync()).ToList();
            var eFormAlistQuery = (await _EFormAlistRepository.WithDetailsAsync()).Where(w => eFormHeadQuery.Select(g => g.rno).Contains(w.rno) && (w.step == 10 || w.step == 11 || w.step == 12 || w.step == 51)
            && w.stepname.Contains('1')).OrderBy(o => o.step).Select(w => new { w.rno, w.cemplid }).ToList();
            var resultQuery = (from e in eFormHeadQuery
                               join h in cashHeadQuery
                               on e.rno equals h.rno
                               join a in cashAmountQuery
                               on e.rno equals a.rno
                               select new QueryPostingDto
                               {
                                   rno = e.rno,
                                   cuser = h.cuser,
                                   cname = h.cname,
                                   cdate = h.cdate,
                                   actamt = a.actamt,
                                   currency = a.currency,
                                   company = h.company,
                                   cemplid = eFormAlistQuery.Count > 0 ? eFormAlistQuery.Where(w => w.rno == e.rno).Select(w => w.cemplid).Distinct().FirstOrDefault() : "",//若此单不需签核会计 则签核会计为空
                                   bank = e.formcode == "CASH_4" ? cashDetailQuery.Where(w => w.rno == e.rno).OrderBy(g => g.bank).Select(g => g.bank).Distinct().FirstOrDefault() : h.bank,
                                   formname = eFormQuery.Where(w => w.FormCode == e.formcode).Select(w => w.FormName).FirstOrDefault(),
                                   apid = e.apid
                               }).ToList();
            result.data = resultQuery
                    .WhereIf(!parameters.data.companyList.IsNullOrEmpty(), w => parameters.data.companyList.Contains(w.company))
                    .WhereIf(!string.IsNullOrEmpty(parameters.data.cemplid), w => w.cemplid == parameters.data.cemplid)
                    .WhereIf(!string.IsNullOrEmpty(parameters.data.bank), w => w.bank == parameters.data.bank)
                    .ToList();
            int count = result.data.Count;
            int pageIndex;
            int pageSize;
            if (parameters.pageIndex <= 0 || parameters.pageSize <= 0 || parameters.pageIndex.ToString().IsNullOrEmpty() || parameters.pageSize.ToString().IsNullOrEmpty())
            {
                pageIndex = 1;
                pageSize = 10;
            }
            else
            {
                pageIndex = parameters.pageIndex;
                pageSize = parameters.pageSize;
            }
            result.data = result.data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = count;
            return result;
        }
        /// <summary>
        /// 生成入账清单
        /// </summary>
        /// <param name="queryFormDtos">选中的单据</param>
        /// <param name="parameters">查询的参数</param>
        /// <returns></returns>
        public async Task<Result<string>> GenerateAccountList(QueryFormDto queryFormDto, string userId)
        {
            Result<string> result = new();
            var xzFormCount = queryFormDto.rno.Count(w => w.StartsWith("XZ")); //薪資請款
            var existRno = (await _CashCarrydetailRepository.WithDetailsAsync()).FirstOrDefault(w => w.rnostatus == "N" && queryFormDto.rno.Contains(w.rno));
            if (xzFormCount != queryFormDto.rno.Count && xzFormCount != 0)
            {
                result.data = null;
                result.status = 2;
                result.message = L["AccountCheck"];
                return result;
            }
            if (existRno != null)
            {
                result.data = null;
                result.status = 2;
                result.message = string.Format(L["ExistsAccountList"], existRno.rno);
                return result;
            }
            if (queryFormDto.rno.All(w => w.StartsWith("XZ")))
            {
                return await GenerateCashXAccountList(queryFormDto, userId);
            }
            int count = 0;
            string sAutoNo = await _AutoNoRepository.CreateAccountNo();
            List<CashCarryhead> cashCarryheads = new();
            List<CashCarrydetail> cashCarrydetails = new();
            CashCarryhead cashCarryhead = new()
            {
                carryno = sAutoNo,
                postdate = DateTime.Now,
                acctant = userId,//入账会计
                company = queryFormDto.company,//选中的公司别
                companycode = queryFormDto.company,
                bank = queryFormDto.bank,
                cuser = userId,
                cdate = DateTime.Now,
                stat = "Y"
            };
            cashCarryheads.Add(cashCarryhead);
            var cashHeadQuery = await (await _CashHeadRepository.WithDetailsAsync()).Where(w => queryFormDto.rno.Contains(w.rno)).AsNoTracking().ToListAsync();
            var cashDetailQuery = await (await _CashDetailRepository.WithDetailsAsync()).Where(w => queryFormDto.rno.Contains(w.rno)).AsNoTracking().ToListAsync();
            var invoiceQuery = await (await _InvoiceRepository.WithDetailsAsync()).Where(w => queryFormDto.rno.Contains(w.rno)).AsNoTracking().ToListAsync();
            var bdexpQuery = await (await _BDExpRepository.WithDetailsAsync()).Where(w => cashHeadQuery.Select(x => x.company).Contains(w.company)).AsNoTracking().ToListAsync();
            var cashdetailpstQuery = await (await _CashDetailPstRepository.WithDetailsAsync()).Where(w => queryFormDto.rno.Contains(w.rno)).AsNoTracking().ToListAsync();
            var companyQuery = await (await _CompanyRepository.WithDetailsAsync()).AsNoTracking().ToListAsync();
            var pmcsCodeCheck = await _AppConfigRepository.GetIsPMCSCodeCHK("PY001_PrCode", "Y");
            decimal selfRate = 0.25M;//个人承担税率
            //记录上一条入账信息的单号
            string preRno = String.Empty;
            int itemFlag = 0;
            foreach (var qf in queryFormDto.rno)
            {
                var chquery = cashHeadQuery.FirstOrDefault(w => w.rno == qf);
                var cdquery = cashDetailQuery.Where(w => w.rno == qf).DistinctBy(g => g.bank).ToList();
                int category = 1;
                if (chquery.formcode == "CASH_3")
                {
                    category = 2;
                }
                string accountant = queryFormDto.cemplid; // accountant 签核会计
                var postingdetail = (from t in cashdetailpstQuery.Where(w => w.rno == qf)
                                     select new PostingdetailDto
                                     {
                                         ddate = t.ddate,
                                         basecurr = t.basecurr,
                                         postkey = t.postkey,
                                         acctcode = t.acctcode,
                                         baseamt = t.baseamt,
                                         txtcode = t.txtcode,
                                         costcenter = t.costcenter,
                                         lintext = t.lintext,
                                         asinmnt = t.asinmnt,
                                         payeeid = t.payeeid,
                                         rno = t.rno,
                                         formcode = t.formcode,
                                         pstitem = t.pstitem,
                                         companysap = companyQuery.FirstOrDefault(w => w.company == t.company)?.CompanySap,
                                         company = chquery.company,
                                         expcode = t.expcode,
                                         lctaxbase = t.lctaxbase,
                                         taxbase = t.taxbase,
                                         detailitem = t.detailitem
                                     })
                                     .OrderBy(w => w.detailitem)
                                     .ThenBy(w => w.postkey)
                                     .ThenBy(s => s.ddate)
                                     .ToList();
                decimal amount1 = 0;
                decimal amount2 = 0;
                //不为税金抵扣的入账信息
                var accountInfoList = postingdetail.Where(w => w.acctcode != "15033100").ToList();
                foreach (var item1 in accountInfoList)
                {
                    if (preRno != item1.rno)
                    {
                        count += 1;
                    }
                    if (item1.formcode == "CASH_4" && item1.detailitem != itemFlag && preRno == item1.rno)
                    {
                        count += 1;
                    }
                    preRno = item1.rno;
                    itemFlag = item1.detailitem;
                    string bdexporder = bdexpQuery.Where(w => w.expcode == item1.expcode && w.category == category && w.company == cashHeadQuery.FirstOrDefault().company).FirstOrDefault()?.pjcode;
                    string bankid = chquery.formcode == "CASH_4" ? cdquery.FirstOrDefault().bank : chquery.bank;
                    string vendercode = await _ComBankRepository.GetVenderCode(chquery.company, bankid);
                    CashCarrydetail cashCarrydetail = new CashCarrydetail();
                    decimal? baseAmount = 0;//报销金额（本位币）
                    //40表示向公司报销的, 31表示公司给员工的,50表示预支金
                    if (item1.formcode == "CASH_1")
                    {
                        //Cash1特殊处理取baseamt
                        var deptid = cashDetailQuery.FirstOrDefault(w => w.seq == item1.detailitem && w.rno == qf)?.deptid;
                        int index1 = deptid.IndexOf("[");
                        int arr = deptid.IndexOf("]") + 1 - deptid.IndexOf("[");
                        var res = JsonConvert.DeserializeObject<List<departCost>>(deptid.Substring(index1, arr));
                        baseAmount = res.Count > 0 ? res.Sum(w => w.baseamount) : 0;
                    }
                    else
                    {
                        baseAmount = Convert.ToDecimal(cashDetailQuery.FirstOrDefault(w => w.seq == item1.detailitem && w.rno == qf)?.baseamt);
                    }
                    if (item1.postkey == "40")
                    {
                        amount1 += Convert.ToDecimal(baseAmount);
                    }
                    if (item1.postkey == "50")
                    {
                        //若cash1的单据选择了预支金报销，baseamt取入账信息当中的baseamt
                        amount2 += item1.baseamt;
                    }
                    int orer = 0;
                    if (pmcsCodeCheck && item1.postkey != "31" && item1.acctcode != "Z800003")
                    {
                        cashCarrydetail.order = String.IsNullOrEmpty(bdexporder) ? cashHeadQuery.Where(w => w.rno == item1.rno).FirstOrDefault()?.projectcode : bdexporder;
                        orer = +1;
                    }
                    if (orer == 0)
                    {
                        //優先抓會計維護的project code，再抓取報銷單中申請人填寫的Project code
                        cashCarrydetail.order = string.IsNullOrEmpty(bdexporder) ? cashHeadQuery.FirstOrDefault(w => w.rno == item1.rno)?.projectcode : bdexporder;
                    }
                    cashCarrydetail.carryno = sAutoNo;
                    cashCarrydetail.seq = count;
                    cashCarrydetail.docdate = DateTime.Now;//改成跟posting date一致
                    cashCarrydetail.postdate = DateTime.Now;
                    cashCarrydetail.companysap = item1.companysap;
                    cashCarrydetail.basecurr = item1.basecurr;
                    cashCarrydetail.rate = 1;
                    cashCarrydetail.company = queryFormDto.company;
                    if (item1.formcode == "CASH_4")
                    {
                        cashCarrydetail.@ref = item1.rno + "-" + item1.detailitem.ToString();
                    }
                    else
                    {
                        cashCarrydetail.@ref = item1.rno;
                    }
                    cashCarrydetail.doctyp = "KR";
                    cashCarrydetail.postkey = item1.postkey;
                    cashCarrydetail.acctcode = item1.acctcode;
                    cashCarrydetail.specgl = string.Empty;
                    //有预支金金额且报销金额小于预支金金额
                    if (amount2 > 0 && (amount1 - amount2 < 0))
                    {
                        cashCarrydetail.actamt1 = amount1;
                        cashCarrydetail.actamt2 = amount1;
                    }
                    else
                    {
                        //如果使用预支金进行报销 postkey为40的金额：填写报销金额 - 个人承担税金 - 费用情景可抵扣税金
                        if (accountInfoList.Any(w => w.postkey == "50"))
                        {
                            //如果选了预支金进行冲账 入账信息里postkey为50的金额 = 冲账金额
                            if (item1.postkey == "50")
                            {
                                cashCarrydetail.actamt1 = Convert.ToDecimal(item1.baseamt);
                                cashCarrydetail.actamt2 = Convert.ToDecimal(item1.baseamt);
                            }
                            else
                            {
                                cashCarrydetail.actamt1 = Convert.ToDecimal(baseAmount) - invoiceQuery.Where(w => w.rno == item1.rno && w.seq == item1.detailitem && w.undertaker == "self").Sum(s => s.taxloss) - (item1.lctaxbase.HasValue ? Convert.ToDecimal(item1.lctaxbase) : 0);
                                cashCarrydetail.actamt2 = Convert.ToDecimal(baseAmount) - invoiceQuery.Where(w => w.rno == item1.rno && w.seq == item1.detailitem && w.undertaker == "self").Sum(s => s.taxloss) - (item1.lctaxbase.HasValue ? Convert.ToDecimal(item1.lctaxbase) : 0);
                            }
                        }
                        else
                        {
                            cashCarrydetail.actamt1 = Convert.ToDecimal(item1.baseamt) - (item1.lctaxbase.HasValue ? Convert.ToDecimal(item1.lctaxbase) : 0);
                            cashCarrydetail.actamt2 = Convert.ToDecimal(item1.baseamt) - (item1.lctaxbase.HasValue ? Convert.ToDecimal(item1.lctaxbase) : 0);
                        }
                        //CASH4特殊处理
                        if (item1.formcode == "CASH_4" && invoiceQuery.Where(w => w.rno == item1.rno && w.underwriter == item1.payeeid && w.undertaker == "self").Sum(s => s.taxloss) > 0)
                        {
                            cashCarrydetail.actamt1 = item1.baseamt - item1.baseamt * selfRate - (item1.lctaxbase.HasValue ? Convert.ToDecimal(item1.lctaxbase) : 0);
                            cashCarrydetail.actamt2 = item1.baseamt - item1.baseamt * selfRate - (item1.lctaxbase.HasValue ? Convert.ToDecimal(item1.lctaxbase) : 0);
                        }
                    }
                    cashCarrydetail.paytyp = string.Empty;
                    cashCarrydetail.baslindate = DateTime.Now;
                    cashCarrydetail.txtcode = item1.txtcode;
                    cashCarrydetail.costcenter = item1.acctcode.StartsWith("7") ? item1.costcenter : String.Empty;//GL account為7開頭的科目需帶costcenter
                    cashCarrydetail.linetext = item1.lintext;
                    cashCarrydetail.asinmnt = item1.asinmnt;//assignment：預支金統一：申請人部門/工號，報銷優先抓會計維護的assignment，若白單備註欄位有信息則抓取該信息
                    cashCarrydetail.ref1 = item1.payeeid;
                    cashCarrydetail.ref3 = string.Empty;
                    cashCarrydetail.formcode = item1.formcode;
                    cashCarrydetail.rno = item1.rno;
                    cashCarrydetail.pstitem = item1.pstitem;
                    cashCarrydetail.cuser = userId;
                    cashCarrydetail.cdate = DateTime.Now;
                    cashCarrydetail.acctant = accountant;
                    cashCarrydetail.bank = bankid;
                    cashCarrydetail.carritem = item1.detailitem;
                    cashCarrydetails.Add(cashCarrydetail);
                    if (amount1 - amount2 > 0)
                    {
                        if (item1.formcode != "CASH_4" && accountInfoList.IndexOf(item1) == accountInfoList.Count - 1)
                        {
                            var selectpost = postingdetail.Where(w => w.postkey == "40" && w.rno == item1.rno && w.acctcode != "15033100").OrderBy(w => w.pstitem).ToList();
                            CashCarrydetail cashCarrydetail1 = new()
                            {
                                company = queryFormDto.company,
                                carryno = sAutoNo,
                                seq = count,
                                docdate = System.DateTime.Now,//改成跟posting date一致
                                postdate = System.DateTime.Now,
                                companysap = selectpost.FirstOrDefault().companysap,
                                basecurr = selectpost.FirstOrDefault().basecurr,
                                rate = 1,
                                @ref = selectpost.FirstOrDefault().rno,
                                doctyp = "KR",
                                postkey = "31",
                                acctcode = !string.IsNullOrEmpty(vendercode) ? vendercode : "Z800003",
                                specgl = string.Empty,
                                actamt1 = accountInfoList.Sum(w => w.baseamt) - amount2,
                                actamt2 = accountInfoList.Sum(w => w.baseamt) - amount2,
                                payterm = "YTTP",
                                paytyp = "T",
                                baslindate = System.DateTime.Now,
                                txtcode = string.Empty,
                                costcenter = string.Empty,//
                                linetext = selectpost.FirstOrDefault().lintext,
                                asinmnt = string.Empty,
                                ref1 = item1.payeeid,
                                ref3 = string.Empty,
                                formcode = selectpost.FirstOrDefault().formcode,
                                rno = selectpost.FirstOrDefault().rno,
                                pstitem = selectpost.FirstOrDefault().pstitem,
                                cuser = userId,
                                cdate = System.DateTime.Now,
                                acctant = accountant,
                                bank = bankid
                            };
                            if (pmcsCodeCheck && item1.postkey != "31" && item1.acctcode != "Z800003")
                            {
                                cashCarrydetail1.order = String.IsNullOrEmpty(bdexporder) ? cashHeadQuery.Where(w => w.rno == item1.rno).FirstOrDefault()?.projectcode : bdexporder;
                                orer = +1;
                            }
                            if (orer == 0)
                            {
                                cashCarrydetail1.order = "";
                            }
                            cashCarrydetails.Add(cashCarrydetail1);
                        }
                        else if (item1.formcode == "CASH_4" && amount2 == 0)
                        {
                            CashCarrydetail cashCarrydetail2 = new()
                            {
                                company = queryFormDto.company,
                                carryno = sAutoNo,
                                seq = count,
                                docdate = System.DateTime.Now,
                                postdate = System.DateTime.Now,
                                companysap = item1.companysap,
                                basecurr = item1.basecurr,
                                rate = 1,
                                @ref = item1.rno + "-" + item1.detailitem.ToString(),
                                doctyp = "KR",
                                postkey = "31",
                                acctcode = vendercode,
                                specgl = string.Empty
                            };
                            //如果有个人承担税金，特殊处理
                            if ((invoiceQuery.Where(w => w.rno == item1.rno && w.underwriter == item1.payeeid && w.undertaker == "self").Sum(s => s.taxloss)) > 0)
                            {
                                //金额 = 实际支付金额amount（cash4的amount未扣除个人承担税额） - 个人承担税额（暂以25%计算） - 可抵扣税额（入账信息生成时计算放入lctaxbase当中）
                                cashCarrydetail2.actamt1 = item1.baseamt - item1.baseamt * selfRate - (item1.lctaxbase.HasValue ? Convert.ToDecimal(item1.lctaxbase) : 0);
                                cashCarrydetail2.actamt2 = item1.baseamt - item1.baseamt * selfRate - (item1.lctaxbase.HasValue ? Convert.ToDecimal(item1.lctaxbase) : 0);
                            }
                            else
                            {
                                cashCarrydetail2.actamt1 = item1.baseamt - (item1.lctaxbase.HasValue ? Convert.ToDecimal(item1.lctaxbase) : 0);
                                cashCarrydetail2.actamt2 = item1.baseamt - (item1.lctaxbase.HasValue ? Convert.ToDecimal(item1.lctaxbase) : 0);
                            }
                            cashCarrydetail2.payterm = "YTTP";
                            cashCarrydetail2.paytyp = "T";
                            cashCarrydetail2.baslindate = System.DateTime.Now;
                            cashCarrydetail2.txtcode = string.Empty;
                            cashCarrydetail2.costcenter = string.Empty;
                            cashCarrydetail2.linetext = item1.lintext;
                            cashCarrydetail2.asinmnt = string.Empty;
                            cashCarrydetail2.ref1 = item1.payeeid;
                            cashCarrydetail2.ref3 = string.Empty;
                            cashCarrydetail2.formcode = item1.formcode;
                            cashCarrydetail2.rno = item1.rno;
                            cashCarrydetail2.pstitem = item1.pstitem;
                            cashCarrydetail2.cuser = userId;
                            cashCarrydetail2.cdate = System.DateTime.Now;
                            cashCarrydetail2.acctant = accountant;
                            cashCarrydetail2.bank = bankid;
                            int orer2 = 0;
                            if (pmcsCodeCheck && item1.postkey != "31" && item1.acctcode != "Z800003")
                            {
                                cashCarrydetail2.order = String.IsNullOrEmpty(bdexporder) ? cashHeadQuery.Where(w => w.rno == item1.rno).FirstOrDefault()?.projectcode : bdexporder;
                                orer2 = +1;
                            }
                            if (orer2 == 0)
                            {
                                cashCarrydetail2.order = "";
                            }
                            cashCarrydetails.Add(cashCarrydetail2);
                        }
                    }
                }
                var invTaxPostingDetail = postingdetail.Where(w => w.acctcode == "15033100").FirstOrDefault();
                if (invTaxPostingDetail != null)
                {
                    CashCarrydetail invTaxDetail = new()
                    {
                        carryno = sAutoNo,
                        seq = cashCarrydetails.FirstOrDefault(w => w.rno == invTaxPostingDetail.rno).seq,
                        docdate = DateTime.Now,
                        postdate = DateTime.Now,
                        companysap = invTaxPostingDetail.companysap,
                        basecurr = invTaxPostingDetail.basecurr,
                        doctyp = "KR",
                        postkey = invTaxPostingDetail.postkey,
                        acctcode = invTaxPostingDetail.acctcode,
                        baslindate = DateTime.Now,
                        linetext = invTaxPostingDetail.lintext,
                        ref1 = invTaxPostingDetail.payeeid,
                        rno = invTaxPostingDetail.rno,
                        pstitem = invTaxPostingDetail.pstitem,
                        company = invTaxPostingDetail.company,
                        @ref = invTaxPostingDetail.rno,
                        actamt1 = Convert.ToDecimal(invTaxPostingDetail.lctaxbase),
                        actamt2 = Convert.ToDecimal(invTaxPostingDetail.lctaxbase),
                        txtcode = invTaxPostingDetail.txtcode,
                        cuser = userId,
                        cdate = DateTime.Now,
                        paytyp = string.Empty,
                        taxamt1 = invTaxPostingDetail.taxbase,
                        carritem = invTaxPostingDetail.detailitem
                    };
                    cashCarrydetails.Add(invTaxDetail);
                }
            }
            if (count != 0)
            {
                result.data = sAutoNo;
                //根据是否有31的line进行拆分
                var da = cashCarrydetails.Where(w => w.postkey == "31" && w.acctcode != "15033100").Select(s => s.@ref).ToList();
                if (da.Count > 0)
                {
                    var dt = cashCarrydetails.Where(w => w.postkey != "31" && w.acctcode != "15033100").Select(s => s.@ref).Distinct().ToList();
                    dt = dt.Except(da).ToList();
                    int newCount = 0;
                    string newCarryNo = sAutoNo + "-" + "1";
                    for (int i = 0; i < dt.Count; i++)
                    {
                        if (newCount == 0)
                        {
                            CashCarryhead newCashCarryHead = new()
                            {
                                carryno = newCarryNo,
                                postdate = cashCarryhead.postdate,
                                acctant = cashCarryhead.acctant,
                                companycode = cashCarryhead.companycode,
                                bank = cashCarryhead.bank,
                                stat = cashCarryhead.stat,
                                cuser = cashCarryhead.cuser,
                                cdate = cashCarryhead.cdate,
                                muser = cashCarryhead.muser,
                                mdate = cashCarryhead.mdate,
                                company = queryFormDto.company
                            };
                            cashCarryheads.Add(newCashCarryHead);
                            newCount += 1;
                            // update 40,50 line 找到cashCarryDetail40 50 的lines
                            var updateLines = cashCarrydetails.Where(w => w.@ref == dt[i]).ToList();
                            foreach (var item in updateLines)
                            {
                                item.carryno = newCarryNo;
                                item.seq = i + 1;
                            }
                        }
                    }
                    if (newCount > 0 && da.Count > 0)
                    {
                        // update 31,40 和 31,40,50 line
                        for (int j = 0; j < da.Count; j++)
                        {
                            var updateLines1 = cashCarrydetails.Where(w => w.carryno == sAutoNo && w.@ref == da[j]).ToList();
                            foreach (var line in updateLines1)
                            {
                                line.seq = j + 1;
                            }
                        }
                        result.data = result.data + " " + newCarryNo;
                    }
                }
                var batchRefList = cashCarrydetails.Where(w => w.rno.Contains('B')).OrderBy(s => s.rno).ThenBy(g => g.seq).ToList();
                string strflag = string.Empty;
                if (batchRefList.Count > 0)
                {
                    string strrno = string.Empty;
                    int scount = 1;
                    for (int i = 0; i < batchRefList.Count; i++)
                    {
                        string seq = batchRefList[i].seq.ToString().Trim();
                        string rno = batchRefList[i].rno.ToString().Trim();
                        if (strflag == string.Empty && strrno == string.Empty)
                        {
                            strflag = seq;
                            strrno = rno;
                        }
                        else
                        {
                            if (strflag != seq && strrno == rno)
                            {
                                strflag = seq;
                                strrno = rno;
                                scount += 1;
                            }
                            if (strflag != seq && strrno != rno)
                            {
                                strflag = seq;
                                strrno = rno;
                                scount = 1;
                            }
                        }
                        rno += "-" + scount.ToString();
                        var updateList = batchRefList.Where(w => w.carryno == sAutoNo && w.seq.ToString() == seq).ToList();
                        for (int j = 0; j < updateList.Count; j++)
                        {
                            updateList[j].@ref = rno;
                        }
                    }
                }
                await _CashCarryheadRepository.InsertManyAsync(cashCarryheads);
                await _CashCarrydetailRepository.InsertManyAsync(cashCarrydetails);
            }
            else
            {
                result.data = null;
                result.status = 2;
            }
            return result;
        }
        /// <summary>
        /// 薪資請款生成入賬清單
        /// </summary>
        /// <param name="queryFormDto"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        async Task<Result<string>> GenerateCashXAccountList(QueryFormDto queryFormDto, string userId)
        {
            Result<string> result = new();
            string sAutoNo = await _AutoNoRepository.CreateAccountNo();
            List<CashCarryhead> addCarryHeadList = new();
            List<CashCarrydetail> addCarryDetailList = new();
            CashCarryhead cashCarryhead = new()
            {
                carryno = sAutoNo,
                postdate = System.DateTime.Now,
                acctant = userId,
                company = queryFormDto.company,
                companycode = queryFormDto.company,
                bank = queryFormDto.bank,
                cuser = userId,
                cdate = System.DateTime.Now,
                stat = "Y"
            };
            addCarryHeadList.Add(cashCarryhead);
            var cashdetailpstQuery = await (await _CashDetailPstRepository.WithDetailsAsync()).Where(w => queryFormDto.rno.Contains(w.rno)).AsNoTracking().ToListAsync();
            var existRnoList = await (await _CashCarrydetailRepository.WithDetailsAsync()).Where(w => w.rnostatus == "N").Select(s => s.rno).AsNoTracking().Distinct().ToListAsync();
            var comBaseData = await (await _CompanyRepository.WithDetailsAsync()).Select(w => new { w.company, w.CompanyCategory, w.CompanySap }).AsNoTracking().ToListAsync();
            var cashdetailQuery = await (await _CashDetailRepository.WithDetailsAsync()).Where(w => queryFormDto.rno.Contains(w.rno)).AsNoTracking().ToListAsync();
            var ExpIdBaseData = await (await _BDExpIDRepository.WithDetailsAsync()).Where(w => cashdetailpstQuery.Select(s => s.expcode).Contains(w.expcode)).ToListAsync();
            string preRno = string.Empty;
            int itemFlag = 0;
            int count = 0;
            for (int i = 0; i < queryFormDto.rno.Count; i++)
            {
                if (existRnoList.Contains(queryFormDto.rno[i]))
                {
                    result.data = null;
                    result.status = 2;
                    result.message = string.Format(L["ExistsAccountList"], queryFormDto.rno[i]);
                    return result;
                }
                var accountInfoList = (from t in cashdetailpstQuery.Where(w => w.rno == queryFormDto.rno[i])
                                       select new PostingdetailDto
                                       {
                                           ddate = t.ddate,
                                           basecurr = t.basecurr,
                                           postkey = t.postkey,
                                           acctcode = t.acctcode,
                                           baseamt = t.baseamt,
                                           txtcode = t.txtcode,
                                           costcenter = t.costcenter,
                                           lintext = t.lintext,
                                           asinmnt = t.asinmnt,
                                           payeeid = t.payeeid,
                                           rno = t.rno,
                                           formcode = t.formcode,
                                           pstitem = t.pstitem,
                                           companysap = comBaseData.Where(w => w.company == t.company).Distinct().FirstOrDefault().CompanySap,
                                           company = t.company,
                                           expcode = t.expcode,
                                           lctaxbase = t.lctaxbase,
                                           taxbase = t.taxbase,
                                           detailitem = t.detailitem
                                       })
                                     .OrderBy(w => w.detailitem)
                                     .ThenBy(w => w.postkey)
                                     .ThenBy(s => s.ddate)
                                     .ToList();

                for (int j = 0; j < accountInfoList.Count; j++)
                {
                    if (accountInfoList[j].detailitem != itemFlag && preRno == accountInfoList[j].rno)
                    {
                        count += 1;
                    }
                    if (preRno != accountInfoList[j].rno)
                    {
                        count += 1;
                    }
                    preRno = accountInfoList[j].rno;
                    itemFlag = accountInfoList[j].detailitem;
                    var tempDetail = cashdetailQuery.FirstOrDefault(w => w.seq == accountInfoList[j].detailitem && w.rno == accountInfoList[j].rno);
                    string identification = ExpIdBaseData.FirstOrDefault(w => w.acctcode == tempDetail.acctcode && w.bank == tempDetail.bank && w.expcode == tempDetail.expcode && w.companycode == tempDetail.companycode)?.identification;
                    //postkey 40
                    CashCarrydetail cashCarrydetail = new()
                    {
                        carryno = sAutoNo,
                        carritem = accountInfoList[j].detailitem,
                        seq = count,
                        docdate = System.DateTime.Now,
                        postdate = System.DateTime.Now,
                        companysap = accountInfoList[j].companysap,
                        basecurr = accountInfoList[j].basecurr,
                        rate = 1,
                        doctyp = "KR",
                        postkey = accountInfoList[j].postkey,
                        actamt1 = accountInfoList[j].baseamt,
                        actamt2 = accountInfoList[j].baseamt,
                        baslindate = System.DateTime.Now,
                        linetext = accountInfoList[j].lintext,
                        asinmnt = accountInfoList[j].asinmnt,
                        @ref = accountInfoList[j].rno + "-" + (j + 1),
                        acctcode = accountInfoList[j].acctcode,
                        ref1 = accountInfoList[j].payeeid,
                        rno = accountInfoList[j].rno,
                        company = accountInfoList[j].company,
                        cuser = userId,
                        cdate = System.DateTime.Now
                    };
                    addCarryDetailList.Add(cashCarrydetail);
                    //postkey 31
                    CashCarrydetail cashCarrydetail2 = (CashCarrydetail)cashCarrydetail.Clone();
                    cashCarrydetail2.postkey = "31";
                    cashCarrydetail2.acctcode = "Z800001";
                    cashCarrydetail2.payterm = "YTTP";
                    cashCarrydetail2.paytyp = "T";
                    var postKeyConfig = await _AppConfigRepository.GetPostKeyBySite(accountInfoList[0].company);
                    if (postKeyConfig != null)
                    {
                        var postKeyConfigList = postKeyConfig.ToString().Split(",");
                        cashCarrydetail2.postkey = postKeyConfigList[0];
                        cashCarrydetail2.acctcode = postKeyConfigList[1];
                        if (postKeyConfigList.Length > 2)
                        {
                            cashCarrydetail2.specgl = postKeyConfigList[2];
                        }
                    }
                    cashCarrydetail2.linetext = accountInfoList[j].lintext + "-" + identification;
                    addCarryDetailList.Add(cashCarrydetail2);
                }
            }
            if (addCarryHeadList.Count > 0 && addCarryDetailList.Count > 0)
            {
                result.data = sAutoNo;
                await _CashCarryheadRepository.InsertManyAsync(addCarryHeadList);
                await _CashCarrydetailRepository.InsertManyAsync(addCarryDetailList);
            }
            else
            {
                result.status = 2;
                result.data = null;
            }
            return result;
        }
        public static string f_getrno(string strRno)
        {
            string result = "";
            if (strRno.Contains('-'))
            {
                result = strRno.Substring(0, strRno.IndexOf("-"));
            }
            else
            {
                result = strRno;
            }
            return result;
        }
        /// <summary>
        /// 入账清单维护（查询）
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<Result<List<GenerateFormDto>>> QueryPageAccountList(Request<GenerateFormParamDto> parameters)
        {
            Result<List<GenerateFormDto>> result = new Result<List<GenerateFormDto>>()
            {
                data = new List<GenerateFormDto>()
            };
            var ccdlist = (await _CashCarrydetailRepository.WithDetailsAsync()).Select(w => new { w.@ref, w.rno }).ToList();
            var notExistsRno = (await _CashPaylistRepository.WithDetailsAsync()).Where(w => ccdlist.Select(w => w.@ref).Contains(w.Rno)).Select(w => w.Rno).ToList();
            var existsRno = (await _CashHeadRepository.WithDetailsAsync())
            .Where(w => ccdlist.Select(w => f_getrno(w.@ref)).Contains(w.rno) && w.payment == null)
            .Select(w => w.rno)
            .ToList();
            //未group的查询
            var preQuery1 = (from t in await _CashCarryheadRepository.WithDetailsAsync()
                             join c in await _CashCarrydetailRepository.WithDetailsAsync()
                             on t.carryno equals c.carryno
                             select new
                             {
                                 @ref = c.@ref,
                                 carryno = t.carryno,
                                 postdate = t.postdate,
                                 acctant = t.acctant ?? c.cuser
                             }).ToList();
            var preQuery = (from c in preQuery1
                            where c.@ref != null && !notExistsRno.Contains(c.@ref) && existsRno.Contains(f_getrno(c.@ref))
                            select new
                            {
                                carryno = c.carryno,
                                postdate = c.postdate,
                                acctant = c.acctant
                            }).ToList();
            var bQuery = (from p in preQuery
                          group p by new
                          {
                              p.acctant,
                              p.carryno,
                              p.postdate
                          }
                         into g
                          select new
                          {
                              carryno = g.FirstOrDefault().carryno,
                              postdate = g.FirstOrDefault().postdate,
                              acctant = g.FirstOrDefault().acctant
                          }).ToList();
            var emps = (await _EmloyeeRepository.WithDetailsAsync()).Where(w => bQuery.Select(w => w.acctant).Contains(w.emplid)).Select(w => new { w.emplid, w.cname, w.ename }).ToList();
            var resultQuery = (from b in bQuery
                               select new GenerateFormDto
                               {
                                   carryno = b.carryno,
                                   postdate = b.postdate,
                                   acctantanme = b.acctant + "/" + emps.Where(w => w.emplid == b.acctant).Select(w => w.cname).FirstOrDefault() + "/" + emps.Where(w => w.emplid == b.acctant).Select(w => w.ename).FirstOrDefault(),
                                   acctant = b.acctant
                               })
                               .Distinct()
                               .OrderByDescending(w => w.postdate)
                               .ToList();
            //数据筛选
            result.data = resultQuery
                        .WhereIf(!string.IsNullOrEmpty(parameters.data.carryno), w => w.carryno == parameters.data.carryno)
                        .WhereIf(!string.IsNullOrEmpty(parameters.data.acctant), w => w.acctant == parameters.data.acctant)
                        .WhereIf(parameters.data.startpostdate.HasValue, w => w.postdate.Value.Date >= Convert.ToDateTime(parameters.data.startpostdate).ToLocalTime().Date)
                        .WhereIf(parameters.data.endpostdate.HasValue, w => w.postdate.Value.Date <= Convert.ToDateTime(parameters.data.endpostdate).ToLocalTime().Date)
                        .ToList();
            int count = result.data.Count;
            int pageIndex;
            int pageSize;
            if (parameters.pageIndex <= 0 || parameters.pageSize <= 0 || parameters.pageIndex.ToString().IsNullOrEmpty() || parameters.pageSize.ToString().IsNullOrEmpty())
            {
                pageIndex = 1;
                pageSize = 10;
            }
            else
            {
                pageIndex = parameters.pageIndex;
                pageSize = parameters.pageSize;
            }
            result.data = result.data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = count;
            return result;
        }
        /// <summary>
        /// 批量删除入账清单
        /// </summary>
        /// <param name="carryno"></param>
        /// <returns></returns>
        public async Task<Result<string>> BatchDeleteAccountlist(List<string> carryno)
        {
            Result<string> result = new Result<string>();
            foreach (var item in carryno)
            {
                List<CashCarryhead> cashCarryheads = await _CashCarryheadRepository.GetListByCarryNo(item);
                List<CashCarrydetail> cashCarrydetails = await _CashCarrydetailRepository.GetListByCarryNo(item);
                if (cashCarryheads.Count > 0 && cashCarrydetails.Count > 0)
                {
                    await _CashCarryheadRepository.DeleteManyAsync(cashCarryheads);
                    await _CashCarrydetailRepository.DeleteManyAsync(cashCarrydetails);
                    result.message += item + ", " + L["DeleteSuccess"];
                }
                else
                {
                    result.status = 2;
                    result.message = L["CarrynoNotFound"] + "：" + item;
                    return result;
                }
            }
            return result;
        }
        /// <summary>
        /// 下载入账清单
        /// </summary>
        /// <param name="carryno"></param>
        /// <returns></returns>
        public async Task<byte[]> DownloadAccountList(string carryno)
        {
            byte[] buffer = null;
            IList<CashCarrydetail> cashCarrydetails = (await _CashCarrydetailRepository.WithDetailsAsync())
                                                     .Where(w => w.carryno == carryno)
                                                     .OrderBy(w => w.seq)
                                                     .ThenBy(w => w.carritem)
                                                     .ThenBy(w => w.postkey)
                                                     .ThenBy(w => w.acctcode)
                                                     .AsNoTracking()
                                                     .ToList();
            string uCompany = await (await _AppConfigRepository.WithDetailsAsync()).Where(i => i.key == "Account_EXCH_TO_USD").Select(i => i.value).AsNoTracking().FirstOrDefaultAsync();
            string iCompany = await (await _AppConfigRepository.WithDetailsAsync()).Where(i => i.key == "Account_Curr_To_Integer").Select(i => i.value).AsNoTracking().FirstOrDefaultAsync();
            foreach (var item in cashCarrydetails)
            {
                if (item.acctcode != "Z800003" && (item.company == "WZS" || item.company == "WTZS"))
                {
                    item.ref1 = string.Empty;
                }
                if (item.postkey == "40" || item.postkey == "50")
                {
                    item.ref1 = string.Empty;
                }
                if (!string.IsNullOrEmpty(uCompany) && uCompany.Split(",").Contains(item.company))
                {
                    if (item.basecurr == "USD")
                    {
                        item.rate = 1;
                    }
                    else
                    {
                        item.rate = (await _CurrencyDomainService.queryRate(item.basecurr, "USD")).ccurrate / (await _CurrencyDomainService.queryRate(item.basecurr, "USD")).ratiofrom * (await _CurrencyDomainService.queryRate(item.basecurr, "USD")).ratioto;
                    }
                    item.actamt2 = Math.Round(item.actamt2 * item.rate, 2, MidpointRounding.AwayFromZero);
                }
                if (!string.IsNullOrEmpty(iCompany) && iCompany.Split(",").Contains(item.company))
                {
                    item.actamt1 = Math.Round(item.actamt1, 0, MidpointRounding.AwayFromZero);
                }
            }
            IList<CarryDetailReportDto> carryDetailReportDtos = _objectMapper.Map<IList<CashCarrydetail>, IList<CarryDetailReportDto>>(cashCarrydetails);
            IList<SACarryDetailReportDto> saCarryDetailReportDtos = _objectMapper.Map<IList<CashCarrydetail>, IList<SACarryDetailReportDto>>(cashCarrydetails);
            if (carryDetailReportDtos.Any(w => w.postkey == "31"))
            {
                //excel表头
                string APheaderColumns = @"Sequence No, Document date, Posting Date, Company Code, Currency key, Exchange Rate, Reference, Document Header Text, Document Type, Posting Key, AccountNumber (GL/ customer/ vendor), Special G/L Indicator, Amount In Document Currency (include tax amount), Amount In Local Currency (include tax amount), Payment Term, Payment Method, Base line date, Tax code, Tax Base Amount, LC Tax Base Amount, Withholding tax type, Withholding tax code, Withholding tax base amount, Withholding tax amount, Cost Center, Order, Line Text, Assignment number, Profit Center, Partner Profit Center, Customer Code (ie. Bill-to Party), Plant, Business Type, End customer, Material Division, Sales Division, Reference 1, Reference 2, Reference 3";
                int[] APnumbericColumns = { 5, 12, 13, 18, 19, 22, 23 };
                int[] CommaColumns = null;
                if (carryDetailReportDtos.All(w => w.@ref.StartsWith("XZ")))
                {
                    CommaColumns = new int[] { 12, 13 };
                }
                MemoryStream ms = NPOIHelper.RenderToExcel<CarryDetailReportDto>(carryDetailReportDtos, APheaderColumns, APnumbericColumns, null, null, CommaColumns);
                ms.Seek(0, SeekOrigin.Begin);
                buffer = ms.ToArray();
            }
            else
            {
                foreach (var item in saCarryDetailReportDtos)
                {
                    item.doctyp = "SA";
                }
                string SAheaderColumns = @"*Sequence No., Company Code, Document date, Posting Date, Document Type, Currency key, Exchange Rate Direct Quotation, Reference, Document Header Text, Posting Key, Account, Amount in document currency, Amount in document currency, Cost Center, Profit Center, Assignment number, Tax Code, ItemText, Order Number, Customer Group/Code, Plant, Business Type, Reference 1, Reference 2, Reference 3, Excel Line Item No., Trading Partner (at header level), Partner Profit Center";
                int[] SAnumbericColumns = { 6 };
                MemoryStream ms = NPOIHelper.RenderToExcel<SACarryDetailReportDto>(saCarryDetailReportDtos, SAheaderColumns, SAnumbericColumns, null, null);
                ms.Seek(0, SeekOrigin.Begin);
                buffer = ms.ToArray();
            }
            return buffer;
        }
        //獲取入賬清單Excel數據
        public async Task<Result<ExcelDto<CarryDetailDto>>> GetCarryDetailExcelData(string carryno)
        {
            Result<ExcelDto<CarryDetailDto>> result = new Result<ExcelDto<CarryDetailDto>>();
            ExcelDto<CarryDetailDto> excelData = new ExcelDto<CarryDetailDto>();
            List<CashCarrydetail> cashCarrydetails = (await _CashCarrydetailRepository.WithDetailsAsync())
                                                     .Where(w => w.carryno == carryno)
                                                     .OrderBy(w => w.seq)
                                                     .ThenBy(w => w.@ref)
                                                     .ThenBy(w => w.postkey)
                                                     .ToList();
            List<CarryDetailDto> carryDetailDtos = _objectMapper.Map<List<CashCarrydetail>, List<CarryDetailDto>>(cashCarrydetails);
            string[] header = new string[]
            {
                L["CarryDetail-SequenceNo"],
                L["CarryDetail-Documentdate"],
                L["CarryDetail-PostingDate"],
                L["CarryDetail-CompanyCode"],
                L["CarryDetail-Currencykey"],
                L["CarryDetail-ExchangeRate"],
                L["CarryDetail-Reference"],
                L["CarryDetail-DocHeaderTxt"],
                L["CarryDetail-DocumentType"],
                L["CarryDetail-PostingKey"],
                L["CarryDetail-AccountNumber"],
                L["CarryDetail-Special G/L Indicator"],
                L["CarryDetail-AmtInDocCurr"],
                L["CarryDetail-AmtInLocalCurr"],
                L["CarryDetail-PaymentTerm"],
                L["CarryDetail-PaymentMethod"],
                L["CarryDetail-BaseLineDate"],
                L["CarryDetail-Taxcode"],
                L["CarryDetail-TaxBaseAm"],
                L["CarryDetail-LCTaxBaseAmt"],
                L["CarryDetail-WithholdingTaxType"],
                L["CarryDetail-WithholdingTaxCode"],
                L["CarryDetail-WithholdingTaxBaseAmt"],
                L["CarryDetail-WithholdingTaxAmount"],
                L["CarryDetail-CostCenter"],
                L["CarryDetail-Order"],
                L["CarryDetail-LineText"],
                L["CarryDetail-AssignmentNumber"],
                L["CarryDetail-ProfitCenter"],
                L["CarryDetail-PartnerProfitCenter"],
                L["CarryDetail-CustomerCode"],
                L["CarryDetail-Plant"],
                L["CarryDetail-BusinessType"],
                L["CarryDetail-EndCustomer"],
                L["CarryDetail-MaterialDivision"],
                L["CarryDetail-SalesDivision"],
                L["CarryDetail-Reference1"],
                L["CarryDetail-Reference2"],
                L["CarryDetail-Reference3"]
            };
            excelData.header = header;
            excelData.body = carryDetailDtos;
            result.data = excelData;
            return result;
        }
        /// <summary>
        /// 入账信息生成
        /// </summary>
        /// <param name="rno"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<string>> SaveAccountInfo(string rno, string userId)
        {
            Result<string> result = new Result<string>();
            var checkpst = (await _CashDetailPstRepository.WithDetailsAsync()).Where(w => w.rno == rno).ToList();
            if (checkpst.Count > 0)
            {
                result.status = 2;
                result.message = L["AccountInfoSaveFail"] + "," + L["AccountInfoExist"];
                return result;
            }
            //1.查询信息
            var cashheads = await _CashHeadRepository.ReadCashHeadsByNo(rno);
            var cashdetails = await _CashDetailRepository.ReadCashDetailsByNo(rno);
            var invoicelist = await _InvoiceRepository.ReadDetailsByNo(rno);
            var advanceInfoList = (await _CashAmountRepository.WithDetailsAsync()).Where(w => cashdetails.Select(s => s.advancerno).Contains(w.rno)).ToList();
            var advanceDetails = (await _CashDetailRepository.WithDetailsAsync()).Where(w => cashdetails.Select(s => s.advancerno).Contains(w.rno)).ToList();
            var advanceHeads = (await _CashHeadRepository.WithDetailsAsync()).Where(w => cashdetails.Select(s => s.advancerno).Contains(w.rno)).ToList();
            if (cashdetails.Count <= 0 || cashheads == null)
            {
                result.message = L["RnoHaveNoData"];
                result.status = 2;
                return result;
            }
            string formcode = cashheads.formcode;
            string company = cashheads.company;
            string baseCurr = cashdetails.Select(w => w.basecurr).FirstOrDefault();
            var bdexpQuery = (await _BDExpRepository.WithDetailsAsync()).Where(w => w.company == company).AsNoTracking().ToList();
            string starwith = (await _CompanyRepository.GetStarwith(company));
            var comtaxcode = (await _ComtaxcodeRepository.GetComtaxcode(company));
            //申请人
            string cuser = cashheads.cuser;
            var cuserInfo = await (await _EmloyeeRepository.WithDetailsAsync()).Where(s => s.emplid == cuser).Select(w => new { w.deptid, w.company }).AsNoTracking().FirstOrDefaultAsync();
            if (cuserInfo == null)
                cuserInfo = await (await _EmloyeeRepository.WithDetailsAsync()).Where(s => s.emplid == cuser.ToUpper()).Select(w => new { w.deptid, w.company }).AsNoTracking().FirstOrDefaultAsync();
            //申請人所在部門的挂账部门
            string costDeptid = (await _EmpOrgRepository.GetCostDeptid(cuserInfo.deptid, cuserInfo.company));
            int category = 1;
            if (formcode == "CASH_3")
            {
                category = 2;
            }
            var bdexpListInfo = bdexpQuery.Where(w => cashdetails.Select(s => s.expcode).Contains(w.expcode) && w.category == category).ToList();
            List<AccountInfoDto> strposting = new List<AccountInfoDto>();
            if (formcode != "CASH_3")
            {
                //2023.3.21 cashdetail有多少条明细就拆分多少条入账信息，不再进行分组
                var tempStrposting = (from t in cashdetails
                                      select new
                                      {
                                          rno = t.rno,
                                          acctcode = t.acctcode,
                                          deptid = t.deptid,
                                          expcode = t.expcode,
                                          baseamt = t.amount,//t.amount(扣除个人承担税金的金额)
                                          payeeid = !String.IsNullOrEmpty(t.payeeid) ? t.payeeid : cashheads.payeeId,
                                          rdate = t.rdate,
                                          company = t.company,
                                          seq = t.seq,
                                          expname = t.expname,
                                          lctaxbase = 0,
                                          taxexpense = t.taxexpense//会计填的可抵扣税额
                                      }).ToList();
                foreach (var item in tempStrposting)
                {
                    decimal taxAmount = 0;
                    var bdexp = bdexpListInfo.Where(w => w.expcode == item.expcode && w.company == item.company).FirstOrDefault();
                    //取出detail的该条item当中的所有发票
                    var invoices = invoicelist.Where(w => w.seq == item.seq).ToList();
                    if (bdexp != null && bdexp.isdeduction == "Y" && !String.IsNullOrEmpty(bdexp.invoicecategory))
                    {
                        var invcategory = ChineseConverter.Convert(bdexp.invoicecategory, ChineseConversionDirection.TraditionalToSimplified)
                        .Split(";")
                        .Select(s => Regex.Replace(s, "[()]", ""))
                        .Select(s => Regex.Replace(s, "[（）]", ""))
                        .ToList();
                        foreach (var inv in invoices)
                        {
                            if (inv.invoiceid != null && !String.IsNullOrEmpty(inv.invtype) && invcategory.Contains(ChineseConverter.Convert(Regex.Replace(inv.invtype, "[()]", ""), ChineseConversionDirection.TraditionalToSimplified)))
                            {
                                taxAmount += item.taxexpense.HasValue ? Convert.ToDecimal(item.taxexpense) : inv.taxamount;
                            }
                        }
                    }
                    AccountInfoDto accountInfoDto = new();
                    accountInfoDto.deptid = item.deptid;
                    accountInfoDto.postkey = "40";
                    accountInfoDto.acctcode = item.acctcode;
                    accountInfoDto.baseamt = item.baseamt;
                    accountInfoDto.txtcode = "M0";
                    accountInfoDto.costcenter = item.deptid;
                    accountInfoDto.asinmnt = "";
                    accountInfoDto.payeeid = item.payeeid;
                    accountInfoDto.invoice = "";
                    accountInfoDto.taxbase = 0;//存发票的不含税金额
                    accountInfoDto.lctaxbase = taxAmount;
                    accountInfoDto.rdate = item.rdate;
                    accountInfoDto.unifycode = "";
                    accountInfoDto.certificate = "";
                    accountInfoDto.item = item.seq;//cashdetailpst的detailitem来自这里
                    accountInfoDto.ref1 = "";
                    accountInfoDto.expcode = item.expcode;
                    accountInfoDto.lintext = GetSummary(cashdetails, cashheads, rno, item.acctcode, item.payeeid, item.deptid, item.seq).Result;
                    strposting.Add(accountInfoDto);
                }
                if (strposting.Count > 0)
                {
                    for (int i = 0; i < strposting.Count; i++)
                    {
                        if (strposting[i].costcenter != string.Empty)
                        {
                            if (formcode == "CASH_1")
                            {
                                List<departCost> res = new List<departCost>();
                                if (!string.IsNullOrEmpty(strposting[i].deptid))
                                {
                                    int index1 = strposting[i].deptid.IndexOf("[");
                                    int arr = strposting[i].deptid.IndexOf("]") + 1 - strposting[i].deptid.IndexOf("[");
                                    res = JsonConvert.DeserializeObject<List<departCost>>(strposting[i].deptid.Substring(index1, arr));
                                }
                                for (int j = 0; j < res.Count; j++)
                                {
                                    //查出deptid当中的部门公司别
                                    string companycode = await _EmpOrgRepository.GetCompanyCodeByDeptid(res[j].deptId);
                                    string deptCompany = (await _CompanyRepository.WithDetailsAsync()).Where(w => w.CompanyCategory == companycode).AsNoTracking().FirstOrDefault()?.company;
                                    //detail當中的部門與head的公司別不一致的話，costcenter取申請人所在的掛賬部門（加縮寫）
                                    if (company != deptCompany)
                                    {
                                        res[j].deptId = starwith + costDeptid;
                                    }
                                    else
                                    {
                                        string deptStarwith = await _CompanyRepository.GetStarwith(deptCompany);
                                        res[j].deptId = deptStarwith + res[j].deptId;
                                    }
                                    //strposting[i].costcenter = deptStarwith + strposting[i].costcenter;
                                }
                                strposting[i].costcenter = JsonConvert.SerializeObject(res);
                            }
                            else
                            {
                                string companycode = await _EmpOrgRepository.GetCompanyCodeByDeptid(strposting[i].deptid);
                                string deptCompany = (await _CompanyRepository.WithDetailsAsync()).Where(w => w.CompanyCategory == companycode).AsNoTracking().FirstOrDefault()?.company;
                                string deptStarwith = await _CompanyRepository.GetStarwith(deptCompany);
                                //detail當中的部門與head的公司別不一致的話，costcenter取申請人所在的掛賬部門（加縮寫）
                                if (company != deptCompany)
                                {
                                    strposting[i].costcenter = starwith + strposting[i].costcenter;
                                }
                                else
                                {
                                    strposting[i].costcenter = deptStarwith + strposting[i].costcenter;
                                }
                            }
                        }
                        if (comtaxcode.Count > 0)
                        {
                            strposting[i].txtcode = comtaxcode[0].taxcode;
                        }
                    }
                }
            }
            else
            {
                var tempAccountInfo = (from t in cashdetails
                                       select new
                                       {
                                           rno = t.rno,
                                           postkey = "40",
                                           acctcode = t.acctcode,
                                           baseamt = t.baseamt,
                                           txtcode = "",
                                           costcenter = "",
                                           asinmnt = cashheads.deptid + cashheads.cname + cashheads.rno.Substring(3, 4),
                                           expname = t.expname,
                                           payeeid = t.payeeid == null ? cashheads.payeeId : t.payeeid,
                                           invoice = "",
                                           taxbase = "0",
                                           lctaxbase = "0",
                                           rdate = t.rdate,
                                           unifycode = "",
                                           certificate = "",
                                           item = t.seq,
                                           ref1 = "",
                                           expcode = t.expcode,
                                           deptid = t.deptid,
                                           seq = t.seq,
                                           company = t.company,
                                           taxepense = t.taxexpense
                                       }).ToList();
                foreach (var item in tempAccountInfo)
                {
                    decimal taxAmount = 0;
                    var bdexp = bdexpListInfo.Where(w => w.expcode == item.expcode && w.expname == item.expname && w.company == item.company).FirstOrDefault();
                    //取出detail的该条item当中的所有发票
                    var invoices = invoicelist.Where(w => w.seq == item.seq).ToList();
                    if (bdexp != null && bdexp.isdeduction == "Y" && !string.IsNullOrEmpty(bdexp.invoicecategory))
                    {
                        var invcategory = ChineseConverter.Convert(bdexp.invoicecategory, ChineseConversionDirection.TraditionalToSimplified)
                        .Split(";")
                        .Select(s => Regex.Replace(s, "[()]", ""))
                        .Select(s => Regex.Replace(s, "[（）]", ""))
                        .ToList();
                        foreach (var inv in invoices)
                        {
                            if (inv.invoiceid != null && !string.IsNullOrEmpty(inv.invtype) &&
                                invcategory.Contains(ChineseConverter.Convert(Regex.Replace(inv.invtype, "[（）]", ""), ChineseConversionDirection.TraditionalToSimplified)))
                            {
                                taxAmount += item.taxepense.HasValue ? Convert.ToDecimal(item.taxepense) : inv.taxamount;
                            }
                        }
                    }
                    AccountInfoDto accountInfoDto = new();
                    accountInfoDto.deptid = item.deptid;
                    accountInfoDto.postkey = item.postkey;
                    accountInfoDto.acctcode = item.acctcode;
                    accountInfoDto.baseamt = item.baseamt;
                    accountInfoDto.txtcode = item.txtcode;
                    accountInfoDto.costcenter = item.costcenter;
                    accountInfoDto.asinmnt = item.asinmnt;
                    accountInfoDto.expname = item.expname;
                    accountInfoDto.expcode = item.expcode;
                    accountInfoDto.payeeid = item.payeeid;
                    accountInfoDto.invoice = "";
                    accountInfoDto.taxbase = 0;
                    accountInfoDto.lctaxbase = taxAmount;
                    accountInfoDto.rdate = item.rdate;
                    accountInfoDto.unifycode = item.unifycode;
                    accountInfoDto.certificate = item.certificate;
                    accountInfoDto.item = item.seq;
                    accountInfoDto.ref1 = "";
                    accountInfoDto.lintext = GetSummary(cashdetails, cashheads, rno, item.acctcode, item.payeeid, item.deptid, item.seq).Result;
                    strposting.Add(accountInfoDto);
                }
                if (strposting.Count > 0)
                {
                    for (int i = 0; i < strposting.Count; i++)
                    {
                        if (strposting[i].costcenter != string.Empty)
                        {
                            string companycode = await _EmpOrgRepository.GetCompanyCodeByDeptid(strposting[i].deptid);
                            string deptCompany = (await _CompanyRepository.WithDetailsAsync()).Where(w => w.CompanyCategory == companycode).AsNoTracking().FirstOrDefault()?.company;
                            string deptStarwith = await _CompanyRepository.GetStarwith(deptCompany);
                            strposting[i].costcenter = deptStarwith + strposting[i].costcenter;
                        }
                        if (comtaxcode.Count > 0)
                        {
                            strposting[i].txtcode = comtaxcode[0].taxcode;
                        }
                    }
                }
            }
            //是否有预支金
            if (cashheads.amount.ToString() != string.Empty && cashheads.amount != 0)
            {
                //如果选择了预支金作报销
                var advanceDetailItems = cashdetails.Where(w => !String.IsNullOrEmpty(w.advancerno)).ToList();
                foreach (var item in advanceDetailItems)
                {
                    var advanceAmountInfo = advanceInfoList.Where(w => w.rno == item.advancerno).ToList();//预支金cashamount的数据
                    var advanceHead = advanceHeads.Where(w => w.rno == item.advancerno).ToList();//预支金cashhead的数据
                    var advanceDetail = advanceDetails.Where(w => w.rno == item.advancerno).ToList();//预支金cashdetail的数据
                    if (advanceAmountInfo.Count > 0 && advanceHead.Count > 0 && advanceDetail.Count > 0)
                    {
                        AccountInfoDto accountInfoDto = new AccountInfoDto();
                        accountInfoDto.psitem = Convert.ToInt32(strposting.Count.ToString() + 1);
                        accountInfoDto.postkey = "50";
                        accountInfoDto.acctcode = "15039000";
                        accountInfoDto.baseamt = cashheads.amount;
                        accountInfoDto.txtcode = string.Empty;
                        accountInfoDto.costcenter = string.Empty;
                        accountInfoDto.lintext = GetSummary(advanceDetails, advanceHead.FirstOrDefault(), item.advancerno, advanceDetail.FirstOrDefault().acctcode, advanceHead.FirstOrDefault().payeeId, advanceHead.FirstOrDefault().deptid, advanceDetail.FirstOrDefault().seq).Result;
                        accountInfoDto.asinmnt = advanceHead.FirstOrDefault().deptid + "/" + advanceHead.FirstOrDefault().cuser;//取预支金的申请人/工号
                        accountInfoDto.payeeid = strposting[0].payeeid;
                        accountInfoDto.item = strposting[0].item;
                        strposting.Add(accountInfoDto);
                    }
                }
            }
            for (int i = 0; i < strposting.Count; i++)
            {
                if (!string.IsNullOrEmpty(strposting[i].acctcode))
                {
                    int acctcode = Convert.ToInt32(strposting[i].acctcode.ToString());
                    if (acctcode < 71100000 || acctcode == 71100000)
                    {
                        strposting[i].txtcode = string.Empty;
                    }
                }
            }
            for (int i = 0; i < strposting.Count; i++)
            {
                strposting[i].psitem = Convert.ToInt32(i.ToString());
            }
            //2.赋值插入数据库
            if (strposting.Count > 0)
            {
                List<CashDetailPst> cashDetailPsts = new List<CashDetailPst>();
                CashDetailPst invTaxDetailPst = new CashDetailPst();
                if (bdexpListInfo.Where(w => w.isdeduction == "Y").ToList().Count > 0)
                {
                    List<string> taxCodeList = new List<string>();//存放taxcode
                    var invListInfo = (await _InvoiceRepository.GetByNo(rno)).Where(w => !String.IsNullOrEmpty(w.invtype)).ToList();
                    var taxCodeInfo = (await _BDTaxRateRepository.WithDetailsAsync()).AsNoTracking().ToList();
                    decimal unTaxAmount = 0;
                    decimal taxAmount = 0;
                    decimal detailTaxAmount = 0;
                    List<string> invoiceNums = new();
                    DateTime? invdate = null;
                    foreach (var detailitem in cashdetails)
                    {
                        //检查费用情景是否税金抵扣
                        var bdexp = bdexpListInfo.Where(w => w.expcode == detailitem.expcode && w.expname == detailitem.expname && w.company == detailitem.company).FirstOrDefault();
                        if (bdexp != null && bdexp.isdeduction == "Y" && !String.IsNullOrEmpty(bdexp.invoicecategory))
                        {
                            var invcategory = ChineseConverter.Convert(bdexp.invoicecategory, ChineseConversionDirection.TraditionalToSimplified)
                            .Split(";")
                            .Select(s => Regex.Replace(s, "[()]", ""))
                            .Select(s => Regex.Replace(s, "[（）]", ""))
                            .ToList();
                            //取出detail的该条item当中的所有符合该报销情景的发票类型的所有发票
                            var invoices = invListInfo.Where(w => w.seq == detailitem.seq)
                            .Where(w =>
                                invcategory.Contains(ChineseConverter.Convert(Regex.Replace(w.invtype, "[()]", ""), ChineseConversionDirection.TraditionalToSimplified)) ||
                                invcategory.Contains(ChineseConverter.Convert(Regex.Replace(w.invtype, "[（）]", ""), ChineseConversionDirection.TraditionalToSimplified))
                            ).ToList();
                            //.Where(w => invcategory.Contains(w.invtype))
                            if (invoices.Count > 0)
                            {
                                invdate = invoices.Where(w => w.invdate.HasValue)?.FirstOrDefault()?.invdate;
                                invoiceNums.AddRange(invoices.Select(w => w.invno));
                                if (detailitem.taxexpense != null)
                                {
                                    detailTaxAmount += Convert.ToDecimal(detailitem.taxexpense);
                                }
                                else
                                {
                                    taxAmount += invoices.Sum(w => w.taxamount);
                                }
                                unTaxAmount += invoices.Sum(w => w.amount);
                                foreach (var inv in invoices)
                                {
                                    if (inv.amount > 0 && inv.taxamount > 0 && taxCodeList.Count == 0)
                                    {
                                        decimal taxRate = Math.Round(inv.taxamount / inv.amount, 2, MidpointRounding.AwayFromZero);
                                        string sapcode = taxCodeInfo.Where(w => w.taxrate == taxRate).FirstOrDefault()?.sapcode;
                                        taxCodeList.Add(sapcode);
                                    }
                                }
                            }
                        }
                    }
                    if (invoiceNums.Count > 0)
                    {
                        invTaxDetailPst.ddate = invdate.HasValue ? invdate : System.DateTime.Now;
                        invTaxDetailPst.asinmnt = String.Empty;
                        invTaxDetailPst.postkey = "40";
                        invTaxDetailPst.acctcode = "15033100";
                        invTaxDetailPst.basecurr = baseCurr;
                        invTaxDetailPst.company = company;
                        invTaxDetailPst.lctaxbase = taxAmount + detailTaxAmount;
                        invTaxDetailPst.txtcode = taxCodeList.FirstOrDefault();
                        invTaxDetailPst.taxbase = unTaxAmount;
                        invTaxDetailPst.costcenter = String.Empty;
                        invTaxDetailPst.formcode = formcode;
                        invTaxDetailPst.rno = rno;
                        invTaxDetailPst.detailitem = 1;
                        invTaxDetailPst.ref1 = "";
                        invTaxDetailPst.payeeid = "";
                        invTaxDetailPst.cdate = System.DateTime.Now;
                        invTaxDetailPst.cuser = userId;
                        invTaxDetailPst.lintext = "零用金税金" + string.Join(";", invoiceNums);
                        cashDetailPsts.Add(invTaxDetailPst);
                    }
                }
                for (int i = 0; i < strposting.Count; i++)
                {
                    string assignment = cashdetails.Where(w => w.company == company && w.expcode == strposting[i].expcode).FirstOrDefault()?.assignment;
                    //會計維護的assignment
                    string bdexpAsinmnt = bdexpQuery.Where(w => w.company == company && w.expcode == strposting[i].expcode && w.category == category).FirstOrDefault()?.assignment;
                    //會計維護的costcenter
                    string bdexpCostcenter = bdexpQuery.Where(w => w.company == company && w.expcode == strposting[i].expcode && w.category == category).FirstOrDefault()?.costcenter;
                    //如果为一般费用报销，
                    if (formcode == "CASH_1")
                    {
                        List<departCost> res = new();
                        if (!string.IsNullOrEmpty(strposting[i].costcenter))
                        {
                            int index1 = strposting[i].costcenter.IndexOf("[");
                            int arr = strposting[i].costcenter.IndexOf("]") + 1 - strposting[i].costcenter.IndexOf("[");
                            res = JsonConvert.DeserializeObject<List<departCost>>(strposting[i].costcenter.Substring(index1, arr));
                            for (int j = 0; j < res.Count; j++)
                            {
                                CashDetailPst cashDetailPst = new CashDetailPst();
                                cashDetailPst.formcode = formcode;
                                cashDetailPst.rno = rno;
                                cashDetailPst.pstitem = i;
                                cashDetailPst.postkey = strposting[i].postkey;
                                cashDetailPst.basecurr = baseCurr;
                                cashDetailPst.baseamt = Convert.ToDecimal(strposting[i].baseamt * res[j].percent) / 100;
                                cashDetailPst.postkey = strposting[i].postkey;
                                cashDetailPst.acctcode = strposting[i].acctcode;
                                cashDetailPst.txtcode = strposting[i].txtcode;
                                //cost center：優先抓會計維護的cctr，如沒有維護即抓報銷單據掛賬部門
                                cashDetailPst.costcenter = !String.IsNullOrEmpty(bdexpCostcenter) ? bdexpCostcenter : (res[j].deptId);
                                cashDetailPst.lintext = strposting[i].lintext;
                                cashDetailPst.cuser = userId;
                                cashDetailPst.payeeid = strposting[i].payeeid;
                                cashDetailPst.invoice = strposting[i].invoice;
                                cashDetailPst.taxbase = strposting[i].taxbase;
                                cashDetailPst.lctaxbase = strposting[i].lctaxbase;
                                cashDetailPst.rdate = strposting[i].rdate.ToString();
                                cashDetailPst.unifycode = strposting[i].unifycode;
                                cashDetailPst.certificate = strposting[i].certificate;
                                cashDetailPst.detailitem = strposting[i].item;
                                cashDetailPst.ref1 = strposting[i].ref1;
                                cashDetailPst.expcode = strposting[i].expcode;
                                cashDetailPst.company = company;
                                cashDetailPst.ddate = strposting[i].rdate;
                                //assignment：預支金統一：申請人部門/工號，報銷抓情景維護的assignment+白單備註，無白單則抓情景維護的assignment或無
                                cashDetailPst.asinmnt = bdexpAsinmnt + assignment;
                                cashDetailPst.cdate = System.DateTime.Now;
                                cashDetailPsts.Add(cashDetailPst);
                            }
                        }
                        if (String.IsNullOrEmpty(strposting[i].costcenter) && strposting[i].postkey == "50")
                        {
                            CashDetailPst cashDetailPst = new CashDetailPst();
                            cashDetailPst.pstitem = strposting.Count + 1;
                            cashDetailPst.postkey = strposting[i].postkey;
                            cashDetailPst.acctcode = strposting[i].acctcode;
                            cashDetailPst.baseamt = Convert.ToDecimal(strposting[i].baseamt);
                            cashDetailPst.txtcode = strposting[i].txtcode;
                            cashDetailPst.costcenter = strposting[i].costcenter;
                            cashDetailPst.lintext = strposting[i].lintext;
                            cashDetailPst.asinmnt = strposting[i].asinmnt;
                            cashDetailPst.payeeid = strposting[i].payeeid;
                            cashDetailPst.detailitem = strposting[i].item;
                            cashDetailPst.formcode = formcode;
                            cashDetailPst.rno = rno;
                            cashDetailPst.basecurr = baseCurr;
                            cashDetailPst.company = company;
                            cashDetailPsts.Add(cashDetailPst);
                        }
                    }
                    else
                    {
                        //預支金assignment
                        string cash3Asinmnt = cashheads.deptid + "/" + cashheads.cuser;
                        CashDetailPst cashDetailPst = new CashDetailPst();
                        cashDetailPst.formcode = formcode;
                        cashDetailPst.rno = rno;
                        cashDetailPst.pstitem = i;
                        cashDetailPst.postkey = strposting[i].postkey;
                        cashDetailPst.basecurr = baseCurr;
                        cashDetailPst.baseamt = Convert.ToDecimal(strposting[i].baseamt);
                        cashDetailPst.postkey = strposting[i].postkey;
                        cashDetailPst.acctcode = strposting[i].acctcode;
                        cashDetailPst.txtcode = strposting[i].txtcode;
                        //cost center：優先抓會計維護的cctr，如沒有維護即抓報銷單據掛賬部門
                        cashDetailPst.costcenter = !String.IsNullOrEmpty(bdexpCostcenter) ? bdexpCostcenter : strposting[i].costcenter;
                        cashDetailPst.lintext = strposting[i].lintext;
                        cashDetailPst.cuser = userId;
                        cashDetailPst.muser = userId;
                        cashDetailPst.payeeid = strposting[i].payeeid;
                        cashDetailPst.invoice = strposting[i].invoice;
                        cashDetailPst.taxbase = strposting[i].taxbase;
                        cashDetailPst.lctaxbase = strposting[i].lctaxbase;
                        cashDetailPst.rdate = strposting[i].rdate.ToString();
                        cashDetailPst.unifycode = strposting[i].unifycode;
                        cashDetailPst.certificate = strposting[i].certificate;
                        cashDetailPst.detailitem = strposting[i].item;
                        cashDetailPst.ref1 = strposting[i].ref1;
                        cashDetailPst.expcode = strposting[i].expcode;
                        cashDetailPst.company = company;
                        cashDetailPst.ddate = strposting[i].rdate;
                        //assignment：預支金統一：申請人部門/工號，報銷抓情景維護的assignment+白單備註，無白單則抓情景維護的assignment或無
                        cashDetailPst.asinmnt = formcode == "CASH_3" ? cash3Asinmnt : (bdexpAsinmnt + assignment);
                        cashDetailPst.cdate = System.DateTime.Now;
                        cashDetailPsts.Add(cashDetailPst);
                    }
                }
                await _CashDetailPstRepository.InsertManyAsync(cashDetailPsts);
                result.message = L["AccountInfoSaveSuccess"];
            }
            else
            {
                result.status = 2;
                result.message = L["AccountInfoSaveFail"];
                return result;
            }
            return result;
        }
        /// <summary>
        /// 保存薪資請款入賬信息
        /// </summary>
        /// <param name="rno"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<string>> SaveCashXAccInfo(string rno, string userId)
        {
            Result<string> result = new Result<string>();
            List<CashDetailPst> infoList = new();
            var pstCount = (await _CashDetailPstRepository.WithDetailsAsync()).Where(w => w.rno == rno).Count();
            if (pstCount > 0)
            {
                result.status = 2;
                result.message = L["AccountInfoSaveFail"] + "," + L["AccountInfoExist"];
                return result;
            }
            //1.查询信息
            var cashheads = await _CashHeadRepository.ReadCashHeadsByNo(rno);
            var cashdetails = await _CashDetailRepository.ReadCashDetailsByNo(rno);
            var comBaseData = (await _CompanyRepository.WithDetailsAsync())
            .Where(w => cashdetails.Select(s => s.companycode).Contains(w.CompanyCategory))
            .Select(g => new { g.CompanyCategory, g.CompanySap }).ToList();
            if (cashdetails.Count == 0 || cashheads == null)
            {
                result.message = L["RnoHaveNoData"];
                result.status = 2;
                return result;
            }
            for (int i = 0; i < cashdetails.Count; i++)
            {
                CashDetailPst cashDetailPst = new()
                {
                    formcode = cashdetails[i].formcode,
                    rno = rno,
                    detailitem = cashdetails[i].seq,
                    basecurr = cashdetails[i].basecurr,
                    baseamt = cashdetails[i].amount.HasValue ? Convert.ToDecimal(cashdetails[i].amount) : 0,
                    postkey = "40",
                    acctcode = cashdetails[i].acctcode,
                    txtcode = string.Empty,
                    costcenter = string.Empty,
                    lintext = cashdetails[i].remarks,
                    asinmnt = "XINZI" + (cashdetails[i].summary.Length >= 4 ? cashdetails[i].summary.Substring(cashdetails[i].summary.Length - 4) : cashdetails[i].summary) + "-" + cashdetails[i].companycode,
                    pstitem = cashdetails[i].seq,
                    ddate = cashdetails[i].rdate,
                    expcode = cashdetails[i].expcode,
                    company = cashdetails[i].company,
                    cdate = System.DateTime.Now,
                    payeeid = cashdetails[i].cuser,//薪資請款無收款人 統一取申請人
                    cuser = userId
                };
                infoList.Add(cashDetailPst);
            }
            if (infoList.Count > 0)
            {
                await _CashDetailPstRepository.InsertManyAsync(infoList);
            }
            result.status = 1;
            result.message = "Success";
            return result;
        }
        public async Task<string> GetSummary(List<CashDetail> cashDetails, CashHead cashhead, string srno, string sacctcode, string semplid, string sdeptid, int seq)
        {
            var bdexps = (await _BDExpRepository.WithDetailsAsync()).Where(w => cashDetails.Select(s => s.expcode).Contains(w.expcode)).ToList();
            var unionquery = (from t in cashDetails
                              where (t.seq == seq && t.rno == cashhead.rno && t.rno == srno && t.acctcode == sacctcode && t.deptid == sdeptid && (t.payeeid == semplid || cashhead.payeeId == semplid))
                              select new
                              {
                                  relst = t.summary,
                                  obj = t.@object,
                                  nemplid = semplid,
                                  expcode = t.expcode,
                                  expname = t.expname,
                                  company = t.company,
                                  formcode = t.formcode
                              }).FirstOrDefault();
            string ymth = System.DateTime.Now.ToString("yy/MM");
            string cash3YMTH = System.DateTime.Now.ToString("yyyyMM");
            string sname = string.Empty;
            if (unionquery?.nemplid != null)
            {
                sname = (await _EmloyeeRepository.GetCnameByEmplid(unionquery.nemplid.ToString()));
            }
            string suffname = string.Empty;
            if (srno.Substring(0, 1) == "A")
            {
                suffname = "預支";
            }
            else if (srno.Substring(0, 1) == "E")
            {
                suffname = "報銷招待";
            }
            else
            {
                suffname = "報銷";
            }
            string relst = string.Empty;
            if (srno.Substring(0, 1) == "E")
            {
                relst = ymth + sname + suffname + unionquery.obj + unionquery.relst + "客人費用";
            }
            //預支金申請Line Text統一修改：年月+申請人所在部門+申請人工號+申請人+預支內容，即202303-MZF350-Z15103067-張焯雅-預支出差費用；
            else if (srno.Substring(0, 1) == "A")
            {
                Employee employee = await (await _EmloyeeRepository.WithDetailsAsync()).Where(w => w.emplid == unionquery.nemplid).FirstOrDefaultAsync();
                relst = cash3YMTH + "-" + employee.deptid + "-" + unionquery.nemplid + "-" + sname + "-" + suffname + unionquery.obj + unionquery.relst + "費用";
            }
            //2023.6.6 返台会议的linetext改为 日期（年/月）+ 收款人 + 单据类型（报销/预支） + 费用情景 + 费用
            else if (srno.Substring(0, 1) == "H")
            {
                string expname = bdexps.Where(w => w.expcode == unionquery.expcode && w.company == unionquery.company && w.category == 1).FirstOrDefault()?.expname;
                if (expname != null)
                {
                    expname = ChineseConverter.Convert(expname, ChineseConversionDirection.TraditionalToSimplified);
                    if (expname.Contains("返台车资"))
                    {
                        //2023.06.07 返臺車資類型寫死“定期返台會議車資”
                        relst = ymth + sname + suffname + unionquery.obj + "定期返台會議車資" + "费用";
                    }
                    else
                    {
                        relst = ymth + sname + suffname + unionquery.obj + unionquery.relst + "費用";
                    }
                }
                else
                {
                    relst = ymth + sname + suffname + unionquery.obj + unionquery.relst + "費用";
                }
            }
            else
            {
                relst = ymth + sname + suffname + unionquery.obj + unionquery.relst + "費用";
            }
            return relst;
        }
    }
}

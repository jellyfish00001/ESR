using ERS.Domain.Entities.Application;
using System.Net.Http;
using ERS.Domain.IDomainServices;
using ERS.DTO.BDExp;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Domain.Repositories;
using ERS.Application.Contracts.DTO.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using System.IO;
namespace ERS.DomainServices
{
    public class Cash3DomainService : CommonDomainService, ICash3DomainService
    {
        private IObjectMapper _ObjectMapper;
        private IAutoNoRepository _AutoNoRepository;
        private ICashHeadRepository _CashHeadRepository;
        private ICashDetailRepository _CashDetailRepository;
        private ICashFileRepository _CashFileRepository;
        private ICashAccountRepository _CashAccountRepository;
        private IBDExpRepository _BDExpRepository;
        private IBDExpDomainService _BDExpDomainService;
        private IInvoiceRepository _InvoiceRepository;
        private ICashAmountRepository _CashAmountRepository;
        private IMinioDomainService _MinioDomainService;
        private IEFormHeadRepository _EFormHeadRepository;
        private IConfiguration _Configuration;
        private IHttpClientFactory _HttpClient;
        private IInvoiceDomainService _InvoiceDomainService;
        private IApprovalPaperDomainService _EFormPaperDomainService;
        private IRepository<BdPayTyp, Guid> _BdPayTypRepository;
        private IEmployeeDomainService _EmployeeDomainService;
        private IEmpOrgRepository _EmpOrgRepository;
        private IAppConfigRepository _AppConfigRepository;
        private IRepository<BDCashReturn, Guid> _bdcashreturnRepository;
        private IApprovalDomainService _ApprovalDomainService;
        private IEFormAlistRepository _EFormAlistRepository;
        private IEFormAuserRepository _EFormAuserRepository;
        private IEmployeeRepository _EmployeeRepository;
        private IBDashReturnRepository _BDashReturnRepository;
        private IBdAccountRepository _BdAccountRepository;
        private IMinioDomainService _minioDomainService;
        public Cash3DomainService(
        IAutoNoRepository AutoNoRepository,
        IObjectMapper ObjectMapper,
        ICashHeadRepository CashHeadRepository,
        ICashDetailRepository CashDetailRepository,
        ICashFileRepository CashFileRepository,
        ICashAccountRepository CashAccountRepository,
        IBDExpDomainService BDExpDomainService,
        IInvoiceRepository InvoiceRepository,
        ICashAmountRepository CashAmountRepository,
        IMinioDomainService MinioDomainService,
        IEFormHeadRepository EFormHeadRepository,
        IConfiguration Configuration,
        IInvoiceDomainService InvoiceDomainService,
        IApprovalPaperDomainService EFormPaperDomainService,
        IEmployeeDomainService EmployeeDomainService,
        IEmpOrgRepository EmpOrgRepository,
        IBDExpRepository BDExpRepository,
        IAppConfigRepository AppConfigRepository,
        IHttpClientFactory HttpClient,
        IRepository<BDCashReturn, Guid> bdcashreturnRepository,
        IBdAccountRepository BdAccountRepository,
        IEmployeeRepository EmployeeRepository,
        IRepository<BdPayTyp, Guid> BdPayTypRepository,
        IApprovalDomainService ApprovalDomainService,
        IEFormAlistRepository EFormAlistRepository,
        IEFormAuserRepository EFormAuserRepository,
        IBDashReturnRepository BDashReturnRepository,
         IMinioDomainService minioDomainService)
        {
            _ObjectMapper = ObjectMapper;
            _AutoNoRepository = AutoNoRepository;
            _CashHeadRepository = CashHeadRepository;
            _CashDetailRepository = CashDetailRepository;
            _CashFileRepository = CashFileRepository;
            _CashAccountRepository = CashAccountRepository;
            _BDExpRepository = BDExpRepository;
            _BDExpDomainService = BDExpDomainService;
            _InvoiceRepository = InvoiceRepository;
            _CashAmountRepository = CashAmountRepository;
            _MinioDomainService = MinioDomainService;
            _EFormHeadRepository = EFormHeadRepository;
            _Configuration = Configuration;
            _HttpClient = HttpClient;
            _InvoiceDomainService = InvoiceDomainService;
            _EFormPaperDomainService = EFormPaperDomainService;
            _EmployeeDomainService = EmployeeDomainService;
            _EmpOrgRepository = EmpOrgRepository;
            _AppConfigRepository = AppConfigRepository;
            _BdPayTypRepository = BdPayTypRepository;
            _BdPayTypRepository = BdPayTypRepository;
            _bdcashreturnRepository = bdcashreturnRepository;
            _ApprovalDomainService = ApprovalDomainService;
            _EFormAlistRepository = EFormAlistRepository;
            _EFormAuserRepository = EFormAuserRepository;
            _EmployeeRepository = EmployeeRepository;
            _BDashReturnRepository = BDashReturnRepository;
            _BdAccountRepository = BdAccountRepository;
            _minioDomainService = minioDomainService;
        }
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P")
        {
            Result<CashResult> result = new();
            CashResult cash = new();
            result.status = 2;
            CashHead list = await TransformAndSetData(formCollection, user);
            if (status == "T")
                list.SetKeepStatus("RQ401");
            //应该判断收款人是不是处级
            Result<bool> result1 = await this.IsChangeApplicant(list.payeeId, Convert.ToDecimal(list.CashDetailList[0].baseamt), list.company);
            if (result1.data)
            {
                EmpOrg empOrg = await this.GetEmpOrgManager(list.deptid);
                if (empOrg != null)
                    list.payeeId = empOrg.manager_id;
                if (empOrg != null)
                {
                    string name = (await _EmployeeRepository.WithDetailsAsync()).Where(i => i.emplid == list.payeeId).AsNoTracking().Select(i => i.cname).First();
                    list.payeename = name;
                }
            }
            list.SetPayeeAccount(await _CashAccountRepository.GetAccount(list.payeeId));
            if (!list.CheckIsExistRno())
            {
                string rno = await _AutoNoRepository.CreateCash3No();
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
            List<string> codes = list.CashDetailList.Select(i => i.expcode).Distinct().ToList();
            List<string> acctcodes = list.CashDetailList.Select(w => w.acctcode).Distinct().ToList();
            string type = await _BDExpDomainService.GetSignDType(list.company, codes, acctcodes);
            list.Setdtype(type);
            // file
            await SaveFile(formCollection, list);
            bool saveFlag = await InsertData(list);
            // 上传附件需加签财务
            if (saveFlag) {
                bool signFin = list.CashFileList.Count > 0;
                if (status.Equals("P"))
                    await _ApprovalDomainService.CreateSignSummary(list, signFin, token);
            }
            cash.rno = list.rno;
            cash.Stat = false;
            result.data = cash;
            result.message = (cash.Stat ? L["AddPaperTipMessage"] : L["UnAddPaperTipMessage"]);
            result.status = 1;
            return result;
        }
        // 根据expcode查询需要上传的附件类型
        public async Task<Result<List<BDExpDto>>> QueryFilecategoryByExpcode(string expcode, string company, int category)
        {
            Result<List<BDExpDto>> result = new();
            List<BDExpDto> typelist = (from b in (await _BDExpRepository.WithDetailsAsync())
                                       where b.expcode == expcode && b.company == company && b.category == category
                                       select new BDExpDto
                                       {
                                           filecategory = b.filecategory,
                                           isupload = b.isupload,
                                           filepoints = b.filepoints,
                                           isinvoice = b.isinvoice
                                       }).ToList();
            result.data = typelist;
            return result;
        }
        async Task<CashHead> TransformAndSetData(IFormCollection formCollection, string user)
        {
            string head = formCollection["head"];
            string detail = formCollection["detail"];
            string file = formCollection["file"];
            //string inv = formCollection["inv"];
            string amount = formCollection["amount"];
            CashHeadDto hData = JsonConvert.DeserializeObject<CashHeadDto>(head);
            IList<CashDetailDto> dData = JsonConvert.DeserializeObject<IList<CashDetailDto>>(detail);
            IList<CashFileDto> fData = JsonConvert.DeserializeObject<IList<CashFileDto>>(file);
            //IList<InvoiceDto> iData = JsonConvert.DeserializeObject<IList<InvoiceDto>>(inv);
            CashAmountDto aData = JsonConvert.DeserializeObject<CashAmountDto>(amount);
            CashHead list = _ObjectMapper.Map<CashHeadDto, CashHead>(hData);
            IList<CashDetail> detailData = _ObjectMapper.Map<IList<CashDetailDto>, IList<CashDetail>>(dData);
            IList<CashFile> fileData = _ObjectMapper.Map<IList<CashFileDto>, IList<CashFile>>(fData);
            //IList<Invoice> invData = _ObjectMapper.Map<IList<InvoiceDto>, IList<Invoice>>(iData);
            CashAmount amountData = _ObjectMapper.Map<CashAmountDto, CashAmount>(aData);
            list.SetCash3(list, user);
            list.AddCash3Detail(detailData);
            list.AddFile(fileData);
            //list.AddInvoice(invData);
            list.AddCashAmount(amountData);
            return list;
        }
        async Task DeleteData(CashHead list)
        {
            List<EFormAlist> eFormAlists = await _EFormAlistRepository.GetListAsync(i => i.rno == list.rno);
            if (eFormAlists.Count > 0) await _EFormAlistRepository.DeleteManyAsync(eFormAlists);
            List<EFormAuser> eFormAuser = await _EFormAuserRepository.GetListAsync(i => i.rno == list.rno);
            if (eFormAuser.Count > 0) await _EFormAuserRepository.DeleteManyAsync(eFormAuser);
            await _CashHeadRepository.DeleteAsync(await _CashHeadRepository.GetByNo(list.rno));
            await _CashDetailRepository.DeleteManyAsync(await _CashDetailRepository.GetByNo(list.rno));
            // await _InvoiceRepository.DeleteManyAsync(await _InvoiceRepository.GetByNo(list.rno));
            await _CashFileRepository.DeleteManyAsync(await _CashFileRepository.GetByNo(list.rno));
            await _CashAmountRepository.DeleteAsync(await _CashAmountRepository.GetByNo(list.rno));
        }
        async Task<bool> InsertData(CashHead list)
        {
            try
            {
                await _CashHeadRepository.InsertAsync(list);
                await _CashDetailRepository.InsertManyAsync(list.CashDetailList);
                //await _InvoiceRepository.InsertManyAsync(list.InvoiceList);
                await _CashFileRepository.InsertManyAsync(list.CashFileList);
                await _CashAmountRepository.InsertAsync(list.CashAmount);
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }
        async Task SaveFile(IFormCollection formCollection, CashHead list)
        {
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
                    path = await _MinioDomainService.PutObjectAsync(list.rno, fileName, steam, type,area);
                }
                list.SetOriginalFileNameAndSavePath(file.Name, path, fileName, type);
            }
        }
        //預支金申請大於一定金額，單據送出時系統增加提示=>收款人變更:“預支金額≥1萬元，收款人變更為***，請知悉”
        public async Task<Result<ChangePayeeTipsDto>> ChangePayeeTips(string emplid, decimal amount, string company)
        {
            Result<ChangePayeeTipsDto> result = new()
            {
                data = new()
                {
                    changeid = "",
                    changename = "",
                    ischange = false
                }
            };
            Result<bool> change = await this.IsChangeApplicant(emplid, amount, company);
            if (change.data)
            {
                var configQuery = (await _AppConfigRepository.WithDetailsAsync()).Where(w => w.company == company && w.key == "advance").AsNoTracking().FirstOrDefault();
                var levelQueyry = (await _EmpOrgRepository.WithDetailsAsync()).Where(w => w.manager_id == emplid && w.tree_level_num <= 7).AsNoTracking().FirstOrDefault();
                string deptid = (await _EmployeeRepository.WithDetailsAsync()).Where(w => w.emplid == emplid).Select(e => e.deptid).FirstOrDefault();
                EmpOrg empOrg = await this.GetEmpOrgManager(deptid);
                string name = (await _EmployeeRepository.WithDetailsAsync()).Where(i => i.emplid == empOrg.manager_id).AsNoTracking().Select(i => i.cname).First();
                result.message = String.Format(L["CASH3-OverAmountTips"], configQuery.value, empOrg.manager_id + "/" + name);
                result.data = new ChangePayeeTipsDto()
                {
                    changeid = empOrg.manager_id,
                    changename = name,
                    ischange = true
                };
                return result;
            }
            return result;
        }
        //预支金逾期用户返回
        public async Task<Result<List<OverdueUserDto>>> OverdueUser(string user)
        {
            Result<List<OverdueUserDto>> result = new();
            List<OverdueUserDto> overdues = new List<OverdueUserDto>();
            Employee employee = await _EmployeeDomainService.QueryEmployeeAsync(user);
            // IList<EmpOrg> empOrgs = await _EmpOrgRepository.GetDeptidList(employee.company);
            IList<EmpOrg> empOrgs = (await _EmpOrgRepository.WithDetailsAsync()).GroupBy(s => s.deptid).Select(g => g.FirstOrDefault()).AsNoTracking().ToList();
            EmpOrg empDept = await GetCostdeptid(employee.deptid, employee.company, empOrgs);
            List<string> depts = new();
            // 截止至处级
            if (empDept.tree_level_num >= 5)
                depts = await GetOutdeptid(empDept.deptid, employee.company, empOrgs);
            depts.Add(empDept.deptid);
            //查詢部門下所有detail表數據
            IList<CashDetail> cashDetails = await _CashDetailRepository.ReadByDeptid(depts, "CASH_3");
            //查詢部門下所有head表數據
            IList<CashHead> cashHeads = await _CashHeadRepository.ReadByDeptid(depts);
            List<string> headRno = new List<string>();
            cashHeads.ForEach(b => headRno.Add(b.rno));
            IList<BDCashReturn> cashReturns = (await _BDashReturnRepository.WithDetailsAsync()).Where(i => headRno.Contains(i.rno)).ToList();
            //查詢部門下所有amount數據
            IList<CashAmount> cashAmounts = await _CashAmountRepository.ReadByRno(headRno);
            //查询单据的所有情景数据
            IList<BdExp> bdExps = (await _BDExpRepository.WithDetailsAsync()).Where(w => cashDetails.Select(s => s.company).Contains(w.company) && cashDetails.Select(g => g.expcode).Contains(w.expcode) && w.category == 2).ToList();
            int level = 0;
            if (empDept.tree_level_num > 7)
            {//大於7為廠處級否則為部級
                level = 1;
            }
            foreach (CashDetail item in cashDetails)
            {
                List<BDCashReturn> bDCashReturn = cashReturns.Where(i => item.rno == i.rno).ToList();
                CashAmount amount = cashAmounts.Where(b => b.rno == item.rno).FirstOrDefault();
                CashHead head = cashHeads.Where(w => w.rno == item.rno).FirstOrDefault();
                if (bDCashReturn != null)
                    if (amount.actamt - bDCashReturn.Sum(i => i.amount) == 0) continue;
                    else { amount.actamt -= bDCashReturn.Sum(i => i.amount); }
                DateTime dt1 = DateTime.Now;
                DateTime dt2 = Convert.ToDateTime(item.cdate);//申请日期
                DateTime dt3 = Convert.ToDateTime(item.revsdate);//预定冲销日期
                DateTime? payDt = head.payment == null ? null : Convert.ToDateTime(head.payment);//付款日期
                DateTime Convert_dt1 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", dt1.Year, dt1.Month, dt1.Day));
                DateTime Convert_dt2 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", dt2.Year, dt2.Month, dt2.Day));
                DateTime Convert_dt3 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", dt3.Year, dt3.Month, dt3.Day));
                var expInfo = bdExps.Where(w => w.company == item.company && w.expcode == item.expcode && w.category == 2).FirstOrDefault();
                int Days_3 = 0;
                if (expInfo != null)
                {
                    //今天-预支付款日期
                    if (expInfo.calmethod == 1)
                    {
                        //已签完但未付款
                        if (head.payment == null)
                        {
                            Days_3 = 0;
                        }
                        else if (head.payment != null)
                        {
                            DateTime paymentDt = Convert.ToDateTime(payDt);
                            var tempPayDt = Convert.ToDateTime(string.Format("{0}-{1}-{2}", paymentDt.Year, paymentDt.Month, paymentDt.Day));
                            Days_3 = (dt1 - tempPayDt).Days;
                        }
                    }
                    //今天-预期冲账日期
                    else if (expInfo.calmethod == 2)
                    {
                        Days_3 = (Convert_dt1 - Convert_dt3).Days;
                    }
                    if (level == 0)
                    {
                        if (Days_3 > 60)
                        {
                            OverdueUserDto overdue = new();
                            overdue.company = item.company;
                            overdue.username = head.cname;
                            overdue.user = head.cuser;
                            overdue.payee = head.payeename;
                            overdue.payeeuser = head.payeeId;
                            overdue.rno = head.rno;
                            overdue.remark = item.summary;//摘要
                            overdue.amount = Convert.ToDecimal(item.baseamt);
                            overdue.actamt = amount.actamt;
                            overdue.opendays = Days_3;
                            overdue.delay = head.overduesum;
                            overdue.delaydays = (Convert_dt1 - Convert_dt3).Days < 0 ? 0 : (Convert_dt1 - Convert_dt3).Days;
                            overdue.cdate = item.cdate;
                            overdues.Add(overdue);
                        }
                    }
                    else
                    {
                        if (Days_3 > 90)
                        {
                            // CashHead head = cashHeads.Where(b => b.rno == item.rno).FirstOrDefault();
                            OverdueUserDto overdue = new();
                            overdue.company = item.company;
                            overdue.username = head.cname;
                            overdue.user = head.cuser;
                            overdue.payee = head.payeename;
                            overdue.payeeuser = head.payeeId;
                            overdue.rno = head.rno;
                            overdue.remark = item.summary;//摘要
                            overdue.amount = Convert.ToDecimal(item.baseamt);
                            overdue.actamt = amount.actamt;
                            overdue.opendays = Days_3;
                            overdue.delay = head.overduesum;
                            overdue.delaydays = (Convert_dt1 - Convert_dt3).Days < 0 ? 0 : (Convert_dt1 - Convert_dt3).Days;
                            overdue.cdate = item.cdate;
                            overdues.Add(overdue);
                        }
                    }
                }
            }
            var eFormHeadQuery = (await _EFormHeadRepository.WithDetailsAsync()).Where(w => overdues.Select(s => s.rno).Contains(w.rno) && w.status == "A").ToList();
            overdues = overdues.Where(w => eFormHeadQuery.Select(s => s.rno).Contains(w.rno)).ToList();
            result.data = overdues;
            result.total = overdues.Count;
            return result;
        }
        public async Task<List<string>> GetOutdeptid(string deptid, string company, IList<EmpOrg> empOrgs)
        {
            List<String> depts = new List<string>();
            IList<EmpOrg> empOrg = empOrgs.Where(b => b.uporg_code_a == deptid).ToList();
            foreach (EmpOrg item in empOrg)
            {
                List<String> _depts = await GetOutdeptid(item.deptid, company, empOrgs);
                _depts.Add(item.deptid);
                depts = depts.Union(_depts).ToList();
                //  depts.Add(item.deptid);
            }
            return depts;
        }
        // 查询预支场景（模糊查询）
        public async Task<Result<List<BDExpDto>>> FuzzyQueryScene(string input, string company)
        {
            Result<List<BDExpDto>> result = new Result<List<BDExpDto>>();
            List<BDExpDto> scene = (from q in (await _BDExpRepository.WithDetailsAsync())
                                    join a in (await _BdAccountRepository.WithDetailsAsync())
                                    on q.acctcode equals a.acctcode
                                    where ((q.description.Contains(input) || q.expname.Contains(input) || q.keyword.Contains(input)) && q.type != "1" && q.company == company && q.category == 2)
                                    select new BDExpDto
                                    {
                                        acctcode = q.acctcode,
                                        acctname = a.acctname,
                                        expcode = q.expcode,
                                        expname = q.expname,
                                        description = q.description,
                                        keyword = q.keyword,
                                        datelevel = q.datelevel
                                    })
                                    .Distinct()
                                    .ToList();
            result.data = scene;
            return result;
        }
        //查询预支场景（全部）
        public async Task<Result<List<BDExpDto>>> QueryAllScenes(string company)
        {
            Result<List<BDExpDto>> result = new();
            List<BDExpDto> scenes = (from q in (await _BDExpRepository.WithDetailsAsync())
                                     join a in (await _BdAccountRepository.WithDetailsAsync())
                                     on q.acctcode equals a.acctcode
                                     where (q.type != "1" && q.company == company && q.expcode != "EXP02" && q.category == 2)
                                     select new BDExpDto
                                     {
                                         acctcode = q.acctcode,
                                         acctname = a.acctname,
                                         expcode = q.expcode,
                                         expname = q.expname,
                                         description = q.description,
                                         keyword = q.keyword,
                                         datelevel = q.datelevel
                                     }).ToList()
                                     .Distinct(new ExpDtoComparer())
                                     .ToList();
            foreach (var item in scenes)
            {
                string keywordText = !String.IsNullOrEmpty(item.keyword) ? (ChineseConverter.Convert(item.keyword, ChineseConversionDirection.TraditionalToSimplified) +
                ChineseConverter.Convert(item.keyword, ChineseConversionDirection.SimplifiedToTraditional)) : String.Empty;
                string descriptionText = !String.IsNullOrEmpty(item.description) ? ChineseConverter.Convert(item.description, ChineseConversionDirection.TraditionalToSimplified) +
                ChineseConverter.Convert(item.description, ChineseConversionDirection.SimplifiedToTraditional) : String.Empty;
                item.keyword = keywordText + descriptionText;
            }
            result.data = scenes;
            return result;
        }
        public async Task<EmpOrg> GetCostdeptid(string deptid, string company, IList<EmpOrg> empOrgs)
        {
            EmpOrg empOrg = empOrgs.Where(b => b.deptid == deptid).FirstOrDefault();
            EmpOrg emp = new EmpOrg();
            emp = empOrg;
            if (empOrg.tree_level_num > 7)
            {
                emp = await GetCostdeptid(empOrg.uporg_code_a, company, empOrgs);
            }
            return emp;
        }
        //获取费用部门，根据申请人部门向上递归抓取部门
        //判断是否收款人与申请人是否为同一人
        async Task<bool> GetUpdept(string user)
        {
            bool status = false;
            Employee employee = await _EmployeeDomainService.QueryEmployeeAsync(user);
            string manager_id = await GetManagerid(employee.deptid, employee.company);
            if (manager_id.Equals(user))
            {
                status = true;
            }
            return status;
        }
        public async Task<string> GetManagerid(string deptid, string company)
        {
            EmpOrg empOrg = await _EmpOrgRepository.GetDeptid(deptid, company);
            string manager_id = empOrg.manager_id;
            if (empOrg.tree_level_num > 7)
            {
                manager_id = await GetManagerid(empOrg.uporg_code_a, company);
            }
            return manager_id;
        }
        // 获取请款方式
        public async Task<Result<List<BDPayDto>>> QueryBDPayType()
        {
            Result<List<BDPayDto>> result = new();
            List<BDPayDto> bDPayList = (from q in (await _BdPayTypRepository.WithDetailsAsync())
                                        select new BDPayDto
                                        {
                                            PayName = q.payname,
                                            PayType = q.paytyp
                                        }).ToList();
            result.data = bDPayList;
            return result;
        }
        // 根据单号从bpm获取签呈附件
        public async Task<Result<string>> GetSignAttachment(string rno, string Token)
        {
            Result<string> result = new Result<string>();
            string api = _Configuration.GetSection("BPM:Fee").Value;
            api = api + "?formno=" + rno;
            HttpClient httpClient = _HttpClient.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", Token);
            HttpResponseMessage response = await httpClient.GetAsync(api);
            if (response.IsSuccessStatusCode)
            {
                string jsonStr = response.Content.ReadAsStringAsync().Result;
                AttachmentDto urlData = JsonConvert.DeserializeObject<AttachmentDto>(jsonStr);
                if (urlData != null && urlData.status == "success")
                {
                    result.message = L["RequestSuccess"];
                    result.status = 1;
                    result.data = urlData.data;
                }
                else
                {
                    result.message = L["rnoOrfileNotExist"];
                    result.status = 2;
                    result.data = null;
                }
            }
            else
            {
                result.status = 2;
                result.message = L["BPMApiRequestException"] + L["StatusCode"] + response.StatusCode;
            }
            return result;
        }
        //预支单号模糊查询api（未完成冲账）
        public async Task<Result<List<string>>> FuzzyQueryAdvRno(string word, string company)
        {
            Result<List<string>> results = new();
            var amountquery = (await _CashAmountRepository.WithDetailsAsync()).Where(q => q.formcode == "CASH_3" && q.actamt > 0 && q.company == company).Select(x => new { x.rno, x.actamt, x.company }).AsNoTracking().ToList();
            var bdcashquery = (await _bdcashreturnRepository.WithDetailsAsync()).Where(w => w.company == company && amountquery.Select(i => i.rno).Contains(w.rno)).Select(x => new { x.rno, x.amount }).AsNoTracking().ToList().GroupBy(i => i.rno).Select(i => new { rno = i.Key, amount = i.Sum(k => k.amount) });
            var cashHeadQuery = (await _CashHeadRepository.WithDetailsAsync()).Where(w => w.formcode == "CASH_3" && w.status == "approved").ToList();
            var aquery = (from aq in amountquery
                          join bcq in bdcashquery
                          on aq.rno equals bcq.rno
                          where aq.actamt == bcq.amount
                          select new
                          {
                              rno = aq.rno
                          }).ToList();
            var arnoquery = amountquery.Select(x => x.rno).ToList();
            var query = amountquery.Where(x => !aquery.Any(a => x.rno == a.rno)).ToList();
            var check = (from q in query
                         join e in cashHeadQuery
                         on q.rno equals e.rno
                         select new
                         {
                             rno = q.rno,
                             actamt = q.actamt,
                             company = q.company
                         }).Distinct().ToList();
            List<string> result = check.WhereIf(!string.IsNullOrEmpty(word), (x => x.rno.Contains(word) && x.company == company)).Select(x => x.rno).ToList();
            results.data = result;
            return results;
        }
        //变更主管为申请人
        // public async Task<Result<string>> ChangeApplicant(string emplid, decimal amount, string company)
        // {
        //     if(await IsChangeApplicant(emplid,amount,company) == true)
        //     {
        //     }
        // }
        //判断是否变更主管为申请人
        public async Task<Result<bool>> IsChangeApplicant(string emplid, decimal amount, string company)
        {
            Result<bool> result = new();
            var configQuery = (await _AppConfigRepository.WithDetailsAsync()).Where(w => w.company == company && w.key == "advance").AsNoTracking().FirstOrDefault();
            if (configQuery != null && !string.IsNullOrEmpty(configQuery.value))
            {
                decimal tryamount = new decimal();
                if (decimal.TryParse(configQuery.value, out tryamount))
                {
                    if (amount > tryamount)
                    {
                        var levelQueyry = (await _EmpOrgRepository.WithDetailsAsync()).Where(w => w.manager_id == emplid && w.tree_level_num <= 7).AsNoTracking().FirstOrDefault();
                        if (levelQueyry == null)
                        {
                            result.data = true;
                            result.message = String.Format(L["ChangeApplicant"], tryamount.ToString());
                            return result;
                        }
                    }
                }
            }
            result.data = false;
            return result;
        }
        async Task<EmpOrg> GetEmpOrgManager(string deptid)
        {
            EmpOrg empOrg = await (await _EmpOrgRepository.WithDetailsAsync()).Where(i => i.deptid == deptid).AsNoTracking().FirstOrDefaultAsync();
            if (empOrg != null)
            {
                if (empOrg.tree_level_num > 7)
                {
                    empOrg = await GetEmpOrgManager(empOrg.uporg_code_a);
                }
            }
            return empOrg;
        }
        /// <summary>
        /// 冲账（减去预支）
        /// </summary>
        /// <param name="cashDetails"></param>
        /// <returns></returns>
        public async Task<Result<string>> Reversal(List<CashDetail> cashDetails)
        {
            Result<string> result = new();
            List<string> advances = cashDetails.Where(i => !string.IsNullOrEmpty(i.advancerno)).Select(i => i.advancerno).ToList();
            if (advances.Count == 0) return result;
            IList<CashAmount> cashAmounts = await _CashAmountRepository.GetRnoAll(advances);
            foreach (CashAmount cashAmount in cashAmounts)
            {
                if (cashAmount.actamt == 0)
                {
                    result.status = 2;
                    result.message = String.Format(L["reversalError"], cashAmount.rno);
                    return result;
                }
                decimal act = Math.Round(Convert.ToDecimal(cashDetails.Where(i => i.advancerno == cashAmount.rno).Select(i => i.amount).First()), 2, MidpointRounding.AwayFromZero);
                cashAmount.actamt = act - cashAmount.actamt >= 0 ? 0 : cashAmount.actamt - act;
            }
            return result;
        }
        /// <summary>
        /// 回扣（加回预支）
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        public async Task Kickback(string rno)
        {
            List<CashDetail> cashDetails = await (await _CashDetailRepository.WithDetailsAsync()).Where(i => i.rno == rno).AsNoTracking().ToListAsync();
            List<string> advances = cashDetails.Where(i => !string.IsNullOrEmpty(i.advancerno)).Select(i => i.advancerno).ToList();
            if (advances.Count == 0) return;
            IList<CashAmount> cashAmounts = await _CashAmountRepository.GetRnoAll(advances);
            foreach (CashAmount cashAmount in cashAmounts)
                cashAmount.actamt += Convert.ToDecimal(cashDetails.Where(i => i.advancerno == cashAmount.rno).Select(i => i.amount).First());
        }
    }
}

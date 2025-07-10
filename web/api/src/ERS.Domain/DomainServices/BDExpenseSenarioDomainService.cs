using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using ERS.Application.Contracts.DTO.EmployeeInfo;
using ERS.DTO;
using ERS.DTO.BDExp;
using ERS.DTO.BDExpenseSenario;
using ERS.DTO.DataDictionary;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;


namespace ERS.DomainServices
{
    public class BDExpenseSenarioDomainService : CommonDomainService, IBDExpenseSenarioDomainService
    {
        private IRepository<BDExpenseSenario, Guid> _bdExpSenarioRepository;
        private IRepository<BdAccount, Guid> _bdAccountRepository;
        private IBdAccountRepository _BdAccountRepository;
        private IBDExpenseRepository _BdExpenseRepository;
        private IDataDictionaryDomainService _DataDictionaryDomainService;
        private IBDExpenseSenarioRepository _BdExpenseSenarioRepository;
        private IAppConfigRepository _AppConfigRepository;
        private IObjectMapper _ObjectMapper;
        private IExpenseSenarioExtraStepsRepository _ExpenseSenarioExtraStepsRepository;
        //private readonly IBDExpenseSenarioRepository _bdSenarioRepository;
        public BDExpenseSenarioDomainService(
            IRepository<BDExpenseSenario, Guid> bdExpRepository,
            IRepository<BdAccount, Guid> bdAccountRepository,
            IDataDictionaryDomainService iDataDictionaryDomainService,
            IBDExpenseSenarioRepository _bdExpenseSenarioRepository,
            IAppConfigRepository AppConfigRepository,
            IBdAccountRepository BdAccountRepository,
            IObjectMapper ObjectMapper,
            IExpenseSenarioExtraStepsRepository iExpenseSenarioExtraStepsRepository,
            IBDExpenseRepository bdExpenseRepository)
        {
            _bdExpSenarioRepository = bdExpRepository;
            _bdAccountRepository = bdAccountRepository;
            _BdExpenseSenarioRepository = _bdExpenseSenarioRepository;
            _AppConfigRepository = AppConfigRepository;
            _BdAccountRepository = BdAccountRepository;
            _ObjectMapper = ObjectMapper;
            _ExpenseSenarioExtraStepsRepository = iExpenseSenarioExtraStepsRepository;
            _BdExpenseRepository = bdExpenseRepository;
            _DataDictionaryDomainService = iDataDictionaryDomainService;
        }

        public async Task<ExpAcctDto> GetExpAcct(string expcode, string companycategory)
        {
            ExpAcctDto data = await (from a in (await _bdExpSenarioRepository.WithDetailsAsync())
                                     join b in (await _bdAccountRepository.WithDetailsAsync()) on a.accountcode equals b.acctcode
                                     where a.companycategory == b.company && a.companycategory == companycategory && a.expensecode == expcode
                                     select new ExpAcctDto
                                     {
                                         expcode = a.expensecode,
                                         //expname = a.expname,
                                         acctcode = a.accountcode,
                                         acctname = b.acctname
                                     }).AsNoTracking().FirstOrDefaultAsync();
            return data;
        }

        public async Task<Result<IEnumerable<BDExpDto>>> GetGeneralCostExp(string companycategory)
        {
            Result<IEnumerable<BDExpDto>> result = new();
            IEnumerable<BDExpDto> data = (await (from a in (await _bdExpSenarioRepository.WithDetailsAsync())
                                                 join b in (await _BdAccountRepository.WithDetailsAsync())
                                                 on a.accountcode equals b.acctcode
                                                 where a.companycategory == companycategory
                                                 //&& a.category == 1 
                                                 //&& a.type != "1"
                                                 //&& !a.expname.Contains("交際費")
                                                 //&& (a.expcategory == 0 || a.expcategory == 1 || a.expcategory == 2)
                                                 select new BDExpDto
                                                 {
                                                     expcode = a.expensecode,
                                                     //expname = a.expname,
                                                     //description = a.description,
                                                     keyword = a.keyword,
                                                     //filecategory = a.filecategory,
                                                     //isupload = a.isupload,
                                                     //filepoints = a.filepoints,
                                                     //isinvoice = a.isinvoice,
                                                     //expcategory = a.expcategory,
                                                     acctcode = a.accountcode,
                                                     acctname = b.acctname
                                                 }).AsNoTracking().ToListAsync()).Distinct(new ExpDtoComparer()).ToList();
            foreach (var item in data)
            {
                string keywordText = !String.IsNullOrEmpty(item.keyword) ? (ChineseConverter.Convert(item.keyword, ChineseConversionDirection.TraditionalToSimplified) +
                ChineseConverter.Convert(item.keyword, ChineseConversionDirection.SimplifiedToTraditional)) : String.Empty;
                string descriptionText = !String.IsNullOrEmpty(item.description) ? ChineseConverter.Convert(item.description, ChineseConversionDirection.TraditionalToSimplified) +
                ChineseConverter.Convert(item.description, ChineseConversionDirection.SimplifiedToTraditional) : String.Empty;
                item.keyword = keywordText + descriptionText;
            }
            result.data = data;
            return result;
        }
        public async Task<Result<IEnumerable<BDExpDto>>> GetReturnTaiwanExp(string companycategory)
        {
            Result<IEnumerable<BDExpDto>> result = new();
            IEnumerable<BDExpDto> data = await (from a in (await _bdExpSenarioRepository.WithDetailsAsync())
                                                join b in (await _bdAccountRepository.WithDetailsAsync())
                                                on a.accountcode equals b.acctcode
                                                where a.companycategory == companycategory
                                                //&& a.expcategory == 3 && a.category == 1 
                                                select new BDExpDto
                                                {
                                                    expcode = a.expensecode,
                                                    //expname = a.expname,
                                                    //description = a.description,
                                                    keyword = a.keyword,
                                                    //filecategory = a.filecategory,
                                                    //isupload = a.isupload,
                                                    //filepoints = a.filepoints,
                                                    //isinvoice = a.isinvoice,
                                                    //expcategory = a.expcategory,
                                                    acctcode = a.accountcode,
                                                    acctname = b.acctname,
                                                    company = a.companycategory,
                                                    category = a.category
                                                }).GroupBy(w => new { w.company, w.expcode }).Select(g => g.FirstOrDefault()).AsNoTracking().ToListAsync();
            foreach (var item in data)
            {
                string keywordText = !String.IsNullOrEmpty(item.keyword) ? (ChineseConverter.Convert(item.keyword, ChineseConversionDirection.TraditionalToSimplified) +
                ChineseConverter.Convert(item.keyword, ChineseConversionDirection.SimplifiedToTraditional)) : String.Empty;
                string descriptionText = !String.IsNullOrEmpty(item.description) ? ChineseConverter.Convert(item.description, ChineseConversionDirection.TraditionalToSimplified) +
                ChineseConverter.Convert(item.description, ChineseConversionDirection.SimplifiedToTraditional) : String.Empty;
                item.keyword = keywordText + descriptionText;
            }
            result.data = data;
            result.total = data.Count();
            return result;
        }
        public async Task<string> GetSignDType(string company, IList<string> codes, IList<string> acctcodes)
        {
            List<string> auditLevelCode = new List<string>();
            string classNo = string.Empty;
            List<BDExpenseSenario> expenseSenarios = await _BdExpenseSenarioRepository.GetBdExps(company, codes, acctcodes);
            foreach (BDExpenseSenario item in expenseSenarios)
            {
                auditLevelCode.Add(item.auditlevelcode);
            }
            AppConfig Configs = await _AppConfigRepository.GetValue("Class_No");
            string[] seq = Configs.value.Trim().Split('/');
            foreach (string item in seq)
            {
                if (auditLevelCode.Exists(b => b.Equals(item)))
                {
                    classNo = item;
                    break;
                }
            }
            return classNo;
        }
        //根据公司别获取会计科目
        public async Task<Result<List<string>>> GetAccountantSubject(string company)
        {
            Result<List<string>> result = new();
            result.data = (await _BdAccountRepository.GetAcctcodeByCompany(company));
            return result;
        }

        /// <summary>
        /// 獲取費用類別清單
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<Result<List<BDExpDto>>> GetExpenseCodes()
        {
            Result<List<BDExpDto>> result = new();
            List<BDExpense> query = (await _BdExpenseRepository.GetExpenseCodes()).OrderBy(x => x.ExpenseName).ToList();

            List<BDExpDto> bdExps = query.Select(x => new BDExpDto
            {
                company = x.company,
                expcode = x.ExpenseCode,
                expname = x.ExpenseName,
                expnamezhcn = x.ExpenseNameZhcn,
                expnamezhtw = x.ExpenseNameZhtw,
                expnamevn = x.ExpenseNameVn,
                expnamees = x.ExpenseNameEs,
                expnamecz = x.ExpenseNameCz,
                description = x.ExpenseDescription
            }).ToList();

            result.data = bdExps;
            result.total = bdExps.Count;
            return result;
        }

        //获取ClassNo
        public async Task<Result<List<string>>> GetClassNo()
        {
            Result<List<string>> result = new();
            List<string> query = (await _AppConfigRepository.GetClassNoList());
            result.data = query;
            return result;
        }

        /// <summary>
        /// 報銷情景查詢
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result<List<BDExpFormDto>>> GetPageExpenseCategory(Request<BDExpParamDto> request)
        {
            Result<List<BDExpFormDto>> result = new Result<List<BDExpFormDto>>()
            {
                data = new List<BDExpFormDto>()
            };
            if (request == null || request.data == null || request.data.companyList.Count == 0)
            {
                result.message = "illegal request";
                result.status = 2;
                return result;
            }

            List<BDExpenseSenario> query = (await _BdExpenseSenarioRepository.WithDetailsAsync())
                        .OrderBy(w => w.expensecode)
                        .WhereIf(!request.data.companyList.IsNullOrEmpty(), w => request.data.companyList.Contains(w.companycategory))
                        .WhereIf(!string.IsNullOrEmpty(request.data.expcode), w => w.expensecode == request.data.expcode)
                        .WhereIf(!string.IsNullOrEmpty(request.data.senarioname), w => w.senarioname.ToUpper().Contains(request.data.senarioname.ToUpper()))
                        .ToList();
            //List<BDExpFormDto> bdExps = _ObjectMapper.Map<List<BDExpenseSenario>, List<BDExpFormDto>>(query);

            //根据ExpenseCode匹配出ExpenseName
            //var bdExpenses = await _BdExpenseRepository.WithDetailsAsync();
            //Result<List<QueryDataDictionaryDto>> senarioCategoryList = await _DataDictionaryDomainService.GetDictionaryByCategory(ERSConsts.DataDictionaryEnum.SenarioCategory.ToValue(), null);

            List<BDExpFormDto> bdExpenseSenarios = query.Select(x => new BDExpFormDto
            {
                Id = x.Id,
                companycategory = x.companycategory,
                category = x.category,
                senarioname = x.senarioname,
                keyword = x.keyword,
                expcode = x.expensecode,
                acctcode = x.accountcode,
                auditlevelcode = x.auditlevelcode,
                descriptionnotice = x.descriptionnotice,
                attachmentnotice = x.attachmentnotice,
                attachmentname = x.attachmentname,
                requirespaperattachment = x.requirespaperattachment,
                requiresinvoice = x.requiresinvoice,
                isvatdeductable = x.isvatdeductable,
                canbypassfinanceapproval = x.canbypassfinanceapproval,
                requiresattachment = x.requiresattachment,
                extraformcode = x.extraformcode,
                assignment = x.assignment,
                costcenter = x.costcenter,
                pjcode = x.projectcode,
                sdate = x.sdate,
                edate = x.edate,
                sectionday = x.sectionday,
                departday = x.departday,
                calmethod = x.calmethod,
                datelevel = x.datelevel,
                authorizer = x.authorizer,
                authorized = x.authorized,
            }).ToList();

            // 預加載所有可能的 ExpenseSenarioExtraSteps 並進行內存分組 desc
            var allStepsByBdexpId = (await (await _ExpenseSenarioExtraStepsRepository.WithDetailsAsync()).AsNoTracking().ToListAsync())
                                                        .GroupBy(w => w.bdexp_id)
                                                        .ToDictionary(g => g.Key, g => g.ToList());
            //System.Console.WriteLine(allAdditionalApprovalStepsByBdexpId);

            // get and add additional approval steps
            foreach (var bdExpenseSenario in bdExpenseSenarios)
            {
                List<AssignStep> assignSteps = new List<AssignStep>();

                // get additional approval steps from allAdditionalApprovalStepsByBdexpId
                if (allStepsByBdexpId.ContainsKey((Guid)bdExpenseSenario.Id))
                {
                    List<ExpenseSenarioExtraSteps> expenseSenarioExtraStepsList = allStepsByBdexpId.TryGetValue((Guid)bdExpenseSenario.Id, out var value) ? value : null;
                    if (expenseSenarioExtraStepsList != null && expenseSenarioExtraStepsList.Count > 0)
                    {
                        // descendant sort by id
                        expenseSenarioExtraStepsList = expenseSenarioExtraStepsList.OrderByDescending(w => w.Id).ToList();
                        // additionalApprovalSteps = additionalApprovalSteps.OrderBy(w => w.Id).ToList();

                        // group by name and position
                        var extraStepsGroup = expenseSenarioExtraStepsList.GroupBy(w => new { w.name, w.position }).ToDictionary(g => g.Key, g => g.ToList());
                        //System.Console.WriteLine("additionalApprovalStepsGroup: " + additionalApprovalStepsGroup);

                        // get result
                        foreach (var extraStepsGroupItem in extraStepsGroup)
                        {
                            AssignStep assignStep = new AssignStep();
                            assignStep.name = extraStepsGroupItem.Key.name;
                            assignStep.position = extraStepsGroupItem.Key.position;
                            assignStep.approverList = new List<EmployeeInfoDto>();

                            // get approver list
                            foreach (var extraStep in extraStepsGroupItem.Value)
                            {
                                assignStep.approverList.Add(new EmployeeInfoDto
                                {
                                    emplid = extraStep.approver_emplid,
                                    name = extraStep.approver_name,
                                    nameA = extraStep.approver_name_a
                                });
                            }

                            assignSteps.Add(assignStep);
                        }
                    }
                }

                bdExpenseSenario.assignSteps = assignSteps;
            }

            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            int count = bdExpenseSenarios.Count;
            result.data = bdExpenseSenarios.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = count;
            return result;
        }

        /// <summary>
        /// 导出成Excel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Byte[]> GetExpenseCategoryExcel(BDExpParamDto request)
        {
            Byte[] data = null;
            if (request != null)
            {
                List<BDExpenseSenario> query = (await _BdExpenseSenarioRepository.WithDetailsAsync())
                        .OrderBy(w => w.expensecode)
                        .WhereIf(!request.company.IsNullOrEmpty(), w => request.company.Contains(w.companycategory))
                        .WhereIf(!string.IsNullOrEmpty(request.expcode), w => w.expensecode == request.expcode)
                        .WhereIf(!string.IsNullOrEmpty(request.senarioname), w => w.senarioname.ToUpper().Contains(request.senarioname.ToUpper()))
                        .ToList();

                //根据ExpenseCode匹配出ExpenseName
                var bdExpensesCategory = await _BdExpenseRepository.WithDetailsAsync();

                Result<List<QueryDataDictionaryDto>> senarioCategoryList = await _DataDictionaryDomainService.GetDictionaryByCategory(ERSConsts.DataDictionaryEnum.SenarioCategory.ToValue(), null);
                Result<List<QueryDataDictionaryDto>> calmethodList = await _DataDictionaryDomainService.GetDictionaryByCategory(ERSConsts.DataDictionaryEnum.CalMethod.ToValue(), null);
                Result<List<QueryDataDictionaryDto>> extraformcodeList = await _DataDictionaryDomainService.GetDictionaryByCategory(ERSConsts.DataDictionaryEnum.ExtraFormCode.ToValue(), null);

                List<BDExpFormDto> bdExps = query.Select(x => new BDExpFormDto
                {
                    Id = x.Id,
                    companycategory = x.companycategory,
                    category = x.category,
                    senarioname = x.senarioname,
                    keyword = x.keyword,
                    expcode = x.expensecode,
                    acctcode = x.accountcode,
                    auditlevelcode = x.auditlevelcode,
                    descriptionnotice = x.descriptionnotice,
                    attachmentnotice = x.attachmentnotice,
                    attachmentname = x.attachmentname,
                    requirespaperattachment = x.requirespaperattachment,
                    requiresinvoice = x.requiresinvoice,
                    isvatdeductable = x.isvatdeductable,
                    canbypassfinanceapproval = x.canbypassfinanceapproval,
                    requiresattachment = x.requiresattachment,
                    calmethod = x.calmethod,
                    extraformcode = x.extraformcode,
                    assignment = x.assignment,
                    costcenter = x.costcenter,
                    pjcode = x.projectcode,
                    departday = x.departday,
                    sectionday = x.sectionday,
                }).ToList();

                // get whole additional approval steps data
                var allStepsByBdexpId = (await (await _ExpenseSenarioExtraStepsRepository.WithDetailsAsync())
                                                                    .WhereIf(!request.company.IsNullOrEmpty(), w => request.company.Contains(w.company))
                                                                    .WhereIf(!string.IsNullOrEmpty(request.expcode), w => w.exp_code == request.expcode).AsNoTracking().ToListAsync())
                                                            .GroupBy(w => w.bdexp_id)
                                                            .ToDictionary(g => g.Key, g => g.ToList());

                XSSFWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("sheet");
                string[] header = new string[]
                {
                    L["CompanyCode"],//公司别
                    L["BDExp-Excel-Category"],//分类
                    L["ExpensesCategory"],//費用類別
                    L["SenarioName"],//報銷情景
                    L["Keyword"], //關鍵字
                    L["AccountCode"], //會計科目
                    L["AuditLevelCode"], //核決權限代碼
                    L["DescriptionNotice"], //摘要提示
                    L["AttachmentNotice"], //上傳附件提示​
                    L["RequiresPaperAttachment"], //是否需繳交紙本附件
                    L["RequiresInvoice"], //是否需附加憑證​
                    L["CanBypassFinanceApproval"], //是否跳過會計簽核
                    L["RequiresAttachment"], //是否需上傳附件
                    L["IsVatdeductable"], //是否可抵扣稅金
                    L["AddSignStep"],//加簽步驟
                    L["OverdueCalMethod"], //逾期计算方式
                    L["ExtraFormCode"], //报销模块
                    L["DepartDelayDay"],
                    L["SectionDelayDay"],
                    L["BDExp-Excel-Costcenter"],
                    L["BDExp-Excel-Assignment"],
                    L["BDExp-Excel-Projectcode"],
                    L["BDExp-Excel-Datelevel"],
                };

                string test = System.Globalization.CultureInfo.CurrentCulture.Name;
                //标题行
                IRow rowHeader = sheet.CreateRow(0);
                for (int i = 0; i < header.Length; i++)
                {
                    rowHeader.CreateCell(i).SetCellValue(header[i]);
                }
                //數據填充
                for (int i = 0; i < bdExps.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 1);
                    dataRow.CreateCell(0).SetCellValue(bdExps[i].companycategory);
                    //dataRow.CreateCell(1).SetCellValue(bdExps[i].category == 1 ? L["BDExp-Excel-Category-Reimbursement"] : (bdExps[i].category == 2 ? L["BDExp-Excel-Category-Advance"] : (bdExps[i].category == 3 ? L["BDExp-Excel-Category-Salary"] : "")));
                    //分类
                    dataRow.CreateCell(1).SetCellValue(System.Globalization.CultureInfo.CurrentCulture.Name switch
                    {
                        "zh" => senarioCategoryList.data.FirstOrDefault(o => o.Value == bdExps[i].category)?.NameZhtw ?? "",
                        "vn" => senarioCategoryList.data.FirstOrDefault(o => o.Value == bdExps[i].category)?.NameVn ?? "",
                        "es" => senarioCategoryList.data.FirstOrDefault(o => o.Value == bdExps[i].category)?.NameEs ?? "",
                        "cz" => senarioCategoryList.data.FirstOrDefault(o => o.Value == bdExps[i].category)?.NameCz ?? "",
                        _ => senarioCategoryList.data.FirstOrDefault(o => o.Value == bdExps[i].category)?.Name ?? "",
                    });

                    //费用类别
                    dataRow.CreateCell(2).SetCellValue(System.Globalization.CultureInfo.CurrentCulture.Name switch
                    {
                        "zh" => bdExpensesCategory.FirstOrDefault(o => o.ExpenseCode == bdExps[i].expcode)?.ExpenseNameZhtw ?? "",
                        "vn" => bdExpensesCategory.FirstOrDefault(o => o.ExpenseCode == bdExps[i].expcode)?.ExpenseNameVn ?? "",
                        "es" => bdExpensesCategory.FirstOrDefault(o => o.ExpenseCode == bdExps[i].expcode)?.ExpenseNameEs ?? "",
                        "cz" => bdExpensesCategory.FirstOrDefault(o => o.ExpenseCode == bdExps[i].expcode)?.ExpenseNameCz ?? "",
                        _ => bdExpensesCategory.FirstOrDefault(o => o.ExpenseCode == bdExps[i].expcode)?.ExpenseName ?? "",
                    });

                    dataRow.CreateCell(3).SetCellValue(bdExps[i].senarioname);
                    dataRow.CreateCell(4).SetCellValue(bdExps[i].keyword);
                    dataRow.CreateCell(5).SetCellValue(bdExps[i].acctcode);
                    dataRow.CreateCell(6).SetCellValue(bdExps[i].auditlevelcode);
                    dataRow.CreateCell(7).SetCellValue(bdExps[i].descriptionnotice);
                    dataRow.CreateCell(8).SetCellValue(bdExps[i].attachmentnotice);
                    dataRow.CreateCell(9).SetCellValue(bdExps[i].requirespaperattachment ? ERSConsts.Value_Y : ERSConsts.Value_N);
                    dataRow.CreateCell(10).SetCellValue(bdExps[i].requiresinvoice ? ERSConsts.Value_Y : ERSConsts.Value_N);
                    dataRow.CreateCell(11).SetCellValue(bdExps[i].canbypassfinanceapproval ? ERSConsts.Value_Y : ERSConsts.Value_N);
                    dataRow.CreateCell(12).SetCellValue(bdExps[i].requiresattachment ? ERSConsts.Value_Y : ERSConsts.Value_N);
                    dataRow.CreateCell(13).SetCellValue(bdExps[i].isvatdeductable ? ERSConsts.Value_Y : ERSConsts.Value_N);
                    dataRow.CreateCell(14).SetCellValue(CheckAndGetAddSignStepData(bdExps[i].Id, allStepsByBdexpId));

                    dataRow.CreateCell(15).SetCellValue(System.Globalization.CultureInfo.CurrentCulture.Name switch
                    {
                        "zh" => calmethodList.data.FirstOrDefault(o => o.Value == bdExps[i].calmethod.ToString())?.NameZhtw ?? "",
                        "vn" => calmethodList.data.FirstOrDefault(o => o.Value == bdExps[i].calmethod.ToString())?.NameVn ?? "",
                        "es" => calmethodList.data.FirstOrDefault(o => o.Value == bdExps[i].calmethod.ToString())?.NameEs ?? "",
                        "cz" => calmethodList.data.FirstOrDefault(o => o.Value == bdExps[i].calmethod.ToString())?.NameCz ?? "",
                        _ => calmethodList.data.FirstOrDefault(o => o.Value == bdExps[i].calmethod.ToString())?.Name ?? "",
                    });
                    dataRow.CreateCell(16).SetCellValue(System.Globalization.CultureInfo.CurrentCulture.Name switch
                    {
                        "zh" => extraformcodeList.data.FirstOrDefault(o => o.Value == bdExps[i].extraformcode.ToString())?.NameZhtw ?? "",
                        "vn" => extraformcodeList.data.FirstOrDefault(o => o.Value == bdExps[i].extraformcode.ToString())?.NameVn ?? "",
                        "es" => extraformcodeList.data.FirstOrDefault(o => o.Value == bdExps[i].extraformcode.ToString())?.NameEs ?? "",
                        "cz" => extraformcodeList.data.FirstOrDefault(o => o.Value == bdExps[i].extraformcode.ToString())?.NameCz ?? "",
                        _ => extraformcodeList.data.FirstOrDefault(o => o.Value == bdExps[i].extraformcode.ToString())?.Name ?? "",
                    });
                    //dataRow.CreateCell(15).SetCellValue(bdExps[i].calmethod == 0 ? "" : (bdExps[i].calmethod == 1 ? L["BDExp-Excel-DateOfAdvancePayment"] : L["BDExp-Excel-ExpectedDateOfCharge"]));//逾期计算方式
                    //dataRow.CreateCell(16).SetCellValue(bdExps[i].extraformcode == 0 ? "" : (bdExps[i].extraformcode == 1 ? L["BDExp-Excel-OvertimeMealAllowance"] : (bdExps[i].extraformcode == 2 ? L["BDExp-Excel-CostOfDriving"] : L["BDExp-Excel-ReturnTaiWanMeeting"])));

                    dataRow.CreateCell(17).SetCellValue(bdExps[i].departday);
                    dataRow.CreateCell(18).SetCellValue(bdExps[i].sectionday);
                    dataRow.CreateCell(19).SetCellValue(bdExps[i].costcenter);
                    dataRow.CreateCell(20).SetCellValue(bdExps[i].assignment);
                    dataRow.CreateCell(21).SetCellValue(bdExps[i].pjcode);
                    dataRow.CreateCell(22).SetCellValue(bdExps[i].datelevel);
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    ms.Flush();
                    data = ms.ToArray();
                }
            }
            return data;
        }

        private string CheckAndGetAddSignStepData(Guid? id, Dictionary<Guid, List<ExpenseSenarioExtraSteps>> allStepsByBdexpId)
        {
            string concatData = "";
            // check id is consist allAdditionalApprovalStepsByBdexpId or not
            List<ExpenseSenarioExtraSteps> extraStepsList = allStepsByBdexpId.TryGetValue((Guid)id, out var value) ? value : null;
            if (extraStepsList == null)
            {
                return concatData;
            }

            var extralStepsGroup = extraStepsList.GroupBy(w => new { w.name, w.position }).ToDictionary(g => g.Key, g => g.ToList());
            //System.Console.WriteLine("additionalApprovalStepsGroup: " + extralStepsGroup);

            // get result
            foreach (var extraStepsGroupItem in extralStepsGroup)
            {
                string extraStepLine = "";

                foreach (var extraStep in extraStepsGroupItem.Value)
                {
                    extraStepLine += String.IsNullOrEmpty(extraStep.approver_name_a.Trim()) ? extraStep.approver_name : extraStep.approver_name_a;
                    extraStepLine += ",";
                }

                extraStepLine = extraStepLine.TrimEnd(',');
                extraStepLine = extraStepsGroupItem.Key.name + "-" + extraStepLine + "-" + extraStepsGroupItem.Key.position;
                extraStepLine += ";";
                concatData += extraStepLine;
            }

            concatData = concatData.TrimEnd(';');
            return concatData;
        }

        /// <summary>
        /// 返回费用类别Excel数据-废弃
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result<ExcelDto<BDExpExcelDto>>> GetBDExpExcelData(Request<BDExpParamDto> request)
        {
            Result<ExcelDto<BDExpExcelDto>> result = new Result<ExcelDto<BDExpExcelDto>>()
            {
            };
            ExcelDto<BDExpExcelDto> excelDto = new ExcelDto<BDExpExcelDto>();
            if (request != null)
            {
                List<BDExpenseSenario> query = (await _BdExpenseSenarioRepository.WithDetailsAsync())
                        .OrderBy(w => w.expensecode)
                        .WhereIf(!request.data.company.IsNullOrEmpty(), w => request.data.company.Contains(w.companycategory))
                        .WhereIf(!string.IsNullOrEmpty(request.data.expcode), w => w.expensecode == request.data.expcode)
                        .WhereIf(!string.IsNullOrEmpty(request.data.senarioname), w => w.senarioname.ToUpper().Contains(request.data.senarioname.ToUpper()))
                        .ToList();
                List<BDExpExcelDto> bdExps = _ObjectMapper.Map<List<BDExpenseSenario>, List<BDExpExcelDto>>(query);
                string[] header = new string[]
                {
                    "#",
                    L["CompanyCode"],
                    L["ExpensesCategory"],
                    L["SenarioName"],
                    L["Keyword"],
                    L["AccountCode"],
                    L["AuditLevelCode"],
                    L["DescriptionNotice"],
                    L["AttachmentNotice"],
                    L["RequiresPaperAttachment"],
                    L["RequiresInvoice"],
                    L["CanBypassFinanceApproval"],
                    L["RequiresAttachment"],
                    L["IsVatdeductable"],
                    L["AddSign"],
                    L["AddSignStep"],
                    L["OverdueCalMethod"],
                    L["DepartDelayDay"],
                    L["SectionDelayDay"],
                    L["ExtraFormCode"],
                    L["BDExp-Excel-Category"],
                    L["BDExp-Excel-Assignment"],
                    L["BDExp-Excel-Costcenter"],
                    L["BDExp-Excel-Projectcode"],
                    L["BDExp-Excel-Datelevel"]
                };
                excelDto.header = header;
                excelDto.body = bdExps;
                result.data = excelDto;
            }
            return result;
        }
        //新增費用類別
        public async Task<Result<string>> AddExpenseCategory(AddBDExpDto request, string userId)
        {
            Result<string> result = new Result<string>();
            if (string.IsNullOrEmpty(request.companycategory))
            {
                result.message = L["AddFail"] + "：" + L["CompanyEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.expcode))
            {
                result.message = L["AddFail"] + "：" + L["ExpenseCodeEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.senarioname))
            {
                result.message = L["AddFail"] + "：" + L["ExpenseSenarioNameEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.acctcode))
            {
                result.message = L["AddFail"] + "：" + L["AccountCodeEmpty"];
                result.status = 2;
                return result;
            }

            // 检查是否存在相同 company、senarioname、expcode、category 但不同 Id 的记录，防止重复
            var querychk = (await _BdExpenseSenarioRepository.WithDetailsAsync()).Where(w => w.expensecode == request.expcode
                                    && w.companycategory == request.companycategory && w.senarioname == request.senarioname
                                    && w.category == request.category).AsNoTracking().ToList();
            if (querychk.Count > 0)
            {
                result.message = L["AddFail"] + "：" + L["ExpenseSenarioDuplicate"];
                result.status = 2;
                return result;
            }
            else
            {
                //BDExpenseSenario bdExpenseSenario = _ObjectMapper.Map<AddBDExpDto, BDExpenseSenario>(request);
                BDExpenseSenario bdExpenseSenario = new BDExpenseSenario();
                bdExpenseSenario.companycategory = request.companycategory;
                bdExpenseSenario.category = request.category;
                bdExpenseSenario.senarioname = request.senarioname;
                bdExpenseSenario.keyword = request.keyword;
                bdExpenseSenario.expensecode = request.expcode;
                bdExpenseSenario.accountcode = request.acctcode;
                bdExpenseSenario.auditlevelcode = request.auditlevelcode;
                bdExpenseSenario.descriptionnotice = request.descriptionnotice;
                bdExpenseSenario.attachmentnotice = request.attachmentnotice;
                bdExpenseSenario.requirespaperattachment = request.requirespaperattachment.Equals(ERSConsts.Value_Y) ? true : false;
                bdExpenseSenario.requiresinvoice = request.requiresinvoice.Equals(ERSConsts.Value_Y) ? true : false;
                bdExpenseSenario.requiresattachment = request.requiresattachment.Equals(ERSConsts.Value_Y) ? true : false;
                bdExpenseSenario.attachmentname = request.attachmentname;
                bdExpenseSenario.isvatdeductable = request.isvatdeductable.Equals(ERSConsts.Value_Y) ? true : false;
                bdExpenseSenario.canbypassfinanceapproval = request.canbypassfinanceapproval.Equals(ERSConsts.Value_Y) ? true : false;
                bdExpenseSenario.extraformcode = request.extraformcode;
                bdExpenseSenario.assignment = request.assignment;
                bdExpenseSenario.costcenter = request.costcenter;
                bdExpenseSenario.projectcode = request.pjcode;
                bdExpenseSenario.calmethod = request.calmethod;
                bdExpenseSenario.departday = request.departday;
                bdExpenseSenario.sectionday = request.sectionday;
                bdExpenseSenario.datelevel = request.datelevel;
                bdExpenseSenario.authorized = request.authorized;
                bdExpenseSenario.authorizer = request.authorizer;
                bdExpenseSenario.edate = request.edate;
                bdExpenseSenario.sdate = request.sdate;

                bdExpenseSenario.cuser = userId;
                bdExpenseSenario.cdate = System.DateTime.Now;
                var bdExpEntity = await _BdExpenseSenarioRepository.InsertAsync(bdExpenseSenario);

                // add data into additional_approval_steps need to forloop to insert each approver
                if (request.assignSteps != null && request.assignSteps.Count > 0)
                {
                    List<ExpenseSenarioExtraSteps> extraStepsList = new List<ExpenseSenarioExtraSteps>();

                    foreach (var assignStep in request.assignSteps)
                    {
                        foreach (var approver in assignStep.approverList)
                        {
                            ExpenseSenarioExtraSteps extralStep = new ExpenseSenarioExtraSteps();
                            extralStep.exp_code = request.expcode;
                            extralStep.name = assignStep.name;
                            extralStep.position = assignStep.position;
                            extralStep.approver_emplid = approver.emplid;
                            extralStep.approver_name = approver.name;
                            extralStep.approver_name_a = approver.nameA;
                            extralStep.bdexp_id = bdExpEntity.Id;

                            extralStep.cuser = userId;
                            extralStep.cdate = System.DateTime.Now;
                            extralStep.muser = userId;
                            extralStep.mdate = System.DateTime.Now;
                            extralStep.company = request.companycategory;
                            extraStepsList.Add(extralStep);

                        }
                    }

                    await _ExpenseSenarioExtraStepsRepository.InsertManyAsync(extraStepsList);
                }

                result.message = L["AddSuccess"];
            }
            return result;
        }

        /// <summary>
        /// 编辑费用情景（费用类别）
        /// </summary>
        /// <param name="request">编辑费用情景的请求参数</param>
        /// <param name="userId">当前操作用户ID</param>
        /// <returns>操作结果，包含成功或失败信息</returns>
        public async Task<Result<string>> EditExpenseCategory(EditBDExpDto request, string userId)
        {
            Result<string> result = new Result<string>();
            if (string.IsNullOrEmpty(request.auditlevelcode))
            {
                result.message = L["SaveFailMsg"] + "：" + L["ClassNoEmpty"];
                result.status = 2;
                return result;
            }
            if (string.IsNullOrEmpty(request.acctcode))
            {
                result.message = L["SaveFailMsg"] + "：" + L["AccountCodeEmpty"];
                result.status = 2;
                return result;
            }
            if (!string.IsNullOrEmpty(request.wording))
            {
                if (request.wording.Length > 15)
                {
                    result.message = L["SaveFailMsg"] + "：" + L["WordingCHKLong"];
                    result.status = 2;
                    return result;
                }
            }

            // 检查是否存在相同 company、senarioname、expcode、category 但不同 Id 的记录，防止重复
            var duplicateSenario = (await _BdExpenseSenarioRepository.WithDetailsAsync()).Where(w => w.expensecode == request.expcode
                                    && w.companycategory == request.companycategory && w.senarioname == request.senarioname
                                    && w.category == request.category && w.Id != request.Id).AsNoTracking().ToList();
            if (duplicateSenario.Count > 0)
            {
                result.message = L["SaveFailMsg"] + "：" + L["ExpenseSenarioDuplicate"];
                result.status = 2;
                return result;
            }

            // 获取待编辑的费用情景实体
            BDExpenseSenario bdExpenseSenario = (await _BdExpenseSenarioRepository.GetBDExpById(request.Id));
            if (bdExpenseSenario == null)
            {
                result.message = L["SaveFailMsg"] + "：" + L["ExpenseSenarioNotExist"];
                result.status = 2;
                return result;
            }
            // 更新费用情景实体
            bdExpenseSenario.companycategory = request.companycategory;
            bdExpenseSenario.category = request.category;
            bdExpenseSenario.senarioname = request.senarioname;
            bdExpenseSenario.keyword = request.keyword;
            bdExpenseSenario.expensecode = request.expcode;
            bdExpenseSenario.accountcode = request.acctcode;
            bdExpenseSenario.auditlevelcode = request.auditlevelcode;
            bdExpenseSenario.descriptionnotice = request.descriptionnotice;
            bdExpenseSenario.attachmentnotice = request.attachmentnotice;
            bdExpenseSenario.requirespaperattachment = request.requirespaperattachment.Equals(ERSConsts.Value_Y) ? true : false;
            bdExpenseSenario.requiresinvoice = request.requiresinvoice.Equals(ERSConsts.Value_Y) ? true : false;
            bdExpenseSenario.requiresattachment = request.requiresattachment.Equals(ERSConsts.Value_Y) ? true : false;
            bdExpenseSenario.attachmentname = request.attachmentname;
            bdExpenseSenario.isvatdeductable = request.isvatdeductable.Equals(ERSConsts.Value_Y) ? true : false;
            bdExpenseSenario.canbypassfinanceapproval = request.canbypassfinanceapproval.Equals(ERSConsts.Value_Y) ? true : false;
            bdExpenseSenario.extraformcode = request.extraformcode;
            bdExpenseSenario.assignment = request.assignment;
            bdExpenseSenario.costcenter = request.costcenter;
            bdExpenseSenario.projectcode = request.pjcode;
            bdExpenseSenario.calmethod = request.calmethod;
            bdExpenseSenario.departday = request.departday;
            bdExpenseSenario.sectionday = request.sectionday;
            bdExpenseSenario.datelevel = request.datelevel;
            bdExpenseSenario.authorized = request.authorized;
            bdExpenseSenario.authorizer = request.authorizer;
            bdExpenseSenario.edate = request.edate;
            bdExpenseSenario.sdate = request.sdate;

            await _BdExpenseSenarioRepository.UpdateAsync(bdExpenseSenario);

            // 查找并删除所有相关的加签步骤数据
            List<ExpenseSenarioExtraSteps> data = (await _ExpenseSenarioExtraStepsRepository.WithDetailsAsync()).Where(w => w.bdexp_id == request.Id).AsNoTracking().ToList();
            if (data.Count > 0)
            {
                await _ExpenseSenarioExtraStepsRepository.DeleteManyAsync(data);
            }

            // 添加新的加签步骤数据
            if (request.assignSteps != null && request.assignSteps.Count > 0)
            {
                List<ExpenseSenarioExtraSteps> extraStepsList = new List<ExpenseSenarioExtraSteps>();

                foreach (var assignStep in request.assignSteps)
                {
                    foreach (var approver in assignStep.approverList)
                    {
                        ExpenseSenarioExtraSteps extralStep = new ExpenseSenarioExtraSteps();
                        extralStep.exp_code = request.expcode;
                        extralStep.name = assignStep.name;
                        extralStep.position = assignStep.position;
                        extralStep.approver_emplid = approver.emplid;
                        extralStep.approver_name = approver.name;
                        extralStep.approver_name_a = approver.nameA;
                        extralStep.bdexp_id = (Guid)request.Id;

                        extralStep.cuser = userId;
                        extralStep.cdate = System.DateTime.Now;
                        extralStep.muser = userId;
                        extralStep.mdate = System.DateTime.Now;
                        extralStep.company = request.companycategory;
                        extraStepsList.Add(extralStep);
                    }
                }

                await _ExpenseSenarioExtraStepsRepository.InsertManyAsync(extraStepsList);
            }

            result.message = L["SaveSuccessMsg"];
            return result;
        }

        /// <summary>
        /// 删除费用类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<string>> DeleteExpenseCategory(Guid id)
        {
            Result<string> result = new Result<string>();
            BDExpenseSenario bdExpenseSenario = (await _BdExpenseSenarioRepository.GetBDExpById(id));
            if (bdExpenseSenario == null)
            {
                result.message = L["DeleteFail"] + "：" + L["ExpenseCategoryNotExist"];
                result.status = 2;
                return result;
            }
            await _BdExpenseSenarioRepository.DeleteAsync(bdExpenseSenario);

            //并删除所有相关的加签步骤数据
            List<ExpenseSenarioExtraSteps> data = (await _ExpenseSenarioExtraStepsRepository.WithDetailsAsync()).Where(w => w.bdexp_id == id).AsNoTracking().ToList();
            if (data.Count > 0)
            {
                await _ExpenseSenarioExtraStepsRepository.DeleteManyAsync(data);
            }

            result.message = L["DeleteSuccess"];
            return result;
        }
        public static DataTable GetDataTableFromExcel(IFormFile file, int checkCellNum = 0)
        {
            IWorkbook fileWorkbook = WorkbookFactory.Create(file.OpenReadStream());
            ISheet sheet = fileWorkbook.GetSheetAt(0);
            DataTable dt = new DataTable();
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row.GetCell(checkCellNum) != null)
                {
                    DataRow dataRow = dt.NewRow();
                    for (int k = row.FirstCellNum; k < cellCount; k++)
                    {
                        if (row.GetCell(k) != null)
                        {
                            if (row.GetCell(k).CellType.ToString() == "Numeric" && DateUtil.IsCellDateFormatted(row.GetCell(k)))
                            {
                                dataRow[k] = row.GetCell(k).DateCellValue;
                            }
                            else if (row.GetCell(k).CellType.ToString() == "Numeric")
                            {
                                dataRow[k] = row.GetCell(k).NumericCellValue.ToString();
                            }
                            else if (row.GetCell(k).CellType.ToString() == "Formula")
                            {
                                if (row.GetCell(k).CachedFormulaResultType.ToString() == "Numeric")
                                {
                                    dataRow[k] = row.GetCell(k).NumericCellValue.ToString();
                                }
                                else if (row.GetCell(k).CachedFormulaResultType.ToString() == "String")
                                {
                                    dataRow[k] = row.GetCell(k).StringCellValue;
                                }
                            }
                            else
                            {
                                dataRow[k] = row.GetCell(k).ToString();
                            }
                        }
                    }
                    dt.Rows.Add(dataRow);
                }
            }
            return dt;
        }
        //批量上传费用情景
        public async Task<Result<List<UploadBDExpDto>>> BatchUploadBDExp(IFormFile excelFile, string userId)
        {
            Result<List<UploadBDExpDto>> result = new Result<List<UploadBDExpDto>>()
            {
                data = new List<UploadBDExpDto>()
            };
            if (excelFile.ContentType == "application/vnd.ms-excel" || excelFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                DataTable dt = GetDataTableFromExcel(excelFile);
                var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
                {
                    companycategory = s[0].ToString().Trim(),
                    category = s[1].ToString().Trim(),
                    expcode = s[2].ToString().Trim(),
                    expname = s[3].ToString().Trim(),
                    description = s[4].ToString().Trim(),
                    keyword = s[5].ToString().Trim(),
                    acctcode = s[6].ToString().Trim(),
                    classno = s[7].ToString().Trim(),
                    // isupload = s[8].ToString().Trim(),
                    // filecategory = s[9].ToString().Trim(),
                    filepoints = s[8].ToString().Trim(),
                    isinvoice = s[9].ToString().Trim(),
                    // invoicecategory = s[12].ToString().Trim(),
                    isdeduction = s[10].ToString().Trim(),
                    // addsign = s[14].ToString().Trim(),
                    // addstep = s[15].ToString().Trim(),
                    calmethod = s[11].ToString().Trim(),
                    expcategory = s[12].ToString().Trim(),
                    costcenter = s[13].ToString().Trim(),
                    assignment = s[14].ToString().Trim(),
                    pjcode = s[15].ToString().Trim(),
                    datelevel = s[16].ToString().Trim(),
                    issendremindemail = s[17].ToString().Trim(),
                    expdesc = s[18].ToString().Trim(),
                    descriptionnotice = s[19].ToString().Trim(),
                }).ToList();
                if (list.Count > 0)
                {
                    List<AddBDExpDto> addList = new List<AddBDExpDto>();
                    foreach (var item in list)
                    {
                        AddBDExpDto addBDExpDto = new AddBDExpDto();
                        addBDExpDto.companycategory = item.companycategory;
                        addBDExpDto.category = item.category;
                        addBDExpDto.expcode = item.expcode;
                        addBDExpDto.expname = item.expname;
                        //addBDExpDto.description = item.description;
                        addBDExpDto.keyword = item.keyword;
                        addBDExpDto.acctcode = item.acctcode;
                        //addBDExpDto.classno = item.classno;
                        //addBDExpDto.filepoints = item.filepoints;
                        //addBDExpDto.isinvoice = item.isinvoice;
                        addBDExpDto.calmethod = Convert.ToInt32(item.calmethod);
                        //addBDExpDto.expcategory = Convert.ToInt32(item.expcategory);
                        addBDExpDto.costcenter = item.costcenter;
                        addBDExpDto.assignment = item.assignment;
                        addBDExpDto.pjcode = item.pjcode;
                        addBDExpDto.datelevel = item.datelevel;
                        addBDExpDto.isdeduction = item.isdeduction;
                        //addBDExpDto.issendremindemail = item.issendremindemail;
                        //addBDExpDto.expdesc = item.expdesc;
                        addBDExpDto.descriptionnotice = item.descriptionnotice;
                        addList.Add(addBDExpDto);
                    }
                    var checkResults = await AddBDExpCheck(addList);
                    for (int i = 0; i < checkResults.Count; i++)
                    {
                        UploadBDExpDto uploadBDExpDto = new UploadBDExpDto();
                        uploadBDExpDto.data = checkResults[i].data;
                        uploadBDExpDto.status = checkResults[i].status;
                        uploadBDExpDto.uploadmsg = checkResults[i].message;
                        uploadBDExpDto.seq = i + 1;
                        result.data.Add(uploadBDExpDto);
                    }
                    //添加
                    List<BDExpenseSenario> iBDExpenseSenarios = _ObjectMapper.Map<List<AddBDExpDto>, List<BDExpenseSenario>>(checkResults.Where(w => w.status == 1).Select(w => w.data).ToList());
                    iBDExpenseSenarios.ForEach(w =>
                    {
                        w.cuser = userId;
                        w.cdate = System.DateTime.Now;
                    });
                    await _BdExpenseSenarioRepository.InsertManyAsync(iBDExpenseSenarios);
                }
            }
            else
            {
                result.message = L["BDExp-UploadErrorFile"];
                result.status = 2;
            }
            return result;
        }
        //添加费用情景检查
        public async Task<List<Result<AddBDExpDto>>> AddBDExpCheck(List<AddBDExpDto> addBDExpDtos)
        {
            List<Result<AddBDExpDto>> result = new List<Result<AddBDExpDto>>();
            List<AddBDExpDto> tempList = new List<AddBDExpDto>();
            foreach (var item in addBDExpDtos)
            {
                Result<AddBDExpDto> check = new Result<AddBDExpDto>()
                {
                    data = new AddBDExpDto()
                };
                check.data = item;
                result.Add(check);
                if (string.IsNullOrEmpty(item.companycategory))
                {
                    check.message = L["AddFail"] + "：" + L["CompanyEmpty"];
                    check.status = 2;
                    continue;
                }
                if (string.IsNullOrEmpty(item.expcode))
                {
                    check.message = L["AddFail"] + "：" + L["ExpenseCodeEmpty"];
                    check.status = 2;
                    continue;
                }
                if (string.IsNullOrEmpty(item.expname))
                {
                    check.message = L["AddFail"] + "：" + L["ExpenseNameEmpty"];
                    check.status = 2;
                    continue;
                }
                if (string.IsNullOrEmpty(item.auditlevelcode))
                {
                    check.message = L["AddFail"] + "：" + L["ClassNoEmpty"];
                    check.status = 2;
                    continue;
                }
                if (string.IsNullOrEmpty(item.acctcode))
                {
                    check.message = L["AddFail"] + "：" + L["AccountCodeEmpty"];
                    check.status = 2;
                    continue;
                }
                //if (string.IsNullOrEmpty(item.description))
                //{
                //    check.message = L["AddFail"] + "：" + L["DescriptionEmpty"];
                //    check.status = 2;
                //    continue;
                //}
                //else if (!string.IsNullOrEmpty(item.description))
                //{
                //    if (item.description.Length > 100)
                //    {
                //        check.message = L["AddFail"] + "：" + L["DescriptionCHKLong"];
                //        check.status = 2;
                //        continue;
                //    }
                //}
                if (!string.IsNullOrEmpty(item.wording))
                {
                    if (item.wording.Length > 15)
                    {
                        check.message = L["AddFail"] + "：" + L["WordingCHKLong"];
                        check.status = 2;
                        continue;
                    }
                }
                // if(item.isupload == "Y" && string.IsNullOrEmpty(item.filecategory))
                // {
                //     check.message = L["AddFail"] + "：" + L["FilecategoryError"];
                //     check.status = 2;
                //     continue;
                // }
                var querychk = (await _BdExpenseSenarioRepository.WithDetailsAsync()).Where(w => w.expensecode == item.expcode && w.companycategory == item.companycategory && w.category == item.category).AsNoTracking().ToList();
                if (querychk.Count > 0)
                {
                    check.message = L["AddFail"] + "：" + L["DataAlreadyexist"];
                    check.status = 2;
                    continue;
                }
                //如果传进来的list当中有重复的
                if (tempList.Where(w => w.expcode == item.expcode && w.companycategory == item.companycategory && w.category == item.category).Count() > 0)
                {
                    check.message = L["AddFail"] + "：" + L["BDExp-BatchUpload-Repeat"];
                    check.status = 2;
                    continue;
                }
                tempList.Add(item);
                check.message = L["AddSuccess"];
            }
            return result;
        }
        public async Task<Result<List<BDExpDto>>> GetXZExp(string companycategory)
        {
            Result<List<BDExpDto>> result = new();
            var expQuery = (await _bdExpSenarioRepository.WithDetailsAsync()).Where(w => w.companycategory == companycategory);
            var bdAccountQuery = (await _bdAccountRepository.WithDetailsAsync()).Where(w => w.company == companycategory);
            List<BDExpDto> data = (from a in expQuery
                                   join b in bdAccountQuery
                                   on a.accountcode equals b.acctcode
                                   where a.companycategory == companycategory
                                   && a.category == "payroll"
                                   select new BDExpDto
                                   {
                                       expcode = a.expensecode,
                                       //expname = a.expname,
                                       //description = a.description,
                                       keyword = a.keyword,
                                       //filecategory = a.filecategory,
                                       //isupload = a.isupload,
                                       //filepoints = a.filepoints,
                                       //isinvoice = a.isinvoice,
                                       //expcategory = a.expcategory,
                                       acctcode = a.accountcode,
                                       acctname = b.acctname
                                   }).Distinct().AsNoTracking().ToList();
            result.data = data;
            return result;
        }

        public async Task<BDSenarioDto> GetSenarioById(Guid Id)
        {
            BDSenarioDto bdSenarioDto = (await _BdExpenseSenarioRepository.GetBDSenarioById(Id));
            BdAccount account = (await _BdAccountRepository.GetBdAccount(bdSenarioDto.accountcode, bdSenarioDto.companycategory));
            if (account != null)
            {
                bdSenarioDto.accountname = account.acctname;
            }
            return bdSenarioDto;
        }

        public async Task<List<BDSenarioOptionDto>> SearchSenarioOptionsByKeyword(BDSenarioOptionFilterDto bDSenarioOptionFilterDto)
        {
            List<BDExpenseSenario> bDSenarios = await _BdExpenseSenarioRepository.SearchBDSenariosByKeywordAsync(bDSenarioOptionFilterDto.Companycategory, bDSenarioOptionFilterDto.Keyword);
            List<BDSenarioOptionDto> bDSenarioOptionDtos = _ObjectMapper.Map<List<BDExpenseSenario>, List<BDSenarioOptionDto>>(bDSenarios);
            return bDSenarioOptionDtos;
        }
    }
}

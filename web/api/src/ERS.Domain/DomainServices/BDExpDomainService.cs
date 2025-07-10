using System.IO;
using ERS.DTO;
using ERS.Entities;
using ERS.IDomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using ERS.DTO.BDExp;
using ERS.IRepositories;
using Volo.Abp.ObjectMapping;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using ERS.Application.Contracts.DTO.EmployeeInfo;
namespace ERS.DomainServices
{
    public class BDExpDomainService : CommonDomainService, IBDExpDomainService
    {
        private IRepository<BdExp, Guid> _bdExpRepository;
        private IRepository<BdAccount, Guid> _bdAccountRepository;
        private IBdAccountRepository _BdAccountRepository;
        private IBDExpRepository _BDExpRepository;
        private IAppConfigRepository _AppConfigRepository;
        private IObjectMapper _ObjectMapper;
        private IExpenseSenarioExtraStepsRepository _AdditionalApprovalStepsRepository;
        private readonly IBDExpenseSenarioRepository _bdSenarioRepository;
        public BDExpDomainService(
            IRepository<BdExp, Guid> bdExpRepository,
            IRepository<BdAccount, Guid> bdAccountRepository,
            IBDExpRepository BDExpRepository,
            IAppConfigRepository AppConfigRepository,
            IBdAccountRepository BdAccountRepository,
            IObjectMapper ObjectMapper,
            IExpenseSenarioExtraStepsRepository AdditionalApprovalStepsRepository,
            IBDExpenseSenarioRepository bdSenarioRepository)
        {
            _bdExpRepository = bdExpRepository;
            _bdAccountRepository = bdAccountRepository;
            _BDExpRepository = BDExpRepository;
            _AppConfigRepository = AppConfigRepository;
            _BdAccountRepository = BdAccountRepository;
            _ObjectMapper = ObjectMapper;
            _AdditionalApprovalStepsRepository = AdditionalApprovalStepsRepository;
            _bdSenarioRepository = bdSenarioRepository;
        }
        public async Task<ExpAcctDto> GetExpAcct(string expcode, string company)
        {
            ExpAcctDto data = await (from a in (await _bdExpRepository.WithDetailsAsync())
                                     join b in (await _bdAccountRepository.WithDetailsAsync()) on a.acctcode equals b.acctcode
                                     where a.company == b.company && a.company == company && a.expcode == expcode
                                     select new ExpAcctDto
                                     {
                                         expcode = a.expcode,
                                         expname = a.expname,
                                         acctcode = a.acctcode,
                                         acctname = b.acctname
                                     }).AsNoTracking().FirstOrDefaultAsync();
            return data;
        }
        public async Task<Result<IEnumerable<BDExpDto>>> GetGeneralCostExp(string company)
        {
            Result<IEnumerable<BDExpDto>> result = new();
            IEnumerable<BDExpDto> data = (await (from a in (await _bdExpRepository.WithDetailsAsync())
                                                join b in (await _BdAccountRepository.WithDetailsAsync())
                                                on a.acctcode equals b.acctcode
                                                where a.company == company && a.category == 1
                                                && a.type != "1"
                                                && !a.expname.Contains("交際費")
                                                && (a.expcategory == 0 || a.expcategory == 1 || a.expcategory == 2)
                                                select new BDExpDto
                                                {
                                                    expcode = a.expcode,
                                                    expname = a.expname,
                                                    description = a.description,
                                                    keyword = a.keyword,
                                                    filecategory = a.filecategory,
                                                    isupload = a.isupload,
                                                    filepoints = a.filepoints,
                                                    isinvoice = a.isinvoice,
                                                    expcategory = a.expcategory,
                                                    acctcode = a.acctcode,
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
        public async Task<Result<IEnumerable<BDExpDto>>> GetReturnTaiwanExp(string company)
        {
            Result<IEnumerable<BDExpDto>> result = new();
            IEnumerable<BDExpDto> data = await (from a in (await _bdExpRepository.WithDetailsAsync())
                                                join b in (await _bdAccountRepository.WithDetailsAsync())
                                                on a.acctcode equals b.acctcode
                                                where a.company == company && a.expcategory == 3 && a.category == 1
                                                select new BDExpDto
                                                {
                                                    expcode = a.expcode,
                                                    expname = a.expname,
                                                    description = a.description,
                                                    keyword = a.keyword,
                                                    filecategory = a.filecategory,
                                                    isupload = a.isupload,
                                                    filepoints = a.filepoints,
                                                    isinvoice = a.isinvoice,
                                                    expcategory = a.expcategory,
                                                    acctcode = a.acctcode,
                                                    acctname = b.acctname,
                                                    company = a.company,
                                                    category = a.category.ToString(),
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
            List<string> classNos = new List<string>();
            string classNo = string.Empty;
            List<BdExp> exps = await _BDExpRepository.GetBdExps(company, codes, acctcodes);
            foreach (BdExp item in exps)
            {
                classNos.Add(item.classno);
            }
            AppConfig Configs = await _AppConfigRepository.GetValue("Class_No");
            string[] seq = Configs.value.Trim().Split('/');
            foreach (string item in seq)
            {
                if (classNos.Exists(b => b.Equals(item)))
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
        //获取ClassNo
        public async Task<Result<List<string>>> GetClassNo()
        {
            Result<List<string>> result = new();
            List<string> query = (await _AppConfigRepository.GetClassNoList());
            result.data = query;
            return result;
        }
        //bd06查询
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

            List<BdExp> query = (await _BDExpRepository.WithDetailsAsync())
                        .OrderBy(w => w.expcode)
                        .WhereIf(!request.data.companyList.IsNullOrEmpty(), w => request.data.companyList.Contains(w.company))
                        .WhereIf(!string.IsNullOrEmpty(request.data.expcode), w => w.expcode == request.data.expcode)
                        .WhereIf(!string.IsNullOrEmpty(request.data.acctcode), w => w.acctcode == request.data.acctcode)
                        .ToList();
            List<BDExpFormDto> bdExps = _ObjectMapper.Map<List<BdExp>, List<BDExpFormDto>>(query);

            // 預加載所有可能的 AdditionalApprovalSteps 並進行內存分組 desc
            var allAdditionalApprovalStepsByBdexpId = (await (await _AdditionalApprovalStepsRepository.WithDetailsAsync()).AsNoTracking().ToListAsync())
                                                        .GroupBy(w => w.bdexp_id)
                                                        .ToDictionary(g => g.Key, g => g.ToList());
            System.Console.WriteLine(allAdditionalApprovalStepsByBdexpId);

            // get and add additional approval steps
            foreach (var bdExp in bdExps)
            {
                List<AssignStep> assignSteps = new List<AssignStep>();

                // get additional approval steps from allAdditionalApprovalStepsByBdexpId
                if (allAdditionalApprovalStepsByBdexpId.ContainsKey((Guid) bdExp.Id))
                {
                    List<ExpenseSenarioExtraSteps> additionalApprovalSteps = allAdditionalApprovalStepsByBdexpId.TryGetValue((Guid) bdExp.Id, out var value) ? value : null;
                    if (additionalApprovalSteps != null && additionalApprovalSteps.Count > 0)
                    {
                        // descendant sort by id
                        additionalApprovalSteps = additionalApprovalSteps.OrderByDescending(w => w.Id).ToList();
                        // additionalApprovalSteps = additionalApprovalSteps.OrderBy(w => w.Id).ToList();

                        // group by name and position
                        var additionalApprovalStepsGroup = additionalApprovalSteps.GroupBy(w => new { w.name, w.position }).ToDictionary(g => g.Key, g => g.ToList());
                        System.Console.WriteLine("additionalApprovalStepsGroup: " + additionalApprovalStepsGroup);
                        
                        // get result
                        foreach (var additionalApprovalStepsGroupItem in additionalApprovalStepsGroup)
                        {
                            AssignStep assignStep = new AssignStep();
                            assignStep.name = additionalApprovalStepsGroupItem.Key.name;
                            assignStep.position = additionalApprovalStepsGroupItem.Key.position;
                            assignStep.approverList = new List<EmployeeInfoDto>();

                            // get approver list
                            foreach (var additionalApprovalStep in additionalApprovalStepsGroupItem.Value)
                            {
                                assignStep.approverList.Add(new EmployeeInfoDto
                                {
                                    emplid = additionalApprovalStep.approver_emplid,
                                    name = additionalApprovalStep.approver_name,
                                    nameA = additionalApprovalStep.approver_name_a
                                });
                            }

                            assignSteps.Add(assignStep);
                        }
                    }
                }
                
                bdExp.assignSteps = assignSteps;
            }

            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            int count = bdExps.Count;
            result.data = bdExps.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = count;
            return result;
        }
        //导出成Excel
        public async Task<Byte[]> GetExpenseCategoryExcel(BDExpParamDto request)
        {
            Byte[] data = null;
            if (request != null)
            {
                List<BdExp> query = (await _BDExpRepository.WithDetailsAsync())
                        .OrderBy(w => w.expcode)
                        .WhereIf(!request.company.IsNullOrEmpty(), w => request.company.Contains(w.company))
                        .WhereIf(!string.IsNullOrEmpty(request.expcode), w => w.expcode == request.expcode)
                        .WhereIf(!string.IsNullOrEmpty(request.acctcode), w => w.acctcode == request.acctcode)
                        .ToList();
                List<BDExpFormDto> bdExps = _ObjectMapper.Map<List<BdExp>, List<BDExpFormDto>>(query);
                // get whole additional approval steps data
                var allAdditionalApprovalStepsByBdexpId = (await (await _AdditionalApprovalStepsRepository.WithDetailsAsync())
                                                                    .WhereIf(!request.company.IsNullOrEmpty(), w => request.company.Contains(w.company))
                                                                    .WhereIf(!string.IsNullOrEmpty(request.expcode), w => w.exp_code == request.expcode).AsNoTracking().ToListAsync())
                                                            .GroupBy(w => w.bdexp_id)
                                                            .ToDictionary(g => g.Key, g => g.ToList());
                
                
                XSSFWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("sheet");
                // string[] header = new string[]
                // {
                //     "#",
                //     L["CompanyCode"],
                //     L["ExpensesCategory"],
                //     L["ExpensesName"],
                //     L["BDExp-ExpenseDescrion"],
                //     L["Keyword"],
                //     L["AccountCode"],
                //     L["ClassNO"],
                //     L["IsUpload"],
                //     L["FileCategory"],
                //     L["FilePoints"],
                //     L["InvoiceFlag"],
                //     L["InvoiceCategory"],
                //     L["IsDeduction"],
                //     L["AddSign"],
                //     L["AddSignStep"],
                //     L["OverdueCalMethod"],
                //     L["ExpCategory"],
                //     L["BDExp-Excel-Category"],//分类
                //     L["BDExp-Excel-Assignment"],
                //     L["BDExp-Excel-Costcenter"],
                //     L["BDExp-Excel-Projectcode"]
                // };//L["DepartDelayDay"],L["SectionDelayDay"],
                string[] header = new string[]
                {
                    L["CompanyCode"],//公司别
                    L["BDExp-Excel-Category"],//分类
                    L["ExpenseCode"],//費用代碼
                    L["ExpensesCategory"],//費用類別
                    L["ReimbursementScene"],//報銷情景（情景描述）
                    L["Keyword"],//關鍵字
                    L["AccountCode"],//費用代碼
                    L["AuditLevelCode"],//核決權限代碼
                    // L["IsUpload"],//附件是否必要上傳
                    // L["FileCategory"],//附件類型
                    L["FilePoints"],//附件審核要點說明
                    L["InvoiceFlag"],//是否必要上傳發票
                    // L["InvoiceCategory"],//發票類型
                    L["IsDeduction"],//是否稅金抵扣
                    // L["AddSign"],//加簽工號
                    // L["AddSignStep"],//加簽步驟
                    L["OverdueCalMethod"],//逾期計算方式
                    L["ExtraFormCode"],//報銷模塊
                    L["BDExp-Excel-Costcenter"],
                    L["BDExp-Excel-Assignment"],
                    L["BDExp-Excel-Projectcode"],
                    L["BDExp-Excel-Datelevel"],//预支冲账日期是否卡关（Y/N）
                    L["BDExp-Excel-Expdesc"],//費用描述
                    L["BDExp-Excel-Descriptionnotice"],//摘要提示
                    L["AddSignStep"],//加簽步驟
                };
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
                    //dataRow.CreateCell(1).SetCellValue(bdExps[i].category == 1 ? L["BDExp-Excel-Category-Reimbursement"] : (bdExps[i].category == 2 ? L["BDExp-Excel-Category-Advance"] : (bdExps[i].category == 3 ? L["BDExp-Excel-Category-Salary"] : "" )));
                    dataRow.CreateCell(1).SetCellValue(bdExps[i].category);
                    dataRow.CreateCell(2).SetCellValue(bdExps[i].expcode);
                    dataRow.CreateCell(3).SetCellValue(bdExps[i].expname);
                    //dataRow.CreateCell(4).SetCellValue(bdExps[i].description);
                    dataRow.CreateCell(5).SetCellValue(bdExps[i].keyword);
                    dataRow.CreateCell(6).SetCellValue(bdExps[i].acctcode);
                    dataRow.CreateCell(7).SetCellValue(bdExps[i].auditlevelcode);
                    // dataRow.CreateCell(8).SetCellValue(bdExps[i].isupload);
                    // dataRow.CreateCell(9).SetCellValue(bdExps[i].filecategory);
                    //dataRow.CreateCell(8).SetCellValue(bdExps[i].filepoints);
                    dataRow.CreateCell(9).SetCellValue(bdExps[i].requiresinvoice);
                    // dataRow.CreateCell(12).SetCellValue(bdExps[i].invoicecategory);
                    dataRow.CreateCell(10).SetCellValue(bdExps[i].isvatdeductable);
                    // dataRow.CreateCell(14).SetCellValue(bdExps[i].addsign);
                    // dataRow.CreateCell(15).SetCellValue(bdExps[i].addsignstep);
                    dataRow.CreateCell(11).SetCellValue(bdExps[i].calmethod == 0 ? "" : (bdExps[i].calmethod == 1 ? L["BDExp-Excel-DateOfAdvancePayment"] : L["BDExp-Excel-ExpectedDateOfCharge"]));//逾期计算方式
                    dataRow.CreateCell(12).SetCellValue(bdExps[i].extraformcode == 0 ? "" : (bdExps[i].extraformcode == 1 ? L["BDExp-Excel-OvertimeMealAllowance"] : (bdExps[i].extraformcode == 2 ? L["BDExp-Excel-CostOfDriving"] : L["BDExp-Excel-ReturnTaiWanMeeting"])));
                    dataRow.CreateCell(13).SetCellValue(bdExps[i].costcenter);
                    dataRow.CreateCell(14).SetCellValue(bdExps[i].assignment);
                    dataRow.CreateCell(15).SetCellValue(bdExps[i].pjcode);
                    dataRow.CreateCell(16).SetCellValue(bdExps[i].datelevel);
                    //dataRow.CreateCell(17).SetCellValue(bdExps[i].expdesc);
                    dataRow.CreateCell(18).SetCellValue(bdExps[i].descriptionnotice);
                    dataRow.CreateCell(19).SetCellValue(CheckAndGetAddSignStepData(bdExps[i].Id, allAdditionalApprovalStepsByBdexpId));
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

        private string CheckAndGetAddSignStepData(Guid? id, Dictionary<Guid, List<ExpenseSenarioExtraSteps>> allAdditionalApprovalStepsByBdexpId)
        {
            string concatData = "";
            // check id is consist allAdditionalApprovalStepsByBdexpId or not
            List<ExpenseSenarioExtraSteps> additionalApprovalSteps = allAdditionalApprovalStepsByBdexpId.TryGetValue((Guid) id, out var value) ? value : null;
            if (additionalApprovalSteps == null) {
                return concatData;
            }

            var additionalApprovalStepsGroup = additionalApprovalSteps.GroupBy(w => new { w.name, w.position }).ToDictionary(g => g.Key, g => g.ToList());
            System.Console.WriteLine("additionalApprovalStepsGroup: " + additionalApprovalStepsGroup);
            
            // get result
            foreach (var additionalApprovalStepsGroupItem in additionalApprovalStepsGroup)
            {
                string additionalApprovalStepLine = "";

                foreach (var additionalApprovalStep in additionalApprovalStepsGroupItem.Value)
                {
                    additionalApprovalStepLine += String.IsNullOrEmpty(additionalApprovalStep.approver_name_a.Trim()) ? additionalApprovalStep.approver_name : additionalApprovalStep.approver_name_a;
                    additionalApprovalStepLine += ",";
                }
                
                additionalApprovalStepLine = additionalApprovalStepLine.TrimEnd(',');
                additionalApprovalStepLine = additionalApprovalStepsGroupItem.Key.name + "-" + additionalApprovalStepLine + "-" + additionalApprovalStepsGroupItem.Key.position;
                additionalApprovalStepLine += ";";
                concatData += additionalApprovalStepLine;
            }

            concatData = concatData.TrimEnd(';');
            return concatData;
        }

        //返回费用类别Excel数据
        public async Task<Result<ExcelDto<BDExpExcelDto>>> GetBDExpExcelData(Request<BDExpParamDto> request)
        {
            Result<ExcelDto<BDExpExcelDto>> result = new Result<ExcelDto<BDExpExcelDto>>()
            {
            };
            ExcelDto<BDExpExcelDto> excelDto = new ExcelDto<BDExpExcelDto>();
            if (request != null)
            {
                List<BdExp> query = (await _BDExpRepository.WithDetailsAsync())
                        .OrderBy(w => w.expcode)
                        .WhereIf(!request.data.company.IsNullOrEmpty(), w => request.data.company.Contains(w.company))
                        .WhereIf(!string.IsNullOrEmpty(request.data.expcode), w => w.expcode == request.data.expcode)
                        .WhereIf(!string.IsNullOrEmpty(request.data.acctcode), w => w.acctcode == request.data.acctcode)
                        .ToList();
                List<BDExpExcelDto> bdExps = _ObjectMapper.Map<List<BdExp>, List<BDExpExcelDto>>(query);
                string[] header = new string[]
                {
                    "#",
                    L["CompanyCode"],
                    L["ExpenseCode"],
                    L["ExpensesCategory"],
                    L["Keyword"],
                    L["AccountCode"],
                    L["AuditLevelCode"],
                    L["RequiresAttachment"],
                    L["FileCategory"],
                    L["FilePoints"],
                    L["InvoiceFlag"],
                    L["InvoiceCategory"],
                    L["IsDeduction"],
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
            if (string.IsNullOrEmpty(request.expname))
            {
                result.message = L["AddFail"] + "：" + L["ExpenseNameEmpty"];
                result.status = 2;
                return result;
            }
            //if (string.IsNullOrEmpty(request.classno))
            //{
            //    result.message = L["AddFail"] + "：" + L["ClassNoEmpty"];
            //    result.status = 2;
            //    return result;
            //}
            if (string.IsNullOrEmpty(request.acctcode))
            {
                result.message = L["AddFail"] + "：" + L["AccountCodeEmpty"];
                result.status = 2;
                return result;
            }
            //if (!string.IsNullOrEmpty(request.description))
            //{
            //    if (request.description.Length > 100)
            //    {
            //        result.message = L["AddFail"] + "：" + L["DescriptionCHKLong"];
            //        result.status = 2;
            //        return result;
            //    }
            //}
            if (!string.IsNullOrEmpty(request.wording))
            {
                if (request.wording.Length > 15)
                {
                    result.message = L["AddFail"] + "：" + L["WordingCHKLong"];
                    result.status = 2;
                    return result;
                }
            }
            var querychk = (await _BDExpRepository.WithDetailsAsync()).Where(w => w.expcode == request.expcode && w.company == request.companycategory && w.category.ToString() == request.category).AsNoTracking().ToList();
            if (querychk.Count > 0)
            {
                result.message = L["AddFail"] + "：" + L["DataAlreadyexist"];
                result.status = 2;
                return result;
            }
            else
            {
                BdExp bdExp = _ObjectMapper.Map<AddBDExpDto, BdExp>(request);
                bdExp.cuser = userId;
                bdExp.cdate = System.DateTime.Now;
                var bdExpEntity = await _BDExpRepository.InsertAsync(bdExp);

                // add data into additional_approval_steps need to forloop to insert each approver
                if (request.assignSteps != null && request.assignSteps.Count > 0)
                {
                    List<ExpenseSenarioExtraSteps> additionalApprovalSteps = new List<ExpenseSenarioExtraSteps>();

                    foreach (var assignStep in request.assignSteps)
                    {
                        foreach (var approver in assignStep.approverList)
                        {
                            ExpenseSenarioExtraSteps additionalApprovalStep = new ExpenseSenarioExtraSteps();
                            additionalApprovalStep.exp_code = request.expcode;
                            additionalApprovalStep.name = assignStep.name;
                            additionalApprovalStep.position = assignStep.position;
                            additionalApprovalStep.approver_emplid = approver.emplid;
                            additionalApprovalStep.approver_name = approver.name;
                            additionalApprovalStep.approver_name_a = approver.nameA;
                            additionalApprovalStep.bdexp_id = bdExpEntity.Id;

                            additionalApprovalStep.cuser = userId;
                            additionalApprovalStep.cdate = System.DateTime.Now;
                            additionalApprovalStep.muser = userId;
                            additionalApprovalStep.mdate = System.DateTime.Now;
                            additionalApprovalStep.company = request.companycategory;
                            additionalApprovalSteps.Add(additionalApprovalStep);

                        }
                    }

                    await _AdditionalApprovalStepsRepository.InsertManyAsync(additionalApprovalSteps);
                }

                result.message = L["AddSuccess"];
            }
            return result;
        }
        //修改費用類別
        public async Task<Result<string>> EditExpenseCategory(EditBDExpDto request, string userId)
        {
            Result<string> result = new Result<string>();
            if (string.IsNullOrEmpty(request.expname))
            {
                result.message = L["SaveFailMsg"] + "：" + L["ExpenseNameEmpty"];
                result.status = 2;
                return result;
            }
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
            var querychk = (await _BDExpRepository.WithDetailsAsync()).Where(w => w.expcode == request.expcode && w.company == request.companycategory && w.category.ToString() == request.category && w.Id != request.Id).AsNoTracking().ToList();
            if (querychk.Count > 0)
            {
                result.message = L["AddFail"] + "：" + L["DataAlreadyexist"];
                result.status = 2;
                return result;
            }
            // BdExp bdExp = (await _BDExpRepository.GetBDExpByCode(request.expcode, request.company));
            BdExp bdExp = (await _BDExpRepository.GetBDExpById(request.Id));
            if (bdExp == null)
            {
                result.message = L["SaveFailMsg"] + "：" + L["ExpenseCategoryNotExist"];
                result.status = 2;
                return result;
            }
            // bdExp = _ObjectMapper.Map<BDExpFormDto, BdExp>(request);
            bdExp.expname = request.expname;
            //bdExp.description = request.description;
            bdExp.keyword = request.keyword;
            bdExp.acctcode = request.acctcode;
            bdExp.classno = request.auditlevelcode;
            //bdExp.flag = request.flag;
            //bdExp.invoiceflag = request.invoiceflag;
            //bdExp.type = request.type;
            bdExp.wording = request.wording;
            //bdExp.special = request.special;
            //bdExp.iszerotaxinvter = request.iszerotaxinvter;
            // bdExp.isupload = request.isupload;
            // bdExp.filecategory = request.filecategory;
            //bdExp.filepoints = request.filepoints;
            //bdExp.isinvoice = request.isinvoice;
            // bdExp.invoicecategory = request.invoicecategory;
            bdExp.isdeduction = request.isdeduction;
            bdExp.addsign = request.addsign;
            bdExp.addsignstep = request.addsignstep;
            bdExp.calmethod = request.calmethod;
            bdExp.expcategory = request.extraformcode;
            //bdExp.category = request.category;
            bdExp.assignment = request.assignment;
            bdExp.costcenter = request.costcenter;
            bdExp.pjcode = request.pjcode;
            bdExp.muser = userId;
            bdExp.mdate = System.DateTime.Now;
            bdExp.datelevel = request.datelevel;
            bdExp.authorized = request.authorized;
            bdExp.authorizer = request.authorizer;
            bdExp.sdate = request.sdate;
            bdExp.edate = request.edate;
            //bdExp.expdesc = request.expdesc;
            bdExp.descriptionnotice = request.descriptionnotice;
            //bdExp.issendremindemail = request.issendremindemail;
            await _BDExpRepository.UpdateAsync(bdExp);

            // find and delete all related data from additional_approval_steps
            List<ExpenseSenarioExtraSteps> data = (await _AdditionalApprovalStepsRepository.WithDetailsAsync()).Where(w => w.bdexp_id == request.Id).AsNoTracking().ToList();
            if (data.Count > 0)
            {
                await _AdditionalApprovalStepsRepository.DeleteManyAsync(data);
            }

            // add data into additional_approval_steps need to forloop to insert each approver
            if (request.assignSteps != null && request.assignSteps.Count > 0)
            {
                List<ExpenseSenarioExtraSteps> additionalApprovalSteps = new List<ExpenseSenarioExtraSteps>();

                foreach (var assignStep in request.assignSteps)
                {
                    foreach (var approver in assignStep.approverList)
                    {
                        ExpenseSenarioExtraSteps additionalApprovalStep = new ExpenseSenarioExtraSteps();
                        additionalApprovalStep.exp_code = request.expcode;
                        additionalApprovalStep.name = assignStep.name;
                        additionalApprovalStep.position = assignStep.position;
                        additionalApprovalStep.approver_emplid = approver.emplid;
                        additionalApprovalStep.approver_name = approver.name;
                        additionalApprovalStep.approver_name_a = approver.nameA;
                        additionalApprovalStep.bdexp_id = (Guid) request.Id;

                        additionalApprovalStep.cuser = userId;
                        additionalApprovalStep.cdate = System.DateTime.Now;
                        additionalApprovalStep.muser = userId;
                        additionalApprovalStep.mdate = System.DateTime.Now;
                        additionalApprovalStep.company = request.companycategory;
                        additionalApprovalSteps.Add(additionalApprovalStep);

                    }
                }

                await _AdditionalApprovalStepsRepository.InsertManyAsync(additionalApprovalSteps);
            }

            result.message = L["SaveSuccessMsg"];
            return result;
        }
        //删除费用类别
        public async Task<Result<string>> DeleteExpenseCategory(BDExpFormDto request)
        {
            Result<string> result = new Result<string>();
            BdExp bdExp = (await _BDExpRepository.GetBDExpById(request.Id));
            if (bdExp == null)
            {
                result.message = L["DeleteFail"] + "：" + L["ExpenseCategoryNotExist"];
                result.status = 2;
                return result;
            }
            await _BDExpRepository.DeleteAsync(bdExp);
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
                    company = s[0].ToString().Trim(),
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
                        addBDExpDto.companycategory = item.company;
                        addBDExpDto.category = item.category;
                        addBDExpDto.expcode = item.expcode;
                        addBDExpDto.expname = item.expname;
                        //addBDExpDto.description = item.description;
                        addBDExpDto.keyword = item.keyword;
                        addBDExpDto.acctcode = item.acctcode;
                        //addBDExpDto.classno = item.classno;
                        // addBDExpDto.isupload = item.isupload;
                        // addBDExpDto.filecategory = item.filecategory;
                        //addBDExpDto.filepoints = item.filepoints;
                        //addBDExpDto.isinvoice = item.isinvoice;
                        // addBDExpDto.invoicecategory = item.invoicecategory;
                        // addBDExpDto.addsign = item.addsign;
                        // addBDExpDto.addsignstep = item.addstep;
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
                    List<BdExp> iBDExps = _ObjectMapper.Map<List<AddBDExpDto>, List<BdExp>>(checkResults.Where(w => w.status == 1).Select(w => w.data).ToList());
                    iBDExps.ForEach(w =>
                    {
                        w.cuser = userId;
                        w.cdate = System.DateTime.Now;
                    });
                    await _BDExpRepository.InsertManyAsync(iBDExps);
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
                //if (string.IsNullOrEmpty(item.classno))
                //{
                //    check.message = L["AddFail"] + "：" + L["ClassNoEmpty"];
                //    check.status = 2;
                //    continue;
                //}
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
                //} else if (!string.IsNullOrEmpty(item.description))
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
                var querychk = (await _BDExpRepository.WithDetailsAsync()).Where(w => w.expcode == item.expcode && w.company == item.companycategory && w.category == Convert.ToInt32(item.category)).AsNoTracking().ToList();
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
        public async Task<Result<List<BDExpDto>>> GetXZExp(string company)
        {
            Result<List<BDExpDto>> result = new();
            var expQuery = (await _bdExpRepository.WithDetailsAsync()).Where(w => w.company == company);
            var bdAccountQuery = (await _bdAccountRepository.WithDetailsAsync()).Where(w => w.company == company);
            List<BDExpDto> data = (from a in expQuery
                                        join b in bdAccountQuery
                                        on a.acctcode equals b.acctcode
                                        where a.company == company && a.category == 3
                                        select new BDExpDto
                                        {
                                            expcode = a.expcode,
                                            expname = a.expname,
                                            description = a.description,
                                            keyword = a.keyword,
                                            filecategory = a.filecategory,
                                            isupload = a.isupload,
                                            filepoints = a.filepoints,
                                            isinvoice = a.isinvoice,
                                            expcategory = a.expcategory,
                                            acctcode = a.acctcode,
                                            acctname = b.acctname
                                        }).Distinct().AsNoTracking().ToList();
            result.data = data;
            return result;
        }
    }
}

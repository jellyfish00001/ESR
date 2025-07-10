using System;
using ERS.Domain.IDomainServices;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDCar;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using System.IO;
using ERS.DTO.Invoice;
using ERS.DTO.MobileSign;
using ERS.Application.Contracts.DTO.Application;
using NPOI.XWPF.UserModel;

namespace ERS.DomainServices
{
    public class Cash5DomainService : CommonDomainService, ICash5DomainService
    {
         private IObjectMapper _ObjectMapper;         
        private IAutoNoRepository _AutoNoRepository;
        private IBDExpenseDeptRepository _BDExpenseDeptRepository;
        private IBDExpenseSenarioRepository _BDExpenseSenarioRepository;
        private ICashHeadRepository _CashHeadRepository;
        private ICashDetailRepository _CashDetailRepository;
        private IEmployeeInfoRepository _EmployeeInfoRepository;
         public Cash5DomainService(
            IObjectMapper objectMapper,
            IAutoNoRepository autoNoRepository,
            IBDExpenseDeptRepository bdEexpenseDeptRepository,
            IBDExpenseSenarioRepository bdExpenseSenarioRepository,
            ICashHeadRepository cashHeadRepository,
            ICashDetailRepository cashDetailRepository,
            IEmployeeInfoRepository employeeInfoRepository
         ){
            _ObjectMapper = objectMapper;
            _AutoNoRepository = autoNoRepository;
            _BDExpenseDeptRepository = bdEexpenseDeptRepository;
            _BDExpenseSenarioRepository = bdExpenseSenarioRepository;
            _CashHeadRepository = cashHeadRepository;
            _CashDetailRepository = cashDetailRepository;
            _EmployeeInfoRepository = employeeInfoRepository;
         }

         public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P"){
            Result<CashResult> result = new();
            if (formCollection == null || formCollection.Count == 0){
                result.message = L["FormCollectionIsNullOrEmpty"];
                return result;
            }

            return null;

         }

         public async Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token, string status = "T"){
            Result<CashResult> result = new();
            if (formCollection == null || formCollection.Count == 0){
                result.message = L["FormCollectionIsNullOrEmpty"];
                return result;
            }

            return null;

         }

        public async Task<Result<List<Cash5ExcelDto>>> CheckExcel(IFormFile excelFile, string company, decimal totalAmount, string userId){
            Result<List<Cash5ExcelDto>> result = new();
            List<Cash5ExcelDto> excelList = new List<Cash5ExcelDto>();
            if (excelFile.ContentType == "application/vnd.ms-excel" || excelFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"){
              DataTable dt = NPOIHelper.GetDataTableFromExcel(excelFile, 1);
              excelList = dt.Rows.Cast<DataRow>().Select((row, idx) => new Cash5ExcelDto
              {
                  senarioname = row[0]?.ToString().Trim(), // 費用類別
                  deptid = row[1]?.ToString().Trim() ?? string.Empty, // 費用歸屬部門
                  agentemplid = row[2]?.ToString().Trim(), // 承辦人
                  unifycode = row[3]?.ToString().Trim(), // 廠商統一編號
                  billnoandsummary = row[4]?.ToString().Trim(), // 提單號碼/費用摘要
                  reportno = row[5]?.ToString().Trim(), // 報單號碼
                  invoice = row[6]?.ToString().Trim(), // 稅單號碼/發票號碼/憑證號碼
                  rdate = DateTime.TryParse(row[7]?.ToString(), out var date) ? (DateTime?) Convert.ToDateTime(Convert.ToDateTime(date).ToString("yyyy/MM/dd")) : null, // 稅單付訖日期/發票日期/憑證日期
                  importtax = decimal.TryParse(row[8]?.ToString(), out var importTax) ? importTax : 0, // 進口稅/其他費用
                  tradepromotionfee = decimal.TryParse(row[9]?.ToString(), out var tradeFee) ? (decimal?)tradeFee : null, // 推貿費
                  taxexpense = decimal.TryParse(row[10]?.ToString(), out var tax) ? (decimal?)tax : null, // 營業稅
                  totaltaxandfee = decimal.TryParse(row[11]?.ToString(), out var totalTax) ? (decimal?)totalTax : null, // 稅費合計
                  taxbaseamount = decimal.TryParse(row[12]?.ToString(), out var taxBase) ? (decimal?)taxBase : null // 稅基
              }).ToList();
            }
            else{
              result.status = 2;  
              result.message = L["OnlyUploadExcel"];
              return result;
            }

            //資料驗證
            //1.檢查Excel是否為空
            if (excelList == null || excelList.Count == 0){
                return errorResult(result, L["ExcelEmpty"]);
            }

            //2.檢查申請者(受款人)是否存在
            var empExist = (await _EmployeeInfoRepository.WithDetailsAsync()).Where(e => e.emplid == userId).ToList();
            if (!empExist.Any()){
                return errorResult(result, String.Format(L["PayeeNotExist"], userId));
            }

            //3.檢查請款金額是否等於稅費合計
            decimal excelTotal = excelList.Sum(x => x.totaltaxandfee ?? 0);
            if (excelTotal != totalAmount){                
              return errorResult(result, L["ExcelRequestAmountNotEqualToTotalAmount"]);
            }

            //4.檢查部門是否存在bd_expense_department或bd_virtual_department
            var excelDeptIds = excelList.Select(x => x.deptid).Distinct().ToList();
            var expenseDepts = (await _BDExpenseDeptRepository.WithDetailsAsync()).Select(d => d.deptid).ToList();
            var virtualDepts = (await _BDExpenseDeptRepository.WithDetailsAsync()).Select(d => d.deptid).ToList();//Todo:要改成virtual的表格
            var combineDepts = expenseDepts.Union(virtualDepts).ToHashSet();
            var missingValues = excelDeptIds.Where(value => !combineDepts.Contains(value)).ToList();
            if (missingValues.Any()){
              return errorResult(result, string.Join(", ", missingValues)+ " "+ L["ExcelDeptNotExist"]);
            }

            //5.費用類別,承辦人,提單號碼/費用摘要的總長度是否超過 37 個字元
            var lenExceed37 = excelList.Select(x=> x.senarioname+x.agentemplid.ToString()+x.billnoandsummary.ToString()).ToList().Where(y=> y.Length > 37).ToList();
            if (lenExceed37.Any()){
              return errorResult(result, string.Join(",", lenExceed37)+L["ExcelSenarionameAgentemplidBillnoandsummaryLengthExceed37"]);
            }

            //6.若費用類別不為空，廠商統一編號和稅單號碼/發票號碼/憑證號碼也不能為空
            var emptyCol = excelList.Where(x => !string.IsNullOrEmpty(x.senarioname) && (string.IsNullOrEmpty(x.unifycode) || string.IsNullOrEmpty(x.invoice))).ToList();
            if (emptyCol.Any()){
              return errorResult(result, string.Join(",", emptyCol.Select(x => x.senarioname)) + L["廠商統一編號或發票不能為空"]);
            }

            //7.若稅基不為空，則營業稅必須不為空且不為0
            var taxBasement = excelList[0].taxbaseamount;
            if(taxBasement != null || taxBasement > 0){
              var taxEmpty = excelList.Where(x => !string.IsNullOrEmpty(x.senarioname) && (x.taxexpense == null || x.taxexpense == 0)).ToList();
              if (taxEmpty.Any()){
                return errorResult(result, L["稅基不為空，營業稅必須不為空且不為0"]);
              }
            }

            //8.若費用類別不為空，檢查每筆進口稅/其它費用+推貿費+營業稅是否等於稅費合計
            string checkResult = CheckTotalFee(excelList);
            if (checkResult != "1"){
                return errorResult(result, checkResult);
            }

            //9.檢查費用類別是否為空，並填上費用類別和會計科目
            foreach (var item in excelList)
            {
              if (!string.IsNullOrEmpty(item.senarioname)){
                BDExpenseSenario senario = (await _BDExpenseSenarioRepository.WithDetailsAsync())
                                               .Where(s => s.companycategory == company && s.senarioname == item.senarioname)
                                               .FirstOrDefault();
                if(senario != null){
                  item.expensecode = senario.expensecode;
                  item.accountcode = senario.accountcode;
                }
                else{
                    return errorResult(result, L["BDSenarioNotFound"]);
                }              
              } 
            }

            //10.計算發票稅前金額 = 進口稅/其他費用 + 推貿費
            foreach (var item in excelList)
            {
                item.invoiceAmountBeforeTax = (item.importtax ?? 0) + (item.tradepromotionfee ?? 0);
            }

            result.status = 1;
            result.data = excelList;
            return result;
        }

        public string CheckTotalFee(List<Cash5ExcelDto> excelList)
        {
            string result = "1";
            var groupedRecords = new List<List<Cash5ExcelDto>>();
            var currentGroup = new List<Cash5ExcelDto>();
            string lastSenarioname = string.Empty;
        
            //sttep 1: 按順序分組，根據非空的 senarioname 分隔
            foreach (var dto in excelList)
            {
                if (!string.IsNullOrEmpty(dto.senarioname) && currentGroup.Any())
                {
                    groupedRecords.Add(currentGroup);
                    currentGroup = new List<Cash5ExcelDto>();
                    lastSenarioname = dto.senarioname;
                }
                // 填入當前費用類別（若為空則使用 lastSenarioname）
                var newDto = new Cash5ExcelDto
                {
                    senarioname = string.IsNullOrEmpty(dto.senarioname) ? lastSenarioname : dto.senarioname,
                    importtax = dto.importtax,
                    tradepromotionfee = dto.tradepromotionfee,
                    taxexpense = dto.taxexpense,
                    totaltaxandfee = dto.totaltaxandfee
                };
                currentGroup.Add(newDto);
                if (!string.IsNullOrEmpty(dto.senarioname))
                {
                    lastSenarioname = dto.senarioname;
                }
            }
            if (currentGroup.Any())
            {
                groupedRecords.Add(currentGroup);
            }
        
            // Step 2: 檢查每組的稅費合計
            foreach (var group in groupedRecords)
            {
                // 計算組內所有進口稅/其它費用 + 推貿費 + 營業稅的總和
                decimal sum = group.Sum(dto => (dto.importtax ?? 0) + (dto.tradepromotionfee ?? 0) + (dto.taxexpense ?? 0));
                
                // 取得組內第一筆非空的稅費合計（而非最後一筆）
                var firstTotalTaxAndFee = group.FirstOrDefault(dto => dto.totaltaxandfee.HasValue)?.totaltaxandfee;
                
                // 如果有稅費合計且不等於計算總和，則記錄整組
                if (firstTotalTaxAndFee.HasValue && sum != firstTotalTaxAndFee.Value)
                {
                    result+= $"費用類別: {group[0].senarioname}，進口稅/其它費用 + 推貿費 + 營業稅 ({sum}) 不等於稅費合計 ({firstTotalTaxAndFee})";
                }
            }
            return result;
        }

        internal Result<T> errorResult<T>(Result<T> result, string message)
        {
            result.status = 2;
            result.message = message;
            return result;
        }
    }
}
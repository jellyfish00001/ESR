using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDAccount;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Volo.Abp.ObjectMapping;
using Microsoft.AspNetCore.Http;
using System.Data;
namespace ERS.DomainServices
{
    public class BDAccountDomainService : CommonDomainService, IBDAccountDomainService
    {
        private IBdAccountRepository _BdAccountRepository;
        private IObjectMapper _ObjectMapper;
        public BDAccountDomainService(
            IBdAccountRepository BdAccountRepository,
            IObjectMapper ObjectMapper
        )
        {
            _BdAccountRepository = BdAccountRepository;
            _ObjectMapper = ObjectMapper;
        }
        /// <summary>
        /// 会计科目查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result<List<QueryBDAcctDto>>> QueryPageBDAccount(Request<BDAccountParamDto> request)
        {
            Result<List<QueryBDAcctDto>> result = new Result<List<QueryBDAcctDto>>()
            {
                data = new List<QueryBDAcctDto>()
            };
            if (request == null || request.data == null || request.data.companyList.Count == 0)
            {
                result.message = "illegal request";
                result.status = 2;
                return result;
            }
            List<BdAccount> query = (await _BdAccountRepository.WithDetailsAsync())
                        .WhereIf(!string.IsNullOrEmpty(request.data.acctcode), w => w.acctcode == request.data.acctcode.Trim())
                        .WhereIf(!string.IsNullOrEmpty(request.data.acctname), w => w.acctname == request.data.acctname.Trim())
                        .WhereIf(!request.data.companyList.IsNullOrEmpty(), w => request.data.companyList.Contains(w.company))
                        .ToList();
            List<QueryBDAcctDto> resultdata = _ObjectMapper.Map<List<BdAccount>, List<QueryBDAcctDto>>(query);
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            int count = query.Count;
            result.data = resultdata.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = count;
            return result;
        }
        /// <summary>
        /// 会计科目添加
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<string>> AddBDAccount(BDAccountParamDto request, string userId)
        {
            Result<string> result = new Result<string>();
            if (string.IsNullOrEmpty(request.acctcode))
            {
                result.status = 2;
                result.message = L["AccountCodeEmpty1"];
                return result;
            }
            if (string.IsNullOrEmpty(request.acctname))
            {
                result.status = 2;
                result.message = L["AccountNameEmpty"];
                return result;
            }
            if (request.company.IsNullOrEmpty())
            {
                result.status = 2;
                result.message = L["CompanyCodeEmpty"];
                return result;
            }
            var checkExist = (await _BdAccountRepository.WithDetailsAsync()).Where(w => w.acctcode == request.acctcode && request.company==w.company).ToList();
            if (checkExist.Count > 0)
            {
                result.status = 2;
                result.message = L["AccountCodeExist"];
                return result;
            }
            BdAccount bdAccount = _ObjectMapper.Map<BDAccountParamDto, BdAccount>(request);
            bdAccount.cuser = userId;
            bdAccount.muser = userId;
            bdAccount.cdate = System.DateTime.Now;
            bdAccount.mdate = System.DateTime.Now;
            await _BdAccountRepository.InsertAsync(bdAccount);
            result.message = L["SaveSuccessMsg"];
            return result;
        }
        /// <summary>
        /// 会计科目编辑
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<string>> EditBDAccount(BDAccountParamDto request, string userId)
        {
            Result<string> result = new Result<string>();
            result.status = 2;
            BdAccount bdAccount = (await _BdAccountRepository.GetBdAccount(request.acctcode, request.company));
            if (bdAccount != null)
            {
                // bdAccount = _ObjectMapper.Map<BDAccountParamDto,BdAccount>(request);
                bdAccount.acctname = request.acctname;
                bdAccount.muser = userId;
                bdAccount.mdate = System.DateTime.Now;
                result.status = 1;
                result.message = L["SaveSuccessMsg"];
                await _BdAccountRepository.UpdateAsync(bdAccount);
            }
            else
            {
                result.message = L["SaveFailMsg"] + "：" + L["BDAccountNotFound"];
            }
            return result;
        }
        public async Task<Result<string>> DeleteBDAccount(BDAccountParamDto request)
        {
            Result<string> result = new Result<string>();
            BdAccount bdAccount = (await _BdAccountRepository.GetBdAccount(request.acctcode, request.company));
            if (bdAccount == null)
            {
                result.message = L["DeleteFail"] + "：" + L["BDAccountNotFound"];
                result.status = 2;
                return result;
            }
            await _BdAccountRepository.DeleteAsync(bdAccount);
            result.message = L["DeleteSuccess"];
            return result;
        }
        public async Task<byte[]> GetBDAccountExcel(Request<BDAccountParamDto> request)
        {
            byte[] data = null;
            if (request != null)
            {
                List<BdAccount> query = (await _BdAccountRepository.WithDetailsAsync())
                        .WhereIf(!string.IsNullOrEmpty(request.data.acctcode), w => w.acctcode == request.data.acctcode.Trim())
                        .WhereIf(!string.IsNullOrEmpty(request.data.acctname), w => w.acctname == request.data.acctname.Trim())
                        .WhereIf(!request.data.companyList.IsNullOrEmpty(), w => request.data.companyList.Contains(w.company))
                        .ToList();
                List<QueryBDAcctDto> bdaccounts = _ObjectMapper.Map<List<BdAccount>, List<QueryBDAcctDto>>(query);
                XSSFWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("sheet");
                string[] header = new string[]
                {
                    "#",
                    L["AccountCode1"],
                    L["AccountName1"],
                    L["Cuser"],
                    L["Cdate"],
                    L["Muser"],
                    L["Mdate"],
                    L["CompanyCode"]
                };
                IRow rowHeader = sheet.CreateRow(0);
                for(int i = 0; i < header.Length; i++)
                {
                    rowHeader.CreateCell(i).SetCellValue(header[i]);
                }
                for(int i = 0; i < bdaccounts.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 1);
                    dataRow.CreateCell(0).SetCellValue("");
                    dataRow.CreateCell(1).SetCellValue(bdaccounts[i].acctcode);
                    dataRow.CreateCell(2).SetCellValue(bdaccounts[i].acctname);
                    dataRow.CreateCell(3).SetCellValue(bdaccounts[i].cuser);
                    dataRow.CreateCell(4).SetCellValue((bdaccounts[i].cdate == null || bdaccounts[i].cdate == DateTime.MinValue) ? "" : Convert.ToDateTime(bdaccounts[i].cdate).ToString("yyyy/MM/dd"));
                    dataRow.CreateCell(5).SetCellValue(bdaccounts[i].muser);
                    dataRow.CreateCell(6).SetCellValue(bdaccounts[i].mdate == null ? "" : Convert.ToDateTime(bdaccounts[i].mdate).ToString("yyyy/MM/dd"));
                    dataRow.CreateCell(7).SetCellValue(bdaccounts[i].company);
                }
                // for (int i = 0; i <= bdaccounts.Count; i++)
                // {
                //     sheet.AutoSizeColumn(i);
                // }
                using(MemoryStream ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    ms.Flush();
                    data = ms.ToArray();
                }
            }
            return data;
        }
        public async Task<Result<ExcelDto<QueryBDAcctDto>>> GetBDExcelData(Request<BDAccountParamDto> request)
        {
            Result<ExcelDto<QueryBDAcctDto>> result = new Result<ExcelDto<QueryBDAcctDto>>()
            {
            };
            ExcelDto<QueryBDAcctDto> excelData = new ExcelDto<QueryBDAcctDto>();
            if (request != null)
            {
                List<BdAccount> query = (await _BdAccountRepository.WithDetailsAsync())
                        .WhereIf(!string.IsNullOrEmpty(request.data.acctcode), w => w.acctcode == request.data.acctcode.Trim())
                        .WhereIf(!string.IsNullOrEmpty(request.data.acctname), w => w.acctname == request.data.acctname.Trim())
                        .WhereIf(!request.data.companyList.IsNullOrEmpty(), w => request.data.companyList.Contains(w.company))
                        .ToList();
                List<QueryBDAcctDto> bdaccounts = _ObjectMapper.Map<List<BdAccount>, List<QueryBDAcctDto>>(query);
                string[] header = new string[]
                {
                    "#",
                    L["AccountCode1"],
                    L["AccountName1"],
                    L["Cuser"],
                    L["Cdate"],
                    L["Muser"],
                    L["Mdate"],
                    L["CompanyCode"]
                };
                excelData.header = header;
                excelData.body = bdaccounts;
                result.data = excelData;
            }
            return result;
        }
        public async Task<Result<string>> BatchUploadBDAccount(IFormFile excelFile)
        {
            Result<string> result = new();
            if (excelFile.ContentType == "application/vnd.ms-excel" || excelFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                DataTable dt = NPOIHelper.GetDataTableFromExcel(excelFile, 1);
                var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
                {
                    company = s[0].ToString().Trim(),
                    acctcode = s[1].ToString().Trim(),
                    acctname = s[2].ToString().Trim()
                }).ToList();
                List<BdAccount> addBdAccountList = new();
                foreach(var item in list)
                {
                    BdAccount bdAccount = new()
                    {
                        company = item.company,
                        acctcode = item.acctcode,
                        acctname = item.acctname,
                        seq = 0
                    };
                    addBdAccountList.Add(bdAccount);
                }
                if(addBdAccountList.Count > 0)
                    await _BdAccountRepository.InsertManyAsync(addBdAccountList);
                    result.message = "success";
            }
            return result;
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDSignlevel;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Volo.Abp.ObjectMapping;
using Microsoft.AspNetCore.Http;
using System.Data;
// BDSignlevel
namespace ERS.DomainServices
{
    public class BDSignlevelDomainService : CommonDomainService, IBDSignlevelDomainService
    {
        private IBDSignlevelRepository _BDSignlevelRepository;
        private IBDTreelevelRepository _BDTreelevelRepository;
        private IObjectMapper _ObjectMapper;
        public BDSignlevelDomainService(
            IBDSignlevelRepository BDSignlevelRepository,
            IBDTreelevelRepository BDTreelevelRepository,
            IObjectMapper ObjectMapper
        )
        {
            _BDSignlevelRepository = BDSignlevelRepository;
            _BDTreelevelRepository = BDTreelevelRepository;
            _ObjectMapper = ObjectMapper;
        }

        //核決權限查詢
        public async Task<Result<List<QueryBDSignlevelDto>>> GetPageBDSignlevel(Request<BDSignlevelParamDto> request)
        {
            Result<List<QueryBDSignlevelDto>> result = new Result<List<QueryBDSignlevelDto>>()
            {
                data = new List<QueryBDSignlevelDto>()
            };
            if (request == null)
            {
                result.message = "illegal request";
                result.status = 2;
                return result;
            }
            List<BDSignlevel> query = (await _BDSignlevelRepository.WithDetailsAsync())
                        .Where(w => request.data.companyList.Contains(w.company.Trim()))
                        //.WhereIf(!string.IsNullOrEmpty(request.data.company), w => request.data.company.Trim().Contains(w.company.Trim()))
                        .ToList();

            List<QueryBDSignlevelDto> resultdata = _ObjectMapper.Map<List<BDSignlevel>, List<QueryBDSignlevelDto>>(query);
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

        //取得核決權限代碼列表By公司別
        public async Task<List<string>> GetBDSignlevelByCompanyCode(string company) {
            return (await _BDSignlevelRepository.WithDetailsAsync())
                        .WhereIf(!string.IsNullOrEmpty(company), w => w.company == company.Trim())
                        .Select(w => w.item)
                        .Distinct()
                        .ToList();
        }

        //新增核決權限
        public async Task<Result<string>> AddBDSignlevel(BDSignlevelParamDto request, string userId)
        {
            Result<string> result = new Result<string>();
            if (string.IsNullOrEmpty(request.company))
            {
                result.status = 2;
                result.message = L["CompanyNotEmpty"];
                return result;
            }
            if (string.IsNullOrEmpty(request.item))
            {
                result.status = 2;
                result.message = L["AprovalItemEmpty"];
                return result;
            }
            if (string.IsNullOrEmpty(request.signlevel))
            {
                result.status = 2;
                result.message = L["ApprovalLevelEmpty"];
                return result;
            }
            if (request.money<=0)
            {
                result.status = 2;
                result.message = L["ApprovalAmountEmpty"];
                return result;
            }
            if (string.IsNullOrEmpty(request.currency))
            {
                result.status = 2;
                result.message = L["CurrencyEmpty"];
                return result;
            }
            
            BDSignlevel hasBDSignlevel = (await _BDSignlevelRepository.GetBDSignlevel(request.company, request.item, request.signlevel));
            if (hasBDSignlevel != null)
            {
                result.status = 2;
                result.message = L["SaveFailMsg"] + "：" + L["BDSignlevelExist"];
                return result;
            }

            BDSignlevel bdSignlevel = _ObjectMapper.Map<BDSignlevelParamDto, BDSignlevel>(request);
            bdSignlevel.cuser = userId;
            bdSignlevel.muser = userId;
            bdSignlevel.cdate = System.DateTime.Now;
            bdSignlevel.mdate = System.DateTime.Now;
            await _BDSignlevelRepository.InsertAsync(bdSignlevel);
            result.message = L["SaveSuccessMsg"];
            return result;

        }

        public async Task<Result<string>> EditBDSignlevel(BDSignlevelParamDto request, string userId)
        {
            Result<string> result = new Result<string>();
            result.status = 2;
            BDSignlevel bdSignlevel = (await _BDSignlevelRepository.GetBDSignlevel(request.company, request.item, request.signlevel));
            if (bdSignlevel != null)
            {                
                bdSignlevel.money = request.money;
                bdSignlevel.currency = request.currency;
                bdSignlevel.muser = userId;
                bdSignlevel.mdate = System.DateTime.Now;
                result.status = 1;
                result.message = L["SaveSuccessMsg"];
                await _BDSignlevelRepository.UpdateAsync(bdSignlevel);
            }
            else
            {
                result.message = L["SaveFailMsg"] + "：" + L["SignLevelNotFound"];
            }
            return result;

        }
        public async Task<Result<string>> DeleteBDSignlevel(BDSignlevelParamDto request)
        {
            Result<string> result = new Result<string>();
            BDSignlevel bdSignlevel = (await _BDSignlevelRepository.GetBDSignlevel(request.company, request.item, request.signlevel));
            if (bdSignlevel == null)
            {  
                result.message = L["DeleteFail"] + "：" + L["SignLevelNotFound"];
                result.status = 2;
                return result;
            }

            await _BDSignlevelRepository.DeleteAsync(bdSignlevel);
            result.message = L["DeleteSuccess"];
            return result;

        }

        public async Task<Result<string>> DeleteBDSignlevelById(List<Guid?> Ids){
            Result<string> result = new Result<string>();
            List<BDSignlevel> bdSignlevel = await _BDSignlevelRepository.GetBDSignlevelByIds(Ids);
            foreach (var item in bdSignlevel)
            {
              if (item == null)
              {  
                result.message = L["DeleteFail"] + "：" + L["SignLevelNotFound"];
                result.status = 2;
                return result;
              }
            }            

            await _BDSignlevelRepository.DeleteManyAsync(bdSignlevel);
            result.message = L["DeleteSuccess"];
            return result;
        }


        public async Task<byte[]> GetBDSignlevelExcel(Request<BDSignlevelParamDto> request)
        {
            byte[] data = null;
            if (request != null)
            {
                List<BDSignlevel> query = (await _BDSignlevelRepository.WithDetailsAsync())
                        .WhereIf(!string.IsNullOrEmpty(request.data.company), w => w.company == request.data.company.Trim())
                        .ToList();
                List<BDTreelevel> treeLevel = (await _BDTreelevelRepository.WithDetailsAsync()).ToList();

                List<QueryBDSignlevelDto> bdSignlevels = _ObjectMapper.Map<List<BDSignlevel>, List<QueryBDSignlevelDto>>(query);
                foreach (var signlevel in bdSignlevels)
                {
                    var tree = treeLevel.FirstOrDefault(w => w.levelnum == decimal.Parse(signlevel.signlevel));
                    if (tree == null){
                      signlevel.signlevel = "Unknown sign level";
                      continue;
                    }
                    signlevel.signlevel = request.data.language switch
                    {
                        "en" => tree?.levelname + " (" + tree?.levelnum + ")",
                        "zh_TW" => tree?.leveltwname + " (" + tree?.levelnum + ")",
                        "zh_CN" => tree?.levelcnname + " (" + tree?.levelnum + ")",
                        _ => tree?.leveltwname + " (" + tree?.levelnum + ")",
                    };
                }
                XSSFWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("sheet");
                string[] header = new string[]
                {
                    "#",
                    L["company"],
                    L["ApprovalItem"],
                    L["ApprovalLevel"],
                    L["ApprovalAmount"],
                    L["currency"],
                    L["Cuser"],
                    L["Cdate"],
                    L["Muser"],
                    L["Mdate"]
                };
                IRow rowHeader = sheet.CreateRow(0);
                for(int i = 0; i < header.Length; i++)
                {
                    rowHeader.CreateCell(i).SetCellValue(header[i]);
                }
                for(int i = 0; i < bdSignlevels.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 1);
                    dataRow.CreateCell(0).SetCellValue("");
                    dataRow.CreateCell(1).SetCellValue(bdSignlevels[i].company);
                    dataRow.CreateCell(2).SetCellValue(bdSignlevels[i].item);
                    dataRow.CreateCell(3).SetCellValue(bdSignlevels[i].signlevel);
                    dataRow.CreateCell(4).SetCellValue(bdSignlevels[i].money.ToString());
                    dataRow.CreateCell(5).SetCellValue(bdSignlevels[i].currency);
                    dataRow.CreateCell(6).SetCellValue(bdSignlevels[i].cuser);
                    dataRow.CreateCell(7).SetCellValue((bdSignlevels[i].cdate == null || bdSignlevels[i].cdate == DateTime.MinValue) ? "" : Convert.ToDateTime(bdSignlevels[i].cdate).ToString("yyyy/MM/dd"));
                    dataRow.CreateCell(8).SetCellValue(bdSignlevels[i].muser);
                    dataRow.CreateCell(9).SetCellValue(bdSignlevels[i].mdate == null ? "" : Convert.ToDateTime(bdSignlevels[i].mdate).ToString("yyyy/MM/dd"));
                }
                using MemoryStream ms = new();
                workbook.Write(ms);
                ms.Flush();
                data = ms.ToArray();
            }
            return data;

        }

        public async Task<byte[]> GetBDSignlevelExcelTemp()
        {
            byte[] data = null;
           
            
             List<BDTreelevel> treeLevel = (await _BDTreelevelRepository.WithDetailsAsync()).ToList();

             
             /*    signlevel.signlevel = language switch
                 {
                     "en" => tree?.levelname + " (" + tree?.levelnum + ")",
                     "zh_TW" => tree?.leveltwname + " (" + tree?.levelnum + ")",
                     "zh_CN" => tree?.levelcnname + " (" + tree?.levelnum + ")",
                     _ => tree?.leveltwname + " (" + tree?.levelnum + ")",
                 };*/
             
             XSSFWorkbook workbook = new XSSFWorkbook();
             ISheet sheet = workbook.CreateSheet("sheet");
             string[] header =
             {                 
                 L["company"],
                 L["ApprovalItem"],
                 L["ApprovalLevel"],
                 L["ApprovalAmount"],
                 L["currency"],
                 L[""],
                 L[""],
                 L["ApprovalLevel"],
                 L["ApprovalLeveltw"],
                 L["ApprovalLevelcn"],
                 L["ApprovalLevelNum"],

             };
             IRow rowHeader = sheet.CreateRow(0);
             for(int i = 0; i < header.Length; i++)
             {
                 if(i<5){
                    rowHeader.CreateCell(i).SetCellValue("* " + header[i]);
                 }else{
                   rowHeader.CreateCell(i).SetCellValue(header[i]);
                 }
             }
             for(int i = 0; i < treeLevel.Count; i++)
             {
                 IRow dataRow = sheet.CreateRow(i + 1);
                 if(i==0){
                    dataRow.CreateCell(0).SetCellValue("WHQ");
                    dataRow.CreateCell(1).SetCellValue("A1");
                    dataRow.CreateCell(2).SetCellValue("-1");
                    dataRow.CreateCell(3).SetCellValue("30000000");
                    dataRow.CreateCell(4).SetCellValue("NTD");

                 }
                 dataRow.CreateCell(7).SetCellValue(treeLevel[i].levelname);
                 dataRow.CreateCell(8).SetCellValue(treeLevel[i].leveltwname);
                 dataRow.CreateCell(9).SetCellValue(treeLevel[i].levelcnname);
                 dataRow.CreateCell(10).SetCellValue(treeLevel[i].levelnum.ToString());
             }
             using MemoryStream ms = new();
             workbook.Write(ms);
             ms.Flush();
             data = ms.ToArray();
            
            return data;

        }

        //目前暫時沒用到
        public async Task<Result<ExcelDto<QueryBDSignlevelDto>>> GetBDExcelData(Request<BDSignlevelParamDto> request)
        {
            Result<ExcelDto<QueryBDSignlevelDto>> result = new ()
            {
                data = new ExcelDto<QueryBDSignlevelDto>()
            };
            BDSignlevel BDSignlevel_Check = (await _BDSignlevelRepository.GetBDSignlevel(request.data.company, request.data.item, request.data.signlevel));
            return result;

        }
        public async Task<Result<string>> BatchUploadBDSignlevel(IFormFile excelFile, string userId)
        {
            Result<string> result = new Result<string>();
            if (excelFile.ContentType == "application/vnd.ms-excel" || excelFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                DataTable dt = NPOIHelper.GetDataTableFromExcel(excelFile, 1);
                var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
                {
                    company = s[0].ToString().Trim(),
                    item = s[1].ToString().Trim(),
                    signlevel = s[2].ToString().Trim(),
                    money = s[3].ToString().Trim(),
                    currency = s[4].ToString().Trim()
                }).ToList();

                var processedItems = new HashSet<(string company, string item, string signlevel)>();//去重
                List<BDSignlevel> addBDSignlevelList = new();
                foreach(var item in list)
                {
                    var currentTuple = (item.company, item.item, item.signlevel);
                    if(processedItems.Contains(currentTuple)){
                        continue; //跳過重複資料
                    }
                    processedItems.Add(currentTuple); //紀錄已處理資料

                    BDSignlevel BDSignlevel = new()
                    {
                        company = item.company,
                        item = item.item,
                        signlevel = item.signlevel,
                        money = decimal.Parse(item.money),
                        currency = item.currency,
                        muser = userId,
                        mdate = DateTime.Now,
                    };
                    BDSignlevel BDSignlevel_Check = (await _BDSignlevelRepository.GetBDSignlevel(item.company, item.item, item.signlevel));
                    if(BDSignlevel_Check == null) addBDSignlevelList.Add(BDSignlevel);
                }
                if(addBDSignlevelList.Count > 0)
                    await _BDSignlevelRepository.InsertManyAsync(addBDSignlevelList);
                    result.message = "success";
            }
            return result;

        }
    }
}
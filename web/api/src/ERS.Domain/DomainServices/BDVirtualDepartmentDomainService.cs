using ERS.DTO;
using ERS.DTO.BDExpenseDept;
using ERS.DTO.BDVirtualDepartments;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace ERS.DomainServices
{
    public class BDVirtualDepartmentDomainService : CommonDomainService, IBDVirtualDepartmentDomainService
    {
        private IBDExpenseDeptRepository _BDExpenseDeptRepository;
        private IObjectMapper _ObjectMapper;
        public BDVirtualDepartmentDomainService(
            IBDExpenseDeptRepository BDExpenseDeptRepository,
            IObjectMapper ObjectMapper
        )
        {
            _BDExpenseDeptRepository = BDExpenseDeptRepository;
            _ObjectMapper = ObjectMapper;
        }

        public async Task<Result<List<QueryBDVirtualDepartmentsDto>>> GetPageBDVirtualDepartments(Request<QueryBDVirtualDepartmentsDto> request){
            Result<List<QueryBDVirtualDepartmentsDto>> result = new Result<List<QueryBDVirtualDepartmentsDto>>(){
                data = new List<QueryBDVirtualDepartmentsDto>()
            };

            if (request == null)
            {
                result.message = "illegal request";
                result.status = 2;
                return result;
            }

            List<BDExpenseDept> query = (await _BDExpenseDeptRepository.WithDetailsAsync())
                        .WhereIf(!string.IsNullOrEmpty(request.data.company), w => w.company.ToUpper().Contains(request.data.company.Trim().ToUpper()))
                        .WhereIf(!string.IsNullOrEmpty(request.data.deptid), w => w.deptid.ToUpper().Contains(request.data.deptid.Trim().ToUpper()))
                        .Where(w => w.isvirtualdept == "Y")
                        .ToList();            

            List<QueryBDVirtualDepartmentsDto> resultdata = _ObjectMapper.Map<List<BDExpenseDept>, List<QueryBDVirtualDepartmentsDto>>(query);
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
        public async Task<Result<string>> AddBDVirtualDepartment(QueryBDVirtualDepartmentsDto request, string userId)
        {
            Result<string> result = new Result<string>();

            if (string.IsNullOrEmpty(request.company))
            {
                result.status = 2;
                result.message = L["CompanyNotEmpty"];
                return result;
            }
            if (string.IsNullOrEmpty(request.deptid))
            {
                result.status = 2;
                result.message = L["DepartmentNotEmpty"];
                return result;
            }

            BDExpenseDept hasExpenseDept = await _BDExpenseDeptRepository.GetBDExpenseDept(request.company, request.deptid, "Y");
            if (hasExpenseDept != null)
            {
                result.status = 2;
                result.message = L["DepartmentExists"];
                return result;
            }

            BDExpenseDept bdExpenseDept = _ObjectMapper.Map<QueryBDVirtualDepartmentsDto, BDExpenseDept>(request);
            bdExpenseDept.cuser = userId;
            bdExpenseDept.muser = userId;
            bdExpenseDept.isvirtualdept = "Y";
            bdExpenseDept.cdate = System.DateTime.Now;
            bdExpenseDept.mdate = System.DateTime.Now;
            await _BDExpenseDeptRepository.InsertAsync(bdExpenseDept);
            result.message = L["SaveSuccessMsg"];

            return result;
        }

        public async Task<Result<string>> EditBDVirtualDepartment(QueryBDVirtualDepartmentsDto request, string userId)
        {
            Result<string> result = new Result<string>();
            result.status = 2;
            BDExpenseDept bdExpenseDept = await _BDExpenseDeptRepository.GetBDExpenseDept(request.company, request.deptid, "Y");
            if (bdExpenseDept != null) //有找到就更新
            {
                bdExpenseDept.company = request.company;
                bdExpenseDept.deptid = request.deptid;
                bdExpenseDept.muser = userId;
                bdExpenseDept.mdate = System.DateTime.Now;
                result.status = 1;
                result.message = L["SaveSuccessMsg"];
                await _BDExpenseDeptRepository.UpdateAsync(bdExpenseDept);
            }
            else //沒找到就新增
            {
                bdExpenseDept.cuser = userId;
                bdExpenseDept.muser = userId;
                bdExpenseDept.cdate = System.DateTime.Now;
                bdExpenseDept.mdate = System.DateTime.Now;
                await _BDExpenseDeptRepository.InsertAsync(bdExpenseDept);
                result.message = L["SaveSuccessMsg"];
            }

            return result;
        }

        public async Task<Result<string>> DeleteBDVirtualDepartment(List<Guid?> Ids)
        {
            Result<string> result = new Result<string>();
            List<BDExpenseDept> bdExpenseDept = await _BDExpenseDeptRepository.GetBDExpenseDeptByIds(Ids);
            foreach (var item in bdExpenseDept)
            {
                if (item == null)
                {
                    result.message = L["DeleteFail"] + "：" + L["DepartmentNotExists"];
                    result.status = 2;
                    return result;
                }
            }

            await _BDExpenseDeptRepository.DeleteManyAsync(bdExpenseDept);
            result.message = L["DeleteSuccess"];
            return result;
        }

        public async Task<byte[]> GetBDVirtualDepartmentExcelTemp()
        {
            byte[] data = null;
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet");
            string[] header =
            {
                L["company"],
                L["DeptId"],
                L["IsVirtualDept"]
            };
            IRow rowHeader = sheet.CreateRow(0);
            for (int i = 0; i < header.Length; i++)
            {
                rowHeader.CreateCell(i).SetCellValue("* " + header[i]);
            }
            IRow dataRow = sheet.CreateRow(1);
            dataRow.CreateCell(0).SetCellValue("WHQ");
            dataRow.CreateCell(1).SetCellValue("MLL1");
            dataRow.CreateCell(2).SetCellValue("N");

            dataRow = sheet.CreateRow(2);
            dataRow.CreateCell(0).SetCellValue("WHQ");
            dataRow.CreateCell(1).SetCellValue("MLL2");
            dataRow.CreateCell(2).SetCellValue("N");

            dataRow = sheet.CreateRow(3);
            dataRow.CreateCell(0).SetCellValue("WHQ");
            dataRow.CreateCell(1).SetCellValue("VLL1");
            dataRow.CreateCell(2).SetCellValue("Y");


            using MemoryStream ms = new();
            workbook.Write(ms);
            ms.Flush();
            data = ms.ToArray();

            return await Task.FromResult(data);
        }

        public async Task<byte[]> GetBDVirtualDepartmentExcelData(Request<QueryBDExpenseDeptDto> request)
        {
            byte[] data = null;
            if (request != null)
            {
                // 只查找虛擬部門
                List<BDExpenseDept> query = (await _BDExpenseDeptRepository.WithDetailsAsync())
                        .WhereIf(!string.IsNullOrEmpty(request.data.company), w => w.company == request.data.company.Trim())
                        .WhereIf(!string.IsNullOrEmpty(request.data.deptid), w => w.deptid == request.data.deptid.Trim())
                        .Where(w => w.isvirtualdept == "Y")
                        .ToList();

                XSSFWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("sheet");
                string[] header = new string[]
                {
                    "#",
                    L["company"],
                    L["DeptId"],
                    L["IsVirtualDept"]
                };
                IRow rowHeader = sheet.CreateRow(0);
                for (int i = 0; i < header.Length; i++)
                {
                    rowHeader.CreateCell(i).SetCellValue(header[i]);
                }
                for (int i = 0; i < query.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 1);
                    dataRow.CreateCell(0).SetCellValue("");
                    dataRow.CreateCell(1).SetCellValue(query[i].company);
                    dataRow.CreateCell(2).SetCellValue(query[i].deptid);
                    dataRow.CreateCell(3).SetCellValue(query[i].isvirtualdept);
                }
                using MemoryStream ms = new();
                workbook.Write(ms);
                ms.Flush();
                data = ms.ToArray();
            }
            return data;
        }

        public async Task<Result<string>> BatchUploadBDVirtualDepartment(IFormFile excelFile, string userId)
        {
            Result<string> result = new Result<string>();
            if (excelFile.ContentType == "application/vnd.ms-excel" || excelFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                DataTable dt = NPOIHelper.GetDataTableFromExcel(excelFile, 1);
                var list = dt.Rows.Cast<DataRow>()
                .Where(s => s[2].ToString().Trim() == "Y")
                .Select((s, i) => new
                {
                    company = s[0].ToString().Trim(),
                    deptid = s[1].ToString().Trim(),
                    isvirtualdept = s[2].ToString().Trim(),
                }).ToList();

                var processedItems = new HashSet<(string company, string deptid, string isvirtualdept)>();//去重
                List<BDExpenseDept> addBDExpenseDeptList = new();
                foreach (var item in list)
                {
                    var currentTuple = (item.company, item.deptid, item.isvirtualdept);
                    if (processedItems.Contains(currentTuple))
                    {
                        continue; //跳過重複資料
                    }
                    processedItems.Add(currentTuple); //紀錄已處理資料

                    BDExpenseDept BDExpenseDept = new()
                    {
                        company = item.company,
                        deptid = item.deptid,
                        isvirtualdept = item.isvirtualdept,
                        muser = userId,
                        mdate = DateTime.Now,
                    };
                    BDExpenseDept BDExpenseDept_Check = (await _BDExpenseDeptRepository.GetBDExpenseDept(item.company, item.deptid, item.isvirtualdept));
                    if (BDExpenseDept_Check == null) addBDExpenseDeptList.Add(BDExpenseDept);
                }
                if (addBDExpenseDeptList.Count > 0)
                    await _BDExpenseDeptRepository.InsertManyAsync(addBDExpenseDeptList);
                result.message = "success";
            }
            return result;
        }
    }
}
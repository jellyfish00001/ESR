using ERS.DTO;
using ERS.DTO.Nickname;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
namespace ERS.DomainServices
{
    public class CustomerNicknameDomainService : CommonDomainService, ICustomerNicknameDomainService
    {
        private ICustomerNicknameRepository _CustomerNicknameRepository;
        private IObjectMapper _ObjectMapper;
        public CustomerNicknameDomainService(ICustomerNicknameRepository CustomerNicknameRepository, IObjectMapper ObjectMapper)
        {
            _CustomerNicknameRepository = CustomerNicknameRepository;
            _ObjectMapper = ObjectMapper;
        }
        public async Task<Result<List<NickNameCommonDto>>> Get(Request<NickNameCommonDto> param)
        {
            Result<List<NickNameCommonDto>> result = new();
            if (param == null || param.data.companyList == null || param.data.companyList.Count == 0) return result;
            List<CustomerNickname> data = await (await _CustomerNicknameRepository.WithDetailsAsync()).Where(i => param.data.companyList.Contains(i.company)).WhereIf(!string.IsNullOrEmpty(param.data.name), i => param.data.name == i.name).AsNoTracking().ToListAsync();
            if (param.pageIndex <= 0 || param.pageSize < 0)
            {
                param.pageIndex = 1;
                param.pageSize = 10;
            }
            result.total = data.Count;
            data = data.Skip((param.pageIndex - 1) * param.pageSize).Take(param.pageSize).ToList();
            result.data = _ObjectMapper.Map<List<CustomerNickname>, List<NickNameCommonDto>>(data);
            return result;
        }
        public async Task<Result<string>> Add(NickNameCommonDto param, string cuser)
        {
            Result<string> result = new();
            var isExist = await (await _CustomerNicknameRepository.WithDetailsAsync()).Where(i => i.company == param.company && i.nickname.ToUpper() == param.nickname.ToUpper() && i.name.ToUpper() == param.name.ToUpper()).ToListAsync();
            if (isExist.Count > 0)
            {
                result.status = 2;
                result.message = L["AddFail"] + "" + L["Nickname-IsExist"];
                return result;
            }
            CustomerNickname data = new()
            {
                company = param.company,
                name = param.name,
                nickname = param.nickname,
                iscarry = param.iscarry,
                cuser = cuser,
                cdate = DateTime.Now
            };
            await _CustomerNicknameRepository.InsertAsync(data);
            return result;
        }
        public async Task<Result<string>> Update(NickNameCommonDto param, string cuser)
        {
            Result<string> result = new();
            result.status = 2;
            CustomerNickname data = await (await _CustomerNicknameRepository.WithDetailsAsync()).Where(i => i.Id == param.Id).FirstOrDefaultAsync();
            if (data == null)
            {
                result.message = "not exist";
                return result;
            }
            data.company = param.company;
            data.name = param.name;
            data.nickname = param.nickname;
            data.iscarry = param.iscarry;
            data.muser = cuser;
            data.mdate = DateTime.Now;
            var isExist = await (await _CustomerNicknameRepository.WithDetailsAsync()).Where(i => i.Id != param.Id && i.company == data.company && i.nickname == data.nickname && i.name == data.name).ToListAsync();
            if (isExist.Count > 0)
            {
                result.status = 2;
                result.message = L["UpdateFail"] + "" + L["Nickname-IsExist"];
                return result;
            }
            await _CustomerNicknameRepository.UpdateAsync(data);
            result.status = 1;
            return result;
        }
        public async Task<Result<string>> Delete(List<Guid?> param)
        {
            Result<string> result = new();
            result.status = 2;
            List<CustomerNickname> data = await (await _CustomerNicknameRepository.WithDetailsAsync()).Where(i => param.Contains(i.Id)).ToListAsync();
            if (data.Count == 0)
            {
                result.message = "not exist";
                return result;
            }
            await _CustomerNicknameRepository.DeleteManyAsync(data);
            result.status = 1;
            return result;
        }
        public async Task<Result<List<UploadNickNameDto>>> BatchUploadNickName(IFormFile excelFile, string userId)
        {
            Result<List<UploadNickNameDto>> result = new()
            {
                data = new()
            };
            if (excelFile.ContentType == "application/vnd.ms-excel" || excelFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                DataTable dt = GetDataTableFromExcel(excelFile);
                var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
                {
                    nickname = s[0].ToString().Trim(),
                    customername = s[1].ToString().Trim(),
                    iscarry = s[2].ToString().Trim()
                }).ToList();
                if(list.Count > 0)
                {
                    List<CustomerNickname> addList = new();
                    foreach(var item in list)
                    {
                        CustomerNickname customerNickname = new();
                        customerNickname.nickname = item.nickname;
                        customerNickname.name = item.customername;
                        customerNickname.iscarry = item.iscarry == "是" ? "Y" : (item.iscarry == "否" ? "N" : String.Empty);
                        customerNickname.company =  "WTZS";
                        customerNickname.cdate = System.DateTime.Now;
                        customerNickname.cuser = userId;
                        addList.Add(customerNickname);
                    }
                    await _CustomerNicknameRepository.InsertManyAsync(addList);
                }
            }
            return result;
        }
        static DataTable GetDataTableFromExcel(IFormFile file, int checkCellNum = 0)
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDInvoiceRail;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Volo.Abp.ObjectMapping;
using Microsoft.AspNetCore.Http;
using System.Data;
using NPOI.SS.UserModel;
namespace ERS.DomainServices
{
    public class BDInvoiceRailDomainService : CommonDomainService, IBDInvoiceRailDomainService
    {
        private IBDInvoiceRailRepository _BDInvoiceRailRepository;
        private IEmployeeRepository _EmployeeRepository;
        private IObjectMapper _ObjectMapper;
        public BDInvoiceRailDomainService(
            IBDInvoiceRailRepository BDInvoiceRailRepository,
            IEmployeeRepository EmployeeRepository,
            IObjectMapper ObjectMapper
        )
        {
            _BDInvoiceRailRepository = BDInvoiceRailRepository;
            _EmployeeRepository = EmployeeRepository;
            _ObjectMapper = ObjectMapper;
        }

        public  async Task<Result<List<BDInvoiceRailDto>>> GetPageBDInvoiceRails(Request<QueryBDInvoiceRailDto> request)
        {
            Result<List<BDInvoiceRailDto>> result = new Result<List<BDInvoiceRailDto>>()
            {
                data = new List<BDInvoiceRailDto>()
            };
            List<BDInvoiceRail> BDInvoiceRailList = await _BDInvoiceRailRepository.GetBDInvoiceRailList();
            List<BDInvoiceRailDto> BDInvoiceRailDtos = _ObjectMapper.Map<List<BDInvoiceRail>, List<BDInvoiceRailDto>>(BDInvoiceRailList);
            BDInvoiceRailDtos = BDInvoiceRailDtos
            .WhereIf(!String.IsNullOrEmpty(request.data.invoicerail), w => w.invoicerail == request.data.invoicerail)
            .WhereIf(!String.IsNullOrEmpty(request.data.invoicetype), w => w.invoicetype == request.data.invoicetype)
            .WhereIf((request.data.year != 0 && !String.IsNullOrEmpty(request.data.year.ToString())), w => w.year == request.data.year)
            .WhereIf((request.data.month != 0 && !String.IsNullOrEmpty(request.data.month.ToString())), w => w.month == request.data.month)
            .ToList();
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            result.data = BDInvoiceRailDtos.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = BDInvoiceRailDtos.Count;
            return result;
        }

        public async Task<Result<string>> DeleteBDInvoiceRails(List<Guid> Ids)
        {
            Result<string> result = new Result<string>();
            List<BDInvoiceRail> BDInvoiceRails = await _BDInvoiceRailRepository.GetBDInvoiceRailListByIds(Ids);
            if (BDInvoiceRails.Count == 0)
            {
                result.status = 2;
                result.message = L["DeleteFail"] + "：" + L["BDInvoiceRail-NotFound"];
                return result;
            }
            //改?修改??
            //await _BDInvoiceRailRepository.DeleteManyAsync(BDInvoiceRails);
            result.message = L["DeleteSuccess"];
            return result;
        }

        public async Task<Result<List<AddBDInvoiceRailDto>>> BatchUploadBDInvoiceRail(IFormFile excelFile, string userId)
        {
            Result<List<AddBDInvoiceRailDto>> result = new Result<List<AddBDInvoiceRailDto>>()
            {
                data = new List<AddBDInvoiceRailDto>()
            };
            if (excelFile.ContentType == "application/vnd.ms-excel" || excelFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                DataTable dt = GetDataTableFromExcel(excelFile);
                var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
                {
                    qi = s[0].ToString().Trim(),
                    invoicerail = s[1].ToString().Trim(),
                    year = s[2].ToString().Trim(),
                    month = s[3].ToString().Trim(),
                    formatcode = s[4].ToString().Trim(),
                    invoicetype = s[5].ToString().Trim()
                }).ToList();
                if (list.Count > 0)
                {
                    List<AddBDInvoiceRailDto> addList = new List<AddBDInvoiceRailDto>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        if (!decimal.TryParse(item.year, out var year))
                        {
                            result.status = 2;
                            result.message = String.Format(L["FormatError"], i+1, item.year);
                            return result;
                        }
                        if (!decimal.TryParse(item.month, out var month))
                        {
                            result.status = 2;
                            result.message = String.Format(L["FormatError"], i + 1, item.month); 
                            return result;
                        }
                        if (!decimal.TryParse(item.formatcode, out var formatcode))
                        {
                            result.status = 2;
                            result.message = String.Format(L["FormatError"], i + 1, item.formatcode);
                            return result;
                        }
                        AddBDInvoiceRailDto addBDInvoiceRailDto = new AddBDInvoiceRailDto();
                        addBDInvoiceRailDto.qi = item.qi;
                        addBDInvoiceRailDto.invoicerail = item.invoicerail;
                        addBDInvoiceRailDto.year = Convert.ToInt32(item.year);
                        addBDInvoiceRailDto.month = Convert.ToInt32(item.month);
                        addBDInvoiceRailDto.formatcode = Convert.ToInt32(item.formatcode);
                        addBDInvoiceRailDto.invoicetype = item.invoicetype;
                        addList.Add(addBDInvoiceRailDto);
                    }
                    //先?除上?文件中年度的?料??据
                    List<decimal> yearList = addList.Select(w => w.year).Distinct().ToList();
                    List<BDInvoiceRail> BDInvoiceRailList = (await _BDInvoiceRailRepository.WithDetailsAsync())
                     .Where(w => yearList.Contains(w.year))
                     .ToList();
                    if (BDInvoiceRailList.Count > 0)
                    {
                        foreach (var item in BDInvoiceRailList)
                        {
                            item.isdeleted = true;
                            item.muser = userId;
                            item.mdate = System.DateTime.Now;
                        }
                        await _BDInvoiceRailRepository.UpdateManyAsync(BDInvoiceRailList);
                    }
                    //添加
                    List<BDInvoiceRail> iBDInvoiceRails = _ObjectMapper.Map<List<AddBDInvoiceRailDto>, List<BDInvoiceRail>>(addList);
                    iBDInvoiceRails.ForEach(w =>
                    {
                        w.cuser = userId;
                        w.cdate = System.DateTime.Now;
                    });
                    await _BDInvoiceRailRepository.InsertManyAsync(iBDInvoiceRails);
                    result.data = addList;
                }
            }
            else
            {
                result.message = L["BDInvoiceRail-UploadErrorFile"];
                result.status = 2;
            }
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
    }
}
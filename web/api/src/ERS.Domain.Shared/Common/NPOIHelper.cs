using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Reflection;



namespace ERS
{
    public class NPOIHelper
    {
        public static IList<string> GetColumnsName(DataTable dt)
        {
            IList<string> result = new List<string>();
            if (dt != null)
            {
                if (dt.Columns.Count > 0)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        result.Add(dt.Columns[i].ColumnName);
                    }
                }
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
                if (row != null)
                {
                    if (row.GetCell(checkCellNum) != null && row.GetCell(checkCellNum).ToString().Length > 0)
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
                                    Decimal value = Convert.ToDecimal(row.GetCell(k).NumericCellValue);
                                    dataRow[k] = value.ToString();
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
            }

            return dt;
        }

        public static byte[] ExportExcelByByte<T>(IList<T> entities, string headerColumns, int[] numbericColumns = null, int[] dateColums = null, string[] ignoreColumns = null)
        {
            MemoryStream ms = RenderToExcel(entities, headerColumns, numbericColumns, dateColums, ignoreColumns);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        public static MemoryStream RenderToExcel<T>(IList<T> entities, string headerColumns, int[] numbericColumns = null, int[] dateColums = null, string[] ignoreColumns = null, int[] commaColumns = null)
        {
            string[] thNameArr = headerColumns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            // sheet.DefaultColumnWidth = 30 * 256;
            sheet.DefaultRowHeight = 30 * 20;
            IRow dataRow = sheet.CreateRow(0);

            //设置单元格的样式
            ICellStyle style = workbook.CreateCellStyle();
            style.VerticalAlignment = VerticalAlignment.Center; //垂直对齐居中
            style.WrapText = true; //设置换行

            //设置单元格的样式 numbericStyle.DataFormat = XSSFDataFormat .xlsx
            ICellStyle numbericStyle = workbook.CreateCellStyle();
            numbericStyle.VerticalAlignment = VerticalAlignment.Center; //垂直对齐居中
            numbericStyle.Alignment = HorizontalAlignment.Right; //数字居右
                                                                 // numbericStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

            ICellStyle dateStyle = workbook.CreateCellStyle();
            dateStyle.VerticalAlignment = VerticalAlignment.Center; //垂直对齐居中
            dateStyle.Alignment = HorizontalAlignment.Left; //数字居右
            dateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("yyyy/MM/dd");
            for (int i = 0; i < thNameArr.Length; i++)
            {
                dataRow.CreateCell(i).SetCellValue(thNameArr[i].ToString());
                sheet.SetColumnWidth(i, 16 * 300);
            }

            //获取 实体类 类型对象
            Type t = typeof(T); // model.GetType();
                                //获取 实体类 所有的 公有属性
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            for (int i = 0; i < entities.Count; i++)
            {
                dataRow = sheet.CreateRow(i + 1); //创建行
                dataRow.HeightInPoints = 20;
                int columnIndex = 0;
                foreach (var item in proInfos)
                {
                    if (ignoreColumns != null && ignoreColumns.Contains(item.Name)) continue;
                    ICell icell = dataRow.CreateCell(columnIndex);
                    object value = item.GetValue(entities[i], null); //object newValue = model.uName;
                    string cell_value = value == null ? "" : (value.GetType().Name == "List`1" ? String.Join(",", (List<string>)value) : value.ToString());
                    if(commaColumns != null && numbericColumns.Contains(columnIndex) && !string.IsNullOrEmpty(cell_value))
                    {
                        icell.CellStyle = numbericStyle;
                        var tempValue = Convert.ToDecimal(cell_value);
                        icell.SetCellValue(string.Format("{0:N2}", tempValue));
                    }
                    else if (numbericColumns != null && numbericColumns.Contains(columnIndex) && !string.IsNullOrEmpty(cell_value))
                    {
                        icell.CellStyle = numbericStyle;
                        icell.SetCellValue(Convert.ToDouble(cell_value));
                    }
                    else if (dateColums != null && dateColums.Contains(columnIndex) && !string.IsNullOrEmpty(cell_value))
                    {
                        DateTime dateV;
                        DateTime.TryParse(cell_value, out dateV);
                        cell_value = dateV.ToString("yyyy/M/d");
                        icell.SetCellValue(cell_value);
                    }
                    else
                    {
                        icell.CellStyle = style;
                        icell.SetCellValue(cell_value);
                    }

                    columnIndex++;
                }
            }

            var ms = new NpoiMemoryStream();
            ms.AllowClose = false;
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }
    }

    public class NpoiMemoryStream : MemoryStream
    {
        public NpoiMemoryStream()
        {
            AllowClose = true;
        }

        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
                base.Close();
        }
    }
}

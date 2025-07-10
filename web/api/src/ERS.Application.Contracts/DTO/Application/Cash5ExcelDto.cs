using System;

namespace ERS.DTO.Application
{
    public class Cash5ExcelDto
    {
        public string? rno { get; set; }  //報銷單號
        public string? senarioname { get; set; }  //報銷情境
        public string deptid { get; set; }  //費用歸屬部門
        public string? agentemplid { get; set; }  //承辦人
        public string? unifycode { get; set; }  //廠商統一編號
        public string? billnoandsummary { get; set; }  //提單號碼/費用摘要
        public string? reportno { get; set; }  //報單號碼
        public string? invoice { get; set; }  //稅單號碼/發票號碼/憑證號碼
        public DateTime? rdate { get; set; }  //稅單付訖日期/發票日期/憑證日期
        public decimal? invoiceAmountBeforeTax { get; set; }  //發票稅前金額 = 進口稅/其他費用 + 推貿費
        public decimal? importtax { get; set; } //進口稅/其他費用
        public decimal? tradepromotionfee { get; set; } //推貿費
        public decimal? taxexpense { get; set; }  //營業稅
        public decimal? totaltaxandfee { get; set; }  //稅費合計
        public decimal? taxbaseamount { get; set; }  //稅基
        public string? expensecode { get; set; }  //費用類別
        public string? accountcode { get; set; }  //會計科目
    }
}
using System;
using System.Collections.Generic;

namespace ERS.DTO.BDSignlevel
{
    public class BDSignlevelParamDto
    {
        public Guid? Id { get; set; }
        public string company { get; set; } //公司別
        public List<string> companyList { get; set; } //公司別
        public string item { get; set; } //呈核項目
        public string signlevel { get; set; } //核決權限
        public decimal money { get; set; } //簽核金額
        public string currency { get; set; } //幣別
        public string language { get; set; } //語系
    }
}
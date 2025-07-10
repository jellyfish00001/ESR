using System;

namespace ERS.DTO.BDSignlevel
{
    public class QueryBDSignlevelDto
    {
        public Guid? Id { get; set; }
        public string company { get; set; } //公司別
        public string item { get; set; } //呈核項目
        public string signlevel { get; set; } //簽核層級
        public decimal money { get; set; } //簽核金額
        public string currency { get; set; } //幣別
        public bool isdeleted { get; set; } //是否已刪除
        public DateTime? mdate { get; set; } //修改時間
        public DateTime? cdate { get; set; } //建立時間
        public string cuser { get; set; } //建立人
        public string muser { get; set; } //修改人

    }
}
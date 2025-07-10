using System;
namespace ERS.DTO.BDCash
{
    public class BDCashDto
    {
        public Guid? Id { get; set; }
        public string company { get; set; } // 公司別
        public string rno { get; set; } // 預支金單號
        public decimal amount { get; set; }//冲账回缴金额
        public string cuser { get; set; }//创建人
        public string muser { get; set; }
        public DateTime? cdate { get; set; }//创建时间
        public DateTime? mdate { get; set; }//更新时间
    }
}
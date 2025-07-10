namespace ERS.DTO.Report
{
    //宴客超标明细报表dto
    public class EntertainOverspendDetailDto
    {
        public string company { get; set; }//公司別
        public string payeename { get; set; }//報銷人
        public string payeeId { get; set; }//報銷人工號
        public string rno { get; set; }//報銷單號
        public string docno { get; set; }//傳票號碼
        public string postdate { get; set; }//入賬日期
        public string chargesite { get; set; }//掛賬Site
        public string expdeptid { get; set; }//掛賬部門
        public decimal? overbudget { get; set; }//扣減部門年度績效之金額
        public string FinancialPic { get; set; }//财务pic
    }
}
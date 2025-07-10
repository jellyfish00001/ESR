namespace ERS.Application.Contracts.DTO.Application
{
    public class CashResult
    {
        public string rno { get; set; }
        public bool Stat { get; set; }
        // 存在电子发票 & 非pdf格式
        /// <summary>
        /// 财务加签
        /// </summary>
        public bool ElecInvStat { get; set; }
        public bool ElecButNotPdf { get; set; }
        public string message { get; set; }
    }
}
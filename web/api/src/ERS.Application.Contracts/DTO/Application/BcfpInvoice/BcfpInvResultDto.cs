namespace ERS.DTO.Application.BcfpInvoice
{
    /// <summary>
    /// 存放获取到的发票信息（区块链电子发票）
    /// </summary>
    public class BcfpInvResultDto
    {
        public int retcode { get; set; }
        public string retmsg { get; set; }
        public BillRecordDto bill_record { get; set; }
    }
}
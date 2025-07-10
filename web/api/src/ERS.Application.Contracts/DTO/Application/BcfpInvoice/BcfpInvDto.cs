namespace ERS.DTO.Application.BcfpInvoice
{
    /// <summary>
    /// 获取区块链电子票请求dto
    /// </summary>
    public class BcfpInvDto
    {
        public string tx_hash { get; set; }
        public string total_amount { get; set; }
        public string bill_num { get; set; }
    }
}
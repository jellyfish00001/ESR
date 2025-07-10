namespace ERS.DTO.BDInvoiceType
{
    /// <summary>
    /// 发票类型输入参数校验Dto
    /// </summary>
    public class InvTypeCheckDto
    {
        public string invtypecode { get; set; }
        public string invtype { get; set; }
        public string area { get; set; }
        public string category { get; set; }
        public string company { get; set; }
    }
}
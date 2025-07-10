namespace ERS.DTO.BDTicketRail
{
    /// <summary>
    /// 发票类型输入参数校验Dto
    /// </summary>
    public class TicketRailCheckDto
    {
        public string invcode { get; set; }
        public string invtype { get; set; }
        public string company { get; set; }
    }
}
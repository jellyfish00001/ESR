namespace ERS.DTO.Application.CashX
{
    public class PayWayDto
    {
        /// <summary>
        /// 请款方式代码 E：现金 T：汇款
        /// </summary>
        /// <value></value>
        public string paytype { get; set; }
        /// <summary>
        /// 请款方式名称
        /// </summary>
        /// <value></value>
        public string payname { get; set; }
    }
}
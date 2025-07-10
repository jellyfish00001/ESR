namespace ERS.DTO
{
    public class UpdatePayStatDto
    {
        /// <summary>
        /// 發票代碼
        /// </summary>
        /// <value></value>
        public string invcode { get; set; }
        /// <summary>
        /// 發票號碼
        /// </summary>
        /// <value></value>
        public string invno { get; set; }
        /// <summary>
        /// 請款狀態：P01（已請款）、P02（待請款）
        /// </summary>
        /// <value></value>
        //public string paymentstat { get; set; }
    }
}
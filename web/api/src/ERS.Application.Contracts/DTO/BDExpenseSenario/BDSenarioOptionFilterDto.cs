namespace ERS.DTO.BDExpenseSenario
{
    /// <summary>
    /// 報銷情景選項查詢篩選 DTO
    /// </summary>
    public class BDSenarioOptionFilterDto
    {
        /// <summary>
        /// 公司別
        /// </summary>
        public string Companycategory { get; set; }

        /// <summary>
        /// 關鍵字
        /// </summary>
        public string Keyword { get; set; }
    }
}

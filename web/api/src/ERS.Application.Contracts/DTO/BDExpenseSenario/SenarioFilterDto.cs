namespace ERS.DTO.BDExpenseSenario
{
    /// <summary>
    /// 報銷情景查詢篩選 DTO
    /// </summary>
    public class BDSenarioFilterDto
    {
        /// <summary>
        /// 公司別
        /// </summary>
        public string CompanyCategory { get; set; }

        /// <summary>
        /// 報銷情景名稱
        /// </summary>
        public string SenarioName { get; set; }

        /// <summary>
        /// 費用類別代碼
        /// </summary>
        public string ExpenseCode { get; set; }
    }
}

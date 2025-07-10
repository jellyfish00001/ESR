using System;

/**************************************************************************
 * Author: Andy YA Chung
 * Date: 2025-05-21
 * Name: 報銷情景選項 DTO
 * Description:
 **************************************************************************/

namespace ERS.DTO.BDExpenseSenario
{
    /// <summary>
    /// 報銷情景選項 DTO
    /// </summary>
    public class BDSenarioOptionDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 公司別
        /// </summary>
        public string companycategory { get; set; }

        /// <summary>
        /// 報銷情景名稱
        /// </summary>
        public string senarioname { get; set; }

        /// <summary>
        /// 關鍵字
        /// </summary>
        public string keyword { get; set; }

        /// <summary>
        /// 費用類別代碼
        /// </summary>
        public string expensecode { get; set; }
    }
}

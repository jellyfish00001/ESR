using System;

namespace ERS.UberToERS.Model
{
    /// <summary>
    /// 員工資料
    /// </summary>
    /// <remarks>
    /// 這個類別用來表示Uber員工的資料結構，包含了員工的基本資訊和一些可選的屬性。
    /// </remarks>
    public class uber_employees
    {
        public DateTime? log_datetime { get; set; } //記錄日期時間(UTC)
        public string first_name { get; set; } //名字
        public string last_name { get; set; } //姓氏
        public string email { get; set; } //電子郵件
        public string employee_id { get; set; } //員工ID（可選
        public string group { get; set; } //組（可選）;
        public string reviewer { get; set; } //審閱者（可選）
        public string mobile_country_code { get; set; } //移動國家/地區代碼（可選）
        public string mobile_number { get; set; } //手機號碼（可選）
        public DateTime? termination_dt { get; set; } //離職日期
    }
}

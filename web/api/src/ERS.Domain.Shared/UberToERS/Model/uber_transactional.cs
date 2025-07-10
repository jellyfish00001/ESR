using System;

namespace ERS.UberToERS.Model
{
    enum transactional_Field
    { 
        trip_id,
        transaction_timestamp,
        request_date_utc,
        request_time_utc,
        request_date,
        request_time,
        drop_off_date_utc,
        drop_off_time_utc,
        drop_off_date,
        drop_off_time,
        request_timezone_offset_from_utc,
        first_name,
        last_name,
        email,
        employee_id,
        service,
        city,
        distance,
        duration,
        pickup_address,
        drop_off_address,
        expense_code,
        expense_memo,
        invoices,
        program,
        group,
        payment_method,
        transaction_type,
        fare_in_local_currency,
        taxes_in_local_currency,
        tip_in_local_currency,
        transaction_amount_in_local_currency,
        local_currency_code,
        fare_in_home_currency,
        taxes_in_home_currency,
        tip_in_home_currency,
        transaction_amount_in_home_currency,
        estimated_service_and_technology_fee
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class uber_transactional
    {
        public Guid trip_id { get; set; }  // Trip/Eats ID
        public DateTime? transaction_timestamp { get; set; }  // 交易時間戳 （UTC）
        public DateOnly? request_date_utc { get; set; }  // 预约日期 （UTC）
        public string request_time_utc { get; set; }  // 预约時間 （UTC）
        public DateOnly? request_date { get; set; }  // 预约日期 （本地）
        public string request_time { get; set; }  // 预约時間 （本地）
        public DateTime? drop_off_date_utc { get; set; }  // 下车日期 （UTC）
        public string drop_off_time_utc { get; set; }  // 下车時間 （UTC）
        public DateOnly? drop_off_date { get; set; }  // 下车日期 （當地）
        public string drop_off_time { get; set; }  // 下车時間 （當地）
        public string request_timezone_offset_from_utc { get; set; }  // 請求與UTC的時區偏移量
        public string first_name { get; set; }  // 名字
        public string last_name { get; set; }  // 姓
        public string email { get; set; }  // 電子郵件
        public string employee_id { get; set; }  // 員工ID
        public string service { get; set; }  // 服務
        public string city { get; set; }  // 城市
        public decimal? distance { get; set; }  // 距離 （mi）
        public int duration { get; set; }  // 总時間 （min）
        public string pickup_address { get; set; }  // 上车位址
        public string drop_off_address { get; set; }  // 下车位址
        public string expense_code { get; set; }  // 費用代碼
        public string expense_memo { get; set; }  // 費用備忘錄
        public string invoices { get; set; }  // 發票
        public string program { get; set; }  // 方案
        public string group { get; set; }  // 群
        public string payment_method { get; set; }  // 付款方式
        public string transaction_type { get; set; }  // 交易類型
        public decimal? fare_in_local_currency { get; set; }  // 以當地貨幣計算的票價（不含稅）
        public decimal? taxes_in_local_currency { get; set; }  // 以當地貨幣表示的稅費
        public decimal? tip_in_local_currency { get; set; }  // 以當地貨幣打賞
        public decimal? transaction_amount_in_local_currency { get; set; }  // 以當地貨幣表示的交易金額（含稅）
        public string local_currency_code { get; set; }  // 本地貨幣代碼
        public decimal? fare_in_home_currency { get; set; }  // 以本國貨幣計算的票價（不含稅）
        public decimal? taxes_in_home_currency { get; set; }  // 本幣稅費
        public decimal? tip_in_home_currency { get; set; }  // 本幣中的小費
        public decimal? transaction_amount_in_home_currency { get; set; }  // 以本幣表示的交易金額（含稅）
        public decimal? estimated_service_and_technology_fee { get; set; }  // 以本國貨幣計算的預計服務費和技術費用（包括稅費，如有）
    }
    public class uber_transactional_CommDB
    {
        public Guid id { get; set; }
        public string rno { get; set; }  // 申请单号
        public string sign_status { get; set; }  // 签核状态(P,已送单;A,已签核;R,已驳回)
        public DateTime? mdate { get; set; }  // 修改時間
        public DateTime? cdate { get; set; }  // 新增時間
        public string cuser { get; set; }  // 新增者
        public string muser { get; set; }  // 修改者
        public bool isdeleted { get; set; }  // 是否刪除
        public string file_name { get; set; }  // 资料来源文件名
    }
}

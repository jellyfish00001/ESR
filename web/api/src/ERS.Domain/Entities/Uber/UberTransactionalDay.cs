using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ERS.Entities.Uber
{
    [Table("uber_transactional_day")]
    public class UberTransactionalDay : SuperBaseEntity
    {
        [Column("trip_id")]
        [Comment("Trip/Eats ID")]
        public Guid? TripId { get; set; }

        [Column("transaction_timestamp")]
        [Comment("交易時間戳 （UTC）")]
        public DateTimeOffset? TransactionTimestamp { get; set; }

        [Column("request_date_utc")]
        [Comment("预约日期 （UTC）")]
        public DateTime? RequestDateUtc { get; set; }

        [StringLength(10)]
        [Column("request_time_utc")]
        [Comment("预约時間 （UTC）")]
        public string RequestTimeUtc { get; set; }

        [Column("request_date")]
        [Comment("预约日期 （本地）")]
        public DateTime? RequestDate { get; set; }

        [StringLength(10)]
        [Column("request_time")]
        [Comment("预约時間 （本地）")]
        public string RequestTime { get; set; }

        [Column("drop_off_date_utc")]
        [Comment("下车日期 （UTC）")]
        public DateTimeOffset? DropOffDateUtc { get; set; }

        [StringLength(10)]
        [Column("drop_off_time_utc")]
        [Comment("下车時間 （UTC）")]
        public string DropOffTimeUtc { get; set; }

        [Column("drop_off_date")]
        [Comment("下车日期 （當地）")]
        public DateTime? DropOffDate { get; set; }

        [StringLength(10)]
        [Column("drop_off_time")]
        [Comment("下车時間 （當地）")]
        public string DropOffTime { get; set; }

        [StringLength(6)]
        [Column("request_timezone_offset_from_utc")]
        [Comment("請求與UTC的時區偏移量")]
        public string RequestTimezoneOffsetFromUtc { get; set; }

        [StringLength(50)]
        [Column("first_name")]
        [Comment("名字")]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Column("last_name")]
        [Comment("姓")]
        public string LastName { get; set; }

        [StringLength(50)]
        [Column("email")]
        [Comment("電子郵件")]
        public string Email { get; set; }

        [StringLength(11)]
        [Column("employee_id")]
        [Comment("員工ID")]
        public string EmployeeId { get; set; }

        [StringLength(50)]
        [Column("service")]
        [Comment("服務")]
        public string Service { get; set; }

        [StringLength(50)]
        [Column("city")]
        [Comment("城市")]
        public string City { get; set; }

        [Column("distance")]
        [Comment("距離 （mi）")]
        public decimal? Distance { get; set; }

        [Column("duration")]
        [Comment("总時間 （min）")]
        public long? Duration { get; set; }

        [StringLength(200)]
        [Column("pickup_address")]
        [Comment("上车位址")]
        public string PickupAddress { get; set; }

        [StringLength(200)]
        [Column("drop_off_address")]
        [Comment("下车位址")]
        public string DropOffAddress { get; set; }

        [StringLength(10)]
        [Column("expense_code")]
        [Comment("費用代碼")]
        public string ExpenseCode { get; set; }

        [StringLength(200)]
        [Column("expense_memo")]
        [Comment("費用備忘錄")]
        public string ExpenseMemo { get; set; }

        [StringLength(50)]
        [Column("invoices")]
        [Comment("發票")]
        public string Invoices { get; set; }

        [StringLength(20)]
        [Column("program")]
        [Comment("方案")]
        public string Program { get; set; }

        [Required]
        [StringLength(6)]
        [Column("group")]
        [Comment("群")]
        public string Group { get; set; }

        [StringLength(50)]
        [Column("payment_method")]
        [Comment("付款方式")]
        public string PaymentMethod { get; set; }

        [StringLength(50)]
        [Column("transaction_type")]
        [Comment("交易類型")]
        public string TransactionType { get; set; }

        [Column("fare_in_local_currency")]
        [Comment("以當地貨幣計算的票價（不含稅）")]
        public decimal? FareInLocalCurrency { get; set; }

        [Column("taxes_in_local_currency")]
        [Comment("以當地貨幣表示的稅費")]
        public decimal? TaxesInLocalCurrency { get; set; }

        [Column("tip_in_local_currency")]
        [Comment("以當地貨幣打賞")]
        public decimal? TipInLocalCurrency { get; set; }

        [Column("transaction_amount_in_local_currency")]
        [Comment("以當地貨幣表示的交易金額（含稅）")]
        public decimal? TransactionAmountInLocalCurrency { get; set; }

        [StringLength(6)]
        [Column("local_currency_code")]
        [Comment("本地貨幣代碼")]
        public string LocalCurrencyCode { get; set; }

        [Column("fare_in_home_currency")]
        [Comment("以本國貨幣計算的票價（不含稅）")]
        public decimal? FareInHomeCurrency { get; set; }

        [Column("taxes_in_home_currency")]
        [Comment("本幣稅費")]
        public decimal? TaxesInHomeCurrency { get; set; }

        [Column("tip_in_home_currency")]
        [Comment("本幣中的小費")]
        public decimal? TipInHomeCurrency { get; set; }

        [Column("transaction_amount_in_home_currency")]
        [Comment("以本幣表示的交易金額（含稅）")]
        public decimal? TransactionAmountInHomeCurrency { get; set; }

        [Column("estimated_service_and_technology_fee")]
        [Comment("以本國貨幣計算的預計服務費和技術費用（包括稅費，如有）")]
        public decimal? EstimatedServiceAndTechnologyFee { get; set; }

        [StringLength(20)]
        [Comment("申请单号")]
        public string rno { get; set; }

        [StringLength(1)]
        [Comment("签核状态(P,已送单;A,已签核;R,已驳回)")]
        [Column("sign_status")]
        public string SignStatus{ get; set; }

        [StringLength(50)]
        [Comment("资料来源文件名")]
        [Column("file_name")]
        public string FileName { get; set; }

        [StringLength(20)]
        [Comment("公司别")]
        [Column("company")]
        public string Company { get; set; }
    }
}

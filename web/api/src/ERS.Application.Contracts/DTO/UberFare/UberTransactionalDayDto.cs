using System;

namespace ERS.DTO.UberFare
{
    public class UberTransactionalDayDto
    {
        public Guid? TripId { get; set; }
        public DateTimeOffset? TransactionTimestamp { get; set; }
        public DateTime? RequestDateUtc { get; set; }
        public string RequestTimeUtc { get; set; }
        public DateTime? RequestDate { get; set; }
        public string RequestTime { get; set; }
        public DateTimeOffset? DropOffDateUtc { get; set; }
        public string DropOffTimeUtc { get; set; }
        public DateTime? DropOffDate { get; set; }
        public string DropOffTime { get; set; }
        public string RequestTimezoneOffsetFromUtc { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string EmployeeId { get; set; }
        public string Service { get; set; }
        public string City { get; set; }
        public decimal? Distance { get; set; }
        public long? Duration { get; set; }
        public string PickupAddress { get; set; }
        public string DropOffAddress { get; set; }
        public string ExpenseCode { get; set; }
        public string ExpenseMemo { get; set; }
        public string Invoices { get; set; }
        public string Program { get; set; }
        public string Group { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionType { get; set; }
        public decimal? FareInLocalCurrency { get; set; }
        public decimal? TaxesInLocalCurrency { get; set; }
        public decimal? TipInLocalCurrency { get; set; }
        public decimal? TransactionAmountInLocalCurrency { get; set; }
        public string LocalCurrencyCode { get; set; }
        public decimal? FareInHomeCurrency { get; set; }
        public decimal? TaxesInHomeCurrency { get; set; }
        public decimal? TipInHomeCurrency { get; set; }
        public decimal? TransactionAmountInHomeCurrency { get; set; }
        public decimal? EstimatedServiceAndTechnologyFee { get; set; }
        public string rno { get; set; }
        public string SignStatus { get; set; }
        public string FileName { get; set; }
    }
}

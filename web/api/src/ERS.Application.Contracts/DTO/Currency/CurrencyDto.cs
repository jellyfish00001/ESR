namespace ERS.Application.Contracts.DTO.Currency
{
    public class CurrencyDto
    {
        //本位币
        public string basecurr { get; set; }
        //汇率
        public decimal? rate { get; set; }
        //币别
        public string currency { get; set; }
        //描述
        public string currency_desc { get; set; }
    }
}
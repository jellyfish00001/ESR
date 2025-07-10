using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Application.Contracts.DTO.Currency
{
    public interface ICurrencyService
    {
        Task<List<CurrencyDto>> GetQueryCurrency(string userId);
        Task<decimal> queryRate(string ccurfrom, string ccurto);
        Task<decimal> queryRate2(string ccurfrom, string ccurto);
    }
}
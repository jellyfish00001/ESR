using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Currency;
using ERS.Entities;
using Volo.Abp.Domain.Services;
namespace ERS.Domain.IDomainServices
{
    public interface ICurrencyDomainService: IDomainService
    {
        //根据公司别获取币别
        Task<List<CurrencyDto>> GetQueryCurrency(string userId);
        Task<SAPExchRate> queryRate(string ccurfrom, string ccurto);
    }
}
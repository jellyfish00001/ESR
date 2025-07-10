using ERS.Application.Contracts.DTO.Application;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.Application
{
    public interface ICash2Service
    {
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token);
        Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token);
        Task<Result<ApplicationDto>> GetQueryApplications(string rno);

        Task<Result<string>> UpdateCashDetailRequestAmount(IFormCollection formCollection);

        Task<Result<string>> UpdateCashDetailRemark(CashDetailDto cashDetailDto);

        Task<Result<string>> UpdateInvoiceData(IFormCollection formCollection);
    }
}

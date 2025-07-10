using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Application;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Domain.Services;
namespace ERS.Domain.IDomainServices
{
    public interface IApplicationDomainService : IDomainService
    {
        Task<Result<ApplicationDto>> QueryApplications(string rno);

        Task<Result<string>> UpdateCashDetailRequestAmount(IFormCollection formCollection);

        Task<Result<string>> UpdateCashDetailRemark(CashDetailDto cashDetailDto);

        Task<Result<string>> UpdateInvoice(IFormCollection formCollection);
    }
}
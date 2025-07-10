using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface ICash2DomainService : IDomainService
    {
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P");
    }
}

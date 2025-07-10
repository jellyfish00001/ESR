using Volo.Abp.Domain.Services;
using ERS.DTO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
namespace ERS.IDomainServices
{
    public interface ICash2ADomainService: IDomainService
    {
             Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P");
    }
}
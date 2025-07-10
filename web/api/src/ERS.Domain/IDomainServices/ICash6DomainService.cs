using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using Microsoft.AspNetCore.Http;
namespace ERS.IDomainServices
{
    public interface ICash6DomainService
    {
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P");
    }
}
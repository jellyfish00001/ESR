using System.Threading.Tasks;
using ERS.DTO;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface IEmpChsDomainService : IDomainService
    {
        Task<Result<string>> UploadBankAccountName(IFormFile excelFile);
    }
}
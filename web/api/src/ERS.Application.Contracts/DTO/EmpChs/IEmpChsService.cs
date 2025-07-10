using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace ERS.DTO.EmpChs
{
    public interface IEmpChsService
    {
        Task<Result<string>> UploadBankAccountName(IFormFile excelFile);
    }
}
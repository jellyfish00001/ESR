using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace ERS.Application.Contracts.DTO.CashFile
{
    public interface ICashFileService
    {
        Task<string> add(IFormCollection formCollection, string user);
        Task<string> delete(CashFileDto cashFile, string user);
    }
}
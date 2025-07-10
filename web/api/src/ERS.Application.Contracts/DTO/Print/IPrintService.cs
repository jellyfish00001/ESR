using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.Print
{
    public interface IPrintService
    {
        Task<Result<List<PrintDto>>> GetQueryPagePrint(Request<PrintQueryDto> request);
        Task<Result<string>> GetPrintAsync(List<string> rnolist, string token);

        Result<string> StoreFormDetails(List<string> rnolist, string token, string userId);
        // Task<byte[]> GetPrintPDFData(List<string> rnolist);
    }
}
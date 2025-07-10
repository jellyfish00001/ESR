using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;

namespace ERS.IDomainServices
{
    public interface IOcrResultDomainService
    {
        Task<Result<List<OcrResultDto>>> Search(Request<OcrResultDto> request);
        Task<Result<string>> Create(OcrResultDto model, string userId);
        Task<Result<string>> Update(OcrResultDto model, string userId);
        Task<Result<string>> Delete(List<string> ids);
    }
}

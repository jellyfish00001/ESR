using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.PJcode;
namespace ERS.Application.Contracts.DTO.PmcsPjcode
{
    public interface IPmcsPjcodeService
    {
        Task<List<PjcodeDto>> QueryPjcode(string code, string company);
        Task<string> CopyPjcode(string company);
    }
}
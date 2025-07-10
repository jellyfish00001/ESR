using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO
{
    public interface IHelpManualService
    {
        Task<Result<List<HelpManualDto>>> Get(string company, string userId);
    }
}

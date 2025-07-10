using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.DataSync
{
    public interface ICashDataSyncService
    {
        Task testSync(List<string> rnos);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface ICashDataSyncDomainService : IDomainService
    {
        Task testSync(List<string> rnos);
    }
}
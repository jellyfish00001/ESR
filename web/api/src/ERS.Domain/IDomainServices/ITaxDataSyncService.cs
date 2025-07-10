using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface ITaxDataSyncService : IDomainService
    {
        Task DownloadTaxDataAndSync();
    }
}
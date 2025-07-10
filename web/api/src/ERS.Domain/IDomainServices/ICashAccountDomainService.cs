using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface ICashAccountDomainService : IDomainService
    {
        Task CashAccountDataSync();
    }
}

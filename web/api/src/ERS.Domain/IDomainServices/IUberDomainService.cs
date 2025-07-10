using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface IUberDomainService : IDomainService
    {
        Task TransactionToSign();
    }
}
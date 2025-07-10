using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface IMinioDomainService : IDomainService
    {
        Task<string> PutObjectAsync(string rno, string filename, Stream data, string contentType, string area);
        Task<string> PutInvoiceAsync(string invno, string filename, Stream data, string contentType, string area);
        Task<string> CopyInvoiceAsync(string rno, string srcObjectName, string invno, string area);
        Task<string> GetMinioArea(string userId);
    }
}

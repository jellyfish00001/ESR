using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using Volo.Abp.Domain.Services;
namespace ERS.Domain.IDomainServices
{
    public interface ICashFileDomainService: IDomainService
    {
        Task<string> add(List<CashFile> cashFile);
         Task<string> delete(CashFile cashFile);
        Task<CashFile> query(CashFile cashFile);
    }
}
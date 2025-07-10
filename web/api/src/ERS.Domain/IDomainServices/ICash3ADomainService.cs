using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface ICash3ADomainService : IDomainService
    {
        //查询未冲账数据
         Task<Result<List<DeferredDto>>> DeferredQuery(string user);
        Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P");
    }
}

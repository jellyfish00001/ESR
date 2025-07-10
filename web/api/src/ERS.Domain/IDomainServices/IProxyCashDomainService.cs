using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Proxy;
using Volo.Abp.Domain.Services;
namespace ERS.IDomainServices
{
    public interface IProxyCashDomainService : IDomainService
    {
        Task<Result<string>> AddProxyCash(AddProxyCashDto request, string userId);
        Task<Result<List<ProxyCashDto>>> QueryProxyCash(Request<QueryProxyCashDto> request, string userId);
        Task<Result<string>> DeleteProxyCash(List<Guid?> ids);
        Task<Result<string>> EditProxyCash(EditProxyCashDto request, string userId);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.Proxy
{
    public interface IProxyCashService
    {
        Task<Result<string>> AddProxyCash(AddProxyCashDto request, string userId);
        Task<Result<List<ProxyCashDto>>> QueryProxyCash(Request<QueryProxyCashDto> request, string userId);
        Task<Result<string>> DeleteProxyCash(List<Guid?> ids);
        Task<Result<string>> EditProxyCash(EditProxyCashDto request, string userId);
    }
}
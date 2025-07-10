using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Proxy;
using ERS.IDomainServices;
using Volo.Abp.Application.Services;
namespace ERS.Services.Proxy
{
    public class ProxyCashService : ApplicationService, IProxyCashService
    {
        private IProxyCashDomainService _ProxyCashDomainService;
        public ProxyCashService(IProxyCashDomainService ProxyCashDomainService)
        {
            _ProxyCashDomainService = ProxyCashDomainService;
        }
        public async Task<Result<string>> AddProxyCash(AddProxyCashDto request, string userId)
        {
            return await _ProxyCashDomainService.AddProxyCash(request, userId);
        }
        public async Task<Result<List<ProxyCashDto>>> QueryProxyCash(Request<QueryProxyCashDto> request, string userId)
        {
            return await _ProxyCashDomainService.QueryProxyCash(request, userId);
        }
        public async Task<Result<string>> DeleteProxyCash(List<Guid?> ids)
        {
            return await _ProxyCashDomainService.DeleteProxyCash(ids);
        }
        public async Task<Result<string>> EditProxyCash(EditProxyCashDto request, string userId)
        {
            return await _ProxyCashDomainService.EditProxyCash(request, userId);
        }
    }
}
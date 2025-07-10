using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
namespace ERS.Services.Application
{
    public class Cash3AService : ApplicationService, ICash3AService
    {
        private ICash3ADomainService _Cash3ADomainService;
        public Cash3AService(ICash3ADomainService Cash3ADomainService)
        {
            _Cash3ADomainService = Cash3ADomainService;
        }
        public async Task<Result<List<DeferredDto>>> DeferredQuery(string user) =>await _Cash3ADomainService.DeferredQuery(user);
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token) => await _Cash3ADomainService.Submit(formCollection, user, token, "P");
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token) => await _Cash3ADomainService.Submit(formCollection, user, token, "T");
    }
}

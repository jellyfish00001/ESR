using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDExp;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
namespace ERS.Services.Application
{
    public class Cash6Service : ApplicationService, ICash6Service
    {
        private ICash6DomainService _Cash6DomainService;
        private IBDExpDomainService _BDExpDoaminService;
        public Cash6Service(
            ICash6DomainService Cash6DomainService,
            IBDExpDomainService BDExpDoaminService)
        {
            _Cash6DomainService = Cash6DomainService;
            _BDExpDoaminService = BDExpDoaminService;
        }
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token) => await _Cash6DomainService.Submit(formCollection, user, token, "P");
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token) => await _Cash6DomainService.Submit(formCollection, user, token, "T");
        public async Task<Result<IEnumerable<BDExpDto>>> GetReturnTaiwanExp(string company) => await _BDExpDoaminService.GetReturnTaiwanExp(company);
    }
}
using ERS.Application.Contracts.DTO.Application;
using ERS.Domain.IDomainServices;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDCar;
using ERS.DTO.BDExp;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
namespace ERS.Services.Application
{
    public class Cash1Service : ApplicationService, ICash1Service
    {
        private ICash1DomainService _Cash1DomainService;
        private IApplicationDomainService _ApplicationDomainService;
        private IBDExpDomainService _BDExpDomainService;
        public Cash1Service(ICash1DomainService Cash1DomainService, IApplicationDomainService applicationDomainService, IBDExpDomainService BDExpDomainService)
        {
            _Cash1DomainService = Cash1DomainService;
            _ApplicationDomainService = applicationDomainService;
            _BDExpDomainService = BDExpDomainService;
        }
        public async Task<Result<IEnumerable<BDExpDto>>> GetGeneralCostExp(string company) => await _BDExpDomainService.GetGeneralCostExp(company);
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token) => await _Cash1DomainService.Submit(formCollection, user, token, "P");
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token) => await _Cash1DomainService.Submit(formCollection, user, token, "T");
        public async Task<Result<List<SelfDriveCostDto>>> GetSelfDriveCostListFromExcel(IFormFile excelFile, string company)
        {
            return await _Cash1DomainService.GetSelfDriveCostListFromExcel(excelFile, company);
        }
        public async Task<Result<List<OvertimeMealDto>>> GetOvertimeMealCostListFromExcel(IFormFile excelFile, string company)
        {
            return await _Cash1DomainService.GetOvertimeMealCostListFromExcel(excelFile, company);
        }
        public async Task<Result<TravelDto>> GetMealExpense(TravelDto data) => await _Cash1DomainService.GetMealExpense(data);
        public async Task<Result<IList<string>>> GetCityByCompany(string company) => await _Cash1DomainService.GetCityByCompany(company);
    }
}

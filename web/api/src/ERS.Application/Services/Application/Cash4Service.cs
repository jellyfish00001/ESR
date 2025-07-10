using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDCar;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
namespace ERS.Services.Application
{
    public class Cash4Service : ApplicationService, ICash4Service
    {
        private ICash4DomainService _Cash4DomainService;
        public Cash4Service(ICash4DomainService Cash4DomainService)
        {
            _Cash4DomainService = Cash4DomainService;
        }
        public async Task<Result<List<ReimbListDto>>> GetReimbursementListFromExcel(IFormFile excelFile, string company)
        {
            return await _Cash4DomainService.GetReimbursementListFromExcel(excelFile, company);
        }
        public async Task<Result<List<OvertimeMealDto>>> GetOvertimeMealCostListFromExcel(IFormFile excelFile, string company)
        {
            return await _Cash4DomainService.GetOvertimeMealCostListFromExcel(excelFile,company);
        }
        public async Task<Result<List<SelfDriveCostDto>>> GetSelfDriveCostListFromExcel(IFormFile excelFile, string company)
        {
            return await _Cash4DomainService.GetSelfDriveCostListFromExcel(excelFile,company);
        }
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token) => await _Cash4DomainService.Submit(formCollection, user, token, "P");
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token) => await _Cash4DomainService.Submit(formCollection, user, token, "T");
        public async Task<Result<List<PayeeDto>>> GetPayeeInfo(string keyword) => await _Cash4DomainService.GetPayeeInfo(keyword);
    }
}

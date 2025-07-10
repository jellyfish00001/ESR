using ERS.Application.Contracts.DTO.Application;
using ERS.Domain.IDomainServices;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ERS.Services.Application
{
    public class Cash5Service : ApplicationService, ICash5Service
    {        
        private ICash5DomainService _Cash5DomainService;

        public Cash5Service(
            ICash5DomainService Cash5DomainService
        ){
            _Cash5DomainService = Cash5DomainService;
            
        }
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P")
        {
            return await _Cash5DomainService.Submit(formCollection, user, token, status);
        }

        public async Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token, string status = "T")
        {
            return await _Cash5DomainService.Submit(formCollection, user, token, status);
        }

        public async Task<Result<List<Cash5ExcelDto>>> CheckExcel(IFormFile excelFile, string company, decimal totalAmount, string userId)
        {
            return await _Cash5DomainService.CheckExcel(excelFile, company, totalAmount, userId);
        }
    }
}
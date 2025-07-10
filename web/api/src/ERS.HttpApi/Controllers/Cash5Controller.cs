using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Attribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERS.Controllers
{
    #if DEBUG
    [AllowAnonymous]
#endif     
     [Route("api/[controller]")]
    public class Cash5Controller : BaseController
    {
        private ICash5Service _Cash5Service;

        public Cash5Controller(
            ICash5Service Cash5Service
        ){
            _Cash5Service = Cash5Service;
        }

        [HttpPost("submit")]
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token, string status = "P"){
            return await _Cash5Service.Submit(formCollection, user, token, status);
        }

        [HttpPost("keep")]
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token, string status = "P"){
            return await _Cash5Service.Submit(formCollection, user, token, status);
        }

        [HttpPost("uploadExcel")]
        public async Task<Result<List<Cash5ExcelDto>>> CheckExcel(IFormFile excelFile, string company, decimal totalAmount, string userId){
            return await _Cash5Service.CheckExcel(excelFile, company, totalAmount, userId);
        }
    }
}
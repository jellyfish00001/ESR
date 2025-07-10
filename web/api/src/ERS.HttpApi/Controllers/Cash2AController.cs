using ERS.Application.Contracts.DTO.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERS.DTO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ERS.Controllers;
using ERS.DTO.Application;
using System.Collections.Generic;
namespace ERS.HttpApi.Controllers
{
    #if DEBUG
    [AllowAnonymous]
     #endif
    [Route("api/[controller]")]
    public class Cash2AController : BaseController
    {
        private ICash2AService _Cash2AService;
        public Cash2AController(ICash2AService Cash2AService)
        {
            _Cash2AService=Cash2AService;
        }
        [HttpPost("keep")]
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection) => await _Cash2AService.Keep(formCollection, this.userId, this.token);
        [HttpPost("submit")]
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection) => await _Cash2AService.Submit(formCollection, this.userId, this.token);
        [HttpGet("mealcost")]
        public async Task<Result<List<MealCostDto>>> MealCost(string company) => await _Cash2AService.MealCost(company);
    }
}
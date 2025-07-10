using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDCar;
using ERS.DTO.BDExp;
using ERS.Entities;
using ERS.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class Cash1Controller : BaseController
    {
        private ICash1Service _Cash1Service;
        private IBDCarRepository _bdcarRepository;
        public Cash1Controller(ICash1Service Cash1Service, IBDCarRepository bdcarRepository)
        {
            _Cash1Service = Cash1Service;
            _bdcarRepository = bdcarRepository;
        }
        /// <summary>
        /// 报销费用情景
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpGet("costs")]
        public async Task<Result<IEnumerable<BDExpDto>>> GetGeneralCostExp(string company) => await _Cash1Service.GetGeneralCostExp(company);
        [HttpPost("keep")]
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection) => await _Cash1Service.Keep(formCollection, this.userId, this.token);
        [HttpPost("submit")]
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection) => await _Cash1Service.Submit(formCollection, this.userId, this.token);
        /// <summary>
        /// 获取公出误餐费用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("meal/expense")]
        public async Task<Result<TravelDto>> GetMealExpense([FromBody] TravelDto data) => await _Cash1Service.GetMealExpense(data);
        /// <summary>
        /// 根据公司别查询BDCar
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpGet("cars")]
        public async Task<Result<List<BDCar>>> GetBDCarByCompany(string company) {
            Result<List<BDCar>> result = new();
            var query = await _bdcarRepository.QueryBDCarByCompany(company);
            result.data = query;
            return result;
        }
        /// <summary>
        /// 读取Excel获取自驾油费大量上传数据
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost("cost/selfdrive")]
        public async Task<Result<List<SelfDriveCostDto>>> GetSelfDriveCostListFromExcel(IFormFile excelFile, string company)
        {
            return await _Cash1Service.GetSelfDriveCostListFromExcel(excelFile, company);
        }
        /// <summary>
        /// 读取Excel获取误餐费大量上传数据
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="company"></param>
        /// /// <returns></returns>
        [HttpPost("cost/overtimemeal")]
        public async Task<Result<List<OvertimeMealDto>>> GetOvertimeMealCostListFromExcel(IFormFile excelFile, string company)
        {
            return await _Cash1Service.GetOvertimeMealCostListFromExcel(excelFile, company);
        }
        /// <summary>
        /// 根据公司别获取公出城市
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpGet("city")]
        public async Task<Result<IList<string>>> GetCityByCompany(string company)
        {
            return await _Cash1Service.GetCityByCompany(company);
        }
    }
}

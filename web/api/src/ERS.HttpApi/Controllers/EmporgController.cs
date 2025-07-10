using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Emporg;
using ERS.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERS.DTO;
namespace ERS.HttpApi.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    public class EmporgController: BaseController
    {
        private IEmporgService _emporgService;
        public EmporgController(IEmporgService emporgService)
        {
            _emporgService = emporgService;
        }
        /// <summary>
        /// 费用归属部门查询
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpGet("org/query")]
        public async Task<List<EmporgDto>> QueryVagues(string deptid) => await _emporgService.QueryVagues(deptid);
        /// <summary>
        /// 获取部分费用归属部门
        /// </summary>
        /// <returns></returns>
        [HttpGet("orgs")]
        public async Task<IList<EmporgDto>> GetPartDeptidList() => await _emporgService.GetPartDeptidList();

        [HttpGet("costDeptid")]
        public async Task<Result<string>> GetCostDeptid(string deptid, string company) => await _emporgService.GetCodtDeptid(deptid, company);
    }
}
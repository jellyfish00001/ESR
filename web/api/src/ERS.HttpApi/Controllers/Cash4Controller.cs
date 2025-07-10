using ERS.DTO.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using Microsoft.AspNetCore.Http;
using ERS.DTO.BDCar;
using ERS.Application.Contracts.DTO.Application;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class Cash4Controller : BaseController
    {
        private ICash4Service _Cash4Service;
        public Cash4Controller(ICash4Service Cash4Service)
        {
            _Cash4Service = Cash4Service;
        }
        /// <summary>
        /// 报销清单批量上传excel文件读取数据
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost("cost/reimblist")]
        public async Task<Result<List<ReimbListDto>>> GetReimbursementListFromExcel(IFormFile excelFile, string company)
        {
            return await _Cash4Service.GetReimbursementListFromExcel(excelFile, company);
        }
        /// <summary>
        /// 誤餐費批量報銷上傳excel文件讀取數據
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost("cost/overtimemeal")]
        public async Task<Result<List<OvertimeMealDto>>> GetOvertimeMealCostListFromExcel(IFormFile excelFile, string company)
        {
            return await _Cash4Service.GetOvertimeMealCostListFromExcel(excelFile,company);
        }
        /// <summary>
        /// 自駕油費批量報銷讀取Excel獲取數據
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost("cost/selfdrive")]
        public async Task<Result<List<SelfDriveCostDto>>> GetSelfDriveCostListFromExcel(IFormFile excelFile, string company)
        {
            return await _Cash4Service.GetSelfDriveCostListFromExcel(excelFile,company);
        }
        [HttpPost("keep")]
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection) => await _Cash4Service.Keep(formCollection, this.userId, this.token);
        [HttpPost("submit")]
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection) => await _Cash4Service.Submit(formCollection, this.userId, this.token);
        /// <summary>
        /// 获取收款人信息
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpPost("payeeinfo")]
        public async Task<Result<List<PayeeDto>>> GetPayeeInfo(string keyword) => await _Cash4Service.GetPayeeInfo(keyword);
    }
}

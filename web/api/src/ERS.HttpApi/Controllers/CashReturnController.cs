using ERS.DTO;
using ERS.DTO.BDCash;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO.BaseData;
using ERS.Attribute;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class CashReturnController : BaseController
    {
        private ICashReturnService _CashReturnService;
        public CashReturnController(ICashReturnService CashReturnService)
        {
            _CashReturnService = CashReturnService;
        }
        //查询api（公司别必填）
        [HttpPost("query")]
        [Permission("ers.CashReturn.View")]
        public async Task<Result<List<BDCashDto>>> QueryBDCash([FromBody]Request<BaseDataDto> parameters)
        {
            return await _CashReturnService.QueryBDCash(parameters);
        }
        //新增BDCash
        [HttpPost("add")]
        [Permission("ers.CashReturn.Add")]
        public async Task<Result<BDCashDto>> AddBDCash([FromBody]BDCashDto parameters)
        {
            return await _CashReturnService.AddBDCash(parameters);
        }
        //修改BDCash
        [HttpPut("edit")]
        [Permission("ers.CashReturn.Edit")]
        public async Task<Result<BDCashDto>> UpdateBDCash([FromBody]BDCashDto parameters)
        {
            parameters.muser = this.userId;
            var result = await _CashReturnService.UpdateBDCash(parameters);
            return result;
        }
         //删除BDCash
        [HttpDelete("delete")]
        [Permission("ers.CashReturn.Delete")]
        public async Task<Result<string>> DeleteBDCash([FromBody]BDCashDto request)
        {
            return await _CashReturnService.DeleteBDCash(request);
        }
    }
}

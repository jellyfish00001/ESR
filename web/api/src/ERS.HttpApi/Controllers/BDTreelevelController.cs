using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Attribute;
using ERS.DTO;
using ERS.DTO.BDTreelevel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif     
     [Route("api/[controller]")]
    public class BDTreelevelController : BaseController
    {
        private IBDTreelevelService _BDTreelevelService;
         public BDTreelevelController(IBDTreelevelService BDTreelevelService)
        {
            _BDTreelevelService = BDTreelevelService;
        }

        //取得所有簽核層級
        [HttpGet("all")]
        [Permission("ers.BDSignlevel.View")]
        public async Task<Result<List<QueryBDTreelevelDto>>> GetBDTreelevelAll()
        {
            return await _BDTreelevelService.GetBDTreelevelAll();
        }

        //根據層級編號取得簽核層級
        [HttpGet("levelnum")]
        [Permission("ers.BDSignlevel.View")]
        public async Task<Result<QueryBDTreelevelDto>> GetBDTreelevelByLevelnum(decimal levelnum)
        {
            return await _BDTreelevelService.GetBDTreelevelByLevelnum(levelnum);
        }
        
    }
}
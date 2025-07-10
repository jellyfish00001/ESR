using ERS.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class HelpManualController : BaseController
    {
        private IHelpManualService _HelpManualService;
        public HelpManualController(IHelpManualService HelpManualService)
        {
            _HelpManualService = HelpManualService;
        }
        [HttpGet]
        public async Task<Result<List<HelpManualDto>>> Get(string company) => await _HelpManualService.Get(company, this.userId);
    }
}

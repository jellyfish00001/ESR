using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class Cash2Controller : BaseController
    {
        private ICash2Service _Cash2Service;
        public Cash2Controller(ICash2Service Cash2Service)
        {
            _Cash2Service = Cash2Service;
        }
        [HttpPost("keep")]
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection) => await _Cash2Service.Keep(formCollection, this.userId, this.token);
        [HttpPost("submit")]
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection) => await _Cash2Service.Submit(formCollection, this.userId, this.token);
    }
}

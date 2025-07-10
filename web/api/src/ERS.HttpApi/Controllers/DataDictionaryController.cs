using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.DataDictionary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class DataDictionaryController : BaseController
    {
        private IDataDictionaryService _IDataDictionaryService;
        public DataDictionaryController(IDataDictionaryService iDataDictionaryService)
        {
            _IDataDictionaryService = iDataDictionaryService;
        }


        [HttpPost("query")]
        public async Task<Result<List<QueryDataDictionaryDto>>> GetPageDataDictionary([FromBody] Request<QueryDataDictionaryDto> request)
        {
            return await _IDataDictionaryService.GetPageDataDictionary(request);
        }


        [HttpGet("by-category-criteria")]
        public async Task<Result<List<QueryDataDictionaryDto>>> GetDictionaryByCategoryAndCriteria(string category, string criteria)
        {
            return await _IDataDictionaryService.GetDictionaryByCategory(category, criteria);
        }
    }
}

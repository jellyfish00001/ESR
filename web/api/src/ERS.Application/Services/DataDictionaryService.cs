using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.DataDictionary;
using ERS.IDomainServices;
using Volo.Abp.Application.Services;
namespace ERS.Services
{
    public class DataDictionaryService : ApplicationService, IDataDictionaryService
    {
        private IDataDictionaryDomainService _IDataDictionaryDomainService;
        public DataDictionaryService(IDataDictionaryDomainService iDataDictionaryDomainService)
        {
            _IDataDictionaryDomainService = iDataDictionaryDomainService;
        }
        public async Task<Result<List<QueryDataDictionaryDto>>> GetPageDataDictionary(Request<QueryDataDictionaryDto> request)
        {
            return await _IDataDictionaryDomainService.QueryPageDataDictionary(request);
        }

        public async Task<Result<List<QueryDataDictionaryDto>>> GetDictionaryByCategory(string category, string criteria)
        {
            return await _IDataDictionaryDomainService.GetDictionaryByCategory(category, criteria);
        }
    }
}

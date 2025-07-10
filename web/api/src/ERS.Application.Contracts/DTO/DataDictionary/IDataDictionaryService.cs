using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERS.DTO.DataDictionary
{
    public interface IDataDictionaryService
    {
        Task<Result<List<QueryDataDictionaryDto>>> GetPageDataDictionary(Request<QueryDataDictionaryDto> request);
        Task<Result<List<QueryDataDictionaryDto>>> GetDictionaryByCategory(string category, string criteria);
        
    }
}
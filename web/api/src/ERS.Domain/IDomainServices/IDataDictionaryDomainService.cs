using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.DataDictionary;


//using ERS.DTO.DataDictionary;

namespace ERS.IDomainServices
{
    public interface IDataDictionaryDomainService
    {
        Task<Result<List<QueryDataDictionaryDto>>> QueryPageDataDictionary(Request<QueryDataDictionaryDto> request);

        Task<Result<List<QueryDataDictionaryDto>>> GetDictionaryByCategory(string category, string criteria);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Volo.Abp.ObjectMapping;
using System.Data;
using ERS.DTO.DataDictionary;

namespace ERS.DomainServices
{
    public class DataDictionaryDomainService : CommonDomainService, IDataDictionaryDomainService
    {
        private IDataDictionaryRepository _DataDictionaryRepository;
        private IDataDictionaryCriteriaRepository _IDataDictionaryCriteriaRepository;
        private IObjectMapper _ObjectMapper;
        public DataDictionaryDomainService(
            IDataDictionaryRepository iDataDictionaryRepository,
            IDataDictionaryCriteriaRepository iDataDictionaryCriteriaRepository,
            IObjectMapper ObjectMapper
        )
        {
            _DataDictionaryRepository = iDataDictionaryRepository;
            _IDataDictionaryCriteriaRepository = iDataDictionaryCriteriaRepository;
            _ObjectMapper = ObjectMapper;
        }

        /// <summary>
        /// 查询所有数据字典设定
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result<List<QueryDataDictionaryDto>>> QueryPageDataDictionary(Request<QueryDataDictionaryDto> request)
        {
            Result<List<QueryDataDictionaryDto>> result = new Result<List<QueryDataDictionaryDto>>()
            {
                data = new List<QueryDataDictionaryDto>()
            };

            if (request == null || request.data == null)
            {
                result.message = "illegal request";
                result.status = 2;
                return result;
            }

            List<QueryDataDictionaryDto> resultdata = await GetDictionary(request.data.Category, request.data.Criteria);

            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            int count = resultdata.Count;
            result.data = resultdata.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = count;
            return result;
        }

        /// <summary>
        /// 根据category和条件查询所有数据字典设定
        /// </summary>
        /// <param name="category"></param>
        /// <param name="criteria">多个条件用逗号拼接</param>
        /// <returns></returns>
        public async Task<Result<List<QueryDataDictionaryDto>>> GetDictionaryByCategory(string category, string criteria)
        {
            Result<List<QueryDataDictionaryDto>> result = new Result<List<QueryDataDictionaryDto>>()
            {
                data = new List<QueryDataDictionaryDto>()
            };

            if (string.IsNullOrWhiteSpace(category))
            {
                result.message = "The parameter cannot be null.";
                result.status = 2;
                return result;
            }

            // Map the data to the DTO
            List<QueryDataDictionaryDto> resultdata = await GetDictionary(category, criteria);

            result.data = resultdata;
            result.total = resultdata.Count;
            result.status = 1;
            result.message = "success";

            return result;
        }

        /// <summary>
        /// 根据category和条件查询所有数据字典设定
        /// </summary>
        /// <param name="category"></param>
        /// <param name="criteria">多个条件用逗号拼接</param>
        /// <returns></returns>
        private async Task<List<QueryDataDictionaryDto>> GetDictionary(string category, string criteria)
        {
            //Fetch data filtered by category
            List<DataDictionary> dictionaryList = (await _DataDictionaryRepository.WithDetailsAsync())
                                             .WhereIf(!string.IsNullOrEmpty(category), dd => dd.Category.ToUpper().Contains(category.ToUpper().Trim()))
                                             .OrderBy(dd => dd.Category).ThenBy(dd => dd.SortOrder)
                                             .ToList();

            // Fetch criteria data from the data_dictionary_criteria table with criteria filter
            List<DataDictionaryCriteria> criteriaList = await _IDataDictionaryCriteriaRepository.GetListAsync();
            if (!string.IsNullOrWhiteSpace(criteria))
            {
                criteriaList = criteriaList
                              .Where(c => criteria.Trim().Contains(c.Criteria, StringComparison.OrdinalIgnoreCase))
                              .ToList();

                // Filter dictionaryList to include only entries where Id matches dictionary_id in criteriaList
                dictionaryList = dictionaryList
                               .Where(dd => criteriaList.Any(c => c.DictionaryId == dd.Id))
                               .ToList();
            }

            // Map the data to the DTO
            List<QueryDataDictionaryDto> resultdata = _ObjectMapper.Map<List<DataDictionary>, List<QueryDataDictionaryDto>>(dictionaryList);

            // Populate the criteria field in the DTO and sort by criteria
            foreach (var dto in resultdata)
            {
                var relatedCriteria = criteriaList
                    .Where(c => c.DictionaryId == dto.Id)
                    .OrderBy(c => c.Criteria)
                    .Select(c => c.Criteria)
                    .ToList();

                dto.Criteria = string.Join(", ", relatedCriteria); // 拼接成字符串
            }

            return resultdata;
        }
    }
}

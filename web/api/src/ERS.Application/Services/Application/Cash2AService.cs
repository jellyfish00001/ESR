using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.Entities;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
namespace ERS.Application.Services.Application
{
    public class Cash2AService : ApplicationService, ICash2AService
    {
         private ICash2ADomainService _Cash2ADomainService;
        private IRepository<SuperCategory> _SuperCategoryRepository;
        private IObjectMapper _ObjectMapper;
        public Cash2AService(ICash2ADomainService Cash2ADomainService, IRepository<SuperCategory> SuperCategoryRepository, IObjectMapper ObjectMapper)
        {
            _Cash2ADomainService = Cash2ADomainService;
            _SuperCategoryRepository = SuperCategoryRepository;
            _ObjectMapper = ObjectMapper;
        }
        public void add()
        {
            throw new System.NotImplementedException();
        }
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token) => await _Cash2ADomainService.Submit(formCollection, user, token, "P");
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token) => await _Cash2ADomainService.Submit(formCollection, user, token, "T");
        public async Task<Result<List<MealCostDto>>> MealCost(string company)
        {
            Result<List<MealCostDto>> result = new();
            List<SuperCategory> superCategories = await _SuperCategoryRepository.GetListAsync(i => i.company == company && i.isreimbursable == "N");
            List<MealCostDto> MealCostList = _ObjectMapper.Map<List<SuperCategory>, List<MealCostDto>>(superCategories);
            result.data = MealCostList;
            return result;
        }
    }
}
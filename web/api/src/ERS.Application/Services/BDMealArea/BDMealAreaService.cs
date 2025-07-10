using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDMealArea;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
namespace ERS.Services.BDMealArea
{
    public class BDMealAreaService : ApplicationService, IBDMealAreaService
    {
        private IBDMealAreaDomainService _BDMealAreaDomainService;
        public BDMealAreaService(IBDMealAreaDomainService BDMealAreaDomainService)
        {
            _BDMealAreaDomainService = BDMealAreaDomainService;
        }
        public async Task<Result<List<UploadBDMealAreaDto>>> BatchUploadBDMealArea(IFormFile excelFile, string userId)
        {
            return await _BDMealAreaDomainService.BatchUploadBDMealArea(excelFile, userId);
        }
    }
}
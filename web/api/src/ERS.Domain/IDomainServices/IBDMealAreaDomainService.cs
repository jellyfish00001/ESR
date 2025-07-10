using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDMealArea;
using Microsoft.AspNetCore.Http;
namespace ERS.IDomainServices
{
    public interface IBDMealAreaDomainService
    {
        Task<Result<List<UploadBDMealAreaDto>>> BatchUploadBDMealArea(IFormFile excelFile, string userId);
    }
}
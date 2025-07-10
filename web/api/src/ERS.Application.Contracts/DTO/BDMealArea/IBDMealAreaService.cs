using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace ERS.DTO.BDMealArea
{
    public interface IBDMealAreaService
    {
        Task<Result<List<UploadBDMealAreaDto>>> BatchUploadBDMealArea(IFormFile excelFile, string userId);
    }
}
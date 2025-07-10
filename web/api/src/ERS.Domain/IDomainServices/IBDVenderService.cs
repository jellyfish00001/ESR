using Abp.Domain.Services;
using ERS.DTO;
using ERS.DTO.BDVender;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERS.IDomainServices
{
    public interface IBDVenderDomainService : IDomainService
    {
        Task<Result<string>> UpdateSupplierInfo(BDVenderParamDto en, string userId);
        Task<Result<string>> Create(BDVenderParamDto model, string userId);
        Task<Result<string>> DeleteSupplier(List<string> ids);
        Task<Result<List<BDVenderParamDto>>> Search(Request<BDVenderParamDto> request);
        Task<Result<List<BDVenderParamDto>>> Download(Request<BDVenderParamDto> request);
        Task<Result<List<BDVenderParamDto>>> GetAllVendors();
    }
}
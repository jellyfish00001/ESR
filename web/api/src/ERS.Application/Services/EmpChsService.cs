using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.EmpChs;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
namespace ERS.Services
{
    public class EmpChsService : ApplicationService, IEmpChsService
    {
        private IEmpChsDomainService _EmpChsDomainService;
        public EmpChsService(IEmpChsDomainService EmpChsDomainService)
        {
            _EmpChsDomainService = EmpChsDomainService;
        }
        public async Task<Result<string>> UploadBankAccountName(IFormFile excelFile)
        {
            return await _EmpChsDomainService.UploadBankAccountName(excelFile);
        }
    }
}
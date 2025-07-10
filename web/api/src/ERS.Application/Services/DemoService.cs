using ERS.DomainServices;
using ERS.DTO;
using ERS.DTO.AppConfig;
using ERS.IDomainServices;
using ERS.QRCodeScan;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
namespace ERS.Services
{
    public class DemoService : ApplicationService, IDemoService
    {
        private IAppConfigService _IAppConfigService;
        private DemoDomainService _DemoDomainService;
        private IQRCodeScanRepository _qRCodeScanRepository;
        private IAccountDomainService _AccountDomainService;
        public DemoService(IAppConfigService AppConfigService, DemoDomainService DemoDomainService, IQRCodeScanRepository qRCodeScanRepository, IAccountDomainService AccountDomainService)
        {
            _IAppConfigService = AppConfigService;
            _DemoDomainService = DemoDomainService;
            _qRCodeScanRepository = qRCodeScanRepository;
            _AccountDomainService = AccountDomainService;
        }
        public async Task<Result<string>> get(string rno, string userId)
        {
            //return await _IAppConfigService.GetValueByKey("123");
            //return await _DemoDomainService.get();
            return await _AccountDomainService.SaveAccountInfo(rno, userId);
        }
        public async Task<Result<IList<string>>> scan(IFormFile file)
        {
            var tem = await _qRCodeScanRepository.Get(file);
            return ObjectMapper.Map<Common.Result<IList<string>>, Result<IList<string>>>(tem);
        }
    }
    public interface IDemoService
    {
        Task<Result<string>> get(string rno, string userId);
        Task<Result<IList<string>>> scan(IFormFile file);
    }
}

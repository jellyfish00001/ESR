using ERS.IRepositories;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
namespace ERS.DomainServices
{
    public class DemoDomainService : CommonDomainService
    {
        private ICompanyRepository _CompanyRepository;
        public DemoDomainService(ICompanyRepository CompanyRepository, IObjectMapper ObjectMapper)
        {
            _CompanyRepository = CompanyRepository;
        }
        public async Task<string> get()
        {
            //return await _IAppConfigService.GetValueByKey("123");
            var ere = L["001"];
            return await _CompanyRepository.demo();
        }
    }
}

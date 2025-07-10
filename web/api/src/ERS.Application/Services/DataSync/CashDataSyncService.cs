using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO.DataSync;
using ERS.IDomainServices;
using Volo.Abp.Application.Services;
namespace ERS.Services.DataSync
{
    public class CashDataSyncService : ApplicationService, ICashDataSyncService
    {
        private ICashDataSyncDomainService _CashDataSyncDomainService;
        public CashDataSyncService(ICashDataSyncDomainService CashDataSyncDomainService)
        {
            _CashDataSyncDomainService = CashDataSyncDomainService;
        }
        public async Task testSync(List<string> rnos)
        {
            await _CashDataSyncDomainService.testSync(rnos);
        }
    }
}
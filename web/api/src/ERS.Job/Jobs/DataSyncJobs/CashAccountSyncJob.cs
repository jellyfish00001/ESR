using ERS.IDomainServices;
using ERS.Job.Util;

namespace ERS.Job.Jobs.DataSyncJobs
{
    public class CashAccountSyncJob : IJobBase
    {
        private ICashAccountDomainService _CashAccountDomainService;
        public CashAccountSyncJob(ICashAccountDomainService CashAccountDomainService)
        {
            _CashAccountDomainService = CashAccountDomainService;
        }
        public async Task Run()
        {
            //    await  _CashAccountDomainService.CashAccountDataSync();
            await _CashAccountDomainService.CashAccountDataSync();
        }
    }
}
namespace ERS.Job.Jobs.DataSyncJobs;

using ERS.IDomainServices;
using ERS.Job.Util;

public class CorporateRegistrationSyncJob : IJobBase
{
    public ITaxDataSyncService _taxDataSyncService;
    public CorporateRegistrationSyncJob(ITaxDataSyncService taxDataSyncService)
    {
        _taxDataSyncService = taxDataSyncService;
    }

    public async Task Run()
    {
        await _taxDataSyncService.DownloadTaxDataAndSync();
    }
}

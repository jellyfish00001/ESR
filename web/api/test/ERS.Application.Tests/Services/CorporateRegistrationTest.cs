namespace ERS.Services;

using ERS.IDomainServices;
using System.Threading.Tasks;
using Xunit;

public class CorporateRegistrationTest : ERSApplicationTestBase
{
    public ITaxDataSyncService _taxDataSyncService;
    public CorporateRegistrationTest(ITaxDataSyncService taxDataSyncService)
    {
        _taxDataSyncService = taxDataSyncService;
    }

    [Fact(DisplayName = "下载税籍资料同步至DB")]
    public async Task SyncTaxData2DB()
    {
        await _taxDataSyncService.DownloadTaxDataAndSync();
        //Assert.NotNull(result);
    }
}

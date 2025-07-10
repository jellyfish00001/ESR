

using ERS.IDomainServices;
using System.Threading.Tasks;
using Xunit;
namespace ERS.Services
{
    public class TaxDataSyncServiceTest : ERSApplicationTestBase
    {
        private ITaxDataSyncService _taxDataSyncService;
        public TaxDataSyncServiceTest()
        {

            _taxDataSyncService = GetRequiredService<ITaxDataSyncService>();

        }

        [Fact]
        public async Task TaxDataSyncTest()
        {
            //Act
            var result = _taxDataSyncService.DownloadTaxDataAndSync();
            //Assert
            Assert.NotNull(result);
        }

    }
}
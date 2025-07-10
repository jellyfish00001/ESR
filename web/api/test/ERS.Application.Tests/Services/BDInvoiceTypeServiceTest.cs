using ERS.DTO;
using ERS.DTO.BDInvoiceType;
using Xunit;

namespace ERS.Services
{
    public class BDInvoiceTypeServiceTest : ERSApplicationTestBase
    {
        private IBDInvoiceTypeService _BDInvoiceTypeService;
        public BDInvoiceTypeServiceTest()
        {
            _BDInvoiceTypeService = GetRequiredService<IBDInvoiceTypeService>();
        }

        [Fact(DisplayName = "分頁查詢發票類型")]
        public async void TestName()
        {
            Request<QueryBDInvTypeDto> request = new()
            {
                data = new()
                {
                    companylist = new()
                    {
                        "WZS"
                    }
                },
                pageIndex = 1,
                pageSize = 10
            };
            var result = await _BDInvoiceTypeService.GetPageInvTypes(request);
            Assert.True(result.status == 1 && result.data.Count > 0);
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDInvoiceFolder;
using Xunit;

namespace ERS.Services
{
    public class BDInvoiceFolderServiceTest : ERSApplicationTestBase
    {
        private IBDInvoiceFolderService _BDInvoiceFolderService;
        public BDInvoiceFolderServiceTest()
        {
            _BDInvoiceFolderService = GetRequiredService<IBDInvoiceFolderService>();
        }

        [Fact(DisplayName = "查詢票夾發票信息")]
        public async Task QueryInvInfo()
        {
            string userId = "Z19081961";
            Request<QueryBDInvoiceFolderDto> request = new Request<QueryBDInvoiceFolderDto>()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new QueryBDInvoiceFolderDto()
                {

                }
            };
            var query = await _BDInvoiceFolderService.GetPageInvInfo(request,userId);
            Assert.True(query.data.Count > 0);
        }

        [Fact(DisplayName = "刪除票夾發票信息")]
        public async Task DeleteInvInfo()
        {
            string userId = "Z19081961";
            Request<QueryBDInvoiceFolderDto> request = new Request<QueryBDInvoiceFolderDto>()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new QueryBDInvoiceFolderDto()
                {

                }
            };
            var query = await _BDInvoiceFolderService.GetPageInvInfo(request,userId);
            var result = await _BDInvoiceFolderService.DeleteInvInfo(query.data.FirstOrDefault().Id, userId);
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "編輯票夾發票信息")]
        public async Task EditInvInfo()
        {
            string userId = "Z19081961";
            Request<QueryBDInvoiceFolderDto> request = new Request<QueryBDInvoiceFolderDto>()
            {
                pageIndex = 1,
                pageSize = 10,
                data = new QueryBDInvoiceFolderDto()
                {

                }
            };
            var query = (await _BDInvoiceFolderService.GetPageInvInfo(request,userId)).data.FirstOrDefault();
            InvoiceDto request1 = new InvoiceDto()
            {
                Id = query.Id,
                invcode = "testCode",
                invno = "testNo",
                invdate = System.DateTime.Now,
                invtype = "testType",
                curr = "RMB",
                amount = 1000
            };
            var result = await _BDInvoiceFolderService.EditInvInfo(request1, "Z22070025");
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "獲取發票驗證狀態")]
        public async Task GetInvPayTypes()
        {
            var result = await _BDInvoiceFolderService.GetInvPayTypes();
            Assert.True(result.data.Count == 1);
        }
    }
}
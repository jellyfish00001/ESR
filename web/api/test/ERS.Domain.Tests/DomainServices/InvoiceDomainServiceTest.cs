using ERS.Domain.IDomainServices;

namespace ERS.DomainServices
{
    public class InvoiceDomainServiceTest : ERSDomainTestBase
    {
        private IInvoiceDomainService _InvoiceDomainService;

        public InvoiceDomainServiceTest()
        {
            _InvoiceDomainService = GetRequiredService<IInvoiceDomainService>();
        }

        //[Theory(DisplayName = "从发票池中检查发票")]
        //[InlineData("21053972", "4400212130")]
        //public async Task CheckInvoiceByAutopa(string invno, string invcode)
        //{
        //    IList<Invoice> invoices = new List<Invoice>()
        //    {
        //        new Invoice()
        //        {
        //            invno = invno,
        //            invcode = invcode
        //        }
        //    };
        //    var data = await _InvoiceDomainService.checkInvoicePaid(invoices, "");
        //    Assert.NotNull(data.data);
        //}
    }
}

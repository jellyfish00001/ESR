using System.Threading.Tasks;
using ERS.DTO.Application;
using Xunit;

namespace ERS.Application.Tests.Services
{
    public class Cash2ServiceTest : ERSApplicationTestBase
    {
        private ICash2Service _Cash2Service;

        public Cash2ServiceTest()
        {

            _Cash2Service = GetRequiredService<ICash2Service>();

        }

        [Theory(DisplayName = "暂存一般交际费单子")]
        [InlineData("Z19081961")]
        public async Task KeepNewCash2(string user)
        {
            // Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            // FormFileCollection filesForm = new FormFileCollection();
            // dictionary.Add("head", "{\"cname\":\"xing\",\"deptid\":\"JML121\",\"ext\":null,\"company\":\"WZS\",\"projectcode\":null,\"currency\":\"RMB\",\"amount\":\"45,645\",\"payeeId\":\"Z19081961\",\"payeename\":\"xing\"}");
            // dictionary.Add("detail", "[{\"seq\":1,\"rdate\":\"2022/08/06\",\"deptid\":\"JML120\",\"currency\":\"RMB\",\"amount1\":45645,\"baseamt\":45645,\"rate\":1,\"object\":\"aaa\",\"hospdate\":\"2022/08/06\",\"keep\":\"a\",\"remarks\":\"a\",\"basecurr\":\"RMB\",\"amount2\":8849.56}]");
            // dictionary.Add("file", "[{\"seq\":1,\"item\":1,\"filetype\":\"image/jpeg\",\"filename\":\"123.jpg\",\"ishead\":\"N\",\"key\":\"1\"}]");
            // dictionary.Add("inv", "[{\"seq\":1,\"item\":1,\"invcode\":\"144002101520\",\"invno\":\"00368555\",\"amount\":8849.56,\"taxloss\":2500,\"curr\":\"RMB\",\"undertaker\":\"company\",\"invdate\":\"2021-06-24T00:00:00\",\"taxamount\":1150.44,\"oamount\":10000,\"invstat\":\"N\",\"abnormalamount\":0,\"paymentName\":\"纬创资通（中山）有限公司\",\"paymentNo\":\"914420007076361689\",\"collectionName\":\"中山市驴鼎餐厅\",\"collectionNo\":\"92442000MA52DXGR1R\",\"expdesc\":\"銷售方地址電話有繁體字\",\"expcode\":\"提示：發票：00368555(發票號碼) 銷售方地址電話有繁體字 請重開發票 稅金損失：2500.00\",\"invdesc\":\"144002101520&00368555&8849.56&1150.44&10000.00&N\",\"reason\":null,\"abnormal\":\"N\"}]");
            // dictionary.Add("amount", "{\"currency\":\"RMB\",\"amount\":\"45,645\",\"actamt\":\"45,645\"}");

            // var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Cash2", "123.jpg");
            // FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
            // filesForm.Add(new FormFile(excelFS, 0, excelFS.Length, "1", "123.jpg"));
            // IFormCollection data = new FormCollection(dictionary, filesForm);

            // Result<CashResult> result = await _Cash2Service.Keep(data, user,"");
            // Assert.True(result.status == 1 && !string.IsNullOrEmpty(result.data.rno));
            Assert.True(true);
        }

        [Theory(DisplayName = "暂存已有单号一般交际费单子")]
        [InlineData("Z19081961")]
        public async Task KeepExistCash2(string user)
        {
            // Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            // FormFileCollection filesForm = new FormFileCollection();
            // dictionary.Add("head", "{\"rno\":\"E22080500003\",\"cname\":\"xing\",\"deptid\":\"JML121\",\"ext\":null,\"company\":\"WZS\",\"projectcode\":null,\"currency\":\"RMB\",\"amount\":\"45,645\",\"payeeId\":\"Z19081961\",\"payeename\":\"xing\"}");
            // dictionary.Add("detail", "[{\"seq\":1,\"rdate\":\"2022/08/06\",\"deptid\":\"JML120\",\"currency\":\"RMB\",\"amount1\":45645,\"baseamt\":45645,\"rate\":1,\"object\":\"aaa\",\"hospdate\":\"2022/08/06\",\"keep\":\"a\",\"remarks\":\"a\",\"basecurr\":\"RMB\",\"amount2\":8849.56}]");
            // dictionary.Add("file", "[{\"seq\":1,\"item\":1,\"filetype\":\"image/jpeg\",\"filename\":\"123.jpg\",\"ishead\":\"N\",\"key\":\"1\"}]");
            // dictionary.Add("inv", "[{\"seq\":1,\"item\":1,\"invcode\":\"144002101520\",\"invno\":\"00368555\",\"amount\":8849.56,\"taxloss\":2500,\"curr\":\"RMB\",\"undertaker\":\"company\",\"invdate\":\"2021-06-24T00:00:00\",\"taxamount\":1150.44,\"oamount\":10000,\"invstat\":\"N\",\"abnormalamount\":0,\"paymentName\":\"纬创资通（中山）有限公司\",\"paymentNo\":\"914420007076361689\",\"collectionName\":\"中山市驴鼎餐厅\",\"collectionNo\":\"92442000MA52DXGR1R\",\"expdesc\":\"銷售方地址電話有繁體字\",\"expcode\":\"提示：發票：00368555(發票號碼) 銷售方地址電話有繁體字 請重開發票 稅金損失：2500.00\",\"invdesc\":\"144002101520&00368555&8849.56&1150.44&10000.00&N\",\"reason\":null,\"abnormal\":\"N\"}]");
            // dictionary.Add("amount", "{\"currency\":\"RMB\",\"amount\":\"45,645\",\"actamt\":\"45,645\"}");

            // var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Cash2", "1234.jpg");
            // FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
            // filesForm.Add(new FormFile(excelFS, 0, excelFS.Length, "1", "1234.jpg"));
            // IFormCollection data = new FormCollection(dictionary, filesForm);

            // Result<CashResult> result = await _Cash2Service.Keep(data, user,"");
            // Assert.True(result.status == 1 && !string.IsNullOrEmpty(result.data.rno));
            Assert.True(true);
        }
    }
}
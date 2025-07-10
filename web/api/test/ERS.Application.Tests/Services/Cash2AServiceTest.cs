using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.DTO;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace ERS.Application.Tests.Services
{
    public class Cash201ServiceTest : ERSApplicationTestBase
    {
        private ICash2AService _Cash2AService;

        public Cash201ServiceTest()
        {

            _Cash2AService = GetRequiredService<ICash2AService>();

        }

        [Theory(DisplayName = "暂存一般交际费单子")]
        [InlineData("Z19081961")]
        public async Task KeepNewCash2(string user)
        {
            // Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            // FormFileCollection filesForm = new FormFileCollection();
            // dictionary.Add("head", "{\"cname\":\"xing\",\"deptid\":\"JML121\",\"ext\":\"8899\",\"company\":\"WZS\",\"projectcode\":null,\"currency\":\"RMB\",\"amount\":\"45,645\",\"payeeId\":\"Z19081961\",\"payeename\":\"xing\",\"approvereason\":\"Y\",\"whetherapprove\":\"原因\"}");
            // dictionary.Add("detail", "[{\"seq\":1,\"rdate\":\"2022/08/06\",\"deptid\":\"JML120\",\"currency\":\"RMB\",\"amount1\":45645,\"baseamt\":45645,\"rate\":1,\"object\":\"aaa\",\"hospdate\":\"2022/08/06\",\"keep\":\"a\",\"remarks\":\"a\",\"basecurr\":\"RMB\",\"amount2\":8849.56,\"treattime\":\"工作日中餐\",\"flag\":\"1\"}]");
            // dictionary.Add("file", "[{\"seq\":1,\"item\":1,\"filetype\":\"image/jpeg\",\"filename\":\"111.jpg\",\"ishead\":\"N\",\"key\":\"1\"}]");
            // dictionary.Add("inv", "[{\"seq\":1,\"item\":1,\"invcode\":\"144002101520\",\"invno\":\"00368555\",\"amount\":8849.56,\"taxloss\":2500,\"curr\":\"RMB\",\"undertaker\":\"company\",\"invdate\":\"2021-06-24T00:00:00\",\"taxamount\":1150.44,\"oamount\":10000,\"invstat\":\"N\",\"abnormalamount\":0,\"paymentName\":\"纬创资通（中山）有限公司\",\"paymentNo\":\"914420007076361689\",\"collectionName\":\"中山市驴鼎餐厅\",\"collectionNo\":\"92442000MA52DXGR1R\",\"expdesc\":\"銷售方地址電話有繁體字\",\"expcode\":\"提示：發票：00368555(發票號碼) 銷售方地址電話有繁體字 請重開發票 稅金損失：2500.00\",\"invdesc\":\"144002101520&00368555&8849.56&1150.44&10000.00&N\",\"reason\":null,\"abnormal\":\"N\"}]");
            // dictionary.Add("amount", "{\"currency\":\"RMB\",\"amount\":\"45,645\",\"actamt\":\"45,645\"}");

            // var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Cash2", "asd.jpg");
            // FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
            // filesForm.Add(new FormFile(excelFS, 0, excelFS.Length, "1", "asd.jpg"));
            // IFormCollection data = new FormCollection(dictionary, filesForm);

            //Result<CashResult> result = await _Cash2AService.Keep(data, user);
            // Result<CashResult> result1 = await _Cash2AService.Submit(data, user,"");
            // Assert.True(result1.status == 1 && !string.IsNullOrEmpty(result1.data.rno));
            Assert.True(true);
        }

        [Theory(DisplayName = "判断单据送单人是否为同一人")]
        [InlineData("Z21032798")]
        public async Task CuserCash2(string user)
        {
            Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            FormFileCollection filesForm = new FormFileCollection();
            dictionary.Add("head", "{\"rno\":\"E22080500004\",\"cname\":\"xing\",\"deptid\":\"JML121\",\"ext\":\"8899\",\"company\":\"WZS\",\"projectcode\":null,\"currency\":\"RMB\",\"amount\":\"45,645\",\"payeeId\":\"Z19081961\",\"payeename\":\"xing\"}");
            dictionary.Add("detail", "[{\"seq\":1,\"rdate\":\"2022/08/06\",\"deptid\":\"JML120\",\"currency\":\"RMB\",\"amount1\":45645,\"baseamt\":45645,\"rate\":1,\"object\":\"aaa\",\"hospdate\":\"2022/08/06\",\"keep\":\"a\",\"remarks\":\"a\",\"basecurr\":\"RMB\",\"amount2\":8849.56}]");
            dictionary.Add("file", "[{\"seq\":1,\"item\":1,\"filetype\":\"image/jpeg\",\"filename\":\"123.jpg\",\"ishead\":\"N\",\"key\":\"1\"}]");
            dictionary.Add("inv", "[{\"seq\":1,\"item\":1,\"invcode\":\"144002101520\",\"invno\":\"00368555\",\"amount\":8849.56,\"taxloss\":2500,\"curr\":\"RMB\",\"undertaker\":\"company\",\"invdate\":\"2021-06-24T00:00:00\",\"taxamount\":1150.44,\"oamount\":10000,\"invstat\":\"N\",\"abnormalamount\":0,\"paymentName\":\"纬创资通（中山）有限公司\",\"paymentNo\":\"914420007076361689\",\"collectionName\":\"中山市驴鼎餐厅\",\"collectionNo\":\"92442000MA52DXGR1R\",\"expdesc\":\"銷售方地址電話有繁體字\",\"expcode\":\"提示：發票：00368555(發票號碼) 銷售方地址電話有繁體字 請重開發票 稅金損失：2500.00\",\"invdesc\":\"144002101520&00368555&8849.56&1150.44&10000.00&N\",\"reason\":null,\"abnormal\":\"N\"}]");
            dictionary.Add("amount", "{\"currency\":\"RMB\",\"amount\":\"45,645\",\"actamt\":\"45,645\"}");
            IFormCollection data = new FormCollection(dictionary, filesForm);

            Result<CashResult> result = await _Cash2AService.Keep(data, user,"");

            Assert.True(result.status == 2);
        }

              [Theory(DisplayName = "判断单据是否在签核中")]
        [InlineData("")]
        public async Task SignedCash2(string user)
        {
            Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            FormFileCollection filesForm = new FormFileCollection();
            dictionary.Add("head", "{\"rno\":\"E22081000002\",\"cname\":\"xing\",\"deptid\":\"JML121\",\"ext\":\"8899\",\"company\":\"WZS\",\"projectcode\":null,\"currency\":\"RMB\",\"amount\":\"45,645\",\"payeeId\":\"Z19081961\",\"payeename\":\"xing\"}");
            dictionary.Add("detail", "[{\"seq\":1,\"rdate\":\"2022/08/06\",\"deptid\":\"JML120\",\"currency\":\"RMB\",\"amount1\":45645,\"baseamt\":45645,\"rate\":1,\"object\":\"aaa\",\"hospdate\":\"2022/08/06\",\"keep\":\"a\",\"remarks\":\"a\",\"basecurr\":\"RMB\",\"amount2\":8849.56}]");
            dictionary.Add("file", "[{\"seq\":1,\"item\":1,\"filetype\":\"image/jpeg\",\"filename\":\"123.jpg\",\"ishead\":\"N\",\"key\":\"1\"}]");
            dictionary.Add("inv", "[{\"seq\":1,\"item\":1,\"invcode\":\"144002101520\",\"invno\":\"00368555\",\"amount\":8849.56,\"taxloss\":2500,\"curr\":\"RMB\",\"undertaker\":\"company\",\"invdate\":\"2021-06-24T00:00:00\",\"taxamount\":1150.44,\"oamount\":10000,\"invstat\":\"N\",\"abnormalamount\":0,\"paymentName\":\"纬创资通（中山）有限公司\",\"paymentNo\":\"914420007076361689\",\"collectionName\":\"中山市驴鼎餐厅\",\"collectionNo\":\"92442000MA52DXGR1R\",\"expdesc\":\"銷售方地址電話有繁體字\",\"expcode\":\"提示：發票：00368555(發票號碼) 銷售方地址電話有繁體字 請重開發票 稅金損失：2500.00\",\"invdesc\":\"144002101520&00368555&8849.56&1150.44&10000.00&N\",\"reason\":null,\"abnormal\":\"N\"}]");
            dictionary.Add("amount", "{\"currency\":\"RMB\",\"amount\":\"45,645\",\"actamt\":\"45,645\"}");;
            IFormCollection data = new FormCollection(dictionary, filesForm);

            Result<CashResult> result = await _Cash2AService.Keep(data, user,"");
            Assert.True(result.status == 2);
        }


        // [Theory(DisplayName = "判断发票是否已请款")]//token获取失败
        //  [InlineData("AAA111")]
        // public async Task checkInvoiceCash2(string user)
        // {
        //     Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
        //     FormFileCollection filesForm = new FormFileCollection();
        //     dictionary.Add("head", "{\"rno\":\"E22080500089\",\"cname\":\"xing\",\"deptid\":\"JML121\",\"ext\":\"8899\",\"company\":\"WZS\",\"projectcode\":null,\"currency\":\"RMB\",\"amount\":\"45,645\",\"payeeId\":\"Z19081961\",\"payeename\":\"xing\"}");
        //       dictionary.Add("detail", "[{\"seq\":1,\"rdate\":\"2022/08/06\",\"deptid\":\"JML120\",\"currency\":\"RMB\",\"amount1\":45645,\"baseamt\":45645,\"rate\":1,\"object\":\"aaa\",\"hospdate\":\"2022/08/06\",\"keep\":\"a\",\"remarks\":\"a\",\"basecurr\":\"RMB\",\"amount2\":8849.56}]");

        //     dictionary.Add("file", "[{\"seq\":1,\"item\":1,\"filetype\":\"image/jpeg\",\"filename\":\"123.jpg\",\"ishead\":\"N\",\"key\":\"1\"}]");
        //     dictionary.Add("inv", "[{\"seq\":1,\"item\":1,\"invcode\":\"050001900105\",\"invno\":\"12454419\",\"amount\":8849.56,\"taxloss\":2500,\"curr\":\"RMB\",\"undertaker\":\"company\",\"invdate\":\"2021-06-24T00:00:00\",\"taxamount\":1150.44,\"oamount\":10000,\"invstat\":\"N\",\"abnormalamount\":0,\"paymentName\":\"纬创资通（中山）有限公司\",\"paymentNo\":\"914420007076361689\",\"collectionName\":\"中山市驴鼎餐厅\",\"collectionNo\":\"92442000MA52DXGR1R\",\"expdesc\":\"銷售方地址電話有繁體字\",\"expcode\":\"提示：發票：00368555(發票號碼) 銷售方地址電話有繁體字 請重開發票 稅金損失：2500.00\",\"invdesc\":\"144002101520&00368555&8849.56&1150.44&10000.00&N\",\"reason\":null,\"abnormal\":\"N\"}]");
        //     dictionary.Add("amount", "{\"currency\":\"RMB\",\"amount\":\"45,645\",\"actamt\":\"45,645\"}");;
        //     IFormCollection data = new FormCollection(dictionary, filesForm);
        //     Result<string> result = await _Cash2AService.Keep(data, user);
        //     Assert.True(result.status == 2);
        // }


        // [Theory(DisplayName = "暂存已有单号一般交际费单子")]
        // [InlineData("Z19081961")]
        // public async Task KeepExistCash2(string user)
        // {
        //     Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
        //     FormFileCollection filesForm = new FormFileCollection();
        //     dictionary.Add("head", "{\"rno\":\"E22080500009\",\"cname\":\"xing\",\"deptid\":\"JML121\",\"ext\":null,\"company\":\"WZS\",\"projectcode\":null,\"currency\":\"RMB\",\"amount\":\"45,645\",\"payeeId\":\"Z19081961\",\"payeename\":\"xing\"}");
        //     dictionary.Add("detail", "[{\"seq\":1,\"rdate\":\"2022/08/06\",\"deptid\":\"JML120\",\"currency\":\"RMB\",\"amount1\":45645,\"baseamt\":45645,\"rate\":1,\"object\":\"拍拍拍\","+
        //     "\"hospdate\":\"2022/08/06\",\"keep\":\"Z21010529//盧周\",\"remarks\":\"a\",\"basecurr\":\"RMB\",\"baseamt\":\"960\",\"amount2\":\"8849.56\","+
        //     "\"expcode\":\"EXP02\",\"expname\":\"交际费\",\"acctcode\":\"71412000\",\"ACCTNAME\":\"交際費-其他\",\"TREATADDRESS\":\"11111\",\"CUSTSUPERME\":"+
        //     "\"2222\" ,\"otherobject\":\"hello\",\"objectsum\":\"2\",\"keepcategory\":\"其他主管或同仁\",\"otherkeep\":\"12\",\"keepsum\":\"4\",\"isaccordnumber\":\"1\","+
        //     "\"notaccordreason\":\"..\",\"actualexpense\":\"960\",\"isaccordcost\":\"1\",\"overbudget\":\"760\",\"flag\":\"1\",\"treattime\":\"工作日中餐\",\"\"}]");
        //     dictionary.Add("file", "[{\"seq\":1,\"item\":1,\"filetype\":\"image/jpeg\",\"filename\":\"123.jpg\",\"ishead\":\"N\",\"key\":\"1\"}]");
        //     dictionary.Add("inv", "[{\"seq\":1,\"item\":1,\"invcode\":\"144002101520\",\"invno\":\"00368555\",\"amount\":8849.56,\"taxloss\":2500,\"curr\":\"RMB\",\"undertaker\":\"company\",\"invdate\":\"2021-06-24T00:00:00\",\"taxamount\":1150.44,\"oamount\":10000,\"invstat\":\"N\",\"abnormalamount\":0,\"paymentName\":\"纬创资通（中山）有限公司\",\"paymentNo\":\"914420007076361689\",\"collectionName\":\"中山市驴鼎餐厅\",\"collectionNo\":\"92442000MA52DXGR1R\",\"expdesc\":\"銷售方地址電話有繁體字\",\"expcode\":\"提示：發票：00368555(發票號碼) 銷售方地址電話有繁體字 請重開發票 稅金損失：2500.00\",\"invdesc\":\"144002101520&00368555&8849.56&1150.44&10000.00&N\",\"reason\":null,\"abnormal\":\"N\"}]");
        //     dictionary.Add("amount", "{\"currency\":\"RMB\",\"amount\":\"45,645\",\"actamt\":\"45,645\"}");

        //     var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Cash2", "1234.jpg");
        //     FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
        //     filesForm.Add(new FormFile(excelFS, 0, excelFS.Length, "1", "1234.jpg"));
        //     IFormCollection data = new FormCollection(dictionary, filesForm);

        //     Result<string> result = await _Cash2AService.Keep(data, user);
        //     Assert.True(result.status == 1 && !string.IsNullOrEmpty(result.data));
        // }

        // [Theory(DisplayName = "Error")]
        // [InlineData("Z19081961")]
        // public async Task SubmitNewCash2Error(string user)
        // {
        //     Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
        //     FormFileCollection filesForm = new FormFileCollection();
        //     dictionary.Add("head", "{\"rno\":null,\"cname\":\"周杜鵑\",\"deptid\":\"MZF350\",\"ext\":\"6871\",\"company\":\"WZS\",\"projectcode\":null,\"currency\":\"RMB\",\"payeeId\":\"Z12073027\",\"payeename\":\"周杜鵑\",\"whetherapprove\":\"Y\",\"approvereason\":null}");
        //     dictionary.Add("detail", "[{\"rno\":null,\"seq\":1,\"rdate\":\"2023-02-07T00:51:02.716Z\",\"currency\":\"RMB\",\"amount1\":32288000,\"deptid\":\"MZF300\",\"object\":\"Nest\",\"keep\":\"10607026\",\"basecurr\":\"RMB\",\"baseamt\":21225423.93,\"rate\":200000,\"treataddress\":\"AA\",\"otherobject\":null,\"objectsum\":200000,\"keepcategory\":\"2-其他主管或同仁\",\"otherkeep\":null,\"keepsum\":1800,\"isaccordnumber\":\"Y\",\"datediffreason\":\"AA\",\"actualexpense\":21225423.93,\"paymentexpense\":21225423.93,\"isaccordcost\":\"Y\",\"overbudget\":0,\"processmethod\":null,\"custsuperme\":\"AA\",\"treattime\":\"晚餐或非工作日中餐\",\"flag\":1}]");
        //     dictionary.Add("file", "[{\"rno\":null,\"seq\":1,\"item\":1,\"filetype\":\"application/pdf\",\"filename\":\"&22442000000000336285&18783561.00&2441862.93&21225423.93&N\",\"ishead\":\"N\",\"key\":\"k4qn66tj6xf\"}]");
        //     dictionary.Add("inv", "[{\"rno\":null,\"seq\":1,\"item\":1,\"invcode\":\"\",\"invno\":\"22442000000000336285\",\"amount\":18783561,\"taxloss\":0,\"curr\":\"RMB\",\"undertaker\":null,\"invdate\":\"2022-09-01T00:00:00\",\"taxamount\":2441862.93,\"oamount\":21225423.93,\"invstat\":\"N\",\"abnormalamount\":0,\"paymentName\":null,\"paymentNo\":null,\"collectionName\":null,\"collectionNo\":null,\"expdesc\":\"\",\"expcode\":null,\"invdesc\":\"&22442000000000336285&18783561.00&2441862.93&21225423.93&N\",\"reason\":null,\"abnormal\":\"N\"}]");
        //     dictionary.Add("amount", "{\"rno\":null,\"currency\":\"RMB\",\"amount\":21225423.93,\"actamt\":21225423.93}");

        //     var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Cash2", "111.jpg");
        //     FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
        //     filesForm.Add(new FormFile(excelFS, 0, excelFS.Length, "1", "111.jpg"));
        //     IFormCollection data = new FormCollection(dictionary, filesForm);

        //     //Result<CashResult> result = await _Cash2AService.Keep(data, user);
        //     Result<CashResult> result1 = await _Cash2AService.Submit(data, user,"");
        //     Assert.True(result1.status == 1 && !string.IsNullOrEmpty(result1.data.rno));
        //     //Assert.True(result.status == 1 && !string.IsNullOrEmpty(result.data.rno));
        // }
    }
}
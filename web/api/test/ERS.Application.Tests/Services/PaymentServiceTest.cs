using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.Payment;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace ERS.Services
{
    public class PaymentServiceTest : ERSApplicationTestBase
    {
        private IPaymentService _PaymentService;
        public PaymentServiceTest()
        {
            _PaymentService = GetRequiredService<IPaymentService>();
        }

        [Fact(DisplayName = "已付款清单查询（有數據）")]
        public async Task GetPagePayListNotEmpty()
        {
            Request<PayParamsDto> request = new Request<PayParamsDto>()
            {
                data = new PayParamsDto(),
                pageIndex = 1,
                pageSize = 10
            };
            var result = await _PaymentService.GetPagePayList(request);
            Assert.True(result.data.Count == 1);
        }

        [Fact(DisplayName = "已付款清单查询（無數據）")]
        public async Task GetPagePayListEmpty()
        {
            Request<PayParamsDto> request = new Request<PayParamsDto>()
            {
                data = new PayParamsDto()
                {
                    bank = "123"
                },
                pageIndex = 1,
                pageSize = 10
            };
            var result = await _PaymentService.GetPagePayList(request);
            Assert.True(result.data.Count == 0);
        }

        [Fact(DisplayName = "按sysno、银行别查询付款清单")]
        public async Task GetPaylistDetails()
        {
            string sysno = "22112200004";
            string bank = "中國工商銀行";
            var result = await _PaymentService.GetPaylistDetails(sysno, bank);
            Assert.NotNull(result);
        }

        // [Fact(DisplayName = "删除付款清单（按单号）")]
        // public async Task DeletePaylist()
        // {
        //     List<string> sysnolist = new List<string>()
        //     {
        //         "22112200004"
        //     };
        //     var result = await _PaymentService.DeletePaylist(sysnolist);
        //     Assert.True(result.status == 1);
        // }

        [Fact(DisplayName = "讀取Excel生成付款清單（无误）")]
        public async Task GeneratePaylist()
        {
            // 付款清單樣本 = Payment List Sample
            Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            FormFileCollection filesForm = new FormFileCollection();
            dictionary.Add("company", "WZS");
            dictionary.Add("paymentdate", "2023/02/18");
            dictionary.Add("identification", "5151515");
            var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Payment", "Payment List Sample Correct.xls");
            FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
            FormFile eFile = new FormFile(excelFS, 0, excelFS.Length, "1", "Payment List Sample Correct.xls");
            eFile.Headers = new HeaderDictionary();
            eFile.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            filesForm.Add(eFile);
            IFormCollection data = new FormCollection(dictionary, filesForm);
            Result<string> result = await _PaymentService.GeneratePaylist(data, "Z22070025", "");
            Assert.True(result.status == 1);
        }

        [Fact(DisplayName = "讀取Excel生成付款清單（公司別為空）")]
        public async Task GeneratePaylist_CompanyEmpty()
        {
            // 付款清單樣本 = Payment List Sample
            Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            FormFileCollection filesForm = new FormFileCollection();
            dictionary.Add("company", " ");
            dictionary.Add("paymentdate", "2023/02/18");
            dictionary.Add("identification", "5151515");
            var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Payment", "Payment List Sample Correct1.xls");
            FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
            FormFile eFile = new FormFile(excelFS, 0, excelFS.Length, "1", "Payment List Sample Correct1.xls");
            eFile.Headers = new HeaderDictionary();
            eFile.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            filesForm.Add(eFile);
            IFormCollection data = new FormCollection(dictionary, filesForm);
            Result<string> result = await _PaymentService.GeneratePaylist(data, "Z22070025", "");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "讀取Excel生成付款清單（Identification為空）")]
        public async Task GeneratePaylist_IdentificationEmpty()
        {
            // 付款清單樣本 = Payment List Sample
            Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            FormFileCollection filesForm = new FormFileCollection();
            dictionary.Add("company", "WZS");
            dictionary.Add("paymentdate", "2023/02/18");
            dictionary.Add("identification", " ");
            var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Payment", "Payment List Sample Correct2.xls");
            FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
            FormFile eFile = new FormFile(excelFS, 0, excelFS.Length, "1", "Payment List Sample Correct2.xls");
            eFile.Headers = new HeaderDictionary();
            eFile.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            filesForm.Add(eFile);
            IFormCollection data = new FormCollection(dictionary, filesForm);
            Result<string> result = await _PaymentService.GeneratePaylist(data, "Z22070025", "");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "讀取Excel生成付款清單（Excel表數據為空）")]
        public async Task GeneratePaylist_ExcelEmpty()
        {
            // 付款清單樣本 = Payment List Sample
            Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            FormFileCollection filesForm = new FormFileCollection();
            dictionary.Add("company", "WZS");
            dictionary.Add("paymentdate", "2023/02/18");
            dictionary.Add("identification", "123");
            var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Payment", "Payment List Sample ExcelEmpty.xls");
            FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
            FormFile eFile = new FormFile(excelFS, 0, excelFS.Length, "1", "Payment List Sample ExcelEmpty.xls");
            eFile.Headers = new HeaderDictionary();
            eFile.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            filesForm.Add(eFile);
            IFormCollection data = new FormCollection(dictionary, filesForm);
            Result<string> result = await _PaymentService.GeneratePaylist(data, "Z22070025", "");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "讀取Excel生成付款清單（輸入框paymentdate數據格式錯誤）")]
        public async Task GeneratePaylist_InputPaymentdate_Invalid()
        {
            // 付款清單樣本 = Payment List Sample
            Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            FormFileCollection filesForm = new FormFileCollection();
            dictionary.Add("company", "WZS");
            dictionary.Add("paymentdate", "456789阿薩德");
            dictionary.Add("identification", "123");
            var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Payment", "Payment List Sample Correct3.xls");
            FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
            FormFile eFile = new FormFile(excelFS, 0, excelFS.Length, "1", "Payment List Sample Correct3.xls");
            eFile.Headers = new HeaderDictionary();
            eFile.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            filesForm.Add(eFile);
            IFormCollection data = new FormCollection(dictionary, filesForm);
            Result<string> result = await _PaymentService.GeneratePaylist(data, "Z22070025", "");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "讀取Excel生成付款清單（ExcelPaymentdate數據格式錯誤）")]
        public async Task GeneratePaylist_ExcelPaymentdate_Invalid()
        {
            // 付款清單樣本 = Payment List Sample
            Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            FormFileCollection filesForm = new FormFileCollection();
            dictionary.Add("company", "WZS");
            dictionary.Add("paymentdate", "");
            dictionary.Add("identification", "123");
            var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Payment", "Payment List Sample InvalidPaymentdate.xls");
            FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
            FormFile eFile = new FormFile(excelFS, 0, excelFS.Length, "1", "Payment List Sample InvalidPaymentdate.xls");
            eFile.Headers = new HeaderDictionary();
            eFile.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            filesForm.Add(eFile);
            IFormCollection data = new FormCollection(dictionary, filesForm);
            Result<string> result = await _PaymentService.GeneratePaylist(data, "Z22070025", "");
            Assert.True(result.status == 2);
        }

        [Fact(DisplayName = "未付款清單查詢（有數據）")]
        public async Task GetPageUnpaiedListNotEmpty()
        {
            Request<string> request = new Request<string>()
            {
                pageIndex = 1,
                pageSize = 10
            };
            var result = await _PaymentService.GetPageUnpaidList(request, "Z21032798");
            Assert.True(result.data.Count == 2);
        }

        [Fact(DisplayName = "未付款清單查詢（無數據）")]
        public async Task GetPageUnpaiedListEmpty()
        {
            Request<string> request = new Request<string>()
            {
                pageIndex = 1,
                pageSize = 10
            };
            var result = await _PaymentService.GetPageUnpaidList(request, "Z22070025");
            Assert.True(result.data.Count == 0);
        }

        [Fact(DisplayName = "下載付款明细")]
        public async Task DownloadRemittanceExcel()
        {
            RemittanceDto request = new RemittanceDto()
            {
                sysno = "22112200004",
                bank = "中國工商銀行",
                company = "WZS"
            };
            var result = await _PaymentService.GetRemittanceExcel(request);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "下載零用金付款清單")]
        public async Task DownloadPettyExcel()
        {
            UpdatePaymentDto request = new UpdatePaymentDto()
            {
                sysno = "22112200004",
                bank = "中國工商銀行"
            };
            var result = await _PaymentService.GetPettyListExcel(request);
            Assert.NotNull(result);
        }
    }
}
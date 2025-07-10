using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Invoice;
using ERS.DTO.Application;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Xunit;

namespace ERS.Services
{
    public class InvoiceServiceTest : ERSApplicationTestBase
    {
        private IInvoiceService _invoiceService;

        public InvoiceServiceTest()
        {
            _invoiceService = GetRequiredService<IInvoiceService>();
        }


        // [Theory(DisplayName = "读取发票信息")]
        // // [InlineData("4400204130","07810427")]
        // [InlineData("044002063111", "33768115")]
        // public async Task get_readInvoice(string invcode, string invno)
        // {       
        //     //Act
        //     var result = await _invoiceService.querPAInvoice(invcode,invno);
                    
        //     //Assert
        //     Assert.NotNull(result);
        // }





        
        // [Theory(DisplayName = "读取发票信息")]
        // [InlineData("Z21032798")]
        // public async Task get_readInvoice(string user)
        // {
        //     Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
        //     FormFileCollection filesForm = new FormFileCollection();
        //     dictionary.Add("file", "[{\"seq\":1,\"item\":1,\"filetype\":\"image/jpeg\",\"filename\":\"222.jpg\",\"ishead\":\"N\",\"key\":\"1\"}]");
        //    var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Cash2", "222.jpg");
        //     FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
        //     filesForm.Add(new FormFile(excelFS, 0, excelFS.Length, "1", "222.jpg"));
        //     IFormCollection data = new FormCollection(dictionary, filesForm);
            
        //     //Act
        //     var result = await _invoiceService.readInvoice(data,user);
                    
        //     //Assert
        //     Assert.NotNull(result);
        // }

        

        // [Theory(DisplayName = "查询发票信息")]
        // [InlineData("4400204130", "03322576")]
        // public async Task get_invoice(string invcode, string invno)
        // {
        //     //Act
        //     var result = await _invoiceService.querPAInvoice(invcode, invno);

        //     //Assert
        //     Assert.NotNull(result);
        // }



        // [Theory(DisplayName = "请求autopa api查询发票信息")]
        // [InlineData("4400204130", "03322576")]
        
        // public async Task get_painvoice(string invcode, string invno)
        // {
        //     //Act
        //     var result = await _invoiceService.querPAInvoice(invcode, invno);

        //     //Assert
        //     Assert.NotNull(result);
        // }

        [Fact(DisplayName = "上傳發票")]
        public async Task UploadInvoices()
        {
            Dictionary<string, Microsoft.Extensions.Primitives.StringValues> dictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            FormFileCollection filesForm = new FormFileCollection();
            dictionary.Add("file", "[{\"seq\":1,\"item\":1,\"filetype\":\"image/jpeg\",\"filename\":\"222.jpg\",\"ishead\":\"N\",\"key\":\"1\"}]");
            dictionary.Add("invoices", "[{\"rno\":null,\"seq\":0,\"item\":0,\"invcode\":\"034131700111\",\"invno\":\"04642294\",\"amount\":337.37,\"invdate\":\"2022-08-22T00:00:00\",\"taxamount\":0,\"oamount\":337.37,\"invstat\":\"Lock\",\"abnormalamount\":0,\"taxloss\":84.34,\"curr\":null,\"undertaker\":null,\"underwriter\":null,\"reason\":null,\"abnormal\":null,\"paymentName\":null,\"paymentNo\":null,\"collectionName\":null,\"collectionNo\":null,\"expdesc\":\"EXP004\",\"expcode\":\"發票：04642294(發票號碼),EXP004, 請重開發票或選擇承擔稅金損失\",\"invdesc\":\"034131700111&04642294&337.37&0.00&337.37&Lock\",\"paymentStat\":true,\"flag\":\"1\",\"invtype\":\"INV003\",\"salestaxno\":\"91341300MA2WAB8L7G\",\"salesname\":\"安徽省鲤江贸易有限公司\",\"batchno\":\"20220907153637\",\"buyertaxno\":\"914420007076361689\",\"buyername\":\"纬创资通（中山）有限公司\",\"tlprice\":337.37,\"verifycode\":\"64522106714114705988\",\"salesaddress\":\"安徽省宿州市埇桥区庸桥街道环城南路南环小区前栋七单元602 13083358881\",\"salesbank\":\"中国银行股份有限公司宿州凤池支行179761646464\",\"buyeraddress\":\"广东省中山市火炬高技术产业开发区科技东路38号0760-23382382\",\"buyerbank\":\"工行开发区支行2011022909024204003\",\"pwarea\":\"009329/21686-0>41770-+788850/3>430/5249+<20+5<73/9/>9/1-7965374+/<+6<2648**91<<5>1/1*58357+96025<1018-0/19+<9-08\",\"remark\":\"\",\"drawer\":\"桂贤政\",\"payee\":\"桂贤政\",\"reviewer\":\"桂生\",\"machinecode\":\"917101948111\",\"machineno\":null,\"verifystat\":\"V01\",\"verifyStateDesc\":null,\"paymentStatDesc\":\"P01\",\"padatetime\":null,\"isfill\":false,\"existautopa\":true,\"uploadmethod\":3,\"msg\":null}]");
            string s = dictionary.GetValueOrDefault("invoices");
            List<InvoiceDto>  list = JsonConvert.DeserializeObject<List<InvoiceDto>>(s);
            var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Cash2", "222.jpg");
            FileStream excelFS = new FileStream(excelFilePath, FileMode.Open);
            filesForm.Add(new FormFile(excelFS, 0, excelFS.Length, "1", "222.jpg"));
            IFormCollection data = new FormCollection(dictionary, filesForm);
            var result = await _invoiceService.UploadInvoices(data, "Z19081961");
            Assert.True(result.data.Count >= 1);
        }
    }
}

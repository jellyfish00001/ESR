using System;
using System.Threading.Tasks;
using ERS.Domain.IDomainServices;
using Volo.Abp.Application.Services;
using ERS.Application.Contracts.DTO.Invoice;
using ERS.DTO.Application;
using System.Net.Http;
using ERS.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using IdentityModel.Client;
using ExpenseApplication.Model.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ERS.QRCodeScan;
using ERS.Common;
using System.Text.RegularExpressions;
using ERS.Localization;
using System.IO;
using ERS.DTO.Invoice;
using Microsoft.AspNetCore.Mvc;
using ERS.DTO.Auth;
namespace ERS.Application.Services
{
    public class InvoiceService : ApplicationService, IInvoiceService
    {
        private IHttpClientFactory _HttpClient;
        private IInvoiceDomainService _invoiceDomainService;
        private IConfiguration _configuration;
        private IQRCodeScanRepository _qRCodeScanRepository;
        private PDFHelper _pDFHelper;
        private IAuthService _AuthService;
        public InvoiceService(IInvoiceDomainService invoiceDomainService, IConfiguration configuration,
        IQRCodeScanRepository qRCodeScanRepository, IHttpClientFactory HttpClient, PDFHelper pDFHelper, IAuthService iAuthService
        )
        {
            LocalizationResource = typeof(ERSResource);
            _invoiceDomainService = invoiceDomainService;
            _HttpClient = HttpClient;
            _configuration = configuration;
            _qRCodeScanRepository = qRCodeScanRepository;
            _pDFHelper = pDFHelper;
            _AuthService = iAuthService;
        }
        public Task<string> invoiceStat(string invcode, string invno)
        {
            throw new NotImplementedException();
        }
        public async Task<InvoiceDto> queryInvoice(string invcode, string invno)
        {
            Invoice invoice = await _invoiceDomainService.queryInvoice(invcode, invno);
            InvoiceDto invoiceDto = new InvoiceDto();
            if (invoice != null)
            {
                invoiceDto.rno = invoice.rno;
                invoiceDto.seq = invoice.seq;
                invoiceDto.item = invoice.item;
                invoiceDto.invcode = invoice.invcode;
                invoiceDto.invno = invoice.invno;
                invoiceDto.amount = invoice.amount;
                invoiceDto.invdate = invoice.invdate;
                invoiceDto.taxamount = invoice.taxamount;
                invoiceDto.oamount = invoice.oamount;
                invoiceDto.invstat = invoice.invstat;
                invoiceDto.abnormalamount = invoice.abnormalamount;
                invoiceDto.taxloss = invoice.taxloss;
                invoiceDto.curr = invoice.curr;
                invoiceDto.undertaker = invoice.undertaker;
                invoiceDto.abnormalreason = invoice.reason;
                //invoiceDto.reason = invoice.reason;
                //invoiceDto.abnormal = invoice.abnormal;
            }
            return invoice == null ? null : invoiceDto;
        }

        //请求autopa api查询发票信息
        public async Task<Result<PaInvoice>> querPAInvoice(string invcode, string invno, string token = "")
        {
            Result<PaInvoice> PaInvoice = new Result<PaInvoice>();
            string url = this._configuration.GetSection("AutoPA:Invoice").Value;
            url = url + "?invcode=" + invcode + "&invno=" + invno;
            HttpClient httpClient = _HttpClient.CreateClient();
            //if (_configuration.GetSection("isDev").Value == "true")
            token = GetIDMToken(httpClient);

            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                String jsonStr = response.Content.ReadAsStringAsync().Result;
                PaInvoice msgdata = JsonConvert.DeserializeObject<PaInvoice>(jsonStr);
                PaInvoice.message = "success";
                PaInvoice.status = 1;
                if (msgdata != null)
                {
                    PaInvoice.data = msgdata;
                }
            }
            return PaInvoice;
        }
        //请求autopa api回调发票已请款
        public async Task<ERS.DTO.Result<string>> UpdatePaInvoiceStat(string invcode, string invno, string token = "")
        {
            return await _invoiceDomainService.UpdatePaInvoiceStat(invcode, invno, token);
        }
        public string GetIDMToken(HttpClient HttpClient)
        {
            //string IDMToken = "";
            //string authorityUrl = this._configuration.GetSection("ElecDocIDM:Authority").Value;
            //// Get token
            ////var HttpClient = HttpClient.CreateClient();
            //var disco = HttpClient.GetDiscoveryDocumentAsync(authorityUrl).Result;
            //var tokenResponse = HttpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            //{
            //    Address = disco.TokenEndpoint,
            //    ClientId = "ElectronicDoc",
            //    ClientSecret = "secret",
            //    Scope = "api1",
            //}).Result;
            //if (tokenResponse.IsError)
            //{
            //    throw new Exception("IDM token获取失败:" + tokenResponse.Error);
            //}
            //IDMToken = "Bearer " + tokenResponse.AccessToken;
            //return IDMToken;

            return _AuthService.GetIDMToken().Result;
        }
        public async Task<List<InvException>> queryExpcode(List<InvException> invoice, string user)
        {
            List<InvException> invs = new List<InvException>();
            PaInvoice paInvoice = new PaInvoice();
            foreach (InvException inv in invoice)
            {
                Result<PaInvoice> data = await querPAInvoice(inv.invcode, inv.invno);
                paInvoice = data.data;
                if (paInvoice != null)
                {
                    if (paInvoice.Paymentstat != "P01" || !string.IsNullOrEmpty(paInvoice.Expcode))
                    {
                        InvException invException = new InvException();
                        invException.invcode = paInvoice.Invcode;
                        invException.invno = paInvoice.Invno;
                        invException.paymentstat = paInvoice.Paymentstat;
                        invException.expcode = paInvoice.Expcode;
                        invException.taxamount = paInvoice.Taxamount;
                        invs.Add(invException);
                    }
                }
            }
            return invs;
        }
        //public async Task<List<InvException>> invoiceExists(List<InvException> invoice)
        //{
        //    List<InvException> invExceptions = new List<InvException>();
        //    foreach (InvException inv in invoice)
        //    {
        //        Result<PaInvoice> data = await querPAInvoice(inv.invcode, inv.invno);
        //        InvoiceDto invoiceDto = await queryInvoice(inv.invcode, inv.invno);
        //        if (data.data == null && invoiceDto == null)
        //        {
        //            invExceptions.Add(inv);
        //        }
        //    }
        //    return invExceptions;
        //}
        public decimal getInvoiceTax(decimal tax, decimal taxamount)
        {
            return Math.Round(tax * taxamount, 2, MidpointRounding.AwayFromZero);
        }
        public async Task<InvoiceResult> readInvoice(IFormCollection formCollection, string user, string token)
        {
            InvoiceResult data = new InvoiceResult();
            List<InvoiceDto> list = new List<InvoiceDto>();
            List<string> msgList = new List<string>();
            decimal Taxamount = 0;
            var files = formCollection.Files;
            foreach (var file in files)
            {
                InvoiceDto invoiceDto = new InvoiceDto();//创建发票对象
                invoiceDto.paymentStat = true;
                invoiceDto.item = Convert.ToInt32(file.Name);
                Result<IList<string>> result;
                if (Path.GetExtension(file.FileName).Replace(".", "").ToLower() == "pdf")
                {
                    byte[] bytes = _pDFHelper.ChangePdfToImg(FileHelper.StreamToBytes(file.OpenReadStream()), file.FileName);
                    result = await _qRCodeScanRepository.Get(bytes, "response.jpg");
                }
                else
                    result = await _qRCodeScanRepository.Get(file);
                if (result != null && result.data != null && result.data.Count > 0)
                {
                    string str1 = result.data[0].Substring(0, 4);
                    if (str1 == "http")
                    {//通用机打发票
                        HttpClient httpClient = _HttpClient.CreateClient();
                        HttpResponseMessage response = await httpClient.GetAsync(result.data[0]);
                        if (response.IsSuccessStatusCode)
                        {
                            String jsonStr = response.Content.ReadAsStringAsync().Result;
                            Regex reg = new Regex(@"<span .*?>(.*?)</span>");
                            var tt = reg.Matches(jsonStr);
                            int i = 0;
                            foreach (Match str in tt)
                            {
                                if (i % 2 != 0 && i != 0)
                                {
                                    i = i + 1;
                                    continue;
                                }
                                i = i + 1;
                                string pp = str.Groups[1].Value;
                                string value = tt[i].Groups[1].Value;
                                if (pp == "发票代码：")
                                {
                                    invoiceDto.invcode = value;
                                }
                                if (pp == "发票号码：")
                                {
                                    invoiceDto.invno = value;
                                }
                                if (pp == "付款方名称：")
                                {
                                    invoiceDto.paymentName = value;
                                }
                                if (pp == "付款方证件号：")
                                {
                                    invoiceDto.paymentNo = value;
                                }
                                if (pp == "收款方名称：")
                                {
                                    invoiceDto.collectionName = value;
                                }
                                if (pp == "收款方证件号：")
                                {
                                    invoiceDto.collectionNo = value;
                                }
                                if (pp == "合计金额：")
                                {
                                    invoiceDto.oamount = Convert.ToDecimal(value);
                                }
                                if (pp == "开票日期：")
                                {
                                    invoiceDto.invdate = Convert.ToDateTime(value);
                                }
                                if (pp == "发票状态：")
                                {
                                    invoiceDto.invstat = value;
                                }
                            }
                        }
                    }
                    else
                    {//增值税发票
                        string[] arr = result.data[0].Split(",");
                        invoiceDto.invcode = arr[2].Trim();
                        invoiceDto.invno = arr[3].Trim();
                        // invoiceDto.oamount = Convert.ToDecimal(arr[4]);
                        // invoiceDto.invdate = DateTime.ParseExact(arr[5], "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    }
                    //检查发票是否存在
                    InvoiceDto ersInvoice = await queryInvoice(invoiceDto.invcode, invoiceDto.invno);
                    if (ersInvoice != null)
                    {
                        invoiceDto.invstat = ersInvoice.invstat;
                        invoiceDto.paymentStat = false;
                        invoiceDto.expcode = (L["001"] + invoiceDto.invno + L["002"] + L["010"]);
                    }
                    else
                    {
                        //检查发票池状态
                        Result<PaInvoice> invoice = await querPAInvoice(invoiceDto.invcode, invoiceDto.invno, token);
                        if (invoice.data != null)
                        {
                            invoiceDto.amount = Convert.ToDecimal(invoice.data.Amount);
                            invoiceDto.taxamount = Convert.ToDecimal(invoice.data.Taxamount);
                            invoiceDto.oamount = Convert.ToDecimal(invoice.data.Tlprice);//价税合计
                            invoiceDto.expdesc = invoice.data.Expcode;
                            // N可请款，F为已请款
                            invoiceDto.invstat = invoice.data.Paymentstat == "P01" ? "N" : "F";
                            if (invoice.data.Verifystat == "V01")
                            {
                                invoiceDto.invstat = "Lock";
                                if (String.IsNullOrEmpty(invoice.data.Expcode))
                                {
                                    invoiceDto.expdesc = "Lock";
                                }
                            }
                            if (!String.IsNullOrEmpty(invoiceDto.expdesc))
                            {
                                if (invoiceDto.expdesc == L["004"] || invoiceDto.expdesc == L["005"])
                                {
                                    invoiceDto.paymentStat = false;
                                    invoiceDto.expcode = (L["001"] + invoiceDto.invno + L["002"] + invoice.data.expcodeDesc + L["007"]);
                                }
                                else
                                {
                                    invoiceDto.taxloss = Math.Round(invoiceDto.oamount * Convert.ToDecimal(0.25), 2, MidpointRounding.AwayFromZero);
                                    invoiceDto.expcode = (L["001"] + invoiceDto.invno + L["002"] + invoice.data.expcodeDesc + L["003"]);
                                }
                            }
                        }
                    }
                    Taxamount += invoiceDto.taxloss;
                    invoiceDto.invdesc = invoiceDto.invcode + "&" + invoiceDto.invno + "&" + invoiceDto.amount + "&" + invoiceDto.taxamount + "&" + invoiceDto.oamount + "&" + invoiceDto.invstat;
                    list.Add(invoiceDto);
                }
            }
            data.list = list;
            data.amount = Taxamount;
            return data;
        }
        /// <summary>
        /// 读取文件识别发票信息
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<DTO.Result<List<InvoiceDto>>> ReadInvoiceInfoByFile(IFormCollection formCollection, string user, string token)
        {
            return await _invoiceDomainService.ReadInvoiceInfoByFile(formCollection, user, token);
        }
        public async Task<DTO.Result<List<InvoiceDto>>> GetInvoiceInfoByInvno(List<string> invno, string Token = "")
        {
            return await _invoiceDomainService.GetInvoiceInfoByInvno(invno, Token);
        }
        public async Task<ERS.DTO.Result<List<ERS.DTO.Result<string>>>> UploadInvoices(IFormCollection formCollection, string userId)
        {
            return await _invoiceDomainService.UploadInvoices(formCollection, userId);
        }
        public async Task<ERS.DTO.Result<byte[]>> GetInvFileFromUrl(ReadInvFileDto request)
        {
            return await _invoiceDomainService.GetInvFileFromUrl(request);
        }
        public Task<ERS.DTO.Result<bool>> UrlCheck(ReadInvFileDto request)
        {
            return _invoiceDomainService.UrlCheck(request);
        }
        public async Task<ERS.DTO.Result<string>> ShareInvoice(ShareInvoiceDto request)
        {
            return await _invoiceDomainService.ShareInvoice(request);
        }
        public async Task<HttpResponseMessage> GetInvFileFromUrlResponse(DTO.Request<ReadInvFileDto> request)
        {
            return await _invoiceDomainService.GetInvFileFromUrlResponse(request);
        }
        public async Task<byte[]> GenerateInvListExcel(string rno)
        {
            var result = await _invoiceDomainService.GenerateInvListExcel(rno);
            byte[] data = null;
            if (result.status == 1 && result.data != null)
            {
                data = result.data;
            }
            return data;
        }
        public async Task<DTO.Result<List<InvoiceDto>>> QueryInvoiceByNo(string invno, string invcode, string invoiceArea, string token)
        {
            return await _invoiceDomainService.QueryInvoiceByNo(invno, invcode, invoiceArea, token);
        }


        public async Task<ERS.DTO.Result<List<InvoiceDto>>> RecognizeInvoice([FromForm] IFormCollection formCollection, string userId, string token)
        {
            return await _invoiceDomainService.RecognizeInvoice(formCollection, userId, token);
        }

        public async Task<ERS.DTO.Result<string>> AddInvoices(IFormCollection formCollection, string userId)
        {
            return await _invoiceDomainService.AddInvoices(formCollection, userId);
        }

        public async Task<ERS.DTO.Result<InvoiceDto>> getOcrResult(string ocrid)
        {
            return await _invoiceDomainService.getOcrResult(ocrid);
        }

        public async Task<ERS.DTO.Result<string>> UpdateInvoice(InvoiceDto invoiceDto)
        {
            return await _invoiceDomainService.UpdateInvoice(invoiceDto);
        }
    }
}
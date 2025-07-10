using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Application;
using ERS.Application.Contracts.DTO.Invoice;
using ERS.Domain.IDomainServices;
using ERS.DomainServices;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.Application.BcfpInvoice;
using ERS.DTO.Auth;
using ERS.DTO.Invoice;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.QRCodeScan;
using ExpenseApplication.Model.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;


namespace ERS.Domain.DomainServices
{
    public class InvoiceDomainService : CommonDomainService, IInvoiceDomainService
    {
        private IRepository<Invoice, Guid> _invoiceRepository;
        private IQRCodeScanRepository _QRCodeScanRepository;
        private IBDInvoiceFolderRepository _BDInvoiceFolderRepository;
        private IEmployeeRepository _EmployeeRepository;
        private ICompanyRepository _CompanyRepository;
        private ICashDetailRepository _CashDetailRepository;
        private IObjectMapper _ObjectMapper;
        private IHttpClientFactory _HttpClient;
        private IConfiguration _configuration;
        private PDFHelper _PDFHelper;
        private ILogger<InvoiceDomainService> _logger;
        private IBDInvoiceTypeRepository _bDInvoiceTypeRepository;
        private IMinioDomainService _MinioDomainService;
        private IDIService _iDIService;
        private IBDCompanyCategoryDomainService _IBDCompanyCategoryDomainService;
        private IBDInvoiceTypeRepository _IBDInvoiceTypeRepository;
        private IOcrResultRepository _IOcrResultRepository;
        private IBDCompanyCategoryRepository _IBDCompanyCategoryRepository;
        private IAuthService _AuthService;

        public InvoiceDomainService(
            IRepository<Invoice, Guid> invoiceRepository,
            IObjectMapper ObjectMapper,
            IHttpClientFactory HttpClient,
            IConfiguration configuration,
            IQRCodeScanRepository QRCodeScanRepository,
            IBDInvoiceFolderRepository BDInvoiceFolderRepository,
            IEmployeeRepository EmployeeRepository,
            ICashDetailRepository CashDetailRepository,
            ICompanyRepository CompanyRepository,
            IMinioDomainService MinioDomainService,
            IBDInvoiceTypeRepository bDInvoiceTypeRepository,
            ILogger<InvoiceDomainService> logger,
            PDFHelper pDFHelper,
            IDIService iDIService,
            IBDCompanyCategoryDomainService iBDCompanyCategoryDomainService,
            IBDInvoiceTypeRepository iBDInvoiceTypeRepository,
            IOcrResultRepository iOcrResultRepository,
            IBDCompanyCategoryRepository iBDCompanyCategoryRepository,
            IAuthService iAuthService
            )
        {
            _invoiceRepository = invoiceRepository;
            _ObjectMapper = ObjectMapper;
            _HttpClient = HttpClient;
            _configuration = configuration;
            _QRCodeScanRepository = QRCodeScanRepository;
            _BDInvoiceFolderRepository = BDInvoiceFolderRepository;
            _EmployeeRepository = EmployeeRepository;
            _CompanyRepository = CompanyRepository;
            _CashDetailRepository = CashDetailRepository;
            _MinioDomainService = MinioDomainService;
            _PDFHelper = pDFHelper;
            _logger = logger;
            _bDInvoiceTypeRepository = bDInvoiceTypeRepository;
            _iDIService = iDIService;
            _IBDCompanyCategoryDomainService = iBDCompanyCategoryDomainService;
            _IBDInvoiceTypeRepository = iBDInvoiceTypeRepository;
            _IOcrResultRepository = iOcrResultRepository;
            _IBDCompanyCategoryRepository = iBDCompanyCategoryRepository;
            _AuthService = iAuthService;
        }

        //生成单子后修改发票池状态
        public async Task<Result<string>> UpdatePaInvoiceStat(string invcode, string invno, string token)
        {
            Result<string> result = new Result<string>();
            string url = this._configuration.GetSection("AutoPA:UpdatePayStat").Value;
            url = url + "?invcode=" + invcode + "&invno=" + invno;
            HttpClient httpClient = _HttpClient.CreateClient();
            //if (_configuration.GetSection("isDev").Value == "true")
            token = GetIDMToken(httpClient);
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                String jsonStr = response.Content.ReadAsStringAsync().Result;
                PaResult<string> paResult = JsonConvert.DeserializeObject<PaResult<string>>(jsonStr);
                result.message = paResult.data;
            }
            return result;
        }
        private async Task<Result<string>> UpdatePaInvoiceStat(List<UpdatePayStatDto> invList, string status, string token)
        {
            Result<string> result = new Result<string>();
            string url = this._configuration.GetSection("AutoPA:UpdatePayStat").Value;
            url = url + "?Paymentstat=" + status;
            HttpClient httpClient = _HttpClient.CreateClient();
            //if (_configuration.GetSection("isDev").Value == "true")
            token = GetIDMToken(httpClient);
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            string datas = JsonConvert.SerializeObject(invList);
            PaResult<string> response = await httpClient.PostHelperAsync<PaResult<string>>(url, datas);
            if (response == null)
            {
                _logger.LogError("AutoPA:变更发票状态({Status})失败。status:fail。入参: {Datas}", status, datas);
            }
            else if (response.data == "success")
            {
                _logger.LogInformation("AutoPA:变更发票状态({Status})。status: {ResponseData}。入参: {Datas}", status, response.data, datas);
            }
            return result;
        }
        private async Task<Result<string>> UpdatePaInvoiceStat(ERSApplyDto ersApplyDto, string token)
        {
            Result<string> result = new Result<string>();
            string url = this._configuration.GetSection("AutoPA:UpdatePaInvoiceStat").Value;
            HttpClient httpClient = _HttpClient.CreateClient();
            //if (_configuration.GetSection("isDev").Value == "true")
            token = GetIDMToken(httpClient);
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            string datas = JsonConvert.SerializeObject(ersApplyDto);
            PaResult<string> response = await httpClient.PostHelperAsync<PaResult<string>>(url, datas);
            if (response == null)
            {
                _logger.LogError("AutoPA:变更发票状态({Paymentstat})失败。status:fail。入参: {Datas}", ersApplyDto.Paymentstat, datas); ;
            }
            else if (response.data == "success")
            {
                _logger.LogInformation("AutoPA:变更发票状态({Paymentstat})。status: {ResponseData}。入参: {Datas}", ersApplyDto.Paymentstat, response.data, datas);
            }
            return result;
        }
        /// <summary>
        /// 更新發票狀態為已請款
        /// </summary>
        /// <param name="invoiceList"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<string>> UpdateInvoiceToRequested(List<UpdatePayStatDto> invList, string token)
        {
            Result<string> result = new Result<string>();
            //if (_configuration.GetSection("isDev").Value == "true") return null;
            if (invList.Where(w => !String.IsNullOrEmpty(w.invno)).Count() > 0)
            {
                result = await UpdatePaInvoiceStat(invList, "P02", token);
            }
            return result;
        }
        /// <summary>
        /// 更新發票狀態為已請款 new
        /// </summary>
        /// <param name="eRSApplyDto"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<string>> UpdateInvStatToRequested(ERSApplyDto eRSApplyDto, string token)
        {
            Result<string> result = new Result<string>();
            //if (_configuration.GetSection("isDev").Value == "true") return null;
            if (eRSApplyDto.Invoices.Any(w => !string.IsNullOrEmpty(w.Invno)))
            {
                eRSApplyDto.Paymentstat = "P02";
                result = await UpdatePaInvoiceStat(eRSApplyDto, token);
            }
            return result;
        }
        /// <summary>
        /// 更新發票狀態為待請款
        /// </summary>
        /// <param name="rno"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<string>> UpdateInvoiceToUnrequested(string rno, string token)
        {
            List<UpdatePayStatDto> invs = await (await _invoiceRepository.WithDetailsAsync()).Where(x => x.rno == rno).Select(i => new UpdatePayStatDto() { invno = i.invno, invcode = i.invcode }).AsNoTracking().ToListAsync();
            return await UpdateInvStatToUnrequested(invs, token);
            //return await UpdateInvoiceToUnrequested(invs, token);
        }
        /// <summary>
        /// 更新發票狀態為待請款
        /// </summary>
        /// <param name="invoiceList"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<string>> UpdateInvoiceToUnrequested(List<UpdatePayStatDto> invList, string token)
        {
            //if (_configuration.GetSection("isDev").Value == "true") return null;
            return await UpdatePaInvoiceStat(invList, "P01", token);
        }
        /// <summary>
        /// 更新發票狀態為待請款 new
        /// </summary>
        /// <param name="invoiceList"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<string>> UpdateInvStatToUnrequested(List<UpdatePayStatDto> invList, string token)
        {
            //if (_configuration.GetSection("isDev").Value == "true") return null;
            ERSApplyDto eRSApplyDto = new()
            {
                Paymentstat = "P01",
                Invoices = _ObjectMapper.Map<List<UpdatePayStatDto>, List<ERSInvDto>>(invList)
            };
            return await UpdatePaInvoiceStat(eRSApplyDto, token);
        }
        public async Task<Result<string>> UpdateInvPayInfo(List<ERSPayDto> ersPayDtos, bool isCancel, string token)
        {
            Result<string> result = new Result<string>();
            string url = this._configuration.GetSection("AutoPA:UpdatePaInvPayInfo").Value;
            url = url + "?isCancel=" + isCancel;
            HttpClient httpClient = _HttpClient.CreateClient();
            //if (_configuration.GetSection("isDev").Value == "true")
            token = GetIDMToken(httpClient);
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            string datas = JsonConvert.SerializeObject(ersPayDtos);
            PaResult<string> response = await httpClient.PostHelperAsync<PaResult<string>>(url, datas);
            if (response == null)
            {
                _logger.LogError("AutoPA:变更发票付款信息失败。status:fail。入参: {Datas}", datas);
            }
            else if (response.data == "success")
            {
                _logger.LogInformation("AutoPA:变更发票付款信息成功。status: {Status}。 入参: {Datas}", response.data, datas);
            }
            return result;
        }
        public async Task<Invoice> queryInvoice(string invcode, string invno)
        {
            return (await _invoiceRepository.WithDetailsAsync()).Where(b => b.invcode == invcode && b.invno == invno).AsNoTracking().FirstOrDefault();
        }
        /// <summary>
        /// 存在发票池&&pdf格式&&没异常 —— 不需加签财务
        /// 存在发票池&&电子发票&&pdf格式 —— 不需送纸本单及提示
        /// </summary>
        /// <param name="head"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<CashResult>> checkInvoicePaid(CashHead head, string token)
        {
            Result<CashResult> result = new Result<CashResult>();
            CashResult invoiceStat = new CashResult();
            string msg = "";
            bool status = true;  // 是否全为电子票
            bool ExistElecInvStatus = true; // 电子发票 & 非pdf格式 & 没异常
            bool ElecButNotPdf = false;
            IList<Invoice> list = head.InvoiceList;
            List<string> electType = (await _bDInvoiceTypeRepository.ReadElectInvByComapny(head.company)).Select(i => i.InvType).ToList();
            electType = ERS.Common.FontConversionHelper.ChangeByList(electType);
            IList<PaInvoice> invoices = await querPAInvoices(list, token);
            foreach (Invoice item in list)
            {
                if (item.invoiceid == null) ExistElecInvStatus = false;
                if (item.invoiceid == null) continue;
                // 无需去发票池识别，并加签&送纸本
                if (string.IsNullOrEmpty(item.invno))
                {
                    ExistElecInvStatus = false;
                    status = false;
                    continue;
                }
                bool existAutopa = await _BDInvoiceFolderRepository.CheckInvoiceExistAutopa(item.invno, item.invcode);
                //Result<PaInvoice> invoice = await querPAInvoice(string.IsNullOrEmpty(item.invcode) ? "" : item.invcode, item.invno, token);
                PaInvoice invoice = null;
                if (invoices != null)
                {
                    invoice = invoices.Where(i => i.Invcode == item.invcode & i.Invno == item.invno).FirstOrDefault();
                }
                if (invoice != null)
                {
                    //string expdesc = invoice.data.Expcode;
                    //string Invtype = invoice.data.Invtype;
                    //if (invoice.data.Paymentstat == "P02" || invoice.data.Paymentstat == "P03")
                    //{
                    //    msg += (L["001"] + invoice.data.Invno + L["002"] + invoice.data.paymentStatDesc + L["007"]) + ",";
                    //    continue;
                    //}
                    //if (!String.IsNullOrEmpty(invoice.data.Expcode))
                    //{
                    //    if (invoice.data.Expcode.Contains("EXP021") || invoice.data.Expcode.Contains("EXP005"))
                    //    {
                    //        msg += (L["001"] + invoice.data.Invno + L["002"] + invoice.data.expcodeDesc + L["007"]) + ",";
                    //        continue;
                    //    }
                    //    if (expdesc == L["004"] || expdesc == L["005"])
                    //    {
                    //        msg += (L["001"] + invoice.data.Invno + L["002"] + expdesc + L["007"]) + ",";
                    //        continue;
                    //    }
                    //}
                    string expdesc = invoice.Expcode;
                    string Invtype = invoice.Invtype;
                    if (invoice.Paymentstat == "P02" || invoice.Paymentstat == "P03")
                    {
                        msg += (L["001"] + invoice.Invno + L["002"] + invoice.paymentStatDesc + L["007"]) + ",";
                        continue;
                    }
                    if (!String.IsNullOrEmpty(invoice.Expcode))
                    {
                        if (invoice.Expcode.Contains("EXP021") || invoice.Expcode.Contains("EXP005"))
                        {
                            msg += (L["001"] + invoice.Invno + L["002"] + invoice.expcodeDesc + L["007"]) + ",";
                            continue;
                        }
                        if (expdesc == L["004"] || expdesc == L["005"])
                        {
                            msg += (L["001"] + invoice.Invno + L["002"] + expdesc + L["007"]) + ",";
                            continue;
                        }
                    }
                }
                item.invtype = ERS.Common.FontConversionHelper.ChangeBySingle(item.invtype);
                if (!electType.Contains(item.invtype))
                {
                    status = false;
                    ExistElecInvStatus = false;
                }
                if (existAutopa)
                {
                    string path = head.CashFileList.Where(i => i.seq == item.seq && i.item == item.item && i.status != "F").FirstOrDefault()?.path;
                    if (electType.Contains(item.invtype) && ((item.invno.Length != 20 && Path.GetExtension(path).Replace(".", "").ToLower() != "pdf") || (item.invno.Length == 20 && !string.IsNullOrEmpty(path) && Path.GetExtension(path).Replace(".", "").ToLower() != "pdf")))
                    {
                        ElecButNotPdf = true;
                        ExistElecInvStatus = false;
                    }
                    if (electType.Contains(item.invtype) && Path.GetExtension(head.CashFileList.Where(i => i.seq == item.seq && i.item == item.item && i.status != "F").FirstOrDefault()?.path).Replace(".", "").ToLower() == "pdf" && !string.IsNullOrEmpty(invoice.Expcode))
                        ExistElecInvStatus = false;
                }
                else
                {
                    if (electType.Contains(item.invtype) && (item.invno.Length != 20 && Path.GetExtension(head.CashFileList.Where(i => i.seq == item.seq && i.item == item.item && i.status != "F").FirstOrDefault()?.path).Replace(".", "").ToLower() != "pdf"))
                        ElecButNotPdf = true;
                    if (electType.Contains(item.invtype))
                        status = false;
                    ExistElecInvStatus = false;
                }
            }
            if (!string.IsNullOrEmpty(msg))
            {
                invoiceStat.message = msg;
                result.message = msg;
                result.status = 2;
            }
            else
            {
                result.status = 1;
            }
            // 是否需要送纸本单
            invoiceStat.Stat = !status;
            invoiceStat.ElecInvStat = !ExistElecInvStatus;
            invoiceStat.ElecButNotPdf = ElecButNotPdf;
            result.data = invoiceStat;
            return result;
        }
        //请求autopa api查询发票信息
        public async Task<Result<PaInvoice>> querPAInvoice(string invcode, string invno, string token = "")
        {
            Result<PaInvoice> PaInvoice = new Result<PaInvoice>();
            string url = this._configuration.GetSection("AutoPA:Invoice").Value;
            url = url + "?invcode=" + invcode + "&invno=" + invno;
            HttpClient httpClient = _HttpClient.CreateClient();
            //using HttpClient httpClient = new HttpClient();//http对象
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
        //请求autopa api查询发票信息
        public async Task<IList<PaInvoice>> querPAInvoices(IList<Invoice> list, string Token = "")
        {
            string url = this._configuration.GetSection("AutoPA:Invoice").Value;
            List<ERSInvDto> eRSInvs = new List<ERSInvDto>();
            foreach (var item in list)
            {
                ERSInvDto eRSInvDto = new ERSInvDto();
                eRSInvDto.Invcode = string.IsNullOrEmpty(item.invcode) ? "" : item.invcode;
                eRSInvDto.Invno = item.invno;
                eRSInvs.Add(eRSInvDto);
            }
            string datas = JsonConvert.SerializeObject(eRSInvs);
            //using HttpClient httpClient = new HttpClient();//http对象
            HttpClient httpClient = _HttpClient.CreateClient();
            //if (_configuration.GetSection("isDev").Value == "true")
            Token = GetIDMToken(httpClient);
            IList<PaInvoice> response = await httpClient.PostHelperAsync<IList<PaInvoice>>(url, datas, token: Token);
            return response;
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
        /// <summary>
        /// 全电票请求autopa获取信息
        /// </summary>
        /// <param name="invno"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public async Task<Result<List<InvoiceDto>>> GetInvoiceInfoByInvno(List<string> invno, string token = "")
        {
            Result<List<InvoiceDto>> result = new()
            {
                data = new()
            };
            for (int i = 0; i < invno.Count; i++)
            {
                InvoiceDto invoiceDto = new();
                invoiceDto.paymentStat = true;
                Result<PaInvoice> paData = new Result<PaInvoice>();
                string url = this._configuration.GetSection("AutoPA:Invoice").Value;
                url = url + "?invno=" + invno[i];
                HttpClient httpClient = _HttpClient.CreateClient();
                //if (_configuration.GetSection("isDev").Value == "true")
                token = GetIDMToken(httpClient);
                httpClient.DefaultRequestHeaders.Add("Authorization", token);
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    String jsonStr = response.Content.ReadAsStringAsync().Result;
                    PaInvoice msgdata = JsonConvert.DeserializeObject<PaInvoice>(jsonStr);
                    paData.message = "success";
                    paData.status = 1;
                    if (msgdata != null)
                    {
                        paData.data = msgdata;
                    }
                }
                if (paData.data != null)
                {
                    invoiceDto.invno = paData.data.Invno;//发票号码
                    invoiceDto.batchno = paData.data.Batchno;
                    invoiceDto.invdate = Convert.ToDateTime(paData.data.Invdate);
                    invoiceDto.invstat = paData.data.Invstat;
                    invoiceDto.salestaxno = paData.data.Salestaxno;
                    invoiceDto.salesname = paData.data.Salesname;//销售方
                    invoiceDto.buyertaxno = paData.data.Buyertaxno;
                    invoiceDto.buyername = paData.data.Buyername;
                    invoiceDto.invdate = Convert.ToDateTime(paData.data.Invdate);//开票日期
                    invoiceDto.amount = Convert.ToDecimal(paData.data.Amount);
                    invoiceDto.taxamount = Convert.ToDecimal(paData.data.Taxamount);
                    //invoiceDto.tlprice = paData.data.Tlprice;//含税总金额
                    invoiceDto.verifycode = paData.data.Verifycode;
                    invoiceDto.salesaddress = paData.data.Salesaddress;
                    invoiceDto.salesbank = paData.data.Salesbank;
                    invoiceDto.buyeraddress = paData.data.Buyeraddress;
                    invoiceDto.buyerbank = paData.data.Buyerbank;
                    invoiceDto.remark = paData.data.Remark;
                    invoiceDto.drawer = paData.data.Drawer;
                    invoiceDto.payee = paData.data.Payee;
                    invoiceDto.reviewer = paData.data.Reviewer;
                    invoiceDto.machinecode = paData.data.Machinecode;
                    invoiceDto.machineno = paData.data.Machineno;
                    invoiceDto.verifystat = paData.data.Verifystat;//发票校验状态
                    invoiceDto.verifyStateDesc = paData.data.VerifyStateDesc;//发票校验状态描述
                    invoiceDto.paymentStatDesc = paData.data.paymentStatDesc;//发票请款状态
                    invoiceDto.expdesc = paData.data.expcodeDesc;//异常原因
                    //invoiceDto.existautopa = true;
                    //invoiceDto.uploadmethod = ERSConsts.Invoice_UploadMethod_1;
                    invoiceDto.source = ERSConsts.InvoiceSourceEnum.InvoicePool.ToValue();
                    invoiceDto.curr = ERSConsts.CurrencyEnum.RMB.ToString();//发票池有数据，默认币别给RMB
                    invoiceDto.flag = paData.data.Invno;
                    invoiceDto.taxrate = paData.data.taxrete;
                    invoiceDto.invdesc = paData.data.Invdesc;
                    invoiceDto.invtype = paData.data.Invdesc;
                    // N可请款，F为已请款
                    invoiceDto.invstat = paData.data.Paymentstat == "P01" ? "N" : "F";
                    if (paData.data.Verifystat == "V01")
                    {
                        invoiceDto.invstat = "Lock";
                        if (String.IsNullOrEmpty(paData.data.Expcode))
                        {
                            invoiceDto.expdesc = "Lock";
                        }
                    }
                    if (!String.IsNullOrEmpty(invoiceDto.expdesc))
                    {
                        if (invoiceDto.expdesc == L["004"] || invoiceDto.expdesc == L["005"])
                        {
                            invoiceDto.paymentStat = false;
                            invoiceDto.expcode = paData.data.Expcode;
                            invoiceDto.expinfo = (L["001"] + invoiceDto.invno + L["002"] + invoiceDto.expdesc + L["007"]);
                        }
                        else
                        {
                            invoiceDto.taxloss = Math.Round(invoiceDto.oamount * Convert.ToDecimal(0.25), 2, MidpointRounding.AwayFromZero);
                            invoiceDto.expcode = paData.data.Expcode;
                            invoiceDto.expinfo = (L["001"] + invoiceDto.invno + L["002"] + invoiceDto.expdesc + L["003"]);
                        }
                    }
                    //如果autopa传过来的paymentstat为p03，则无法请款
                    if (paData.data.Paymentstat == "P03")
                    {
                        invoiceDto.paymentStat = false;
                    }
                    //红冲发票无法请款
                    if (paData.data.Expcode.Contains("EXP021") || paData.data.Expcode.Contains("EXP005"))
                    {
                        invoiceDto.paymentStat = false;
                        invoiceDto.expinfo = paData.data.expcodeDesc;
                    }
                    if (invoiceDto.invstat == "F")
                    {
                        invoiceDto.paymentStat = false;
                        invoiceDto.expinfo = L["UploadInvoice-Requested"];
                    }
                    var existList = (await _BDInvoiceFolderRepository.WithDetailsAsync()).Where(w => w.invno == invoiceDto.invno).Select(g => new { g.invno, g.invcode, g.emplid }).AsNoTracking().ToList();
                    //已被上傳過得發票
                    if (existList.Count > 0)
                    {
                        invoiceDto.paymentStat = false;
                        invoiceDto.expinfo = L["UploadInvoice-UploadFail"] + L["UploadInvoice-ExistFolder"] + existList.FirstOrDefault().emplid;
                    }
                    result.data.Add(invoiceDto);
                }
                else
                {
                    result.message += "全電發票號碼：" + invno[i] + L["在发票池无数据"];
                    result.status = 2;
                }
            }
            return result;
        }
        /// <summary>
        /// 讀取文件發票信息，QRCode識別發票信息，根據發票號碼去發票池進行資料匹配
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<List<InvoiceDto>>> ReadInvoiceInfoByFile(IFormCollection formCollection, string user, string token)
        {
            Result<List<InvoiceDto>> resultList = new Result<List<InvoiceDto>>();
            List<InvoiceDto> list = new List<InvoiceDto>();

            try
            {
                var files = formCollection.Files;
                foreach (var file in files)
                {
                    InvoiceDto invoiceDto = new InvoiceDto();//创建发票对象
                    invoiceDto.paymentStat = true;
                    invoiceDto.flag = file.Name;
                    ERS.Common.Result<IList<string>> result;

                    if (file.ContentType.Equals("application/pdf"))
                    {
                        _logger.LogInformation("change to pdf");
                        byte[] bytes = _PDFHelper.ChangePdfToImg(FileHelper.StreamToBytes(file.OpenReadStream()), Path.GetExtension(file.FileName).Replace(".", "").ToLower() == "pdf" ? file.FileName : (file.FileName + ".pdf"));
                        result = await _QRCodeScanRepository.Get(bytes, "response.jpg");
                    }
                    else
                        result = await _QRCodeScanRepository.Get(file);
                    _logger.LogInformation("scan qrcode result: {Result}", JsonConvert.SerializeObject(result));
                    //识别成功
                    if (result != null && result.data != null && result.data.Count > 0)
                    {
                        //invoiceDto.isfill = false;//识别成功，信息齐全无需填补。
                        string str1 = result.data[0].Substring(0, 4);
                        if (str1 == "http" && !result.data[0].Contains("bcfp.shenzhen") && result.data[0].Contains("guangdong.chinatax.gov"))
                        {   //通用机打发票
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
                        else if (result.data[0].StartsWith("https") && result.data[0].Contains("bcfp.shenzhen"))
                        {
                            InvoiceDto bcfpInfo = await GetBcfpInvInfo(result.data[0]);
                            invoiceDto.invcode = bcfpInfo.invcode;
                            invoiceDto.invno = bcfpInfo.invno;
                        }
                        else
                        {
                            int num = Regex.Matches(result.data[0], ",").Count;
                            if (num < 3)
                            {
                                //invoiceDto.isfill = true;//识别失败
                                invoiceDto.msg = L["ReadInvoice-QRCodeError"];
                                list.Add(invoiceDto);
                                continue;
                            }
                            //增值税发票
                            string[] arr = result.data[0].Split(",");
                            invoiceDto.invcode = arr[2].Trim();
                            invoiceDto.invno = arr[3].Trim();
                            // invoiceDto.oamount = Convert.ToDecimal(arr[4]);
                            // invoiceDto.invdate = DateTime.ParseExact(arr[5], "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        }

                        //检查发票池状态
                        Result<PaInvoice> invoice = await querPAInvoice(invoiceDto.invcode, invoiceDto.invno, token);
                        var existList = (await _BDInvoiceFolderRepository.WithDetailsAsync()).Where(w => w.invno == invoiceDto.invno).Select(g => new { g.invno, g.invcode, g.emplid }).AsNoTracking().ToList();
                        if (invoice.data != null)
                        {
                            //invoiceDto.existautopa = true;
                            invoiceDto.source = ERSConsts.InvoiceSourceEnum.InvoicePool.ToValue();
                            invoiceDto.amount = Convert.ToDecimal(invoice.data.Amount);
                            invoiceDto.taxamount = Convert.ToDecimal(invoice.data.Taxamount);
                            invoiceDto.oamount = Convert.ToDecimal(invoice.data.Tlprice);//价税合计
                            invoiceDto.expdesc = invoice.data.expcodeDesc;
                            invoiceDto.expcode = invoice.data.Expcode;
                            invoiceDto.batchno = invoice.data.Batchno;
                            invoiceDto.salestaxno = invoice.data.Salestaxno;
                            invoiceDto.salesname = invoice.data.Salesname;
                            invoiceDto.buyertaxno = invoice.data.Buyertaxno;
                            invoiceDto.buyername = invoice.data.Buyername;
                            //invoiceDto.tlprice = invoice.data.Tlprice;
                            invoiceDto.verifycode = invoice.data.Verifycode;
                            invoiceDto.salesaddress = invoice.data.Salesaddress;
                            invoiceDto.salesbank = invoice.data.Salesbank;
                            invoiceDto.buyeraddress = invoice.data.Buyeraddress;
                            invoiceDto.buyerbank = invoice.data.Buyerbank;
                            //invoiceDto.pwarea = invoice.data.Pwarea;
                            invoiceDto.remark = invoice.data.Remark;
                            invoiceDto.drawer = invoice.data.Drawer;
                            invoiceDto.payee = invoice.data.Payee;
                            invoiceDto.reviewer = invoice.data.Reviewer;
                            invoiceDto.machinecode = invoice.data.Machinecode;
                            invoiceDto.machineno = invoice.data.Machineno;
                            invoiceDto.verifystat = invoice.data.Verifystat;
                            invoiceDto.paymentStatDesc = invoice.data.paymentStatDesc;
                            invoiceDto.taxrate = invoice.data.taxrete;
                            invoiceDto.invdate = Convert.ToDateTime(invoice.data.Invdate);
                            // N可请款，F为已请款
                            invoiceDto.invstat = invoice.data.Paymentstat == "P01" ? "N" : "F";
                            invoiceDto.verifystat = invoice.data.Verifystat;
                            invoiceDto.verifyStateDesc = invoice.data.VerifyStateDesc;
                            invoiceDto.invdesc = invoice.data.Invdesc;
                            invoiceDto.curr = ERSConsts.CurrencyEnum.RMB.ToString();
                            invoiceDto.expinfo = invoice.data.expcodeDesc;
                            if (invoice.data.Verifystat == "V01")
                            {
                                invoiceDto.invstat = "Lock";
                                if (String.IsNullOrEmpty(invoice.data.Expcode))
                                {
                                    invoiceDto.expcode = "Lock";
                                }
                            }
                            if (!String.IsNullOrEmpty(invoiceDto.expdesc))
                            {
                                if (invoiceDto.expdesc == L["004"] || invoiceDto.expdesc == L["005"])
                                {
                                    invoiceDto.paymentStat = false;
                                }
                            }
                            if (invoiceDto.expcode != "Lock") invoiceDto.expcode = invoiceDto.expcode;
                            //如果autopa传过来的paymentstat为p03，则无法请款
                            if (invoice.data.Paymentstat == "P03")
                            {
                                invoiceDto.paymentStat = false;
                            }
                            //红冲发票无法请款
                            if (invoice.data.Expcode.Contains("EXP021") || invoice.data.Expcode.Contains("EXP005"))
                            {
                                invoiceDto.paymentStat = false;
                                invoiceDto.expinfo = invoice.data.expcodeDesc;
                            }
                            if (invoiceDto.invstat == "F")
                            {
                                invoiceDto.paymentStat = false;
                                invoiceDto.expinfo = L["UploadInvoice-Requested"];
                            }
                            //已被上傳過得發票
                            if (existList.Count > 0)
                            {
                                invoiceDto.paymentStat = false;
                                invoiceDto.expinfo = L["UploadInvoice-UploadFail"] + L["UploadInvoice-ExistFolder"] + existList.FirstOrDefault().emplid;
                            }
                            if (!string.IsNullOrEmpty(invoice.data.Invtype))
                            {
                                BDInvoiceType bdInvoiceType = (await _IBDInvoiceTypeRepository.WithDetailsAsync()).FirstOrDefault(w => w.InvTypeCode == invoice.data.Invtype);
                                if (bdInvoiceType != null)
                                {
                                    invoiceDto.invtype = bdInvoiceType.InvType;
                                    invoiceDto.invtypecode = bdInvoiceType.InvTypeCode;
                                    //前端会用到发票类型判断该显示发票哪些栏位
                                    invoiceDto.invoicecategory = bdInvoiceType.Category;
                                }
                            }
                        }
                        //二维码可识别，但发票池无数据
                        else
                        {
                            //invoiceDto.isfill = true;
                            //invoiceDto.existautopa = false;
                            invoiceDto.msg = L["ReadInvoice-PaNoData"];
                        }
                    }
                    else
                    {
                        //invoiceDto.isfill = true;//识别失败
                        invoiceDto.msg = L["ReadInvoice-RecognizeError"];
                    }
                    list.Add(invoiceDto);
                }

                resultList.data = list;
            }
            catch (Exception ex)
            {
                _logger.LogError("ReadInvoiceInfoByFile error: {Error}", ex.Message);
                resultList.status = 2;
                resultList.message = ex.Message;
            }

            return resultList;
        }

        /// <summary>
        /// 上傳發票
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<List<Result<string>>>> UploadInvoices(IFormCollection formCollection, string userId)
        {
            string area = await _MinioDomainService.GetMinioArea(userId);
            Result<List<Result<string>>> result = new();
            string invoices = formCollection["invoices"];
            List<InvoiceDto> invData = System.Text.Json.JsonSerializer.Deserialize<List<InvoiceDto>>(invoices);
            List<Result<string>> results = new List<Result<string>>();

            //string companycode = await _EmployeeRepository.GetCompanyCodeByUser(userId);
            //string company = await _CompanyRepository.GetCompanyByCode(companycode);
            List<string> invcodes = invData.Where(w => !String.IsNullOrEmpty(w.invno)).Select(s => s.invno).Distinct().ToList();
            var existList = (await _BDInvoiceFolderRepository.WithDetailsAsync()).Where(w => invcodes.Contains(w.invno)).Select(g => new { g.invno, g.invcode, g.emplid }).AsNoTracking().ToList();//1
            List<BDInvoiceFolder> bDInvoiceFolders = new List<BDInvoiceFolder>();
            List<string> fileFlagList = formCollection.Files.Select(w => w.Name).ToList();
            foreach (var item in invData)
            {
                //if (!item.tlprice.HasValue || String.IsNullOrEmpty(item.invtype) || String.IsNullOrEmpty(item.curr))
                if (String.IsNullOrEmpty(item.invtype) || String.IsNullOrEmpty(item.curr))
                {
                    results.Add(new Result<string> { data = item.flag, message = L["UploadInvoice-UploadFail"] + L["UploadInvoice-IncompleteInfo"], status = 2 });
                    continue;
                }
                if (existList.Where(s => s.invcode == (String.IsNullOrEmpty(item.invcode) ? "" : item.invcode) && s.invno == item.invno).Count() > 0)
                {
                    var existInvoice = existList.Where(s => s.invcode == (String.IsNullOrEmpty(item.invcode) ? "" : item.invcode) && s.invno == item.invno).FirstOrDefault();
                    results.Add(new Result<string> { data = item.flag, message = L["UploadInvoice-UploadFail"] + L["UploadInvoice-ExistFolder"] + existInvoice.emplid, status = 2 });
                    continue;
                }
                if (item.invstat == "F")
                {
                    results.Add(new Result<string> { data = item.flag, message = L["UploadInvoice-UploadFail"] + L["UploadInvoice-Requested"], status = 2 });
                    continue;
                }
                if (item.expcode != null)
                {
                    if (item.expcode.Contains("EXP004"))
                    {
                        results.Add(new Result<string> { data = item.flag, message = L["UploadInvoice-UploadFail"] + L["UploadInvoice-InvoiceInvalid"], status = 2 });
                        continue;
                    }
                    if (item.expcode.Contains("EXP005") || item.expcode.Contains("EXP021"))
                    {
                        results.Add(new Result<string> { data = item.flag, message = L["UploadInvoice-UploadFail"] + L["UploadInvoice-RedInvoice"], status = 2 });
                        continue;
                    }
                }
                BDInvoiceFolder bDInvoiceFolder = _ObjectMapper.Map<InvoiceDto, BDInvoiceFolder>(item);
                bDInvoiceFolder.buyername = bDInvoiceFolder.buyername;
                bDInvoiceFolder.invdate = bDInvoiceFolder.invdate.HasValue ? Convert.ToDateTime(item.invdate).ToLocalTime() : null;
                bDInvoiceFolder.paytype = ERSConsts.InvoicePayTypeEnum.Unrequested.ToValue();
                bDInvoiceFolder.emplid = userId;
                bDInvoiceFolder.company = ERSConsts.Company_All;
                bDInvoiceFolder.cuser = userId;
                bDInvoiceFolder.cdate = System.DateTime.Now;
                if (String.IsNullOrEmpty(bDInvoiceFolder.invcode))
                {
                    bDInvoiceFolder.invcode = "";
                }
                foreach (var item1 in formCollection.Files)
                {
                    if (item1.Name == item.flag)
                    {
                        string invno = invData.Where(w => w.flag == item1.Name).FirstOrDefault()?.invno;
                        if (String.IsNullOrEmpty(invno))
                        {
                            invno = "other";
                        }
                        string path = "";
                        string type = "";
                        if (!string.IsNullOrEmpty(_configuration.GetSection("UnitTest")?.Value))
                            type = "image/jpeg";
                        else
                            type = item1.ContentType;
                        using (var steam = item1.OpenReadStream())
                        {
                            if (!String.IsNullOrEmpty(invno))
                            {
                                if (item1.ContentType == "image/jpeg")
                                {
                                    path = await _MinioDomainService.PutInvoiceAsync(invno, Path.GetExtension(item1.FileName).Replace(".", "").ToLower() == "jpg" ? item1.FileName : (item1.FileName + ".jpg"), steam, type, area);
                                }
                                else if (item1.ContentType == "image/png")
                                {
                                    path = await _MinioDomainService.PutInvoiceAsync(invno, Path.GetExtension(item1.FileName).Replace(".", "").ToLower() == "png" ? item1.FileName : (item1.FileName + ".png"), steam, type, area);
                                }
                                else if (item1.ContentType == "application/pdf")
                                {
                                    path = await _MinioDomainService.PutInvoiceAsync(invno, Path.GetExtension(item1.FileName).Replace(".", "").ToLower() == "pdf" ? item1.FileName : (item1.FileName + invno + ".pdf"), steam, type, area);
                                }
                            }
                            //path = await _MinioDomainService.PutInvoiceAsync(invno, item1.FileName, steam, type);
                        }
                        bDInvoiceFolder.filepath = path;
                        bDInvoiceFolder.category = type;
                    }
                }
                if (!String.IsNullOrEmpty(item.invno))
                {
                    if (bDInvoiceFolders.Select(w => w.invno).Contains(item.invno))
                    {
                        results.Add(new Result<string> { data = item.flag, message = L["UploadInvoice-UploadFail"] + L["UploadInvoice-ExistUploadList"], status = 2 });
                        continue;
                    }
                }
                bDInvoiceFolders.Add(bDInvoiceFolder);
                results.Add(new Result<string> { data = item.flag, message = L["UploadInvoice-UploadSuccess"] });
            }
            await _BDInvoiceFolderRepository.InsertManyAsync(bDInvoiceFolders);
            result.data = results;
            result.total = results.Count;
            return result;
        }
        //url检查
        public async Task<Result<bool>> UrlCheck(ReadInvFileDto request)
        {
            Result<bool> result = new Result<bool>();
            result.data = true;
            request.url = request.url.Trim().Replace(" ", "");
            string regex = @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$";
            if (String.IsNullOrEmpty(request.url) || !Regex.IsMatch(request.url, regex))
            {
                result.message = L["url格式不合法，请检查！"];
                result.status = 2;
                result.data = false;
                return result;
            }
            HttpClient httpClient = _HttpClient.CreateClient();
            HttpResponseMessage responseMessage = await httpClient.GetAsync(request.url);
            if (!responseMessage.IsSuccessStatusCode)
            {
                result.message = L["该url返回数据异常，请检查！"];
                result.status = 2;
                result.data = false;
                return result;
            }
            return result;
        }
        //读取url获取文件
        public async Task<Result<byte[]>> GetInvFileFromUrl(ReadInvFileDto request)
        {
            Result<byte[]> result = new Result<byte[]>();
            byte[] fileBytes = null;
            request.url = request.url.Trim().Replace(" ", "");
            HttpClient httpClient = _HttpClient.CreateClient();
            HttpResponseMessage responseMessage = await httpClient.GetAsync(request.url);
            if (responseMessage.IsSuccessStatusCode)
            {
                fileBytes = await responseMessage.Content.ReadAsByteArrayAsync();
            }
            result.data = fileBytes;
            return result;
        }
        public async Task<HttpResponseMessage> GetInvFileFromUrlResponse(Request<ReadInvFileDto> request)
        {
            HttpClient httpClient = _HttpClient.CreateClient();
            return await httpClient.GetAsync(request.data.url);
        }
        //发票分享
        public async Task<Result<string>> ShareInvoice(ShareInvoiceDto request)
        {
            Result<string> result = new Result<string>();
            BDInvoiceFolder bDInvoiceFolder = await _BDInvoiceFolderRepository.GetInvInfoById(request.Id);
            Employee employee = (await _EmployeeRepository.WithDetailsAsync()).FirstOrDefault(w => w.emplid == request.emplid);//被分享人
            Employee employee1 = (await _EmployeeRepository.WithDetailsAsync()).FirstOrDefault(w => w.emplid == bDInvoiceFolder.emplid);//分享人
            if (employee == null)
            {
                result.status = 2;
                result.message = L["Invoice-ShareFail"] + "：" + L["ShareInvoice-EmplidNotExist"];
                return result;
            }
            if (!String.IsNullOrEmpty(employee.tdate))
            {
                result.status = 2;
                result.message = L["Invoice-ShareFail"] + "：" + L["ShareInvoice-EmplidDimission"];
                return result;
            }
            if (employee.company != employee1.company)
            {
                if (await (await _CompanyRepository.WithDetailsAsync()).Where(i => i.CompanyCategory == employee.company || i.CompanyCategory == employee1.company).Select(i => i.CompanySap).Distinct().CountAsync() > 1)
                {
                    result.status = 2;
                    result.message = L["Invoice-ShareFail"] + "：" + L["ShareInvoice-SiteError"];
                    return result;
                }
            }
            if (bDInvoiceFolder == null)
            {
                result.status = 2;
                result.message = L["Invoice-ShareFail"] + "：" + L["BDInvoiceFolder-NotFound"];
                return result;
            }
            if (bDInvoiceFolder.paytype == "已請款")
            {
                result.status = 2;
                result.message = L["Invoice-ShareFail"] + "：" + L["ShareInvoice-Requested"];
                return result;
            }
            bDInvoiceFolder.emplid = request.emplid;
            await _BDInvoiceFolderRepository.UpdateAsync(bDInvoiceFolder);
            result.message = L["Invoice-ShareSuccess"];
            return result;
        }
        //下载发票清单Excel（根據報銷單號獲取）
        public async Task<Result<byte[]>> GenerateInvListExcel(string rno)
        {
            Result<byte[]> result = new Result<byte[]>();
            byte[] buffer = null;
            var detailList = (await _CashDetailRepository.WithDetailsAsync()).Where(w => w.rno == rno).ToList();
            var invList = (await _invoiceRepository.WithDetailsAsync()).Where(w => w.rno == rno).ToList();
            var detailGroupList = detailList.GroupBy(w => w.seq).ToList();
            var invGroupList = invList.GroupBy(w => w.seq).ToList();
            var joinList = (from d in detailGroupList
                            join i in invGroupList
                            on d.Key equals i.Key
                            select new
                            {
                                expcode = d.Select(w => w.expcode).FirstOrDefault(),
                                expname = d.Select(w => w.expname).FirstOrDefault(),
                                invoices = i.GroupBy(w => w.item).ToList()
                            }).ToList();
            List<InvListDto> invListDtos = new List<InvListDto>();
            if (detailList.FirstOrDefault().formcode != "CASH_4")
            {
                foreach (var item in joinList)
                {
                    foreach (var item1 in item.invoices)
                    {
                        InvListDto invListDto = new InvListDto();
                        invListDto.expcode = item.expcode;
                        invListDto.expname = item.expname;
                        invListDto.invcode = item1.Select(w => w.invcode).FirstOrDefault();
                        invListDto.invno = item1.Select(w => w.invno).FirstOrDefault();
                        invListDto.invdate = (!item1.Select(w => w.invdate).FirstOrDefault().HasValue) ? "" : Convert.ToDateTime(item1.Select(w => w.invdate).FirstOrDefault()).ToString("yyyy/MM/dd");
                        invListDto.amount = item1.Select(w => w.amount).FirstOrDefault();
                        invListDto.oamount = item1.Select(w => w.oamount).FirstOrDefault();
                        invListDto.taxamount = item1.Select(w => w.taxamount).FirstOrDefault();
                        invListDto.invtype = item1.Select(w => w.invtype).FirstOrDefault();
                        invListDto.curr = item1.Select(w => w.curr).FirstOrDefault();
                        invListDtos.Add(invListDto);
                    }
                }
            }
            else
            {
                foreach (var item in invList)
                {
                    InvListDto invListDto = new InvListDto();
                    invListDto.expcode = detailList.First()?.expcode;
                    invListDto.expname = detailList.First()?.expname;
                    invListDto.invcode = item.invcode;
                    invListDto.invno = item.invno;
                    invListDto.invdate = !item.invdate.HasValue ? "" : Convert.ToDateTime(item.invdate).ToString("yyyy/MM/dd");
                    invListDto.amount = item.amount;
                    invListDto.oamount = item.oamount;
                    invListDto.taxamount = item.taxamount;
                    invListDto.invtype = item.invtype;
                    invListDto.curr = item.curr;
                    invListDtos.Add(invListDto);
                }
            }
            string headerName = L["DownloadInvList-HeaderName"];
            //創建Excel工作簿
            XSSFWorkbook workbook = new XSSFWorkbook();
            XSSFSheet excelSheet = (XSSFSheet)workbook.CreateSheet("InvoiceList");
            //第一行 標題行
            IRow headrow = excelSheet.CreateRow(0);
            ICell headcell = headrow.CreateCell(0);
            headcell.SetCellValue(L["DownloadInvList-HeaderName"]);
            CellRangeAddress headerAddress = new CellRangeAddress(0, 0, 0, 8);
            excelSheet.AddMergedRegion(headerAddress);
            headcell.CellStyle = GetheadStyle_1(workbook);//设置headerrow1样式
            //第二行 報銷單號行
            IRow rnoRow = excelSheet.CreateRow(1);
            ICell rnoCell = rnoRow.CreateCell(0);
            rnoCell.SetCellValue(L["DownloadInvList-Rno"] + rno);
            CellRangeAddress rnoAddress = new CellRangeAddress(1, 1, 0, 8);
            excelSheet.AddMergedRegion(rnoAddress);
            rnoCell.CellStyle = GetheadStyle_1(workbook);
            //第三行 空行
            IRow emptyRow = excelSheet.CreateRow(2);
            ICell emptyCell = emptyRow.CreateCell(0);
            CellRangeAddress emptyAddress = new CellRangeAddress(2, 2, 0, 8);
            excelSheet.AddMergedRegion(emptyAddress);
            //第四行 標題行
            IRow titleRow = excelSheet.CreateRow(3);
            string[] titles =
            {
                L["DownloadInvList-Scene"],
                L["DownloadInvList-Seq"],
                L["DownloadInvList-InvCode"],
                L["DownloadInvList-InvNo"],
                L["DownloadInvList-InvDate"],
                L["DownloadInvList-Amount"],
                L["DownloadInvList-TaxAmount"],
                L["DownloadInvList-TaxIncludedAmount"],
                L["DownloadInvList-InvType"]
            };
            for (int i = 0; i < titles.Count(); i++)
            {
                ICell titleCell = titleRow.CreateCell(i);
                titleCell.SetCellValue(titles[i]);
                titleCell.CellStyle = SetColumnStyle(workbook);
            }
            //具體內容填充
            for (int i = 0; i < invListDtos.Count; i++)
            {
                IRow row = excelSheet.CreateRow(4 + i);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(invListDtos[i].expname);
                cell.CellStyle = SetColumnStyle(workbook);
                ICell seqCell = row.CreateCell(1);
                seqCell.SetCellValue(i + 1);
                seqCell.CellStyle = SetColumnStyle(workbook);
                ICell invCodeCell = row.CreateCell(2, CellType.String);
                invCodeCell.SetCellValue(invListDtos[i].invcode);
                invCodeCell.CellStyle = SetColumnStyle(workbook);
                ICell invNoCell = row.CreateCell(3, CellType.String);
                invNoCell.SetCellValue(invListDtos[i].invno);
                invNoCell.CellStyle = SetColumnStyle(workbook);
                ICell invDateCell = row.CreateCell(4);
                invDateCell.SetCellValue(invListDtos[i].invdate);
                invDateCell.CellStyle = SetColumnStyle(workbook);
                ICell amountCell = row.CreateCell(5, CellType.Numeric);
                amountCell.CellStyle = SetNumberColumnStyle(workbook);
                amountCell.SetCellValue(invListDtos[i].amount.HasValue ? Convert.ToDouble(invListDtos[i].amount) : 0);
                ICell taxAmountCell = row.CreateCell(6, CellType.Numeric);
                taxAmountCell.CellStyle = SetNumberColumnStyle(workbook);
                taxAmountCell.SetCellValue(invListDtos[i].taxamount.HasValue ? Convert.ToDouble(invListDtos[i].taxamount) : 0);
                ICell oAmountCell = row.CreateCell(7, CellType.Numeric);
                oAmountCell.CellStyle = SetNumberColumnStyle(workbook);
                oAmountCell.SetCellValue(invListDtos[i].oamount.HasValue ? Convert.ToDouble(invListDtos[i].oamount) : 0);
                ICell invTypeCell = row.CreateCell(8);
                invTypeCell.SetCellValue(invListDtos[i].invtype);
                invTypeCell.CellStyle = SetColumnStyle(workbook);
            }
            //发票总数
            int invCount = invListDtos.Count;
            //发票总金额
            decimal? invAmount = invListDtos.Sum(w => w.oamount);
            IRow invCountRow = excelSheet.CreateRow(excelSheet.LastRowNum + 2);
            ICell invCountTitle = invCountRow.CreateCell(7);
            invCountTitle.SetCellValue(L["DownloadInvList-InvCount"]);
            ICell invCountCell = invCountRow.CreateCell(8);
            invCountCell.SetCellValue(invCount);
            invCountCell.CellStyle = SetColumnStyle(workbook);
            IRow invAmountRow = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
            ICell invAmountTitle = invAmountRow.CreateCell(7);
            invAmountTitle.SetCellValue(L["DownloadInvList-InvTotalAmt"]);
            ICell invAmountCell = invAmountRow.CreateCell(8, CellType.Numeric);
            invAmountCell.CellStyle = SetNumberColumnStyle(workbook);
            invAmountCell.SetCellValue(invAmount.HasValue ? Double.Parse(invAmount.ToString()) : 0);
            excelSheet.SetColumnWidth(0, 10 * 512);
            excelSheet.SetColumnWidth(2, 15 * 512);
            excelSheet.SetColumnWidth(3, 15 * 512);
            excelSheet.SetColumnWidth(4, 15 * 256);
            excelSheet.SetColumnWidth(5, 15 * 256);
            excelSheet.SetColumnWidth(6, 15 * 256);
            excelSheet.SetColumnWidth(7, 15 * 256);
            excelSheet.SetColumnWidth(8, 15 * 256);
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                buffer = ms.ToArray();
            }
            result.data = buffer;
            return result;
        }

        /// <summary>
        /// 根据发票号码/代码查询
        /// </summary>
        /// <param name="invno"></param>
        /// <param name="invcode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<List<InvoiceDto>>> QueryInvoiceByNo(string invno, string invcode, string invoiceArea, string token)
        {
            Result<List<InvoiceDto>> result = new Result<List<InvoiceDto>>()
            {
                data = new List<InvoiceDto>()
            };
            Result<PaInvoice> invoice = null;
            InvoiceDto invoiceDto = new InvoiceDto();

            if (invoiceArea.Equals(ERSConsts.RegionEnum.CN.ToString()))
            {
                invoice = await QueryPAInvInfo(invcode, invno, token);
            }

            if (invoice != null && invoice.data != null)
            {
                var existList = (await _BDInvoiceFolderRepository.WithDetailsAsync()).Where(w => w.invno == invoice.data.Invno).Select(g => new { g.invno, g.invcode, g.emplid }).AsNoTracking().ToList();
                //invoiceDto.existautopa = true;
                invoiceDto.invno = invoice.data.Invno;
                invoiceDto.invcode = invoice.data.Invcode;
                invoiceDto.amount = Convert.ToDecimal(invoice.data.Amount);
                invoiceDto.taxamount = Convert.ToDecimal(invoice.data.Taxamount);
                invoiceDto.oamount = Convert.ToDecimal(invoice.data.Tlprice);//价税合计
                invoiceDto.expdesc = invoice.data.expcodeDesc;
                invoiceDto.expcode = invoice.data.Expcode;
                invoiceDto.batchno = invoice.data.Batchno;
                invoiceDto.salestaxno = invoice.data.Salestaxno;
                invoiceDto.salesname = invoice.data.Salesname;
                invoiceDto.buyertaxno = invoice.data.Buyertaxno;
                invoiceDto.buyername = invoice.data.Buyername;
                //invoiceDto.tlprice = invoice.data.Tlprice;
                invoiceDto.verifycode = invoice.data.Verifycode;
                invoiceDto.salesaddress = invoice.data.Salesaddress;
                invoiceDto.salesbank = invoice.data.Salesbank;
                invoiceDto.buyeraddress = invoice.data.Buyeraddress;
                invoiceDto.buyerbank = invoice.data.Buyerbank;
                //invoiceDto.pwarea = invoice.data.Pwarea;
                invoiceDto.remark = invoice.data.Remark;
                invoiceDto.drawer = invoice.data.Drawer;
                invoiceDto.payee = invoice.data.Payee;
                invoiceDto.reviewer = invoice.data.Reviewer;
                invoiceDto.machinecode = invoice.data.Machinecode;
                invoiceDto.machineno = invoice.data.Machineno;
                invoiceDto.verifystat = invoice.data.Verifystat;
                invoiceDto.paymentStatDesc = invoice.data.paymentStatDesc;
                invoiceDto.taxrate = invoice.data.taxrete;
                invoiceDto.invdate = Convert.ToDateTime(invoice.data.Invdate);
                //invoiceDto.uploadmethod = ERSConsts.Invoice_UploadMethod_3;//上傳方式：上傳檔案
                invoiceDto.source = ERSConsts.InvoiceSourceEnum.InvoicePool.ToValue();
                // N可请款，F为已请款
                invoiceDto.invstat = invoice.data.Paymentstat == "P01" ? ERSConsts.Value_N : ERSConsts.Value_F;
                invoiceDto.verifystat = invoice.data.Verifystat;
                invoiceDto.verifyStateDesc = invoice.data.VerifyStateDesc;
                invoiceDto.invdesc = invoice.data.Invdesc;
                invoiceDto.curr = ERSConsts.CurrencyEnum.RMB.ToString();
                invoiceDto.expinfo = invoice.data.expcodeDesc;
                invoiceDto.paymentStat = true;
                if (invoice.data.Verifystat == "V01")
                {
                    invoiceDto.invstat = "Lock";
                    if (String.IsNullOrEmpty(invoice.data.Expcode))
                    {
                        invoiceDto.expcode = "Lock";
                    }
                }
                if (!String.IsNullOrEmpty(invoiceDto.expdesc))
                {
                    if (invoiceDto.expdesc == L["004"] || invoiceDto.expdesc == L["005"])
                    {
                        invoiceDto.paymentStat = false;
                    }
                }
                if (invoiceDto.expcode != "Lock") invoiceDto.expcode = invoiceDto.expcode;
                //如果autopa传过来的paymentstat为p03，则无法请款
                if (invoice.data.Paymentstat == "P03")
                {
                    invoiceDto.paymentStat = false;
                }
                //红冲发票无法请款
                if (invoice.data.Expcode.Contains("EXP021") || invoice.data.Expcode.Contains("EXP005"))
                {
                    invoiceDto.paymentStat = false;
                    invoiceDto.expinfo = invoice.data.expcodeDesc;
                }
                if (invoiceDto.invstat == "F")
                {
                    invoiceDto.paymentStat = false;
                    invoiceDto.expinfo = L["UploadInvoice-Requested"];
                }
                //已被上傳過得發票
                if (existList.Count > 0)
                {
                    invoiceDto.paymentStat = false;
                    invoiceDto.expinfo = L["UploadInvoice-UploadFail"] + L["UploadInvoice-ExistFolder"] + existList.FirstOrDefault().emplid;
                }

                if (!string.IsNullOrEmpty(invoice.data.Invtype))
                {
                    BDInvoiceType bdInvoiceType = (await _IBDInvoiceTypeRepository.WithDetailsAsync()).FirstOrDefault(w => w.InvTypeCode == invoice.data.Invtype);
                    if (bdInvoiceType != null)
                    {
                        invoiceDto.invtype = bdInvoiceType.InvType;
                        invoiceDto.invtypecode = bdInvoiceType.InvTypeCode;
                        //前端会用到发票类型判断该显示发票哪些栏位
                        invoiceDto.invoicecategory = bdInvoiceType.Category;
                    }
                }

                result.data.Add(invoiceDto);
            }
            else
            {
                //输入的参数在发票池找不到数据，需要填补
                //invoiceDto.isfill = true;
                //invoiceDto.existautopa = false;
                result.message = L["ReadInvoice-PaNoData"];
                //result.data.Add(invoiceDto);
            }
            return result;
        }
        private async Task<Result<PaInvoice>> QueryPAInvInfo(string invcode, string invno, string token = "")
        {
            Result<PaInvoice> PaInvoice = new Result<PaInvoice>();
            string url = this._configuration.GetSection("AutoPA:Invoice").Value;
            if (!String.IsNullOrEmpty(invcode))
            {
                url = url + "?invcode=" + invcode + "&invno=" + invno;
            }
            else
            {
                url = url + "?invno=" + invno;
            }
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
        /// <summary>
        /// 处理深圳市特殊发票（区块链电子发票）
        /// </summary>
        /// <param name="url">扫描二维码识别到的url</param>
        /// <returns></returns>
        public async Task<InvoiceDto> GetBcfpInvInfo(string url)
        {
            InvoiceDto result = new();
            string getInfoUrl = "https://bcfp.shenzhen.chinatax.gov.cn/dzswj/bers_ep_web/query_bill_detail";
            string hash = "", bill_num = "", total_amount = "";
            Regex hashRegex = new Regex(@"hash=([^&]*)");
            Regex billRegex = new Regex(@"bill_num=([^&]*)");
            Regex amountRegex = new Regex(@"total_amount=([^&]*)");
            Match hashMatch = hashRegex.Match(url);
            Match billMatch = billRegex.Match(url);
            Match amountMatch = amountRegex.Match(url);
            if (hashMatch.Success)
                hash = hashMatch.Groups[1].Value;
            if (billMatch.Success)
                bill_num = billMatch.Groups[1].Value;
            if (amountMatch.Success)
                total_amount = amountMatch.Groups[1].Value;
            BcfpInvDto bcfpInvDto = new()
            {
                tx_hash = hash,
                total_amount = total_amount,
                bill_num = bill_num
            };
            HttpClient httpClient = _HttpClient.CreateClient();
            string postData = System.Text.Json.JsonSerializer.Serialize(bcfpInvDto);
            StringContent content = new StringContent(postData, Encoding.UTF8, "application/json");
            // 发起POST请求，并获取响应数据
            HttpResponseMessage response = await httpClient.PostAsync(getInfoUrl, content);
            response.EnsureSuccessStatusCode();
            string responseStr = await response.Content.ReadAsStringAsync();
            if (!String.IsNullOrEmpty(responseStr))
            {
                BcfpInvResultDto bcfpInvResultDto = JsonConvert.DeserializeObject<BcfpInvResultDto>(responseStr);
                result.invcode = bcfpInvResultDto.bill_record.bill_code;
                result.invno = bcfpInvResultDto.bill_record.bill_num;
            }
            return result;
        }
        //设置header样式
        protected ICellStyle GetheadStyle_1(XSSFWorkbook xssfWorkbook)
        {
            ICellStyle style = xssfWorkbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            return style;
        }
        //設置單元格居中樣式？
        protected ICellStyle SetColumnStyle(XSSFWorkbook xssfWorkbook)
        {
            ICellStyle style = xssfWorkbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            return style;
        }
        protected ICellStyle SetNumberColumnStyle(XSSFWorkbook xssfWorkbook)
        {
            ICellStyle style = xssfWorkbook.CreateCellStyle();
            IDataFormat dataFormat = xssfWorkbook.CreateDataFormat();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.DataFormat = dataFormat.GetFormat("#,##0.00");
            return style;
        }

        /// <summary>
        /// 添加發票
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<string>> AddInvoices(IFormCollection formCollection, string userId)
        {
            string area = await _MinioDomainService.GetMinioArea(userId);
            Result<List<Result<string>>> result = new();
            string invoices = formCollection["invoices"];
            List<InvoiceDto> invData = System.Text.Json.JsonSerializer.Deserialize<List<InvoiceDto>>(invoices);

            List<string> invcodes = invData.Where(w => !String.IsNullOrEmpty(w.invno)).Select(s => s.invno).Distinct().ToList();
            var existList = (await _BDInvoiceFolderRepository.WithDetailsAsync()).Where(w => invcodes.Contains(w.invno)).Select(g => new { g.invno, g.invcode, g.emplid }).AsNoTracking().ToList();//1
            List<BDInvoiceFolder> bDInvoiceFolders = new List<BDInvoiceFolder>();
            List<string> fileFlagList = formCollection.Files.Select(w => w.Name).ToList();

            var companyCategorys = (await _IBDCompanyCategoryRepository.WithDetailsAsync()).
                     OrderBy(e => e.CompanyDesc)
                    .ToList();

            foreach (var item in invData)
            {
                if (String.IsNullOrEmpty(item.invtype) || String.IsNullOrEmpty(item.curr))
                {
                    return new Result<string> { data = item.flag, message = L["UploadInvoice-UploadFail"] + L["UploadInvoice-IncompleteInfo"], status = 2 };
                }
                if (existList.Where(s => s.invcode == (String.IsNullOrEmpty(item.invcode) ? "" : item.invcode) && s.invno == item.invno).Count() > 0)
                {
                    var existInvoice = existList.Where(s => s.invcode == (String.IsNullOrEmpty(item.invcode) ? "" : item.invcode) && s.invno == item.invno).FirstOrDefault();
                    return new Result<string> { data = item.flag, message = L["UploadInvoice-UploadFail"] + L["UploadInvoice-ExistFolder"] + existInvoice.emplid, status = 2 };
                }
                if (item.invstat == "F")
                {
                    return new Result<string> { data = item.flag, message = L["UploadInvoice-UploadFail"] + L["UploadInvoice-Requested"], status = 2 };
                }
                if (item.expcode != null)
                {
                    if (item.expcode.Contains("EXP004"))
                    {
                        return new Result<string> { data = item.flag, message = L["UploadInvoice-UploadFail"] + L["UploadInvoice-InvoiceInvalid"], status = 2 };
                    }
                    if (item.expcode.Contains("EXP005") || item.expcode.Contains("EXP021"))
                    {
                        return new Result<string> { data = item.flag, message = L["UploadInvoice-UploadFail"] + L["UploadInvoice-RedInvoice"], status = 2 };
                    }
                }

                BDInvoiceFolder bDInvoiceFolder = _ObjectMapper.Map<InvoiceDto, BDInvoiceFolder>(item);
                bDInvoiceFolder.buyername = bDInvoiceFolder.buyername;
                bDInvoiceFolder.invdate = bDInvoiceFolder.invdate.HasValue ? Convert.ToDateTime(item.invdate).ToLocalTime() : null;
                bDInvoiceFolder.amount = item.oamount;
                bDInvoiceFolder.untaxamount = item.amount;
                bDInvoiceFolder.taxamount = item.taxamount;
                bDInvoiceFolder.paytype = ERSConsts.InvoicePayTypeEnum.Unrequested.ToValue();
                bDInvoiceFolder.emplid = userId;
                bDInvoiceFolder.company = ERSConsts.Company_All;
                bDInvoiceFolder.source = item.source;
                bDInvoiceFolder.ocrid = item.ocrid;
                bDInvoiceFolder.identificationno = item.identificationno;
                bDInvoiceFolder.invoicetitle = item.invoicetitle;
                if (item.taxbase != null)
                {
                    bDInvoiceFolder.taxbase = (decimal)item.taxbase;
                }
                if (item.importtaxamount != null)
                {
                    bDInvoiceFolder.importtaxamount = (decimal)item.importtaxamount;
                }
                if (item.servicefee != null)
                {
                    bDInvoiceFolder.servicefee = (decimal)item.servicefee;
                }
                if (item.shippingfee != null)
                {
                    bDInvoiceFolder.shippingfee = (decimal)item.shippingfee;
                }
                if (item.transactionfee != null)
                {
                    bDInvoiceFolder.transactionfee = (decimal)item.transactionfee;
                }
                if (item.quantity != null)
                {
                    bDInvoiceFolder.quantity = (decimal)item.quantity;
                }
                bDInvoiceFolder.productinfo = item.productinfo;
                bDInvoiceFolder.startstation = item.startstation;
                bDInvoiceFolder.endstation = item.endstation;
                bDInvoiceFolder.endstation = item.endstation;
                bDInvoiceFolder.remarks = item.remarks;
                bDInvoiceFolder.abnormalreason = item.abnormalreason;
                bDInvoiceFolder.responsibleparty = item.responsibleparty;
                bDInvoiceFolder.taxtype = item.taxtype;

                bDInvoiceFolder.invtype = item.invtype;
                //如果OCR辨识不到，原始的invtypecode为null，User选择发票类型后，要根据所选类型重新压值回invtypecode
                if (string.IsNullOrEmpty(item.invtypecode))
                {
                    BDInvoiceType bdInvoiceType = (await _IBDInvoiceTypeRepository.WithDetailsAsync()).FirstOrDefault(w => w.InvType == item.invtype);
                    if (bdInvoiceType != null)
                    {
                        bDInvoiceFolder.invtypecode = bdInvoiceType.InvTypeCode;
                    }
                }

                var matchingCompany = companyCategorys.FirstOrDefault(o => o.IdentificationNo == item.buyertaxno);
                if (matchingCompany != null)
                {
                    bDInvoiceFolder.buyername = matchingCompany.CompanyDesc;
                }

                if (String.IsNullOrEmpty(bDInvoiceFolder.invcode))
                {
                    bDInvoiceFolder.invcode = "";
                }
                foreach (var item1 in formCollection.Files)
                {
                    string invno = item.invno;
                    if (String.IsNullOrEmpty(invno))
                    {
                        invno = "other";
                    }
                    string path = "";
                    string type = "";
                    if (!string.IsNullOrEmpty(_configuration.GetSection("UnitTest")?.Value))
                        type = "image/jpeg";
                    else
                        type = item1.ContentType;
                    using (var steam = item1.OpenReadStream())
                    {
                        if (!String.IsNullOrEmpty(invno))
                        {
                            if (item1.ContentType == "image/jpeg")
                            {
                                path = await _MinioDomainService.PutInvoiceAsync(invno, Path.GetExtension(item1.FileName).Replace(".", "").ToLower() == "jpg" ? item1.FileName : (item1.FileName + ".jpg"), steam, type, area);
                            }
                            else if (item1.ContentType == "image/png")
                            {
                                path = await _MinioDomainService.PutInvoiceAsync(invno, Path.GetExtension(item1.FileName).Replace(".", "").ToLower() == "png" ? item1.FileName : (item1.FileName + ".png"), steam, type, area);
                            }
                            else if (item1.ContentType == "application/pdf")
                            {
                                path = await _MinioDomainService.PutInvoiceAsync(invno, Path.GetExtension(item1.FileName).Replace(".", "").ToLower() == "pdf" ? item1.FileName : (item1.FileName + invno + ".pdf"), steam, type, area);
                            }
                        }
                    }
                    bDInvoiceFolder.filepath = path;
                    bDInvoiceFolder.category = type;
                }
                if (!String.IsNullOrEmpty(item.invno))
                {
                    if (bDInvoiceFolders.Select(w => w.invno).Contains(item.invno))
                    {
                        return new Result<string> { data = item.flag, message = L["UploadInvoice-UploadFail"] + L["UploadInvoice-ExistUploadList"], status = 2 };
                    }
                }
                bDInvoiceFolders.Add(bDInvoiceFolder);
            }

            await _BDInvoiceFolderRepository.InsertManyAsync(bDInvoiceFolders);
            return new Result<string> { data = L["AddSuccess"], message = L["UploadInvoice-UploadSuccess"] }; ;
        }

        /// <summary>
        /// 發票辨識獲取發票信息
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<List<InvoiceDto>>> RecognizeInvoice([FromForm] IFormCollection formCollection, string userId, string token)
        {
            /* AutoPA:從大陸稅務局下載的發票信息
            * 總部：從SAP串接發票信息-TBD
            * 發票池里的發票屬於稅局發票，稅局發票帶出來的欄位是固定的；OCR辨析的發票屬於非稅局發票，非稅局發票需要根據不同發票類型帶出不同欄位
            * 發票識別：從發票池讀取，如果讀不到，則調用OCR API進行解析，所以最終還是要靠OCR API來解析發票；一次只能識別一張發票
            */

            Result<List<InvoiceDto>> result = new Result<List<InvoiceDto>>();

            var files = formCollection.Files;
            if (files == null || formCollection.Files.Count.Equals(0))
            {
                //上传的文件为空时报错
                result.message = "The parameter cannot be null.";
                result.status = 2;
                return result;
            }

            var file = formCollection.Files[0];
            //根据登入者获取所属的区域
            var companyCategoryListResult = await _IBDCompanyCategoryDomainService.GetCategoryListByUserId(userId);
            if (companyCategoryListResult.data != null)
            {
                //添加发票可以选不同区域；但发票minio储存就要跟开票人区域绑定，大陆发票只能在大陆地区落地，不可存到境外(法规要求)
                DIInfo dIInfo = new DIInfo();
                dIInfo.InvoiceRegion = !string.IsNullOrEmpty(formCollection["invoiceArea"]) ? formCollection["invoiceArea"].ToString() : companyCategoryListResult.data[0].Area;
                dIInfo.Currency = companyCategoryListResult.data[0].BaseCurrency;

                //大陆區域發票才會存在AutoPA發票池
                if (dIInfo.InvoiceRegion.ToUpper().Equals(ERSConsts.RegionEnum.CN.ToString()))
                {
                    //判断发票是否存在发票池
                    result = await ReadInvoiceInfoByFile(formCollection, userId, token);
                    //result.status = 1; //待拿掉
                    //result.data = new List<InvoiceDto>(); //待拿掉
                    if (result.data.Count > 0)
                    {
                        InvoiceDto invoiceDto = result.data[0];
                        if (string.IsNullOrEmpty(invoiceDto.msg))
                        {
                            result.message = "success";
                            result.status = 1;
                            result.data = new List<InvoiceDto> { invoiceDto };
                        }
                        else
                        {
                            //如果发票不存在发票池，则调用OCR解析发票
                            var res = await _iDIService.AnalysisFile(file, userId, dIInfo);
                            //轉換成Json格式,加入是否存在發票夾Flag
                            InvoiceDto parsedInvoice = await MapToInvoiceDto(res, dIInfo.Currency, userId);
                            result.message = "success";
                            result.status = 1;
                            result.data = new List<InvoiceDto> { parsedInvoice };
                        }
                    }
                }
                else
                {
                    //來源非发票池，则调用OCR解析发票
                    var res = await _iDIService.AnalysisFile(file, userId, dIInfo);
                    //轉換成Json格式,加入是否存在發票夾Flag
                    //return Content(jsonObject.ToString(), "application/json");
                    InvoiceDto parsedInvoice = await MapToInvoiceDto(res, dIInfo.Currency, userId);
                    result.message = "success";
                    result.status = 1;
                    result.data = new List<InvoiceDto> { parsedInvoice };
                }
            }

            return result;
        }

        /// <summary>
        /// 根據不同發票類型OCR辨識返回相應信息，匹配到InvoiceDto實體
        /// </summary>
        /// <param name="res"></param>
        /// <param name="currency"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<InvoiceDto> MapToInvoiceDto(string res, string currency, string userId)
        {
            //将 res 转换为 JSON 对象
            var jsonObject = JObject.Parse(res);

            //创建 InvoiceDto 实例
            InvoiceDto invoiceDto = new InvoiceDto();
            invoiceDto.curr = currency; //按发票上传者所在区域带出币别
            // N可请款，F为已请款
            invoiceDto.invstat = ERSConsts.Value_N;
            invoiceDto.paymentStat = true;
            invoiceDto.source = ERSConsts.InvoiceSourceEnum.OCR.ToValue();

            /* 辨析结果json格式：
                "code": 200,
                "status": "OK",
                "message": "success",
                "body": [
                {
                    "com_model_id": "cn_i_com_250326",
                    "model_id": "cn_i_taxi_250326",
                    "file_id": "20250512145200821_6BSCGTUO",
                    "analysis_result": 
                    [
                        {
                            "key": "invoice_title",
                            "value": "广东通用 打发票",
                            "confidence": 0.6269999742507935
                        },
                        {
                            "key": "invoice_code",
                            "value": "144002101520",
                            "confidence": 0.8560000061988831
                        }
                    ]
                 },
            */

            if (jsonObject["body"] != null)
            {
                //获取body节点
                if (jsonObject["body"] is JArray bodyArray && bodyArray.Count > 0)
                {
                    JObject firstBodyElement = bodyArray[0] as JObject;
                    if (firstBodyElement != null)
                    {
                        //检查节点是否存在并获取值
                        if (firstBodyElement.TryGetValue("model_id", out JToken modelIdToken))
                        {
                            //OCR模型module name命名以发票类型代码比如INV0001为前缀，辨析后会返回发票代码信息，然后根据发票代码串出发票模型
                            string modelId = modelIdToken.ToString();
                            invoiceDto.modelid = modelId;
                            string invTypeCode = modelId.Split('_')[0];
                            if (!string.IsNullOrEmpty(invTypeCode))
                            {
                                BDInvoiceType bdInvoiceType = (await _IBDInvoiceTypeRepository.WithDetailsAsync()).FirstOrDefault(w => w.InvTypeCode == invTypeCode);
                                if (bdInvoiceType != null)
                                {
                                    invoiceDto.invtype = bdInvoiceType.InvType;
                                    invoiceDto.invtypecode = bdInvoiceType.InvTypeCode;
                                    invoiceDto.salestaxno = bdInvoiceType.SellerTaxId;
                                    invoiceDto.invoicecategory = bdInvoiceType.Category;
                                }
                            }
                        }

                        if (firstBodyElement["analysis_result"] is JArray resultArray && resultArray.Count > 0)
                        {
                            foreach (JObject resultElement in resultArray)
                            {
                                if (resultElement.TryGetValue("key", out JToken keyToken) && resultElement.TryGetValue("value", out JToken valueToken))
                                {
                                    string key = keyToken.ToString();
                                    string value = valueToken.ToString();

                                    switch (key.ToLower())
                                    {
                                        case "invoice_code":
                                            invoiceDto.invcode = value;
                                            break;
                                        case "invoice_id":
                                            invoiceDto.invno = value;
                                            break;
                                        case "invoice_date":
                                            invoiceDto.invdate = !string.IsNullOrEmpty(value) ? DateTime.Parse(value) : null;
                                            break;
                                        case "sub_amount":
                                            invoiceDto.amount = !string.IsNullOrEmpty(value) ? decimal.Parse(value) : 0;
                                            break;
                                        case "tax_amount":
                                            invoiceDto.taxamount = !string.IsNullOrEmpty(value) ? decimal.Parse(value) : 0;
                                            break;
                                        case "amount":
                                            invoiceDto.oamount = !string.IsNullOrEmpty(value) ? decimal.Parse(value) : 0;
                                            break;
                                        case "seller_name":
                                            invoiceDto.salesname = value;
                                            break;
                                        case "seller_tax_id":
                                            invoiceDto.salestaxno = value;
                                            break;
                                        case "buyer_name":
                                            invoiceDto.buyername = value;
                                            break;
                                        case "buyer_tax_id":
                                            invoiceDto.buyertaxno = value;
                                            break;
                                        case "start_station":
                                            invoiceDto.startstation = value;
                                            break;
                                        case "end_station":
                                            invoiceDto.endstation = value;
                                            break;
                                        case "ticket_number":
                                            invoiceDto.identificationno = value;
                                            break;
                                        case "invoice_title":
                                            invoiceDto.invoicetitle = value;
                                            break;
                                        case "taxbase":
                                            invoiceDto.taxbase = !string.IsNullOrEmpty(value) ? decimal.Parse(value) : null;
                                            break;
                                        case "import_tax_amount":
                                            invoiceDto.importtaxamount = !string.IsNullOrEmpty(value) ? decimal.Parse(value) : null;
                                            break;
                                        case "service_fee":
                                            invoiceDto.servicefee = !string.IsNullOrEmpty(value) ? decimal.Parse(value) : null;
                                            break;
                                        case "shipping_fee":
                                            invoiceDto.shippingfee = !string.IsNullOrEmpty(value) ? decimal.Parse(value) : null;
                                            break;
                                        case "transaction_fee":
                                            invoiceDto.transactionfee = !string.IsNullOrEmpty(value) ? decimal.Parse(value) : null;
                                            break;
                                        case "quantity":
                                            invoiceDto.quantity = !string.IsNullOrEmpty(value) ? decimal.Parse(value) : null;
                                            break;
                                        case "product_info":
                                            invoiceDto.productinfo = value;
                                            break;
                                        default:
                                            // Handle other keys if necessary  
                                            break;
                                    }
                                }
                            }
                        }

                        //OCR辨識記錄要保存到OCRResult
                        OCRResults ocrResult = new OCRResults
                        {
                            InvoiceNo = invoiceDto.invno,
                            InvoiceCode = invoiceDto.invcode,
                            InvoiceDate = invoiceDto.invdate,
                            InvoiceType = invoiceDto.invtype,
                            InvoiceTypeCode = invoiceDto.invtypecode,
                            UnTaxAmount = invoiceDto.amount,
                            TaxAmount = invoiceDto.taxamount,
                            Amount = invoiceDto.oamount,
                            Currency = invoiceDto.curr,
                            BuyerTaxId = invoiceDto.buyertaxno,
                            BuyerName = invoiceDto.buyername,
                            SellerTaxId = invoiceDto.salestaxno,
                            SellerName = invoiceDto.salesname,
                            TaxRate = invoiceDto.taxrate,
                            TaxBase = invoiceDto.taxbase,
                            ImportTaxAmount = invoiceDto.importtaxamount,
                            ServiceFee = invoiceDto.servicefee,
                            ShippingFee = invoiceDto.shippingfee,
                            TransactionFee = invoiceDto.transactionfee,
                            Quantity = invoiceDto.quantity,
                            ProductInfo = invoiceDto.productinfo,
                            StartStation = invoiceDto.startstation,
                            EndStation = invoiceDto.endstation,
                            Remarks = invoiceDto.remarks ?? invoiceDto.remark,
                            IdentificationNo = invoiceDto.identificationno,
                            InvoiceTitle = invoiceDto.invoicetitle,
                        };

                        var inserted = await _IOcrResultRepository.InsertAsync(ocrResult);
                        invoiceDto.ocrid = inserted.Id.ToString(); //OCR識別記錄表ID
                    }
                }
            }

            return invoiceDto;
        }

        /// <summary>
        /// 获取OCR辨析结果记录
        /// </summary>
        /// <param name="ocrid"></param>
        /// <returns></returns>
        public async Task<Result<InvoiceDto>> getOcrResult(string ocrId)
        {
            Result<InvoiceDto> result = new Result<InvoiceDto>();

            // Input validation
            if (string.IsNullOrEmpty(ocrId))
            {
                result.message = "The parameter cannot be null.";
                result.status = 2;
                return result;
            }

            var ocrResults = (await _IOcrResultRepository.WithDetailsAsync())
                            .Where(o => o.Id.ToString() == (ocrId)).AsNoTracking().ToList();

            if (ocrResults.Count > 0)
            {
                string invCategory = null;
                OCRResults oCRResult = ocrResults[0];

                BDInvoiceType bdInvoiceType = (await _IBDInvoiceTypeRepository.WithDetailsAsync()).FirstOrDefault(w => w.InvTypeCode == oCRResult.InvoiceTypeCode);
                if (bdInvoiceType != null)
                {
                    invCategory = bdInvoiceType.Category;
                }

                InvoiceDto invoiceDto = new InvoiceDto
                {
                    Id = oCRResult.Id,
                    identificationno = oCRResult.IdentificationNo,
                    invno = oCRResult.InvoiceNo,
                    invcode = oCRResult.InvoiceCode,
                    invoicetitle = oCRResult.InvoiceTitle,
                    invdate = oCRResult.InvoiceDate,
                    invtype = oCRResult.InvoiceType,
                    invtypecode = oCRResult.InvoiceTypeCode,
                    amount = oCRResult.UnTaxAmount ?? 0,
                    taxamount = oCRResult.TaxAmount ?? 0,
                    oamount = oCRResult.Amount ?? 0,
                    curr = oCRResult.Currency,
                    invoicecategory = invCategory,

                    taxrate = oCRResult.TaxRate,
                    taxbase = oCRResult.TaxBase,
                    importtaxamount = oCRResult.ImportTaxAmount,
                    buyertaxno = oCRResult.BuyerTaxId,
                    buyername = oCRResult.BuyerName,
                    salestaxno = oCRResult.SellerTaxId,
                    salesname = oCRResult.SellerName,
                    servicefee = oCRResult.ServiceFee,
                    shippingfee = oCRResult.ShippingFee,
                    transactionfee = oCRResult.TransactionFee,
                    quantity = oCRResult.Quantity,
                    productinfo = oCRResult.ProductInfo,
                    startstation = oCRResult.StartStation,
                    endstation = oCRResult.EndStation,
                };
                result.data = invoiceDto;
            }
            else
            {
                result.data = null;
                result.message = L["docNotFound"];
                result.status = 2;
            }

            return result;

        }

        public async Task<Result<string>> UpdateInvoice(InvoiceDto invoiceDto)
        {
            var result = new Result<string>();
            Invoice invoice = (await _invoiceRepository.WithDetailsAsync()).Where(i => i.Id == invoiceDto.Id).FirstOrDefault();
            if (invoice != null)
            {
                invoice.invno = invoiceDto.invno;
                invoice.oamount = invoiceDto.oamount;
                invoice.taxamount = invoiceDto.taxamount;
                invoice.sellerTaxId = invoiceDto.sellerTaxId;
                await _invoiceRepository.UpdateAsync(invoice);
                result.status = 1;
            }
            else
            {
                result.status = 2;
            }

            return result;
        }
    }
}

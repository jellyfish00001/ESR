using Castle.Components.DictionaryAdapter.Xml;
using ERS.Domain.IDomainServices;
using ERS.DTO;
using ERS.DTO.Invoice;
using ERS.DTO.Payment;
using ERS.Entities;
using ERS.Entities.Payment;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.Minio;
using ExpenseApplication.Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
namespace ERS.DomainServices
{
    public class PaymentDomainService : CommonDomainService, IPaymentDomainService
    {
        private IAutoNoRepository _AutoNoRepository;
        private IMinioRepository _MinioRepository;
        private IEFormAlistRepository _EFormAlistRepository;
        private ICashAccountRepository _CashAccountRepository;
        private IEmployeeInfoRepository _EmployeeInfoRepository;
        private IEmpchsRepository _EmpchsRepository;
        private ICashPaymentDetailRepository _CashPaymentDetailRepository;
        private ICashHeadRepository _CashHeadRepository;
        private IObjectMapper _ObjectMapper;
        private IMinioDomainService _MinioDomainService;
        private ICashFileRepository _CashFileRepository;
        private IEFormAuserRepository _EFormAuserRepository;
        private IFinreviewRepository _FinreviewRepository;
        private IMailtemplateRepository _MailtemplateRepository;
        private IAppConfigRepository _AppConfigRepository;
        private ICompanyRepository _CompanyRepository;
        private IInvoiceRepository _InvoiceRepository;
        private IBPMDomainService _BPMDomainService;
        private IInvoiceDomainService _InvoiceDomainService;
        private IConfiguration _Configuretion;
        private Common.EmailHelper _EmailHelper;
        private IMinioDomainService _minioDomainService;
        private ICashPaymentHeadRepository _cashPaymentHeadRepository;
        public PaymentDomainService(
            IAutoNoRepository AutoNoRepository,
            IMinioRepository MinioRepository,
            IEFormAlistRepository EFormAlistRepository,
            ICashAccountRepository CashAccountRepository,
            IEmployeeInfoRepository EmployeeInfoRepository,
            ICashPaymentDetailRepository CashPaymentDetailRepository,
            ICashHeadRepository CashHeadRepository,
            IObjectMapper ObjectMapper,
            IMinioDomainService MinioDomainService,
            ICashFileRepository CashFileRepository,
            IEFormAuserRepository EFormAuserRepository,
            IFinreviewRepository FinreviewRepository,
            IMailtemplateRepository MailtemplateRepository,
            IAppConfigRepository AppConfigRepository,
            ICompanyRepository CompanyRepository,
            IInvoiceRepository InvoiceRepository,
            IEmpchsRepository EmpchsRepository,
            IBPMDomainService BPMDomainService,
            IInvoiceDomainService InvoiceDomainService,
            Common.EmailHelper EmailHelper,
            IConfiguration Configuretion,
            IMinioDomainService minioDomainService,
            ICashPaymentHeadRepository cashPaymentHeadRepository)
        {
            _AutoNoRepository = AutoNoRepository;
            _MinioRepository = MinioRepository;
            _EFormAlistRepository = EFormAlistRepository;
            _CashAccountRepository = CashAccountRepository;
            _EmployeeInfoRepository = EmployeeInfoRepository;
            _CashPaymentDetailRepository = CashPaymentDetailRepository;
            _CashHeadRepository = CashHeadRepository;
            _ObjectMapper = ObjectMapper;
            _MinioDomainService = MinioDomainService;
            _CashFileRepository = CashFileRepository;
            _EFormAuserRepository = EFormAuserRepository;
            _FinreviewRepository = FinreviewRepository;
            _MailtemplateRepository = MailtemplateRepository;
            _AppConfigRepository = AppConfigRepository;
            _CompanyRepository = CompanyRepository;
            _InvoiceRepository = InvoiceRepository;
            _EmpchsRepository = EmpchsRepository;
            _BPMDomainService = BPMDomainService;
            _InvoiceDomainService = InvoiceDomainService;
            _Configuretion = Configuretion;
            _EmailHelper = EmailHelper;
            _minioDomainService = minioDomainService;
            _cashPaymentHeadRepository = cashPaymentHeadRepository;
        }
        /// <summary>
        /// 生成付款清单
        /// </summary>
        /// <param name="excelFile">上传的Excel文件</param>
        /// <param name="generateListDto">用户选择的参数</param>
        /// <returns></returns>
        public async Task<Result<string>> GeneratePaylist(IFormCollection formCollection, string userId, string token)
        {
            string area = await _minioDomainService.GetMinioArea(userId);
            string company = formCollection["company"].ToString().Trim();
            string paymentdate = formCollection["paymentdate"].ToString().Trim();
            string identification = formCollection["identification"].ToString().Trim();
            var excelFile = formCollection.Files.FirstOrDefault();
            Result<string> result = new Result<string>();
            List<CashPaymentDetail> cashPaymentDetails = new List<CashPaymentDetail>();
            result.status = 2;
            if (string.IsNullOrEmpty(company))
            {
                result.message = L["CompanyEmpty"];
                return result;
            }
            if (string.IsNullOrEmpty(identification))
            {
                result.message = L["IdentificationEmpty"];
                return result;
            }
            string fileName = excelFile.FileName;
            //判断是否为excel文件
            if (fileName.ToLower().EndsWith(".xls") || fileName.ToLower().EndsWith(".xlsx"))
            {
                //读取Excel文件转为DataTable
                DataTable dt = NPOIHelper.GetDataTableFromExcel(excelFile, 2);
                var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
                {
                    emplid = (s[0].ToString().Trim() == string.Empty && s[6].ToString().Trim().Contains('-') && s[6].ToString().Trim().Split('-').Length > 2) ? s[6].ToString().Trim().Split('-')[2] : s[0].ToString().Trim(),//Employee ID
                    amount = s[1].ToString().Trim(),//Amount
                    summons = s[2].ToString().Trim(),//Summons
                    postingdate = s[3].ToString().Trim(),//Posting date
                    rno = s[4].ToString().Trim(),//Request No
                    reference = s[5].ToString().Trim(),//Reference
                    content = s[6].ToString().Trim(),//content
                    paymentdate = s[7].ToString().Trim(),// PaymentDate
                    paymentmethod = s[8].ToString().Trim(),// PaymentMethod
                    acctant = s[9].ToString().Trim(),// 入帳會計
                    paymentcurr = s[10].ToString().Trim()//付款幣別
                }).ToList();
                if (list.Count() == 0)
                {
                    result.message = L["ExcelEmpty"];
                    return result;
                }
                //判断所有单据状态为已入账
                var headquery = (await _CashHeadRepository.WithDetailsAsync()).Where(w => list.Select(w => w.rno).Contains(w.rno)).AsNoTracking().ToList();
                var notPostedList = headquery
                    .Where(h => h.status != "confirmed_posted")
                    .Select(h => new { h.rno, h.status })
                    .ToList();
                if (notPostedList.Count() > 0)
                {
                    notPostedList.ForEach(n =>
                    {
                        result.message += string.Format(L["RnoStatusError"], n.rno, n.status) + "\n";
                    });
                    return result;
                }
                var accountquery = (await _CashAccountRepository.WithDetailsAsync()).Where(w => list.Select(w => w.emplid).Contains(w.emplid)).ToList();
                var employeequery = (await _EmployeeInfoRepository.WithDetailsAsync()).Where(w => list.Select(w => w.emplid).Contains(w.emplid)).AsNoTracking().ToList();
                var pamentdatelist = list.Select(w => w.paymentdate).ToList();
                var empchsQuery = (await _EmpchsRepository.WithDetailsAsync()).Where(w => list.Select(w => w.emplid).Contains(w.emplid)).AsNoTracking().ToList();
                //读取Excel当中数据
                for (int i = 0; i < list.Count; i++)
                {
                    CashPaymentDetail cashPaylist = new CashPaymentDetail();
                    //PaymentDay默认为选中的值
                    string strPaymentDay = "";
                    if (pamentdatelist.Count > 1 && !string.IsNullOrEmpty(paymentdate.ToString()))
                    {
                        if (!IsDate(paymentdate))
                        {
                            result.message = L["IncorrectDate"];
                            return result;
                        }
                        strPaymentDay = Convert.ToDateTime(paymentdate).ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        strPaymentDay = list[i].paymentdate;
                    }
                    //对paymentdate做时间合法性校验
                    if (!IsDate(strPaymentDay))
                    {
                        result.message = L["IncorrectDate"];
                        return result;
                    }
                    cashPaylist.PaymentDate = Convert.ToDateTime(strPaymentDay);
                    cashPaylist.PaymentMethod = list[i].paymentmethod.ToString();
                    cashPaylist.PaymentId = list[i].acctant.ToString();
                    cashPaylist.PaymentCurr = list[i].paymentcurr.ToString();
                    int a = i + 1;
                    if (string.IsNullOrEmpty(list[i].amount.ToString()))//|| !IsNumber(list[i].amount.ToString())
                    {
                        result.message = string.Format(L["ExcelAmountEmpty"], i + 1);
                        return result;
                    }
                    cashPaylist.Amt = Convert.ToDecimal(list[i].amount);
                    if (string.IsNullOrEmpty(list[i].summons))
                    {
                        result.message = string.Format(L["ExcelSummonsEmpty"], i + 1);
                        return result;
                    }
                    cashPaylist.DocNo = list[i].summons;
                    //对postingdate做时间合法性校验
                    if (!IsDate(list[i].postingdate) && list[i].postingdate == string.Empty)
                    {
                        result.message = L["IncorrectDate"];
                        return result;
                    }
                    cashPaylist.PostDate = Convert.ToDateTime(list[i].postingdate);
                    var strFormcode = headquery.Where(w => w.rno == (list[i].rno.IndexOf("-") < 0 ? list[i].rno : list[i].rno.Substring(0, list[i].rno.IndexOf("-")))).Select(w => w.formcode).FirstOrDefault();
                    //如果该单号不存在ers，则不进行签核检查
                    if (!String.IsNullOrEmpty(strFormcode))
                    {
                        if (list[i].rno != string.Empty)
                        {
                            string srno = list[i].rno.ToString().Trim();
                            if (srno.IndexOf('-') > 0 && srno.Trim().Substring(0, 1) == "B")
                            {
                                srno = srno.Trim().Substring(0, srno.IndexOf('-'));
                            }
                            var _temp = await _BPMDomainService.BPMQueryIsSigned(srno, strFormcode, token);
                            if (_temp.status != 1)
                            {
                                result.message = _temp.message;
                                return result;
                            }
                            if (_temp.data == false)
                            {
                                result.message = string.Format(L["StatusError"], i + 1);
                                return result;
                            }
                        }
                    }
                    if (list[i].content == string.Empty)
                    {
                        result.message = string.Format(L["ContentEmpty"], i + 1);
                    }
                    string[] content = list[i].content.ToString().Trim().Split('-');
                    string payemplid = list[i].emplid;
                    var empdata = (from x in accountquery
                                   from y in employeequery
                                   where
                                   x.emplid == y.emplid &&
                                   x.emplid == payemplid
                                   select new
                                   {
                                       emplid = x.emplid,
                                       account = x.account,
                                       bank = x.bank,
                                       deptid = y.deptid,
                                       cname = y.name,
                                       scname = x.accountname
                                   }).ToList();
                    if (empdata.Count > 0)
                    {
                        // string chkbank = empdata.Select(w => w.bank).FirstOrDefault();
                        cashPaylist.DeptId = empdata.Select(w => w.deptid).FirstOrDefault();
                        cashPaylist.ScName = empdata.Select(w => w.cname).FirstOrDefault();
                        //取银行户名
                        string empchsScname = empchsQuery.Where(w => w.emplid == list[i].emplid)?.Select(s => s.scname)?.FirstOrDefault();
                        cashPaylist.ScName = String.IsNullOrEmpty(empchsScname) ? String.IsNullOrEmpty(cashPaylist.ScName) ? "" : ChineseConverter.Convert(cashPaylist.ScName, ChineseConversionDirection.TraditionalToSimplified) : empchsScname;
                        cashPaylist.Bank = empdata.Select(w => w.bank).FirstOrDefault();
                        cashPaylist.PayeeAccount = empdata.Select(w => w.account).FirstOrDefault();
                    }
                    else
                    {
                        result.message = string.Format(L["EmplidError"], i + 1);
                        return result;
                    }
                    string rno = list[i].rno.Trim();
                    if (!String.IsNullOrEmpty(rno))
                    {
                        if (await _CashPaymentDetailRepository.IsExistSysNo(rno))
                        {
                            result.message = string.Format(L["RnoMsg"], rno);
                            return result;
                        }
                    }
                    if (strFormcode != null && strFormcode != string.Empty)
                    {
                        cashPaylist.FormCode = strFormcode;
                    }
                    cashPaylist.SysNo = "";
                    cashPaylist.SysItem = a;
                    cashPaylist.Seq = a.ToString();
                    cashPaylist.PayeeId = payemplid;
                    cashPaylist.Amt = Convert.ToDecimal(list[i].amount);
                    cashPaylist.Usage = L["Usage"];
                    cashPaylist.DocNo = list[i].summons;
                    cashPaylist.PostDate = Convert.ToDateTime(Convert.ToDateTime(list[i].postingdate).ToString("yyyy/MM/dd"));
                    cashPaylist.Rno = list[i].rno;
                    cashPaylist.Contnt = list[i].content;
                    cashPaylist.BaseCurr = (await _CashHeadRepository.GetBCurrency(list[i].rno));
                    cashPaylist.cuser = userId;
                    cashPaylist.cdate = System.DateTime.Now;
                    cashPaylist.company = company;
                    cashPaylist.Identification = identification;
                    cashPaylist.Stat = "N";
                    cashPaymentDetails.Add(cashPaylist);
                }
                //分组
                var grouped = cashPaymentDetails
                 .GroupBy(x => new { x.Bank, x.PaymentDate })
                 .Select(g => g.ToList())
                 .ToList();
                List<CashPaymentHead> cashPaymentHeads = new List<CashPaymentHead>();
                //获取PaymentRun 签核主管，根据(上传人)PaymentRun会计
                IList<Finreview> finreviews=await _FinreviewRepository.GetCashXFinanceByCompany(company);
                EmployeeInfo employeeInfo=await _EmployeeInfoRepository.QueryByEmplid(finreviews[0].rv1);
                string fileNo = String.Empty;
                for (int i = 0; i < grouped.Count; i++)
                {
                    //sysNo
                    string sRno = await _AutoNoRepository.CreatePaymentNo();
                    if (i==0)
                    {
                        fileNo = sRno;
                    }
                    var group = grouped[i];
                    CashPaymentHead cashPaymentHead = new CashPaymentHead();
                    // 用第一个明细赋值
                    var first = group[0];
                    cashPaymentHead.SysNo = sRno;
                    cashPaymentHead.SysItem = i + 1;
                    cashPaymentHead.Bank = first.Bank;
                    cashPaymentHead.PaymentDate = first.PaymentDate; 
                    cashPaymentHead.Amt = group.Sum(x => x.Amt); // 金额合计
                    cashPaymentHead.Identification = first.Identification;
                    cashPaymentHead.PaymentStatus = "";
                    cashPaymentHead.PaymentDocNo = "";
                    cashPaymentHead.Status = "pending_approval";
                    //放入签核主管
                    cashPaymentHead.AssignedEmplid = employeeInfo.emplid;
                    cashPaymentHead.AssignedName = employeeInfo.name_a;

                    cashPaymentHeads.Add(cashPaymentHead);
                    for (int j = 0; j < group.Count; j++)
                    {
                        group[j].SysNo = sRno;
                        group[j].SysItem = i+1;
                        group[j].Seq = (j+1).ToString();
                    }
                }
                //保存上传的文件
                CashFile cashFile = new CashFile();
                cashFile.rno = fileNo;
                cashFile.seq = 1;
                cashFile.item = 1;
                cashFile.filetype = excelFile.ContentType;
                string objectName = "";
                string path = "";
                using (var steam = excelFile.OpenReadStream())
                {
                    objectName = DateTime.Now.ToString("yyyyMM") + "/" + fileNo + "/" + fileName;
                    await _MinioRepository.PutObjectAsync(objectName, steam, excelFile.ContentType, area);
                }
                path = objectName;
                cashFile.path = objectName;
                cashFile.filename = fileName;
                string TOFN = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                cashFile.tofn = TOFN;
                cashFile.ishead = "Y";
                cashFile.company = company;
                cashFile.formcode = "Payment";
                cashFile.cuser = userId;
                cashFile.cdate = System.DateTime.Now;
                await _CashFileRepository.InsertAsync(cashFile);
                //保存数据
                await _cashPaymentHeadRepository.InsertManyAsync(cashPaymentHeads);
                await _CashPaymentDetailRepository.InsertManyAsync(cashPaymentDetails);
               
                //var invoices = (await _InvoiceRepository.WithDetailsAsync()).Where(w => cashPaymentDetails.Select(s => s.Rno).Distinct().Contains(w.rno)).ToList();
                ////生成付款清单后回传付款信息给autopa
                //List<ERSPayDto> ersPayDtos = new List<ERSPayDto>();
                //foreach (var payInfo in cashPaymentDetails)
                //{
                //    ERSPayDto eRSPayDto = new()
                //    {
                //        ERSRno = payInfo.Rno,
                //        DocNo = payInfo.DocNo,
                //        DocDate = payInfo.PostDate,
                //        invoices = _ObjectMapper.Map<List<Invoice>, List<ERSInvDto>>(invoices.Where(w => w.rno == payInfo.Rno).ToList())
                //    };
                //    ersPayDtos.Add(eRSPayDto);
                //}
                //if (ersPayDtos.Count > 0)
                //{
                //    await _InvoiceDomainService.UpdateInvPayInfo(ersPayDtos, false, token);
                //}
                result.data = "rno";
                result.status = 1;
            }
            return result;
        }
        /// <summary>
        /// 已付款清单分页（条件）查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result<List<PaymentListDto>>> QueryPagePayList(Request<PayParamsDto> request)
        {
            Result<List<PaymentListDto>> result = new Result<List<PaymentListDto>>();
            var tQuery = (await _CashPaymentDetailRepository.WithDetailsAsync()).Where(w => w.Stat == "Y").ToList();
            var companyCodes = (await _CompanyRepository.WithDetailsAsync()).ToList();
            List<CashPaymentDetail> cashPaylistQuery = tQuery
                              .WhereIf(!string.IsNullOrEmpty(request.data.sysno), w => w.SysNo == request.data.sysno)
                              .WhereIf(!string.IsNullOrEmpty(request.data.paymentdate.ToString()), w => Convert.ToDateTime(w.PaymentDate).ToLocalTime().ToString("yyyy/MM/dd") == Convert.ToDateTime(request.data.paymentdate).ToLocalTime().ToString("yyyy/MM/dd"))
                              .WhereIf(!string.IsNullOrEmpty(request.data.identification), w => w.Identification == request.data.identification)
                              .WhereIf(!request.data.companyList.IsNullOrEmpty(), w => request.data.companyList.Contains(w.company))
                              .WhereIf(!string.IsNullOrEmpty(request.data.bank), w => w.Bank == request.data.bank)
                              .WhereIf(!string.IsNullOrEmpty(request.data.cuser), w => w.cuser == request.data.cuser)
                              .ToList();
            var resultQuery = (from t in (cashPaylistQuery)
                               group t by new { t.SysNo, t.cuser, t.Bank, t.company, t.Identification }
                               into g
                               select new PaymentListDto
                               {
                                   sysno = g.Key.SysNo,
                                   cuser = g.Key.cuser,
                                   amt = g.Sum(w => w.Amt),
                                   payment = g.Min(w => w.PaymentDate),
                                   bank = g.Key.Bank,
                                   company = g.Key.company,
                                   identification = g.Key.Identification
                               }).ToList();
            foreach (var item in resultQuery)
            {
                string companysap = companyCodes.Where(w => w.company == item.company).Distinct().FirstOrDefault().CompanySap;
                item.no = companysap.Substring(1, companysap.Length - 1) + Convert.ToDateTime(item.payment).ToString("yyMMdd") + ((item.bank == "中國建設銀行" || item.bank == "中国建设银行") ? "CCB" : ((item.bank == "中國工商銀行" || item.bank == "中国工商银行" ? "ICBC" : "")));
            }
            if (request.data != null)
            {
                string sysno = request.data.sysno;
                DateTime? paymentdate = request.data.paymentdate;
                string identification = request.data.identification;
                // string company = request.data.company;
                string bank = request.data.bank;
                string cuser = request.data.cuser;
            }
            int pageIndex;
            int pageSize;
            if (request.pageIndex <= 0 || request.pageSize <= 0 || request.pageIndex.ToString().IsNullOrEmpty() || request.pageSize.ToString().IsNullOrEmpty())
            {
                pageIndex = 1;
                pageSize = 10;
            }
            else
            {
                pageIndex = request.pageIndex;
                pageSize = request.pageSize;
            }
            result.data = resultQuery.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = resultQuery.Count;
            return result;
        }
        /// <summary>
        /// 未付款清单查询
        /// Comfirm list选项卡页面
        /// <param name="request">data：当前登录人</param>
        /// <returns></returns>
        public async Task<Result<List<PaymentListDto>>> QueryPageUnpaidList(Request<string> request, string userId)
        {
            Result<List<PaymentListDto>> result = new Result<List<PaymentListDto>>();
            var cashPayListQuery = (await _CashPaymentDetailRepository.WithDetailsAsync()).Where(w => w.Stat == "N").ToList();
            var companyCodes = (await _CompanyRepository.WithDetailsAsync()).ToList();
            var query = (from t in cashPayListQuery
                         group t by new { t.SysNo, t.cuser, t.Bank, t.company, t.Identification }
                              into g
                         select new PaymentListDto
                         {
                             sysno = g.Key.SysNo,
                             cuser = g.Key.cuser,
                             amt = g.Sum(w => w.Amt),
                             payment = g.Min(w => w.PaymentDate),
                             bank = g.Key.Bank,
                             company = g.Key.company,
                             identification = g.Key.Identification
                             //  no = g.Key.sysno + ((g.Key.bank == "中國建設銀行" || g.Key.bank == "中国建设银行") ? "CCB" : ((g.Key.bank == "中國工商銀行" || g.Key.bank == "中国工商银行" ? "ICBC" : "")))
                         }).ToList();
            foreach (var item in query)
            {
                string companysap = companyCodes.Where(w => w.company == item.company).Distinct().FirstOrDefault().CompanySap;
                item.no = companysap.Substring(1, companysap.Length - 1) + Convert.ToDateTime(item.payment).ToString("yyMMdd") + ((item.bank == "中國建設銀行" || item.bank == "中国建设银行") ? "CCB" : ((item.bank == "中國工商銀行" || item.bank == "中国工商银行" ? "ICBC" : "")));
            }
            var existQuery = (from t in (await _CashPaymentDetailRepository.WithDetailsAsync())
                              join f in (await _EFormAuserRepository.WithDetailsAsync())
                              on t.SysNo equals f.rno.IndexOf("-") < 0 ? f.rno : f.rno.Substring(0, f.rno.IndexOf("-"))
                              select new
                              {
                                  sysno = t.SysNo
                              }).ToList();
            var resultquery = query.Where(w => !(existQuery.Select(x => x.sysno).Contains(w.sysno)) && w.cuser == userId).ToList();
            int pageIndex;
            int pageSize;
            if (request.pageIndex <= 0 || request.pageSize <= 0 || request.pageIndex.ToString().IsNullOrEmpty() || request.pageSize.ToString().IsNullOrEmpty())
            {
                pageIndex = 1;
                pageSize = 10;
            }
            else
            {
                pageIndex = request.pageIndex;
                pageSize = request.pageSize;
            }
            result.data = resultquery.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = resultquery.Count;
            return result;
        }
        /// <summary>
        /// 按sysno、银行别查询付款清单
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public async Task<List<PaymentDetailDto>> QueryPaylistDetails(string sysno, string bank)
        {
            var query = (await _CashPaymentDetailRepository.WithDetailsAsync()).Where(w => w.SysNo == sysno && w.Bank == bank).ToList();
            var mapdata = _ObjectMapper.Map<List<CashPaymentDetail>, List<PaymentDetailDto>>(query);
            return mapdata;
        }
        /// <summary>
        /// 删除付款清单（按单号）
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public async Task<Result<string>> DeletePaylist(List<string> sysno, string token)
        {
            Result<string> result = new Result<string>();
            var paylists = (await _CashPaymentDetailRepository.QueryPayListBySysNo(sysno));
            var cashfiles = (await _CashFileRepository.WithDetailsAsync()).Where(w => sysno.Contains(w.rno)).ToList();
            if (paylists.Count <= 0 || cashfiles.Count <= 0)
            {
                result.status = 2;
                result.message = L["DeleteFail"] + ": " + "未找到对应单据/对应单据附件删除失败";
                return result;
            }
            //生成付款清单后回传付款信息给autopa
            var invoices = (await _InvoiceRepository.WithDetailsAsync()).Where(w => paylists.Select(s => s.Rno).Distinct().Contains(w.rno)).ToList();
            List<ERSPayDto> ersPayDtos = new List<ERSPayDto>();
            foreach (var payInfo in paylists)
            {
                ERSPayDto eRSPayDto = new()
                {
                    ERSRno = payInfo.Rno,
                    DocNo = payInfo.DocNo,
                    DocDate = payInfo.PostDate,
                    invoices = _ObjectMapper.Map<List<Invoice>, List<ERSInvDto>>(invoices)
                };
                ersPayDtos.Add(eRSPayDto);
            }
            if (ersPayDtos.Count > 0)
            {
                await _InvoiceDomainService.UpdateInvPayInfo(ersPayDtos, true, token);
            }
            //cash_paylist删除
            await _CashPaymentDetailRepository.DeleteManyAsync(paylists);
            //cash_file删除
            await _CashFileRepository.DeleteManyAsync(cashfiles);
            result.status = 1;
            result.message = L["DeleteSuccess"];
            return result;
        }
        public async Task<Result<byte[]>> GenerateRemittanceExcel(RemittanceDto parameters)
        {
            Result<byte[]> result = new Result<byte[]>();
            // @"select t.SEQ,t.PAYEE_ACCOUNT,t.bank,t.SCNAME,t.AMT,t.USAGE,e.emplid,t.payment,e.deptid from cash_paylist t,employee e where t.payee_id = e.emplid
            // and t.sysno='" + sysno + "' and t.bank ='" + bank + "' and t.company = '" + company + "' order by t.docno asc";
            IList<RemittanceExcelDto> query = (from t in (await _CashPaymentDetailRepository.WithDetailsAsync())
                                               join e in (await _EmployeeInfoRepository.WithDetailsAsync())
                                               on t.PayeeId equals e.emplid
                                               where (t.SysNo == parameters.sysno && t.Bank == parameters.bank && t.company == parameters.company)
                                               select new RemittanceExcelDto
                                               {
                                                   seq = t.Seq,
                                                   payeeaccount = t.PayeeAccount,
                                                   bank = t.Bank,
                                                   scname = t.ScName,
                                                   amt = t.Amt,
                                                   usage = t.Usage,
                                                   emplid = e.emplid,
                                                   payment = t.PaymentDate,
                                                   deptid = e.deptid,
                                               })
                                               .OrderBy(w => w.seq)
                                               .ToList();
            byte[] buffer = null;
            if (query.Count > 0)
            {
                if (parameters.bank == "中國工商銀行" || parameters.bank == "中国工商银行")
                {
                    // string ICBCheaders = L["PayeeAccount"] + ", " + L["Scname"] + ", " + L["Amt"];
                    XSSFWorkbook dataworkbook = new XSSFWorkbook();
                    ISheet datasheet = dataworkbook.CreateSheet("Sheet");
                    IRow rowHeader = datasheet.CreateRow(0);
                    rowHeader.CreateCell(0).SetCellValue(L["Seq"]);
                    rowHeader.CreateCell(1).SetCellValue(L["PayeeAccount"]);
                    rowHeader.CreateCell(2).SetCellValue(L["Scname"]);
                    rowHeader.CreateCell(3).SetCellValue(L["Amt"]);
                    rowHeader.CreateCell(4).SetCellValue(L["Usage1"]);
                    for (int i = 0; i < query.Count; i++)
                    {
                        IRow dataRow = datasheet.CreateRow(i + 1);
                        dataRow.CreateCell(0).SetCellValue(String.Format("{0:D3}", Convert.ToInt32(i + 1)));
                        dataRow.CreateCell(1).SetCellValue(query[i].payeeaccount);
                        dataRow.CreateCell(2).SetCellValue(query[i].scname);
                        dataRow.CreateCell(3).SetCellValue(Convert.ToDouble(query[i].amt));
                        dataRow.CreateCell(4).SetCellValue(query[i].usage);
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        dataworkbook.Write(ms);
                        ms.Flush();
                        buffer = ms.ToArray();
                    }
                }
                else
                {
                    // string CCBheaders = L["Seq"] + ", " + L["PayeeAccount"] + ", " + L["Scname"] + ", " + L["Amt"] + ", " + L["Usage1"];
                    XSSFWorkbook dataworkbook = new XSSFWorkbook();
                    ISheet datasheet = dataworkbook.CreateSheet("Sheet");
                    for (int i = 0; i < query.Count; i++)
                    {
                        IRow dataRow = datasheet.CreateRow(i);
                        dataRow.CreateCell(0).SetCellValue(String.Format("{0:D3}", Convert.ToInt32(i + 1)));
                        dataRow.CreateCell(1).SetCellValue(query[i].payeeaccount);
                        dataRow.CreateCell(2).SetCellValue(query[i].scname);
                        dataRow.CreateCell(3).SetCellValue(Convert.ToDouble(query[i].amt));
                        dataRow.CreateCell(4).SetCellValue(query[i].usage);
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        dataworkbook.Write(ms);
                        ms.Flush();
                        buffer = ms.ToArray();
                    }
                }
                result.data = buffer;
            }
            else
            {
                result.status = 2;
                result.message = L["NoDoc"];
            }
            return result;
        }
        /// <summary>
        /// 生成零用金付款清单Excel
        /// </summary>
        /// <param name="sysno">单号</param>
        /// <param name="bank">银行别</param>
        /// <returns></returns>
        public async Task<Result<byte[]>> GeneratePettyListExcel(UpdatePaymentDto request)
        {
            Result<byte[]> result = new Result<byte[]>();
            byte[] buffer = null;
            //查询所需数据
            var query = (from t in (await _CashPaymentDetailRepository.WithDetailsAsync())
                         join e in (await _EmployeeInfoRepository.WithDetailsAsync())
                         on t.cuser equals e.emplid
                         where (t.SysNo == request.sysno && t.Bank == request.bank)
                         select new RemittanceExcelDto
                         {
                             seq = t.Seq,
                             deptid = t.DeptId,
                             payeeid = t.PayeeId,
                             scname = t.ScName,
                             bank = t.Bank,
                             amt = t.Amt,
                             usage = t.Usage,
                             docno = t.DocNo,
                             postdate = t.PostDate,
                             contnt = t.Contnt,
                             identification = t.Identification,
                             payment = t.PostDate,
                             company = t.company,
                             ename = e.name,
                         }).OrderBy(w => w.docno).ToList();
            if (query.Count <= 0)
            {
                result.message = L["NoDoc"];
                result.status = 2;
                result.data = buffer;
                return result;
            }
            string fileName = request.bank + "零用金付款清單";
            string headerName = query.FirstOrDefault().company + "-" + Convert.ToDateTime(query.FirstOrDefault().payment).ToString("yyyyMMdd") + "-" + query.FirstOrDefault().identification + (request.bank == "中國工商銀行" ? "I" : "C");
            Decimal totalamt = 0;
            string ename = query.FirstOrDefault().ename.ToString().Trim();
            //创建Excel
            XSSFWorkbook workbook = new XSSFWorkbook();
            XSSFSheet excelSheet = (XSSFSheet)workbook.CreateSheet("sheet");
            //第一行内容数据填充
            IRow headrow1 = excelSheet.CreateRow(0);
            headrow1.Height = 400;
            ICell headcell_1 = headrow1.CreateCell(0);
            headcell_1.SetCellValue(request.bank + "零用金付款清單");
            headcell_1.CellStyle = GetheadStyle_1(workbook);//设置headerrow1样式
            //第二行内容数据填充
            IRow headrow2 = excelSheet.CreateRow(1);
            headrow2.Height = 400;
            ICell headcell_2 = headrow2.CreateCell(0);
            headcell_2.SetCellValue(headerName);
            headcell_2.CellStyle = GetheadStyle_2(workbook);
            CellRangeAddress cellRangeAddress = new CellRangeAddress(0, 0, 0, 9);
            excelSheet.AddMergedRegion(cellRangeAddress);
            CellRangeAddress cellRangeAddress1 = new CellRangeAddress(1, 1, 0, 9);
            excelSheet.AddMergedRegion(cellRangeAddress1);
            //第三行内容数据填充(标题)
            IRow headrow3 = excelSheet.CreateRow(2);
            headrow3.Height = 400;
            ICell headcell0 = headrow3.CreateCell(0);
            headcell0.SetCellValue(L["Seq"]);
            headcell0.CellStyle = GetCellStyle(workbook, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, 42, "@");
            ICell headcell1 = headrow3.CreateCell(1);
            headcell1.SetCellValue(L["Deptid"]);
            headcell1.CellStyle = GetCellStyle(workbook, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, 42, "@");
            ICell headcell2 = headrow3.CreateCell(2);
            headcell2.SetCellValue(L["Emplid"]);
            headcell2.CellStyle = GetCellStyle(workbook, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, 42, "@");
            ICell headcell3 = headrow3.CreateCell(3);
            headcell3.SetCellValue(L["AccountName"]);
            headcell3.CellStyle = GetCellStyle(workbook, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, 42, "@");
            ICell headcell4 = headrow3.CreateCell(4);
            headcell4.SetCellValue(L["Bankid"]);
            headcell4.CellStyle = GetCellStyle(workbook, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, 42, "@");
            ICell headcell5 = headrow3.CreateCell(5);
            headcell5.SetCellValue(L["Amount"]);
            headcell5.CellStyle = GetCellStyle(workbook, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, 42, "@");
            ICell headcell6 = headrow3.CreateCell(6);
            headcell6.SetCellValue(L["Purpose"]);
            headcell6.CellStyle = GetCellStyle(workbook, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, 42, "@");
            ICell headcell7 = headrow3.CreateCell(7);
            headcell7.SetCellValue(L["Summons"]);
            headcell7.CellStyle = GetCellStyle(workbook, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, 42, "@");
            ICell headcell8 = headrow3.CreateCell(8);
            headcell8.SetCellValue(L["PostingDate"]);
            headcell8.CellStyle = GetCellStyle(workbook, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, 42, "@");
            ICell headcell9 = headrow3.CreateCell(9);
            headcell9.SetCellValue(L["Content"]);
            headcell9.CellStyle = GetheadStyle_3(workbook);
            XSSFCellStyle strstyle = (XSSFCellStyle)workbook.CreateCellStyle();
            strstyle.BorderBottom = BorderStyle.Thin;
            strstyle.BorderLeft = BorderStyle.Thin;
            strstyle.BorderRight = BorderStyle.Thin;
            strstyle.BorderTop = BorderStyle.Thin;
            for (int i = 0; i < query.Count; i++)
            {
                excelSheet.SetColumnWidth(0, 5 * 256);
                excelSheet.SetColumnWidth(2, 12 * 256);
                excelSheet.SetColumnWidth(3, 15 * 256);
                excelSheet.SetColumnWidth(4, 5 * 256);
                excelSheet.SetColumnWidth(5, 10 * 256);
                excelSheet.SetColumnWidth(6, 5 * 256);
                excelSheet.SetColumnWidth(7, 12 * 256);
                excelSheet.SetColumnWidth(8, 10 * 256);
                excelSheet.SetColumnWidth(9, 40 * 256);
                IRow row = excelSheet.CreateRow(i + 3);
                row.Height = 400;
                ICell cell = row.CreateCell(0);
                cell.SetCellValue((i + 1).ToString("000").Trim());
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue(query[i].deptid.ToString().Trim());
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue(query[i].payeeid.ToString().Trim());
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(query[i].scname.ToString().Trim());
                ICell cell4 = row.CreateCell(4);
                if (query[i].bank.ToString().Trim() == "中國工商銀行")
                {
                    cell4.SetCellValue("工行");
                }
                else if (query[i].bank.ToString().Trim() == "中國建設銀行")
                {
                    cell4.SetCellValue("建行");
                }
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(Convert.ToDecimal(query[i].amt.ToString().Trim()).ToString("N2"));
                totalamt += Convert.ToDecimal(query[i].amt.ToString().Trim());
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(ChineseConverter.Convert(query[i].usage.ToString().Trim(), ChineseConversionDirection.TraditionalToSimplified));
                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(query[i].docno.ToString().Trim());
                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue(Convert.ToDateTime(query[i].postdate).ToString("yyyyMMdd"));
                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue(query[i].contnt.ToString().Trim());
                cell.CellStyle = GetCellStyle(strstyle, "@");
                cell1.CellStyle = GetCellStyle(strstyle, "@");
                cell2.CellStyle = GetCellStyle(strstyle, "@");
                cell3.CellStyle = GetCellStyle(strstyle, "@");
                cell4.CellStyle = GetCellStyle(strstyle, "@");
                cell5.CellStyle = GetCellStyle(strstyle, "@");
                cell6.CellStyle = GetCellStyle(strstyle, "@");
                cell7.CellStyle = GetCellStyle(strstyle, "@");
                cell8.CellStyle = GetCellStyle(strstyle, "@");
                cell9.CellStyle = GetCellStyle(strstyle, "@");
            }
            IRow emptyrow = excelSheet.CreateRow(query.Count + 3);
            emptyrow.Height = 400;
            ICell emptycell = emptyrow.CreateCell(5);
            ICellStyle emptystyle = workbook.CreateCellStyle();
            emptystyle.BorderBottom = BorderStyle.Thin;
            emptycell.CellStyle = emptystyle;
            IRow totalrow = excelSheet.CreateRow(query.Count + 4);
            totalrow.Height = 400;
            ICell totalcell = totalrow.CreateCell(5);
            totalcell.SetCellValue(totalamt.ToString("N2"));
            ICellStyle totalstyle = workbook.CreateCellStyle();
            totalstyle.BorderBottom = BorderStyle.Double;
            IDataFormat df = workbook.CreateDataFormat();
            totalstyle.DataFormat = df.GetFormat("#,##0.00");
            totalstyle.Alignment = HorizontalAlignment.Right;
            totalstyle.VerticalAlignment = VerticalAlignment.Center;
            totalcell.CellStyle = totalstyle;
            IRow emptyrow1 = excelSheet.CreateRow(query.Count + 5);
            emptyrow1.Height = 400;
            for (int i = 0; i < 10; i++)
            {
                ICell emptycell1 = emptyrow1.CreateCell(i);
                ICellStyle emptystyle1 = workbook.CreateCellStyle();
                emptystyle1.BorderBottom = BorderStyle.Thin;
                emptycell.CellStyle = emptystyle1;
            }
            IRow approverow = excelSheet.CreateRow(query.Count + 6);
            approverow.Height = 400;
            for (int i = 0; i < 10; i++)
            {
                ICell approvecell = approverow.CreateCell(i);
                if (i == 0)
                {
                    approvecell.SetCellValue("Approve：");
                }
                else if (i == 4)
                {
                    approvecell.SetCellValue("Review：");
                }
                else if (i == 8)
                {
                    approvecell.SetCellValue("Prepared：");
                }
                else if (i == 9)
                {
                    approvecell.SetCellValue(ename);
                }
                ICellStyle approvestyle = workbook.CreateCellStyle();
                approvestyle.BorderBottom = BorderStyle.Thin;
                approvestyle.VerticalAlignment = VerticalAlignment.Center;
                if (i == 8)
                {
                    approvestyle.Alignment = HorizontalAlignment.Right;
                }
                approvecell.CellStyle = approvestyle;
            }
            CellRangeAddress cellRangeAddress2 = new CellRangeAddress(query.Count + 6, query.Count + 6, 0, 1);
            excelSheet.AddMergedRegion(cellRangeAddress2);
            IRow emptyrow2 = excelSheet.CreateRow(query.Count + 7);
            emptyrow2.Height = 400;
            for (int i = 0; i < 10; i++)
            {
                ICell emptycell2 = emptyrow2.CreateCell(i);
                ICellStyle emptystyle2 = workbook.CreateCellStyle();
                emptystyle2.BorderBottom = BorderStyle.Thin;
                emptycell2.CellStyle = emptystyle2;
            }
            IRow empower = excelSheet.CreateRow(query.Count + 8);
            empower.Height = 400;
            for (int i = 0; i < 10; i++)
            {
                ICell approvecell = empower.CreateCell(i);
                if (i == 0)
                {
                    approvecell.SetCellValue(L["Certigier1"]);
                }
                else if (i == 3)
                {
                    approvecell.SetCellValue(L["Certigier2"]);
                }
                else if (i == 7)
                {
                    approvecell.SetCellValue(L["Review"]);
                }
                else if (i == 9)
                {
                    approvecell.SetCellValue(L["ResponsiblePerson"]);
                }
                ICellStyle approvestyle = workbook.CreateCellStyle();
                approvestyle.BorderBottom = BorderStyle.Double;
                approvestyle.VerticalAlignment = VerticalAlignment.Center;
                approvecell.CellStyle = approvestyle;
            }
            CellRangeAddress cellRangeAddress3 = new CellRangeAddress(query.Count + 8, query.Count + 8, 0, 1);
            excelSheet.AddMergedRegion(cellRangeAddress3);
            //
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
        /// 变更付款状态（点击OK）
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<Result<string>> UpdatePaymentStatus(List<UpdatePaymentDto> parameters)
        {
            Result<string> result = new Result<string>();
            List<string> sysnos = parameters.Select(x => x.sysno).ToList();
            List<CashPaymentDetail> payList = new();
            if (sysnos.Count > 0)
                payList = await (await _CashPaymentDetailRepository.WithDetailsAsync()).Where(i => sysnos.Contains(i.SysNo)).ToListAsync();
            List<string> rnos = payList.Select(x => x.Rno).ToList();
            List<string> headrnos = payList.Select(x => x.Rno).ToList();
            for (int i = 0; i < headrnos.Count; i++)
            {
                if (headrnos[i].IndexOf('-') > 0)
                {
                    headrnos[i] = headrnos[i].Substring(0, headrnos[i].IndexOf('-'));
                }
                else
                {
                    continue;
                }
            }
            rnos = rnos.Distinct().ToList();
            headrnos = headrnos.Distinct().ToList();
            List<CashHead> cashHeads = await (await _CashHeadRepository.WithDetailsAsync()).Where(i => headrnos.Contains(i.rno)).ToListAsync();
            var tempPayList = payList.Where(s => !String.IsNullOrEmpty(s.Rno) && s.Rno.Length >= 12 && s.Rno.StartsWith("B") ).ToList();//筛出批量报销的付款清单记录
            foreach (var item in payList)
                item.Stat = "Y";
            foreach (var item in cashHeads)
            {
                item.stat = "Y";
                if (item.rno.StartsWith("B"))
                {
                    item.payment = tempPayList.Where(i => i.Rno.Substring(0, 12) == item.rno).First().PaymentDate;
                }
                else
                {
                    item.payment = payList.Where(i => !String.IsNullOrEmpty(i.Rno) && i.Rno.ToString().Substring(0, i.Rno.Length >=12?12: i.Rno.Length) == item.rno).First().PaymentDate;
                }
            }
            await _CashHeadRepository.UpdateManyAsync(cashHeads);
            await _CashPaymentDetailRepository.UpdateManyAsync(payList);
            var groupPaylist = payList.GroupBy(w => new
            {
                w.PayeeId,
                w.SysNo
            }).Select(s => s.FirstOrDefault()).ToList();
            foreach (var item in groupPaylist)
            {
                await SendMailToUserForPayment(item.PayeeId, item.SysNo);
            }
            result.message = L["PaymentSuccess"];
            return result;
        }
        //付款后发送邮件
        public async Task SendMailToUserForPayment(string emplid, string sysno)
        {
            var cashPaylist = (await _CashPaymentDetailRepository.WithDetailsAsync())
            .Where(w => w.SysNo == sysno && w.PayeeId == emplid)
            .Select(s => new
            {
                deptid = s.DeptId,
                payeeid = s.PayeeId,
                scname = s.ScName,
                payeeaccount = s.PayeeAccount,
                bank = s.Bank,
                amt = s.Amt,
                usage = s.Usage,
                docno = s.DocNo,
                postdate = s.PostDate.ToString("yyyy/MM/dd"),
                formcode = s.FormCode,
                rno = s.Rno,
                contnt = s.Contnt,
                payment = s.PaymentDate.HasValue ? Convert.ToDateTime(s.PaymentDate).ToString("yyyy/MM/dd") : string.Empty,
                company = s.company
            }).ToList();
            var empInfo = await (await _EmployeeInfoRepository.WithDetailsAsync()).Where(w => w.emplid == emplid).FirstOrDefaultAsync();
            //读取模板
            var mail = (await _MailtemplateRepository.GetPaymentTemplate(cashPaylist.FirstOrDefault().company));
            //string mailTemplate = "<HTML><BODY>Dear&emsp;{0}.<br />{1}<br/>以上申請報銷單已於(日期:{2})提交銀行安排付款，請注意查收是否到賬,若2天后尚未收到款項，請與財務處張焯雅聯繫(分機:8560+6837)，將協助處理確認。<br />Please be noted the expenses you applied have been paid on {3}. Please check your bank account, If you have not received two days later, please contact with Chair Zhang (ext.8560+6837)。<br/><br /> Expense Reimbursement System(員工費用報銷系統) <br /> --------------------------------------------------- <br /> This email was sent by system.<br />Please do not reply.<br />Helpdesk: Chair Zhang (ext.8560+6837)<BODY></HTML>";
            StringBuilder paymentTable = new StringBuilder();
            paymentTable.Append(@$"<table style='border-color:#d3d3d3; text-align: center; margin-top: 10px; margin-bottom: 10px; border-collapse: collapse;border-width:0px;' cellspacing='0' border='1'><tbody><tr><td>{L["PaymentMail-Deptid"]}</td><td>{L["PaymentMail-Emplid"]}</td><td>{L["PaymentMail-AccountName"]}</td><td>{L["PaymentMail-Bankid"]}</td><td>{L["PaymentMail-Amount"]}</td><td>{L["PaymentMail-Purpose"]}</td><td>{L["PaymentMail-Summons"]}</td><td>{L["PaymentMail-PostingDate"]}</td><td>{L["PaymentMail-Reference"]}</td><td>{L["PaymentMail-Content"]}</td><td>{L["PaymentMail-Payment"]}</td></tr>");
            foreach (var item in cashPaylist)
            {
                paymentTable.Append(@$"<tr><td>{item.deptid}</td><td>{item.payeeid}</td><td>{item.scname}</td><td>{item.bank}</td><td>{item.amt}</td><td>{item.usage}</td><td>{item.docno}</td><td>{item.postdate}</td><td>{item.rno}</td><td>{item.contnt}</td><td>{item.payment}</td></tr>");
            }
            paymentTable.Append(@"</tbody></table>");
            string mailSubject = String.Format(mail.subject, empInfo.name);
            string mailTemplate = mail.mailmsg;
            string mailMsg = String.Format(mailTemplate, empInfo.name, paymentTable.ToString(), cashPaylist.FirstOrDefault().payment, cashPaylist.FirstOrDefault().payment);
            var replaceEmployees = await _AppConfigRepository.GetReplaceMailBySite(cashPaylist.FirstOrDefault().company);
            if (replaceEmployees == null || replaceEmployees.Count == 0)
            {
                if (!String.IsNullOrEmpty(empInfo.email_address_a))
                {
                    _EmailHelper.SendEmail(mailSubject, empInfo.email_address_a, "", mailMsg);
                }
            }
            else
            {
                var rEmpInfs = (await _EmployeeInfoRepository.WithDetailsAsync()).Where(w => replaceEmployees.Contains(w.emplid) && !String.IsNullOrEmpty(w.email_address_a)).Select(s => s.email_address_a).ToList();
                _EmailHelper.SendEmail(mailSubject, string.Join(";", rEmpInfs), "", mailMsg);
            }
        }
        //时间合法性校验
        // public static bool IsDate(string strDate)
        // {
        //     bool result = false;
        //     try
        //     {
        //         string[] DateTimeList =
        //         {
        //             "yyyy/MM/dd",
        //             "dd/MM/yyyy"
        //         };
        //         DateTime dt = DateTime.ParseExact(strDate,
        //                                           DateTimeList,
        //                                           System.Globalization.CultureInfo.InvariantCulture,
        //                                           System.Globalization.DateTimeStyles.AllowWhiteSpaces
        //                                           );
        //         result = true;
        //     }
        //     catch (Exception)
        //     {
        //         result = false;
        //     }
        //     return result;
        // }
        public bool IsDate(string strDate)
        {
            try
            {
                DateTime.Parse(strDate);
                return true;
            }
            catch
            {
                return false;
            }
        }
        //判断是否字符串为数字
        public static bool IsNumber(string str)
        {
            bool isMatch = Regex.IsMatch(str, @"^\d+$"); // 判断字符串是否为数字的正则表达式
            return isMatch;
        }
        //设置header样式
        protected ICellStyle GetheadStyle_1(XSSFWorkbook xssfWorkbook)
        {
            ICellStyle style = xssfWorkbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            IFont font = xssfWorkbook.CreateFont();
            font.FontHeightInPoints = 12;
            style.SetFont(font);
            return style;
        }
        protected ICellStyle GetheadStyle_2(XSSFWorkbook xssfWorkbook)
        {
            ICellStyle style = xssfWorkbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            IFont font = xssfWorkbook.CreateFont();
            font.FontHeightInPoints = 11;
            style.SetFont(font);
            return style;
        }
        //标题单元格样式设置
        private ICellStyle GetCellStyle(XSSFWorkbook xssfWorkbook
        , NPOI.SS.UserModel.BorderStyle borderLeft, NPOI.SS.UserModel.BorderStyle borderBottom, NPOI.SS.UserModel.BorderStyle borderRight, NPOI.SS.UserModel.BorderStyle borderTop
        , short fillforgeroundColor
        , string dataFormat)
        {
            ICellStyle styleInfo = xssfWorkbook.CreateCellStyle();
            styleInfo.BorderLeft = borderLeft;
            styleInfo.BorderBottom = borderBottom;
            styleInfo.BorderRight = borderRight;
            styleInfo.BorderTop = borderTop;
            if (dataFormat != "@")
            {
                styleInfo.Alignment = HorizontalAlignment.Right;
                styleInfo.VerticalAlignment = VerticalAlignment.Center;
            }
            else
            {
                styleInfo.Alignment = HorizontalAlignment.Left;
                styleInfo.VerticalAlignment = VerticalAlignment.Center;
            }
            XSSFDataFormat df = (XSSFDataFormat)xssfWorkbook.CreateDataFormat();
            styleInfo.DataFormat = df.GetFormat(dataFormat);
            return styleInfo;
        }
        private ICellStyle GetCellStyle(ICellStyle styleInfo, string dataFormat)
        {
            if (dataFormat != "@")
            {
                styleInfo.Alignment = HorizontalAlignment.Right;
                styleInfo.VerticalAlignment = VerticalAlignment.Center;
            }
            else
            {
                styleInfo.Alignment = HorizontalAlignment.Left;
                styleInfo.VerticalAlignment = VerticalAlignment.Center;
            }
            // styleInfo.FillForegroundColor = fillforgeroundColor;//设置填充色
            //  styleInfo.FillPattern = FillPatternType.SOLID_FOREGROUND;//设置填充色的时候必须设置这个
            // 当前日期格式的需要以下这样设置
            //HSSFDataFormat format = (HSSFDataFormat)hssfworkbook.CreateDataFormat();
            //styleInfo.DataFormat = format.GetFormat("yyyy年m月d日");
            return styleInfo;
        }
        //零用金付款清单内容单元格样式设置
        protected ICellStyle GetheadStyle_3(XSSFWorkbook xssfworkbook)
        {
            ICellStyle style = xssfworkbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            IFont font = xssfworkbook.CreateFont();
            font.FontHeightInPoints = 10;
            style.SetFont(font);
            return style;
        }
    }
}

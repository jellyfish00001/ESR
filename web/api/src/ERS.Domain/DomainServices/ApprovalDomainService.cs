using ERS.Domain.IDomainServices;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.Approval;
using ERS.DTO.MobileSign;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
namespace ERS.DomainServices
{
    public class ApprovalDomainService : CommonDomainService, IApprovalDomainService
    {
        private IBDExpRepository _BDExpRepository;
        private IHttpClientFactory _HttpClient;
        private IConfiguration _configuration;
        private IFinreviewRepository _FinreviewRepository;
        private IEmpOrgRepository _EmpOrgRepository;
        private ILogger<ApprovalDomainService> _logger;
        private IEFormAuditRepository _EFormAuditRepository;
        private IAppConfigRepository _AppConfigRepository;
        private IMailtemplateRepository _MailtemplateRepository;
        private IEmployeeDomainService _IEmployeeDomainService;
        private IConfiguration _Configuration;
        private IEFormAlistRepository _EFormAlistRepository;
        private IEFormAuserRepository _EFormAuserRepository;
        private IEFormHeadRepository _EFormHeadRepository;
        private IEFormSignlogRepository _EFormSignlogRepository;
        private IEFormProxyRepository _EFormProxyRepository;
        private IEmployeeRepository _EmployeeRepository;
        private ICashDetailRepository _CashDetailRepository;
        private IBDFormRepository _BDFormRepository;
        private ICashHeadRepository _CashHeadRepository;
        private ICashAmountRepository _CashAmountRepository;
        private IBDCashReturnRepository _bdcashreturnRepository;
        private IBPMDomainService _BPMDomainService;
        private IApprovalPaperRepository _EFormPaperRepository;
        private ICompanyDomainService _CompanyDomainService;
        private IBDInvoiceFolderRepository _BDInvoiceFolderRepository;
        private ICashFileRepository _CashFileRepository;
        private IInvoiceDomainService _InvoiceDomainService;
        private IApprovalPaperDomainService _EFormPaperDomainService;
        private IAccountDomainService _AccountDomainService;
        private IObjectMapper _objectMapper;
        private IMinioDomainService _MinioDomainService;
        private IMobileSignService _mobileSignService;
        private Common.EmailHelper _EmailHelper;
        private IFinreviewRepository _finreviewRepository;
        private IApprovalAssignedApproverRepository _ApprovalAssignedApproverRepository;
        private IApprovalFlowRepository _ApprovalFlowRepository;


        public ApprovalDomainService(IMobileSignService mobileSignService, IBDExpRepository BDExpRepository, IHttpClientFactory HttpClient, IConfiguration configuration, IFinreviewRepository FinreviewRepository, IEmpOrgRepository EmpOrgRepository, ILogger<ApprovalDomainService> logger, IEFormAuditRepository EFormAuditRepository, IAppConfigRepository AppConfigRepository, IMailtemplateRepository MailtemplateRepository, IEmployeeDomainService EmployeeDomainService, IConfiguration Configuration, IEFormAlistRepository EFormAlistRepository, IEFormAuserRepository EFormAuserRepository, IEFormHeadRepository EFormHeadRepository, IEFormSignlogRepository EFormSignlogRepository, IEmployeeRepository EmployeeRepository, IObjectMapper ObjectMapper, IEFormProxyRepository EFormProxyRepository, ICashDetailRepository CashDetailRepository, IBDFormRepository BDFormRepository, ICashHeadRepository CashHeadRepository, ICashAmountRepository CashAmountRepository, IApprovalPaperDomainService EFormPaperDomainService, IAccountDomainService AccountDomainService, IBDCashReturnRepository BDCashReturnRepository, IBPMDomainService BPMDomainService, IApprovalPaperRepository EFormPaperRepository, ICompanyDomainService CompanyDomainService, IBDInvoiceFolderRepository BDInvoiceFolderRepository, IInvoiceDomainService InvoiceDomainService, ICashFileRepository CashFileRepository, IMinioDomainService MinioDomainService, Common.EmailHelper emailHelper, IFinreviewRepository finreviewRepository, IApprovalAssignedApproverRepository approvalAssignedApproverRepository, IApprovalFlowRepository approvalFlowRepository)
        {
            _mobileSignService = mobileSignService;
            _BDExpRepository = BDExpRepository;
            _HttpClient = HttpClient;
            _configuration = configuration;
            _FinreviewRepository = FinreviewRepository;
            _EmpOrgRepository = EmpOrgRepository;
            _logger = logger;
            _EFormAuditRepository = EFormAuditRepository;
            _AppConfigRepository = AppConfigRepository;
            _MailtemplateRepository = MailtemplateRepository;
            _IEmployeeDomainService = EmployeeDomainService;
            _Configuration = Configuration;
            _EFormAlistRepository = EFormAlistRepository;
            _EFormAuserRepository = EFormAuserRepository;
            _EFormHeadRepository = EFormHeadRepository;
            _EFormSignlogRepository = EFormSignlogRepository;
            _EmployeeRepository = EmployeeRepository;
            _objectMapper = ObjectMapper;
            _EFormProxyRepository = EFormProxyRepository;
            _CashDetailRepository = CashDetailRepository;
            _BDFormRepository = BDFormRepository;
            _CashHeadRepository = CashHeadRepository;
            _CashAmountRepository = CashAmountRepository;
            _EFormPaperDomainService = EFormPaperDomainService;
            _AccountDomainService = AccountDomainService;
            _bdcashreturnRepository = BDCashReturnRepository;
            _BPMDomainService = BPMDomainService;
            _EFormPaperRepository = EFormPaperRepository;
            _CompanyDomainService = CompanyDomainService;
            _BDInvoiceFolderRepository = BDInvoiceFolderRepository;
            _InvoiceDomainService = InvoiceDomainService;
            _CashFileRepository = CashFileRepository;
            _MinioDomainService = MinioDomainService;
            _EmailHelper = emailHelper;
            _finreviewRepository = finreviewRepository;
            _ApprovalAssignedApproverRepository = approvalAssignedApproverRepository;
            _ApprovalFlowRepository = approvalFlowRepository;
        }
        /// <summary>
        /// 获取是否为会计一签核
        /// </summary>
        /// <param name="rno"></param>
        /// <param name="cuser"></param>
        /// <returns></returns>
        public async Task<bool> GetIsAccountant1Sign(string rno, string cuser)
        {
            return await (from a in await _EFormAuserRepository.WithDetailsAsync()
                          join b in await _EFormAlistRepository.WithDetailsAsync() on a.rno equals b.rno
                          where a.rno == rno && b.step == 10 && a.cemplid == b.cemplid
                          select new
                          {
                              a.rno
                          }).AsNoTracking().CountAsync() > 0;
            //return await (await _EFormAuserRepository.WithDetailsAsync()).Where(i => i.rno == rno && i.seq == 10 && i.cemplid == cuser).AsNoTracking().CountAsync() > 0;
        }
        public async Task<bool> SendMail(string msg, string subject)
        {
            _EmailHelper.SendEmail(subject, "samsong_zhang@wistron.com", "wiwi_wei@wistron.com", msg);
            return true;
        }
        /// <summary>
        /// 获取签核全部会计
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<Result<IList<FinReviewDto>>> GetFinance(string user)
        {
            Result<IList<FinReviewDto>> result = new();
            result.status = 2;
            List<string> companycode = (await _CompanyDomainService.GetCompanyByArea(user)).data.ToList();
            IList<Finreview> finreviews = await _FinreviewRepository.ReadOnlyByCompany(companycode);
            IList<FinReviewDto> finReviewDtos = _objectMapper.Map<IList<Finreview>, IList<FinReviewDto>>(finreviews);
            result.data = finReviewDtos;
            result.status = 1;
            return result;
        }
        /// <summary>
        /// 会计1转单(没邮件通知)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cuser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<string>> Transform(SignDto data, string cuser, string token)
        {
            Result<string> result = new();
            result.status = 2;
            EFormAuser auser = await (await _EFormAuserRepository.WithDetailsAsync()).Where(i => i.rno == data.rno && i.cemplid == cuser && i.seq == 10).FirstOrDefaultAsync();
            if (auser == null)
            {
                result.message = "unsuitable condition";
                return result;
            }
            data.formCode = auser.formcode;
            Result<IList<SignForm>> signs = await _BPMDomainService.BPMQuerySign(data.rno, data.formCode, token);
            List<EFormAlist> formAlist = await (await _EFormAlistRepository.WithDetailsAsync()).Where(i => i.rno == data.rno).ToListAsync();
            if (signs.status != 1)
            {
                result.message = signs.message;
                return result;
            }
            List<SignForm> eFormAlists = signs.data.Where(i => i.seq == 10 || i.seq == 11 || i.seq == 12).ToList();
            EFormAlist temp = formAlist.Where(i => i.step == 10).FirstOrDefault();
            SignForm account1 = eFormAlists.Where(i => i.seq == 10 && i.signer_emplid == temp.cemplid).FirstOrDefault();
            if (account1 != null && !string.IsNullOrEmpty(data.toEmplid1) && data.toEmplid1 != "*")
            {
                if (temp != null)
                    temp.cemplid = data.toEmplid1;
                data.fromEmplid = account1.signer_emplid;
                data.toEmplid = data.toEmplid1;
                auser.cemplid = data.toEmplid1;
                result = await BPMTransform(data, cuser, token);
            }
            if (result.status != 1) return result;
            temp = formAlist.Where(i => i.step == 11).FirstOrDefault();
            SignForm account2 = eFormAlists.Where(i => i.seq == 11 && i.signer_emplid == temp.cemplid).FirstOrDefault();
            if (account2 != null && !string.IsNullOrEmpty(data.toEmplid2) && data.toEmplid2 != "*")
            {
                if (temp != null)
                    temp.cemplid = data.toEmplid2;
                data.fromEmplid = account2.signer_emplid;
                data.toEmplid = data.toEmplid2;
                result = await BPMTransform(data, cuser, token);
            }
            if (result.status != 1) return result;
            temp = formAlist.Where(i => i.step == 12).FirstOrDefault();
            SignForm account3 = eFormAlists.Where(i => i.seq == 12 && i.signer_emplid == temp.cemplid).FirstOrDefault();
            if (account3 != null && !string.IsNullOrEmpty(data.toEmplid3) && data.toEmplid3 != "*")
            {
                if (temp != null)
                    temp.cemplid = data.toEmplid3;
                data.fromEmplid = account3.signer_emplid;
                data.toEmplid = data.toEmplid3;
                result = await BPMTransform(data, cuser, token);
            }
            if (result.status != 1) return result;
            await _EFormAuserRepository.UpdateAsync(auser);
            await _EFormAlistRepository.UpdateManyAsync(formAlist);
            return result;
        }
        /// <summary>
        /// BPM转单签核(没邮件通知)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cuser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ERS.Common.BPMException"></exception>
        async Task<Result<string>> BPMTransform(SignDto data, string cuser, string token)
        {
            Result<string> result = new();
            result.status = 2;
            CreateSign sign = new()
            {
                formCode = data.formCode,
                formno = data.rno,
                reason = "transform",
                fromEmplid = data.fromEmplid,
                toEmplid = data.toEmplid,
                userid = cuser
            };
            string datas = JsonConvert.SerializeObject(sign);
            HttpClient httpClient = _HttpClient.CreateClient();
            _logger.LogInformation("转单入参： {Datas}", datas);
            BPMResult<SignResult> response = await httpClient.PostHelperAsync<BPMResult<SignResult>>(_configuration.GetSection("BPM:transformsign").Value, datas, token: token);
            if (response == null)
            {
                throw new ERS.Common.BPMException("BPM:转单签核请求失败。 status: !200； data: " + datas);
            }
            else if (response.status == "success")
            {
                result.status = 1;
            }
            else
            {
                _logger.LogError("BPM:转单签核请求失败。 status: {Status}; msg: {Msg}; data: {Datas}", response.status, response.msg, datas);
                throw new ERS.Common.BPMException("BPM:转单签核请求失败。 status:" + response.status + "; " + response.msg);
            }
            return result;
        }
        /// <summary>
        /// 取消签核
        /// </summary>
        /// <param name="rno"></param>
        /// <param name="cuser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<string>> Cancel(string rno, string cuser, string token)
        {
            Result<string> result = new();
            result.status = 2;
            CashHead cashHead = await _CashHeadRepository.GetByNo(rno);
            if (cashHead == null)
            {
                result.message = "Not exist";
                return result;
            }
            if (cashHead.cuser != cuser)
            {
                result.message = L["NotSelfToCancel"];
                return result;
            }
            if (cashHead.status == "canceled")
            {
                result.message = L["HaveBeenCancelled"];
                return result;
            }
            if (cashHead.status == "temporary")
            {
                result.message = L["HaveBeenTemperatory"];
                return result;
            }
            //if (cashHead.status == "R")
            //{
            //    result.message = L["HaveBeenReturn"];
            //    return result;
            //}
            Result<bool> tem = await _BPMDomainService.BPMQueryHaveSign(rno, cuser, token);
            if (tem.status != 1)
            {
                result.message = tem.message;
                return result;
            }
            if (tem.data)
            {
                result.message = L["HaveSignedLogs"];
                return result;
            }
            SignDto data = new()
            {
                formCode = cashHead.formcode,
                rno = rno,
            };
            await _BDInvoiceFolderRepository.CancelRno(rno);
            cashHead.ChangeStatus("canceled");
            //cashHead.ChangeApidToRequest();
            await _CashHeadRepository.UpdateAsync(cashHead);
            EFormAuser eFormAuser = await _EFormAuserRepository.FindAsync(i => i.rno == rno);
            await _EFormAuserRepository.DeleteAsync(eFormAuser);
            EFormSignlog eFormSignlog = new()
            {
                rno = rno,
                seq = 1,
                aemplid = cuser,
                aresult = "C",
                formcode = cashHead.formcode,
                adate = DateTime.Now,
                cemplid = cuser,
                step = 1,
                company = cashHead.company
            };
            ApprovalPaper eFormPaper = await _EFormPaperRepository.FindAsync(i => i.rno == rno);
            if (eFormPaper != null)
                await _EFormPaperRepository.DeleteAsync(eFormPaper);
            await _EFormSignlogRepository.InsertAsync(eFormSignlog);
            await _CashAmountRepository.Kickback(rno);
            result = await _BPMDomainService.BPMCancel(data, cuser, token);
            await _InvoiceDomainService.UpdateInvoiceToUnrequested(rno, token);
            //抽单
            await _mobileSignService.recalllForm(data.rno);
            result.status = 1;
            return result;
        }
        /// <summary>
        /// 驳回
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cuser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<string>> Reject(SignDto data, string cuser, string token, string devicetype)
        {
            Result<string> result = new();
            result.status = 2;
            EFormHead head = await _EFormHeadRepository.GetByNo(data.rno);
            data.formCode = head.formcode;
            data.company = head.company;
            EFormAuser eFormAuser = await _EFormAuserRepository.FindAsync(i => i.rno == data.rno);
            await _EFormAuserRepository.DeleteAsync(eFormAuser);
            head.SetStatus("R");
            head.SetStep(0);
            await _EFormHeadRepository.UpdateAsync(head);
            EFormSignlog eFormSignlog = new EFormSignlog()
            {
                rno = data.rno,
                seq = Convert.ToDecimal(eFormAuser.seq),
                aemplid = cuser,
                adate = DateTime.Now,
                aresult = "R",
                formcode = data.formCode,
                step = Convert.ToDecimal(eFormAuser.seq),
                company = data.company,
                aremark = data.remark
            };
            await _EFormSignlogRepository.InsertAsync(eFormSignlog);
            await _EFormPaperDomainService.RejectPaper(data.rno, cuser);
            await _BDInvoiceFolderRepository.CancelRno(data.rno);
            await _CashAmountRepository.Kickback(data.rno);
            result = await BPMReturn(data, cuser, token);
            await _InvoiceDomainService.UpdateInvoiceToUnrequested(data.rno, token);
            if (devicetype == "web")//电脑端驳回去抽单
            {
                await _mobileSignService.recalllForm(data.rno);
            }
            result.status = 1;
            return result;
        }
        /// <summary>
        /// 签核
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cuser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<string>> Approval(SignDto data, string cuser, string token, string devicetype)
        {
            Result<string> result = new();
            result.status = 2;
            var head = await _EFormHeadRepository.GetByNo(data.rno);
            data.formCode = head.formcode;
            data.company = head.company;
            Result<IList<SignForm>> signs = await _BPMDomainService.BPMQuerySign(data.rno, data.formCode, token);
            try
            {
                if (signs.status != 1)
                {
                    result.message = signs.message;
                    return result;
                }
                Result<bool> isSigned = _BPMDomainService.BPMQueryIsSigned(signs);
                if (isSigned.status != 1)
                {
                    result.message = isSigned.message;
                    return result;
                }
                if (isSigned.data)
                {
                    result.message = L["IsSigned"];
                    return result;
                }
                // 邀签
                if ((data.inviteMethod == 1 || data.inviteMethod == -1) && !string.IsNullOrEmpty(data.inviteUser))
                {
                    result = await BPMInviteSign(data, cuser, token);
                    if (result.status == 2) return result;
                    if (signs.data.Count > 0)
                    {
                        SignForm sign = signs.data.Where(i => i.status == "Current").FirstOrDefault();
                        EFormAuser eFormAuser = await _EFormAuserRepository.FindAsync(i => i.rno == data.rno);
                        eFormAuser.seq = sign.seq;
                        eFormAuser.cemplid = data.inviteUser;
                        await _EFormAuserRepository.UpdateAsync(eFormAuser);
                    }
                    //邀签以后抛手机签核
                    if (devicetype == "web")
                    {
                        //先抽单再抛单
                        await _mobileSignService.recalllForm(data.rno);
                    }
                    await _mobileSignService.SendMobileSignXMLData(data.rno);
                }
                else
                {
                    SignForm NextSign = signs.data.Where(i => i.status == "Not Approve" && i.signer_emplid != cuser).FirstOrDefault();
                    // paper
                    if ((new decimal[3] { 10, 11, 12 }).Contains(NextSign == null ? 0 : NextSign.seq) && data.paperSign)
                        await _EFormPaperDomainService.SignPaper(new List<string>() { data.rno }, cuser, true);
                    result = await BPMApproval(data, cuser, token, NextSign?.signer_emplid);
                    if (result.status == 2) return result;
                    EFormAuser eFormAuser = await _EFormAuserRepository.FindAsync(i => i.rno == data.rno);
                    EFormHead eFormHead = await _EFormHeadRepository.FindAsync(i => i.rno == data.rno);
                    EFormSignlog eFormSignlog = new EFormSignlog()
                    {
                        rno = data.rno,
                        seq = Convert.ToDecimal(eFormAuser.seq),
                        aemplid = cuser,
                        adate = DateTime.Now,
                        aresult = "A",
                        formcode = data.formCode,
                        step = Convert.ToDecimal(eFormAuser.seq),
                        company = data.company,
                        aremark = data.remark
                    };
                    if (data.detail != null && data.detail.Count > 0)
                    {
                        List<CashDetail> cashDetailList = await _CashDetailRepository.GetListAsync(i => i.rno == data.rno && data.detail.Select(i => i.seq).ToList().Contains(i.seq));
                        foreach (CashDetail item in cashDetailList)
                        {
                            var temp = data.detail.Where(i => i.seq == item.seq).First();
                            if (!string.IsNullOrEmpty(temp.assignment))
                                item.assignment = temp.assignment;
                            if (temp.taxexpense != null)
                                item.taxexpense = temp.taxexpense;
                        }
                    }
                    // 签核完
                    if (NextSign == null)
                    {
                        await _EFormAuserRepository.DeleteAsync(eFormAuser);
                        eFormHead.SetStatus("A");
                        eFormHead.SetStep(-1);
                        bool paperUnSigned = await _EFormPaperRepository.UnSigned(data.rno);
                        if (eFormHead.formcode != "CASH_3A" && !paperUnSigned)
                        {
                            try
                            {
                                await _AccountDomainService.SaveAccountInfo(data.rno, cuser);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Smart ERS 入账清单生成失败: {Rno}", data.rno);
                                _EmailHelper.SendEmail("Smart ERS 入账清单生成失败", "samsong_zhang@wistron.com", "wiwi_wei@wistron.com", data.rno + "，生成入账清单失败，请处理！" + ex.ToString());
                            }
                        }
                    }
                    else
                    {
                        // 未签完
                        eFormHead.SetStep(NextSign.seq);
                        eFormAuser.seq = NextSign.seq;
                        eFormAuser.cemplid = NextSign.signer_emplid;
                        await _EFormAuserRepository.UpdateAsync(eFormAuser);
                    }
                    await _EFormHeadRepository.UpdateAsync(eFormHead);
                    await _EFormSignlogRepository.InsertAsync(eFormSignlog);
                    //没签核完抛手机签核
                    if (NextSign != null)
                    {
                        if (devicetype == "web")
                        {
                            //先抽单再抛单
                            await _mobileSignService.recalllForm(data.rno);
                        }
                        await _mobileSignService.SendMobileSignXMLData(data.rno);
                    }
                    else
                    {
                        if (devicetype == "web")
                        {
                            //签核完抽单
                            await _mobileSignService.recalllForm(data.rno);
                        }
                    }
                }
                result.status = 1;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Smart ERS 签核失败: {Rno}, SignsData: {SignsData}", data.rno, signs.data.ToString());
                //_EmailHelper.SendEmail("Smart ERS 签核失败", "samsong_zhang@wistron.com", "wiwi_wei@wistron.com", data.rno + "，生成入账清单失败，请处理！" + e.ToString()+ signs.data);
            }
            return result;
        }
        /// <summary>
        /// BPM邀签api
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cuser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ERS.Common.BPMException"></exception>
        async Task<Result<string>> BPMInviteSign(SignDto data, string cuser, string token)
        {
            Result<string> result = new();
            result.status = 2;
            CreateSign sign = new();
            sign.SetFormCode(data.formCode);
            sign.SetRno(data.rno);
            sign.SetRemark(data.remark);
            sign.SetSignUser(cuser);
            sign.SetInvite(data.inviteMethod, data.inviteUser);
            await AddEmailContent(sign, data.company, cuser, data.rno, data.inviteUser);
            string datas = JsonConvert.SerializeObject(sign);
            HttpClient httpClient = _HttpClient.CreateClient();
            _logger.LogInformation("邀签入参： {Datas}", datas);
            BPMResult<string> response = await httpClient.PostHelperAsync<BPMResult<string>>(_configuration.GetSection("BPM:invitesign").Value, datas, token: token);
            if (response == null)
            {
                throw new ERS.Common.BPMException("BPM:邀签签核请求失败。 status: !200； data: " + datas);
            }
            else if (response.info == "success")
            {
                result.status = 1;
            }
            else
            {
                _logger.LogError("BPM:邀签签核请求失败。 status: {Info}; msg: {Msg}; data: {Datas}", response.info, response.msg, datas);
                throw new ERS.Common.BPMException("BPM:邀签签核请求失败。 status:" + response.info + "; " + response.msg);
            }
            return result;
        }
        /// <summary>
        /// BPM签核
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cuser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ERS.Common.BPMException"></exception>
        async Task<Result<string>> BPMApproval(SignDto data, string cuser, string token, string NextSign)
        {
            Result<string> result = new();
            result.status = 2;
            CreateSign sign = new();
            sign.SetFormCode(data.formCode);
            sign.SetRno(data.rno);
            sign.SetRemark(data.remark);
            sign.SetSignUser(cuser);
            await AddEmailContent(sign, data.company, cuser, data.rno,  NextSign);
            string datas = JsonConvert.SerializeObject(sign);
            HttpClient httpClient = _HttpClient.CreateClient();
            _logger.LogInformation("签核入参： {Datas}", datas);
            BPMResult<SignResult> response = await httpClient.PostHelperAsync<BPMResult<SignResult>>(_configuration.GetSection("BPM:approvalsign").Value, datas, token: token);
            if (response == null)
            {
                throw new ERS.Common.BPMException("BPM:同意签核请求失败。 status: !200； data: " + datas);
            }
            else if (response.info == "success")
            {
                result.status = 1;
            }
            else
            {
                _logger.LogError("BPM:同意签核请求失败。 status: {Info}; msg: {Msg}; data: {Datas}", response.info, response.msg, datas);
                throw new ERS.Common.BPMException("BPM:同意签核请求失败。 status:" + response.info + "; " + response.msg);
            }
            return result;
        }
        /// <summary>
        /// BPM驳回
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cuser"></param>
        /// <param name="token"></param>
        /// <param name="NextSign"></param>
        /// <returns></returns>
        /// <exception cref="ERS.Common.BPMException"></exception>
        async Task<Result<string>> BPMReturn(SignDto data, string cuser, string token)
        {
            Result<string> result = new();
            result.status = 2;
            CreateSign sign = new();
            sign.SetFormCode(data.formCode);
            sign.SetRno(data.rno);
            sign.SetRemark(data.remark);
            sign.SetSignUser(cuser);
            await AddRejectEmailContent(sign, data.company, cuser, data.rno, data.remark);
            string datas = JsonConvert.SerializeObject(sign);
            HttpClient httpClient = _HttpClient.CreateClient();
            _logger.LogInformation("驳回入参： {Datas}", datas);
            BPMResult<SignResult> response = await httpClient.PostHelperAsync<BPMResult<SignResult>>(_configuration.GetSection("BPM:returnsign").Value, datas, token: token);
            if (response == null)
            {
                throw new ERS.Common.BPMException("BPM:拒绝签核请求失败。 status: !200； data: " + datas);
            }
            else if (response.info == "success")
            {
                _logger.LogInformation("BPM驳回。入参： {Datas}。出参：{Response}", datas, JsonConvert.SerializeObject(response));
                result.status = 1;
            }
            else
            {
                _logger.LogError("BPM:拒绝签核请求失败。 status: {Info}; msg: {Msg}; data: {Datas}", response.info, response.msg, datas);
                throw new ERS.Common.BPMException("BPM:拒绝签核请求失败。 status:" + response.info + "; " + response.msg);
            }
            return result;
        }
        /// <summary>
        /// 产生ers的签核
        /// </summary>
        /// <param name="list"></param>
        /// <param name="invoiceAbnormal"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<string>> CreateSignSummary(CashHead list, bool invoiceAbnormal, string token)
        {
            Result<string> result = new();
            result.status = 1;
            if (!string.IsNullOrEmpty(_configuration.GetSection("UnitTest")?.Value)) return result;
            CreateSign data = new();
            IList<Signs> signs = new List<Signs>();
            IList<Signs> t_signs = new List<Signs>();
            Signs t_sign = new();
            // 延期预支
            if (list.formcode == "CASH_3A")
            {
                Result<List<Signs>> result1 = await GetDeferrChargeSign(list);
                if (result1.status != 1)
                {
                    result.message = result1.message;
                    return result;
                }
                signs = signs.Union(result1.data).ToList();
            }
            else if (list.formcode == "CASH_X")
            {
                var CashXSigns = await CreateCashXSigns(list, token);
                //List<int> removeIndexs = new();
                List<string> removeEmplids = new();
                var today = System.DateTime.Now;
                var expInfoList = (await _BDExpRepository.WithDetailsAsync()).Where(w => list.CashDetailList.Select(w => w.expcode).Contains(w.expcode) && w.category == 3 && (today > w.edate || today < w.sdate || String.IsNullOrEmpty(w.authorized) || String.IsNullOrEmpty(w.authorizer))).ToList();
                if (expInfoList.Count == 0)
                {
                    foreach (var item in list.CashDetailList.Select(w => w.expcode))
                    {
                        var expInfo = (await _BDExpRepository.WithDetailsAsync()).Where(w => w.expcode == item && w.category == 3 && w.company == list.company).FirstOrDefault();
                        if (expInfo != null)
                        {
                            var tempSign = CashXSigns.Where(w => w.emplid == expInfo.authorizer).FirstOrDefault();
                            if (tempSign != null && CashXSigns.Select(w => w.emplid).Contains(expInfo.authorized))
                            {
                                //removeIndexs.Add(CashXSigns.IndexOf(tempSign));
                                removeEmplids.Add(tempSign.emplid);
                            }
                        }
                    }
                    CashXSigns = CashXSigns.Where(w => !removeEmplids.Contains(w.emplid)).ToList();
                }
                if (CashXSigns.Count > 0) signs = CashXSigns;
            }
            else
            {
                t_signs = await AddAdvanceRequestSign(list);
                if (t_signs.Count > 0) signs = signs.Union(t_signs).ToList();
                t_sign = AddPayeeSign(list.cuser, list.payeeId);
                if (t_sign != null) signs.Add(t_sign);
                t_signs = await AddCostSignBefore(list);
                if (t_signs.Count > 0) signs = signs.Union(t_signs).ToList();
                t_signs = await AddFinanceSign(list, invoiceAbnormal);
                if (t_signs.Count > 0) signs = signs.Union(t_signs).ToList();
                t_signs = await AddDefinePermissionSign(list, token);
                if (t_signs.Count > 0) signs = signs.Union(t_signs).ToList();
                t_signs = await AddCostSignAfter(list);
                if (t_signs.Count > 0) signs = signs.Union(t_signs).ToList();
                //t_signs = await AddAuditSign(list);
                //if (t_signs.Count > 0) signs = signs.Union(t_signs).ToList();
            }
            data.signs = signs.Distinct().ToList();
            data.SetSignListCUser(list.cuser);
            data.SetFormCode(list.formcode);
            data.SetRno(list.rno);
            data.SetCurrApproval();
            await AddEmailContent(data, list.company, list.cuser, list.rno, NextSign: signs.Select(i => i.emplid).First(), firstCreate: true);
            result = await _BPMDomainService.BPMCreateSign(data, token);
            if (result.status != 1) return result;
            Result<IList<SignForm>> signResult = await _BPMDomainService.BPMQuerySign(list.rno, list.formcode, token);
            if (signResult.data.Count > 0)
            {
                IList<SignForm> signList = signResult.data.Where(i => i.sign_result != "Return").ToList();
                List<EFormAlist> eformalist = new List<EFormAlist>();
                EFormAuser auser = new();
                foreach (SignForm sign in signList)
                {
                    EFormAlist temp = new EFormAlist()
                    {
                        formcode = list.formcode,
                        rno = list.rno,
                        step = sign.seq,
                        cemplid = sign.signer_emplid,
                        status = sign.status == "Current" ? "C" : "P",
                        deptid = sign.deptid,
                        stepname = sign.step,
                        company = list.company,
                        cuser = list.cuser,
                        cdate = list.cdate,
                        seq = sign.sortNum
                    };
                    temp.SetCDate();
                    temp.SetCUser(list.cuser);
                    eformalist.Add(temp);
                    if (sign.status == "Current")
                    {
                        auser.formcode = list.formcode;
                        auser.rno = list.rno;
                        auser.cemplid = sign.signer_emplid;
                        auser.seq = sign.seq;
                        auser.company = list.company;
                        auser.SetCDate();
                        auser.SetCUser(list.cuser);
                        list.EFormHead.SetStep(sign.seq);
                    }
                }
                List<EFormAlist> eFormAlists = await _EFormAlistRepository.GetListAsync(i => i.rno == list.rno);
                if (eFormAlists.Count > 0) await _EFormAlistRepository.DeleteManyAsync(eFormAlists);
                await _EFormAlistRepository.InsertManyAsync(eformalist);
                await _EFormAuserRepository.InsertAsync(auser);
            }
            return result;
        }
        /// <summary>
        /// add课级
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        async Task<IList<Signs>> AddSectionSign(string deptid, int seq)
        {
            IList<Signs> result = new List<Signs>();
            string emplid = await (await _EmpOrgRepository.WithDetailsAsync()).Where(i => i.deptid == deptid && (i.tree_level_num == 8 || i.tree_level_num == 9)).AsNoTracking().Select(i => i.manager_id).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(emplid))
                result.Add(new Signs(emplid, "Section", seq));
            return result;
        }
        /// <summary>
        /// 预支延期申请流程
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        async Task<Result<List<Signs>>> GetDeferrChargeSign(CashHead list)
        {
            Result<List<Signs>> result = new();
            result.status = 2;
            int signLevel = 7;
            int maxday = list.CashDetailList.Max(i => i.delaydays);
            if (maxday > 30) signLevel = 5;
            List<EmpOrg> empOrgs = new();
            empOrgs = await GetEmpOrgs(empOrgs, list.deptid, signLevel);
            if (empOrgs.Count == 0)
            {
                result.message = "Not find approved officer(deferred sign)";
                return result;
            }
            List<Signs> signs = new();
            foreach (EmpOrg empOrg in empOrgs)
            {
                if (empOrg.tree_level_num == 7) signs.Add(new Signs(empOrg.manager_id, "Department", 1));
                if (empOrg.tree_level_num == 5) signs.Add(new Signs(empOrg.manager_id, "Division", 2));
            }
            foreach (var item in signs) item.SetCUser(list.cuser);
            result.status = 1;
            result.data = signs;
            return result;
            async Task<List<EmpOrg>> GetEmpOrgs(List<EmpOrg> orgList, string deptid, int signLevel)
            {
                EmpOrg _temp = await (await _EmpOrgRepository.WithDetailsAsync()).Where(i => i.deptid == deptid).AsNoTracking().FirstAsync();
                if (_temp.tree_level_num == 5 || _temp.tree_level_num == 7)
                    orgList.Add(_temp);
                if (_temp.tree_level_num > signLevel)
                    orgList = await GetEmpOrgs(orgList, _temp.uporg_code_a, signLevel);
                return orgList;
            }
        }
        /// <summary>
        /// 预支金申请人加签（报销别人的预支金时）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        async Task<IList<Signs>> AddAdvanceRequestSign(CashHead list)
        {
            IList<Signs> data = new List<Signs>();
            if (list.formcode != "CASH_1") return data;
            List<string> advanceRnoList = list.CashDetailList.Where(i => !string.IsNullOrEmpty(i.advancerno)).Select(i => i.advancerno).Distinct().ToList();
            if (advanceRnoList.Count == 0) return data;
            List<string> userList = await _CashHeadRepository.ReadCuserByRno(advanceRnoList);
            int seq = -10;
            foreach (string item in userList)
            {
                if (item != list.cuser)
                {
                    Signs _data = new();
                    _data.SetCurrSign(item, L["addAdvanceApplicant"], seq);
                    seq += 1;
                    _data.SetCUser(list.cuser);
                    data.Add(_data);
                }
            }
            return data;
        }
        /// <summary>
        /// 添加收款人
        /// </summary>
        /// <param name="cuser"></param>
        /// <param name="payeeid"></param>
        /// <returns></returns>
        public Signs AddPayeeSign(string cuser, string payeeid)
        {
            if (cuser == payeeid) return null;
            Signs data = new();
            data.SetCurrSign(payeeid, L["addPayeeStep"], 1);
            return data;
        }
        /// <summary>
        /// 添加申请人
        /// </summary>
        /// <param name="cuser"></param>
        /// <param name="payeeid"></param>
        /// <returns></returns>
        public Signs AddApplicantSign(string cuser, int seq)
        {
            Signs data = new();
            data.SetCurrSign(cuser, L["Applicant"], seq);
            return data;
        }
        /// <summary>
        /// 费用情景加签（前）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<IList<Signs>> AddCostSignBefore(CashHead list, int category = 1)
        {
            IList<Signs> data = new List<Signs>();
            if (list.formcode == "CASH_3A") return data;
            IList<string> expcodes = list.CashDetailList.Select(i => i.expcode).Distinct().ToList();
            if (expcodes.Count == 0) return data;
            // 走BG
            var exp = await _BDExpRepository.GetAddSignBefore(expcodes, list.company, category);
            if (exp == null || exp.Count == 0) return data;
            int seq = 2;
            foreach (var item in exp)
            {
                if (!string.IsNullOrEmpty(item.addsign) && !string.IsNullOrEmpty(item.addsignstep))
                {
                    Signs _data = new();
                    _data.SetCurrSign(item.addsign, item.addsignstep, seq);
                    seq += 1;
                    _data.SetCUser(list.cuser);
                    data.Add(_data);
                }
            }
            return data;
        }
        /// <summary>
        /// 财务加签
        /// </summary>
        /// <param name="list"></param>
        /// <param name="invoiceAbnormal"></param>
        /// <returns></returns>
        public async Task<IList<Signs>> AddFinanceSign(CashHead list, bool invoiceAbnormal)
        {
            IList<Signs> data = new List<Signs>();
            string deptid = "";
            // 取最大金额的deptid
            if (list.formcode == "CASH_1")
            {
                List<signCost> signCosts = new();
                List<departCost> _signCosts = new();
                foreach (var item in list.CashDetailList)
                {
                    List<departCost> res = JsonConvert.DeserializeObject<List<departCost>>(item.deptid);
                    _signCosts = _signCosts.Union(res).ToList();
                }
                List<string> depts = _signCosts.Select(i => i.deptId).Distinct().ToList();
                foreach (string dept in depts)
                    signCosts.Add(new signCost() { deptid = dept, cost = _signCosts.Where(i => i.deptId == dept).Sum(i => i.baseamount).Value });
                deptid = signCosts.OrderByDescending(i => i.cost).FirstOrDefault()?.deptid;
            }
            else
                deptid = list.CashDetailList.OrderByDescending(i => i.baseamt).FirstOrDefault()?.deptid;
            if (list.formcode == "CASH_5") deptid = "CASH_5";
            if (string.IsNullOrEmpty(deptid)) deptid = "*";
            if ((list.formcode == "CASH_2" && list.CashDetailList.Where(i => !string.IsNullOrEmpty(i.treattime)).Select(i => i.flag).FirstOrDefault() == 2) ||
                invoiceAbnormal || (list.formcode == "CASH_1" && await IsHaveReverseDocument(list.cuser)))
            {
                data = await GetFinance(deptid);
                foreach (var f in data)
                    f.SetCUser(list.cuser);
            }
            return data;
            async Task<IList<Signs>> GetFinance(string deptid)
            {
                IList<Signs> data = new List<Signs>();
                string plant = await _EmpOrgRepository.GetPlantByDeptid(deptid);
                var fins = await _FinreviewRepository.GetFinanceByPlant(list.company, plant);
                if (fins.Any(i => i.plant == plant))
                {
                    var tmp1 = fins.Where(i => i.plant == plant).FirstOrDefault();
                    if (tmp1.rv1 != "*" && !string.IsNullOrEmpty(tmp1.rv1))
                    {
                        await CheckFinIsLeftAndThrowException(tmp1.rv1);
                        data.Add(new Signs(tmp1.rv1, L["finance1"], 10));
                    }
                    if (tmp1.rv2 != "*" && !string.IsNullOrEmpty(tmp1.rv2))
                    {
                        await CheckFinIsLeftAndThrowException(tmp1.rv1);
                        data.Add(new Signs(tmp1.rv2, L["finance2"], 11));
                    }
                    if (tmp1.rv3 != "*" && !string.IsNullOrEmpty(tmp1.rv3))
                    {
                        await CheckFinIsLeftAndThrowException(tmp1.rv1);
                        data.Add(new Signs(tmp1.rv3, L["finance3"], 12));
                    }
                }
                else if (fins.Any(i => i.plant == "OTHERS"))
                {
                    var tmp1 = fins.Where(i => i.plant == "OTHERS").FirstOrDefault();
                    if (tmp1.rv1 != "*" && !string.IsNullOrEmpty(tmp1.rv1))
                    {
                        await CheckFinIsLeftAndThrowException(tmp1.rv1);
                        data.Add(new Signs(tmp1.rv1, L["finance1"], 10));
                    }
                    if (tmp1.rv2 != "*" && !string.IsNullOrEmpty(tmp1.rv2))
                    {
                        await CheckFinIsLeftAndThrowException(tmp1.rv1);
                        data.Add(new Signs(tmp1.rv2, L["finance2"], 11));
                    }
                    if (tmp1.rv3 != "*" && !string.IsNullOrEmpty(tmp1.rv3))
                    {
                        await CheckFinIsLeftAndThrowException(tmp1.rv1);
                        data.Add(new Signs(tmp1.rv3, L["finance3"], 12));
                    }
                }
                return data;
            }
        }
        async Task CheckFinIsLeftAndThrowException(string emplid)
        {
            if (!await _EmployeeRepository.CheckIsLeft(emplid))
                throw new Exception($"Finance({emplid}) has Left! Please contract your administrator!");
        }
        //判断申请人是否有预支金未冲账
        public async Task<bool> IsHaveReverseDocument(string cuser)
        {
            bool result = false;
            var efheads = (await _EFormHeadRepository.WithDetailsAsync()).Where(w => w.formcode == "CASH_3" && w.cuser == cuser && w.status == "A").Select(s => s.rno).AsNoTracking().ToList();
            var amountquery = (await _CashAmountRepository.WithDetailsAsync()).Where(w => efheads.Contains(w.rno) && w.actamt != 0).Select(x => new { x.rno, x.actamt, x.company }).AsNoTracking().ToList();
            var cashReturnQuery = (await _bdcashreturnRepository.WithDetailsAsync()).Where(w => amountquery.Select(i => i.rno).Contains(w.rno)).GroupBy(i => i.rno).Select(x => new { rno = x.Key, amount = x.Sum(i => i.amount) }).ToList();
            var aquery = (from aq in amountquery
                          join bcq in cashReturnQuery
                          on aq.rno equals bcq.rno
                          where aq.actamt == bcq.amount
                          select new
                          {
                              aq.rno
                          }).ToList();
            var query = amountquery.Where(x => !aquery.Any(a => x.rno == a.rno)).ToList();
            List<string> rnos = query.Select(x => x.rno).ToList();
            if (rnos.Count > 0)
            {
                result = true;
            }
            return result;
        }
        /// <summary>
        /// 核决权限加签
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<IList<Signs>> AddDefinePermissionSign(CashHead list, string token)
        {
            IList<Signs> data = new List<Signs>();
            List<signCost> signCosts = new();
            if (list.formcode != "CASH_1")
            {
                List<string> depts = list.CashDetailList.Select(i => i.deptid).Distinct().ToList();
                foreach (string dept in depts)
                {
                    if (list.formcode == "CASH_2" && list.CashDetailList.FirstOrDefault()?.flag != 0)
                        signCosts.Add(new signCost() { deptid = dept, cost = list.CashDetailList.Where(i => i.deptid == dept).Sum(i => i.amount).Value });
                    else
                        signCosts.Add(new signCost() { deptid = dept, cost = list.CashDetailList.Where(i => i.deptid == dept).Sum(i => i.baseamt).Value });
                }
            }
            else
            {
                List<departCost> _signCosts = new();
                foreach (var item in list.CashDetailList)
                {
                    List<departCost> res = JsonConvert.DeserializeObject<List<departCost>>(item.deptid);
                    _signCosts = _signCosts.Union(res).ToList();
                }
                List<string> depts = _signCosts.Select(i => i.deptId).Distinct().ToList();
                foreach (string dept in depts)
                    signCosts.Add(new signCost() { deptid = dept, cost = _signCosts.Where(i => i.deptId == dept).Sum(i => i.baseamount).Value });
            }
            int seq = 14;
            bool isFirst = true;
            foreach (signCost dept in signCosts)
            {
                if (isFirst)
                {
                    if (list.formcode == "CASH_3" && dept.deptid == list.deptid)
                    {
                        // Add specific logic for CASH_3 if needed
                    }
                    else if (list.formcode == "CASH_X" && dept.deptid == list.deptid)
                    {
                        // Add specific logic for CASH_X if needed
                    }
                    else
                    {
                        var _data = await AddSectionSign(list.deptid, seq);
                        if (_data.Count > 0)
                        {
                            data = data.Union(_data).ToList();
                            seq += 1;
                        }
                    }
                }
                isFirst = false;
                data = data.Union(await GetBPMSigns(list.payeeId, dept.cost, dept.deptid)).ToList();
            }
            return data;
            async Task<List<Signs>> GetBPMSigns(string payeeid, decimal? cost, string deptid, bool selectFirst = false)
            {
                List<Signs> data = new List<Signs>();
                DirectorSign para = new DirectorSign()
                {
                    project = list.dtype.Substring(0, 1),
                    project_num = list.dtype.Substring(1, list.dtype.Length - 1),
                    money = Convert.ToDecimal(cost),
                    currency = list.currency,
                    emplid = payeeid,
                    deptid = deptid,
                    key = list.company
                };
                string datas = JsonConvert.SerializeObject(para);
                HttpClient httpClient = _HttpClient.CreateClient();
                BPMResult<VerificationAuthorityResult> response = await httpClient.PostHelperAsync<BPMResult<VerificationAuthorityResult>>(_configuration.GetSection("BPM:directorsign").Value, datas, token: token);
                if (response == null)
                {
                    throw new ERS.Common.BPMException("BPM:获取核决权限请求失败。 status: !200； data: " + datas);
                }
                else if (response.info == "success")
                {
                    _logger.LogInformation("BPM核决权限。入参： {Datas}。出参：{Response}", datas, JsonConvert.SerializeObject(response));
                    foreach (var item in response.data?.signUsers)
                    {
                        Signs sign = await AddAuditSign(item.emplid, list.company, list.formcode, seq, list.cuser);
                        if (sign != null && !string.IsNullOrEmpty(sign.emplid))
                        {
                            data.Add(sign);
                            seq += 1;
                        }
                        Signs _data = new();
                        _data.SetCurrSign(item.emplid, item.levelDescA, seq);
                        _data.SetCUser(list.cuser);
                        data.Add(_data);
                        seq += 1;
                        if (selectFirst) break;
                    }
                }
                else
                {
                    _logger.LogError("BPM:获取核决权限请求失败。 status: {Info}; msg: {Msg}; data: {Datas}", response.info, response.msg, datas);
                    throw new ERS.Common.BPMException("BPM:获取核决权限请求失败。 status:" + response.info + ";  msg:" + response.msg);
                }
                return data;
            }
        }
        /// <summary>
        /// 费用情景加签（后）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<IList<Signs>> AddCostSignAfter(CashHead list, int category = 1)
        {
            IList<Signs> data = new List<Signs>();
            if (list.formcode == "CASH_3A") return data;
            IList<string> expcodes = list.CashDetailList.Select(i => i.expcode).Distinct().ToList();
            if (expcodes.Count == 0) return data;
            // BG
            var exp = await _BDExpRepository.GetAddSignAfter(expcodes, list.company, category);
            if (exp == null || exp.Count == 0) return data;
            int seq = 35;
            var temp = list.CashDetailList.Where(i => exp.Select(i => i.expcode).ToList().Contains(i.expcode)).ToList();
            var depts = new List<string>();
            if (list.formcode == "CASH_1")
            {
                List<departCost> _signCosts = new();
                foreach (var item in list.CashDetailList)
                {
                    List<departCost> res = JsonConvert.DeserializeObject<List<departCost>>(item.deptid);
                    _signCosts = _signCosts.Union(res).ToList();
                }
                depts = _signCosts.Select(i => i.deptId).Distinct().ToList();
            }
            else
                depts = temp.Select(i => i.deptid).Distinct().ToList();
            foreach (var item in depts)
            {
                string emplid = await _EmpOrgRepository.GetBGByDeptid(item);
                Signs _data = new();
                _data.SetCurrSign(emplid, exp[0].addsignstep, seq);
                seq += 1;
                _data.SetCUser(list.cuser);
                data.Add(_data);
            }
            return data;
        }
        /// <summary>
        /// audit邀签加签
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        async Task<Signs> AddAuditSign(string user, string company, string formcode, int step, string cuser)
        {
            Signs _data = new();
            string emplid = await _EFormAuditRepository.GetAudit(user, company, formcode);
            if (!string.IsNullOrEmpty(emplid))
            {
                _data.SetCurrSign(emplid, "Audit", step);
                _data.SetCUser(cuser);
            }
            return _data;
        }
        async Task<IList<string>> GetReplaceMail(string company) => await _AppConfigRepository.GetReplaceMailBySite(company);
        /// <summary>
        /// 获取邮件通知人
        /// </summary>
        /// <param name="data"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        async Task AddMailTo(CreateSign data, string mailtouser, string company)
        {
            IList<string> mails = await GetReplaceMail(company);
            if (mails.Count > 0)
            {
                data.SetMailTo(mails);
                return;
            }
            data.SetMailTo(new List<string>() { mailtouser });
        }
        /// <summary>
        /// 添加邮件内容
        /// </summary>
        /// <param name="data"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        async Task AddEmailContent(CreateSign data, string company, string cuser, string rno, string NextSign = "", bool firstCreate = false)
        {
            Mailtemplate template = null;
            bool finishToSign = false;
            string currUser = "";
            string efUser = (await _EFormHeadRepository.WithDetailsAsync()).Where(i => i.rno == rno).Select(i => i.cuser).AsNoTracking().FirstOrDefault();
            string formName = null;
            string cdate = null;
            if (!string.IsNullOrEmpty(efUser))
            {
                cuser = efUser;
                EFormHead eFormHead = (await _EFormHeadRepository.WithDetailsAsync()).Where(i => i.rno == rno).AsNoTracking().FirstOrDefault();
                cdate = eFormHead.cdate?.ToString("yyyy-MM-dd") ?? new DateTime().ToString("yyyy-MM-dd");
                formName = (await _BDFormRepository.WithDetailsAsync()).Where(i => i.FormCode == eFormHead.formcode).Select(i => i.FormName).AsNoTracking().FirstOrDefault();
            }
            else
            {
                cdate = new DateTime().ToString("yyyy-MM-dd");
                formName = (await _BDFormRepository.WithDetailsAsync()).Where(i => i.FormCode == data.formCode).Select(i => i.FormName).AsNoTracking().FirstOrDefault();
            }
            if (string.IsNullOrEmpty(NextSign) && !firstCreate)
            {
                template = await _MailtemplateRepository.GetFinishApply(company);
                NextSign = data.userid;
                finishToSign = true;
            }
            else
            {
                template = await _MailtemplateRepository.GetNextApply(company);
                if (!firstCreate)
                {
                    string tempCurrUser = (await _IEmployeeDomainService.QueryEmployeeAsync(data.userid))?.ename;
                    currUser = !String.IsNullOrEmpty(tempCurrUser) ? tempCurrUser : data.userid;
                }
            }
            Employee approval = await _IEmployeeDomainService.QueryEmployeeAsync(NextSign);
            Employee applicant = await _IEmployeeDomainService.QueryEmployeeAsync(cuser);
            if (template == null) return;
            string subject = string.Format(template.subject, formName, rno);
            string content = "";
            if (finishToSign)
                content = string.Format(template.mailmsg, applicant?.ename, rno, approval?.ename, _Configuration.GetSection("SignPage").Value);
            else
                content = string.Format(template.mailmsg, approval?.ename, rno, formName, cdate, applicant?.ename, _Configuration.GetSection("SignPage").Value);
            data.SetMailSubject(subject);
            data.SetMailMsg(content);
            await AddMailTo(data, finishToSign ? cuser : NextSign, company);
        }
        async Task AddRejectEmailContent(CreateSign data, string company, string cuser, string rno, string remark = "")
        {
            Mailtemplate template = await _MailtemplateRepository.GetRejectApply(company);
            string applyuser = (await _EFormHeadRepository.WithDetailsAsync()).Where(i => i.rno == rno).Select(i => i.cuser).AsNoTracking().First();
            Employee approval = await _IEmployeeDomainService.QueryEmployeeAsync(cuser);
            Employee applicant = await _IEmployeeDomainService.QueryEmployeeAsync(applyuser);
            string subject = string.Format(template.subject, applicant.ename);
            string content = string.Format(template.mailmsg, applicant.ename, approval.ename, rno, remark, _Configuration.GetSection("FrontDomain").Value);
            data.SetMailSubject(subject);
            data.SetMailMsg(content);
            await AddMailTo(data, applyuser, company);
        }
        /// <summary>
        /// 获取某单的签核历史
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        public async Task<Result<SignProcessAndLogDto>> GetSignLogs(string rno, string token)
        {
            Result<SignProcessAndLogDto> result = new();
            if (_configuration.GetSection("UnitTest").Value == "true")
                return result;
            SignProcessAndLogDto signs = new();
            result.status = 2;
            var head = await _EFormHeadRepository.GetByNo(rno);
            Result<IList<SignForm>> data = await _BPMDomainService.BPMQuerySign(rno, head.formcode, token);
            if (data.status == 2)
            {
                result.message = data.message;
                result.status = 1;
                return result;
            }
            // 方式一（直接用BPM的）
            List<SignlogDto> signlogs = _objectMapper.Map<List<SignForm>, List<SignlogDto>>(data.data.Where(i => i.status != "Current" && i.status != "Not Approve").ToList());
            List<AlistDto> signprocess = _objectMapper.Map<List<SignForm>, List<AlistDto>>(data.data.ToList());
            foreach (var item in signprocess)
                item.company = head.company;
            signs.signLog = signlogs;
            signs.signProcess = signprocess;
            result.data = signs;
            result.status = 1;
            return result;
        }
        /// <summary>
        /// 获取某单的签核历史
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task<Result<List<SignlogDto>>> QueryHistoricalSignRecords(string rno)
        {
            Result<List<SignlogDto>> result = new();
            var signlogquery = await _EFormSignlogRepository.GetListByRno(rno);
            var results = _objectMapper.Map<List<EFormSignlog>, List<SignlogDto>>(signlogquery);
            result.data = results;
            return result;
        }
        /// <summary>
        /// 获取某单的签核流程
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        public async Task<Result<List<AlistDto>>> QuerySignedData(string rno)
        {
            Result<List<AlistDto>> result = new();
            var alistquery = (await _EFormAlistRepository.GetListByRno(rno));
            var results = _objectMapper.Map<List<EFormAlist>, List<AlistDto>>(alistquery);
            var cnames = (await _EmployeeRepository.WithDetailsAsync()).Where(w => results.Select(w => w.cemplid).Contains(w.emplid)).Select(w => new { w.cname, w.emplid }).ToList();
            foreach (var item in results)
            {
                string cname = cnames.Where(w => w.emplid == item.cemplid).Select(w => w.cname).FirstOrDefault();
                if (!string.IsNullOrEmpty(cname))
                {
                    item.approval = item.cemplid + @"/" + cname;
                }
            }
            result.data = results;
            return result;
        }
        /// <summary>
        /// 根据当前登录人获取待签的单子api（分页），按申请时间升序排序
        /// </summary>
        /// <param name="parameters">parameters.data = userId</param>
        /// <returns></returns>
        public async Task<Result<List<ToBeSignedDto>>> QuerySignedFormByUserId(Request<string> parameters, string userId)
        {
            Result<List<ToBeSignedDto>> result = new Result<List<ToBeSignedDto>>()
            {
                data = new List<ToBeSignedDto>()
            };
            if (userId == null || userId.Length == 0)
            {
                result.data = null;
                result.status = 2;
                result.message = L["NotHaveCemplid"];
                return result;
            }
            else
            {
                int pageIndex = parameters.pageIndex;
                int pageSize = parameters.pageSize;
                if (pageIndex < 1)
                    pageIndex = 1;
                if (pageSize < 10)
                    pageSize = 10;
                 //userId = "10710047";
                //auser表查询当前登录人是否有可签单据
                //var ausersQuery = (await _ApprovalAssignedApproverRepository.WithDetailsAsync()).Where(w => w.approver_emplid == userId).OrderBy(w => w.cdate).AsTracking().ToList();
                var ausersQuery = (from flow in (await _ApprovalFlowRepository.WithDetailsAsync()) 
                                   join approver in (await _ApprovalAssignedApproverRepository.WithDetailsAsync()) 
                                   on flow.Id equals approver.flow_id 
                                   where approver.approver_emplid == userId 
                                   && flow.status == "pending_approval" 
                                   orderby approver.flow_id descending 
                                   select new
                                   {
                                       Id = flow.Id,
                                       approver_emplid = approver.approver_emplid,
                                       flow_status = flow.status,
                                       flow_cdate = flow.cdate,
                                       rno = flow.rno,
                                       step = flow.step,
                                       stename = flow.stepname,
                                   }).ToList();



                result.total = ausersQuery.Count;
                if (ausersQuery.Count == 0)
                {
                    return result;
                }

                //A：E_FORM_HEAD
                var existQuery = (from ausers in ausersQuery
                                  join cashheads in (await _CashHeadRepository.WithDetailsAsync())
                                  on ausers.rno equals cashheads.rno
                                  select new
                                  {
                                      rno = cashheads.rno,
                                      formcode = cashheads.formcode,
                                      cuser = cashheads.cuser,
                                      cdate = cashheads.cdate,
                                      cname = cashheads.cname,
                                      step = ausers.step,
                                      nemplid = ausers.approver_emplid,
                                      status = cashheads.status,
                                      cemplid = ausers.approver_emplid,
                                      currency= cashheads.currency,
                                      actamt = cashheads.actualamount,
                                  }).Where(w => w.status == "pending_approval" && w.step > 0).OrderBy(w => w.cdate).ToList();
                if (existQuery.Count > 0)
                {
                    List<ToBeSignedDto> resultQuery = new();
                    //B：E_FORM 获取表单类型名
                    var eforms = await _BDFormRepository.GetListAsync();
                    //C：e_Form_Alist
                    var alists = (await _EFormAlistRepository.WithDetailsAsync()).Where(w => existQuery.Select(w => w.rno).Contains(w.rno)).AsNoTracking().ToList();
                    //D：cash_head
                    var cashheads = (await _CashHeadRepository.WithDetailsAsync()).Where(w => existQuery.Select(w => w.rno).Contains(w.rno)).AsNoTracking().ToList();
                    //E：cash_amount
                    var cashamounts = (await _CashAmountRepository.WithDetailsAsync()).Where(w => existQuery.Select(w => w.rno).Contains(w.rno)).AsNoTracking().ToList();
                    //select distinct t.rno,t.expname,t.form_code from cash_detail t
                    var aQuery = (await _CashDetailRepository.WithDetailsAsync()).Where(w => existQuery.Select(w => w.rno).Contains(w.rno)).Select(w => new { w.rno, w.expname, w.formcode }).Distinct().AsNoTracking().ToList();
                    // select distinct LISTAGG(a.expname,',') within group (order by a.expname) over(partition by a.rno) expname,a.rno,a.form_code
                    //        from (select distinct t.rno,t.expname,t.form_code from cash_detail t) a
                    var FQuery = (from aq in aQuery
                                  group aq by new { aq.rno } into model
                                  select new
                                  {
                                      expname = string.Join(",", model.OrderBy(w => w.expname).Select(w => w.expname)),
                                      rno = model.FirstOrDefault().rno,
                                      formcode = model.FirstOrDefault().formcode
                                  }).ToList();
                    foreach (var item in existQuery)
                    {
                        ToBeSignedDto toBeSignedDto = new ToBeSignedDto();
                        toBeSignedDto.rno = item.rno;
                        toBeSignedDto.formcode = item.formcode;
                        toBeSignedDto.company = cashheads.Where(w => w.rno == item.rno).FirstOrDefault().company;
                        toBeSignedDto.formname = eforms.Where(w => w.FormCode == item.formcode).FirstOrDefault().FormName;
                        toBeSignedDto.apid = eforms.Where(w => w.FormCode == item.formcode).FirstOrDefault().SignMenuKey;
                        toBeSignedDto.expname = FQuery.Where(w => w.rno == item.rno).FirstOrDefault().expname;
                        if (item.formcode == "CASH_3A"|| item.formcode == "CASH_UBER")
                        {
                            var detailQuery = (await _CashDetailRepository.WithDetailsAsync()).Where(w => w.rno == item.rno).Select(w => new { currency = w.currency, actamt = w.amount1 });
                            toBeSignedDto.currency = detailQuery.Select(w => w.currency).FirstOrDefault();
                            toBeSignedDto.actamt = detailQuery.Select(w => w.actamt).FirstOrDefault();
                        }
                        else
                        {
                            toBeSignedDto.currency = cashamounts .Where(w => w.rno == item.rno) .FirstOrDefault()?.currency ?? "";
                            toBeSignedDto.actamt = cashamounts.Where(w => w.rno == item.rno).FirstOrDefault()?.actamt?? 0;
                        }
                        toBeSignedDto.cuser = item.cuser;
                        toBeSignedDto.cdate = item.cdate;
                        toBeSignedDto.cname = item.cname;
                        toBeSignedDto.step = item.step;
                        toBeSignedDto.formcategory = item.rno.ToString().Substring(0, 1) == "N" ? "電子發票" : (item.rno.ToString().Substring(0, 1) == "H" ? "其他發票" : eforms.Where(w => w.FormCode == item.formcode).FirstOrDefault().FormName);
                        toBeSignedDto.stepname = alists.Where(w => w.rno == item.rno && w.cemplid == item.nemplid && w.step == item.step).FirstOrDefault()?.stepname;
                        if (string.IsNullOrEmpty(toBeSignedDto.stepname)) toBeSignedDto.stepname = "Invite";
                        resultQuery.Add(toBeSignedDto);
                    }
                    result.total = resultQuery.Count;
                    result.data = resultQuery.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    return result;
                }
                else
                {
                    result.data = null;
                    result.message = L["NoRequiredSignedDoc"];
                    result.status = 2;
                    return result;
                }
            }
        }
        /// <summary>
        /// 获取个人待签核单据（NEW）
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<List<ToBeSignedDto>>> QuerySignedFormByUserId(Request<string> parameters, string userId, string token)
        {
            Result<List<ToBeSignedDto>> result = new Result<List<ToBeSignedDto>>()
            {
                data = new List<ToBeSignedDto>()
            };
            if (userId == null || userId.Length == 0)
            {
                result.data = null;
                result.status = 2;
                result.message = L["NotHaveCemplid"];
                return result;
            }
            else
            {
                int pageIndex = parameters.pageIndex;
                int pageSize = parameters.pageSize;
                if (pageIndex < 1)
                    pageIndex = 1;
                if (pageSize < 10)
                    pageSize = 10;
                var ausersQuery = await _BPMDomainService.GetBPMSelfPendingForm(userId, token);
                if (ausersQuery.status == 1 && ausersQuery.data.Count == 0)
                {
                    return result;
                }
                var pendingSignList = ausersQuery.data;
                List<string> pendingSignRno = pendingSignList.Select(i => i.formno).ToList();
                var existQuery = await (await _CashHeadRepository.WithDetailsAsync()).Where(i => pendingSignRno.Contains(i.rno)).OrderBy(i => i.cdate).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
                if (existQuery.Count > 0)
                {
                    List<ToBeSignedDto> resultQuery = new();
                    //B：E_FORM 获取表单类型名
                    var eforms = await _BDFormRepository.GetListAsync();
                    //E：cash_amount
                    var cashamounts = (await _CashAmountRepository.WithDetailsAsync()).Where(w => existQuery.Select(w => w.rno).Contains(w.rno)).AsNoTracking().ToList();
                    //select distinct t.rno,t.expname,t.form_code from cash_detail t
                    var aQuery = (await _CashDetailRepository.WithDetailsAsync()).Where(w => existQuery.Select(w => w.rno).Contains(w.rno)).Select(w => new { w.rno, w.expname, w.formcode }).Distinct().AsNoTracking().ToList();
                    var FQuery = (from aq in aQuery
                                  group aq by new { aq.rno } into model
                                  select new
                                  {
                                      expname = string.Join(",", model.OrderBy(w => w.expname).Select(w => w.expname)),
                                      rno = model.FirstOrDefault().rno,
                                      formcode = model.FirstOrDefault().formcode
                                  }).ToList();
                    foreach (var item in existQuery)
                    {
                        ToBeSignedDto toBeSignedDto = new ToBeSignedDto();
                        toBeSignedDto.rno = item.rno;
                        toBeSignedDto.formcode = item.formcode;
                        toBeSignedDto.company = item.company;
                        toBeSignedDto.formname = eforms.Where(w => w.FormCode == item.formcode).FirstOrDefault().FormName;
                        toBeSignedDto.expname = FQuery.Where(w => w.rno == item.rno).FirstOrDefault().expname;
                        if (item.formcode == "CASH_3A")
                        {
                            var detailQuery = (await _CashDetailRepository.WithDetailsAsync()).Where(w => w.rno == item.rno).Select(w => new { currency = w.currency, actamt = w.amount1 });
                            toBeSignedDto.currency = detailQuery.Select(w => w.currency).FirstOrDefault();
                            toBeSignedDto.actamt = detailQuery.Select(w => w.actamt).FirstOrDefault();
                        }
                        else
                        {
                            toBeSignedDto.currency = cashamounts.Where(w => w.rno == item.rno).FirstOrDefault().currency;
                            toBeSignedDto.actamt = cashamounts.Where(w => w.rno == item.rno).FirstOrDefault().actamt;
                        }
                        toBeSignedDto.cuser = item.cuser;
                        toBeSignedDto.cdate = item.cdate;
                        toBeSignedDto.cname = item.cname;
                        //toBeSignedDto.step = item.step;
                        toBeSignedDto.formcategory = item.rno.ToString().Substring(0, 1) == "N" ? "電子發票" : (item.rno.ToString().Substring(0, 1) == "H" ? "其他發票" : eforms.Where(w => w.FormCode == item.formcode).FirstOrDefault().FormName);
                        toBeSignedDto.stepname = pendingSignList.Where(w => w.formno == item.rno).FirstOrDefault()?.step;
                        if (string.IsNullOrEmpty(toBeSignedDto.stepname)) toBeSignedDto.stepname = "Invite";
                        //toBeSignedDto.apid = item.apid;
                        resultQuery.Add(toBeSignedDto);
                    }
                    result.total = pendingSignRno.Count;
                    result.data = resultQuery.ToList();
                    return result;
                }
                else
                {
                    result.data = null;
                    result.message = L["NoRequiredSignedDoc"];
                    result.status = 2;
                    return result;
                }
            }
        }
        //当前登录人工号，判断这个人是不是会计。 finreview存在这个人返回TRUE
        public async Task<Result<bool>> IsAccountantOrNot(string user)
        {
            Result<bool> result = new Result<bool>();
            Finreview finData = (await _finreviewRepository.WithDetailsAsync()).Where(i => i.rv1 == user || i.rv2 == user).FirstOrDefault();
            if(finData != null)
            {
                result.data = true;
            }
            else
            {
                result.data = false;
            }
            return result;
        }
        /// <summary>
        /// 判断当前签核人是否是申请人
        /// </summary>
        /// <param name="rno"></param>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<int>> IsApplicant(string rno, string user, string token)
        {
            Result<int> result = new Result<int>();
            int _result = 0;
            var head = await _EFormHeadRepository.GetByNo(rno);
            if (head == null) return result;
            if (head.cuser != user) return result;
            Result<IList<SignForm>> data = await _BPMDomainService.BPMQuerySign(rno, head.formcode, token);
            var temp = data.data;
            if (temp == null)
                _result = 0;
            else if (temp.Any(w => w.status == "Current" && w.emplid == user))
                _result = 1;
            result.data = _result;
            return result;
        }
        public async Task<IList<Signs>> CreateCashXSigns(CashHead list, string token)
        {
            IList<Signs> signs = new List<Signs>();
            IList<Signs> t_signs = new List<Signs>();
            Signs t_sign = new();
            t_signs = await AddCostSignBefore(list, 3);
            if (t_signs.Count > 0) signs = signs.Union(t_signs).ToList();
            t_signs = await AddDefinePermissionSign(list, token);
            if (t_signs.Count > 0) signs = signs.Union(t_signs).ToList();
            t_sign = AddApplicantSign(list.cuser, signs.Max(w => w.seq) + 1);
            if (t_sign != null) signs.Add(t_sign);
            t_signs = await AddCashXFinanceSign(list.CashDetailList.FirstOrDefault().companycode, 50);
            if (t_signs.Count > 0) signs = signs.Union(t_signs).ToList();
            t_signs = await AddCostSignAfter(list, 3);
            if (t_signs.Count > 0) signs = signs.Union(t_signs).ToList();
            return signs;
        }
        /// <summary>
        /// 薪资请款财务加签
        /// </summary>
        /// <param name="companycode">公司别代码</param>
        /// <param name="currseq">当前签核步骤</param>
        /// <returns></returns>
        public async Task<IList<Signs>> AddCashXFinanceSign(string companycode, int currseq)
        {
            IList<Signs> data = new List<Signs>();
            var fins = await _FinreviewRepository.GetCashXFinanceByCompany(companycode);
            if (fins.Any())
            {
                var tmp1 = fins.FirstOrDefault();
                if (tmp1.rv1 != "*" && !string.IsNullOrEmpty(tmp1.rv1))
                {
                    data.Add(new Signs(tmp1.rv1, L["finance1"], currseq + 1));
                    await CheckFinIsLeftAndThrowException(tmp1.rv1);
                }
                if (tmp1.rv2 != "*" && !string.IsNullOrEmpty(tmp1.rv2))
                {
                    data.Add(new Signs(tmp1.rv2, L["finance2"], currseq + 2));
                    await CheckFinIsLeftAndThrowException(tmp1.rv2);
                }
                if (tmp1.rv3 != "*" && !string.IsNullOrEmpty(tmp1.rv3))
                {
                    data.Add(new Signs(tmp1.rv3, L["finance3"], currseq + 3));
                    await CheckFinIsLeftAndThrowException(tmp1.rv3);
                }
            }
            return data;
        }
        /// <summary>
        /// 薪资请款签核 额外添加保存附件及实际金额
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cuser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<string>> CashXApproval(IFormCollection formCollection, string cuser, string token, string devicetype)
        {
            string signData = formCollection["sign"];
            string fileData = formCollection["file"];
            SignDto data = JsonConvert.DeserializeObject<SignDto>(signData);
            IList<CashFile> reuploadFiles = null;
            if (!String.IsNullOrEmpty(fileData))
            {
                var fData = JsonConvert.DeserializeObject<IList<CashFileDto>>(fileData);
                reuploadFiles = _objectMapper.Map<IList<CashFileDto>, IList<CashFile>>(fData);
            }
            Result<string> result = new();
            result.status = 2;
            var head = await _EFormHeadRepository.GetByNo(data.rno);
            data.formCode = head.formcode;
            data.company = head.company;
            Result<IList<SignForm>> signs = await _BPMDomainService.BPMQuerySign(data.rno, data.formCode, token);
            if (signs.status != 1)
            {
                result.message = signs.message;
                return result;
            }
            // 邀签
            if ((data.inviteMethod == 1 || data.inviteMethod == -1) && !string.IsNullOrEmpty(data.inviteUser))
            {
                result = await BPMInviteSign(data, cuser, token);
                if (result.status == 2) return result;
                if (signs.data.Count > 0)
                {
                    SignForm sign = signs.data.Where(i => i.status == "Current").FirstOrDefault();
                    EFormAuser eFormAuser = await _EFormAuserRepository.FindAsync(i => i.rno == data.rno);
                    eFormAuser.seq = sign.seq;
                    eFormAuser.cemplid = data.inviteUser;
                    await _EFormAuserRepository.UpdateAsync(eFormAuser);
                }
            }
            else
            {
                SignForm NextSign = signs.data.Where(i => i.status == "Not Approve" && i.signer_emplid != cuser).FirstOrDefault();
                SignForm CurrentSign = signs.data.Where(w => w.status == "Current").FirstOrDefault();
                result = await BPMApproval(data, cuser, token, NextSign?.signer_emplid);
                if (result.status == 2) return result;
                EFormAuser eFormAuser = await _EFormAuserRepository.FindAsync(i => i.rno == data.rno);
                EFormHead eFormHead = await _EFormHeadRepository.FindAsync(i => i.rno == data.rno);
                CashHead cashHead = await _CashHeadRepository.FindAsync(w => w.rno == data.rno);
                EFormSignlog eFormSignlog = new EFormSignlog()
                {
                    rno = data.rno,
                    seq = Convert.ToDecimal(eFormAuser.seq),
                    aemplid = cuser,
                    adate = DateTime.Now,
                    aresult = "A",
                    formcode = data.formCode,
                    step = Convert.ToDecimal(eFormAuser.seq),
                    company = data.company,
                    aremark = data.remark
                };
                if (data.detail != null && data.detail.Count > 0)
                {
                    List<CashDetail> cashDetailList = await _CashDetailRepository.GetListAsync(i => i.rno == data.rno && data.detail.Select(i => i.seq).ToList().Contains(i.seq));
                    foreach (CashDetail item in cashDetailList)
                    {
                        var temp = data.detail.FirstOrDefault(i => i.seq == item.seq);
                        if (temp != null)
                            item.amount = temp.amount;
                    }
                }
                if (reuploadFiles != null && reuploadFiles.Count > 0)
                {
                    List<CashFile> removeFiles = await _CashFileRepository.GetListAsync(w => w.rno == data.rno && w.ishead == "Y");
                    if (removeFiles.Count > 0)
                    {
                        await _CashFileRepository.DeleteManyAsync(removeFiles);
                    }
                    await SaveReuploadFile(formCollection, reuploadFiles);
                    if (reuploadFiles != null && reuploadFiles.Count > 0)
                    {
                        foreach (var file in reuploadFiles)
                        {
                            file.SetRno(head.rno);
                            file.SetCompany(head.company);
                            file.SetFormCode(head.formcode);
                            file.cuser = cuser;
                        }
                        await _CashFileRepository.InsertManyAsync(reuploadFiles);
                    }
                }
                if (data.amount != null && data.amount.actamt > 0)
                {
                    CashAmount cashAmount = await _CashAmountRepository.FindAsync(w => w.rno == data.rno);
                    cashAmount.actamt = data.amount.actamt;
                }
                // 签核完
                if (NextSign == null)
                {
                    await _EFormAuserRepository.DeleteAsync(eFormAuser);
                    eFormHead.SetStatus("A");
                    eFormHead.SetStep(-1);
                    cashHead.SetPayment(System.DateTime.Now);
                    cashHead.stat = "Y";
                }
                else
                {
                    //會計初審一簽完生成入賬信息
                    if (NextSign.seq == 52)
                    {
                        await _AccountDomainService.SaveCashXAccInfo(head.rno, cuser);
                    }
                    // 未签完
                    eFormHead.SetStep(NextSign.seq);
                    eFormAuser.seq = NextSign.seq;
                    eFormAuser.cemplid = NextSign.signer_emplid;
                    await _EFormAuserRepository.UpdateAsync(eFormAuser);
                }
                await _EFormHeadRepository.UpdateAsync(eFormHead);
                await _CashHeadRepository.UpdateAsync(cashHead);
                await _EFormSignlogRepository.InsertAsync(eFormSignlog);
                //没签核完抛手机签核
                if (NextSign != null)
                {
                    if (devicetype == "web")
                    {
                        //先抽单再抛单
                        await _mobileSignService.recalllForm(data.rno);
                    }
                    await _mobileSignService.SendMobileSignXMLData(data.rno);
                }
                else
                {
                    if (devicetype == "web")
                    {
                        //签核完抽单
                        await _mobileSignService.recalllForm(data.rno);
                    }
                }
            }
            result.status = 1;
            return result;
        }
        /// <summary>
        /// 重新上传附件
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="cashFiles"></param>
        /// <returns></returns>
        async Task SaveReuploadFile(IFormCollection formCollection, IList<CashFile> cashFiles)
        {
            foreach (var file in formCollection.Files)
            {
                string path = "";
                string type = "";
                if (!string.IsNullOrEmpty(_Configuration.GetSection("UnitTest")?.Value))
                    type = "image/jpeg";
                else
                    type = file.ContentType;
                //using (var steam = file.OpenReadStream())
                    //todo  加入区域区分
                    //path = await _MinioDomainService.PutObjectAsync(cashFiles.First().rno, file.FileName, steam, type);
                var temp = cashFiles.Where(i => i.key == file.Name).FirstOrDefault();
                if (temp != null) temp.SetSavePath(path);
                if (temp != null) temp.SetOriginalFileName(file.FileName);
            }
        }
    }
}

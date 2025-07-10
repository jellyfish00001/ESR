using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.IDomainServices;
using ERS.IRepositories;
using Volo.Abp.Application.Services;
using ERS.Localization;
using ERS.DTO.MobileSign;
using ERS.DTO.Application;
using System.Xml.Linq;
using ERS.Domain.IDomainServices;
using ERS.DTO;
using ERS.Model;
using NUglify.Helpers;
using System.IO;
using ERS.Model.MobileSign;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using ERS.Entities;
using Microsoft.Extensions.Logging;
using ERS.Minio;
namespace ERS.Services.MobileSign
{
    public class MobileSignService : ApplicationService, IMobileSignService
    {
        private readonly IApplicationDomainService _applicationDomainService;
        private readonly IBPMDomainService _BPMDomainService;
        private readonly IEFormHeadRepository _eformheadRepository;
        private readonly IBDFormRepository _bdformRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _HttpClient;
        private readonly ISendMobileLogRepository _sendMobileLogRepository;
        private readonly ISendMobileFileLogRepository _sendMobileFileLogRepository;
        private readonly IAppConfigRepository _appConfigRepository;
        private readonly IMinioRepository _minioRepository;
        private readonly ILogger<MobileSignService> _logger;
        private readonly Common.EmailHelper _EmailHelper;
        public MobileSignService(
            IAppConfigRepository appConfigRepository,
            ISendMobileLogRepository sendMobileLogRepository,
            ISendMobileFileLogRepository sendMobileFileLogRepository,
            ILogger<MobileSignService> logger,
            IHttpClientFactory HttpClient,
            IConfiguration configuration,
            IEFormHeadRepository eformheadRepository,
            IBDFormRepository bdformRepository,
            IApplicationDomainService applicationDomainService,
            IBPMDomainService BPMDomainService,
            IMinioRepository minioRepository,
            Common.EmailHelper emailHelper)
        {
            _sendMobileLogRepository = sendMobileLogRepository;
            _sendMobileFileLogRepository = sendMobileFileLogRepository;
            _HttpClient = HttpClient;
            _eformheadRepository = eformheadRepository;
            LocalizationResource = typeof(ERSResource);
            _applicationDomainService = applicationDomainService;
            _BPMDomainService = BPMDomainService;
            _bdformRepository = bdformRepository;
            _configuration = configuration;
            _logger = logger;
            _appConfigRepository = appConfigRepository;
            _minioRepository = minioRepository;
            _EmailHelper = emailHelper;
        }
        /**
         * 抽单
         */
        public async Task<DTO.Result<string>> recalllForm(string rno)
        {
            //抽回所有此单据没抽回的单据
            DTO.Result<string> result = new();
            IList<SendMobileLog> sendMobileFLogs = await _sendMobileLogRepository.GetDataByRecallStatusAndRno("N", rno);
            if (sendMobileFLogs.Count > 0)
            {
                sendMobileFLogs.ForEach(async sendMobileLog =>
                {
                    if (!string.IsNullOrEmpty(sendMobileLog.messageid))
                    {
                        MobileCancelFormRequset mobileCancelFormRequset = new MobileCancelFormRequset
                        {
                            serviceID = _configuration.GetSection("MCP:ServiceID").Value,
                            authCode = _configuration.GetSection("MCP:AuthCode").Value,
                            messageID = sendMobileLog.messageid
                        };
                        string datas = JsonConvert.SerializeObject(mobileCancelFormRequset);
                        HttpClient httpClient = _HttpClient.CreateClient();
                        MobileCancelFormRespose response = await httpClient.PostHelperAsync<MobileCancelFormRespose>(_configuration.GetSection("MCP:SendMobileUrl").Value + "CancelApprovalForm", datas);
                        _logger.LogError("Mcp抽单结果: {Response} 参数: {Datas}", response.ToString(), datas);
                        if (response.rtnCode == 1)//抽单成功
                        {
                            sendMobileLog.recallcode = response.rtnCode.ToString();
                            sendMobileLog.recallmessage = "抽单成功";
                            sendMobileLog.recallstatus = "Y";
                            result.message = "抽单成功";
                        }
                        else
                        {
                            result.status = -1;
                            result.message = "抽单失败";
                        }
                    }
                });
                await _sendMobileLogRepository.UpdateManyAsync(sendMobileFLogs);
            }
            return result;
        }
        /**
       * 抽单
       */
        public async Task<DTO.Result<string>> recalllFormAll()
        {
            //抽回所有此单据没抽回的单据
            DTO.Result<string> result = new();
            IList<SendMobileLog> sendMobileFLogs = await _sendMobileLogRepository.GetDataByRecallStatus("N");
            if (sendMobileFLogs.Count > 0)
            {
                sendMobileFLogs.ForEach(async sendMobileLog =>
                {
                    if (!string.IsNullOrEmpty(sendMobileLog.messageid))
                    {
                        MobileCancelFormRequset mobileCancelFormRequset = new MobileCancelFormRequset
                        {
                            serviceID = _configuration.GetSection("MCP:ServiceID").Value,
                            authCode = _configuration.GetSection("MCP:AuthCode").Value,
                            messageID = sendMobileLog.messageid
                        };
                        string datas = JsonConvert.SerializeObject(mobileCancelFormRequset);
                        _logger.LogError("MCP:抽单参数 {Datas}", datas.ToString());
                        HttpClient httpClient = _HttpClient.CreateClient();
                        MobileCancelFormRespose response = await httpClient.PostHelperAsync<MobileCancelFormRespose>(_configuration.GetSection("MCP:SendMobileUrl").Value + "CancelApprovalForm", datas);
                        _logger.LogError("MCP:抽单返回 单号: {Rno}, {MessageId}, {Response}", sendMobileLog.rno, sendMobileLog.messageid, response);
                        if (response.rtnCode == 1)//抽单成功
                        {
                            sendMobileLog.recallcode = response.rtnCode.ToString();
                            sendMobileLog.recallmessage = response.rtnMsg;
                            sendMobileLog.recallstatus = "Y";
                            result.message = "抽单成功";
                        }
                        else
                        {
                            result.status = -1;
                            result.message = "抽单失败";
                        }
                    }
                });
                await _sendMobileLogRepository.UpdateManyAsync(sendMobileFLogs);
            }
            return result;
        }
        //根据单号抛送手机签核
        public async Task<Result<string>> SendMobileSignXMLData(string rno)
        {
            DTO.Result<string> res = new();
            AppConfig sendMobileSignSwitch = await _appConfigRepository.GetValue("sendMobileSignSwitch");
            if (sendMobileSignSwitch.value != "open")
            {
                res.status = -1;
                res.message = "MCP:抛送手机端开关关闭！";
                _logger.LogError("MCP:抛送手机端开关关闭。 单号: {Rno}", rno);
                return res;
            }
            SendMobileLog sendMobileLog = null;
            try
            {
                //获取表单信息
                Result<ApplicationDto> result = await _applicationDomainService.QueryApplications(rno);
                ApplicationDto applicationDto = result.data;
                CashHeadDto head = applicationDto.head;
                List<string> formcodeList = new List<string>() { "CASH_1", "CASH_2", "CASH_3", "CASH_4", "CASH_3A", "CASH_6", "CASH_X" };
                if (!formcodeList.Contains(head.formcode))
                {
                    return res;
                }
                AppConfig IDMUser = await _appConfigRepository.GetValue("IDMUser");
                AppConfig IDMPassword = await _appConfigRepository.GetValue("IDMPassword");
                AppConfig mobileENCompany = await _appConfigRepository.GetValue("mobileENCompany");
                bool enFlag = false;
                if (mobileENCompany.value.Contains(head.company))
                {
                    enFlag = true;
                }
                EFormHead eFormHead = await _eformheadRepository.GetByNo(head.rno);
                List<CashFileDto> fileList = applicationDto.file;
                List<InvoiceDto> invList = applicationDto.invList;
                List<CashDetailDto> detailList = applicationDto.detail;
                //文件列表
                IList<SendMobileFileLog> sendMobileFileLogs = new List<SendMobileFileLog>();
                List<MobileSignFile> mobileSignFiles = new List<MobileSignFile>();
                List<SignFile> signFileList = GetSignFileList(fileList, invList, detailList,head.cuser);
                if (sendMobileFileLogs.Count > 0)//有抛送附件的记录
                {
                    sendMobileFileLogs.ForEach(sendMobileFileLog =>
                    {
                        MobileSignFile mobileSignFile = new MobileSignFile()
                        {
                            fileName = sendMobileFileLog.filename,
                            fileSize = sendMobileFileLog.filesize,
                        };
                        mobileSignFiles.Add(mobileSignFile);
                    });
                }
                else
                {
                    if (signFileList.Count > 0)
                    {
                        signFileList.ForEach(file =>
                        {
                            MobileSignFile mobileSignFile = new MobileSignFile()
                            {
                                fileName = file.fileName,
                                fileSize = file.fileSize,
                            };
                            mobileSignFiles.Add(mobileSignFile);
                        });
                    }
                }
                //查询已经抛到mcp的附件，拿掉已抛的，避免重复抛
                MobileAttInfoRequest mobileAttInfoRequest = new MobileAttInfoRequest
                {
                    serviceID = _configuration.GetSection("MCP:ServiceID").Value,
                    authCode = _configuration.GetSection("MCP:AuthCode").Value,
                    formID = rno
                };
                HttpClient httpClient = _HttpClient.CreateClient();
                string datas = JsonConvert.SerializeObject(mobileAttInfoRequest);
                MobileAttInfoRespose responseAttInfo = await httpClient.PostHelperAsync<MobileAttInfoRespose>(_configuration.GetSection("MCP:SendMobileUrl").Value + "AttInfo", datas);
                if (responseAttInfo.rtnCode == 1)//获取附件
                {
                    List<MobileFile> files = responseAttInfo.data;
                    // 移除signFileList中fileName与files相等的数据
                    signFileList.RemoveAll(signFile => files.Any(file => file.fileName == signFile.fileName));
                }
                //获取token
                var formData = new List<KeyValuePair<string, string>>
                {
                new KeyValuePair<string, string>("client_id", _configuration.GetSection("MCP:IDM_client_id").Value),
                new KeyValuePair<string, string>("client_secret", _configuration.GetSection("MCP:IDM_client_secret").Value),
                new KeyValuePair<string, string>("grant_type", _configuration.GetSection("MCP:IDM_grant_type").Value),
                new KeyValuePair<string, string>("username",IDMUser.value),
                new KeyValuePair<string, string>("password", IDMPassword.value),
                };
                IDMTokenResponse response = await httpClient.PostHelperAsync<IDMTokenResponse>(_configuration.GetSection("IDM:GetIDMToken").Value, formData);
                if (response == null)
                {
                    return res;
                }
                //获取签核
                Result<IList<SignForm>> signs = await _BPMDomainService.BPMQuerySign(rno, applicationDto.head.formcode, response.token_type + " " + response.access_token);
                IList<SignForm> signList = signs.data;
                SignForm sign = signList.FirstOrDefault(i => i.status == "Current");// SignForm sign = signList.Where(i => i.status == "Current").FirstOrDefault();
                string name = "";
                if (enFlag)
                {
                    name = GetFormName(head.formcode);
                }
                else
                {
                    name = await _bdformRepository.GetNameByFormcode(applicationDto.head.formcode);
                }
                string xml = GenerateXMLData(applicationDto, signList, sign, name, mobileSignFiles, eFormHead.apid, enFlag);
                //抛单
                HttpClient httpClientMcp = _HttpClient.CreateClient();
                MobileSendReturn mobileSendReturn = await httpClientMcp.PostHelperAsyncForMcp<MobileSendReturn>(_configuration.GetSection("MCP:SendMobileUrl").Value + "InsertApprovalForm", xml);
                if (mobileSendReturn.rtnCode == 1)//抛送手机端成功(保存抛送记录)
                {
                    res.message = "MCP:抛送请求成功！";
                    sendMobileLog = new SendMobileLog
                    {
                        rno = rno,
                        formcode = head.formcode,
                        seq = sign.seq,
                        step = sign.step,
                        emplid = sign.emplid,
                        deptid = sign.deptid,
                        signstatus = "C",
                        messageid = mobileSendReturn.data,
                        sendreturncode = mobileSendReturn.rtnCode.ToString(),
                        sendreturnmessage = mobileSendReturn.rtnMsg,
                        sendstatus = "Y",
                        recallstatus = "N",
                        company = head.company,
                        isdeleted = false
                    };
                    if (signFileList.Count > 0 && sendMobileFileLogs.Count <= 0)
                    {
                        for (int i = 0; i < signFileList.Count; i++)
                        {
                            try
                            {
                                if (long.TryParse(signFileList[i]?.fileSize?.ToString().Replace("KB", "").Trim(), out long filesize))
                                {
                                    if (filesize > 10240)//大于10M不抛
                                    {
                                        SendMobileFileLog sendMobileFileLog = new SendMobileFileLog
                                        {
                                            rno = rno,
                                            messageid = mobileSendReturn.data,
                                            fileId = "",
                                            filename = "",
                                            filesize = signFileList[i].fileSize,
                                            company = head.company,
                                            isdeleted = false
                                        };
                                        await _sendMobileFileLogRepository.InsertAsync(sendMobileFileLog);
                                    }
                                    else
                                    {
                                        MobileSignFileRespose response2 = await httpClient.PostHelperNoTokenAsync<MobileSignFileRespose>(_configuration.GetSection("MCP:SendMobileUrl").Value + "AddApprovalFile", mobileSendReturn.data, signFileList[i].filestring, signFileList[i].fileName);
                                        if (response2.rtnCode == 1)//抛送成功
                                        {
                                            SendMobileFileLog sendMobileFileLog = new SendMobileFileLog
                                            {
                                                rno = rno,
                                                messageid = response2.data.messageId,
                                                fileId = response2.data.fileId,
                                                filename = response2.data.fileName,
                                                filesize = signFileList[i].fileSize,
                                                company = head.company,
                                                isdeleted = false
                                            };
                                            await _sendMobileFileLogRepository.InsertAsync(sendMobileFileLog);
                                        }
                                        else
                                        {
                                            res.status = -1;
                                            res.message = "MCP:抛送附件请求失败:" + response2.rtnMsg;
                                            _logger.LogError("MCP:抛送附件请求失败。 status: {Rno}; code: {RtnCode}; mag: {RtnMsg}", rno, response2.rtnCode, response2.rtnMsg);
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "MCP:抛送附件请求失败");
                                _EmailHelper.SendEmail("Smart ERS MCP:抛送附件请求失败：", "samsong_zhang@wistron.com", "wiwi_wei@wistron.com", rno + "抛送附件请求失败：" + e.ToString());
                            }
                        }
                    }
                }
                else//抛送失败
                {
                    res.status = -1;
                    res.message = "MCP:抛送请求失败：" + mobileSendReturn.rtnMsg;
                    _logger.LogError("MCP:抛送请求失败。 status: {Rno}; code: {RtnCode}; mag: {RtnMsg}", rno, mobileSendReturn.rtnCode, mobileSendReturn.rtnMsg);
                }
            }
            catch (Exception e)
            {
                res.status = -1;
                res.message = "MCP:抛送请求失败：" + e.ToString();
                _EmailHelper.SendEmail("Smart ERS MCP:抛送请求失败", "samsong_zhang@wistron.com", "wiwi_wei@wistron.com", rno + "：抛送请求失败" + e.ToString());
            }
            finally
            {
                if (sendMobileLog != null)
                {
                    await _sendMobileLogRepository.InsertAsync(sendMobileLog);
                }
            }
            return res;
        }
        public static string GetFormName(string formCode)
        {
            string name = "";
            switch (formCode)
            {
                case "CASH_1":
                    name = "Reimbursement of general expenses";
                    break;
                case "CASH_2":
                    name = "Reimbursement of party expenses";
                    break;
                case "CASH_3":
                    name = "Prepayments";
                    break;
                case "CASH_3A":
                    name = "Deffered advance";
                    break;
                case "CASH_4":
                    name = "Batch reimbursement";
                    break;
                case "CASH_5":
                    name = "A large number of reimbursement";
                    break;
                case "CASH_6":
                    name = "Return to Taiwan meeting";
                    break;
                case "CASH_X":
                    name = "Salary requests";
                    break;
            }
            return name;
        }
        /**
        * 抛单
        */
        public string GenerateXMLData(ApplicationDto applicationDto, IList<SignForm> signList, SignForm sign, string name, List<MobileSignFile> mobileSignFiles, string apid, bool enFlag)
        {
            CashHeadDto head = applicationDto.head;
            List<CashDetailDto> detailList = applicationDto.detail;
            CashAmountDto amount = applicationDto.amount;
            string subject = head.rno + "-" + head.payeeId + "-" + head.payeename;
            // 创建根节点
            XElement rootNode = new XElement("REQUEST");
            // 创建HEADER节点
            XElement headerNode = new XElement("HEADER",
                new XElement("SERVICEID", _configuration.GetSection("MCP:ServiceID").Value),
                new XElement("AUTHORIZATIONCODE", _configuration.GetSection("MCP:AuthCode").Value),
                  new XElement("WEBENABLE", "true"),
                new XElement("DATACREATEDTIME", GetNowDateToString()));
            rootNode.Add(headerNode);
            // 创建FORMDATA节点
            XElement formDataNode = new XElement("FORMDATA",
                new XElement("SYSTEMID", _configuration.GetSection("MCP:ServiceID").Value),
                new XElement("FORMTYPE", head.formcode),
                new XElement("FORMID", head.rno),
                new XElement("STEPID", sign.seq),
                new XElement("StepName", sign.step),
                new XElement("ProcessruleID", head.rno),
                new XElement("UniqueID", "0000001"),
                new XElement("SerialID", head.rno),
                new XElement("SignerName", sign.signer_cname),
                new XElement("FROMSITE", head.company),
                new XElement("FormName", name),
                new XElement("PRIORITY", "0"),
                new XElement("USERID", sign.signer_emplid),
                new XElement("TITLE", name),
                new XElement("SUBJECT", subject)
            );
            //单据信息
            XElement summaryNode = new XElement("SUMMARY");
            if (head.formcode == "CASH_1")//一般费用报销
            {
                summaryNode.Add(generateFormXMLForCash1(head, amount, enFlag));
                summaryNode.Add(generateDetailXMLForCash1(detailList, enFlag));
            }
            else if (head.formcode == "CASH_2")//交际费
            {
                if (apid == "rq201a" || apid == "RQ201A" || apid == "rq204a" || apid == "RQ204A")
                {
                    summaryNode.Add(generateDetailXMLForCash2A(head, amount, detailList, enFlag));
                }
                else
                {
                    summaryNode.Add(generateFormXMLForCash2(head, amount, enFlag));
                    summaryNode.Add(generateDetailXMLForCash2(detailList, enFlag));
                }
            }
            else if (head.formcode == "CASH_3")
            {
                summaryNode.Add(generateFormXMLForCash3(head, amount, enFlag));
                summaryNode.Add(generateDetailXMLForCash3(detailList, enFlag));
            }
            else if (head.formcode == "CASH_4")
            {
                summaryNode.Add(generateFormXMLForCash4(head, amount, detailList, enFlag));
                summaryNode.Add(generateDetailXMLForCash4(detailList, enFlag));
            }
            else if (head.formcode == "CASH_3A")
            {
                summaryNode.Add(generateFormXMLForCash3A(head, amount, enFlag));
                summaryNode.Add(generateDetailXMLForCash3A(detailList, enFlag));
            }
            else if (head.formcode == "CASH_X")
            {
                summaryNode.Add(generateFormXMLForCashX(head, amount, enFlag));
                summaryNode.Add(generateDetailXMLForCashX(detailList, enFlag));
            }
            else if (head.formcode == "CASH_6")
            {
                summaryNode.Add(generateFormXMLForCash6(head, amount, enFlag));
                summaryNode.Add(generateDetailXMLForCash6(detailList, enFlag));
            }
            //异常提示
            summaryNode.Add(generateTotalWarningXML(detailList, enFlag));
            if (enFlag)
            {
                //签核
                XElement signNode = new XElement("section", new XAttribute("title", "Sign Level(sun:" + signList.Count + "strokes)"),
                            new XAttribute("type", "table"));
                //签核明细
                List<XElement> signNodeList = new List<XElement>();
                signList.ForEach(sign =>
                {
                    if (sign.status == "Finish")
                    {
                        XElement signXElement = new XElement("Row",
                                new XElement("col", new XAttribute("title", "sign Fuction"), sign.step),
                                new XElement("col", new XAttribute("title", "sign user"), sign.signer_ename),
                                new XElement("col", new XAttribute("title", "actual signer"), sign.signer_ename),
                                new XElement("col", new XAttribute("title", "sign status"), "approved"),
                                new XElement("col", new XAttribute("title", "sign Opinion"), sign.signer_ename),
                                new XElement("col", new XAttribute("title", "sign time"), sign.sign_date)
                            );
                        signNodeList.Add(signXElement);
                    }
                    else if (sign.status == "Current")
                    {
                        XElement signXElement = new XElement("Row",
                              new XElement("col", new XAttribute("title", "sign Fuction"), sign.step),
                              new XElement("col", new XAttribute("title", "sign user"), sign.signer_ename),
                              new XElement("col", new XAttribute("title", "actual signer")),
                              new XElement("col", new XAttribute("title", "sign status"), "Approval in progress"),
                              new XElement("col", new XAttribute("title", "sign Opinion")),
                              new XElement("col", new XAttribute("title", "sign time"))
                          );
                        signNodeList.Add(signXElement);
                    }
                    else
                    {
                        XElement signXElement = new XElement("Row",
                              new XElement("col", new XAttribute("title", "sign Fuction"), sign.step),
                              new XElement("col", new XAttribute("title", "sign user"), sign.signer_ename),
                              new XElement("col", new XAttribute("title", "actual signer")),
                              new XElement("col", new XAttribute("title", "sign status"), "Pending approval"),
                              new XElement("col", new XAttribute("title", "sign Opinion")),
                              new XElement("col", new XAttribute("title", "sign time"))
                          );
                        signNodeList.Add(signXElement);
                    }
                });
                signNode.Add(signNodeList);
                summaryNode.Add(signNode);
            }
            else
            {
                //签核
                XElement signNode = new XElement("section", new XAttribute("title", "簽核層級(共" + signList.Count + "筆)"),
                            new XAttribute("type", "table"));
                //签核明细
                List<XElement> signNodeList = new List<XElement>();
                signList.ForEach(sign =>
                {
                    if (sign.status == "Finish")
                    {
                        XElement signXElement = new XElement("Row",
                                new XElement("col", new XAttribute("title", "簽核Fuction"), sign.step),
                                new XElement("col", new XAttribute("title", "簽核人"), sign.signer_ename),
                                new XElement("col", new XAttribute("title", "实际簽核人"), sign.signer_ename),
                                new XElement("col", new XAttribute("title", "簽核狀態"), "已完成"),
                                new XElement("col", new XAttribute("title", "簽核意見"), sign.signer_ename),
                                new XElement("col", new XAttribute("title", "簽核時間"), sign.sign_date)
                            );
                        signNodeList.Add(signXElement);
                    }
                    else if (sign.status == "Current")
                    {
                        XElement signXElement = new XElement("Row",
                              new XElement("col", new XAttribute("title", "簽核Fuction"), sign.step),
                              new XElement("col", new XAttribute("title", "簽核人"), sign.signer_ename),
                              new XElement("col", new XAttribute("title", "实际簽核人")),
                              new XElement("col", new XAttribute("title", "簽核狀態"), "进行中"),
                              new XElement("col", new XAttribute("title", "簽核意見")),
                              new XElement("col", new XAttribute("title", "簽核時間"))
                          );
                        signNodeList.Add(signXElement);
                    }
                    else
                    {
                        XElement signXElement = new XElement("Row",
                              new XElement("col", new XAttribute("title", "簽核Fuction"), sign.step),
                              new XElement("col", new XAttribute("title", "簽核人"), sign.signer_ename),
                              new XElement("col", new XAttribute("title", "实际簽核人")),
                              new XElement("col", new XAttribute("title", "簽核狀態"), "待签核"),
                              new XElement("col", new XAttribute("title", "簽核意見")),
                              new XElement("col", new XAttribute("title", "簽核時間"))
                          );
                        signNodeList.Add(signXElement);
                    }
                });
                signNode.Add(signNodeList);
                summaryNode.Add(signNode);
            }
            formDataNode.Add(summaryNode);
            //文件列表
            List<CashFileDto> cashFileDtos = new List<CashFileDto>();
            List<InvoiceDto> invoiceDtos = new List<InvoiceDto>();
            detailList.ForEach(x =>
            {
                if (x.fileList.Count > 0)
                {
                    cashFileDtos.AddRange(x.fileList);
                }
                if (x.invList.Count > 0)
                {
                    invoiceDtos.AddRange(x.invList);
                }
            });
            //文件节点
            if (mobileSignFiles.Count > 0)
            {
                List<MobileSignFile> xmlFileList = new List<MobileSignFile>();
                mobileSignFiles.ForEach(fileInfo =>
                {
                    xmlFileList.Add(new MobileSignFile()
                    {
                        fileName = fileInfo.fileName,
                        fileSize = fileInfo.fileSize,
                    });
                });
                //文件节点
                XElement attachListElement = new XElement("ATTACHLIST");
                xmlFileList.ForEach(file =>
                {
                    XElement fileElement = new XElement("FILE",
                                    new XElement("ATTACH_NAME", file.fileName),
                                    new XElement("ATTACH_SIZE", file.fileSize));
                    attachListElement.Add(fileElement);
                });
                formDataNode.Add(attachListElement);
            }
            //签核节点
            // 添加子元素并设置文本内容
            formDataNode.Add(new XElement("MESSAGEID"));
            formDataNode.Add(new XElement("CHKRESULT"));
            formDataNode.Add(new XElement("APPRSUMMARY"));
            formDataNode.Add(new XElement("APPROVALCOMMENT"));
            rootNode.Add(formDataNode);
            string xmlstrNode = rootNode.ToString(SaveOptions.OmitDuplicateNamespaces | SaveOptions.DisableFormatting);
            return xmlstrNode;
        }
        /**
        * 文件转base64
        */
        public static string ConvertFileInfoToBinaryString(FileInfo fileInfo)
        {
            byte[] fileBytes = File.ReadAllBytes(fileInfo.FullName);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
        public static string FileToBase64String(string fileUrl)
        {
            byte[] fileBytes = File.ReadAllBytes(fileUrl);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
        public static string FileToBase64String(string fileUrl, out string fileSize)
        {
            byte[] fileBytes = File.ReadAllBytes(fileUrl);
            string base64String = Convert.ToBase64String(fileBytes);
            FileInfo fileInfo = new FileInfo(fileUrl);
            long fileSizeBytes = fileInfo.Length;
            fileSize = (fileSizeBytes / 1000).ToString() + "KB";
            return base64String;
        }
        public (string, string) GetRemoteFileDetails(string path, string userId)
        {
            MemoryStream memoryStream = _minioRepository.GetObjectAsync(path,userId);
            byte[] byteArray = memoryStream.ToArray();
            string base64String = Convert.ToBase64String(byteArray);
            long fileSizeBytes = memoryStream.Length;
            string fileSize = (fileSizeBytes / 1000).ToString() + "KB";
            return (base64String, fileSize);
        }
        public List<SignFile> GetSignFileList(List<CashFileDto> fileList, List<InvoiceDto> invList, List<CashDetailDto> detailList, string userId)
        {
            List<SignFile> signFiles = new List<SignFile>();
            List<string> fileNames = new List<string>();
            //文件列表
            List<CashFileDto> cashFileDtos = new List<CashFileDto>();
            List<InvoiceDto> invoiceDtos = new List<InvoiceDto>();
            detailList.ForEach(x =>
            {
                if (x.fileList.Count > 0)
                {
                    cashFileDtos.AddRange(x.fileList);
                }
                if (x.invList.Count > 0)
                {
                    invoiceDtos.AddRange(x.invList);
                }
            });
            if (fileList.Count > 0 || cashFileDtos.Count > 0)
            {
                if (cashFileDtos.Count > 0)
                {
                    List<CashFileDto> invoicesList = cashFileDtos.Where(o => !string.IsNullOrEmpty(o.path) && o.path.Contains("Invoices")).ToList();
                    List<CashFileDto> attachList = cashFileDtos.Where(o => !string.IsNullOrEmpty(o.path) && !o.path.Contains("Invoices")).ToList();
                    if (invoicesList.Count > 0)
                    {
                        invoicesList.ForEach(file =>
                        {
                            if (!string.IsNullOrEmpty(file.path) && !fileNames.Contains(file.filename))
                            {
                                fileNames.Add(file.filename);
                                (string base64String, string fileSize) = GetRemoteFileDetails(file.path,userId);
                                SignFile signFile = new()
                                {
                                    fileName = file.filename,
                                    fileSize = fileSize,
                                    filestring = base64String,
                                };
                                signFiles.Add(signFile);
                            }
                        });
                    }
                    if (attachList.Count > 0)
                    {
                        attachList.ForEach(file =>
                        {
                            if (!string.IsNullOrEmpty(file.path) && !fileNames.Contains(file.filename))
                            {
                                fileNames.Add(file.filename);
                                (string base64String, string fileSize) = GetRemoteFileDetails(file.path,userId);
                                SignFile signFile = new()
                                {
                                    fileName = file.filename,
                                    fileSize = fileSize,
                                    filestring = base64String,
                                };
                                signFiles.Add(signFile);
                            }
                        });
                    }
                }
                if (fileList.Count > 0)
                {
                    fileList.ForEach(file =>
                    {
                        if (!string.IsNullOrEmpty(file.path) && !fileNames.Contains(file.filename))
                        {
                            fileNames.Add(file.filename);
                            (string base64String, string fileSize) = GetRemoteFileDetails(file.path,userId);
                            SignFile signFile = new SignFile()
                            {
                                fileName = file.filename,
                                fileSize = fileSize,
                                filestring = base64String,
                            };
                            signFiles.Add(signFile);
                        }
                    });
                }
            }
            return signFiles;
        }
        /**
          * 获取文件
          */
        public List<FileInfo> getFileInfoList(List<CashFileDto> fileList, List<InvoiceDto> invList, List<CashDetailDto> detailList)
        {
            List<FileInfo> fileInfos = new List<FileInfo>();
            List<string> fileNames = new List<string>();
            //文件列表
            List<CashFileDto> cashFileDtos = new List<CashFileDto>();
            List<InvoiceDto> invoiceDtos = new List<InvoiceDto>();
            detailList.ForEach(x =>
            {
                if (x.fileList.Count > 0)
                {
                    cashFileDtos.AddRange(x.fileList);
                }
                if (x.invList.Count > 0)
                {
                    invoiceDtos.AddRange(x.invList);
                }
            });
            if (fileList.Count > 0 || invList.Count > 0 || cashFileDtos.Count > 0 || invoiceDtos.Count > 0)
            {
                if (fileList.Count > 0)
                {
                    fileList.ForEach(async file =>
                    {
                        // 使用FileInfo类获取文件信息
                        if (!fileNames.Contains(file.filename))
                        {
                            FileInfo fileInfo = await GetFileInfoFromUrlAsync(file.url, file.filename);
                            fileInfos.Add(fileInfo);
                            fileNames.Add(file.filename);
                        }
                    });
                }
                if (invList.Count > 0)
                {
                    invList.ForEach(async file =>
                    {
                        if (!string.IsNullOrEmpty(file.fileurl) && !fileNames.Contains(file.invtype + file.baseamt + ".pdf"))
                        {
                            FileInfo fileInfo = await GetFileInfoFromUrlAsync(file.fileurl, file.invtype + file.baseamt + ".pdf");
                            fileInfos.Add(fileInfo);
                            fileNames.Add(file.invtype + file.baseamt + ".pdf");
                        }
                    });
                }
                if (cashFileDtos.Count > 0)
                {
                    detailList[0].fileList.ForEach(async file =>
                    {
                        if (!fileNames.Contains(file.filename))
                        {
                            FileInfo fileInfo = await GetFileInfoFromUrlAsync(file.url, file.filename);
                            fileInfos.Add(fileInfo);
                            fileNames.Add(file.filename);
                        }
                    });
                }
                if (invoiceDtos.Count > 0)
                {
                    detailList[0].invList.ForEach(async file =>
                    {
                        if (!string.IsNullOrEmpty(file.fileurl) && !fileNames.Contains(file.invtype + file.baseamt + ".pdf"))
                        {
                            FileInfo fileInfo = await GetFileInfoFromUrlAsync(file.fileurl, file.invtype + file.baseamt + ".pdf");
                            fileInfos.Add(fileInfo);
                            fileNames.Add(file.invtype + file.baseamt + ".pdf");
                        }
                    });
                }
            }
            return fileInfos;
        }
        public static XElement generateTotalWarningXML(List<CashDetailDto> detailList, bool enFlag)
        {
            List<string> warningList = new List<string>();
            detailList.ForEach(detail =>
            {
                detail.invList.ForEach(i =>
                {
                    if (!string.IsNullOrEmpty(i.expcode))
                    {
                        warningList.Add(i.expcode);
                    }
                });
            });
            if (warningList.Count > 0)
            {
                if (enFlag)
                {
                    XElement detailNode = new XElement("section", new XAttribute("title", "Exception Prompt"), new XAttribute("titlecolor", "red"), new XAttribute("titlesize", "large"),
                                          new XAttribute("type", "LIST"));
                    List<XElement> detailNodeList = new List<XElement>();
                    XElement rowNode = new XElement("Row");
                    for (int i = 0; i < warningList.Count; i++)
                    {
                        XElement colNode = new XElement("col",
                            new XAttribute("title", (i + 1).ToString()),
                            new XAttribute("titlecolor", "red"),
                            new XAttribute("titlesize", "large"),
                            new XAttribute("textsize", "large"),
                            new XAttribute("textcolor", "red"),
                            warningList[i]
                        );
                        rowNode.Add(colNode);
                    }
                    detailNodeList.Add(rowNode);
                    detailNode.Add(detailNodeList);
                    return detailNode;
                }
                else
                {
                    XElement detailNode = new XElement("section", new XAttribute("title", "异常提示"), new XAttribute("titlecolor", "red"), new XAttribute("titlesize", "large"),
                                          new XAttribute("type", "LIST"));
                    List<XElement> detailNodeList = new List<XElement>();
                    XElement rowNode = new XElement("Row");
                    for (int i = 0; i < warningList.Count; i++)
                    {
                        XElement colNode = new XElement("col",
                            new XAttribute("title", (i + 1).ToString()),
                            new XAttribute("titlecolor", "red"),
                            new XAttribute("titlesize", "large"),
                            new XAttribute("textsize", "large"),
                            new XAttribute("textcolor", "red"),
                            warningList[i]
                        );
                        rowNode.Add(colNode);
                    }
                    detailNodeList.Add(rowNode);
                    detailNode.Add(detailNodeList);
                    return detailNode;
                }
            }
            else
            {
                return null;
            }
        }
        public static XElement generateFormXMLForCash1(CashHeadDto head, CashAmountDto amount, bool enFlag)
        {
            if (enFlag)
            {
                XElement formNode = new XElement("section", new XAttribute("title", "Form details"),
                       new XAttribute("type", "LIST"));
                //表单明细
                XElement rowNode = new XElement("Row",
                                new XElement("col", new XAttribute("title", "Company code"), head.company),
                                new XElement("col", new XAttribute("title", "Expense project"), head.dtype),
                                new XElement("col", new XAttribute("title", "Project code"), head.projectcode),
                                new XElement("col", new XAttribute("title", "Emplid"), head.payeeId),
                                new XElement("col", new XAttribute("title", "Dept"), head.deptid),
                                new XElement("col", new XAttribute("title", "Payee"), head.payeename),
                                new XElement("col", new XAttribute("title", "Total reimbursement"), amount.currency + ":" + formatAmount(amount.amount, amount.currency)),
                                new XElement("col", new XAttribute("title", "Actual pay amount"), amount.currency + ":" + formatAmount(amount.actamt, amount.currency))
                            );
                formNode.Add(rowNode);
                return formNode;
            }
            else
            {
                XElement formNode = new XElement("section", new XAttribute("title", "表單明細"),
                       new XAttribute("type", "LIST"));
                //表单明细
                XElement rowNode = new XElement("Row",
                                new XElement("col", new XAttribute("title", "公司别"), head.company),
                                new XElement("col", new XAttribute("title", "費用簽核項目"), head.dtype),
                                new XElement("col", new XAttribute("title", "Project code"), head.projectcode),
                                new XElement("col", new XAttribute("title", "工號"), head.payeeId),
                                new XElement("col", new XAttribute("title", "部門代碼"), head.deptid),
                                new XElement("col", new XAttribute("title", "收款人"), head.payeename),
                                new XElement("col", new XAttribute("title", "報銷總計"), amount.currency + ":" + formatAmount(amount.amount, amount.currency)),
                                new XElement("col", new XAttribute("title", "實際支付金額"), amount.currency + ":" + formatAmount(amount.actamt, amount.currency))
                            );
                formNode.Add(rowNode);
                return formNode;
            }
        }
        public static XElement generateDetailXMLForCash1(List<CashDetailDto> detailList, bool enFlag)
        {
            if (enFlag)
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "Reimbursement description"),
                                     new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "Expense category"), detail.expname),
                                    new XElement("col", new XAttribute("title", "Date of expense"), detail.rdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "Currency"), detail.basecurr),
                                    new XElement("col", new XAttribute("title", "Expense Attribution Dept"), detail.deptList[0].deptId),
                                    new XElement("col", new XAttribute("title", "Percent(%)"), detail.deptList[0].percent),
                                    new XElement("col", new XAttribute("title", "Reimbursement amount"), detail.deptList[0].amount),
                                    new XElement("col", new XAttribute("title", "Conversion to local currency"), detail.deptList[0].baseamount),
                                    new XElement("col", new XAttribute("title", "Exchange Rate"), detail.rate),
                                    new XElement("col", new XAttribute("title", "Digest"), detail.summary),
                                    new XElement("col", new XAttribute("title", "Individual responsibility for taxes"), detail.amount2),
                                    new XElement("col", new XAttribute("title", "Actual reimbursable amount"), detail.amount),
                                    new XElement("col", new XAttribute("title", "Advance Fund RNo"), detail.advancerno),
                                    new XElement("col", new XAttribute("title", "Tax deductible"), detail.taxexpense),
                                    new XElement("col", new XAttribute("title", "Blank form remarks"), detail.assignment)
                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
            else
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "報銷描述"),
                     new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "費用類別"), detail.expname),
                                    new XElement("col", new XAttribute("title", "費用發生日期"), detail.rdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "幣別"), detail.basecurr),
                                    new XElement("col", new XAttribute("title", "費用歸屬部門"), detail.deptList[0].deptId),
                                    new XElement("col", new XAttribute("title", "比例(%)"), detail.deptList[0].percent),
                                    new XElement("col", new XAttribute("title", "報銷金額"), detail.deptList[0].amount),
                                    new XElement("col", new XAttribute("title", "折算本位幣"), detail.deptList[0].baseamount),
                                    new XElement("col", new XAttribute("title", "匯率"), detail.rate),
                                    new XElement("col", new XAttribute("title", "摘要"), detail.summary),
                                    new XElement("col", new XAttribute("title", "個人承擔稅金"), detail.amount2),
                                    new XElement("col", new XAttribute("title", "實際可報銷金額"), detail.amount),
                                    new XElement("col", new XAttribute("title", "預支金單號"), detail.advancerno),
                                    new XElement("col", new XAttribute("title", "可抵扣税额"), detail.taxexpense),
                                    new XElement("col", new XAttribute("title", "白单备注"), detail.assignment)
                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
        }
        public static XElement generateFormXMLForCash2(CashHeadDto head, CashAmountDto amount, bool enFlag)
        {
            XElement formNode = new XElement("section", new XAttribute("title", enFlag ? "Form details" : "表單明細"),
                    new XAttribute("type", "LIST"));
            //表单明细
            XElement rowNode = new XElement("Row",
                    new XElement("col", new XAttribute("title", enFlag ? "Company code" : "公司别"), head.company),
                    new XElement("col", new XAttribute("title", enFlag ? "Expense project" : "費用簽核項目"), head.dtype),
                    new XElement("col", new XAttribute("title", "Project code"), head.projectcode),
                    new XElement("col", new XAttribute("title", enFlag ? "Emplid" : "收款人工號"), head.payeeId),
                    new XElement("col", new XAttribute("title", enFlag ? "Name" : "姓名"), head.cname),
                    new XElement("col", new XAttribute("title", enFlag ? "Total expense" : "費用總計"), amount.currency + ":" + formatAmount(amount.amount, amount.currency)),
                    new XElement("col", new XAttribute("title", enFlag ? "Actual reimbursable amount" : "實際可報銷金額"), amount.currency + ":" + formatAmount(amount.actamt, amount.currency)),
                    new XElement("col", new XAttribute("title", enFlag ? "Actual payment amount" : "實際支付金額"), amount.currency + ":" + formatAmount(amount.actamt, amount.currency))
                );
            formNode.Add(rowNode);
            return formNode;
        }
        public static XElement generateDetailXMLForCash2(List<CashDetailDto> detailList, bool enFlag)
        {
            if (enFlag)
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "Reimbursement description"),
                     new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    decimal individualAffordAmt = 0;
                    decimal companyAffordAmt = 0;
                    decimal selfTaxAmt = 0;
                    detail.invList.ForEach(i =>
                    {
                        if (i.undertaker == "self")
                        {
                            individualAffordAmt += i.amount;
                            selfTaxAmt += i.taxloss;
                        }
                        else if (i.undertaker == "company")
                        {
                            companyAffordAmt += i.amount;
                        }
                    });
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "Date of fee/voucher"), detail.rdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "Host Object"), detail.@object),
                                    new XElement("col", new XAttribute("title", "Company Escort"), detail.keep),
                                    new XElement("col", new XAttribute("title", "Remark"), detail.remarks),
                                    new XElement("col", new XAttribute("title", "Expense Attribution Dept"), detail.deptid),
                                    new XElement("col", new XAttribute("title", "Currency"), detail.currency),
                                    new XElement("col", new XAttribute("title", "Applied Amount"), detail.amount1),
                                    new XElement("col", new XAttribute("title", "Conversion to local currency"), detail.baseamt),
                                    new XElement("col", new XAttribute("title", "Exchange Rate"), detail.rate),
                                    new XElement("col", new XAttribute("title", "Individual responsibility for taxes"), selfTaxAmt),
                                    new XElement("col", new XAttribute("title", "Actual reimbursable amount"), detail.amount),
                                    new XElement("col", new XAttribute("title", "Blank form remarks"), detail.assignment)
                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
            else
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "報銷描述"),
                     new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    decimal individualAffordAmt = 0;
                    decimal companyAffordAmt = 0;
                    decimal selfTaxAmt = 0;
                    detail.invList.ForEach(i =>
                    {
                        if (i.undertaker == "self")
                        {
                            individualAffordAmt += i.amount;
                            selfTaxAmt += i.taxloss;
                        }
                        else if (i.undertaker == "company")
                        {
                            companyAffordAmt += i.amount;
                        }
                    });
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "費用/憑證日期"), detail.rdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "招待對象"), detail.@object),
                                    new XElement("col", new XAttribute("title", "公司陪同人員"), detail.keep),
                                    new XElement("col", new XAttribute("title", "備註"), detail.remarks),
                                    new XElement("col", new XAttribute("title", "費用歸屬部門"), detail.deptid),
                                    new XElement("col", new XAttribute("title", "幣別"), detail.currency),
                                    new XElement("col", new XAttribute("title", "申請金額"), detail.amount1),
                                    new XElement("col", new XAttribute("title", "折算本位幣"), detail.baseamt),
                                    new XElement("col", new XAttribute("title", "匯率"), detail.rate),
                                    new XElement("col", new XAttribute("title", "個人承擔稅金"), selfTaxAmt),
                                    new XElement("col", new XAttribute("title", "實際可報銷金額"), detail.amount),
                                    new XElement("col", new XAttribute("title", "白单备注"), detail.assignment)
                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
        }
        public static List<XElement> generateDetailXMLForCash2A(CashHeadDto head, CashAmountDto amount, List<CashDetailDto> detailList, bool enFlag)
        {
            List<XElement> sectionNodeList = new List<XElement>();
            if (enFlag)
            {
                XElement sectionNode1 = new XElement("section", new XAttribute("title", ""),
                     new XAttribute("type", "LIST"));
                XElement rowNode1 = new XElement("Row",
                                new XElement("col", head.company, new XAttribute("title", "Company code")),
                                new XElement("col", head.rno, new XAttribute("title", "RNo")),
                                new XElement("col", head.projectcode, new XAttribute("title", "Project code")),
                                new XElement("col", head.payeeId, new XAttribute("title", "Emplid")),
                                new XElement("col", head.payeename, new XAttribute("title", "Payee")),
                                new XElement("col", head.whetherapprove == "Y" ? "Yes" : "No", new XAttribute("title", "Is prior approval obtained from the supervisor?")),
                                new XElement("col", head.approvereason, new XAttribute("title", "Please give a reason"))
                );
                sectionNode1.Add(rowNode1);
                XElement sectionNode3 = new XElement("section", new XAttribute("title", "Banquet Detail"),
                   new XAttribute("type", "LIST"));
                XElement rowNode3 = new XElement("Row",
                                new XElement("col", detailList[0].@object, new XAttribute("title", "Customer/Banquet Name")),
                                new XElement("col", detailList[0].treataddress, new XAttribute("title", "Banquet Restaurant Name")),
                                new XElement("col", detailList[0].deptid, new XAttribute("title", "Expense Attribution Dept")),
                                new XElement("col", detailList[0].rdate?.ToString("yyyy/MM/dd"), new XAttribute("title", "Banquet Time")),
                                new XElement("col", detailList[0].currency, new XAttribute("title", "Currency"))
                                );
                sectionNode3.Add(rowNode3);
                XElement sectionNode4 = new XElement("section", new XAttribute("title", "Customer Participants"),
                   new XAttribute("type", "LIST"));
                XElement rowNode4 = new XElement("Row",
                                new XElement("col", detailList[0].custsuperme, new XAttribute("title", "Customer Top Executive Name/Title")),
                                new XElement("col", detailList[0].otherobject, new XAttribute("title", "Other Members")),
                                new XElement("col", detailList[0].objectsum, new XAttribute("title", "Overall Number Of People"))
                                );
                sectionNode4.Add(rowNode4);
                XElement sectionNode5 = new XElement("section", new XAttribute("title", "Company Participants"),
                   new XAttribute("type", "LIST"));
                XElement rowNode5 = new XElement("Row",
                                new XElement("col", detailList[0].keep, new XAttribute("title", "Name/Title of the company's top executive")),
                                new XElement("col", detailList[0].keepcategory, new XAttribute("title", "The category of the company's top executive")),
                                new XElement("col", detailList[0].otherkeep, new XAttribute("title", "Other Members")),
                                new XElement("col", detailList[0].keepsum, new XAttribute("title", "Overall Number Of People"))
                                );
                sectionNode5.Add(rowNode5);
                XElement sectionNode2 = new XElement("section", new XAttribute("title", ""),
                   new XAttribute("type", "LIST"));
                string processMethodDescription = detailList[0].processmethod == "0"
                    ? "Colleagues bear their own burden(The Company only approves the amount within the budget for reimbursement)"
                    : detailList[0].processmethod == "1"
                    ? "Deduct department annual performance bonus budget"
                    : "";
                XElement rowNode2 = new XElement("Row",
                                new XElement("col", detailList[0].isaccordnumber == "Y" ? "Accord" : "Not Accord", new XAttribute("title", "Is it in accordance with our specification for number of persons(remark 1)?")),
                                new XElement("col", detailList[0].amount1, new XAttribute("title", "The total budget based on the number of persons")),
                                new XElement("col", detailList[0].paymentexpense, new XAttribute("title", "Actual Expenditure")),
                                new XElement("col", detailList[0].isaccordcost == "Y" ? "Accord" : "Not Accord", new XAttribute("title", "Whether it meets the expense specification(remark 2)?")),
                                new XElement("col", detailList[0].overbudget, new XAttribute("title", "The total amount exceeded")),
                                new XElement("col", processMethodDescription, new XAttribute("title", "The treatment of such amount"))
                                );
                sectionNode2.Add(rowNode2);
                XElement sectionNode6 = new XElement("section", new XAttribute("title", ""),
                        new XAttribute("type", "LIST"));
                XElement rowNode6 = new XElement("Row",
                                new XElement("col", amount.amount, new XAttribute("title", "Actual Reimbursement Amount")));
                sectionNode6.Add(rowNode6);
                sectionNodeList.Add(sectionNode1);
                sectionNodeList.Add(sectionNode3);
                sectionNodeList.Add(sectionNode4);
                sectionNodeList.Add(sectionNode5);
                sectionNodeList.Add(sectionNode2);
                sectionNodeList.Add(sectionNode6);
            }
            else
            {
                XElement sectionNode1 = new XElement("section", new XAttribute("title", ""),
                                     new XAttribute("type", "LIST"));
                XElement rowNode1 = new XElement("Row",
                                new XElement("col", head.company, new XAttribute("title", "公司別")),
                                new XElement("col", head.rno, new XAttribute("title", "報銷單號")),
                                new XElement("col", head.projectcode, new XAttribute("title", "Project code")),
                                new XElement("col", head.payeeId, new XAttribute("title", "受款人工號")),
                                new XElement("col", head.payeename, new XAttribute("title", "姓名")),
                                new XElement("col", head.whetherapprove == "Y" ? "是" : "否", new XAttribute("title", "宴客前是否已事先取得權責主管核准?")),
                                new XElement("col", head.approvereason, new XAttribute("title", "請說明原因"))
                );
                sectionNode1.Add(rowNode1);
                XElement sectionNode3 = new XElement("section", new XAttribute("title", "宴客明細"),
                   new XAttribute("type", "LIST"));
                XElement rowNode3 = new XElement("Row",
                                new XElement("col", detailList[0].@object, new XAttribute("title", "宴客客戶/宴客名稱")),
                                new XElement("col", detailList[0].treataddress, new XAttribute("title", "宴客餐厅名称")),
                                new XElement("col", detailList[0].deptid, new XAttribute("title", "費用歸屬部門")),
                                new XElement("col", detailList[0].rdate?.ToString("yyyy/MM/dd"), new XAttribute("title", "宴客時間")),
                                new XElement("col", detailList[0].currency, new XAttribute("title", "幣別"))
                                );
                sectionNode3.Add(rowNode3);
                XElement sectionNode4 = new XElement("section", new XAttribute("title", "客戶參加人員"),
                   new XAttribute("type", "LIST"));
                XElement rowNode4 = new XElement("Row",
                                new XElement("col", detailList[0].custsuperme, new XAttribute("title", "客戶/賓客最高主管姓名/職稱")),
                                new XElement("col", detailList[0].otherobject, new XAttribute("title", "其他成员")),
                                new XElement("col", detailList[0].objectsum, new XAttribute("title", "总人数"))
                                );
                sectionNode4.Add(rowNode4);
                XElement sectionNode5 = new XElement("section", new XAttribute("title", "公司參加人員"),
                   new XAttribute("type", "LIST"));
                XElement rowNode5 = new XElement("Row",
                                new XElement("col", detailList[0].keep, new XAttribute("title", "公司最高主管姓名/職稱")),
                                new XElement("col", detailList[0].keepcategory, new XAttribute("title", "公司最高主管所屬类别")),
                                new XElement("col", detailList[0].otherkeep, new XAttribute("title", "其他成员")),
                                new XElement("col", detailList[0].keepsum, new XAttribute("title", "总人数"))
                                );
                sectionNode5.Add(rowNode5);
                XElement sectionNode2 = new XElement("section", new XAttribute("title", ""),
                   new XAttribute("type", "LIST"));
                string processMethodDescription = detailList[0].processmethod == "0"
                    ? "同仁自行负担(公司仅核准报销预算内之金额)"
                    : detailList[0].processmethod == "1"
                    ? "扣减部门年度绩效奖金预算"
                    : "";
                XElement rowNode2 = new XElement("Row",
                                new XElement("col", detailList[0].isaccordnumber == "Y" ? "符合" : "不符合", new XAttribute("title", "是否符合我方人數規範(註1)?")),
                                new XElement("col", detailList[0].amount1, new XAttribute("title", "依人數預算之總預算")),
                                new XElement("col", detailList[0].paymentexpense, new XAttribute("title", "實際支出")),
                                new XElement("col", detailList[0].isaccordcost == "Y" ? "符合" : "不符合", new XAttribute("title", "是否符合費用規範(註2)?")),
                                new XElement("col", detailList[0].overbudget, new XAttribute("title", "超出預算金額共")),
                                new XElement("col", processMethodDescription, new XAttribute("title", "該金額之處理方式(註3)"))
                                );
                sectionNode2.Add(rowNode2);
                XElement sectionNode6 = new XElement("section", new XAttribute("title", ""),
                        new XAttribute("type", "LIST"));
                XElement rowNode6 = new XElement("Row",
                                new XElement("col", amount.amount, new XAttribute("title", "实际报销金额\r\n")));
                sectionNode6.Add(rowNode6);
                sectionNodeList.Add(sectionNode1);
                sectionNodeList.Add(sectionNode3);
                sectionNodeList.Add(sectionNode4);
                sectionNodeList.Add(sectionNode5);
                sectionNodeList.Add(sectionNode2);
                sectionNodeList.Add(sectionNode6);
            }
            return sectionNodeList;
        }
        public static XElement generateFormXMLForCash3(CashHeadDto head, CashAmountDto amount, bool enFlag)
        {
            if (enFlag)
            {
                XElement formNode = new XElement("section", new XAttribute("title", "Form details"),
                      new XAttribute("type", "LIST"));
                //表单明细
                XElement rowNode = new XElement("Row",
                                new XElement("col", new XAttribute("title", "Company code"), head.company),
                                new XElement("col", new XAttribute("title", "Applicant ID"), head.cuser),
                                new XElement("col", new XAttribute("title", "Applicant Name"), head.cname),
                                new XElement("col", new XAttribute("title", "Dept"), head.deptid),
                                new XElement("col", new XAttribute("title", "Expense project"), head.dtype),
                                new XElement("col", new XAttribute("title", "Project code"), head.projectcode),
                                new XElement("col", new XAttribute("title", "Payee ID"), head.payeeId),
                                new XElement("col", new XAttribute("title", "Payee Name"), head.payeename),
                                new XElement("col", new XAttribute("title", "Total Application Amount"), amount.currency + ":" + formatAmount(amount.amount, amount.currency)),
                                new XElement("col", new XAttribute("title", "Actual pay amount"), amount.currency + ":" + formatAmount(amount.actamt, amount.currency))
                            );
                formNode.Add(rowNode);
                return formNode;
            }
            else
            {
                XElement formNode = new XElement("section", new XAttribute("title", "表單明細"),
                      new XAttribute("type", "LIST"));
                //表单明细
                XElement rowNode = new XElement("Row",
                                new XElement("col", new XAttribute("title", "公司别"), head.company),
                                new XElement("col", new XAttribute("title", "申請人工號"), head.cuser),
                                new XElement("col", new XAttribute("title", "申請人姓名"), head.cname),
                                new XElement("col", new XAttribute("title", "部門代碼"), head.deptid),
                                new XElement("col", new XAttribute("title", "費用簽核項目"), head.dtype),
                                new XElement("col", new XAttribute("title", "Project code"), head.projectcode),
                                new XElement("col", new XAttribute("title", "收款人工號"), head.payeeId),
                                new XElement("col", new XAttribute("title", "收款人姓名"), head.payeename),
                                new XElement("col", new XAttribute("title", "申請金額總計"), amount.currency + ":" + formatAmount(amount.amount, amount.currency)),
                                new XElement("col", new XAttribute("title", "實際支付金額"), amount.currency + ":" + formatAmount(amount.actamt, amount.currency))
                            );
                formNode.Add(rowNode);
                return formNode;
            }
        }
        public static XElement generateDetailXMLForCash3(List<CashDetailDto> detailList, bool enFlag)
        {
            if (enFlag)
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "Reimbursement description"),
                      new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "Advance Situation"), detail.expname),
                                    new XElement("col", new XAttribute("title", "Required Payment Date"), detail.rdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "Digest"), detail.summary),
                                    new XElement("col", new XAttribute("title", "Advance Charge Against Date"), detail.revsdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "Request Payment"), detail.payname),
                                    new XElement("col", new XAttribute("title", "Currency"), detail.currency),
                                    new XElement("col", new XAttribute("title", "Applied Amount"), detail.amount1),
                                    new XElement("col", new XAttribute("title", "Conversion to local currency"), detail.baseamt),
                                    new XElement("col", new XAttribute("title", "Exchange Rate"), detail.rate),
                                    new XElement("col", new XAttribute("title", "Remark"), detail.remarks)
                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
            else
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "報銷描述"),
                      new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "預支情景"), detail.expname),
                                    new XElement("col", new XAttribute("title", "需款日期"), detail.rdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "摘要"), detail.summary),
                                    new XElement("col", new XAttribute("title", "預定沖銷日期"), detail.revsdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "請款方式"), detail.payname),
                                    new XElement("col", new XAttribute("title", "幣別"), detail.currency),
                                    new XElement("col", new XAttribute("title", "申請金額"), detail.amount1),
                                    new XElement("col", new XAttribute("title", "折算本位幣"), detail.baseamt),
                                    new XElement("col", new XAttribute("title", "匯率"), detail.rate),
                                    new XElement("col", new XAttribute("title", "備註"), detail.remarks)
                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
        }
        public static XElement generateFormXMLForCash4(CashHeadDto head, CashAmountDto amount, List<CashDetailDto> detailList, bool enFlag)
        {
            if (enFlag)
            {
                XElement formNode = new XElement("section", new XAttribute("title", "Form details"),
                                      new XAttribute("type", "LIST"));
                //表单明细
                XElement rowNode = new XElement("Row",
                                new XElement("col", new XAttribute("title", "Company code"), head.company),
                                new XElement("col", new XAttribute("title", "Reimbursement"), detailList[0].expname),
                                new XElement("col", new XAttribute("title", "Emplid"), head.cuser),
                                new XElement("col", new XAttribute("title", "Dept"), head.deptid),
                                new XElement("col", new XAttribute("title", "Name"), head.cname),
                                new XElement("col", new XAttribute("title", "Expense project"), head.dtype),
                                new XElement("col", new XAttribute("title", "Project code"), head.projectcode),
                                new XElement("col", new XAttribute("title", "Ext"), head.ext),
                                new XElement("col", new XAttribute("title", "Total reimbursement"), amount.currency + ":" + formatAmount(amount.amount, amount.currency)),
                                new XElement("col", new XAttribute("title", "Actual pay amount"), amount.currency + ":" + formatAmount(amount.actamt, amount.currency))
                            );
                formNode.Add(rowNode);
                return formNode;
            }
            else
            {
                XElement formNode = new XElement("section", new XAttribute("title", "表單明細"),
                      new XAttribute("type", "LIST"));
                //表单明细
                XElement rowNode = new XElement("Row",
                                new XElement("col", new XAttribute("title", "公司别"), head.company),
                                new XElement("col", new XAttribute("title", "報銷情景"), detailList[0].expname),
                                new XElement("col", new XAttribute("title", "工號"), head.cuser),
                                new XElement("col", new XAttribute("title", "部門代碼"), head.deptid),
                                new XElement("col", new XAttribute("title", "姓名"), head.cname),
                                new XElement("col", new XAttribute("title", "費用簽核項目"), head.dtype),
                                new XElement("col", new XAttribute("title", "Project code"), head.projectcode),
                                new XElement("col", new XAttribute("title", "分機"), head.ext),
                                new XElement("col", new XAttribute("title", "報銷總計"), amount.currency + ":" + formatAmount(amount.amount, amount.currency)),
                                new XElement("col", new XAttribute("title", "實際支付金額"), amount.currency + ":" + formatAmount(amount.actamt, amount.currency))
                            );
                formNode.Add(rowNode);
                return formNode;
            }
        }
        public static XElement generateDetailXMLForCash4(List<CashDetailDto> detailList, bool enFlag)
        {
            if (enFlag)
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "Reimbursement description"),
                                      new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "Date of expense"), detail.rdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "Receiver ID"), detail.payeeid),
                                    new XElement("col", new XAttribute("title", "Payee Name"), detail.payeename),
                                    new XElement("col", new XAttribute("title", "Payee Dept"), detail.payeedeptid),
                                    new XElement("col", new XAttribute("title", "Bank Name"), detail.bank),
                                    new XElement("col", new XAttribute("title", "Digest"), detail.summary),
                                    new XElement("col", new XAttribute("title", "Expense Attribution Dept"), detail.deptid),
                                    //new XElement("col", new XAttribute("title", "公出城市"), detail.city),
                                    //new XElement("col", new XAttribute("title", "出發時點"), detail.gotime?.ToString("HH:mm")),
                                    //new XElement("col", new XAttribute("title", "回到時點"), detail.backtime?.ToString("HH:mm")),
                                    new XElement("col", new XAttribute("title", "Currency"), detail.currency),
                                    new XElement("col", new XAttribute("title", "Reimbursement amount"), detail.amount1),
                                    new XElement("col", new XAttribute("title", "Exchange Rate"), detail.rate),
                                    new XElement("col", new XAttribute("title", "Conversion to local currency"), detail.baseamt),
                                      new XElement("col", new XAttribute("title", "Tax deductible"), detail.taxexpense),
                                    new XElement("col", new XAttribute("title", "Blank form remarks"), detail.assignment)
                                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
            else
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "報銷描述"),
                      new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "費用發生日期"), detail.rdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "收款人工號"), detail.payeeid),
                                    new XElement("col", new XAttribute("title", "收款人"), detail.payeename),
                                    new XElement("col", new XAttribute("title", "收款人部門"), detail.payeedeptid),
                                    new XElement("col", new XAttribute("title", "銀行名稱"), detail.bank),
                                    new XElement("col", new XAttribute("title", "摘要"), detail.summary),
                                    new XElement("col", new XAttribute("title", "費用歸屬部門"), detail.deptid),
                                    //new XElement("col", new XAttribute("title", "公出城市"), detail.city),
                                    //new XElement("col", new XAttribute("title", "出發時點"), detail.gotime?.ToString("HH:mm")),
                                    //new XElement("col", new XAttribute("title", "回到時點"), detail.backtime?.ToString("HH:mm")),
                                    new XElement("col", new XAttribute("title", "幣別"), detail.currency),
                                    new XElement("col", new XAttribute("title", "報銷金額"), detail.amount1),
                                    new XElement("col", new XAttribute("title", "匯率"), detail.rate),
                                    new XElement("col", new XAttribute("title", "折算本位幣"), detail.baseamt),
                                    new XElement("col", new XAttribute("title", "可抵扣税额"), detail.taxexpense),
                                    new XElement("col", new XAttribute("title", "白单备注"), detail.assignment)
                                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
        }
        public static XElement generateFormXMLForCash3A(CashHeadDto head, CashAmountDto amount, bool enFlag)
        {
            if (enFlag)
            {
                XElement formNode = new XElement("section", new XAttribute("title", "Form details"),
                        new XAttribute("type", "LIST"));
                //表单明细
                XElement rowNode = new XElement("Row",
                                new XElement("col", new XAttribute("title", "Company code"), head.company),
                                new XElement("col", new XAttribute("title", "Emplid"), head.cuser),
                                new XElement("col", new XAttribute("title", "Applicant"), head.cname),
                                new XElement("col", new XAttribute("title", "Dept"), head.deptid),
                                new XElement("col", new XAttribute("title", "Ext"), head.ext)
                            );
                formNode.Add(rowNode);
                return formNode;
            }
            else
            {
                XElement formNode = new XElement("section", new XAttribute("title", "表單明細"),
                       new XAttribute("type", "LIST"));
                //表单明细
                XElement rowNode = new XElement("Row",
                                new XElement("col", new XAttribute("title", "公司别"), head.company),
                                new XElement("col", new XAttribute("title", "工號"), head.cuser),
                                new XElement("col", new XAttribute("title", "申請人"), head.cname),
                                new XElement("col", new XAttribute("title", "部門代碼"), head.deptid),
                                new XElement("col", new XAttribute("title", "分機"), head.ext)
                            );
                formNode.Add(rowNode);
                return formNode;
            }
        }
        public static XElement generateDetailXMLForCash3A(List<CashDetailDto> detailList, bool enFlag)
        {
            if (enFlag)
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "Reimbursement description"),
                                     new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "Advance Fund RNo"), detail.advancerno),
                                    new XElement("col", new XAttribute("title", "Digest"), detail.summary),
                                    new XElement("col", new XAttribute("title", "Applied Amount"), detail.amount1),
                                    new XElement("col", new XAttribute("title", "Not Charge Against Amount"), detail.amount2),
                                    new XElement("col", new XAttribute("title", "Scheduled debit date"), detail.revsdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "Open Days"), (int)Math.Ceiling(((TimeSpan)(DateTime.Now - detail.cdate)).TotalDays)),
                                    new XElement("col", new XAttribute("title", "Days of Delay Charge Against"), detail.delaydays),
                                    new XElement("col", new XAttribute("title", "Delay after the scheduled date of debit"), detail.revsdate?.AddDays(detail.delaydays).ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "Delay Reason"), detail.delayreason)
                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
            else
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "報銷描述"),
                      new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "預支金單號"), detail.advancerno),
                                    new XElement("col", new XAttribute("title", "摘要"), detail.summary),
                                    new XElement("col", new XAttribute("title", "申請金額"), detail.amount1),
                                    new XElement("col", new XAttribute("title", "未沖帳金額"), detail.amount2),
                                    new XElement("col", new XAttribute("title", "預定沖帳日期"), detail.revsdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "open天數"), (int)Math.Ceiling(((TimeSpan)(DateTime.Now - detail.cdate)).TotalDays)),
                                    new XElement("col", new XAttribute("title", "延期沖帳天數"), detail.delaydays),
                                    new XElement("col", new XAttribute("title", "延期後預定沖帳日期"), detail.revsdate?.AddDays(detail.delaydays).ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "延期原因"), detail.delayreason)
                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
        }
        public static XElement generateFormXMLForCash6(CashHeadDto head, CashAmountDto amount, bool enFlag)
        {
            if (enFlag)
            {
                XElement formNode = new XElement("section", new XAttribute("title", "Form details"),
                       new XAttribute("type", "LIST"));
                //表单明细
                XElement rowNode = new XElement("Row",
                                new XElement("col", new XAttribute("title", "Company code"), head.company),
                                new XElement("col", new XAttribute("title", "Expense project"), head.dtype),
                                new XElement("col", new XAttribute("title", "Project code"), head.projectcode),
                                new XElement("col", new XAttribute("title", "Payee ID"), head.payeeId),
                                new XElement("col", new XAttribute("title", "Name"), head.payeename),
                                new XElement("col", new XAttribute("title", "Total Amount"), amount.currency + ":" + formatAmount(amount.amount, amount.currency)),
                                new XElement("col", new XAttribute("title", "Actual reimbursable amount"), amount.currency + ":" + formatAmount(amount.actamt, amount.currency)),
                                new XElement("col", new XAttribute("title", "Actual pay amount"), amount.currency + ":" + formatAmount(amount.actamt, amount.currency))
                            );
                formNode.Add(rowNode);
                return formNode;
            }
            else
            {
                XElement formNode = new XElement("section", new XAttribute("title", "表單明細"),
                       new XAttribute("type", "LIST"));
                //表单明细
                XElement rowNode = new XElement("Row",
                                new XElement("col", new XAttribute("title", "公司别"), head.company),
                                new XElement("col", new XAttribute("title", "費用簽核項目"), head.dtype),
                                new XElement("col", new XAttribute("title", "Project code"), head.projectcode),
                                new XElement("col", new XAttribute("title", "收款人工號"), head.payeeId),
                                new XElement("col", new XAttribute("title", "姓名"), head.payeename),
                                new XElement("col", new XAttribute("title", "費用總計"), amount.currency + ":" + formatAmount(amount.amount, amount.currency)),
                                new XElement("col", new XAttribute("title", "實際可報銷金額"), amount.currency + ":" + formatAmount(amount.actamt, amount.currency)),
                                new XElement("col", new XAttribute("title", "實際支付金額"), amount.currency + ":" + formatAmount(amount.actamt, amount.currency))
                            );
                formNode.Add(rowNode);
                return formNode;
            }
        }
        public static XElement generateDetailXMLForCash6(List<CashDetailDto> detailList, bool enFlag)
        {
            if (enFlag)
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "Reimbursement description"),
                                      new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    decimal individualAffordAmt = 0;
                    decimal companyAffordAmt = 0;
                    decimal selfTaxAmt = 0;
                    detail.invList.ForEach(i =>
                    {
                        if (i.undertaker == "self")
                        {
                            individualAffordAmt += i.amount;
                            selfTaxAmt += i.taxloss;
                        }
                        else if (i.undertaker == "company")
                        {
                            companyAffordAmt += i.amount;
                        }
                    });
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "Date"), detail.rdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "Expense category"), detail.expname),
                                    new XElement("col", new XAttribute("title", "Digest"), detail.summary),
                                    new XElement("col", new XAttribute("title", "Expense Attribution Dept"), detail.deptid),
                                    new XElement("col", new XAttribute("title", "Currency"), detail.currency),
                                    new XElement("col", new XAttribute("title", "Applied Amount"), detail.amount1),
                                    new XElement("col", new XAttribute("title", "Exchange Rate"), detail.rate),
                                    new XElement("col", new XAttribute("title", "Conversion to local currency"), detail.baseamt),
                                    new XElement("col", new XAttribute("title", "Remark"), detail.remarks),
                                    new XElement("col", new XAttribute("title", "Individual responsibility for taxes"), selfTaxAmt),
                                    new XElement("col", new XAttribute("title", "Actual reimbursable amount"), detail.amount),
                                    new XElement("col", new XAttribute("title", "Blank form remarks"), detail.assignment)
                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
            else
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "報銷描述"),
                                      new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    decimal individualAffordAmt = 0;
                    decimal companyAffordAmt = 0;
                    decimal selfTaxAmt = 0;
                    detail.invList.ForEach(i =>
                    {
                        if (i.undertaker == "self")
                        {
                            individualAffordAmt += i.amount;
                            selfTaxAmt += i.taxloss;
                        }
                        else if (i.undertaker == "company")
                        {
                            companyAffordAmt += i.amount;
                        }
                    });
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "日期"), detail.rdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "費用類別"), detail.expname),
                                    new XElement("col", new XAttribute("title", "摘要"), detail.summary),
                                    new XElement("col", new XAttribute("title", "費用歸屬部門"), detail.deptid),
                                    new XElement("col", new XAttribute("title", "幣別"), detail.currency),
                                    new XElement("col", new XAttribute("title", "申請金額"), detail.amount1),
                                    new XElement("col", new XAttribute("title", "匯率"), detail.rate),
                                    new XElement("col", new XAttribute("title", "折算本位幣"), detail.baseamt),
                                    new XElement("col", new XAttribute("title", "備註"), detail.remarks),
                                    new XElement("col", new XAttribute("title", "個人承擔稅金"), selfTaxAmt),
                                    new XElement("col", new XAttribute("title", "實際可報銷金額"), detail.amount),
                                    new XElement("col", new XAttribute("title", "白单备注"), detail.assignment)
                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
        }
        public static XElement generateFormXMLForCashX(CashHeadDto head, CashAmountDto amount, bool enFlag)
        {
            if (enFlag)
            {
                XElement formNode = new XElement("section", new XAttribute("title", "Form details"),
                        new XAttribute("type", "LIST"));
                //表单明细
                XElement rowNode = new XElement("Row",
                                new XElement("col", new XAttribute("title", "Company code"), head.company),
                                new XElement("col", new XAttribute("title", "Expense project"), head.dtype),
                                new XElement("col", new XAttribute("title", "Project code"), head.projectcode),
                                new XElement("col", new XAttribute("title", "Payee ID"), head.payeeId),
                                new XElement("col", new XAttribute("title", "Name"), head.payeename),
                                new XElement("col", new XAttribute("title", "Total Application Amount"), amount.currency + ":" + formatAmount(amount.amount, amount.currency)),
                                new XElement("col", new XAttribute("title", "Actual pay amount"), formatAmount(amount.actamt, amount.currency))
                            );
                formNode.Add(rowNode);
                return formNode;
            }
            else
            {
                XElement formNode = new XElement("section", new XAttribute("title", "表單明細"),
                         new XAttribute("type", "LIST"));
                //表单明细
                XElement rowNode = new XElement("Row",
                                new XElement("col", new XAttribute("title", "公司别"), head.company),
                                new XElement("col", new XAttribute("title", "費用簽核項目"), head.dtype),
                                new XElement("col", new XAttribute("title", "Project code"), head.projectcode),
                                new XElement("col", new XAttribute("title", "收款人工號"), head.payeeId),
                                new XElement("col", new XAttribute("title", "姓名"), head.payeename),
                                new XElement("col", new XAttribute("title", "申請金額總計"), amount.currency + ":" + formatAmount(amount.amount, amount.currency)),
                                new XElement("col", new XAttribute("title", "實際支付金額"), formatAmount(amount.actamt, amount.currency))
                            );
                formNode.Add(rowNode);
                return formNode;
            }
        }
        public static XElement generateDetailXMLForCashX(List<CashDetailDto> detailList, bool enFlag)
        {
            if (enFlag)
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "Reimbursement description"),
                      new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "Company"), detail.companycode),
                                    new XElement("col", new XAttribute("title", "Reimbursement scene"), detail.expname),
                                    new XElement("col", new XAttribute("title", "Required Payment Date"), detail.rdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "Pay period"), detail.summary),
                                    new XElement("col", new XAttribute("title", "bank"), detail.bank),
                                    new XElement("col", new XAttribute("title", "Request Payment"), detail.payname),
                                    new XElement("col", new XAttribute("title", "Currency"), detail.currency),
                                    new XElement("col", new XAttribute("title", "Applied Amount"), detail.amount1),
                                    new XElement("col", new XAttribute("title", "Payment Amount"), detail.amount),
                                    new XElement("col", new XAttribute("title", "Digest"), detail.remarks)
                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
            else
            {
                XElement detailNode = new XElement("section", new XAttribute("title", "報銷描述"),
                      new XAttribute("type", "table"));
                List<XElement> detailNodeList = new List<XElement>();
                detailList.ForEach(detail =>
                {
                    //detail明细
                    XElement rowNode = new XElement("Row",
                                    new XElement("col", new XAttribute("title", "公司"), detail.companycode),
                                    new XElement("col", new XAttribute("title", "報銷情景"), detail.expname),
                                    new XElement("col", new XAttribute("title", "需款日期"), detail.rdate?.ToString("yyyy/MM/dd")),
                                    new XElement("col", new XAttribute("title", "薪資所屬期"), detail.summary),
                                    new XElement("col", new XAttribute("title", "銀行"), detail.bank),
                                    new XElement("col", new XAttribute("title", "請款方式"), detail.payname),
                                    new XElement("col", new XAttribute("title", "幣別"), detail.currency),
                                    new XElement("col", new XAttribute("title", "申請金額"), detail.amount1),
                                    new XElement("col", new XAttribute("title", "支付金額"), detail.amount),
                                    new XElement("col", new XAttribute("title", "摘要"), detail.remarks)
                    );
                    detailNodeList.Add(rowNode);
                });
                detailNode.Add(detailNodeList);
                return detailNode;
            }
        }
        //Now日期转变
        public static string GetNowDateToString()
        {
            DateTime date = DateTime.Now;
            string date2 = date.ToString("yyyyMMddhhmmss");
            return date2;
        }
        //金额转换
        public static string formatAmount(decimal amount, string currency)
        {
            if (currency == "NTD")
            {
                // 台币金额，不保留小数并加入千分位
                return amount.ToString("N0");
            }
            else
            {
                // 其他金额，保留两位小数并加入千分位
                return amount.ToString("N2");
            }
        }
        public static async Task<Stream> GetFileStreamAsync(string url)
        {
            // 创建HTTPClient实例
            using (HttpClient client = new HttpClient())
            {
                // 发送GET请求并获取响应
                HttpResponseMessage response = await client.GetAsync(url);
                // 确保响应成功
                response.EnsureSuccessStatusCode();
                // 读取响应内容的流
                Stream stream = await response.Content.ReadAsStreamAsync();
                return stream;
            }
        }
        public static async Task<FileInfo> GetFileInfoFromUrlAsync(string url, string fileName)
        {
            // 创建一个临时文件名
            string tempFilePath = fileName;
            // 使用 HttpClient 下载文件
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                using (var fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }
            // 使用本地文件路径创建 FileInfo 对象
            FileInfo fileInfo = new FileInfo(tempFilePath);
            return fileInfo;
        }
        public async Task<string> test(string rno)
        {
            await SendMobileSignXMLData(rno);
            return "";
        }
    }
}
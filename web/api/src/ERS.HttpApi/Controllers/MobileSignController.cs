using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using ERS.DTO;
using ERS.DTO.MobileSign;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.Model.MobileSign;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUglify.Helpers;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class MobileSignController
    {
        private IMobileSignService _mobileSignService;
        private  IHttpContextAccessor _httpContextAccessor;
        private IConfiguration _configuration;
        private IHttpClientFactory _HttpClient;
        private ISendMobileLogRepository _sendMobileLogRepository;
        private IApprovalDomainService _approvalDomainService;
        private IAppConfigRepository _appConfigRepository;
        private ILogger<MobileSignController> _logger;
        public MobileSignController(IMobileSignService mobileSignService,
             IAppConfigRepository appConfigRepository,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IHttpClientFactory HttpClient,
            ISendMobileLogRepository sendMobileLogRepository,
            IApprovalDomainService approvalDomainService,
            ILogger<MobileSignController> logger)
        {
            _appConfigRepository = appConfigRepository;
            _mobileSignService = mobileSignService;
            _httpContextAccessor = httpContextAccessor;
            _configuration= configuration;
            _HttpClient = HttpClient;
            _sendMobileLogRepository=   sendMobileLogRepository;
            _approvalDomainService= approvalDomainService;
            _logger= logger;
        }
        [HttpPost("callback")]
        public async Task<MobileOutput> CallbackAsync()
        {
            // 在这里获取请求正文的值
            string requestBody = "";
            HttpContext context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                using (StreamReader reader = new StreamReader(context.Request.Body))
                {
                     requestBody = await reader.ReadToEndAsync();
                }
            }
            string signStatus = "";
            // 创建一个XmlDocument对象并加载XML字符串
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(requestBody);
            // 使用XPath选择器选择具体的字段
            XmlNode formIdNode = xmlDoc.SelectSingleNode("//FORMDATA/FORMID");
            string rno = formIdNode?.InnerText;
            XmlNode formCodeNode = xmlDoc.SelectSingleNode("//FORMDATA/FORMTYPE");
            string formCode = formCodeNode?.InnerText;
            XmlNode fromsitenode = xmlDoc.SelectSingleNode("//FORMDATA/FROMSITE");
            string fromsite = fromsitenode?.InnerText;
            XmlNode userIdNode = xmlDoc.SelectSingleNode("//FORMDATA/USERID");
            string userid = userIdNode?.InnerText;
            XmlNode signresultNode = xmlDoc.SelectSingleNode("//FORMDATA/CHKRESULT");
            string signresult = signresultNode?.InnerText;
            XmlNode messageIdNode = xmlDoc.SelectSingleNode("//FORMDATA/MESSAGEID");
            string messageId = messageIdNode?.InnerText;
            XmlNode signRemarkNode = xmlDoc.SelectSingleNode("//FORMDATA/APPRSUMMARY");
            string signRemark = signRemarkNode?.InnerText;
            // 以此类推，从XML文档中选择和提取其他的字段
            _logger.LogDebug("手机签核回调参数抽取 {Rno}, {SignResult}, {SignRemark}, {MessageId}, {UserId}", rno, signresult, signRemark, messageId, userid);
            AppConfig IDMUser = await _appConfigRepository.GetValue("IDMUser");
            AppConfig IDMPassword = await _appConfigRepository.GetValue("IDMPassword");
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", _configuration.GetSection("MCP:IDM_client_id").Value),
                new KeyValuePair<string, string>("client_secret", _configuration.GetSection("MCP:IDM_client_secret").Value),
                new KeyValuePair<string, string>("grant_type", _configuration.GetSection("MCP:IDM_grant_type").Value),
                new KeyValuePair<string, string>("username",IDMUser.value),
                new KeyValuePair<string, string>("password", IDMPassword.value),
            };
            HttpClient httpClient = _HttpClient.CreateClient();
            IDMTokenResponse response = await httpClient.PostHelperAsync<IDMTokenResponse>(_configuration.GetSection("IDM:GetIDMToken").Value, formData);
            SignDto signDto = new()
            {
                company = fromsite,
                rno = rno,
                formCode = formCode,
                remark= signRemark
            };
            Result<string> res = null;
            IList<SendMobileLog> sendMobileLogs = await _sendMobileLogRepository.GetDataBySignstatusAndRno("C",rno);
            if (signresult == "Approve")
            {
                if (signDto.formCode=="CASH_X")
                {
                    var signData = new Dictionary<string, string>
                    {
                        { "company", fromsite },
                        { "remark",signRemark },
                        { "rno", rno }
                     };
                    string signJson = JsonConvert.SerializeObject(signData);
                    // 创建FormCollection并装入表单数据
                    IFormCollection formCollection = new FormCollection(
                        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                        {
                            { "sign", new Microsoft.Extensions.Primitives.StringValues(signJson) }
                        }
                    );
                    res = await _approvalDomainService.CashXApproval(formCollection, userid, response.token_type + " " + response.access_token, "mobile");
                }
                else
                {
                    res = await _approvalDomainService.Approval(signDto, userid, response.token_type + " " + response.access_token, "mobile");
                }
                sendMobileLogs.ForEach(sendMobileLog=>{
                    sendMobileLog.signstatus = "P";
                });
            }
            else
            {
                res = await _approvalDomainService.Reject(signDto, userid, response.token_type + " " + response.access_token, "mobile");
                sendMobileLogs.ForEach(sendMobileLog => {
                    sendMobileLog.signstatus = "R";
                });
            }
            MobileOutput mobileOutput = new MobileOutput();
            if (res != null)
            {
                if (res.status == 1)
                {
                    mobileOutput.rtnCode = 1;
                    //修改状态 手机签的默认已经抽单
                    sendMobileLogs.ForEach(sendMobileLog => {
                        sendMobileLog.recallstatus = "M";
                    });
                    await _sendMobileLogRepository.UpdateManyAsync(sendMobileLogs);
                }
                else
                {
                    mobileOutput.rtnCode = -1;
                    mobileOutput.rtnMsg = res.message;
                }
            }
            return mobileOutput;
        }
        [HttpPost("test")]
        public Task<string> Test([FromBody] string rno)
        {
            return _mobileSignService.test(rno);
        }
        [HttpPost("recalllForm")]
        public async Task<Result<string>> RecalllForm([FromBody] string rno)
        {
            return await _mobileSignService.recalllForm(rno);
        }
        [HttpPost("sendMobileSign")]
        public async Task<Result<string>> SendMobileSignXMLData(string rno)
        {
            return await _mobileSignService.SendMobileSignXMLData(rno);
        }
        [HttpPost("recalllFormAll")]
        public async Task<Result<string>> RecalllFormAll()
        {
            return await _mobileSignService.recalllFormAll();
        }
    }
}
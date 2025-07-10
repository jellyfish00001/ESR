using ERS.DTO;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
namespace ERS.DomainServices
{
    public class BPMDomainService : CommonDomainService, IBPMDomainService
    {
        private IHttpClientFactory _HttpClient;
        private IConfiguration _configuration;
        private ILogger<BPMDomainService> _logger;
        private IAppConfigRepository _AppConfigRepository;
        public BPMDomainService(IHttpClientFactory HttpClient, IConfiguration configuration, ILogger<BPMDomainService> logger, IAppConfigRepository AppConfigRepository)
        {
            _HttpClient = HttpClient;
            _configuration = configuration;
            _logger = logger;
            _AppConfigRepository = AppConfigRepository;
        }
        /// <summary>
        /// BPM:获取个人待签核单据
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="userid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ERS.Common.BPMException"></exception>
        public async Task<Result<IList<SignForm>>> GetBPMSelfPendingForm(string userid, string token)
        {
            Result<IList<SignForm>> result = new();
            result.status = 2;
            var param = new
            {
                systemCode = "ers",
                userid = userid
            };
            string datas = JsonConvert.SerializeObject(param);
            HttpClient httpClient = _HttpClient.CreateClient();
            BPMResult<IList<SignForm>> response = await httpClient.GetHelperAsync<BPMResult<IList<SignForm>>>(_configuration.GetSection("BPM:selfPendingSign").Value, datas, token: token);
            if (response == null)
            {
                throw new ERS.Common.BPMException("BPM:获取待签请求失败。 status: !200； data: " + datas);
            }
            else if (response.status == "success")
            {
                _logger.LogInformation("BPM获取待签。入参： {Datas}。出参：{Response}", datas, JsonConvert.SerializeObject(response));
                result.status = 1;
                result.data = response.data;
            }
            else
            {
                _logger.LogError("BPM:获取待签请求失败。 status: {Status}; msg: {Msg}; data: {Datas}", response.status, response.msg, datas);
                throw new ERS.Common.BPMException("BPM:获取待签请求失败。 status:" + response.status + "; " + response.msg);
            }
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
        public async Task<Result<string>> BPMTransform(SignDto data, string cuser, string token)
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
            BPMResult<SignResult> response = await httpClient.PostHelperAsync<BPMResult<SignResult>>(_configuration.GetSection("BPM:cancelsign").Value, datas, token: token);
            if (response == null)
            {
                throw new ERS.Common.BPMException("BPM:取消签核请求失败。 status: !200； data: " + datas);
            }
            else if (response.status == "success")
            {
                _logger.LogInformation("BPM转单。入参： {Datas}。出参：{Response}", datas, JsonConvert.SerializeObject(response));
                result.status = 1;
            }
            else
            {
                _logger.LogError("BPM:取消签核请求失败。 status: {Status}; msg: {Msg}; data: {Datas}", response.status, response.msg, datas);
                throw new ERS.Common.BPMException("BPM:取消签核请求失败。 status:" + response.status + "; " + response.msg);
            }
            return result;
        }
        /// <summary>
        /// BPM取消签核
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cuser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ERS.Common.BPMException"></exception>
        public async Task<Result<string>> BPMCancel(SignDto data, string cuser, string token)
        {
            Result<string> result = new();
            result.status = 2;
            CreateSign sign = new();
            sign.SetFormCode(data.formCode);
            sign.SetRno(data.rno);
            sign.SetSignUser(cuser);
            var param = new
            {
                systemCode = sign.systemCode,
                formCode = sign.formCode,
                formno = sign.formno
            };
            string datas = JsonConvert.SerializeObject(param);
            HttpClient httpClient = _HttpClient.CreateClient();
            BPMResult<SignResult> response = await httpClient.GetHelperAsync<BPMResult<SignResult>>(_configuration.GetSection("BPM:cancelsign").Value, datas, token: token);
            if (response == null)
            {
                throw new ERS.Common.BPMException("BPM:取消签核请求失败。 status: !200； data: " + datas);
            }
            else if (response.info == "success")
            {
                _logger.LogInformation("BPM取消。入参： {Datas}。出参：{Response}", datas, JsonConvert.SerializeObject(response));
                result.status = 1;
            }
            else
            {
                _logger.LogError("BPM:取消签核请求失败。 status: {Info}; msg: {Msg}; data: {Datas}", response.info, response.msg, datas);
                throw new ERS.Common.BPMException("BPM:取消签核请求失败。 status:" + response.info + "; " + response.msg);
            }
            return result;
        }
        /// <summary>
        /// 查询BPM签核明细
        /// </summary>
        /// <param name="rno"></param>
        /// <param name="formcode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ERS.Common.BPMException"></exception>
        public async Task<Result<IList<SignForm>>> BPMQuerySign(string rno, string formcode, string token)
        {
            Result<IList<SignForm>> result = new Result<IList<SignForm>>();
            result.status = 2;
            SignQuery data = new()
            {
                li = new List<FormMsg>()
                {
                    new FormMsg()
                    {
                        formNo = rno,
                        formCode = formcode
                    }
                }
            };
            string datas = JsonConvert.SerializeObject(data);
            HttpClient httpClient = _HttpClient.CreateClient();
            BPMResult<IList<SignForm>> response = await httpClient.PostHelperAsync<BPMResult<IList<SignForm>>>(_configuration.GetSection("BPM:querysign").Value, datas, token: token);
            if (response == null)
            {
                throw new ERS.Common.BPMException("BPM:查询签核请求失败。 status: !200； data: " + datas);
            }
            else if (response.info == "success")
            {
                if (response.data != null && response.data.Count > 0)
                {
                    result.data = response.data;
                    result.status = 1;
                    _logger.LogInformation("BPM查询结果： {Data}", response.data);
                }
            }
            else
            {
                _logger.LogError("BPM:查询签核请求失败。 status: {Info}; msg: {Msg}; data: {Datas}", response.info, response.msg, datas);
                throw new ERS.Common.BPMException("BPM:查询签核请求失败。 status:" + response.info + "; " + response.msg);
            }
            return result;
        }
        /// <summary>
        /// 请求bpm api产生签核
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ERS.Common.BPMException"></exception>
        public async Task<Result<string>> BPMCreateSign(CreateSign data, string token)
        {
            Result<string> result = new Result<string>();
            result.status = 2;
            string datas = JsonConvert.SerializeObject(data);
            HttpClient httpClient = _HttpClient.CreateClient();
            BPMResult<string> response = await httpClient.PostHelperAsync<BPMResult<string>>(_configuration.GetSection("BPM:createsign").Value, datas, token: token);
            if (response == null)
            {
                throw new ERS.Common.BPMException("BPM:生成签核请求失败。 status: !200； data: " + datas);
            }
            else if (response.info == "success")
            {
                _logger.LogInformation("BPM产生签核。入参： {Datas}。出参：{Response}", datas, JsonConvert.SerializeObject(response));
                result.status = 1;
            }
            else
            {
                _logger.LogError("BPM:生成签核请求失败。 status: {Info}; msg: {Msg}; data: {Datas}", response.info, response.msg, datas);
                throw new ERS.Common.BPMException("BPM:生成签核请求失败。 status:" + response.info + "; " + response.msg);
            }
            return result;
        }
        /// <summary>
        /// 是否已签核完
        /// </summary>
        /// <param name="rno"></param>
        /// <param name="formcode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<bool>> BPMQueryIsSigned(string rno, string formcode, string token)
        {
            Result<bool> result = new();
            if (_configuration.GetSection("UnitTest").Value == "true")
            {
                result.data = true;
                return result;
            }
            result.status = 2;
            Result<IList<SignForm>> data = await this.BPMQuerySign(rno, formcode, token);
            result = BPMQueryIsSigned(data);
            return result;
        }
        /// <summary>
        /// 是否已签核完
        /// </summary>
        /// <param name="rno"></param>
        /// <param name="formcode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Result<bool> BPMQueryIsSigned(Result<IList<SignForm>> data)
        {
            Result<bool> result = new();
            result.status = 2;
            if (data != null && data.status != 1)
            {
                result.message = data.message;
                return result;
            }
            result.data = data.data.Where(i => i.status != "Finish").Count() == 0;
            result.status = 1;
            return result;
        }
        /// <summary>
        /// 是否签核过
        /// </summary>
        /// <param name="rno"></param>
        /// <param name="formcode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result<bool>> BPMQueryHaveSign(string rno, string formcode, string token)
        {
            Result<bool> result = new();
            result.status = 2;
            Result<IList<SignForm>> data = await this.BPMQuerySign(rno, formcode, token);
            if (data != null && data.status != 1)
            {
                result.message = data.message;
                return result;
            }
            bool signed = false;
            var _data = data.data.Where(i => i.status != "Current" && i.status != "Not Approve").ToList();
            if (_data.Count == 0)
                signed = false;
            else if (_data.Count > 0 && _data.LastOrDefault().sign_result != "Return")
                signed = true;
            else if (_data.LastOrDefault().sign_result == "Return")
                signed = false;
            result.data = signed;
            result.status = 1;
            return result;
        }
    }
}

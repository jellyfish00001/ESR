using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net;
using ERS.Domain.DomainServices;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.IO;
using System.Net.Http.Headers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using ERS.Application.Contracts.DTO.Invoice;

namespace ERS.Application.Services
{
    /// <summary>
    /// 微软DI OCR服务类
    /// </summary>
    public class DIService : ApplicationService, IDIService
    {
        private IConfiguration _configuration;
        private readonly HttpClient _HttpClient;
        private ILogger<InvoiceDomainService> _logger;

        public DIService(IHttpClientFactory httpClient, IConfiguration configuration, ILogger<InvoiceDomainService> logger)
        {
            _HttpClient = httpClient.CreateClient();
            _configuration = configuration;
            _logger = logger;
        }

        private bool IsImageFile(IFormFile file) => file.ContentType.StartsWith("image/");


        // 判断是否为JSON
        private bool IsJson(string str)
        {
            try
            {
                // 尝试解析JSON字符串
                using (JsonDocument.Parse(str)) { }
                return true; // 如果解析成功，返回true
            }
            catch (System.Text.Json.JsonException)
            {
                return false; // 如果解析失败，返回false
            }
        }

        /// <summary>
        /// 将Value内的no_vaule替换为""
        /// </summary>
        /// <param name="token"></param>
        private void ReplaceValue(JToken token)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (var property in token.Children<JProperty>())
                {
                    if (property.Name == "value" && (string)property.Value == "no_vaule")
                    {
                        property.Value = "";
                    }
                    else
                    {
                        ReplaceValue(property.Value);
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in token.Children())
                {
                    ReplaceValue(item);
                }
            }
        }

        private Stream ReduceImageQuality(IFormFile file)
        {
            try
            {
                DateTime CurrentTime = DateTime.Now;
                _logger.LogInformation($"開始壓縮圖片大小 {CurrentTime.ToString("yyyy-MM-dd HH:mm:ss")}.");

                using var image = Image.Load(file.OpenReadStream());
                var memoryStream = new MemoryStream();
                var encoder = new JpegEncoder { Quality = 20 };
                if (file.ContentType.ToLower() == "image/png")
                {
                    image.SaveAsJpegAsync(memoryStream, encoder);
                }
                else
                {
                    image.Save(memoryStream, encoder);
                }

                memoryStream.Position = 0;

                CurrentTime = DateTime.Now;
                _logger.LogInformation($"壓縮完成 {CurrentTime.ToString("yyyy-MM-dd HH:mm:ss")}.");
                return memoryStream;
            }
            catch (Exception)
            {
                return file.OpenReadStream();
            }

        }

        /// <summary>
        /// 生成JWT令牌。
        /// </summary>
        /// <param name="emplid">员工ID。</param>
        /// <returns>返回生成的JWT令牌。</returns>
        public string GenerateToken(string emplid)
        {
            try
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.UniqueName, emplid),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
                };

                SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(this._configuration.GetSection("DIToken:Secret").Value));
                SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

                JwtSecurityToken token = new(
                    issuer: this._configuration.GetSection("DIToken:Issuer").Value,
                    audience: this._configuration.GetSection("DIToken:Audience").Value,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(int.Parse(this._configuration.GetSection("DIToken:Expiration").Value)),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 调用DI OCR API解析发票文件
        /// </summary>
        /// <param name="file">上传的文件</param>
        /// <param name="emplid">工号</param>
        /// <param name="dIInfo">DI配置</param>
        /// <returns>解析结果</returns>
        public async Task<string> AnalysisFile(IFormFile file, string emplid, DIInfo dIInfo)
        {
            try
            {
                //判斷獲取的用戶工號是否為空
                if (string.IsNullOrEmpty(emplid))
                {
                    //獲取的用戶工號為空時報錯
                    _logger.LogWarning("OCR解析,判斷提取的emplid為空.");
                    throw new Exception("emplid is null.");
                }

                //獲取的用戶工號不為空時,調用JWT Service生成新的Token
                string diToken = this.GenerateToken(emplid);

                //判斷新生成的Token是否為空
                if (string.IsNullOrEmpty(diToken))
                {
                    //新生成的Token為空時報錯
                    _logger.LogWarning("生成DI API所需的Token失敗.");
                    throw new Exception("Token generate fail.");
                }

                // 创建请求消息
                string url = this._configuration.GetSection("DIService:Url").Value;
                string method = this._configuration.GetSection("DIService:method").Value;
                string clientId = this._configuration.GetSection("DIService:clientId").Value;

                HttpRequestMessage requestMessage = new(HttpMethod.Post, $"{url}{method}");

                // 添加Token到请求头
                requestMessage.Headers.Add("Authorization", $"Bearer {diToken}");

                // 使用压缩后的流构建MultipartContent
                var multipartContent = new MultipartFormDataContent();

                var fileContent = new StreamContent(IsImageFile(file) ? ReduceImageQuality(file) : file.OpenReadStream());

                var fileType = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1).ToLower();
                var fileName = file.FileName.Substring(0, file.FileName.LastIndexOf("."));
                //DI OCR识别仅支持jpg，pdf和jpeg文件，所以要转换
                var newFileName = fileType == "png" ? fileName + ".jpg" : file.FileName; 

                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = newFileName
                };
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType == "image/png" ? "image/jpg" : file.ContentType);

                multipartContent.Add(fileContent, "file", newFileName);

                //添加body到multipartContent中
                multipartContent.Add(new StringContent(clientId), "clientId");
                multipartContent.Add(new StringContent(dIInfo.InvoiceRegion), "invoiceRegion");
                //OCR API发票类型已调整成非required参数
                //multipartContent.Add(new StringContent(dIInfo.InvoiceType), "*");

                requestMessage.Content = multipartContent;

                // 发送请求到第三方接口				
                DateTime start = DateTime.Now;
                _logger.LogInformation($"發送請求信息 {start.ToString("yyyy-MM-dd HH:mm:ss")}.");

                //呼叫OCR API
                HttpResponseMessage response = await _HttpClient.SendAsync(requestMessage);

                DateTime end = DateTime.Now;
                // 计算两个 DateTime 对象之间的差值
                TimeSpan timeSpan = end - start;
                // 获取差值的总秒数
                double totalSeconds = timeSpan.TotalSeconds;

                _logger.LogInformation($"{emplid} 請求OCR API解析發票 {start.ToString("yyyy-MM-dd HH:mm:ss")} ~ {end.ToString("yyyy-MM-dd HH:mm:ss")},共計 {totalSeconds} 秒,共計 {totalSeconds} 秒");

                //判断请求返回的Status Code
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //获取DI Service返回的数据
                    var res = await response.Content.ReadAsStringAsync();

                    if (IsJson(res))
                    {
                        //将数据转换格式
                        JObject parsedJson = JObject.Parse(res);

                        //替换掉数据内的no_value
                        ReplaceValue(parsedJson);

                        //return数据
                        return parsedJson.ToString();
                    }
                    else
                    {
                        _logger.LogWarning($"DIAnalysisFile返回的信息不是標準的Json: {res}.");
                        throw new Exception($"DIAnalysisFile返回的信息不是標準的Json: {res}.");
                    }
                }
                else
                {
                    //非200报错
                    _logger.LogError($"請求報錯 {response.RequestMessage?.ToString()}.");
                    throw new Exception(response.RequestMessage?.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"DIAnalysisFile Service Error: {e.Message?.ToString()}.");
                throw new Exception(e.Message?.ToString());
            }
        }
    }
}

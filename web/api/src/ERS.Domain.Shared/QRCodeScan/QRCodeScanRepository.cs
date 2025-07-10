using ERS.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;


namespace ERS.QRCodeScan
{
    public class QRCodeScanRepository : IQRCodeScanRepository, IScopedDependency
    {
        private IHttpClientFactory _HttpClient;
        private IConfiguration _Configuretion;
        private readonly ILogger<QRCodeScanRepository> _logger;

        public QRCodeScanRepository(IHttpClientFactory HttpClient, IConfiguration Configuretion, ILogger<QRCodeScanRepository> logger)
        {
            _HttpClient = HttpClient;
            _Configuretion = Configuretion;
            _logger = logger;
        }

        public async Task<Result<IList<string>>> Get(IFormFile file)
        {
            string api = _Configuretion.GetSection("QRCodeScan").Value;
            try
            {
                HttpClient httpClient = _HttpClient.CreateClient();
                MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
                multipartFormData.Add(new ByteArrayContent(FileHelper.StreamToBytes(file.OpenReadStream())), "file", file.FileName);
                Result<IList<string>> result = await httpClient.PostHelperAsync<Result<IList<string>>>(api, multipartFormData);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while scanning QR code.");

                return new Result<IList<string>>
                {
                    status = 2, // Indicate failure
                    message = $"{api} QRScan error occurred: {ex.Message}",
                    data = null
                };
            }
        }
        public async Task<Result<IList<string>>> Get(byte[] file, string FileName)
        {
            string api = _Configuretion.GetSection("QRCodeScan").Value;
            try
            {
                HttpClient httpClient = _HttpClient.CreateClient();
                MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
                multipartFormData.Add(new ByteArrayContent(file), "file", FileName);
                Result<IList<string>> result = await httpClient.PostHelperAsync<Result<IList<string>>>(api, multipartFormData);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while scanning QR code.");

                return new Result<IList<string>>
                {
                    status = 2, // Indicate failure
                    message = $"{api} QRScan error occurred: {ex.Message}",
                    data = null
                };
            }
        }
    }
}

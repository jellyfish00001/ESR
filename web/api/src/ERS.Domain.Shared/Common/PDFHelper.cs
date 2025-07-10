using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using Volo.Abp.DependencyInjection;

namespace ERS
{
    public class PDFHelper : IScopedDependency
	{
		public PDFHelper() { }
		private IConfiguration _configuration;
		private IHttpClientFactory _httpClientFactory;
		public PDFHelper(IConfiguration Configuretion, IHttpClientFactory HttpClient) {
			_configuration = Configuretion;
			_httpClientFactory = HttpClient;
		}

		public virtual byte[] ChangePdfToImg(byte[] bytes, string FileName)
        {
			string officeToPdfApi = _configuration.GetSection("PDFAPI:ToImg").Value;
			HttpClient httpClient = _httpClientFactory.CreateClient();
			MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
			multipartFormData.Add(new ByteArrayContent(bytes), "file", FileName);
			byte[] response = httpClient.GetFileToByteAsync(officeToPdfApi, multipartFormData).Result;
			return response;
		}

        public virtual Stream ChangeOfficeToPdf(IFormFile file)
        {
            string officeToPdfApi = _configuration.GetSection("PDFAPI:OfficeToPdf").Value;
            HttpClient httpClient = _httpClientFactory.CreateClient();
            MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
            multipartFormData.Add(new ByteArrayContent(FileHelper.StreamToBytes(file.OpenReadStream())), "file", file.FileName);
            Stream response = httpClient.GetFileToStreamAsync(officeToPdfApi, multipartFormData).Result;
            return response;
        }
		public virtual Stream ChangeOfficeToPdf(Stream file, string FileName)
		{
			string officeToPdfApi = _configuration.GetSection("PDFAPI:OfficeToPdf").Value;
			HttpClient httpClient = _httpClientFactory.CreateClient();
			MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
			multipartFormData.Add(new ByteArrayContent(FileHelper.StreamToBytes(file)), "file", FileName);
			Stream response = httpClient.GetFileToStreamAsync(officeToPdfApi, multipartFormData).Result;
			return response;
		}
		public virtual Stream ChangeExcelToPdf(IFormFile file)
		{
			string officeToPdfApi = _configuration.GetSection("PDFAPI:ExcelToPdf").Value;
			HttpClient httpClient = _httpClientFactory.CreateClient();
			MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
			multipartFormData.Add(new ByteArrayContent(FileHelper.StreamToBytes(file.OpenReadStream())), "file", file.FileName);
			Stream response = httpClient.GetFileToStreamAsync(officeToPdfApi, multipartFormData).Result;
			return response;
		}

		public Stream HtmlFileToPdf(IConfiguration Configuretion, IHttpClientFactory HttpClient, IFormFile file)
		{
			string officeToPdfApi = Configuretion["PDFAPI:HtmlFileToPdf"];
			HttpClient httpClient = HttpClient.CreateClient();
			MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
			multipartFormData.Add(new ByteArrayContent(FileHelper.StreamToBytes(file.OpenReadStream())), "file", Path.GetFileNameWithoutExtension(file.FileName) + ".html");
			Stream response = httpClient.GetFileToStreamAsync(officeToPdfApi, multipartFormData).Result;
			return response;
		}

		public virtual Stream HtmlFileToPdf(string filePath)
		{
			string officeToPdfApi = _configuration.GetSection("PDFAPI:HtmlFileToPdf").Value;
			HttpClient httpClient = _httpClientFactory.CreateClient();
			MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
			FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			multipartFormData.Add(new ByteArrayContent(FileHelper.StreamToBytes(fileStream)), "file", Path.GetFileNameWithoutExtension(filePath) + ".html");
			Stream response = httpClient.GetFileToStreamAsync(officeToPdfApi, multipartFormData).Result;
			return response;
		}
	}
}

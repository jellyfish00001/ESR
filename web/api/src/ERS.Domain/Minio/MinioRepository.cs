using ERS.Entities;
using ERS.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace ERS.Minio
{
    public class MinioRepository : IMinioRepository, IScopedDependency
    {
        private string _endpoint { get; set; }
        private string _accessKey { get; set; }
        private string _secretKey { get; set; }
        private bool _isSSL { get; set; }
        private string _bucketName { get; set; }

        private readonly IConfiguration _configuration;

        private readonly IAppConfigRepository _appConfigRepository;

        private readonly ILogger _logger;

        private MinioClient _minio;
        public MinioRepository(IConfiguration configuration, ILogger<MinioRepository> logger, IAppConfigRepository appConfigRepository)
        {
            this._configuration = configuration;
            this._logger = logger;
            _appConfigRepository = appConfigRepository;
        }

        public async Task MakeBucketAsync(string bucketName,string area)
        {
            this._minio = CreateNewMinioClient(area);
            await this._minio.MakeBucketAsync(bucketName);
        }

        public async Task<bool> BucketExistsAsync(string bucketName, string area)
        {
            this._minio = CreateNewMinioClient(area);
            return await this._minio.BucketExistsAsync(bucketName);
        }

        public async Task RemoveBucketAsync(string bucketName, string area)
        {
            this._minio = CreateNewMinioClient(area);
            await this._minio.RemoveBucketAsync(bucketName);
        }

        public async Task RemoveObjectAsync(string objectName, string area)
        {
            this._minio = CreateNewMinioClient(area);
            await this._minio.RemoveObjectAsync(this._bucketName,objectName);
        }


        public MemoryStream GetObjectAsync(string objectName, string area)
        {
            this._minio = CreateNewMinioClient(area);
            MemoryStream ms = new MemoryStream();
            this._minio.GetObjectAsync(this._bucketName, objectName, (stream) =>
            {
                stream.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
            }).Wait();

            return ms;
        }

        public async Task CopyObjectAsync(string srcObjectName, string destObjectName, string area)
        {
            this._minio = CreateNewMinioClient(area);
            bool found = await this._minio.BucketExistsAsync(this._bucketName);
            if (!found)
            {
                await this._minio.MakeBucketAsync(this._bucketName);
            }
            await this._minio.CopyObjectAsync(_bucketName, srcObjectName, _bucketName, destObjectName);
        }

        public async Task PutObjectAsync(string objectName, Stream data, string contentType, string area)
        {
            this._minio = CreateNewMinioClient(area);
            bool found = await this._minio.BucketExistsAsync(this._bucketName);
            if (!found)
            {
                await this._minio.MakeBucketAsync(this._bucketName);
            }

            try
            {
                await this._minio.PutObjectAsync(this._bucketName, objectName, data, data.Length, contentType);
            }
            catch (Exception ex)
            {
                throw new Exception($"{objectName} throw error :{ex.StackTrace}");
            }
        }

        public async Task PutObjectAsync(string objectName, string filePath, string contentType, string area)
        {
            this._minio = CreateNewMinioClient(area);
            bool found = await this._minio.BucketExistsAsync(this._bucketName);
            if (!found)
            {
                await this._minio.MakeBucketAsync(this._bucketName);
            }

            try
            {
                await this._minio.PutObjectAsync(this._bucketName, objectName, filePath, contentType);
            }
            catch (Exception ex)
            {
                throw new Exception($"{objectName} throw error :{ex.StackTrace}");
            }
        }

        public async Task<ObjectStat> StatObjectAsync(string objectName, string area)
        {
            this._minio = CreateNewMinioClient(area);
            return await this._minio.StatObjectAsync(this._bucketName, objectName);
        }

        public async Task<string> PresignedGetObjectAsync(string objectName, string area, int expiresInt = 3 * 24 * 60 * 60)
        {
            this._minio = CreateNewMinioClient(area);
            string url = await this._minio.PresignedGetObjectAsync(this._bucketName, objectName, expiresInt);
            return url;
        }

        public void DeleteFiles(string area, string objectPrefix = null, bool recursive = true)
        {
            this._minio = CreateNewMinioClient(area);
            IObservable<Item> observable = this._minio.ListObjectsAsync(this._bucketName, objectPrefix, recursive);
            IDisposable subscription = observable.Subscribe(
                item =>
                {
                    Console.WriteLine("OnNext: {0}", item.Key);
                    Task.Run(() =>
                    {
                        this._minio.RemoveObjectAsync(this._bucketName, item.Key);
                    }).Wait();
                },
                ex => Console.WriteLine("OnError: {0}", ex.Message),
                () => Console.WriteLine("OnComplete: {0}"));
        }

        private MinioClient CreateNewMinioClient(string area)
        {
            // Correcting the issue by awaiting the asynchronous method call  
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault; // 兼容；SSL/TLS 版本的选择依赖于操作系统配置  
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13; // 兼容  
            AppConfig appConfig = _appConfigRepository.GetValue("CNArea").Result;
            if (appConfig != null && appConfig.value.Contains(area))
            {
                this._bucketName = this._configuration.GetSection("MinioSettings:bucketName").Value;
                this._endpoint = this._configuration.GetSection("MinioSettings:endpoint").Value;
                this._accessKey = this._configuration.GetSection("MinioSettings:accessKey").Value;
                this._secretKey = this._configuration.GetSection("MinioSettings:secretKey").Value;
                this._isSSL = Convert.ToBoolean(this._configuration.GetSection("MinioSettings:isSSL").Value);
            }
            else
            {
                this._bucketName = this._configuration.GetSection("TWMinioSettings:bucketName").Value;
                this._endpoint = this._configuration.GetSection("TWMinioSettings:endpoint").Value;
                this._accessKey = this._configuration.GetSection("TWMinioSettings:accessKey").Value;
                this._secretKey = this._configuration.GetSection("TWMinioSettings:secretKey").Value;
                this._isSSL = Convert.ToBoolean(this._configuration.GetSection("TWMinioSettings:isSSL").Value);
            }
            if (this._isSSL)
            {
                return new MinioClient(this._endpoint, this._accessKey, this._secretKey).WithSSL();
            }
            else
            {
                return new MinioClient(this._endpoint, this._accessKey, this._secretKey);
            }
        }

        public string GetMIMEType(string extension)
        {
            string MIMEType = string.Empty;
            switch (extension.ToLower())
            {
                case "xls":
                    MIMEType = "application/vnd.ms-excel";
                    break;
                case "xlsx":
                    MIMEType = "application/vnd.ms-excel";
                    break;
                case "pdf":
                    MIMEType = "application/pdf";
                    break;
                case "html":
                    MIMEType = "text/html";
                    break;
                case "txt":
                    MIMEType = "text/plain";
                    break;
                case "doc":
                    MIMEType = "application/msword";
                    break;
                case "docx":
                    MIMEType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "jpg":
                    MIMEType = "image/jpeg";
                    break;
            }

            return MIMEType;
        }
    }
}

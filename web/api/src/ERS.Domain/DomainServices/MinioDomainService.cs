using ERS.Application.Contracts.DTO.Invoice;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.Minio;
using NPOI.POIFS.Crypt.Dsig;
using System;
using System.IO;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;
namespace ERS.DomainServices
{
    public class MinioDomainService : IMinioDomainService
    {
        private IMinioRepository _MinioRepository;

        private IBDCompanyCategoryDomainService _IBDCompanyCategoryDomainService;
        public MinioDomainService(IMinioRepository MinioRepository, IBDCompanyCategoryDomainService iBDCompanyCategoryDomainService)
        {
            _MinioRepository = MinioRepository;
            _IBDCompanyCategoryDomainService = iBDCompanyCategoryDomainService;
        }
        public async Task<string> PutObjectAsync(string rno, string filename, Stream data, string contentType,string area)
        {
            string objectName = "Request/" + DateTime.Now.ToString("yyyyMM") + "/" + rno + "/" + filename;
            await _MinioRepository.PutObjectAsync(objectName, data, contentType,area);
            return objectName;
        }
        public async Task<string> PutInvoiceAsync(string invno, string filename, Stream data, string contentType, string area)
        {
            string objectName = "Invoice/" + DateTime.Now.ToString("yyyyMM") + "/" + invno + "/" + filename;
            await _MinioRepository.PutObjectAsync(objectName, data, contentType,area);
            return objectName;
        }
        public async Task<string> CopyInvoiceAsync(string rno, string srcObjectName, string invno, string area)
        {
            string objectName = "Request/" + DateTime.Now.ToString("yyyyMM") + "/" + rno + "/" + "Invoices" +"/" + invno + "/" +Path.GetFileName(srcObjectName);
            await _MinioRepository.CopyObjectAsync(srcObjectName, objectName, area);
            return objectName;
        }

        public async Task<string> GetMinioArea(string userId)
        {
            string area = ""; //默认区域
            //根据登入者获取所属的区域
            var companyCategoryListResult = await _IBDCompanyCategoryDomainService.GetCategoryListByUserId(userId);
            if (companyCategoryListResult.data != null)
            {
                area = companyCategoryListResult.data[0].Area;
            }
            return area;
         }
    }
}

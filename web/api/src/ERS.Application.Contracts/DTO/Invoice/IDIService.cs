using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ERS.Application.Contracts.DTO.Invoice
{
    public interface IDIService
    {
        string GenerateToken(string emplid);
        Task<string> AnalysisFile(IFormFile file, string emplid, DIInfo dIInfo);
    }

    /// <summary>
    /// DI发票信息
    /// </summary>
    public class DIInfo
    {
        /// <summary>
        /// 发票区域
        /// </summary>
        public string InvoiceRegion { get; set; }

        /// <summary>
        /// 发票类型
        /// </summary>
        public string InvoiceType { get; set; }

        /// <summary>
        /// 币别
        /// </summary>
        public string Currency { get; set; }
    }
}
using ERS.Common;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERS.QRCodeScan
{
    public interface IQRCodeScanRepository
    {
        Task<Result<IList<string>>> Get(IFormFile file);
        Task<Result<IList<string>>> Get(byte[] file, string FileName);
    }
}

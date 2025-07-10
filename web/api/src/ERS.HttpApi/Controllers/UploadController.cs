using ERS.DomainServices;
using ERS.DTO;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    [Route("api/[controller]")]
    public class UploadController : BaseController
    {
        private IMinioDomainService _MinioDomainService;
        public UploadController(IMinioDomainService minioDomainService)
        {
            _MinioDomainService = minioDomainService;
        }

        [HttpPost]
        public async Task<string> UploadFile(IFormFile file, string company)
        {
            string area = await _MinioDomainService.GetMinioArea(this.userId);
            string path = "";
            string type = file.ContentType;
            string fileName = file.FileName;
            using (var steam = file.OpenReadStream())
            {
                path = await _MinioDomainService.PutObjectAsync("testrno", fileName, steam, type,area);
            }
            return path;
        }
    }
}

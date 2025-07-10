using ERS.DTO;
using ERS.DTO.BDVender;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERS.Controllers
{

    public class BDVenderController : BaseController
    {
        public IBDVenderDomainService _supplierSercive;

        public BDVenderController(IBDVenderDomainService supplierSercive)
        {
            _supplierSercive = supplierSercive;
        }

        [HttpPost("supplierinfo/view")]
        public async Task<Result<List<BDVenderParamDto>>> GetSupplierInfoByKeys([FromBody] Request<BDVenderParamDto> model) => await _supplierSercive.Search(model);

        [HttpPut("supplierinfo/edit")]
        public async Task<Result<string>> EditSupplierInfo([FromBody] BDVenderParamDto request)
        {
            return await _supplierSercive.UpdateSupplierInfo(request, this.userId);
        }

        [HttpPost("supplierinfo/add")]
        public async Task<Result<string>> AddSupplierInfo([FromBody] BDVenderParamDto request)
        {
            return await _supplierSercive.Create(request, this.userId);
        }

        [HttpDelete("supplierinfo/delete")]
        public async Task<Result<string>> DeleteSupplierInfo([FromBody]List<string> ids)
        {
            return await _supplierSercive.DeleteSupplier(ids);
        }

        [HttpPost("supplierinfo/download")]
        public async Task<Result<List<BDVenderParamDto>>> DownloadInfoByKeys([FromBody] Request<BDVenderParamDto> model) => await _supplierSercive.Download(model);

        [HttpGet("supplierinfo/list")]
        public async Task<Result<List<BDVenderParamDto>>> GetAllVendors() => await _supplierSercive.GetAllVendors();

    }
}
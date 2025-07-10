using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERS.Application.Contracts.DTO.Nickname;
using ERS.DTO;
using ERS.Attribute;
using System;
using ERS.DTO.Nickname;
using Microsoft.AspNetCore.Http;
namespace ERS.Controllers
{
#if DEBUG
    [AllowAnonymous]
#endif
    public class BDCustomerNicknameController :BaseController
    {
        private INicknameService _nicknameService;
        public BDCustomerNicknameController(INicknameService nicknameService)
        {
            _nicknameService=nicknameService;
        }
       [HttpGet("Nickname")]
       public  Task<List<NicknameDto>> GetNickname(string name,string company){
            return _nicknameService.GetNickname(name,company);
        }
        /// <summary>
        /// 获取全部客户昵称
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpGet("Nicknames")]
        public Task<List<NicknameDto>> GetNicknames(string company)
        {
            return _nicknameService.GetNameByCompany(company);
        }
        /// <summary>
        /// BaseData 查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("Nickname/query")]
        [Permission("ers.CustomerNickname.View")]
        public async Task<Result<List<NickNameCommonDto>>> Get([FromBody]Request<NickNameCommonDto> param) => await _nicknameService.Get(param);
        /// <summary>
        /// BaseData 新增
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("Nickname")]
        [Permission("ers.CustomerNickname.Add")]
        public async Task<Result<string>> Add([FromBody] NickNameCommonDto param) => await _nicknameService.Add(param, this.userId);
        /// <summary>
        /// BaseData 编辑
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPut("Nickname")]
        [Permission("ers.CustomerNickname.Edit")]
        public async Task<Result<string>> Update([FromBody] NickNameCommonDto param) => await _nicknameService.Update(param, this.userId);
        /// <summary>
        /// BaseData 删除
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpDelete("Nickname")]
        [Permission("ers.CustomerNickname.Delete")]
        public async Task<Result<string>> Delete([FromBody]List<Guid?> param) => await _nicknameService.Delete(param);
        /// <summary>
        /// BaseData 批量上傳
        /// </summary>
        /// <param name="excelFile"></param>
        /// <returns></returns>
        [HttpPost("Nickname/BatchUpload")]
        public async Task<Result<List<UploadNickNameDto>>> BatchUploadNickName(IFormFile excelFile) => await _nicknameService.BatchUploadNickName(excelFile,this.userId);
    }
}
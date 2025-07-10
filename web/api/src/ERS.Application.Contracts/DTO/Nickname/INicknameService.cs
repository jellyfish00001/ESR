using ERS.DTO;
using ERS.DTO.Nickname;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Application.Contracts.DTO.Nickname
{
    public interface INicknameService
    {
        Task<List<NicknameDto>> GetNickname(string name, string company);
        Task<List<NicknameDto>> GetNameByCompany(string company);
        Task<Result<List<NickNameCommonDto>>> Get(Request<NickNameCommonDto> param);
        Task<Result<string>> Add(NickNameCommonDto param, string cuser);
        Task<Result<string>> Update(NickNameCommonDto param, string cuser);
        Task<Result<string>> Delete(List<Guid?> param);
        Task<Result<List<UploadNickNameDto>>> BatchUploadNickName(IFormFile excelFile, string userId);
    }
}
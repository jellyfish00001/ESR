using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using ERS.Entities;
using ERS.Application.Contracts.DTO.Nickname;
using ERS.IRepositories;
using ERS.IDomainServices;
using ERS.DTO;
using System;
using ERS.DTO.Nickname;
using Microsoft.AspNetCore.Http;
namespace ERS.Application.Services
{
    public class NicknameService : ApplicationService, INicknameService
    {
        private ICustomerNicknameRepository _CustomerNicknameRepository;
        private ICustomerNicknameDomainService _CustomerNicknameDomainService;
        public NicknameService(ICustomerNicknameRepository CustomerNicknameRepository, ICustomerNicknameDomainService CustomerNicknameDomainService)
        {
            _CustomerNicknameRepository = CustomerNicknameRepository;
            _CustomerNicknameDomainService = CustomerNicknameDomainService;
        }
        public async Task<List<NicknameDto>> GetNickname(string name,string company)
        {
            List<NicknameDto> nicknameDtos  = new List<NicknameDto> ();
            IEnumerable<CustomerNickname> customers= await _CustomerNicknameRepository.GetNickname(name,company);
            customers.ToList().ForEach(b => nicknameDtos.Add(new NicknameDto{ nickname=b.nickname,name=b.name }));
            return nicknameDtos;
        }
        public async Task<List<NicknameDto>> GetNameByCompany(string company)
        {
            return await _CustomerNicknameRepository.GetNameByCompany(company);
        }
        public async Task<Result<List<NickNameCommonDto>>> Get(Request<NickNameCommonDto> param) => await _CustomerNicknameDomainService.Get(param);
        public async Task<Result<string>> Add(NickNameCommonDto param, string cuser) => await _CustomerNicknameDomainService.Add(param, cuser);
        public async Task<Result<string>> Update(NickNameCommonDto param, string cuser) => await _CustomerNicknameDomainService.Update(param, cuser);
        public async Task<Result<string>> Delete(List<Guid?> param) => await _CustomerNicknameDomainService.Delete(param);
        public async Task<Result<List<UploadNickNameDto>>> BatchUploadNickName(IFormFile excelFile, string userId) => await _CustomerNicknameDomainService.BatchUploadNickName(excelFile,userId);
    }
}
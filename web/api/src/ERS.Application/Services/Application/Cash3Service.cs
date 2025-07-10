using ERS.Application.Contracts.DTO.Application;
using ERS.Domain.Entities.Application;
using ERS.DTO;
using ERS.DTO.Application;
using ERS.DTO.BDExp;
using ERS.IDomainServices;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
namespace ERS.Services.Application
{
    public class Cash3Service : ApplicationService, ICash3Service
    {
        private ICash3DomainService _Cash3DomainService;
        public Cash3Service(ICash3DomainService Cash3DomainService)
        {
            _Cash3DomainService = Cash3DomainService;
        }
        public async Task<Result<CashResult>> Submit(IFormCollection formCollection, string user, string token) => await _Cash3DomainService.Submit(formCollection, user, token, "P");
        public async Task<Result<CashResult>> Keep(IFormCollection formCollection, string user, string token) => await _Cash3DomainService.Submit(formCollection, user, token, "T");
        public async Task<Result<List<OverdueUserDto>>> OverdueUser(string user) => await _Cash3DomainService.OverdueUser(user);
        public async Task<Result<List<BDExpDto>>> GetAdvanceScene(string input,string company)
        {
            return await _Cash3DomainService.FuzzyQueryScene(input,company);
        }
        public async Task<Result<List<BDExpDto>>> GetAllAdvanceScenes(string company)
        {
            return await _Cash3DomainService.QueryAllScenes(company);
        }
        public async Task<Result<List<BDExpDto>>> GetFilecategoryByExpcode(string expcode, string company, int category)
        {
            return await _Cash3DomainService.QueryFilecategoryByExpcode(expcode, company, category);
        }
        public async Task<Result<List<BDPayDto>>> GetBDPayType()
        {
            return await _Cash3DomainService.QueryBDPayType();
        }
        public async Task<Result<string>> GetSignAttachment(string rno, string Token)
        {
            return await _Cash3DomainService.GetSignAttachment(rno, Token);
        }
        public async Task<Result<List<string>>> GetUnreversAdvRno(string word,string company)
        {
            return await _Cash3DomainService.FuzzyQueryAdvRno(word,company);
        }
        public async Task<Result<bool>> IsChangeApplicant(string emplid, decimal amount, string company)
        {
            return await _Cash3DomainService.IsChangeApplicant(emplid, amount, company);
        }
        public async Task<Result<ChangePayeeTipsDto>> ChangePayeeTips(string emplid, decimal amount, string company)
        {
            return await _Cash3DomainService.ChangePayeeTips(emplid, amount, company);
        }
    }
}

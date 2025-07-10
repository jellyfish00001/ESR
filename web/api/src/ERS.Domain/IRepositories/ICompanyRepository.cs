using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICompanyRepository : IRepository<BDCompanyCategory, Guid>
    {
        Task<string> demo();
        Task<string> GetStarwith(string company);
        Task<string> GetCurrency(string company);
        Task<List<BDCompanyCategory>> GetComInfoByCompany(string company);
        Task<BDCompanyCategory> GetCompanyByCode(string company, string companycode);
        Task<BDCompanyCategory> GetCompanyById(Guid? Id);
        //Task<string> GetCompanyByCode(string companycode);
        Task<string> GetAreaByCode(string companycode);
    }
}

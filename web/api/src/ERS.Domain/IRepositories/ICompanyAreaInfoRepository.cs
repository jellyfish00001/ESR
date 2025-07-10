using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICompanyAreaInfoRepository : IRepository<CompanyAreaInfo, Guid>
    {
        Task<List<CompanyAreaInfo>> GetComInfoBycompanyCode(string companyCode);

        Task<List<string>> GetCompanyByCompanyCodeAndSite(string companyCode,string site);
    }
}

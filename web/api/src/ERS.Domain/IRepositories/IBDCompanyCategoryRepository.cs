
using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IBDCompanyCategoryRepository : IRepository<BDCompanyCategory, Guid>
    {
        Task<string> QueryAreaByCompanyCategory(string CompanyCategory);
    }
}
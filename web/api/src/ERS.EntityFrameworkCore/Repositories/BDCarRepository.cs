using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class BDCarRepository : EfCoreRepository<ERSDbContext, BDCar, Guid>, IBDCarRepository
    {
        public BDCarRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<BDCar>> QueryBDCarByCompany(string company)
        {
            return (await GetDbSetAsync()).Where(q => q.company == company).ToList();
        }
    }
}
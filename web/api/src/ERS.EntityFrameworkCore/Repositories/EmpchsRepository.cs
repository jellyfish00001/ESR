using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class EmpchsRepository : EfCoreRepository<ERSDbContext, Empchs, Guid>, IEmpchsRepository
    {
        public EmpchsRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<Empchs>> GetAllEmpchsList()
        {
            return await (await GetDbSetAsync()).ToListAsync();
        }
    }
}
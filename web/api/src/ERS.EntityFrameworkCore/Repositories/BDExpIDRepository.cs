using System;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class BDExpIDRepository : EfCoreRepository<ERSDbContext, BDExpID, Guid>, IBDExpIDRepository
    {
        public BDExpIDRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
            
        }
    }
}
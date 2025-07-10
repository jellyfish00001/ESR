using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
namespace ERS.Repositories
{
    public class BDCompanySiteRepository : EfCoreRepository<ERSDbContext, BDCompanySite, Guid>, IBDCompanySiteRepository
    {
        public BDCompanySiteRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

    }
}
using System;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class BDCashReturnRepository : EfCoreRepository<ERSDbContext, BDCashReturn, Guid>, IBDCashReturnRepository
    {
        public BDCashReturnRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
    }
}
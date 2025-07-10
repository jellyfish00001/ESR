using System;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class CashDetailPstRepository : EfCoreRepository<ERSDbContext, CashDetailPst, Guid>, ICashDetailPstRepository
    {
        public CashDetailPstRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
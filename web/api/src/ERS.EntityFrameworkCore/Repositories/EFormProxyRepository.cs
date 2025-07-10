using System;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class EFormProxyRepository : EfCoreRepository<ERSDbContext, EFormProxy, Guid>, IEFormProxyRepository
    {
        public EFormProxyRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
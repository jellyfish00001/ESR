using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class MobileCallBackLogRepository : EfCoreRepository<ERSDbContext, MobileCallBackLog, Guid>, IMobileCallBackLogRepository
    {
        public MobileCallBackLogRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
    
        
       
    }
}

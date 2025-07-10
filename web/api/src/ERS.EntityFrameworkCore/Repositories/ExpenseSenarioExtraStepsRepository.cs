using System;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class ExpenseSenarioExtraStepsRepository : EfCoreRepository<ERSDbContext, ExpenseSenarioExtraSteps, Guid>, IExpenseSenarioExtraStepsRepository
    {
        public ExpenseSenarioExtraStepsRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        
    }
}

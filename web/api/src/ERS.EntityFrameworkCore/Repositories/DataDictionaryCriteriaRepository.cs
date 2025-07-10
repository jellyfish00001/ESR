using System;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class DataDictionaryCriteriaRepository : EfCoreRepository<ERSDbContext, DataDictionaryCriteria, Guid>, IDataDictionaryCriteriaRepository
    {
        public DataDictionaryCriteriaRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

    }
}
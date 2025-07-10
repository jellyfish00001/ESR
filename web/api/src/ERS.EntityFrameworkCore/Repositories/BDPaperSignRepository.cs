using System;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class BDPaperSignRepository : EfCoreRepository<ERSDbContext, BDPaperSign, Guid>, IBDPaperSignRepository
    {
        public BDPaperSignRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
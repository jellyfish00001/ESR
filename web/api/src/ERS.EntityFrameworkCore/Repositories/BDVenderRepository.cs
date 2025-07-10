namespace ERS.Repositories;

using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

public class BDVenderRepository : EfCoreRepository<ERSDbContext, BDVender, Guid>, IBDVenderRepository
{
    public BDVenderRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}

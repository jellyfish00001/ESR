using ERS.Application.Contracts.DTO.Nickname;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class CustomerNicknameRepository : EfCoreRepository<ERSDbContext, CustomerNickname, Guid>, ICustomerNicknameRepository
    {
        public CustomerNicknameRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<IEnumerable<CustomerNickname>> GetNickname(string name, string company)
        {
            return await (await GetDbSetAsync()).Where(b => (b.name.ToLower().Contains(name.ToLower()) || b.nickname.ToLower().Contains(name.ToLower())) && b.company == company).AsNoTracking().OrderBy(b => b.name).Take(10).ToListAsync();
        }
        public async Task<List<NicknameDto>> GetNameByCompany(string company)
        {
            return await (await GetDbSetAsync()).Where(x => x.company == company).Select(x => new NicknameDto{nickname = x.nickname, name = x.name}).ToListAsync();
        }
    }
}

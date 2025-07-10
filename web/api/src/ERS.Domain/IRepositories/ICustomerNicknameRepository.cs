using ERS.Application.Contracts.DTO.Nickname;
using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICustomerNicknameRepository : IRepository<CustomerNickname, Guid>
    {
        Task<IEnumerable<CustomerNickname>> GetNickname(string name, string company);
        Task<List<NicknameDto>> GetNameByCompany(string company);
    }
}

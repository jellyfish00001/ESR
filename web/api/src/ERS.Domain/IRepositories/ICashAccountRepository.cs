using ERS.DTO.Employee;
using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICashAccountRepository : IRepository<CashAccount, Guid>
    {
        Task<CashAccountDto> GetAccount(string user);
        Task<List<string>> GetAllBankByCompany(string company);
        Task<List<CashAccount>> ReadCAListByEmplid();
        Task<List<CashAccount>> GetCashAccountList();
    }
}

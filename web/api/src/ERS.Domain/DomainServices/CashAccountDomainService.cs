using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
namespace ERS.DomainServices
{
    public class CashAccountDomainService : CommonDomainService, ICashAccountDomainService
    {
        private ICashAccountPsRepository _cashaccountpsRepository;
        private ICashAccountRepository _cashaccountRepository;
        private IEmployeeRepository _EmployeeRepository;
        private ICompanyRepository _CompanyRepository;
        public CashAccountDomainService(ICashAccountPsRepository cashaccountpsRepository,
        ICashAccountRepository cashaccountRepository,
        IEmployeeRepository EmployeeRepository,
        ICompanyRepository CompanyRepository)
        {
            _EmployeeRepository = EmployeeRepository;
            _cashaccountpsRepository = cashaccountpsRepository;
            _cashaccountRepository = cashaccountRepository;
            _CompanyRepository = CompanyRepository;
        }
        public async Task CashAccountDataSync()
        {
            var psList = (await _cashaccountpsRepository.GetCashAccountPsBySql("SELECT * FROM (SELECT *, ROW_NUMBER() OVER(PARTITION BY emplid ORDER BY effdt DESC) AS rn FROM cash_account_ps cap) AS t WHERE t.rn = 1 AND t.isdeleted = false "));
            List<CashAccount> Accountlist = await _cashaccountRepository.GetCashAccountList();
            var accemplids = Accountlist.Select(x => x.emplid).ToList();
            List<string> emplidlist = psList.DistinctBy(x => x.emplid).Select(s => s.emplid).ToList();
            List<CashAccount> insertList = new();
            List<CashAccount> updateList = new();
            for (int i = 0; i < emplidlist.Count; i++)
            {
                if (accemplids.Contains(emplidlist[i]))
                {
                    var newAccount = psList.Where(x => x.emplid == emplidlist[i] && !String.IsNullOrEmpty(x.accountname)).OrderByDescending(t => t.effdt).FirstOrDefault();
                    var cashAccount = Accountlist.Where(x => x.emplid == emplidlist[i]).FirstOrDefault();
                    if (cashAccount != null && newAccount != null)
                    {
                        //如果户名 账号 工号 有效期 全都相同就不更新
                        if (cashAccount.emplid == newAccount.emplid && cashAccount.account == newAccount.account && !string.IsNullOrEmpty(cashAccount.accountname) && cashAccount.accountname == newAccount.accountname && cashAccount.effdt == newAccount.effdt)
                        {
                            continue;
                        }
                        cashAccount.account = newAccount.account;
                        cashAccount.bank = newAccount.bank;
                        cashAccount.accountname = newAccount.accountname;
                        cashAccount.effdt = newAccount.effdt;
                        cashAccount.company = "ALL";
                        Console.WriteLine("Update: " + "No: " + (i + 1) + " " + emplidlist[i] + " " + cashAccount.account);
                        updateList.Add(cashAccount);
                        //await _cashaccountRepository.UpdateAsync(cashAccount);
                    }
                }
                else
                {
                    var newAccount = psList.Where(x => x.emplid == emplidlist[i]).OrderByDescending(t => t.effdt).FirstOrDefault();
                    if (newAccount != null)
                    {
                        CashAccount cashAccount = new CashAccount();
                        cashAccount.emplid = newAccount.emplid;
                        cashAccount.account = newAccount.account;
                        cashAccount.accountname = newAccount.accountname;
                        cashAccount.uuser = newAccount.uuser;
                        cashAccount.bank = newAccount.bank;
                        cashAccount.effdt = newAccount.effdt;
                        cashAccount.company = "ALL";
                        Console.WriteLine("Insert: " + "No: " + (i + 1) + " " + emplidlist[i] + " " + cashAccount.account);
                        insertList.Add(cashAccount);
                        //await _cashaccountRepository.InsertAsync(cashAccount);
                    }
                }
                if (insertList.Count > 2000)
                {
                    await _cashaccountRepository.InsertManyAsync(insertList);
                    insertList.Clear();
                }
                if (updateList.Count > 2000)
                {
                    await _cashaccountRepository.UpdateManyAsync(updateList,true);
                    updateList.Clear();
                }
            }
            if (insertList.Count > 0)
                await _cashaccountRepository.InsertManyAsync(insertList);
            if (updateList.Count > 0)
                await _cashaccountRepository.UpdateManyAsync(updateList);
        }
    }
}

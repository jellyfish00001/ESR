using ERS.Job.Util;

namespace ERS.Job.Jobs
{
    public class DemoJob : IJobBase
    {
        public async Task Run()
        {
            await test();
        }
        async Task test()
        {
            Console.WriteLine("test");
        }

        // async Task test()
        // {
        //     List<CashAccountPs> PSlist = (await _cashaccountpsRepository.WithDetailsAsync()).ToList();
        //     List<CashAccount> Accountlist = (await _cashaccountRepository.WithDetailsAsync()).ToList();
        //     var accemplids = Accountlist.Select(x => x.emplid).ToList();

        //     foreach(var item in PSlist)
        //     {
        //         if(accemplids.Contains(item.emplid))
        //         {
        //             var newAccount = PSlist.Where(x => x.emplid == item.emplid).OrderByDescending(t=>t.effdt).FirstOrDefault();
        //             var cashAccount = Accountlist.Where(x => x.emplid == item.emplid).FirstOrDefault();
        //             if(cashAccount != null && newAccount != null && cashAccount.account != newAccount.account)
        //             {
        //                 cashAccount.account = newAccount.account;
        //                 cashAccount.bank = newAccount.bank;
        //                 await _cashaccountRepository.UpdateAsync(cashAccount);
        //             }
        //         }
        //         else
        //         {
        //             CashAccount cashAccount = new CashAccount();
        //             cashAccount.emplid = item.emplid;
        //             cashAccount.account = item.account;
        //             cashAccount.uuser = item.uuser;
        //             cashAccount.bank = item.bank;
        //             cashAccount.effdt = item.effdt;
        //             cashAccount.company = item.company;
        //             await _cashaccountRepository.InsertAsync(cashAccount);
        //         }
        //     }
        // }
    }
}

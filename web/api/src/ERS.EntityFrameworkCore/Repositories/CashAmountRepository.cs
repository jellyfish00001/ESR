using ERS.DTO;
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
    public class CashAmountRepository : EfCoreRepository<ERSDbContext, CashAmount, Guid>, ICashAmountRepository
    {
        private ICashDetailRepository _CashDetailRepository;
        private IBDCashReturnRepository _BDCashReturnRepository;

        public CashAmountRepository(IDbContextProvider<ERSDbContext> dbContextProvider, ICashDetailRepository CashDetailRepository, IBDCashReturnRepository BDCashReturnRepository) : base(dbContextProvider)
        {
            _CashDetailRepository = CashDetailRepository;
            _BDCashReturnRepository = BDCashReturnRepository;
        }
        public async Task<CashAmount> GetByNo(string rno) => await (await GetDbSetAsync()).Where(i => i.rno == rno).FirstOrDefaultAsync();

        public async Task<IList<CashAmount>> GetRnoAll(List<string> rno)
        {
            return (await GetDbSetAsync()).Where(b => rno.Any(c => c.Equals(b.rno))).ToList();
        }

        public async Task<IList<CashAmount>> ReadByRno(List<string> rno) => (await GetDbSetAsync()).Where(b => rno.Any(c => c.Equals(b.rno))).AsNoTracking().ToList();
        public async Task<IList<CashAmount>> GetuserAll(string user)
        {
            return (await GetDbSetAsync()).Where(b => b.cuser == user).AsNoTracking().ToList();
        }

        public async Task<CashAmount> GetCashAmountByNo(string rno) => await (await GetDbSetAsync()).Where(i => i.rno == rno).FirstOrDefaultAsync();

        /// <summary>
        /// 冲账（减去预支）
        /// </summary>
        /// <param name="cashDetails"></param>
        /// <returns></returns>
        public async Task<Result<string>> Reversal(CashHead cash)
        {
            Result<string> result = new();
            List<string> advances = cash.CashDetailList.Where(i => !string.IsNullOrEmpty(i.advancerno)).Select(i => i.advancerno).ToList();

            if (advances.Count == 0) return result;
            IList<CashAmount> cashAmounts = await this.GetRnoAll(advances);
            IList<BDCashReturn> cashReturns = (await _BDCashReturnRepository.WithDetailsAsync()).Where(w => advances.Contains(w.rno)).ToList();
            foreach (CashAmount cashAmount in cashAmounts)
            {
                if (cashAmount.actamt == 0)
                {
                    result.status = 2;
                    result.message = cashAmount.rno;
                    return result;
                }
                var temp = cash.CashDetailList.Where(i => i.advancerno == cashAmount.rno).First();//用预支金作为报销的detail item
                var tempCashReturn = cashReturns.Where(w => w.rno == temp.advancerno).ToList();//该笔预支金的缴回记录
                decimal taxloss = Math.Round(cash.InvoiceList.Where(i => i.seq == temp.seq && i.undertaker == "self").Sum(i => i.taxloss), 2, MidpointRounding.AwayFromZero);//报销的税金损失金额
                // decimal amount = Convert.ToDecimal(temp.amount);
                // if (amount > 0)
                //     cashAmount.actamt = 0;
                // else
                // { 
                if(temp.baseamt - taxloss >= cashAmount.actamt - tempCashReturn.Sum(s => s.amount))
                {
                    cashAmount.actamt = tempCashReturn.Sum(s => s.amount);
                }
                else
                {
                    cashAmount.actamt -= Math.Round(Convert.ToDecimal(temp.baseamt - taxloss), 2, MidpointRounding.AwayFromZero);
                }
                temp.amount2 = Math.Round(Convert.ToDecimal(temp.baseamt), 2, MidpointRounding.AwayFromZero) - taxloss;
                //cashAmount.actamt -= Math.Round(Convert.ToDecimal(temp.baseamt), 2, MidpointRounding.AwayFromZero) - taxloss;
                // cashAmount.actamt -= Math.Round(Convert.ToDecimal(temp.baseamt), 2, MidpointRounding.AwayFromZero) - taxloss - tempCashReturn.Sum(s => s.amount);
                // if (cashAmount.actamt <= 0)
                // {
                //     cashAmount.actamt = tempCashReturn.Sum(s => s.amount);
                // }
                // }
            }
            (await GetDbSetAsync()).UpdateRange(cashAmounts);
            return result;
        }

        /// <summary>
        /// 回扣（加回预支）
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        public async Task Kickback(string rno)
        {
            List<CashDetail> cashDetails = await (await _CashDetailRepository.WithDetailsAsync()).Where(i => i.rno == rno).AsNoTracking().ToListAsync();
            List<string> advances = cashDetails.Where(i => !string.IsNullOrEmpty(i.advancerno)).Select(i => i.advancerno).ToList();
            if (advances.Count == 0) return;
            IList<CashAmount> cashAmounts = await this.GetRnoAll(advances);
            foreach (CashAmount cashAmount in cashAmounts)
                cashAmount.actamt += Convert.ToDecimal(cashDetails.Where(i => i.advancerno == cashAmount.rno).Select(i => i.amount2).First());
            (await GetDbSetAsync()).UpdateRange(cashAmounts);
        }
    }
}

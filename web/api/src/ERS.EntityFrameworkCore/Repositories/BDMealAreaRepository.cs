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
    public class BDMealAreaRepository : EfCoreRepository<ERSDbContext, BDMealArea, Guid>, IBDMealAreaRepository
    {
        public BDMealAreaRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<IList<string>> GetCity(string company) => await (await GetDbSetAsync()).Where(i => i.company == company).Select(i => i.city).AsNoTracking().Distinct().ToListAsync();

        public async Task<TravelDto> GetExpense(TravelDto travel)
        {
            TravelDto result = new();
            List<BDMealArea> data = await(await GetDbSetAsync()).Where(i => i.company == travel.company && i.city == travel.city).AsNoTracking().ToListAsync();
            return TravelCount(data, travel);
        }

        public TravelDto TravelCount(List<BDMealArea> data, TravelDto travel)
        {
            TravelDto result = new();
            if (travel.gotime.TimeOfDay >= travel.backtime.TimeOfDay) return result;
            List<BDMealArea> temp = new List<BDMealArea>();
            // 出前
            temp = data.Where(i => i.gotype1 == -1 && Convert.ToDateTime(i.gotime1).TimeOfDay > travel.gotime.TimeOfDay).ToList();
            if (temp.Count > 0)
            {
                result = BackTravel(temp, travel);
                if (!string.IsNullOrEmpty(result.currency)) return result;
                ////出前回前
                //temp = temp.Where(i => i.backtype1 == -1 && Convert.ToDateTime(i.backtime1).TimeOfDay > travel.backtime.TimeOfDay).ToList();
                //if (temp.Count > 0)
                //{
                //    return temp.Select(i => new TravelDto()
                //    {
                //        amount = i.amount,
                //        currency = i.currency
                //    }).FirstOrDefault();
                //}
                ////出前回后
                //temp = temp.Where(i => i.backtype1 == 1 && i.backtype2 == 0 && Convert.ToDateTime(i.backtime1).TimeOfDay <= travel.backtime.TimeOfDay).ToList();
                //if (temp.Count > 0)
                //{
                //    return temp.Select(i => new TravelDto()
                //    {
                //        amount = i.amount,
                //        currency = i.currency
                //    }).FirstOrDefault();
                //}
                ////出前回间
                //temp = temp.Where(i => i.backtype1 == 1 && i.backtype2 == -1 && Convert.ToDateTime(i.backtime1).TimeOfDay <= travel.backtime.TimeOfDay && Convert.ToDateTime(i.backtime2).TimeOfDay >= travel.backtime.TimeOfDay).ToList();
                //if (temp.Count > 0)
                //{
                //    return temp.Select(i => new TravelDto()
                //    {
                //        amount = i.amount,
                //        currency = i.currency
                //    }).FirstOrDefault();
                //}
            }

            // 出间
            temp = data.Where(i => i.gotype1 == 1 && i.gotype2 == -1 && Convert.ToDateTime(i.gotime1).TimeOfDay <= travel.gotime.TimeOfDay && Convert.ToDateTime(i.gotime2).TimeOfDay >= travel.gotime.TimeOfDay).ToList();
            if (temp.Count > 0)
            {
                result = BackTravel(temp, travel);
                if (!string.IsNullOrEmpty(result.currency)) return result;
            }
            // 出后
            temp = data.Where(i => i.gotype1 == 1 && i.gotype2 == 0 && Convert.ToDateTime(i.gotime1).TimeOfDay <= travel.gotime.TimeOfDay).ToList();
            if (temp.Count > 0)
            {
                result = BackTravel(temp, travel);
                if (!string.IsNullOrEmpty(result.currency)) return result;
            }
            return result;
        }

        TravelDto BackTravel(List<BDMealArea> temp, TravelDto travel)
        {
            TravelDto result = new TravelDto();
            List<BDMealArea> _temp = new();
            //回前
            _temp = temp.Where(i => i.backtype1 == -1 && Convert.ToDateTime(i.backtime1).TimeOfDay > travel.backtime.TimeOfDay).ToList();
            if (_temp.Count > 0)
            {
                return _temp.Select(i => new TravelDto()
                {
                    amount = i.amount,
                    currency = i.currency
                }).FirstOrDefault();
            }
            //回后
            _temp = temp.Where(i => i.backtype1 == 1 && i.backtype2 == 0 && Convert.ToDateTime(i.backtime1).TimeOfDay <= travel.backtime.TimeOfDay).ToList();
            if (_temp.Count > 0)
            {
                return _temp.Select(i => new TravelDto()
                {
                    amount = i.amount,
                    currency = i.currency
                }).FirstOrDefault();
            }
            //回间
            _temp = temp.Where(i => i.backtype1 == 1 && i.backtype2 == -1 && Convert.ToDateTime(i.backtime1).TimeOfDay <= travel.backtime.TimeOfDay && Convert.ToDateTime(i.backtime2).TimeOfDay >= travel.backtime.TimeOfDay).ToList();
            if (_temp.Count > 0)
            {
                return _temp.Select(i => new TravelDto()
                {
                    amount = i.amount,
                    currency = i.currency
                }).FirstOrDefault();
            }
            _temp = temp.Where(i => i.backtype1 == 0 && i.backtype2 == 0).ToList();
            if (_temp.Count > 0)
            {
                return _temp.Select(i => new TravelDto()
                {
                    amount = i.amount,
                    currency = i.currency
                }).FirstOrDefault();
            }
            return result;
        }

    }
}

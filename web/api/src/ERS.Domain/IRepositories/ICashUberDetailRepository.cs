﻿using ERS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace ERS.IRepositories
{
    public interface ICashUberDetailRepository : IRepository<CashUberDetail, Guid>
    {
        Task<List<CashUberDetail>> GetCashUberDetailsByNo(string rno);
    }
}

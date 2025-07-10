using ERS.DTO;
using ERS.DTO.UberFare;
using ERS.IRepositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ERS.Services.UberFare
{
    public class UberTransactionalDayService : ApplicationService, IUberTransactionalDayService
    {
         
        private IUberTransactionalDayRepository _uberTransactionalDayRepository;

        public UberTransactionalDayService(IUberTransactionalDayRepository uberTransactionalDayRepository)
        {
            _uberTransactionalDayRepository = uberTransactionalDayRepository;
        }

        public async Task<Result<List<UberTransactionalDayDto>>> GetUberTransactional(Request<QueryUberTransactionalDto> request)
        {
            Result<List<UberTransactionalDayDto>> result = new Result<List<UberTransactionalDayDto>>()
            {
                data = new List<UberTransactionalDayDto>()
            };
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            var query = (await _uberTransactionalDayRepository.WithDetailsAsync())
              .AsQueryable();
            if (request.data.startDate.HasValue)
                query = query.Where(q => q.RequestDate >= request.data.startDate.Value.Date);

            if (request.data.endDate.HasValue)
                query = query.Where(q => q.RequestDate <= request.data.endDate.Value.Date);

            if (!string.IsNullOrEmpty(request.data.rno))
                query = query.Where(q => q.rno == request.data.rno);

            if (!request.data.signStatus.IsNullOrEmpty())
                query = query.Where(q => request.data.signStatus.Contains(q.SignStatus));

            var uberTransactional = query
                .Select(w => new UberTransactionalDayDto
                {
                    TripId = w.TripId,
                    TransactionTimestamp = w.TransactionTimestamp,
                    RequestDateUtc = w.RequestDateUtc,
                    RequestTimeUtc = w.RequestTimeUtc,
                    RequestDate = w.RequestDate,
                    RequestTime = w.RequestTime,
                    DropOffDateUtc = w.DropOffDateUtc,
                    DropOffTimeUtc = w.DropOffTimeUtc,
                    DropOffDate = w.DropOffDate,
                    DropOffTime = w.DropOffTime,
                    RequestTimezoneOffsetFromUtc = w.RequestTimezoneOffsetFromUtc,
                    FirstName = w.FirstName,
                    LastName = w.LastName,
                    Email = w.Email,
                    EmployeeId = w.EmployeeId,
                    Service = w.Service,
                    City = w.City,
                    Distance = w.Distance,
                    Duration = w.Duration,
                    PickupAddress = w.PickupAddress,
                    DropOffAddress = w.DropOffAddress,
                    ExpenseCode = w.ExpenseCode,
                    ExpenseMemo = w.ExpenseMemo,
                    Invoices = w.Invoices,
                    Program = w.Program,
                    Group = w.Group,
                    PaymentMethod = w.PaymentMethod,
                    TransactionType = w.TransactionType,
                    FareInLocalCurrency = w.FareInLocalCurrency,
                    TaxesInLocalCurrency = w.TaxesInLocalCurrency,
                    TipInLocalCurrency = w.TipInLocalCurrency,
                    TransactionAmountInLocalCurrency = w.TransactionAmountInLocalCurrency,
                    LocalCurrencyCode = w.LocalCurrencyCode,
                    FareInHomeCurrency = w.FareInHomeCurrency,
                    TaxesInHomeCurrency = w.TaxesInHomeCurrency,
                    TipInHomeCurrency = w.TipInHomeCurrency,
                    TransactionAmountInHomeCurrency = w.TransactionAmountInHomeCurrency,
                    EstimatedServiceAndTechnologyFee = w.EstimatedServiceAndTechnologyFee,
                    rno = w.rno,
                    SignStatus = w.SignStatus
                })
                .OrderByDescending(i => i.TransactionTimestamp).ToList(); // Fix: Add ToList() to materialize the query  

            if (uberTransactional.Count == 0) // Fix: Use the materialized list to check the count  
            {
                return result;
            }
            result.total = query.Count(); // Fix: Add parentheses to call Count() method  
            result.data = uberTransactional.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(); // Fix: Use the materialized list for pagination  
            return result;
        }
    }
}

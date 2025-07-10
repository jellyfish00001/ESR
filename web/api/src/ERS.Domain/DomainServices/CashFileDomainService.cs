using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using ERS.Domain.IDomainServices;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;
using System.Linq;
namespace ERS.Domain.DomainServices
{
    public class CashFileDomainService : ICashFileDomainService
    {
        private IRepository<CashFile, Guid> _cashFileRepository;
        public CashFileDomainService(IRepository<CashFile, Guid> cashFileRepository)
        {
            _cashFileRepository = cashFileRepository;
        }
        public async Task<string> add(List<CashFile> cashFile)
        {
            try
            {
                await _cashFileRepository.InsertManyAsync(cashFile);
                return "success";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public async Task<string> delete(CashFile cashFile)
        {
             try
            {
                await _cashFileRepository.DeleteAsync(cashFile);
                return "success";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public async Task<CashFile> query(CashFile cashFile)
        {
           return (await _cashFileRepository.WithDetailsAsync()).Where(b =>b.rno==cashFile.rno&&b.seq==cashFile.seq
           &&b.item==cashFile.item&&b.formcode==cashFile.formcode&&b.company==cashFile.company).FirstOrDefault();
        }
    }
}
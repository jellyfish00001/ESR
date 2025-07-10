using ERS.Dapper;
using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class AutoNoRepository : EfCoreRepository<ERSDbContext, EAutono, Guid>, IAutoNoRepository
    {
        private IRedisRepository _RedisRepository;
        private ILogger<AutoNoRepository> _logger;
        private IDapperFactory _DapperFactory;

        public AutoNoRepository(IDbContextProvider<ERSDbContext> dbContextProvider, IRedisRepository RedisRepository,ILogger<AutoNoRepository> logger, IDapperFactory DapperFactory) : base(dbContextProvider)
        {
            _RedisRepository = RedisRepository;
            _logger = logger;
            _DapperFactory = DapperFactory;
        }
        public async Task<string> CreateCash1No() => await CreateNo("CASH_1");
        public async Task<string> CreateCash2No() => await CreateRNo("CASH_2");
        public async Task<string> CreateCash3No() => await CreateNo("CASH_3");
        public async Task<string> CreateCash3ANo() => await CreateNo("CASH_3A");
        public async Task<string> CreateCash4No() => await CreateNo("CASH_4");
        public async Task<string> CreateCash5No() => await CreateNo("CASH_5");
        public async Task<string> CreateCashXNo() => await CreateNo("CASH_X");
        public async Task<string> CreateAccountNo() => await CreateNo("Account");
        public async Task<string> CreatePaymentNo() => await CreateRNo("Payment");
        public async Task<string> CreateCash6No() => await CreateNo("CASH_6");

        public async Task<string> CreateCash1RNo() => await CreateRNo("CASH_1");
        public async Task<string> CreateCashUberRNo() => await CreateUberRNo("CASH_UBER");

        async Task<EAutono> GetData(string form_code)
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            return await (await GetDbSetAsync()).Where(b => b.formcode == form_code && b.date == today).FirstOrDefaultAsync();
        }

        async Task<string> CreateNo(string form_code)
        {
            string key = $"{form_code}_{DateTime.Now.ToString("yyyyMMdd")}";
            int index = 0;
            while(!await _RedisRepository.AddLockAutoNoAsync(key))
            {
                Thread.Sleep(500);
                index++;
            }
            _logger.LogInformation("Lock key: {Key}", key);
            try
            {
                EAutono data = new(form_code);

                DapperHelper dapper = _DapperFactory.CreateConnection();
                var temp = (await dapper.QueryAsync<EAutono>($"select getautono('{form_code}','{data.date}') as no,'3a0dc7c7-c0a9-a2a4-49b1-7403e1e91400'::uuid \"Id\",'' company,null cdate,'' \"date\",'' formcode,null mdate,null cuser,null muser,false isdeleted")).FirstOrDefault();
                if (temp == null || string.IsNullOrEmpty(temp.no))
                    throw new Exception("require request no error!!!");
                data.no = temp.no;



                //EAutono eAutono = await GetData(form_code);
                //data.SetNo(eAutono);
                //var dbcontext = await this.GetDbContextAsync();
                //if (eAutono == null)
                //{
                //    (await GetDbSetAsync()).FromSqlRaw($"INSERT INTO e_autono (\"Id\",date,no,formcode,company,cdate,isdeleted) VALUES ('${this.GuidGenerator.Create()}', '${data.date}', '${data.no}', '${form_code}', 'ALL', now(), false)").FirstOrDefault();
                //    //await InsertAsync(data, true);
                //    await dbcontext.AddAsync(data);
                //}
                //else
                //{
                //    eAutono.SetNo(eAutono);
                //    dbcontext.Update(eAutono);
                //    //await UpdateAsync(eAutono, true);
                //}
                //await dbcontext.SaveChangesAsync(true);


                _logger.LogInformation("产生单号: {No}", data.GetNo());
                return data.GetNo();
            }
            finally
            {
                _logger.LogInformation("UnLock key: {Key}", key);
                await _RedisRepository.DelLockAsync(key);
            }
        }

        async Task<string> CreateRNo(string form_code)
        {
            string today = DateTime.Now.ToString("yyMMdd");
            string rnoHead = "";
            string rno = "";
            switch (form_code)
            {
                case "CASH_1":
                    rnoHead = "C" + today;
                    break;
                case "CASH_2":
                    rnoHead = "E" + today;
                    break;
                case "CASH_3":
                    rnoHead = "E" + today;
                    break;
                case "Payment":
                    rnoHead = "P" + today;
                    break;
            }
            try
            {
                DapperHelper dapper = _DapperFactory.CreateConnection();
                var no = (await dapper.QueryAsync<int>($"SELECT nextval('rno_seq') as no")).FirstOrDefault();
                if (no <=0 )
                    throw new Exception("require request no error!!!");
                rno = rnoHead + no.ToString("D5");

                _logger.LogInformation("产生单号: {No}", rno);
                return rno;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        async Task<string> CreateUberRNo(string form_code)
        {
            string today = DateTime.Now.ToString("yyMMdd");
            string rnoHead = "U" + today;
            string key = $"{form_code}_{DateTime.Now.ToString("yyyyMMdd")}";
            string rno = "";
            DapperHelper dapper = _DapperFactory.CreateConnection();
            var no = (await dapper.QueryAsync<int>($"SELECT nextval('uber_rno_seq') as no")).FirstOrDefault();
            if (no <= 0)
               throw new Exception("require request no error!!!");
            rno = rnoHead + no.ToString("D5");

            _logger.LogInformation("产生单号: {No}", rno);
            return rno;
        }
    }
}

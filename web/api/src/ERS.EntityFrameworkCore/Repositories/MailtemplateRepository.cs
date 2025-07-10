using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class MailtemplateRepository : EfCoreRepository<ERSDbContext, Mailtemplate, Guid>, IMailtemplateRepository
    {
        public MailtemplateRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<Mailtemplate> GetNextApply(string company) => await (await GetDbSetAsync()).FromSqlRaw("select \"Id\", subject, replace(mailmsg, '@Helpdesk', helpdesk) mailmsg,apid,mailtype,seq,company,helpdesk,cdate,cuser,muser,mdate,isdeleted from mailtemple where (company='ALL' or company='"+ company +"') and mailtype=1 and isdeleted=false order by company desc").FirstOrDefaultAsync();

        public async Task<Mailtemplate> GetFinishApply(string company) => await (await GetDbSetAsync()).FromSqlRaw("select \"Id\", subject, replace(mailmsg, '@Helpdesk', helpdesk) mailmsg,apid,mailtype,seq,company,helpdesk,cdate,cuser,muser,mdate,isdeleted from mailtemple where (company='ALL' or company='"+ company +"') and mailtype=2 and isdeleted=false order by company desc").FirstOrDefaultAsync();
        public async Task<Mailtemplate> GetRejectApply(string company) => await (await GetDbSetAsync()).FromSqlRaw("select \"Id\", subject, replace(mailmsg, '@Helpdesk', helpdesk) mailmsg,apid,mailtype,seq,company,helpdesk,cdate,cuser,muser,mdate,isdeleted from mailtemple where (company='ALL' or company='"+ company +"') and mailtype=3 and isdeleted=false order by company desc").FirstOrDefaultAsync();
        public async Task<Mailtemplate> GetPaymentTemplate(string company) => await (await GetDbSetAsync()).FromSqlRaw("select \"Id\",subject, replace(mailmsg, '@Helpdesk', helpdesk) mailmsg,apid,mailtype,seq,company,helpdesk,cdate,cuser,muser,mdate,isdeleted from mailtemple where (company='ALL' or company='"+ company +"') and mailtype=4 and isdeleted=false order by company desc").FirstOrDefaultAsync();
    }
}

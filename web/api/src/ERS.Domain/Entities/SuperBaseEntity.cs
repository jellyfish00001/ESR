using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;
namespace ERS.Entities
{
    public class SuperBaseEntity : BasicAggregateRoot<Guid>, ISoftDelete
    {
        public virtual DateTime? mdate { get; set; }
        public virtual DateTime? cdate { get; set; }
        [StringLength(20)]
        public virtual string cuser { get; set; }
        [StringLength(20)]
        public virtual string muser { get; set; }
        public virtual bool isdeleted { get; set; }
        public void SetCDate(int timeZone = 8) => this.cdate = timeZone == 8 ? DateTime.Now : DateTime.UtcNow.AddHours(timeZone);
        public void SetMDate(int timeZone = 8) => this.cdate = timeZone == 8 ? DateTime.Now : DateTime.UtcNow.AddHours(timeZone);
        public void SetCUser(string user) => this.cuser = user;
    }
    public interface ISoftDelete
    {
        public bool isdeleted { get; set; }
    }
}

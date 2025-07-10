using System.ComponentModel.DataAnnotations;
namespace ERS.Entities
{
    public class BaseEntity : SuperBaseEntity
    {
        [Required]
        [StringLength(20)]
        public virtual string company { get; set; }
        public void SetCompany(string company) => this.company = company;
    }
}

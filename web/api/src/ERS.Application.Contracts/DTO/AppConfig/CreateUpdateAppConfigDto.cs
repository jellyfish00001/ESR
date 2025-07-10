using System;
using System.ComponentModel.DataAnnotations;
namespace ERS.DTO.AppConfig;
public class CreateUpdateAppConfigDto
{
    [Required]
    [StringLength(50)]
    public string key { get; set; }
    [Required]
    [StringLength(500)]
    public string value { get; set; }
    [Editable(false)]
    public DateTime creationTime = DateTime.Now.AddDays(-10);
}

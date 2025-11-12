using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("SystemLog")]
[PrimaryKey("Id")]
internal class SystemLogEntity
{
    [Key] [Required]
    public int Id { get; set; }
    
    [Required]
    public DateTime InsertedTime { get; set; }

    [Required]
    public string? MessageLevel { get; set; }

    [Required]
    public string? Message { get; set; }

    public string? Exception { get; set; }
}

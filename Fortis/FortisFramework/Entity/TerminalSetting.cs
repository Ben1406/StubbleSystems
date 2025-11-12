using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("Terminal")]
[PrimaryKey("TerminalName")]
public class TerminalSetting
{
    [Key] [Required] [StringLength(100)]
    public string TerminalName { get; set; } = null!;

    [Required] [StringLength(int.MaxValue)]
    public string Description { get; set; } = null!;
}

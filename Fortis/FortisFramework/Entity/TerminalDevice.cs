using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("TerminalDevice")]
[PrimaryKey("TerminalName", "DeviceName")]
public class TerminalDevice
{
    [Key] [Required] [StringLength(100)]
    public string TerminalName { get; set; } = null!;

    [Key] [Required] [StringLength(100)]
    public string DeviceName { get; set; } = null!;

    [Required]
    public short DeviceId { get; set; }

    [NotMapped]
    public DeviceSetting? DeviceSetting { get; set; }
}

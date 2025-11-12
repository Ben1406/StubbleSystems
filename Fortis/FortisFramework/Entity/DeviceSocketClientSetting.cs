using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("DeviceSocketClientSetting")]
[PrimaryKey("DeviceName")]
public class DeviceSocketClientSetting
{
    [Key] [Required] [StringLength(100)]
    public string DeviceName { get; set; } = null!;

    [Required] [StringLength(100)]
    public string Encoding { get; set; } = null!;

    [Required] [StringLength(100)]
    public string IpAddress { get; set; } = null!;

    public short IpPort { get; set; } = 0;
}

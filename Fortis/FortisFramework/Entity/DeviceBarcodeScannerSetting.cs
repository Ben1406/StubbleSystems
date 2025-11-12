using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("DeviceBarcodeScannerSetting")]
[PrimaryKey("DeviceName")]
public class DeviceBarcodeScannerSetting
{
    [Key] [Required] [StringLength(100)]
    public string DeviceName { get; set; } = null!;

    [Required] [StringLength(100)]
    public string Type { get; set; } = null!;

    [Required] [StringLength(100)]
    public string Protocol { get; set; } = null!;

    public bool? SendFeedbackToHost { get; set; }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("Device")]
[PrimaryKey("DeviceName")]
public class DeviceSetting
{
    [Key] [Required] [StringLength(100)]
    public string DeviceName { get; set; } = null!;

    [Required] [StringLength(int.MaxValue)]
    public string Description { get; set; } = null!;

    [Required]
    [StringLength(int.MaxValue)]
    public string SerialNumber { get; set; } = null!;

    [Required] [StringLength(100)]
    public string Type { get; set; } = null!;

    [Required] [StringLength(100)]
    public string Communication { get; set; } = null!;

    [Required]
    public bool RxLogEnable { get; set; } = false;

    [Required]
    public bool TxLogEnable { get; set; } = false;

    [Required]
    public bool AutoConnect { get; set; } = false;

    [NotMapped]
    internal DeviceSocketServerSetting? SocketServerSetting { get; set; }

    [NotMapped]
    internal DeviceSocketClientSetting? SocketClientSetting { get; set; }

    [NotMapped]
    internal DeviceSerialSetting? SerialSetting { get; set; }

    [NotMapped]
    internal DeviceScaleSetting? ScaleSetting { get; set; }

    [NotMapped]
    internal DeviceLabelPrinterSetting? LabelPrinterSetting { get; set; }

    [NotMapped]
    internal DeviceBarcodeScannerSetting? BarcodeScannerSetting { get; set; }
}

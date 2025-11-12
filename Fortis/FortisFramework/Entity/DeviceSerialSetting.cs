using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("DeviceSerialSetting")]
[PrimaryKey("DeviceName")]
public class DeviceSerialSetting
{
    [Key] [Required] [StringLength(100)]
    public string DeviceName { get; set; } = null!;

    [Required] [StringLength(100)]
    public string Encoding { get; set; } = null!;

    [Required] [StringLength(100)]
    public string PortName { get; set; } = null!;

    [Required] [StringLength(100)]
    public string BaudRate { get; set; } = null!;

    [Required] [StringLength(100)]
    public string Parity { get; set; } = null!;

    [Required] [StringLength(100)]
    public string DataBit { get; set; } = null!;

    [Required] [StringLength(100)]
    public string StopBit { get; set; } = null!;

    [Required]
    public int ReadBufferSize { get; set; } = 0;

    [Required]
    public int WriteBufferSize { get; set; } = 0;
}

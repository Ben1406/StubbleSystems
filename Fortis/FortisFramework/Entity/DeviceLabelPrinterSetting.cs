using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("DeviceLabelPrinterSetting")]
[PrimaryKey("DeviceName")]
public class DeviceLabelPrinterSetting
{
    [Key] [Required] [StringLength(100)]
    public string DeviceName { get; set; } = null!;

    [Required] [StringLength(100)]
    public string Type { get; set; } = null!;

    [Required] [StringLength(100)]
    public string Protocol { get; set; } = null!;

    [StringLength(100)]
    public string Dpi { get; set; } = null!;

    [StringLength(100)]
    public string Rotate { get; set; } = null!;

    [StringLength(100)]
    public string? PrintMode { get; set; }

    [StringLength(100)]
    public string? MediaType { get; set; }

    [StringLength(100)]
    public string? PaperType { get; set; }

    public short? PrintSpeed { get; set; }

    public short? LabelLength { get; set; }

    public short? LabelWidth { get; set; }

    public short? StartAdjust { get; set; }

    public short? StopAdjust { get; set; }

    public short? LeftMargin { get; set; }

    public bool? PrintButtonCopy { get; set; }

    [StringLength(100)]
    public string? ProgramBeforePrint { get; set; }

    [StringLength(100)]
    public string? ProgramAfterPrint { get; set; }
}

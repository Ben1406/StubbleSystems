using FortisFramework.Utilities;
using Microsoft.EntityFrameworkCore;

namespace FortisFramework.Entity;

public class FortisFrameworkDbContext : DbContext
{
    public readonly ISystemSetting _systemSetting;

    public FortisFrameworkDbContext(ISystemSetting systemSetting)
    {
        _systemSetting = systemSetting;
    }

    public DbSet<TerminalSetting> TerminalSetting { get; set; } = null!;
    public DbSet<TerminalDevice> TerminalDevice { get; set; } = null!;
    public DbSet<DeviceSetting> DeviceSetting { get; set; } = null!;
    public DbSet<DeviceSocketServerSetting> DeviceSocketServerSetting { get; set; } = null!;
    public DbSet<DeviceSocketClientSetting> DeviceSocketClientSetting { get; set; } = null!;
    public DbSet<DeviceSerialSetting> DeviceSerialSetting { get; set; } = null!;
    public DbSet<DeviceScaleSetting> DeviceScaleSetting { get; set; } = null!;
    public DbSet<DeviceLabelPrinterSetting> DeviceLabelPrinterSetting { get; set; } = null!;
    public DbSet<DeviceBarcodeScannerSetting> DeviceBarcodeScannerSetting { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(_systemSetting.MsSqlConnectionString);
    }
}

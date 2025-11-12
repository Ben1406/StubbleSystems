using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace FortisFramework.Utilities;

public interface ISystemSetting
{
    public string TerminalName { get; set; }
    public string MsSqlServerName { get; set; }
    public string MsSqlDatabaseName { get; set; }
    bool MsSqlTrustServerCertificateEnabled { get; set; }
    MsSqlAuthenticationType MsSqlAuthenticationType { get; set; }
    MsSqlConnectionSecurityEncryption MsSqlConnectionSecurityEncryption { get; set; }
    int MsSqlConnectionTimeout { get; set; }
    string MsSqlUserId { get; set; }
    string MsSqlUserPassword { get; set; }

    string MsSqlConnectionString { get; }
    DateTime SystemStartupTime { get; }
    string VersionNumberCore { get; }
    string VersionNumberClient { get; }
    string WindowsComputerName { get; }
    string WindowsUserName { get; }
    string WindowsApplicationFolderPath { get; }
    string WindowsLocalApplicationDataFolderPath { get; }
    string WindowsDocumentFolderPath { get; }
}

public class SystemSetting : ISystemSetting
{
    public string TerminalName { get; set; } = string.Empty;
    public string MsSqlServerName { get; set; } = string.Empty;
    public string MsSqlDatabaseName { get; set; } = string.Empty;
    public bool MsSqlTrustServerCertificateEnabled { get; set; } = true;
    public MsSqlAuthenticationType MsSqlAuthenticationType { get; set; } = MsSqlAuthenticationType.WindowsAuthentication;
    public MsSqlConnectionSecurityEncryption MsSqlConnectionSecurityEncryption { get; set; } = MsSqlConnectionSecurityEncryption.Optional;
    public int MsSqlConnectionTimeout { get; set; } = 30;
    public string MsSqlUserId { get; set; } = string.Empty;
    public string MsSqlUserPassword { get; set; } = string.Empty;

    public string MsSqlConnectionString => GetConnectionStringSettings().ConnectionString;
    public DateTime SystemStartupTime => GetStartupTime();
    public string VersionNumberCore => GetCoreVersionNumber();
    public string VersionNumberClient => GetClientVersionNumber();
    public string WindowsComputerName => GetComputerName();
    public string WindowsUserName => System.Security.Principal.WindowsIdentity.GetCurrent().Name;
    public string WindowsApplicationFolderPath => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? string.Empty;
    public string WindowsLocalApplicationDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    public string WindowsDocumentFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal);

    private ConnectionStringSettings GetConnectionStringSettings()
    {
        var integratedSecurity = MsSqlAuthenticationType == MsSqlAuthenticationType.WindowsAuthentication;
        var connectTimeout = MsSqlConnectionTimeout > 10 ? MsSqlConnectionTimeout : 10;

        var connectionStringBuilder = new SqlConnectionStringBuilder
        {
            DataSource = MsSqlServerName ?? "",
            InitialCatalog = MsSqlDatabaseName ?? "",
            UserID = integratedSecurity ? string.Empty : MsSqlUserId ?? "",
            Password = integratedSecurity ? string.Empty : MsSqlUserPassword ?? "",
            ConnectTimeout = connectTimeout,
            MultipleActiveResultSets = true,
            IntegratedSecurity = integratedSecurity,
            TrustServerCertificate = MsSqlTrustServerCertificateEnabled
        };

        var connectionStringSettings =
            new ConnectionStringSettings("FortisConnectionString", connectionStringBuilder.ConnectionString)
            {
                ProviderName = "System.Data.SqlClient"
            };

        return connectionStringSettings;
    }

    private DateTime? _startupTime;
    private DateTime GetStartupTime()
    {
        if (_startupTime is null)
        {
            _startupTime = DateTime.Now;
        }

        return (DateTime)_startupTime;
    }

    private string GetCoreVersionNumber()
    {
        return Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString(4) ?? "";
    }

    private string GetClientVersionNumber()
    {
        return Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString(4) ?? "";
    }

    private string GetComputerName()
    {
        return Environment.MachineName ?? "";
    }
}

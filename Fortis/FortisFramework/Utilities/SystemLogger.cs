using System.Data;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace FortisFramework.Utilities;

public interface ISystemLogger
{
    // Level 6
    void Fatal(string message);
    void Fatal(Exception exception, string message);
    // Level 5
    void Error(string message);
    void Error(Exception exception, string message);
    // Level 4
    void Warning(string message);
    void Warning(Exception exception, string message);
    // Level 3
    void Information(string message);
    // Level 2
    void Debug(string message);
    void Debug(Exception exception, string message);
    // Level 1
    void Verbose(string message);
    void Verbose(Exception exception, string message);
}

public class SystemLogger : ISystemLogger
{
    private readonly ISystemSetting _systemSettings;

    public SystemLogger(ISystemSetting systemSettings)
    {
        _systemSettings = systemSettings;

        var columnOptions = new ColumnOptions();
        columnOptions.Store.Remove(StandardColumn.MessageTemplate);
        columnOptions.Store.Remove(StandardColumn.Properties);
        columnOptions.TimeStamp.ColumnName = "InsertedTime";
        columnOptions.Level.ColumnName = "MessageLevel";
        columnOptions.Message.ColumnName = "Message";
        columnOptions.Exception.ColumnName = "Exception";
        columnOptions.AdditionalColumns = new List<SqlColumn> { new SqlColumn("Application", SqlDbType.NVarChar) { DataLength = 100 } };

        var sqlServerOptions = new MSSqlServerSinkOptions
        {
            TableName = "SystemLog",
            AutoCreateSqlDatabase = false,
            AutoCreateSqlTable = false
        };

        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.MSSqlServer
            (
                connectionString: _systemSettings.MsSqlConnectionString,
                sinkOptions: sqlServerOptions,
                columnOptions: columnOptions,
                restrictedToMinimumLevel: LogEventLevel.Verbose
            );

        loggerConfiguration.Enrich.WithProperty("Application", $"Terminal: {_systemSettings.TerminalName}");

        Log.Logger = loggerConfiguration.CreateLogger();
    }

    // Level 6
    public void Fatal(string message) => Log.Fatal(message);
    public void Fatal(Exception exception, string message) => Log.Fatal(exception, message);
    // Level 5
    public void Error(string message) => Log.Error(message);
    public void Error(Exception exception, string message) => Log.Error(exception, message);
    // Level 4
    public void Warning(string message) => Log.Warning(message);
    public void Warning(Exception exception, string message) => Log.Warning(exception, message);
    // Level 3
    public void Information(string message) => Log.Information(message);
    // Level 2
    public void Debug(string message) => Log.Debug(message);
    public void Debug(Exception exception, string message) => Log.Debug(exception, message);
    // Level 1
    public void Verbose(string message) => Log.Verbose(message);
    public void Verbose(Exception exception, string message) => Log.Verbose(exception, message);
}

using Autofac;
using FortisDeviceCenter;
using FortisFramework.Utilities;

namespace FortisFramework;

public class FortisFrameworkBase
{
    public IContainer BaseContainer { get; }

    private readonly ISystemLogger _systemLogger;
    private readonly IDeviceCenter _deviceCenter;
    private readonly ITerminal _terminal;

    public FortisFrameworkBase(SystemSetting systemSetting)
    {
        BaseContainer = CompositionRoot.Configure();

        var iSystemSetting = BaseContainer.Resolve<ISystemSetting>();
        iSystemSetting.TerminalName = systemSetting.TerminalName;
        iSystemSetting.MsSqlServerName = systemSetting.MsSqlServerName;
        iSystemSetting.MsSqlDatabaseName = systemSetting.MsSqlDatabaseName;
        iSystemSetting.MsSqlTrustServerCertificateEnabled = systemSetting.MsSqlTrustServerCertificateEnabled;
        iSystemSetting.MsSqlAuthenticationType = systemSetting.MsSqlAuthenticationType;
        iSystemSetting.MsSqlConnectionSecurityEncryption = systemSetting.MsSqlConnectionSecurityEncryption;

        _systemLogger = BaseContainer.Resolve<ISystemLogger>();
        _systemLogger.Information($"Framework started | ComputerName: {systemSetting.WindowsComputerName} | CoreVersion: {systemSetting.VersionNumberCore} | ClientVersion: {systemSetting.VersionNumberClient}");

        _deviceCenter = BaseContainer.Resolve<IDeviceCenter>();
        _terminal = BaseContainer.Resolve<ITerminal>();
    }

    private bool _isInitialized = false;
    public void Initialize()
    {
        if (_isInitialized)
        {
            _systemLogger.Error("Framework already initialized");
            return;
        }

        _systemLogger.Information($"Framework Initializing");

        _terminal.InitializeTerminalFromDatabase();
        _terminal.InitializeDevicesFromDatabase();

        _systemLogger.Information("Framework Initialized successfully");

        //_deviceCenter.ScaleWeightResult += OnScaleWeightResult;
        //_deviceCenter.BarcodeScannerResult += OnBarcodeScannerResult;
        //_deviceCenter.ReceivedData += OnDeviceCenter_ReceivedData;
        //_deviceCenter.TransmittedData += OnDeviceCenter_TransmittedData;
        _deviceCenter.Messages += OnDeviceCenter_Messages;

        _isInitialized = true;
    }

    private void OnDeviceCenter_Messages(object? sender, DeviceCenterMessageEventArgs e)
    {
        switch (e.MessageLevel)
        {
            case FortisCommunication.MessageLevel.Info:
                _systemLogger.Information(e.Text);
                break;
            case FortisCommunication.MessageLevel.Success:
                _systemLogger.Information(e.Text);
                break;
            case FortisCommunication.MessageLevel.Error:
                if (e.Exception is null) _systemLogger.Error(e.Text);
                else _systemLogger.Error(e.Exception, e.Text);
                break;
        }
    }
}

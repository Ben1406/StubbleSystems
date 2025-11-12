using System.Net;
using FortisCommunication.Serial;
using FortisDeviceCenter;
using FortisDeviceCenter.Scale;
using FortisDeviceCenter.LabelPrinter;
using FortisDeviceCenter.BarcodeScanner;
using FortisFramework.Entity;

namespace FortisFramework.Utilities;

public interface ITerminal
{
    string Name { get; set; }
    string Description { get; set; }

    TerminalSetting? TerminalSetting { get; set; }
    List<TerminalDevice>? TerminalDevices { get; set; }

    bool InitializeTerminalFromDatabase();
    bool InitializeDevicesFromDatabase();
}

public class Terminal(
    ISystemSetting systemSettings,
    ISystemLogger systemLogger,
    IDeviceCenter deviceCenter,
    Func<FortisFrameworkDbContext> createDbContext) : ITerminal
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public TerminalSetting? TerminalSetting { get; set; }
    public List<TerminalDevice>? TerminalDevices { get; set; }

    public bool InitializeTerminalFromDatabase()
    {
        if (TerminalSetting is not null)
        {
            systemLogger.Error("Terminal already added");
            return false;
        }

        using var dbContext = createDbContext();

        TerminalSetting = dbContext.TerminalSetting
            .FirstOrDefault(x => x.TerminalName == systemSettings.TerminalName);

        if (TerminalSetting is null)
        {
            systemLogger.Error("Terminal settings not found");
            return false;
        }

        systemLogger.Information($"Terminal is added successfully (name: '{TerminalSetting.TerminalName}', description: '{TerminalSetting.Description}')");

        return true;
    }

    public bool InitializeDevicesFromDatabase()
    {
        if (TerminalSetting is null)
        {
            systemLogger.Error("Terminal not added");
            return false;
        }

        using var dbContext = createDbContext();

        TerminalDevices = dbContext.TerminalDevice
            .Where(x => x.TerminalName == systemSettings.TerminalName)
            .ToList();

        if (TerminalDevices.Count == 0)
        {
            systemLogger.Information("No device settings found for terminal");
            return true;
        }

        foreach (var terminalDevice in TerminalDevices)
        {
            var deviceName = terminalDevice.DeviceName;
            terminalDevice.DeviceSetting = dbContext.DeviceSetting
                .FirstOrDefault(x => x.DeviceName == deviceName);
        }

        foreach (var terminalDevice in TerminalDevices)
        {
            var deviceName = terminalDevice.DeviceName;
            var deviceSetting = terminalDevice.DeviceSetting;
            var deviceId = terminalDevice.DeviceId;

            if (deviceSetting is not null)
            {
                deviceSetting.SocketServerSetting = dbContext.DeviceSocketServerSetting
                    .FirstOrDefault(x => x.DeviceName == deviceName);
                deviceSetting.SocketClientSetting = dbContext.DeviceSocketClientSetting
                    .FirstOrDefault(x => x.DeviceName == deviceName);
                deviceSetting.SerialSetting = dbContext.DeviceSerialSetting
                    .FirstOrDefault(x => x.DeviceName == deviceName);

                deviceSetting.ScaleSetting = dbContext.DeviceScaleSetting
                    .FirstOrDefault(x => x.DeviceName == deviceName);
                deviceSetting.LabelPrinterSetting = dbContext.DeviceLabelPrinterSetting
                    .FirstOrDefault(x => x.DeviceName == deviceName);
                deviceSetting.BarcodeScannerSetting = dbContext.DeviceBarcodeScannerSetting
                    .FirstOrDefault(x => x.DeviceName == deviceName);

                var rxLogEnable = deviceSetting.RxLogEnable;
                var txLogEnable = deviceSetting.TxLogEnable;
                var autoConnect = deviceSetting.AutoConnect;

                var r1 = Enum.TryParse<DeviceType>(deviceSetting.Type, out var deviceTypeAsEnum);
                var r2 = Enum.TryParse<DeviceCommunication>(deviceSetting.Communication, out var deviceCommunicationAsEnum);
                if (!r1)
                {
                    systemLogger.Error($"Device type '{deviceSetting.Type}' for '{deviceName}' is not correct");
                    return false;
                }
                if (!r2)
                {
                    systemLogger.Error($"Device communication '{deviceSetting.Communication}' for '{deviceName}' is not correct");
                    return false;
                }

                var deviceSettings = new Device(deviceName, deviceSetting.Description, deviceSetting.SerialNumber, deviceTypeAsEnum, deviceCommunicationAsEnum)
                {
                    ClientDeviceId = deviceId,
                    RxLogEnable = rxLogEnable,
                    TxLogEnable = txLogEnable,
                    AutoConnect = autoConnect
                };
                deviceCenter.CreateDevice(deviceName, deviceSettings);

                systemLogger.Information($"Device is added successfully (devicename: '{deviceName}' deviceid: '{deviceId}', type: '{deviceSetting.Type}', communication: '{deviceSetting.Communication}', rxlog: '{rxLogEnable}', txlog: '{txLogEnable}', autoconnect: '{autoConnect}')");

                if (deviceTypeAsEnum == DeviceType.Scale)
                {
                    var scaleSetting = deviceSetting.ScaleSetting;
                    if (scaleSetting is not null)
                    {
                        var r111 = Enum.TryParse<ScaleType>(scaleSetting.Type, out var typeAsEnum);
                        var r112 = Enum.TryParse<ScaleProtocol>(scaleSetting.Protocol, out var protocolAsEnum);
                        if (!r111)
                        {
                            systemLogger.Error($"Scale type '{scaleSetting.Type}' for '{deviceName}' is not correct");
                            return false;
                        }
                        if (!r112)
                        {
                            systemLogger.Error($"Scale protocol '{scaleSetting.Protocol}' for '{deviceName}' is not correct");
                            return false;
                        }

                        var settings = new DeviceSettings.ScaleSettings
                        {
                            Type = typeAsEnum,
                            Protocol = protocolAsEnum,
                            AllowTareFromIndicator = scaleSetting.AllowTareFromIndicator
                        };
                        deviceCenter.AddScaleSettingsToDevice(deviceName, settings);

                        systemLogger.Information($"Scale is added successfully (devicename: '{deviceName}', type: '{scaleSetting.Type}', protocol: '{scaleSetting.Protocol}', allowtarefromindicator: '{scaleSetting.AllowTareFromIndicator}')");
                    }
                }

                if (deviceTypeAsEnum == DeviceType.LabelPrinter)
                {
                    var labelPrinterSetting = deviceSetting.LabelPrinterSetting;
                    if (labelPrinterSetting is not null)
                    {
                        var r121 = Enum.TryParse<LabelPrinterType>(labelPrinterSetting.Type, out var typeAsEnum);
                        var r122 = Enum.TryParse<LabelPrinterProtocol>(labelPrinterSetting.Protocol, out var protocolAsEnum);
                        var r123 = Enum.TryParse<LabelPrinterDpi>(labelPrinterSetting.Dpi, out var dpiAsEnum);
                        var r124 = Enum.TryParse<LabelPrinterRotate>(labelPrinterSetting.Rotate, out var rotateAsEnum);
                        var r125 = Enum.TryParse<LabelPrinterPrintMode>(labelPrinterSetting.PrintMode, out var printModeAsEnum);
                        var r126 = Enum.TryParse<LabelPrinterMediaType>(labelPrinterSetting.MediaType, out var mediaTypeAsEnum);
                        var r127 = Enum.TryParse<LabelPrinterPaperType>(labelPrinterSetting.PaperType, out var paperTypeAsEnum);
                        if (!r121)
                        {
                            systemLogger.Error($"Labelprinter type '{labelPrinterSetting.Type}' for '{deviceName}' is not correct");
                            return false;
                        }
                        if (!r122)
                        {
                            systemLogger.Error($"Labelprinter protocol '{labelPrinterSetting.Protocol}' for '{deviceName}' is not correct");
                            return false;
                        }
                        if (!r123)
                        {
                            systemLogger.Error($"Labelprinter dpi '{labelPrinterSetting.Dpi}' for '{deviceName}' is not correct");
                            return false;
                        }
                        if (!r124)
                        {
                            systemLogger.Error($"Labelprinter rotate '{labelPrinterSetting.Rotate}' for '{deviceName}' is not correct");
                            return false;
                        }
                        if (!r125)
                        {
                            systemLogger.Error($"Labelprinter printmode '{labelPrinterSetting.PrintMode}' for '{deviceName}' is not correct");
                            return false;
                        }
                        if (!r126)
                        {
                            systemLogger.Error($"Labelprinter mediatype '{labelPrinterSetting.MediaType}' for '{deviceName}' is not correct");
                            return false;
                        }
                        if (!r127)
                        {
                            systemLogger.Error($"Labelprinter papertype '{labelPrinterSetting.PaperType}' for '{deviceName}' is not correct");
                            return false;
                        }

                        var settings = new DeviceSettings.LabelPrinterSettings
                        {
                            Type = typeAsEnum,
                            Protocol = protocolAsEnum,
                            Dpi = dpiAsEnum,
                            Rotate = rotateAsEnum,
                            PrintMode = printModeAsEnum,
                            MediaType = mediaTypeAsEnum,
                            PaperType = paperTypeAsEnum,
                            PrintSpeed = labelPrinterSetting.PrintSpeed,
                            LabelLength = labelPrinterSetting.LabelLength,
                            LabelWidth = labelPrinterSetting.LabelWidth,
                            StartAdjust = labelPrinterSetting.StartAdjust,
                            StopAdjust = labelPrinterSetting.StopAdjust,
                            LeftMargin = labelPrinterSetting.LeftMargin,
                            PrintButtonCopy = labelPrinterSetting.PrintButtonCopy,
                            ProgramBeforePrint = labelPrinterSetting.ProgramBeforePrint,
                            ProgramAfterPrint = labelPrinterSetting.ProgramAfterPrint
                        };
                        deviceCenter.AddLabelPrinterSettingsToDevice(deviceName, settings);

                        systemLogger.Information($"Labelprinter is added successfully (devicename: '{deviceName}', type: '{labelPrinterSetting.Type}', protocol: '{labelPrinterSetting.Protocol}', dpi: '{labelPrinterSetting.Dpi}', rotate: '{labelPrinterSetting.Rotate}', printmode '{labelPrinterSetting.PrintMode}', mediatype: '{labelPrinterSetting.MediaType}', papertype: '{labelPrinterSetting.PaperType}', printspeed: '{labelPrinterSetting.PrintSpeed}', labellength: '{labelPrinterSetting.LabelLength}', labelwidth: '{labelPrinterSetting.LabelWidth}', startadjust: '{labelPrinterSetting.StartAdjust}', stopadjust: '{labelPrinterSetting.StopAdjust}', leftmargin: '{labelPrinterSetting.LeftMargin}', printbuttoncopy: '{labelPrinterSetting.PrintButtonCopy}', programbeforeprint: '{labelPrinterSetting.ProgramBeforePrint}', programafterprint: '{labelPrinterSetting.ProgramAfterPrint}')");
                    }
                }

                if (deviceTypeAsEnum == DeviceType.BarcodeScanner)
                {
                    var barcodeScannerSetting = deviceSetting.BarcodeScannerSetting;
                    if (barcodeScannerSetting is not null)
                    {
                        var r131 = Enum.TryParse<BarcodeScannerType>(barcodeScannerSetting.Type, out var typeAsEnum);
                        var r132 = Enum.TryParse<BarcodeScannerProtocol>(barcodeScannerSetting.Protocol, out var protocolAsEnum);
                        if (!r131)
                        {
                            systemLogger.Error($"Barcodescanner type '{barcodeScannerSetting.Type}' for '{deviceName}' is not correct");
                            return false;
                        }
                        if (!r132)
                        {
                            systemLogger.Error($"Barcodescanner protocol '{barcodeScannerSetting.Protocol}' for '{deviceName}' is not correct");
                            return false;
                        }

                        var settings = new DeviceSettings.BarcodeScannerSettings
                        {
                            Type = typeAsEnum,
                            Protocol = protocolAsEnum,
                            SendFeedbackToHost = barcodeScannerSetting.SendFeedbackToHost
                        };
                        deviceCenter.AddBarcodeScannerSettingsToDevice(deviceName, settings);

                        systemLogger.Information($"Barcodescanner is added successfully (devicename: '{deviceName}', type: '{barcodeScannerSetting.Type}', protocol: '{barcodeScannerSetting.Protocol}', sendfeedbacktohost: '{barcodeScannerSetting.SendFeedbackToHost}')");
                    }
                }

                if (deviceTypeAsEnum == DeviceType.Typeless)
                {
                }

                if (deviceCommunicationAsEnum == DeviceCommunication.SocketServer)
                {
                    var socketServerSetting = deviceSetting.SocketServerSetting;
                    if (socketServerSetting is not null)
                    {
                        var deviceEncoding = Helper.ConvertToEncoding(socketServerSetting.Encoding);
                        var settings = new DeviceSettings.SocketServerSettings
                        {
                            Encoding = deviceEncoding,
                            IpPort = socketServerSetting.IpPort
                        };
                        deviceCenter.AddSocketServerSettingsToDevice(deviceName, settings);
                    }
                }

                if (deviceCommunicationAsEnum == DeviceCommunication.SocketClient)
                {
                    var socketClientSetting = deviceSetting.SocketClientSetting;
                    if (socketClientSetting is not null)
                    {
                        var deviceEncoding = Helper.ConvertToEncoding(socketClientSetting.Encoding);
                        var r211 = IPAddress.TryParse(socketClientSetting.IpAddress, out var ipAddress);
                        if (!r211 || ipAddress is null)
                        {
                            systemLogger.Error($"Socketclient ipaddress '{socketClientSetting.IpAddress}' for '{deviceName}' is not correct");
                            return false;
                        }

                        var settings = new DeviceSettings.SocketClientSettings
                        {
                            Encoding = deviceEncoding,
                            IpAddress = ipAddress,
                            IpPort = socketClientSetting.IpPort
                        };
                        deviceCenter.AddSocketClientSettingsToDevice(deviceName, settings);

                        systemLogger.Information($"Socketclient is added successfully (devicename: '{deviceName}', ipaddress: '{socketClientSetting.IpAddress}', ipport: '{socketClientSetting.IpPort}', encoding: '{socketClientSetting.Encoding}')");
                    }
                }

                if (deviceCommunicationAsEnum == DeviceCommunication.SerialComport)
                {
                    var serialSetting = deviceSetting.SerialSetting;
                    if (serialSetting is not null)
                    {
                        var deviceEncoding = Helper.ConvertToEncoding(serialSetting.Encoding);
                        var r221 = Enum.TryParse<SerialPortName>(serialSetting.PortName, out var portNameAsEnum);
                        var r222 = Enum.TryParse<SerialBaudRate>(serialSetting.BaudRate, out var baudRateAsEnum);
                        var r223 = Enum.TryParse<SerialParity>(serialSetting.Parity, out var parityAsEnum);
                        var r224 = Enum.TryParse<SerialDataBit>(serialSetting.DataBit, out var dataBitAsEnum);
                        var r225 = Enum.TryParse<SerialStopBit>(serialSetting.StopBit, out var stopBitAsEnum);
                        if (!r221)
                        {
                            systemLogger.Error($"Serialcomport portname '{serialSetting.PortName}' for '{deviceName}' is not correct");
                            return false;
                        }
                        if (!r222)
                        {
                            systemLogger.Error($"Serialcomport baudrate '{serialSetting.BaudRate}' for '{deviceName}' is not correct");
                            return false;
                        }
                        if (!r223)
                        {
                            systemLogger.Error($"Serialcomport parity '{serialSetting.Parity}' for '{deviceName}' is not correct");
                            return false;
                        }
                        if (!r224)
                        {
                            systemLogger.Error($"Serialcomport databit '{serialSetting.DataBit}' for '{deviceName}' is not correct");
                            return false;
                        }
                        if (!r225)
                        {
                            systemLogger.Error($"Serialcomport stopbit '{serialSetting.StopBit}' for '{deviceName}' is not correct");
                            return false;
                        }

                        var settings = new DeviceSettings.SerialComportSettings
                        {
                            Encoding = deviceEncoding,
                            PortName = portNameAsEnum,
                            BaudRate = baudRateAsEnum,
                            Parity = parityAsEnum,
                            DataBit = dataBitAsEnum,
                            StopBit = stopBitAsEnum,
                            ReadBufferSize = serialSetting.ReadBufferSize,
                            WriteBufferSize = serialSetting.WriteBufferSize
                        };
                        deviceCenter.AddSerialSettingsToDevice(deviceName, settings);

                        systemLogger.Information($"Serialcomport is added successfully (devicename: '{deviceName}', portname: '{serialSetting.PortName}', baudrate: '{serialSetting.BaudRate}', parity: '{serialSetting.Parity}', databit: '{serialSetting.DataBit}', stopbit: '{serialSetting.StopBit}', readbuffersize: '{serialSetting.ReadBufferSize}', writebuffersize: '{serialSetting.WriteBufferSize}', encoding: '{serialSetting.Encoding}')");
                    }
                }

                deviceCenter.FinalizeConnection(deviceName);
            }
        }

        return true;
    }
}

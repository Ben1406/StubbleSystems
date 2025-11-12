using FortisCommunication.Serial;
using FortisCommunication.SocketClient;
using FortisCommunication.SocketServer;

namespace FortisDeviceCenter;

public class Device(string deviceName, string description, string serialNumber, DeviceType deviceType, DeviceCommunication communication)
{
    // Mandatory
    public string DeviceName { get; set; } = deviceName;
    public string Description { get; set; } = description;
    public string SerialNumber { get; set; } = serialNumber;
    public DeviceType Type { get; set; } = deviceType;
    public DeviceCommunication Communication { get; set; } = communication;

    // Default
    public bool RxLogEnable { get; set; } = false;
    public bool TxLogEnable { get; set; } = false;
    public bool AutoConnect { get; set; } = false;

    // Client Setters
    public short? ClientDeviceId { get; set; }

    // Communication
    internal DeviceSettings.SocketServerSettings? SocketServerSettings { get; set; }
    internal SocketServer? SocketServerConnection { get; set; }

    internal DeviceSettings.SocketClientSettings? SocketClientSettings { get; set; }
    internal SocketClient? SocketClientConnection { get; set; }

    internal DeviceSettings.SerialComportSettings? SerialSettings { get; set; }
    internal Serial? SerialConnection { get; set; }

    // Device Settings
    internal DeviceSettings.ScaleSettings? ScaleSettings { get; set; }
    internal DeviceSettings.LabelPrinterSettings? LabelPrinterSettings { get; set; }
    internal DeviceSettings.BarcodeScannerSettings? BarcodeScannerSettings { get; set; }

    // Receive Buffer
    internal string TextBuffer { get; set; } = string.Empty;
    internal byte[] BytesBuffer { get; set; } = [];
}

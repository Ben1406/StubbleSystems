using System.Net;
using System.Text;
using FortisCommunication;
using FortisCommunication.Serial;
using FortisCommunication.SocketClient;
using FortisCommunication.SocketServer;
using FortisDeviceCenter.BarcodeScanner;
using FortisDeviceCenter.BarcodeScanner.Decoders;
using FortisDeviceCenter.Scale;
using FortisDeviceCenter.Scale.Decoders;

namespace FortisDeviceCenter;

public interface IDeviceCenter
{
    bool CreateDevice(string deviceName, Device device);

    bool AddSocketServerSettingsToDevice(string deviceName, DeviceSettings.SocketServerSettings settings);
    bool AddSocketClientSettingsToDevice(string deviceName, DeviceSettings.SocketClientSettings settings);
    bool AddSerialSettingsToDevice(string deviceName, DeviceSettings.SerialComportSettings settings);
    bool AddScaleSettingsToDevice(string deviceName, DeviceSettings.ScaleSettings settings);
    bool AddLabelPrinterSettingsToDevice(string deviceName, DeviceSettings.LabelPrinterSettings settings);
    bool AddBarcodeScannerSettingsToDevice(string deviceName, DeviceSettings.BarcodeScannerSettings settings);

    bool FinalizeConnection(string deviceName);

    bool OpenDevice(string deviceName);
    bool CloseDevice(string deviceName);
    bool IsConnected(string deviceName);

    bool TransmitData(string deviceName, string text);
    bool TransmitData(string deviceName, byte[] bytes);
    
    void SimulateScaleWeightResult(string deviceName, short? clientDeviceId, ScaleWeightResult scaleWeightResult);
    void SimulateBarcodeScannerResult(string deviceName, short? clientDeviceId, BarcodeScannerResult barcodeScannerResult);

    event EventHandler<DeviceCenterDataEventArgs> ReceivedData;
    event EventHandler<DeviceCenterDataEventArgs> TransmittedData;
    event EventHandler<DeviceCenterMessageEventArgs> Messages;
    event EventHandler<ScaleWeightResultEventArgs> ScaleWeightResult;
    event EventHandler<BarcodeScannerResultEventArgs> BarcodeScannerResult;
    event EventHandler<DeviceCenterDeviceStatusEventArgs> DeviceStatusChanged;

    List<Device> Devices { get; }
}

public class DeviceCenter : IDeviceCenter
{
    public event EventHandler<DeviceCenterDataEventArgs> ReceivedData = delegate { };
    public event EventHandler<DeviceCenterDataEventArgs> TransmittedData = delegate { };
    public event EventHandler<DeviceCenterMessageEventArgs> Messages = delegate { };
    public event EventHandler<ScaleWeightResultEventArgs> ScaleWeightResult = delegate { };
    public event EventHandler<BarcodeScannerResultEventArgs> BarcodeScannerResult = delegate { };
    public event EventHandler<DeviceCenterDeviceStatusEventArgs> DeviceStatusChanged = delegate { };

    public List<Device> Devices { get; } = new List<Device>();

    public bool CreateDevice(string deviceName, Device device)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is not null)
        {
            return false;
        }

        Devices.Add(device);
        return true;
    }

    public bool AddSocketServerSettingsToDevice(string deviceName, DeviceSettings.SocketServerSettings settings)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is null)
        {
            return false;
        }
        if (existingDevice.Communication != DeviceCommunication.SocketServer)
        {
            return false;
        }
        if (existingDevice.SocketServerSettings is not null)
        {
            return false;
        }

        existingDevice.SocketServerSettings = settings;
        return true;
    }

    public bool AddSocketClientSettingsToDevice(string deviceName, DeviceSettings.SocketClientSettings settings)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is null)
        {
            return false;
        }
        if (existingDevice.Communication != DeviceCommunication.SocketClient)
        {
            return false;
        }
        if (existingDevice.SocketClientSettings is not null)
        {
            return false;
        }
        
        existingDevice.SocketClientSettings = settings;
        return true;
    }

    public bool AddSerialSettingsToDevice(string deviceName, DeviceSettings.SerialComportSettings settings)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is null)
        {
            return false;
        }
        if (existingDevice.Communication != DeviceCommunication.SerialComport)
        {
            return false;
        }
        if (existingDevice.SerialSettings is not null)
        {
            return false;
        }

        existingDevice.SerialSettings = settings;
        return true;
    }

    public bool AddScaleSettingsToDevice(string deviceName, DeviceSettings.ScaleSettings settings)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is null)
        {
            return false;
        }
        if (existingDevice.Type != DeviceType.Scale)
        {
            return false;
        }
        if (existingDevice.ScaleSettings is not null)
        {
            return false;
        }
        
        existingDevice.ScaleSettings = settings;
        return true;
    }

    public bool AddLabelPrinterSettingsToDevice(string deviceName, DeviceSettings.LabelPrinterSettings settings)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is null)
        {
            return false;
        }
        if (existingDevice.Type != DeviceType.LabelPrinter)
        {
            return false;
        }
        if (existingDevice.LabelPrinterSettings is not null)
        {
            return false;
        }

        existingDevice.LabelPrinterSettings = settings;
        return true;
    }

    public bool AddBarcodeScannerSettingsToDevice(string deviceName, DeviceSettings.BarcodeScannerSettings settings)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is null)
        {
            return false;
        }
        if (existingDevice.Type != DeviceType.BarcodeScanner)
        {
            return false;
        }
        if (existingDevice.BarcodeScannerSettings is not null)
        {
            return false;
        }

        existingDevice.BarcodeScannerSettings = settings;
        return true;
    }

    public bool FinalizeConnection(string deviceName)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is null)
        {
            return false;
        }

        var deviceDescription = existingDevice.Description;
        var deviceAutoConnect = existingDevice.AutoConnect;

        switch (existingDevice.Communication)
        {
            case DeviceCommunication.SerialComport:
                if (existingDevice.SerialConnection is not null)
                {
                    return false;
                }
                if (existingDevice.SerialSettings is null)
                {
                    return false;
                }

                var serialEncoding = existingDevice.SerialSettings?.Encoding ?? Encoding.Default;
                var serialPortName = existingDevice.SerialSettings?.PortName ?? SerialPortName.Com1;
                var serialBaudRate = existingDevice.SerialSettings?.BaudRate ?? SerialBaudRate.Rate9600;
                var serialParity = existingDevice.SerialSettings?.Parity ?? SerialParity.None;
                var serialDataBit = existingDevice.SerialSettings?.DataBit ?? SerialDataBit.Eight;
                var serialStopBit = existingDevice.SerialSettings?.StopBit ?? SerialStopBit.One;
                var serialReadBufferSize = existingDevice.SerialSettings?.ReadBufferSize ?? 512;
                var serialWriteBufferSize = existingDevice.SerialSettings?.WriteBufferSize ?? 512;

                var serialDevice = existingDevice.SerialConnection = new Serial(deviceName, deviceDescription,
                    serialEncoding, serialPortName, serialBaudRate, serialParity, serialDataBit, serialStopBit,
                    serialReadBufferSize, serialWriteBufferSize, deviceAutoConnect);

                serialDevice.ReceivedData += OnSerialDevice_ReceivedData;
                serialDevice.TransmittedData += OnSerialDevice_TransmittedData;
                serialDevice.SerialMessages += OnSerialDevice_SerialMessages;
                serialDevice.DeviceStatus += OnDeviceStatusChanged;

                return true;

            case DeviceCommunication.SocketServer:
                if (existingDevice.SocketServerConnection is not null)
                {
                    return false;
                }
                if (existingDevice.SocketServerSettings is null)
                {
                    return false;
                }

                var serverEncoding = existingDevice.SocketServerSettings?.Encoding ?? Encoding.Default;
                var serverIpPort = existingDevice.SocketServerSettings?.IpPort ?? 9100;

                var serverDevice = existingDevice.SocketServerConnection = new SocketServer(deviceName, deviceDescription,
                    serverEncoding, serverIpPort, deviceAutoConnect);

                serverDevice.ReceivedData += OnSocketServerDevice_ReceivedData;
                serverDevice.TransmittedData += OnSocketServerDevice_TransmittedData;
                serverDevice.SocketMessages += OnSocketServerDevice_SocketMessages;
                serverDevice.DeviceStatus += OnDeviceStatusChanged;

                return true;

            case DeviceCommunication.SocketClient:
                if (existingDevice.SocketClientConnection is not null)
                {
                    return false;
                }
                if (existingDevice.SocketClientSettings is null)
                {
                    return false;
                }

                var clientEncoding = existingDevice.SocketClientSettings?.Encoding ?? Encoding.Default;
                var clientIpAddress = existingDevice.SocketClientSettings?.IpAddress ?? IPAddress.Parse("127.0.0.1");
                var clientIpPort = existingDevice.SocketClientSettings?.IpPort ?? 9100;

                var clientDevice = existingDevice.SocketClientConnection = new SocketClient(deviceName, deviceDescription,
                    clientEncoding, clientIpAddress, clientIpPort, deviceAutoConnect);

                clientDevice.ReceivedData += OnSocketClientDevice_ReceivedData;
                clientDevice.TransmittedData += OnSocketClientDevice_TransmittedData;
                clientDevice.SocketMessages += OnSocketClientDevice_SocketMessages;
                clientDevice.DeviceStatus += OnDeviceStatusChanged;

                return true;
        }

        return false;
    }

    public bool OpenDevice(string deviceName)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is null)
        {
            return false;
        }

        switch (existingDevice.Communication)
        {
            case DeviceCommunication.SerialComport:
                if (existingDevice.SerialConnection is null)
                {
                    return false;
                }
                existingDevice.SerialConnection.Connect();
                return true;

            case DeviceCommunication.SocketServer:
                if (existingDevice.SocketServerConnection is null)
                {
                    return false;
                }
                existingDevice.SocketServerConnection.Connect();
                return true;

            case DeviceCommunication.SocketClient:
                if (existingDevice.SocketClientConnection is null)
                {
                    return false;
                }
                existingDevice.SocketClientConnection.Connect();
                return true;
        }

        return false;
    }

    public bool CloseDevice(string deviceName)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is null)
        {
            return false;
        }

        switch (existingDevice.Communication)
        {
            case DeviceCommunication.SerialComport:
                if (existingDevice.SerialConnection is null)
                {
                    return false;
                }
                existingDevice.SerialConnection.Disconnect();
                return true;

            case DeviceCommunication.SocketServer:
                if (existingDevice.SocketServerConnection is null)
                {
                    return false;
                }
                existingDevice.SocketServerConnection.Disconnect();
                return true;

            case DeviceCommunication.SocketClient:
                if (existingDevice.SocketClientConnection is null)
                {
                    return false;
                }
                existingDevice.SocketClientConnection.Disconnect();
                return true;
        }

        return false;
    }

    public bool IsConnected(string deviceName)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is null)
        {
            return false;
        }

        switch (existingDevice.Communication)
        {
            case DeviceCommunication.SerialComport:
                if (existingDevice.SerialConnection is null)
                {
                    return false;
                }
                return existingDevice.SerialConnection.IsConnected();

            case DeviceCommunication.SocketServer:
                if (existingDevice.SocketServerConnection is null)
                {
                    return false;
                }
                return existingDevice.SocketServerConnection.IsConnected();

            case DeviceCommunication.SocketClient:
                if (existingDevice.SocketClientConnection is null)
                {
                    return false;
                }
                return existingDevice.SocketClientConnection.IsConnected();
        }

        return false;
    }

    public bool TransmitData(string deviceName, string text)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is null)
        {
            return false;
        }

        switch (existingDevice.Communication)
        {
            case DeviceCommunication.SerialComport:
                if (existingDevice.SerialConnection is null)
                {
                    return false;
                }
                existingDevice.SerialConnection.TransmitData(text);
                return true;

            case DeviceCommunication.SocketServer:
                if (existingDevice.SocketServerConnection is null)
                {
                    return false;
                }
                existingDevice.SocketServerConnection.TransmitData(text);
                return true;

            case DeviceCommunication.SocketClient:
                if (existingDevice.SocketClientConnection is null)
                {
                    return false;
                }
                existingDevice.SocketClientConnection.TransmitData(text);
                return true;
        }

        return false;
    }

    public bool TransmitData(string deviceName, byte[] bytes)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is null)
        {
            return false;
        }

        switch (existingDevice.Communication)
        {
            case DeviceCommunication.SerialComport:
                if (existingDevice.SerialConnection is null)
                {
                    return false;
                }
                existingDevice.SerialConnection.TransmitData(bytes);
                return true;

            case DeviceCommunication.SocketServer:
                if (existingDevice.SocketServerConnection is null)
                {
                    return false;
                }
                existingDevice.SocketServerConnection.TransmitData(bytes);
                return true;

            case DeviceCommunication.SocketClient:
                if (existingDevice.SocketClientConnection is null)
                {
                    return false;
                }
                existingDevice.SocketClientConnection.TransmitData(bytes);
                return true;
        }

        return false;
    }

    public void SimulateScaleWeightResult(string deviceName, short? clientDeviceId, ScaleWeightResult scaleWeightResult)
    {
        ScaleWeightResult(this, new ScaleWeightResultEventArgs(deviceName, clientDeviceId, scaleWeightResult));
    }

    public void SimulateBarcodeScannerResult(string deviceName, short? clientDeviceId, BarcodeScannerResult barcodeScannerResult)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        BarcodeScannerResult(this, new BarcodeScannerResultEventArgs(deviceName, clientDeviceId, barcodeScannerResult));
    }

    private void OnSerialDevice_ReceivedData(object? sender, SerialDataEventArgs e)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == e.DeviceName);
        ReceivedData(this, new DeviceCenterDataEventArgs(e.DeviceName, existingDevice?.ClientDeviceId, e.Text, e.Bytes));
        HandleProtocol(e.DeviceName, e.Text, e.Bytes);
    }

    private void OnSerialDevice_TransmittedData(object? sender, SerialDataEventArgs e)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == e.DeviceName);
        TransmittedData(this, new DeviceCenterDataEventArgs(e.DeviceName, existingDevice?.ClientDeviceId, e.Text, e.Bytes));
    }

    private void OnSerialDevice_SerialMessages(object? sender, SerialMessageEventArgs e)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == e.DeviceName);
        Messages(this, new DeviceCenterMessageEventArgs(e.DeviceName, existingDevice?.ClientDeviceId, e.Text, e.MessageLevel, e.Exception));
    }

    private void OnSocketServerDevice_ReceivedData(object? sender, SocketServerDataEventArgs e)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == e.DeviceName);
        ReceivedData(this, new DeviceCenterDataEventArgs(e.DeviceName, existingDevice?.ClientDeviceId, e.Text, e.Bytes));
        HandleProtocol(e.DeviceName, e.Text, e.Bytes);
    }

    private void OnSocketServerDevice_TransmittedData(object? sender, SocketServerDataEventArgs e)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == e.DeviceName);
        TransmittedData(this, new DeviceCenterDataEventArgs(e.DeviceName, existingDevice?.ClientDeviceId, e.Text, e.Bytes));
    }

    private void OnSocketServerDevice_SocketMessages(object? sender, SocketServerMessageEventArgs e)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == e.DeviceName);
        Messages(this, new DeviceCenterMessageEventArgs(e.DeviceName, existingDevice?.ClientDeviceId, e.Text, e.MessageLevel, e.Exception));
    }

    private void OnSocketClientDevice_ReceivedData(object? sender, SocketClientDataEventArgs e)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == e.DeviceName);
        ReceivedData(this, new DeviceCenterDataEventArgs(e.DeviceName, existingDevice?.ClientDeviceId, e.Text, e.Bytes));
        HandleProtocol(e.DeviceName, e.Text, e.Bytes);
    }

    private void OnSocketClientDevice_TransmittedData(object? sender, SocketClientDataEventArgs e)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == e.DeviceName);
        TransmittedData(this, new DeviceCenterDataEventArgs(e.DeviceName, existingDevice?.ClientDeviceId, e.Text, e.Bytes));
    }

    private void OnSocketClientDevice_SocketMessages(object? sender, SocketClientMessageEventArgs e)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == e.DeviceName);
        Messages(this, new DeviceCenterMessageEventArgs(e.DeviceName, existingDevice?.ClientDeviceId, e.Text, e.MessageLevel, e.Exception));
    }

    private void OnDeviceStatusChanged(object? sender, DeviceStatusEventArgs e)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == e.DeviceName);
        DeviceStatusChanged(this, new DeviceCenterDeviceStatusEventArgs(e.DeviceName, existingDevice?.ClientDeviceId, e.Connected));
    }

    private void HandleProtocol(string deviceName, string text, byte[] bytes)
    {
        var existingDevice = Devices.SingleOrDefault(x => x.DeviceName == deviceName);
        if (existingDevice is null)
        {
            return;
        }

        var deviceType = existingDevice.Type;

        if (deviceType == DeviceType.None)
        {
            return;
        }

        if (deviceType == DeviceType.LabelPrinter)
        {
            return;
        }

        if (deviceType == DeviceType.BarcodeScanner)
        {
            switch (existingDevice.BarcodeScannerSettings?.Protocol)
            {
                case BarcodeScannerProtocol.StxEtx:
                    existingDevice.TextBuffer += text;
                    var resultBarcodeScannerStxEtx = BarcodeWithSuffix.Decode(existingDevice.TextBuffer, new char[] { Helper.Stx, Helper.Etx });
                    if (resultBarcodeScannerStxEtx.Item2 == DecodeState.Fail)
                    {
                        existingDevice.TextBuffer = string.Empty;
                        existingDevice.BytesBuffer = [];
                    }
                    if (resultBarcodeScannerStxEtx.Item2 == DecodeState.Success)
                    {
                        existingDevice.TextBuffer = string.Empty;
                        existingDevice.BytesBuffer = [];

                        if (resultBarcodeScannerStxEtx.Item1 is not null)
                        {
                            BarcodeScannerResult(this, new BarcodeScannerResultEventArgs(existingDevice.DeviceName, existingDevice?.ClientDeviceId, resultBarcodeScannerStxEtx.Item1));
                        }
                    }
                    break;

                case BarcodeScannerProtocol.CrLf:
                    existingDevice.TextBuffer += text;
                    var resultBarcodeScannerCrLf = BarcodeWithSuffix.Decode(existingDevice.TextBuffer, new char[] { Helper.Cr, Helper.Lf });
                    if (resultBarcodeScannerCrLf.Item2 == DecodeState.Fail)
                    {
                        existingDevice.TextBuffer = string.Empty;
                        existingDevice.BytesBuffer = [];
                    }
                    if (resultBarcodeScannerCrLf.Item2 == DecodeState.Success)
                    {
                        existingDevice.TextBuffer = string.Empty;
                        existingDevice.BytesBuffer = [];

                        if (resultBarcodeScannerCrLf.Item1 is not null)
                        {
                            BarcodeScannerResult(this, new BarcodeScannerResultEventArgs(existingDevice.DeviceName, existingDevice?.ClientDeviceId, resultBarcodeScannerCrLf.Item1));
                        }
                    }
                    break;

                case BarcodeScannerProtocol.Lf:
                    existingDevice.TextBuffer += text;
                    var resultBarcodeScannerLf = BarcodeWithSuffix.Decode(existingDevice.TextBuffer, new char[] { Helper.Lf });
                    if (resultBarcodeScannerLf.Item2 == DecodeState.Fail)
                    {
                        existingDevice.TextBuffer = string.Empty;
                        existingDevice.BytesBuffer = [];
                    }
                    if (resultBarcodeScannerLf.Item2 == DecodeState.Success)
                    {
                        existingDevice.TextBuffer = string.Empty;
                        existingDevice.BytesBuffer = [];

                        if (resultBarcodeScannerLf.Item1 is not null)
                        {
                            BarcodeScannerResult(this, new BarcodeScannerResultEventArgs(existingDevice.DeviceName, existingDevice?.ClientDeviceId, resultBarcodeScannerLf.Item1));
                        }
                    }
                    break;

                case BarcodeScannerProtocol.Cr:
                    existingDevice.TextBuffer += text;
                    var resultBarcodeScannerCr = BarcodeWithSuffix.Decode(existingDevice.TextBuffer, new char[] { Helper.Cr });
                    if (resultBarcodeScannerCr.Item2 == DecodeState.Fail)
                    {
                        existingDevice.TextBuffer = string.Empty;
                        existingDevice.BytesBuffer = [];
                    }
                    if (resultBarcodeScannerCr.Item2 == DecodeState.Success)
                    {
                        existingDevice.TextBuffer = string.Empty;
                        existingDevice.BytesBuffer = [];

                        if (resultBarcodeScannerCr.Item1 is not null)
                        {
                            BarcodeScannerResult(this, new BarcodeScannerResultEventArgs(existingDevice.DeviceName, existingDevice?.ClientDeviceId, resultBarcodeScannerCr.Item1));
                        }
                    }
                    break;

                case BarcodeScannerProtocol.None:
                    existingDevice.TextBuffer += text;
                    var barcodeScannerResult = new BarcodeScannerResult { Barcode = existingDevice.TextBuffer, Length = existingDevice.TextBuffer.Length };
                    BarcodeScannerResult(this, new BarcodeScannerResultEventArgs(existingDevice.DeviceName, existingDevice?.ClientDeviceId, barcodeScannerResult));
                    existingDevice.TextBuffer = string.Empty;
                    existingDevice.BytesBuffer = [];
                    break;
            }
        }

        if (deviceType == DeviceType.Scale)
        {
            switch (existingDevice.ScaleSettings?.Protocol)
            {
                case ScaleProtocol.ScanvaegtCommunicationThree:
                    existingDevice.TextBuffer += text;
                    var resultScvCom3 = ScanvaegtCommunicationThree.Decode(existingDevice.TextBuffer);
                    if (resultScvCom3.Item2 == DecodeState.Fail)
                    {
                        existingDevice.TextBuffer = string.Empty;
                        existingDevice.BytesBuffer = [];
                    }
                    if (resultScvCom3.Item2 == DecodeState.Success)
                    {
                        TransmitData(existingDevice.DeviceName, [6]); // Send acknowledge (ACK)

                        existingDevice.TextBuffer = string.Empty;
                        existingDevice.BytesBuffer = [];

                        if (resultScvCom3.Item1 is not null)
                        {
                            ScaleWeightResult(this, new ScaleWeightResultEventArgs(existingDevice.DeviceName, existingDevice?.ClientDeviceId, resultScvCom3.Item1));
                        }
                    }
                    break;

                    case ScaleProtocol.ScanvaegtContinuousSerialOutput:
                        existingDevice.TextBuffer += text;
                        var resultScvCso = ScanvaegtContinuousSerialOutput.Decode(existingDevice.TextBuffer);
                        if (resultScvCso.Item2 == DecodeState.Fail)
                        {
                            existingDevice.TextBuffer = string.Empty;
                            existingDevice.BytesBuffer = [];
                        }
                        if (resultScvCso.Item2 == DecodeState.Success)
                        {
                            existingDevice.TextBuffer = string.Empty;
                            existingDevice.BytesBuffer = [];

                            if (resultScvCso.Item1 is not null)
                            {
                                ScaleWeightResult(this, new ScaleWeightResultEventArgs(existingDevice.DeviceName, existingDevice?.ClientDeviceId, resultScvCso.Item1));
                            }
                        }
                        break;

                    case ScaleProtocol.SysTekCustomizedProtocol:
                        existingDevice.TextBuffer += text;
                        var resultSysCus = SysTekCustomizedProtocol.Decode(existingDevice.TextBuffer);
                        if (resultSysCus.Item2 == DecodeState.Fail)
                        {
                            existingDevice.TextBuffer = string.Empty;
                            existingDevice.BytesBuffer = [];
                        }
                        if (resultSysCus.Item2 == DecodeState.Success)
                        {
                            existingDevice.TextBuffer = string.Empty;
                            existingDevice.BytesBuffer = [];

                            if (resultSysCus.Item1 is not null)
                            {
                                ScaleWeightResult(this, new ScaleWeightResultEventArgs(existingDevice.DeviceName, existingDevice?.ClientDeviceId, resultSysCus.Item1));
                            }
                        }
                        break;
                    
                    case ScaleProtocol.SysTekExtentedStandardProtocol:
                        existingDevice.TextBuffer += text;
                        var resultSysExt = SysTekExtendedStandardProtocol.Decode(existingDevice.TextBuffer);
                        if (resultSysExt.Item2 == DecodeState.Fail)
                        {
                            existingDevice.TextBuffer = string.Empty;
                            existingDevice.BytesBuffer = [];
                        }
                        if (resultSysExt.Item2 == DecodeState.Success)
                        {
                            existingDevice.TextBuffer = string.Empty;
                            existingDevice.BytesBuffer = [];

                            if (resultSysExt.Item1 is not null)
                            {
                                ScaleWeightResult(this, new ScaleWeightResultEventArgs(existingDevice.DeviceName, existingDevice?.ClientDeviceId, resultSysExt.Item1));
                            }
                        }
                        break;

                    case ScaleProtocol.Toledo:
                        existingDevice.TextBuffer += text;
                        var resultToledo = MettlerToledo.Decode(existingDevice.TextBuffer);
                        if (resultToledo.Item2 == DecodeState.Fail)
                        {
                            existingDevice.TextBuffer = string.Empty;
                            existingDevice.BytesBuffer = [];
                        }
                        if (resultToledo.Item2 == DecodeState.Success)
                        {
                            existingDevice.TextBuffer = string.Empty;
                            existingDevice.BytesBuffer = [];

                            if (resultToledo.Item1 is not null)
                            {
                                ScaleWeightResult(this, new ScaleWeightResultEventArgs(existingDevice.DeviceName, existingDevice?.ClientDeviceId, resultToledo.Item1));
                            }
                        }
                        break;
            }
        }
    }
}

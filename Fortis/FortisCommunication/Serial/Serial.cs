using System.Text;
using System.Timers;
using System.IO.Ports;

namespace FortisCommunication.Serial;

public class Serial
{
    public string DeviceName { get; }
    public string Description { get; }
    public Encoding Encoding { get; }

    public event EventHandler<SerialDataEventArgs> ReceivedData = delegate { };
    public event EventHandler<SerialDataEventArgs> TransmittedData = delegate { };
    public event EventHandler<SerialMessageEventArgs> SerialMessages = delegate { };
    public event EventHandler<DeviceStatusEventArgs> DeviceStatus = delegate { };

    private readonly SerialPort _comPort;
    private readonly System.Timers.Timer _reconnectTimer;

    public Serial(string deviceName, string description, Encoding encoding,
        SerialPortName portName, SerialBaudRate baudRate, SerialParity parity,
        SerialDataBit dataBit, SerialStopBit stopBit, int readBufferSize, int writeBufferSize,
        bool autoConnect)
    {
        DeviceName = deviceName;
        Description = description;
        Encoding = encoding;

        _comPort = new SerialPort();
        _reconnectTimer = new System.Timers.Timer();

        try
        {
            _comPort.PortName = portName.ToString();
            _comPort.BaudRate = (int)baudRate;
            _comPort.Parity = (Parity)parity;
            _comPort.DataBits = (int)dataBit;
            _comPort.StopBits = (StopBits)stopBit;
            _comPort.Encoding = encoding;
            _comPort.ReadBufferSize = readBufferSize > 0 ? readBufferSize : 512;
            _comPort.WriteBufferSize = writeBufferSize > 0 ? writeBufferSize : 512;

            _comPort.ReceivedBytesThreshold = 1;
            _comPort.RtsEnable = true;
            _comPort.DtrEnable = true;
            _comPort.ReadTimeout = 5000;
            _comPort.WriteTimeout = 5000;

            _comPort.DataReceived += OnComPortDataReceived;
            _comPort.ErrorReceived += OnComPortErrorReceived;

            _reconnectTimer.Elapsed += OnReconnectTimerOnElapsed;
            _reconnectTimer.Interval = 3000;
            _reconnectTimer.AutoReset = true;
            _reconnectTimer.Enabled = true;
            _reconnectTimer.Stop();

            if (autoConnect)
            {
                _reconnectTimer.Start();
            }

            SerialMessages(this, new SerialMessageEventArgs(DeviceName, $"Serial-Comport '{DeviceName}' is created successfully", MessageLevel.Success, null));
        }
        catch (Exception ex)
        {
            SerialMessages(this, new SerialMessageEventArgs(DeviceName, $"Serial-Comport '{DeviceName}' failed to create", MessageLevel.Error, ex));
        }
    }

    public void Connect()
    {
        try
        {
            _comPort.Open();

            DeviceStatus(this, new DeviceStatusEventArgs(DeviceName, true));
            SerialMessages(this, new SerialMessageEventArgs(DeviceName, $"Serial-Comport '{DeviceName}' is open successfully", MessageLevel.Success, null));
        }
        catch (Exception ex)
        {
            SerialMessages(this, new SerialMessageEventArgs(DeviceName, $"Serial-Comport '{DeviceName}' failed to open", MessageLevel.Error, ex));
        }
    }

    public void Disconnect()
    {
        var closeThread = new Thread(CloseByThread);
        closeThread.Start();
    }

    private void CloseByThread()
    {
        try
        {
            _comPort.Close();

            DeviceStatus(this, new DeviceStatusEventArgs(DeviceName, false));
            SerialMessages(this, new SerialMessageEventArgs(DeviceName, $"Serial-Comport '{DeviceName}' is closed successfully", MessageLevel.Success, null));
        }
        catch (Exception ex)
        {
            SerialMessages(this, new SerialMessageEventArgs(DeviceName, $"Serial-Comport '{DeviceName}' failed to close", MessageLevel.Error, ex));
        }
    }

    public void TransmitData(string text)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            var bytes = Encoding.GetBytes(text);

            _comPort.Write(bytes, 0, bytes.Length);

            TransmittedData(this, new SerialDataEventArgs(DeviceName, text, bytes));
            return;
        }
        catch (Exception ex)
        {
            SerialMessages(this, new SerialMessageEventArgs(DeviceName, $"Serial-Comport '{DeviceName}' failed to transmit data (as text)", MessageLevel.Error, ex));
        }
    }

    public void TransmitData(byte[] bytes)
    {
        try
        {
            if (bytes.Length <= 0) return;
            var text = Encoding.GetString(bytes);

            _comPort.Write(bytes, 0, bytes.Length);

            TransmittedData(this, new SerialDataEventArgs(DeviceName, text, bytes));
            return;
        }
        catch (Exception ex)
        {
            SerialMessages(this, new SerialMessageEventArgs(DeviceName, $"Serial-Comport '{DeviceName}' failed to transmit data (as bytes)", MessageLevel.Error, ex));
        }
    }

    public bool IsConnected()
    {
        return _comPort.IsOpen;
    }

    private void OnReconnectTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        _reconnectTimer.Stop();

        Connect();
    }

    private void OnComPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {
        SerialMessages(this, new SerialMessageEventArgs(DeviceName, $"Serial-Comport '{DeviceName}' failed to receive data ({e.EventType})", MessageLevel.Error, null));
    }

    private void OnComPortDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            if (_comPort.BytesToRead == 0)
            {
                return;
            }

            var byteCount = _comPort.BytesToRead;
            var receivedBytes = new byte[byteCount];
            _comPort.Read(receivedBytes, 0, byteCount);

            var text = Encoding.GetString(receivedBytes, 0, byteCount);

            ReceivedData(this, new SerialDataEventArgs(DeviceName, text, receivedBytes));
        }
        catch (Exception ex)
        {
            SerialMessages(this, new SerialMessageEventArgs(DeviceName, $"Serial-Comport '{DeviceName}' failed to receive data", MessageLevel.Error, ex));
        }
    }
}

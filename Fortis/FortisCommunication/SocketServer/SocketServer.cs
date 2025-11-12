using FortisCommunication.SocketClient;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using static System.Net.Mime.MediaTypeNames;

namespace FortisCommunication.SocketServer;

public class SocketServer
{
    public string DeviceName { get; }
    public string Description { get; }
    public Encoding Encoding { get; }
    public IPEndPoint IpEndPoint { get; } = null!;

    public event EventHandler<SocketServerDataEventArgs> ReceivedData = delegate { };
    public event EventHandler<SocketServerDataEventArgs> TransmittedData = delegate { };
    public event EventHandler<SocketServerMessageEventArgs> SocketMessages = delegate { };
    public event EventHandler<DeviceStatusEventArgs> DeviceStatus = delegate { };

    private readonly System.Timers.Timer _reconnectTimer;

    private Socket? _socketServer = null!;
    private Socket? _socketHandler = null!;
    private NetworkStream? _networkStream = null!;
    private Thread _receiveDataThread = null!;
    private object _handleMutex = null!;
    private bool _closeConnection = true;

    public SocketServer(string deviceName, string description, Encoding encoding, int ipPort, bool autoConnect)
    {
        DeviceName = deviceName;
        Description = description;
        Encoding = encoding;

        _reconnectTimer = new System.Timers.Timer();

        try
        {
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ipAddress in ipHostInfo.AddressList.Where(ipAddress => ipAddress.AddressFamily.ToString() == "InterNetwork"))
            {
                IpEndPoint = new IPEndPoint(ipAddress, ipPort);
                break;
            }

            _reconnectTimer.Elapsed += OnReconnectTimerOnElapsed;
            _reconnectTimer.Interval = 3000;
            _reconnectTimer.AutoReset = true;
            _reconnectTimer.Enabled = true;
            _reconnectTimer.Stop();

            if (autoConnect)
            {
                _reconnectTimer.Start();
            }

            SocketMessages(this, new SocketServerMessageEventArgs(DeviceName, $"Socket-server '{DeviceName}' is created successfully", MessageLevel.Error, null));
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketServerMessageEventArgs(DeviceName, $"Socket-server '{DeviceName}' failed to create", MessageLevel.Error, ex));
        }
    }

    public void Connect()
    {
        _reconnectTimer.Stop();

        _closeConnection = false;

        PrepareReceiveDataThread();
        _receiveDataThread.Start();

        PrepareSocketServerListen();
    }

    public void Disconnect()
    {
        CloseConnection(false);
    }

    private void PrepareSocketServerListen()
    {
        if (_socketServer is null)
        {
            _socketServer = new Socket(IpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socketServer.Bind(IpEndPoint);
            _socketServer.Listen(100);

            DeviceStatus(this, new DeviceStatusEventArgs(DeviceName, true));
        }

        while (!_closeConnection)
        {
            _socketServer?.Listen(1);
            var handle = _socketServer?.Accept();
            lock (_handleMutex)
            {
                _socketHandler?.Dispose();
                _networkStream?.Dispose();
                _socketHandler = handle;
                if (_socketHandler is not null)
                {
                    _networkStream = new NetworkStream(_socketHandler);
                }
            }

            Thread.Sleep(500);
        }
    }

    private void PrepareReceiveDataThread()
    {
        _handleMutex = new object();

        _receiveDataThread = new Thread(() =>
        {
            while (!_closeConnection)
            {
                lock (_handleMutex)
                {
                    while (_networkStream?.DataAvailable ?? false)
                    {
                        var buffer = new byte[1024];
                        var length = _networkStream?.Read(buffer, 0, buffer.Length);
                        if (length is not null) OnDataReceive(buffer, (int)length);
                    }
                }

                Thread.Sleep(333);
            }
        });
    }

    private void OnDataReceive(byte[] data, int dataLength)
    {
        try
        {
            if (dataLength == 0 || data is null || data.Length == 0) return;

            var receivedBytes = new byte[dataLength];
            for (var i = 0; i < dataLength; i++)
            {
                receivedBytes[i] = data[i];
            }

            var text = Encoding.GetString(receivedBytes, 0, dataLength);

            ReceivedData(this, new SocketServerDataEventArgs(DeviceName, text, receivedBytes));
            return;
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketServerMessageEventArgs(DeviceName, $"Socket-server '{DeviceName}' failed to receive data", MessageLevel.Error, ex));
        }

        CloseConnection(true);
    }

    public void TransmitData(string text)
    {
        try
        {
            lock (_handleMutex)
            {
                var bytes = Encoding.GetBytes(text);
                _socketHandler?.Send(bytes);

                TransmittedData(this, new SocketServerDataEventArgs(DeviceName, text, bytes));
            }
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketServerMessageEventArgs(DeviceName, $"Socket-server '{DeviceName}' failed to transmit data (as text)", MessageLevel.Error, ex));
        }
    }

    public void TransmitData(byte[] bytes)
    {
        try
        {
            lock (_handleMutex)
            {
                _socketHandler?.Send(bytes);
                var text = Encoding.GetString(bytes);

                TransmittedData(this, new SocketServerDataEventArgs(DeviceName, text, bytes));
            }
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketServerMessageEventArgs(DeviceName, $"Socket-server '{DeviceName}' failed to transmit data (as bytes)", MessageLevel.Error, ex));
        }
    }

    private void CloseConnection(bool reconnect)
    {
        try
        {
            _reconnectTimer.Stop();
            _closeConnection = true;

            _socketHandler?.Dispose();
            _socketHandler = null;
            _networkStream?.Dispose();
            _networkStream = null;
            _socketServer?.Dispose();
            _socketServer = null;

            if (reconnect)
            {
                _reconnectTimer.Start();
            }

            DeviceStatus(this, new DeviceStatusEventArgs(DeviceName, false));
            SocketMessages(this, new SocketServerMessageEventArgs(DeviceName, $"Socket-server '{DeviceName}' is closed successfully", MessageLevel.Info, null));
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketServerMessageEventArgs(DeviceName, $"Socket-server '{DeviceName}' failed to close", MessageLevel.Error, ex));
        }
    }

    public bool IsConnected()
    {
        if (_socketServer is null) return false;
        return _socketServer.Connected;
    }

    public bool IsReplying()
    {
        try
        {
            var result = false;
            if (_socketServer is not null)
            {
                var part1 = _socketServer.Poll(1000, SelectMode.SelectRead);
                var part2 = (_socketServer.Available == 0);
                if (part1 & part2) result = false;
                else result = true;
            }
            return result;
        }
        catch
        {
            return false;
        }
    }

    private void OnReconnectTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        Connect();
    }
}

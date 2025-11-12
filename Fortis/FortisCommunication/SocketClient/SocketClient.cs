using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using System.Timers;

namespace FortisCommunication.SocketClient;

public class SocketClient
{
    public string DeviceName { get; }
    public string Description { get; }
    public Encoding Encoding { get; }
    public IPEndPoint IpEndPoint { get; } = null!;

    public event EventHandler<SocketClientDataEventArgs> ReceivedData = delegate { };
    public event EventHandler<SocketClientDataEventArgs> TransmittedData = delegate { };
    public event EventHandler<SocketClientMessageEventArgs> SocketMessages = delegate { };
    public event EventHandler<DeviceStatusEventArgs> DeviceStatus = delegate { };

    private readonly Socket _socketClient = null!;
    private readonly System.Timers.Timer _reconnectTimer;

    public SocketClient(string deviceName, string description, Encoding encoding, IPAddress ipAddress, int ipPort, bool autoConnect)
    {
        DeviceName = deviceName;
        Description = description;
        Encoding = encoding;

        _reconnectTimer = new System.Timers.Timer();

        try
        {
            IpEndPoint = new IPEndPoint(ipAddress, ipPort);

            _socketClient = new Socket(IpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _reconnectTimer.Elapsed += OnReconnectTimerOnElapsed;
            _reconnectTimer.Interval = 3000;
            _reconnectTimer.AutoReset = true;
            _reconnectTimer.Enabled = true;
            _reconnectTimer.Stop();

            if (autoConnect)
            {
                _reconnectTimer.Start();
            }

            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' is created successfully", MessageLevel.Error, null));
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' failed to create", MessageLevel.Error, ex));
        }
    }

    public void Connect()
    {
        _reconnectTimer.Stop();

        try
        {
            if (_socketClient is null) return;

            _socketClient.BeginConnect(IpEndPoint, new AsyncCallback(ConnectCallback), _socketClient);
            return;
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' failed to begin connect", MessageLevel.Error, ex));
        }

        CloseConnection(true);
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            var client = (Socket)ar.AsyncState!;
            client.EndConnect(ar);

            DeviceStatus(this, new DeviceStatusEventArgs(DeviceName, true));
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' is connected successfully", MessageLevel.Success, null));

            BeginListening(_socketClient);
            return;
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' failed to connect", MessageLevel.Error, ex));
        }

        CloseConnection(true);
    }

    private void BeginListening(Socket client)
    {
        try
        {
            var stateObject = new SocketClientStateObject
            {
                WorkSocket = _socketClient
            };

            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' begin listening", MessageLevel.Info, null));

            client.BeginReceive(stateObject.Buffer, 0, SocketClientStateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), stateObject);
            return;
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' failed to begin listening", MessageLevel.Error, ex));
        }

        CloseConnection(true);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            var state = (SocketClientStateObject)ar.AsyncState!;
            var workSocket = state.WorkSocket;

            if (workSocket == null) return;

            var bytesRead = workSocket.EndReceive(ar);
            if (bytesRead <= 0) return;

            var text = Encoding.GetString(state.Buffer, 0, bytesRead);
            var bytes = state.Buffer;
            ReceivedData(this, new SocketClientDataEventArgs(DeviceName, text, bytes));

            workSocket.BeginReceive(state.Buffer, 0, SocketClientStateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            return;
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' failed to receive data", MessageLevel.Error, ex));
        }

        CloseConnection(true);
    }

    public void TransmitData(string text)
    {
        try
        {
            if (_socketClient is null) return;
            if (string.IsNullOrWhiteSpace(text)) return;

            var bytes = Encoding.GetBytes(text);

            _socketClient.BeginSend(bytes, 0, bytes.Length, 0, new AsyncCallback(TransmitCallback), _socketClient);
            
            TransmittedData(this, new SocketClientDataEventArgs(DeviceName, text, bytes));
            return;
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' failed to transmit data (as text)", MessageLevel.Error, ex));
        }

        CloseConnection(true);
    }

    public void TransmitData(byte[] bytes)
    {
        try
        {
            if (_socketClient is null) return;
            if (bytes.Length <= 0) return;

            var text = Encoding.GetString(bytes);

            _socketClient.BeginSend(bytes, 0, bytes.Length, 0, new AsyncCallback(TransmitCallback), _socketClient);
            
            TransmittedData(this, new SocketClientDataEventArgs(DeviceName, text, bytes));
            return;
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' failed to transmit data (as bytes)", MessageLevel.Error, ex));
        }

        CloseConnection(true);
    }

    private void TransmitCallback(IAsyncResult ar)
    {
        try
        {
            var client = (Socket)ar.AsyncState!;
            client.EndSend(ar);
            return;
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' failed to transmit data", MessageLevel.Error, ex));
        }

        CloseConnection(true);
    }

    public void Disconnect()
    {
        try
        {
            _reconnectTimer.Stop();

            if (_socketClient is null) return;

            _socketClient.BeginDisconnect(true, new AsyncCallback(DisconnectCallback), _socketClient);
            return;
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' failed to begin disconnect", MessageLevel.Error, ex));
        }
    }

    private void DisconnectCallback(IAsyncResult ar)
    {
        try
        {
            var client = (Socket)ar.AsyncState!;
            client.EndDisconnect(ar);

            DeviceStatus(this, new DeviceStatusEventArgs(DeviceName, false));
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' is disconnected successfully", MessageLevel.Success, null));
            return;
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' failed to disconnect", MessageLevel.Error, ex));
        }

        CloseConnection(false);
    }

    private void CloseConnection(bool autoReconnect)
    {
        try
        {
            _socketClient.Disconnect(true);

            DeviceStatus(this, new DeviceStatusEventArgs(DeviceName, false));
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' is closed successfully", MessageLevel.Success, null));
        }
        catch (Exception ex)
        {
            SocketMessages(this, new SocketClientMessageEventArgs(DeviceName, $"Socket-client '{DeviceName}' failed to close", MessageLevel.Error, ex));
        }

        if (autoReconnect)
        {
            _reconnectTimer.Start();
        }
    }

    public bool IsConnected()
    {
        if (_socketClient is null) return false;
        return _socketClient.Connected;
    }

    public bool IsReplying()
    {
        try
        {
            var ping = new Ping();
            var pingReply = ping.Send(IpEndPoint.Address);
            if (pingReply is not null)
            {
                return pingReply.Status == IPStatus.Success;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private void OnReconnectTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        Connect();
    }
}

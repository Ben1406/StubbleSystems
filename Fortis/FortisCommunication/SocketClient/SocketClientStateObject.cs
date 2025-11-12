using System.Net.Sockets;

namespace FortisCommunication.SocketClient;

public class SocketClientStateObject
{
    public Socket? WorkSocket = null;
    public const int BufferSize = 2048;
    public byte[] Buffer = new byte[BufferSize];
}

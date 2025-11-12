namespace FortisCommunication.SocketClient;

public class SocketClientDataEventArgs(string deviceName, string text, byte[] bytes) : EventArgs
{
    public string DeviceName { get; } = deviceName;
    public string Text { get; } = text;
    public byte[] Bytes { get; } = bytes;
}

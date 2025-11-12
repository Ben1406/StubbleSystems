namespace FortisCommunication.SocketServer;

public class SocketServerMessageEventArgs(string deviceName, string text, MessageLevel messageLevel, Exception? exception) : EventArgs
{
    public string DeviceName { get; } = deviceName;
    public string Text { get; } = text;
    public MessageLevel MessageLevel { get; } = messageLevel;
    public Exception? Exception { get; } = exception;
}

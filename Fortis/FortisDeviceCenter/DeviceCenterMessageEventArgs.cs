using FortisCommunication;

namespace FortisDeviceCenter;

public class DeviceCenterMessageEventArgs(string deviceName, short? clientDeviceId, string text, MessageLevel messageLevel, Exception? exception) : EventArgs
{
    public string DeviceName { get; } = deviceName;
    public short? ClientDeviceId { get; } = clientDeviceId;
    public string Text { get; } = text;
    public MessageLevel MessageLevel { get; } = messageLevel;
    public Exception? Exception { get; } = exception;
}

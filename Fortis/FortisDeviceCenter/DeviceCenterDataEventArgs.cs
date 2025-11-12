namespace FortisDeviceCenter;

public class DeviceCenterDataEventArgs(string deviceName, short? clientDeviceId, string text, byte[] bytes) : EventArgs
{
    public string DeviceName { get; } = deviceName;
    public short? ClientDeviceId { get; } = clientDeviceId;
    public string Text { get; } = text;
    public byte[] Bytes { get; } = bytes;
}

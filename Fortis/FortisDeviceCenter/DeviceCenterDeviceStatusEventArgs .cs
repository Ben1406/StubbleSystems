namespace FortisDeviceCenter;

public class DeviceCenterDeviceStatusEventArgs(string deviceName, short? clientDeviceId, bool connected) : EventArgs
{
    public string DeviceName { get; } = deviceName;
    public short? ClientDeviceId { get; } = clientDeviceId;
    public bool Connected { get; } = connected;
}

namespace FortisCommunication;

public class DeviceStatusEventArgs(string deviceName, bool connected) : EventArgs
{
    public string DeviceName { get; } = deviceName;
    public bool Connected { get; } = connected;
}

namespace FortisDeviceCenter.Scale;

public class ScaleWeightResultEventArgs(string deviceName, short? clientDeviceId, ScaleWeightResult scaleWeightResult) : EventArgs
{
    public string DeviceName { get; } = deviceName;
    public short? ClientDeviceId { get; } = clientDeviceId;
    public ScaleWeightResult ScaleWeightResult { get; } = scaleWeightResult;
}

namespace FortisDeviceCenter.BarcodeScanner;

public class BarcodeScannerResultEventArgs(string deviceName, short? clientDeviceId, BarcodeScannerResult barcodeScannerResult) : EventArgs
{
    public string DeviceName { get; } = deviceName;
    public short? ClientDeviceId { get; } = clientDeviceId;
    public BarcodeScannerResult BarcodeScannerResult { get; } = barcodeScannerResult;
}

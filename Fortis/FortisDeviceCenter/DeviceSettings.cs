using System.Net;
using System.Text;
using FortisCommunication.Serial;
using FortisDeviceCenter.BarcodeScanner;
using FortisDeviceCenter.LabelPrinter;
using FortisDeviceCenter.Scale;

namespace FortisDeviceCenter;

public class DeviceSettings
{
    public struct SocketServerSettings
    {
        public Encoding Encoding;
        public short IpPort;
    }

    public struct SocketClientSettings
    {
        public Encoding Encoding;
        public IPAddress IpAddress;
        public short IpPort;
    }

    public struct SerialComportSettings
    {
        public Encoding Encoding;
        public SerialPortName PortName;
        public SerialBaudRate BaudRate;
        public SerialParity Parity;
        public SerialDataBit DataBit;
        public SerialStopBit StopBit;
        public int ReadBufferSize;
        public int WriteBufferSize;
    }

    public struct ScaleSettings
    {
        public ScaleType Type;
        public ScaleProtocol Protocol;
        public bool AllowTareFromIndicator;
    }

    public struct LabelPrinterSettings
    {
        public LabelPrinterType Type;
        public LabelPrinterProtocol Protocol;
        public LabelPrinterDpi Dpi;
        public LabelPrinterRotate Rotate;

        public LabelPrinterPrintMode? PrintMode;
        public LabelPrinterMediaType? MediaType;
        public LabelPrinterPaperType? PaperType;
        public short? PrintSpeed;
        public short? LabelLength;
        public short? LabelWidth;
        public short? StartAdjust;
        public short? StopAdjust;
        public short? LeftMargin;
        public bool? PrintButtonCopy;
        public string? ProgramBeforePrint;
        public string? ProgramAfterPrint;
    }

    public struct BarcodeScannerSettings
    {
        public BarcodeScannerType Type;
        public BarcodeScannerProtocol Protocol;

        public bool? SendFeedbackToHost;
    }
}

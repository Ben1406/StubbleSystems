using System.ComponentModel;

namespace FortisDeviceCenter;

public enum DeviceType
{
    [Description("None")]
    None,

    [Description("Scale")]
    Scale,

    [Description("Label Printer")]
    LabelPrinter,

    [Description("Barcode Scanner")]
    BarcodeScanner,

    [Description("Digital I/O Relay")]
    DigitalIoRelay,

    [Description("Rfid Reader")]
    RfidReader,

    [Description("Typeless")]
    Typeless
}

public enum DeviceCommunication
{
    [Description("None")]
    None,

    [Description("Socket Server")]
    SocketServer,

    [Description("Socket Client")]
    SocketClient,

    [Description("Serial Comport")]
    SerialComport
}

public enum DecodeState
{
    Partial,
    Success,
    Fail
}

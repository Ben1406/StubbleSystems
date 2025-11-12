using System.ComponentModel;

namespace FortisDeviceCenter.LabelPrinter;

public enum LabelPrinterType
{
    [Description("Intermec")]
    Intermec,

    [Description("Honeywell")]
    Honeywell,

    [Description("Zebra")]
    Zebra,

    [Description("Zebra Compatible")]
    ZebraCompatible
}

public enum LabelPrinterProtocol
{
    [Description("Direct Protocol")]
    DirectProtocol,

    [Description("ZPL2")]
    Zpl2
}

public enum LabelPrinterDpi
{
    [Description("203")]
    Dpi203 = 203,

    [Description("300")]
    Dpi300 = 300,

    [Description("600")]
    Dpi600 = 600
}

public enum LabelPrinterRotate
{
    [Description("Rotate 0°")]
    Rotate0 = 0,

    [Description("Rotate 90°")]
    Rotate90 = 90,

    [Description("Rotate 180°")]
    Rotate180 = 180,

    [Description("Rotate 270°")]
    Rotate270 = 270
}

public enum LabelPrinterPrintMode
{
    [Description("Tear Off (without roll-up)")]
    TearOff,

    [Description("Peel Off (with roll-up)")]
    PeelOff,

    [Description("Rewind")]
    Rewind
}

public enum LabelPrinterMediaType
{
    [Description("Label With Gap")]
    LabelWithGaps,

    [Description("Ticket With Mark")]
    TicketWithMark,

    [Description("Continous")]
    Continuous,

    [Description("Variable")]
    Variable,

    [Description("Fixed Length")]
    FixedLength
}

public enum LabelPrinterPaperType
{
    [Description("Direct Thermal")]
    DirectThermal,

    [Description("Thermal Transfer Ribbon")]
    ThermalTransferRibbon
}

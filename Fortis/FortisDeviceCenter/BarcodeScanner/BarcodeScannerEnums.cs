using System.ComponentModel;

namespace FortisDeviceCenter.BarcodeScanner;

public enum BarcodeScannerType
{
    [Description("Intermec")]
    Intermec,

    [Description("Honeywell")]
    Honeywell,

    [Description("Honeywell With FeedBack")]
    HoneywellWithFeedBack,

    [Description("DataLogic")]
    DataLogic,

    [Description("Other")]
    Other
}

public enum BarcodeScannerProtocol
{
    [Description("<Stx>...<Etx>")]
    StxEtx,

    [Description("...<CrLf>")]
    CrLf,

    [Description("...<Cr>")]
    Cr,

    [Description("...<Lf>")]
    Lf,

    [Description("None")]
    None
}

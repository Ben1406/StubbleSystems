using System.ComponentModel;

namespace FortisDeviceCenter.Scale;

public enum ScaleType
{
    [Description("Scanvaegt")]
    Scanvaegt,

    [Description("Scanvaegt SV10")]
    ScanvaegtSv10,

    [Description("Scanvaegt SV11")]
    ScanvaegtSv11,

    [Description("Marel")]
    Marel,

    [Description("Marel M1100")]
    MarelM1100,

    [Description("Marel M2200")]
    MarelM2200,

    [Description("SysTek")]
    SysTek,

    [Description("SysTek ITx000 (like IT6000 or IT8000)")]
    SysTekITx000,

    [Description("Mettler Toledo")]
    MettlerToledo
}

public enum ScaleProtocol
{
    [Description("Scanvaegt > CSO (continuously)")]
    ScanvaegtContinuousSerialOutput,

    [Description("Scanvaegt > SV3/COM3 (one-shot)")]
    ScanvaegtCommunicationThree,

    [Description("Marel > M1100")]
    MarelM1100,

    [Description("Marel > M2200")]
    MarelM2200,

    [Description("Marel > Port 52253 (continuously)")]
    MarelPort52253,

    [Description("SysTek > Customized Protocol (one-shot)")]
    SysTekCustomizedProtocol,

    [Description("SysTek > Extended Standard Protocol (one-shot)")]
    SysTekExtentedStandardProtocol,

    [Description("Toledo (continously)")]
    Toledo
}

public enum WeightType
{
    Gross,
    Net
}

public enum WeightUnit
{
    Ton,
    Kilogram,
    Gram,
    Pound,
    Ounce
}

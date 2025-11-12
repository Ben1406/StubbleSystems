namespace FortisDeviceCenter.BarcodeScanner.Decoders;

public static class BarcodeWithSuffix
{
    public static (BarcodeScannerResult?, DecodeState) Decode(string data, char[] suffix)
    {
        if (string.IsNullOrEmpty(data) || data.Length == 0)
        {
            return (null, DecodeState.Fail);
        }

        if (!data.EndsWith(Helper.CharsAsString(suffix)))
        {
            return (null, DecodeState.Fail);
        }

        var suffixOffset = data.LastIndexOf(Helper.CharsAsString(suffix));

        var barcode = data.Substring(0, suffixOffset);
        var length = barcode.Length;

        var scaleWeightResult = new BarcodeScannerResult
        {
            Barcode = barcode,
            Length = length
        };

        return (scaleWeightResult, DecodeState.Success);
    }
}

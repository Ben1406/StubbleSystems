namespace FortisDeviceCenter.BarcodeScanner.Decoders;

public static class BarcodeWithPrefixSuffix
{
    public static (BarcodeScannerResult?, DecodeState) Decode(string data, char[] prefix, char[] suffix)
    {
        if (string.IsNullOrEmpty(data) || data.Length == 0)
        {
            return (null, DecodeState.Fail);
        }

        if (data.Length < 60 || !data.Contains(Helper.CharsAsString(prefix)))
        {
            return (null, DecodeState.Partial);
        }

        if (!data.EndsWith(Helper.CharsAsString(suffix)))
        {
            return (null, DecodeState.Partial);
        }

        var prefixOffset = data.LastIndexOf(Helper.CharsAsString(prefix));
        var suffixOffset = data.LastIndexOf(Helper.CharsAsString(suffix));

        if (prefixOffset >= suffixOffset)
        {
            return (null, DecodeState.Fail);
        }

        var barcode = data.Substring(prefixOffset, suffixOffset - prefixOffset + 1);
        var length = barcode.Length;

        var scaleWeightResult = new BarcodeScannerResult
        {
            Barcode = barcode,
            Length = length
        };

        return (scaleWeightResult, DecodeState.Success);
    }
}

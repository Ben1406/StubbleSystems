namespace FortisDeviceCenter.Scale.Decoders;

public static class SysTekExtendedStandardProtocol
{
    // SYSTEK EXTENDED STANDARD PROTOCOL (for IT6000, IT8000 or similar)
    //
    // Offset         0       2      3           4       5     6       7       16     17    19   
    //               <HEADER><RANGE><WEIGHTTYPE><MOTION><ZERO><SIGNAL><WEIGHT><SPACE><UNIT><CRLF>
    // 21 bytes:      2       1      1           1       1     1       9       1      2     2    
    //
    // <HEADER>      "XW" (always X and always W for Weigh-data-string)
    // <RANGE>       Weighing-range 1, 2 etc. or space-character for single-range scale
    // <WEIGHTTYPE>  N for net-weight or G for gross-weight
    // <MOTION>      M for motion or S for scale settled
    // <ZERO>        Z when scale is in zero-range, otherwise space-character
    // <SIGNAL>      S for traffic-light function
    // <WEIGHT>      Weight, 9 characters, right justified, with preceding minus sign and decimal separator
    // <SPACE>       fixed space-character
    // <UNIT>        Weight unit 't', 'kg', 'g' or 'lb', 2 characters, left justified
    // <CRLF>        (char)13 + (char)10
    public static (ScaleWeightResult?, DecodeState) Decode(string data)
    {
        if (string.IsNullOrEmpty(data) || !data.Contains("XW"))
        {
            return (null, DecodeState.Fail);
        }

        data = data.Replace(Helper.Dle.ToString(), "");

        if (data.Length < 21 || !data.Contains(Helper.Cr) || !data.Contains(Helper.Lf))
        {
            return (null, DecodeState.Partial);
        }

        var xwOffset = data.IndexOf("XW", StringComparison.Ordinal);
        var crOffset = data.IndexOf(Helper.Cr);

        if (crOffset >= xwOffset)
        {
            return (null, DecodeState.Fail);
        }

        // TODO: 

        return (null, DecodeState.Fail);
    }
}

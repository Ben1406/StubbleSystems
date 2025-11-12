namespace FortisDeviceCenter.Scale.Decoders;

public static class ScanvaegtCommunicationThree
{
    // SCANVAEGT COM3/SV3
    //
    // Offset         0    1       4        7           8            9         10        12    13      23    24        25    31         33       45     56      58
    //               <STX><HEADER><STATION><RECORDTYPE><MEASUREMENT><DECIMALS><DIVISION><SIGN><WEIGHT><UNIT><TARETYPE><TARE><INDICATOR><PROGRAM><ALIBI><CHKSUM><ETX>
    // 60 bytes:      1    3       3        1           1            1         2         1     10      1     1         6     2          12       11     2       1
    //
    // <STX>         (char)2
    // <HEADER>      "CW1" (always CW1)
    // <STATION>     station number
    // <RECORDTYPE>  0=single, 1=summation
    // <MEASUREMENT> 0=exact, 1=approx
    // <DECIMALS>    0=no decimals, 1=one decimals, 2=two decimals, 3=three decimals etc.
    // <DIVISION>    01=1, 02=2, 03=5, 04=10, 05=20
    // <SIGN>        "-"=negative, space=positive
    // <WEIGHT>      weight value (without delimiter)
    // <UNIT>        t=ton, k=kilogram, g=gram, l=pound, o=ounce
    // <TARETYPE>    0=no tare, 1=semi/auto, 2=preset, 3=calculated
    // <TARE>        tare value (without delimiter)
    // <INDICATOR>   00=no type, xx=type number
    // <PROGRAM>     indicator weighing program code
    // <ALIBI>       alibi number
    // <CHKSUM>      calculated checksum
    // <ETX>         (char)3
    private static long _lastScvCom3Alibi = 0;
    public static (ScaleWeightResult?, DecodeState) Decode(string data)
    {
        if (string.IsNullOrEmpty(data) || !data.Contains(Helper.Stx))
        {
            return (null, DecodeState.Fail);
        }

        if (data.Length < 60 || !data.Contains(Helper.Etx))
        {
            return (null, DecodeState.Partial);
        }

        var stxOffset = data.IndexOf(Helper.Stx);
        var etxOffset = data.IndexOf(Helper.Etx);

        if (stxOffset >= etxOffset || etxOffset - stxOffset + 1 != 60)
        {
            return (null, DecodeState.Fail);
        }

        var partialData = data.Substring(stxOffset, etxOffset - stxOffset + 1);

        if (!partialData.StartsWith($"{Helper.Stx}CW1"))
        {
            return (null, DecodeState.Fail);
        }

        try
        {
            var decimals = Convert.ToInt16(data.Substring(9, 1));
            var weight = Convert.ToDecimal(Convert.ToInt32(data.Substring(13, 10)));
            var tare = Convert.ToDecimal(Convert.ToInt32(data.Substring(25, 6)));
            var weightType = tare > 0 ? WeightType.Net : WeightType.Gross;
            var alibi = Convert.ToInt64(data.Substring(45, 11));

            var weightUnit = WeightUnit.Kilogram;
            var unit = data.Substring(23, 1);
            switch (unit)
            {
                case "t":
                    weightUnit = WeightUnit.Ton;
                    break;
                case "k":
                    weightUnit = WeightUnit.Kilogram;
                    break;
                case "g":
                    weightUnit = WeightUnit.Gram;
                    break;
                case "l":
                    weightUnit = WeightUnit.Pound;
                    break;
                case "o":
                    weightUnit = WeightUnit.Ounce;
                    break;
            }

            var pow = (decimal)Math.Pow(10, decimals);
            weight /= pow;
            tare /= pow;

            if (data.Substring(12, 1) == "-")
            {
                weight *= -1;
            }

            var scaleWeightResult = new ScaleWeightResult
            {
                Weight = weight,
                TareWeight = tare,
                Decimals = decimals,
                Motion = false,
                Registration = true,
                SwingLoad = false,
                WeightType = weightType,
                WeightUnit = weightUnit,
                Alibi = alibi
            };

            if (_lastScvCom3Alibi == alibi)
            {
                return (null, DecodeState.Fail);
            }

            _lastScvCom3Alibi = alibi;

            return (scaleWeightResult, DecodeState.Success);
        }
        catch
        {
            // ignored
        }

        return (null, DecodeState.Fail);
    }
}

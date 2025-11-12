namespace FortisDeviceCenter.Scale.Decoders;

public static class ScanvaegtContinuousSerialOutput
{
    // SCANVAEGT CSO
    //
    // Offset         0    1    2    3       8           9     10    15      16
    //               <STX><ID1><ID2><WEIGHT><MULTIPLIER><SIGN><TARE><CHKSUM><ETX>
    // 18 bytes:      1    1    1    5       1           1     5     2       1
    //
    // <STX>         (char)2
    // <ID1>         bit0 = weight unit (0=kg, 1=lb)
    //               bit1 = swing load (0=no, 1=yes)
    //               bit2 = always 1
    //               bit3 = weight type (0=gross, 1=net)
    //               bit4 = info (0=same, 1=new)
    //               bit5 = no motion (0=yes, 1=no)
    //               bit6 = invalid (0=yes, 1=no)
    //               bit7 = registration (0=no, 1=yes)
    // <ID2>         bit pattern (0-7) 100XX110 > Division = 1
    //               bit pattern (0-7) 010XX110 > Division = 2
    //               bit pattern (0-7) 101XX110 > Division = 5
    //               bit pattern (0-7) 110XX110 > Division = 10
    //               bit pattern (0-7) 001XX110 > Division = 20
    //               bit pattern (0-7) XXX0X110 > Tare or Gross Mode
    //               bit pattern (0-7) XXX1X110 > Semi Auto Tare Mode
    //               bit pattern (0-7) XXXX0110 > Outside Centre of Zero
    //               bit pattern (0-7) XXXX1110 > Inside Centre of Zero
    // <WEIGHT>      weight value (no delimiter)
    // <MULTIPLIER>  0=1g (3 dec), 1=10g (2 dec), 2=100g (1 dec), 3=1000g (0 dec)
    // <SIGN>        0=positive, 1=negative
    // <TARE>        tare value (no delimiter)
    // <CHKSUM>      calculated checksum
    // <ETX>         (char)3
    public static (ScaleWeightResult?, DecodeState) Decode(string data)
    {
        if (string.IsNullOrEmpty(data) || !data.Contains(Helper.Stx))
        {
            return (null, DecodeState.Fail);
        }

        if (data.Length < 18 || !data.Contains(Helper.Etx))
        {
            return (null, DecodeState.Partial);
        }

        var stxOffset = data.IndexOf(Helper.Stx);
        var etxOffset = data.IndexOf(Helper.Etx);

        if (stxOffset >= etxOffset || etxOffset - stxOffset + 1 != 18)
        {
            return (null, DecodeState.Fail);
        }

        var partialData = data.Substring(stxOffset, etxOffset - stxOffset + 1);

        try
        {
            var swingLoad = Helper.GetBit(partialData[1], 1);
            var weightUnit = Helper.GetBit(partialData[1], 0) ? WeightUnit.Pound : WeightUnit.Kilogram;
            var weightType = Helper.GetBit(partialData[1], 3) ? WeightType.Net : WeightType.Gross;
            var noMotion = Helper.GetBit(partialData[1], 5);
            var invalid = Helper.GetBit(partialData[1], 6);
            var registration = Helper.GetBit(partialData[1], 7);
            var weightUnderZero = Helper.GetBit(partialData[9], 0);
            var weight = Convert.ToDecimal(partialData.Substring(3, 5));
            var tare = Convert.ToDecimal(partialData.Substring(10, 5));

            if (invalid)
            {
                return (null, DecodeState.Fail);
            }

            short decimals = 0;
            var multiplexer = 1;
            if (partialData[8] == '0')
            {
                decimals = 3;
                multiplexer = 1000;
            }
            if (partialData[8] == '1')
            {
                decimals = 2;
                multiplexer = 100;
            }
            if (partialData[8] == '2')
            {
                decimals = 1;
                multiplexer = 10;
            }
            if (partialData[8] == '3')
            {
                decimals = 0;
                multiplexer = 1;
            }

            weight /= multiplexer;
            tare /= multiplexer;

            if (weightUnderZero)
            {
                weight *= -1;
            }

            var scaleWeightResult = new ScaleWeightResult
            {
                Weight = weight,
                TareWeight = tare,
                Decimals = decimals,
                Motion = !noMotion,
                Registration = registration,
                SwingLoad = swingLoad,
                WeightType = weightType,
                WeightUnit = weightUnit,
                Alibi = -1
            };

            return (scaleWeightResult, DecodeState.Success);
        }
        catch
        {
            // ignored
        }

        return (null, DecodeState.Fail);
    }
}

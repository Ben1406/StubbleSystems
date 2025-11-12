using System.Globalization;

namespace FortisDeviceCenter.Scale.Decoders;

public static class SysTekCustomizedProtocol
{
    // SYSTEK CUSTOMIZED PROTOCOL (for IT6000, IT8000 or similar)
    //
    // 1;  ;18.02.20;    08:03;                            6;           1;           TEST;    30.5;     1.9;   28.6;      kg;  0.00;  0.00;        ;    252;      
    // ?;  ;    date;     time;    item number of this batch;   productno;    productname;   gross;    tare;    net;    unit;     ?;     ?;       ?;  alibi;      
    // 0; 1;       2;        3;                            4;           5;              6;       7;       8;      9;      10;    11;    12;      13;     14;    15
    public static (ScaleWeightResult?, DecodeState) Decode(string data)
    {
        if (string.IsNullOrEmpty(data) || !data.Contains(Helper.Stx))
        {
            return (null, DecodeState.Fail);
        }

        data = data.Replace(Helper.Dle.ToString(), "");

        if (data.Length < 10 || !data.Contains(Helper.Etx))
        {
            return (null, DecodeState.Partial);
        }

        var stxOffset = data.IndexOf(Helper.Stx);
        var etxOffset = data.IndexOf(Helper.Etx);

        if (stxOffset >= etxOffset)
        {
            return (null, DecodeState.Fail);
        }

        var splitData = data.Split(';').Select(p => p.Trim()).ToList();

        if (splitData.Count != 16)
        {
            return (null, DecodeState.Fail);
        }

        try
        {
            var successParseTare = decimal.TryParse(splitData[8], out var outTare);
            var successParseWeight = decimal.TryParse(splitData[9], out var outWeight);
            var successParseAlibi = long.TryParse(splitData[14], out var outAlibi);

            if (!successParseWeight || !successParseTare || !successParseAlibi)
            {
                return (null, DecodeState.Fail);
            }

            var grossWeight = decimal.Parse(splitData[7], CultureInfo.InvariantCulture);
            var tare = decimal.Parse(splitData[8], CultureInfo.InvariantCulture);
            var netWeight = decimal.Parse(splitData[9], CultureInfo.InvariantCulture);
            var alibi = long.Parse(splitData[14], CultureInfo.InvariantCulture);

            var weight = tare > 0 ? netWeight : grossWeight;
            var weightType = tare > 0 ? WeightType.Net : WeightType.Gross;

            var weightUnit = WeightUnit.Kilogram;
            var unit = splitData[14];
            switch (unit)
            {
                case "t":
                    weightUnit = WeightUnit.Ton;
                    break;
                case "kg":
                    weightUnit = WeightUnit.Kilogram;
                    break;
                case "g":
                    weightUnit = WeightUnit.Gram;
                    break;
                case "lb":
                    weightUnit = WeightUnit.Pound;
                    break;
            }

            short decimals = BitConverter.GetBytes(decimal.GetBits(netWeight)[3])[2];

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

            return (scaleWeightResult, DecodeState.Success);
        }
        catch
        {
            // ignored
        }

        return (null, DecodeState.Fail);
    }
}

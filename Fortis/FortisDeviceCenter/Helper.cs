using System.Collections;

namespace FortisDeviceCenter;

internal class Helper
{
    internal static bool GetBit(char data, int bitOffSet)
    {
        var bitArray = new BitArray(new[] { Convert.ToByte(data) });
        return bitArray[bitOffSet];
    }

    internal static string CharsAsString(char char1, char? char2 = null, char? char3 = null)
    {
        var result = $"{char1}";

        if (char2 is not null) result += $"{char2}";
        if (char3 is not null) result += $"{char3}";

        return result;
    }

    internal static string CharsAsString(char[] chars)
    {
        var result = string.Empty;

        foreach (var xChar in chars)
        {
            result += $"{xChar}";
        }

        return result;
    }

    internal static char Stx = (char)2;
    internal static char Etx = (char)3;
    internal static char Dle = (char)16;
    internal static char Cr = (char)13;
    internal static char Lf = (char)13;
}

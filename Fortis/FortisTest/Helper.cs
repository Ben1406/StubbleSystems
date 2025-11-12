namespace FortisDeviceCenter;

internal class Helper
{
    internal static string ConvertBytesToStringTags(string data)
    {
        var newText = "";

        foreach (var xChar in data)
        {
            newText += xChar < 32 || xChar > 128 ? $"<{((byte)xChar)}>" : xChar;
        }

        return newText;
    }

    internal static char Stx = (char)2;
    internal static char Etx = (char)3;
    internal static char Dle = (char)16;
    internal static char Cr = (char)13;
    internal static char Lf = (char)13;
}

using System.Text;

namespace FortisDeviceCenter;

internal class Helper
{
    internal static Encoding ConvertToEncoding(string encodingName)
    {
        try
        {
            return Encoding.GetEncoding(encodingName);
        }
        catch
        {
            return Encoding.Default;
        }
    }
}

using System.Drawing;

namespace xEyedropper
{
    internal class ConvertColor
    {
        internal static string HexConverter(Color c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }

        internal static string RGBConverter(Color c)
        {
            return $"{c.R}, {c.G}, {c.B}";
        }
    }
}

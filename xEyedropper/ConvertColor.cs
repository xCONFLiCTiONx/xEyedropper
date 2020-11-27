using System.Drawing;

namespace xEyedropper
{
    internal class ConvertColor
    {
        internal static string HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        internal static string RGBConverter(Color c)
        {
            return c.R.ToString() + ", " + c.G.ToString() + ", " + c.B.ToString();
        }
    }
}

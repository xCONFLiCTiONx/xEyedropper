using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace xEyedropper
{
    internal class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);


        [DllImport("user32.dll")]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        internal static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        internal const UInt32 SWP_NOSIZE = 0x0001;
        internal const UInt32 SWP_NOMOVE = 0x0002;
        internal const UInt32 SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out Point lpPoint);
    }
}

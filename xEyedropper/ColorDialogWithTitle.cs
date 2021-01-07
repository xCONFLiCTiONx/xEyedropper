using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Windows.Forms;
using xEyedropper.Properties;

namespace xEyedropper
{
    /// <summary>
    /// The standard ColorDialog dialog box with a title property.
    /// </summary>
    internal class ColorDialogWithTitle : ColorDialog
    {
        private const int InitDialogMessage = 0x0110; // WM_INITDIALOG

        /// <summary>
        /// Initializes a new instance of the ColorDialogWithTitle class.
        /// </summary>
        internal ColorDialogWithTitle() :
            base()
        {
            this.Title = Settings.Default.ColorDialogWithTitle_DefaultTitle;

            return;
        }

        /// <summary>
        /// Gets or sets the title that will be displayed on the dialog when it's shown.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The title that will be displayed on the dialog when it's shown.")]
        internal string Title
        {
            get;
            set;
        }

        /// <summary>
        /// The hook into the dialog's WndProc that we can leverage to set the
        /// window's text.
        /// </summary>
        /// <param name="hWnd">The handle to the dialog box window.</param>
        /// <param name="msg">The message being received.</param>
        /// <param name="wparam">Additional information about the message.</param>
        /// <param name="lparam">More additional information about the message.</param>
        /// <returns>
        /// A zero value if the default dialog box procedure processes the
        /// message, a non-zero value if the default dialog box procedure 
        /// ignores the message.
        /// </returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            if (msg == InitDialogMessage)
            {
                // We'll ignore failure cases for now.  The default text isn't 
                // so bad and this isn't library code.
                NativeMethods.SetWindowText(hWnd, this.Title);
            }

            return base.HookProc(hWnd, msg, wparam, lparam);
        }
    }
}

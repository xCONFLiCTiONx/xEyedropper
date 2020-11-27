using System;
using System.Drawing;
using System.Windows.Forms;
using xEyedropper.Properties;

namespace xEyedropper
{
    static class Program
    {
        static NotifyIcon notifyIcon;
        static MenuItem menuItemHTML;
        static MenuItem menuItemRGB;
        static MenuItem menuItemEditor;
        static MenuItem menuItemGrabber;
        static MenuItem menuItemClose;

        internal static ColorDialog colorDialog1 = new ColorDialog();

        [STAThread]
        static void Main()
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
            }
            Settings.Default.Save();

            notifyIcon = new NotifyIcon
            {
                Visible = true,
                Icon = Resources.ICON,
                Text = "xEyedropper"
            };

            ContextMenu contextMenu = new ContextMenu();

            menuItemHTML = new MenuItem("&HTML");
            menuItemRGB = new MenuItem("&RGB");
            menuItemEditor = new MenuItem("&Editor");
            menuItemGrabber = new MenuItem("&Grabber");
            menuItemClose = new MenuItem("&Close");

            menuItemHTML.Click += MenuItemHTML_Click;
            menuItemRGB.Click += MenuItemRGB_Click;
            menuItemEditor.Click += Editor_Click;
            menuItemGrabber.Click += Grabber_Click;
            menuItemClose.Click += MenuItemClose_Click;

            contextMenu.MenuItems.Add(menuItemHTML);
            contextMenu.MenuItems.Add(menuItemRGB);
            contextMenu.MenuItems.Add(menuItemEditor);
            contextMenu.MenuItems.Add(menuItemGrabber);
            contextMenu.MenuItems.Add(menuItemClose);

            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.MouseClick += NotifyIcon_MouseClick;

            if (Settings.Default.ColorHTML)
            {
                menuItemHTML.Checked = true;
            }
            else if (Settings.Default.ColorRGB)
            {
                menuItemRGB.Checked = true;
            }

            Application.EnableVisualStyles();
            Application.Run();
        }

        private static void Editor_Click(object sender, EventArgs e)
        {
            try
            {
                string color = Clipboard.GetText();
                colorDialog1.FullOpen = true;
                colorDialog1.Color = ColorTranslator.FromHtml(color);
            }
            catch { /*ignore*/ }

            colorDialog1.ShowDialog();

            Color selectedColor = colorDialog1.Color;

            if (Settings.Default.ColorHTML)
            {
                Clipboard.SetText(ConvertColor.HexConverter(selectedColor));
            }
            else if (Settings.Default.ColorRGB)
            {
                Clipboard.SetText(ConvertColor.RGBConverter(selectedColor));
            }
        }

        private static void MenuItemRGB_Click(object sender, EventArgs e)
        {
            menuItemHTML.Checked = false;
            menuItemRGB.Checked = true;
            Settings.Default.ColorHTML = false;
            Settings.Default.ColorRGB = true;
            Settings.Default.Save();

            Settings.Default.Reload();
        }

        private static void MenuItemHTML_Click(object sender, EventArgs e)
        {
            menuItemRGB.Checked = false;
            menuItemHTML.Checked = true;
            Settings.Default.ColorRGB = false;
            Settings.Default.ColorHTML = true;
            Settings.Default.Save();

            Settings.Default.Reload();
        }

        private static void MenuItemClose_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Environment.Exit(0);
        }

        private static void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                new PreviewImage().Show();
            }
        }

        private static void Grabber_Click(object sender, EventArgs e)
        {
            new PreviewImage().Show();
        }
    }
}

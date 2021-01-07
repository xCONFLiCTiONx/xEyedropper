using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using xEyedropper.Properties;

namespace xEyedropper
{
    static class Program
    {
        static NotifyIcon notifyIcon;
        static MenuItem menuItemHTML, menuItemRGB, editorOptions, menuItemSaveCustomColor, menuItemResetCustomColor, menuItemEditor, menuItemGrabber, menuItemClose;

        internal static ColorDialogWithTitle colorDialog1;

        [STAThread]
        static void Main()
        {
            var assembly = typeof(Program).Assembly;
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            var id = attribute.Value;

            var mutex = new Mutex(true, id);

            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    StartApp();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
            {
                ExitApp();
            }
        }

        private static void StartApp()
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
            editorOptions = new MenuItem("&Editor Options");
            editorOptions.MenuItems.Add(menuItemSaveCustomColor = new MenuItem("&Save Custom Colors"));
            editorOptions.MenuItems.Add(menuItemResetCustomColor = new MenuItem("&Reset Custom Colors"));
            menuItemEditor = new MenuItem("&Editor");
            menuItemGrabber = new MenuItem("&Grabber");
            menuItemClose = new MenuItem("&Close");

            menuItemHTML.Click += MenuItemHTML_Click;
            menuItemRGB.Click += MenuItemRGB_Click;
            menuItemSaveCustomColor.Click += MenuItemSaveCustomColor_Click;
            menuItemResetCustomColor.Click += MenuItemResetCustomColor_Click;
            menuItemEditor.Click += Editor_Click;
            menuItemGrabber.Click += Grabber_Click;
            menuItemClose.Click += MenuItemClose_Click;

            contextMenu.MenuItems.Add(menuItemHTML);
            contextMenu.MenuItems.Add(menuItemRGB);
            contextMenu.MenuItems.Add(editorOptions);
            contextMenu.MenuItems.Add(menuItemEditor);
            contextMenu.MenuItems.Add(menuItemGrabber);
            contextMenu.MenuItems.Add(menuItemClose);

            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.MouseClick += NotifyIcon_MouseClick;

            if (Settings.Default.SaveCustomColors)
            {
                menuItemSaveCustomColor.Checked = true;
            }

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

        private static void MenuItemResetCustomColor_Click(object sender, EventArgs e)
        {
            if (menuItemResetCustomColor.Checked)
            {
                menuItemResetCustomColor.Checked = false;
            }
            else
            {
                menuItemResetCustomColor.Checked = true;
            }
        }

        private static void MenuItemSaveCustomColor_Click(object sender, EventArgs e)
        {
            if (menuItemSaveCustomColor.Checked)
            {
                menuItemSaveCustomColor.Checked = false;
                Settings.Default.SaveCustomColors = false;
            }
            else
            {
                menuItemSaveCustomColor.Checked = true;
                Settings.Default.SaveCustomColors = true;
            }
            Settings.Default.Save();
        }

        private static void ExitApp()
        {
            Application.Exit();
        }

        private static void Editor_Click(object sender, EventArgs e)
        {
            try
            {
                string color = Clipboard.GetText();
                Settings.Default.ColorDialogWithTitle_DefaultTitle = "Color in Clipboard: " + ConvertColor.RGBConverter(ColorTranslator.FromHtml(color));
                Settings.Default.Save();
                Settings.Default.Reload();
            }
            catch
            {
            }
            using (colorDialog1 = new ColorDialogWithTitle())
            {

                try
                {
                    string color = Clipboard.GetText();
                    colorDialog1.FullOpen = true;
                    colorDialog1.Color = ColorTranslator.FromHtml(color);
                }
                catch
                {
                    Settings.Default.ColorDialogWithTitle_DefaultTitle = "Color Dialog";
                }

                if (menuItemResetCustomColor.Checked)
                {
                    colorDialog1.CustomColors = new int[] { 16777215, 16777215, 16777215, 16777215, 16777215, 16777215, 16777215, 16777215, 16777215, 16777215, 16777215, 16777215, 16777215, 16777215, 16777215, 16777215 };

                    SaveCustomColors();
                }
                else
                {
                    colorDialog1.CustomColors = new int[] { Settings.Default.CustomColor1, Settings.Default.CustomColor2, Settings.Default.CustomColor3, Settings.Default.CustomColor4, Settings.Default.CustomColor5, Settings.Default.CustomColor6, Settings.Default.CustomColor7, Settings.Default.CustomColor8, Settings.Default.CustomColor9, Settings.Default.CustomColor10, Settings.Default.CustomColor11, Settings.Default.CustomColor12, Settings.Default.CustomColor13, Settings.Default.CustomColor14, Settings.Default.CustomColor15, Settings.Default.CustomColor16 };
                }

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

                if (Settings.Default.SaveCustomColors)
                {
                    SaveCustomColors();
                }
            }
        }

        private static void SaveCustomColors()
        {
            Settings.Default.CustomColor1 = colorDialog1.CustomColors[0];
            Settings.Default.CustomColor2 = colorDialog1.CustomColors[1];
            Settings.Default.CustomColor3 = colorDialog1.CustomColors[2];
            Settings.Default.CustomColor4 = colorDialog1.CustomColors[3];
            Settings.Default.CustomColor5 = colorDialog1.CustomColors[4];
            Settings.Default.CustomColor6 = colorDialog1.CustomColors[5];
            Settings.Default.CustomColor7 = colorDialog1.CustomColors[6];
            Settings.Default.CustomColor8 = colorDialog1.CustomColors[7];
            Settings.Default.CustomColor9 = colorDialog1.CustomColors[8];
            Settings.Default.CustomColor10 = colorDialog1.CustomColors[9];
            Settings.Default.CustomColor11 = colorDialog1.CustomColors[10];
            Settings.Default.CustomColor12 = colorDialog1.CustomColors[11];
            Settings.Default.CustomColor13 = colorDialog1.CustomColors[12];
            Settings.Default.CustomColor14 = colorDialog1.CustomColors[13];
            Settings.Default.CustomColor15 = colorDialog1.CustomColors[14];
            Settings.Default.CustomColor16 = colorDialog1.CustomColors[15];
            Settings.Default.Save();
        }

        private static void MenuItemRGB_Click(object sender, EventArgs e)
        {
            try
            {
                string color = Clipboard.GetText();

                Clipboard.SetText(ConvertColor.RGBConverter(ColorTranslator.FromHtml(color)));
            }
            catch (Exception)
            {
                // ignore
            }

            menuItemHTML.Checked = false;
            menuItemRGB.Checked = true;
            Settings.Default.ColorHTML = false;
            Settings.Default.ColorRGB = true;
            Settings.Default.Save();

            Settings.Default.Reload();
        }

        private static void MenuItemHTML_Click(object sender, EventArgs e)
        {
            try
            {
                string color = Clipboard.GetText();

                Clipboard.SetText(ConvertColor.HexConverter(ColorTranslator.FromHtml(color)));
            }
            catch (Exception)
            {
                // ignore
            }

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

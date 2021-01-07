using Hotkeys;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace xEyedropper
{
    public partial class PreviewImage : Form
    {
        Thread thread;
        private GlobalHotkey hotkey;
        bool ColorValueIsHex;

        int screenLeft = SystemInformation.VirtualScreen.Left;
        int screenTop = SystemInformation.VirtualScreen.Top;
        int screenWidth = SystemInformation.VirtualScreen.Width;
        int screenHeight = SystemInformation.VirtualScreen.Height;
        int width = 0;
        int screens = 0;

        public PreviewImage()
        {
            InitializeComponent();

            this.Load += PreviewImage_Load;
            this.MouseDown += PreviewImage_MouseDown;
            this.FormClosing += GrabColor_FormClosing;
            this.TransparencyKey = Color.Turquoise;
            this.BackColor = Color.Turquoise;
        }

        private void PreviewImage_Load(object sender, EventArgs e)
        {
            ColorValueIsHex = Properties.Settings.Default.ColorHTML;
            hotkey = new GlobalHotkey(Constants.NOMOD, Keys.Escape, this);

            if (hotkey.Register())
            {
                Console.WriteLine("Hotkey registered.");
            }

            foreach (Screen screen in Screen.AllScreens)
            {
                screens++;
            }

            this.Size = new Size(screenWidth, screenHeight);
            this.Location = new Point(screenLeft, screenTop);

            Screen screenCurrent = Screen.FromPoint(Cursor.Position);
            if (screens > 1)
            {
                width = screenCurrent.Bounds.Width;
            }
            else
            {
                width = 0;
            }

            panel1.Location = new Point((Cursor.Position.X + 30) + screenWidth / screens, ((Cursor.Position.Y - Height) / 2) + screenHeight);

            thread = new Thread(SetColorLoop)
            {
                IsBackground = true
            };
            thread.Start();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
                HandleHotkey();
            base.WndProc(ref m);
        }

        private void HandleHotkey()
        {
            thread.Abort();

            this.Close();
        }

        private void GrabColor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!hotkey.Unregiser())
            {
                Console.WriteLine("Hotkey failed to unregister!");
            }
        }

        private void PreviewImage_MouseDown(object sender, MouseEventArgs e)
        {
            thread.Abort();

            if (e.Button == MouseButtons.Left)
            {
                Color color = GetColorFromPixel(Cursor.Position.X, Cursor.Position.Y);

                if (Properties.Settings.Default.ColorHTML)
                {
                    Clipboard.SetText(ConvertColor.HexConverter(color));
                }
                else if (Properties.Settings.Default.ColorRGB)
                {
                    Clipboard.SetText(ConvertColor.RGBConverter(color));
                }
            }

            this.Close();
        }

        private void ChooseColorForm_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private static Color GetColorFromPixel(int X, int Y)
        {
            Bitmap bmp = new Bitmap(1, 1);
            Rectangle bounds = new Rectangle(X, Y, 1, 1);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
            }
            return bmp.GetPixel(0, 0);
        }

        private void SetColorLoop()
        {
            while (true)
            {
                SetColor(GetColorFromPixel(Cursor.Position.X, Cursor.Position.Y));
            }
        }

        public void SetColor(Color color)
        {
            this.Invoke(new Action(() =>
            {
                NativeMethods.SetWindowPos(this.Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_SHOWWINDOW);
            }));

            if ((Cursor.Position.X + 256) > SystemInformation.VirtualScreen.Right)
            {
                this.Invoke(new Action(() =>
                {
                    panel1.Location = new Point(Cursor.Position.X - 256 + width, Cursor.Position.Y - (panel1.Height / 2));
                }));
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    panel1.Location = new Point(Cursor.Position.X + 30 + width, Cursor.Position.Y - (panel1.Height / 2));
                }));
            }

            if (ColorValueIsHex)
            {
                this.Invoke(new Action(() =>
                {
                    label1.Text = ColorTranslator.ToHtml(color);
                }));
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    label1.Text = color.R.ToString() + ", " + color.G.ToString() + ", " + color.B.ToString();
                }));
            }

            float scale = 15;

            Bitmap printscreen = new Bitmap(60, 60);

            int wid = (int)(printscreen.Width * scale);
            int hgt = (int)(printscreen.Height * scale);

            using (Graphics g = Graphics.FromImage(printscreen))
            {
                g.CopyFromScreen(Cursor.Position.X - 3, Cursor.Position.Y - 3, 0, 0, printscreen.Size, CopyPixelOperation.SourceCopy);
                g.InterpolationMode = InterpolationMode.NearestNeighbor;

                Point[] dest = { new Point(0, 0), new Point(wid, 0), new Point(0, hgt) };
                Rectangle source = new Rectangle(1, 1, printscreen.Width, printscreen.Height);
                g.DrawImage(printscreen, dest, source, GraphicsUnit.Pixel);

                Pen blackPen = new Pen(Color.Black, 1);
                Pen whitePen = new Pen(Color.White, 1);
                PointF point1 = new PointF(30.0F, 15.0F);
                PointF point2 = new PointF(30.0F, 45.0F);
                PointF point3 = new PointF(15.0F, 30);
                PointF point4 = new PointF(45.0F, 30.0F);

                if (ContrastReadableIs(Color.Black, color))
                {
                    g.DrawLine(blackPen, point1, point2);
                    g.DrawLine(blackPen, point3, point4);
                }
                else
                {
                    g.DrawLine(whitePen, point1, point2);
                    g.DrawLine(whitePen, point3, point4);
                }
            }
            this.Invoke(new Action(() =>
            {
                pictureBox1.Image = printscreen;
            }));
        }

        public static bool ContrastReadableIs(Color color1, Color color2)
        {
            float minContrast = 0.5f;

            float brightness1 = color1.GetBrightness();
            float brightness2 = color2.GetBrightness();

            return (Math.Abs(brightness1 - brightness2) >= minContrast);
        }
    }
}

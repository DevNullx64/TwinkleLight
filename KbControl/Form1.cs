// Decompiled with JetBrains decompiler
// Type: LedControl.Form1
// Assembly: LedControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2625D33F-FFFD-422A-94C4-E655C9C02803
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbControl.exe

using KbAcpi;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LedControl
{
    public partial  class Form1 : Form
    {

        private const int AW_HOR_POSITIVE = 0x00000001;
        private const int AW_HOR_NEGATIVE = 0x00000002;
        private const int AW_VER_POSITIVE = 0x00000004;
        private const int AW_VER_NEGATIVE = 0x00000008;
        private const int AW_CENTER = 0x00000010;
        private const int AW_HIDE = 0x00010000;
        private const int AW_ACTIVE = 0x00020000;
        private const int AW_SLIDE = 0x00040000;
        private const int AW_BLEND = 0x00080000;

        private Point mouseOffset;
        private bool isMouseDown;

        [DllImport("user32")]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);


        public Form1()
        {
            InitializeComponent();
        }

        public static Bitmap KiResizeImage(Bitmap bmp, int newW, int newH)
        {
            try
            {
                Bitmap bitmap = new Bitmap(newW, newH);
                Graphics graphics = Graphics.FromImage((Image)bitmap);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage((Image)bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                graphics.Dispose();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public void Mousedown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            mouseOffset = new Point(-e.X - SystemInformation.FrameBorderSize.Width + 10, -e.Y - SystemInformation.CaptionHeight - SystemInformation.FrameBorderSize.Height + 30);
            isMouseDown = true;
        }

        public void Mousemove(object sender, MouseEventArgs e)
        {
            if (!isMouseDown)
                return;
            Point mousePosition = MousePosition;
            mousePosition.Offset(mouseOffset.X, mouseOffset.Y);
            Location = mousePosition;
        }

        public void Mouseup(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            isMouseDown = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }


        public void HookKeyProcess(KeyboardHookLib.HookStruct hookStruct, out bool handle)
        {
            handle = false;
            ((Keys)hookStruct.vkCode).ToString();
            if (hookStruct.scanCode != 112 || Visible)
                return;
            Visible = true;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

    }
}
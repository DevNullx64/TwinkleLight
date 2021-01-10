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
    public class Form1 : Form
    {
        public AcpiDriver acpi = new AcpiDriver();
        private MouseHookLib _mouseHook;
        private bool MouseHookInstalled;

        private const int AW_HOR_POSITIVE = 0x00000001;
        private const int AW_HOR_NEGATIVE = 0x00000002;
        private const int AW_VER_POSITIVE = 0x00000004;
        private const int AW_VER_NEGATIVE = 0x00000008;
        private const int AW_CENTER = 0x00000010;
        private const int AW_HIDE = 0x00010000;
        private const int AW_ACTIVE = 0x00020000;
        private const int AW_SLIDE = 0x00040000;
        private const int AW_BLEND = 0x00080000;

        public int CurrentSetValue;
        public int CurrentGetValue;
        private Form1.CONFIG_DATA ConfigData;
        private Point mouseOffset;
        private bool isMouseDown;
        private IContainer components;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Panel panel1;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ImageList imageList1;
        private Timer timer1;
        private CheckBox checkBox3;
        private CheckBox checkBox1;
        private CheckBox checkBox2;

        [DllImport("user32")]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);

        public void DEBUG(string message)
        {
            Console.WriteLine(message);
            string currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Application.StartupPath);
            File.AppendAllText(Path.Combine(Application.StartupPath, "msg.log"), message + "\r\n");
            Directory.SetCurrentDirectory(currentDirectory);
        }

        public Form1() => InitializeComponent();

        public byte[] StructToBytes(object structObj, int size)
        {
            byte[] destination = new byte[size];
            IntPtr num = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structObj, num, false);
            Marshal.Copy(num, destination, 0, size);
            Marshal.FreeHGlobal(num);
            return destination;
        }

        public object ByteToStruct(byte[] bytes, System.Type type)
        {
            int num1 = Marshal.SizeOf(type);
            if (num1 > bytes.Length)
                return (object)null;
            IntPtr num2 = Marshal.AllocHGlobal(num1);
            Marshal.Copy(bytes, 0, num2, num1);
            object structure = Marshal.PtrToStructure(num2, type);
            Marshal.FreeHGlobal(num2);
            return structure;
        }

        public void ConfigInitialization()
        {
            byte[] numArray = new byte[40];
            IntPtr zero = IntPtr.Zero;
            string currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Application.StartupPath);
            DEBUG("Config Initial");
            try
            {
                FileStream fileStream = new FileStream("Config.cfg", FileMode.Open);
                fileStream.Seek(0L, SeekOrigin.Begin);
                fileStream.Read(numArray, 0, 40);
                fileStream.Close();
                for (int index = 0; index < 40; ++index)
                    DEBUG(string.Format("{0:X2}", (object)numArray[index]));
                ConfigData = (Form1.CONFIG_DATA)ByteToStruct(numArray, typeof(Form1.CONFIG_DATA));
                Color color = Color.FromArgb(ConfigData.SingleColorRGB);
                DEBUG(string.Format("Init{0:X8}", (object)ConfigData.SingleColorRGB));
                pictureBox2.BackColor = color;
                if (ConfigData.Mode == 2)
                {
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                }
                else if (ConfigData.Mode == 1)
                {
                    radioButton1.Checked = false;
                    radioButton2.Checked = true;
                }
                int autoStartUp = ConfigData.AutoStartUp;
                int startUpMinimize = ConfigData.StartUpMinimize;
                if (ConfigData.BacklightOn == 0)
                {
                    radioButton1.Enabled = false;
                    radioButton2.Enabled = false;
                    checkBox3.Checked = false;
                }
                else
                {
                    radioButton1.Enabled = true;
                    radioButton2.Enabled = true;
                    checkBox3.Checked = true;
                }
                checkBox2.Checked = ConfigData.Dustenable == 1;
            }
            catch (IOException ex)
            {
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                checkBox3.Checked = true;
                ConfigData.SingleColorRGB = 16646144;
            }
            Directory.SetCurrentDirectory(currentDirectory);
        }

        public void SaveConfig()
        {
            byte[] numArray = new byte[40];
            IntPtr zero = IntPtr.Zero;
            string currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Application.StartupPath);
            FileStream fileStream = new FileStream("Config.cfg", FileMode.Create);
            DEBUG("Save Config");
            ConfigData.StartUpMinimize = 1;
            byte[] bytes = StructToBytes((object)ConfigData, 40);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Flush();
            fileStream.Close();
            Directory.SetCurrentDirectory(currentDirectory);
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
                return (Bitmap)null;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigInitialization();
            ShowInTaskbar = false;
            TopMost = true;
            Visible = true;
            Image image = imageList1.Images[0];
            Size size = pictureBox1.Size;
            int width = size.Width;
            size = pictureBox1.Size;
            int height = size.Height;
            pictureBox1.Image = (Image)new Bitmap(image, width, height);
            DEBUG("Load form");
            InstallHookMouse();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void setRGB()
        {
            try
            {
                ManagementObject managementObject = new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_0'", (ObjectGetOptions)null);
                ManagementBaseObject methodParameters = managementObject.GetMethodParameters("Set");
                methodParameters["Data"] = (object)CalValue();
                managementObject.InvokeMethod("Set", methodParameters, (InvokeMethodOptions)null);
                CurrentSetValue = int.Parse(methodParameters["Data"].ToString());
                DEBUG("Set RGB: " + string.Format("Init{0:X8}", (object)CurrentSetValue));
            }
            catch (ManagementException ex)
            {
            }
        }

        private void SwitchTp(int parameter)
        {
            try
            {
                ManagementObject managementObject = new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_1'", null);
                ManagementBaseObject methodParameters = managementObject.GetMethodParameters("Set");
                methodParameters["Data"] = parameter;
                managementObject.InvokeMethod("Set", methodParameters, (InvokeMethodOptions)null);
                CurrentSetValue = int.Parse(methodParameters["Data"].ToString());
                DEBUG($"Set RGB: Init{CurrentSetValue:X8}");
            }
            catch (ManagementException ex)
            {
            }
        }

        private void Dust(int parameter)
        {
            try
            {
                ManagementObject managementObject = new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_2'", null);
                ManagementBaseObject methodParameters = managementObject.GetMethodParameters("Set");
                methodParameters["Data"] = parameter;
                managementObject.InvokeMethod("Set", methodParameters, (InvokeMethodOptions)null);
                CurrentSetValue = int.Parse(methodParameters["Data"].ToString());
                DEBUG($"Set Dust: Init{CurrentSetValue:X8}");
            }
            catch (ManagementException ex)
            {
            }
        }

        private void getRGB()
        {
            try
            {
                CurrentGetValue = int.Parse(new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_0'", null).InvokeMethod("Get", null, null)["Data"].ToString());
                DEBUG($"Get RGB: Init{CurrentGetValue:X8}");
            }
            catch (ManagementException ex)
            {
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Bitmap image = (Bitmap)pictureBox1.Image;
            DEBUG(nameof(pictureBox1_MouseClick));
            try
            {
                pictureBox2.BackColor = image.GetPixel(e.X, e.Y);
                ref Form1.CONFIG_DATA local = ref ConfigData;
                int num1 = pictureBox2.BackColor.A << 24;
                Color backColor = pictureBox2.BackColor;
                int num2 = backColor.R << 16;
                int num3 = num1 | num2;
                backColor = pictureBox2.BackColor;
                int num4 = backColor.G << 8;
                int num5 = num3 | num4;
                backColor = pictureBox2.BackColor;
                int b = backColor.B;
                int num6 = num5 | b;
                local.SingleColorRGB = num6;
                SaveConfig();
                if (!radioButton1.Checked)
                    return;
                setRGB();
            }
            catch
            {
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
            UninstallHookMouse();
            acpi.AcpiUnLoadDll();
            Dispose();
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            DEBUG(nameof(checkBox1_CheckedChanged));
            SaveConfig();
        }

        public int CalValue() => 
            !checkBox3.Checked ? 
            0x2000000 : 
            ConfigData.Mode << 24 | 
            ((ConfigData.SingleColorRGB & 0x00FF0000) >> 16) * 39 / byte.MaxValue << 16 | 
            ((ConfigData.SingleColorRGB & 0x0000FF00) >> 8) * 39 / byte.MaxValue << 8 | 
            (ConfigData.SingleColorRGB & 0x000000FF) * 39 / byte.MaxValue;

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            DEBUG(nameof(radioButton1_CheckedChanged));
            ConfigData.Mode = !radioButton1.Checked ? 1 : 2;
            SaveConfig();
            setRGB();
        }

        private void InstallHookMouse()
        {
            if (_mouseHook != null || MouseHookInstalled)
                return;
            DEBUG("Install Hook Mouse");
            MouseHookInstalled = true;
            _mouseHook = new MouseHookLib();
            _mouseHook.InstallHook(new MouseHookLib.ProcessKeyHandle(HookMouseProcess));
        }

        public void UninstallHookMouse()
        {
            DEBUG("Uninstall Hook Mouse");
            MouseHookInstalled = false;
            if (_mouseHook == null)
                return;
            _mouseHook.UninstallHook();
        }

        public void HookKeyProcess(KeyboardHookLib.HookStruct hookStruct, out bool handle)
        {
            handle = false;
            ((Keys)hookStruct.vkCode).ToString();
            if (hookStruct.scanCode != 112 || Visible)
                return;
            Visible = true;
        }

        public void HookMouseProcess(MouseHookLib.HookStruct hookStruct, out bool handle)
        {
            handle = false;
            MouseHookLib.PointStruct point = hookStruct.point;
            if (point.x >= Left && point.x <= Left + Width && (point.y >= Top && point.y <= Top + Height) || point.y >= Top + Height)
                return;
            Visible = false;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            acpi.AcpiLoadDll();
            acpi.GetAcpiValueProc(64);
            acpi.AcpiUnLoadDll();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            DEBUG(nameof(radioButton2_CheckedChanged));
            SaveConfig();
            setRGB();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e) => DEBUG("Mouse Down");

        private void pictureBox2_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            getRGB();
            if (CurrentGetValue != CurrentSetValue)
                setRGB();
            else
                timer1.Enabled = false;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox3.Checked)
            {
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                ConfigData.BacklightOn = 0;
            }
            else
            {
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                ConfigData.BacklightOn = 1;
            }
            SaveConfig();
            setRGB();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (!(CultureInfo.InstalledUICulture.NativeName.Substring(0, 2) == "中文"))
                return;
            checkBox3.Text = "跑马灯";
            radioButton1.Text = "单色模式";
            radioButton2.Text = "多彩模式";
            checkBox1.Text = "关闭触摸板";
            checkBox2.Text = "除尘/高性能";
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            int height1 = Screen.PrimaryScreen.Bounds.Height;
            int height2 = Screen.PrimaryScreen.WorkingArea.Height;
            int width = Screen.PrimaryScreen.Bounds.Width;
            int num = height1 - height2;
            Top = height1 - Size.Height - num;
            Left = width - Size.Width - 26;
            ShowInTaskbar = false;
            if (CultureInfo.InstalledUICulture.NativeName.Substring(0, 2) == "中文")
            {
                checkBox3.Text = "跑马灯";
                radioButton1.Text = "单色模式";
                radioButton2.Text = "多彩模式";
                checkBox1.Text = "关闭触摸板";
            }
            DEBUG("Visible: " + Visible.ToString());
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SwitchTp(0);
                Visible = false;
            }
            else
                SwitchTp(0x1000000);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                ConfigData.Dustenable = 1;
                SaveConfig();
                Dust(0x1000000);
            }
            else
            {
                ConfigData.Dustenable = 0;
                SaveConfig();
                Dust(0);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Form1));
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            panel1 = new Panel();
            checkBox1 = new CheckBox();
            checkBox3 = new CheckBox();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            contextMenuStrip1 = new ContextMenuStrip(components);
            openToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            imageList1 = new ImageList(components);
            timer1 = new Timer(components);
            checkBox2 = new CheckBox();
            ((ISupportInitialize)pictureBox1).BeginInit();
            ((ISupportInitialize)pictureBox2).BeginInit();
            panel1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(23, 15);
            pictureBox1.Margin = new Padding(2, 3, 2, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(131, 139);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += new EventHandler(pictureBox1_Click);
            pictureBox1.MouseClick += new MouseEventHandler(pictureBox1_MouseClick);
            pictureBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            pictureBox2.BackColor = Color.DimGray;
            pictureBox2.Location = new Point(96, 37);
            pictureBox2.Margin = new Padding(2, 3, 2, 3);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(57, 16);
            pictureBox2.TabIndex = 3;
            pictureBox2.TabStop = false;
            pictureBox2.Click += new EventHandler(pictureBox2_Click);
            panel1.BackColor = Color.Transparent;
            panel1.Controls.Add(checkBox2);
            panel1.Controls.Add(checkBox1);
            panel1.Controls.Add(checkBox3);
            panel1.Controls.Add(radioButton2);
            panel1.Controls.Add(radioButton1);
            panel1.Controls.Add(pictureBox2);
            panel1.Location = new Point(179, 17);
            panel1.Margin = new Padding(2, 3, 2, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(158, 142);
            panel1.TabIndex = 4;
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("Calibri", 10.8f, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkBox1.ForeColor = Color.White;
            checkBox1.Location = new Point(3, 82);
            checkBox1.Margin = new Padding(2, 3, 2, 3);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(90, 22);
            checkBox1.TabIndex = 13;
            checkBox1.Text = "Disable Tp";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += new EventHandler(checkBox1_CheckedChanged_1);
            checkBox3.AutoSize = true;
            checkBox3.Font = new Font("Calibri", 10.8f, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkBox3.ForeColor = Color.White;
            checkBox3.Location = new Point(3, 13);
            checkBox3.Margin = new Padding(2, 3, 2, 3);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(109, 22);
            checkBox3.TabIndex = 12;
            checkBox3.Text = "Twinkle Light";
            checkBox3.UseVisualStyleBackColor = true;
            checkBox3.CheckedChanged += new EventHandler(checkBox3_CheckedChanged);
            radioButton2.AutoSize = true;
            radioButton2.Font = new Font("Calibri", 10.8f, FontStyle.Bold, GraphicsUnit.Point, 0);
            radioButton2.ForeColor = Color.White;
            radioButton2.Location = new Point(7, 54);
            radioButton2.Margin = new Padding(2, 3, 2, 3);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(79, 22);
            radioButton2.TabIndex = 1;
            radioButton2.TabStop = true;
            radioButton2.Text = "Dynamic";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += new EventHandler(radioButton2_CheckedChanged);
            radioButton1.AutoSize = true;
            radioButton1.Font = new Font("Calibri", 10.8f, FontStyle.Bold, GraphicsUnit.Point, 0);
            radioButton1.ForeColor = Color.White;
            radioButton1.Location = new Point(7, 33);
            radioButton1.Margin = new Padding(2, 3, 2, 3);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(92, 22);
            radioButton1.TabIndex = 0;
            radioButton1.TabStop = true;
            radioButton1.Text = "Singleness";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += new EventHandler(radioButton1_CheckedChanged);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[2]
            {
                 openToolStripMenuItem,
                 exitToolStripMenuItem
            });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(104, 48);
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(103, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += new EventHandler(openToolStripMenuItem_Click);
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(103, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += new EventHandler(exitToolStripMenuItem_Click);
            imageList1.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "未标题-1.png");
            timer1.Interval = 10000;
            timer1.Tick += new EventHandler(timer1_Tick);
            checkBox2.AutoSize = true;
            checkBox2.Font = new Font("Calibri", 10.8f, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkBox2.ForeColor = Color.White;
            checkBox2.Location = new Point(3, 110);
            checkBox2.Margin = new Padding(2, 3, 2, 3);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(133, 22);
            checkBox2.TabIndex = 14;
            checkBox2.Text = "Dust/Hight mode";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += new EventHandler(checkBox2_CheckedChanged);
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(375, 172);
            Controls.Add(panel1);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(2, 3, 2, 3);
            Name = nameof(Form1);
            Opacity = 0.7;
            Text = nameof(Form1);
            TransparencyKey = SystemColors.Control;
            Load += new EventHandler(Form1_Load);
            Shown += new EventHandler(Form1_Shown);
            VisibleChanged += new EventHandler(Form1_VisibleChanged);
            ((ISupportInitialize)pictureBox1).EndInit();
            ((ISupportInitialize)pictureBox2).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        public struct CONFIG_DATA
        {
            public int Mode;
            public int SingleColorRGB;
            public int R;
            public int G;
            public int B;
            public int AutoStartUp;
            public int StartUpMinimize;
            public int BacklightOn;
            public int Dustenable;
            public int Reserve7;
        }
    }
}
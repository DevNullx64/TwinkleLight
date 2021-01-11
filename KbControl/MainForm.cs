using KbAcpi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LedControl
{
    public partial class MainForm : Form
    {
        #region old interesting dead code
        private const int AW_HOR_POSITIVE = 0x00000001;
        private const int AW_HOR_NEGATIVE = 0x00000002;
        private const int AW_VER_POSITIVE = 0x00000004;
        private const int AW_VER_NEGATIVE = 0x00000008;
        private const int AW_CENTER = 0x00000010;
        private const int AW_HIDE = 0x00010000;
        private const int AW_ACTIVE = 0x00020000;
        private const int AW_SLIDE = 0x00040000;
        private const int AW_BLEND = 0x00080000;

        [DllImport("user32")]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);

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
        #endregion

        private CONFIG_DATA ConfigData;

        public AcpiDriver acpi = new AcpiDriver();
        private MouseHookLib _mouseHook;
        private bool MouseHookInstalled;

        public uint CurrentSetValue;
        public uint CurrentGetValue;

        private void button1_Click_1(object sender, EventArgs e)
        {
            acpi.AcpiLoadDll();
            acpi.GetAcpiValueProc(64);
            acpi.AcpiUnLoadDll();
        }

        private static ManagementObject KBC0_0 = new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_0'", null);
        private static readonly ManagementBaseObject KBC0_0_Parameters = KBC0_0.GetMethodParameters("Set");

        private uint RGB
        {
            get => (uint)KBC0_0.InvokeMethod("Get", null, null)["Data"];
            set
            {
                KBC0_0_Parameters["Data"] = value;
                KBC0_0.InvokeMethod("Set", KBC0_0_Parameters, null);
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        public void ConfigInitialization()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Application.StartupPath);
            try
            {
                ConfigData = CONFIG_DATA.Load();
                Color color = Color.FromArgb(ConfigData.SingleColorRGB);
                pbLightColor.BackColor = color;
                if (ConfigData.Mode == 2)
                {
                    rbLightSingle.Checked = true;
                    rbLightDynamic.Checked = false;
                }
                else if (ConfigData.Mode == 1)
                {
                    rbLightSingle.Checked = false;
                    rbLightDynamic.Checked = true;
                }
                int autoStartUp = ConfigData.AutoStartUp;
                int startUpMinimize = ConfigData.StartUpMinimize;
                if (ConfigData.BacklightOn == 0)
                {
                    rbLightSingle.Enabled = false;
                    rbLightDynamic.Enabled = false;
                    chkEnableLight.Checked = false;
                }
                else
                {
                    rbLightSingle.Enabled = true;
                    rbLightDynamic.Enabled = true;
                    chkEnableLight.Checked = true;
                }
                chkDustMode.Checked = ConfigData.Dustenable == 1;
            }
            catch (IOException ex)
            {
                rbLightSingle.Enabled = true;
                rbLightDynamic.Enabled = true;
                chkEnableLight.Checked = true;
                ConfigData.SingleColorRGB = 0xFE0000;
            }
            Directory.SetCurrentDirectory(currentDirectory);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ConfigInitialization();
            ShowInTaskbar = false;
            TopMost = true;
            Visible = true;
            //Image image = imageList1.Images[0];
            //pictureBox1.Image = new Bitmap(image, width, height);
            InstallHookMouse();
        }

        private struct CONFIG_DATA
        {
            public int Mode;
            public int SingleColorRGB;
            public int AutoStartUp;
            public int StartUpMinimize;
            public int BacklightOn;
            public int Dustenable;
            public int Reserve7;

            public static byte[] StructToBytes(object structObj)
            {
                int size = Marshal.SizeOf(structObj);
                byte[] destination = new byte[size];
                IntPtr num = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(structObj, num, false);
                Marshal.Copy(num, destination, 0, size);
                Marshal.FreeHGlobal(num);
                return destination;
            }

            public void Save()
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(Application.StartupPath);
                FileStream fileStream = new FileStream("Config.cfg", FileMode.Create);
                StartUpMinimize = 1;
                byte[] bytes = StructToBytes(this);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Flush();
                fileStream.Close();
                Directory.SetCurrentDirectory(currentDirectory);
            }

            public static T ByteToStruct<T>(byte[] bytes)
            {
                int size = Marshal.SizeOf(typeof(T));
                if (size <= bytes.Length)
                {
                    IntPtr ptr = Marshal.AllocHGlobal(size);
                    Marshal.Copy(bytes, 0, ptr, size);
                    object structure = Marshal.PtrToStructure(ptr, typeof(T));
                    Marshal.FreeHGlobal(ptr);
                    return (T)structure;
                }

                throw new InvalidCastException();
            }

            public static CONFIG_DATA Load()
            {
                byte[] numArray = new byte[Marshal.SizeOf(typeof(CONFIG_DATA))];
                FileStream fileStream = new FileStream("Config.cfg", FileMode.Open);
                fileStream.Seek(0L, SeekOrigin.Begin);
                fileStream.Read(numArray, 0, numArray.Length);
                fileStream.Close();
                return ByteToStruct<CONFIG_DATA>(numArray);
            }
        }

        public uint CalValue() =>
            !chkEnableLight.Checked ?
            0x02000000 :
            (uint)ConfigData.Mode << 24 |
            (((uint)ConfigData.SingleColorRGB >> 16) & byte.MaxValue) * 39 / byte.MaxValue << 16 |
            (((uint)ConfigData.SingleColorRGB >> 8) & byte.MaxValue) * 39 / byte.MaxValue << 8 |
            ((uint)ConfigData.SingleColorRGB & byte.MaxValue) * 39 / byte.MaxValue;

        public static ManagementObject KBC0_1 = new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_1'", null);
        public static ManagementBaseObject KBC0_1_Parameters = KBC0_1.GetMethodParameters("Set");
        private void SwitchTp(bool parameter)
        {
            try
            {
                KBC0_1_Parameters["Data"] = parameter ? 0x1000000 : 0;
                KBC0_1.InvokeMethod("Set", KBC0_1_Parameters, null);
                CurrentSetValue = (uint)KBC0_1_Parameters["Data"];
            }
            catch (ManagementException ex)
            {
            }
        }

        public static ManagementObject KBC0_2 = new ManagementObject("root\\WMI", "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_2'", null);
        public static ManagementBaseObject KBC0_2_Parameters = KBC0_2.GetMethodParameters("Set");
        private void Dust(bool parameter)
        {
            KBC0_2_Parameters["Data"] = parameter ? 0x1000000 : 0;
            KBC0_2.InvokeMethod("Set", KBC0_2_Parameters, null);
            CurrentSetValue = (uint)KBC0_2_Parameters["Data"];
        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            rbLightSingle.Enabled = chkEnableLight.Checked;
            rbLightDynamic.Enabled = chkEnableLight.Checked;
            ConfigData.BacklightOn = (chkEnableLight.Checked) ? 1 : 0;
            ConfigData.Save();
            RGB = CalValue();
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            ConfigData.Mode = !rbLightSingle.Checked ? 1 : 2;
            ConfigData.Save();
            RGB = CalValue();
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            ConfigData.Save();
            RGB = CalValue();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            SwitchTp(!chkDisableTP.Checked);
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            ConfigData.Dustenable = chkDustMode.Checked ? 1 : 0;
            Dust(chkDustMode.Checked);
            ConfigData.Save();
        }

        private static readonly PropertyInfo ImageRectangleProperty = typeof(PictureBox).GetProperty("ImageRectangle", BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance);
        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Bitmap original = (Bitmap)pbSelectColor.Image;

            Color? color = null;
            if (pbSelectColor.SizeMode == PictureBoxSizeMode.Normal || pbSelectColor.SizeMode == PictureBoxSizeMode.AutoSize)
            {
                color = original.GetPixel(e.X, e.Y);
            }
            else
            {
                Rectangle rectangle = (Rectangle)ImageRectangleProperty.GetValue(pbSelectColor, null);
                if (rectangle.Contains(e.Location))
                {
                    using (Bitmap copy = new Bitmap(pbSelectColor.ClientSize.Width, pbSelectColor.ClientSize.Height))
                    using (Graphics g = Graphics.FromImage(copy))
                    {
                        g.DrawImage(pbSelectColor.Image, rectangle);
                        color = copy.GetPixel(e.X, e.Y);
                    }
                }
            }

            if (color.HasValue)
            {
                pbLightColor.BackColor = color.Value;
                ConfigData.SingleColorRGB = (color.Value.R << 16) | (color.Value.G << 8) | color.Value.B;
                ConfigData.Save();
                if (rbLightSingle.Checked)
                    RGB = CalValue();
            }
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            if (CultureInfo.InstalledUICulture.NativeName.Substring(0, 2) == "中文")
            {
                chkEnableLight.Text = "跑马灯";
                rbLightSingle.Text = "单色模式";
                rbLightDynamic.Text = "多彩模式";
                chkDisableTP.Text = "关闭触摸板";
                chkDustMode.Text = "除尘/高性能";
            }
        }

        private void Main_VisibleChanged(object sender, EventArgs e)
        {
            int num = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
            Top = Screen.PrimaryScreen.Bounds.Height - Size.Height - num;
            Left = Screen.PrimaryScreen.Bounds.Width - Size.Width - 26;
            Main_Shown(sender, e);
        }

        #region Mouse hook
        public void HookMouseProcess(MouseHookLib.HookStruct hookStruct, out bool handle)
        {
            handle = false;
            MouseHookLib.PointStruct point = hookStruct.point;
            if ((point.x < Left || point.x > Left + Width || point.y < Top || point.y > Top + Height) && point.y < Top + Height)
                Visible = false;
        }

        private void InstallHookMouse()
        {
            if (_mouseHook == null && !MouseHookInstalled)
            {
                MouseHookInstalled = true;
                _mouseHook = new MouseHookLib();
                _mouseHook.InstallHook(new MouseHookLib.ProcessKeyHandle(HookMouseProcess));
            }
        }
        public void UninstallHookMouse()
        {
            MouseHookInstalled = false;
            if (_mouseHook != null)
                _mouseHook.UninstallHook();
        }
        #endregion

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UninstallHookMouse();
            acpi.AcpiUnLoadDll();
            Dispose();
            Close();
        }
    }
}

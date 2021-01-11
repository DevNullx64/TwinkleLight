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
        private const uint TRUE = 0x01000000U;
        private const uint FALSE = 0x00000000U;

        private CONFIG_DATA ConfigData;

        public AcpiDriver acpi = new AcpiDriver();
        private MouseHookLib _mouseHook;

        private void button1_Click_1(object sender, EventArgs e)
        {
            acpi.AcpiLoadDll();
            acpi.GetAcpiValueProc(64);
            acpi.AcpiUnLoadDll();
        }

        private const string WMIRoot = "root\\WMI";

        private byte OutNormalize(byte value) => (byte)(value * 39 / byte.MaxValue);
        private byte InNormalize(byte value) => (byte)(value * byte.MaxValue / 39);

        private static readonly ManagementObject KBC0_0_ = new ManagementObject(WMIRoot, "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_0'", null);
        private static readonly ManagementBaseObject KBC0_0_Parameters = KBC0_0_.GetMethodParameters("Set");
        private uint KBC0_0
        {
            get => (uint)KBC0_0_.InvokeMethod("Get", null, null)["Data"];
            set
            {
                try { 
                KBC0_0_Parameters["Data"] = value;
                KBC0_0_.InvokeMethod("Set", KBC0_0_Parameters, null);
                }
                catch (ManagementException ex)
                {
                    MessageBox.Show(ex.Message, "LedColor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Exit();
                }

            }
        }

        public enum LedModeState { Off, Static, Dynamic};
        public LedModeState Mode
        {
            get
            {
                uint value = KBC0_0;
                if ((value & 0xFF000000U) == TRUE)
                    return LedModeState.Dynamic;
                else if ((value & 0x00FFFFFF) == 0)
                    return LedModeState.Off;
                else
                    return LedModeState.Static;
            }
            set
            {
                if (value != Mode)
                {
                    if (value == LedModeState.Off)
                        KBC0_0 = 0x02000000;
                    else if (value == LedModeState.Dynamic)
                        KBC0_0 = TRUE;
                    else
                    {
                        Color v = Color.FromArgb(ConfigData.SingleColorRGB);
                        KBC0_0 = 0x02000000U | (uint)(OutNormalize(v.R) << 16 | OutNormalize(v.G) << 8 | OutNormalize(v.B));
                    }
                }
            }
        }

        public Color LedColor
        {
            get
            {
                uint value = KBC0_0;
                return Color.FromArgb(InNormalize((byte)(value >> 16)), InNormalize((byte)(value >> 8)), InNormalize((byte)value));
            }
            set
            {
                pbLightColor.BackColor = value;
                ConfigData.SingleColorRGB = pbLightColor.BackColor.ToArgb();
                if (Mode == LedModeState.Static)
                    KBC0_0 = 0x02000000U | (uint)(OutNormalize(value.R) << 16 | OutNormalize(value.G) << 8 | OutNormalize(value.B));
                ConfigData.Save();
            }
        }


        public static ManagementObject KBC0_1 = new ManagementObject(WMIRoot, "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_1'", null);
        public static ManagementBaseObject KBC0_1_Parameters = KBC0_1.GetMethodParameters("Set");
        private void SwitchTp(bool parameter)
        {
            try
            {
                KBC0_1_Parameters["Data"] = parameter ? TRUE : FALSE;
                KBC0_1.InvokeMethod("Set", KBC0_1_Parameters, null);
            }
            catch (ManagementException ex)
            {
                MessageBox.Show(ex.Message, "SwitchTP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Exit();
            }
        }

        public static ManagementObject KBC0_2 = new ManagementObject(WMIRoot, "Acpi_WMI_KBC.InstanceName='ACPI\\PNP0C14\\KBC0_2'", null);
        public static ManagementBaseObject KBC0_2_Parameters = KBC0_2.GetMethodParameters("Set");
        private void Dust(bool parameter)
        {
            try
            {
                KBC0_2_Parameters["Data"] = parameter ? TRUE : FALSE;
                KBC0_2.InvokeMethod("Set", KBC0_2_Parameters, null);
            }
            catch (ManagementException ex)
            {
                MessageBox.Show(ex.Message, "Dust", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Exit();
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
                Color color = Color.FromArgb((int)ConfigData.SingleColorRGB);
                LedColor = color;
                if (ConfigData.Mode == 1)
                {
                    rbLightStatic.Checked = false;
                    rbLightDynamic.Checked = true;
                }
                bool backlightOn = ConfigData.BacklightOn != 0;
                rbLightStatic.Enabled = backlightOn;
                rbLightDynamic.Enabled = backlightOn;
                chkEnableLight.Checked = backlightOn;
                chkDustMode.Checked = ConfigData.Dustenable == 1;
            }
            catch (IOException)
            {
                ConfigData.SingleColorRGB = 0xFF0000;
            }
            Directory.SetCurrentDirectory(currentDirectory);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ConfigInitialization();
            InstallHookMouse();
        }

        private struct CONFIG_DATA
        {
            public int Mode;
            public int SingleColorRGB;
            public int StartUpMinimize;
            public int BacklightOn;
            public int Dustenable;

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

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnableLight.Checked)
            {
                if (rbLightStatic.Checked)
                    Mode = LedModeState.Static;
                else
                    Mode = LedModeState.Dynamic;
            }
            else
                Mode = LedModeState.Off;
            rbLightStatic.Enabled = chkEnableLight.Checked;
            rbLightDynamic.Enabled = chkEnableLight.Checked;
            ConfigData.BacklightOn = chkEnableLight.Checked ? 1 : 0;
            ConfigData.Save();
        }

        private void rbLightStatic_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLightStatic.Checked && Mode != LedModeState.Static)
            {
                Mode = LedModeState.Static;
                LedColor = pbLightColor.BackColor;
                ConfigData.Mode = 2;
                ConfigData.Save();
            }
        }

        private void rbLightDynamic_CheckedChanged(object sender, EventArgs e)
        {
            Mode = LedModeState.Dynamic;
            ConfigData.Mode = 1;
            ConfigData.Save();
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

            if (pbSelectColor.SizeMode == PictureBoxSizeMode.Normal || pbSelectColor.SizeMode == PictureBoxSizeMode.AutoSize)
                LedColor = original.GetPixel(e.X, e.Y);
            else
            {
                Rectangle rectangle = (Rectangle)ImageRectangleProperty.GetValue(pbSelectColor, null);
                if (rectangle.Contains(e.Location))
                {
                    using (Bitmap copy = new Bitmap(pbSelectColor.ClientSize.Width, pbSelectColor.ClientSize.Height))
                    using (Graphics g = Graphics.FromImage(copy))
                    {
                        g.DrawImage(pbSelectColor.Image, rectangle);
                        LedColor = copy.GetPixel(e.X, e.Y);
                    }
                }
            }
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            if (CultureInfo.InstalledUICulture.NativeName.Substring(0, 2) == "中文")
            {
                chkEnableLight.Text = "跑马灯";
                rbLightStatic.Text = "单色模式";
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
        public bool HookMouseProcess(MouseHookLib.HookStruct hookStruct)
        {
            MouseHookLib.PointStruct point = hookStruct.point;
            if ((point.x < Left || point.x > Left + Width || point.y < Top || point.y > Top + Height) && point.y < Top + Height)
                Visible = false;
            return false;
        }

        private void InstallHookMouse()
        {
            if (_mouseHook == null)
            {
                _mouseHook = new MouseHookLib();
                _mouseHook.InstallHook(new MouseHookLib.ProcessMouseHandle(HookMouseProcess));
            }
        }
        public void UninstallHookMouse()
        {
            if (_mouseHook != null)
            {
                _mouseHook.UninstallHook();
                _mouseHook = null;
            }
        }
        #endregion

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Exit();

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visible = true;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            Visible = true;
        }

        private void Exit()
        {
            UninstallHookMouse();
            acpi.AcpiUnLoadDll();
            Close();
            Dispose();
        }
    }
}
